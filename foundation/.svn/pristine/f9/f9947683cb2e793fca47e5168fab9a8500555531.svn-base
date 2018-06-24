Imports Automation.Components.Services
Imports Automation.mainIOHardware
Imports System.IO
Imports System.Text.RegularExpressions

Public Class clsRobotCresboxHandshake
    Inherits systemControlPrototype
    Public Event MeasureResultEvent As EventHandler(Of MeasureEventArgs)
    Public Class MeasureEventArgs : Inherits EventArgs
        Property resutlList As List(Of String)
    End Class

#Region "Device declare"
    ' DO
    Property do_RbRunning As ULong   'Runing1	    RB->EQ      'DO Robot Running    
    Property do_TrRequest As ULong   'TR Request	RB->EQ      'DO Robot Transfer Request
    Property do_RbLoadBusy As ULong  'Load Busy		RB->EQ      'DO Robot Busy
    Property do_RbUnLdBusy As ULong  'Unload Busy   RB->EQ      'DO Robot Busy
    Property do_RbComplete As ULong  'Complete	    RB->EQ      'DO Robot Complete
    ' DI
    Property di_EqRunning As ULong   'Runing2	    EQ->RB      'DI EQ Running
    Property di_LdRequest As ULong   'L Request	    EQ->RB      'DI EQ Load Request
    Property di_UldRequest As ULong  'U Request	    EQ->RB      'DI EQ Unload Request
    Property di_EqReady As ULong     'Ready		    EQ->RB      'DI EQ Ready for Transfer
    Property di_DoorOpen As ULong                               'DI Is EQ door open
#End Region
#Region "External Data declare"
    Property RobotLoad As New flagController(Of interlockedFlag)
    Property RobotUnLoad As New flagController(Of interlockedFlag)
