﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Imports Automation.Components


Public Class StackWaferLift
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStation As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public liftFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Public UD_Motor As motorControl = New motorControl With {.IsEnabled = True}

    Public UD_Shell_Motor As motorControl = New motorControl With {.IsEnabled = True}

    Public UD_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True} 'Shared , Hsien , 2015.06.04 , compatible with DC/SERVO

    Public UD_ConveyerSlowDownSen As sensorControl = New sensorControl 'With {.IsEnabled = True} '卡匣減速感測器
    Public UD_ConveyerReachSen As sensorControl = New sensorControl 'With {.IsEnabled = True} '卡匣到達感測器

    Public SetCasStyle As CassetteStyle

    Public WaferReachSenCheckTime As Integer = 100
    Public BlowWaferOnWaitTime As Integer = 50
    Public BlowWaferOffWaitTime As Integer = 50
    Public blnEnableMoveDownSmallDist As Boolean = False
    Dim blnDownSmallDistOk As Boolean = False

    Dim blnWaferUpAgain As Boolean = False

    Public ConBlowSol As ULong '連續吹氣電磁閥
    Public WaferUpReachPosSen As sensorControl = New sensorControl With {.IsEnabled = True} '堆疊冶具內硅片上升到取片位置
    Public WaferExistSen As sensorControl = New sensorControl With {.IsEnabled = True} '堆疊冶具內是否存在硅片感測器


    Public Function stateIgnite() As Integer
        If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
            FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)   'Hsien , 2015.04.09
            systemMainState = systemStatesEnum.EXECUTE
        End If
        Return 0
    End Function

    Public Function stateExecute() As Integer

        Select Case systemSubState
            '---------------------------------------------------------------------------
            '                                 載入的程序
            '---------------------------------------------------------------------------
            Case 0 '檢查堆疊冶具
                If liftFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then 'Stack備便
                    If WaferExistSen.IsSensorCovered Then '檢查堆疊冶具硅片
                        systemSubState = 10
                    Else
                        sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具內無硅片!")
                        liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False)
                        liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                    End If
                ElseIf UpstreamStation.Count > 0 AndAlso
                    UpstreamStation.TrueForAll(Function(station As IFinishableStation) station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)) Then
                    '-------------------
                    '   Upstream had finished
                    '-------------------
                    Me.FinishableFlag.setFlag(IFinishableStation.controlFlags.STATION_FINISHED)
                    systemSubState = 1000   'to wait to be finished
                End If
            Case 10 '檢查硅片是否過多
                If WaferUpReachPosSen.IsSensorCovered Then '堆疊卡匣到位後,就檢查到硅片到位,代表硅片太多
                    sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具硅片過多!")
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False)
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                    systemSubState = 0
                    Return 0
                Else
                    systemSubState = 80
                End If
            Case 80 '使SD致能
                UD_Motor.SlowdownEnable = enableEnum.ENABLE '使SD致能
                UD_Motor.SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
                systemSubState = 90
            Case 90 '馬達快速度往上
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_START) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 100
                End If
            Case 100 '檢查上升馬達停止時的狀態
                '------------------------------------------------------------
                '硅片到位感測器SD,+上升到底為會碰到下dog片,回原點下降會上dog片
                '------------------------------------------------------------
                If (UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END) Then
                    If (UD_Motor.ErrorStatus And errorStatusEnum.STOPPED_EL_PLUS) Then '上升超過+極限
                        sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具內硅片上升到達正極限感測器!")
                        UD_Motor.SlowdownEnable = enableEnum.DISABLE '使SD去能
                        systemSubState = 500
                    ElseIf (UD_Motor.ErrorStatus And errorStatusEnum.STOPPED_SD_ON) Then '馬達SD停止
                        UD_Motor.SlowdownEnable = enableEnum.DISABLE '使SD去能
                        systemSubState = 110
                    Else
                        sendMessage(statusEnum.GENERIC_MESSAGE, "馬達快速上升無法到達感測器!" & UD_Motor.ErrorStatus)
                        Dim ap As alarmContentSensor = New alarmContentSensor With {.Sender = Me, .Inputs = WaferUpReachPosSen.InputBit}
                        With ap
                            ap.PossibleResponse = alarmContextBase.responseWays.RETRY
                            ap.AdditionalInfo = "馬達快速上升無法到達感測器"
                            ap.Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                            ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                           systemSubState = 80
                                                                                           Return True
                                                                                       End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    End If
                End If
            Case 110 '馬達下降一距離
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_SPACE) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 120
                End If
            Case 120 '使SD致能
                UD_Motor.SlowdownEnable = enableEnum.ENABLE '使SD致能
                systemSubState = 130
            Case 130 '馬達慢升一距離
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_SUB_SPACE) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 150
                End If
            Case 150 '檢查上升馬達停止時的狀態
                '------------------------------------------------------------
                '硅片到位感測器SD,+上升到底為會碰到下dog片,回原點下降會上dog片
                '------------------------------------------------------------
                If (UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END) Then
                    If (UD_Motor.ErrorStatus And errorStatusEnum.STOPPED_EL_PLUS) Then '上升超過+極限
                        sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具內硅片上升到達正極限感測器!")
                        UD_Motor.SlowdownEnable = enableEnum.DISABLE '使SD去能
                        systemSubState = 500
                    ElseIf (UD_Motor.ErrorStatus And errorStatusEnum.STOPPED_SD_ON) Then '馬達SD停止
                        sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具內硅片上升到位!")
                        Call writeBit(ConBlowSol, True) '到位後預吹開啟
                        UD_Motor.SlowdownEnable = enableEnum.DISABLE '使SD去能
                        systemSubState = 160
                    Else
                        sendMessage(statusEnum.GENERIC_MESSAGE, "馬達慢速上升無法到達感測器!" & UD_Motor.ErrorStatus)
                        Dim ap As alarmContentSensor = New alarmContentSensor With {.Sender = Me, .Inputs = WaferUpReachPosSen.InputBit}
                        With ap
                            ap.PossibleResponse = alarmContextBase.responseWays.RETRY
                            ap.AdditionalInfo = "馬達慢速上升無法到達感測器"
                            ap.Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                            ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                           systemSubState = 120
                                                                                           Return True
                                                                                       End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    End If
                End If
                '---------------------------------------------------------------------------
            Case 160 '第一次馬達下降一小段距離
                If Not blnDownSmallDistOk And blnEnableMoveDownSmallDist Then
                    If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_WAFER_DOWN) = motorControl.statusEnum.EXECUTION_END Then
                        blnDownSmallDistOk = True
                        systemSubState = 200
                    End If
                Else
                    systemSubState = 200
                End If
                '---------------------------------------------------------------------------
                '                                 連續供料程序
                '---------------------------------------------------------------------------
            Case 200 '檢查極板是否到位
                If Not liftFlags.viewFlag(flagsInLoaderUnloader.WaferReadyToPick_f) Then  '等待極板吸走
                    If liftFlags.viewFlag(flagsInLoaderUnloader.ChangeStack_f) Then
                        systemSubState = 500
                    Else
                        tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, WaferReachSenCheckTime) '檢查硅片有無到達感測器的時間設定
                        tmr.IsEnabled = True    'restart
                        systemSubState = 210
                    End If
                Else '檢查極板是否吸取中,及換堆疊冶具狀況
                    If Not liftFlags.viewFlag(flagsInLoaderUnloader.PickWaferInProc_f) And liftFlags.viewFlag(flagsInLoaderUnloader.ChangeStack_f) Then
                        liftFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, False)
                        systemSubState = 500
                    End If
                End If
            Case 210 '檢查硅片是否在吸取位置上
                If WaferUpReachPosSen.IsSensorCovered Then '檢查堆疊卡匣硅片 
                    Call writeBit(ConBlowSol, True) '預吹氣開啟
                    If blnWaferUpAgain Then
                        tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, BlowWaferOffWaitTime) '檢查硅片有無到達感測器的時間設定
                        tmr.IsEnabled = True    'restart
                        systemSubState = 230
                        blnWaferUpAgain = False
                    Else
                        tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, BlowWaferOnWaitTime) '檢查硅片有無到達感測器的時間設定
                        tmr.IsEnabled = True    'restart
                        systemSubState = 230
                    End If
                Else
                    If tmr.IsTimerTicked Then '檢查時間是否到達
                        blnWaferUpAgain = True
                        systemSubState = 120
                    End If
                End If
            Case 230
                If tmr.IsTimerTicked Then '檢查時間是否到達
                    systemSubState = 240
                End If
            Case 240
                If liftFlags.viewFlag(flagsInLoaderUnloader.ChangeStack_f) Then '先檢查換冶具旗標
                    systemSubState = 500
                ElseIf Not liftFlags.viewFlag(flagsInLoaderUnloader.WaferReadyToPick_f) Then '檢查Cell是否放置完成
                    liftFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, True) '吸嘴可以取硅片
                    systemSubState = 200
                End If
                '---------------------------------------------------------------------------
                '                                 退堆疊卡匣程序
                '---------------------------------------------------------------------------
            Case 500 '預吹氣關閉、換料黃色按鈕燈滅、旗標重置,退冶具
                liftFlags.writeFlag(flagsInLoaderUnloader.ChangeStack_f, False)
                liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False)
                liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)

                Call writeBit(ConBlowSol, False) '預吹氣關閉
                liftFlags.writeFlag(flagsInLoaderUnloader.FirstTimeToSuck_f, False) '設定第一次吸取旗標
                blnDownSmallDistOk = False

                systemSubState = 0
                '-------------------------------------------------------------
                '   FINISHED , wait to be released
                '-------------------------------------------------------------
            Case 1000

                'keep releasing current stack
                liftFlags.readFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)

                If UpstreamStation.Count > 0 AndAlso
                    UpstreamStation.TrueForAll(Function(station As IFinishableStation) Not station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)) Then
                    '-----------------
                    '   Upstream had reset finish flag
                    '-----------------
                    Me.FinishableFlag.resetFlag(IFinishableStation.controlFlags.STATION_FINISHED)

                    systemSubState = 0 'rewind
                End If
        End Select

        Return 0
    End Function

    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態

        With UD_Motor
            .AcceptableErrorStatus = errorStatusEnum.STOPPED_SD_ON Or errorStatusEnum.STOPPED_EL_PLUS
        End With

        Return 0
    End Function
End Class


