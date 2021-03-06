﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services


Public Class clsLoadingBufferV2
    Inherits systemControlPrototype
    Implements IModuleMulti
    Implements IFinishableStation


    Public Enum enTargetPositions
        BUFFER_POSITION = 0
    End Enum

    ReadOnly Property WaferCount As Integer
        Get
            Return __waferCount
        End Get
    End Property

#Region "Flag declare"
    Public Enum controlFlagsEnum
        OUT_PROCESS '如果Buffer一旦吐出，就要吐完，以此flag為準
        FIRST_CYCLE_DOWN
        IS_ENABLE 'buffer enable
    End Enum
#End Region
#Region "Device declare"
    Public motorBuffer As motorControl = New motorControl
    Public spBuffer As sensorControl = New sensorControl 'buffer正下方sensor
#End Region
#Region "External Data declare"
    Public controlFlags As flagController(Of controlFlagsEnum) = New flagController(Of controlFlagsEnum)
    Public Property TargetPositions As New List(Of Func(Of shiftDataPackBase)) Implements IModuleMulti.TargetPositionInfo

    'Property WaferRealCapability As Short = 15  '此Buffer設計最大可存片數
    Property WaferSettingCapability As Short = 15  '此Buffer設定可存片數

    Property LastPosition As List(Of Func(Of shiftDataPackBase))
    Public Enum motorPositions
        TOP '最上片位置,abs
        PITCH '每片間距,rel
    End Enum
    Public loadingCassetteReady As Func(Of Boolean) 'ready link to cassette handler
    Public Event Stored As EventHandler
    Public Event Outputted As EventHandler
    Public Event ToAction As EventHandler
