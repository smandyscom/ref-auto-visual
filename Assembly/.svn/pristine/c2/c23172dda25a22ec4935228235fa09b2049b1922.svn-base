Imports System.Text

Public Class UserControl_LightControl

    Public refLightControl As lightControl = Nothing

    Dim __controlCollection As List(Of Label) = New List(Of Label)
    Dim __valueList As List(Of Integer) = Nothing ' build be inner object

    Private Sub loadControl(sender As Object, e As EventArgs) Handles MyBase.Load

        If refLightControl Is Nothing Then
            Exit Sub
        End If

        cmb_channel.SelectedItem = "1"
        TextBox_LightIntensity.Text = "0"

        Me.DataGridViewLight.DataSource = refLightControl.CurrentSettingValue

        timerRefresh.Enabled = True
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        TextBox_LightIntensity.Text = TrackBar1.Value
    End Sub

    Private Sub TextBox_LightMagnitude_TextChanged(sender As Object, e As EventArgs) Handles TextBox_LightIntensity.TextChanged
        If Not IsNumeric(TextBox_LightIntensity.Text) Then
            TextBox_LightIntensity.Text = ""
            Exit Sub
        End If
        If Convert.ToInt16(TextBox_LightIntensity.Text) >= TrackBar1.Minimum And Convert.ToInt16(TextBox_LightIntensity.Text) <= TrackBar1.Maximum Then
            TrackBar1.Value = Convert.ToInt16(TextBox_LightIntensity.Text)
        End If
    End Sub

    Private Sub Btn_Set_Click(sender As Object, e As EventArgs) Handles Btn_Set.Click
        Dim __channel As Integer = 0
        Dim __intesity As Integer = 0

        If Integer.TryParse(cmb_channel.Text, __channel) And
           Integer.TryParse(TextBox_LightIntensity.Text, __intesity) Then

            refLightControl.Intensity(__channel) = __intesity
        End If

        refLightControl.IsCommunicating = True
    End Sub

    Private Sub timerScan(sender As Object, e As EventArgs) Handles timerRefresh.Tick
        PanelControl.Enabled = Not refLightControl.IsCommunicating
        Me.Enabled = refLightControl.IsLinked

        For Each item As Binding In Me.DataGridViewLight.DataBindings
            item.ReadValue()
        Next

    End Sub
End Class
