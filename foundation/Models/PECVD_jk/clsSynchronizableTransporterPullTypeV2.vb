﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.mainIOHardware
Imports System.IO

''' <summary>
''' 第2版，重新寫過，只看upstream是否還有殘料來決定是否要等前方來料
''' </summary>
''' <remarks></remarks>
Public Class clsSynchronizableTransporterPullTypeV2
    Inherits shiftingModel
    Implements IChainSynchronizable
    Implements IFinishableStation
    Protected __processState As Integer = 0
    Public Enum FlagsEnum
        SYNC_MASTER_MODE '同動中是否為master(主要控制多段皮帶，以拉式方法是同動中最後一段的皮帶)
        SET_LOCK    '被外部MODULE鎖住
        MOVING      ' reflect status only
        END_STREAM 'True: 設定此段是否為末端輸送帶，不可向下游輸送帶發出同動請求
        SYNC_WITH_UP_STREAM_NODE
        AUTO_CLEAN '是否要自動清料
        MANUAL_CLEAN '手動清料，一旦清完就reset
        ALLOW_TRANSFER_EMPTY '允許傳送空片
        MOTOR_MOVING '真正馬達移動
    End Enum

    Enum isAbleTransferEnum
        _0
        _10
        _20
        _100
        _200
        _300
    End Enum
    Enum transferProcessEnum
        _0
        _10
        _20
        _30
        _40
        _100
        _110
        _300
        _200

        _120

        _310

    End Enum
    Public Property UpstreamNode As IChainSynchronizable Implements IChainSynchronizable.NeighborNode
    Public Property SynchronFlags As flagController(Of IChainSynchronizable.synchronizingFlagsEnum) Implements IChainSynchronizable.SynchronFlags
    Public Property Flags As flagController(Of FlagsEnum) = New flagController(Of FlagsEnum)
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property FinishableFlags As flagController(Of IFinishableStation.controlFlags) = New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property selfMove As Func(Of Boolean) = Function() (False) '自我移動條件，不需要等前段皮帶移動
    Public Property outputBitMoving As ULong = 0 '輸送帶移動時，moving為true，可設定輸出點位
    Public Property outputBitMotorMoving As ULong = 0
    Public waitAllUpstreamNodesEmptyDelayTime As TimeSpan = New TimeSpan(0, 0, 0)
    Protected tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0)}
    'Public setMove As Func(Of List(Of cMotorPoint), Integer) '設定移動前所要設定的程序，如point table, velocity profile
    ReadOnly Property PointTable As List(Of cMotorPoint) '若要自己動，但要與上段輸送帶同動，必須把要移動的馬達點位集合在此
        'Hsien , 簡化資料結構 , 2015.04.27
        Get
            Return motorMasterConveyor.PointTable
        End Get
    End Property
    Public Function ResendModuleAction() As Integer
        systemSubState = shiftingStates.SET_MODULES_ACTION
        SynchronFlags.writeFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST, False) 'need sync with next stream
        Return 0
    End Function
    Property MyPointTable As List(Of cMotorPoint) = New List(Of cMotorPoint) '自己輸送帶的pointTable集合 , for logic conveyor , may including multi physical conveyor motors , Hsien , 2015.04.27
    Public Event DataRecord(ByVal sender As Object, ByVal e As EventArgs)
#Region "Exteranl Data delcare"
    Public motorMasterConveyor As New motorControl With {.IsEnabled = True}
