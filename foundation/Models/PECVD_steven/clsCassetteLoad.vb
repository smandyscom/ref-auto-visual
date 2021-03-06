﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components

Public Enum LoadConveyerUsedPositions
    MOTOR_POSITION_1
    'MOTOR_POSITION_2
End Enum

Public Class SensorInfo
    Public sensor As sensorControl = Nothing
    Public sw As Boolean = False
    Public status As Boolean = False
End Class
Public Class ActuatorInfo
    Public Actuator As ULong
    Public sw As Boolean = False
End Class

Public Class CassetteLoad : Inherits systemControlPrototype
    Implements IFinishableStation
    Public Enum CheckCasStyleSenIndex
        SEN_1
        SEN_2
        SEN_3
        SEN_4   '   Hsien , 2015.07.03 , added 
        SEN_5   '   Hsien , 2016.03.24 , expand
    End Enum

    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public extensionSequence As Func(Of Boolean) = Function() (True)     'Hsien , used to attach extension sequence , i.e read RFID on ready position
    Public loadFlags As flagController(Of flagsInLoaderUnloader)

    Public timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen

''' <summary>
    ''' 卡匣進入碰到的第1個分料氣缸
    ''' </summary>
    ''' <remarks></remarks>    
Public IN_Stopper1 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True} 'seperator , down: not on lifter transmission , up: on lifter transmission
 ''' <summary>
    ''' 卡匣進入碰到的第2個分料氣缸
    ''' </summary>
    ''' <remarks></remarks>
