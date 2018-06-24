<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlAnalogMonitor
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.LabelChannelName = New System.Windows.Forms.Label()
        Me.TextBoxValue = New System.Windows.Forms.TextBox()
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.LabelChannelName)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TextBoxValue)
        Me.SplitContainer1.Size = New System.Drawing.Size(150, 20)
        Me.SplitContainer1.SplitterDistance = 53
        Me.SplitContainer1.TabIndex = 0
        '
        'LabelChannelName
        '
        Me.LabelChannelName.AutoSize = True
        Me.LabelChannelName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LabelChannelName.Location = New System.Drawing.Point(0, 0)
        Me.LabelChannelName.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelChannelName.Name = "LabelChannelName"
        Me.LabelChannelName.Size = New System.Drawing.Size(52, 12)
        Me.LabelChannelName.TabIndex = 0
        Me.LabelChannelName.Text = "PD_LEFT"
        '
        'TextBoxValue
        '
        Me.TextBoxValue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxValue.Location = New System.Drawing.Point(0, 0)
        Me.TextBoxValue.Margin = New System.Windows.Forms.Padding(0)
        Me.TextBoxValue.Name = "TextBoxValue"
        Me.TextBoxValue.ReadOnly = True
        Me.TextBoxValue.Size = New System.Drawing.Size(93, 22)
        Me.TextBoxValue.TabIndex = 0
        '
        'timerScan
        '
        '
        'userControlAnalogMonitor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "userControlAnalogMonitor"
        Me.Size = New System.Drawing.Size(150, 20)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents LabelChannelName As System.Windows.Forms.Label
    Friend WithEvents TextBoxValue As System.Windows.Forms.TextBox
    Friend WithEvents timerScan As System.Windows.Forms.Timer

End Class