#End Region
    Protected Overrides Function dataVerifyAction() As Boolean
        Select Case systemSubState
            Case shiftingStates.DATA_POST_VERIFY 'before moving check
                Return True
            Case shiftingStates.DATA_PRE_VERIFY 'after moving check
                If checkWaferThroughJammed() And checkWaferCoverLossAndUnknown() Then
                    writeBit(outputBitMoving, False)
                    RaiseEvent DataRecord(Me, Nothing)
                    Return True '回應 移動完成
                Else
                    Return False
                End If
        End Select
        Return True 'checkWaferCoverLossAndUnknown() ' And checkWaferThroughJammed() 'And checkWaferThroughLoss() 'And checkWaferThroughUnknown()
    End Function

    Protected Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0 'check is needing to restore bakcup data or not.
                If ShiftFlags.viewFlag(shifingModelFlags.IS_BACKUPDATA) = False OrElse (dataInitialProcess(__processState)) Then
                    __processState = 0
                    systemMainState = systemStatesEnum.EXECUTE
                End If
        End Select
        Return 0
    End Function
    '-----------------------------------------
    '   The common data initializing procedure
    '-----------------------------------------
    Public Function dataInitialProcess(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                'check if persistance file existed
                Dim fullParentName As String = utilities.getFullParentName(Me)
                Dim fi As FileInfo = New FileInfo(My.Application.Info.DirectoryPath + "\Data\" + fullParentName + ".trayinfo")
                If (fi.Exists) Then
                    Try
                        'trayInfo.Load(fi.FullName)
                        __occupiedStatus.Load(fi.FullName)
                    Catch ex As Exception
                        '--------------------------------------
                        'something wrong , create file
                        'marked as inexisted tray , but user still to override info afterward 
                        '--------------------------------------
                        Try
                            fi.Delete()
                            __occupiedStatus.Create(fi.FullName)
                        Catch __ex As Exception
                            '-------------------------------------------
                            '   Failed to delete/create
                            '-------------------------------------------
                            sendMessage(fi.Name & __ex.Message) 'record error reason
                        End Try
                        __occupiedStatus.DataCollection.First.IsPositionOccupied = False
                    End Try
                Else
                    '--------------------------------------
                    'inexisted , create file
                    'marked as inexisted tray , but user still to override info afterward 
                    '--------------------------------------
                    'trayInfo.Create(fi.FullName)
                    __occupiedStatus.Create(fi.FullName)
                    '__occupiedStatus.DataCollection.First.IsPositionOccupied = False
                End If
                state = 10
            Case 10
                '---------------------------------------------------------
                'data verification , if conflict with record , raise alarm
                '---------------------------------------------------------
                If (checkWaferCoverLossAndUnknown()) Then
                    Return True
                End If
        End Select

        Return False
    End Function
    Protected Overrides Function isAbleTransfer() As Boolean 'shiftingModel中定義必須實作出的函式
        Return _isAbleTransfer(actionState)
    End Function
    Private Function _isAbleTransfer(ByRef cStep As isAbleTransferEnum) As Boolean 'shiftingModel中定義必須實作出的函式
        Select Case cStep
            Case isAbleTransferEnum._0 '等待上段輸送帶請求同動，若上段輸送帶們都沒料，則判斷是否自行移動
                If selfMove.Invoke() = True Then

                    cStep = isAbleTransferEnum._10
                ElseIf IsAllUpstreamNodesAnyRemaind(UpstreamNode) = True Then

                    '--------------------------------------------
                    '   At least one of upstream remained
                    '--------------------------------------------

                    If UpstreamNode.SynchronFlags.viewFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST) = True Then
                        Flags.writeFlag(FlagsEnum.SYNC_WITH_UP_STREAM_NODE, True) '記憶前段輸送帶是否有請求
                        tmr.TimerGoal = waitAllUpstreamNodesEmptyDelayTime : tmr.IsEnabled = True
                        cStep = isAbleTransferEnum._10        '取得上段輸送帶請求，檢查自我移動
                    Else
                        '-------------------
                        '   Wait until upstream requested
                        '-------------------
                    End If

                ElseIf tmr.IsEnabled = True AndAlso tmr.IsTimerTicked = False Then
                    'do nothing, wait until the timer is ticked
                ElseIf Flags.viewFlag(FlagsEnum.AUTO_CLEAN) OrElse
                    Flags.viewFlag(FlagsEnum.MANUAL_CLEAN) OrElse
                    (UpstreamStations.Count > 0 AndAlso UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)))) Then '檢查自我移動
                    cStep = isAbleTransferEnum._10        '取得上段輸送帶請求，檢查自我移動
                End If
            Case isAbleTransferEnum._10 '已取得與上段皮帶是否同動的旗標，再看看自己是否需要與下段皮帶同動?
                '----------------------------------------------------------------------
                '   check The last position had occupied , need to synchron with next station
                '----------------------------------------------------------------------
                If OccupiedStatus.DataCollection.Last().IsPositionOccupied OrElse
                    (IsAnyRemained = True AndAlso Flags.viewFlag(FlagsEnum.ALLOW_TRANSFER_EMPTY) = True) Then 'jk note :最後一個位置有片 或 允許空片傳遞
                    '----------------------------------------------------------------------
                    '   The last position had occupied , need to synchron with next station
                    '----------------------------------------------------------------------

                    If Flags.viewFlag(FlagsEnum.END_STREAM) = False Then '不是末段輸送帶，可請求與下段輸送帶同動
                        Flags.writeFlag(FlagsEnum.SYNC_MASTER_MODE, False) '請求與下段皮帶同動(設為slave模式)
                        SynchronFlags.writeFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST, True) 'need sync with next stream
                        Return True '回應 需要移動
                    Else
                        cStep = isAbleTransferEnum._100 '重新module action，等待module action完後，回到上一步重新等待上一家reply
                    End If

                Else
                    '----------------------------------------------------------------------
                    '   The last position is NOT occupied , I can be the master
                    '----------------------------------------------------------------------
                    Flags.writeFlag(FlagsEnum.SYNC_MASTER_MODE, True) '自己移動(設為master模式)，接下來再查看是否要帶動前段皮帶
                    cStep = isAbleTransferEnum._20 '自已移動

                End If
            Case isAbleTransferEnum._20 'master mode(no need sync with next stream), check do i need sync with up stream node or move self
                '----------------------------------------------------------------------
                '   This station had capability to load 
                '   take upstream station moving synchonizly if needed
                '----------------------------------------------------------------------
                If Flags.viewFlag(FlagsEnum.SYNC_WITH_UP_STREAM_NODE) = True Then
                    '----------------------------------------------------------------------
                    '  MASTER MODE 必須與上一段皮帶同動
                    '----------------------------------------------------------------------

                    IncomingShiftData.Assign(CType(UpstreamNode, shiftingModel).OccupiedStatus.DataCollection.Last) 'assignment
                    Return True '回應 需要移動

                Else '可不用等前段輸送帶請求同動旗標
                    If IsAnyRemained = True Then
                        cStep = isAbleTransferEnum._200
                    Else 'there is no wafer in conveyor andalso don't need sync with up stream
                        'check finish flag first
                        Flags.writeFlag(FlagsEnum.MANUAL_CLEAN, False) 'reset
                        If UpstreamStations.Count > 0 AndAlso UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) Then
                            FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True)
                            cStep = isAbleTransferEnum._300
                        Else 'resend action
                            cStep = isAbleTransferEnum._100 'resend action
                        End If
                    End If
                End If

            Case isAbleTransferEnum._100  'resend action
                systemSubState = shiftingStates.SET_MODULES_ACTION
                Return False '回應，不需要移動，且把狀態改回重新action

            Case isAbleTransferEnum._200 'SPONTANOUS MODE 只有自己動
                '-----------------------------------------------------------------------
                '   SPONTANOUS MODE , (CAUTION)incoming should be empty data
                '-----------------------------------------------------------------------
                Return True '回應 需要移動

            Case isAbleTransferEnum._300 'finished, waiting up stream exit not finish
                ' 若上段站只要有一個收料結束不成立，則回復到正常運作模式，重新要片
                If UpstreamStations.Exists(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False) 'clear me finish flag
                    cStep = isAbleTransferEnum._100
                End If
            Case Else

        End Select
        Return False

    End Function
    Private Function IsAllUpstreamNodesAnyRemaind(_UpstreamNode As IChainSynchronizable) As Boolean '判斷上一段輸送帶們是否有料
        If _UpstreamNode Is Nothing Then
            Return False
        Else
            If CType(_UpstreamNode, shiftingModel).IsAnyRemained = True Then
                Return True
            Else
                Return IsAllUpstreamNodesAnyRemaind(_UpstreamNode.NeighborNode)
            End If
        End If
    End Function
    Public Function SetMotorMovingBitOut(bitValue As Boolean) As Integer
        writeBit(outputBitMotorMoving, bitValue)
        If UpstreamNode IsNot Nothing Then
            If TryCast(UpstreamNode, clsSynchronizableTransporterPullTypeV2) IsNot Nothing Then
                If UpstreamNode IsNot Nothing AndAlso UpstreamNode.SynchronFlags.viewFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST) = True Then
                    TryCast(UpstreamNode, clsSynchronizableTransporterPullTypeV2).SetMotorMovingBitOut(bitValue)
                End If
            End If
        End If
        Return 0
    End Function
    ''' <summary>
    ''' 實做 單軸 或 多軸同動的功能，若是被動軸則等待同動旗標完成
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function transferProcess() As Boolean
        'caution : mechanical(CSTA) action achived firstly , then data transfer synchron
        'otherwise , data verification may failed
        'if slave
        'motorControl.drive(WAIT_CSTA_DONE)
        'jk note: slave指的是等待被拉的輸送帶。 master只有一個，slave有很多個。
        Select Case CType(actionState, transferProcessEnum)
            Case transferProcessEnum._0
                If Flags.viewFlag(FlagsEnum.SET_LOCK) = True Then '被外部MODULE鎖住
                    '等待外部解鎖
                Else
                    Flags.setFlag(FlagsEnum.MOVING)
                    writeBit(outputBitMoving, True)

                    actionState = transferProcessEnum._10
                End If
            Case transferProcessEnum._10
                If Flags.viewFlag(FlagsEnum.SYNC_MASTER_MODE) = True Then


                    actionState = transferProcessEnum._100
                Else
                    '------------------------
                    '   The slave mode
                    '------------------------
                    actionState = transferProcessEnum._200
                End If

            Case transferProcessEnum._100 'master mode
