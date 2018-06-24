Imports Automation.Components.Services

Public Class unloadingCassetteTongueBuffer
    Inherits systemControlPrototype
    Implements IFinishableStation
    Implements ICassetteTongueBuffer


    Public Enum positionEnum
        '-----------------------------------
        '   Used to define conveyor work-station position
        '-----------------------------------
        CASSETTE = 2
        BEFORE_CASSETTE = 1
        BUFFER = 0
    End Enum

    Public ReadOnly Property Buffer1 As Integer Implements ICassetteTongueBuffer.BufferCounts
        Get
            Return buffer.WaferCount
        End Get
    End Property

    Public ReadOnly Property Cassette As cassetteSystemBase Implements ICassetteTongueBuffer.Cassette
        Get
            Return cassetteSubSystem
        End Get
    End Property

    Public ReadOnly Property Tongue As shiftingModel Implements ICassetteTongueBuffer.Tongue
        Get
            Return conveyorTongue
        End Get
    End Property

    Property IsStarted As Boolean
        Get
            Return cassetteSubSystem.commonFlags.viewFlag(flagsInLoaderUnloader.Start_f)
        End Get
        Set(value As Boolean)
            cassetteSubSystem.commonFlags.writeFlag(flagsInLoaderUnloader.Start_f, value)
        End Set
    End Property

    Public Property FinishableFlags As flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
        Get
            Return cassetteSubSystem._cassetteTransport._FinishableFlag
        End Get
        Set(value As flagController(Of IFinishableStation.controlFlags))
            'do nothing
        End Set
    End Property

    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
        Get
            Return buffer.UpstreamStations
        End Get
        Set(value As List(Of IFinishableStation))
            buffer.UpstreamStations = value
        End Set
    End Property

    Dim dataType As Type

#Region "control memebers"

    Public cassetteSubSystem As cassetteSystemBase = New cassetteSystemBase With {.WorkingType = cassetteSystemBase.workingTypeEnum.AS_UNLOADER,
                                                                                  .StyleCheckSelection = cassetteSystemBase.styleCheckSelectionEnum.AT_LOADER,
                                                                              .CassetteType = CassetteStyle.LAYER2N_STANDARD} 'Cassette system , 
    Public conveyorTongue As clsSynchronizableTransporterPullTypeV2 = New clsSynchronizableTransporterPullTypeV2
    Public buffer As clsUnloadingBuffer = New clsUnloadingBuffer
    Public clamp As clampModule = New clampModule With {.ActuatorComponent = clampModule.actuatorComponentEnum.Motor,
                                                        .TriggerClampMethod = clampModule.triggerClampMethodEnum.METHOD_BY_BELT_POSITION,
                                                        .MotionMethod = Components.CommandStateMachine.motorControl.motorCommandEnum.GO_POSITION_COMBINED}


    Public waferUnloadingTonqueSenser As sensorControl = New sensorControl 'for cassetteStepUp
    Public bufferSensor As sensorControl = New sensorControl
    Public tongueJammedSensor As sensorControl = New sensorControl

