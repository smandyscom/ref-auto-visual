<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlDrivable
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonDrive = New System.Windows.Forms.Button()
        Me.ComboBoxCommand = New System.Windows.Forms.ComboBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonDrive)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxCommand)
        Me.SplitContainer1.Size = New System.Drawing.Size(640, 480)
        Me.SplitContainer1.SplitterDistance = 213
        Me.SplitContainer1.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(17, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 12)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Command："
        '
        'ButtonDrive
        '
        Me.ButtonDrive.Location = New System.Drawing.Point(89, 30)
        Me.ButtonDrive.Name = "ButtonDrive"
        Me.ButtonDrive.Size = New System.Drawing.Size(121, 23)
        Me.ButtonDrive.TabIndex = 2
        Me.ButtonDrive.Text = "Drive"
        Me.ButtonDrive.UseVisualStyleBackColor = True
        '
        'ComboBoxCommand
        '
        Me.ComboBoxCommand.FormattingEnabled = True
        Me.ComboBoxCommand.Location = New System.Drawing.Point(89, 4)
        Me.ComboBoxCommand.Name = "ComboBoxCommand"
        Me.ComboBoxCommand.Size = New System.Drawing.Size(121, 20)
        Me.ComboBoxCommand.TabIndex = 0
        '
        'userControlDrivable
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "userControlDrivable"
        Me.Size = New System.Drawing.Size(640, 480)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ButtonDrive As System.Windows.Forms.Button
    Friend WithEvents ComboBoxCommand As System.Windows.Forms.ComboBox

End Class
