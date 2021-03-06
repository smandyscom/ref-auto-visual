﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components
Imports Automation.mainIOHardware

Public Enum SideStatus
    is_A
    is_B
End Enum
Public Enum RotatryMotorUsedPositions
    MOTOR_HOME
    MOTOR_POSITION_A
    MOTOR_POSITION_B
    MOTOR_POSITION_C
End Enum


Public Class StackWaferPick : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public PickFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Const SuckerNum As Short = 2
    Public Enum CheckWaferErrorSen
        SEN_1
        SEN_2
        SEN_3
        SEN_4
        SEN_5
    End Enum
    Public Enum PickDevice
        CYLINDER
        MOTOR
    End Enum

    Public Rotatry_Motor As New motorControl With {.IsEnabled = True} '旋轉馬達
    Public Cam_Motor As New motorControl With {.IsEnabled = True} '旋轉馬達

    Public WaferErrorSen As SensorInfo() = New SensorInfo(4) {New SensorInfo With {.sw = IS_OFF},
                                                              New SensorInfo With {.sw = IS_OFF},
                                                              New SensorInfo With {.sw = IS_OFF},
                                                              New SensorInfo With {.sw = IS_OFF},
                                                              New SensorInfo With {.sw = IS_OFF}} '檢查硅片有無在不正確位置上
    Private SensorIndex As Integer
    Public WaferExistSen As sensorControl = New sensorControl With {.IsEnabled = True} '堆疊冶具內是否存在硅片感測器

    Public UpDownCylinder As New cylinderGeneric With {.IsEnabled = True,
                                                       .IsMonitorSensor = True} '取硅片上下氣缸
    Public VacGenerate(SuckerNum) As ULong '真空產生電磁閥
    Public VacSeneor(SuckerNum) As sensorControl  '真空感測器

    Public ConBlowSol As ULong '連續吹氣電磁閥
    Public IntervalBlowSol As ULong '間斷吹氣電磁閥

    Private VacFailureCnt As Short '取片失敗次數記數器
    Private blnNowVacFailure As Boolean '當下取片是否失敗
    Private BlowCnt As Short '間斷吹氣次數記數器

    Public SetBlowTimes As Short = 3 '間斷吹氣的次數,3次
    Public SetBlowOnTime As Single = 50 '間斷吹氣On的時間,100ms
    Public SetBlowOffTime As Single = 50 '間斷吹氣Off的時間,100ms

    Public AfterBlowWaferDelayTime As Integer = 100
    Public preBlowDuration As TimeSpan = New TimeSpan(0, 0, 2)

    Public blnEnableUpFirstPos As Boolean = False

    Public blnReadyToPlaceWafer(SuckerNum) As Boolean '已吸取硅片且在放片位置
    Public blnWaferReadyOnSucker(SuckerNum) As Boolean '已吸取硅片
    Public PickDeviceSelect As PickDevice = PickDevice.CYLINDER
    Private NowPickSideIndex As SideStatus = SideStatus.is_A '旋轉馬達起始位置
    Public NowPlaceSideIndex As SideStatus = SideStatus.is_B '旋轉馬達起始位置

    Public Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0
                If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 10
                End If
            Case 10 '選擇使用的下吸裝置
                Select Case PickDeviceSelect
                    Case PickDevice.CYLINDER
                        systemSubState = 20
                    Case PickDevice.MOTOR
                        systemSubState = 30
                End Select
                '--------------------------------------------------------
            Case 20 '氣缸上升
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 50
                End If
                '--------------------------------------------------------
            Case 30 '上下馬達回原點
                If (Cam_Motor.drive(motorControl.motorCommandEnum.GO_HOME, RotatryMotorUsedPositions.MOTOR_HOME) =
                                    motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 40
                End If
            Case 40 '上下馬達移到上位
                If (Cam_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, RotatryMotorUsedPositions.MOTOR_POSITION_C) =
                                       motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 50
                End If
                '--------------------------------------------------------
            Case 50 '旋轉馬達回原點
                If (Rotatry_Motor.drive(motorControl.motorCommandEnum.GO_HOME, RotatryMotorUsedPositions.MOTOR_HOME) =
                                        motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 60
                End If
            Case 60 '旋轉馬達到工作點
                If (Rotatry_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, RotatryMotorUsedPositions.MOTOR_POSITION_A) =
                                        motorControl.statusEnum.EXECUTION_END) Then
                    _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)   'Hsien , 2015.04.09
                    systemMainState = systemStatesEnum.EXECUTE
                    systemSubState = 0
                End If
        End Select
        Return 0
    End Function

    Public Function stateExecute() As Integer

        Select Case systemSubState
            Case 0 '吸取硅片前判別硅片有無掉落在感測器上,如果為On代表有可以硅片斜躺在堆疊冶具上
                SensorIndex = CheckWaferErrorSen.SEN_1
                systemSubState = 10
            Case 10 '判別感測器是否要檢查On或是Off
                If (WaferErrorSen(SensorIndex).sw = IS_ON) Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 20
                Else
                    systemSubState = 30
                End If
            Case 20 '檢查料匣有無掉落在感測器上
                If WaferErrorSen(SensorIndex).status = IS_ON And WaferErrorSen(SensorIndex).sensor.IsSensorCovered Then
                    systemSubState = 30
                ElseIf WaferErrorSen(SensorIndex).status = IS_OFF And (Not WaferErrorSen(SensorIndex).sensor.IsSensorCovered) Then
                    systemSubState = 30
                Else
                    If tmr.IsTimerTicked Then
                        Dim ap As New alarmContentSensor
                        With ap
                            .Sender = Me
                            .Inputs = WaferErrorSen(SensorIndex).sensor.InputBit
                            .PossibleResponse = alarmContextBase.responseWays.RETRY
                            If WaferErrorSen(SensorIndex).status = IS_ON Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                            If WaferErrorSen(SensorIndex).status = IS_OFF Then .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         systemSubState = 0 '重新檢查
                                                                                         Return True
                                                                                     End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    End If
                End If
            Case 30
                SensorIndex = SensorIndex + 1
                If SensorIndex > [Enum].GetValues(GetType(CheckWaferErrorSen)).Length - 1 Then
                    systemSubState = 40
                Else
                    systemSubState = 10
                End If

            Case 40 '檢查是否可以開始取料
                If PickFlags.viewFlag(flagsInLoaderUnloader.WaferReadyToPick_f) Then
                    PickFlags.writeFlag(flagsInLoaderUnloader.PickWaferInProc_f, True)
                    If WaferExistSen.IsSensorCovered Then '檢查冶具內是否有硅片
                        systemSubState = 50
                    Else
                        sendMessage(statusEnum.GENERIC_MESSAGE, "堆疊冶具內無硅片!")
                        PickFlags.writeFlag(flagsInLoaderUnloader.PickWaferInProc_f, False)
                        PickFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, False)
                        PickFlags.writeFlag(flagsInLoaderUnloader.ChangeStack_f, True) '退堆疊冶具
                        systemSubState = 0
                    End If
                End If

                '----------------------
                ' dRIVE dONW THE Cylinder
                ''--------------------- 
            Case 50 '選擇使用的下吸裝置
                Select Case PickDeviceSelect
                    Case PickDevice.CYLINDER
                        systemSubState = 55
                    Case PickDevice.MOTOR
                        systemSubState = 60
                End Select
            Case 55 '上下氣缸往下,下吸硅片
                If UpDownCylinder.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 80
                End If
            Case 60 '上下馬達往下,下吸硅片
                If (Cam_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, RotatryMotorUsedPositions.MOTOR_POSITION_B) =
                                      motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 80
                End If


                '--------------------------------------------------------
            Case 80 '第一次預吹2sec時間
                If Not PickFlags.viewFlag(flagsInLoaderUnloader.FirstTimeToSuck_f) Then
                    PickFlags.writeFlag(flagsInLoaderUnloader.FirstTimeToSuck_f, True) '設定第一次吸取旗標
                    tmr.TimerGoal = preBlowDuration '第一次預吹時間
                    tmr.IsEnabled = True    'restart
                    systemSubState = 90
                Else
                    systemSubState = 100
                End If
            Case 90 '第一次預吹2秒
                If tmr.IsTimerTicked Then
                    systemSubState = 100
                End If
                '--------------------------------------------------------
            Case 100 '真空產生吸取極板,預吹氣關閉
                Call writeBit(VacGenerate(NowPickSideIndex), True) '真空產生吸取極板
                Call writeBit(ConBlowSol, False) '預吹氣關閉
                systemSubState = 120
                '--------------------------------------------------------
            Case 120 '開始點放吹氣
                If BlowCnt >= SetBlowTimes Then
                    BlowCnt = 0
                    Call writeBit(IntervalBlowSol, False) '點吹氣關閉
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, AfterBlowWaferDelayTime)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 150
                Else
                    Call writeBit(IntervalBlowSol, True) '吹氣分離極板啟動
                    BlowCnt = BlowCnt + 1
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, SetBlowOnTime) '吹氣時間
                    tmr.IsEnabled = True    'restart
                    systemSubState = 130
                End If
            Case 130 '等待一段時間
                If tmr.IsTimerTicked Then
                    Call writeBit(IntervalBlowSol, False) '關閉吹氣
                    tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, SetBlowOffTime) '停止吹氣時間
                    tmr.IsEnabled = True    'restart
                    systemSubState = 140
                End If
            Case 140 '等待一段時間,再點放吹氣
                If tmr.IsTimerTicked Then
                    systemSubState = 120
                End If
                '--------------------------------------------------------
            Case 150 '停一時間使硅片落下
                If tmr.IsTimerTicked Then
                    '選擇使用的下吸裝置
                    Select Case PickDeviceSelect
                        Case PickDevice.CYLINDER
                            systemSubState = 160
                        Case PickDevice.MOTOR
                            systemSubState = 170
                    End Select
                End If
            Case 160 '氣缸上升取極板
                If UpDownCylinder.drive(cylinderGeneric.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    'Missed cancel flags , Hsien , 2016.05.09
                    '重置進行處理中旗標
                    PickFlags.writeFlag(flagsInLoaderUnloader.PickWaferInProc_f, False)
                    PickFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, False) '使馬達重新上升
                    systemSubState = 200
                End If
            Case 170 '馬達第一段上升取極板
                If blnEnableUpFirstPos Then
                    If (Cam_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, RotatryMotorUsedPositions.MOTOR_POSITION_A) =
                                    motorControl.statusEnum.EXECUTION_END) Then
                        '重置進行處理中旗標
                        PickFlags.writeFlag(flagsInLoaderUnloader.PickWaferInProc_f, False)
                        PickFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, False) '使馬達重新上升
                        systemSubState = 180
                    End If
                Else
                    systemSubState = 180
                End If
            Case 180 '馬達第二段上升取極板
                If (Cam_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, RotatryMotorUsedPositions.MOTOR_POSITION_C) =
                                   motorControl.statusEnum.EXECUTION_END) Then
                    If Not blnEnableUpFirstPos Then
                        '重置進行處理中旗標
                        PickFlags.writeFlag(flagsInLoaderUnloader.PickWaferInProc_f, False)
                        PickFlags.writeFlag(flagsInLoaderUnloader.WaferReadyToPick_f, False) '使馬達重新上升
                    End If
                    systemSubState = 200
                End If
            Case 200 '檢查真空感測的狀況
                If VacSeneor(NowPickSideIndex).IsSensorCovered Then '檢查真空感測器
                    blnNowVacFailure = False
                    systemSubState = 210
                Else
                    VacFailureCnt = VacFailureCnt + 1
                    If VacFailureCnt >= 3 Then
                        VacFailureCnt = 0
                        Call writeBit(VacGenerate(NowPickSideIndex), False) '真空關閉
                        systemSubState = 1000
                    Else
                        blnNowVacFailure = True '真空失敗再吸一次
                        Call writeBit(VacGenerate(NowPickSideIndex), False) '真空關閉
                        sendMessage(statusEnum.GENERIC_MESSAGE, "真空吸取硅片失敗!")
                        systemSubState = 210
                    End If
                End If
            Case 210 '檢查吸取是否失敗
                If blnNowVacFailure Then
                    blnNowVacFailure = False
                    systemSubState = 0 '失敗再取一次
                Else '成功
                    VacFailureCnt = 0 '吸取失敗計數重置
                    blnWaferReadyOnSucker(NowPickSideIndex) = True
                    systemSubState = 250
                End If
            Case 250 '檢查極板是否放好
                If NowPickSideIndex = SideStatus.is_A And (Not blnReadyToPlaceWafer(SideStatus.is_B)) Then '檢查對邊Station2的極板是否放完成
                    systemSubState = 260
                End If
                If NowPickSideIndex = SideStatus.is_B And (Not blnReadyToPlaceWafer(SideStatus.is_A)) Then '檢查對邊Station1的極板是否放完成
                    systemSubState = 260
                End If
            Case 260 '馬達旋轉
                If Rotatry_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, _
                IIf(NowPickSideIndex = SideStatus.is_A, RotatryMotorUsedPositions.MOTOR_POSITION_B, RotatryMotorUsedPositions.MOTOR_POSITION_A)) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 280
                Else

                End If
            Case 270
                Dim ap As New alarmContentSensor
                With ap
                    .Inputs = VacSeneor(NowPickSideIndex).InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 260
                                                                                 Return True
                                                                             End Function

                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 280 '設定抓片狀況及重新設定取硅片邊
                blnReadyToPlaceWafer(NowPickSideIndex) = True '放置完成旗標致能
                NowPlaceSideIndex = NowPickSideIndex
                NowPickSideIndex = IIf(NowPickSideIndex = SideStatus.is_A, SideStatus.is_B, SideStatus.is_A) '設定取極板的邊
                systemSubState = 0
            Case 1000 '錯誤顯示
                Dim ap As New alarmContentSensor
                With ap
                    .Inputs = VacSeneor(NowPickSideIndex).InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 blnNowVacFailure = True '真空失敗再吸一次
                                                                                 PickFlags.writeFlag(flagsInLoaderUnloader.FirstTimeToSuck_f, False) '設定第一次吸取旗標
                                                                                 systemSubState = 210
                                                                                 Return True
                                                                             End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  blnNowVacFailure = False
                                                                                  systemSubState = 210
                                                                                  Return True
                                                                              End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
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
End Class




