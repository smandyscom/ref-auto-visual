﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlMainPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(userControlMainPanel))
        Me.TimerFlash = New System.Windows.Forms.Timer(Me.components)
        Me.TimerRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonShutdown = New System.Windows.Forms.Button()
        Me.ButtonPause = New System.Windows.Forms.Button()
        Me.ButtonCalibration = New System.Windows.Forms.Button()
        Me.fraMainMsg = New System.Windows.Forms.GroupBox()
        Me.UserControlMessageMainPanel = New Automation.userControlMessage()
        Me.GroupBoxOperation = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonIgnite = New System.Windows.Forms.Button()
        Me.ButtonPointTeach = New System.Windows.Forms.Button()
        Me.ButtonFinish = New System.Windows.Forms.Button()
        Me.ButtonBonding = New System.Windows.Forms.Button()
        Me.GroupBoxInfor = New System.Windows.Forms.GroupBox()
        Me.DataGridViewErrorMatrix = New System.Windows.Forms.DataGridView()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelBondMaterial = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonGRIPVAC = New System.Windows.Forms.Button()
        Me.ButtonGRIPOPEN = New System.Windows.Forms.Button()
        Me.ButtonDisp = New System.Windows.Forms.Button()
        Me.ButtonPdEnable = New System.Windows.Forms.Button()
        Me.ButtonLdr1 = New System.Windows.Forms.Button()
        Me.ButtonLdr2 = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelAnalog = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.UserControlChannelDataLeft = New FA.userControlChannelData()
        Me.UserControlChannelDataRight = New FA.userControlChannelData()
        Me.ButtonSyringe = New System.Windows.Forms.Button()
        Me.fraMainMsg.SuspendLayout()
        Me.GroupBoxOperation.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupBoxInfor.SuspendLayout()
        CType(Me.DataGridViewErrorMatrix, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'TimerFlash
        '
        Me.TimerFlash.Interval = 1000
        '
        'TimerRefresh
        '
        '
        'ButtonShutdown
        '
        Me.ButtonShutdown.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonShutdown.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonShutdown, "ButtonShutdown")
        Me.ButtonShutdown.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonShutdown.Name = "ButtonShutdown"
        Me.ButtonShutdown.UseVisualStyleBackColor = False
        '
        'ButtonPause
        '
        Me.ButtonPause.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonPause.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonPause, "ButtonPause")
        Me.ButtonPause.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonPause.Name = "ButtonPause"
        Me.ButtonPause.UseVisualStyleBackColor = False
        '
        'ButtonCalibration
        '
        Me.ButtonCalibration.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonCalibration.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonCalibration, "ButtonCalibration")
        Me.ButtonCalibration.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonCalibration.Name = "ButtonCalibration"
        Me.ButtonCalibration.UseVisualStyleBackColor = False
        '
        'fraMainMsg
        '
        Me.fraMainMsg.BackColor = System.Drawing.SystemColors.Control
        Me.fraMainMsg.Controls.Add(Me.UserControlMessageMainPanel)
        resources.ApplyResources(Me.fraMainMsg, "fraMainMsg")
        Me.fraMainMsg.ForeColor = System.Drawing.Color.Black
        Me.fraMainMsg.Name = "fraMainMsg"
        Me.fraMainMsg.TabStop = False
        '
        'UserControlMessageMainPanel
        '
        resources.ApplyResources(Me.UserControlMessageMainPanel, "UserControlMessageMainPanel")
        Me.UserControlMessageMainPanel.MaxLines = 64
        Me.UserControlMessageMainPanel.messengerReference = Nothing
        Me.UserControlMessageMainPanel.Name = "UserControlMessageMainPanel"
        '
        'GroupBoxOperation
        '
        Me.GroupBoxOperation.Controls.Add(Me.TableLayoutPanel2)
        resources.ApplyResources(Me.GroupBoxOperation, "GroupBoxOperation")
        Me.GroupBoxOperation.Name = "GroupBoxOperation"
        Me.GroupBoxOperation.TabStop = False
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonIgnite, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonPause, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonShutdown, 0, 6)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonPointTeach, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonFinish, 0, 5)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonBonding, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.ButtonCalibration, 0, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'ButtonIgnite
        '
        Me.ButtonIgnite.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonIgnite.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonIgnite, "ButtonIgnite")
        Me.ButtonIgnite.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonIgnite.Name = "ButtonIgnite"
        Me.ButtonIgnite.UseVisualStyleBackColor = False
        '
        'ButtonPointTeach
        '
        resources.ApplyResources(Me.ButtonPointTeach, "ButtonPointTeach")
        Me.ButtonPointTeach.Name = "ButtonPointTeach"
        Me.ButtonPointTeach.UseVisualStyleBackColor = True
        '
        'ButtonFinish
        '
        Me.ButtonFinish.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonFinish.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonFinish, "ButtonFinish")
        Me.ButtonFinish.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonFinish.Name = "ButtonFinish"
        Me.ButtonFinish.UseVisualStyleBackColor = False
        '
        'ButtonBonding
        '
        Me.ButtonBonding.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonBonding.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.ButtonBonding, "ButtonBonding")
        Me.ButtonBonding.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonBonding.Name = "ButtonBonding"
        Me.ButtonBonding.UseVisualStyleBackColor = False
        '
        'GroupBoxInfor
        '
        resources.ApplyResources(Me.GroupBoxInfor, "GroupBoxInfor")
        Me.GroupBoxInfor.Controls.Add(Me.DataGridViewErrorMatrix)
        Me.GroupBoxInfor.Name = "GroupBoxInfor"
        Me.GroupBoxInfor.TabStop = False
        '
        'DataGridViewErrorMatrix
        '
        Me.DataGridViewErrorMatrix.AllowUserToAddRows = False
        Me.DataGridViewErrorMatrix.AllowUserToDeleteRows = False
        Me.DataGridViewErrorMatrix.AllowUserToResizeColumns = False
        Me.DataGridViewErrorMatrix.AllowUserToResizeRows = False
        Me.DataGridViewErrorMatrix.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.DataGridViewErrorMatrix.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DataGridViewErrorMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.DataGridViewErrorMatrix, "DataGridViewErrorMatrix")
        Me.DataGridViewErrorMatrix.Name = "DataGridViewErrorMatrix"
        Me.DataGridViewErrorMatrix.RowTemplate.Height = 24
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanelBondMaterial)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'TableLayoutPanelBondMaterial
        '
        resources.ApplyResources(Me.TableLayoutPanelBondMaterial, "TableLayoutPanelBondMaterial")
        Me.TableLayoutPanelBondMaterial.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns
        Me.TableLayoutPanelBondMaterial.Name = "TableLayoutPanelBondMaterial"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TableLayoutPanel1)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonSyringe, 0, 6)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonGRIPVAC, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonGRIPOPEN, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonDisp, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonPdEnable, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonLdr1, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonLdr2, 0, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'ButtonGRIPVAC
        '
        resources.ApplyResources(Me.ButtonGRIPVAC, "ButtonGRIPVAC")
        Me.ButtonGRIPVAC.Name = "ButtonGRIPVAC"
        Me.ButtonGRIPVAC.UseVisualStyleBackColor = True
        '
        'ButtonGRIPOPEN
        '
        resources.ApplyResources(Me.ButtonGRIPOPEN, "ButtonGRIPOPEN")
        Me.ButtonGRIPOPEN.Name = "ButtonGRIPOPEN"
        Me.ButtonGRIPOPEN.UseVisualStyleBackColor = True
        '
        'ButtonDisp
        '
        resources.ApplyResources(Me.ButtonDisp, "ButtonDisp")
        Me.ButtonDisp.Name = "ButtonDisp"
        Me.ButtonDisp.UseVisualStyleBackColor = True
        '
        'ButtonPdEnable
        '
        resources.ApplyResources(Me.ButtonPdEnable, "ButtonPdEnable")
        Me.ButtonPdEnable.Name = "ButtonPdEnable"
        Me.ButtonPdEnable.UseVisualStyleBackColor = True
        '
        'ButtonLdr1
        '
        resources.ApplyResources(Me.ButtonLdr1, "ButtonLdr1")
        Me.ButtonLdr1.Name = "ButtonLdr1"
        Me.ButtonLdr1.UseVisualStyleBackColor = True
        '
        'ButtonLdr2
        '
        resources.ApplyResources(Me.ButtonLdr2, "ButtonLdr2")
        Me.ButtonLdr2.Name = "ButtonLdr2"
        Me.ButtonLdr2.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Controls.Add(Me.TableLayoutPanelAnalog)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'TableLayoutPanelAnalog
        '
        resources.ApplyResources(Me.TableLayoutPanelAnalog, "TableLayoutPanelAnalog")
        Me.TableLayoutPanelAnalog.Name = "TableLayoutPanelAnalog"
        '
        'TableLayoutPanel3
        '
        resources.ApplyResources(Me.TableLayoutPanel3, "TableLayoutPanel3")
        Me.TableLayoutPanel3.Controls.Add(Me.UserControlChannelDataLeft, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.UserControlChannelDataRight, 1, 0)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        '
        'UserControlChannelDataLeft
        '
        resources.ApplyResources(Me.UserControlChannelDataLeft, "UserControlChannelDataLeft")
        Me.UserControlChannelDataLeft.Name = "UserControlChannelDataLeft"
        '
        'UserControlChannelDataRight
        '
        resources.ApplyResources(Me.UserControlChannelDataRight, "UserControlChannelDataRight")
        Me.UserControlChannelDataRight.Name = "UserControlChannelDataRight"
        '
        'ButtonSyringe
        '
        resources.ApplyResources(Me.ButtonSyringe, "ButtonSyringe")
        Me.ButtonSyringe.Name = "ButtonSyringe"
        Me.ButtonSyringe.UseVisualStyleBackColor = True
        '
        'userControlMainPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel3)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBoxInfor)
        Me.Controls.Add(Me.GroupBoxOperation)
        Me.Controls.Add(Me.fraMainMsg)
        Me.Name = "userControlMainPanel"
        Me.fraMainMsg.ResumeLayout(False)
        Me.GroupBoxOperation.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.GroupBoxInfor.ResumeLayout(False)
        CType(Me.DataGridViewErrorMatrix, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TimerFlash As System.Windows.Forms.Timer
    Friend WithEvents TimerRefresh As System.Windows.Forms.Timer
    Public WithEvents ButtonShutdown As System.Windows.Forms.Button
    Public WithEvents ButtonPause As System.Windows.Forms.Button
    Public WithEvents ButtonCalibration As System.Windows.Forms.Button
    Public WithEvents fraMainMsg As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBoxOperation As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBoxInfor As System.Windows.Forms.GroupBox
    Friend WithEvents UserControlMessageMainPanel As Automation.userControlMessage
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonPointTeach As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonGRIPVAC As System.Windows.Forms.Button
    Friend WithEvents ButtonGRIPOPEN As System.Windows.Forms.Button
    Friend WithEvents ButtonDisp As System.Windows.Forms.Button
    Friend WithEvents ButtonPdEnable As System.Windows.Forms.Button
    Friend WithEvents DataGridViewErrorMatrix As System.Windows.Forms.DataGridView
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanelAnalog As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanelBondMaterial As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonLdr1 As System.Windows.Forms.Button
    Friend WithEvents ButtonLdr2 As System.Windows.Forms.Button
    Public WithEvents ButtonBonding As System.Windows.Forms.Button
    Public WithEvents ButtonFinish As System.Windows.Forms.Button
    Public WithEvents ButtonIgnite As System.Windows.Forms.Button
    Friend WithEvents UserControlChannelDataLeft As FA.userControlChannelData
    Friend WithEvents UserControlChannelDataRight As FA.userControlChannelData
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonSyringe As System.Windows.Forms.Button

End Class
