﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlMessage
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
        Me.RichTextBoxMessage = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'RichTextBoxMessage
        '
        Me.RichTextBoxMessage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBoxMessage.Location = New System.Drawing.Point(0, 0)
        Me.RichTextBoxMessage.Name = "RichTextBoxMessage"
        Me.RichTextBoxMessage.Size = New System.Drawing.Size(960, 560)
        Me.RichTextBoxMessage.TabIndex = 0
        Me.RichTextBoxMessage.Text = ""
        '
        'userControlMessageHistory
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RichTextBoxMessage)
        Me.Name = "userControlMessageHistory"
        Me.Size = New System.Drawing.Size(960, 560)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RichTextBoxMessage As System.Windows.Forms.RichTextBox

End Class