#If TESTING = 1 Then
                If MyPointTable.Exists(Function(obj As cMotorPoint) (obj.AxisIndex = motorMasterConveyor.MotorIndex)) = False Then
                    MsgBox("motor is not the same!")
                End If
#End If
                PointTable.Clear() '先清除point table
                setMove(PointTable) ' collect all point from upstream(s) to master
                PointTable.ForEach(Sub(obj As cMotorPoint) AMaxM4_CmdPos_Reset(obj.AxisIndex))    'Hsien , tested , have to reset before movement , 2015.11.02
                'otherwise would affect error count for some axis with bad dynamic 
                SetMotorMovingBitOut(True)
                actionState = transferProcessEnum._110

                '-----------------------------------------
                ' Start Master Moving
                '-----------------------------------------
            Case transferProcessEnum._110
                If motorMasterConveyor.drive(motorControl.motorCommandEnum.SYNCHRON_MASTER) =
                    motorControl.statusEnum.EXECUTION_END Then
                    SetMotorMovingBitOut(False)
                    actionState = transferProcessEnum._300  'Hsien , 2015.04.27 , since motion had done , no need re-check if motion done
                End If
                '-----------------------------------------
                '  End of Master Moving
                '----------------------------------------

            Case transferProcessEnum._200 'slave mode
                '-----------------------------------------------------------------
                '   Wait until synchronized ( Be pull down by Downstream Stations)
                '-----------------------------------------------------------------
                'jk note: 雖然此時是LOCK，但主軸
                If (Not SynchronFlags.viewFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST)) Then
                    '-------------------------------------------------------------
                    '   SLAVE MODE , take upstream together if needed. 若有上一段皮帶 且 已請求，則順便清除同動旗標
                    '-------------------------------------------------------------
                    actionState = transferProcessEnum._300
                End If

            Case transferProcessEnum._300 '同動完成了，check 是否有upstream 請求同動，若有，則清除同動

                If Flags.readFlag(FlagsEnum.SYNC_WITH_UP_STREAM_NODE) = True Then '讀完即焚
                    If UpstreamNode IsNot Nothing Then
                        IncomingShiftData.Assign(CType(UpstreamNode, shiftingModel).OccupiedStatus.DataCollection.Last) 'assignment
                        UpstreamNode.SynchronFlags.writeFlag(IChainSynchronizable.synchronizingFlagsEnum.SYNCHRON_REQUEST, False)  ' reset operation should in this state , in order to coperate with enqueue operation
                    End If
                Else
                    IncomingShiftData.IsPositionOccupied = False    'nessary
                End If
                Flags.resetFlag(FlagsEnum.MOVING) '解除busy
                Flags.resetFlag(FlagsEnum.MOTOR_MOVING) 'reset motor moving
                Return True '回應 移動完成
        End Select

        Return False
    End Function

    Sub New()

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf MyBase.stateExecute
        systemMainState = systemStatesEnum.IGNITE
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))

        SynchronFlags = New flagController(Of IChainSynchronizable.synchronizingFlagsEnum)

    End Sub

    Overridable Function initMappingAndSetup()

        'load default pointTable (for unit test use) , Hsien , 2015.04.28
        motorMasterConveyor.PointTable.Clear()
        motorMasterConveyor.PointTable.AddRange(MyPointTable.ToArray())

        Return 0
    End Function

    Public Overridable Function setMove(ByVal pointTable As List(Of cMotorPoint)) As Integer '設定移動前所要設定的程序，如point table, velocity profile
        Flags.setFlag(FlagsEnum.MOTOR_MOVING)
        If Flags.viewFlag(FlagsEnum.SYNC_WITH_UP_STREAM_NODE) = True Then
            If UpstreamNode IsNot Nothing AndAlso TypeOf UpstreamNode Is clsSynchronizableTransporterPullTypeV2 Then
                CType(UpstreamNode, clsSynchronizableTransporterPullTypeV2).setMove(pointTable) '如果需要與上段輸送帶同動，則呼叫upstreamNode的setMove
            End If
        End If
        pointTable.AddRange(MyPointTable)  'Hsien , cascade my shiftPitch,  Hsien , 2015.04.27
        Return 0
    End Function

