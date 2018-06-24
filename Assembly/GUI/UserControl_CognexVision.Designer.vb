<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControl_CognexVision
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UserControl_CognexVision))
        Me.CogToolBlockEditV21 = New Cognex.VisionPro.ToolBlock.CogToolBlockEditV2()
        Me.CogDisplay1 = New Cognex.VisionPro.Display.CogDisplay()
        Me.CogRecordDisplay1 = New Cognex.VisionPro.CogRecordDisplay()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Checkbox_showtb = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.CogToolBlockEditV21, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CogDisplay1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CogRecordDisplay1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CogToolBlockEditV21
        '
        Me.CogToolBlockEditV21.AllowDrop = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.CogToolBlockEditV21, 2)
        Me.CogToolBlockEditV21.ContextMenuCustomizer = Nothing
        Me.CogToolBlockEditV21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CogToolBlockEditV21.Location = New System.Drawing.Point(3, 33)
        Me.CogToolBlockEditV21.MinimumSize = New System.Drawing.Size(489, 0)
        Me.CogToolBlockEditV21.Name = "CogToolBlockEditV21"
        Me.CogToolBlockEditV21.ShowNodeToolTips = True
        Me.CogToolBlockEditV21.Size = New System.Drawing.Size(739, 261)
        Me.CogToolBlockEditV21.SuspendElectricRuns = False
        Me.CogToolBlockEditV21.TabIndex = 0
        '
        'CogDisplay1
        '
        Me.CogDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black
        Me.CogDisplay1.ColorMapLowerRoiLimit = 0.0R
        Me.CogDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None
        Me.CogDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black
        Me.CogDisplay1.ColorMapUpperRoiLimit = 1.0R
        Me.CogDisplay1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CogDisplay1.Location = New System.Drawing.Point(3, 300)
        Me.CogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1
        Me.CogDisplay1.MouseWheelSensitivity = 1.0R
        Me.CogDisplay1.Name = "CogDisplay1"
        Me.CogDisplay1.OcxState = CType(resources.GetObject("CogDisplay1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.CogDisplay1.Size = New System.Drawing.Size(366, 264)
        Me.CogDisplay1.TabIndex = 1
        '
        'CogRecordDisplay1
        '
        Me.CogRecordDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black
        Me.CogRecordDisplay1.ColorMapLowerRoiLimit = 0.0R
        Me.CogRecordDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None
        Me.CogRecordDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black
        Me.CogRecordDisplay1.ColorMapUpperRoiLimit = 1.0R
        Me.CogRecordDisplay1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CogRecordDisplay1.Location = New System.Drawing.Point(375, 300)
        Me.CogRecordDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1
        Me.CogRecordDisplay1.MouseWheelSensitivity = 1.0R
        Me.CogRecordDisplay1.Name = "CogRecordDisplay1"
        Me.CogRecordDisplay1.OcxState = CType(resources.GetObject("CogRecordDisplay1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.CogRecordDisplay1.Size = New System.Drawing.Size(367, 264)
        Me.CogRecordDisplay1.TabIndex = 2
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.CogToolBlockEditV21, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Checkbox_showtb, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.CogDisplay1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.CogRecordDisplay1, 1, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(745, 587)
        Me.TableLayoutPanel1.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(533, 571)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(50, 12)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "Processed"
        '
        'Checkbox_showtb
        '
        Me.Checkbox_showtb.AutoSize = True
        Me.Checkbox_showtb.Checked = True
        Me.Checkbox_showtb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Checkbox_showtb.Location = New System.Drawing.Point(3, 3)
        Me.Checkbox_showtb.Name = "Checkbox_showtb"
        Me.Checkbox_showtb.Size = New System.Drawing.Size(101, 16)
        Me.Checkbox_showtb.TabIndex = 0
        Me.Checkbox_showtb.Text = "Show Toolblock"
        Me.Checkbox_showtb.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(146, 571)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 12)
        Me.Label1.TabIndex = 23
        Me.Label1.Text = "Captured Image"
        '
        'UserControl_CognexVision
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "UserControl_CognexVision"
        Me.Size = New System.Drawing.Size(745, 587)
        CType(Me.CogToolBlockEditV21, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CogDisplay1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CogRecordDisplay1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CogToolBlockEditV21 As Cognex.VisionPro.ToolBlock.CogToolBlockEditV2
    Friend WithEvents CogDisplay1 As Cognex.VisionPro.Display.CogDisplay
    Friend WithEvents CogRecordDisplay1 As Cognex.VisionPro.CogRecordDisplay
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Checkbox_showtb As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
