﻿Option Explicit On
Imports Automation.Components.Services
Imports System.ComponentModel
Imports Automation
Imports System.Threading
Imports System.Windows.Forms

Public MustInherit Class compHexaPodBase
    Inherits driveBase
    Implements IDrivable
    Implements Automation.IXYRTable

#Region "ENUMs"
    Public Enum motorCommandEnum As Integer
        NONE = 0
        HOME
        ''' <summary>
        ''' '利用點至點動作將手臂從目前位置移至指定點（先垂直向上移動，接著水平移動，最後向下垂直移至最終目標點）。 
        ''' </summary>
        ''' <remarks></remarks>
        JUMP
        GO
        ''' <summary>
        ''' EPSON無此功能，此功能為將here加上相對移動量
        '''  go here+xy(-1,2,3,4)，即相對移動x:-1, y:2, z:3, u:4
        ''' </summary>
        ''' <remarks></remarks>
        GO_REL
        MOTION_PAUSE
        MOTION_RESUME
        STOP_SLOW_DOWN
        WAIT_RECALL
#Region "copy from compMotor, not implement yet"
        STOP_FORCELY
#End Region
    End Enum
    Public Enum motionSubStateEnum
        SETUP_AND_GO = 0
        WAITEXECUTION
        WAITFINEPOSITION
    End Enum
    Enum returnErrorCodes As Short
        ERR_NoError = 0
        ERR_TaskIsNotCompleted = -1
        ERR_Execution_Time_Out = -2
        ERR_No_Assigning_Point = -3

    End Enum
    Enum windowsEnum
        IOMonitor = 1
        TaskManager = 2
        ForceMonitor = 3
        Simulator = 4
    End Enum
    Enum dialogEnum
        RobotManager = 1
        ControllerTools = 2
    End Enum
#End Region
    ''' <summary>
    ''' remember command 
    ''' </summary>
    ''' <remarks></remarks>
    Protected commandInExecute As motorCommandEnum
    Protected commandBeforePause As motorCommandEnum          ' used to back the command in execute before motion paused 
    Protected timeOutTimer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 30)}
    Protected __commandSubState As motionSubStateEnum = motionSubStateEnum.SETUP_AND_GO ' shared by all command functions , command function should rewind this in the end of execution
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTING
    Protected __returnStatus As Integer = 0
#Region "actions"
    Protected MustOverride Function _Stop() As Integer
    ''' <summary>
    ''' similar with motion done
    ''' </summary>
    ''' <remarks></remarks>
    Protected MustOverride Function CommandInCycle() As Boolean
    Public MustOverride Function GetPoint(pointIndex As Integer) As hexaPoint
    Protected MustOverride Function GetPosition() As Single()
    Protected MustOverride Function checkMotionDone() As hexaPodExceptionPack
    Protected MustOverride Function pauseCommand() As Integer
    Protected MustOverride Function resumeCommand() As Integer
    Protected MustOverride Function go() As hexaPodExceptionPack
    Protected MustOverride Function goRel() As hexaPodExceptionPack
    Protected MustOverride Function jump() As hexaPodExceptionPack
    Protected MustOverride Function home() As hexaPodExceptionPack
    Public MustOverride Function Connect(strIP As String, strPort As String) As UInteger ' "network:192.168.1.200:5000"

    Public MustOverride Function Connect(modelNum As UInteger, strIP As String, strPort As String) As UInteger ' "network:192.168.1.200:5000"

#End Region

    Public alarmPackage As alarmContextBase = New alarmContextBase With {.Sender = Me, .PossibleResponse = alarmContextBase.responseWays.ABORT}
#Region "Properties"
    Public Delegate Function commandFunctionPrototype(ByRef subState As Short) As IDrivable.endStatus
    <Browsable(False)> Property CommandFunctionDictionary As Dictionary(Of [Enum], commandFunctionPrototype) = New Dictionary(Of [Enum], commandFunctionPrototype)    ' managed all command functions
    Property PositionDictionary As Dictionary(Of [Enum], Short) = New Dictionary(Of [Enum], Short)  ' mapping the local position index to global position index
#End Region
#Region "Inner Class"
    Public Class hexaPoint
        Public Property PositionX As Double
        Public Property PositionY As Double
        Public Property PositionZ As Double
        Public Property RotationX As Double
        Public Property RotationY As Double
        Public Property RotationZ As Double
    End Class
    Public Class hexaPodExceptionPack
        Public Property ErrorNumber As Integer
        Public Property StackTrace As String
        Public Property Message As String
    End Class
#End Region

#Region "Path Table Settings / Simultanous move"
    'used to store motor point ready to execute iterationally , Hsien , 2014.09.27
    ReadOnly Property PointTable As List(Of hexaPoint) ' = New List(Of cMotorPoint)   'inject the MotorPoints as simultanous commands
        Get
            Return __pointTable
        End Get
        'Set(value As List(Of cMotorPoint))
        '    __pointTable = value
        'End Set
    End Property
    Protected __pointTable As List(Of hexaPoint) = New List(Of hexaPoint)
#End Region
    Property PositionPoint As hexaPoint 'Hsien , 2015.12.21, used when single position moving going to execute
        Get
            If PointTable.Count = 0 Then
                Return Nothing
            End If

            Return __pointTable.First
        End Get
        Set(value As hexaPoint)
            __pointTable.Clear()
            __pointTable.Add(value)
        End Set
    End Property
    Public Function drive(ByVal command As motorCommandEnum, ByVal globalCommandPosition As hexaPoint) As IDrivable.endStatus
        'the extension alias function , hsien , 2015.01.05
        If (commandInExecute.Equals(motorCommandEnum.NONE)) Then
            'avoid to corrupt the thread-shared data structure - pointTable , Hsien , 2016.02.15
            PositionPoint = globalCommandPosition
        End If
        Return drive(command)
    End Function
    Public Function drive(ByVal command As motorCommandEnum, ByVal ParamArray globalCommandPosition() As hexaPoint) As IDrivable.endStatus
        If (commandInExecute.Equals(motorCommandEnum.NONE)) Then
            'avoid to corrupt the thread-shared data structure - pointTable , Hsien , 2016.02.15
            PointTable.Clear()
            For Each positionIndex As hexaPoint In globalCommandPosition
                PointTable.Add(positionIndex)
            Next
        End If
        Return drive(command)
    End Function
    Public Function drive(ByVal command As motorCommandEnum, ByVal ParamArray commandPosition() As [Enum]) As IDrivable.endStatus
        'the extension alias function , hsien , 2015.01.05
        If (commandInExecute.Equals(motorCommandEnum.NONE)) Then
            'avoid to corrupt the thread-shared data structure - pointTable , Hsien , 2016.02.15
            PointTable.Clear()
            For Each positionIndex As [Enum] In commandPosition
                PointTable.Add(GetPoint(Convert.ToInt32(positionIndex)))
            Next
        End If
        Return drive(command)
    End Function
    Public Function drive(ByVal command As motorCommandEnum, Optional ByVal commandPosition As [Enum] = Nothing) As IDrivable.endStatus
        '-------------------------------------------------------------
        '   when command UNDETERMINED , will do initializing procedure
        '-------------------------------------------------------------
        ' first priority command : STOP_FORCELY
        If (command.Equals(motorCommandEnum.STOP_FORCELY)) Then
            '-----------------------------
            ' highest priority command
            'direct firing command
            '------------------------------
            timeOutTimer.IsEnabled = False
            __commandSubState = motionSubStateEnum.SETUP_AND_GO
            __commandEndStatus = IDrivable.endStatus.EXECUTION_END
            commandInExecute = motorCommandEnum.NONE

            'Slow down stop need time to down , have to use drive-command sequence control
            __returnStatus = _Stop()

            '----------------
            '   Wait until motor stopped
            '----------------
            While (CommandInCycle() <> False)
                'do nothing, waitting all stopped
            End While

            'reset error status
            '__returnStatus = AMaxM4_ErrorStatus(MotorIndex, __errorStatus)

            Return IDrivable.endStatus.EXECUTION_END

        End If

        '------------------
        '   PAUSE AND RESUME
        '------------------
        If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso
            command.Equals(motorCommandEnum.MOTION_PAUSE) Then
            pauseCommand()
            commandBeforePause = commandInExecute
            commandInExecute = motorCommandEnum.MOTION_PAUSE
            Return IDrivable.endStatus.EXECUTING
        End If
        If commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso
           Not command.Equals(motorCommandEnum.MOTION_RESUME) Then
            ' only resume command able to release pause command
            Return IDrivable.endStatus.EXECUTING 'reject
        End If
        If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso
          command.Equals(motorCommandEnum.MOTION_RESUME) Then
            ' only resume command able to release pause command
            Return IDrivable.endStatus.EXECUTING 'reject 
        End If
        If commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso
            command.Equals(motorCommandEnum.MOTION_RESUME) Then
            resumeCommand()
            commandInExecute = commandBeforePause
            Return IDrivable.endStatus.EXECUTING
        End If

        '------------------------
        '   STOP SLOWLY , Hsien  2015.01.10
        '-------------------------
        If (command.Equals(motorCommandEnum.STOP_SLOW_DOWN) AndAlso
            (Not commandInExecute.Equals(motorCommandEnum.STOP_SLOW_DOWN)) AndAlso
            (Not commandInExecute.Equals(motorCommandEnum.WAIT_RECALL))) Then
            '-----------------------------
            ' high priority command ,would override previous in-execute command
            '------------------------------
            resetCommand()
            commandInExecute = motorCommandEnum.STOP_SLOW_DOWN
            timeOutTimer.IsEnabled = True
            Return IDrivable.endStatus.EXECUTING
        End If


        ' when command is executed to the end , do not accecpt next command 
        ' caller should determine if recall by recognizing return states in this device
        If (commandInExecute.Equals(motorCommandEnum.WAIT_RECALL)) Then
            '----------------------------------
            ' last command had been executed
            '   1. rewind execution status
            '   2. return the execution result 
            '----------------------------------
            commandInExecute = motorCommandEnum.NONE
            Return __commandEndStatus
        End If

        If (Not commandInExecute.Equals(motorCommandEnum.NONE)) Then
            ' command on execute , reject command override
        Else
            If (commandPosition IsNot Nothing) Then
                '----------------------------------------------------------------
                'Hsien , 2014.11.14
                'point indicated , fetch point data from pData tank
                '----------------------------------------------------------------

                Try
                    PositionPoint = GetPoint(Convert.ToInt32(commandPosition))
                Catch ex As Exception
                    '---------------------------------
                    '   Key Not Found , Reject Command
                    '---------------------------------
                    ' raising alarm : exception
                    __returnStatus = returnErrorCodes.ERR_No_Assigning_Point
                    alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, __returnStatus, [Enum].GetName(GetType(returnErrorCodes), __returnStatus))
                    CentralAlarmObject.raisingAlarm(alarmPackage)
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End Try
            Else
                '----------------------------------------------------------------
                'Hsien , 2014.11.14
                '   No corresponding point indicated , use  positionPoint instead
                '   if positionPoint not assigned , motor would generate error when executing
                '----------------------------------------------------------------
            End If

            timeOutTimer.IsEnabled = True   'start time out checking for all command , Hsien , 2014.10.09
            commandInExecute = command

        End If

        Return IDrivable.endStatus.EXECUTING

    End Function



    Protected Overrides Function process() As Integer

        '---------------------------------
        'if alarmed , pause all motions? , answer : no , controlled by Host
        '---------------------------------

        '--------------------------
        '   drive the state-machine
        '--------------------------
        If (commandInExecute.Equals(motorCommandEnum.NONE) Or
            commandInExecute.Equals(motorCommandEnum.WAIT_RECALL) Or
            commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE)) Then
            ' performance considering
            Return 0
        End If

        __commandEndStatus = CommandFunctionDictionary(commandInExecute)(__commandSubState)
        If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
            ' bitwise operation
            ' reset command
            timeOutTimer.IsEnabled = False
            __commandSubState = motionSubStateEnum.SETUP_AND_GO
            commandInExecute = motorCommandEnum.WAIT_RECALL
        End If

        If (__commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL) Then
            '----------------------------
            'error occured
            'raise alarm and rewind state
            '----------------------------
            alarmPackage.CallbackResponse(alarmContextBase.responseWays.ABORT) = alarmContextBase.abortMethod   'Hsien , setup before raise alarm  , 2015.10.12
            CentralAlarmObject.raisingAlarm(alarmPackage)       'raising alarm
            'stop motor and reset all status


        End If

        Return 0
    End Function

    Protected Overridable Function resetCommand() As Integer
        ' reset motor into initial stage (wait command)
        __commandSubState = 0
        commandInExecute = motorCommandEnum.NONE
        'otherwise , next command may be skipped
        Return 0
    End Function


    Public Sub New()
        CommandFunctionDictionary.Add(motorCommandEnum.HOME, AddressOf homeCommand)
        CommandFunctionDictionary.Add(motorCommandEnum.GO, AddressOf goCommand)
        CommandFunctionDictionary.Add(motorCommandEnum.JUMP, AddressOf jumpCommand)
        CommandFunctionDictionary.Add(motorCommandEnum.GO_REL, AddressOf goRelCommand)
        'CommandFunctionDictionary.Add(motorCommandEnum.JOG, AddressOf jogCommand)
        CommandFunctionDictionary.Add(motorCommandEnum.STOP_FORCELY, Function() (IDrivable.endStatus.EXECUTION_END))

    End Sub
    ''' <summary>
    '''  
    ''' </summary>
    ''' <param name="subState">subState是__commandSubState，在New裡定義__commandEndStatus = CommandFunctionDictionary(commandInExecute)(__commandSubState)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function goCommand(ByRef subState As Short) As IDrivable.endStatus
        Select Case subState
            Case motionSubStateEnum.SETUP_AND_GO
                '--------------------------------------------
                '   Command content must be setted before
                '--------------------------------------------
                If CommandInCycle() = True Then Return IDrivable.endStatus.EXECUTION_END_FAIL 'if command in cycle, cannot execute new command

                Dim ex As hexaPodExceptionPack = go()
                If ex IsNot Nothing Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                timeOutTimer.IsEnabled = True   'restart timer when examnition changes
                subState = motionSubStateEnum.WAITEXECUTION

            Case motionSubStateEnum.WAITEXECUTION
                If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso Not timeOutTimer.IsEnabled Then
                    timeOutTimer.IsEnabled = True   'restart timer ,when changed examinition method
                ElseIf commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) Then
                    timeOutTimer.IsEnabled = False  'suspend timer
                End If
                If CommandInCycle() = False Then
                    Dim ex As hexaPodExceptionPack = checkMotionDone()
                    If ex IsNot Nothing Then
                        __returnStatus = ex.ErrorNumber
                        alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, ex.ErrorNumber, ex.Message)
                        Return IDrivable.endStatus.EXECUTION_END_FAIL
                    Else
                        Return IDrivable.endStatus.EXECUTION_END
                    End If
                End If
                If timeOutTimer.IsTimerTicked Then
                    __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
                    alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, __returnStatus, [Enum].GetName(GetType(returnErrorCodes), __returnStatus))
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
        End Select
        Return IDrivable.endStatus.EXECUTING
    End Function
    Protected Function goRelCommand(ByRef subState As Short) As IDrivable.endStatus
        Select Case subState
            Case motionSubStateEnum.SETUP_AND_GO
                '--------------------------------------------
                '   Command content must be setted before
                '--------------------------------------------
                If CommandInCycle() = True Then Return IDrivable.endStatus.EXECUTION_END_FAIL 'if command in cycle, cannot execute new command

                Dim ex As hexaPodExceptionPack = goRel()
                If ex IsNot Nothing Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                timeOutTimer.IsEnabled = True   'restart timer when examnition changes
                subState = motionSubStateEnum.WAITEXECUTION

            Case motionSubStateEnum.WAITEXECUTION
                If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso Not timeOutTimer.IsEnabled Then
                    timeOutTimer.IsEnabled = True   'restart timer ,when changed examinition method
                ElseIf commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) Then
                    timeOutTimer.IsEnabled = False  'suspend timer
                End If
                If CommandInCycle() = False Then
                    Dim ex As hexaPodExceptionPack = checkMotionDone()
                    If ex IsNot Nothing Then
                        __returnStatus = ex.ErrorNumber
                        alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, ex.ErrorNumber, ex.Message)
                        Return IDrivable.endStatus.EXECUTION_END_FAIL
                    Else
                        Return IDrivable.endStatus.EXECUTION_END
                    End If
                End If
                If timeOutTimer.IsTimerTicked Then
                    __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
                    alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, __returnStatus, [Enum].GetName(GetType(returnErrorCodes), __returnStatus))
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
        End Select
        Return IDrivable.endStatus.EXECUTING
    End Function
    Protected Function jumpCommand(ByRef subState As Short) As IDrivable.endStatus
        Select Case subState
            Case motionSubStateEnum.SETUP_AND_GO
                '--------------------------------------------
                '   Command content must be setted before
                '--------------------------------------------
                If CommandInCycle() = True Then Return IDrivable.endStatus.EXECUTION_END_FAIL 'if command in cycle, cannot execute new command

                Dim ex As hexaPodExceptionPack = jump()
                If ex IsNot Nothing Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                timeOutTimer.IsEnabled = True   'restart timer when examnition changes
                subState = motionSubStateEnum.WAITEXECUTION

            Case motionSubStateEnum.WAITEXECUTION
                If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso Not timeOutTimer.IsEnabled Then
                    timeOutTimer.IsEnabled = True   'restart timer ,when changed examinition method
                ElseIf commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) Then
                    timeOutTimer.IsEnabled = False  'suspend timer
                End If
                If CommandInCycle() = False Then
                    Dim ex As hexaPodExceptionPack = checkMotionDone()
                    If ex IsNot Nothing Then
                        __returnStatus = ex.ErrorNumber
                        alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, ex.ErrorNumber, ex.Message)
                        Return IDrivable.endStatus.EXECUTION_END_FAIL
                    Else
                        Return IDrivable.endStatus.EXECUTION_END
                    End If
                End If
                If timeOutTimer.IsTimerTicked Then
                    __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
                    alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, __returnStatus, [Enum].GetName(GetType(returnErrorCodes), __returnStatus))
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
        End Select
        Return IDrivable.endStatus.EXECUTING
    End Function
    Protected Function homeCommand(ByRef subState As Short) As IDrivable.endStatus
        Select Case subState
            Case motionSubStateEnum.SETUP_AND_GO
                '--------------------------------------------
                '   Command content must be setted before
                '--------------------------------------------
                If CommandInCycle() = True Then Return IDrivable.endStatus.EXECUTION_END_FAIL 'if command in cycle, cannot execute new command

                Dim ex As hexaPodExceptionPack = home()
                If ex IsNot Nothing Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                timeOutTimer.IsEnabled = True   'restart timer when examnition changes
                subState = motionSubStateEnum.WAITEXECUTION

            Case motionSubStateEnum.WAITEXECUTION
                If Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) AndAlso Not timeOutTimer.IsEnabled Then
                    timeOutTimer.IsEnabled = True   'restart timer ,when changed examinition method
                ElseIf commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) Then
                    timeOutTimer.IsEnabled = False  'suspend timer
                End If
                If CommandInCycle() = False Then
                    Dim ex As hexaPodExceptionPack = checkMotionDone()
                    If ex IsNot Nothing Then
                        __returnStatus = ex.ErrorNumber
                        alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, ex.ErrorNumber, ex.Message)
                        Return IDrivable.endStatus.EXECUTION_END_FAIL
                    Else
                        Return IDrivable.endStatus.EXECUTION_END
                    End If
                End If
                If timeOutTimer.IsTimerTicked Then
                    __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
                    alarmPackage.AdditionalInfo = String.Format("Scara Error!{0}Error Code={1}{0}Error Message={2}", vbNewLine, __returnStatus, [Enum].GetName(GetType(returnErrorCodes), __returnStatus))
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
        End Select
        Return IDrivable.endStatus.EXECUTING
    End Function
    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            If (commandInExecute.Equals(motorCommandEnum.WAIT_RECALL)) Then
                Return IDrivable.drivenState.WAIT_RECALL
            ElseIf (commandInExecute.Equals(motorCommandEnum.NONE)) Then
                Return IDrivable.drivenState.LISTENING
            Else
                Return IDrivable.drivenState.EXECUTING
            End If
        End Get
    End Property

    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get
            If (commandInExecute.Equals(motorCommandEnum.WAIT_RECALL)) Then
                resetCommand()
            End If
            'used interpreter to bridge
            Return __commandEndStatus
        End Get
    End Property

    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        Return CommandFunctionDictionary.Keys
    End Function

    Public Property TimeoutLimit As TimeSpan Implements IDrivable.TimeoutLimit

    Private Function driveIDrivable(command As [Enum], Optional __arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive
        Return drive(command)
    End Function

    Public ReadOnly Property CommandInExecuteIDrivable As Object Implements IDrivable.CommandInExecute
        Get
            Return commandInExecute
        End Get
    End Property
    Public Overrides Function raisingGUI() As Control
        '----------------------------------------------------
        '   Hsien , 2015.02.05s
        '----------------------------------------------------
        Dim uc As userControlScara = New userControlScara
        With uc
            .scara = Me
            .PropertyView = New userControlPropertyView With {.Drive = Me}
        End With
        Return uc
    End Function

    Public MustOverride Sub showWindow(window As windowsEnum, sender As Windows.Forms.Form)
    Public MustOverride Sub runDialog(dialog As dialogEnum, sender As Windows.Forms.Form)
    Public MustOverride Sub programMode()
    Public MustOverride Sub reset()
    Public MustOverride Sub teachPoint()
    Public MustOverride ReadOnly Property IsConnected As Boolean
    Public MustOverride ReadOnly Property IsReferenced As Boolean
    Protected Overrides Sub Finalize()
        Disconnect()
        MyBase.Finalize()
    End Sub
    Public MustOverride Function Disconnect() As Integer

    Public Function Connect() As Automation.IXYRTable.enStatus Implements Automation.IXYRTable.Connect
        Return Automation.IXYRTable.enStatus.DONE
    End Function

    Public Function GetPosition(ByRef x As Single, ByRef y As Single, ByRef r As Single) As Integer Implements Automation.IXYRTable.GetPosition
        Dim value() As Single = GetPosition()
        If value.GetUpperBound(0) <> 8 Then Return 0
        x = value(0)
        y = value(1)
        r = value(3)
        Return 0
    End Function
    Public Function GetPosition(ByRef x As Single, ByRef y As Single, ByRef z As Single, ByRef u As Single) As Integer
        Dim value() As Single = GetPosition()
        If value.GetUpperBound(0) <> 8 Then Return 0
        x = value(0)
        y = value(1)
        z = value(2)
        u = value(3)
        Return 0
    End Function

    Public Function MotionDone() As Automation.IXYRTable.enStatus Implements Automation.IXYRTable.MotionDone
        Select Case CommandEndStatus
            Case Automation.IDrivable.endStatus.EXECUTING : Return Automation.IXYRTable.enStatus.BUSY
            Case Automation.IDrivable.endStatus.EXECUTION_END : Return Automation.IXYRTable.enStatus.DONE
            Case Automation.IDrivable.endStatus.EXECUTION_END_FAIL : Return Automation.IXYRTable.enStatus.FAIL
        End Select
        Return Automation.IXYRTable.enStatus.BUSY
    End Function

    Public Function MoveRel(x As Single, y As Single, r As Single) As Automation.IXYRTable.enStatus Implements Automation.IXYRTable.MoveRel
        '選擇哪個面向(側、底、上)
        drive(motorCommandEnum.GO_REL, New hexaPoint With {.PositionX = x, .PositionY = y, .RotationZ = r})
        Return IXYRTable.enStatus.DONE
    End Function

    Public ReadOnly Property PositionR As Double Implements Automation.IXYRTable.PositionR
        Get
            Dim x As Single, y As Single, r As Single
            GetPosition(x, y, r)
            Return r
        End Get
    End Property

    Public ReadOnly Property PositionX As Double Implements Automation.IXYRTable.PositionX
        Get
            Dim x As Single, y As Single, r As Single
            GetPosition(x, y, r)
            Return x
        End Get
    End Property
    Public ReadOnly Property PositionY As Double Implements Automation.IXYRTable.PositionY
        Get
            Dim x As Single, y As Single, r As Single
            GetPosition(x, y, r)
            Return y
        End Get
    End Property
End Class
