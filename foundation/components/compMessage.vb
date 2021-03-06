﻿Imports Automation.Components.Services
Imports System.Collections.Concurrent
Imports System.Threading
Imports IWshRuntimeLibrary
Imports System.IO

'---------------------------
'   Basic Message Type/Value
'---------------------------
Public Enum alarmGeneric
    DUMMY
End Enum
Public Enum exceptionEnum
    PROGRAM_ERROR
    UNHANDLED_ERROR
    MESSAGE_KEY_NOT_FOUND
End Enum
Public Class warningMessagePackage
    Inherits messagePackage
    Property _isEnableBuzzer As Boolean
    Public Sub New(ByVal _sender As Object, Optional ByVal _addition As String = "", Optional IsEnableBuzzer As Boolean = False)
        MyBase.New(_sender, statusEnum.WARNING_MESSAGE_ADD, _addition)
        _isEnableBuzzer = IsEnableBuzzer
    End Sub
End Class
Public Class messagePackage

    Shared Property Format As String = "{0}({1}){2},{3}"     ' timestamp-sender-message(queryed by key)-additional info
    Shared Property QueryInterface As IMessageQuery = Nothing

    Property TimeStamp As Date
    Property Sender As Object       ' to device name
    Property PrimaryKey As Object        ' primary key
    Property AdditionalInfo As String = ""  ' would append on the tail

    Public Sub New(ByVal _sender As Object, ByVal _key As Object, Optional ByVal _addition As String = "")
        TimeStamp = DateAndTime.Now
        Sender = _sender
        PrimaryKey = _key
        AdditionalInfo = _addition
    End Sub

    Public Overrides Function ToString() As String
        Try
            Dim queriedMessage As String = "Query Interface Not Linked"
            Dim senderString As String = Sender.ToString()
            If (QueryInterface IsNot Nothing) Then
                queriedMessage = QueryInterface.query(PrimaryKey)
                senderString = QueryInterface.query(Sender)
            Else
                queriedMessage = PrimaryKey.ToString()
            End If
            'Console.WriteLine(QueryInterface.query(PrimaryKey))
            Return String.Format(Format _
                                                 , TimeStamp.ToString("yyyy/MM/dd HH:mm:ss.ffff") _
                                                 , senderString _
                                                 , queriedMessage _
                                                 , AdditionalInfo)
        Catch ex As Exception
            Return "Exception : messagePack.ToString()"
        End Try

    End Function

    'Hsien , 2015.12.30 , could be used to identify if redundant message
    Public Overrides Function Equals(obj As Object) As Boolean
        Dim handle As messagePackage = TryCast(obj, messagePackage)
        If (handle IsNot Nothing) Then
            Return Me.Sender.Equals(handle.Sender) And
                Me.PrimaryKey.Equals(handle.PrimaryKey) And
                Me.AdditionalInfo.Equals(handle.AdditionalInfo)
        Else
            'cannot casting to messagePackage type
            Return MyBase.Equals(obj)
        End If
    End Function

End Class

Public Class messagePackageEventArg : Inherits EventArgs
    Property Message As messagePackage
End Class
Public Class messageHandler

    'Property MessageQueue As Queue(Of messagePackage) = New Queue(Of messagePackage)
    Property MessageQueue As ConcurrentQueue(Of messagePackage) = New ConcurrentQueue(Of messagePackage)

    Public Event MessagePoped As EventHandler(Of messagePackageEventArg)

    Dim arg As messagePackageEventArg = New messagePackageEventArg()

    Public Function messageHandling() As Integer

        '-------------------------------------------------------------
        '   When Theres something in queue , pop out and raising event
        '-------------------------------------------------------------

        If (MessageQueue.Count <> 0) Then
            'arg.Message = MessageQueue.Dequeue()

            'considered the multithread issue (i.e : sendMessage in GUI thread) , use thread-safe collection
            'Hsien , 2015.10.05
            If (MessageQueue.TryDequeue(arg.Message)) Then
                RaiseEvent MessagePoped(Me, arg)
            End If

        End If

        Return 0
    End Function

End Class

