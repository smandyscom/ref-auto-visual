Imports Automation.Components.Services

Public Class loadingCassetteTongueBuffer
    Inherits systemControlPrototype
    Implements IFinishableStation
    Implements ICassetteTongueBuffer


    Enum positionEnum
        CASSETTE_POSITION = 0
        MIDDLE_POSITION = 1
        BUFFER_POSITION = 2
    End Enum

    Public ReadOnly Property BufferCounts As Integer Implements ICassetteTongueBuffer.BufferCounts
        Get
            Return buffer.WaferCount
        End Get
    End Property

    Public ReadOnly Property Cassette As cassetteSystemBase Implements ICassetteTongueBuffer.Cassette
        Get
            Return cassetteSubsystem
        End Get
    End Property

    Public ReadOnly Property Tongue As shiftingModel Implements ICassetteTongueBuffer.Tongue
        Get
            Return conveyorTongue
        End Get
    End Property

    Property IsStarted As Boolean
        Get
            Return cassetteSubsystem.commonFlags.viewFlag(flagsInLoaderUnloader.Start_f)
        End Get
        Set(value As Boolean)
            cassetteSubsystem.commonFlags.writeFlag(flagsInLoaderUnloader.Start_f, value)
        End Set
    End Property

    Public Property FinishableFlags As flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
        Get
            Return buffer.FinishableFlags
        End Get
        Set(value As flagController(Of IFinishableStation.controlFlags))
            'do nothing
        End Set
    End Property

    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
        Get
            Return cassetteSubsystem.UpstreamStation
        End Get
        Set(value As List(Of IFinishableStation))
            cassetteSubsystem.UpstreamStation = value
        End Set
    End Property

    Dim dataType As Type = Nothing
    Dim WithEvents ___cassetteController As CassetteTransport
#Region "control members"

    Public cassetteSubsystem As cassetteSystemBase = New cassetteSystemBase With {.WorkingType = cassetteSystemBase.workingTypeEnum.AS_LOADER}
    Public buffer As clsLoadingBufferV2 = Nothing
    Public conveyorTongue As conveyorTongue = New conveyorTongue

    Dim waferLoadingTimer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 500)}  'Hsien , 2015.04.30 , used to delay-sensing
    Public waferLoadingTonqueSenser As sensorControl = New sensorControl
    Public tongueJammedSensor As sensorControl = New sensorControl
    Public bufferSensor As sensorControl = New sensorControl

