﻿Imports Automation
Imports Automation.Components.Services
Imports System.ComponentModel
Imports System.IO.Ports
Imports System.Text
Imports System.Text.RegularExpressions
Public Interface ISingleWeight
    Property SingleWeightData As Single
End Interface

Public Class clsWeightStation
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public Property TargetPositionInfo As shiftDataPackBase 'Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo 'link to lane
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property StableRange As Single = 0.001
    Public Property Timeout As Integer = 6
    Property CurrentValue As Single = 0
    Property TimePerCycle As Double = 500
    Public WaferExistSen As sensorControl = New sensorControl
    Public Property MaxAutoRetry As Integer = 1
    Public cyClose As cylinderGeneric = New cylinderGeneric
    Dim __serialPort As SerialPort = New SerialPort
    Dim comOpen As Boolean = False 'serialPort opened ornot
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}
    Dim ap As alarmContextBase = New alarmContextBase With {.Sender = Me, .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE}


    Public Event WeightEngaged(ByVal sender As Object, ByVal e As EventArgs)
    Property DatasCompare As UInteger = 5
    Dim datasCount As Integer = 0
    Dim DatasList As New List(Of Double)
    Dim dataLogger As logHandler
    Dim retryCounter As Integer = 0

    Public Enum ControlEnum
        Bypass
        Initialize 'for make balance to be zero 
        ReadError
        WaitforStableVaule
    End Enum
    Public controlFlag As flagController(Of ControlEnum) = New flagController(Of ControlEnum)
#Region "Device declare"
    <DisplayName("設定串列埠名稱")> Property ComPort As String
        Get
            Return __serialPort.PortName
        End Get
        Set(value As String)
            '1. com port name
            '2. baud rate
            '3. odd check
            '4. data bit length
            '5. stop bit
            __serialPort = New SerialPort(value,
                                          9600,
                                          Parity.None,
                                          8,
                                          StopBits.One)
            Try
                'reopen , and clear buffer content
                With __serialPort
                    .Open()
                    .DiscardInBuffer()
                    .DiscardOutBuffer()
                    .Encoding = Encoding.ASCII
                    comOpen = __serialPort.IsOpen  'if serialPort open successed, comOpen = True
                End With
            Catch ex As Exception
                comOpen = False
                MsgBox("Error Open: " & ex.Message)
            End Try

        End Set
    End Property



