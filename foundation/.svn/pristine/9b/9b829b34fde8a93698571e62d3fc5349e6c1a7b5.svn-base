Public Class LoginForm
    Property blnPassed As Boolean = False
    Property strPassword As String = ""
    Property strDescription As String = "Please enter the password."
    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        If PasswordTextBox.Text = strPassword Then
            blnPassed = True
            Me.Close()
        Else
            MsgBox("The password is incorrect.")
        End If
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PasswordLabel.Text = strDescription
    End Sub
End Class
