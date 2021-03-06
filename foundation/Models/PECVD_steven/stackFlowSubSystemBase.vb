﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine

Public Class StackFlowSystemBase
    Inherits systemControlPrototype
    'Inherits assemblyArch  'unit test use
    '---------------------------------------------------
    '   The Cassette Systembase template , for different type of
    '       Cassette module , re-config the configuration on the assembly only
    '---------------------------------------------------

    Implements IFinishableStation

    Property CassetteId As String
        Get
            Return __cassetteId
        End Get
        Set(value As String)
            __cassetteId = value
            RaiseEvent CassetteIdRefreshed(Me, Nothing)  'Hsien , 2015.06.11
        End Set
    End Property
    Dim __cassetteId As String = ""
    Public Event CassetteIdRefreshed(ByVal sender As Object, ByVal e As EventArgs)

    '--------------------------------------------
    '   Working Type Wrapper , Hsien  ,2015.05.12
    '--------------------------------------------
    Public Enum workingTypeEnum
        AS_LOADER
        AS_UNLOADER
    End Enum

    Property CassetteType As CassetteStyle
        Get
            Return __cassetteType
        End Get
        Set(value As CassetteStyle)
            __cassetteType = value
            _stackWaferLift.SetCasStyle = __cassetteType
            _stackLoad.SetCasStyle = __cassetteType
        End Set
    End Property

    Property WaferReachCheckTime As Integer
        Get
            Return _WaferReachSenCheckTime
        End Get
        Set(value As Integer)
            _WaferReachSenCheckTime = value
            _stackWaferLift.WaferReachSenCheckTime = _WaferReachSenCheckTime
        End Set
    End Property

    Property EnableDownSmallDist As Boolean
        Get
            Return _blnEnableMoveDownSmallDist
        End Get
        Set(value As Boolean)
            _blnEnableMoveDownSmallDist = value
            _stackWaferLift.blnEnableMoveDownSmallDist = _blnEnableMoveDownSmallDist
        End Set
    End Property
    Property AfterBlowDelayTime As Integer
        Get
            Return _AfterBlowWaferDelayTime
        End Get
        Set(value As Integer)
            _AfterBlowWaferDelayTime = value
            _stackWaferPick.AfterBlowWaferDelayTime = _AfterBlowWaferDelayTime
        End Set
    End Property
    Property EnableUpSecondPos As Boolean
        Get
            Return _blnEnableUpFirstPos
        End Get
        Set(value As Boolean)
            _blnEnableUpFirstPos = value
            _stackWaferPick.blnEnableUpFirstPos = _blnEnableUpFirstPos
        End Set
    End Property
    Property ConBlowWaferOnWaitTime As Integer
        Get
            Return _BlowWaferOnWaitTime
        End Get
        Set(value As Integer)
            _BlowWaferOnWaitTime = value
            _stackWaferLift.BlowWaferOnWaitTime = _BlowWaferOnWaitTime
        End Set
    End Property
    Property ConBlowWaferOffWaitTime As Integer
        Get
            Return _BlowWaferOffWaitTime
        End Get
        Set(value As Integer)
            _BlowWaferOffWaitTime = value
            _stackWaferLift.BlowWaferOffWaitTime = _BlowWaferOffWaitTime
        End Set
    End Property
    Dim _WaferReachSenCheckTime As Integer
    Dim _blnEnableMoveDownSmallDist As Boolean
    Dim _AfterBlowWaferDelayTime As Integer
    Dim _blnEnableUpFirstPos As Boolean
    Dim _BlowWaferOnWaitTime As Integer
    Dim _BlowWaferOffWaitTime As Integer


    Dim __workingType As workingTypeEnum = workingTypeEnum.AS_UNLOADER
    Dim __cassetteType As CassetteStyle = CassetteStyle.LAYER2N_STANDARD

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    ReadOnly Property commonFlags As flagController(Of flagsInLoaderUnloader)
        Get
            Return _stackLoad.loadFlags
        End Get
    End Property

    Public _stackLoad As StackLoad = New StackLoad
    Public _stackUnload As StackUnload = New StackUnload

    Public _stackWaferLift As StackWaferLift = New StackWaferLift
    Public _stackWaferPick As StackWaferPick = New StackWaferPick
    Public _stackWaferPlace As StackWaferPlace = New StackWaferPlace

    Dim StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {_stackWaferLift, _stackWaferPlace}

    Public Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0 'Assembly檢查初始程序是否被致能
                If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    _stackUnload._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                    systemSubState = 10
                End If
            Case 10
                If _stackUnload._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    _stackLoad._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                    _stackWaferPick._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                    systemSubState = 20
                End If
            Case 20 '等待_stackLoad & _stackWaferPick 初始程序完成
                If _stackLoad._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False And _
                    _stackWaferPick._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    systemSubState = 30
                End If
            Case 30 '使其它站的初始程序致能
                StationCollection.ForEach(Function(Station As IFinishableStation) (Station.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)))
                systemSubState = 40
            Case 40 '等待其它站初始程序完成
                If StationCollection.TrueForAll(Function(Station As IFinishableStation) (Not Station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE))) = True Then
                    FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False) 'Assembly初始程序完成
                    systemMainState = systemStatesEnum.EXECUTE
                End If
        End Select
        Return 0
    End Function

    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initLinkPause),
                                           New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Overridable Function initMappingAndSetup() As Integer
        '共用旗標連結
        _stackLoad.loadFlags = New flagController(Of flagsInLoaderUnloader)
        _stackUnload.UnloadFlags = _stackLoad.loadFlags
        _stackWaferLift.liftFlags = _stackLoad.loadFlags
        _stackWaferPick.PickFlags = _stackLoad.loadFlags
        _stackWaferPlace.PlaceFlags = _stackLoad.loadFlags

        _stackWaferPick.ConBlowSol = _stackWaferLift.ConBlowSol

        'for performace cost reason , do not added redundant component into actionComponent list , hsien  ,2015.05.11
        _stackLoad.initialize()
        _stackUnload.initialize()

        _stackWaferLift.initialize()
        _stackWaferPick.initialize()
        _stackWaferPlace.initialize()

        With _stackLoad
            .UD_Motor = _stackWaferLift.UD_Motor
        End With

        With _stackUnload
            .UnloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, True)
        End With

        With _stackWaferPick
            .WaferExistSen = _stackWaferLift.WaferExistSen
        End With
        '=======================
        '     StackWaferPlace
        '=======================
        With _stackWaferPlace
            .VacSeneor = _stackWaferPick.VacSeneor
            .VacGenerate = _stackWaferPick.VacGenerate
            .CheckReadyToPlaceWafer = Function() (_stackWaferPick.blnReadyToPlaceWafer(SideStatus.is_A) Or _stackWaferPick.blnReadyToPlaceWafer(SideStatus.is_B))
            .CheckWaferReadyOnSucker = Function() (_stackWaferPick.blnWaferReadyOnSucker(SideStatus.is_A) Or _stackWaferPick.blnWaferReadyOnSucker(SideStatus.is_B) Or _stackWaferPick.PickFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f))
            .GetNowPlaceWaferSide = Function() (_stackWaferPick.NowPlaceSideIndex)
            .ResetReadyToPlaceWafer = Sub(_sideIndex)
                                          _stackWaferPick.blnReadyToPlaceWafer(_sideIndex) = False
                                          _stackWaferPick.blnWaferReadyOnSucker(_sideIndex) = False
                                      End Sub
        End With




        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (0)     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        initEnableAllDrives()
        Return 0
    End Function


End Class
