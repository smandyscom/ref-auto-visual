Imports Automation

Public Class userControlAnalogMonitor
    Sub loadControl() Handles MyBase.Load
        Me.LabelChannelName.Text = Me.Tag.ToString
        timerScan.Enabled = True
    End Sub

    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles timerScan.Tick
        Me.TextBoxValue.Text = mainIOHardware.readDouble(Me.Tag)
    End Sub
End Class
