﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFrameManager1
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
        Me.ComboBoxRItems = New System.Windows.Forms.ComboBox()
        Me.ComboBoxMovingItems = New System.Windows.Forms.ComboBox()
        Me.PropertyGridRItem = New System.Windows.Forms.PropertyGrid()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ComboBoxRItems
        '
        Me.ComboBoxRItems.FormattingEnabled = True
        Me.ComboBoxRItems.Location = New System.Drawing.Point(79, 29)
        Me.ComboBoxRItems.Name = "ComboBoxRItems"
        Me.ComboBoxRItems.Size = New System.Drawing.Size(200, 20)
        Me.ComboBoxRItems.TabIndex = 0
        '
        'ComboBoxMovingItems
        '
        Me.ComboBoxMovingItems.FormattingEnabled = True
        Me.ComboBoxMovingItems.Location = New System.Drawing.Point(79, 3)
        Me.ComboBoxMovingItems.Name = "ComboBoxMovingItems"
        Me.ComboBoxMovingItems.Size = New System.Drawing.Size(200, 20)
        Me.ComboBoxMovingItems.TabIndex = 0
        '
        'PropertyGridRItem
        '
        Me.PropertyGridRItem.HelpVisible = False
        Me.PropertyGridRItem.Location = New System.Drawing.Point(79, 55)
        Me.PropertyGridRItem.Name = "PropertyGridRItem"
        Me.PropertyGridRItem.Size = New System.Drawing.Size(200, 200)
        Me.PropertyGridRItem.TabIndex = 1
        Me.PropertyGridRItem.ToolbarVisible = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.PropertyGridRItem, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboBoxMovingItems, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboBoxRItems, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(282, 258)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Moving Item"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(0, 26)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Reference Item"
        '
        'userControlFrameManager1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "userControlFrameManager1"
        Me.Size = New System.Drawing.Size(288, 264)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ComboBoxRItems As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxMovingItems As System.Windows.Forms.ComboBox
    Friend WithEvents PropertyGridRItem As System.Windows.Forms.PropertyGrid
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label

End Class
