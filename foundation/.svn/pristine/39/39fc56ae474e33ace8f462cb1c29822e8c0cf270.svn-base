Imports Automation.Components.Services

Public Class UserControlComponent
    Property componentReference As driveBase
    Private subUserControl As Control = Nothing

    Public Overrides Sub Refresh()
        If (componentReference.isEnabled) Then
            LabelIsEnabled.BackColor = Color.Green
        Else
            LabelIsEnabled.BackColor = Color.Red
        End If

        If (Not subUserControl Is Nothing) Then
            subUserControl.Refresh()
        End If

        MyBase.Refresh()
    End Sub

    Private Sub LabelIsEnabled_Click(sender As Object, e As EventArgs) Handles LabelIsEnabled.Click
        ' flip
        componentReference.isEnabled = Not componentReference.isEnabled
    End Sub

    Private Sub LabelIsEnabled_ParentChanged(sender As Object, e As EventArgs) Handles LabelIsEnabled.ParentChanged

    End Sub

    

    Private Sub UserControlComponent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'loading()
        TextBoxInstanceName.Text = componentReference.ToString()
        TextBoxType.Text = componentReference.GetType().Name
    End Sub

    Private Sub TextBoxType_DoubleClick(sender As Object, e As EventArgs) Handles TextBoxType.DoubleClick
        ' raising target gui
        Dim dialogForm As Form = New Form()
        Dim uc As UserControl = componentReference.raisingGUI()
        dialogForm.Controls.Add(uc)
        dialogForm.AutoSize = True  'enlarge the window , hsine 2015.03.28
        dialogForm.ShowDialog()
        uc.Dispose()
        'subUserControl = componentReference.raisingGUI()
        'If (Not subUserControl Is Nothing) Then
        '    dialogForm = New Form()
        '    dialogForm.AutoSize = True
        '    dialogForm.Controls.Add(subUserControl)
        '    'dialogForm.Show()
        '    dialogForm.ShowDialog()
        'End If

    End Sub
End Class
