﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formMain
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formMain))
        Me.TimerRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.TextBoxTime = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.PictureBoxDeltaSideLogo = New System.Windows.Forms.PictureBox()
        Me.userControlAliveBarMain = New Automation.userControlAliveBar()
        Me.TabPageFirer = New System.Windows.Forms.TabPage()
        Me.UserControl_LightControl1 = New FA.UserControl_LightControl()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TabPageEnginner = New System.Windows.Forms.TabPage()
        Me.TabPageSetting = New System.Windows.Forms.TabPage()
        Me.userControlSettingMain = New FA.userControlSetting()
        Me.TabPageHistory = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.userControlMessageHistory = New Automation.userControlMessage()
        Me.TabPageMainPanel = New System.Windows.Forms.TabPage()
        Me.UserControlMainPanel1 = New FA.userControlMainPanel()
        Me.MainTabControl = New System.Windows.Forms.TabControl()
        Me.TabPageFrameManager = New System.Windows.Forms.TabPage()
        Me.UserControlFrameManagers1 = New FA.userControlFrameManagers()
        Me.Panel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        CType(Me.PictureBoxDeltaSideLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageFirer.SuspendLayout()
        Me.TabPageSetting.SuspendLayout()
        Me.TabPageHistory.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPageMainPanel.SuspendLayout()
        Me.MainTabControl.SuspendLayout()
        Me.TabPageFrameManager.SuspendLayout()
        Me.SuspendLayout()
        '
        'TimerRefresh
        '
        Me.TimerRefresh.Interval = 1000
        '
        'TextBoxTime
        '
        resources.ApplyResources(Me.TextBoxTime, "TextBoxTime")
        Me.TextBoxTime.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxTime.Name = "TextBoxTime"
        Me.TextBoxTime.ReadOnly = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.TableLayoutPanel2)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.PictureBoxDeltaSideLogo, 1, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'PictureBoxDeltaSideLogo
        '
        Me.PictureBoxDeltaSideLogo.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.PictureBoxDeltaSideLogo, "PictureBoxDeltaSideLogo")
        Me.PictureBoxDeltaSideLogo.Name = "PictureBoxDeltaSideLogo"
        Me.PictureBoxDeltaSideLogo.TabStop = False
        '
        'userControlAliveBarMain
        '
        Me.userControlAliveBarMain.assemblyReference = Nothing
        resources.ApplyResources(Me.userControlAliveBarMain, "userControlAliveBarMain")
        Me.userControlAliveBarMain.Name = "userControlAliveBarMain"
        '
        'TabPageFirer
        '
        resources.ApplyResources(Me.TabPageFirer, "TabPageFirer")
        Me.TabPageFirer.Controls.Add(Me.UserControl_LightControl1)
        Me.TabPageFirer.Controls.Add(Me.TableLayoutPanel1)
        Me.TabPageFirer.Name = "TabPageFirer"
        Me.TabPageFirer.UseVisualStyleBackColor = True
        '
        'UserControl_LightControl1
        '
        resources.ApplyResources(Me.UserControl_LightControl1, "UserControl_LightControl1")
        Me.UserControl_LightControl1.Name = "UserControl_LightControl1"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'TabPageEnginner
        '
        resources.ApplyResources(Me.TabPageEnginner, "TabPageEnginner")
        Me.TabPageEnginner.Name = "TabPageEnginner"
        Me.TabPageEnginner.UseVisualStyleBackColor = True
        '
        'TabPageSetting
        '
        resources.ApplyResources(Me.TabPageSetting, "TabPageSetting")
        Me.TabPageSetting.BackColor = System.Drawing.Color.White
        Me.TabPageSetting.Controls.Add(Me.userControlSettingMain)
        Me.TabPageSetting.Name = "TabPageSetting"
        '
        'userControlSettingMain
        '
        Me.userControlSettingMain.BackColor = System.Drawing.SystemColors.Control
        resources.ApplyResources(Me.userControlSettingMain, "userControlSettingMain")
        Me.userControlSettingMain.Name = "userControlSettingMain"
        '
        'TabPageHistory
        '
        Me.TabPageHistory.Controls.Add(Me.GroupBox1)
        resources.ApplyResources(Me.TabPageHistory, "TabPageHistory")
        Me.TabPageHistory.Name = "TabPageHistory"
        Me.TabPageHistory.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.userControlMessageHistory)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'userControlMessageHistory
        '
        resources.ApplyResources(Me.userControlMessageHistory, "userControlMessageHistory")
        Me.userControlMessageHistory.MaxLines = 64
        Me.userControlMessageHistory.messengerReference = Nothing
        Me.userControlMessageHistory.Name = "userControlMessageHistory"
        '
        'TabPageMainPanel
        '
        resources.ApplyResources(Me.TabPageMainPanel, "TabPageMainPanel")
        Me.TabPageMainPanel.Controls.Add(Me.UserControlMainPanel1)
        Me.TabPageMainPanel.Name = "TabPageMainPanel"
        Me.TabPageMainPanel.UseVisualStyleBackColor = True
        '
        'UserControlMainPanel1
        '
        resources.ApplyResources(Me.UserControlMainPanel1, "UserControlMainPanel1")
        Me.UserControlMainPanel1.MaxLines = 8
        Me.UserControlMainPanel1.Name = "UserControlMainPanel1"
        '
        'MainTabControl
        '
        resources.ApplyResources(Me.MainTabControl, "MainTabControl")
        Me.MainTabControl.Controls.Add(Me.TabPageMainPanel)
        Me.MainTabControl.Controls.Add(Me.TabPageHistory)
        Me.MainTabControl.Controls.Add(Me.TabPageSetting)
        Me.MainTabControl.Controls.Add(Me.TabPageEnginner)
        Me.MainTabControl.Controls.Add(Me.TabPageFirer)
        Me.MainTabControl.Controls.Add(Me.TabPageFrameManager)
        Me.MainTabControl.Multiline = True
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        '
        'TabPageFrameManager
        '
        Me.TabPageFrameManager.Controls.Add(Me.UserControlFrameManagers1)
        resources.ApplyResources(Me.TabPageFrameManager, "TabPageFrameManager")
        Me.TabPageFrameManager.Name = "TabPageFrameManager"
        Me.TabPageFrameManager.UseVisualStyleBackColor = True
        '
        'UserControlFrameManagers1
        '
        resources.ApplyResources(Me.UserControlFrameManagers1, "UserControlFrameManagers1")
        Me.UserControlFrameManagers1.Name = "UserControlFrameManagers1"
        '
        'formMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ControlBox = False
        Me.Controls.Add(Me.userControlAliveBarMain)
        Me.Controls.Add(Me.MainTabControl)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TextBoxTime)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MinimizeBox = False
        Me.Name = "formMain"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        CType(Me.PictureBoxDeltaSideLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageFirer.ResumeLayout(False)
        Me.TabPageFirer.PerformLayout()
        Me.TabPageSetting.ResumeLayout(False)
        Me.TabPageHistory.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TabPageMainPanel.ResumeLayout(False)
        Me.TabPageMainPanel.PerformLayout()
        Me.MainTabControl.ResumeLayout(False)
        Me.TabPageFrameManager.ResumeLayout(False)
        Me.TabPageFrameManager.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TimerRefresh As System.Windows.Forms.Timer
    Friend WithEvents TextBoxTime As System.Windows.Forms.TextBox
    Friend WithEvents userControlAliveBarMain As Automation.userControlAliveBar
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PictureBoxDeltaSideLogo As System.Windows.Forms.PictureBox
    Friend WithEvents TabPageFirer As System.Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TabPageEnginner As System.Windows.Forms.TabPage
    Friend WithEvents TabPageSetting As System.Windows.Forms.TabPage
    Friend WithEvents userControlSettingMain As FA.userControlSetting
    Friend WithEvents TabPageHistory As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents userControlMessageHistory As Automation.userControlMessage
    Friend WithEvents TabPageMainPanel As System.Windows.Forms.TabPage
    Friend WithEvents UserControlMainPanel1 As FA.userControlMainPanel
    Friend WithEvents MainTabControl As System.Windows.Forms.TabControl
    Friend WithEvents TabPageFrameManager As System.Windows.Forms.TabPage
    Friend WithEvents UserControlFrameManagers1 As FA.userControlFrameManagers
    Friend WithEvents UserControl_LightControl1 As FA.UserControl_LightControl
    'Friend WithEvents CogToolBlockEditV2_main As Cognex.VisionPro.ToolBlock.CogToolBlockEditV2
End Class
