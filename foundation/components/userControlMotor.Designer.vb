﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlMotor
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
        Me.ButtonSimultanousSetup = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonDrive = New System.Windows.Forms.Button()
        Me.ComboBoxPositions = New System.Windows.Forms.ComboBox()
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonSimultanousSetup)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonDrive)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxPositions)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxCommand)
        Me.SplitContainer1.Size = New System.Drawing.Size(640, 480)
        Me.SplitContainer1.SplitterDistance = 213
        Me.SplitContainer1.TabIndex = 0
        '
        'ButtonSimultanousSetup
        '
        Me.ButtonSimultanousSetup.Enabled = False
        Me.ButtonSimultanousSetup.Location = New System.Drawing.Point(89, 94)
        Me.ButtonSimultanousSetup.Name = "ButtonSimultanousSetup"
        Me.ButtonSimultanousSetup.Size = New System.Drawing.Size(121, 23)
        Me.ButtonSimultanousSetup.TabIndex = 5
        Me.ButtonSimultanousSetup.Text = "Simultanous Setup"
        Me.ButtonSimultanousSetup.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(29, 42)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 12)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Position："
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
        Me.ButtonDrive.Location = New System.Drawing.Point(89, 65)
        Me.ButtonDrive.Name = "ButtonDrive"
        Me.ButtonDrive.Size = New System.Drawing.Size(121, 23)
        Me.ButtonDrive.TabIndex = 2
        Me.ButtonDrive.Text = "Drive"
        Me.ButtonDrive.UseVisualStyleBackColor = True
        '
        'ComboBoxPositions
        '
        Me.ComboBoxPositions.FormattingEnabled = True
        Me.ComboBoxPositions.Location = New System.Drawing.Point(89, 39)
        Me.ComboBoxPositions.Name = "ComboBoxPositions"
        Me.ComboBoxPositions.Size = New System.Drawing.Size(121, 20)
        Me.ComboBoxPositions.TabIndex = 1
        '
        'ComboBoxCommand
        '
        Me.ComboBoxCommand.FormattingEnabled = True
        Me.ComboBoxCommand.Location = New System.Drawing.Point(89, 4)
        Me.ComboBoxCommand.Name = "ComboBoxCommand"
        Me.ComboBoxCommand.Size = New System.Drawing.Size(121, 20)
        Me.ComboBoxCommand.TabIndex = 0
        '
        'userControlMotor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "userControlMotor"
        Me.Size = New System.Drawing.Size(640, 480)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ButtonDrive As System.Windows.Forms.Button
    Friend WithEvents ComboBoxPositions As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxCommand As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonSimultanousSetup As System.Windows.Forms.Button

End Class
