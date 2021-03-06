﻿
Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Public Enum MDc_Dir
    CCW = 0
    CW = 1
End Enum

Public Class dcMotorPoint
    'the command block for DC motor
    Property DrivingTime As TimeSpan = New TimeSpan(0, 0, 1)
    Property Direction As MDc_Dir = MDc_Dir.CCW
End Class


Public MustInherit Class dcMotorChannelBase
    MustOverride Function runMethod(ByRef state As Integer, direction As MDc_Dir) As Boolean
    MustOverride Function stopMethod(ByRef state As Integer) As Boolean
    MustOverride Sub pauseMethod()
    MustOverride Sub resumeMethod()

    Public inAlarm As ULong   ' driver->host

End Class

''' <summary>
''' Model : DC
''' </summary>
''' <remarks></remarks>
Public Class dcMotorChannelType1
    Inherits dcMotorChannelBase


    Public outStartStop As ULong         ' host->driver
    Public outRunBrake As ULong          ' host->driver
    Public outDir As ULong               ' host->driver

    Dim timerDelay As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)} '150ms

    Public Overrides Function runMethod(ByRef state As Integer, direction As MDc_Dir) As Boolean
        Select Case state
            Case 0 '(啟動/剎車) 訊號首先致能,然後(Start/Stop)致能
                writeBit(outRunBrake, True) '1. Run/Brake 首先致能
                writeBit(outDir, direction = MDc_Dir.CW)
                timerDelay.IsEnabled = True
                state = 10

            Case 10 '延遲一段開始RUN所必須的時間
                If timerDelay.IsTimerTicked Then
                    writeBit(outStartStop, True) '2. Start/Stop 第二致能

                    Return True
                Else
                    '------------------------
                    '   Wait delay time comes
                    '------------------------
                End If
        End Select

        Return False
    End Function

    Public Overrides Function stopMethod(ByRef state As Integer) As Boolean
        writeBit(outRunBrake, False) 'DC馬達剎車
        writeBit(outStartStop, False) '取消啟動訊號
        writeBit(outDir, False) '取消方向訊號
        Return True
    End Function

    Public Overrides Sub pauseMethod()
        writeBit(outStartStop, False) '取消啟動訊號
    End Sub

    Public Overrides Sub resumeMethod()
        writeBit(outStartStop, True) '取消啟動訊號
    End Sub
End Class
''' <summary>
''' Model : BMU460SC-30-3
''' </summary>
''' <remarks></remarks>
Public Class dcMotorChannelType2
    Inherits dcMotorChannelBase


    Public outForward As ULong
    Public outBackward As ULong
    Public outMode As ULong

    Dim __lastOutForwardStatus As Boolean = False
    Dim __lastOutBackwardStatus As Boolean = False

    Public Overrides Function runMethod(ByRef state As Integer, direction As MDc_Dir) As Boolean
        writeBit(outForward, direction = MDc_Dir.CW)
        writeBit(outBackward, direction = MDc_Dir.CCW)
        Return True
    End Function

    Public Overrides Function stopMethod(ByRef state As Integer) As Boolean
        writeBit(outForward, False)
        writeBit(outBackward, False)
        Return True
    End Function

    Public Overrides Sub pauseMethod()
        'memorize last status
        __lastOutForwardStatus = readBit(outForward)
        __lastOutBackwardStatus = readBit(outBackward)
        stopMethod(Nothing) 'same as stop method
    End Sub

    Public Overrides Sub resumeMethod()
        writeBit(outForward, __lastOutForwardStatus)
        writeBit(outBackward, __lastOutBackwardStatus)
    End Sub
End Class

