﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlHardwareNode
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LabelHardwareCode = New System.Windows.Forms.Label()
        Me.LabelStatus = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TimerScan = New System.Windows.Forms.Timer(Me.components)
        Me.ComboBoxHardwareType = New System.Windows.Forms.ComboBox()
        Me.PropertyGridHardwareDetail = New System.Windows.Forms.PropertyGrid()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 1)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(90, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Hardware Code："
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 14)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Status："
        '
        'LabelHardwareCode
        '
        Me.LabelHardwareCode.AutoSize = True
        Me.LabelHardwareCode.Location = New System.Drawing.Point(101, 1)
        Me.LabelHardwareCode.Name = "LabelHardwareCode"
        Me.LabelHardwareCode.Size = New System.Drawing.Size(37, 12)
        Me.LabelHardwareCode.TabIndex = 2
        Me.LabelHardwareCode.Text = "Label3"
        '
        'LabelStatus
        '
        Me.LabelStatus.AutoSize = True
        Me.LabelStatus.Location = New System.Drawing.Point(101, 14)
        Me.LabelStatus.Name = "LabelStatus"
        Me.LabelStatus.Size = New System.Drawing.Size(37, 12)
        Me.LabelStatus.TabIndex = 3
        Me.LabelStatus.Text = "Label4"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LabelStatus, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LabelHardwareCode, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 26)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(142, 27)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'TimerScan
        '
        Me.TimerScan.Interval = 250
        '
        'ComboBoxHardwareType
        '
        Me.ComboBoxHardwareType.FormattingEnabled = True
        Me.ComboBoxHardwareType.Location = New System.Drawing.Point(2, 4)
        Me.ComboBoxHardwareType.Name = "ComboBoxHardwareType"
        Me.ComboBoxHardwareType.Size = New System.Drawing.Size(201, 20)
        Me.ComboBoxHardwareType.TabIndex = 5
        '
        'PropertyGridHardwareDetail
        '
        Me.PropertyGridHardwareDetail.Location = New System.Drawing.Point(3, 55)
        Me.PropertyGridHardwareDetail.Name = "PropertyGridHardwareDetail"
        Me.PropertyGridHardwareDetail.Size = New System.Drawing.Size(200, 200)
        Me.PropertyGridHardwareDetail.TabIndex = 6
        '
        'userControlHardware
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.PropertyGridHardwareDetail)
        Me.Controls.Add(Me.ComboBoxHardwareType)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "userControlHardware"
        Me.Size = New System.Drawing.Size(206, 258)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LabelHardwareCode As System.Windows.Forms.Label
    Friend WithEvents LabelStatus As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TimerScan As System.Windows.Forms.Timer
    Friend WithEvents ComboBoxHardwareType As System.Windows.Forms.ComboBox
    Friend WithEvents PropertyGridHardwareDetail As System.Windows.Forms.PropertyGrid

End Class
