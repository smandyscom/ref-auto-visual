﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.CassetteFeed


Public Class nonExtendCassetteSystemBase
    Inherits systemControlPrototype
    Implements IFinishableStation
    '---------------------------------------------------
    '   The Cassette Systembase template , for different type of
    '       Cassette module , re-config the configuration on the assembly only
    '---------------------------------------------------
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
    WriteOnly Property ConveyerWaferEmpty As Func(Of Boolean) '輸送帶設備給的升降卡匣的狀態
        Set(value As Func(Of Boolean))
            _cassetteFeed.ConveyerWaferEmpty = value
            _cassetteLift.ConveyerWaferEmpty = value
        End Set
    End Property

    '--------------------------------------------
    '   Working Type Wrapper , Hsien  ,2015.05.12
    '--------------------------------------------
    Public Enum workingTypeEnum
        AS_LOADER
        AS_UNLOADER
    End Enum
    Property WorkingType As workingTypeEnum
        Get
            Return __workingType
        End Get
        Set(value As workingTypeEnum)
            __workingType = value
            Select Case __workingType
                Case workingTypeEnum.AS_LOADER '作為硅片載出的卡匣
                    _cassetteLift.SetWaferMoveDir = FeedDir.MOVE_OUT
                    _cassetteFeed.SetWaferMoveDir = FeedDir.MOVE_OUT
                Case workingTypeEnum.AS_UNLOADER '作為硅片載入的卡匣
                    _cassetteLift.SetWaferMoveDir = FeedDir.MOVE_IN
                    _cassetteFeed.SetWaferMoveDir = FeedDir.MOVE_IN
                Case Else

            End Select
        End Set
    End Property
    Property CassetteType As CassetteStyle
        Get
            Return __cassetteType
        End Get
        Set(value As CassetteStyle)
            __cassetteType = value
            _cassetteLift.SetCasStyle = __cassetteType
            _cassetteTransport.SetCasStyle = __cassetteType
        End Set
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
                    _cassetteLoad.alarmPackCassetteStyleError.PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    _cassetteLoad.alarmPackCassetteStyleError.CallbackResponse(alarmContextBase.responseWays.IGNORE) = AddressOf _cassetteLoad.cassetteStyleErrorIgnoreHandler
                    _cassetteButton.styleSensorCheck = Function() (True)
                Case styleCheckSelectionEnum.AT_INLET
                    _cassetteLoad.styleSensorCheck = Function() (True)
                    _cassetteLoad.alarmPackCassetteStyleError.PossibleResponse = alarmContextBase.responseWays.RETRY ' retry only , Hsien , 2016.08.22
                    '_cassetteLoad.alarmPackCassetteStyleError.CallbackResponse(alarmContextBase.responseWays.IGNORE) = AddressOf _cassetteButton.cassetteStyleErrorIgnoreHandler
                    _cassetteButton.styleSensorCheck = AddressOf _cassetteLoad.styleSensorCheckProcedure
            End Select
        End Set
    End Property

    Dim __workingType As workingTypeEnum = workingTypeEnum.AS_UNLOADER
    Dim __cassetteType As CassetteStyle = CassetteStyle.LAYER2N_STANDARD

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    ''' <summary>
    ''' Direct bridge to cassette transport
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
        Get
            Return _cassetteTransport._UpstreamStation
        End Get
        Set(value As List(Of IFinishableStation))
            _cassetteTransport._UpstreamStation = value
        End Set
    End Property

    ReadOnly Property commonFlags As flagController(Of flagsInLoaderUnloader)
        Get
            Return _cassetteLoad.loadFlags
        End Get
    End Property

    Public _cassetteLoad As CassetteLoad = New CassetteLoad
    Public _cassetteUnload As CassetteUnload = New CassetteUnload
    Public _cassetteTransport As CassetteTransport = New CassetteTransport
    Public _cassetteFeed As nonExtendCassetteFeed = New nonExtendCassetteFeed With {.SetWaferMoveDir = FeedDir.MOVE_IN}
    Public _cassetteLift As CassetteLift = New CassetteLift With {.SetWaferMoveDir = FeedDir.MOVE_IN}   'the default direction , Hsien , 2015.05.14
    Public _cassetteButton As CassetteButton = New CassetteButton

    Public Cy1 As cylinderGeneric = New cylinderGeneric  '後退檔點汽缸
    Public Cy2 As cylinderGeneric = New cylinderGeneric  '預備位檔點汽缸
    Public Cy3 As cylinderGeneric = New cylinderGeneric

    'Dim Ignite_Step As Integer = 0

    Dim StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {_cassetteLoad, _cassetteUnload, _cassetteLift, _cassetteButton}
    Public Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0 'Assembly檢查初始程序是否被致能
                If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then

                    initEnableAllDrives()   'Hsien , 2015.09.10 , enable all device once make sure to ignite

                    systemSubState = 10
                End If

            Case 10
                With _cassetteFeed
                    Dim sensorList As New List(Of sensorControl) From {.OutReachSen, .InReachSen, .InSlowDownSen, _cassetteLift.UD_ConveyerSlowDownSen}
                    If sensorList.Exists(Function(sensor As sensorControl) (sensor.IsSensorCovered)) Then
                        .transferOutatIgnite.setFlag(interlockedFlag.POSITION_OCCUPIED)
                        Cy1.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                        Cy2.drive(cylinderGeneric.cylinderCommands.GO_B_END)
                    Else
                        Cy1.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                        Cy2.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                    End If
                    systemSubState = 20
                End With

            Case 20
                If (Cy1.CommandEndStatus = IDrivable.endStatus.EXECUTION_END) AndAlso (Cy2.CommandEndStatus = IDrivable.endStatus.EXECUTION_END) Then
                    _cassetteFeed._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                    systemSubState = 30
                End If

           
            Case 30 '等待_cassetteFeed初始程序完成
                If _cassetteFeed._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    Cy2.drive(cylinderGeneric.cylinderCommands.GO_A_END)
                    systemSubState = 40
                End If
            Case 40 '使_cassetteTransport初始程序致能使升降平台上卡匣可以退出
                If Cy2.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    _cassetteTransport._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                    systemSubState = 50
                End If

            Case 50 '等待_cassetteTransport初始程序完成
                If _cassetteTransport._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    systemSubState = 60
                End If
            Case 60 '使其它站的初始程序致能
                StationCollection.ForEach(Function(Station As IFinishableStation) (Station.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)))
                systemSubState = 70
            Case 70 '等待其它站初始程序完成
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
        _cassetteFeed.feedFlags = _cassetteTransport.transportFlags
        _cassetteLift.liftFlags = _cassetteTransport.transportFlags
        _cassetteButton.buttonFlags = _cassetteTransport.transportFlags


        _cassetteFeed.feedFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, True) '使輸送帶開始儲料

        'for performace cost reason , do not added redundant component into actionComponent list , hsien  ,2015.05.11
        _cassetteLoad.initialize()
        _cassetteUnload.initialize()
        _cassetteTransport.initialize()
        _cassetteFeed.initialize()
        _cassetteLift.initialize()
        _cassetteButton.initialize()


        '=======================
        '     CassetteLoad
        '=======================
        ' Cassette Direction   >>>>>>>>>>>>>
        '             Stopper1       Stopper2
        'Sen1          Sen2           Sen3

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
        '=======================
        '     CassetteLift
        '=======================
        '感測器初始化設定
        With _cassetteLift
            .getBoxCounter = Function() (_cassetteTransport.BoxCnt)
            .setBoxCounter = Sub() _cassetteTransport.BoxCnt = BoxSelect.P25X3_Box_3    'Hsien , 2015.06 17         '.SetCasStyle = CassetteStyle.Manz  '設定卡匣的形式
            '.SetWaferMoveDir = FeedDir.MOVE_IN '設定硅片要流出或流入卡匣    'for unloader , set as move in
            'for loader , set as move out

        End With
        '=======================
        '      CassetteFeed
        '=======================
        With _cassetteFeed
            .UD_ConveyerMotor = _cassetteLift.UD_ConveyerMotor
        End With
        '=======================
        '    CassetteButton
        '=======================
        With _cassetteButton

            '共用馬達連結
            .IN_ConveyerMotor = _cassetteLoad.IN_ConveyerMotor
            .OUT_ConveyerMotor = _cassetteUnload.OUT_ConveyerMotor

            .IN_ConveyerPosSen1Reference = _cassetteLoad.IN_ConveyerPosSen1
            .IN_ConveyerPosSen2Reference = _cassetteLoad.IN_ConveyerPosSen2
            .OUT_ConveyerPosSen2Reference = _cassetteUnload.OUT_ConveyerPosSen2
        End With

        '=======================
        '   CassetteTransport
        '=======================
        '共用馬達連結
        With _cassetteTransport
            .IN_ConveyerMotor = _cassetteLoad.IN_ConveyerMotor
            .OUT_ConveyerMotor = _cassetteUnload.OUT_ConveyerMotor
            .UD_ConveyerMotor = _cassetteLift.UD_ConveyerMotor
            .UD_Motor = _cassetteLift.UD_Motor
            .UD_Shell_Motor = _cassetteLift.UD_Shell_Motor
            '共用氣壓缸連結
            .inStopper2Reference = _cassetteLoad.IN_Stopper2
            .inStopper1Reference = _cassetteLoad.IN_Stopper1

            .IN_ConveyerPosSen3 = _cassetteLoad.IN_ConveyerPosSen3
          

            .OUT_ConveyerPosSen1 = _cassetteUnload.OUT_ConveyerPosSen1
            .OUT_ConveyerPosSen2 = _cassetteUnload.OUT_ConveyerPosSen2

            .UD_ConveyerSlowDownSen = _cassetteLift.UD_ConveyerSlowDownSen
            .UD_ConveyerReachSen = _cassetteLift.UD_ConveyerReachSen

         
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_1)
                .cy = Cy1
            End With
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_2)
                .cy = Cy2
            End With
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_3)
                .cy = Cy3
            End With
            'fixing failure , give up and reject cassette
            For index = 0 To .CyFixCas.Length - 1
                'only valid when fixture existed , Hsien , 2016.08.05
                With .CyFixCas(index).cy.alarmPackEndFail
                    .PossibleResponse += alarmContextBase.responseWays.IGNORE
                    .AdditionalInfo = "忽略：退出卡匣"
                End With
            Next

        End With

        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = Function() (0)     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        Return 0
    End Function
End Class
