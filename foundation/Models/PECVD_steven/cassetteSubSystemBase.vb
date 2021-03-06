﻿Imports Automation.Components.Services
Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.CassetteFeed

Public Class cassetteSystemBase
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
    Property GetWaferMethod As getWaferMethodEnum
        Get
            Return _cassetteFeed.GetWaferMethod
        End Get
        Set(value As getWaferMethodEnum)
            _cassetteFeed.GetWaferMethod = value
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
    Public WithEvents _cassetteTransport As CassetteTransport = New CassetteTransport
    Public _cassetteFeed As CassetteFeed = New CassetteFeed With {.SetWaferMoveDir = FeedDir.MOVE_IN}
    Public _cassetteLift As CassetteLift = New CassetteLift With {.SetWaferMoveDir = FeedDir.MOVE_IN}   'the default direction , Hsien , 2015.05.14
    Public _cassetteButton As CassetteButton = New CassetteButton

    Public Cy1 As cylinderGeneric = New cylinderGeneric
    Public Cy2 As cylinderGeneric = New cylinderGeneric
    Public Cy3 As cylinderGeneric = New cylinderGeneric ' expanded by Hsien , 2016.04.08

    'Dim Ignite_Step As Integer = 0

    Dim StationCollection As List(Of IFinishableStation) = New List(Of IFinishableStation) From {_cassetteLoad, _cassetteUnload, _cassetteLift, _cassetteButton}
    Public Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0 'Assembly檢查初始程序是否被致能
                If FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then

                    initEnableAllDrives()   'Hsien , 2015.09.10 , enable all device once make sure to ignite

                    systemSubState = 10
                End If
            Case 10 '先處理_cassetteFeed舌頭回原點
                _cassetteFeed._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                systemSubState = 20
            Case 20 '等待_cassetteFeed初始程序完成
                If _cassetteFeed._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
                    systemSubState = 30
                End If
            Case 30 '使_cassetteTransport初始程序致能使升降平台上卡匣可以退出
                _cassetteTransport._FinishableFlag.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, True)
                systemSubState = 40
            Case 40 '等待_cassetteTransport初始程序完成
                If _cassetteTransport._FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = False Then
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
            '.SetWaferMoveDir = FeedDir.MOVE_IN '設定硅片要流出或流入卡匣
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
            'allocating before initMappingAndSetup , Hsien , 2015.06.24
            '.SafeStopper = New ActuatorInfo With {.sw = IS_OFF}
            ''作動時Off
            '.SafeStoperSen = New SensorInfo With {.sw = IS_OFF,
            '                                      .status = IS_OFF}
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

            .IN_ConveyerPosSen3 = _cassetteLoad.IN_ConveyerPosSen3  'Hsien , 2016.03.24, link
            'no need to use overriden sensor , Hsien , 2015.09.22
            ''感測器初始化設定
            '.IN_ConveyerOverrideSen = _cassetteLoad.IN_ConveyerOverrideSen     '設定輸入點
            '.OUT_ConveyerOverrideSen = _cassetteUnload.OUT_ConveyerOverrideSen    '設定輸入點

            '.IN_ConveyerOverrideSen.IsEnabled = True    'Hsien , 2015.05.06
            '.OUT_ConveyerOverrideSen.IsEnabled = True

            .OUT_ConveyerPosSen1 = _cassetteUnload.OUT_ConveyerPosSen1
            .OUT_ConveyerPosSen2 = _cassetteUnload.OUT_ConveyerPosSen2

            .UD_ConveyerSlowDownSen = _cassetteLift.UD_ConveyerSlowDownSen
            .UD_ConveyerReachSen = _cassetteLift.UD_ConveyerReachSen

            '.CyExtendForthSen = _cassetteFeed.CyExtendForthSen    '設定輸入點(舌頭伸出感測器)
            '.CyExtendBackSen = _cassetteFeed.CyExtendBackSen   '設定輸入點(舌頭縮回感測器)

            '卡匣形式選擇(ACI or JR)
            '.SetCasStyle = CassetteStyle.Manz

            'Hsien , 2015.06.12
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_1)
                '.sw = IS_OFF
                .cy = Cy1
            End With
            With .CyFixCas(CassetteTransport.CyFixCasIndex.CY_2)
                '.sw = IS_OFF
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


    Private Sub _cassetteTransport_CassetteCancelManualLoad(sender As Object, e As EventArgs) Handles _cassetteTransport.CassetteCancelManualLoad
        _cassetteButton.btnLoadWait.TimerGoal = New TimeSpan(0) 'force stop auto manual load
    End Sub
    Private Sub _cassetteTransport_CassetteCancelManulUnload(sender As Object, e As EventArgs) Handles _cassetteTransport.CassetteCancelManualUnload
        _cassetteButton.btnUnloadWait.TimerGoal = New TimeSpan(0)
    End Sub
