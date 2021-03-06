﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components

Public Enum UnloadConveyerUsedPositions
    MOTOR_POSITION_1
    MOTOR_POSITION_2
End Enum
Public Class CassetteUnload : Inherits systemControlPrototype
    Implements IFinishableStation
    Enum ConveyerCassetteStatusEnum
        NOT_FULL
        FULL
        MOVE_OUT
    End Enum

    Enum ConveyerPositionSensorEnum
        CHECK_SENSOR1
        CHECK_SENSOR2
        CHECK_SENSOR3
    End Enum

    Const CassetteFullCheckSensorNum As Integer = 3
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public unloadFlags As flagController(Of flagsInLoaderUnloader)
    Dim __timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 10)}

    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen
    'Public OUT_ConveyerMotor As motorControl = New motorControl            '載出馬達   ,
    Public OUT_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}        'Hsien , regular the data type , 2015.06.04

    'Public OUT_ConveyerOverrideSen As sensorControl = New sensorControl With {.IsEnabled = False}
    ''' <summary>
    ''' Lifter Side
    ''' </summary>
    ''' <remarks></remarks>
    Public OUT_ConveyerPosSen1 As sensorControl = New sensorControl With {.IsEnabled = True} '載出卡匣位置確認感測器1 , the lifter side
    ''' <summary>
    ''' Outlet Side 
    ''' </summary>
    ''' <remarks></remarks>
    Public OUT_ConveyerPosSen2 As sensorControl = New sensorControl With {.IsEnabled = True} '載出卡匣位置確認感測器2 , away from lifter

    Dim blnStatus(CassetteFullCheckSensorNum) As Boolean

    Public Function stateExecute() As Integer

        '==========================================
        ' <<<<<<<<<<<< Cassette Direction     
        '          Sen2           Sen1
        '==========================================
        _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)


        Select Case systemSubState
            Case 0 '檢查輸送帶是否滿料
                If Not unloadFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonBusy_f) Then '不在手動載出模式下
                    If CheckConveyerPosSen() = ConveyerCassetteStatusEnum.FULL Then '檢查卡匣狀態辨別是否滿料
                        systemSubState = 10
                    Else '輸送帶未滿
                        'If unloadFlags.viewFlag(flagsInLoaderUnloader.Start_f) Then    'Hsien , 2015.06.19 , may occur dead-lock , so that cancel the flag control
                        unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadFull_f, False)
                        systemSubState = 40
                        'End If
                    End If
                Else
                    unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadFull_f, False)
                End If
            Case 10 '再次檢查料匣是否在輸送帶上
                If Not unloadFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonBusy_f) Then '不在手動載入模式下
                    If CheckConveyerPosSen() = ConveyerCassetteStatusEnum.FULL Then
                        unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadFull_f, True) '設定輸送帶滿卡匣
                    Else
                        systemSubState = 30
                    End If
                Else '在手動載入模式下
                    systemSubState = 30
                End If
            Case 30 '重置不暫停旗標
                systemSubState = 0
                '===================================================================
            Case 40 '輸送帶最前端無卡匣可以把卡匣往前移動且目前沒有卡匣要送出
                If CheckConveyerPosSen() = ConveyerCassetteStatusEnum.MOVE_OUT And (Not unloadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f)) Then
                    systemSubState = 70
                Else
                    systemSubState = 50
                End If
            Case 50 '在輸送帶上的料匣未滿,設定檢查Time out 時間
                unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, True)
                __timer.TimerGoal = New TimeSpan(0, 0, 5)
                __timer.IsEnabled = True    'restart
                systemSubState = 60
            Case 60 '等待旗標被設為False時執行下面程序,移出料匣
                If Not unloadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f) Then
                    systemSubState = 70
                ElseIf __timer.IsTimerTicked And
                    Not unloadFlags.viewFlag(flagsInLoaderUnloader.CasUnloadInProcess_f) Then '在還未進行載出卡匣程序時,載出程式可以再去檢查輸送帶感測器
                    '在還未載出時設定時間到,再次檢查是否滿料
                    '是否Time out
                    unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, False) '設為沒空間載出卡匣,回到前面檢查狀況,是否滿料
                    systemSubState = 0
                End If
            Case 70 '去能載出按鈕和錯誤及暫停旗標
                If Not unloadFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonBusy_f) Then
                    unloadFlags.writeFlag(flagsInLoaderUnloader.UnloadButtonDisable_f, True)
                    systemSubState = 80
                End If
            Case 80 '輸送帶馬達開始運轉料匣移出
                If (OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END) Then
                    __timer.TimerGoal = New TimeSpan(0, 0, 6)
                    __timer.IsEnabled = True    'restart
                    systemSubState = 90
                End If
            Case 90 '輸送帶馬達停止運轉
                If __timer.IsTimerTicked Then
                    If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                        IDrivable.endStatus.EXECUTION_END Then
                        systemSubState = 100
                    End If
                End If
            Case 100 '重置所有的旗標
                unloadFlags.writeFlag(flagsInLoaderUnloader.UnloadButtonDisable_f, False) '去能手動按鈕
                systemSubState = 0
        End Select
        Return 0
    End Function

    Public Function CheckConveyerPosSen() As ConveyerCassetteStatusEnum


        '==========================================
        ' <<<<<<<<<<<< Cassette Direction     
        '          Sen2           Sen1
        '==========================================

        CheckConveyerPosSen = ConveyerCassetteStatusEnum.NOT_FULL

        blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR1) = OUT_ConveyerPosSen1.OnTimer.TimeElapsed.TotalMilliseconds > 100 '卡匣移出第一個感測器
        blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR2) = OUT_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds > 100 '卡匣移出第二個感測器


        If blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR1) And blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR2) Then '滿料
            Return ConveyerCassetteStatusEnum.FULL
        ElseIf blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR1) And (Not blnStatus(ConveyerPositionSensorEnum.CHECK_SENSOR2)) Then '卡匣可以往前到最前端取卡匣位置,第二個感測器沒有卡匣
            Return ConveyerCassetteStatusEnum.MOVE_OUT
        End If

        Return 0
    End Function
    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute     '鍊結主狀態函式
        systemMainState = systemStatesEnum.EXECUTE   '設定初始主狀態
        _FinishableFlag.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
        CType(OUT_ConveyerMotor, driveBase).IsEnabled = True
        Return 0

    End Function

    Sub pauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        __timer.IsEnabled = False '時間計時暫停
    End Sub
    Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        __timer.IsEnabled = True '時間計時恢復
    End Sub
End Class

