﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.mainIOHardware

Public Class clsUnloadingBuffer
    Inherits systemControlPrototype
    Implements IModuleMulti
    Implements IFinishableStation

    Public Enum enTargetPositions
        BUFFER_POSITION = 0
        CASSETTE_POSITION = 1
    End Enum
    Public Enum motorPositions
        TOP '最上片位置
        PITCH '每片間距
    End Enum

#Region "Flag declare"
    Public Enum controlFlagsEnum
        FIRST_CYCLE_DOWN
        IS_ENABLE 'buffer enable
        IS_FULL 'buffer is full or not
    End Enum
#End Region
#Region "External Data declare"
    Property controlFlags As flagController(Of controlFlagsEnum) = New flagController(Of controlFlagsEnum)
    Public Property TargetPositions As New List(Of Func(Of shiftDataPackBase)) Implements IModuleMulti.TargetPositionInfo
    Property waferRealCapability As Short = 20  '此Buffer設計最大可存片數
    Property waferSettingCapability As Short = 15 - 1  '此Buffer設定可存片數(必須要空1片當做卡匣預先補片的空間)
    Public unloadingCassetteSayBufferCanStore As Func(Of Boolean) 'ready link to cassette handler 'todo jk note: cassetteReady是函數指標，需要在Assembly中實做。 查看卡匣是否Ready
    Property shiftDataType As Type = GetType(shiftDataPackBase) 'jk note: type 後 可以不用寫預設型別
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    ''' <summary>
    ''' Used to check if down stream status had been done , then decide whether to store wafter
    ''' default : no need to synchron , compatible with previous version
    ''' Hsien , 2016.09.08
    ''' </summary>
    ''' <remarks></remarks>
    Public synchronizeWithDownstream As stateFunction = Function() (True)
    Protected synchronState As Integer = 0
#End Region
#Region "data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Dim __top As cMotorPoint  '= motorBuffer.enum2cPoint(motorPositions.TOP)
    Dim __pitch As cMotorPoint  '= motorBuffer.enum2cPoint(motorPositions.PITCH)
    Dim motorPoint As cMotorPoint '計算最後的馬達點位
    Property WaferCount As Short = 0 '目前Buffer存片數量
    Property OccupiedStatus As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase) 'Buffer本身的wafer queue
    Public Event LoadQueue As EventHandler
    Public Event Stored As EventHandler
    Public Event Outputted As EventHandler
#End Region