''' <summary>
''' IO motor control acturally
''' </summary>
''' <remarks></remarks>
Public Class dcMotorControl
    Inherits driveBase
    Implements IDrivable


    '----------------------------------
    '   the command is Coincide with compMotor
    '----------------------------------
    Public Enum dcMotorCommands As Integer
        NONE = 0                  ' no command in execute
        'WAIT_RECALL = 1             '
        ''----------------------------
        'GO_HOME = 2
        GO_POSITION = 3         ''given stroke time , would move until point reached
        STOP_FORCELY = 4         ' stop motion immeditatly
        STOP_SLOW_DOWN = 5       ' stop motion in slope
        '---------------------------
        JOG = 6
        'GO_POSITION_OPEN_LOOP = 7
        ''---------------------------
        'WAIT_MOTION_STOP = 8        'Hsien , 2015.01.23 ,   wait hardware stopped signal
        MOTION_PAUSE = 9
        MOTION_RESUME = 10
        ''---------------------------
        'V_CHANGE            ' Hsien , 2014.09.26
        ''---------------------------
        'GO_POSITION_COMBINED    ' Hsien , 2014.09.27
        ''----------------------------
        'START_MOVE  'master mode
        'START_MOVE_SLAVE    'slave mode
        ''----------------------------
        'SYNCHRON_MASTER 'master mode
    End Enum

    Protected Enum driveStateEnum As Integer
        READY
        RUNNING       ' execute the command
        COUNTING        'time counting
        STOPPING        'when alarm occured
        ALARM_HANDLING  ' the state wait user response
        GIVEUP          ' user give up to check sensor
    End Enum

    Protected __commandDriveState As IDrivable.drivenState = IDrivable.drivenState.LISTENING
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTION_END
    Protected __commandBeforePause As dcMotorCommands = dcMotorCommands.NONE
    Protected __commandInExecute As dcMotorCommands = dcMotorCommands.NONE
    Protected driveState As driveStateEnum = driveStateEnum.READY
    Protected methodState As Integer = 0 'the fiber state machine
    Protected __direction As MDc_Dir = MDc_Dir.CW
    Protected __motionPeriod As TimeSpan = New TimeSpan(0, 0, 1)  'indicating how long this move run
    Protected __lastRunningStatus As Boolean = False 'used to memorize last state before pause , Hsien , 2016.03.11 , true:running , false:stopped
    Protected __drivenChannel As dcMotorChannelBase = Nothing   'ready to be assign by constructor

    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            Return __commandDriveState
        End Get
    End Property

    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get
            If (__commandDriveState = IDrivable.drivenState.WAIT_RECALL) Then
                __commandInExecute = dcMotorCommands.NONE
                __commandDriveState = IDrivable.drivenState.LISTENING
            End If
            Return __commandEndStatus
        End Get
    End Property

    Public ReadOnly Property CommandInExecute As Object Implements IDrivable.CommandInExecute
        Get
            Return __commandInExecute
        End Get
    End Property
    Property Direction As MDc_Dir
        Get
            Return __direction
        End Get
        Set(value As MDc_Dir)
            __direction = value
        End Set
    End Property


    Public Function drive(command As [Enum], Optional arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive


        'Hsien , should cast into right type
        Dim __commandInDcMotorCommand As dcMotorCommands = [Enum].ToObject(GetType(dcMotorCommands), command)
        If (__commandInDcMotorCommand.Equals(Nothing)) Then
            'error , cannot casting , command reject
            Return IDrivable.endStatus.EXECUTION_END_FAIL
        End If

        '-------------------------
        '   Command accepted
        '-------------------------
        Select Case __commandInDcMotorCommand
            Case dcMotorCommands.MOTION_PAUSE
                If (Not __commandInExecute.Equals(dcMotorCommands.MOTION_PAUSE)) Then
                    __drivenChannel.pauseMethod()

                    timerDuring.IsEnabled = False    'temp stop timer
                    __commandBeforePause = __commandInExecute
                    __commandInExecute = dcMotorCommands.MOTION_PAUSE
                End If
                'reject redundant resuming
                Return IDrivable.endStatus.EXECUTING
            Case dcMotorCommands.MOTION_RESUME
                If __commandInExecute.Equals(dcMotorCommands.MOTION_PAUSE) Then

                    'writeBit(SoStartStop, __lastStartStopStatus)    'resume motor
                    If (__lastRunningStatus) Then
                        __drivenChannel.resumeMethod()
                    End If

                    timerDuring.IsEnabled = True    'resume timer
                    __commandInExecute = __commandBeforePause
                End If
                'reject redundant resuming
                Return IDrivable.endStatus.EXECUTING
            Case Else
                'in pause state , reject other commands
                If __commandInExecute.Equals(dcMotorCommands.MOTION_PAUSE) Then
                    Return IDrivable.endStatus.EXECUTING
                End If
        End Select



        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING
                '------------------------------
                '   Able to accept command
                '-------------------------------
                __commandInExecute = __commandInDcMotorCommand

                If (arg IsNot Nothing) Then
                    'the alias function
                    Me.__direction = PositionDictionary(arg).Direction
                    Me.__motionPeriod = PositionDictionary(arg).DrivingTime
                Else
                    __direction = PositionDictionary.First.Value.Direction
                    __motionPeriod = PositionDictionary.First.Value.DrivingTime
                End If

                __commandDriveState = IDrivable.drivenState.EXECUTING

            Case IDrivable.drivenState.WAIT_RECALL
                '------------------------
                '   Last command had beed executed , this cycle used to rewind
                '------------------------
                __commandDriveState = IDrivable.drivenState.LISTENING
                __commandInExecute = dcMotorCommands.NONE
                Return __commandEndStatus
            Case IDrivable.drivenState.EXECUTING
                '--------------------
                '   Do nothing
                '--------------------
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function
    Public Function drive(command As dcMotorCommands, __direction As MDc_Dir, __period As TimeSpan) As IDrivable.endStatus
        'the alias function
        Me.__direction = __direction
        Me.__motionPeriod = __period
        Return drive(command)
    End Function

    Public Property TimeoutLimit As TimeSpan Implements IDrivable.TimeoutLimit

    Dim timerDuring As singleTimerContinueType = New singleTimerContinueType With {.TimerGoal = New TimeSpan(0, 0, 5)}

#Region "command functions"
    Function jogCommand() As IDrivable.endStatus

        Select Case driveState
            Case driveStateEnum.READY
                '檢查DC馬達是否錯誤
                'Alarm : Normal ON
                If Not readBit(__drivenChannel.inAlarm) Then
                    'driver alarm occured and drive state is not on alarm handling , raising alarm
                    driveState = driveStateEnum.STOPPING

                Else
                    'normal
                    driveState = driveStateEnum.RUNNING
                End If

            Case driveStateEnum.RUNNING '延遲一段開始RUN所必須的時間
                If __drivenChannel.runMethod(methodState, Me.__direction) Then
                    methodState = 0
                    __lastRunningStatus = True
                    Return IDrivable.endStatus.EXECUTION_END
                Else
                    '------------------------
                    '   Sequence 
                    '------------------------
                End If
            Case driveStateEnum.STOPPING
                If __drivenChannel.stopMethod(methodState) Then
                    methodState = 0

                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = __drivenChannel.inAlarm
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     driveState = driveStateEnum.READY
                                                                                     Return True
                                                                                 End Function

                    End With

                    CentralAlarmObject.raisingAlarm(ap)
                    driveState = driveStateEnum.ALARM_HANDLING  'wait user response
                Else
                    '------------------------
                    '   Sequence 
                    '------------------------
                End If

            Case driveStateEnum.ALARM_HANDLING
                'wait user responsing , Hsien , 2015.06.03
            Case driveStateEnum.GIVEUP

        End Select


        Return IDrivable.endStatus.EXECUTING

    End Function
    Function positionCommand() As IDrivable.endStatus
        Select Case driveState
            Case driveStateEnum.READY
                '檢查DC馬達是否錯誤
                'Alarm : Normal ON
                If Not readBit(__drivenChannel.inAlarm) Then
                    'driver alarm occured and drive state is not on alarm handling , raising alarm
                    driveState = driveStateEnum.STOPPING
                Else
                    'normal

                    driveState = driveStateEnum.RUNNING
                End If

            Case driveStateEnum.RUNNING '延遲一段開始RUN所必須的時間
                If __drivenChannel.runMethod(methodState, Me.__direction) Then
                    methodState = 0

                    With timerDuring
                        .TimerGoal = __motionPeriod
                        .resetTimer()
                        .IsEnabled = True 'start counting
                    End With
                    __lastRunningStatus = True

                    driveState = driveStateEnum.COUNTING
                Else
                    '------------------------
                    '   Sequence 
                    '------------------------
                End If
            Case driveStateEnum.COUNTING
                If timerDuring.IsTimerTicked AndAlso
                    __drivenChannel.stopMethod(methodState) Then
                    'time'up stop motor
                    __lastRunningStatus = False
                    methodState = 0
                    Return IDrivable.endStatus.EXECUTION_END
                End If
                '----------------------
                '   Error Stopping
                '----------------------
            Case driveStateEnum.STOPPING
                If __drivenChannel.stopMethod(methodState) Then
                    methodState = 0

                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = __drivenChannel.inAlarm
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     driveState = driveStateEnum.READY
                                                                                     Return True
                                                                                 End Function

                    End With

                    CentralAlarmObject.raisingAlarm(ap)
                    driveState = driveStateEnum.ALARM_HANDLING  'wait user response
                Else
                    '------------------------
                    '   Sequence 
                    '------------------------
                End If

            Case driveStateEnum.ALARM_HANDLING
                'wait user responsing , Hsien , 2015.06.03
            Case driveStateEnum.GIVEUP

        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function
    Public Function stopCommand() As IDrivable.endStatus

        If (__drivenChannel.stopMethod(methodState)) Then
            __lastRunningStatus = False
            Return IDrivable.endStatus.EXECUTION_END
        End If

        Return IDrivable.endStatus.EXECUTING

    End Function
#End Region

    Protected commandDictionary As Dictionary(Of [Enum], IDrivable.commandFunctionPrototype) = New Dictionary(Of [Enum], IDrivable.commandFunctionPrototype)
    Property PositionDictionary As Dictionary(Of [Enum], dcMotorPoint) = New Dictionary(Of [Enum], dcMotorPoint)  ' mapping the local position index to global position index


    Sub New(channel As dcMotorChannelBase)
        '-----------------------
        '   Initializing Command functions
        '-----------------------
        With commandDictionary
            .Add(dcMotorCommands.JOG, AddressOf jogCommand)
            .Add(dcMotorCommands.GO_POSITION, AddressOf positionCommand)
            .Add(dcMotorCommands.STOP_FORCELY, AddressOf stopCommand)
            .Add(dcMotorCommands.STOP_SLOW_DOWN, AddressOf stopCommand)

            .Add(dcMotorCommands.MOTION_PAUSE, Function() (IDrivable.endStatus.EXECUTION_END))
            .Add(dcMotorCommands.MOTION_RESUME, Function() (IDrivable.endStatus.EXECUTION_END))
        End With

        Me.__drivenChannel = channel ' Hsien , 2016.03.30
    End Sub


    Protected Overrides Function process() As Integer

        '---------------------------
        '   Driven state-mahcine
        '---------------------------

        If __commandInExecute.Equals(dcMotorCommands.MOTION_PAUSE) Or
            __commandInExecute.Equals(dcMotorCommands.NONE) Then
            Return 0
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING,
                IDrivable.drivenState.WAIT_RECALL
                Return 0
            Case IDrivable.drivenState.EXECUTING
                '--------------------------------
                '   Executing
                '--------------------------------
                __commandEndStatus = commandDictionary(__commandInExecute)()
                If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
                    driveState = driveStateEnum.READY       'rewind
                    __commandDriveState = IDrivable.drivenState.WAIT_RECALL
                End If

        End Select

        Return 0
    End Function

    Public Overrides Function raisingGUI() As Control
        '----------------------------------------------------
        '   Hsien , 2015.02.05
        '----------------------------------------------------
        Dim uc As userControlDrivable = New userControlDrivable
        With uc
            .Component = Me
            .PropertyView = MyBase.raisingGUI()
        End With
        Return uc
    End Function

    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        Return commandDictionary.Keys
    End Function

End Class