Public Class logHandler : Implements IDisposable
    '----------------------------------------
    '   Seperated log functions from messageHandler
    '----------------------------------------
    Shared Property DirectoryPath As String = My.Application.Info.DirectoryPath + "\Log\"
    Property Extension As String = ".log"
    Property MaxLogFileCapability As Integer = 30
    Property IsAsynchronWriteLog As Boolean
        Get
            Return __isAsynchronWriteLog
        End Get
        Set(value As Boolean)
            __isAsynchronWriteLog = value
            If (__isAsynchronWriteLog) Then
                '-------------------------------
                '   Fire the file-writing thread
                '   Hsien , 2015.10.01
                '-------------------------------
                fileWriteWorkerThread.Start()
            Else
                fileWriteWorkerThread.Abort()
            End If
        End Set
    End Property

    Public ContentFilter As Func(Of messageHandler, messagePackageEventArg, Boolean) = Function() (True)

    Public WithEvents MessengerReference As messageHandler  'used as reference to messenger going to handling
    Shared threadSleepTime As Integer = 50
    '----------------
    '   logger system
    '----------------
    Private logFile As System.IO.StreamWriter
    Private logFilePath As String
    Private logDir As IO.DirectoryInfo
    Private currentDate As Date

    Dim fileWriteWorkerThread As Thread = New Thread(AddressOf logMessageTask) With {.IsBackground = True,
                                                                                     .Priority = ThreadPriority.BelowNormal}
    Dim __isAsynchronWriteLog As Boolean = False 'used to choose method to write into file

    Shared ReadOnly Property TestLogFilePath(__extenstion As String) As String
        Get
            Return DirectoryPath & "\" & Date.Today.ToString("yyyy-MM-dd") & __extenstion
        End Get
    End Property

    Public Sub New(ByVal __extension As String)

        Me.Extension = __extension

        'establish log
        '1. find if file existed with matched file name
        '2. if no , establish one
        '3. open the file
        logDir = New IO.DirectoryInfo(DirectoryPath)
        If (Not logDir.Exists) Then
            IO.Directory.CreateDirectory(logDir.FullName)
        End If

        currentDate = Date.Today
        'logFilePath = logDir.FullName + "\" + currentDate.ToString("yyyy-MM-dd") + extension
        logFilePath = TestLogFilePath(__extension)
        logFile = New IO.StreamWriter(logFilePath, True)    ' if file not existed , would create new one , otherwise in append mode

        fileWriteWorkerThread.Name = logFilePath


        If (directoryManagementThread Is Nothing) Then
            directoryManagementThread = New Tasks.Task(AddressOf maintenanceWorks)
            directoryManagementThread.Start()
        End If

    End Sub

    Protected Sub swapFileCrossDay()

        If (Date.Today <> currentDate) Then
            '----------------
            '   Create new file
            '----------------
            logFile.Close()     ' close old file    , 

            currentDate = Date.Today
            logFilePath = logDir.FullName + "\" + currentDate.ToString("yyyy-MM-dd") + Extension
            logFile = New IO.StreamWriter(logFilePath, True)    'create new file

        End If

    End Sub

    Public Sub logMessageToFile(ByVal sender As messageHandler, ByVal e As messagePackageEventArg) Handles MessengerReference.MessagePoped
        'the default event handler

        '-----------------------
        '   Content inlet Filter
        '   return value : true - passed , would do following write-in procedure
        '                : false - failed , would skip following write-in procedure
        '-----------------------
        If (Not ContentFilter(sender, e)) Then
            Exit Sub
        End If

        If (IsAsynchronWriteLog) Then
            'asynchron way
            __crossThreadQueue.Enqueue(e.Message)  'enqueue , asynchron write-in
        Else
            '--------------------------
            'synchron way
            '--------------------------
            swapFileCrossDay()

            logFile.WriteLine(e.Message.ToString())
            logFile.Flush() 'Hsien , flush into hard-drive immediately , 2015.07.28
            'from MSDN : 
            ' https://msdn.microsoft.com/zh-tw/library/system.io.streamwriter.autoflush(v=vs.110).aspx
            'Flushing the stream will not flush its underlying encoder unless you explicitly call Flush or Close. 
            'Setting AutoFlush to true means that data will be flushed from the buffer to the stream, but the encoder state will not be flushed. This allows the encoder to keep its state (partial characters) so that it can encode the next block of characters correctly. This scenario affects UTF8 and UTF7 where certain characters can only be encoded after the encoder receives the adjacent character or characters.

            'so that, had to explicit call flush avoiding memory exception , Hsien , 2015.07.28

        End If
    End Sub


    Dim __crossThreadQueue As Concurrent.ConcurrentQueue(Of messagePackage) = New Concurrent.ConcurrentQueue(Of messagePackage)    'cross thread concurrent , Hsien , 2015.09.30
    Dim loopFuse As Boolean = True
    Dim __popedMessage As messagePackage = Nothing
    Sub logMessageTask()
        '----------------------------------
        '   Comminicate With Machine-Control thread by concurrent queue
        '----------------------------------
        'looping condition : loopFuse was True or __queue not cleared
        Using logFile

            While loopFuse Or __crossThreadQueue.Count <> 0

                If (__crossThreadQueue.TryDequeue(__popedMessage)) Then
                    'once poped successfully
                    swapFileCrossDay()

                    logFile.WriteLine(__popedMessage.ToString())
                    logFile.Flush() 'Hsien , flush into hard-drive immediately , 2015.07.28

                End If

                Dim __schedualedSleepTime As Integer = threadSleepTime - __crossThreadQueue.Count
                If (__schedualedSleepTime < 0) Then
                    'too much things on queue , running at maximum speed
                    __schedualedSleepTime = 0
                End If

                Thread.Sleep(__schedualedSleepTime)
            End While

        End Using


        'loop released

    End Sub


