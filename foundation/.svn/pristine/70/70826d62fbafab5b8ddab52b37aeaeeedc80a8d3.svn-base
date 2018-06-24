Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class stackLane
    Inherits systemControlPrototype
    Implements IFinishableStation

    Enum conveyorMoveEnum
        MOVE
    End Enum


    Property DrivingTime As TimeSpan '= New TimeSpan(0, 0, 5)
        Get
            Return mainConveyorMotor.PositionDictionary.First.Value.DrivingTime
        End Get
        Set(value As TimeSpan)
            mainConveyorMotor.PositionDictionary.First.Value.DrivingTime = value
        End Set
    End Property
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

#Region "control memers"

    Friend stackFlags As flagController(Of flagsInLoaderUnloader) = Nothing

    Public mainConveyorMotor As dcMotorControl = Nothing

    Public loadZoneStopper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public feedZoneStopper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public workZoneStopper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public leaveZoneStopper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}
    Public workZoneClamper As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True}

    Public loadZoneExistedSensor As sensorControl = New sensorControl With {.IsEnabled = True}
    Public feedZoneExistedSensor As sensorControl = New sensorControl With {.IsEnabled = True}
    Public workZoneExistedSensor As sensorControl = New sensorControl With {.IsEnabled = True}
    Public leaveZoneExistedSensor As sensorControl = New sensorControl With {.IsEnabled = True}

    Public interlockSensors As List(Of sensorControl) = New List(Of sensorControl)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public leavingZoneSensors As List(Of sensorControl) = New List(Of sensorControl)

    Dim cylindersCollection As List(Of cylinderGeneric) = New List(Of cylinderGeneric) From {loadZoneStopper,
                                                                                                                     feedZoneStopper,
                                                                                                                     workZoneStopper,
                                                                                                                     leaveZoneStopper,
                                                                                                                     workZoneClamper}
    Dim alarmPackInterlockBlocked As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                                       .PossibleResponse = alarmContextBase.responseWays.RETRY,
                                                                                       .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF}
    ''' <summary>
    ''' Loaded corresponding sequence
    ''' </summary>
    ''' <remarks></remarks>
    Dim sequenceGroups As List(Of sequenceGroup) = New List(Of sequenceGroup)
    Dim currentSequence As sequenceGroup = Nothing
    Dim currentSequenceState As Integer = 0
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Friend isExistedFeedZone As Boolean = False
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Friend isExistedWorkZone As Boolean = False
#End Region


    Enum sequenceStates As Integer
        MOVE_CONDITION_POLLING
        PRE_MOVE_CHECK
        PRE_MOVE_ACTION
        MOVING
        POST_MOVE_CHECK
        POST_MOVE_ACTION
    End Enum

    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If (FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)) Then
                    returnCylinders()
                    systemSubState = 10
                End If
            Case 10
                If (cylindersCollection.TrueForAll(Function(cylinder As IDrivable) cylinder.CommandEndStatus = IDrivable.endStatus.EXECUTION_END)) Then
                    systemSubState = 20
                End If
            Case 20
                If (checkInterlocks()) Then
                    'check if any work stack there
                    isExistedWorkZone = (workZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100)

                    systemSubState = 500
                End If
            Case 500
                FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                systemMainState = systemStatesEnum.EXECUTE
        End Select


        Return 0
    End Function

    Function stateExecute() As Integer

        'warning , all sensor in leave zone had been ON , indicate emtpy stacks fulled
        If (leavingZoneSensors.TrueForAll(Function(sensor As sensorControl) sensor.IsSensorCovered)) Then
            sendMessageTimed(statusEnum.GENERIC_MESSAGE, "Cassette Fulled")
        End If

        Select Case systemSubState
            Case sequenceStates.MOVE_CONDITION_POLLING
                'find which sequence's condition had been statisfied
                currentSequence = sequenceGroups.Find(Function(__sequence As sequenceGroup) __sequence.condtion.Invoke)
                If (currentSequence IsNot Nothing) Then
                    'something going to execute
                    systemSubState = sequenceStates.PRE_MOVE_CHECK
                End If
            Case sequenceStates.PRE_MOVE_CHECK
                If (checkInterlocks()) Then
                    systemSubState = sequenceStates.PRE_MOVE_ACTION
                Else
                    '---------------
                    '   Check failed
                    '---------------
                End If
            Case sequenceStates.PRE_MOVE_ACTION
                If (currentSequence.preAction.Invoke(currentSequenceState)) Then
                    currentSequenceState = 0
                    systemSubState = sequenceStates.MOVING
                End If
            Case sequenceStates.MOVING
                'given step moing time (zone-zone)
                If (mainConveyorMotor.drive(dcMotorControl.dcMotorCommands.GO_POSITION, conveyorMoveEnum.MOVE) =
                   IDrivable.endStatus.EXECUTION_END) Then
                    systemSubState = sequenceStates.POST_MOVE_CHECK
                End If
            Case sequenceStates.POST_MOVE_CHECK
                If (checkInterlocks()) Then
                    systemSubState = sequenceStates.POST_MOVE_ACTION
                Else
                    '---------------
                    '   Check failed
                    '---------------
                End If
            Case sequenceStates.POST_MOVE_ACTION
                If (currentSequence.postAction.Invoke(currentSequenceState)) Then
                    currentSequenceState = 0
                    'cylinders returns to normal position
                    returnCylinders()

                    systemSubState = 500
                End If
            Case 500
                If (cylindersCollection.TrueForAll(Function(__cylinder As IDrivable) __cylinder.CommandEndStatus = IDrivable.endStatus.EXECUTION_END)) Then
                    systemSubState = sequenceStates.MOVE_CONDITION_POLLING
                End If
        End Select

        Return 0
    End Function


