﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components

Public Class StackLoad : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Enum CheckStackStyleSenIndex
        SEN_1
        SEN_2
    End Enum
    Public Enum BasePosition
        LOAD
        START
        UNLOAD
    End Enum
    Public Enum StackFlow
        LOAD
        UNLOAD
        UNLOAD_LOAD
    End Enum
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Event CassetteRemoved(ByVal sender As Object, ByVal e As EventArgs)  'Cassette is removed from lifter
    Public Event CassetteRejected(ByVal sender As Object, ByVal e As EventArgs) 'Cassette is ejected by User
    Public cassetteHadArrived As Func(Of Boolean) = Function() (True)        ' occured when cassette had arrived UD_conveyor , return value , true : action done
    Public extensionSequence As Func(Of Boolean) = Function() (True)     'Hsien , used to attach extension sequence , i.e read RFID on ready position
    Public loadFlags As flagController(Of flagsInLoaderUnloader)

    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}


    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen
    Public IN_Stopper1 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public IN_Stopper2 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public IN_Stopper3 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public IN_Stopper4 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}

    Public UD_Motor As New motorControl

    Public IN_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}   '載出馬達 , Hsien , regular the data type , 2015.06.04

    Public UpDownCylinder As New cylinderGeneric With {.IsEnabled = True} '堆疊冶具頂升氣缸
    Public UpDownStackPosSen As sensorControl = New sensorControl With {.IsEnabled = True}


    Public IN_ConveyerPosSen1 As sensorControl = New sensorControl With {.IsEnabled = True} '載入堆疊冶具位置確認感測器1    'on Loading Side
    Public IN_ConveyerPosSen2 As sensorControl = New sensorControl With {.IsEnabled = True} '載入堆疊冶具位置確認感測器2    'on Stopper 1
    Public IN_ConveyerPosSen3 As sensorControl = New sensorControl With {.IsEnabled = True} '載入堆疊冶具位置確認感測器3    'on Stopper 2
    Public IN_ConveyerPosSen4 As sensorControl = New sensorControl With {.IsEnabled = True} '載入堆疊冶具位置確認感測器3    'on Stopper 3

    Public OUT_ConveyeReachSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Public In_ConveyeReachSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Public OUT_ConveyeLeaveSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Public IN_ConveyerOverrideSen As sensorControl = New sensorControl With {.IsEnabled = True} '載入堆疊冶具位置防呆確認感測器 , the lifter side
    Public OUT_ConveyerOverrideSen As sensorControl = New sensorControl With {.IsEnabled = True} '載出堆疊冶具位置防呆確認感測器 , away from lifter

    Public SetCasStyle As CassetteStyle = CassetteStyle.MANZ

    Public IN_CasStyleCheckSen As SensorInfo() = New SensorInfo(1) {New SensorInfo With {.sw = IS_OFF}, New SensorInfo With {.sw = IS_OFF}}
    Private SensorIndex As Integer

    Private StackProcess As StackFlow

    Private GoToPosition_Step As Integer
    Private NowManzCstPos As Integer
    Private blnRejectStack As Boolean

    Public Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 20
                End If
            Case 20 '馬達回原點
                If (UD_Motor.drive(motorControl.motorCommandEnum.GO_HOME, LiftMotorUsedPositions.MOTOR_HOME) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_APPROCH
                    systemSubState = 30
                End If
            Case 30 '平台下降
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 70
                End If
            Case 70 '堆疊冶具位置感測器Sen2要為Off
                If IN_ConveyerPosSen2.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 80
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = IN_ConveyerPosSen2.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 70
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 80 '堆疊冶具位置感測器Sen3要為Off
                If IN_ConveyerPosSen3.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 90
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = IN_ConveyerPosSen3.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 80
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 90 '堆疊冶具位置感測器Sen4要為Off
                If OUT_ConveyeLeaveSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 300
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyeLeaveSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 90
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 300 '檢查防呆感測器
                If IN_ConveyerOverrideSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 310
                Else '產生錯誤
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = IN_ConveyerOverrideSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 300
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 310 '檢查防呆感測器
                If OUT_ConveyerOverrideSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 320
                Else '產生錯誤
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerOverrideSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 310
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 320 '使擋堆疊冶具氣缸上升
                If IN_Stopper2.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 330
                End If
            Case 330 '輸送帶開始移動料匣,如有堆疊冶具使其接近感測器
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 340
                End If
            Case 340 '延遲一段時間
                If (tmr.IsTimerTicked) Then
                    systemSubState = 350
                End If
            Case 350 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 500
                End If
            Case 500 '檢查堆疊冶具是否存在
                If In_ConveyeReachSen.IsSensorCovered Then
                    loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                    systemSubState = 560
                Else
                    systemSubState = 560
                End If
            Case 560 '完成
                _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)   'Hsien , 2015.04.09
                systemMainState = systemStatesEnum.EXECUTE
                systemSubState = 0
        End Select

        Return 0
    End Function
    Public Function stateExecute1() As Integer

        '==========================================================
        ' Cassette Direction   >>>>>>>>>>>>>
        '           Stopper1     Stopper2     Stopper3     
        'Sen1           Sen2         Sen3     CheckSen    LeaveSen
        '==========================================================
        '初始狀態     伸出        縮回  工作位    伸出 

        Select Case systemSubState

            Case 0 '檢查堆疊冶具是否備便及是否已啟動
                If loadFlags.viewFlag(flagsInLoaderUnloader.Start_f) Then
                    If (Not loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)) And _
                        (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then
                        If loadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f) Then '是否要作退堆疊冶具
                            loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                            systemSubState = 400 '至載出堆疊冶具程序
                        Else
                            If loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) And _
                                 (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then '堆疊冶具是否在輸送帶上備便
                                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                                StackProcess = StackFlow.LOAD
                                systemSubState = 800 '至載入堆疊冶具程序
                            Else
                                systemSubState = 90
                            End If
                        End If
                    Else
                        systemSubState = 90
                    End If
                End If
                '-------------------------------------------------------
                '               檢查Sen2是否有堆疊冶具
                '-------------------------------------------------------
            Case 90 '檢查Sen2是否有堆疊冶具
                If IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 100
                Else
                    systemSubState = 300 '檢查其它位置是否有堆疊冶具
                End If
            Case 100 '檢查堆疊冶具在輸送帶上是否備便
                If Not loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) And _
                  (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then
                    loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                    systemSubState = 110
                Else
                    systemSubState = 0
                End If
                '-------------------------------------------------------
                '              堆疊冶具進入備便位置程序
                '-------------------------------------------------------
            Case 110 '使氣缸下降
                If IN_Stopper1.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 120
                End If
            Case 120 '輸送帶開始移動料匣
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 130
                End If
            Case 130 '檢查Sen3是否有堆疊冶具
                If IN_ConveyerPosSen3.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 160
                ElseIf (tmr.IsTimerTicked) Then
                    systemSubState = 140
                End If
            Case 140 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 150
                End If
            Case 150 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = IN_ConveyerPosSen3.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 120
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 160 '延遲一段時輸送帶馬達停止
                If tmr.IsTimerTicked Then
                    systemSubState = 165
                End If
            Case 165 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 170
                End If
            Case 170 '使氣缸上升
                If IN_Stopper1.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 180
                End If
            Case 180
                If (extensionSequence()) Then
                    systemSubState = 190
                End If
            Case 190
                loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, True)
                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
                systemSubState = 0
                '-------------------------------------------------------
                '               檢查Sen1是否有堆疊冶具
                '-------------------------------------------------------

            Case 300 '檢查Sen1是否有堆疊冶具
                If IN_ConveyerPosSen1.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                    systemSubState = 310
                Else
                    systemSubState = 0
                End If
            Case 310 '輸送帶開始移動料匣
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    If IN_ConveyerPosSen2.IsSensorCovered Then '檢查Sen2目前是否有堆疊冶具
                        tmr.TimerGoal = New TimeSpan(0, 0, 3)
                        tmr.IsEnabled = True    'restart
                        systemSubState = 330
                    Else
                        tmr.TimerGoal = New TimeSpan(0, 0, 10)
                        tmr.IsEnabled = True    'restart
                        systemSubState = 320
                    End If
                End If
            Case 320 '檢查Sen2是否有卡匣到達
                If IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 340
                ElseIf (tmr.IsTimerTicked) Then
                    systemSubState = 340
                End If
            Case 330 '使輸送帶馬達走一小段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 340
                End If
            Case 340 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
                    systemSubState = 0
                End If
                '-------------------------------------------------------
                '                      退堆疊冶具程序
                '-------------------------------------------------------
            Case 400 '升降馬達移動至開始位置
                If GoToBasePosition(BasePosition.UNLOAD) Then
                    systemSubState = 410
                End If
            Case 410 '平台下降
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 460
                End If
            Case 460 '檢查堆疊冶具是否下降
                If Not UpDownStackPosSen.IsSensorCovered Then
                    systemSubState = 480
                Else
                    systemSubState = 470
                End If
            Case 470 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UpDownStackPosSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 460
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 480 '使擋堆疊冶具氣缸下降
                If IN_Stopper3.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 490
                End If
            Case 490 '檢查是否可以載出卡匣
                If loadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f) Then
                    systemSubState = 500
                End If
            Case 500 '決定堆疊冶具載入及移出的程序
                If loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) Then
                    StackProcess = StackFlow.UNLOAD_LOAD
                    systemSubState = 800 '作堆疊冶具移出同時載入堆疊冶具
                Else
                    StackProcess = StackFlow.UNLOAD
                    systemSubState = 510 '只作堆疊冶具移出
                End If
                '-------------------------------------------------------
                '   到達OUT_ConveyeLeaveSen 及 OUT_ConveyeReachSen程序
                '-------------------------------------------------------
            Case 510 '輸送帶開始移動堆疊冶具
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 520
                End If
            Case 520 '檢查到位感測器
                If OUT_ConveyeLeaveSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 570
                ElseIf (tmr.IsTimerTicked) Then
                    systemSubState = 550
                End If
            Case 550 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 560
                End If
            Case 560 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = OUT_ConveyeLeaveSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 510
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 570 '使擋堆疊冶具氣缸上升
                If IN_Stopper3.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 580
                End If
            Case 580 '檢查到位感測器
                If OUT_ConveyeReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 700
                ElseIf (tmr.IsTimerTicked) Then
                    systemSubState = 590
                End If
            Case 590 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 600
                End If
            Case 600 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = OUT_ConveyeReachSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 610
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 610 '輸送帶開始移動料匣
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 580
                End If
            Case 700 '延遲一時間
                If tmr.IsTimerTicked Then
                    systemSubState = 710
                End If
            Case 710 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 720
                End If
            Case 720 '檢查防呆感測器
                If Not OUT_ConveyerOverrideSen.IsSensorCovered Then
                    loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False)
                    loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, False)
                    loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, False)
                    RaiseEvent CassetteRemoved(Me, Nothing)    'Hsien , 2015.05.21 , cassette removed any way
                    Select Case StackProcess
                        Case StackFlow.UNLOAD
                            ' '重置旗標
                            systemSubState = 0 '單獨堆疊冶具載出完成
                        Case StackFlow.UNLOAD_LOAD
                            tmr.TimerGoal = New TimeSpan(0, 0, 1)
                            tmr.IsEnabled = True    'restart
                            systemSubState = 820 '檢查卡匣載入狀況
                    End Select
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerOverrideSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 720
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
                '-------------------------------------------------------
            Case 800 '使擋堆疊冶具氣缸下降
                If IN_Stopper2.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    Select Case StackProcess
                        Case StackFlow.LOAD
                            systemSubState = 810 '單獨載入堆疊冶具
                        Case StackFlow.UNLOAD_LOAD
                            systemSubState = 510 '輸送帶開始移動堆疊冶具
                    End Select
                End If
                '-------------------------------------------------------
                '                 檢查堆疊冶具載入狀況
                '-------------------------------------------------------
            Case 810 '輸送帶開始移動料匣
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 820
                End If
                '-------------------------------------------------------
            Case 820 '檢查是否到達工作區
                If In_ConveyeReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    systemSubState = 830
                Else
                    If tmr.IsTimerTicked Then
                        systemSubState = 825
                    End If
                End If
            Case 825 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = In_ConveyeReachSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 870
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 830 '指定檢查感測器
                SensorIndex = CheckStackStyleSenIndex.SEN_1
                systemSubState = 835
            Case 835 '判別感測器是否要檢查On或是Off
                If (IN_CasStyleCheckSen(SensorIndex).sw = IS_ON) Then
                    systemSubState = 840
                Else
                    systemSubState = 880
                End If
            Case 840 '檢查料匣形式防呆感測器
                If IN_CasStyleCheckSen(SensorIndex).status = IS_ON And IN_CasStyleCheckSen(SensorIndex).sensor.IsSensorCovered Then
                    systemSubState = 880
                ElseIf IN_CasStyleCheckSen(SensorIndex).status = IS_OFF And (Not IN_CasStyleCheckSen(SensorIndex).sensor.IsSensorCovered) Then
                    systemSubState = 880
                Else
                    If tmr.IsTimerTicked Then
                        systemSubState = 850
                    End If
                End If
            Case 850 '輸送帶馬達停止,重新檢查感測器
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 860
                End If
            Case 860 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = IN_CasStyleCheckSen(SensorIndex).sensor.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    If IN_CasStyleCheckSen(SensorIndex).status = IS_ON Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    If IN_CasStyleCheckSen(SensorIndex).status = IS_OFF Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 870
                                                                                 Return True
                                                                             End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  systemSubState = 900
                                                                                  blnRejectStack = True
                                                                                  Return True
                                                                              End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 870 '輸送帶開始移動料匣
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                        IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 820
                End If
            Case 880 '檢查下一個感測器
                SensorIndex = SensorIndex + 1
                If SensorIndex > [Enum].GetValues(GetType(CheckStackStyleSenIndex)).Length - 1 Then
                    systemSubState = 890
                Else
                    systemSubState = 835
                End If
            Case 890 '輸送帶馬達停止,重新檢查感測器
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 900
                End If
            Case 900 '檢查防呆感測器
                If Not IN_ConveyerOverrideSen.IsSensorCovered Then
                    systemSubState = 920
                Else '產生錯誤
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = IN_ConveyerOverrideSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 900
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 920 '使擋堆疊冶具氣缸上升
                If IN_Stopper2.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    If blnRejectStack Then '堆疊冶具不正確強制退出
                        blnRejectStack = False
                        loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
                        loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                        loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, False)
                        systemSubState = 0
                    Else
                        systemSubState = 930
                    End If
                End If
            Case 930 '平台上升
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 940
                End If
            Case 940 '升降馬達移動至開始位置
                If GoToBasePosition(BasePosition.START) Then
                    loadFlags.setFlag(flagsInLoaderUnloader.IsCassetteStyleAcceptable_f)
                    systemSubState = 950
                End If
            Case 950 '告知外部控卡匣已備便
                If (cassetteHadArrived()) Then
                    systemSubState = 960
                End If
            Case 960
                If (loadFlags.viewFlag(flagsInLoaderUnloader.IsCassetteStyleAcceptable_f)) Then '外部控制回應卡匣工作
                    systemSubState = 980
                Else '外部控制回應卡匣退出
                    'rejected
                    systemSubState = 970
                End If
            Case 970
                loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                RaiseEvent CassetteRejected(Me, Nothing) 'Hsien , 2015.05.21 , cassette ejected by Host
                systemSubState = 0
            Case 980 '設定堆疊冶具備便
                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
                loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, True)
                loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, False)
                systemSubState = 0
        End Select

        Return 0
    End Function
    'Public Function stateExecute2() As Integer

    '    '==========================================================
    '    ' Cassette Direction   >>>>>>>>>>>>>
    '    '           Stopper1     Stopper2     Stopper3     Stopper3
    '    'Sen1           Sen2         Sen3         Sen4     CheckSen
    '    '==========================================================
    '    '初始狀態      伸出          伸出       縮回  工作位 伸出

    '    Select Case systemSubState

    '        Case 0 '檢查堆疊冶具是否備便及是否已啟動
    '            Dim blntest As Boolean
    '            blntest = loadFlags.viewFlag(flagsInLoaderUnloader.Start_f)
    '            If (Not loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)) And _
    '               loadFlags.viewFlag(flagsInLoaderUnloader.Start_f) And _
    '               (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then
    '                If loadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f) Then '是否要作退堆疊冶具
    '                    loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
    '                    systemSubState = 400 '至載出堆疊冶具程序
    '                Else
    '                    If loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) And _
    '                         (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then '堆疊冶具是否在輸送帶上備便
    '                        loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
    '                        systemSubState = 600 '至載入堆疊冶具程序
    '                    Else
    '                        systemSubState = 90
    '                    End If
    '                End If
    '            Else
    '                systemSubState = 90
    '            End If
    '            '-------------------------------------------------------
    '            '               檢查Sen3是否有堆疊冶具
    '            '-------------------------------------------------------
    '        Case 90 '檢查Sen3是否有堆疊冶具
    '            If IN_ConveyerPosSen3.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                systemSubState = 100
    '            Else
    '                systemSubState = 200 '檢查其它位置是否有堆疊冶具
    '            End If
    '        Case 100 '檢查堆疊冶具在輸送帶上是否備便
    '            If Not loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) And _
    '              (Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f)) Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
    '                systemSubState = 110
    '            Else
    '                If IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                    systemSubState = 0
    '                Else
    '                    systemSubState = 300 '檢查其它位置是否有堆疊冶具
    '                End If
    '            End If
    '            '-------------------------------------------------------
    '            '              堆疊冶具進入備便位置程序
    '            '-------------------------------------------------------
    '        Case 110 '使氣缸下降
    '            If IN_Stopper2.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 120
    '            End If
    '        Case 120 '輸送帶開始移動料匣
    '            If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
    '                 IDrivable.endStatus.EXECUTION_END) Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 6)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 130
    '            End If
    '        Case 130 '檢查Sen4是否有堆疊冶具
    '            If IN_ConveyerPosSen4.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                systemSubState = 160
    '            ElseIf (tmr.IsTimerTicked) Then
    '                systemSubState = 140
    '            End If
    '        Case 140 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 150
    '            End If
    '        Case 150 '產生錯誤
    '            Dim ap As New alarmContentSensor
    '            With ap
    '                .Sender = Me
    '                .Inputs = IN_ConveyerPosSen4.InputBit
    '                .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
    '                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                             systemSubState = 120
    '                                                                             Return True
    '                                                                         End Function
    '                CentralAlarmObject.raisingAlarm(ap)
    '            End With
    '        Case 160 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 170
    '            End If
    '        Case 170 '使氣缸上升
    '            If IN_Stopper2.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, True)
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
    '                systemSubState = 0
    '            End If
    '            '-------------------------------------------------------
    '            '               檢查Sen2是否有堆疊冶具
    '            '-------------------------------------------------------
    '        Case 200 '檢查Sen2是否有堆疊冶具
    '            If IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
    '                systemSubState = 210
    '            Else
    '                systemSubState = 300 '檢查其它位置是否有堆疊冶具
    '            End If
    '        Case 210 '使氣缸下降
    '            If IN_Stopper1.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 220
    '            End If
    '        Case 220 '輸送帶開始移動料匣
    '            If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
    '                 IDrivable.endStatus.EXECUTION_END) Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 10)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 230
    '            End If
    '        Case 230 '檢查Sen3是否有堆疊冶具
    '            If IN_ConveyerPosSen3.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                systemSubState = 260
    '            ElseIf (tmr.IsTimerTicked) Then
    '                systemSubState = 240
    '            End If
    '        Case 240 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 250
    '            End If
    '        Case 250 '產生錯誤
    '            Dim ap As New alarmContentSensor
    '            With ap
    '                .Sender = Me
    '                .Inputs = IN_ConveyerPosSen3.InputBit
    '                .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
    '                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                             systemSubState = 220
    '                                                                             Return True
    '                                                                         End Function
    '                CentralAlarmObject.raisingAlarm(ap)
    '            End With
    '        Case 260 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 270
    '            End If
    '        Case 270 '使氣缸上升
    '            If IN_Stopper1.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
    '                systemSubState = 0
    '            End If
    '            '-------------------------------------------------------
    '            '               檢查Sen1是否有堆疊冶具
    '            '-------------------------------------------------------

    '        Case 300 '檢查Sen1是否有堆疊冶具
    '            If IN_ConveyerPosSen1.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
    '                systemSubState = 310
    '            Else
    '                systemSubState = 0
    '            End If
    '        Case 310 '輸送帶開始移動料匣
    '            If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
    '                 IDrivable.endStatus.EXECUTION_END) Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 6)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 320
    '            End If
    '        Case 320 '檢查Sen2是否有卡匣到達
    '            If IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                systemSubState = 330
    '            ElseIf (tmr.IsTimerTicked) Then
    '                systemSubState = 330
    '            End If
    '        Case 330 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
    '                systemSubState = 0
    '            End If
    '            '-------------------------------------------------------
    '            '                      退堆疊冶具程序
    '            '-------------------------------------------------------
    '        Case 400 '升降馬達移動至開始位置
    '            If GoToBasePosition(BasePosition.UNLOAD) Then
    '                systemSubState = 410
    '            End If
    '        Case 410 '平台下降
    '            If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 460
    '            End If
    '        Case 460 '檢查堆疊冶具是否下降
    '            If UpDownStackPosSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                systemSubState = 480
    '            Else
    '                systemSubState = 470
    '            End If
    '        Case 470 '產生錯誤
    '            Dim ap As New alarmContentSensor
    '            With ap
    '                .Sender = Me
    '                .Inputs = UpDownStackPosSen.InputBit
    '                .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
    '                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                             systemSubState = 460
    '                                                                             Return True
    '                                                                         End Function
    '                CentralAlarmObject.raisingAlarm(ap)
    '            End With
    '        Case 480 '使擋堆疊冶具氣缸下降
    '            If IN_Stopper4.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 490
    '            End If
    '        Case 490 '檢查是否可以載出卡匣
    '            If loadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f) Then
    '                systemSubState = 500
    '            End If
    '        Case 500 '輸送帶開始移動料匣
    '            If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
    '                 IDrivable.endStatus.EXECUTION_END) Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 6)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 510
    '            End If
    '        Case 510 '檢查到位感測器
    '            If OUT_ConveyeReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 1)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 565
    '            ElseIf (tmr.IsTimerTicked) Then
    '                systemSubState = 550
    '            End If
    '        Case 550 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 560
    '            End If
    '        Case 560 '產生錯誤
    '            Dim ap As New alarmContentSensor
    '            With ap
    '                .Sender = Me
    '                .Inputs = OUT_ConveyeReachSen.InputBit
    '                .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
    '                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                             systemSubState = 500
    '                                                                             Return True
    '                                                                         End Function
    '                CentralAlarmObject.raisingAlarm(ap)
    '            End With
    '        Case 565 '延遲一時間
    '            If tmr.IsTimerTicked Then
    '                systemSubState = 570
    '            End If
    '        Case 570 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 580
    '            End If
    '        Case 580 '檢查防呆感測器
    '            If Not OUT_ConveyerOverrideSen.IsSensorCovered Then
    '                systemSubState = 590
    '            Else
    '                Dim ap As New alarmContentSensor
    '                With ap
    '                    .Sender = Me
    '                    .Inputs = OUT_ConveyerOverrideSen.InputBit
    '                    .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
    '                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                                 systemSubState = 580
    '                                                                                 Return True
    '                                                                             End Function
    '                    CentralAlarmObject.raisingAlarm(ap)
    '                End With
    '            End If
    '        Case 590 '使擋堆疊冶具氣缸上升
    '            If IN_Stopper4.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
    '                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False)
    '                loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, False)
    '                loadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, False)
    '                systemSubState = 0
    '            End If
    '            '-------------------------------------------------------
    '            '                      進堆疊冶具程序
    '            '-------------------------------------------------------
    '        Case 600 '使擋堆疊冶具氣缸下降
    '            If IN_Stopper3.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 610
    '            End If
    '        Case 610 '輸送帶開始移動料匣
    '            If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
    '                 IDrivable.endStatus.EXECUTION_END) Then
    '                tmr.TimerGoal = New TimeSpan(0, 0, 3)
    '                tmr.IsEnabled = True    'restart
    '                systemSubState = 620
    '            End If
    '        Case 620
    '            SensorIndex = CheckStackStyleSenIndex.SEN_1
    '            systemSubState = 630
    '        Case 630 '判別感測器是否要檢查On或是Off
    '            If (IN_CasStyleCheckSen(SensorIndex).sw = IS_ON) Then
    '                systemSubState = 640
    '            Else
    '                systemSubState = 680
    '            End If
    '        Case 640 '檢查料匣形式防呆感測器
    '            If IN_CasStyleCheckSen(SensorIndex).status = IS_ON And IN_CasStyleCheckSen(SensorIndex).sensor.IsSensorCovered Then
    '                systemSubState = 680
    '            ElseIf IN_CasStyleCheckSen(SensorIndex).status = IS_OFF And (Not IN_CasStyleCheckSen(SensorIndex).sensor.IsSensorCovered) Then
    '                systemSubState = 680
    '            Else
    '                If tmr.IsTimerTicked Then
    '                    systemSubState = 650
    '                End If
    '            End If
    '        Case 650 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 670
    '            End If
    '        Case 670 '產生錯誤
    '            Dim ap As New alarmContentSensor
    '            With ap
    '                .Sender = Me
    '                .Inputs = IN_CasStyleCheckSen(SensorIndex).sensor.InputBit
    '                .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                If IN_CasStyleCheckSen(SensorIndex).status = IS_ON Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
    '                If IN_CasStyleCheckSen(SensorIndex).status = IS_OFF Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
    '                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                             systemSubState = 610
    '                                                                             Return True
    '                                                                         End Function
    '                CentralAlarmObject.raisingAlarm(ap)
    '            End With
    '        Case 680 '檢查下一個感測器
    '            SensorIndex = SensorIndex + 1
    '            If SensorIndex > [Enum].GetValues(GetType(CheckStackStyleSenIndex)).Length - 1 Then
    '                systemSubState = 690
    '            Else
    '                systemSubState = 630
    '            End If
    '        Case 690 '輸送帶馬達停止
    '            If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 700
    '            End If
    '        Case 700 '檢查防呆感測器
    '            If Not IN_ConveyerOverrideSen.IsSensorCovered Then
    '                systemSubState = 720
    '            Else '產生錯誤
    '                Dim ap As New alarmContentSensor
    '                With ap
    '                    .Sender = Me
    '                    .Inputs = IN_ConveyerOverrideSen.InputBit
    '                    .PossibleResponse = alarmContextBase.responseWays.RETRY
    '                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
    '                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
    '                                                                                 systemSubState = 700
    '                                                                                 Return True
    '                                                                             End Function
    '                    CentralAlarmObject.raisingAlarm(ap)
    '                End With
    '            End If
    '        Case 720 '使擋堆疊冶具氣缸上升
    '            If IN_Stopper3.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 730
    '            End If
    '        Case 730 '平台上升
    '            If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
    '                systemSubState = 740
    '            End If
    '        Case 740 '升降馬達移動至開始位置
    '            If GoToBasePosition(BasePosition.START) Then
    '                systemSubState = 750
    '            End If
    '        Case 750 '設定堆疊冶具備便
    '            loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
    '            loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, True)
    '            loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, False)
    '            systemSubState = 0
    '    End Select

    '    Return 0
    'End Function

    Function GoToBasePosition(ByVal basePos As BasePosition) As Boolean
        Select Case GoToPosition_Step
            Case 0
                Select Case basePos
                    Case BasePosition.START
                        GoToPosition_Step = 200
                    Case BasePosition.UNLOAD
                        GoToPosition_Step = 300
                End Select
                '-----------------------------------Manz Cassette
            Case 200 'MANZ(使馬達運行)===(Start)
                UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH) '頂升靠近內部卡匣
                GoToPosition_Step = 230
            Case 210 '檢查馬達是否停止
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_START) '頂升內部卡匣到舌頭工作位置
                    GoToPosition_Step = 230
                End If
            Case 230 '檢查馬達是否停止
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_APPROCH
                    GoToPosition_Step = 0
                    Return True
                End If
            Case 300 'MANZ(使馬達運行)===(Unload)
                If NowManzCstPos <> LiftMotorUsedPositions.MOTOR_MANZ_WAIT Then '如果內卡匣不在Wait位置,強制退卡匣時,內卡已在此位置就不用再作動
                    UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH)
                    GoToPosition_Step = 310
                Else
                    GoToPosition_Step = 330
                End If
            Case 310 '檢查內卡匣是否到逹Approch位置
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_WAIT) '內卡匣移至Wait位置
                    GoToPosition_Step = 320
                End If
            Case 320 '檢查馬達是否停止
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_WAIT
                    GoToPosition_Step = 330
                End If
            Case 330 '檢查馬達是否停止
                GoToPosition_Step = 0
                Return True
        End Select
        Return False
    End Function
    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute1     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        Return 0
    End Function
    Sub pauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        '載入卡匣輸送帶暫停
        IN_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        tmr.IsEnabled = False '時間計時暫停
    End Sub
    Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        '載入卡匣輸送帶恢復
        IN_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        tmr.IsEnabled = True '時間計時恢復
    End Sub
End Class
