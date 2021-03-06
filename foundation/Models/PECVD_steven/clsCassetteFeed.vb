﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Public Enum LiftConveyerUsedPositions
    MOTOR_MOVE_IN
    MOTOR_MOVE_OUT
End Enum
Public Enum ForthBackMotorUsedPositions
    MOTOR_Home
    MOTOR_MOVE_FORTH
    MOTOR_MOVE_FORTH_HALF
    MOTOR_MOVE_BACK
    MOTOR_MOVE_BACK_SLOW
End Enum

Public Enum AlignMotorUsedPositions
    MOTOR_Home
    MOTOR_CLAMP
    MOTOR_RELEASE
End Enum

Public Class CassetteFeed : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public Enum getWaferMethodEnum
        METHOD_BY_BELT_TONGUE
        METHOD_BY_PLATE_TONGUE
    End Enum
    Public stateExecutePlateTongue As Func(Of Integer) = New Func(Of Integer)(Function() (0))
    Dim currentMethod As getWaferMethodEnum = getWaferMethodEnum.METHOD_BY_BELT_TONGUE

    Property GetWaferMethod As getWaferMethodEnum
        Get
            Return currentMethod
        End Get

        Set(value As getWaferMethodEnum)
            Select Case value
                Case getWaferMethodEnum.METHOD_BY_BELT_TONGUE
                    systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgniteBeltTongue
                    systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecuteBeltTongue
                Case getWaferMethodEnum.METHOD_BY_PLATE_TONGUE
                    systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnitePlateTongue
                    systemMainStateFunctions(systemStatesEnum.EXECUTE) = stateExecutePlateTongue
                Case Else
            End Select
            currentMethod = value
        End Set
    End Property


    Public feedFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim tmrVacuumDelay As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen
    '---------------------------------------------------------------
    '針對舌頭輸送硅片和定位硅片
    Public ForthBack_TongueMotor As motorControl = New motorControl '輸送硅片舌頭馬達
    Public UpDownTongueCy As cylinderGeneric = New cylinderGeneric '升降硅片舌頭氣缸
    Public TongueVacuum As Integer '舌頭真空
    Public TongueVacuumSen As sensorControl = New sensorControl '舌頭真空感測器
    Public Side_AlignerMotor As motorControl = New motorControl '側定位硅片馬達
    Public Forth_AlignerMotor As motorControl = New motorControl '前定位硅片馬達
    Public UpDownAlignerCy As cylinderGeneric = New cylinderGeneric '升降頂硅片氣缸

    Dim blnWaferOnTongueReady As Boolean '硅片在舌頭上已備便
    Dim blnWaferDownOk As Boolean '舌頭硅片下降完成
    Dim AlignPut_Step As Integer

    Dim blnFirstTimeGetWafer As Boolean
    Dim blnTongueUpOk As Boolean
    Dim blnAlignerFinish As Boolean = True
    Dim TongueGetWaferCnt As Short
    '---------------------------------------------------------------

    Public WaferExistCheckSen As sensorControl = New sensorControl

    Public CyExtendTongue As cylinderGeneric = New cylinderGeneric With {.IsMonitorSensor = True}   'keep monitoring tongue , Hsien  ,2015.10.05
    
    Public TongueBackWaferExistSen As sensorControl = New sensorControl '卡匣升降前要檢查
    Dim blnWaferExistOnTongueBackPos As Boolean = False

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
            Case 10 '氣缸縮回
                If CyExtendTongue.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
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
                If CyExtendTongue.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    feedFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, False) '使輸送帶開始儲料
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackStatus, False) '設定氣缸為伸出狀態此程序使用,False伸出,True縮回
                    systemSubState = 0
                End If
                Call ConveyerMotionReset() '在伸縮氣缸時也可以回應輸送帶

            Case 30 '氣缸縮回,檢查氣缸是否縮回---------------(2)
                If CyExtendTongue.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
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
    Public Function stateIgnitePlateTongue() As Integer
        Select Case systemSubState
            Case 0 '等待初始化啟動()
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 10
                End If
            Case 10 '馬達回原點
                ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_HOME, ForthBackMotorUsedPositions.MOTOR_Home) '舌頭
                Side_AlignerMotor.drive(motorControl.motorCommandEnum.GO_HOME, AlignMotorUsedPositions.MOTOR_Home) '側定位
                Forth_AlignerMotor.drive(motorControl.motorCommandEnum.GO_HOME, AlignMotorUsedPositions.MOTOR_Home) '前定位
                systemSubState = 20
            Case 20 '等待馬達回原點完成
                If ForthBack_TongueMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                    Forth_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                    Side_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 30
                End If
            Case 30 '定位馬達到位放開位置
                ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_BACK)
                Side_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_RELEASE)
                Forth_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_RELEASE)
                systemSubState = 40
            Case 40 '等待馬達到達位置
                If ForthBack_TongueMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                     Forth_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                     Side_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 50
                End If
            Case 50 '舌頭下降
                If UpDownTongueCy.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 60
                End If
            Case 60 '定位下降
                If UpDownAlignerCy.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 100
                End If
            Case 100 '初始化完成
                _FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
                systemSubState = 0
        End Select

        Return 0
    End Function

    Public Function TongueGetWafer() As Integer

        Select Case systemSubState
            Case 0 '檢查是否可以取放片
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then '卡匣備便
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackReady_f, False) '設定氣缸為伸出,實際的狀況,要提早設定以免舌頭被壓到(CassetteTransport)
                    systemSubState = 10
                Else '卡匣未備便
                    feedFlags.writeFlag(flagsInLoaderUnloader.CyBackReady_f, True) '舌頭縮回旗標致能
                    TongueGetWaferCnt = 0 '第一次取硅片計數重置
                End If
            Case 10 '判別是否已第2次取硅片
                If TongueGetWaferCnt < 2 Then
                    TongueGetWaferCnt = TongueGetWaferCnt + 1
                    systemSubState = 20
                Else
                    systemSubState = 100
                End If
            Case 20 '舌頭馬達移動至一半位置,避免碰撞硅片
                If ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_FORTH_HALF) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 40
                End If
            Case 40 '舌頭上升及舌頭馬達移動
                UpDownTongueCy.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                systemSubState = 60
            Case 60 '舌頭提升到位完成
                If UpDownTongueCy.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 200)
                    tmr.resetTimer()
                    tmr.IsEnabled = True    'restart
                    systemSubState = 70
                End If
            Case 70 '延遲一段時間
                If tmr.IsTimerTicked Then
                    ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_FORTH)
                    systemSubState = 80
                End If
            Case 80 '舌頭馬達移動到位完成
                If ForthBack_TongueMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 200
                End If
                '-----------------------------------
            Case 100 '舌頭馬達移動到位
                If ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_FORTH) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 110
                Else
                    Dim pos As Double = ForthBack_TongueMotor.pulse2Unit(ForthBack_TongueMotor.FeedBackPosition)
                    If Math.Abs(pos) > Math.Abs(pData.MotorPoints(ForthBack_TongueMotor.PositionDictionary(ForthBackMotorUsedPositions.MOTOR_MOVE_FORTH_HALF)).DistanceInUnit) Then '位置到達MOTOR_MOVE_FORTH_HALF後舌頭提升
                        If Not blnTongueUpOk Then
                            UpDownTongueCy.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                            'ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)   'test use , 2015.06.17 Hsien 
                            blnTongueUpOk = True '舌頭已上升旗標
                            'ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.MOTION_RESUME)   'test use , 2015.06.17 Hsien 
                        End If
                    End If
                End If
            Case 110 '檢查舌頭是否已事先升起
                If Not blnTongueUpOk Then
                    UpDownTongueCy.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                End If
                systemSubState = 120
            Case 120 '舌頭提升完成
                If UpDownTongueCy.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    blnTongueUpOk = False '舌頭已上升旗標重置
                    systemSubState = 200
                End If
                '-----------------------------------
            Case 200 '通知卡匣可以升降
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f) Then
                    feedFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, False) '重置旗標,使Cassette上升
                    systemSubState = 220
                Else '卡匣未備便
                    If Not feedFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then
                        systemSubState = 400 '舌頭下降及縮回
                    End If
                End If
            Case 220 '檢查卡匣是否升降完成
                If feedFlags.viewFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f) Then '卡匣上升完成
                    feedFlags.writeFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f, False)
                    Call writeBit(TongueVacuum, True) '開啟真空

                    systemSubState = 240
                End If
            Case 240 '檢查輸送帶上是否已沒有硅片及硅片是否定位完成,可以退回
                Dim blnIsVacuumSen As Boolean = TongueVacuumSen.IsSensorCovered
                Dim blnIsTongueBackWaferExist As Boolean = TongueBackWaferExistSen.IsSensorCovered
                Dim blnConveyorMoving As Boolean = CheckWaferOnConveyerInCassette.Invoke

                If blnAlignerFinish AndAlso (Not blnIsTongueBackWaferExist OrElse blnConveyorMoving) Then
                    blnAlignerFinish = False
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 500)
                    tmr.IsEnabled = True    'restart
                    '------------------------------------
                    tmrVacuumDelay.TimerGoal = New TimeSpan(0, 0, 0, 0, 100)
                    tmrVacuumDelay.IsEnabled = True    'restart
                    '------------------------------------
                    systemSubState = 300
                End If

            Case 300 '檢查舌頭真空狀況
                Dim blnIsVacuumSen As Boolean = TongueVacuumSen.IsSensorCovered

                If blnIsVacuumSen And tmrVacuumDelay.IsTimerTicked Then '快速退回-在真空建立及舌頭縮回的位置上沒有硅片
                    ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_BACK)
                    systemSubState = 310
                ElseIf tmr.IsTimerTicked = True Then '慢速退回
                    ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_BACK_SLOW)
                    systemSubState = 310
                End If

            Case 310
                If ForthBack_TongueMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 320
                End If
            Case 320 '送出旗標,等候通之舌頭是否下降
                blnWaferOnTongueReady = True
                systemSubState = 330
            Case 330 '舌頭可以下降
                If Not blnWaferOnTongueReady Then
                    Call writeBit(TongueVacuum, False) '破真空
                    systemSubState = 340
                End If
            Case 340 '舌頭下降
                If UpDownTongueCy.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 350
                End If
            Case 350 '檢查硅片存在感測器
                Dim ap As New alarmContentSensor
                If TongueBackWaferExistSen.IsSensorCovered Then
                    blnWaferExistOnTongueBackPos = True '舌頭縮回的位置上有硅片
                    systemSubState = 360
                Else
                    With ap
                        .Sender = Me    'Hsien , 2015.06.19 , marked the sender
                        .Inputs = TongueBackWaferExistSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     systemSubState = 350
                                                                                     Return True
                                                                                 End Function
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                      systemSubState = 360
                                                                                      Return True
                                                                                  End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                    blnWaferExistOnTongueBackPos = False '舌頭縮回的位置上沒有硅片
                End If
            Case 360
                blnWaferDownOk = True '舌頭下降完成旗標
                systemSubState = 0
                '--------------
                '如果卡匣未備便
                '--------------
            Case 400 '舌頭下降
                If UpDownTongueCy.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 420
                End If
            Case 420 '舌頭縮回
                If ForthBack_TongueMotor.drive(motorControl.motorCommandEnum.GO_POSITION, ForthBackMotorUsedPositions.MOTOR_MOVE_BACK) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 0
                End If
        End Select

        Return 0
    End Function
    Public Function AlignWaferPutOnConveyer() As Integer

        Select Case AlignPut_Step
            Case 0 '檢查舌頭上的硅片是否備便
                If blnWaferOnTongueReady Then
                    AlignPut_Step = 20
                Else
                    Call ConveyerMotionReset() '重置MotionAction
                End If
            Case 20 '檢查是否要升降導軌
                If feedFlags.viewFlag(flagsInLoaderUnloader.GuideDontRaiseWafer_f) Then
                    blnWaferOnTongueReady = False '通知舌頭下降
                    AlignPut_Step = 40
                Else
                    AlignPut_Step = 30
                End If
            Case 30 '定位上升接硅片
                If UpDownAlignerCy.drive(cylinderGeneric.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    blnWaferOnTongueReady = False '通知舌頭下降
                    AlignPut_Step = 40
                End If
            Case 40  '舌頭下降完成
                If blnWaferDownOk Then
                    blnWaferDownOk = False
                    AlignPut_Step = 60
                End If
            Case 60  '定位硅片
                Side_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_CLAMP) '側定位
                Forth_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_CLAMP) '前定位
                AlignPut_Step = 80
            Case 80 '等待馬定位完成
                If Side_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And Forth_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    AlignPut_Step = 100
                End If
            Case 100  '放開硅片
                Side_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_RELEASE)
                Forth_AlignerMotor.drive(motorControl.motorCommandEnum.GO_POSITION, AlignMotorUsedPositions.MOTOR_RELEASE)
                AlignPut_Step = 120
            Case 120 '等待馬放開完成
                If Side_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And Forth_AlignerMotor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    AlignPut_Step = 140
                End If
            Case 140 '定位下降放硅片
                If UpDownAlignerCy.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    AlignPut_Step = 160
                End If
            Case 160  '通知輸送帶可以移動
                If (CassetteUpDownOK()) Then '重置ModuleAction、設定IsPositionOccupied
                    __cycleTime = cycleTimer.TimeElapsed
                    cycleTimer.IsEnabled = True 'restart cycle time calculating , Hsien , 2015.04.15
                    blnAlignerFinish = True
                    AlignPut_Step = 0
                End If
        End Select

        Return 0
    End Function

    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
        stateExecutePlateTongue = [Delegate].Combine(stateExecutePlateTongue,
                                       New Func(Of Integer)(AddressOf TongueGetWafer),
                                       New Func(Of Integer)(AddressOf AlignWaferPutOnConveyer))

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgniteBeltTongue       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecuteBeltTongue  '鍊結主狀態函式
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        feedFlags.writeFlag(flagsInLoaderUnloader.CyBackStatus, True) '
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        initEnableAllDrives()

        'Hsien , disable sensors to enhance performance , 2015.09.09
        actionComponents.ForEach(Sub(__drive As driveBase)
                                     Dim __sensor As sensorControl = TryCast(__drive, sensorControl)
                                     If (__sensor IsNot Nothing) Then
                                         __sensor.IsEnabled = False
                                     End If
                                 End Sub)

        Return 0
    End Function
End Class



