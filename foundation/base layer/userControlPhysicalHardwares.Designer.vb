﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlPhysicalHardwares
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
        Me.DataGridViewList = New System.Windows.Forms.DataGridView()
        Me.TimerScan = New System.Windows.Forms.Timer()
        CType(Me.DataGridViewList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridViewList
        '
        Me.DataGridViewList.AllowUserToAddRows = False
        Me.DataGridViewList.AllowUserToDeleteRows = False
        Me.DataGridViewList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridViewList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DataGridViewList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewList.Location = New System.Drawing.Point(0, 0)
        Me.DataGridViewList.Name = "DataGridViewList"
        Me.DataGridViewList.ReadOnly = True
        Me.DataGridViewList.RowTemplate.Height = 24
        Me.DataGridViewList.Size = New System.Drawing.Size(150, 150)
        Me.DataGridViewList.TabIndex = 0
        '
        'TimerScan
        '
        '
        'userControlPhysicalHardwares
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.DataGridViewList)
        Me.Name = "userControlPhysicalHardwares"
        CType(Me.DataGridViewList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridViewList As System.Windows.Forms.DataGridView
    Friend WithEvents TimerScan As System.Windows.Forms.Timer

End Class