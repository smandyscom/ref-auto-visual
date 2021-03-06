﻿Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports System.Threading
'Imports Microsoft.VisualBasic

Public Interface IChainSynchronizable

    Property NeighborNode As IChainSynchronizable    'link to previous/next station which in the synchrozing chain
    '--------------------------------------------------------------------------------------
    'For pull/push-type logic , once this shifter cannot make any move , should raise this flag , indicating this station is ready to move.
    ' Once the flag was pull-down by next station , the data shifting operation should begin 
    '--------------------------------------------------------------------------------------
    Enum synchronizingFlagsEnum
        SYNCHRON_REQUEST 'request sync move with down stream node
        'SYNCHRON_REPLIED '附加選項 request flag activated until up stream reply is true.  必須等到上一段皮帶回應，才去看request旗標
    End Enum

    Property SynchronFlags As flagController(Of synchronizingFlagsEnum)

End Interface

'jk: base conveyor model
Public MustInherit Class shiftingModel
    Inherits systemControlPrototype
    '---------------------------
    '   The Generic Model to depict the step-transporting system
    '   For Pull-Type logic ( Set SyncRequest Flag self when fulled , Unset by downstream   Shifting model
    '   For Push-Type logic ( Set SyncRequest Flag self when emptyed, Unset by upstream     Shifting model
    '---------------------------
    ''' <summary>
    ''' The total slots this device holds
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Capability As Integer
        Get
            'Return __waferCapability
            Return __occupiedStatus.DataCount
        End Get
        Set(ByVal value As Integer)
            '----------------------------------
            '   Check if shiftDatatype is inherit from shiftDataBase
            '----------------------------------
            If ((Not ShiftDataType.IsSubclassOf(GetType(shiftDataPackBase))) And
                Not (ShiftDataType = GetType(shiftDataPackBase))) Then
                Throw New Exception("Should assign the specific shift data type")
            End If

            '----------------
            '   Prepare module link status table
            '----------------
            moduleLinkedStatus.Clear()
            For i = 0 To value - 1
                moduleLinkedStatus.Add(False)
            Next

            __occupiedStatus.DataCount = value  'Hsien , 2015.02.06 , laneData had been regulared

        End Set
    End Property
    ReadOnly Property IsAnyRemained As Boolean
        Get
            Return OccupiedStatus.IsAnyRemained
        End Get
    End Property
    ReadOnly Property ShiftState As shiftingStates
        Get
            Return SubState
        End Get
    End Property
    'Reflect action state , Hsien , 2015.07.28
    ReadOnly Property ActionStates As Short
        Get
            Return actionState
        End Get
    End Property

    Public Enum shifingModelFlags
        '--------------------------------------------------------------------------------------
        'For pull-type logic , once this shifter cannot make any move , should raise this flag , indicating this station is ready to move.
        ' Once the flag was pull-down by next station , the data shifting operation should begin 
        '--------------------------------------------------------------------------------------
        ON_TRANSFERING      ' lock queue until dequeue operation finished
        IS_BACKUPDATA       ' the setup to control if auto-backup data after every transfering , Hsien , 2015.02.06
    End Enum

    '------------------------------------------------
    '   Data expand , and united , Hsien , 2014.12.19
    '------------------------------------------------
    Property ShiftFlags As flagController(Of shifingModelFlags) = New flagController(Of shifingModelFlags)

    'Public shiftDataType As Type = GetType(shiftDataPackBase)       'factory-mode instance generating
    Property ShiftDataType As Type
        Get
            Return __occupiedStatus.DataType
        End Get
        Set(value As Type)
            __occupiedStatus.DataType = value
        End Set
    End Property

    Property IncomingShiftData As shiftDataPackBase = Nothing
    Property OutcomingShiftData As shiftDataPackBase = Nothing                              'used to take dequeued data
    Public moduleLinkedStatus As List(Of Boolean) = New List(Of Boolean)     'note: action mask: used to depict if the position linked with module
    ReadOnly Property OccupiedStatus As shiftDataCollection
        '---------------------------
        '   Give the fixed reference 
        '---------------------------
        Get
            Return __occupiedStatus
        End Get
    End Property

    Protected __occupiedStatus As shiftDataCollection = New shiftDataCollection
    Protected alarmPackConveyor As alarmContentConveyor = New alarmContentConveyor() With {.Sender = Me}
