﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlAliveBar
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
        Me.TextBoxCycleTime = New System.Windows.Forms.TextBox()
        Me.ProgressBarSystem = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'TextBoxCycleTime
        '
        Me.TextBoxCycleTime.Location = New System.Drawing.Point(60, 0)
        Me.TextBoxCycleTime.Margin = New System.Windows.Forms.Padding(0)
        Me.TextBoxCycleTime.Multiline = True
        Me.TextBoxCycleTime.Name = "TextBoxCycleTime"
        Me.TextBoxCycleTime.ReadOnly = True
        Me.TextBoxCycleTime.Size = New System.Drawing.Size(91, 32)
        Me.TextBoxCycleTime.TabIndex = 8
        Me.TextBoxCycleTime.Text = "1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "2"
        '
        'ProgressBarSystem
        '
        Me.ProgressBarSystem.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ProgressBarSystem.Location = New System.Drawing.Point(0, 0)
        Me.ProgressBarSystem.Margin = New System.Windows.Forms.Padding(0)
        Me.ProgressBarSystem.Maximum = 10
        Me.ProgressBarSystem.Name = "ProgressBarSystem"
        Me.ProgressBarSystem.Size = New System.Drawing.Size(60, 32)
        Me.ProgressBarSystem.Step = 1
        Me.ProgressBarSystem.TabIndex = 7
        '
        'userControlAliveBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TextBoxCycleTime)
        Me.Controls.Add(Me.ProgressBarSystem)
        Me.Name = "userControlAliveBar"
        Me.Size = New System.Drawing.Size(149, 29)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxCycleTime As System.Windows.Forms.TextBox
    Friend WithEvents ProgressBarSystem As System.Windows.Forms.ProgressBar

End Class
