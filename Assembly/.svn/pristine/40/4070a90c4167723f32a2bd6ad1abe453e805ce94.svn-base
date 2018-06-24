Public Class formLogin

    ' (請參閱 http://go.microsoft.com/fwlink/?LinkId=35339)。
    ' 如此便可將自訂主體附加到目前執行緒的主體，如下所示: 
    '     My.User.CurrentPrincipal = CustomPrincipal
    ' 其中 CustomPrincipal 是用來執行驗證的 IPrincipal 實作。
    ' 接著，My.User 便會傳回封裝在 CustomPrincipal 物件中的識別資訊，
    ' 例如使用者名稱、顯示名稱等。
    Public Enum accessLevelEnum
        DEVELOPE = &H100
        SERVICE = &H10
        END_USER = &H1
    End Enum

    Public Class userData
        Property Account As String
        Property Password As String
        Property level As accessLevelEnum = accessLevelEnum.END_USER
        Public Sub New(ByVal account As String, ByVal password As String, ByVal level As accessLevelEnum)
            Me.Account = account
            Me.Password = password
            Me.level = level
        End Sub
    End Class

    Property currentUser As userData = Nothing

    Private userDataBase As Dictionary(Of String, userData) = New Dictionary(Of String, userData)


    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Dim tempUser As userData
        Try
            tempUser = userDataBase(UsernameTextBox.Text)
            If (tempUser.Password <> PasswordTextBox.Text) Then
                Throw New Exception()
            Else
                currentUser = tempUser
                MessageBox.Show(currentUser.Account + " , Welcome")
            End If


        Catch ex As Exception
            MessageBox.Show("Invalid user or password")
        End Try

        Me.Close()
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

    Private Sub LoadLogin(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            userDataBase.Add("KUNG", New userData("STEVEN.KUNG", "DELTA99", accessLevelEnum.DEVELOPE + accessLevelEnum.END_USER + accessLevelEnum.SERVICE))
            userDataBase.Add("user", New userData("user", "user", accessLevelEnum.END_USER))
            userDataBase.Add("service", New userData("service", "service", accessLevelEnum.SERVICE + accessLevelEnum.END_USER))
            'userDataBase.Add("user", New userData("user", "user", accessLevelEnum.END_USER))
        Catch ex As Exception

        End Try

        ComboBoxLanguage.SelectedIndex = 0
        Application.CurrentCulture = Globalization.CultureInfo.CreateSpecificCulture("en-US")

        Me.Text = String.Format("{0:G} {1:G}", My.Application.Info.ProductName, My.Application.Info.Version.ToString())

    End Sub

    Private Sub languageChange(sender As Object, e As EventArgs) Handles ComboBoxLanguage.SelectedIndexChanged
        Select Case ComboBoxLanguage.SelectedIndex
            Case 0
                Application.CurrentCulture = Globalization.CultureInfo.CreateSpecificCulture("en-US")
            Case 1
                Application.CurrentCulture = Globalization.CultureInfo.CreateSpecificCulture("zh-TW")
            Case Else

        End Select

    End Sub
End Class
