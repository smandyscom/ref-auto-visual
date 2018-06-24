Imports Automation
Imports AutoNumeric

Public Class userControlFrameManager2
    Sub loadControl() Handles MyBase.Load
        For Each item As htmEdgeBase In frames.Instance.HtmsNeedReload
            ComboBoxSelection.Items.Add(item)
        Next

    End Sub

    Sub comboBoxItemChanged(sender As ComboBox, e As EventArgs) Handles ComboBoxSelection.SelectedIndexChanged
        PropertyGridManager.SelectedObject =
            ComboBoxSelection.SelectedItem
    End Sub
End Class