#Region "individual sequences"
    ''' <summary>
    ''' Nothing left in work area , but there's a stack waiting in feed zone
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function preFeedIn(ByRef state As Integer) As Boolean
        'raise stopper only
        Select Case state
            Case 0
                feedZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                workZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                state = 10
            Case 10
                If (cylindersCollection.TrueForAll(Function(cylinder As cylinderGeneric) cylinder.CommandEndStatus = IDrivable.endStatus.EXECUTION_END)) Then
                    Return True
                End If
        End Select

        Return False
    End Function
    ''' <summary>
    ''' Working stack is going to leave
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function preWorkOut(ByRef state As Integer) As Boolean

        Select Case state
            Case 0
                leaveZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_B_END)

                'release clamper and stopper
                workZoneClamper.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                workZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END)

                'If (stackFlags.viewFlag(flagsInLoaderUnloader.Start_f)) Then
                '    '-----------------------------
                '    'on working , release feedzone
                '    '-----------------------------
                '    isExistedFeedZone = (feedZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100) 'follow the real status
                '    feedZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                'Else
                '    '----------------------------------------
                '    'otherwise , do not release feed-in stack
                '    '----------------------------------------
                '    isExistedFeedZone = False
                'End If

                state = 10
            Case 10
                'release clamp (release working one
                'leave zone raise
                'feed zone fall
                If (cylindersCollection.TrueForAll(Function(cylinder As cylinderGeneric) cylinder.CommandEndStatus = IDrivable.endStatus.EXECUTION_END)) Then
                    isExistedWorkZone = False 'release status
                    Return True
                End If
        End Select

        Return False
    End Function


    ''' <summary>
    ''' After stack entered work zone
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function postFeedIn(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                If (isExistedFeedZone And workZoneExistedSensor.IsSensorCovered) Then
                    state = 10
                ElseIf isExistedFeedZone And Not workZoneExistedSensor.IsSensorCovered Then
                    'TODO , alarm , stack do not arrived work zone successfully
                ElseIf isExistedFeedZone And feedZoneExistedSensor.IsSensorCovered Then
                    'TODO , alarm , stack do not arrived work zone successfully
                    'ElseIf Not isExistedFeedZone Then
                    '    'nothing going to feed
                    '    state = 20
                End If
            Case 10
                'release the stopper
                If feedZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    state = 20
                End If
            Case 20
                If (workZoneClamper.drive(cylinderGeneric.cylinderCommands.GO_B_END) =
                   IDrivable.endStatus.EXECUTION_END) Then

                    stackFlags.setFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)  'let wafer feed-in start to work
                    isExistedWorkZone = True 'status memorize

                    state = 500
                End If
            Case 500
                isExistedFeedZone = False 'reset
                Return True
        End Select

        Return False
    End Function

    ''' <summary>
    ''' Loading stack going to feed zone 
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function preLoadIn(ByRef state As Integer) As Boolean
        'fall the loadin cylinder
        Return (loadZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                IDrivable.endStatus.EXECUTION_END)
    End Function
    Function postLoadIn(ByRef state As Integer) As Boolean
        isExistedFeedZone = (feedZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100)
        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function postWorkOut(ByRef state As Integer) As Boolean
        Return True
    End Function


    ''' <summary>
    ''' Work zone stack existed and had finished wafer loading procedure , leave area should be empty
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function checkWorkOut() As Boolean
        Return isExistedWorkZone And
           (Not stackFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) And Not stackFlags.viewFlag(flagsInLoaderUnloader.CasUnloadEnable_f)) And
            leaveZoneExistedSensor.OffTimer.TimeElapsed.TotalMilliseconds > 100
    End Function
    ''' <summary>
    ''' Feed zone stack existed and work zone is empty
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function checkFeedInOnly() As Boolean
        isExistedFeedZone = (feedZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100) And stackFlags.viewFlag(flagsInLoaderUnloader.Start_f) 'follow the real status
        Return isExistedFeedZone And Not isExistedWorkZone
    End Function
    ''' <summary>
    ''' Leave zone stack existed
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function checkLeave() As Boolean
        Return leaveZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100
    End Function
    ''' <summary>
    ''' Load zone stack existed and feed zone had no stack
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function checkLoad() As Boolean
        Return (loadZoneExistedSensor.OnTimer.TimeElapsed.TotalMilliseconds > 100) And
            (Not isExistedFeedZone) And
            (feedZoneExistedSensor.OffTimer.TimeElapsed.TotalMilliseconds > 100)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function checkInterlocks() As Boolean

        If (interlockSensors.TrueForAll(Function(sensor As sensorControl) sensor.IsSensorCovered) Or
                    (interlockSensors.Count = 0)) Then
            'nothing blocked , go ahead
            Return True
        Else
            'something blocked cyclinder , error

            'find out which one had been blocked
            alarmPackInterlockBlocked.Inputs =
                interlockSensors.Find(Function(sensor As sensorControl) sensor.IsSensorCovered).InputBit

            CentralAlarmObject.raisingAlarm(alarmPackInterlockBlocked)
            Return False
        End If

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub returnCylinders()
        'always resume these cylinder after moving
        leaveZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END)
        'workZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_A_END)
        loadZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_B_END)
        feedZoneStopper.drive(cylinderGeneric.cylinderCommands.GO_B_END)
    End Sub

    Sub New()

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initLinkPause),
                                           New Func(Of Integer)(AddressOf initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf initEnableAllDrives))

    End Sub


    Function initMappingAndSetup() As Integer

        Dim workOutSequence As sequenceGroup = New sequenceGroup With {.condtion = AddressOf checkWorkOut,
                                                                   .preAction = AddressOf preWorkOut,
                                                                   .postAction = AddressOf postWorkOut}
        Dim feedInOnlySequence As sequenceGroup = New sequenceGroup With {.condtion = AddressOf checkFeedInOnly,
                                                           .preAction = AddressOf preFeedIn,
                                                           .postAction = AddressOf postFeedIn}
        Dim loadInSequence As sequenceGroup = New sequenceGroup With {.condtion = AddressOf checkLoad,
                                                   .preAction = AddressOf preLoadIn,
                                                   .postAction = AddressOf postLoadIn}
        Dim leaveSequence As sequenceGroup = New sequenceGroup With {.condtion = AddressOf checkLeave}

        'loading sequnece groups
        sequenceGroups.AddRange({workOutSequence,
                                 feedInOnlySequence,
                                 loadInSequence,
                                  leaveSequence})


        Return 0
    End Function


#End Region


    Public Class sequenceGroup
        ''' <summary>
        ''' the moving condition
        ''' </summary>
        ''' <remarks></remarks>
        Friend condtion As Func(Of Boolean) = Function() (False)
        ''' <summary>
        ''' The action before moving
        ''' </summary>
        ''' <remarks></remarks>
        Friend preAction As stateFunction = Function() (True)
        ''' <summary>
        ''' The action after moving
        ''' </summary>
        ''' <remarks></remarks>
        Friend postAction As stateFunction = Function() (True)
    End Class

End Class

