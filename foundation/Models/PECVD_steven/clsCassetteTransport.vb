﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine
Public Enum flagsInLoaderUnloader

    'Cassette:Cas    UD:Up Down
    CasOn_UD_ConveyerReady_f '卡匣已在升降平台就定位,可以使卡匣硅片載入載出
    CasMove_UD_Ok_f '硅片進出卡匣後升降完成

    CasUnloadSpaceReady_f '卡匣載出輸送帶是否有空間可以使卡匣載出
    CasUnloadInProcess_f '卡匣載出進行中
    CasUnloadEnable_f '卡匣載出致能
    CasUnloadFull_f '載出輸送帶卡匣是否滿料

    UnloadButtonBusy_f '手動載出忙碌狀態下(按鈕給程序)
    UnloadButtonDisable_f '去能載出按鈕(程序給按鈕)

    LoadButtonBusy_f '手動載入忙碌狀態下(按鈕給程序)
    LoadButtonDisable_f '使載入按鈕失效(程序給按鈕)

    CyBackStatus '設定氣缸為伸出狀態此程序為使用
    CyBackReady_f '設定氣缸實際的狀況為伸出
    CasReadyWaferInOut_f '卡匣硅片可以載入載出

    CasOn_IN_ConveyerReady_f '卡匣是否在載入輸送帶最前端備便

    CasCollect_f '卡匣收料

    BufferCanStore_f '輸送帶暫存區可以開始儲硅片

    IgnoreCasFirstWafer_f '如果卡是P25X3型式,可以作忽略取第一片
    GuideDontRaiseWafer_f '導軌是否有升起支持硅片

    'Hsien , 2015.05.21
    IsCassetteStyleAcceptable_f    '                           on: cassette style accepted , off: cassette style rejected
    IsCassetteAvailable_f           '                          the final decision data when cassette on lifter , on : would use to work , off : would reject
    Start_f 'Hsien , 2015.04.13 ' wait for start signal after ignited

    '2016.01.12 Steven
    WaferReadyToPick_f
    PickWaferInProc_f
    ChangeStack_f
    FirstTimeToSuck_f


End Enum
Public Enum BoxSelect
    P25X3_NONE
    P25X3_Box_1
    P25X3_Box_2
    P25X3_Box_3
End Enum
Public Class CyFixCasInfo
    Public cy As cylinderGeneric = Nothing
    Public sw As Boolean = False
End Class

