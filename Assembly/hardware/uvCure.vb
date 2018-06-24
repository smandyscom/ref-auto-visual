Imports Automation
Imports AutoNumeric
Imports Automation.Components
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports System.IO.Ports
Public Enum commandList
    CONN '連接設備
    DCON '斷開設備
    LOC '鎖定前面板
    RUN '運行定時曝光
    OPN '打開檔板
    CLS '關閉檔板
    TON '打開燈泡
    TOF '關閉燈泡
    GIL '獲取光圈等級
    GTM '獲取曝光時間
    ''' <summary>
    ''' 設置光圈等級n%, 0-n-100
    ''' Iris Level
    ''' </summary>
    ''' <remarks></remarks>
    SIL
    ''' <summary>
    ''' Set Irradiance(W/cm2)
    ''' </summary>
    ''' <remarks></remarks>
    SIR
    STM '設置曝光時間n/10秒, 2<=n<=9999
    GUS '獲取設備狀況
End Enum
Public Enum responseList As Integer
    ''' <summary>
    ''' Command Response
    ''' Response bitwise HF equals 0 , is treated as failed
    ''' </summary>
    ''' <remarks></remarks>
    READY = &H1 '準備就緒
    RECEIVED = &H2
    DONE = &H4

    SUCCESS = &HF0
    EXECUTING = &H10000

End Enum
Public Enum statusList As Integer
    ''''GUS Response''''
    ALARM_BIT = &H1
    LAMP_BIT = &H2
    SHUTTER_BIT = &H4
    HOME_BIT = &H8
    LAMP_READY_BIT = &H10
    LOCK_BIT = &H20
    CALIBRATION_BIT = &H40
    EXPOSURE_FAULT_BIT = &H80
End Enum

Public Enum uvCuringMode As Integer
    ABSOLUTE = 0
    RELATIVE