#Region "wafer check routines (在輸送帶上檢查每個位置與sensor的狀態是否符合)"
    '------------------------
    '   Throgh Type : ignore only
    '   Covey Type : retry/ignore , ignore is valid when reality matched
    '------------------------
    Public checkListWaferThroughSensors As List(Of KeyValuePair(Of Integer, sensorControl)) = New List(Of KeyValuePair(Of Integer, sensorControl))   ' integer type as occupied positon index , pulse counter as sensor going to verified
    Public Function checkWaferThroughJammed() As Boolean
        '----------------------------------------------------------------
        '   Wafer Jammed Check , all through-type sensor should not be ON
        '----------------------------------------------------------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            If (pair.Value.IsSensorCovered()) Then
                With alarmPackConveyor
                    .PossibleResponse = alarmContextBase.responseWays.RETRY 'forced to remove wafer
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() (True)
                    .Position = pair.Key                    ' report which position error occured
                    .Inputs = pair.Value.InputBit
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_JAMMED
                End With
                CentralAlarmObject.raisingAlarm(alarmPackConveyor)
                Return False    ' check failed
            End If
        Next

        Return True
    End Function
    Public Function checkWaferThroughLoss() As Boolean
        'alarmPackConveyor.PossibleResponse = alarmContextBase.responseWays.IGNORE
        '----------------------------------------------------------------
        '   Wafer Existence Check , following the internal record
        '----------------------------------------------------------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            '----------------------
            '   No any pulse : Loss
            '----------------------
            If (__occupiedStatus.DataCollection(pair.Key).IsPositionOccupied And
                (pair.Value.PulseCount = 0 And Not pair.Value.IsSensorCovered)) Then
                With alarmPackConveyor
                    .PossibleResponse = alarmContextBase.responseWays.IGNORE 'Or alarmContextBase.responseWays.RETRY
                    '.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                    '                                                             '-------------------------
                    '                                                             ' Confirmed with wafer existed
                    '                                                             '-------------------------
                    '                                                             __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = True
                    '                                                             Return True
                    '                                                         End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  '-------------------------
                                                                                  ' Confirmed with No wafer
                                                                                  '-------------------------
                                                                                  'should not be covered
                                                                                  'If (Not pair.Value.IsSensorCovered) Then
                                                                                  __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = False
                                                                                  Return True
                                                                                  'End If
                                                                                  '        Return False
                                                                              End Function
                    '-------------------------------------------------------
                    '   Additional info
                    '--------------------------------------------------------
                    .Inputs = pair.Value.InputBit
                    .Position = pair.Key
                    .Reason = alarmContentSensor.alarmReasonSensor.NO_PULSE
                    .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
                    .AdditionalInfo = My.Resources.AlarmPackThroughLoss
                End With
                CentralAlarmObject.raisingAlarm(alarmPackConveyor)
                Return False    'check failed
            End If
        Next
        Return True
    End Function
    Public Function checkWaferThroughUnknown() As Boolean
        '----------------------------------------------------------------
        '   Wafer Existence Check , following the internal record
        '----------------------------------------------------------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            '-----------------------------
            '   Unexpected pulse : unknown
            '-----------------------------
            If (Not (__occupiedStatus.DataCollection(pair.Key).IsPositionOccupied) And pair.Value.PulseCount > 0) Then
                With alarmPackConveyor
                    .PossibleResponse = alarmContextBase.responseWays.IGNORE Or alarmContextBase.responseWays.RETRY
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                 '-------------------------
                                                                                 ' Confirmed with wafer existed
                                                                                 '-------------------------
                                                                                 __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = True
                                                                                 Return True
                                                                             End Function
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                  '-------------------------
                                                                                  ' Confirmed with No wafer
                                                                                  '-------------------------
                                                                                  pair.Value.PulseCount = 0 ' reset sensor
                                                                                  Return True
                                                                              End Function
                    '-------------------------------------------------------
                    '   Additional info
                    '--------------------------------------------------------
                    .Inputs = pair.Value.InputBit
                    .Position = pair.Key
                    .Reason = alarmContentSensor.alarmReasonSensor.UNKNOWN_PULSE
                    .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
                    .AdditionalInfo = My.Resources.AlarmPackThroughUnknown
                End With
                CentralAlarmObject.raisingAlarm(alarmPackConveyor)
                Return False
            End If
        Next

        Return True
    End Function
    Public checkListWaferCoverSensors As List(Of KeyValuePair(Of Integer, sensorControl)) = New List(Of KeyValuePair(Of Integer, sensorControl))
    Public Function checkWaferCoverLossAndUnknown() As Boolean
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferCoverSensors

            Dim sensor As sensorControl = pair.Value     'Hsien , 2014.10.29 , used to re-check

            '------------------------------
            '   Loss (Supposed to be coverd)
            '----------------
            If (__occupiedStatus.DataCollection(pair.Key).IsPositionOccupied And (Not pair.Value.IsSensorCovered)) Then
                'Dim sensor As pulseCounter = pair.Value     'Hsien , 2014.10.29 , used to re-check , temporary reference used in lambda
                With alarmPackConveyor
                    .PossibleResponse = alarmContextBase.responseWays.IGNORE Or alarmContextBase.responseWays.RETRY
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function()
                                                                                  '-------------------------------------------------------------
                                                                                  '  Treat as disappered , need to confirm physical sensor again
                                                                                  '-------------------------------------------------------------
                                                                                  'If (Not sensor.IsSensorCovered) Then
                                                                                  __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = False
                                                                                  Return True
                                                                                  'End If
                                                                                  '        Return False
                                                                              End Function
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function()
                                                                                 'If (sensor.IsSensorCovered) Then
                                                                                 __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = True
                                                                                 Return True
                                                                                 'End If
                                                                                 '        Return False
                                                                             End Function
                    '-------------------------------------------------------
                    '   Additional info
                    '--------------------------------------------------------
                    .Inputs = pair.Value.InputBit
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
                    .Position = pair.Key
                    '.AdditionalInfo = "感測器未偵測到物體  , 重試: 確認無物  , 忽略:確認有物"
                End With
                CentralAlarmObject.raisingAlarm(alarmPackConveyor)
                Return False
            End If
            '----------------
            '   Unknown(Supposed not to be covered)
            '----------------
            If (Not __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied _
                    And pair.Value.IsSensorCovered) Then

                With alarmPackConveyor
                    'Dim ap As alarmContentConveyor = New alarmContentConveyor
                    .PossibleResponse = alarmContextBase.responseWays.IGNORE Or alarmContextBase.responseWays.RETRY
                    .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function()
                                                                                  '---------------------
                                                                                  '  Treat as existed ,  need to confirm physical sensor again
                                                                                  '----------------------
                                                                                  'If (sensor.IsSensorCovered) Then
                                                                                  __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = True
                                                                                  Return True
                                                                                  'End If
                                                                                  '        Return False
                                                                              End Function
                    .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function()
                                                                                 'If (Not sensor.IsSensorCovered) Then
                                                                                 __occupiedStatus.DataCollection(pair.Key).IsPositionOccupied = False
                                                                                 Return True
                                                                                 'End If
                                                                                 '        Return False
                                                                             End Function
                    '-------------------------------------------------------
                    '   Additional info
                    '--------------------------------------------------------
                    .Inputs = pair.Value.InputBit
                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                    .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
                    .Position = pair.Key
                End With
                CentralAlarmObject.raisingAlarm(alarmPackConveyor)
                Return False
            End If
        Next

        Return True

    End Function
    Public Function enableThroughSensors() As Boolean
        '--------------
        '   Enable all sensors , and reset pulse counter
        '---------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            pair.Value.IsEnabled = True
            pair.Value.PulseCount = 0
        Next

        Return True
    End Function
    Public Function disableThroughSensors() As Boolean
        '--------------
        '   Disable all sensors , so that snapped shot took
        '---------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            pair.Value.IsEnabled = False
        Next

        Return True
    End Function
    Public Function initAddSensors() As Integer
        '----------------------------
        '   Hsien , 2015.02.06 , offed routine to add sensors to drive list
        '   Caution : call this routine after checkListWaferCoverSensors,checkListWaferThroughSensors configured
        '----------------------------
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferThroughSensors
            actionComponents.Add(pair.Value)
            drivesRunningInvoke = [Delegate].Combine(drivesRunningInvoke, New Func(Of Integer)(AddressOf pair.Value.running))
        Next
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferCoverSensors
            actionComponents.Add(pair.Value)
            drivesRunningInvoke = [Delegate].Combine(drivesRunningInvoke, New Func(Of Integer)(AddressOf pair.Value.running))
        Next

        Return 0
    End Function