Public Class CassetteTransport : Inherits systemControlPrototype
    Implements IFinishableStation

    Public Event CassetteRemoved(ByVal sender As Object, ByVal e As EventArgs)  'Cassette is removed from lifter
    Public Event CassetteRejected(ByVal sender As Object, ByVal e As EventArgs) 'Cassette is ejected by User
    Public Event CassetteRemoving(ByVal sender As Object, ByVal e As EventArgs) ' Tongue had extracted , going to unload cassette
    Public Event CassetteEntered(ByVal sender As Object, ByVal e As EventArgs) 'Cassette just arrived load positon
    Public Event CassetteCancelManualLoad As EventHandler
    Public Event CassetteCancelManualUnload As EventHandler

    ''' <summary>
    ''' occured when cassette had arrived start position, return value , true : action done
    ''' </summary>
    ''' <remarks></remarks>
    Public cassetteHadArrived As Func(Of Boolean) = Function() (True)
    ''' <summary>
    ''' occured when cassette had entered load position, return value , true : action done
    ''' </summary>
    ''' <remarks></remarks>
    Public cassetteHadEntered As Func(Of Boolean) = Function() (True)
    'Load
    '==========================================
    ' Cassette Direction   >>>>>>>>>>>>>
    '             Stopper1       Stopper2
    'Sen1          Sen2           Sen3
    '==========================================
    'Unload
    '==========================================
    ' <<<<<<<<<<<< Cassette Direction     
    '          Sen2           Sen1
    '==========================================

    'Property WorkingType As cassetteSystemBase.workingTypeEnum
    Public Enum CyFixCasIndex
        CY_1
        CY_2
        CY_3    'Hsien , 2016.04.08 expanded
    End Enum
    Public Enum ConveyerIndex
        UD_PART
        IN_PART
        OUT_PART
    End Enum
    Public Enum BasePosition
        LOAD
        START
        UNLOAD
    End Enum

    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public transportFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimerContinueType = New singleTimerContinueType With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim tmrMoveOut As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 3)}
    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen

    Friend inStopper1Reference As cylinderGeneric = Nothing
    Friend inStopper2Reference As cylinderGeneric = Nothing

    'Hsien , added default instance , 2015.06.12
    Public CyFixCas As CyFixCasInfo() = New CyFixCasInfo(2) {New CyFixCasInfo With {.sw = IS_OFF},
                                                             New CyFixCasInfo With {.sw = IS_OFF},
                                                             New CyFixCasInfo With {.sw = IS_OFF}}  'setup default

    'Public IN_ConveyerOverrideSen As sensorControl



    Friend IN_ConveyerMotor As IDrivable
    Friend OUT_ConveyerMotor As IDrivable
    Friend UD_ConveyerMotor As IDrivable    '   Hsien , 2015.06.04 , compitable with DC/servo


    Friend UD_Motor As motorControl
    Friend UD_Shell_Motor As motorControl = New motorControl With {.IsEnabled = True}

    Friend IN_ConveyerPosSen3 As sensorControl = Nothing

    'Public OUT_ConveyerOverrideSen As sensorControl

    Friend OUT_ConveyerPosSen1 As sensorControl
    Friend OUT_ConveyerPosSen2 As sensorControl
    Public UD_BrokenSen1 As New sensorControl
    Public UD_BrokenSen2 As New sensorControl
    Public UD_ConveyerSlowDownSen As sensorControl
    Public UD_ConveyerReachSen As sensorControl

    'Public CyExtendForthSen As sensorControl
    'Public CyExtendBackSen As sensorControl

    Public ManzCasWaferExistSen As sensorControl = New sensorControl With {.IsEnabled = False} '檢查卡匣內部是否有硅片(要為Off)
    Public ManzCasSafeSen As sensorControl = New sensorControl With {.IsEnabled = False} '下降後內部卡匣是否有卡住(要為Off)
    Public ManzCasStyleCheckSen As SensorInfo = New SensorInfo '(要為On)
    Public ManzCasWaferSafeSen As New sensorControl '檢查硅片是否有完全進入內部卡匣(要為Off)

    Public SetCasStyle As CassetteStyle = CassetteStyle.LAYER2N_STANDARD
    Public BoxCnt As Integer

    'Public blnCyExtendBackSenCheck = True

    Private CyIndex As Integer
    Private blnConveyerRun(3) As Boolean
    Private GoToPosition_Step As Short
    Private blnManzCstStyleErr As Boolean
    'Private NowManzCstPos As Integer


    Public Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1) '1 sec
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 5
                End If
            Case 5
                If tmr.IsTimerTicked Then
                    systemSubState = 20
                End If
                '----------------------
                ' Hsien , use slowdown input on motor board to implement interlock sensing
                ' 2015.09.22
                '----------------------
                'Case 10 '檢查出料匣是否有越位
                '    If Not OUT_ConveyerOverrideSen.IsSensorCovered Then
                '        systemSubState = 15
                '    Else
                '        Dim ap As New alarmContentSensor
                '        With ap
                '            .Sender = Me
                '            .Inputs = OUT_ConveyerOverrideSen.InputBit
                '            .PossibleResponse = alarmContextBase.responseWays.RETRY
                '            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                '            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                '                                                                         Return True
                '                                                                     End Function
                '            CentralAlarmObject.raisingAlarm(ap)
                '        End With
                '    End If
                'Case 15 '檢查入料匣是否有越位
                '    If Not IN_ConveyerOverrideSen.IsSensorCovered Then
                '        systemSubState = 20
                '    Else
                '        Dim ap As New alarmContentSensor
                '        With ap
                '            .Sender = Me
                '            .Inputs = IN_ConveyerOverrideSen.InputBit
                '            .PossibleResponse = alarmContextBase.responseWays.RETRY
                '            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                '            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                '                                                                         Return True
                '                                                                     End Function
                '            CentralAlarmObject.raisingAlarm(ap)
                '        End With
                '    End If

            Case 20 '檢查卡匣是否有在升降輸送帶上
                If UD_ConveyerSlowDownSen.IsSensorCovered Then
                    systemSubState = 40 '卡匣載出程序
                Else
                    systemSubState = 400 '馬達回原點,及移動至載入卡匣位置
                End If
                'Case 30 '檢查卡匣是否到位

                '    If UD_ConveyerReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                '        systemSubState = 150
                '    Else
                '        systemSubState = 40
                '    End If

                '    sensorControl.activateSensorControl(UD_ConveyerReachSen, systemSubState = 30)

            Case 40 '升降輸送帶開始運轉
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_IN) =
                    IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 60
                End If
            Case 60 '如果料匣保持移動直到到達
                If UD_ConveyerReachSen.IsSensorCovered Then
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    systemSubState = 70
                ElseIf tmr.IsTimerTicked Then
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    systemSubState = 120
                End If
            Case 70 '檢查馬達是否停止
                If UD_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 180
                End If
            Case 120 '產生錯誤訊息
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UD_ConveyerReachSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 20
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 150 '初始定位的氣壓缸
                CyIndex = CyFixCasIndex.CY_1
                systemSubState = 180
            Case 180 '使氣缸固定料匣
                If CyFixCas(CyIndex).sw = IS_ON Then
                    If CyFixCas(CyIndex).cy.drive(cylinderGeneric.cylinderCommands.GO_B_END) And IDrivable.endStatus.EXECUTION_END Then
                        systemSubState = 190
                    End If
                Else
                    systemSubState = 190
                End If
            Case 190 '指定下一定位氣壓缸
                CyIndex = CyIndex + 1
                If CyIndex > [Enum].GetValues(GetType(CyFixCasIndex)).Length - 1 Then
                    systemSubState = 200
                Else
                    systemSubState = 180
                End If
            Case 200 '檢查硅片是否有完全進入內部卡匣(要為Off)
                Select Case SetCasStyle
                    Case CassetteStyle.Manz
                        If Not ManzCasWaferSafeSen.IsSensorCovered Then
                            systemSubState = 205
                        Else
                            Dim ap As New alarmContentSensor
                            With ap
                                .Inputs = ManzCasWaferSafeSen.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    Case Else
                        systemSubState = 210
                End Select

            Case 210 '馬達回原點
                If (UD_Motor.drive(motorControl.motorCommandEnum.GO_HOME, LiftMotorUsedPositions.MOTOR_HOME) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    If SetCasStyle = CassetteStyle.Manz Then
                        'NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_APPROCH
                        systemSubState = 230
                    Else
                        systemSubState = 260
                    End If
                End If
            Case 230 '馬達回原點
                If (UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_HOME, LiftShellMotorUsedPositions.MOTOR_HOME) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 260
                End If
            Case 260 '把卡匣設為載出
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                If SetCasStyle = CassetteStyle.P25X3 Then '卡匣的型式為疊3個25層的料盒
                    BoxCnt = BoxSelect.P25X3_Box_3
                End If
                systemSubState = 530
                '---------------------------------------------------------------------------------------------
            Case 400 '檢查硅片是否有完全進入內部卡匣(要為Off)
                Select Case SetCasStyle
                    Case CassetteStyle.Manz
                        If Not ManzCasWaferSafeSen.IsSensorCovered Then
                            systemSubState = 405
                        Else
                            Dim ap As New alarmContentSensor
                            With ap
                                .Sender = Me
                                .Inputs = ManzCasWaferSafeSen.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    Case Else
                        systemSubState = 410
                End Select

            Case 410 '馬達回原點
                If (UD_Motor.drive(motorControl.motorCommandEnum.GO_HOME, LiftMotorUsedPositions.MOTOR_HOME) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    If SetCasStyle = CassetteStyle.Manz Then
                        'NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_WAIT
                        systemSubState = 430
                    Else
                        systemSubState = 460
                    End If
                End If
            Case 430 '馬達回原點
                If (UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_HOME, LiftShellMotorUsedPositions.MOTOR_HOME) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 440
                End If
            Case 440 '移動到的位置SHELL_LOAD
                If (UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_LOAD) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 450
                End If
            Case 450 '移動到的位置Wait
                If (UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_WAIT) = motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 510
                End If
            Case 460 '移動到的位置Loading
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_LOAD) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 510
                End If
                'missed unloading cassette procedure in ignite phase , Hsien , 2015.09.10


            Case 510 '如果料匣到達入料輸送帶最前端Sen3
                If Not IN_ConveyerPosSen3.IsSensorCovered Then
                    systemSubState = 530
                Else '如果逾時到錯誤檢查程序
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = IN_ConveyerPosSen3.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If

            Case 530
                If inStopper2Reference.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 540
                End If
            Case 540
                _FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
                systemSubState = 0
        End Select

        Return 0
    End Function

    Public Function stateExecute() As Integer

        Select Case systemSubState
            Case 0 '檢查按鈕所在的模式和輸送帶是空的
                If transportFlags.viewFlag(flagsInLoaderUnloader.UnloadButtonBusy_f) = False Then '不在手動載出模式
                    '檢查料匣是否可被移動，必須確定下輸帶沒有Cassette
                    If transportFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) = False Then '檢查卡匣是否載入完成
                        systemSubState = 5
                    End If
                ElseIf transportFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f) Then
                    RaiseEvent CassetteCancelManualUnload(Me, Nothing)
                End If
            Case 5
                If transportFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f) Then '檢查是否有卡匣要移出
                    If SetCasStyle = CassetteStyle.P25X3 And
                        BoxCnt < BoxSelect.P25X3_Box_3 Then
                        '卡匣的型式為疊3個25層的料盒
                        BoxCnt = BoxCnt + 1 '料盒的個數加1最多為3
                        systemSubState = 800 '移動至下一料盒的Start位置
                    Else
                        BoxCnt = BoxSelect.P25X3_NONE
                        systemSubState = 30 '卡匣載出
                    End If
                ElseIf _UpstreamStation IsNot Nothing AndAlso
                    _UpstreamStation.Count > 0 AndAlso
                    _UpstreamStation.TrueForAll(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) = True Then
                    '----------------------------
                    'upstream station is exsited
                    '   1. case : upstream station had finished job , set  "i m finished" , and wait rest protocal to done
                    '   2. case : upstream station on working , keep working still
                    '-----------------------------
                    _FinishableFlag.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True) '設定本站收料完成

                    'upstream station had finished job
                    systemSubState = 15
                ElseIf transportFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) Then
                    'upstream station still work
                    transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadInProcess_f, False) '通知載出卡匣程序未處理卡匣,載出程式可以再去檢查輸送帶感測器
                    systemSubState = 300 '卡匣載入
                Else
                    transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadInProcess_f, False) '通知載出卡匣程序未處理卡匣,載出程式可以再去檢查輸送帶感測器
                    systemSubState = 0
                End If
            Case 15
                '檢查上站是否收料完成,重新啟動
                If _UpstreamStation.Exists(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) = True Then
                    _FinishableFlag.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False) '設定本站啟動
                    systemSubState = 20
                End If
            Case 20
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadInProcess_f, False) '通知載出卡匣程序未處理卡匣,載出程式可以再去檢查輸送帶感測器
                systemSubState = 0
                '=======================================================
                '                       卡匣載出
                '=======================================================
            Case 30 '檢查舌頭是否縮回
                If transportFlags.viewFlag(flagsInLoaderUnloader.CyBackReady_f) Then
                    If (transportFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f)) Then
                        RaiseEvent CassetteRejected(Me, Nothing)    'cassette ejected by user , Hsien , 2015.05.22
                    End If

                    RaiseEvent CassetteRemoving(Me, Nothing) ' Hsien , 2016.03.28
                    systemSubState = 55
                End If
                'Case 50 '檢查防呆感測器是否有載具在上面
                '    If Not OUT_ConveyerOverrideSen.IsSensorCovered Then
                '        systemSubState = 55
                '    Else
                '        Dim ap As New alarmContentSensor
                '        With ap
                '            .Inputs = OUT_ConveyerOverrideSen.InputBit
                '            .PossibleResponse = alarmContextBase.responseWays.RETRY
                '            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                '            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                '                                                                         Return True
                '                                                                     End Function
                '            CentralAlarmObject.raisingAlarm(ap)
                '        End With
                '    End If

            Case 55 '檢查硅片是否有完全進入內部卡匣(要為Off),如果是放錯卡匣則不檢查
                Select Case SetCasStyle
                    Case CassetteStyle.MANZ
                        If Not ManzCasWaferSafeSen.IsSensorCovered Or blnManzCstStyleErr Then
                            blnManzCstStyleErr = False
                            systemSubState = 60
                        Else
                            Dim ap As New alarmContentSensor
                            With ap
                                .Sender = Me
                                .Inputs = ManzCasWaferSafeSen.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    Case Else
                        systemSubState = 60
                End Select
            Case 60 '移動到的位置卡匣載出位置
                If GoToBasePosition(BasePosition.UNLOAD) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 150) '150ms
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 70
                End If
            Case 70 '等待一段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 75
                End If
            Case 75
                Select Case SetCasStyle
                    Case CassetteStyle.MANZ
                        If Not ManzCasSafeSen.IsSensorCovered Then ' 下降後內部卡匣是否有卡住(要為Off)
                            systemSubState = 80
                        Else
                            Dim ap As New alarmContentSensor
                            With ap
                                .Inputs = ManzCasSafeSen.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY
                                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    Case Else
                        systemSubState = 80
                End Select
            Case 80 '初始定位的氣壓缸
                CyIndex = CyFixCasIndex.CY_1
                systemSubState = 90
            Case 90 '使氣缸釋放料匣
                If CyFixCas(CyIndex).sw = IS_ON Then '是否要動作  
                    If CyFixCas(CyIndex).cy.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                        systemSubState = 95
                    End If
                Else
                    systemSubState = 95
                End If
            Case 95 '指定下一定位氣壓缸
                CyIndex = CyIndex + 1
                If CyIndex > [Enum].GetValues(GetType(CyFixCasIndex)).Length - 1 Then
                    systemSubState = 100
                Else
                    systemSubState = 90
                End If
            Case 100 '等待一段時間
                systemSubState = 110
            Case 110 '檢查Sen1不在On的狀況下才送出載具
                If Not OUT_ConveyerPosSen1.IsSensorCovered Then
                    systemSubState = 145
                ElseIf Not OUT_ConveyerPosSen2.IsSensorCovered Then
                    systemSubState = 120 '載出馬運轉3sec使卡匣離開Sen1
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerPosSen2.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                      systemSubState = 145
                                                                                      Return True
                                                                                  End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 120 '載出卡匣馬達開始運轉
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                     IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 4) '4sec
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 130
                End If
            Case 130 '等待一段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 135
                End If
            Case 135 '檢查馬達是否停止
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                    IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 110
                End If
            Case 140 '檢查Sen1不在On的狀況下才送出載具
                If Not OUT_ConveyerPosSen1.IsSensorCovered Then
                    systemSubState = 145
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = OUT_ConveyerPosSen1.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 110
                                                                                     Return True
                                                                                 End Function
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                      systemSubState = 145
                                                                                      Return True
                                                                                  End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If

            Case 145 '載出卡匣馬達開始運轉
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 100)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 150
                End If
            Case 150 '等待一段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 155
                End If
            Case 155 '升降輸送帶開始移出料匣
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_OUT) =
                    IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 160
                End If
            Case 160 '檢查料匣移出感測器

                If UD_ConveyerSlowDownSen.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    '------------------------------------
                    '設定輸送帶最少的運轉時間
                    tmrMoveOut.TimerGoal = New TimeSpan(0, 0, 3)
                    tmrMoveOut.IsEnabled = True    'restart
                    '------------------------------------
                    systemSubState = 190
                ElseIf (tmr.IsTimerTicked) Then
                    OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    systemSubState = 170
                End If

                sensorControl.activateSensorControl(UD_ConveyerSlowDownSen, systemSubState = 160)

            Case 170 '檢查馬達是否停止
                If OUT_ConveyerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                   UD_ConveyerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 180
                End If
            Case 180 '產生錯誤訊息
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UD_ConveyerSlowDownSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 120
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With

            Case 190 '檢查是否到達位置感測器1

                If OUT_ConveyerPosSen1.OnTimer.TimeElapsed.TotalMilliseconds > 100 AndAlso
                    UD_BrokenSen1.IsSensorCovered = False AndAlso
                    UD_BrokenSen2.IsSensorCovered = False Then
                    systemSubState = 225
                ElseIf (tmr.IsTimerTicked) Then
                    OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    systemSubState = 200
                End If

                'sensorControl.activateSensorControl(OUT_ConveyerPosSen1, systemSubState = 190)

            Case 200 '檢查馬達是否停止
                If OUT_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END And
                   UD_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 210
                End If
            Case 210
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = OUT_ConveyerPosSen1.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 215
                                                                                 Return True
                                                                             End Function

                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  systemSubState = 225
                                                                                  Return True
                                                                              End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 215 '錯誤後輸送帶馬達再次運轉送出卡匣
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, UnloadConveyerUsedPositions.MOTOR_POSITION_1) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 220
                End If
            Case 220 '輸送帶馬達再次運轉送出卡匣
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_OUT) = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 190
                End If
                '----------------------------------------------------------------------------------
            Case 225 '使個載出卡匣的時間最少有過3sec
                If tmrMoveOut.IsTimerTicked Then
                    systemSubState = 230
                End If
            Case 230 '升降輸送帶馬達剎車
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 200)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 235
                End If
            Case 235 '等待一段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 240
                End If
            Case 240 '載出卡匣輸送帶剎車和重置輸送帶旗標
                If OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 250
                End If
            Case 250
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, False) '重置下輸送帶載出旗標，使其可以把Cassette送出
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadInProcess_f, False) '載出卡匣程序未處理卡匣
                systemSubState = 270
                'Case 260 '檢查料匣是否有越位,在載出輸送帶
                '    If Not OUT_ConveyerOverrideSen.IsSensorCovered Then
                '        systemSubState = 270
                '    Else
                '        Dim ap As New alarmContentSensor
                '        With ap
                '            .Inputs = OUT_ConveyerOverrideSen.InputBit
                '            .PossibleResponse = alarmContextBase.responseWays.RETRY
                '            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                '            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                '                                                                         Return True
                '                                                                     End Function
                '            CentralAlarmObject.raisingAlarm(ap)
                '        End With
                '    End If
            Case 270 '移動到的位置Loading
                If GoToBasePosition(BasePosition.LOAD) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 500)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 280
                End If
            Case 280 '等待一段時間
                If tmr.IsTimerTicked Then
                    systemSubState = 290
                End If
            Case 290 '檢查完成訊號

                'If (transportFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f)) Then
                '    RaiseEvent CassetteRejected(Me, Nothing)    'cassette ejected by user
                'End If

                transportFlags.writeFlag(flagsInLoaderUnloader.CasCollect_f, False)
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, False)
                RaiseEvent CassetteRemoved(Me, Nothing)    'Hsien , 2015.05.21 , cassette removed any way
                systemSubState = 5
                '=======================================================
                '                       卡匣載入
                '=======================================================
            Case 300 '等待啟動
                If (transportFlags.viewFlag(flagsInLoaderUnloader.Start_f)) Then
                    systemSubState = 305
                End If
            Case 305 '等待按鈕不是在忙碌的狀態下

                If Not transportFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f) Then '不在手動載入模式下
                    '-------------------------------------------------------------------------
                    transportFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入卡匣按鈕失效
                    '-------------------------------------------------------------------------
                    systemSubState = 310
                Else
                    'wait manual loading finished.
                    'send manual loading stop
                    RaiseEvent CassetteCancelManualLoad(Me, Nothing)
                End If
            Case 310 '檔桿縮回及檢查感測器
                If inStopper2Reference.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 320
                End If
            Case 320 '升降輸送帶開始運轉
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_IN) = IDrivable.endStatus.EXECUTION_END) Then
                    systemSubState = 330
                End If
            Case 330 '載入輸送帶開始移動
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 335
                End If
            Case 335
                'until minimum travelling interval satisfied , Hsien  , 2016.03.30
                If (tmr.IsTimerTicked) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 10)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart

                    systemSubState = 340
                End If
            Case 340 '如果料匣到達馬達停止運轉

                If UD_ConveyerSlowDownSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 370
                ElseIf tmr.IsTimerTicked Then
                    IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN)
                    systemSubState = 350
                End If

                sensorControl.activateSensorControl(UD_ConveyerSlowDownSen, systemSubState = 340)

            Case 350 '檢查馬達是否停止
                If IN_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END And
                    UD_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 360
                End If
            Case 360 '產生錯誤訊息
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UD_ConveyerSlowDownSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 320
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 370 '延遲一時間後再使載入輸帶停止
                If tmr.IsTimerTicked Then
                    systemSubState = 380
                End If
            Case 380 '載入輸帶停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 6)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 410
                End If
            Case 410 '如果料匣保持移動直到到達

                If UD_ConveyerReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    Select Case SetCasStyle
                        Case CassetteStyle.MANZ '檢查是否為Manz卡匣
                            systemSubState = 420
                        Case Else
                            systemSubState = 490
                    End Select
                ElseIf (tmr.IsTimerTicked) Then
                    UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) '輸送帶馬達停止
                    systemSubState = 470
                End If

                sensorControl.activateSensorControl(UD_ConveyerReachSen, systemSubState = 410)

            Case 420 '檢查Manz卡匣位置是否正常
                If (ManzCasStyleCheckSen.sw = IS_ON) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 430
                Else
                    systemSubState = 490
                End If
            Case 430 '檢查料匣形式防呆感測器
                If ManzCasStyleCheckSen.status = IS_ON And ManzCasStyleCheckSen.sensor.IsSensorCovered Then
                    systemSubState = 490
                ElseIf ManzCasStyleCheckSen.status = IS_OFF And (Not ManzCasStyleCheckSen.sensor.IsSensorCovered) Then
                    systemSubState = 490
                Else
                    If tmr.IsTimerTicked Then
                        systemSubState = 440
                    End If
                End If
            Case 440 '輸送帶馬達停止
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 450
                End If
            Case 450
                Dim ap As New alarmContentSensor
                With ap
                    .Inputs = ManzCasStyleCheckSen.sensor.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    If ManzCasStyleCheckSen.status = IS_ON Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    If ManzCasStyleCheckSen.status = IS_OFF Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 320
                                                                                 Return True
                                                                             End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  blnManzCstStyleErr = True 'Manz卡匣放置位置錯,強制退出
                                                                                  systemSubState = 500
                                                                                  Return True
                                                                              End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 470 '檢查馬達是否停止
                If UD_ConveyerMotor.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 480
                End If
            Case 480 '產生錯誤訊息
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = UD_ConveyerReachSen.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 320
                                                                                 Return True
                                                                             End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 490 '馬達剎車
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 500
                End If
            Case 500 '檔桿伸出
                If inStopper2Reference.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    '-------------------------------------------------------------------------
                    transportFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '此時手動鈕可以動作
                    transportFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, False) '料匣是否備便旗標去能

                    'transmit cassette style status , Hsien  ,2016.06.15
                    transportFlags.writeFlag(flagsInLoaderUnloader.IsCassetteAvailable_f, transportFlags.viewFlag(flagsInLoaderUnloader.IsCassetteStyleAcceptable_f))

                    RaiseEvent CassetteEntered(Me, Nothing)  'used to plug some event handlers , e.g  read Barcode on loading postion , Hsien , 2016.05.19
                    '-------------------------------------------------------------------------
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 250)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 520
                End If
            Case 520 '等待一段時間後CyDA or CYAH 往上使料匣固定
                If tmr.IsTimerTicked Then
                    systemSubState = 530
                End If
            Case 530 '初始定位的氣壓缸
                CyIndex = CyFixCasIndex.CY_1
                systemSubState = 540
            Case 540 '使氣缸固定料匣
                If CyFixCas(CyIndex).sw = IS_ON Then
                    CyFixCas(CyIndex).cy.drive(cylinderGeneric.cylinderCommands.GO_B_END) ' drive to clamp
                    systemSubState = 545
                Else
                    systemSubState = 550
                End If
            Case 545 'check if fix cylinder worked normally
                If CyFixCas(CyIndex).cy.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 550
                ElseIf CyFixCas(CyIndex).cy.CommandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL Then
                    'failed , user selected ignore , reject to loading
                    systemSubState = 610  ' eject cassette
                End If
            Case 550 '指定下一定位氣壓缸
                CyIndex = CyIndex + 1
                If CyIndex > [Enum].GetValues(GetType(CyFixCasIndex)).Length - 1 Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 570
                Else
                    systemSubState = 540
                End If

            Case 570 'Manz卡匣放置位置錯,強制退出
                If SetCasStyle = CassetteStyle.MANZ AndAlso
                    blnManzCstStyleErr Then
                    transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                    RaiseEvent CassetteRejected(Me, Nothing) 'Hsien , 2015.05.21 , cassette eject due to ignore alarm
                    systemSubState = 0
                    Return True
                Else
                    systemSubState = 572
                End If
            Case 572
                'on load position , used to trigger some external procedure , i.e read rfid , Hsien , 2016.05.24
                If (cassetteHadEntered()) Then
                    systemSubState = 575
                End If
            Case 575 '移動到的位置Start位置
                If GoToBasePosition(BasePosition.START) Then
                    systemSubState = 580
                End If
            Case 580
                Select Case SetCasStyle
                    Case CassetteStyle.MANZ
                        '下料時感測器要為a接點,檢查卡匣內部是否有硅片,如卡匣內有硅片則退卡匣
                        '上料時感測器要為b接點,檢查卡匣內部是否有硅片,如卡匣內有硅片則進卡匣
                        If Not ManzCasWaferExistSen.IsSensorCovered Then
                            'Hsien , 2015.05.21 , alway to check if cassette acceptable
                            systemSubState = 590
                        Else
                            Dim ap As New alarmContentSensor
                            With ap
                                .Sender = Me
                                .Inputs = ManzCasWaferExistSen.InputBit
                                .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                                .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                                .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                             Return True
                                                                                         End Function
                                .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                              transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                                                                                              RaiseEvent CassetteRejected(Me, Nothing) 'Hsien , 2015.05.21 , cassette eject due to ignore alarm
                                                                                              systemSubState = 0
                                                                                              Return True
                                                                                          End Function
                                CentralAlarmObject.raisingAlarm(ap)
                            End With
                        End If
                    Case Else '檢查是否致能外部控制
                        systemSubState = 590 ' always check if acceptable
                End Select
            Case 590 '告知外部控卡匣已備便
                If (cassetteHadArrived()) Then
                    systemSubState = 600
                End If
            Case 600 '等待外部控制回應
                'temp code-------------
                'transportFlags.resetFlag(flagsInLoaderUnloader.ExternalCassetteReady_f)
                'transportFlags.setFlag(flagsInLoaderUnloader.ExternalCaseetteAccepted_f)
                '--------------------------
                If (transportFlags.viewFlag(flagsInLoaderUnloader.IsCassetteAvailable_f)) Then '外部控制回應卡匣工作
                    transportFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, True)
                    systemSubState = 0
                Else '外部控制回應卡匣退出
                    'rejected
                    systemSubState = 610

                End If

            Case 610
                transportFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                RaiseEvent CassetteRejected(Me, Nothing) 'Hsien , 2015.05.21 , cassette ejected by Host
                systemSubState = 0
                '----------------------------------------------------------------------------------
            Case 800 '等待舌頭馬達縮回
                If transportFlags.viewFlag(flagsInLoaderUnloader.CyBackReady_f) Then
                    systemSubState = 810
                End If
            Case 810 'P25X3 移動到的位置Start1,2...位置
                If GoToBasePosition(BasePosition.START, BoxCnt) Then
                    systemSubState = 820
                End If
            Case 820
                transportFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, True)
                systemSubState = 0
        End Select
        Return 0
    End Function
    Function GoToBasePosition(ByVal basePos As BasePosition, Optional BoxIndex As Integer = BoxSelect.P25X3_Box_1) As Boolean
        Static CassettePosIndex As LiftMotorUsedPositions
        Select Case GoToPosition_Step
            Case 0
                Select Case basePos
                    Case BasePosition.LOAD
                        Select Case SetCasStyle
                            Case CassetteStyle.LAYER2N_STANDARD,
                                CassetteStyle.LAYER100_PLATE_TONGUE,
                                CassetteStyle.LAYER50_STANDARD,
                                CassetteStyle.LAYER150_STANDARD

                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_LOAD : GoToPosition_Step = 10
                            Case CassetteStyle.MANZ
                                GoToPosition_Step = 100
                            Case CassetteStyle.P25X3
                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_LOAD : GoToPosition_Step = 10
                        End Select
                    Case BasePosition.START
                        Select Case SetCasStyle
                            Case CassetteStyle.LAYER2N_STANDARD,
                                CassetteStyle.LAYER100_PLATE_TONGUE,
                                 CassetteStyle.LAYER150_STANDARD

                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_START : GoToPosition_Step = 10
                            Case CassetteStyle.LAYER50_STANDARD
                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_LAYER50_START : GoToPosition_Step = 10
                            Case CassetteStyle.MANZ
                                GoToPosition_Step = 200
                            Case CassetteStyle.P25X3
                                Select Case BoxIndex
                                    Case BoxSelect.P25X3_Box_1 '第1料的起始位置
                                        CassettePosIndex = LiftMotorUsedPositions.MOTOR_P25X3_START_1 : GoToPosition_Step = 10
                                        BoxCnt = BoxSelect.P25X3_Box_1
                                    Case BoxSelect.P25X3_Box_2 '第2料的起始位置
                                        CassettePosIndex = LiftMotorUsedPositions.MOTOR_P25X3_START_26 : GoToPosition_Step = 10
                                    Case BoxSelect.P25X3_Box_3 '第3料的起始位置
                                        CassettePosIndex = LiftMotorUsedPositions.MOTOR_P25X3_START_51 : GoToPosition_Step = 10
                                End Select
                        End Select
                    Case BasePosition.UNLOAD
                        Select Case SetCasStyle
                            Case CassetteStyle.LAYER2N_STANDARD,
                                CassetteStyle.LAYER100_PLATE_TONGUE,
                                CassetteStyle.LAYER50_STANDARD,
                                 CassetteStyle.LAYER150_STANDARD

                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_UNLOAD : GoToPosition_Step = 10
                            Case CassetteStyle.MANZ
                                GoToPosition_Step = 300
                            Case CassetteStyle.P25X3
                                CassettePosIndex = LiftMotorUsedPositions.MOTOR_UNLOAD : GoToPosition_Step = 10
                        End Select
                End Select
                '-----------------------------------ACI、JR Cassette
            Case 10 'ACI、JR(使馬達運行) Load 、 Unload、 Start
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, CassettePosIndex) = motorControl.statusEnum.EXECUTION_END Then
                    GoToPosition_Step = 0
                    Return True
                End If
                '-----------------------------------Manz Cassette
            Case 100 'MANZ(使馬達運行)===(Load)
                If UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_LOAD) = motorControl.statusEnum.EXECUTION_END Then
                    GoToPosition_Step = 0
                    Return True
                End If
            Case 200 'MANZ(使馬達運行)===(Start)
                UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH) '頂升靠近內部卡匣
                UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_START) '外部卡匣舌頭可以插入位置
                GoToPosition_Step = 210
            Case 210 '檢查馬達是否停止
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH) = motorControl.statusEnum.EXECUTION_END Then

                    If (Parent.GetType() = GetType(cassetteSystemBase)) Then
                        UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_START) '頂升內部卡匣到舌頭工作位置
                    End If
                    'NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_START

                    GoToPosition_Step = 220
                End If
            Case 220 '檢查馬達是否停止
                If UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_START) = motorControl.statusEnum.EXECUTION_END Then
                    GoToPosition_Step = 230
                End If
            Case 230 '檢查馬達是否停止
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    GoToPosition_Step = 0
                    Return True
                End If
            Case 300 'MANZ(使馬達運行)===(Unload)
                UD_Shell_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_UNLOAD)
                'If NowManzCstPos <> LiftMotorUsedPositions.MOTOR_MANZ_WAIT Then '如果內卡匣不在Wait位置,強制退卡匣時,內卡已在此位置就不用再作動
                '    UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH)
                '    GoToPosition_Step = 310
                'Else
                '    GoToPosition_Step = 330
                'End If
                UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_APPROCH)
                GoToPosition_Step = 310

            Case 310 '檢查內卡匣是否到逹Approch位置
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_WAIT) '內卡匣移至Wait位置
                    GoToPosition_Step = 320
                End If
            Case 320 '檢查馬達是否停止
                If UD_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    'NowManzCstPos = LiftMotorUsedPositions.MOTOR_MANZ_WAIT
                    GoToPosition_Step = 330
                End If
            Case 330 '檢查馬達是否停止
                If UD_Shell_Motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    GoToPosition_Step = 0
                    Return True
                End If

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
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態

        Return 0
    End Function

    Sub pauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        '上下卡匣輸送帶暫停
        UD_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        '載入卡匣輸送帶暫停
        IN_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        '載出卡匣輸送帶暫停
        OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)

        tmr.IsEnabled = False '時間計時暫停
        tmrMoveOut.IsEnabled = False
    End Sub
    Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        '上下卡匣輸送帶恢復
        UD_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        '載入卡匣輸送帶恢復
        IN_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        '載出卡匣輸送帶恢復
        OUT_ConveyerMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)

        tmr.IsEnabled = True '時間計時恢復
        tmrMoveOut.IsEnabled = True
    End Sub
End Class