#End Region
#Region "Internal Data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 5)}
#End Region

    Enum ExcuteEnum
        _0_CheckEqRun               'check Equipment Running
        _1_WaitRequest              'wait for load/unload request
        _2_TransferOn               'turn on transfer request
        _3_WaitEqReady              'wait for equipment ready
        _4_CheckDoorOpen            'check the equipment door is opened
        _5_StartLoadUnload          'ask Robot to do load/unload
        _6_WaitRobotComplete        'wait for robot load/unload complete
        _7_CheckRequestOff          'check equipment request off
        _100_LoopEnd
    End Enum

    Enum LULstate
        _Load
        _Unload
    End Enum
    Private _HandshakeState As Integer = 0

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub

    Function initMappingAndSetup()

        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Return 0
    End Function

    Private Sub SensorAlarm(sensor As ULong, reason As alarmContentSensor.alarmReasonSensor, information As String, response As alarmContentSensor.responseWays)
        Dim ap As New alarmContentSensor With {.Sender = Me, .Inputs = sensor, .Reason = reason, .AdditionalInfo = information, .PossibleResponse = response}
        With ap
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                         Return True
                                                                     End Function
            '.CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean

            '                                                              Return True
            '                                                          End Function
            CentralAlarmObject.raisingAlarm(ap)
        End With
    End Sub

    Private Sub OtherAlarm(information As String, response As alarmContentSensor.responseWays)
        Dim ap As New alarmContextBase With {.Sender = Me, .AdditionalInfo = information, .PossibleResponse = response}
    End Sub

    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0_CheckEqRun
                writeBit(do_RbRunning, True)
                If readBit(di_EqRunning) Then
                    cStep = ExcuteEnum._1_WaitRequest
                Else
                    ' Warning: EQ no power on                    
                End If

            Case ExcuteEnum._1_WaitRequest
                If readBit(di_LdRequest) OrElse readBit(di_UldRequest) Then
                    If readBit(di_LdRequest) AndAlso readBit(di_UldRequest) Then
                        ' Alarm (Load and Unload both On)
                        Me.OtherAlarm("Load and unload request are both ON.", alarmContextBase.responseWays.RETRY)
                    Else
                        cStep = ExcuteEnum._2_TransferOn
                    End If
                End If

            Case ExcuteEnum._2_TransferOn
                writeBit(do_TrRequest, True)
                tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 5)  ' Wait 5 seconds
                cStep = ExcuteEnum._3_WaitEqReady

            Case ExcuteEnum._3_WaitEqReady
                If readBit(di_EqReady) Then
                    cStep = ExcuteEnum._4_CheckDoorOpen
                ElseIf tmr.IsTimerTicked = True Then
                    'Alarm (time out: Equipment Ready should be On)
                    Me.SensorAlarm(di_EqReady, alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON, _
                                        "time out: Equipment Ready should be ON", alarmContextBase.responseWays.RETRY)
                End If
            Case ExcuteEnum._4_CheckDoorOpen
                If readBit(di_DoorOpen) = False Then
                    cStep = ExcuteEnum._5_StartLoadUnload
                Else
                    'Alarm (Equipment door doesn't open)
                    Me.SensorAlarm(di_DoorOpen, alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON, _
                                        "Equipment door doesn't open.", alarmContextBase.responseWays.RETRY)
                End If

            Case ExcuteEnum._5_StartLoadUnload
                If readBit(di_LdRequest) = True AndAlso readBit(di_UldRequest) = False Then    'Load
                    _HandshakeState = LULstate._Load
                    writeBit(do_RbLoadBusy, True)
                    'writeBit(do_RbUnldBusy, True)
                    RobotLoad.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)
                    'tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerMinute * 5))     ' Wait 5 minutes
                    cStep = ExcuteEnum._6_WaitRobotComplete
                ElseIf readBit(di_LdRequest) = False AndAlso readBit(di_UldRequest) = True Then    'Unload
                    'writeBit(do_RbLoadBusy, True)
                    _HandshakeState = LULstate._Unload
                    writeBit(do_RbUnLdBusy, True)
                    RobotUnLoad.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)
                    'tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerMinute * 5))     ' Wait 5 minutes
                    cStep = ExcuteEnum._6_WaitRobotComplete
                Else
                    'Alarm (Load/Unload request error)
                    Me.SensorAlarm(di_UldRequest, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, _
                                        "Load and Unload Request Error.", alarmContextBase.responseWays.RETRY)
                End If

            Case ExcuteEnum._6_WaitRobotComplete
                If RobotLoad.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False AndAlso RobotUnLoad.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    writeBit(do_RbLoadBusy, False)
                    writeBit(do_RbUnLdBusy, False)
                    writeBit(do_TrRequest, False)
                    writeBit(do_RbComplete, True)
                    tmr.IsEnabled = True : tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 5)      ' Wait 5 mins
                    cStep = ExcuteEnum._7_CheckRequestOff

                    'ElseIf tmr.IsTimerTicked = True Then
                    'Alarm (time out: Robot didn't complete)
                    'Me.OtherAlarm("Robot didn't complete load/unload.", alarmContextBase.responseWays.RETRY)
                End If

            Case ExcuteEnum._7_CheckRequestOff

                If _HandshakeState = LULstate._Load Then
                    If readBit(di_LdRequest) = False AndAlso readBit(di_EqReady) = False Then
                        writeBit(do_RbComplete, False)
                        cStep = ExcuteEnum._100_LoopEnd
                    End If
                Else
                    If readBit(di_UldRequest) = False AndAlso readBit(di_EqReady) = False Then
                        writeBit(do_RbComplete, False)
                        readData()
                        cStep = ExcuteEnum._100_LoopEnd
                    End If
                End If

                'ElseIf tmr.IsTimerTicked = True Then
                If tmr.IsTimerTicked = True AndAlso cStep <> ExcuteEnum._100_LoopEnd Then
                    'Alarm (time out: Load/Unload request and Equipment ready should be off)
                    If readBit(di_LdRequest) Then
                        Me.SensorAlarm(di_LdRequest, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, "", alarmContextBase.responseWays.RETRY)
                    End If
                    If readBit(di_UldRequest) Then
                        Me.SensorAlarm(di_UldRequest, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, "", alarmContextBase.responseWays.RETRY)
                    End If
                    If readBit(di_EqReady) Then
                        Me.SensorAlarm(di_EqReady, alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, "", alarmContextBase.responseWays.RETRY)
                    End If
                End If

            Case ExcuteEnum._100_LoopEnd
                cStep = ExcuteEnum._0_CheckEqRun

        End Select
        Return 0
    End Function
    Function readData()
        Dim result As New List(Of String)
        Using sr As New StreamReader("Z:\LotID.CSV")
            Do While sr.Peek <> -1
                Dim str As String = sr.ReadLine
                Dim match1 As Match = Regex.Match(str, "\x22RES_AVE\x22.+") '".+\x22RES_AVE\x22,((-?\d+)(\.\d+)).+")
                If match1.Success = True Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(1))
                End If
                Dim match2 As Match = Regex.Match(str, "\x22POINT\x22,1")
                If match2.Success Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(11))
                End If
                match2 = Regex.Match(str, "\x22POINT\x22,2")
                If match2.Success Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(11))
                End If
                match2 = Regex.Match(str, "\x22POINT\x22,3")
                If match2.Success Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(11))
                End If
                match2 = Regex.Match(str, "\x22POINT\x22,4")
                If match2.Success Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(11))
                End If
                match2 = Regex.Match(str, "\x22POINT\x22,5")
                If match2.Success Then
                    Dim ary As String() = Strings.Split(str, ",")
                    result.Add(ary(11))
                    RaiseEvent MeasureResultEvent(Me, New MeasureEventArgs With {.resutlList = result})
                    Exit Do
                End If
            Loop
        End Using
        'File.Delete("Z:\LotID.csv")
        File.Move("Z:\LotID.csv", "Z:\backup" & Now.ToString("yyyy-MM-dd_HH.mm.ss") & ".csv")
        Return 0
    End Function
End Class
