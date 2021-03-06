﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine

Public Class StackSystemBase
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
            _cassetteTransport.SetCasStyle = __cassetteType
        End Set
    End Property
   

    Dim __workingType As workingTypeEnum = workingTypeEnum.AS_UNLOADER
    Dim __cassetteType As CassetteStyle = CassetteStyle.LAYER2N_STANDARD

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    ReadOnly Property commonFlags As flagController(Of flagsInLoaderUnloader)
        Get
            Return _cassetteLoad.loadFlags
        End Get
    End Property
    Enum styleCheckSelectionEnum
        AT_LOADER
        AT_INLET
    End Enum
    ''' <summary>
    ''' Used to configure style sensor check position
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    WriteOnly Property StyleCheckSelection As styleCheckSelectionEnum
        Set(value As styleCheckSelectionEnum)
            Select Case value
                Case styleCheckSelectionEnum.AT_LOADER
                    _cassetteLoad.styleSensorCheck = AddressOf _cassetteLoad.styleSensorCheckProcedure
                    _cassetteButton.styleSensorCheck = Function() (True)
                Case styleCheckSelectionEnum.AT_INLET
                    _cassetteLoad.styleSensorCheck = Function() (True)
                    _cassetteButton.styleSensorCheck = AddressOf _cassetteLoad.styleSensorCheckProcedure
            End Select
        End Set
    End Property
    Public _stackWaferLift As New StackWaferLift
    Public _stackWaferPick As New StackWaferPick
    Public _stackWaferPlace As New StackWaferPlace

    Public _cassetteLoad As CassetteLoad = New CassetteLoad
    Public _cassetteUnload As CassetteUnload = New CassetteUnload
    Public _cassetteTransport As CassetteTransport = New CassetteTransport

    Public _cassetteButton As CassetteButton = New CassetteButton
    Public Cy1 As cylinderGeneric = New cylinderGeneric
    Public Cy2 As cylinderGeneric = New cylinderGeneric


    Dim StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {_cassetteLoad, _cassetteUnload, _stackWaferLift, _stackWaferPlace, _cassetteButton}
    Public Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0 'Assembly檢查初始程序是否被致能
                If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    initEnableAllDrives()   'Hsien , 2015.09.10 , enable all device once make sure to ignite
                    systemSubState = 30
                End If
            Case 30 '使_cassetteTransport初始程序致能使升降平台上卡匣可以退出,_stackWaferPick旋轉軸到起始位置
                _cassetteTransport._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                _stackWaferPick._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                systemSubState = 40
            Case 40 '等待_cassetteTransport、_stackWaferPick初始程序完成()
                If _cassetteTransport._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False And _
                   _stackWaferPick._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    systemSubState = 50
                End If
            Case 50 '使其它站的初始程序致能
                StationCollection.ForEach(Function(Station As IFinishableStation) (Station.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)))
                systemSubState = 60
            Case 60 '等待其它站初始程序完成
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
        _cassetteTransport.transportFlags = New flagController(Of flagsInLoaderUnloader)
        _cassetteUnload.unloadFlags = _cassetteTransport.transportFlags
        _cassetteLoad.loadFlags = _cassetteTransport.transportFlags
        _stackWaferLift.liftFlags = _cassetteTransport.transportFlags
        _stackWaferPick.PickFlags = _cassetteTransport.transportFlags
        _stackWaferPlace.PlaceFlags = _cassetteTransport.transportFlags
        _cassetteButton.buttonFlags = _cassetteTransport.transportFlags

        '_stackWaferPick.VacSeneor(SideStatus.is_A) = New sensorControl
        '_stackWaferPick.VacSeneor(SideStatus.is_B) = New sensorControl
        _stackWaferPick.ConBlowSol = _stackWaferLift.ConBlowSol

        'for performace cost reason , do not added redundant component into actionComponent list , hsien  ,2015.05.11
        _cassetteLoad.initialize()
        _cassetteUnload.initialize()
        _cassetteTransport.initialize()

        _stackWaferLift.initialize()
        _stackWaferPick.initialize()
        _stackwaferPlace.initialize()

        _cassetteButton.initialize()


        '=======================
        '     CassetteLoad
        '=======================
        ' Cassette Direction   >>>>>>>>>>>>>
        '             Stopper1       Stopper2
        '         Sen2           Sen3
        '感測器初始化設定
        With _cassetteLoad
            .loadFlags.writeFlag(flagsInLoaderUnloader.CyBackReady_f, True) '設定目前舌頭為縮回
        End With

        '=======================
        '    CassetteUnload
        '=======================
        ' <<<<<<<<<<<< Cassette Direction     
        '          Sen2           Sen1
        With _cassetteUnload
            .unloadFlags.writeFlag(flagsInLoaderUnloader.CasUnloadSpaceReady_f, True)
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

        '=======================
        '    CassetteButton
        '=======================
        With _cassetteButton
            '共用馬達連結
            .IN_ConveyerMotor = _cassetteLoad.IN_ConveyerMotor
            .OUT_ConveyerMotor = _cassetteUnload.OUT_ConveyerMotor

            .IN_ConveyerPosSen1Reference = _cassetteLoad.IN_ConveyerPosSen1
            .IN_ConveyerPosSen2Reference = _cassetteLoad.IN_ConveyerPosSen2   'hsien , 2016.03.30 , inner link
        End With
        '=======================
        '   CassetteTransport
        '=======================
        '共用馬達連結
        With _cassetteTransport

            .IN_ConveyerMotor = _cassetteLoad.IN_ConveyerMotor
            .OUT_ConveyerMotor = _cassetteUnload.OUT_ConveyerMotor
            .UD_ConveyerMotor = _stackWaferLift.UD_ConveyerMotor
            .UD_Motor = _stackWaferLift.UD_Motor
            .UD_Shell_Motor = _stackWaferLift.UD_Shell_Motor
            '共用氣壓缸連結
            .inStopper1Reference = _cassetteLoad.IN_Stopper1
            .inStopper2Reference = _cassetteLoad.IN_Stopper2
            '感測器初始化設定

            .OUT_ConveyerPosSen1 = _cassetteUnload.OUT_ConveyerPosSen1
            .OUT_ConveyerPosSen2 = _cassetteUnload.OUT_ConveyerPosSen2

            .UD_ConveyerSlowDownSen = _stackWaferLift.UD_ConveyerSlowDownSen
            .UD_ConveyerReachSen = _stackWaferLift.UD_ConveyerReachSen

            .IN_ConveyerPosSen3 = Me._cassetteLoad.IN_ConveyerPosSen3 'hsien , 2016.03.30 , inner link
            '卡匣形式選擇(ACI or JR)
            .SetCasStyle = CassetteStyle.MANZ

            'Hsien , 2015.06.12
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_1)
                .cy = Cy1
            End With
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_2)
                .cy = Cy2
            End With
        End With

        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (0)     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        Return 0
    End Function


End Class
