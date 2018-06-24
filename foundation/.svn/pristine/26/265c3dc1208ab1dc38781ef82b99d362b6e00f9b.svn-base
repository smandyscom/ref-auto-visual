Imports Automation.Components.Services
Public Class userControlPropertyView
    Property Drive As driveBase
        Get
            Return __driveReference
        End Get
        Set(value As driveBase)
            __driveReference = value
            loadProperties(Me, Nothing)
        End Set
    End Property

    Dim __driveReference As driveBase

    Private Sub enableRefresh(sender As Object, e As EventArgs) Handles CheckBoxTimerEnabled.CheckedChanged
        Me.TimerRefresh.Enabled = CheckBoxTimerEnabled.Checked
        Me.PropertyGridDrive.Enabled = Not CheckBoxTimerEnabled.Checked
    End Sub

    Private Sub refreshProperties(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        Me.PropertyGridDrive.Refresh()
    End Sub

    Private Sub loadProperties(sender As Object, e As EventArgs) Handles MyBase.Load
        If (Drive Is Nothing) Then
            Exit Sub
        End If

        Me.PropertyGridDrive.SelectedObject = Drive
    End Sub
End Class
