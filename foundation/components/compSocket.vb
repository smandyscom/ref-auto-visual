﻿Imports Automation.Components
Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions

Public Class alarmContextSocket : Inherits alarmContextBase

    Public Enum ErrorOperations
        SENDING_ERROR
        RECEIVING_ERROR
        CONNECTING_ERROR
    End Enum

    Property Operation As ErrorOperations = ErrorOperations.CONNECTING_ERROR
    Property ExceptionBlock As SocketException

    Public Overrides Function ToString() As String
        Return String.Format("{0}({1}){2}",
                             Me.Operation,
                             ExceptionBlock.Message,
                             ExceptionBlock.SocketErrorCode)
    End Function

End Class

Public Interface IDrivable
    Enum endStatus
        '-------------------------
        'mutually exclusive states
        '-------------------------
        ' the internal status 
        ' also the return value to caller of drive()
        'NONE
        EXECUTING = &H0                 ' command on executing , result is not determined
        EXECUTION_END = &H1000           ' 0x1000
        EXECUTION_END_FAIL = &H1100       ' 0x1100 , with sub-code
    End Enum
    Enum drivenState
        '-------------
        '   The main drive state-machine states
        '-------------
        LISTENING
        EXECUTING
        WAIT_RECALL
    End Enum


    Function drive(command As [Enum], Optional __arg As Object = Nothing) As endStatus
    ReadOnly Property CommandEndStatus As endStatus
    ReadOnly Property CommandDrivenState As drivenState
    ReadOnly Property CommandInExecute As Object
    Property TimeoutLimit As TimeSpan

    Delegate Function commandFunctionPrototype() As endStatus   'as function prototype


    'Hsien , 2015.06.03
    Function getCommands() As ICollection
End Interface

Public Class compSocket
    Inherits driveBase
    Implements IDrivable
    Implements IDisposable


    '------------------------
    '   As Command State Machine
    '------------------------
    Public Enum socketCommand
        CONNECT
        'SERVER_ACCEPT
        SEND_STRING
        RECEIVE_STRING
        '--------------
        STOP_OPERATION
    End Enum

    Public Enum resetModeEnum
        AS_SERVER
        AS_CLIENT
    End Enum

#Region "common interface"
    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get
            If (__commandDrivenState = IDrivable.drivenState.WAIT_RECALL) Then
                __commandDrivenState = IDrivable.drivenState.LISTENING
            End If
            Return __commandEndStatus
        End Get
    End Property
    Public ReadOnly Property CommandInExecute As Object Implements IDrivable.CommandInExecute
        Get
            Return __commandInExecute
        End Get
    End Property
    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            Return __commandDrivenState
        End Get
    End Property
    Public Property TimeoutLimit As TimeSpan Implements IDrivable.TimeoutLimit
        Get
            Return timeoutTimer.TimerGoal
        End Get
        Set(value As TimeSpan)
            timeoutTimer.TimerGoal = value
        End Set
    End Property
