Public Class userControlPhysicalHardwares

    WriteOnly Property HardwareHubReference As mainIOHardware
        Set(value As mainIOHardware)
            __hardwareHubReference = value
            __bindingSource.DataSource = __hardwareHubReference.__physicalHardwareList
        End Set
    End Property


    Dim __hardwareHubReference As mainIOHardware = Nothing
    Dim __bindingSource As BindingSource = New BindingSource

    Sub loadControl() Handles MyBase.Load

        If (__hardwareHubReference Is Nothing) Then
            Exit Sub
        End If

        With Me.DataGridViewList
            .DataSource = __bindingSource
            .Columns(0).Visible = False 'do not show the physical type
        End With

        TimerScan.Enabled = True
    End Sub



    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles TimerScan.Tick
        With Me.DataGridViewList
            .Update()
            .Refresh()
        End With
    End Sub
End Class