#Region "Device declare"
    Property motorBuffer As motorControl = New motorControl
    Property SpBuffer As sensorControl = New sensorControl 'buffer正下方sensor
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
                If SpBuffer.IsSensorCovered = True Then
                    Dim ap As New alarmContentSensor
                    With ap
                        .Sender = Me
                        .Inputs = SpBuffer.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                Else
                    systemSubState = 20
                End If
            Case 20
                WaferCount = waferRealCapability '假設wafer都填滿buffer
                controlFlags.setFlag(controlFlagsEnum.FIRST_CYCLE_DOWN)
                controlFlags.setFlag(controlFlagsEnum.IS_FULL)
                __top = motorBuffer.enum2cPoint(motorPositions.TOP)
                __pitch = motorBuffer.enum2cPoint(motorPositions.PITCH)
                systemSubState = 30

            Case 30
                '片片向下尋找
                '向下前檢查輸送帶上的sensor是否ON
                If WaferCount > 0 Then
                    WaferCount -= 1
                    motorPoint = __top.Clone()
                    motorPoint.Distance = __top.Distance + (WaferCount) * __pitch.Distance
                    motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)
                    systemSubState = 40
                Else
                    systemSubState = 100
                End If

            Case 40
                If motorBuffer.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 50
                End If
            Case 50
                If SpBuffer.IsSensorCovered = False Then '判斷sensor off 且 持續一段時間
                    If SpBuffer.OffTimer.TimeElapsed > New TimeSpan(TimeSpan.TicksPerSecond * 0.5) Then
                        systemSubState = 60
                    End If
                Else
                    '找到最後一片了
                    WaferCount += 1
                    systemSubState = 55 '再向上一格
                End If
            Case 55
                motorPoint = __top.Clone()
                motorPoint.Distance = __top.Distance + (WaferCount) * __pitch.Distance
                motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)
                systemSubState = 57
            Case 57
                If motorBuffer.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 100
                End If

            Case 60
                If WaferCount < waferSettingCapability Then
                    controlFlags.writeFlag(controlFlagsEnum.IS_FULL, False) '清除滿片
                End If
                If WaferCount = 0 Then '吐完
                    controlFlags.resetFlag(controlFlagsEnum.FIRST_CYCLE_DOWN) '清除第一輪旗標
                End If
                systemSubState = 30
            Case 100
                '若需要加MES，則要載入先前的OccupiedStatus queue
                '載入先前儲存的queue，要與waferCount數量相符
                OccupiedStatus.Clear()
                For i As Integer = 0 To WaferCount - 1
                    OccupiedStatus.Add(Activator.CreateInstance(shiftDataType))
                Next
                RaiseEvent LoadQueue(Me, Nothing)
                FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
        End Select
        Return 0
    End Function

    Protected Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                '檢查該站任務'waiting cassette module action done
                If TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True AndAlso
                             TargetPositions(enTargetPositions.CASSETTE_POSITION).Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    If controlFlags.viewFlag(controlFlagsEnum.IS_ENABLE) = True Then
                        systemSubState = 10
                    Else
                        systemSubState = 300
                    End If
                End If

            Case 10
                If TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.IsPositionOccupied = True Then '該站有片，檢查是否要存片
                    systemSubState = 50
                Else
                    '該站無片，檢查是否要放片
                    systemSubState = 200
                End If
                '--------------------------
                '   Synchron State
                '-------------------------
            Case 50
                If synchronizeWithDownstream(synchronState) Then
                    synchronState = 0 ' reset
                    systemSubState = 100
                Else
                    ''------------------
                    '   Synchronizing
                    '-------------------    
                End If
            Case 100 '判斷是否存片
                '檢查 出料卡匣快滿了 且 未存滿
                '若 buffer已到達設定容量，則等待卡匣再次可放片 <-- 會一直卡在這裡，使輸送帶無法得到module action完成
                If unloadingCassetteSayBufferCanStore() = True Then
                    If OccupiedStatus.Count < waferSettingCapability Then
                        systemSubState = 110
                    Else
                        'Hsien , 2015.05.21 , Bug , 若Cassette到達49片 且 Buffer已存滿 ---->死結
                    End If
                Else
                    systemSubState = 300 '不存片，直接pass
                End If
            Case 110 '不需要檢查sensor，因為conveyor會檢查
                motorPoint = __top.Clone()
                motorPoint.Distance = __top.Distance + (WaferCount + 1) * __pitch.Distance
                'motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)   'Hsien , 2015.07.07 , move to next step
                systemSubState = 120
            Case 120
                If motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 130
                End If
            Case 130 '存片位置走完
                WaferCount += 1
                'OccupiedStatus.Add(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.Clone())
                OccupiedStatus.Add(CTypeDynamic(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke, shiftDataType).Clone)    'Hsien , 2015.08.11
                TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.IsPositionOccupied = False '清除該片
                If WaferCount >= waferSettingCapability Then controlFlags.setFlag(controlFlagsEnum.IS_FULL)
                RaiseEvent Stored(Me, Nothing)
                systemSubState = 300

            Case 200 '判斷是否放片
                '檢查buffe內有片
                If WaferCount > 0 AndAlso unloadingCassetteSayBufferCanStore() = False Then
                    systemSubState = 210
                Else
                    systemSubState = 300 '不放片，直接pass
                End If
            Case 210
                motorPoint = __top.Clone()
                motorPoint.Distance = __top.Distance + (WaferCount - 1) * __pitch.Distance  'Hsien , 為何是從最下面往上算？仍然有壓破第一格的可能,2015.05.18
                motorBuffer.drive(motorControl.motorCommandEnum.GO_POSITION, motorPoint)
                systemSubState = 220
            Case 220
                If motorBuffer.CommandEndStatus = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 230
                End If
            Case 230 '第一輪放片清料時的特別處理
                If controlFlags.viewFlag(controlFlagsEnum.FIRST_CYCLE_DOWN) = True Then '第一輪放片清料，可依sensor on off判斷wafer有無
                    If SpBuffer.IsSensorCovered = False Then '判斷sensor off 且 持續一段時間
                        If SpBuffer.OffTimer.TimeElapsed > New TimeSpan(TimeSpan.TicksPerSecond * 0.5) Then
                            OccupiedStatus.Last.IsPositionOccupied = False
                            systemSubState = 240
                        End If
                    Else
                        'CType(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke, IValueAssignable).Assign(OccupiedStatus.Last) 'copy the buttom position to target position
                        OccupiedStatus.Last.IsPositionOccupied = True '設為有片
                        CTypeDynamic(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke, shiftDataType).Assign(OccupiedStatus.Last)  'Hsien , 2015.08.11 ,should in dynamic type conversion
                        systemSubState = 240
                    End If
                Else
                    systemSubState = 240
                End If
            Case 240
                WaferCount -= 1
                'CType(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke, IValueAssignable).Assign(OccupiedStatus.Last) 'copy the buttom position to target position
                'TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.Assign(OccupiedStatus.Last) 'copy the buttom position to target position
                CTypeDynamic(TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke, shiftDataType).Assign(OccupiedStatus.Last)  'Hsien , 2015.08.11 ,should in dynamic type conversion

                OccupiedStatus.Remove(OccupiedStatus.Last)
                RaiseEvent Outputted(Me, Nothing)
                If WaferCount < waferSettingCapability Then
                    controlFlags.writeFlag(controlFlagsEnum.IS_FULL, False) '清除滿片
                End If
                If WaferCount = 0 Then '吐完
                    controlFlags.resetFlag(controlFlagsEnum.FIRST_CYCLE_DOWN) '清除第一輪旗標
                End If

                systemSubState = 300


            Case 300 'reset module action
                TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)

                systemSubState = 301


            Case 301
                Dim blnTrueForAll As Boolean = True
                If UpstreamStations.Count > 0 Then
                    For Each obj As IFinishableStation In UpstreamStations
                        If obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                            blnTrueForAll = False
                            Exit For
                        End If
                    Next
                Else
                    blnTrueForAll = False
                End If

                If blnTrueForAll = True AndAlso WaferCount = 0 AndAlso unloadingCassetteSayBufferCanStore() = False Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True)
                    systemSubState = 302
                Else
                    systemSubState = 0
                End If
            Case 302 'wait for finish clear
                TargetPositions(enTargetPositions.BUFFER_POSITION).Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED) 'auto reply module action

                Dim blnExit As Boolean = False
                For Each obj As IFinishableStation In UpstreamStations
                    If obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                        blnExit = True
                        Exit For
                    End If
                Next

                If blnExit = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False)
                    systemSubState = 0
                End If

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

    Function initMappingAndSetup() As Integer

        '==建立wafer queue 長度
        OccupiedStatus.Clear()
        For i = 0 To waferRealCapability - 1
            OccupiedStatus.Add(Activator.CreateInstance(shiftDataType)) '為何是shiftDataType而不是shiftDataPackBase? 因為實際加入的成員可能是繼承shiftDataPackBase的種類
            'OccupiedStatus.Add(New shiftDataPackBase) '這樣的方式無法加入衍生的類別
        Next

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE


        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
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