#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '處置 Managed 狀態 (Managed 物件)。
                loopFuse = False 'release file write thread

                'wait until file writing thread finished 
                While (fileWriteWorkerThread.ThreadState And (ThreadState.Stopped Or ThreadState.Unstarted)) = False

                End While
                logFile.Close()
            End If

            '釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下面的 Finalize()。
            '將大型欄位設定為 null。
        End If
        Me.disposedValue = True
    End Sub

    '只有當上面的 Dispose(ByVal disposing As Boolean) 有可釋放 Unmanaged 資源的程式碼時，才覆寫 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region


    Shared directoryManagementThread As Tasks.Task = Nothing
    Shared __interval As TimeSpan = New TimeSpan(0, 0, 30)
    Shared outdateValue As Double = -30 ' should be minus
    Shared alarmHistoryName As String = "alarmHistory\"
    Shared alarmDirectory As DirectoryInfo = New DirectoryInfo(DirectoryPath & alarmHistoryName)
    ''' <summary>
    ''' Check out whether any out-dated 
    ''' </summary>
    ''' <remarks></remarks>
    Sub maintenanceWorks()

        While True

            '------------------------------------------------
            '   Find out all files outof data , and delete it
            '------------------------------------------------
            Dim filesInfo As List(Of System.IO.FileInfo) = logDir.GetFiles().ToList
            filesInfo.FindAll(Function(__info As System.IO.FileInfo) __info.CreationTime < DateTime.Today.AddDays(outdateValue)).ForEach(Sub(__info As System.IO.FileInfo) __info.Delete())
            '------------------------------------------------
            '   Create alarmHistory Directory and build link
            '------------------------------------------------
            If (Not alarmDirectory.Exists) Then
                Directory.CreateDirectory(alarmDirectory.FullName)
            End If
            'examine if all link existed in alarmHistory and connected to alarm log
            Dim alarmInfos As List(Of FileInfo) = alarmDirectory.GetFiles("*.lnk").ToList()
            filesInfo = logDir.GetFiles("*.alarm.log").ToList
            'find those whose didnt existed in alarmHistory
            Dim missedLink As List(Of FileInfo) = filesInfo.FindAll(Function(info As FileInfo) Not alarmInfos.Exists(Function(__info As FileInfo) __info.Name = info.Name))

            'create link for thosed missed links
            Dim wsh As WshShell = New WshShell
            For Each item As FileInfo In missedLink
                Dim sc As IWshShortcut = wsh.CreateShortcut(alarmDirectory.FullName & item.Name & ".lnk")
                With sc
                    .TargetPath = logDir.FullName & item.Name
                    .Save()
                End With
            Next

            Thread.Sleep(__interval)

        End While


    End Sub


End Class