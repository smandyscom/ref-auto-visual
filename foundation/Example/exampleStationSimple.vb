Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine

Public Class exampleStationSimple
    Inherits systemControlPrototype

    Public Enum localUsedPositions
        MOTOR_POSITION_1
        MOTOR_POSITION_2
        MOTOR_POSITION_3
    End Enum

    Public Enum controlFlagsEnum
        ABLE_IGNITE
        ACTION
    End Enum

    Public controlFlags As flagController(Of controlFlagsEnum) = New flagController(Of controlFlagsEnum) 'todo jk: 旗標的集合

#Region "control members(componenets used)"
    '本站會用到的控制元件 必須要New
    Dim __sensor As sensorControl = New sensorControl
    Dim __timer As singleTimer = New singleTimer

    Dim __cylinder As cylinderControl = New cylinderControl
    Dim __motor As motorControl = New motorControl
    Dim __motorB As motorControl = New motorControl

    Dim __vacuumGenerator As Integer = outputsEnums.Vg1     'Digital Output輸出

#End Region

    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                '等待外部給予旗標觸發
                If (controlFlags.readFlag(controlFlagsEnum.ABLE_IGNITE)) Then
                    '同時歸原點寫法
                    __motor.drive(motorControl.motorCommandEnum.GO_HOME)
                    __motorB.drive(motorControl.motorCommandEnum.GO_HOME)
                    systemSubState = 10
                End If
            Case 10
                '檢查至兩軸都到位為止
                If (__motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                   __motorB.CommandEndStatus = motorControl.statusEnum.EXECUTION_END) Then
                    systemMainState = systemStatesEnum.EXECUTE  '主狀態轉移
                End If
            Case Else

        End Select

        Return 0
    End Function

    Function stateWorking() As Integer

        Select Case systemSubState
            Case 0
                '等待動作開始旗標
                If (controlFlags.viewFlag(controlFlagsEnum.ACTION)) Then
                    'viewFlag: 只看不清
                    'readFlag:看完就清除
                End If
            Case 0
                '確認到馬達到位為止
                If (__motor.drive(motorControl.motorCommandEnum.GO_POSITION, localUsedPositions.MOTOR_POSITION_1) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 10
                End If
            Case 10
                '同時作動
                __motor.drive(motorControl.motorCommandEnum.GO_POSITION, localUsedPositions.MOTOR_POSITION_1)
                __motorB.drive(motorControl.motorCommandEnum.GO_POSITION, localUsedPositions.MOTOR_POSITION_1)
            Case 20
                If (__motor.CommandEndStatus = motorControl.statusEnum.EXECUTION_END And
                   __motorB.CommandEndStatus = motorControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 30
                End If
            Case 30
                If (__cylinder.drive(cylinderControl.cylinderCommandEnum.GO_ON_END) =
                    cylinderControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 40
                End If
            Case 40
                If (__cylinder.drive(cylinderControl.cylinderCommandEnum.GO_OFF_END) =
                    cylinderControl.statusEnum.EXECUTION_END) Then
                    systemSubState = 50
                End If
            Case 50
                '檢查sensor脈寬(上/下緣之間的時間間隔)是否大於100ms （Debounce能力)
                If (__sensor.OnPulseWidth.Milliseconds > 100) Then '適用於Push Button等(Debounce功能)
                    systemSubState = 60
                Else
                    '警報寫法
                    Dim ap As alarmContentSensor = New alarmContentSensor
                    With ap
                        '--------------------------
                        '   設定回應可能性 以及 回應時處理
                        '--------------------------
                        .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() (True)   'retry , 停在此步序
                        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function()
                                                                                      systemSubState = 30
                                                                                      Return True    'ignore , 改寫步序  , 從改寫後的步序開始
                                                                                  End Function
                        '----------------------------
                        '   設定補充資訊
                        '----------------------------
                        .Inputs = __sensor.InputBit 'digital input類
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON 'should be on
                        'or
                        .Reason = alarmContentSensor.alarmReasonSensor.PULSE_WIDTH_OVERRED '脈寬過長(如on fly 塞片時，sensor恆亮)


                        CentralAlarmObject.raisingAlarm(ap) '發送警報

                    End With

                End If
            Case 60
                '檢查sensor脈寬(上緣至今的時間間隔)是否大於100ms （Debounce能力)
                If (__sensor.OnTimer.TimeElapsed.Milliseconds > 100) Then '目前仍恆on
                    systemSubState = 500
                End If
            Case 70
                DO_SetB(__vacuumGenerator)              '輸出Digital Output
                DO_ClrB(__vacuumGenerator)              '清除Digital Output
                DO_Write(__vacuumGenerator, True)       '寫入Digital Output 等同 DO_SetB(__vacuumGenerator) 
                DO_Write(__vacuumGenerator, False)       '寫入Digital Output 等同 DO_ClrB(__vacuumGenerator) 
            Case 500
                controlFlags.resetFlag(controlFlagsEnum.ACTION)     '工作完成消除旗標
                systemSubState = 0                                  '流程循環
            Case Else

        End Select

        Return 0
    End Function


    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        'todo jk note: 設定initialize中再加入initMappingAndSetup
        '原本的initialze(在systemControlPrototype裡的New裡)已有設定包含三個function={initAddAllDrive,initLinkAlarm,initLinkMessenger}
        '全部設定完後，必須在Assembly下call this instance.initialize
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))

    End Sub

    Function initMappingAndSetup() As Integer

        '馬達初始化設定
        With __motor
            .MotorIndex = motorAddress.MSA1      '馬達軸對應
            .PositionDictionary.Add(localUsedPositions.MOTOR_POSITION_1, motorPoints.MSA1_LOAD)     '在此馬達元件連結點位關係
            .PositionDictionary.Add(localUsedPositions.MOTOR_POSITION_2, motorPoints.MSA1_UNLOAD)
        End With

        'SENSOR初始化設定
        With __sensor
            .InputBit = inputsEnums.PB_START    '設定輸入點
        End With

        '單動電磁閥初始化設定
        With __cylinder
            .OffEndSensor = inputsEnums.Spa0    '磁簧開關設定
            .OnEndSensor = inputsEnums.Spa1
            .ActuatorBit = outputsEnums.Sa0     '輸出點位設定
        End With

        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateWorking     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態

        Return 0
    End Function

    Protected Overrides Function process() As Integer
        Return MyBase.process()
    End Function

End Class
#Region "應放在Assembly裡，以下只是示範"
'-------------------------------------------
'以下為範例資訊 , 實際上應定義在Assembly專案
'-------------------------------------------
Public Enum motorAddress
    MSA1 = 0
    MSA2 = 1
    MSA3 = 2

    MsE1B

End Enum

Public Enum motorPoints
    MSA1_LOAD = 0
    MSA1_UNLOAD = 1
    MSA1_SEND = 2
    MSA2_LOAD = 3
    MSA2_UNLOAD = 4
    MSA2_SEND = 5

    MsE1B_TOP

End Enum


Public Enum inputsEnums
    PB_START
    Spa0
    Spa1
End Enum

Public Enum outputsEnums
    Sa0
    Vg1
End Enum
#End Region
