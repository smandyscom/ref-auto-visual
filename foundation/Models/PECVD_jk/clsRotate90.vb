﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.mainIOHardware
Public Class clsRotate90
    Inherits systemControlPrototype
    Implements IModuleSingle
    Implements IFinishableStation

#Region "Device declare"
    Property motorRotate As motorControl = New motorControl
    Property doVg As ULong
    Property doVb As ULong
    Property diVs As ULong
#End Region
#Region "External Data declare"
    Property motorPointForward As cMotorPoint
    Property motorPointBack As cMotorPoint
    Property TargetPositionInfo As Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Property IsEnableRotate As New flagController(Of interlockedFlag)
    Property vacummBrakeTime As UInteger = 150
#End Region
#Region "Internal Data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim alarmPackSensorVacuum As alarmContentSensor = New alarmContentSensor
#End Region
    Enum stateIgniteEnum
        _0
        _10
        _20
        _30
        _40
        _100
        _110
        _120
        _200
        _300

        _5

    End Enum 'only for stateIgnite
    Enum ExcuteEnum
        _0
        _10
        _20
        _30
        _40
        _100
        _110
        _120
        _300
        _200
        _1000
    End Enum 'only for stateIgnite
    Sub alarmOccursHandler(sender As Object, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured, PauseBlock.InterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) Then
            motorRotate.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        End If
    End Sub
    Sub alarmReleaseHandler(sender As Object, e As alarmEventArgs) Handles CentralAlarmObject.alarmReleased, PauseBlock.UninterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) Then
            motorRotate.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        End If
    End Sub
    Protected Function stateIgnite() As Integer
        Select Case CType(systemSubState, stateIgniteEnum)
            Case stateIgniteEnum._0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = stateIgniteEnum._5
                End If
            Case stateIgniteEnum._5
                If motorRotate.drive(motorControl.motorCommandEnum.GO_HOME) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = stateIgniteEnum._10
                End If
            Case stateIgniteEnum._10
                If motorRotate.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointBack) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = stateIgniteEnum._20
                End If
            Case stateIgniteEnum._20
                FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
                'systemSubState = 0 '不需要，會自動歸0
        End Select
        Return 0
    End Function
    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Return 0
    End Function
    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0 '等待module action
                If TargetPositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    If TargetPositionInfo.Invoke.IsPositionOccupied = True AndAlso IsEnableRotate.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                        writeBit(doVg, True) '真空產生
                        tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(0, 0, 0, 1, 0)
                        cStep = ExcuteEnum._10
                    Else
                        cStep = ExcuteEnum._100
                    End If
                End If
            Case ExcuteEnum._10 '真空確認
                If readBit(diVs) Then
                    cStep = ExcuteEnum._20
                ElseIf tmr.IsTimerTicked = True Then '報警
                    cStep = ExcuteEnum._1000
                End If
            Case ExcuteEnum._20 '旋轉
                If motorRotate.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointForward) = motorControl.statusEnum.EXECUTION_END Then
                    writeBit(doVg, False)
                    writeBit(doVb, True)
                    tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, vacummBrakeTime)
                    cStep = ExcuteEnum._30
                End If
            Case ExcuteEnum._30
                If tmr.IsTimerTicked = True Then
                    writeBit(doVb, False)
                    TargetPositionInfo.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                    cStep = ExcuteEnum._40
                End If
            Case ExcuteEnum._40
                If motorRotate.drive(motorControl.motorCommandEnum.GO_POSITION, motorPointBack) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = ExcuteEnum._0
                End If
            Case ExcuteEnum._100
                TargetPositionInfo.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                cStep = ExcuteEnum._0

            Case ExcuteEnum._1000 'alarm occured
                alarmPackSensorVacuum.Inputs = diVs
                alarmPackSensorVacuum.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                                  systemSubState = ExcuteEnum._10
                                                                                                  Return True
                                                                                              End Function
                alarmPackSensorVacuum.CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                                   systemSubState = ExcuteEnum._20
                                                                                                   Return True
                                                                                               End Function
                CentralAlarmObject.raisingAlarm(alarmPackSensorVacuum)
        End Select
        Return 0
    End Function
    Function initMappingAndSetup()

        '預先定義報警資訊，真空無法建立時，可重試或忽略
        With alarmPackSensorVacuum
            .Sender = Me
            .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
        End With
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub

End Class