End Class
Public Interface IChainSynchronizableMasterSlave

    Property NeighborNode As IChainSynchronizableMasterSlave    'link to previous/next station which in the synchrozing chain
    '--------------------------------------------------------------------------------------
    'For pull/push-type logic , once this shifter cannot make any move , should raise this flag , indicating this station is ready to move.
    ' Once the flag was pull-down by next station , the data shifting operation should begin 
    '--------------------------------------------------------------------------------------
    Enum synchronizingFlagsEnum
        SYNCHRON_REQUEST 'request sync move with down stream node
        SYNCHRON_REPLIED '附加選項 request flag activated until up stream reply is true.  必須等到上一段皮帶回應，才去看request旗標
    End Enum

    Property SynchronFlags As flagController(Of synchronizingFlagsEnum)
    'Property ExternalShiftDataLink As Func(Of shiftDataPackBase) ' the data going to enqueue , as delegate , able to access collection-index

    'Property Arguments As Object       'data need to pass when synchron, i.e motorIndex,position (for CSTA setting use)
    ''' <summary>
    ''' 設定同動
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function setMove(ByVal pointTable As List(Of cMotorPoint)) As Integer
End Interface
#Region "必須放在外面的"
#If 1 = 0 Then
'上料區 舌頭段輸送帶可以用此class
'上料區 傳送段輸送帶可以用此class
Public Class TestBench
    Dim unloadingConveyorTongue As synchronizableTransporterPullTypeJk = New synchronizableTransporterPullTypeJk()
    Dim unloadingBuffer As clsUnloadingBuffer = New clsUnloadingBuffer
    Sub Test()
        With unloadingConveyorTongue
            .highestPriority = Function() As Boolean '下料輸送帶 buffer滿片時，必須自己動
                                   If unloadingBuffer.controlFlags.viewFlag(clsUnloadingBuffer.controlFlagsEnum.IS_FULL) = True Then
                                       Return True
                                   End If
                                   Return False
                               End Function

            '.highestPriority = Function() (True)


        End With
    End Sub
End Class
#End If

#End Region