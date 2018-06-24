Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components

Public Enum LiftMotorUsedPositions
    MOTOR_HOME
    MOTOR_LOAD
    MOTOR_LAYER2N_START
    MOTOR_LAYER2N_SUB_SPACE
    MOTOR_LAYER2N_SPACE
    MOTOR_LAYER2N_BLANK
    'MOTOR_JR_START
    'MOTOR_JR_SUB_SPACE
    'MOTOR_JR_SPACE
    'MOTOR_JR_BLANK
    MOTOR_LAYER50_START
    MOTOR_LAYER50_SUB_SPACE
    MOTOR_LAYER50_SPACE

    MOTOR_MANZ_WAIT         'stack : inner pillar very down position to let stack load-in
    MOTOR_MANZ_APPROCH      'stack : inner piller touched plate position
    MOTOR_MANZ_START        'stack : inner plate highest possible position
    MOTOR_MANZ_SUB_SPACE    'stack : inner piller driving up slow (would stopped by SD)
    MOTOR_MANZ_SPACE        'stack : inner piller driving down fast
    MOTOR_MANZ_SHELL_START
    MOTOR_MANZ_WAFER_DOWN
    MOTOR_P25X3_START_1
    MOTOR_P25X3_START_26
    MOTOR_P25X3_START_51
    MOTOR_P25X3_SUB_SPACE_1
    MOTOR_P25X3_SUB_SPACE_26
    MOTOR_P25X3_SUB_SPACE_51
    MOTOR_P25X3_SPACE
    MOTOR_UNLOAD
End Enum
Public Enum LiftShellMotorUsedPositions
    MOTOR_HOME
    MOTOR_MANZ_SHELL_LOAD
    MOTOR_MANZ_SHELL_START
    MOTOR_MANZ_SHELL_UNLOAD
