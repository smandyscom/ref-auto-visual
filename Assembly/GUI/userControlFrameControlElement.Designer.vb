﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFrameControlElement
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelName = New System.Windows.Forms.Label()
        Me.TextBoxCurrentValue = New System.Windows.Forms.TextBox()
        Me.TextBoxJogValue = New System.Windows.Forms.TextBox()
        Me.ButtonBackward = New System.Windows.Forms.Button()
        Me.ButtonForward = New System.Windows.Forms.Button()
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        Me.TextBoxRealValue = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 6
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.LabelName, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxCurrentValue, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxJogValue, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonBackward, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonForward, 5, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxRealValue, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(366, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'LabelName
        '
        Me.LabelName.Location = New System.Drawing.Point(3, 0)
        Me.LabelName.Name = "LabelName"
        Me.LabelName.Size = New System.Drawing.Size(50, 22)
        Me.LabelName.TabIndex = 0
        Me.LabelName.Text = "Label1"
        '
        'TextBoxCurrentValue
        '
        Me.TextBoxCurrentValue.Location = New System.Drawing.Point(59, 3)
        Me.TextBoxCurrentValue.Name = "TextBoxCurrentValue"
        Me.TextBoxCurrentValue.ReadOnly = True
        Me.TextBoxCurrentValue.Size = New System.Drawing.Size(60, 22)
        Me.TextBoxCurrentValue.TabIndex = 1
        '
        'TextBoxJogValue
        '
        Me.TextBoxJogValue.Location = New System.Drawing.Point(247, 3)
        Me.TextBoxJogValue.Name = "TextBoxJogValue"
        Me.TextBoxJogValue.Size = New System.Drawing.Size(60, 22)
        Me.TextBoxJogValue.TabIndex = 2
        Me.TextBoxJogValue.Text = "12.34567"
        '
        'ButtonBackward
        '
        Me.ButtonBackward.Location = New System.Drawing.Point(191, 3)
        Me.ButtonBackward.Name = "ButtonBackward"
        Me.ButtonBackward.Size = New System.Drawing.Size(50, 23)
        Me.ButtonBackward.TabIndex = 3
        Me.ButtonBackward.Text = "-"
        Me.ButtonBackward.UseVisualStyleBackColor = True
        '
        'ButtonForward
        '
        Me.ButtonForward.Location = New System.Drawing.Point(313, 3)
        Me.ButtonForward.Name = "ButtonForward"
        Me.ButtonForward.Size = New System.Drawing.Size(50, 23)
        Me.ButtonForward.TabIndex = 4
        Me.ButtonForward.Text = "+"
        Me.ButtonForward.UseVisualStyleBackColor = True
        '
        'timerScan
        '
        '
        'TextBoxRealValue
        '
        Me.TextBoxRealValue.Location = New System.Drawing.Point(125, 3)
        Me.TextBoxRealValue.Name = "TextBoxRealValue"
        Me.TextBoxRealValue.ReadOnly = True
        Me.TextBoxRealValue.Size = New System.Drawing.Size(60, 22)
        Me.TextBoxRealValue.TabIndex = 5
        Me.TextBoxRealValue.Text = "12.34567"
        '
        'userControlFrameControlElement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "userControlFrameControlElement"
        Me.Size = New System.Drawing.Size(372, 35)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelName As System.Windows.Forms.Label
    Friend WithEvents TextBoxCurrentValue As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxJogValue As System.Windows.Forms.TextBox
    Friend WithEvents ButtonBackward As System.Windows.Forms.Button
    Friend WithEvents ButtonForward As System.Windows.Forms.Button
    Friend WithEvents timerScan As System.Windows.Forms.Timer
    Friend WithEvents TextBoxRealValue As System.Windows.Forms.TextBox

End Class
