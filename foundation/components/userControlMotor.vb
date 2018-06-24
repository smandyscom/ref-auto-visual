Imports Automation.Components.CommandStateMachine
Imports System.ComponentModel

Public Class userControlMotor
    Property Motor As motorControl
    Property PropertyView As userControlPropertyView

    'Dim simultanousCommands As List(Of cMotorPoint) = New List(Of cMotorPoint)  'Hsien , 2015.01.25 , support simulatanous command

    Private Sub loadUserControlMotor(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        For Each pair As KeyValuePair(Of [Enum], motorControl.commandFunctionPrototype) In Motor.CommandFunctionDictionary
            ComboBoxCommand.Items.Add(pair.Key)
        Next

        For Each pair As KeyValuePair(Of [Enum], Short) In Motor.PositionDictionary
            ComboBoxPositions.Items.Add(pair.Key)
        Next

        'UserControlPropertyViewMotor.Drive = Motor
        Me.SplitContainer1.Panel2.Controls.Add(PropertyView)
        PropertyView.Dock = DockStyle.Fill
    End Sub

    Private Sub driveMotor(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonDrive.Click
        If (ComboBoxPositions.SelectedItem IsNot Nothing) Then
            Motor.drive(ComboBoxCommand.SelectedItem, ComboBoxPositions.SelectedItem)
        Else
            Motor.drive(ComboBoxCommand.SelectedItem)
        End If
    End Sub

    'Private Sub simultanousSetup(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonSimultanousSetup.Click
    '    Dim __form As Form = New Form()
    '    __form.Controls.Add(New DataGridView With {.DataSource = simultanousCommands,
    '                                              .Dock = DockStyle.Fill})
    '    If (__form.ShowDialog()) Then
    '        Motor.PointTable = simultanousCommands
    '    End If

    'End Sub


End Class