End Enum
Public Enum CassetteStyle
    ''' <summary>
    ''' For standard type of 100 layers
    ''' Such as , ACI , JR
    ''' Running Sequence:
    ''' SUB_SPACE->SPACE * 50->BLANK->SPACE*50
    ''' </summary>
    ''' <remarks></remarks>
    LAYER2N_STANDARD
    ''' <summary>
    ''' For 100 layers cassette , but fetched by plate tonque
    ''' </summary>
    ''' <remarks></remarks>
    LAYER100_PLATE_TONGUE
    'JR
    ''' <summary>
    ''' For generic type of 50 layers
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    LAYER50_STANDARD
    MANZ
    P25X3

    ''' <summary>
    ''' For special cassstte as 150 layers
    ''' </summary>
    ''' <remarks></remarks>
    LAYER150_STANDARD
End Enum
Public Enum FeedDir
    MOVE_IN
    MOVE_OUT
End Enum
Public Class CassetteLift
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public Property FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    ''' <summary>
    ''' The total count for this cassette
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property GoalCount As Integer    'total count for single cassette , Hsien , 2016.03.30
        Set(value As Integer)
            __goalCount = value
        End Set
        Get
            Return __goalCount
        End Get
    End Property
    Dim __goalCount As Integer = 100
    ''' <summary>
    ''' The cassette style used for counting
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SetCasStyle As CassetteStyle
        Get
            Return __setCasStyle
        End Get
        Set(ByVal value As CassetteStyle)
            __setCasStyle = value
        End Set
    End Property

    Private __setCasStyle As CassetteStyle = CassetteStyle.LAYER2N_STANDARD

    Public liftFlags As flagController(Of flagsInLoaderUnloader)
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    'Cassette:Cas    UD:Up Down    Position:Pos    Sensor:Sen
    'Hsien , for UD motor , use hardware slowdown to implement interlock protection , 2015.09.22
    Public UD_Motor As motorControl = New motorControl With {.IsEnabled = True}

    Public UD_Shell_Motor As motorControl = New motorControl With {.IsEnabled = True}

    Public UD_ConveyerMotor As IDrivable = New motorControlDrivable With {.IsEnabled = True} 'Shared , Hsien , 2015.06.04 , compatible with DC/SERVO


    Public UD_ConveyerSlowDownSen As sensorControl = New sensorControl 'With {.IsEnabled = True} '卡匣減速感測器
    Public UD_ConveyerReachSen As sensorControl = New sensorControl 'With {.IsEnabled = True} '卡匣到達感測器

    Friend ConveyerWaferEmpty As Func(Of Boolean) = Function() (True) '輸送帶設備給的升降卡匣的狀態

    Public cntWafer As Integer = 0 '硅片計數


    'Public __setCasStyle As CassetteStyle

    Public SetWaferMoveDir As FeedDir '2015.10.24 jk: DO NOT SET IT OUTSIDE, just use Property WorkingType As workingTypeEnum to set them all

    Public ManzCasWaferSafeSen As New sensorControl With {.IsEnabled = False} '檢查硅片是否有完全進入內部卡匣(要為Off)

    Dim MotionFinished_f As Boolean

    Friend getBoxCounter As Func(Of Integer) = Function() (0) '得到clsCassetteTransport物件的工作料盒,針對P25X3型式的卡匣
    Friend setBoxCounter As Action = Sub() Console.WriteLine("") '設定clsCassetteTransport物件的工作料盒,如果為3則會載出卡匣
    Dim blnRaiseSpaceAgain As Boolean = False



    Public Function stateExecute() As Integer

        FinishableFlag.readFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) 'keep reset

        Select Case systemSubState
            '-----------------------------------------------------
            '              硅片移出到輸送帶
            '-----------------------------------------------------
            Case 0 '檢查馬達是否在升降平台上
                If liftFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) Then
                    systemSubState = 20
                End If
            Case 20 '設定旗標
                liftFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, True) '可使極板放入旗標
                systemSubState = 30
            Case 30 '等待重置
                If Not liftFlags.viewFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f) Then
                    cntWafer = cntWafer + 1   '計數料匣的位置
                    MotionFinished_f = False

                    UD_Motor.SlowdownEnable = enableEnum.DISABLE    'shut-off slowdown let UD_motor able to do pitch move , Hsien , 2015.10.12
                    systemSubState = 70

                ElseIf liftFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f) Then '按下清卡匣鈕
                    '如果按下清卡匣鈕
                    '--------------------------------------------------------------------------------------
                    '在硅片載出卡匣情形下,此旗標設沒有作用,在硅片載入卡匣的情形下,此旗標設為True時,暫存區開始收料
                    liftFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, True) '使輸送帶開始儲料
                    '--------------------------------------------------------------------------------------
                    If ConveyerWaferEmpty() Then '清卡匣時要,檢查舌頭輸送帶上的兩個位置是否有硅片
                        liftFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, False)
                        liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False)
                        liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                        setBoxCounter() '在此要把clsCassetteTransport的BoxCnt設為3,載出卡匣
                        cntWafer = 0
                        systemSubState = 0
                    End If
                ElseIf UpstreamStation IsNot Nothing AndAlso
                    UpstreamStation.Count > 0 AndAlso
                    UpstreamStation.TrueForAll(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED))) Then '收料
                    '收料
                    liftFlags.writeFlag(flagsInLoaderUnloader.BufferCanStore_f, True) '使輸送帶開始儲料
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, False)
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False)
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True)
                    setBoxCounter() '在此要把clsCassetteTransport的BoxCnt設為3,載出卡匣
                    cntWafer = 0
                    systemSubState = 0
                End If

            Case 70 '移動到的位置SubSpace,Space or Blank
                Dim PosIndex As LiftMotorUsedPositions
                PosIndex = DecideLifterPostion() '傳回判別後決定馬達要上升的位置點
                If UD_Motor.drive(motorControl.motorCommandEnum.GO_POSITION, PosIndex) = motorControl.statusEnum.EXECUTION_END Then
                    UD_Motor.SlowdownEnable = enableEnum.ENABLE     'resume protection , Hsien , 2015.10.12
                    MotionFinished_f = True '馬達升降完成
                End If

                If MotionFinished_f Then 'MotionFinished_f=True可以即時設定CasMove_UD_Ok_f=True

                    Select Case __setCasStyle '設定檢查硅片的個數
                        Case CassetteStyle.LAYER2N_STANDARD
                            __goalCount = GoalCount
                        Case CassetteStyle.LAYER100_PLATE_TONGUE
                            __goalCount = 100
                        Case CassetteStyle.LAYER50_STANDARD,
                            CassetteStyle.MANZ
                            __goalCount = 50
                        Case CassetteStyle.LAYER150_STANDARD
                            __goalCount = 150
                        Case CassetteStyle.P25X3
                            __goalCount = 25 * getBoxCounter()
                    End Select

                    If cntWafer < __goalCount Then '檢查被移出的硅片的個數,未到達卡匣的片數

                        If __setCasStyle = CassetteStyle.P25X3 Then
                            If liftFlags.viewFlag(flagsInLoaderUnloader.IgnoreCasFirstWafer_f) Then '忽略每一層卡匣的第一片
                                If cntWafer = 1 Or cntWafer = 26 Or cntWafer = 51 Then
                                    cntWafer = cntWafer + 1   '計數料匣的位置
                                    MotionFinished_f = False
                                    Return 0
                                Else
                                    systemSubState = 20
                                End If
                            Else
                                systemSubState = 20
                            End If
                        ElseIf __setCasStyle = CassetteStyle.LAYER100_PLATE_TONGUE Then '阿特斯卡匣上升方式(Blank<未到逹硅片>+Space<到達硅片>)
                            If cntWafer = 51 And (Not blnRaiseSpaceAgain) Then
                                blnRaiseSpaceAgain = True '使卡匣再卡升一格(為了和40MW的升降動作一致)
                                MotionFinished_f = False
                                Return 0
                            Else
                                If blnRaiseSpaceAgain = True Then blnRaiseSpaceAgain = False
                                systemSubState = 20
                            End If
                        Else
                            'cassette other than P25X3 , ACI_CANADIAN
                            systemSubState = 20
                        End If

                    Else
                        '到達卡匣的片數
                        '所有的硅片已被移出/移入

                        If __setCasStyle = CassetteStyle.MANZ And SetWaferMoveDir = FeedDir.MOVE_OUT Then
                            liftFlags.writeFlag(flagsInLoaderUnloader.CasCollect_f, True) '主要目的使舌頭輸送帶上的兩個位置沒有硅片
                            liftFlags.writeFlag(flagsInLoaderUnloader.CasReadyWaferInOut_f, True) '使CassetteFeed可以繼續運作
                            systemSubState = 30
                        Else
                            liftFlags.writeFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f, False) '設定卡匣未備便
                            liftFlags.writeFlag(flagsInLoaderUnloader.CasUnloadEnable_f, True) '設定卡匣要被移出

                            'condition : (P25X3 and thrid box)  or Not the P25X3
                            '卡匣為第三料盒,硅片個數才能清為0,不然個會疊加
                            If ((__setCasStyle = CassetteStyle.P25X3) And (getBoxCounter() = BoxSelect.P25X3_Box_3)) Or
                                (Not (__setCasStyle = CassetteStyle.P25X3)) Then
                                cntWafer = 0
                            End If

                            systemSubState = 0

                        End If

                    End If
                    liftFlags.writeFlag(flagsInLoaderUnloader.CasMove_UD_Ok_f, True) '使輸送帶可以移動
                Else
                    '--------------------------
                    '   Motion Not Ready
                    '--------------------------
                End If

        End Select

        Return 0

    End Function
    Function DecideLifterPostion() As LiftConveyerUsedPositions
        '判別後決定馬達要上升的位置點
        Dim PosIndex As LiftMotorUsedPositions

        Select Case SetWaferMoveDir '硅片流出/入卡匣方式選擇
            Case FeedDir.MOVE_OUT '硅片流出卡匣
                Select Case __setCasStyle '卡匣型式選擇
                    Case CassetteStyle.LAYER2N_STANDARD
                        If cntWafer = 1 Then 'PreSpaceDist
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SUB_SPACE
                        ElseIf cntWafer = (GoalCount / 2) + 1 Then 'BlankPos
                            If __setCasStyle = CassetteStyle.LAYER100_PLATE_TONGUE Then
                                If blnRaiseSpaceAgain Then '再上升一格
                                    PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                                Else
                                    PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                                End If
                            End If
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                        Else '<=(GoalCount / 2) - 1 or >=(GoalCount / 2) + 1'SpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                        End If
                    Case CassetteStyle.LAYER100_PLATE_TONGUE,
                       CassetteStyle.LAYER150_STANDARD
                        If cntWafer = 1 Then 'PreSpaceDist
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SUB_SPACE
                        ElseIf cntWafer = 51 Then 'BlankPos
                            If __setCasStyle = CassetteStyle.LAYER100_PLATE_TONGUE Then
                                If blnRaiseSpaceAgain Then '再上升一格
                                    PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                                Else
                                    PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                                End If
                            End If
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                        Else '<=49 or >=51'SpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                        End If
                    Case CassetteStyle.LAYER50_STANDARD, CassetteStyle.MANZ '50層
                        If cntWafer = 1 Then 'PreSpaceDist
                            If __setCasStyle = CassetteStyle.LAYER50_STANDARD Then PosIndex = LiftMotorUsedPositions.MOTOR_LAYER50_SUB_SPACE
                            If __setCasStyle = CassetteStyle.MANZ Then PosIndex = LiftMotorUsedPositions.MOTOR_MANZ_SUB_SPACE
                        Else '<=50 SpacePos
                            If __setCasStyle = CassetteStyle.LAYER50_STANDARD Then PosIndex = LiftMotorUsedPositions.MOTOR_LAYER50_SPACE
                            If __setCasStyle = CassetteStyle.MANZ Then PosIndex = LiftMotorUsedPositions.MOTOR_MANZ_SPACE
                        End If
                    Case CassetteStyle.P25X3 '75層
                        If cntWafer = 1 Then 'PreSpaceDist1
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_1
                        ElseIf cntWafer = 26 Then 'PreSpaceDist26
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_26
                        ElseIf cntWafer = 51 Then 'PreSpaceDist51
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_51
                        Else '<=75 SpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SPACE
                        End If
                End Select
            Case FeedDir.MOVE_IN '硅片流入卡匣
                Select Case __setCasStyle '卡匣型式選擇
                    Case CassetteStyle.LAYER2N_STANDARD
                        If cntWafer <= (GoalCount / 2) - 1 Or (cntWafer > (GoalCount / 2) And cntWafer < GoalCount) Then
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                        ElseIf cntWafer = (GoalCount / 2) Then 'BlankPos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                        ElseIf cntWafer = GoalCount Then 'PreSpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SUB_SPACE
                        End If
                    Case CassetteStyle.LAYER100_PLATE_TONGUE,
                         CassetteStyle.LAYER150_STANDARD
                        If cntWafer <= 49 Or (cntWafer > 50 And cntWafer < GoalCount) Then  '<=49 or >50 'SpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE
                        ElseIf cntWafer = 50 Then 'BlankPos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK
                        ElseIf cntWafer = GoalCount Then 'PreSpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_SUB_SPACE
                        End If
                    Case CassetteStyle.LAYER50_STANDARD, CassetteStyle.MANZ
                        If cntWafer = 50 Then 'PreSpaceDist
                            If __setCasStyle = CassetteStyle.LAYER50_STANDARD Then PosIndex = LiftMotorUsedPositions.MOTOR_LAYER50_SUB_SPACE
                            If __setCasStyle = CassetteStyle.MANZ Then PosIndex = LiftMotorUsedPositions.MOTOR_MANZ_SUB_SPACE
                        Else '<=50 SpacePos
                            If __setCasStyle = CassetteStyle.LAYER50_STANDARD Then PosIndex = LiftMotorUsedPositions.MOTOR_LAYER50_SPACE
                            If __setCasStyle = CassetteStyle.MANZ Then PosIndex = LiftMotorUsedPositions.MOTOR_MANZ_SPACE
                        End If
                    Case CassetteStyle.P25X3
                        If cntWafer = 25 Then 'PreSpaceDist26
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_51
                        ElseIf cntWafer = 50 Then 'PreSpaceDist51
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_26
                        ElseIf cntWafer = 75 Then 'PreSpaceDist1
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SUB_SPACE_1
                        Else '<=75 SpacePos
                            PosIndex = LiftMotorUsedPositions.MOTOR_P25X3_SPACE
                        End If
                End Select
        End Select
        'prevent human setting errors... 2016.2.22 jk
        Select Case PosIndex
            Case LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE,
                LiftMotorUsedPositions.MOTOR_LAYER50_SPACE,
                LiftMotorUsedPositions.MOTOR_MANZ_SPACE,
                LiftMotorUsedPositions.MOTOR_P25X3_SPACE
                pData.MotorPoints(UD_Motor.PositionDictionary(PosIndex)).PointType = pointTypeEnum.REL
        End Select

        '' For 150 layer cassette , do internal overriden  , 
        '' Hsien , 2016.09.09
        If (__setCasStyle = CassetteStyle.LAYER150_STANDARD) And
            PosIndex = LiftMotorUsedPositions.MOTOR_LAYER2N_BLANK Then
            With pData.MotorPoints(UD_Motor.PositionDictionary(PosIndex))
                .PointType = pointTypeEnum.REL
                .DistanceInUnit = pData.MotorPoints(UD_Motor.PositionDictionary(LiftMotorUsedPositions.MOTOR_LAYER2N_SPACE)).DistanceInUnit
            End With
        End If

        Return PosIndex

    End Function
    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf initEnableAllDrives))
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute     '鍊結主狀態函式
        systemMainState = systemStatesEnum.EXECUTE   '設定初始主狀態

        'Hsien , 2015.10.12 , for lifter motor , use slow-down to do whid-wide protection
        With UD_Motor
            .SlowdownEnable = enableEnum.ENABLE 'enabled since start
            .SlowdownLatch = sdLatchEnum.LATCH
            .SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
        End With

        Return 0
    End Function


    Sub lifterPauseHandler() Handles PauseBlock.InterceptedEvent, CentralAlarmObject.alarmOccured
        '升降暫停
        UD_Motor.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
    End Sub
    Sub lifterUnpauseHandler() Handles PauseBlock.UninterceptedEvent, CentralAlarmObject.alarmReleased
        '升降恢復
        UD_Motor.drive(motorControl.motorCommandEnum.MOTION_RESUME)
    End Sub

    '-----------------------------------------
    'Hsien , 2015.10.12 , the local translator
    '-----------------------------------------
    Sub translateLifterMotorAlarm(sender As alarmManager, e As EventArgs) Handles CentralAlarmObject.alarmOccured
        If (UD_Motor.IsMyAlarmCurrent AndAlso UD_Motor.ErrorStatus = errorStatusEnum.STOPPED_SD_ON) OrElse
            (UD_Shell_Motor.IsMyAlarmCurrent AndAlso UD_Shell_Motor.ErrorStatus = errorStatusEnum.STOPPED_SD_ON) Then

            sender.CurrentAlarm.PossibleResponse += alarmContextBase.responseWays.RETRY ' able to retry
            sender.CurrentAlarm.AdditionalInfo = My.Resources.AlarmPackLifterInterfere
        End If
    End Sub


End Class