Public IN_Stopper2 As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}

    'Public IN_ConveyerOverrideSen As sensorControl = New sensorControl
    Public IN_CasStyleCheckSen As SensorInfo() = New SensorInfo(4) {New SensorInfo With {.sw = IS_OFF},
                                                                    New SensorInfo With {.sw = IS_OFF},
                                                                    New SensorInfo With {.sw = IS_OFF},
                                                                    New SensorInfo With {.sw = IS_OFF},
                                                                    New SensorInfo With {.sw = IS_OFF}} 'Hsen , pre-allocating , 2015.06.26
    Private SensorIndex As Integer


    Public IN_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True}   '載出馬達 , Hsien , regular the data type , 2015.06.04
    Public Property IN_PosSen1DelayTime As Single = 0.1 'unit:second
    ''' <summary>
    ''' 卡匣進入碰到的第1個感測器(靠近使用者)
    ''' </summary>
    ''' <remarks></remarks>
    Public IN_ConveyerPosSen1 As sensorControl = New sensorControl '載出卡匣位置確認感測器1    'manaul mode trigger
    ''' <summary>
    ''' 載入卡匣位置確認感測器2 靠近第1個碰到的氣缸
    ''' </summary>
    ''' <remarks></remarks>
    Public IN_ConveyerPosSen2 As sensorControl = New sensorControl '載出卡匣位置確認感測器2    'on Stopper 1
    ''' <summary>
    ''' 載入卡匣位置確認感測器3 靠近第2個碰到的氣缸
    ''' </summary>
    ''' <remarks></remarks>
    Public IN_ConveyerPosSen3 As sensorControl = New sensorControl '載出卡匣位置確認感測器3    'on Stopper 2


    'Public LP_LoadConveyerMove As ULong

    Public alarmPackCassetteStyleError As alarmContentSensor = New alarmContentSensor   'prepared alarm context

    Public Function stateIgnite() As Integer
        If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
            _FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
            systemMainState = systemStatesEnum.EXECUTE
        End If
        Return 0
    End Function

    Public Function stateExecuteMethod2() As Integer

        '==========================================
        ' Cassette Direction   >>>>>>>>>>>>>
        '             Stopper1       Stopper2
        'Sen1          Sen2           Sen3
        '==========================================
        Select Case systemSubState
            Case 0 '檢查料匣備便旗標是否去能
                If Not loadFlags.viewFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f) And _
                       loadFlags.viewFlag(flagsInLoaderUnloader.Start_f) Then
                    systemSubState = 10
                End If
            Case 10 '使氣缸下降
                If IN_Stopper1.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 20
                End If
            Case 20 '檢查是否有料匣或檢查是否有按下入卡匣鈕

                If Not loadFlags.viewFlag(flagsInLoaderUnloader.LoadButtonBusy_f) Then '不在手動載入模式下
                    If IN_ConveyerPosSen3.OnTimer.TimeElapsed.TotalMilliseconds >= 100 Then
                        loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                        systemSubState = 290 '卡匣到位正確位置,檢查卡匣型式是否正確
                    ElseIf IN_ConveyerPosSen2.OnTimer.TimeElapsed.TotalMilliseconds >= 100 Then
                        loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, True) '使載入按鈕失效
                        systemSubState = 160 '使卡匣載入至正確位置
                    End If
                End If

                'sensorControl.activateSensorControl(IN_ConveyerPosSen1, systemSubState = 20)
                'sensorControl.activateSensorControl(IN_ConveyerPosSen2, systemSubState = 20)
                'sensorControl.activateSensorControl(IN_ConveyerPosSen3, systemSubState = 20)


            Case 160 '輸送帶開始移動
                If (IN_ConveyerMotor.drive(motorControl.motorCommandEnum.JOG, LoadConveyerUsedPositions.MOTOR_POSITION_1) =
                    IDrivable.endStatus.EXECUTION_END) Then
                    timer.TimerGoal = New TimeSpan(0, 0, 30)
                    timer.IsEnabled = True    'restart
                    systemSubState = 170
                End If
            Case 170 '如果料匣到達入料輸送帶最前端Sen3
                If IN_ConveyerPosSen3.IsSensorCovered Then
                    timer.TimerGoal = New TimeSpan(0, 0, 2)
                    timer.IsEnabled = True    'restart
                    systemSubState = 280
                ElseIf timer.IsTimerTicked Then
                    '如果逾時到錯誤檢查程序
                    systemSubState = 180
                End If
            Case 180 '輸送帶馬達停止
                If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) =
                    IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 190
                End If
            Case 190 '產生錯誤
                Dim ap As New alarmContentSensor
                With ap
                    .Sender = Me
                    .Inputs = IN_ConveyerPosSen3.InputBit
                    .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 systemSubState = 160
                                                                                 Return True
                                                                             End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  systemSubState = 200
                                                                                  Return True
                                                                              End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End With
            Case 200 '逾時忽略
                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '使載入按鈕致能
                systemSubState = 0
            Case 280 '延遲一段時間，停止運轉
                If timer.IsTimerTicked Then
                    If IN_ConveyerMotor.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = motorControl.statusEnum.EXECUTION_END Then
                        systemSubState = 290
                    End If
                End If
                '----------IN_CasStyleCheckSen----------
            Case 290
                'default : accepted cassette
                loadFlags.setFlag(flagsInLoaderUnloader.IsCassetteStyleAcceptable_f)

                systemSubState = 295
            Case 295
                If (styleSensorCheck(styleSensorCheckState)) Then
                    styleSensorCheckState = 0 'reset
                    systemSubState = 500
                End If
               
                '--------------------------------------------------------
            Case 500 '上升擋料匣
                If IN_Stopper1.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 510
                End If
            Case 510
                If (extensionSequence()) Then
                    'Hsien , used to call extension sequence  , i.e rfid read
                    systemSubState = 560
                End If
            Case 560
                loadFlags.writeFlag(flagsInLoaderUnloader.CasOn_IN_ConveyerReady_f, True)
                loadFlags.writeFlag(flagsInLoaderUnloader.LoadButtonDisable_f, False) '手動鈕致能
                systemSubState = 0
        End Select

        Return 0
    End Function
    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))

        With alarmPackCassetteStyleError
            .Sender = Me
            .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                         styleSensorCheckTimer.IsEnabled = True ' restart timer
                                                                         Return True
                                                                     End Function
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = AddressOf cassetteStyleErrorIgnoreHandler
            .AdditionalInfo = "卡匣形式偵測錯誤"
        End With


    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecuteMethod2     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態

        CType(IN_ConveyerMotor, driveBase).IsEnabled = True

        'enable all sensors
        For Each item As sensorControl In {IN_ConveyerPosSen1,
                                    IN_ConveyerPosSen2,
                                    IN_ConveyerPosSen3}
            item.IsEnabled = True
        Next


            Return 0
    End Function

    Sub pauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        timer.IsEnabled = False '時間計時暫停
    End Sub
    Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        timer.IsEnabled = True '時間計時恢復
    End Sub

    Friend styleSensorCheckTimer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 2)}
    Friend styleSensorCheck As stateFunction = AddressOf styleSensorCheckProcedure
    Dim styleSensorCheckState As Integer = 0
    Friend Function cassetteStyleErrorIgnoreHandler() As Boolean
        'let cassette go , and eject , Hsien , 2015.10.06
        loadFlags.resetFlag(flagsInLoaderUnloader.IsCassetteStyleAcceptable_f)
        styleSensorCheckState = 500
        Return True
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function styleSensorCheckProcedure(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                SensorIndex = CheckCasStyleSenIndex.SEN_1
                state = 10
            Case 10 '判別感測器是否要檢查On或是Off
                If (IN_CasStyleCheckSen(SensorIndex).sw = IS_ON) Then
                    With styleSensorCheckTimer
                        .IsEnabled = True
                    End With
                    state = 20
                Else
                    '----------------------
                    '   Dont care
                    '----------------------
                    state = 100
                End If
            Case 20 '檢查料匣形式防呆感測器
                If IN_CasStyleCheckSen(SensorIndex).status =
                    IN_CasStyleCheckSen(SensorIndex).sensor.IsSensorCovered Then
                    'sensor status coincidence with setup , Hsien  ,2016.04.14

                    state = 100

                ElseIf styleSensorCheckTimer.IsTimerTicked Then
                    With alarmPackCassetteStyleError
                        .Inputs = IN_CasStyleCheckSen(SensorIndex).sensor.InputBit
                        If IN_CasStyleCheckSen(SensorIndex).status = IS_ON Then
                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                        End If
                        If IN_CasStyleCheckSen(SensorIndex).status = IS_OFF Then
                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        End If
                    End With
                    CentralAlarmObject.raisingAlarm(Me.alarmPackCassetteStyleError)
                End If
            Case 100
                SensorIndex = SensorIndex + 1
                'Hsien , follow the enum length adaptlly , 2015.10.06
                If SensorIndex > [Enum].GetValues(GetType(CheckCasStyleSenIndex)).Length - 1 Then
                    state = 500
                Else
                    state = 10 'rewind
                End If
            Case 500
                Return True
        End Select

        Return False
    End Function


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

