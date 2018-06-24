Imports Automation.Components.CommandStateMachine
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class stackLaneSubSystemBase
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    ''' <summary>
    ''' Direct bridge to cassette transport
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
        Get
            Return __stackWaferLift.UpstreamStation
        End Get
        Set(value As List(Of IFinishableStation))
            __stackWaferLift.UpstreamStation = value
        End Set
    End Property

    ReadOnly Property commonFlags As flagController(Of flagsInLoaderUnloader)
        Get
            Return __stackLane.stackFlags
        End Get
    End Property


#Region "control members"
    Public __stackWaferLift As New StackWaferLift
    Public __stackWaferPick As New StackWaferPick
    Public __stackWaferPlace As New StackWaferPlace

    Public __stackLane As stackLane = New stackLane
#End Region

    Dim ignitingParts As List(Of IFinishableStation) = New List(Of IFinishableStation) From {__stackWaferLift,
                                                                                             __stackWaferPlace}


    Sub New()
        Me.systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.IGNITE

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initLinkPause),
                                           New Func(Of Integer)(AddressOf initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf initSubsystemInitialize),
                                           New Func(Of Integer)(AddressOf initEnableAllDrives))

    End Sub

    Function initMappingAndSetup() As Integer

        'flags link
        __stackLane.stackFlags = New flagController(Of flagsInLoaderUnloader)
        __stackWaferLift.liftFlags = __stackLane.stackFlags
        __stackWaferPick.PickFlags = __stackLane.stackFlags
        __stackWaferPlace.PlaceFlags = __stackLane.stackFlags

        'shared devices
        __stackWaferPick.ConBlowSol = __stackWaferLift.ConBlowSol
        '
        __stackWaferPick.WaferExistSen = __stackWaferLift.WaferExistSen

        '=======================
        '     StackWaferPlace
        '=======================
        With __stackWaferPlace
            .VacSeneor = __stackWaferPick.VacSeneor
            .VacGenerate = __stackWaferPick.VacGenerate
            .CheckReadyToPlaceWafer = Function() (__stackWaferPick.blnReadyToPlaceWafer(SideStatus.is_A) Or
                                                  __stackWaferPick.blnReadyToPlaceWafer(SideStatus.is_B))
            .CheckWaferReadyOnSucker = Function() (__stackWaferPick.blnWaferReadyOnSucker(SideStatus.is_A) Or
                                                   __stackWaferPick.blnWaferReadyOnSucker(SideStatus.is_B) Or
                                                   __stackWaferPick.PickFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f))
            .GetNowPlaceWaferSide = Function() (__stackWaferPick.NowPlaceSideIndex)
            .ResetReadyToPlaceWafer = Sub(_sideIndex)
                                          __stackWaferPick.blnReadyToPlaceWafer(_sideIndex) = False
                                          __stackWaferPick.blnWaferReadyOnSucker(_sideIndex) = False
                                      End Sub
        End With

        Return 0
    End Function


    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState = 10
                End If
            Case 10
                'let robot arm return
                __stackWaferPick._FinishableFlag.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                systemSubState = 20
            Case 20
                If Not __stackWaferPick._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    'clamp
                    'return the updown
                    systemSubState = 30
                End If
            Case 30
                'clamp current stack (whether it existed
                If __stackLane.workZoneClamper.drive(cylinderGeneric.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 40
                End If
            Case 40
                'home returning
                If __stackWaferLift.UD_Motor.drive(Components.CommandStateMachine.motorControl.motorCommandEnum.GO_HOME) =
                    motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 50
                End If
            Case 50
                'the wait position , ready to move out stack
                If __stackWaferLift.UD_Motor.drive(Components.CommandStateMachine.motorControl.motorCommandEnum.GO_POSITION,
                                                   LiftMotorUsedPositions.MOTOR_MANZ_WAIT) =
                    motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 60
                End If
            Case 60
                'release current stack
                If __stackLane.workZoneClamper.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                    IDrivable.endStatus.EXECUTION_END Then
                    'then , lane start igniting 
                    __stackLane.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                    systemSubState = 70
                End If
            Case 70
                If (Not __stackLane.FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)) Then
                    'igniting rest parts
                    ignitingParts.ForEach(Sub(part As IFinishableStation) part.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE))
                    systemSubState = 500
                Else
                    '-------------------------------
                    'wait stack lane finish its work
                    '-------------------------------
                End If
            Case 500
                'wait all station 
                If ignitingParts.TrueForAll(Function(part As IFinishableStation) Not part.FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)) Then
                    FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                    systemMainState = systemStatesEnum.EXECUTE
                Else
                    '-------------------------
                    '   Reset Part on igniting
                    '-------------------------
                End If
        End Select

        Return 0

    End Function

    ''' <summary>
    ''' Used to bridge stackLift and stackLane
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateExecute() As Integer

        'signal flow:
        '1. stack lane set stack to work
        '2. stack lift receive work command
        '3. stack lift unset stack to work
        '4. stack lane receive work done status

        Select Case systemSubState
            Case 0
                'stack start wafer loading
                If commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then
                    systemSubState = 10
                Else
                    '-----------------
                    '   Wait stack lane
                    '-----------------
                End If
            Case 10
                'wait until stack end wafer loading
                If Not commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then
                    systemSubState = 20
                Else
                    '-------------------
                    '   Wait stack lift
                    '-------------------
                End If
            Case 20
                'drive down stack pill
                If (__stackWaferLift.UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, LiftMotorUsedPositions.MOTOR_MANZ_WAIT) =
                    motorControl.statusEnum.EXECUTION_END) Then
                    'return flag
                    'inform stack lane to reject cassette
                    commonFlags.resetFlag(flagsInLoaderUnloader.CasUnloadEnable_f)
                    systemSubState = 0
                End If
        End Select

        Return 0
    End Function


End Class
