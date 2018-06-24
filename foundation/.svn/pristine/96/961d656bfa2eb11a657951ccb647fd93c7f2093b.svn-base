Public Class userControlDrivable
    Property Component As IDrivable
    Property PropertyView As userControlPropertyView

    Private Sub loadUserControl(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        'For Each pair As KeyValuePair(Of [Enum], motorControl.commandFunctionPrototype) In Motor.CommandFunctionDictionary
        '    ComboBoxCommand.Items.Add(pair.Key)
        'Next
        Dim __obj = Component.getCommands
        'ComboBoxCommand.Items.Add(__obj(0))
        For index = 0 To Component.getCommands.Count - 1
            ComboBoxCommand.Items.Add(__obj(index))
        Next


        'UserControlPropertyViewMotor.Drive = Motor
        Me.SplitContainer1.Panel2.Controls.Add(PropertyView)
        PropertyView.Dock = DockStyle.Fill
    End Sub

    Private Sub driveMotor(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonDrive.Click
        If (ComboBoxCommand.SelectedItem IsNot Nothing) Then
            Component.drive(ComboBoxCommand.SelectedItem)
        End If
    End Sub
End Class
