﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlSystem
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.PropertyGridSystem = New System.Windows.Forms.PropertyGrid()
        Me.TabPageControlFlags = New System.Windows.Forms.TabPage()
        Me.TabPageStatusFlags = New System.Windows.Forms.TabPage()
        Me.TabPageMessage = New System.Windows.Forms.TabPage()
        Me.RichTextBoxMessage = New System.Windows.Forms.RichTextBox()
        Me.GroupBoxSystemName = New System.Windows.Forms.GroupBox()
        Me.UserControlAlarmObject = New Automation.userControlAlarm()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.PanelDevices = New System.Windows.Forms.Panel()
        Me.TableLayoutPanelComponents = New System.Windows.Forms.TableLayoutPanel()
        Me.TimerRefresh = New System.Windows.Forms.Timer()
        Me.GroupBox1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPageMessage.SuspendLayout()
        Me.GroupBoxSystemName.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.PanelDevices.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBox1.Controls.Add(Me.TabControl1)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 21)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(527, 565)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Monitor"
        '
        'TabControl1
        '
        Me.TabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPageControlFlags)
        Me.TabControl1.Controls.Add(Me.TabPageStatusFlags)
        Me.TabControl1.Controls.Add(Me.TabPageMessage)
        Me.TabControl1.Location = New System.Drawing.Point(6, 15)
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(515, 544)
        Me.TabControl1.TabIndex = 14
        '
        'TabPage1
        '
        Me.TabPage1.AutoScroll = True
        Me.TabPage1.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.TabPage1.Controls.Add(Me.PropertyGridSystem)
        Me.TabPage1.Location = New System.Drawing.Point(4, 4)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(507, 518)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Generic"
        '
        'PropertyGridSystem
        '
        Me.PropertyGridSystem.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.PropertyGridSystem.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridSystem.Location = New System.Drawing.Point(3, 3)
        Me.PropertyGridSystem.Name = "PropertyGridSystem"
        Me.PropertyGridSystem.Size = New System.Drawing.Size(501, 512)
        Me.PropertyGridSystem.TabIndex = 0
        '
        'TabPageControlFlags
        '
        Me.TabPageControlFlags.AutoScroll = True
        Me.TabPageControlFlags.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.TabPageControlFlags.Location = New System.Drawing.Point(4, 4)
        Me.TabPageControlFlags.Name = "TabPageControlFlags"
        Me.TabPageControlFlags.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageControlFlags.Size = New System.Drawing.Size(507, 518)
        Me.TabPageControlFlags.TabIndex = 1
        Me.TabPageControlFlags.Text = "ControlFlags"
        '
        'TabPageStatusFlags
        '
        Me.TabPageStatusFlags.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.TabPageStatusFlags.Location = New System.Drawing.Point(4, 4)
        Me.TabPageStatusFlags.Name = "TabPageStatusFlags"
        Me.TabPageStatusFlags.Size = New System.Drawing.Size(507, 518)
        Me.TabPageStatusFlags.TabIndex = 2
        Me.TabPageStatusFlags.Text = "StatusFlags"
        '
        'TabPageMessage
        '
        Me.TabPageMessage.AutoScroll = True
        Me.TabPageMessage.Controls.Add(Me.RichTextBoxMessage)
        Me.TabPageMessage.Location = New System.Drawing.Point(4, 4)
        Me.TabPageMessage.Name = "TabPageMessage"
        Me.TabPageMessage.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageMessage.Size = New System.Drawing.Size(507, 518)
        Me.TabPageMessage.TabIndex = 3
        Me.TabPageMessage.Text = "Message"
        Me.TabPageMessage.UseVisualStyleBackColor = True
        '
        'RichTextBoxMessage
        '
        Me.RichTextBoxMessage.Location = New System.Drawing.Point(7, 3)
        Me.RichTextBoxMessage.Name = "RichTextBoxMessage"
        Me.RichTextBoxMessage.Size = New System.Drawing.Size(494, 512)
        Me.RichTextBoxMessage.TabIndex = 0
        Me.RichTextBoxMessage.Text = ""
        '
        'GroupBoxSystemName
        '
        Me.GroupBoxSystemName.Controls.Add(Me.UserControlAlarmObject)
        Me.GroupBoxSystemName.Controls.Add(Me.GroupBox5)
        Me.GroupBoxSystemName.Controls.Add(Me.GroupBox1)
        Me.GroupBoxSystemName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxSystemName.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxSystemName.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBoxSystemName.Name = "GroupBoxSystemName"
        Me.GroupBoxSystemName.Size = New System.Drawing.Size(982, 592)
        Me.GroupBoxSystemName.TabIndex = 13
        Me.GroupBoxSystemName.TabStop = False
        Me.GroupBoxSystemName.Text = "GroupBoxSystemName"
        '
        'UserControlAlarmObject
        '
        Me.UserControlAlarmObject.AlarmReference = Nothing
        Me.UserControlAlarmObject.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlAlarmObject.Font = New System.Drawing.Font("微軟正黑體", 9.75!)
        Me.UserControlAlarmObject.Location = New System.Drawing.Point(536, 21)
        Me.UserControlAlarmObject.Margin = New System.Windows.Forms.Padding(0)
        Me.UserControlAlarmObject.Name = "UserControlAlarmObject"
        Me.UserControlAlarmObject.Size = New System.Drawing.Size(437, 192)
        Me.UserControlAlarmObject.TabIndex = 17
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.PanelDevices)
        Me.GroupBox5.Location = New System.Drawing.Point(536, 216)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(440, 370)
        Me.GroupBox5.TabIndex = 13
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Devices"
        '
        'PanelDevices
        '
        Me.PanelDevices.AutoScroll = True
        Me.PanelDevices.Controls.Add(Me.TableLayoutPanelComponents)
        Me.PanelDevices.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDevices.Location = New System.Drawing.Point(3, 18)
        Me.PanelDevices.Name = "PanelDevices"
        Me.PanelDevices.Size = New System.Drawing.Size(434, 349)
        Me.PanelDevices.TabIndex = 0
        '
        'TableLayoutPanelComponents
        '
        Me.TableLayoutPanelComponents.AutoScroll = True
        Me.TableLayoutPanelComponents.AutoSize = True
        Me.TableLayoutPanelComponents.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelComponents.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.TableLayoutPanelComponents.ColumnCount = 1
        Me.TableLayoutPanelComponents.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelComponents.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanelComponents.Name = "TableLayoutPanelComponents"
        Me.TableLayoutPanelComponents.RowCount = 1
        Me.TableLayoutPanelComponents.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelComponents.Size = New System.Drawing.Size(2, 2)
        Me.TableLayoutPanelComponents.TabIndex = 0
        '
        'TimerRefresh
        '
        '
        'userControlSystem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.Controls.Add(Me.GroupBoxSystemName)
        Me.Name = "userControlSystem"
        Me.Size = New System.Drawing.Size(982, 592)
        Me.GroupBox1.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPageMessage.ResumeLayout(False)
        Me.GroupBoxSystemName.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.PanelDevices.ResumeLayout(False)
        Me.PanelDevices.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBoxSystemName As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanelComponents As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PanelDevices As System.Windows.Forms.Panel
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPageControlFlags As System.Windows.Forms.TabPage
    Friend WithEvents TabPageStatusFlags As System.Windows.Forms.TabPage
    Friend WithEvents UserControlAlarmObject As Automation.userControlAlarm
    Friend WithEvents TabPageMessage As System.Windows.Forms.TabPage
    Friend WithEvents RichTextBoxMessage As System.Windows.Forms.RichTextBox
    Friend WithEvents PropertyGridSystem As System.Windows.Forms.PropertyGrid
    Friend WithEvents TimerRefresh As System.Windows.Forms.Timer

End Class