#End Region
    WriteOnly Property EnableTimeout
        Set(value)
            __isCheckTimeout = value
        End Set
    End Property
    ReadOnly Property IsConnected As Boolean
        Get
            Return __isSocketConnected
        End Get
    End Property
    ''' <summary>
    ''' Used to detect whether connection break
    ''' </summary>
    ''' <param name="microseconds"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsConnected(microseconds As Integer) As Boolean
        Get
            Return (Not (__commandDrivenState = IDrivable.drivenState.EXECUTING And __commandInExecute = socketCommand.CONNECT)) AndAlso
                Not ((communicationSocket.Poll(microseconds, SelectMode.SelectRead) And (communicationSocket.Available = 0)) Or
                     (Not communicationSocket.Connected))
        End Get
    End Property
    ReadOnly Property OperationString As String
        Get
            Return __operationString
        End Get
    End Property
    Property EncodingMethod As Encoding = Encoding.Default

    '--------------------------------
    'the interface in form of IP:Port
    '--------------------------------
    Property AddressConfigure As String
        Get

            Return String.Format("{0}:{1}",
                                 __endPoint.Address.ToString,
                                 __endPoint.Port)
        End Get
        Set(value As String)
            Dim __match As Match = Regex.Match(value, "^((?:[0-9]{1,3}\.){3}[0-9]{1,3}):([0-9]+)$")

            If (__match.Success) Then
                __endPoint.Address = IPAddress.Parse(__match.Groups(1).Value)
                Integer.TryParse(__match.Groups(2).Value, __endPoint.Port)
            End If


        End Set
    End Property


    Property ResetMode As resetModeEnum = resetModeEnum.AS_CLIENT       'decide the behavior when reconnecting
    

    Dim __endPoint As IPEndPoint = New IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001)

    'Hsien , 2015.09.15, offered the possibility the control layer buffer
    WriteOnly Property ReceiveBufferSize As Integer
        Set(value As Integer)
            communicationSocket.ReceiveBufferSize = value
        End Set
    End Property
    WriteOnly Property SendBufferSize As Integer
        Set(value As Integer)
            communicationSocket.SendBufferSize = value
        End Set
    End Property
    '------------------------
    '   Delimiter control
    '-------------------------
    Property IsReceivingWithDelimiter As Boolean = False    'Hsien , 2015.04.22 , control only receiving with delimiter
    Property Delimiter As String = vbCrLf

    Property IsAutoMessageOut As Boolean = False    'Hsien , 2015.09.08 , controlled whether auto push message into central messenger

    Protected __commandInExecute As socketCommand = socketCommand.CONNECT
    Protected __commandDrivenState As IDrivable.drivenState = IDrivable.drivenState.LISTENING
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTION_END
    Protected __isSocketConnected As Boolean = False
    Protected __isCheckTimeout As Boolean
    Protected commandDictionary As Dictionary(Of [Enum], IDrivable.commandFunctionPrototype) = New Dictionary(Of [Enum], IDrivable.commandFunctionPrototype)

    '-------------------
    ' control interface for host
    '-------------------
    Public Function drive(command As socketCommand, __operationString As String) As IDrivable.endStatus
        Me.__operationString = __operationString
        Return DirectCast(Me, IDrivable).drive(command) 'Hsien , 2015.06.04 , explict type casting
    End Function
    Public Function drive(command As [Enum], Optional arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive

        Select Case __commandDrivenState
            Case IDrivable.drivenState.LISTENING
                '------------------------------
                '   Able to accept command
                '-------------------------------
                __commandInExecute = command
                timeoutTimer.IsEnabled = __isCheckTimeout
                __commandEndStatus = IDrivable.endStatus.EXECUTING
                __commandDrivenState = IDrivable.drivenState.EXECUTING
            Case IDrivable.drivenState.WAIT_RECALL
                '------------------------
                '   Last command had beed executed , rewind
                '------------------------
                __commandDrivenState = IDrivable.drivenState.LISTENING
                Return __commandEndStatus
            Case IDrivable.drivenState.EXECUTING
                '---------------------------
                '   Forced to stop operation
                '   Hsien , 2015.03.09
                '---------------------------
                If (command.Equals(socketCommand.STOP_OPERATION)) Then
                    communicationSocket.Disconnect(True)
                    __commandDrivenState = IDrivable.drivenState.LISTENING
                    operationSubState = operationStates.BEGIN_OPERATION
                    timeoutTimer.IsEnabled = False
                End If
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

    Protected listeningSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)       'used in server mode
    Protected communicationSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)   'used for data transmit

    Protected alarmPackSocketError As alarmContextSocket = New alarmContextSocket()
    Protected alarmPackTimeoutError As alarmContextSocket = New alarmContextSocket()    'Hsien , 2015.03.02

    '-----------------------------
    '   Asynchron operation report
    '-----------------------------
    Protected socketResult As IAsyncResult
    Protected result As SocketError = SocketError.Success
    Protected exceptionBlock As Exception = New SocketException      'used to store exception when error occured

    Protected operationLocked As Boolean = True 'hsien , used as interlock for worker thread and socket operation thread

    Protected __operationString As String
    Protected __operationBuffer As Byte() =
        New Byte(communicationSocket.ReceiveBufferSize) {}
    Protected timeoutTimer As singleTimer = New singleTimer() With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Protected sendedBytes As Integer

    Protected operationSubState As operationStates = operationStates.BEGIN_OPERATION
    Public Enum operationStates As Short
        '--------------------------------------
        '   Defined basic handshake status only
        '--------------------------------------
        BEGIN_OPERATION = 0
        CHECK_CONNECTION_ACTIVE = 50
        ON_OPERATION = 100
        RECONNECT = 300 'Hsien , used on retry mode , 2016.03.11
        RECONNECT_WAITING = 400
        ALARM_HANDLING
    End Enum

    Sub New()
        '--------------------
        '   Initializing common socket error handling procedure
        '--------------------
        Me.alarmPackSocketError = New alarmContextSocket()
        With alarmPackSocketError
            .Sender = Me

            'retry , reconnect
            'ignore , give up connect , and record connection states
            .PossibleResponse = alarmContextBase.responseWays.RETRY Or
                 alarmContextBase.responseWays.IGNORE
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                         operationSubState = operationStates.RECONNECT
                                                                         Return True
                                                                     End Function
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                          __isSocketConnected = False
                                                                          '-----------------------------
                                                                          ' Reset driven state machine with end_fail
                                                                          '-----------------------------
                                                                          timeoutTimer.IsEnabled = False
                                                                          __commandDrivenState = IDrivable.drivenState.WAIT_RECALL
                                                                          __commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL
                                                                          operationSubState = operationStates.BEGIN_OPERATION

                                                                          Return True
                                                                      End Function
        End With
        With alarmPackTimeoutError
            .Sender = Me
            .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                                                                         timeoutTimer.IsEnabled = True  'restart the timer
                                                                         Return True
                                                                     End Function
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                                                                          '------------------------------
                                                                          ' Stop the asynchron operation
                                                                          '------------------------------
                                                                          communicationSocket.Shutdown(SocketShutdown.Both)
                                                                          '-----------------------------
                                                                          ' Reset driven state machine with end_fail
                                                                          '-----------------------------
                                                                          timeoutTimer.IsEnabled = False
                                                                          __commandDrivenState = IDrivable.drivenState.WAIT_RECALL
                                                                          __commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL
                                                                          operationSubState = operationStates.BEGIN_OPERATION
                                                                          Return True
                                                                      End Function
            .ExceptionBlock = New SocketException(SocketError.TimedOut) 'indicate the timeout error
        End With

        '-----------------------
        '   Initializing Command functions
        '-----------------------
        commandDictionary.Add(socketCommand.CONNECT, AddressOf commandConnect)
        commandDictionary.Add(socketCommand.RECEIVE_STRING, AddressOf commandReceive)
        commandDictionary.Add(socketCommand.SEND_STRING, AddressOf commandSend)
    End Sub

    Protected Overrides Function process() As Integer

        '---------------------------
        '   Driven state-mahcine
        '---------------------------
        Select Case __commandDrivenState
            Case IDrivable.drivenState.LISTENING,
                IDrivable.drivenState.WAIT_RECALL
                Return 0
            Case IDrivable.drivenState.EXECUTING

                __commandEndStatus = commandDictionary(__commandInExecute)()
                If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
                    timeoutTimer.IsEnabled = False
                    operationSubState = operationStates.BEGIN_OPERATION       'rewind
                    __commandDrivenState = IDrivable.drivenState.WAIT_RECALL

                    RaiseEvent CommandExecuted(Me, Nothing) 'push the report , Hsien , 2015.02.16

                    'Hsien , added control mechanism , 2015.09.08
                    If (IsAutoMessageOut) Then
                        CentralMessenger.MessageQueue.Enqueue(New messagePackage(Me, __commandInExecute, String.Format("Local:{0},Remote:{1},Data:{2}", communicationSocket.LocalEndPoint.ToString,
                                                                                                                        communicationSocket.RemoteEndPoint.ToString,
                                                                                                                       OperationString)))  'send message out
                    End If

                End If

        End Select

        Return 0
    End Function

