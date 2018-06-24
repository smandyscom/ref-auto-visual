Public Class userControlProcedureContextList

    WriteOnly Property ProcedureListReference As List(Of procedureContext)
        Set(value As List(Of procedureContext))
            __procedureListReference = value
        End Set
    End Property
    Dim __procedureListReference As List(Of procedureContext) = Nothing

    Sub loadControl() Handles MyBase.Load

        If __procedureListReference Is Nothing Then
            Exit Sub
        End If

        DataGridViewProcedure.DataSource = __procedureListReference
        timerScan.Enabled = True
    End Sub

    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles timerScan.Tick
        With DataGridViewProcedure
            .Refresh()
            .Update()
        End With
    End Sub

End Class
