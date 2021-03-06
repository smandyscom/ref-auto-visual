﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Public Class nonExtendCassetteFeed
    Inherits systemControlPrototype
    Implements IFinishableStation
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public transferOutatIgnite As flagController(Of interlockedFlag) = New flagController(Of interlockedFlag)
    Public feedFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim tmrVacuumDelay As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Friend UD_ConveyerMotor As IDrivable

    Public CySway As cylinderGeneric = New cylinderGeneric With {.IsMonitorSensor = True} '橫儀汽缸
    Public CyFix As cylinderGeneric = New cylinderGeneric With {.IsMonitorSensor = True} '下定位汽缸



    Public ConveyerMotionOkCasAction As Func(Of Boolean) = Function() (False) '輸送帶設備給的旗標
    Public ConveyerMotionReset As Action = Sub() Console.WriteLine("")
    Friend ConveyerWaferEmpty As Func(Of Boolean) = Function() (True) '輸送帶設備給的升降卡匣的狀態


    '重置輸送帶設備旗標
    Public CassetteUpDownOK As Func(Of Boolean) = Function() (False) 'Hsien , 2015.04.04 'Action
    Public CheckWaferOnConveyerInCassette As Func(Of Boolean) = Function() (False)

    Friend SetWaferMoveDir As FeedDir
  

    Public Function stateIgniteBeltTongue() As Integer

        Select Case systemSubState
            Case 0 '等待初始化啟動
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 10
                End If

            Case 10 '是否需要移出cassette
                If transferOutatIgnite.readFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    systemSubState = 20
                Else
                    systemSubState = 100
                End If

            Case 20
                If transferOut() Then
                    OutAction = 0
                    systemSubState = 100
                End If

            Case 100 '初始化完成
                _FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
                systemSubState = 0
        End Select

        Return 0

    End Function



    Public Function stateExecuteBeltTongue() As Integer

        Select Case systemSubState
            Case 0 '檢查目前輸送帶的狀況
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) And feedFlags.viewFlag(flagsInLoaderUnloader.CyBackStatus) Then
                    '------------------------------------
                    '在卡匣已備便和氣壓缸縮回的狀況下<<<把氣缸伸出>>>
                    '------------------------------------

                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackReady_f, False) '設定氣缸為伸出,實際的狀況,要提早設定以免舌頭被壓到(CassetteTransport)
                    systemSubState = 10
                ElseIf (Not feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)) AndAlso
                    (Not feedFlags.viewFlag(flagsInLoaderUnloader.CyBackStatus)) AndAlso
                    ConveyerWaferEmpty() = True Then
                    '----------------------------------------------------
                    '在卡匣無備便和氣壓缸伸出的狀況下,且輸送帶沒有硅片<<<把氣缸縮回>>>
                    '----------------------------------------------------
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackStatus, True) '設定氣缸為縮回狀態,等實際實正縮回CyBackReady_f會設定為True
                    systemSubState = 30
                ElseIf feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) And (Not feedFlags.viewFlag(flagsInLoaderUnloader.CyBackStatus)) Then
                    '------------------------------------
                    '在卡匣已備便和氣壓缸伸出就位的狀況下<<<傳輸硅片>>>
                    '------------------------------------
                    systemSubState = 100
                Else 'If SetWaferMoveDir = FeedDir.MOVE_OUT OrElse (Not feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)) Then
                    Call ConveyerMotionReset()
                End If
            Case 10 '氣缸伸出,檢查氣缸是否伸出---------------(1)
                If transferIn() Then
                    InAction = 0
                    feedFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, False) '使輸送帶開始儲料
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackStatus, False) '設定氣缸為伸出狀態此程序使用,False伸出,True縮回
                    systemSubState = 0
                End If
                Call ConveyerMotionReset() '在伸縮氣缸時也可以回應輸送帶

            Case 30 '氣缸縮回,檢查氣缸是否縮回---------------(2)
                If transferOut() Then
                    OutAction = 0
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 300)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 60
                End If
                Call ConveyerMotionReset() '在伸縮氣缸時也可以回應輸送帶

            Case 60 '氣缸縮回且使縮回旗標致能
                If tmr.IsTimerTicked Then
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackReady_f, True)
                    systemSubState = 0
                End If
                Call ConveyerMotionReset() '在伸縮氣缸時也可以回應輸送帶

            Case 100 '檢查輸送帶是否準備往前 ---------------(3)
                If ConveyerMotionOkCasAction() = True Then      '使卡匣升降硅片於輸送帶(輸送帶下的命令)
                    systemSubState = 330
                Else
                    systemSubState = 0
                End If
                '----------------------------------------------------------------------------------------------
            Case 330 '檢查卡匣位置是否有硅片,如有卡匣要上升
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then
                    If feedFlags.viewFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f) Then '硅片可以進出卡匣
                        Select Case SetWaferMoveDir
                            Case FeedDir.MOVE_IN
                                feedFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, False) '重置旗標,使Cassette上升
                                systemSubState = 350
                            Case FeedDir.MOVE_OUT
                                If feedFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f) Then '按下清卡匣鈕
                                    ConveyerMotionReset()
                                    systemSubState = 0  'Hsien , 2016.01.20 , do not execute up-down procedure
                                Else
                                    tmr.IsEnabled = True
                                    feedFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, False) '重置旗標,使Cassette上升
                                    systemSubState = 350
                                End If
                        End Select
                    Else
                        systemSubState = 0
                    End If
                Else
                    systemSubState = 0
                End If
            Case 350 '卡匣上升完成,馬達可往前移動
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f) Then '卡匣上升完成
                    systemSubState = 360
                End If
            Case 360
                '-------------------------------------------
                '檢查設定硅片有無狀態,及使輸送帶可以續繼移動
                '-------------------------------------------
                If (CassetteUpDownOK()) Then
                    __cycleTime = cycleTimer.TimeElapsed
                    cycleTimer.IsEnabled = True 'restart cycle time calculating , Hsien , 2015.04.15
                    feedFlags.writeFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f, False)
                    systemSubState = 370
                End If
                'Case 365
                '    If (CassetteUpDownOK(False)) Then
                '        __cycleTime = cycleTimer.TimeElapsed
                '        cycleTimer.IsEnabled = True 'restart cycle time calculating , Hsien , 2015.04.15
                '        feedFlags.writeFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f, False)
                '        systemSubState = 370
                '    End If
            Case 370 '檢查硅片是否已進出卡匣,載出要檢查資料流,載入直接設為True
                If CheckWaferOnConveyerInCassette() Then    'synchronizing with conveyor
                    systemSubState = 0
                End If
        End Select
        Return 0

    End Function

    Public InSlowDownSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Public InReachSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Public OutReachSen As sensorControl = New sensorControl With {.IsEnabled = True}
    Dim InAction As Integer = 0
    Dim OutAction As Integer = 0
    Dim transferAlarm As alarmContentSensor = New alarmContentSensor With {.Sender = Me, .PossibleResponse = alarmContextBase.responseWays.RETRY, .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON}
    
    Function transferIn() As Boolean
        Select Case InAction

            Case 0
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_IN) =
                 IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 5)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    InAction = 10
                End If

            Case 10
                If tmr.IsTimerTicked Then
                    InAction = 20
                End If

            Case 20
                If UD_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = IDrivable.endStatus.EXECUTION_END Then
                    InAction = 40
                End If


            Case 40
                If InSlowDownSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 AndAlso
                    InReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 AndAlso
                  Not OutReachSen.IsSensorCovered Then
                    InAction = 60
                Else
                    With transferAlarm
                        .AdditionalInfo = "cassette移入失敗，請檢查Lifter下層所有sensor"
                        .Inputs = InReachSen.InputBit
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function()
                                                                                     InAction = 0
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(transferAlarm)
                    End With
                End If

            Case 60
                If CySway.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    If CyFix.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                        InAction = 80
                    End If
                End If

            Case 80
                InAction = 0
                Return True

        End Select
        Return False
    End Function
    Function transferOut() As Boolean

        Select Case OutAction

            Case 0
                If CyFix.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    If CySway.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                        OutAction = 20
                    End If
                End If

            Case 20
                If (UD_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LiftConveyerUsedPositions.MOTOR_MOVE_OUT) =
              IDrivable.endStatus.EXECUTION_END) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 5)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    OutAction = 40
                End If


            Case 40
                If tmr.IsTimerTicked Then
                    OutAction = 60
                End If

            Case 60
                If OutReachSen.OnTimer.TimeElapsed.TotalMilliseconds > 100 AndAlso
                    Not InSlowDownSen.IsSensorCovered AndAlso Not InReachSen.IsSensorCovered Then
                    OutAction = 80
                Else

                    With transferAlarm
                        .AdditionalInfo = "cassette移出失敗，請檢查Lifter下層所有sensor"
                        .Inputs = OutReachSen.InputBit
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function()
                                                                                     OutAction = 20
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(transferAlarm)
                    End With

                End If

            Case 80
                OutAction = 0
                Return True

        End Select
        Return False
    End Function


    Public Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgniteBeltTongue       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecuteBeltTongue  '鍊結主狀態函式
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        feedFlags.writeFlag(flagsInLoaderUnloader.CyBackStatus, True) '
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        initEnableAllDrives()
        Return 0
    End Function
End Class
