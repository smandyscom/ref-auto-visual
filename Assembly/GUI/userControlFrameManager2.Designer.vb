﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFrameManager2
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
        Me.PropertyGridManager = New System.Windows.Forms.PropertyGrid()
        Me.ComboBoxSelection = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelTo = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PropertyGridManager
        '
        Me.PropertyGridManager.HelpVisible = False
        Me.PropertyGridManager.Location = New System.Drawing.Point(86, 29)
        Me.PropertyGridManager.Name = "PropertyGridManager"
        Me.PropertyGridManager.Size = New System.Drawing.Size(200, 200)
        Me.PropertyGridManager.TabIndex = 0
        Me.PropertyGridManager.ToolbarVisible = False
        '
        'ComboBoxSelection
        '
        Me.ComboBoxSelection.FormattingEnabled = True
        Me.ComboBoxSelection.Location = New System.Drawing.Point(86, 3)
        Me.ComboBoxSelection.Name = "ComboBoxSelection"
        Me.ComboBoxSelection.Size = New System.Drawing.Size(200, 20)
        Me.ComboBoxSelection.TabIndex = 2
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.PropertyGridManager, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboBoxSelection, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LabelTo, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(289, 232)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'LabelTo
        '
        Me.LabelTo.AutoSize = True
        Me.LabelTo.Location = New System.Drawing.Point(3, 0)
        Me.LabelTo.Name = "LabelTo"
        Me.LabelTo.Size = New System.Drawing.Size(77, 12)
        Me.LabelTo.TabIndex = 4
        Me.LabelTo.Text = "Transformation"
        '
        'userControlFrameManager2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "userControlFrameManager2"
        Me.Size = New System.Drawing.Size(295, 238)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PropertyGridManager As System.Windows.Forms.PropertyGrid
    Friend WithEvents ComboBoxSelection As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelTo As System.Windows.Forms.Label

End Class