#End Region

#Region "actions"
    Protected actionState As Short = 0                                 ' for sub-sequence use
    Protected MustOverride Function isAbleTransfer() As Boolean           ' check if all condition passed before transfer
    Protected MustOverride Function transferProcess() As Boolean
    Protected MustOverride Function dataVerifyAction() As Boolean
#End Region
#Region "state functions"
    Public Enum shiftingStates As Short
        SET_MODULES_ACTION = 0
        MODULES_ON_ACTION
        DATA_POST_VERIFY
        LISTEN_TRANSFER
        TRANSFERING
        DATA_PRE_VERIFY
    End Enum

    Protected Function stateExecute() As Integer
        Try
            Select Case systemSubState
                Case shiftingStates.SET_MODULES_ACTION
                    '-------------------------------
                    'spread out action flags
                    '-------------------------------
                    For index = 0 To Capability - 1
                        With __occupiedStatus.DataCollection(index)
                            .ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, moduleLinkedStatus(index))
                            .ModuleCycleTimer.IsEnabled = True
                        End With
                    Next
                    actionState = 0 : systemSubState = shiftingStates.MODULES_ON_ACTION
                Case shiftingStates.MODULES_ON_ACTION
                    '------------------------------
                    '   Data Verified , Post Action
                    '------------------------------
                    Dim isAllModuleFinished As Boolean = True
                    Dim isModuleFinished As Boolean
                    For Each position As shiftDataPackBase In __occupiedStatus.DataCollection
                        '-----------------------------------
                        '   State Stayed if any module on working
                        '-----------------------------------
                        isModuleFinished = Not position.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)
                        isAllModuleFinished = (isAllModuleFinished And isModuleFinished) 'fault if any module not done 
                        '----------------------------
                        '   Spend time calculating
                        '----------------------------
                        If (isModuleFinished And
                           position.ModuleCycleTimer.IsEnabled) Then
                            position.ModuleCycleTimer.IsEnabled = False
                        End If
                    Next
                    If (isAllModuleFinished) Then
                        actionState = 0 : systemSubState = shiftingStates.DATA_POST_VERIFY
                    End If

                Case shiftingStates.DATA_POST_VERIFY
                    '-----------------------------------------------------
                    '   After all module finished work , verify data again
                    '-----------------------------------------------------
                    If (dataVerifyAction()) Then
                        actionState = 0 : systemSubState = shiftingStates.LISTEN_TRANSFER
                    End If
                Case shiftingStates.LISTEN_TRANSFER
                    '------------
                    'listen stage
                    '------------
                    If (isAbleTransfer()) Then 'jk note: 會分支 用 isResendAction, actionStates = 0 : systemSubState = shiftingStates.SET_MODULES_ACTION
                        ShiftFlags.setFlag(shifingModelFlags.ON_TRANSFERING)   'queue is in the state of undermined
                        actionState = 0

                        enableThroughSensors()  'start to take snap ,   Hsien , 2015.04.07

                        systemSubState = shiftingStates.TRANSFERING
                    Else
                        '------------------------------------------------------------------
                        '   Once disable to transfer and need to synchorn with next shifter
                        '   should Raising sync request flag
                        '------------------------------------------------------------------
                    End If
                Case shiftingStates.TRANSFERING
                    '-----------------------
                    '   Transfer action
                    '-----------------------
                    If (transferProcess()) Then

                        disableThroughSensors() ' stop to take snap , Hsien , 2015.04.07

                        '--------------------------------------
                        '   Data shifting over, do data transit
                        '--------------------------------------
                        actionState = 0

                        '------------------------------------------------------------
                        '   Data Handling
                        '   Shifting , and Backup if need , Hsien , 2015.02.06
                        '------------------------------------------------------------
                        __occupiedStatus.DataCollection.Insert(0, IncomingShiftData.Clone())    'enqueue
                        OutcomingShiftData = __occupiedStatus.DataCollection(__occupiedStatus.DataCount - 1)    'back-up
                        __occupiedStatus.DataCollection.RemoveAt(__occupiedStatus.DataCount - 1)    'dequeue

                        If (ShiftFlags.viewFlag(shifingModelFlags.IS_BACKUPDATA)) Then
                            Try
                                __occupiedStatus.Save()
                            Catch ex As Exception
                                sendMessage(ex.Message) 'Hsien , 2015.07.28 , although saving failure , still possible to working
                            End Try
                        End If
                        '------------------------------------------------------------
                        '   Data Handling
                        '   Shifting , and Backup if need , Hsien , 2015.02.06
                        '------------------------------------------------------------
                        ShiftFlags.resetFlag(shifingModelFlags.ON_TRANSFERING) 'queue state is determined
                        systemSubState = shiftingStates.DATA_PRE_VERIFY
                    End If
                    '----------------------------
                    '   Sensor - Data verify
                    '----------------------------
                Case shiftingStates.DATA_PRE_VERIFY
                    If (dataVerifyAction()) Then
                        '------------------------------
                        'sensor verification finished
                        '-------------------------------
                        actionState = 0 : systemSubState = shiftingStates.SET_MODULES_ACTION
                    Else
                        '----------------
                        ' verifiing
                        ' stay here
                        '----------------
                    End If
            End Select

        Catch ex As Exception

            Throw New Exception("shiftModel.stateExecute() , subState:" + systemSubState.ToString() + ex.Message, ex)

        End Try



        Return 0
    End Function