End Enum


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class uvCure
    Inherits systemControlPrototype
    Implements IProcedure

    Private enc As New System.Text.ASCIIEncoding()

    Dim WithEvents omni As SerialPort = New System.IO.Ports.SerialPort("COM1",
                                                                       19200,
                                                                       Parity.None,
                                                                       8,
                                                                       StopBits.One)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Mode As uvCuringMode
        Get
            Return __arguments(2)
        End Get
    End Property


    Dim __arguments() As Integer = {30, 50}
    Protected ReadOnly Property IrisLevel As Integer
        Get
            Return __arguments(0)
        End Get
    End Property
    Protected ReadOnly Property ExposureTime As Integer
        Get
            Return __arguments(1)
        End Get
    End Property
    Protected ReadOnly Property Irradiance As Single
        Get
            Return __arguments(0)
        End Get
    End Property


    ReadOnly Property Status(item As statusList) As Boolean
        Get
            Return (__statusCode And item) > 0
        End Get
    End Property

    ReadOnly Property Response As responseList
        Get
            Return __response
        End Get
    End Property

    Friend __rawStringData As String = Nothing
    Friend __response As responseList = responseList.EXECUTING
    Friend __statusCode As Byte = 0

    ''' <summary>
    ''' Generate command string pattern and sendout
    ''' </summary>
    ''' <param name="command"></param>
    ''' <remarks></remarks>
    Sub sendRequest(command As commandList)

        Dim __commandString As String = Nothing

        Select Case command
            Case commandList.SIL
                'Iris Level
                __commandString = crc8(command.ToString & IrisLevel.ToString) + Chr(13)
            Case commandList.STM
                'Exposure Time
                __commandString = crc8(command.ToString & ExposureTime.ToString) + Chr(13)
            Case commandList.SIR
                'Irradiance
                __commandString = crc8(command.ToString & Irradiance.ToString) + Chr(13)
            Case Else

        End Select

        omni.Write(__commandString) 'send out
        __response = responseList.EXECUTING ' reset

    End Sub
    Dim __timer As singleTimer = New singleTimer() With {.TimerGoal = New TimeSpan(0, 0, 5),
                                                         .IsEnabled = True}

    Sub New()
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
    End Sub
#Region "GUS List"
    '             0     1
    '0 報警       OFF   ON   
    '1 燈泡       OFF   ON
    '2 檔板       打開  閉合
    '3 原點       通過  故障
    '4 燈泡準備好  NO   YES
    '5 鎖定       OFF   ON
    '6 是否校準    NO   YES
    '7 曝光故障    NO   YES
#End Region

    Dim __igniteCommandList As List(Of commandList) = New List(Of commandList) From {commandList.CONN,
                                                                                     commandList.LOC,
                                                                                     commandList.TON,
                                                                                     commandList.CLS}
    Dim __absoluteExecuteCommandList As List(Of commandList) = New List(Of commandList) From {commandList.SIR,
                                                                                              commandList.STM,
                                                                                              commandList.RUN}
    Dim __relativeExecuteCommandList As List(Of commandList) = New List(Of commandList) From {commandList.SIL,
                                                                                              commandList.STM,
                                                                                              commandList.RUN}


    Dim __commandIterator As List(Of commandList).Enumerator = Nothing
    Dim alarmPackOmniCureFailed As alarmContextBase = New alarmContextBase With {.Sender = Me,
                                                                                 .PossibleResponse = alarmContextBase.responseWays.RETRY Or
                                                                                 alarmContextBase.responseWays.ABORT}

    Public Function stateIgnite()
        Select Case systemSubState
            Case 0 '開啟serialport連線

                __commandIterator = __igniteCommandList.GetEnumerator

                systemSubState = 10

            Case 10
                If __commandIterator.MoveNext Then
                    sendRequest(__commandIterator.Current)
                    'setup timeout time
                    With __timer
                        .TimerGoal = New TimeSpan(0, 0, 5)
                        .IsEnabled = True
                    End With
                    systemSubState = 20
                Else
                    '------------------------------
                    'all command had been send-out
                    '------------------------------
                    systemSubState = 100
                End If
            Case 20
                If (Response And responseList.SUCCESS) > 0 Then
                    systemSubState = 10 'do next command
                ElseIf Response <> responseList.EXECUTING Then
                    'returned failed message
                    'send-alarm 
                    alarmPackOmniCureFailed.AdditionalInfo = __rawStringData
                    CentralAlarmObject.raisingAlarm(alarmPackOmniCureFailed)
                ElseIf __timer.IsTimerTicked Then
                    'time-out
                    sendMessageTimed(statusEnum.GENERIC_MESSAGE, "OmniCure Communication Time-out")
                End If
                '----------------------
                '   Status Check
                '----------------------
            Case 100
                sendRequest(commandList.GUS)
                systemSubState += 10
            Case 110
                If Response <> responseList.EXECUTING Then

                    If Status(statusList.LAMP_READY_BIT) Then

                        systemMainState = systemStatesEnum.EXECUTE
                    Else
                        'need query again
                        '燈泡加熱中
                        systemSubState = 100
                    End If
                Else
                    '-----------------------------
                    '   Responsing
                    '-----------------------------
                End If

        End Select
        Return 0
    End Function
    Public Function stateExecute()
        Select Case systemSubState
            Case 0 '運行定時曝光
                If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then

                    Result = IProcedure.procedureResultEnums.FAILED ' default status

                    Select Case Mode
                        Case uvCuringMode.ABSOLUTE
                            __commandIterator = __absoluteExecuteCommandList.GetEnumerator
                        Case uvCuringMode.RELATIVE
                            __commandIterator = __relativeExecuteCommandList.GetEnumerator
                    End Select


                    systemSubState = 10
                Else
                    '----------
                    'continuing
                    '----------
                End If
                '-----------------------------------------
                '   Setup
                '-----------------------------------------
            Case 10
                If __commandIterator.MoveNext Then
                    sendRequest(__commandIterator.Current)
                    'setup timeout time
                    With __timer
                        .TimerGoal = New TimeSpan(0, 0, 5)
                        .IsEnabled = True
                    End With
                    systemSubState = 20
                Else
                    '------------------------------
                    'all command had been send-out
                    '------------------------------
                    With __timer
                        .TimerGoal = New TimeSpan(0, 0, ExposureTime)
                        .IsEnabled = True
                    End With
                    systemSubState = 100

                End If
            Case 20
                If (Response And responseList.SUCCESS) > 0 Then
                    systemSubState = 10
                ElseIf Response <> responseList.EXECUTING Then
                    'returned failed message
                    'send-alarm
                    alarmPackOmniCureFailed.AdditionalInfo = __rawStringData
                    CentralAlarmObject.raisingAlarm(alarmPackOmniCureFailed)
                ElseIf __timer.IsTimerTicked Then
                    'time-out
                    sendMessageTimed(statusEnum.GENERIC_MESSAGE, "OmniCure Communication Time-out")
                End If
                '----------------------
                '   Status Check
                '----------------------
            Case 100
                'wait until exposuring time reached
                If __timer.IsTimerTicked Then
                    sendRequest(commandList.GUS)
                    systemSubState += 10
                Else
                    '---------------------
                    '   Counting-Down
                    '---------------------
                End If
            Case 110
                If Response <> responseList.EXECUTING Then

                    If Status(statusList.LAMP_READY_BIT) Then
                        Result = IProcedure.procedureResultEnums.SUCCESS
                        systemSubState = 500
                    Else
                        '------------------------
                        '   Lamp is working
                        '------------------------
                        systemSubState = 100
                    End If
                Else
                    '-----------------------------
                    '   Responsing
                    '-----------------------------
                End If
                '---------------------
                '   End of Procedure
                '---------------------
            Case 500
                IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = 0
        End Select
        Return 0
    End Function

#Region "CRC8 transfer"
    Private Function crc8(str As String) As String
        Dim bData() As Byte = enc.GetBytes(str)
        Dim crc_Temp As Byte
        Dim crc_Index As Byte
        Dim i As Long
        Dim toHex As String
        crc_Temp = 0
        For i = 0 To bData.Length - 1
            crc_Index = crc_Temp Xor bData(i)
            crc_Temp = getCRC8(crc_Index)
        Next
        toHex = String.Format("{0:X}", Convert.ToInt32(crc_Temp))
        If toHex.Length = 1 Then
            Return str & "0" & toHex
        Else
            Return str & String.Format("{0:X}", Convert.ToInt32(crc_Temp))
        End If
    End Function
    Private Function getCRC8(Index As Byte) As Byte
        getCRC8 = Choose(Index + 1,
        &H0, &H5E, &HBC, &HE2, &H61, &H3F, &HDD, &H83, &HC2, &H9C, &H7E, &H20, &HA3, &HFD, &H1F, &H41,
        &H9D, &HC3, &H21, &H7F, &HFC, &HA2, &H40, &H1E, &H5F, &H1, &HE3, &HBD, &H3E, &H60, &H82, &HDC,
        &H23, &H7D, &H9F, &HC1, &H42, &H1C, &HFE, &HA0, &HE1, &HBF, &H5D, &H3, &H80, &HDE, &H3C, &H62,
        &HBE, &HE0, &H2, &H5C, &HDF, &H81, &H63, &H3D, &H7C, &H22, &HC0, &H9E, &H1D, &H43, &HA1, &HFF,
        &H46, &H18, &HFA, &HA4, &H27, &H79, &H9B, &HC5, &H84, &HDA, &H38, &H66, &HE5, &HBB, &H59, &H7,
        &HDB, &H85, &H67, &H39, &HBA, &HE4, &H6, &H58, &H19, &H47, &HA5, &HFB, &H78, &H26, &HC4, &H9A,
        &H65, &H3B, &HD9, &H87, &H4, &H5A, &HB8, &HE6, &HA7, &HF9, &H1B, &H45, &HC6, &H98, &H7A, &H24,
        &HF8, &HA6, &H44, &H1A, &H99, &HC7, &H25, &H7B, &H3A, &H64, &H86, &HD8, &H5B, &H5, &HE7, &HB9,
        &H8C, &HD2, &H30, &H6E, &HED, &HB3, &H51, &HF, &H4E, &H10, &HF2, &HAC, &H2F, &H71, &H93, &HCD,
        &H11, &H4F, &HAD, &HF3, &H70, &H2E, &HCC, &H92, &HD3, &H8D, &H6F, &H31, &HB2, &HEC, &HE, &H50,
        &HAF, &HF1, &H13, &H4D, &HCE, &H90, &H72, &H2C, &H6D, &H33, &HD1, &H8F, &HC, &H52, &HB0, &HEE,
        &H32, &H6C, &H8E, &HD0, &H53, &HD, &HEF, &HB1, &HF0, &HAE, &H4C, &H12, &H91, &HCF, &H2D, &H73,
        &HCA, &H94, &H76, &H28, &HAB, &HF5, &H17, &H49, &H8, &H56, &HB4, &HEA, &H69, &H37, &HD5, &H8B,
        &H57, &H9, &HEB, &HB5, &H36, &H68, &H8A, &HD4, &H95, &HCB, &H29, &H77, &HF4, &HAA, &H48, &H16,
        &HE9, &HB7, &H55, &HB, &H88, &HD6, &H34, &H6A, &H2B, &H75, &H97, &HC9, &H4A, &H14, &HF6, &HA8,
        &H74, &H2A, &HC8, &H96, &H15, &H4B, &HA9, &HF7, &HB6, &HE8, &HA, &H54, &HD7, &H89, &H6B, &H35)
    End Function
#End Region
    ''' <summary>
    ''' Raised by back-ground thread
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub dataReceived(sender As SerialPort, e As System.IO.Ports.SerialDataReceivedEventArgs) Handles omni.DataReceived
        __rawStringData = sender.ReadTo(Chr(13))
        Dim __tempResponse As responseList = responseList.EXECUTING
        [Enum].TryParse(Of responseList)(Strings.Left(__rawStringData, Len(__rawStringData) - 2), __tempResponse)
        __statusCode = Integer.TryParse(__rawStringData, __statusCode)

        __response = __tempResponse 'assign at once
    End Sub
    ''' <summary>
    ''' {SIL (percentage),STM (1/10 second) }
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Arguments As Object Implements IProcedure.Arguments
        Get
            Return True
        End Get
        Set(value As Object)
            __arguments = value
        End Set
    End Property
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property IsProcedureAbort As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort
    Public Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result


End Class
