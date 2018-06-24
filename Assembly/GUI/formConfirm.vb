Public Class formConfirm
    Property Message As String

    Private Sub loadMessage(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBoxMessage.Text = Message
    End Sub
End Class