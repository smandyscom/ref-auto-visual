﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlAlarm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(userControlAlarm))
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonEnd = New System.Windows.Forms.Button()
        Me.TextBoxAlarm = New System.Windows.Forms.TextBox()
        Me.ButtonIgnore = New System.Windows.Forms.Button()
        Me.ButtonRetry = New System.Windows.Forms.Button()
        Me.TimerFlash = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox6.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.TableLayoutPanel1)
        resources.ApplyResources(Me.GroupBox6, "GroupBox6")
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.TabStop = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonEnd, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxAlarm, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonIgnore, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonRetry, 1, 1)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'ButtonEnd
        '
        resources.ApplyResources(Me.ButtonEnd, "ButtonEnd")
        Me.ButtonEnd.Name = "ButtonEnd"
        Me.ButtonEnd.UseVisualStyleBackColor = True
        '
        'TextBoxAlarm
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxAlarm, 3)
        resources.ApplyResources(Me.TextBoxAlarm, "TextBoxAlarm")
        Me.TextBoxAlarm.Name = "TextBoxAlarm"
        Me.TextBoxAlarm.ReadOnly = True
        '
        'ButtonIgnore
        '
        resources.ApplyResources(Me.ButtonIgnore, "ButtonIgnore")
        Me.ButtonIgnore.Name = "ButtonIgnore"
        Me.ButtonIgnore.UseVisualStyleBackColor = True
        '
        'ButtonRetry
        '
        resources.ApplyResources(Me.ButtonRetry, "ButtonRetry")
        Me.ButtonRetry.Name = "ButtonRetry"
        Me.ButtonRetry.UseVisualStyleBackColor = True
        '
        'TimerFlash
        '
        Me.TimerFlash.Interval = 500
        '
        'userControlAlarm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox6)
        Me.Name = "userControlAlarm"
        Me.GroupBox6.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents ButtonEnd As System.Windows.Forms.Button
    Friend WithEvents ButtonIgnore As System.Windows.Forms.Button
    Friend WithEvents ButtonRetry As System.Windows.Forms.Button
    Friend WithEvents TimerFlash As System.Windows.Forms.Timer
    Public WithEvents TextBoxAlarm As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel

End Class