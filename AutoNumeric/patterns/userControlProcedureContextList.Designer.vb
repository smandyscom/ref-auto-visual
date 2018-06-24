<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlProcedureContextList
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
        Me.DataGridViewProcedure = New System.Windows.Forms.DataGridView()
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        CType(Me.DataGridViewProcedure, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridViewProcedure
        '
        Me.DataGridViewProcedure.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewProcedure.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewProcedure.Location = New System.Drawing.Point(0, 0)
        Me.DataGridViewProcedure.Name = "DataGridViewProcedure"
        Me.DataGridViewProcedure.RowTemplate.Height = 24
        Me.DataGridViewProcedure.Size = New System.Drawing.Size(642, 593)
        Me.DataGridViewProcedure.TabIndex = 0
        '
        'timerScan
        '
        '
        'userControlProcedureContextList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.DataGridViewProcedure)
        Me.Name = "userControlProcedureContextList"
        Me.Size = New System.Drawing.Size(642, 593)
        CType(Me.DataGridViewProcedure, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridViewProcedure As System.Windows.Forms.DataGridView
    Friend WithEvents timerScan As System.Windows.Forms.Timer

End Class
