Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' Availiable for clamp-on-fly , static clamp , clamp 4 side
''' </summary>
''' <remarks></remarks>
Public Class clampModule
    Inherits systemControlPrototype
    Implements IFinishableStation
    Implements IModuleSingle


    Dim _triggerClampMethod As triggerClampMethodEnum = triggerClampMethodEnum.METHOD_BY_SENSOR
    Dim _motionMethod As motorControl.motorCommandEnum = motorControl.motorCommandEnum.GO_POSITION_COMBINED
    Dim _actuatorComponent As actuatorComponentEnum = actuatorComponentEnum.Motor

    ''' <summary>
    ''' Used to synchronize with conveyor (optional)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetPositionInfo As Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo

    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

#Region "control members"
    Public motorClamp As motorControl = New motorControl()
    Public motorConveyorReference As motorControl = Nothing
    Public clampCylinder As cylinderGeneric = New cylinderGeneric
    ''' <summary>
    ''' Used to prevent interfered before motion start
    ''' </summary>
    ''' <remarks></remarks>
    Public interfereSensors As List(Of sensorControl) = New List(Of sensorControl)
    Public clampTriggerSensor As sensorControl = New sensorControl With {.InputBit = 0}
#End Region
#Region "External Data declare"
    Public motorPointClamp As cMotorPoint
    Public motorPointRelease As cMotorPoint

    Private isTriggered As Func(Of Boolean) = AddressOf isTriggerSensorDetected '查看觸發條件是否成立
    Private isTriggerReseted As Func(Of Boolean) = Function() (True)

    Dim motionSequenceState As Integer = 0
    Dim motionSequence As stateFunction = AddressOf motionMethodCombined
    Dim homeSequence As stateFunction = AddressOf homeMethodMotor

    Dim alarmPackHomingSensorCovered As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                                          .PossibleResponse = alarmContextBase.responseWays.RETRY,
                                                                                          .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF}

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConveyorTriggerDistanceInUnit As Double
        Get
            If motorConveyorReference Is Nothing Then
                Return 0
            End If
            Return motorConveyorReference.pulse2Unit(ConveyorTriggerDistance)
        End Get
        Set(value As Double)
            If motorConveyorReference IsNot Nothing Then
                ConveyorTriggerDistance = motorConveyorReference.unit2Pulse(value)
            End If
        End Set
    End Property
    Public Property ConveyorTriggerDistance As Double = 0 'record conveyor trigger position
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property TriggerClampMethod As triggerClampMethodEnum
        Get
            Return _triggerClampMethod
        End Get
        Set(value As triggerClampMethodEnum)
            _triggerClampMethod = value
            Select Case value
                Case triggerClampMethodEnum.METHOD_BY_SENSOR
                    isTriggered = AddressOf isTriggerSensorDetected
                    isTriggerReseted = Function() (True)
                Case triggerClampMethodEnum.METHOD_BY_BELT_POSITION
                    isTriggered = AddressOf isTriggerPositionDetected
                    isTriggerReseted = AddressOf isTriggerPositionReseted
                Case triggerClampMethodEnum.METHOD_BY_MODULE_ACTION
                    isTriggered = AddressOf isTriggerModuleActionRised
                    isTriggerReseted = Function() (True)
            End Select
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property MotionMethod As motorControl.motorCommandEnum
        Get
            Return _motionMethod
        End Get
        Set(value As motorControl.motorCommandEnum)
            _motionMethod = value
            Select Case value
                Case motorControl.motorCommandEnum.GO_POSITION_COMBINED
                    motionSequence = AddressOf motionMethodCombined
                Case motorControl.motorCommandEnum.GO_POSITION
                    motionSequence = AddressOf motionMethodSoftSerial
                Case Else
                    MsgBox(Me.DeviceName & " motion method setting error!")
            End Select

        End Set
    End Property

    ''' <summary>
    ''' Clamp actuator maybe Motor or Cylinder
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ActuatorComponent As actuatorComponentEnum
        Get
            Return _actuatorComponent
        End Get
        Set(value As actuatorComponentEnum)
            _actuatorComponent = value
            Select Case value
                Case actuatorComponentEnum.Motor
                    homeSequence = AddressOf homeMethodMotor
                Case actuatorComponentEnum.Cylinder
                    homeSequence = AddressOf homeMethodCylinder
                    motionSequence = AddressOf motionMethodCylinder
            End Select

        End Set
    End Property

