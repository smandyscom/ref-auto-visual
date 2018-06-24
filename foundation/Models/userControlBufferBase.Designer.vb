<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlBufferBase
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
        Me.PropertyGridBuffer = New System.Windows.Forms.PropertyGrid()
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'PropertyGridBuffer
        '
        Me.PropertyGridBuffer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridBuffer.HelpVisible = False
        Me.PropertyGridBuffer.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGridBuffer.Name = "PropertyGridBuffer"
        Me.PropertyGridBuffer.Size = New System.Drawing.Size(150, 150)
        Me.PropertyGridBuffer.TabIndex = 0
        Me.PropertyGridBuffer.ToolbarVisible = False
        '
        'timerScan
        '
        '
        'userControlBufferBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PropertyGridBuffer)
        Me.Name = "userControlBufferBase"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PropertyGridBuffer As System.Windows.Forms.PropertyGrid
    Friend WithEvents timerScan As System.Windows.Forms.Timer

End Class
