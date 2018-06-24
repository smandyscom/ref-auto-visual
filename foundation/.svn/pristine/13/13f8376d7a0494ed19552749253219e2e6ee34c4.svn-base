Imports Automation
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components
Imports Automation.mainIOHardware

Public Class StackWaferPlace : Inherits systemControlPrototype

    Implements IFinishableStation
    Public Event DoubleWaferEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Event DoubleWaferFlowEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Property _FinishableFlag As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property _UpstreamStation As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    Public PlaceFlags As flagController(Of flagsInLoaderUnloader)

    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Const SuckerNum As Short = 2
    Private SideIndex As Integer = 0

    Public ConveyerMotionOkCasAction As Func(Of Boolean) '輸送帶設備給的旗標
    Public PlaceWaferOnConveyerOK As Func(Of Boolean, Boolean) = Function() (False) '放置硅片在輸送帶上完成

    Public CheckReadyToPlaceWafer As Func(Of Boolean) = Function() (False) '檢查是否有硅片要放置在輸送帶上
    Public CheckWaferReadyOnSucker As Func(Of Boolean) = Function() (False) '檢查是否有硅片要放置在輸送帶上
    Public GetNowPlaceWaferSide As Func(Of Integer) = Function() (1) '得到目前要放置硅片的邊
    Public ResetReadyToPlaceWafer As Action(Of Integer) = Sub()
                                                          End Sub '重置可放硅片邊的旗標
    Public ConveyerMotionReset As Action = Sub()

                                           End Sub '重置移動旗標

    Public VacGenerate(SuckerNum) As ULong '真空產生電磁閥
    Public VacDestroy(SuckerNum) As ULong '真空破壞電磁閥
    Public VacSeneor(SuckerNum) As sensorControl  '真空感測器

    Public DoubleWaferSenser As New sensorControl With {.IsEnabled = True} '疊片感測器

    Public RejectBinEnable As Boolean = False
    Public RejectBinCylinder As New cylinderGeneric With {.IsEnabled = True} '排出硅片上下氣缸
    Public RejectBinFullSen As New sensorControl With {.IsEnabled = True} '排出硅片盒滿料
    Public WaferFlowAwaySen As New sensorControl With {.IsEnabled = True} '排出硅片已從輸送帶流走
    Public RejectBinWaferExistSen As New sensorControl With {.IsEnabled = True} '排出硅片盒滿料


    Public cntWafer As Integer



    Public Function stateIgnite() As Integer
        If _FinishableFlag.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
            _FinishableFlag.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)   'Hsien , 2015.04.09
            systemMainState = systemStatesEnum.EXECUTE
        End If
        Return 0
    End Function
    Public Function stateExecute() As Integer

        Select Case systemSubState
            Case 0 '檢查放置旗標是否致能
                If ConveyerMotionOkCasAction() Then '輸送帶停止,可以放置硅片
                    If CheckReadyToPlaceWafer() Then '檢查是否有硅片要放在輸送帶上
                        SideIndex = GetNowPlaceWaferSide()
                        systemSubState = 30
                    Else
                        If CheckWaferReadyOnSucker() = False Then '任何一支手臂上有硅片為True
                            Call ConveyerMotionReset() '重置移動旗標
                        End If
                    End If
                End If
            Case 30 '產生真空破壞
                Call writeBit(VacGenerate(SideIndex), False) '關閉真空
                Call writeBit(VacDestroy(SideIndex), True) '真空破壞產生
                tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, 200)
                tmr.IsEnabled = True    'restart
                systemSubState = 50
            Case 50
                If tmr.IsTimerTicked Then
                    Call writeBit(VacDestroy(SideIndex), False) '真空破壞關閉
                    Call ResetReadyToPlaceWafer(SideIndex) '放置旗標去能
                    systemSubState = 80
                End If
            Case 70 '等待真空感測器Off
                If Not VacSeneor(SideIndex).IsSensorCovered Then
                    Call writeBit(VacDestroy(SideIndex), False) '真空破壞關閉
                    Call ResetReadyToPlaceWafer(SideIndex) '放置旗標去能
                    systemSubState = 80
                Else
                    If tmr.IsTimerTicked Then
                        Dim ap As New alarmContentSensor
                        With ap
                            .Sender = Me
                            .Inputs = VacSeneor(SideIndex).InputBit
                            .PossibleResponse = alarmContextBase.responseWays.RETRY
                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         Return True
                                                                                     End Function
                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    End If
                End If
            Case 80
                If (DoubleWaferSenser.IsSensorCovered) = True Then '沒有疊片
                    systemSubState = 90
                Else
                    RaiseEvent DoubleWaferEvent(Me, Nothing)

                    If Not RejectBinEnable Then '疊片是否要自動排出
                        Dim ap As New alarmContentSensor
                        With ap
                            .Inputs = DoubleWaferSenser.InputBit
                            .PossibleResponse = alarmContextBase.responseWays.RETRY
                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         Return True
                                                                                     End Function

                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    Else '自動排出
                        systemSubState = 100
                    End If
                End If
            Case 90 '重置馬達移動旗標及決定輸送帶上有無片子(有:True)
                If PlaceWaferOnConveyerOK(True) Then
                    'non-double wafer treat as picked?
                    systemSubState = 0
                End If
                '------------------------
                '       排料程序
                '------------------------
            Case 100 '排料盒未滿料
                If Not RejectBinFullSen.IsSensorCovered Then
                    systemSubState = 110
                Else
                    Dim ap As New alarmContentSensor
                    With ap
                        .Inputs = RejectBinFullSen.InputBit
                        .PossibleResponse = alarmContextBase.responseWays.RETRY
                        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                     Return True
                                                                                 End Function

                        CentralAlarmObject.raisingAlarm(ap)
                    End With
                End If
            Case 110 '排料馬達上升
                If RejectBinCylinder.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 120
                End If
            Case 120 '檢查硅片是否流走
                If Not WaferFlowAwaySen.IsSensorCovered Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 2)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 130
                Else
                    If tmr.IsTimerTicked Then
                        Dim ap As New alarmContentSensor
                        With ap
                            .Inputs = WaferFlowAwaySen.InputBit
                            .PossibleResponse = alarmContextBase.responseWays.RETRY
                            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
                            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                         systemSubState = 110
                                                                                         Return True
                                                                                     End Function

                            CentralAlarmObject.raisingAlarm(ap)
                        End With
                    End If
                End If
            Case 130 '設定時間到達
                If tmr.IsTimerTicked Then
                    systemSubState = 140
                End If
            Case 140 '排料馬達下降
                If RejectBinCylinder.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    tmr.IsEnabled = True    'restart
                    systemSubState = 150
                End If
            Case 150 '等一時間
                If tmr.IsTimerTicked Then
                    RaiseEvent DoubleWaferFlowEvent(Me, Nothing)
                    systemSubState = 160
                End If
            Case 160
                If PlaceWaferOnConveyerOK(False) Then
                    systemSubState = 0
                End If
        End Select
        Return 0
    End Function
    Sub New()
        '將自定義起始化函式加入 通用起始化引動清單
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Function initMappingAndSetup() As Integer
        '本站主狀態函式設定
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite       '鍊結主狀態函式
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute     '鍊結主狀態函式
        systemMainState = systemStatesEnum.IGNITE   '設定初始主狀態
        Return 0
    End Function
End Class