End Class
#Region "mapping sample"
'sample from IOX (ASA-04-002)
'-------------------------------------------------
'   Cassette Configuration
'-------------------------------------------------
'        With lane1.cassetteSubSystem

'            With ._cassetteLoad

'                With .IN_ConveyerMotor
'                    .MotorIndex = motorAddress.MSG1B      '馬達軸對應
'                    .PositionDictionary.Add(LoadConveyerUsedPositions.MOTOR_POSITION_1, motorPoints.MSG1B_MOVE1)     '在此馬達元件連結點位關係
'                End With

'                .PB_LoadConveyerMove.InputBit = inputAddress.BPG_InM1    '載入按鈕設定輸入點
'                .LP_LoadConveyerMove = outputAddress.LPG_InM1 '載入按鈕燈設定輸入點

''卡匣形式檢查感測器
'                .IN_CasStyleCheckSen(CassetteLoad.CheckCasStyleSenIndex.SEN_1) = New SensorInfo With {.sw = IS_OFF,
'                                                                                 .status = IS_ON,
'                                                                                 .sensor = New sensorControl With {.InputBit = 0}} '凸點朝下
'                .IN_CasStyleCheckSen(CassetteLoad.CheckCasStyleSenIndex.SEN_2) = New SensorInfo With {.sw = IS_OFF,
'                                                                                 .status = IS_OFF,
'                                                                                 .sensor = New sensorControl With {.InputBit = 0}}
'                .IN_CasStyleCheckSen(CassetteLoad.CheckCasStyleSenIndex.SEN_3) = New SensorInfo With {.sw = IS_OFF,
'                                                                                 .status = IS_OFF,
'                                                                                 .sensor = New sensorControl With {.InputBit = 0}}
''卡匣在輸送帶上檢查感測器
'                .IN_ConveyerPosSen1.InputBit = inputAddress.SpG1A
'                .IN_ConveyerPosSen2.InputBit = inputAddress.SpG1B
'                .IN_ConveyerPosSen3.InputBit = inputAddress.SpG1C

'                .IN_ConveyerOverrideSen.InputBit = inputAddress.SpG1D     '設定輸入點

''單動電磁閥初始化設定
'                With .IN_Stopper1
'                    .OffEndSensor = inputAddress.G1c0
'                    .OnEndSensor = inputAddress.G1c1
'                    .ActuatorBit = outputAddress.CyG1C
'                End With

'                With .IN_Stopper2
'                    .OffEndSensor = inputAddress.G1b0
'                    .OnEndSensor = inputAddress.G1b1
'                    .ActuatorBit = outputAddress.CyG1B
'                End With

'            End With

'            With ._cassetteUnload

'                With .OUT_ConveyerMotor
'                    .MotorIndex = motorAddress.MSG1C      '馬達軸對應
'                    .PositionDictionary.Add(UnloadConveyerUsedPositions.MOTOR_POSITION_1, motorPoints.MSG1C_MOVE1)     '在此馬達元件連結點位關係
'                End With

''感測器初始化設定
'                .OUT_ConveyerOverrideSen.InputBit = inputAddress.SpG1G     '設定輸入點
'                .OUT_ConveyerPosSen1.InputBit = inputAddress.SpG1H
'                .OUT_ConveyerPosSen2.InputBit = inputAddress.SpG1I

'            End With

'            With ._cassetteLift

