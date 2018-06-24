<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlPropertyView
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
        Me.PropertyGridDrive = New System.Windows.Forms.PropertyGrid()
        Me.CheckBoxTimerEnabled = New System.Windows.Forms.CheckBox()
        Me.TimerRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'PropertyGridDrive
        '
        Me.PropertyGridDrive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridDrive.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGridDrive.Name = "PropertyGridDrive"
        Me.PropertyGridDrive.Size = New System.Drawing.Size(320, 240)
        Me.PropertyGridDrive.TabIndex = 0
        '
        'CheckBoxTimerEnabled
        '
        Me.CheckBoxTimerEnabled.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckBoxTimerEnabled.AutoSize = True
        Me.CheckBoxTimerEnabled.Location = New System.Drawing.Point(3, 221)
        Me.CheckBoxTimerEnabled.Name = "CheckBoxTimerEnabled"
        Me.CheckBoxTimerEnabled.Size = New System.Drawing.Size(60, 16)
        Me.CheckBoxTimerEnabled.TabIndex = 1
        Me.CheckBoxTimerEnabled.Text = "Refresh"
        Me.CheckBoxTimerEnabled.UseVisualStyleBackColor = True
        '
        'TimerRefresh
        '
        '
        'userControlPropertyView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.Controls.Add(Me.CheckBoxTimerEnabled)
        Me.Controls.Add(Me.PropertyGridDrive)
        Me.Name = "userControlPropertyView"
        Me.Size = New System.Drawing.Size(320, 240)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PropertyGridDrive As System.Windows.Forms.PropertyGrid
    Friend WithEvents CheckBoxTimerEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents TimerRefresh As System.Windows.Forms.Timer

End Class
