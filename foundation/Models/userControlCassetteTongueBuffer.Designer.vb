<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlCassetteTongueBuffer
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
        Me.timerScan = New System.Windows.Forms.Timer(Me.components)
        Me.PanelCassette = New System.Windows.Forms.Panel()
        Me.TableLayoutPanelTongueBuffer = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelBufferCount = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.UserControlLane1 = New Automation.userControlLane()
        Me.UserControlCassette1 = New Automation.userControlCassette()
        Me.PanelCassette.SuspendLayout()
        Me.TableLayoutPanelTongueBuffer.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'timerScan
        '
        '
        'PanelCassette
        '
        Me.PanelCassette.AutoSize = True
        Me.PanelCassette.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.PanelCassette.Controls.Add(Me.UserControlCassette1)
        Me.PanelCassette.Location = New System.Drawing.Point(0, 0)
        Me.PanelCassette.Margin = New System.Windows.Forms.Padding(0)
        Me.PanelCassette.Name = "PanelCassette"
        Me.PanelCassette.Size = New System.Drawing.Size(230, 60)
        Me.PanelCassette.TabIndex = 4
        '
        'TableLayoutPanelTongueBuffer
        '
        Me.TableLayoutPanelTongueBuffer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelTongueBuffer.AutoSize = True
        Me.TableLayoutPanelTongueBuffer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelTongueBuffer.ColumnCount = 1
        Me.TableLayoutPanelTongueBuffer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelTongueBuffer.Controls.Add(Me.LabelBufferCount, 0, 0)
        Me.TableLayoutPanelTongueBuffer.Controls.Add(Me.UserControlLane1, 0, 1)
        Me.TableLayoutPanelTongueBuffer.Location = New System.Drawing.Point(230, 0)
        Me.TableLayoutPanelTongueBuffer.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanelTongueBuffer.Name = "TableLayoutPanelTongueBuffer"
        Me.TableLayoutPanelTongueBuffer.RowCount = 2
        Me.TableLayoutPanelTongueBuffer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelTongueBuffer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelTongueBuffer.Size = New System.Drawing.Size(27, 60)
        Me.TableLayoutPanelTongueBuffer.TabIndex = 5
        '
        'LabelBufferCount
        '
        Me.LabelBufferCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelBufferCount.AutoSize = True
        Me.LabelBufferCount.Location = New System.Drawing.Point(13, 0)
        Me.LabelBufferCount.Name = "LabelBufferCount"
        Me.LabelBufferCount.Size = New System.Drawing.Size(11, 12)
        Me.LabelBufferCount.TabIndex = 2
        Me.LabelBufferCount.Text = "0"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanelTongueBuffer, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PanelCassette, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(257, 60)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'UserControlLane1
        '
        Me.UserControlLane1.AutoSize = True
        Me.UserControlLane1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlLane1.BackColor = System.Drawing.SystemColors.Control
        Me.UserControlLane1.elementNumber = 1
        Me.UserControlLane1.IsElementModuleActionVisualizing = True
        Me.UserControlLane1.IsLaneModuleActionVisual = False
        Me.UserControlLane1.IsLaneOccupiedVisual = False
        Me.UserControlLane1.IsMirror = False
        Me.UserControlLane1.Location = New System.Drawing.Point(0, 12)
        Me.UserControlLane1.Margin = New System.Windows.Forms.Padding(0)
        Me.UserControlLane1.Name = "UserControlLane1"
        Me.UserControlLane1.Size = New System.Drawing.Size(27, 32)
        Me.UserControlLane1.TabIndex = 1
        '
        'UserControlCassette1
        '
        Me.UserControlCassette1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.UserControlCassette1.CassetteReference = Nothing
        Me.UserControlCassette1.Font = New System.Drawing.Font("新細明體", 8.25!)
        Me.UserControlCassette1.Location = New System.Drawing.Point(0, 0)
        Me.UserControlCassette1.Margin = New System.Windows.Forms.Padding(0)
        Me.UserControlCassette1.Name = "UserControlCassette1"
        Me.UserControlCassette1.Size = New System.Drawing.Size(230, 60)
        Me.UserControlCassette1.TabIndex = 0
        '
        'userControlCassetteTongueBuffer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "userControlCassetteTongueBuffer"
        Me.Size = New System.Drawing.Size(257, 60)
        Me.PanelCassette.ResumeLayout(False)
        Me.TableLayoutPanelTongueBuffer.ResumeLayout(False)
        Me.TableLayoutPanelTongueBuffer.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents timerScan As System.Windows.Forms.Timer
    Friend WithEvents PanelCassette As System.Windows.Forms.Panel
    Friend WithEvents UserControlCassette1 As Automation.userControlCassette
    Friend WithEvents TableLayoutPanelTongueBuffer As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelBufferCount As System.Windows.Forms.Label
    Friend WithEvents UserControlLane1 As Automation.userControlLane
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel

End Class
