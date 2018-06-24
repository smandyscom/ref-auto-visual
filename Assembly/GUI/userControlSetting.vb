Imports System.Text.RegularExpressions
Imports System.Reflection
Imports Automation
Public Class userControlSetting

    Sub loadControl() Handles MyBase.Load

        For Each item As [Enum] In Assembly.Instance.settingDictionary.Keys
            ComboBoxSettingSelection.Items.Add(item)
        Next

    End Sub

    Sub selectionChanged() Handles ComboBoxSettingSelection.SelectedIndexChanged

        Me.PropertyGridSetting.SelectedObject =
            Assembly.Instance.settingDictionary(ComboBoxSettingSelection.SelectedItem)

    End Sub



End Class