#End Region

    Public Sub New(dataType As Type)
        Me.dataType = dataType
        buffer = New clsLoadingBufferV2(Me.dataType)
        Me.initialize = [Delegate].Combine(Me.initialize,
                                       New Func(Of Integer)(AddressOf initLinkPause),
                               New Func(Of Integer)(AddressOf initMappingAndSetup),
                               New Func(Of Integer)(AddressOf initSubsystemInitialize),
                               New Func(Of Integer)(AddressOf initEnableAllDrives),
                               New Func(Of Integer)(AddressOf initPostLink))
    End Sub

    Function initMappingAndSetup() As Integer


        With conveyorTongue
            .Capability = 3
            .Flags.writeFlag(clsSynchronizableTransporterPullTypeV2.FlagsEnum.AUTO_CLEAN, True)     'for the first conveyor of loader, need to resend action to get wafer , Hsien , 2015.04.28
            .ShiftFlags.writeFlag(shiftingModel.shifingModelFlags.IS_BACKUPDATA, False) 'set auto save restore data flow status
            '-------------------------------------
            '   Conveyor Data Verification
            '-------------------------------------
            .checkListWaferCoverSensors.AddRange({
                                              New KeyValuePair(Of Integer, sensorControl)(2, bufferSensor)
                                              })
            .checkListWaferThroughSensors.AddRange({
                                                New KeyValuePair(Of Integer, sensorControl)(1, tongueJammedSensor)
                                                })
            .ShiftDataType = dataType
            .IncomingShiftData = Activator.CreateInstance(dataType)
            '--------------------------------------
            '   Module Link Status
            '--------------------------------------
            .moduleLinkedStatus(positionEnum.CASSETTE_POSITION) = True 'for loading cassette
            .moduleLinkedStatus(positionEnum.BUFFER_POSITION) = True 'for loading buffer
            .UpstreamStations = New List(Of IFinishableStation) From {buffer}
        End With


        With buffer
            .TargetPositions = New List(Of Func(Of shiftDataPackBase)) From {Function() (conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BUFFER_POSITION))}
            .controlFlags.writeFlag(clsLoadingBufferV2.controlFlagsEnum.IS_ENABLE, True)
            .spBuffer = bufferSensor
            .loadingCassetteReady = Function() As Boolean
                                        Return ((Not cassetteSubsystem.commonFlags.viewFlag(flagsInLoaderUnloader.CyBackReady_f)) And _
                                                            cassetteSubsystem.commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f))
                                    End Function
            .UpstreamStations = New List(Of IFinishableStation) From {cassetteSubsystem._cassetteTransport}
        End With


        'configure relation between conveyorSystem and Cassette
        '下料卡匣可升降條件判別
        With cassetteSubsystem
            ._cassetteFeed.ConveyerMotionOkCasAction = AddressOf conveyorModuleAction 'sensing the conveyor signal
            .ConveyerWaferEmpty = Function() (Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION).IsPositionOccupied And
                                              Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.MIDDLE_POSITION).IsPositionOccupied) '檢查載入/載出舌頭輸送帶上的硅片是否存在以決定按清料時是否退卡匣
            ._cassetteFeed.CassetteUpDownOK = AddressOf cassetteStepDownFinished '升降馬達移動完成後,把記憶資料重置
            ._cassetteFeed.CheckWaferOnConveyerInCassette = Function() (Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION).IsPositionOccupied)
            'after wafer step down , should release conveyor
            ._cassetteFeed.ConveyerMotionReset = Sub() conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION).ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
        End With


        With AlarmEmptyCassette
            .Inputs = waferLoadingTonqueSenser.InputBit
            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() (True)
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = AddressOf ejectCassettte
            .CallbackResponse(alarmContextBase.responseWays.OPTION3) = Function() As Boolean
                                                                           isAlarmSlotEmptyDisable = True
                                                                           Return True
                                                                       End Function
            .AdditionalInfo = "Cassette slot is empty   1.Retry Again  2.Ignore to reject cassette  3.Continue to search wafer"
        End With

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainState = systemStatesEnum.IGNITE
        Return 0


        Return 0
    End Function

    Function initPostLink() As Integer

        '----------------------
        '   Disable wafer time/sensor
        '----------------------
        waferLoadingTimer.IsEnabled = False
        waferLoadingTonqueSenser.IsEnabled = False

        '---------------------------------------------
        'cassette transport , the loading procedure had to reset  SlotEmptyCount
        '---------------------------------------------
        ___cassetteController = cassetteSubsystem._cassetteTransport

        Return 0
    End Function


    Private Function stateIgnite() As Integer
        Static StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {cassetteSubsystem, buffer}   'Hsien , 2015.04.23

        Select Case systemSubState
            Case 0
                '-----------------------------
                '   Let all ignite
                '-----------------------------
                If (FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)) Then '等待啟動flag
                    '設定所有子站開始復歸
                    StationCollection.ForEach(Function(obj As IFinishableStation) (obj.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)))
                    systemSubState = 10
                End If

            Case 10 '等待所有復歸完成
                If StationCollection.TrueForAll(Function(obj As IFinishableStation) (Not obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE))) = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False) '清除自己的ignite task

                    systemSubState = 0
                End If
        End Select
        Return 0
    End Function


