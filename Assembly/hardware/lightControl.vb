Imports Automation
Imports System.IO.Ports
Imports System.Text
Imports Automation.Components.Services

Public Class lightChannelData
    ReadOnly Property Channel As Integer
        Get
            Return __channel
        End Get
    End Property
    Property CurrentValue As Integer
        Get
            Return __currentValue
        End Get
        Set(value As Integer)
            __currentValue = value
        End Set
    End Property
    ReadOnly Property IsChanged As Boolean
        Get
            Return lastValue <> __currentValue
        End Get
    End Property


    Protected __channel As Integer = 0
    Friend __currentValue As Integer = 0
    Friend lastValue As Integer = 0

    Sub New(__channel As Integer)
        Me.__channel = __channel
    End Sub

    Public Overrides Function ToString() As String
        Dim __outputString As String = __channel.ToString & "," & __currentValue.ToString & vbCrLf
        'once outputed  , record last value
        lastValue = __currentValue
        Return __outputString
    End Function

End Class

Public Class lightControl
    Inherits systemControlPrototype

    ReadOnly Property IsLinked As Boolean
        Get
            Return __serialPort.IsOpen
        End Get
    End Property
    Property IsCommunicating As Boolean
        Get
            Return __isCommunicating
        End Get
        Set(value As Boolean)
            __isCommunicating = True
        End Set
    End Property
    WriteOnly Property Intensity(channel As Integer) As Integer
        Set(value As Integer)
            Dim __data As lightChannelData = __settingCache.Find(Function(____data As lightChannelData) ____data.Channel = channel)
            If __data IsNot Nothing Then
                __data.CurrentValue = value
            End If
        End Set
    End Property
    ReadOnly Property CurrentSettingValue As List(Of lightChannelData)
        Get
            Return __settingCache
        End Get
    End Property

#Region "Device declare"
    Public WithEvents __serialPort As SerialPort = New SerialPort With {.PortName = "COM2",
                                                                      .BaudRate = 9600,
                                                                      .Parity = Parity.None,
                                                                      .StopBits = StopBits.One,
                                                                      .DataBits = 8,
                                                                      .Encoding = Encoding.UTF8}
#End Region

    ''' <summary>
    ''' {channel,intesity}
    ''' </summary>
    ''' <remarks></remarks>
    Dim __settingCache As List(Of lightChannelData) = New List(Of lightChannelData)
    Dim __cacheIterator As List(Of lightChannelData).Enumerator = Nothing

    Dim __isCommunicating As Boolean = False

    Private __timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}

    Private strSend As String = ""
    Private strReceived As String = ""

    Dim __index As Integer = 0

    Protected Sub New()
        Try
            __serialPort.Open()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        For index = 0 To 3
            __settingCache.Add(New lightChannelData(index + 1))
        Next


        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE

    End Sub

    Protected Function stateExecute() As Integer
        Select Case systemSubState

            Case 0 ' check serial port opened
                If __isCommunicating And
                    IsLinked Then

                    __cacheIterator = __settingCache.GetEnumerator
                    systemSubState = 10

                Else
                    __isCommunicating = False
                End If
            Case 10
                If __cacheIterator.MoveNext Then


                    If __cacheIterator.Current.IsChanged Then

                        strReceived = ""

                        Try
                            __serialPort.Write(__cacheIterator.Current.ToString)
                        Catch ex As Exception
                            sendMessage(ex.Message)
                        End Try

                        __timer.IsEnabled = True
                        systemSubState = 20
                    Else
                        '-------------------------------------
                        '   Not Changed , Move to next one
                        '-------------------------------------
                    End If
                Else
                    'all done ,
                    systemSubState = 500
                End If

            Case 20 ' check response
                If strReceived <> "" Then
                    If strReceived = __cacheIterator.Current.ToString Then
                        sendMessage(statusEnum.GENERIC_MESSAGE, __serialPort.PortName + " light setting complete.")
                    ElseIf strReceived = "E" + vbCrLf Then
                        sendMessage(statusEnum.GENERIC_MESSAGE, __serialPort.PortName + " Error strSend:" + strSend)
                    Else
                        sendMessage(statusEnum.GENERIC_MESSAGE, __serialPort.PortName + " Error Message:" + strReceived)
                    End If
                    systemSubState = 10
                ElseIf __timer.IsTimerTicked Then
                    ' overtime
                    sendMessage(statusEnum.GENERIC_MESSAGE, __serialPort.PortName + " time out.")
                    systemSubState = 10
                Else
                    '-----------------------
                    '   Wait Response
                    '-----------------------
                End If
            Case 500
                'reset flag
                __isCommunicating = False
                systemSubState = 0
        End Select
        Return 0
    End Function

    Private Sub dataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles __serialPort.DataReceived
        strReceived = __serialPort.ReadExisting()
    End Sub

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As lightControl
        Get
            If __instance Is Nothing Then
                __instance = New lightControl
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As lightControl = Nothing
#End Region


End Class
