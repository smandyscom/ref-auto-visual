Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components
Imports Automation.Components.Services
Imports Automation.mainIOHardware
Imports Automation.alarmContentSensor

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class connectionPort

    ReadOnly Property Score As Integer
        Get
            Return Math.Abs(CInt(IsReady)) * UShort.MaxValue - 1 * __choosenCounter
        End Get
    End Property

    ''' <summary>
    ''' Cassette not on auto-mode , and cassette sensor on/off
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsReady As Boolean
        Get
            'Return readySignal.OnTimer.TimeElapsed.Milliseconds > 100
            Return readySignal.IsSensorCovered
        End Get
    End Property
    ''' <summary>
    ''' Simulating the push button signal
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    WriteOnly Property MoveCommand As Boolean
        Set(value As Boolean)
            writeBit(___moveCommand, value)
            __choosenCounter += 1
            If __choosenCounter = Byte.MaxValue Then
                __choosenCounter = 0
            End If
        End Set
    End Property
    ReadOnly Property Position As cMotorPoint
        Get
            Return __position
        End Get
    End Property

    Protected __position As cMotorPoint = Nothing
    Protected readySignal As sensorControl = New sensorControl With {.IsEnabled = True}
    Protected ___moveCommand As ULong = 0
    Friend stopper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}

    Sub New(____postion As cMotorPoint,
            readySignal As ULong,
            moveCommand As ULong,
            Optional stopper As cylinderGeneric = Nothing)

        Me.__position = ____postion
        Me.readySignal.InputBit = readySignal
        Me.___moveCommand = moveCommand
        Me.stopper = stopper

    End Sub

    Dim __choosenCounter As Integer = 0

End Class

Public Class emptyCassetteLogistic
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public Enum conveyorMotorPositionEnum As Integer
        RECEIVE
        SEND
    End Enum


#Region "control members"
    Public shiftMotor As motorControl = New motorControl With {.IsEnabled = True}
    Public conveyorMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}
    Public slowdownSensor As sensorControl = New sensorControl
    Public reachSensor As sensorControl = New sensorControl

    Protected deliveryPort As List(Of connectionPort) = New List(Of connectionPort)
    Protected dischargePort As List(Of connectionPort) = New List(Of connectionPort)

    Protected availablePort As connectionPort = Nothing

    Protected simulatedButtonTime As TimeSpan = New TimeSpan(0, 0, 0, 0, 200)
    Protected chosenTimeout As TimeSpan = New TimeSpan(0, 0, 0, 5)

    Protected __timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 500)}

    Protected alarmPackSendFailed As alarmContextMultiSensors = New alarmContextMultiSensors With {.Sender = Me,
                                                                                                           .PossibleResponse = alarmContextBase.responseWays.RETRY}
    Protected alarmPackReceiveFailed As alarmContextMultiSensors = New alarmContextMultiSensors With {.Sender = Me,
                                                                                                           .PossibleResponse = alarmContextBase.responseWays.RETRY}
    Protected alarmPackPortOccupied As alarmContextBase = New alarmContextBase With {.Sender = Me,
                                                                                          .PossibleResponse = responseWays.RETRY,
                                                                                     .AdditionalInfo = My.Resources.PORT_OCCUPIED}