#Region "command functions"
    Protected Function commandConnect() As IDrivable.endStatus
        'Protected Function commandConnect() As IDrivable.endStatus
        Select Case operationSubState
            Case operationStates.BEGIN_OPERATION,
                 operationStates.RECONNECT
                If (resetSocket()) Then
                    operationSubState = operationStates.ON_OPERATION
                End If
            Case operationStates.ON_OPERATION,
                operationStates.RECONNECT_WAITING
                '---------------------------------
                '   Reconnect waiting
                '---------------------------------
                If (socketResult.IsCompleted And
                    operationLocked = False) Then
                    '-------------------------
                    'asynchron operation done
                    'check operation result
                    '-------------------------
                    If (Me.result = SocketError.Success) Then
                        'done 

                        'If (ResetMode = resetModeEnum.AS_SERVER) Then
                        '    listeningSocket.Close() '   stop listening
                        'End If

                        __isSocketConnected = True
                        Return IDrivable.endStatus.EXECUTION_END
                    Else

                        sendAlarmSocketError(alarmContextSocket.ErrorOperations.CONNECTING_ERROR)
                    End If
                Else
                    '--------------
                    'todo , put a timer?
                    '--------------
                End If
            Case operationStates.ALARM_HANDLING
                '---------------------
                '   User response determine next state
                '---------------------
        End Select


        Return IDrivable.endStatus.EXECUTING
    End Function



    Dim receivedBytes As Integer = 0
    Protected Function commandReceive() As IDrivable.endStatus
        '-----------------------------
        '   All sub flow tested , Hsien 2015.1.11
        '   1. not connected but drive to receive
        '   2. drive to receive , but disconnected
        '   3. drive to receive with delimiter, successfully first receving , but fail when doing next receiving
        '-----------------------------
        Select Case operationSubState
            Case operationStates.BEGIN_OPERATION
                __operationString = ""      'reset the string
                '------------------------------
                '   Setup default execption , prevent thread overhead
                '   Hsien , 2015.03.05
                '------------------------------
                result = SocketError.SocketError
                exceptionBlock = New SocketException(SocketError.SocketError)
                operationSubState = 10
            Case 10
                '----------------------------------
                '   Execute asynchron operation
                '----------------------------------
                Try
                    __operationBuffer = New Byte(communicationSocket.ReceiveBufferSize) {}

                    operationLocked = True ' lock the operation

                    socketResult = communicationSocket.BeginReceive(__operationBuffer,
                                                       0,
                                                       __operationBuffer.Length,
                                                       SocketFlags.None,
                                                       Sub(ar As IAsyncResult)
                                                           Try
                                                               receivedBytes = communicationSocket.EndReceive(ar)
                                                               operationLocked = False
                                                           Catch __socketException As SocketException
                                                               '--------------------------
                                                               '   carry-out error reasons
                                                               '--------------------------
                                                               result = __socketException.ErrorCode
                                                               exceptionBlock = __socketException
                                                               operationLocked = False
                                                           Catch __ex As Exception
                                                               '----------------------------
                                                               '    Exceptions other than socket exception
                                                               '----------------------------
                                                               result = SocketError.SocketError 'meant to unknown error
                                                               exceptionBlock = __ex

                                                               operationLocked = False

                                                           End Try
                                                       End Sub,
                                                       Nothing)

                    operationSubState = operationStates.ON_OPERATION

                Catch __socketException As SocketException

                    timeoutTimer.IsEnabled = False      '
                    result = __socketException.SocketErrorCode
                    exceptionBlock = __socketException

                    sendAlarmSocketError(alarmContextSocket.ErrorOperations.RECEIVING_ERROR)

                    operationLocked = False

                End Try

            Case operationStates.ON_OPERATION
                If (socketResult.IsCompleted And
                    operationLocked = False) Then
                    '-------------------------
                    'asynchron operation done
                    'check operation result
                    '-------------------------
                    '---------------------------
                    '   Due to multi-thread issue , string handling should put after aychrnon opertion unlocked
                    '   Hsien  ,2015.03.05
                    '---------------------------
                    If (receivedBytes = 0) Then
                        '-----------------
                        '   Connection shutdown
                        '-----------------
                        exceptionBlock = New SocketException(SocketError.ConnectionAborted)
                        result = SocketError.ConnectionAborted
                    Else
                        '-----------------
                        '   Fetched data successfully
                        '-----------------
                        __operationString += EncodingMethod.GetString(__operationBuffer).Substring(0, receivedBytes)
                        '---------------------
                        '   Completed command fetched
                        '---------------------
                        result = SocketError.Success
                    End If

                    If (result = SocketError.Success) Then
                        '---------------------------------
                        'done , check if delimiter reached
                        '---------------------------------
                        If ((IsReceivingWithDelimiter And __operationString.EndsWith(Delimiter)) Or
                            (Not IsReceivingWithDelimiter)) Then
                            '--------------------------------------------------
                            '   Delimiter fetched/Without Delimiter request , operation done successfully
                            '--------------------------------------------------
                            'operationSubState = handshakeStates.END_HANDSHAKE
                            Return IDrivable.endStatus.EXECUTION_END
                        Else
                            '-----------
                            '   Need to do further receive
                            '-----------
                            operationSubState = 10
                        End If

                    Else
                        sendAlarmSocketError(alarmContextSocket.ErrorOperations.RECEIVING_ERROR)
                    End If
                ElseIf (timeoutTimer.IsEnabled AndAlso timeoutTimer.IsTimerTicked) Then
                    '------------------
                    '   Wait until operation done
                    '------------------
                    alarmPackTimeoutError.Operation = alarmContextSocket.ErrorOperations.RECEIVING_ERROR
                    CentralAlarmObject.raisingAlarm(alarmPackTimeoutError)
                    operationSubState = operationStates.ALARM_HANDLING
                    'If (timeoutTimer.IsTimerTicked And timeoutTimer.IsEnabled) Then
                    '    'todo , send a alarm pack , then shutdown socket
                    '    communicationSocket.Shutdown(SocketShutdown.Receive)
                    'End If
                End If

                '-------------------------------------------
                '   Error Occured , Reconnect
                '--------------------------------------------
            Case operationStates.RECONNECT
                If (resetSocket()) Then
                    operationSubState = operationStates.RECONNECT_WAITING
                End If
            Case operationStates.RECONNECT_WAITING
                '---------------------------------
                '   Reconnect waiting
                '---------------------------------
                If (socketResult.IsCompleted And
                    operationLocked = False) Then
                    '-------------------------
                    'asynchron operation done
                    'check operation result
                    '------------------------
                    If (result = SocketError.Success) Then
                        If (__isCheckTimeout = True) Then
                            timeoutTimer.IsEnabled = True
                        End If
                        operationSubState = 10
                    Else
                        sendAlarmSocketError(alarmContextSocket.ErrorOperations.CONNECTING_ERROR)
                    End If
                Else
                    '--------------
                    '   Todo , put a timer?
                    '--------------
                End If
            Case operationStates.ALARM_HANDLING
                '---------------------
                '   User response determine next state
                '---------------------
        End Select


        Return IDrivable.endStatus.EXECUTING
    End Function

    Protected Function commandSend() As IDrivable.endStatus
        '-------------------------------
        '   
        '-------------------------------
        Select Case operationSubState
            Case operationStates.BEGIN_OPERATION

                __operationBuffer =
                    EncodingMethod.GetBytes(__operationString)    'control if end with delimiter externally

                Try
                    operationLocked = True
                    socketResult = communicationSocket.BeginSend(__operationBuffer,
                                                    0,
                                                    __operationBuffer.Length,
                                                    SocketFlags.None,
                                                    Sub(ar As IAsyncResult)
                                                            Try
                                                                sendedBytes = communicationSocket.EndSend(ar)
                                                                result = SocketError.Success

                                                                operationLocked = False

                                                            Catch __socketException As SocketException

                                                                'catch the EndSend error
                                                                result = __socketException.SocketErrorCode
                                                                exceptionBlock = __socketException

                                                                operationLocked = False


                                                            Catch ex As Exception
                                                                '-------------------------------------------------------------------------------------
                                                                '  Should catch exception here, because this call used in the thread other than worker
                                                                '  Hsien , 2014.12.12
                                                                '-------------------------------------------------------------------------------------
                                                                result = SocketError.SocketError 'meant to unknown error
                                                                exceptionBlock = ex

                                                                operationLocked = False

                                                            End Try
                                                        End Sub,
                                                    Nothing)

                    operationSubState = operationStates.ON_OPERATION

                Catch __socketException As SocketException

                    timeoutTimer.IsEnabled = False
                    result = __socketException.SocketErrorCode
                    exceptionBlock = __socketException
                    sendAlarmSocketError(alarmContextSocket.ErrorOperations.SENDING_ERROR)

                    operationLocked = False

                End Try


            Case operationStates.ON_OPERATION
                '-------------------
                '   Wait sending complete
                '-------------------
                If (socketResult.IsCompleted And
                    operationLocked = False) Then
                    '-------------------------
                    'asynchron operation done
                    'check operation result
                    '-------------------------
                    If (result = SocketError.Success) Then

                        If (sendedBytes = __operationBuffer.Length) Then
                            Return IDrivable.endStatus.EXECUTION_END
                        Else
                            '-------------
                            '   Sending not complete , trim and send rest
                            '-------------
                            Dim resendBuffer As Byte() = New Byte(__operationBuffer.Length - sendedBytes) {}
                            __operationBuffer.CopyTo(resendBuffer, sendedBytes)   'zero-based index
                            __operationBuffer = resendBuffer
                            operationSubState = operationStates.BEGIN_OPERATION
                        End If

                    Else
                        '--------------------
                        'raising alarmPackage
                        '--------------------
                        sendAlarmSocketError(alarmContextSocket.ErrorOperations.SENDING_ERROR)
                    End If
                ElseIf (timeoutTimer.IsTimerTicked And
                       __isCheckTimeout) Then
                    'send alarmPack
                    'Console.WriteLine("")
                End If

                '-------------------------------------------
                '   Error Occured , Reconnect
                '--------------------------------------------
            Case operationStates.RECONNECT
                If (resetSocket()) Then
                    operationSubState = operationStates.RECONNECT_WAITING
                End If
            Case operationStates.RECONNECT_WAITING
                '---------------------------------
                '   Reconnect waiting
                '---------------------------------
                If (socketResult.IsCompleted And
                    operationLocked = False) Then
                    '-------------------------
                    'asynchron operation done
                    'check operation result
                    '-------------------------

                    'Select Case ResetMode
                    '    Case resetModeEnum.AS_CLIENT

                    '    Case resetModeEnum.AS_SERVER
                    '        listeningSocket.Close()
                    '    Case Else

                    'End Select

                    If (result = SocketError.Success) Then
                        'done 
                        'Return IDrivable.endStatus.EXECUTION_END
                        operationSubState = operationStates.BEGIN_OPERATION
                    Else
                        'raising alarmPackage
                        sendAlarmSocketError(alarmContextSocket.ErrorOperations.CONNECTING_ERROR)
                    End If
                End If
            Case operationStates.ALARM_HANDLING
                '---------------------
                '   User response determine next state
                '---------------------
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

    Protected Function resetSocket() As Boolean
        '-------------------------------------
        '   Reconnect socket in asynchron mode
        '-------------------------------------
        Try

            communicationSocket.Close()
            communicationSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)


            operationLocked = True

            Select Case ResetMode
                Case resetModeEnum.AS_CLIENT

                    'communicationSocket.Close()
                    'communicationSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

                    socketResult = communicationSocket.BeginConnect(__endPoint,
                                                       Sub(ar As IAsyncResult)
                                                           Try
                                                               communicationSocket.EndConnect(ar)
                                                               result = SocketError.Success
                                                               __isSocketConnected = True

                                                               operationLocked = False

                                                           Catch __socketException As SocketException
                                                               result = __socketException.ErrorCode
                                                               exceptionBlock = __socketException

                                                               operationLocked = False

                                                           Catch ex As Exception
                                                               '-------------------------------------------------------------------------------------
                                                               '  Should catch exception here, because this call used in the thread other than worker
                                                               '  Hsien , 2014.12.12
                                                               '-------------------------------------------------------------------------------------
                                                               result = SocketError.SocketError 'meant to unknown error
                                                               exceptionBlock = ex

                                                               operationLocked = False

                                                           End Try
                                                       End Sub,
                                                       Nothing)
                Case resetModeEnum.AS_SERVER
                    '----------------------------
                    '   Hsien ,  use listening socket to grap client ,2015.05.06
                    '----------------------------
                    'If communicationSocket.Connected Then
                    '    communicationSocket.Disconnect(False)
                    'End If

                    If (Not listeningSocket.IsBound) Then
                        'bind end point when not is bound
                        listeningSocket.Bind(__endPoint)
                    End If
                    listeningSocket.Listen(0) 'resume to listen


                    socketResult = listeningSocket.BeginAccept(Sub(ar As IAsyncResult)
                                                                   Try
                                                                       communicationSocket.Close() 'dispose , if any
                                                                       communicationSocket = listeningSocket.EndAccept(ar)
                                                                       'listeningSocket.shu  'communication build , direct closed
                                                                       result = SocketError.Success
                                                                       __isSocketConnected = True

                                                                       operationLocked = False

                                                                   Catch __socketException As SocketException
                                                                       result = __socketException.ErrorCode
                                                                       exceptionBlock = __socketException

                                                                       operationLocked = False

                                                                   Catch ex As Exception
                                                                       '-------------------------------------------------------------------------------------
                                                                       '  Should catch exception here, because this call used in the thread other than worker
                                                                       '  Hsien , 2014.12.12
                                                                       '-------------------------------------------------------------------------------------
                                                                       result = SocketError.SocketError 'meant to unknown error
                                                                       exceptionBlock = ex

                                                                       operationLocked = False

                                                                   End Try
                                                               End Sub,
                                                               Nothing)
            End Select



            'operationSubState = operationStates.RECONNECT_WAITING
            Return True

        Catch __socketException As SocketException
            '--------------------------------------
            'reconnecting failed , re-raising alarm
            '--------------------------------------
            result = __socketException.SocketErrorCode
            exceptionBlock = __socketException

            sendAlarmSocketError(alarmContextSocket.ErrorOperations.CONNECTING_ERROR)

            operationLocked = False

            Return False

        End Try

    End Function

    Protected Sub sendAlarmSocketError(reason As alarmContextSocket.ErrorOperations)
        '--------------------
        'raising alarmPackage
        '--------------------
        With alarmPackSocketError
            .Operation = reason
            .ExceptionBlock = Me.exceptionBlock
        End With
        CentralAlarmObject.raisingAlarm(alarmPackSocketError)

        operationSubState = operationStates.ALARM_HANDLING
    End Sub


    Public Event CommandExecuted(ByVal sender As Object, ByVal e As EventArgs)  'raise event when command executed successfully
#End Region

    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        Return commandDictionary.Keys
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '處置 Managed 狀態 (Managed 物件)。

            End If
            communicationSocket.Close()
            listeningSocket.Close()         'Hsien , 2015.08.06 , releasing resources
            '釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下面的 Finalize()。
            '將大型欄位設定為 null。
        End If
        Me.disposedValue = True
    End Sub

    '只有當上面的 Dispose(ByVal disposing As Boolean) 有可釋放 Unmanaged 資源的程式碼時，才覆寫 Finalize()。
    Protected Overrides Sub Finalize()
        ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
