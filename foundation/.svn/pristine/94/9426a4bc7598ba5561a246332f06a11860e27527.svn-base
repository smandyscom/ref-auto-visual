Public Class userControlFlagElement

    Property FlagElementReference As flagElement
        Get
            Return __flagElementReference
        End Get
        Set(value As flagElement)
            __flagElementReference = value
        End Set
    End Property


    Dim __flagElementReference As flagElement


    Public Overrides Sub Refresh()
        If (FlagElementReference.Flag) Then
            LabelFlagStatus.BackColor = Color.Green
            LabelFlagStatus.Text = "T"
        Else
            LabelFlagStatus.BackColor = Color.Red
            LabelFlagStatus.Text = "F"
        End If
        MyBase.Refresh()
    End Sub

    Private Sub MouseClickFlip(sender As Object, e As MouseEventArgs) Handles LabelFlagStatus.MouseClick
        ' if right-click , flip the login
        If (e.Button = Windows.Forms.MouseButtons.Right) Then
            FlagElementReference.Flag = Not FlagElementReference.Flag
        End If
    End Sub

    Private Sub MouseDownSet(sender As Object, e As MouseEventArgs) Handles LabelFlagStatus.MouseDown
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            FlagElementReference.Flag = True
        End If
    End Sub

    Private Sub MouseDownUnset(sender As Object, e As MouseEventArgs) Handles LabelFlagStatus.MouseUp
        If (e.Button = Windows.Forms.MouseButtons.Left) Then
            FlagElementReference.Flag = False
        End If
    End Sub


    Private Sub LoadNameLabel(sender As Object, e As EventArgs) Handles MyBase.Load
        'load name label
        LabelFlagName.Text = String.Format("{0}{1}{2}",
                                           FlagElementReference.controllerName,
                                           Environment.NewLine,
                                           FlagElementReference.Label)
    End Sub
End Class
