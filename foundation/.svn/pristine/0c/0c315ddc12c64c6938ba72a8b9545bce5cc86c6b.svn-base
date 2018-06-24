''' <summary>
''' Viewing only , no way to edit
''' </summary>
''' <remarks></remarks>
Public Class userControlListViewer

    WriteOnly Property CollectionReference As ListAsQueue(Of Object)
        Set(value As ListAsQueue(Of Object))
            __collectionReference = value
            binding.DataSource = __collectionReference
            DataGridViewList.DataSource = binding
            LabelCount.Text = __collectionReference.Count
            TimerScan.Enabled = True
        End Set
    End Property

    Dim binding As BindingSource = New BindingSource
    Dim WithEvents __collectionReference As ListAsQueue(Of Object) = Nothing


    Sub collectionChanged() Handles __collectionReference.CollectionChanged

        Me.Invoke(Sub()
                      LabelCount.Text = __collectionReference.Count
                      binding.ResetBindings(True)
                  End Sub)

    End Sub

    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles TimerScan.Tick
        With DataGridViewList
            .Update()
            .Refresh()
        End With
    End Sub
End Class
