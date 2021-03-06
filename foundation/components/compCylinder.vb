﻿Imports Automation.Components.Services
Imports System.Text
Imports Automation.mainIOHardware

Public Class cylinderControlBase
    Inherits driveBase
    Implements IDrivable

    '----------------------------------
    '   For Those Type of Cylinder with control flow:
    '   1. drive
    '   2. check end
    '   3. check stable
    '   4. start checkservice
    '   Derived type should determine : 
    '   methods:
    '       1. trigger method
    '       2. check end reached method
    '       3. check end stable method
    '   service:
    '       1. check service when end reached
    '----------------------------------
    Public Enum cylinderCommands
        GO_A_END
        GO_B_END
    End Enum

#Region "common interface"
    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            Return __commandDriveState
        End Get
    End Property

    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get
            If (__commandDriveState = IDrivable.drivenState.WAIT_RECALL) Then
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

    Public Property StrokeGoal As TimeSpan Implements IDrivable.TimeoutLimit
        Get
            'Return timeoutTimer.TimerGoal
            Return __strokeGoal
        End Get
        Set(value As TimeSpan)
            'timeoutTimer.TimerGoal = value
            __strokeGoal = value
        End Set
    End Property

#End Region

    Property EnableCheckService As Boolean    'determine if check service open
        Get
            Return monitorCheckService.IsEnabled
        End Get
        Set(value As Boolean)
            monitorCheckService.IsEnabled = True
        End Set
    End Property
    '-------------------
    '   Used to indicate the method index
    '-------------------
    Protected Enum methodsIndexEnum As Integer
        TRIGGER_METHOD = 0
        CHECK_END_REACHED_METHOD = 1
        CHECK_END_STABLE_METHOD = 2

    End Enum
    'key : command
    'value : methods , 1: checkMethod 2: trigger method
    Protected methodsDictionary As Dictionary(Of cylinderCommands, Func(Of Boolean)()) = New Dictionary(Of cylinderCommands, Func(Of Boolean)())

    Protected WithEvents monitorCheckService As genericCheckService = New genericCheckService
    Property IsMonitorSensor As Boolean = False ' hsien , 2015.04.04 , use to control whether keeping monitoring endSensor after drived


    Protected __commandDriveState As IDrivable.drivenState = IDrivable.drivenState.LISTENING
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTION_END
    Protected __commandInExecute As cylinderCommands = cylinderCommands.GO_A_END

    ' internal - components
    Protected timeoutTimer As singleTimer = New singleTimer() With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Protected __stableGoal As TimeSpan = New TimeSpan(0, 0, 0, 0, 500)  'used to check if statble
    Protected __strokeGoal As TimeSpan = New TimeSpan(0, 0, 5)

    Protected Friend alarmPackEndFail As alarmContextBase

    Public Function drive(command As [Enum], Optional arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive

        If Not IsEnabled Then
            Return __commandEndStatus
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING
                '------------------------------
                '   Able to accept command
                '-------------------------------
                __commandInExecute = command
                timeoutTimer.IsEnabled = True
                '-------------------------
                '   Command accepted
                '-------------------------
                __commandDriveState = IDrivable.drivenState.EXECUTING
            Case IDrivable.drivenState.WAIT_RECALL
                '------------------------
                '   Last command had beed executed , rewind
                '------------------------
                __commandDriveState = IDrivable.drivenState.LISTENING
                Return __commandEndStatus
            Case IDrivable.drivenState.EXECUTING

                '__commandDriveState = IDrivable.drivenState.EXECUTING

        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function
    Protected Overrides Function process() As Integer
        '--------------------------
        '   Running the end sensors
        '--------------------------
        monitorCheckService.running()
        '---------------------------
        '   Driven state-mahcine
        '---------------------------
        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING,
                IDrivable.drivenState.WAIT_RECALL
                Return 0
            Case IDrivable.drivenState.EXECUTING
                '--------------------------------
                '   Executing
                '--------------------------------
                __commandEndStatus = driveCylinder()
                If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
                    timeoutTimer.IsEnabled = False
                    driveState = driveStateEnum.READY       'rewind
                    __commandDriveState = IDrivable.drivenState.WAIT_RECALL
                End If

        End Select

        Return 0
    End Function

    Protected Enum driveStateEnum
        READY
        EXECUTE             'execute the trigger method
        CHECK_SATISFIED       ' wait until sensor turns on
        'STABLE_SATISFIED   ' wait until sensor satisfied stable condition
        ALARM_HANDLING  ' the state wait user response
        GIVEUP          ' user give up to check sensor
    End Enum
    Protected driveState As driveStateEnum = driveStateEnum.READY

    Private Function driveCylinder() As IDrivable.endStatus

        Select Case driveState
            Case driveStateEnum.READY
                driveState = driveStateEnum.EXECUTE
                '------------------------------
                '   Start Drive Cyclinder
                '------------------------------
            Case driveStateEnum.EXECUTE
                If (methodsDictionary(__commandInExecute)(methodsIndexEnum.TRIGGER_METHOD)()) Then

                    ' cylinder should arrive in goal time
                    timeoutTimer.TimerGoal = __strokeGoal
                    timeoutTimer.IsEnabled = True

                    'Hsien , shut sensor check servive in transit state
                    monitorCheckService.IsEnabled = False

                    driveState = driveStateEnum.CHECK_SATISFIED
                End If
            Case driveStateEnum.CHECK_SATISFIED
                If (methodsDictionary(__commandInExecute)(methodsIndexEnum.CHECK_END_REACHED_METHOD)()) Then
                    ' successfully , start monitoring if signal stable
                    ' successfully , work done
                    '-----------------------------------------------------------
                    'Hsien , start sensor check servive to monitor sensor status
                    '-----------------------------------------------------------
                    With monitorCheckService
                        .ObjectConditionsNeedToCheck.Clear()
                        .ObjectConditionsNeedToCheck.Add(New genericCheckService.genericCheckCondition With {.Sender = __commandInExecute,
                                                                                                             .Condition = methodsDictionary(__commandInExecute)(methodsIndexEnum.CHECK_END_REACHED_METHOD)})
                        .IsEnabled = IsMonitorSensor ' hsien , 2015.04.04 performance issue , keep I/O polling would increase latency  ' true
                    End With

                    driveState = driveStateEnum.READY       'rewind
                    Return IDrivable.endStatus.EXECUTION_END

                ElseIf (timeoutTimer.IsTimerTicked) Then
                    ' failed , raising alarm (sensor failed)
                    CentralAlarmObject.raisingAlarm(alarmPackEndFail)   'end failed
                    driveState = driveStateEnum.ALARM_HANDLING
                End If

            Case driveStateEnum.ALARM_HANDLING
                '-------------------------------
                '   The waiting state
                '-------------------------------
            Case driveStateEnum.GIVEUP
                '--------------------------------
                '   Give up this operation
                '--------------------------------
                driveState = driveStateEnum.READY
                Return IDrivable.endStatus.EXECUTION_END_FAIL
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

    Public Overrides Function raisingGUI() As Control
        '----------------------------------------------------
        '   Hsien , 2015.02.05
        '----------------------------------------------------
        Dim uc As userControlCylinderBase = New userControlCylinderBase
        With uc
            .cylinderReference = Me
            .PropertyView = MyBase.raisingGUI()
        End With
        Return uc
    End Function
    Protected Overrides Function enableDetail(arg As Boolean) As Integer

        If arg Then
            'if enabled , set default command end status as EXECUTING
            __commandEndStatus = IDrivable.endStatus.EXECUTING
        Else
            'if disabled , set default command end status as END (As Dummy Response)
            __commandEndStatus = IDrivable.endStatus.EXECUTION_END
        End If

        Return MyBase.enableDetail(arg)
    End Function


    Public Sub New()

        '--------------------------------------
        '   Configure Default Method Pairs
        '--------------------------------------
        methodsDictionary.Add(cylinderCommands.GO_A_END, New Func(Of Boolean)() {New Func(Of Boolean)(Function() (True)), New Func(Of Boolean)(Function() (True)), New Func(Of Boolean)(Function() (True))})
        methodsDictionary.Add(cylinderCommands.GO_B_END, New Func(Of Boolean)() {New Func(Of Boolean)(Function() (True)), New Func(Of Boolean)(Function() (True)), New Func(Of Boolean)(Function() (True))})
    End Sub


    'Hsien , 2015.08.31 , reconfigure end sensor check service
    Private Sub endSensorMonitoringHandler(sender As Object, e As genericCheckService.genericCheckEventArgs) Handles monitorCheckService.CheckFailed
        If ((Not Me.IsMyAlarmCurrent() And
            Not Me.IsMyAlarmInQueue)) Then
            'once alarm occured , stop monitor , prevent alarm over-run , 2015.09.30 

            '-------------------------
            '   Prepare alarm message
            '-------------------------

            CentralAlarmObject.raisingAlarm(alarmPackEndFail)   'raise alarm

        End If
    End Sub

    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        'enginnering GUI usage
        Return methodsDictionary.Keys
    End Function
End Class

Public Class cylinderVirtual
    Inherits cylinderControlBase
    '--------------------------------------------------
    '   Used to demostrate/verify cylinder control base
    '--------------------------------------------------

    Enum virtualControlFlagsEnum
        END_ACTURATOR
        END1_SENSOR
        END2_SENSOR
    End Enum

    Public virtualControlFlags As flagController(Of virtualControlFlagsEnum) = New flagController(Of virtualControlFlagsEnum)

    Function triggerMethod() As Boolean

        Select Case __commandInExecute
            Case cylinderCommands.GO_A_END
                virtualControlFlags.setFlag(virtualControlFlagsEnum.END_ACTURATOR)
            Case cylinderCommands.GO_B_END
                virtualControlFlags.resetFlag(virtualControlFlagsEnum.END_ACTURATOR)
            Case Else

        End Select

        Return True
    End Function

    Function checkEndReached() As Boolean

        Select Case __commandInExecute
            Case cylinderCommands.GO_A_END
                Return virtualControlFlags.viewFlag(virtualControlFlagsEnum.END1_SENSOR)
            Case cylinderCommands.GO_B_END
                Return virtualControlFlags.viewFlag(virtualControlFlagsEnum.END2_SENSOR)
            Case Else

        End Select

        Return False
    End Function

    Sub New()
        'method configuration
        methodsDictionary.Clear()
        methodsDictionary.Add(cylinderCommands.GO_A_END,
                              {New Func(Of Boolean)(AddressOf triggerMethod),
                              Function() (virtualControlFlags.viewFlag(virtualControlFlagsEnum.END1_SENSOR)),
                              Function() (virtualControlFlags.viewFlag(virtualControlFlagsEnum.END1_SENSOR))})
        methodsDictionary.Add(cylinderCommands.GO_B_END,
                      {New Func(Of Boolean)(AddressOf triggerMethod),
                      Function() (virtualControlFlags.viewFlag(virtualControlFlagsEnum.END2_SENSOR)),
                      Function() (virtualControlFlags.viewFlag(virtualControlFlagsEnum.END2_SENSOR))})
        'alarm pack configuration
        Me.alarmPackEndFail = New alarmContentSensor() With {.Sender = Me,
                                                           .Inputs = 0,
                                                           .AdditionalInfo = "End failed"}
        With CType(alarmPackEndFail, alarmContentSensor)
            .Sender = Me
            .Inputs = 0
            .AdditionalInfo = "End failed"
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                         timeoutTimer.IsEnabled = True  'reset
                                                                         driveState = cylinderControlBase.driveStateEnum.EXECUTE
                                                                         Return True
                                                                     End Function
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                          driveState = driveStateEnum.GIVEUP
                                                                          Return True
                                                                      End Function
        End With

    End Sub

End Class


Public Class alarmContextMultiSensors
    Inherits alarmContentSensor

    Public Overrides Property Inputs As ULong
        Get
            If (SensorConditionList.Count > 0) Then
                Return SensorConditionList.First.Key.InputBit
            End If

            Return MyBase.Inputs
        End Get
        Set(value As ULong)
            __input = value
        End Set
    End Property
    Property SensorConditionList As List(Of KeyValuePair(Of sensorControl, alarmReasonSensor)) = New List(Of KeyValuePair(Of sensorControl, alarmReasonSensor))

    Public Overrides Function ToString() As String
        'output formatted strings
        Try

            Dim __string As StringBuilder = New StringBuilder()
            For Each pair As KeyValuePair(Of sensorControl, alarmReasonSensor) In SensorConditionList
                Dim __sensorName As [Enum] = [Enum].ToObject(InputsEnumType, pair.Key.InputBit)
                __string.AppendLine(String.Format("{0} , {1}", __sensorName, pair.Value))
            Next

            Return __string.ToString()

        Catch ex As Exception
            Return "alarmContextMultiSensors , error"
        End Try
    End Function

End Class

''' <summary>
''' The generic cylinder perform trigger->confirm->monitoring operation.
''' </summary>
''' <remarks></remarks>
Public Class cylinderGeneric
    Inherits cylinderControlBase
    '----------------------------------
    '   The cylinder with one actuator but dual end sensor on single end
    '----------------------------------

    ''' <summary>
    ''' The wrapper for single actuator usage
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActuatorBit As ULong
        Get
            Return actuatorBGroup.First
        End Get
        Set(ByVal value As ULong)
            actuatorAGroup.Clear()
            With actuatorBGroup
                .Clear()
                .Add(value)
            End With
        End Set
    End Property
    ''' <summary>
    ''' The wrapper for single actuator usage
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OffEndSensor As ULong
        Get
            Return endASensorGroup.First.InputBit
        End Get
        Set(ByVal value As ULong)
            With endASensorGroup
                .Clear()
                .Add(New sensorControl With {.InputBit = value})
            End With
        End Set
    End Property
    ''' <summary>
    ''' The wrapper for single actuator usage
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OnEndSensor As ULong
        Get
            Return endBSensorGroup.First.InputBit
        End Get
        Set(ByVal value As ULong)
            With endBSensorGroup
                .Clear()
                .Add(New sensorControl With {.InputBit = value})
            End With
        End Set
    End Property

    Public endASensorGroup As List(Of sensorControl) = New List(Of sensorControl)
    Public endBSensorGroup As List(Of sensorControl) = New List(Of sensorControl)

    Public actuatorAGroup As List(Of ULong) = New List(Of ULong)
    Public actuatorBGroup As List(Of ULong) = New List(Of ULong)


    Function triggerAend() As Boolean
        'trigger
        actuatorAGroup.ForEach(Sub(output As ULong) writeBit(output, True))
        actuatorBGroup.ForEach(Sub(output As ULong) writeBit(output, False))

        'setup alarmpackage
        With CType(alarmPackEndFail, alarmContextMultiSensors)
            .SensorConditionList.Clear()    'clear the list

            For Each sensor As sensorControl In endASensorGroup
                .SensorConditionList.Add(New KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor)(sensor, alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON))
            Next
            For Each sensor As sensorControl In endBSensorGroup
                .SensorConditionList.Add(New KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor)(sensor, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF))
            Next
        End With
        Return True
    End Function
    Function triggerBend() As Boolean
        'trigger
        actuatorAGroup.ForEach(Sub(output As ULong) writeBit(output, False))
        actuatorBGroup.ForEach(Sub(output As ULong) writeBit(output, True))

        'setup alarmpackage
        With CType(alarmPackEndFail, alarmContextMultiSensors)
            .SensorConditionList.Clear()    'clear the list

            For Each sensor As sensorControl In endASensorGroup
                .SensorConditionList.Add(New KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor)(sensor, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF))
            Next
            For Each sensor As sensorControl In endBSensorGroup
                .SensorConditionList.Add(New KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor)(sensor, alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON))
            Next
        End With
        Return True
    End Function


    Function isAllUncovered(group As List(Of sensorControl)) As Boolean
        'return value : true : all sensor is uncovered (or no any sensor need to check), false : any of one convered
        'return value : true : all sensor is convered , false : any of one unconvered
        Return (group.Count = 0) Or group.TrueForAll(Function(__sensor As sensorControl) (Not __sensor.IsSensorCovered))
    End Function
    Function isAllCovered(group As List(Of sensorControl)) As Boolean
        'return value : true : all sensor is convered , false : any of one unconvered
        Return (group.Count = 0) Or group.TrueForAll(Function(__sensor As sensorControl) (__sensor.IsSensorCovered))
    End Function

    Sub New()
        'configure methods
        methodsDictionary.Clear()
        methodsDictionary.Add(cylinderCommands.GO_A_END,
                              {New Func(Of Boolean)(AddressOf triggerAend),
                               Function() (isAllCovered(endASensorGroup) And isAllUncovered(endBSensorGroup)),
                               Function() (isAllCovered(endASensorGroup) And isAllUncovered(endBSensorGroup))})
        methodsDictionary.Add(cylinderCommands.GO_B_END,
                      {New Func(Of Boolean)(AddressOf triggerBend),
                       Function() (isAllCovered(endBSensorGroup) And isAllUncovered(endASensorGroup)),
                       Function() (isAllCovered(endBSensorGroup) And isAllUncovered(endASensorGroup))})
        'alarm configuration
        Me.alarmPackEndFail = New alarmContextMultiSensors
        With CType(alarmPackEndFail, alarmContextMultiSensors)
            .Sender = Me
            .PossibleResponse = alarmContextBase.responseWays.RETRY 'Hsien , 2015.10.05 , retry only
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean

                                                                         If (__commandDriveState = IDrivable.drivenState.LISTENING Or
                                                                            __commandDriveState = IDrivable.drivenState.WAIT_RECALL) Then
                                                                             'on monitoring
                                                                         Else
                                                                             'on method execution
                                                                             timeoutTimer.IsEnabled = True  'timer reset
                                                                             driveState = driveStateEnum.READY  'Hsien , 2015.04.16 , re-drive
                                                                         End If

                                                                         Return True
                                                                     End Function
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean

                                                                          If (__commandDriveState = IDrivable.drivenState.LISTENING Or
                                                                              __commandDriveState = IDrivable.drivenState.WAIT_RECALL) Then
                                                                              'on monitoring
                                                                          Else
                                                                              'on method execution
                                                                              driveState = driveStateEnum.GIVEUP
                                                                          End If

                                                                          monitorCheckService.IsEnabled = False
                                                                          Return True
                                                                      End Function
        End With

    End Sub

End Class