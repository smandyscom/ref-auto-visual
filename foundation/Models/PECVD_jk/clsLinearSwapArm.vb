﻿Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Imports Automation.Components.Services

''' <summary>
''' 一個馬達、兩個白努力吸盤
''' 交換方式以A點為優先，即A,B同時吸到時，B手先移至A點放置，告知完成任務。A點再移至B點放置。
''' 問題: writeBit 的點位如何設定為無效?
''' 問題: 如果不看整片wafer的資訊，用interface(介面)取代，那是否有片是如何實現呢?
''' 問題: 如何搬移wafer資料?
''' </summary>
''' <remarks></remarks>
Public Class clsLinearSwapArm
    Inherits systemControlPrototype
    Implements IFinishableStation

#Region "Device declare"
    Public Property motor As motorControl = New motorControl
    ''' <summary>
    ''' gripper a is at positon a, and gripper b is at position b
    ''' </summary>
    Public Property motorPointStandby As cMotorPoint
    Public Property motorPointGripperBtoPositionA As cMotorPoint
    Public Property motorPointGripperAtoPositionB As cMotorPoint

    Public Property gripperA As New gripper
    Public Property gripperB As New gripper
    Public Property TargetPositionInfoA As Func(Of shiftDataPackBase)
    Public Property TargetPositionInfoB As Func(Of shiftDataPackBase)
    Public Property ForceSwapCondition As Func(Of Boolean) = Function() (False)
    'Public Property forceSwap As New flagController(Of interlockedFlag)
#End Region
    
#Region "External Data declare"
    Property IsAbleSwap As Func(Of Boolean)
    Property SwapFinishProcess As Func(Of Boolean) = Function() (True)
    'Property shiftDataType As Type = GetType(shiftDataPackBase) 'todo jk note: type 後 可以不用寫預設型別
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property UsingSensorAtTargetPositionB As Boolean = False
    Public Property TargetPositionBSensor As ULong
#End Region
#Region "Internal Data declare"
    Dim tmr As New singleTimer
    Dim blnFirstPickFromWeight As Boolean = True
#End Region
    Private Function ignite(ByRef cStep As igniteEnum) As Integer
        Select Case cStep
            Case igniteEnum._0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    gripperA.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END)
                    gripperB.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END)
                    writeBit(gripperA.DoVacuum, False)
                    writeBit(gripperB.DoVacuum, False)
                    cStep = igniteEnum._10
                End If
            Case igniteEnum._10
                If gripperA.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END AndAlso
                    gripperB.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    cStep = igniteEnum._20
                End If
            Case igniteEnum._20
                If motor.drive(motorControl.motorCommandEnum.GO_HOME) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = igniteEnum._30
                End If
            Case igniteEnum._30
                If motor.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointStandby) = motorControl.statusEnum.EXECUTION_END Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                    systemMainState = systemStatesEnum.EXECUTE
                End If
        End Select
        Return 0
    End Function
    Protected Function execute(ByRef cStep As executeEnum) As Integer
        Select Case cStep
            Case executeEnum._0
                If IsAbleSwap.Invoke = True Then
                    cStep = executeEnum._5
                End If
            Case executeEnum._5
                If UsingSensorAtTargetPositionB = True Then
                    If readBit(TargetPositionBSensor) = True AndAlso TargetPositionInfoB.Invoke.IsPositionOccupied = False Then
                        '有殘片在位置B
                        Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = TargetPositionBSensor,
                                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF,
                                            .AdditionalInfo = "Please set this sensor off!",
                                            .PossibleResponse = alarmContextBase.responseWays.RETRY}
                        With ap
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         Return True
                                                                                     End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    ElseIf readBit(TargetPositionBSensor) = False AndAlso TargetPositionInfoB.Invoke.IsPositionOccupied = True Then
                        '位置B有片遺失
                        Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = TargetPositionBSensor,
                                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                            .AdditionalInfo = "Please check it!",
                                            .PossibleResponse = alarmContextBase.responseWays.RETRY}
                        With ap
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         TargetPositionInfoB.Invoke.IsPositionOccupied = False
                                                                                         Return True
                                                                                     End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    Else
                        cStep = executeEnum._7
                    End If
                Else
                    '檢查抓手是否要下去吸
                    cStep = executeEnum._7
                End If
            Case executeEnum._7
                If TargetPositionInfoA.Invoke.IsPositionOccupied = True Then gripperA.cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END)
                If TargetPositionInfoB.Invoke.IsPositionOccupied = True Then gripperB.cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END)
                cStep = executeEnum._10

            Case executeEnum._10
                If gripperA.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END AndAlso
                    gripperB.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    If TargetPositionInfoA.Invoke.IsPositionOccupied = True Then writeBit(gripperA.DoVacuum, True)
                    If TargetPositionInfoB.Invoke.IsPositionOccupied = True Then writeBit(gripperB.DoVacuum, True)
                    tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(0.05 * TimeSpan.TicksPerSecond)
                    cStep = executeEnum._20
                End If
            Case executeEnum._20
                If tmr.IsTimerTicked = True Then
                    gripperA.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END)
                    gripperB.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END)
                    cStep = executeEnum._30
                End If
            Case executeEnum._30
                If gripperA.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END AndAlso
                    gripperB.cyDown.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    cStep = executeEnum._40
                End If
            Case executeEnum._40 'check is th wafer picked or not
                If TargetPositionInfoA.Invoke.IsPositionOccupied = True AndAlso readBit(gripperA.spIsOccupied) = False Then
                    Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = gripperA.spIsOccupied,
                                                          .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                                           .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE}
                    With ap
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                      TargetPositionInfoA.Invoke.IsPositionOccupied = False
                                                                                      writeBit(gripperA.DoVacuum, False)
                                                                                      Return True
                                                                                  End Function

                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                ElseIf TargetPositionInfoB.Invoke.IsPositionOccupied = True AndAlso readBit(gripperB.spIsOccupied) = False AndAlso blnFirstPickFromWeight = False Then
                    Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = gripperB.spIsOccupied,
                                                          .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                                           .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE}
                    With ap
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                      TargetPositionInfoB.Invoke.IsPositionOccupied = False
                                                                                      writeBit(gripperB.DoVacuum, False)
                                                                                      Return True
                                                                                  End Function

                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                Else 'copy target position to gripper
                    If blnFirstPickFromWeight = True Then
                        If readBit(gripperB.spIsOccupied) = False Then TargetPositionInfoB.Invoke.IsPositionOccupied = False
                    End If
                    blnFirstPickFromWeight = False
                    gripperA.shiftData.Assign(TargetPositionInfoA.Invoke)
                    gripperB.shiftData.Assign(TargetPositionInfoB.Invoke)
                    SwapFinishProcess.Invoke()
                    TargetPositionInfoA.Invoke.IsPositionOccupied = False 'clear position a occupied
                    TargetPositionInfoB.Invoke.IsPositionOccupied = False 'clear position b occupied
                    cStep = executeEnum._100
                End If


            Case executeEnum._100 'gripper b moves to position a
                If gripperB.shiftData.IsPositionOccupied = False Then 'if there is no wafer on the gripper, jump to next procedure
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                    cStep = executeEnum._200
                ElseIf motor.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointGripperBtoPositionA) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = executeEnum._110
                End If
            Case executeEnum._110
                If gripperB.cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    writeBit(gripperB.DoVacuum, False)
                    tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(0.05 * TimeSpan.TicksPerSecond)
                    cStep = executeEnum._120
                End If
            Case executeEnum._120
                If tmr.IsTimerTicked = True Then
                    If gripperB.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                        'assign gripper b data to position a
                        TargetPositionInfoA.Invoke.Assign(gripperB.shiftData)
                        TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                        cStep = executeEnum._200
                    End If
                End If
            Case executeEnum._200
                If gripperA.shiftData.IsPositionOccupied = False Then
                    cStep = executeEnum._300
                ElseIf motor.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointGripperAtoPositionB) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = executeEnum._210
                End If
            Case executeEnum._210
                If gripperA.cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    writeBit(gripperA.DoVacuum, False)
                    tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(0.05 * TimeSpan.TicksPerSecond)
                    cStep = executeEnum._220
                End If
            Case executeEnum._220
                If tmr.IsTimerTicked = True Then
                    If gripperA.cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                        TargetPositionInfoB.Invoke.Assign(gripperA.shiftData)
                        TargetPositionInfoB.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True) 'trigger outside module to work
                        cStep = executeEnum._300
                    End If
                End If
            Case executeEnum._300 'go back to standby position
                If motor.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointStandby) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = executeEnum._0
                End If
        End Select
        Return 0
    End Function
    Overridable Function initMappingAndSetup() As Integer

        '預先定義歸Home時，若進出料的位置有片，則馬達立即停止歸Home，並報警
        systemMainStateFunctions(systemStatesEnum.IGNITE) = Function() (ignite(systemSubState))
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (execute(systemSubState))
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Public Class gripper
        Public DoVacuum As ULong
        Public cyDown As IDrivable
        Public spIsOccupied As ULong
        Public shiftData As shiftDataPackBase
    End Class
    Enum igniteEnum

        _0

        _10

        _20

        _30

    End Enum
    Enum executeEnum
        _0

        _10

        _20

        _30

        _40

        _100

        _200

        _110

        _120

        _210

        _300

        _220

        _5

        _7

    End Enum
End Class