'                With .UD_ConveyerMotor
'                    .MotorIndex = motorAddress.MPG1A    '馬達軸對應
'                    .PositionDictionary.Add(LiftConveyerUsedPositions.MOTOR_MOVE_IN, motorPoints.MPG1A_MOVE_IN)     '在此馬達元件連結點位關係
'                    .PositionDictionary.Add(LiftConveyerUsedPositions.MOTOR_MOVE_OUT, motorPoints.MPG1A_MOVE_OUT)
'                End With

'                With .UD_Shell_Motor
'                    .MotorIndex = motorAddress.MSG1A      '馬達軸對應
'                    .PositionDictionary.Add(LiftShellMotorUsedPositions.MOTOR_Home, motorPoints.MSG1A_HOME)     '在此馬達元件連結點位關係
'                    .PositionDictionary.Add(LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_UNLOAD, motorPoints.MSG1A_MANZ_UNLOAD)
'                    .PositionDictionary.Add(LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_LOAD, motorPoints.MSG1A_MANZ_LOAD)
'                    .PositionDictionary.Add(LiftShellMotorUsedPositions.MOTOR_MANZ_SHELL_START, motorPoints.MSG1A_MANZ_START)
'                End With

'                With .UD_Motor
'                    .MotorIndex = motorAddress.MSG1D      '馬達軸對應
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_Home, motorPoints.MSG1D_HOME)     '在此馬達元件連結點位關係
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_MANZ_WAIT, motorPoints.MSG1D_MANZ_WAIT)
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_MANZ_APPROCH, motorPoints.MSG1D_MANZ_APPROCH)
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_MANZ_START, motorPoints.MSG1D_MANZ_START)
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_MANZ_SUB_SPACE, motorPoints.MSG1D_MANZ_SUB_SPACE)
'                    .PositionDictionary.Add(LiftMotorUsedPositions.MOTOR_MANZ_SPACE, motorPoints.MSG1D_MANZ_SPACE)
'                End With

'                .UD_ConveyerSlowDownSen.InputBit = inputAddress.SPG1E
'                .UD_ConveyerReachSen.InputBit = inputAddress.SLG1A

'            End With

'            With ._cassetteFeed
''感測器初始化設定
'                .WaferPassCheckSen.InputBit = inputAddress.SpA1B  '設定輸入點
'                .WaferExistCheckSen.InputBit = inputAddress.SpA1A  '設定輸入點
''單動電磁閥初始化設定
'                With .CyExtendTongue
'                    .OffEndSensor = inputAddress.A1A0
'                    .OnEndSensor = inputAddress.A1A1
'                    .ActuatorBit = outputAddress.CyA1A
'                End With
'                .CyExtendForthSen.InputBit = inputAddress.A1A1     '設定輸入點(舌頭伸出感測器)
'                .CyExtendBackSen.InputBit = inputAddress.A1A0     '設定輸入點(舌頭縮回感測器)
'            End With

'            With ._cassetteButton
''感測器初始化設定
'                .PB_Unload.InputBit = inputAddress.BPG_Out1   '設定輸入點
'                .LP_Unload = outputAddress.LPG_Out1

'                .PB_UnloadConveyerMove.InputBit = inputAddress.BPG_OutM1  '設定輸入點
'                .LP_UnloadConveyerMove = outputAddress.LPG_OutM1

'                .PB_Load.InputBit = inputAddress.BPG_In1  '設定輸入點
'                .LP_Load = outputAddress.LPG_In1

'                .PB_LoadConveyerMove.InputBit = inputAddress.BPG_InM1  '設定輸入點
'                .LP_LoadConveyerMove = outputAddress.LPG_InM1

'                .IN_ConveyerPosSen2.InputBit = inputAddress.SpG1B

'                .SafeStopper = New ActuatorInfo With {.sw = IS_OFF,
'                                                      .Actuator = outputAddress.CyG1D}
''作動時Off
'                .SafeStoperSen = New SensorInfo With {.sw = IS_OFF,
'                                                      .status = IS_OFF,
'                                                      .sensor = New sensorControl With {.InputBit = inputAddress.G1d0}}

'            End With

'            With ._cassetteTransport
''感測器初始化設定
'                .IN_ConveyerPosSen3.InputBit = inputAddress.SpG1C
'                .ManzCasSafeSen.InputBit = inputAddress.SFG1B
'                .ManzCasWaferExistSen.InputBit = inputAddress.SFG1A
'            End With

'        End With
#End Region
