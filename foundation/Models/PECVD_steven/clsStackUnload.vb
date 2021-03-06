﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components
Imports Automation.mainIOHardware

Public Class StackUnload : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public UnloadFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Public UpDownCylinder As New cylinderGeneric With {.IsEnabled = True} '輸送帶上下氣缸
    Public OUT_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}
    Public UD_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}

    Public UD_ConveyerReachSen As sensorControl = New sensorControl With {.IsEnabled = True} '載出堆疊冶具位置確認感測器
    Public OUT_ConveyerPosSen1 As sensorControl = New sensorControl With {.IsEnabled = True} '載出堆疊冶具位置確認感測器1,接近升降端
    Public OUT_ConveyerPosSen2 As sensorControl = New sensorControl With {.IsEnabled = True} '載出堆疊冶具位置確認感測器2,遠離升降端

    Public Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 10
                End If
            Case 10 '堆疊冶具位置感測器要為Off
                If UD_ConveyerReachSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 20
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = UD_ConveyerReachSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 10
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 20 '堆疊冶具位置感測器要為Off
                If OUT_ConveyerPosSen1.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 30
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerPosSen1.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 20
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 30
                _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)   'Hsien , 2015.04.09
                systemMainState = systemStatesEnum.EXECUTE
                systemSubState = 0
        End Select
        Return 0

    End Function
    Public Function stateExecute() As Integer

        Select Case systemSubState
            Case 0 '檢查載出堆疊冶具旗標是否致能
                UnloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, True)
                systemSubState = 10
            Case 10 '等待堆疊冶具載出
                If Not UnloadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f) Then
                    systemSubState = 20
                End If
            Case 20 '檢查載出卡匣位置確認感測器,接近升降端,不能為On(升降氣缸會撞到)
                If Not OUT_ConveyerPosSen1.IsSensorCovered Then
                    systemSubState = 40
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerPosSen1.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 20
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 40 '輸送帶氣缸往上升
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 50
                End If
            Case 50 '輸送帶開始運轉,運送堆疊冶具
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 60
                End If
            Case 60 '如果堆疊冶具保持移動直到到達感測器
                If UD_ConveyerReachSen.IsSensorCovered Then
                    systemSubState = 70
                ElseIf tmr.IsTimerTicked Then
                    systemSubState = 100
                End If
            Case 70 '檢查馬達是否停止
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 200
                End If
            Case 100 '檢查馬達是否停止
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 120
                End If
            Case 120 '產生錯誤訊息
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UD_ConveyerReachSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 50
                                                                                 Return True
                                                                             End Function

                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 200 '輸送帶往下降
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 400
                End If
            Case 400 '輸送帶開始運轉,運送堆疊冶具
                If (OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 410
                End If
            Case 410 '堆疊冶具保持移動直到到達感測器On
                If OUT_ConveyerPosSen1.IsSensorCovered Then
                    systemSubState = 450
                ElseIf tmr.IsTimerTicked Then
                    systemSubState = 420
                End If
            Case 420 '檢查馬達是否停止
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 430
                End If
            Case 430 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = OUT_ConveyerPosSen1.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 400
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 450 '檢查馬達是否停止
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 500
                End If
            Case 500 '輸送帶開始運轉,運送堆疊冶具
                If (OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                   IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 510
                End If
            Case 510 '堆疊冶具保持移動直到到達感測器Off
                If Not OUT_ConveyerPosSen1.IsSensorCovered Then
                    If OUT_ConveyerPosSen2.IsSensorCovered Then '檢查後端是否已有堆疊冶具
                        tmr.TimerGoal = New TimeSpan(0, 0, 3)
                        tmr.IsEnabled = True    'restart
                        systemSubState = 610
                    Else
                        tmr.TimerGoal = New TimeSpan(0, 0, 10)
                        tmr.IsEnabled = True    'restart
                        systemSubState = 600
                    End If

                ElseIf tmr.IsTimerTicked Then
                    systemSubState = 530
                End If
            Case 530 '檢查馬達是否停止
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 540
                End If
            Case 540 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = OUT_ConveyerPosSen1.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 500
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With


            Case 600 '檢查是否有堆疊冶具
                If OUT_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 620
                ElseIf (tmr.IsTimerTicked) Then
                    systemSubState = 620
                End If
            Case 610 '檢查延遲時間是否到達
                If tmr.IsTimerTicked Then
                    systemSubState = 620
                End If
            Case 620 '檢查馬達是否停止
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 0
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
        Return 0
    End Function
    Sub pauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        '上下卡匣輸送帶暫停
        UD_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        '載入卡匣輸送帶暫停
        OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        tmr.IsEnabled = False '時間計時暫停
    End Sub
    Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        '上下卡匣輸送帶恢復
        UD_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        '載出卡匣輸送帶恢復
        OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        tmr.IsEnabled = True '時間計時恢復
    End Sub
End Class
