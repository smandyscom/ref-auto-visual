<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlSetting
    Inherits System.Windows.Forms.UserControl

    'UserControl 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意:  以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請不要使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(userControlSetting))
        Me.PropertyGridSetting = New System.Windows.Forms.PropertyGrid()
        Me.TimerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.ComboBoxSettingSelection = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'PropertyGridSetting
        '
        resources.ApplyResources(Me.PropertyGridSetting, "PropertyGridSetting")
        Me.PropertyGridSetting.Name = "PropertyGridSetting"
        '
        'TimerStatus
        '
        Me.TimerStatus.Interval = 500
        '
        'ComboBoxSettingSelection
        '
        resources.ApplyResources(Me.ComboBoxSettingSelection, "ComboBoxSettingSelection")
        Me.ComboBoxSettingSelection.FormattingEnabled = True
        Me.ComboBoxSettingSelection.Name = "ComboBoxSettingSelection"
        '
        'userControlSetting
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.ComboBoxSettingSelection)
        Me.Controls.Add(Me.PropertyGridSetting)
        Me.Name = "userControlSetting"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TimerStatus As System.Windows.Forms.Timer
    Friend WithEvents PropertyGridSetting As System.Windows.Forms.PropertyGrid
    Friend WithEvents ComboBoxSettingSelection As System.Windows.Forms.ComboBox

End Class