#End Region

    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState = 10
                Else
                    '--------------
                    '
                    '-------------
                End If
            Case 10
                If Not deliveryPort.TrueForAll(Function(__port As connectionPort) __port.IsReady) Then
                    deliveryPort.ForEach(Sub(__port As connectionPort) __port.stopper.drive(cylinderGeneric.cylinderCommands.GO_B_END))
                    systemSubState = 20
                Else
                    '--------------------------------
                    '   Alarm , make sure no obstacle
                    '--------------------------------
                    CentralAlarmObject.raisingAlarm(alarmPackPortOccupied)
                End If
            Case 20
                'wait all things done
                If deliveryPort.TrueForAll(Function(__port As connectionPort) __port.stopper.CommandEndStatus = IDrivable.endStatus.EXECUTION_END) Then
                    systemSubState = 30
                End If
            Case 30
                If Not slowdownSensor.IsSensorCovered And
                    Not reachSensor.IsSensorCovered Then
                    systemSubState = 40
                Else
                    '-----------------------
                    'alarm , remained things
                    '-----------------------
                    CentralAlarmObject.raisingAlarm(alarmPackSendFailed)
                End If
            Case 40
                If shiftMotor.drive(motorControl.motorCommandEnum.GO_HOME) =
                     motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 100
                End If
            Case 100
                FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                Me.IsEnabled = False
                systemMainState = systemStatesEnum.EXECUTE


        End Select

        Return 0
    End Function

    Function stateExecute() As Integer

        Select Case systemSubState
            Case 0
                ''----------------------------
                ''  Choose the highest score one
                ''----------------------------
                availablePort = deliveryPort.Find(Function(__port As connectionPort) __port.Score = deliveryPort.Max(Function(___port) ___port.Score))
                systemSubState = 10
            Case 10
                'drive there to check 
                If shiftMotor.drive(motorControl.motorCommandEnum.GO_POSITION, availablePort.Position) =
                     motorControl.statusEnum.EXECUTION_END Then

                    With __timer
                        .TimerGoal = chosenTimeout
                        .IsEnabled = True
                    End With

                    systemSubState = 15
                Else
                    '---------------
                    '
                    '---------------
                End If
            Case 15
                '--------------------------------------
                '   Wait until it is ready
                '--------------------------------------
                If availablePort.IsReady Then
                    availablePort.MoveCommand = True 'engaged
                    systemSubState = 20
                ElseIf __timer.IsTimerTicked Then
                    systemSubState = 0 'back to choose another one
                End If
            Case 20
                'release cylinder , then start driving
                If availablePort.stopper.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then

                    'point assignment, receive
                    Me.conveyorMotor.drive(motorControl.motorCommandEnum.GO_POSITION, conveyorMotorPositionEnum.RECEIVE)

                    With __timer
                        .TimerGoal = simulatedButtonTime
                        .IsEnabled = True
                    End With

                    systemSubState = 25
                End If
            Case 25
                If __timer.IsTimerTicked Then
                    availablePort.MoveCommand = False 'trigger to move
                    systemSubState = 30
                End If
            Case 30
                If Me.conveyorMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 40
                End If
            Case 40
                If slowdownSensor.IsSensorCovered And
                    reachSensor.IsSensorCovered Then
                    systemSubState = 50
                Else
                    '-----------------------
                    'alarm ,  transmission failed
                    '-----------------------
                    CentralAlarmObject.raisingAlarm(alarmPackReceiveFailed)
                End If
            Case 50
                If availablePort.stopper.drive(cylinderGeneric.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 100
                End If
                '---------------------
                '   Send Procedure
                '---------------------
            Case 100
                availablePort = dischargePort.Find(Function(__port As connectionPort) __port.Score = dischargePort.Max(Function(___port) ___port.Score))
                systemSubState = 110
            Case 110
                'drive there to check 
                If shiftMotor.drive(motorControl.motorCommandEnum.GO_POSITION, availablePort.Position) =
                     motorControl.statusEnum.EXECUTION_END Then

                    With __timer
                        .TimerGoal = chosenTimeout
                        .IsEnabled = True
                    End With

                    systemSubState = 115
                Else
                    '---------------------
                    '   On Driving
                    '---------------------
                End If
            Case 115
                If availablePort.IsReady Then
                    availablePort.MoveCommand = True    'engaged

                    With __timer
                        .TimerGoal = simulatedButtonTime
                        .IsEnabled = True
                    End With
                    systemSubState = 120

                ElseIf __timer.IsTimerTicked Then
                    systemSubState = 100 'back to choose
                End If
            Case 120
                If __timer.IsTimerTicked Then

                    availablePort.MoveCommand = False    'trigger to move

                    Me.conveyorMotor.drive(motorControl.motorCommandEnum.GO_POSITION, conveyorMotorPositionEnum.SEND)
                    systemSubState = 130
                Else
                    '------------------
                    '   Wait bouncing
                    '------------------ 
                End If
            Case 130
                If Me.conveyorMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 140
                End If
            Case 140
                If Not slowdownSensor.IsSensorCovered And
                    Not reachSensor.IsSensorCovered Then
                    systemSubState = 0 ' rewind
                Else
                    '-----------------------
                    'alarm ,  transmission failed
                    '-----------------------
                    CentralAlarmObject.raisingAlarm(alarmPackSendFailed)
                End If
        End Select

        Return 0

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub motorPause() Handles CentralAlarmObject.alarmOccured, PauseBlock.InterceptedEvent
        shiftMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        conveyorMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub motorResume() Handles CentralAlarmObject.alarmReleased, PauseBlock.UninterceptedEvent
        shiftMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        conveyorMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
    End Sub


    Function initMappingAndSetup() As Integer
        'take component of delievary/discharge port into account
        Dim allports As List(Of connectionPort) = New List(Of connectionPort)
        With allports
            .AddRange(deliveryPort.ToArray)
            .AddRange(deliveryPort.ToArray)

            .ForEach(Sub(port As connectionPort)
                         Me.actionComponents.AddRange({port.stopper})
                     End Sub)
        End With
        With alarmPackSendFailed
            .SensorConditionList.AddRange({New KeyValuePair(Of sensorControl, alarmReasonSensor)(New sensorControl With {.InputBit = slowdownSensor.InputBit}, alarmReasonSensor.SHOULD_BE_OFF),
                                           New KeyValuePair(Of sensorControl, alarmReasonSensor)(New sensorControl With {.InputBit = reachSensor.InputBit}, alarmReasonSensor.SHOULD_BE_OFF)})
        End With
        With alarmPackReceiveFailed
            .SensorConditionList.AddRange({New KeyValuePair(Of sensorControl, alarmReasonSensor)(New sensorControl With {.InputBit = slowdownSensor.InputBit}, alarmReasonSensor.SHOULD_BE_ON),
                                           New KeyValuePair(Of sensorControl, alarmReasonSensor)(New sensorControl With {.InputBit = reachSensor.InputBit}, alarmReasonSensor.SHOULD_BE_ON)})
        End With
        'for prevent interfere
        With shiftMotor
            .SlowdownEnable = enableEnum.ENABLE
            .SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
            .SlowdownLatch = sdLatchEnum.DO_NOT_LATCH
        End With
        Return 0
    End Function

    Public Sub New(delieveryPorts As List(Of connectionPort),
                   dischargePorts As List(Of connectionPort))

        '------------------------------------
        '   Port Initializing
        '------------------------------------
        Me.deliveryPort.AddRange(delieveryPorts)
        Me.dischargePort.AddRange(dischargePorts)

        Me.initialize = [Delegate].Combine(New Func(Of Integer)(AddressOf initMappingAndSetup),
                                           Me.initialize,
                                           New Func(Of Integer)(AddressOf initLinkPause),
                                           New Func(Of Integer)(AddressOf initEnableAllDrives))

        Me.systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.IGNITE
    End Sub

    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    '-----------------------------------------
    'Hsien , 2015.10.12 , the local translator
    '-----------------------------------------
    Sub translateMotorAlarm(sender As alarmManager, e As EventArgs) Handles CentralAlarmObject.alarmOccured
        If shiftMotor.IsMyAlarmCurrent AndAlso shiftMotor.ErrorStatus = errorStatusEnum.STOPPED_SD_ON Then
            sender.CurrentAlarm.PossibleResponse = alarmContextBase.responseWays.RETRY 'retry only , no need to reboot
            sender.CurrentAlarm.AdditionalInfo = My.Resources.AlarmPackInterfere
        End If
    End Sub
End Class
