﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlCassette
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(userControlCassette))
        Me.LabelFlash = New System.Windows.Forms.Label()
        Me.TimerFlash = New System.Windows.Forms.Timer(Me.components)
        Me.TimerScan = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBoxTitle = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelSlot = New System.Windows.Forms.TableLayoutPanel()
        Me.PanelFullAndId = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelCassetteId = New System.Windows.Forms.Label()
        Me.ButtonEject = New System.Windows.Forms.Button()
        Me.LabelCount = New System.Windows.Forms.Label()
        Me.GroupBoxTitle.SuspendLayout()
        Me.TableLayoutPanelSlot.SuspendLayout()
        Me.PanelFullAndId.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelFlash
        '
        Me.LabelFlash.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LabelFlash, "LabelFlash")
        Me.LabelFlash.Name = "LabelFlash"
        '
        'TimerFlash
        '
        Me.TimerFlash.Interval = 500
        '
        'TimerScan
        '
        Me.TimerScan.Interval = 200
        '
        'GroupBoxTitle
        '
        resources.ApplyResources(Me.GroupBoxTitle, "GroupBoxTitle")
        Me.GroupBoxTitle.Controls.Add(Me.TableLayoutPanelSlot)
        Me.GroupBoxTitle.Name = "GroupBoxTitle"
        Me.GroupBoxTitle.TabStop = False
        '
        'TableLayoutPanelSlot
        '
        resources.ApplyResources(Me.TableLayoutPanelSlot, "TableLayoutPanelSlot")
        Me.TableLayoutPanelSlot.Controls.Add(Me.PanelFullAndId, 2, 0)
        Me.TableLayoutPanelSlot.Controls.Add(Me.ButtonEject, 0, 0)
        Me.TableLayoutPanelSlot.Controls.Add(Me.LabelCount, 1, 0)
        Me.TableLayoutPanelSlot.Name = "TableLayoutPanelSlot"
        '
        'PanelFullAndId
        '
        resources.ApplyResources(Me.PanelFullAndId, "PanelFullAndId")
        Me.PanelFullAndId.Controls.Add(Me.TableLayoutPanel1)
        Me.PanelFullAndId.Name = "PanelFullAndId"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.LabelFlash, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LabelCassetteId, 0, 1)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'LabelCassetteId
        '
        Me.LabelCassetteId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LabelCassetteId, "LabelCassetteId")
        Me.LabelCassetteId.Name = "LabelCassetteId"
        '
        'ButtonEject
        '
        resources.ApplyResources(Me.ButtonEject, "ButtonEject")
        Me.ButtonEject.Name = "ButtonEject"
        Me.ButtonEject.UseVisualStyleBackColor = True
        '
        'LabelCount
        '
        Me.LabelCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.LabelCount, "LabelCount")
        Me.LabelCount.Name = "LabelCount"
        '
        'userControlCassette
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.GroupBoxTitle)
        Me.Name = "userControlCassette"
        Me.GroupBoxTitle.ResumeLayout(False)
        Me.GroupBoxTitle.PerformLayout()
        Me.TableLayoutPanelSlot.ResumeLayout(False)
        Me.TableLayoutPanelSlot.PerformLayout()
        Me.PanelFullAndId.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LabelFlash As System.Windows.Forms.Label
    Friend WithEvents TimerFlash As System.Windows.Forms.Timer
    Friend WithEvents TimerScan As System.Windows.Forms.Timer
    Friend WithEvents ButtonEject As System.Windows.Forms.Button
    Friend WithEvents LabelCount As System.Windows.Forms.Label
    Friend WithEvents LabelCassetteId As System.Windows.Forms.Label
    Public WithEvents GroupBoxTitle As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanelSlot As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PanelFullAndId As System.Windows.Forms.Panel

End Class