#Region "Cassette-Tongue interface"

    Function conveyorModuleAction() As Boolean
        If conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) AndAlso
            (Not buffer.controlFlags.viewFlag(clsLoadingBufferV2.controlFlagsEnum.OUT_PROCESS) Or (Not buffer.controlFlags.viewFlag(clsLoadingBufferV2.controlFlagsEnum.IS_ENABLE))) Then
            Return True
        Else
            conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION).ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
        End If
        Return False
    End Function

    Dim isAlarmSlotEmptyDisable As Boolean = False
    Dim __slotEmptyAcceptance As UInteger = 3
    Dim slotEmptyCount As UInteger = 0  '連續N片卡夾空片則報警
    Dim AlarmEmptyCassette As alarmContentSensor = New alarmContentSensor With {.Sender = Me, .PossibleResponse = alarmContextBase.responseWays.RETRY Or
        alarmContextBase.responseWays.IGNORE Or alarmContextBase.responseWays.OPTION3}

    Public Event LoadWafer(ByVal sender As Object, ByVal e As waferInfoEventArgs) 'Tongue detect wafer After cassetteLifter down
    Public Event LoadEmpty(ByVal sender As Object, ByVal e As EventArgs)
    Public WriteOnly Property SlotEmptyAcceptance As UInteger
        Set(value As UInteger)
            __slotEmptyAcceptance = value
        End Set
    End Property

    Function cassetteStepDownFinished() As Boolean

        Dim __timer As singleTimer = waferLoadingTimer
        Dim __data As shiftDataPackBase = conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE_POSITION)

        If (waferLoadingTonqueSenser.IsSensorCovered) Then '檢查有無硅片
            __data.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
            __data.IsPositionOccupied = True
            slotEmptyCount = 0

            'wafer on Tongue
            RaiseEvent LoadWafer(Me, New waferInfoEventArgs With {.waferInfo = __data})

            'cycle time counting 
            __cycleTime = cycleTimer.TimeElapsed    'take snap
            cycleTimer.IsEnabled = True ' restart

            __timer.IsEnabled = False
            Return True
        ElseIf (Not __timer.IsEnabled) Then '使等待時間致能
            __timer.IsEnabled = True
        ElseIf (__timer.IsEnabled And __timer.IsTimerTicked) Then '等待一時間,以確定硅片有無
            RaiseEvent LoadEmpty(Me, EventArgs.Empty)
            slotEmptyCount += 1
            __data.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
            __data.IsPositionOccupied = False
            __timer.IsEnabled = False
            If slotEmptyCount >= __slotEmptyAcceptance And isAlarmSlotEmptyDisable = False Then
                slotEmptyCount = 0
                CentralAlarmObject.raisingAlarm(AlarmEmptyCassette)
                Return True
            End If
            Return True
        End If
        Return False
    End Function

    Sub CasstteRemoved() Handles ___cassetteController.CassetteRemoved 'Cassette is removed from lifter
        slotEmptyCount = 0
        isAlarmSlotEmptyDisable = False
    End Sub
    Function ejectCassettte() As Boolean
        cassetteSubsystem.commonFlags.setFlag(flagsInLoaderUnloader.CasCollect_f)
        slotEmptyCount = 0
        isAlarmSlotEmptyDisable = False
        Return True
    End Function

    Sub autoAddedCassetteEjectOption(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured

        '將出卡夾sensor報警只能RETRY
        Dim _sensorAlarm As alarmContentSensor = TryCast(e.Content, alarmContentSensor)
        If e.Content.Sender IsNot Nothing AndAlso _sensorAlarm IsNot Nothing AndAlso e.Content.Sender.Equals(cassetteSubsystem._cassetteTransport) AndAlso
            (_sensorAlarm.Inputs = cassetteSubsystem._cassetteUnload.OUT_ConveyerPosSen1.InputBit Or _sensorAlarm.Inputs = cassetteSubsystem._cassetteUnload.OUT_ConveyerPosSen2.InputBit) Then
            e.Content.PossibleResponse = e.Content.PossibleResponse And alarmContextBase.responseWays.RETRY
        End If
    End Sub
#End Region

   
End Class
