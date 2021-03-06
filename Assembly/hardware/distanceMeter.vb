﻿Imports Automation
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Linq


Public Class distanceMeter
    Inherits systemControlPrototype
    Implements IDrivable

    Enum dmCommands As Integer
        NONE
        CONNECT
        ''' <summary>
        ''' Zero-reset
        ''' </summary>
        ''' <remarks></remarks>
        ZR
        ZC

        ''' <summary>
        ''' Acquire the task measures
        ''' </summary>
        ''' <remarks></remarks>
        MS
        ''' <summary>
        ''' Turn On/Off LD
        ''' </summary>
        ''' <remarks></remarks>
        LD
        ''' <summary>
        ''' Perform the trigger action
        ''' </summary>
        ''' <remarks></remarks>
        TM
        ''' <summary>
        ''' Perform the reset action
        ''' </summary>
        ''' <remarks></remarks>
        RT
        ''' <summary>
        ''' Perform the restart
        ''' </summary>
        ''' <remarks></remarks>
        RS
    End Enum
    Enum dmResponse As Integer
        ''' <summary>
        ''' OK
        ''' </summary>
        ''' <remarks></remarks>
        OK
        ''' <summary>
        ''' Error
        ''' </summary>
        ''' <remarks></remarks>
        ER
    End Enum
    Enum dmOnOff As Integer
        __ON = 1
        __OFF = 0
    End Enum



    Enum dmSubstates As Integer
        SEND_COMMAND = 0
        RECEIVE_RESPOND = 1
    End Enum


    ''' <summary>
    ''' In mm
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property MeasureValue As Double
        Get
            '-30719923 = -30.719923 mm
            Return __measureValue * 0.000001
        End Get
    End Property
    Protected __measureValue As Integer = 0
    ReadOnly Property Response As dmResponse
        Get
            Return __response
        End Get
    End Property
    ReadOnly Property IsMeasureValueAvailable As Boolean
        Get
            Return __isMeasureValueAvailable
        End Get
    End Property
    Dim __isMeasureValueAvailable As Boolean = False

    Dim __interface As compSocket = New compSocket With {.AddressConfigure = "192.168.1.1",
                                                         .IsAutoMessageOut = True,
                                                         .ResetMode = compSocket.resetModeEnum.AS_CLIENT,
                                                         .Delimiter = vbCr,
                                                         .IsReceivingWithDelimiter = True,
                                                         .IsEnabled = True}

    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            Return __commandDriveState
        End Get
    End Property

    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get


            If __commandEndStatus = IDrivable.endStatus.EXECUTION_END And
            (__commandDriveState = IDrivable.drivenState.WAIT_RECALL And
             __commandInExecute <> dmCommands.CONNECT And
             __response = dmResponse.ER) Then
                'negtive response
                __commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL
            End If

            If __commandDriveState = IDrivable.drivenState.WAIT_RECALL Then
                __commandInExecute = dmCommands.NONE
                __commandDriveState = IDrivable.drivenState.LISTENING
            End If

            '-------------
            '   Directly response
            '-------------
            Return __commandEndStatus

        End Get
    End Property

    Public ReadOnly Property CommandInExecute As Object Implements IDrivable.CommandInExecute
        Get
            Return __commandInExecute
        End Get
    End Property

    ''' <summary>
    ''' Off:0
    ''' On:1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property TestArgs As List(Of Integer) = New List(Of Integer) From {1}

#Region "gui reflection"
    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        Return __commandDictionary.Keys
    End Function
    Public Overrides Function raisingGUI() As Control
        Dim uc As userControlDrivable = New userControlDrivable
        With uc
            .Component = Me
            .PropertyView = New userControlPropertyView With {.Drive = Me}
        End With
        Return uc
    End Function
#End Region

    Public Property TimeoutLimit As TimeSpan Implements IDrivable.TimeoutLimit

    ''' <summary>
    ''' Event driven mechanism
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event CommandExecuted(ByVal sender As Object, ByVal e As EventArgs)

    Protected __commandInExecute As dmCommands = dmCommands.NONE
    Protected __commandDriveState As IDrivable.drivenState = IDrivable.drivenState.LISTENING
    Protected __commandSubState As dmSubstates = dmSubstates.SEND_COMMAND ' shared by all command functions , command function should rewind this in the end of execution
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTING
    Protected __commandDictionary As Dictionary(Of dmCommands, IDrivable.commandFunctionPrototype) = New Dictionary(Of dmCommands, IDrivable.commandFunctionPrototype)

    Protected __response As dmResponse = dmResponse.ER
    Protected __args As Array = Nothing

    Protected Overrides Function process() As Integer
        '---------------------------
        '   Driven state-mahcine
        '---------------------------
        drivesRunningInvoke()

        If __commandInExecute.Equals(dmCommands.NONE) Then
            Return 0
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING,
                IDrivable.drivenState.WAIT_RECALL
                Return 0
            Case IDrivable.drivenState.EXECUTING
                '--------------------------------
                '   Executing
                '--------------------------------
                __commandEndStatus = __commandDictionary(__commandInExecute)()
                If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
                    __commandSubState = dmSubstates.SEND_COMMAND       'rewind
                    __commandDriveState = IDrivable.drivenState.WAIT_RECALL
                    RaiseEvent CommandExecuted(Me, EventArgs.Empty)

                    If (__commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL) Then
                        '----------------------------
                        'error occured
                        'raise alarm and rewind state
                        '----------------------------
                        'bug patch , to prevent missed end failed condition , Hsien  ,2016.05.16
                        Dim alarmPackage = New alarmContextBase With {.Sender = Me}
                        With alarmPackage
                            .CallbackResponse(alarmContextBase.responseWays.ABORT) = alarmContextBase.abortMethod   'Hsien , setup before raise alarm  , 2015.10.12
                            .AdditionalInfo = String.Format("Command{0},Args:{1},Result:{2},Output:{3}",
                                                            __commandInExecute,
                                                            0,
                                                            0,
                                                            0)
                        End With
                        CentralAlarmObject.raisingAlarm(alarmPackage)       'raising alarm
                    End If
                Else
                    '-------------------------
                    '   Executing
                    '-------------------------
                End If

        End Select

        Return 0
    End Function
    Public Function drive(command As [Enum], Optional __arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive

        'direct break and response dummy result
        If (Not IsEnabled) Then
            Return __commandEndStatus
        End If

        'Hsien , should cast into right type
        Dim __commandInDmCommand As dmCommands = [Enum].ToObject(GetType(dmCommands), command)
        If __commandInDmCommand.Equals(Nothing) Then
            'error , cannot casting , command reject
            Return IDrivable.endStatus.EXECUTION_END_FAIL
        End If

        'override empty arguments
        If __arg Is Nothing Then
            __arg = TestArgs.ToArray
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING
                '------------------------------
                '   Able to accept command
                '-------------------------------
                Me.__args = __arg
                Me.__isMeasureValueAvailable = False ' reset
                __commandInExecute = __commandInDmCommand
                __commandDriveState = IDrivable.drivenState.EXECUTING
            Case IDrivable.drivenState.WAIT_RECALL
                '------------------------
                '   Last command had beed executed , this cycle used to rewind
                '------------------------
                __commandDriveState = IDrivable.drivenState.LISTENING
                __commandInExecute = dmCommands.NONE
                Return __commandEndStatus
            Case IDrivable.drivenState.EXECUTING
                '--------------------
                '   Do nothing
                '--------------------
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

#Region "command functions"

    Function connectCommand() As IDrivable.endStatus
        Return __interface.drive(compSocket.socketCommand.CONNECT)
    End Function

    Function regularCommand() As IDrivable.endStatus

        Select Case __commandSubState
            Case dmSubstates.SEND_COMMAND
                If __interface.drive(compSocket.socketCommand.SEND_STRING, command2String(__commandInExecute, __args)) =
                     IDrivable.endStatus.EXECUTION_END Then
                    __commandSubState = dmSubstates.RECEIVE_RESPOND
                Else
                    '----------------------
                    '   Receiving
                    '----------------------
                End If
            Case dmSubstates.RECEIVE_RESPOND
                If __interface.drive(compSocket.socketCommand.RECEIVE_STRING) =
                     IDrivable.endStatus.EXECUTION_END Then

                    string2Response(__interface.OperationString)

                    Return IDrivable.endStatus.EXECUTION_END
                Else
                    '-----------------------
                    '   Receiving
                    '-----------------------
                End If
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

#End Region


#Region "command/string interpreter"

    Function command2String(__command As dmCommands, args As Integer()) As String

        Dim sb As StringBuilder = New StringBuilder

        'command<space>
        sb.AppendFormat("{0}", __command.ToString)

        Select Case __command
            Case dmCommands.CONNECT
                Return ""
            Case dmCommands.MS,
                 dmCommands.ZR,
                  dmCommands.ZC
                sb.AppendFormat(" {0}", args(0))
            Case dmCommands.TM,
                 dmCommands.LD,
                 dmCommands.RT
                'OFF:0
                'ON:1
                sb.AppendFormat(" {0} 0", args(0))
            Case dmCommands.RS
                'do nothing
        End Select

        sb.Append(vbCr)

        Return sb.ToString
    End Function
    Sub string2Response(input As String)
        Dim __match As Match = Regex.Match(input,
                                           "(-?[0-9]+|ER|OK)")

        If __match.Success Then

            If Not Integer.TryParse(__match.Captures(0).Value, __measureValue) Then
                Me.__response = [Enum].Parse(GetType(dmResponse), __match.Captures(0).Value)
            Else
                '----------------------------
                ''  Measure Value Passed
                '----------------------------
                __isMeasureValueAvailable = True
            End If
        Else

            '---------------------
            '   Something wrong
            '---------------------
        End If

    End Sub


#End Region

    Sub New(Optional address As String = "192.168.1.1:5000")
        Me.__interface.AddressConfigure = address
        For Each item As dmCommands In utilities.enumObjectsListing(GetType(dmCommands))
            Me.__commandDictionary(item) = AddressOf regularCommand
        Next
        Me.__commandDictionary(dmCommands.CONNECT) = AddressOf connectCommand

    End Sub

End Class