#End Region
#Region "Internal Data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim __pointTop As cMotorPoint '= motorBuffer.enum2cPoint(motorPositions.TOP)
    Dim __pointPitch As cMotorPoint '= motorBuffer.enum2cPoint(motorPositions.PITCH)
    Protected __occupiedStatus As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase) 'Buffer本身的wafer queue
    Dim __waferCount As Integer = 0 '目前Buffer存片數量
    Dim motorPoint As cMotorPoint '計算最後的馬達點位
    'Property shiftDataType As Type = GetType(shiftDataPackBase) 'todo jk note: type 後 可以不用寫預設型別
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    ''' <summary>
    ''' 讀取buffer正下方, sensor off必須持續一段時間才認為off
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SenseOffDelayTime As New TimeSpan(TimeSpan.TicksPerSecond * 0.5)
#End Region


    ''' <summary>
    ''' 歸原點時，若發生任何Alarm，馬達motorFlip會立即停止(Pause功能)，解除後會重新運作(Resume)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function stateIgnite() As Integer 'motor homing , cylinder initial position
        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    systemSubState = 5
                End If
            Case 5
                If motorBuffer.drive(motorControl.motorCommandEnum.GO_HOME) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 10
                End If
            Case 10
                If spBuffer.IsSensorCovered = True Then
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = spBuffer.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                Else
                    systemSubState = 100
                End If
            Case 100
                FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                controlFlags.setFlag(controlFlagsEnum.OUT_PROCESS) '預設要先吐完
                controlFlags.setFlag(controlFlagsEnum.FIRST_CYCLE_DOWN)
                __pointPitch = motorBuffer.enum2cPoint(motorPositions.PITCH) '將pData裡的點位指給__pitch
                __pointTop = motorBuffer.enum2cPoint(motorPositions.TOP) '將pData裡的點位指給__top
                __waferCount = __occupiedStatus.Count '假設wafer都填滿buffer
                systemMainState = systemStatesEnum.EXECUTE
        End Select
        Return 0
    End Function

    Protected Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                '檢查該站任務
                If TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    If controlFlags.viewFlag(controlFlagsEnum.IS_ENABLE) = True Then
                        systemSubState = 10
                    Else
                        systemSubState = 300
                    End If
                    '檢查自己是否也沒料了 且 檢查所有的上家站是否都收料結束了
                    'Hsien , Direct bridge finish signals when buffer not enabled , 2015.08.13
                ElseIf (__waferCount = 0 Or Not controlFlags.viewFlag(controlFlagsEnum.IS_ENABLE)) AndAlso
                    UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) = True Then

                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True) '自己也收料結束
                    systemSubState = 2

                End If
            Case 2 '收料結束，等待上家恢復自動執行。若還沒恢復執行，也要自動應答moduel action
                If UpstreamStations.Exists(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False) '清除自己收料結束
                    systemSubState = 0 '恢復自動執行
                Else
                    TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.readFlag(interlockedFlag.POSITION_OCCUPIED) '自動清除module action
                End If
            Case 10
                If TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.IsPositionOccupied = True Then '該站有片，檢查是否要存片
                    systemSubState = 100
                Else '該站無片，檢查是否要放片
                    systemSubState = 200
                End If
                RaiseEvent ToAction(Me, Nothing)
            Case 100 '判斷是否存片
                '檢查輸送帶最末片有片 且 進料卡匣備妥 且 未存滿
                'If TargetPositions(enTargetPositions.LAST_POSITION).Invoke.IsPositionOccupied AndAlso
                If LastPosition.TrueForAll(Function(obj As Func(Of shiftDataPackBase)) (obj.Invoke.IsPositionOccupied = True)) = True AndAlso
                    loadingCassetteReady() = True AndAlso
                    __occupiedStatus.Count < WaferSettingCapability AndAlso
                    controlFlags.viewFlag(controlFlagsEnum.OUT_PROCESS) = False Then
                    systemSubState = 110
                Else
                    systemSubState = 300 '不存片，直接pass
                End If
            Case 110 '不需要檢查sensor，因為conveyor會檢查
                '取得top 與 pitch的馬達點位，最後點位為 top + waferCount*pitch
                motorPoint = __pointTop.Clone()
                motorPoint.Distance = __pointTop.Distance + (__waferCount + 1) * __pointPitch.Distance
                motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)
                systemSubState = 120
            Case 120
                If motorBuffer.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 130
                End If

            Case 130 '存片位置走完
                __waferCount += 1
                With TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke
                    __occupiedStatus.Add(.Clone())
                    .IsPositionOccupied = False '清除該片
                End With
                RaiseEvent Stored(Me, Nothing)
                systemSubState = 300

            Case 200 '判斷是否放片
                '檢查 (進料卡匣未備妥 或 一定要吐完) 且 buffe內有片
                If (loadingCassetteReady() = False OrElse
                    controlFlags.viewFlag(controlFlagsEnum.OUT_PROCESS) = True) AndAlso
                    __waferCount > 0 Then
                    controlFlags.setFlag(controlFlagsEnum.OUT_PROCESS) '設定開始吐片
                    systemSubState = 210
                Else
                    systemSubState = 300 '不放片，直接pass
                End If
            Case 210
                motorPoint = __pointTop.Clone()
                motorPoint.Distance = __pointTop.Distance + (__waferCount - 1) * __pointPitch.Distance
                motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)
                systemSubState = 220
            Case 220
                If motorBuffer.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    tmr.IsEnabled = True : tmr.TimerGoal = SenseOffDelayTime
                    systemSubState = 230
                End If
            Case 230 '第一輪放片清料時的特別處理
                If controlFlags.viewFlag(controlFlagsEnum.FIRST_CYCLE_DOWN) = True Then '第一輪放片清料，可依sensor on off判斷wafer有無
                    If spBuffer.IsSensorCovered = False Then
                        If tmr.IsTimerTicked = True Then
                            TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.IsPositionOccupied = False
                            systemSubState = 240
                        End If
                    Else
                        With TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke
                            .Assign(__occupiedStatus.Last) '設為有片
                            .IsPositionOccupied = True
                        End With
                        systemSubState = 240
                    End If

                Else
                    'Missed data copying , Hsien , 2015.08.12
                    With TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke
                        .Assign(__occupiedStatus.Last)
                        .IsPositionOccupied = True '設為有片
                    End With
                    systemSubState = 240
                End If
                If systemSubState = 240 Then
                    __waferCount -= 1
                    __occupiedStatus.Remove(__occupiedStatus.Last)
                    RaiseEvent Outputted(Me, Nothing)
                End If
            Case 240
                If __waferCount = 0 Then '吐完
                    controlFlags.resetFlag(controlFlagsEnum.OUT_PROCESS) '清除一定要吐完
                    controlFlags.resetFlag(controlFlagsEnum.FIRST_CYCLE_DOWN) '清除第一輪旗標
                End If
                systemSubState = 300
            Case 300 'reset module action
                TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = 0
        End Select
        Return 0
    End Function

    Sub alarmOccursHandler(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured
        If (MainState = systemStatesEnum.IGNITE) Then
            motorBuffer.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        End If
    End Sub

    Sub alarmReleaseHandler(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmReleased
        If (MainState = systemStatesEnum.IGNITE) Then
            motorBuffer.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        End If
    End Sub
    Public Function loadingCassetteCanPut() As Boolean
        If controlFlags.viewFlag(controlFlagsEnum.OUT_PROCESS) = True Then '吐片模式下，剩一片時就要預先吐片
            If __waferCount <= 2 Then
                Return True
            End If
        Else '不是在吐片模式，卡匣就可以放片
            Return True
        End If
        Return False
    End Function

    Public Sub New(dataType As Type,
                   Optional waferRealCapability As Integer = 20)
        '預先定義歸Home時，若進出料的位置有片，則馬達立即停止歸Home，並報警

        '==建立wafer queue 長度
        __occupiedStatus.Clear()
        For i = 0 To waferRealCapability - 1
            __occupiedStatus.Add(Activator.CreateInstance(dataType)) '為何是shiftDataType而不是shiftDataPackBase? 因為實際加入的成員可能是繼承shiftDataPackBase的種類
            'OccupiedStatus.Add(New shiftDataPackBase) '這樣的方式無法加入衍生的類別
        Next

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initEnableAllDrives))
    End Sub

End Class

#Region "必須放在外面的"
#If 1 = 0 Then
Public Class TestBench
    Sub Test()
        Dim MyFlipper As clsFlipper = New clsFlipper
        MyFlipper.shiftDataType = GetType(waferDataGintechPECVD)
    End Sub
End Class
#End If

#End Region

