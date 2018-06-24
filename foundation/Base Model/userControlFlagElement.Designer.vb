<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFlagElement
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
        Me.LabelFlagName = New System.Windows.Forms.Label()
        Me.LabelFlagStatus = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'LabelFlagName
        '
        Me.LabelFlagName.AutoSize = True
        Me.LabelFlagName.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.LabelFlagName.Location = New System.Drawing.Point(34, 4)
        Me.LabelFlagName.Name = "LabelFlagName"
        Me.LabelFlagName.Size = New System.Drawing.Size(127, 12)
        Me.LabelFlagName.TabIndex = 0
        Me.LabelFlagName.Text = "FLAG_NAME_XXXXXX"
        '
        'LabelFlagStatus
        '
        Me.LabelFlagStatus.BackColor = System.Drawing.Color.Red
        Me.LabelFlagStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelFlagStatus.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.LabelFlagStatus.Location = New System.Drawing.Point(3, 3)
        Me.LabelFlagStatus.Margin = New System.Windows.Forms.Padding(3)
        Me.LabelFlagStatus.Name = "LabelFlagStatus"
        Me.LabelFlagStatus.Size = New System.Drawing.Size(25, 25)
        Me.LabelFlagStatus.TabIndex = 1
        Me.LabelFlagStatus.Text = "F"
        '
        'userControlFlagElement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.LabelFlagStatus)
        Me.Controls.Add(Me.LabelFlagName)
        Me.Name = "userControlFlagElement"
        Me.Size = New System.Drawing.Size(164, 31)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LabelFlagName As System.Windows.Forms.Label
    Friend WithEvents LabelFlagStatus As System.Windows.Forms.Label

End Class
