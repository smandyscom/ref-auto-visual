<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlFlags
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
        Me.TableLayoutPanelFlagControl = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBoxFlags = New System.Windows.Forms.GroupBox()
        Me.GroupBoxFlags.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanelFlagControl
        '
        Me.TableLayoutPanelFlagControl.AutoScroll = True
        Me.TableLayoutPanelFlagControl.AutoSize = True
        Me.TableLayoutPanelFlagControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelFlagControl.ColumnCount = 1
        Me.TableLayoutPanelFlagControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelFlagControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelFlagControl.Location = New System.Drawing.Point(6, 21)
        Me.TableLayoutPanelFlagControl.Name = "TableLayoutPanelFlagControl"
        Me.TableLayoutPanelFlagControl.RowCount = 1
        Me.TableLayoutPanelFlagControl.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelFlagControl.Size = New System.Drawing.Size(0, 0)
        Me.TableLayoutPanelFlagControl.TabIndex = 0
        '
        'GroupBoxFlags
        '
        Me.GroupBoxFlags.AutoSize = True
        Me.GroupBoxFlags.Controls.Add(Me.TableLayoutPanelFlagControl)
        Me.GroupBoxFlags.Location = New System.Drawing.Point(3, 3)
        Me.GroupBoxFlags.Name = "GroupBoxFlags"
        Me.GroupBoxFlags.Size = New System.Drawing.Size(300, 300)
        Me.GroupBoxFlags.TabIndex = 1
        Me.GroupBoxFlags.TabStop = False
        Me.GroupBoxFlags.Text = "Flags Controller"
        '
        'userControlFlags
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.Controls.Add(Me.GroupBoxFlags)
        Me.MaximumSize = New System.Drawing.Size(310, 500)
        Me.Name = "userControlFlags"
        Me.Size = New System.Drawing.Size(306, 306)
        Me.GroupBoxFlags.ResumeLayout(False)
        Me.GroupBoxFlags.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanelFlagControl As System.Windows.Forms.TableLayoutPanel
    Public WithEvents GroupBoxFlags As System.Windows.Forms.GroupBox

End Class
