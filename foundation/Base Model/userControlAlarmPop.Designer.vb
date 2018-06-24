<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlAlarmPop
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(userControlAlarmPop))
        Me.checkBoxBuzzer = New System.Windows.Forms.CheckBox()
        Me.GroupBoxArea = New System.Windows.Forms.GroupBox()
        Me.buttonOption3 = New System.Windows.Forms.Button()
        Me.ButtonAbort = New System.Windows.Forms.Button()
        Me.ButtonIgnore = New System.Windows.Forms.Button()
        Me.ButtonRetry = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblErrorTitle = New System.Windows.Forms.Label()
        Me.TextBoxAlarmMessage = New System.Windows.Forms.TextBox()
        Me.ButtonHide = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'checkBoxBuzzer
        '
        Me.checkBoxBuzzer.AutoSize = True
        Me.checkBoxBuzzer.Font = New System.Drawing.Font("PMingLiU", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.checkBoxBuzzer.Location = New System.Drawing.Point(419, 39)
        Me.checkBoxBuzzer.Name = "checkBoxBuzzer"
        Me.checkBoxBuzzer.Size = New System.Drawing.Size(79, 23)
        Me.checkBoxBuzzer.TabIndex = 30
        Me.checkBoxBuzzer.Text = "Buzzer"
        Me.checkBoxBuzzer.UseVisualStyleBackColor = True
        '
        'GroupBoxArea
        '
        Me.GroupBoxArea.AutoSize = True
        Me.GroupBoxArea.Font = New System.Drawing.Font("PMingLiU", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.GroupBoxArea.Location = New System.Drawing.Point(3, 3)
        Me.GroupBoxArea.Name = "GroupBoxArea"
        Me.GroupBoxArea.Size = New System.Drawing.Size(6, 21)
        Me.GroupBoxArea.TabIndex = 31
        Me.GroupBoxArea.TabStop = False
        Me.GroupBoxArea.Text = "Error Show"
        '
        'buttonOption3
        '
        Me.buttonOption3.BackColor = System.Drawing.SystemColors.Control
        Me.buttonOption3.Cursor = System.Windows.Forms.Cursors.Default
        Me.buttonOption3.Enabled = False
        Me.buttonOption3.Font = New System.Drawing.Font("Times New Roman", 12.0!)
        Me.buttonOption3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.buttonOption3.Location = New System.Drawing.Point(255, 3)
        Me.buttonOption3.Name = "buttonOption3"
        Me.buttonOption3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.buttonOption3.Size = New System.Drawing.Size(120, 49)
        Me.buttonOption3.TabIndex = 28
        Me.buttonOption3.Text = "其他"
        Me.buttonOption3.UseVisualStyleBackColor = False
        '
        'ButtonAbort
        '
        Me.ButtonAbort.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonAbort.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonAbort.Enabled = False
        Me.ButtonAbort.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonAbort.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonAbort.Location = New System.Drawing.Point(381, 3)
        Me.ButtonAbort.Name = "ButtonAbort"
        Me.ButtonAbort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonAbort.Size = New System.Drawing.Size(81, 49)
        Me.ButtonAbort.TabIndex = 27
        Me.ButtonAbort.Text = "結束程式"
        Me.ButtonAbort.UseVisualStyleBackColor = False
        '
        'ButtonIgnore
        '
        Me.ButtonIgnore.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonIgnore.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonIgnore.Font = New System.Drawing.Font("Times New Roman", 12.0!)
        Me.ButtonIgnore.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonIgnore.Location = New System.Drawing.Point(129, 3)
        Me.ButtonIgnore.Name = "ButtonIgnore"
        Me.ButtonIgnore.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonIgnore.Size = New System.Drawing.Size(120, 49)
        Me.ButtonIgnore.TabIndex = 26
        Me.ButtonIgnore.Text = "忽略"
        Me.ButtonIgnore.UseVisualStyleBackColor = False
        '
        'ButtonRetry
        '
        Me.ButtonRetry.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonRetry.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonRetry.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonRetry.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonRetry.Location = New System.Drawing.Point(3, 3)
        Me.ButtonRetry.Name = "ButtonRetry"
        Me.ButtonRetry.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonRetry.Size = New System.Drawing.Size(120, 49)
        Me.ButtonRetry.TabIndex = 25
        Me.ButtonRetry.Text = "重試"
        Me.ButtonRetry.UseVisualStyleBackColor = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(0, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(84, 55)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 24
        Me.PictureBox1.TabStop = False
        '
        'lblErrorTitle
        '
        Me.lblErrorTitle.AutoSize = True
        Me.lblErrorTitle.BackColor = System.Drawing.SystemColors.Control
        Me.lblErrorTitle.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblErrorTitle.Font = New System.Drawing.Font("Times New Roman", 36.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblErrorTitle.ForeColor = System.Drawing.Color.Red
        Me.lblErrorTitle.Location = New System.Drawing.Point(90, 0)
        Me.lblErrorTitle.Name = "lblErrorTitle"
        Me.lblErrorTitle.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblErrorTitle.Size = New System.Drawing.Size(319, 54)
        Me.lblErrorTitle.TabIndex = 23
        Me.lblErrorTitle.Text = "Error Message"
        Me.lblErrorTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'TextBoxAlarmMessage
        '
        Me.TextBoxAlarmMessage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxAlarmMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxAlarmMessage.Font = New System.Drawing.Font("Microsoft JhengHei", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.TextBoxAlarmMessage.Location = New System.Drawing.Point(3, 30)
        Me.TextBoxAlarmMessage.Multiline = True
        Me.TextBoxAlarmMessage.Name = "TextBoxAlarmMessage"
        Me.TextBoxAlarmMessage.ReadOnly = True
        Me.TextBoxAlarmMessage.Size = New System.Drawing.Size(465, 137)
        Me.TextBoxAlarmMessage.TabIndex = 32
        Me.TextBoxAlarmMessage.Text = "ABCDEFG"
        '
        'ButtonHide
        '
        Me.ButtonHide.AutoSize = True
        Me.ButtonHide.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonHide.Location = New System.Drawing.Point(461, 3)
        Me.ButtonHide.Name = "ButtonHide"
        Me.ButtonHide.Size = New System.Drawing.Size(37, 22)
        Me.ButtonHide.TabIndex = 33
        Me.ButtonHide.Text = "Hide"
        Me.ButtonHide.UseVisualStyleBackColor = True
        Me.ButtonHide.Visible = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.lblErrorTitle)
        Me.Panel1.Controls.Add(Me.checkBoxBuzzer)
        Me.Panel1.Controls.Add(Me.ButtonHide)
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(505, 62)
        Me.Panel1.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBoxArea, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxAlarmMessage, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel2, 0, 2)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 68)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(471, 231)
        Me.TableLayoutPanel1.TabIndex = 34
        '
        'Panel2
        '
        Me.Panel2.AutoSize = True
        Me.Panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Panel2.Controls.Add(Me.ButtonAbort)
        Me.Panel2.Controls.Add(Me.ButtonIgnore)
        Me.Panel2.Controls.Add(Me.buttonOption3)
        Me.Panel2.Controls.Add(Me.ButtonRetry)
        Me.Panel2.Location = New System.Drawing.Point(3, 173)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(465, 55)
        Me.Panel2.TabIndex = 33
        '
        'userControlAlarmPop
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "userControlAlarmPop"
        Me.Size = New System.Drawing.Size(508, 302)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents checkBoxBuzzer As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxArea As System.Windows.Forms.GroupBox
    Public WithEvents buttonOption3 As System.Windows.Forms.Button
    Public WithEvents ButtonAbort As System.Windows.Forms.Button
    Public WithEvents ButtonIgnore As System.Windows.Forms.Button
    Public WithEvents ButtonRetry As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Public WithEvents lblErrorTitle As System.Windows.Forms.Label
    Public WithEvents TextBoxAlarmMessage As System.Windows.Forms.TextBox
    Friend WithEvents ButtonHide As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel

End Class