#Region "configure used tools"
    ''' <summary>
    ''' Given slot definition enum type , enable the corresponding slot
    ''' </summary>
    ''' <param name="__slotDefinitionEnumType"></param>
    ''' <remarks></remarks>
    Sub configureModuleLinkStatus(__slotDefinitionEnumType As Type)
        For index = 0 To [Enum].GetValues(__slotDefinitionEnumType).Length - 1
            moduleLinkedStatus([Enum].GetValues(__slotDefinitionEnumType)(index)) = True
        Next
    End Sub

#End Region

#End Region

    Dim alarmCondition As Boolean = False
    Protected Overrides Function process() As Integer
        '----------------------------
        'standard system control flow
        '----------------------------
        drivesRunningInvoke()
        '-------------------------------------------
        '   Could not interrupted by others during transfering
        '   Should stop control flow when self alarm raised
        ' R = alarmCondition 
        ' alarm condition : alarmed and (myalarm or (otheralarm  and not in transfering state)) 
        '-------------------------------------------
        'dont affect by alarm other than mine , Hsien  2015.10.15
        If (CentralAlarmObject.IsAlarmed AndAlso ((IsMyAlarmInQueue Or IsMyAlarmCurrent))) Then
            Return 0
        End If

        stateControl()
        processProgress()

        Return 0
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'todo , added argument of shift-data type , force to initialize 
        '(semi-static instance
    End Sub
End Class