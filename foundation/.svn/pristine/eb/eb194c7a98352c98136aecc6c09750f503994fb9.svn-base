Public Class userControlAliveBar

    Public WithEvents assemblyReference As assemblyArch

    Dim maxScanningTime As TimeSpan
    Dim tempMaxScanningTime As TimeSpan
    Dim currentPrivateMemory As Long
    Dim maxPrivateMemory As Long

    Public Sub systemAlive(ByVal sender As Object, ByVal e As EventArgs) Handles assemblyReference.ProcessProgressed
        If (Me.IsDisposed) Then
            Exit Sub 'reject to delegate
        End If

        'record max cycle time used
        If (assemblyReference.CycleTime.TotalMilliseconds > maxScanningTime.TotalMilliseconds) Then
            maxScanningTime = assemblyReference.CycleTime
            assemblyReference.sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Max program scanning time detected : {0} ms", maxScanningTime.TotalMilliseconds))
        End If

        If (assemblyReference.CycleTime.TotalMilliseconds > tempMaxScanningTime.TotalMilliseconds) Then
            tempMaxScanningTime = assemblyReference.CycleTime
        End If

        'record max private memory used
        currentPrivateMemory = Process.GetCurrentProcess.PrivateMemorySize64
        If (currentPrivateMemory > maxPrivateMemory) Then
            maxPrivateMemory = currentPrivateMemory
            assemblyReference.sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Max private memory detected : {0} B", maxPrivateMemory))
        End If

        Me.Invoke(Sub()
                      With ProgressBarSystem
                          Me.TextBoxCycleTime.Text = assemblyReference.CycleTime.TotalMilliseconds.ToString("0.0") & "ms" & vbNewLine & "max:" & tempMaxScanningTime.TotalMilliseconds.ToString("0.0") & "ms"
                          .PerformStep()
                          If (.Value = .Maximum) Then
                              .Value = 0
                          End If
                      End With
                  End Sub)
    End Sub


    Private Sub TextBoxCycleTime_DoubleClick(sender As Object, e As EventArgs) Handles TextBoxCycleTime.DoubleClick
        tempMaxScanningTime = New TimeSpan(0)
    End Sub
End Class
