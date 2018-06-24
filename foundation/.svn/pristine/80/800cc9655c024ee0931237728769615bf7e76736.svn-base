Public Class userControlCylinderBase
    Property cylinderReference As cylinderControlBase
    Property PropertyView As userControlPropertyView
    Private Sub loadUserControlMotor(sender As Object, e As EventArgs) Handles MyBase.Load
        '----------------------------------------------
        '   Loading commands
        '----------------------------------------------
        For Each value As Object In [Enum].GetValues(GetType(cylinderControlBase.cylinderCommands))
            ComboBoxCommand.Items.Add([Enum].ToObject(GetType(cylinderControlBase.cylinderCommands), value))
        Next

        Me.SplitContainer1.Panel2.Controls.Add(PropertyView)
        PropertyView.Dock = DockStyle.Fill
    End Sub

    Private Sub driveCylinder(sender As Object, e As EventArgs) Handles ButtonDrive.Click
        cylinderReference.drive(ComboBoxCommand.SelectedItem)
    End Sub
End Class