#End Region

    Enum triggerClampMethodEnum
        METHOD_BY_BELT_POSITION
        METHOD_BY_SENSOR
        METHOD_BY_MODULE_ACTION 'Hsien , integrate with normal clamp , 2016.08.24
    End Enum
    Enum actuatorComponentEnum
        Motor
        Cylinder
    End Enum

    Sub alarmOccursHandler() Handles CentralAlarmObject.alarmOccured, PauseBlock.InterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) AndAlso _actuatorComponent = actuatorComponentEnum.Motor Then
            motorClamp.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        End If
    End Sub
    Sub alarmReleaseHandler() Handles CentralAlarmObject.alarmReleased, PauseBlock.UninterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) AndAlso _actuatorComponent = actuatorComponentEnum.Motor Then
            motorClamp.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        End If
    End Sub

    Protected Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState = 10
                End If
            Case 10
                If interfereSensors.Count > 0 AndAlso
                    interfereSensors.Exists(Function(sensor As sensorControl) sensor.IsSensorCovered) Then

                    'had been interfered , error
                    With alarmPackHomingSensorCovered '只有重試
                        .Inputs = interfereSensors.Find(Function(sensor As sensorControl) sensor.IsSensorCovered).InputBit
                    End With
                    CentralAlarmObject.raisingAlarm(alarmPackHomingSensorCovered)  'Hsien , 2015.06.15
                Else
                    systemSubState = 20
                End If
            Case 20
                If homeSequence(motionSequenceState) Then
                    motionSequenceState = 0
                    systemSubState = 500
                End If

            Case 500 '設定連續動作
                FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
        End Select

        Return 0
    End Function

    Protected Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                If isTriggered.Invoke Then '偵測上緣觸發
                    systemSubState = 10
                Else
                    '---------------------------------------------------------------
                    'not invoke   release conveyor
                    '---------------------------------------------------------------
                    TargetPositionInfo.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                End If
            Case 10
                If motionSequence(motionSequenceState) Then
                    motionSequenceState = 0 ' reset state

                    systemSubState = 20
                Else
                    '-----------------
                    '   Motion is executing
                    '-----------------
                End If
            Case 20

                'synchron after motion done , ensure all motion in regular sequence , Hsien , 2016.02.29
                TargetPositionInfo.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)

                If (isTriggerReseted.Invoke) Then
                    systemSubState = 0
                Else
                    '----------------
                    '   Not reseted yet
                    '----------------
                End If

        End Select

        Return 0
    End Function

    Function initMappingAndSetup()

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE

        actionComponents.Remove(motorConveyorReference)  'Hsien , 2016.02.17 , do not inlcude the conveyor

        Return 0
    End Function
    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf Me.initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf Me.initEnableAllDrives))
    End Sub
    Protected Overrides Function process() As Integer
        'Hsien , prevent interrupted by alarm , 2015.07.08

        '----------------------------
        'standard system control flow
        '----------------------------

        drivesRunningInvoke()

        'on the executing mode ,wont be affectted by alarm , Hsien , 2015.07.08
        If ((CentralAlarmObject.IsAlarmed AndAlso MainState = systemStatesEnum.IGNITE) OrElse
            PauseBlock.IsPaused And systemSubState = 0) Then
            'do not pause during moving , Hsien , 2015.09.07
            Return 0
        End If

        stateControl()
        processProgress()

        Return 0

    End Function


#Region "Trigger methods"
    Function isTriggerSensorDetected() As Boolean
        Return clampTriggerSensor.RisingEdge.IsDetected
    End Function
    Function isTriggerPositionDetected() As Boolean
        Return motorConveyorReference.CommandPosition >= ConveyorTriggerDistance
    End Function
    Function isTriggerPositionReseted() As Boolean
        Return motorConveyorReference.CommandPosition < ConveyorTriggerDistance
    End Function
    Function isTriggerModuleActionRised() As Boolean
        Return TargetPositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) And
            TargetPositionInfo.Invoke.IsPositionOccupied
    End Function
#End Region

#Region "motion sequences"
    Function motionMethodCombined(ByRef state As Integer) As Boolean
        Return motorClamp.drive(motorControl.motorCommandEnum.GO_POSITION_COMBINED) =
            motorControl.statusEnum.EXECUTION_END
    End Function
    Function motionMethodSoftSerial(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                If motorClamp.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointClamp) =
                     motorControl.statusEnum.EXECUTION_END Then
                    state = 10
                End If
            Case 10
                If motorClamp.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointRelease) =
                     motorControl.statusEnum.EXECUTION_END Then
                    Return True
                End If
        End Select

        Return False
    End Function

    Function motionMethodCylinder(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                If clampCylinder.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    state = 10
                End If

            Case 10
                If clampCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    Return True
                End If
        End Select
        Return False
    End Function

    Function homeMethodMotor(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                If motorClamp.drive(motorControl.motorCommandEnum.GO_HOME) = motorControl.statusEnum.EXECUTION_END Then
                    state = 10
                End If

            Case 10
                If motorClamp.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointRelease) = motorControl.statusEnum.EXECUTION_END Then
                    state = 20
                End If

            Case 20 '設定連續動作
                motorClamp.PointTable.Clear()
                motorClamp.PointTable.AddRange({motorPointClamp, motorPointRelease})
                Return True

        End Select
        Return False
    End Function

    Function homeMethodCylinder(ByRef state As Integer) As Boolean
        Return clampCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
            IDrivable.endStatus.EXECUTION_END
    End Function
#End Region

End Class