#End Region


    Function initMappingAndSetup() As Integer

        With conveyorTongue
            .Capability = 3
            .Flags.setFlag(clsSynchronizableTransporterPullTypeV2.FlagsEnum.AUTO_CLEAN)
            .Flags.setFlag(clsSynchronizableTransporterPullTypeV2.FlagsEnum.END_STREAM)
            '-------------------------------------
            '   Conveyor Data Verification
            '-------------------------------------
            .checkListWaferThroughSensors.AddRange({
                                                New KeyValuePair(Of Integer, sensorControl)(0, tongueJammedSensor)
                                                })
            .checkListWaferCoverSensors.AddRange({
                                                 New KeyValuePair(Of Integer, sensorControl)(positionEnum.BUFFER, bufferSensor)
                                                 })
            .ShiftDataType = dataType
            .IncomingShiftData = Activator.CreateInstance(dataType)
            .UpstreamStations = New List(Of IFinishableStation)
            '--------------------------------------
            '   Module Link Status
            '--------------------------------------
            .moduleLinkedStatus(positionEnum.BUFFER) = True   'for unloading buffer
            .moduleLinkedStatus(positionEnum.BEFORE_CASSETTE) = True 'for clamping
            .moduleLinkedStatus(positionEnum.CASSETTE) = True 'for unloading cassette
        End With

        With clamp
            .interfereSensors.Add(tongueJammedSensor)
            .motorConveyorReference = conveyorTongue.motorMasterConveyor
            .TargetPositionInfo = Function() (conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BEFORE_CASSETTE))
        End With


        With buffer
            .TargetPositions.AddRange({
                    Function() (conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BUFFER)),
                    Function() (conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE))})
            .shiftDataType = dataType
            .SpBuffer = bufferSensor
            .controlFlags.writeFlag(clsUnloadingBuffer.controlFlagsEnum.IS_ENABLE, True)
            .controlFlags.writeFlag(clsUnloadingBuffer.controlFlagsEnum.FIRST_CYCLE_DOWN, True)
            .unloadingCassetteSayBufferCanStore = Function() (cassetteSubSystem.commonFlags.viewFlag(flagsInLoaderUnloader.BufferCanStore_f))
            .synchronizeWithDownstream = Function() (Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED))
        End With


        'configure relation between conveyorSystem and Cassette
        With cassetteSubSystem
            '下料卡匣可升降條件判別
            ._cassetteFeed.ConveyerMotionOkCasAction = AddressOf isConveyorMotionDone

            '檢查載入/載出舌頭輸送帶上的硅片是否存在以決定按清料時是否退卡匣
            .ConveyerWaferEmpty = Function() (Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).IsPositionOccupied And
                                              Not conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BEFORE_CASSETTE).IsPositionOccupied)

            '升降馬達移動完成後,把記憶資料重置
            ._cassetteFeed.CassetteUpDownOK = AddressOf cassetteStepFinished
            ._cassetteFeed.CheckWaferOnConveyerInCassette = Function() (True)
            ._cassetteFeed.ConveyerMotionReset = Sub()
                                                     conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).ModuleAction.readFlag(interlockedFlag.POSITION_OCCUPIED)
                                                     conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).IsPositionOccupied = False
                                                 End Sub
        End With

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (0)
        systemMainState = systemStatesEnum.IGNITE

        'finish flag link
        cassetteSubSystem._cassetteTransport._UpstreamStation = New List(Of IFinishableStation) From {conveyorTongue}  'hsien , 2015.05.18
        cassetteSubSystem._cassetteLift.UpstreamStation = New List(Of IFinishableStation) From {conveyorTongue}   'Hsien , 2015.05.18


        Return 0
    End Function

    Public Sub New(dataType As Type)
        Me.dataType = dataType
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initLinkPause),
                                   New Func(Of Integer)(AddressOf initMappingAndSetup),
                                   New Func(Of Integer)(AddressOf initSubsystemInitialize),
                                   New Func(Of Integer)(AddressOf initEnableAllDrives))
    End Sub
    Private Function stateIgnite() As Integer
        Static StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {cassetteSubSystem, buffer, clamp}

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
                    systemSubState = 10
                    systemMainState = systemStatesEnum.EXECUTE
                End If
        End Select
        Return 0
    End Function

    Public Event ReceiveWafer(ByVal sender As Object, e As waferInfoEventArgs)
    Function cassetteStepFinished() As Boolean

        Dim waferData As shiftDataPackBase = conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE)
        With waferData

            RaiseEvent ReceiveWafer(Me, New waferInfoEventArgs With {.waferInfo = waferData})

            'release conveyor and vanish data
            .ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
            .IsPositionOccupied = False

        End With
        Return True
    End Function

#Region "cassette/conveyor relation"
    Function isConveyorMotionDone() As Boolean
        '馬達移動訊號
        '舌頭上是否有硅片
        If conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then

            With cassetteSubSystem

                '------------------------
                '   The condition let Buffer start to store
                '------------------------
                If ._cassetteLift.cntWafer >= (cassetteSubSystem._cassetteLift.GoalCount - 2) And
                 conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BEFORE_CASSETTE).IsPositionOccupied And
                 conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).IsPositionOccupied Then
                    ._cassetteLift.liftFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, True) '使輸送帶開始儲料
                End If
                If ._cassetteLift.cntWafer >= (cassetteSubSystem._cassetteLift.GoalCount - 1) And
                    conveyorTongue.OccupiedStatus.DataCollection(positionEnum.BEFORE_CASSETTE).IsPositionOccupied And
                    conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).IsPositionOccupied = False Then
                    ._cassetteLift.liftFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, True) '使輸送帶開始儲料
                End If
            End With


            If conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).IsPositionOccupied Or
                waferUnloadingTonqueSenser.IsSensorCovered Then

                'added enlarged condition to lift cassette 

                __cycleTime = cycleTimer.TimeElapsed
                cycleTimer.IsEnabled = True 'restart

                Return True 'trigger the cassette move

            Else
                'nothing on cassette position , release conveyor
                conveyorTongue.OccupiedStatus.DataCollection(positionEnum.CASSETTE).ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            End If
        Else
            '-----------------------------
            '   Wait conveyor trigger
            '-----------------------------
        End If
        Return False
    End Function
#End Region

#Region "auto eject cassette when ignore"
    Sub autoAddedCassetteEjectOption(sender As alarmManager, e As Object) Handles CentralAlarmObject.alarmOccured

        '將出卡夾sensor報警只能RETRY
        Dim _sensorAlarm As alarmContentSensor = TryCast(e.Content, alarmContentSensor)
        If _sensorAlarm IsNot Nothing AndAlso e.Content.Sender.Equals(cassetteSubSystem._cassetteTransport) AndAlso
            (_sensorAlarm.Inputs = cassetteSubSystem._cassetteUnload.OUT_ConveyerPosSen1.InputBit Or _sensorAlarm.Inputs = cassetteSubSystem._cassetteUnload.OUT_ConveyerPosSen2.InputBit) Then
            e.Content.PossibleResponse = e.Content.PossibleResponse And alarmContextBase.responseWays.RETRY
        End If

    End Sub
#End Region

   
End Class
