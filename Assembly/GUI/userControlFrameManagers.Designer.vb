﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFrameManagers
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
        Me.UserControlFrameManager11 = New FA.userControlFrameManager1()
        Me.UserControlFrameManager21 = New FA.userControlFrameManager2()
        Me.GroupBoxAxisControl = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.UserControlFrameControlElementX = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementY = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementZ = New FA.userControlFrameControlElement()
        Me.GroupBoxSmarpod = New System.Windows.Forms.GroupBox()
        Me.ButtonLoadPose = New System.Windows.Forms.Button()
        Me.ButtonSafePose = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.UserControlFrameControlElementSX = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementSY = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementSZ = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementSA = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementSB = New FA.userControlFrameControlElement()
        Me.UserControlFrameControlElementSC = New FA.userControlFrameControlElement()
        Me.DataGridViewCoordinates = New System.Windows.Forms.DataGridView()
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBoxAxisControl.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBoxSmarpod.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        CType(Me.DataGridViewCoordinates, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UserControlFrameManager11
        '
        Me.UserControlFrameManager11.AutoSize = True
        Me.UserControlFrameManager11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameManager11.Location = New System.Drawing.Point(3, 3)
        Me.UserControlFrameManager11.Name = "UserControlFrameManager11"
        Me.UserControlFrameManager11.Size = New System.Drawing.Size(288, 264)
        Me.UserControlFrameManager11.TabIndex = 0
        '
        'UserControlFrameManager21
        '
        Me.UserControlFrameManager21.AutoSize = True
        Me.UserControlFrameManager21.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameManager21.Location = New System.Drawing.Point(-4, 269)
        Me.UserControlFrameManager21.Name = "UserControlFrameManager21"
        Me.UserControlFrameManager21.Size = New System.Drawing.Size(295, 238)
        Me.UserControlFrameManager21.TabIndex = 1
        '
        'GroupBoxAxisControl
        '
        Me.GroupBoxAxisControl.AutoSize = True
        Me.GroupBoxAxisControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxAxisControl.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBoxAxisControl.Location = New System.Drawing.Point(297, 69)
        Me.GroupBoxAxisControl.Name = "GroupBoxAxisControl"
        Me.GroupBoxAxisControl.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBoxAxisControl.Size = New System.Drawing.Size(381, 156)
        Me.GroupBoxAxisControl.TabIndex = 2
        Me.GroupBoxAxisControl.TabStop = False
        Me.GroupBoxAxisControl.Text = "Axis Value"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.UserControlFrameControlElementX, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.UserControlFrameControlElementY, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.UserControlFrameControlElementZ, 0, 2)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 18)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(378, 123)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'UserControlFrameControlElementX
        '
        Me.UserControlFrameControlElementX.AutoSize = True
        Me.UserControlFrameControlElementX.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementX.Location = New System.Drawing.Point(3, 3)
        Me.UserControlFrameControlElementX.Name = "UserControlFrameControlElementX"
        Me.UserControlFrameControlElementX.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementX.TabIndex = 0
        '
        'UserControlFrameControlElementY
        '
        Me.UserControlFrameControlElementY.AutoSize = True
        Me.UserControlFrameControlElementY.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementY.Location = New System.Drawing.Point(3, 44)
        Me.UserControlFrameControlElementY.Name = "UserControlFrameControlElementY"
        Me.UserControlFrameControlElementY.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementY.TabIndex = 1
        '
        'UserControlFrameControlElementZ
        '
        Me.UserControlFrameControlElementZ.AutoSize = True
        Me.UserControlFrameControlElementZ.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementZ.Location = New System.Drawing.Point(3, 85)
        Me.UserControlFrameControlElementZ.Name = "UserControlFrameControlElementZ"
        Me.UserControlFrameControlElementZ.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementZ.TabIndex = 2
        '
        'GroupBoxSmarpod
        '
        Me.GroupBoxSmarpod.AutoSize = True
        Me.GroupBoxSmarpod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxSmarpod.Controls.Add(Me.ButtonLoadPose)
        Me.GroupBoxSmarpod.Controls.Add(Me.ButtonSafePose)
        Me.GroupBoxSmarpod.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupBoxSmarpod.Location = New System.Drawing.Point(297, 228)
        Me.GroupBoxSmarpod.Name = "GroupBoxSmarpod"
        Me.GroupBoxSmarpod.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBoxSmarpod.Size = New System.Drawing.Size(381, 279)
        Me.GroupBoxSmarpod.TabIndex = 3
        Me.GroupBoxSmarpod.TabStop = False
        Me.GroupBoxSmarpod.Text = "Smarpod Control"
        '
        'ButtonLoadPose
        '
        Me.ButtonLoadPose.Location = New System.Drawing.Point(208, 0)
        Me.ButtonLoadPose.Name = "ButtonLoadPose"
        Me.ButtonLoadPose.Size = New System.Drawing.Size(75, 23)
        Me.ButtonLoadPose.TabIndex = 6
        Me.ButtonLoadPose.Text = "Load Pose"
        Me.ButtonLoadPose.UseVisualStyleBackColor = True
        '
        'ButtonSafePose
        '
        Me.ButtonSafePose.Location = New System.Drawing.Point(127, 0)
        Me.ButtonSafePose.Name = "ButtonSafePose"
        Me.ButtonSafePose.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSafePose.TabIndex = 5
        Me.ButtonSafePose.Text = "Safe Pose"
        Me.ButtonSafePose.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoSize = True
        Me.TableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSX, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSY, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSZ, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSA, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSB, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.UserControlFrameControlElementSC, 0, 5)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 18)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 6
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(378, 246)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'UserControlFrameControlElementSX
        '
        Me.UserControlFrameControlElementSX.AutoSize = True
        Me.UserControlFrameControlElementSX.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSX.Location = New System.Drawing.Point(3, 3)
        Me.UserControlFrameControlElementSX.Name = "UserControlFrameControlElementSX"
        Me.UserControlFrameControlElementSX.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSX.TabIndex = 0
        '
        'UserControlFrameControlElementSY
        '
        Me.UserControlFrameControlElementSY.AutoSize = True
        Me.UserControlFrameControlElementSY.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSY.Location = New System.Drawing.Point(3, 44)
        Me.UserControlFrameControlElementSY.Name = "UserControlFrameControlElementSY"
        Me.UserControlFrameControlElementSY.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSY.TabIndex = 1
        '
        'UserControlFrameControlElementSZ
        '
        Me.UserControlFrameControlElementSZ.AutoSize = True
        Me.UserControlFrameControlElementSZ.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSZ.Location = New System.Drawing.Point(3, 85)
        Me.UserControlFrameControlElementSZ.Name = "UserControlFrameControlElementSZ"
        Me.UserControlFrameControlElementSZ.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSZ.TabIndex = 2
        '
        'UserControlFrameControlElementSA
        '
        Me.UserControlFrameControlElementSA.AutoSize = True
        Me.UserControlFrameControlElementSA.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSA.Location = New System.Drawing.Point(3, 126)
        Me.UserControlFrameControlElementSA.Name = "UserControlFrameControlElementSA"
        Me.UserControlFrameControlElementSA.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSA.TabIndex = 3
        '
        'UserControlFrameControlElementSB
        '
        Me.UserControlFrameControlElementSB.AutoSize = True
        Me.UserControlFrameControlElementSB.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSB.Location = New System.Drawing.Point(3, 167)
        Me.UserControlFrameControlElementSB.Name = "UserControlFrameControlElementSB"
        Me.UserControlFrameControlElementSB.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSB.TabIndex = 4
        '
        'UserControlFrameControlElementSC
        '
        Me.UserControlFrameControlElementSC.AutoSize = True
        Me.UserControlFrameControlElementSC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlFrameControlElementSC.Location = New System.Drawing.Point(3, 208)
        Me.UserControlFrameControlElementSC.Name = "UserControlFrameControlElementSC"
        Me.UserControlFrameControlElementSC.Size = New System.Drawing.Size(372, 35)
        Me.UserControlFrameControlElementSC.TabIndex = 5
        '
        'DataGridViewCoordinates
        '
        Me.DataGridViewCoordinates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewCoordinates.Location = New System.Drawing.Point(681, 3)
        Me.DataGridViewCoordinates.Name = "DataGridViewCoordinates"
        Me.DataGridViewCoordinates.RowTemplate.Height = 24
        Me.DataGridViewCoordinates.Size = New System.Drawing.Size(566, 814)
        Me.DataGridViewCoordinates.TabIndex = 5
        '
        'timerScan
        '
        '
        'userControlFrameManagers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.DataGridViewCoordinates)
        Me.Controls.Add(Me.GroupBoxSmarpod)
        Me.Controls.Add(Me.GroupBoxAxisControl)
        Me.Controls.Add(Me.UserControlFrameManager21)
        Me.Controls.Add(Me.UserControlFrameManager11)
        Me.Name = "userControlFrameManagers"
        Me.Size = New System.Drawing.Size(1250, 820)
        Me.GroupBoxAxisControl.ResumeLayout(False)
        Me.GroupBoxAxisControl.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBoxSmarpod.ResumeLayout(False)
        Me.GroupBoxSmarpod.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        CType(Me.DataGridViewCoordinates, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UserControlFrameManager11 As FA.userControlFrameManager1
    Friend WithEvents UserControlFrameManager21 As FA.userControlFrameManager2
    Friend WithEvents GroupBoxAxisControl As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents UserControlFrameControlElementX As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementY As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementZ As FA.userControlFrameControlElement
    Friend WithEvents GroupBoxSmarpod As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents UserControlFrameControlElementSX As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementSY As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementSZ As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementSA As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementSB As FA.userControlFrameControlElement
    Friend WithEvents UserControlFrameControlElementSC As FA.userControlFrameControlElement
    Friend WithEvents ButtonSafePose As System.Windows.Forms.Button
    Friend WithEvents ButtonLoadPose As System.Windows.Forms.Button
    Friend WithEvents DataGridViewCoordinates As System.Windows.Forms.DataGridView
    Friend WithEvents timerScan As System.Windows.Forms.Timer

End Class
