﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Public Class CassetteButton : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    '--------------------------
    '   Configurable parameters
    '--------------------------
    Public buttonDeboucingTime As TimeSpan = New TimeSpan(0, 0, 0, 0, 100)
    Public sen1ConstantOnTime As TimeSpan = New TimeSpan(0, 0, 3)
    Public cassetteSearchingTime As TimeSpan = New TimeSpan(0, 0, 30)
    ''' <summary>
    ''' Given sen2 had been occupied , force to new coming cassettte once sen1 had been triggered
    ''' </summary>
    ''' <remarks></remarks>
    Public sen1ForceLoadingTime As TimeSpan = New TimeSpan(0, 0, 5)

    Public buttonFlags As flagController(Of flagsInLoaderUnloader)

    Public btnUnloadWait As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim movUnloadWait As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Public btnLoadWait As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim movLoadWait As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Dim PB_Unload_Step As ULong
    Public PB_Unload As sensorControl = New sensorControl With {.IsEnabled = True}
    Public LP_Unload As ULong

    Dim PB_UnloadConveyerMove_Step As ULong
    Public PB_UnloadConveyerMove As sensorControl = New sensorControl With {.IsEnabled = True}
    Public LP_UnloadConveyerMove As ULong

    Dim PB_Load_Step As ULong
    Public PB_Load As sensorControl = New sensorControl With {.IsEnabled = True}
    Property unloadCassetteDelayTime As New TimeSpan(TimeSpan.TicksPerSecond * 5)
    Property LP_Load As ULong
        Get
            Return __lpLoad.OutputBit
        End Get
        Set(value As ULong)
            __lpLoad.OutputBit = value
        End Set
    End Property
    Protected __lpLoad As flipService = New flipService With {.FlipGoal = New TimeSpan(0, 0, 1)}

    Dim blnLampOn As Boolean

    Dim PB_LoadConveyerMove_Step As ULong
    Public PB_LoadConveyerMove As sensorControl = New sensorControl With {.IsEnabled = True}
    Public LP_LoadConveyerMove As ULong


    Friend IN_ConveyerMotor As IDrivable
    Friend OUT_ConveyerMotor As IDrivable

    '----------------------------------------------------------------------------
    '   Hsien , used to fit the first station style checking request , 2016.04.14
    '----------------------------------------------------------------------------
    Public IN_Stopper0 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = False} 'inlet style check reference stopper , down: on manual transmission , lift: not on manual transmission
    Public IN_ConveyerPosSen0 As sensorControl = New sensorControl With {.IsEnabled = False} 'inlet clearance check , on: manual mode is prohibited , transmission : check on-off , Hsien , 2016.04.14
    Friend styleSensorCheck As stateFunction = Function() (True)    'default : pass
    Dim styleSensorCheckState As Integer = 0

    Friend IN_ConveyerPosSen1Reference As sensorControl = Nothing '載出卡匣位置確認感測器1    'manaul mode trigger
    Friend IN_ConveyerPosSen2Reference As sensorControl = Nothing

    Friend OUT_ConveyerPosSen2Reference As sensorControl = Nothing '2016.10.20 jk 退出卡匣輸送帶最末端感測器(遠離lifter)，只要卡匣退出時，此感測器on時，輸送帶停止退出。

    '作動時Off , allocating before initmapping and setup , Hsien , 2015.06.24
    Public SafeStopper As ActuatorInfo = New ActuatorInfo With {.sw = IS_OFF}
    Public SafeStoperSen As SensorInfo = New SensorInfo With {.sensor = New sensorControl,
                                                              .sw = IS_OFF,
                                                              .status = IS_OFF}

    Public Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then

                    If (IN_Stopper0.IsEnabled) Then
                        systemSubState = 10
                    Else
                        systemSubState = 20
                    End If
                End If
            Case 10
                If (IN_Stopper0.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    systemSubState = 20
                End If
            Case 20
                _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                systemMainState = systemStatesEnum.EXECUTE
        End Select

        Return 0
    End Function

    Public Function stateExecute() As Integer
        Call CheckUnloadButton()
        Call CassetteMoveOut()
        Call CheckLoadButton()
        Call CassetteMoveIn()
        Return 0
    End Function
    Private Sub CheckUnloadButton()

        Select Case PB_Unload_Step
            Case 0 '檢查按鈕是否失能
                If Not buttonFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonDisable_f) Then
                    PB_Unload_Step = 10
                End If
            Case 10 '檢查按鈕是否有按下
                If PB_Unload.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    If Not buttonFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonDisable_f) Then '檢查按鈕是否仍然在失能模式
                        buttonFlags.writeFlag(flagsInLoaderUnloader.UnloadButtonBusy_f, True)
                        btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 2)
                        btnUnloadWait.IsEnabled = True    'restart
                        If SafeStopper.sw = IS_ON Then Call writeBit(SafeStopper.Actuator, False) '擋卡匣氣缸縮回(單一感測器無作動伸出時感測器On,作動縮回Off)
                        PB_Unload_Step = 15
                    End If
                    PB_Unload_Step = 15

                ElseIf (PB_Unload.OnTimer.TimeElapsed.TotalMilliseconds <= 100) Then

                    PB_Unload_Step = 0

                End If


            Case 15 '擋卡匣氣缸縮回
                If SafeStoperSen.sw = IS_ON Then
                    If (SafeStoperSen.status = IS_ON And SafeStoperSen.sensor.IsSensorCovered) Or (SafeStoperSen.status = IS_OFF And (Not SafeStoperSen.sensor.IsSensorCovered)) Then
                        btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                        btnUnloadWait.IsEnabled = True    'restart
                        PB_Unload_Step = 20
                    Else
                        If btnUnloadWait.IsTimerTicked Then
                            Dim ap As New alarmContentSensor
                            With ap
                                .Sender = Me
                                .Inputs = SafeStoperSen.sensor.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                If SafeStoperSen.status = IS_ON Then
                                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                                Else
                                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                End If
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    End If
                Else
                    btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    btnUnloadWait.IsEnabled = True    'restart
                    PB_Unload_Step = 20
                End If
            Case 20 '等待一段時間
                If btnUnloadWait.IsTimerTicked Then
                    Call writeBit(LP_Unload, True) '按鈕燈亮
                    PB_Unload_Step = 30
                End If
            Case 30

                If PB_Unload.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    btnUnloadWait.IsEnabled = True    'restart
                    PB_Unload_Step = 50
                End If


            Case 50 '檢查按鈕往上
                If btnUnloadWait.IsTimerTicked Then
                    If PB_Unload.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                        If SafeStopper.sw = IS_ON Then Call writeBit(SafeStopper.Actuator, False) '擋卡匣氣缸伸出
                        Call writeBit(LP_Unload, False) '把按鈕燈滅
                        btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 2)
                        btnUnloadWait.IsEnabled = True    'restart
                        PB_Unload_Step = 60
                    Else
                        PB_Unload_Step = 30
                    End If
                End If

            Case 60 '擋卡匣氣缸伸出
                If SafeStoperSen.sw = IS_ON Then
                    If (SafeStoperSen.status = IS_ON And (Not SafeStoperSen.sensor.IsSensorCovered)) Or (SafeStoperSen.status = IS_OFF And SafeStoperSen.sensor.IsSensorCovered) Then
                        btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                        btnUnloadWait.IsEnabled = True    'restart
                        PB_Unload_Step = 70
                    Else
                        If btnUnloadWait.IsTimerTicked Then
                            Dim ap As New alarmContentSensor
                            With ap
                                .Sender = Me
                                .Inputs = SafeStoperSen.sensor.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                If SafeStoperSen.status = IS_ON Then
                                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                                Else
                                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                End If
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    End If
                Else
                    PB_Unload_Step = 70
                End If
            Case 70 '輸送帶馬達剎車
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                    IDrivable.endStatus.EXECUTION_END Then
                    btnUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    btnUnloadWait.IsEnabled = True    'restart
                    PB_Unload_Step = 80
                End If
            Case 80 '等待一段時間
                If btnUnloadWait.IsTimerTicked Then
                    PB_Unload_Step = 90
                End If
            Case 90 '移出卡匣馬達移動
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    PB_Unload_Step = 120
                End If
            Case 120 '完成
                buttonFlags.writeFlag(flagsInLoaderUnloader.UnloadButtonBusy_f, False) '重置忙碌旗標
                PB_UnloadConveyerMove_Step = 0 '重置移動程序
                PB_Unload_Step = 0 '完成
        End Select
    End Sub
    Private Sub CassetteMoveOut()

        If Not buttonFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonBusy_f) Then Exit Sub

        Select Case PB_UnloadConveyerMove_Step
            Case 0 '檢查按鈕是否被按下
                If PB_UnloadConveyerMove.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    movUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    movUnloadWait.IsEnabled = True    'restart
                    PB_UnloadConveyerMove_Step = 5
                End If
            Case 5 '等待一段時間
                If movUnloadWait.IsTimerTicked Then
                    PB_UnloadConveyerMove_Step = 10
                End If
            Case 10 '再次檢查按鈕
                If PB_UnloadConveyerMove.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    Call writeBit(LP_UnloadConveyerMove, True)
                    PB_UnloadConveyerMove_Step = 15
                Else
                    PB_UnloadConveyerMove_Step = 0
                End If
            Case 15 '移出卡匣馬達移動
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    PB_UnloadConveyerMove_Step = 20
                End If
            Case 20 '等待馬達放開
                If PB_UnloadConveyerMove.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    Call writeBit(LP_UnloadConveyerMove, False)

                    PB_UnloadConveyerMove_Step = 25
                End If
            Case 25 '移出卡匣馬達剎車
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                     IDrivable.endStatus.EXECUTION_END Then
                    movUnloadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    movUnloadWait.IsEnabled = True    'restart
                    PB_UnloadConveyerMove_Step = 30
                End If
            Case 30 '返回
                If movUnloadWait.IsTimerTicked Then
                    PB_UnloadConveyerMove_Step = 0
                End If
        End Select
    End Sub
    Private Sub CheckLoadButton()

        Select Case PB_Load_Step
            Case 0
                'Hsien , 2015.04.13
                If buttonFlags.viewFlag(flagsInLoaderUnloader.Start_f) Then
                    PB_Load_Step = 10
                End If
            Case 10 '再次檢查按鈕
                '----------------------------------------------
                'light flipping
                '卡匣未在升降平台備便的狀況下,使其閃燈已提示操作者
                '----------------------------------------------
                If Not buttonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) And
                    Not __lpLoad.IsEnabled And
                    PB_Load.OnTimer.TimeElapsed.TotalMilliseconds < 100 Then '檢查卡匣是否在升降平台備便
                    __lpLoad.IsEnabled = True ' start flippng
                ElseIf __lpLoad.IsEnabled And PB_Load.OnTimer.TimeElapsed.TotalMilliseconds >= 100 Then
                    __lpLoad.IsEnabled = False
                    writeBit(LP_Load, True) 'toggle on

                End If

                'trigger condition : 
                '1. pb up-down
                '2. IN_SEN1 , had constantly on (able to parameterize)
                '3. not IN_SEN0 had on (inlet space had cleared) or not enabled (in case of this sensor not existed)
                If ((PB_Load.PulseCount > 0 And PB_Load.OnPulseWidth > buttonDeboucingTime) Or IN_ConveyerPosSen1Reference.OnTimer.TimeElapsed > sen1ConstantOnTime) AndAlso
                    Not buttonFlags.viewFlag(flagsInLoaderUnloader.LoadButtonDisable_f) And
                   (IN_ConveyerPosSen0.OffTimer.TimeElapsed.TotalMilliseconds > 100 Or Not IN_ConveyerPosSen0.IsEnabled) Then

                    buttonFlags.writeFlag(flagsInLoaderUnloader.LoadButtonBusy_f, True)
                    sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Cassette Button Locked"))
                    '檢查按鈕是否仍然在去能狀況
                    '在按鈕模式
                    __lpLoad.IsEnabled = False  'stop flashing
                    writeBit(LP_Load, False)    'cancel toggle

                    PB_Load_Step = 15
                Else
                    PB_Load_Step = 0 ' back to check if started
                End If
                PB_Load.PulseCount = 0 'reset pulse counter

            Case 15
                If (styleSensorCheck(styleSensorCheckState)) Then
                    styleSensorCheckState = 0 'rewind

                    PB_Load_Step = 20
                End If
            Case 20
                'extract cylinder or cylinder not enabled, Hsien , 2016.04.15
                If (Not IN_Stopper0.IsEnabled OrElse
                    IN_Stopper0.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                   IDrivable.endStatus.EXECUTION_END) Then

                    PB_Load_Step = 65
                End If
            Case 65 '(假如)載入輸送帶在移動狀態,作剎車
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                    IDrivable.endStatus.EXECUTION_END Then
                    PB_LoadConveyerMove_Step = 0 '重置馬達移動程序
                    btnLoadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    btnLoadWait.IsEnabled = True    'restart
                    PB_Load_Step = 75
                End If
            Case 75 '等待一時間
                If btnLoadWait.IsTimerTicked Then
                    PB_Load_Step = 100
                End If
                '======================================2016.04.14,steven
                '1.開始移動輸送帶馬達
                '2.判別輸送帶位置感測器Sen2的狀況-On至(3) ,Off至(4)
                '3.如果為On
                '  a.設定輸送帶馬達運轉時間5 sec
                '  b.時間到後輸送帶停止,結束
                '4.如果為Off
                '  a.設定time out時間30 sec及搜尋卡匣
                '  b.檢查輸送帶位置感測器Sen2,是否為On,是(5),否(6)
                '5.為On
                '  a.設定輸送帶馬達運轉時間3 sec
                '  b.時間到後輸送帶停止,結束
                '6.為Off
                '  a.檢查time out 時間是否到達
                '  b.時間到後輸送帶停止,結束

            Case 100 '開始移動卡匣
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    PB_Load_Step = 190
                End If
            Case 190
                If (Not IN_Stopper0.IsEnabled OrElse
                    IN_Stopper0.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    PB_Load_Step = 200
                End If
            Case 200 '重置忙碌旗標
                buttonFlags.writeFlag(flagsInLoaderUnloader.LoadButtonBusy_f, False) '不在按鈕模式
                sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Cassette Button Unlocked"))
                PB_Load_Step = 0 '完成
        End Select
    End Sub
    ''' <summary>
    '''
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function cassetteStyleErrorIgnoreHandler() As Boolean
        'let cassette go , and eject , Hsien , 2015.10.06
        buttonFlags.resetFlag(flagsInLoaderUnloader.LoadButtonBusy_f)
        PB_Load_Step = 10 'rewind to listen
        styleSensorCheckState = 0
        Return True
    End Function

    Private Sub CassetteMoveIn()
        Select Case PB_LoadConveyerMove_Step
            Case 0 '檢查按鈕是否按下
                If Not readBit(LP_Load) Then Exit Sub '按鈕在上
                If Not buttonFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f) Then Exit Sub '不在按鈕模式

                If PB_LoadConveyerMove.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    movLoadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    movLoadWait.IsEnabled = True    'restart
                    PB_LoadConveyerMove_Step = 5
                End If
            Case 5 '等待一段時間
                If movLoadWait.IsTimerTicked Then
                    PB_LoadConveyerMove_Step = 10
                End If
            Case 10 '再次確認按鈕
                If PB_LoadConveyerMove.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    Call writeBit(LP_LoadConveyerMove, True)
                    PB_LoadConveyerMove_Step = 20
                Else
                    PB_LoadConveyerMove_Step = 0
                End If

            Case 20 '載入馬逹運轉
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    PB_LoadConveyerMove_Step = 25
                End If
            Case 25 '等待按鈕放開或到達感測器


                If PB_LoadConveyerMove.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    Call writeBit(LP_LoadConveyerMove, False) '移動燈滅
                    PB_LoadConveyerMove_Step = 30
                End If

                'sensorControl.activateSensorControl(IN_ConveyerPosSen2Reference, PB_LoadConveyerMove_Step = 25)

            Case 30 '載入馬逹剎車
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                    IDrivable.endStatus.EXECUTION_END Then
                    movLoadWait.TimerGoal = New TimeSpan(0, 0, 0, 0, 150)
                    movLoadWait.IsEnabled = True    'restart
                    PB_LoadConveyerMove_Step = 35
                End If
            Case 35 '等待一段時間
                If movLoadWait.IsTimerTicked Then
                    PB_LoadConveyerMove_Step = 0
                End If
        End Select
    End Sub

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

End Class

