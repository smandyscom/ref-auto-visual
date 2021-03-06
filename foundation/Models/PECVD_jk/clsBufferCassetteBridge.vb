﻿Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Imports Automation.Components.Services

''' <summary>
''' 建立下料區 卡匣與buffer之間的bridge 使得buffer與卡匣可同時作動，而不會互卡
''' </summary>
''' <remarks></remarks>
Public Class clsUnloadingBufferCassetteBridge
    Inherits systemControlPrototype


#Region "Device declare"
    Public Property ConveyorBufferPositionInfo As Func(Of shiftDataPackBase)
    Public Property ConveyorCassettePositionInfo As Func(Of shiftDataPackBase)
    Public Property ToBufferPositionInfo As shiftDataPackBase
    Public Property ToCassettePositionInfo As shiftDataPackBase
    Public Property flags As New flagController(Of enumFlag)
    Public Property unloadingBufferReference As clsUnloadingBuffer
    Public Property unloadingCassetteReference As cassetteSystemBase
    Public Property wafersBetweenBufferCassette As New List(Of Func(Of shiftDataPackBase)) 'buffer到卡匣之間還有幾片，包含舌頭伸出至卡匣內的位置，共兩片
    Public Event DisappearWafer As EventHandler
    Public TongueJamSensorBit As ULong
#End Region
    Enum enumFlag
        REJECT_CASSETTE
        BUFFER_CAN_STORE
    End Enum
#Region "External Data declare"

#End Region
#Region "Internal Data declare"
    Dim tmr As New singleTimer
    Dim blnRetryCheckJamSensor As Boolean
    Dim _checkJamSensorStep As enumCheckJam
    ReadOnly Property checkJamSensorStep As enumCheckJam
        Get
            Return _checkJamSensorStep
        End Get
    End Property