#End Region

    Private Function stateIGNITE() As Integer
        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    TargetPositionInfo.ModuleAction.setFlag(interlockedFlag.POSITION_OCCUPIED)
                    cyClose.drive(cylinderControlBase.cylinderCommands.GO_A_END) '不做動下秤重蓋為open
                    ap.PossibleResponse = alarmContextBase.responseWays.RETRY
                    systemSubState = 5
                End If

            Case 5 'open gate
                If cyClose.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 10
                End If

            Case 10
                If Not WaferExistSen.IsSensorCovered Then
                    ap.PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                    ap.CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                    systemSubState = 200
                                                                                    Return True
                                                                                End Function
                    systemSubState = 20
                Else
                    ap.AdditionalInfo = "請將磅秤上的wafer清除"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() (True)
                    CentralAlarmObject.raisingAlarm(ap)
                End If



            Case 20 'check bypass
                If controlFlag.viewFlag(ControlEnum.Bypass) Then
                    systemSubState = 200
                Else
                    systemSubState = 30
                End If

            Case 30 'isnitialize the balance setting
                clearSerialBuffer()
                __serialPort.Write("M80 0 0" & vbLf) 'disable auto shutdown
                tmr.TimerGoal = New TimeSpan(0, 0, 1)
                tmr.IsEnabled = True
                systemSubState = 40

            Case 40 'check balance return command acknowledge
                If __serialPort.ReadExisting.Equals("M80 A" & vbCrLf & "") Then
                    systemSubState = 50
                ElseIf tmr.IsTimerTicked Then
                    ap.AdditionalInfo = "秤重機未回傳命令確認"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 30
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)

                End If


            Case 50 'command set for alaways on backLight
                clearSerialBuffer()
                __serialPort.Write("M81 1 0" & vbLf)
                tmr.IsEnabled = True
                systemSubState = 60

            Case 60 'check balance return command acknowledge
                If __serialPort.ReadExisting.Equals("M81 A" & vbCrLf & "") Then
                    systemSubState = 65
                ElseIf tmr.IsTimerTicked Then
                    ap.AdditionalInfo = "秤重機未回傳命令確認"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 50
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)

                End If

            Case 65
                If cyClose.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                    IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 70
                End If


            Case 70 'set zero
                clearSerialBuffer()
                __serialPort.Write("Z" & vbLf)
                tmr.IsEnabled = True
                systemSubState = 75

            Case 75
                If tmr.IsTimerTicked = True Then
                    tmr.IsEnabled = True
                    systemSubState = 80
                End If

            Case 80
                If __serialPort.ReadExisting.Equals("Z A" & vbCrLf & "") Then
                    systemSubState = 90
                ElseIf tmr.IsTimerTicked Then
                    ap.AdditionalInfo = "秤重機未回傳命令確認"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 70
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)

                End If

            Case 90 'check stable weight is 0
                clearSerialBuffer()
                __serialPort.Write("S" & vbLf)
                tmr.TimerGoal = New TimeSpan(0, 0, Timeout)
                tmr.IsEnabled = True
                systemSubState = 95

            Case 95
                If tmr.IsTimerTicked Then
                    systemSubState = 100
                    tmr.IsEnabled = True
                End If

            Case 100
                Dim strWeight As String = getWeight()
                If String.IsNullOrEmpty(strWeight) = False AndAlso Val(strWeight) = 0 Then
                    systemSubState = 200
                ElseIf tmr.IsTimerTicked Then
                    ap.AdditionalInfo = "秤重機未在穩定狀態"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 70
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)

                End If

            Case 200
                If cyClose.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 210
                End If

            Case 210
                systemSubState = 0
                TargetPositionInfo.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                ap.CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                                systemSubState = 100
                                                                                Return True
                                                                            End Function
                systemMainState = systemStatesEnum.EXECUTE



        End Select
        Return 0
    End Function
    Private Function stateExecute() As Integer

        '必須在待機環境下才可由外部歸零
        If controlFlag.readFlag(ControlEnum.Initialize) AndAlso systemSubState = 0 Then
            FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
            systemMainState = systemStatesEnum.IGNITE
        End If

        Select Case systemSubState

            Case 0
                If TargetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    retryCounter = 0
                    systemSubState = 10
                ElseIf controlFlag.readFlag(ControlEnum.ReadError) Then
                    systemSubState = 200
                Else
                    '----------------------
                    ' wait for Trigger
                    '---------------------
                End If

            Case 10
                If controlFlag.viewFlag(ControlEnum.Bypass) Then
                    'Realease ModuleAction
                    systemSubState = 100
                Else
                    systemSubState = 20 'do it
                End If

            Case 20 'close gate
                If cyClose.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    datasCount = 0
                    DatasList.Clear()
                    tmr.TimerGoal = New TimeSpan(0, 0, 3)
                    tmr.IsEnabled = True
                    systemSubState = 21
                End If

            Case 21
                If tmr.IsTimerTicked Then
                    systemSubState = 25
                End If

            Case 25
                If controlFlag.viewFlag(ControlEnum.WaitforStableVaule) Then
                    systemSubState = 60
                Else
                    systemSubState = 30
                End If

            Case 30 'check balance immediately value
                clearSerialBuffer()
                __serialPort.Write("SI" & vbLf)
                tmr.TimerGoal = New TimeSpan(0, 0, 0, 0, TimePerCycle)
                tmr.IsEnabled = True
                systemSubState = 40

            Case 40
                If tmr.IsTimerTicked Then
                    systemSubState = 45
                End If

            Case 45
                Dim weight As String = getWeight()
                If String.IsNullOrEmpty(weight) = True Then
                    systemSubState = 30
                ElseIf IsNumeric(weight) = True AndAlso datasCount <= DatasCompare Then
                    DatasList.Add(CDbl(weight))
                    datasCount += 1
                    systemSubState = 30
                Else
                    systemSubState = 50
                End If

            Case 50 '檢查秤重質是否區近於穩定
                Dim max As Double = DatasList.Max
                Dim min As Double = DatasList.Min

                If (max - min) < StableRange Then
                    CType(TargetPositionInfo, ISingleWeight).SingleWeightData = DatasList.Last
                    CurrentValue = DatasList.Last
                    systemSubState = 55
                Else
                    CType(TargetPositionInfo, ISingleWeight).SingleWeightData = DatasList.Last
                    CurrentValue = DatasList.Last
                    'ap.AdditionalInfo = "無法量測穩定值 Retry:再次等待  Ignore:使用目前秤重資料"
                    'ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                    '                                                               systemSubState = 20
                    '                                                               Return True
                    '                                                           End Function
                    'CentralAlarmObject.raisingAlarm(ap)
                    sendMessage(String.Format("Balance is not stable. Ignore Alarm! weight={0}  max-min={1}", DatasList.Last.ToString, (max - min).ToString))
                    If retryCounter < MaxAutoRetry Then
                        retryCounter += 1
                        systemSubState = 20
                    Else
                        systemSubState = 55
                    End If

                End If

            Case 55
                If controlFlag.viewFlag(ControlEnum.WaitforStableVaule) = True Then
                    systemSubState = 60
                Else
                    RaiseEvent WeightEngaged(Me, Nothing)
                    systemSubState = 100
                End If

            Case 60 'get Stable Value
                clearSerialBuffer()
                __serialPort.Write("@" & vbLf)
                __serialPort.Write("S" & vbLf)
                tmr.TimerGoal = New TimeSpan(0, 0, 1)
                tmr.IsEnabled = True
                systemSubState = 65

            Case 65
                If tmr.IsTimerTicked Then
                    tmr.TimerGoal = New TimeSpan(0, 0, Timeout)
                    tmr.IsEnabled = True
                    systemSubState = 70
                End If

            Case 70
                Dim strCurrentValue As String
                strCurrentValue = getWeight()
                If IsNumeric(strCurrentValue) Then
                    CType(TargetPositionInfo, ISingleWeight).SingleWeightData = CDbl(strCurrentValue)
                    CurrentValue = CDbl(strCurrentValue)
                    tmr.TimerGoal = New TimeSpan(0, 0, 1)
                    RaiseEvent WeightEngaged(Me, Nothing)
                    systemSubState = 100
                ElseIf tmr.IsTimerTicked Then
                    ap.AdditionalInfo = "秤重機未在穩定狀態"
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 60
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)
                End If



                '------------------------
                'end of balance process
                '-------------------------

            Case 100 'open gates
                If cyClose.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 110
                End If

            Case 110 'Release ModuleAction
                TargetPositionInfo.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = 0


                '----------------------------
                'Read Error Message
                '-----------------------------
            Case 200
                clearSerialBuffer()
                __serialPort.Write("E01" & vbLf) 'Query of current system error state
                tmr.IsEnabled = True
                systemSubState = 210

            Case 210
                If tmr.IsTimerTicked Then
                    ap.AdditionalInfo = __serialPort.ReadExisting
                    ap.CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                                   systemSubState = 0
                                                                                   Return True
                                                                               End Function
                    CentralAlarmObject.raisingAlarm(ap)

                End If


        End Select

        Return 0
    End Function
    Function initMappingAndSetup() As Integer

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIGNITE
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
        ap.CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                        controlFlag.setFlag(ControlEnum.Bypass)
                                                                        systemSubState = 0
                                                                        Return True
                                                                    End Function
        dataLogger = New logHandler(utilities.getFullParentName(Me)) With {.IsAsynchronWriteLog = True,
                                                           .MessengerReference = Me.CentralMessenger}
        ' log info belongs to me , only
        dataLogger.ContentFilter = Function(sender As messageHandler, e As messagePackageEventArg) (Me.IsSenderBelongToMe(e.Message.Sender))

        Return 0
    End Function
    Protected Overrides Function process() As Integer

        '----------------------------
        'standard system control flow
        '----------------------------

        drivesRunningInvoke()

        If CentralAlarmObject.IsAlarmed Then  'only stop by Alarm, not blocked by Pause
            Return 0
        End If

        stateControl()
        processProgress()

        Return 0

    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize,
                                       New Func(Of Integer)(AddressOf initLinkPause),
                               New Func(Of Integer)(AddressOf initMappingAndSetup),
                               New Func(Of Integer)(AddressOf initSubsystemInitialize),
                               New Func(Of Integer)(AddressOf initEnableAllDrives))

    End Sub
    Private Sub clearSerialBuffer()
        __serialPort.DiscardInBuffer()
        __serialPort.DiscardOutBuffer()
    End Sub
    Private Function getWeight() As String
        Dim str As String = __serialPort.ReadExisting
        Dim match As Match = Regex.Match(str, "((\w\s+)+)(-?\d+\.\d+)")
        If match.Success Then
            Return match.Groups(3).Value
        Else
            Return Nothing
        End If
    End Function
End Class
