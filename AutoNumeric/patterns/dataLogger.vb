Imports System.IO
Imports System.Text
Imports System.Threading

''' <summary>
''' Offered :
''' 1. mechanism of asynchronse writing , in default encoding
''' 2. title of file including time stamp
''' 3. delete those files out of date
''' </summary>
''' <remarks></remarks>
Public Class dataLogger
    Shared Property DirectoryPath As String = "Outputs\"
    Shared outdateValue As Double = -30 ' should be minus
    Shared __interval As TimeSpan = New TimeSpan(0, 0, 30)
    Shared directoryManagementThread As Tasks.Task = Nothing

    Dim __filenameFormat As String = "{0}{1}_{2}.dat" 'datetime_procedurename

    Dim logFile As FileStream = Nothing
    Dim __encoding As Encoding = Encoding.Default

    Sub New(postfix As String)

        'once directory not existed , create one
        If Not Directory.Exists(DirectoryPath) Then
            Directory.CreateDirectory(DirectoryPath)
        End If

        'initialize maintainer(delete old files
        If (directoryManagementThread Is Nothing) Then
            directoryManagementThread = New Tasks.Task(AddressOf maintenanceWorks)
            directoryManagementThread.Start()
        End If

        logFile = New FileStream(String.Format(__filenameFormat,
                                               DirectoryPath,
                                               Now.ToString("yyyyMMddhhmmss"),
                                               postfix),
                                           FileMode.Append,
                                           FileAccess.Write)

    End Sub

    Sub write(input As String, Optional isCloseAfterWritten As Boolean = False)

        Dim __bytes As Byte() = __encoding.GetBytes(input)

        logFile.BeginWrite(__bytes,
                                      0,
                                      __bytes.Length,
                                      Sub(ar As IAsyncResult)
                                          logFile.EndWrite(ar) 'release block state of log file
                                          If CBool(ar.AsyncState) Then
                                              logFile.Dispose() 'after last write command executed
                                          End If
                                      End Sub,
                                      isCloseAfterWritten)

    End Sub
    Sub writeLine(input As String, Optional isCloseAfterWritten As Boolean = False)
        write(String.Format("{0}{1}",
                            input,
                            vbCrLf), isCloseAfterWritten)
    End Sub

    ''' <summary>
    ''' Check out whether any out-dated 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub maintenanceWorks()

        While True

            '------------------------------------------------
            '   Find out all files outof data , and delete it
            '------------------------------------------------
            Dim filesInfo As List(Of FileInfo) = New List(Of FileInfo)
            For Each item As String In Directory.GetFiles(DirectoryPath)
                filesInfo.Add(New FileInfo(item))
            Next
            filesInfo.FindAll(Function(__info As System.IO.FileInfo) __info.CreationTime < DateTime.Today.AddDays(outdateValue)).ForEach(Sub(__info As System.IO.FileInfo) __info.Delete())


            Thread.Sleep(__interval)

        End While


    End Sub


End Class