#End Region
    Private Function ignite(ByRef cStep As igniteEnum) As Integer
        Select Case cStep
            Case igniteEnum._0
                unloadingBufferReference.unloadingCassetteSayBufferCanStore = Function() (flags.viewFlag(enumFlag.BUFFER_CAN_STORE)) '設定下料BUFFER是否要存片要看此flag
                systemMainState = systemStatesEnum.EXECUTE
        End Select
        Return 0
    End Function
    Protected Function execute(ByRef cStep As executeEnum) As Integer
        checkWaferJam()
        Select Case cStep
            Case executeEnum._0
                If ConveyorBufferPositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    '檢查卡匣是否準備好，buffer是否可放片
                    '先檢查是否塞片報警，若塞片，則報警，但仍然讓buffer存片
                    If CentralAlarmObject.IsAlarmed = True Then
                        flags.writeFlag(enumFlag.BUFFER_CAN_STORE, True) '通知Buffer存片
                    ElseIf flags.readFlag(enumFlag.REJECT_CASSETTE) = True Then  '檢查是否要退出卡匣
                        flags.writeFlag(enumFlag.BUFFER_CAN_STORE, True) '通知Buffer存片
                        unloadingCassetteReference.commonFlags.writeFlag(flagsInLoaderUnloader.CasCollect_f, True) '通知卡匣收料
                    Else '計算卡匣是否快要滿料
                        Dim waferCountOnWay As Integer
                        For Each obj As Func(Of shiftDataPackBase) In wafersBetweenBufferCassette
                            If obj.Invoke.IsPositionOccupied = True Then waferCountOnWay += 1
                        Next
                        If unloadingCassetteReference._cassetteLift.cntWafer + waferCountOnWay >= unloadingCassetteReference._cassetteLift.GoalCount Then
                            flags.writeFlag(enumFlag.BUFFER_CAN_STORE, True) '通知Buffer存片
                        ElseIf unloadingCassetteReference.commonFlags.viewFlag(flagsInLoaderUnloader.CyBackStatus) = False AndAlso
                            unloadingCassetteReference.commonFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f) = False Then
                            flags.writeFlag(enumFlag.BUFFER_CAN_STORE, False) '通知Buffer放片
                        Else
                            flags.writeFlag(enumFlag.BUFFER_CAN_STORE, True) '通知Buffer存片
                        End If
                    End If
                    '將wafer資料轉移至buffer及卡匣
                    ToBufferPositionInfo.Assign(ConveyorBufferPositionInfo.Invoke)
                    ToCassettePositionInfo.Assign(ConveyorCassettePositionInfo.Invoke)
                    cStep = executeEnum._10
                End If
            Case executeEnum._10 '等待卡匣及buffer完成動作
                If ToBufferPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False AndAlso
                    ToCassettePositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    ConveyorBufferPositionInfo.Invoke.Assign(ToBufferPositionInfo)
                    ConveyorCassettePositionInfo.Invoke.Assign(ToCassettePositionInfo)
                    cStep = executeEnum._0
                ElseIf unloadingBufferReference.controlFlags.viewFlag(clsUnloadingBuffer.controlFlagsEnum.IS_FULL) = True Then '檢查buffer是否滿料
                    '查看卡匣是否可以收片(舌頭是否伸出) 且沒報警
                    If unloadingCassetteReference.commonFlags.viewFlag(flagsInLoaderUnloader.CyBackStatus) = False AndAlso
                            CentralAlarmObject.IsAlarmed = False AndAlso
                            unloadingCassetteReference.commonFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f) = False Then

                        flags.writeFlag(enumFlag.BUFFER_CAN_STORE, False) '通知buffer放片
                    End If
                ElseIf CentralAlarmObject.IsAlarmed = True Then 'alarm occured , reset cassette module action
                    ToCassettePositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                End If
        End Select
        Return 0
    End Function
    Protected Function checkWaferJam()
        Select Case _checkJamSensorStep
            Case enumCheckJam._0

                If CentralAlarmObject.IsAlarmed = False AndAlso ConveyorCassettePositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True AndAlso readBit(TongueJamSensorBit) = True Then
                    Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = TongueJamSensorBit,
                                           .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF,
                                           .AdditionalInfo = "Please let this sensor off!",
                                           .PossibleResponse = alarmContextBase.responseWays.RETRY}
                    With ap
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     If readBit(TongueJamSensorBit) = True Then
                                                                                         ToCassettePositionInfo.IsPositionOccupied = True
                                                                                         ConveyorCassettePositionInfo.Invoke.IsPositionOccupied = True
                                                                                     End If
                                                                                     Return True
                                                                                 End Function
                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If




                If CentralAlarmObject.IsAlarmed = True Then
                    'auto reset cassette module action and clear wafers on conveyor
                    ConveyorCassettePositionInfo.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) '清除卡匣上的module action
                    If ConveyorCassettePositionInfo.Invoke.IsPositionOccupied Then
                        RaiseEvent DisappearWafer(Me, Nothing)
                        ConveyorCassettePositionInfo.Invoke.IsPositionOccupied = False '清除卡匣上的wafer
                    End If

                End If
        End Select
        Return 0
    End Function
    Protected Overrides Function process() As Integer

        Me.CentralAlarmObject.alarmHandling()
        'Me.PauseBlock.pauseHandling()
        '不受暫停 報警影響
        'If (CentralAlarmObject.IsAlarmed OrElse
        '    PauseBlock.IsPaused) Then
        '    Return 0
        'End If
        'MyBase.process()
        drivesRunningInvoke()
        stateControl()
        'processProgress()
        Return 0
    End Function
    Overridable Function initMappingAndSetup() As Integer

        'Me.relatedFlags.AddRange(SynchronFlags.FlagElementsArray)
        'If (UpstreamNode IsNot Nothing) Then
        '    Me.relatedFlags.AddRange(UpstreamNode.SynchronFlags.FlagElementsArray) 'relatingFlags
        'End If
        '預先定義歸Home時，若進出料的位置有片，則馬達立即停止歸Home，並報警
        systemMainStateFunctions(systemStatesEnum.IGNITE) = Function() (ignite(systemSubState))
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (execute(systemSubState))
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub


    Enum igniteEnum

        _0

    End Enum
    Enum executeEnum
        _0

        _10

        _20

        _30

        _40

        _100

        _200

        _110

        _120

        _210

        _300

        _220

   

    End Enum
    Enum enumCheckJam
        _0
    End Enum
End Class
