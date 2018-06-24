<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlSmarPod
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ComboBoxCommand = New System.Windows.Forms.ComboBox()
        Me.ButtonSet = New System.Windows.Forms.Button()
        Me.ButtonMove = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ComboBox1stPositions = New System.Windows.Forms.ComboBox()
        Me.ButtonFindReferenceMark = New System.Windows.Forms.Button()
        Me.ButtonMoveToZero = New System.Windows.Forms.Button()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonRZminus = New System.Windows.Forms.Button()
        Me.ButtonRZplus = New System.Windows.Forms.Button()
        Me.ButtonRYminus = New System.Windows.Forms.Button()
        Me.ButtonRYplus = New System.Windows.Forms.Button()
        Me.ButtonRXminus = New System.Windows.Forms.Button()
        Me.ButtonRXplus = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TextBox_Rx = New System.Windows.Forms.TextBox()
        Me.TextBox_Ry = New System.Windows.Forms.TextBox()
        Me.TextBox_Rz = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.ComboBoxRelDegree = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_Px = New System.Windows.Forms.TextBox()
        Me.lblX = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ButtonZminus = New System.Windows.Forms.Button()
        Me.ButtonZplus = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonYplus = New System.Windows.Forms.Button()
        Me.ButtonYminus = New System.Windows.Forms.Button()
        Me.ButtonXplus = New System.Windows.Forms.Button()
        Me.ButtonXminus = New System.Windows.Forms.Button()
        Me.TextBox_Py = New System.Windows.Forms.TextBox()
        Me.TextBox_Pz = New System.Windows.Forms.TextBox()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.btnDisconnect = New System.Windows.Forms.Button()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ComboBoxRelDistance = New System.Windows.Forms.ComboBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxCommand)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonSet)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonMove)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBox1stPositions)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonFindReferenceMark)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonMoveToZero)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnConnect)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnDisconnect)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel2)
        Me.SplitContainer1.Panel1MinSize = 400
        Me.SplitContainer1.Size = New System.Drawing.Size(835, 555)
        Me.SplitContainer1.SplitterDistance = 419
        Me.SplitContainer1.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label5.Location = New System.Drawing.Point(8, 324)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(105, 20)
        Me.Label5.TabIndex = 42
        Me.Label5.Text = "Command："
        '
        'ComboBoxCommand
        '
        Me.ComboBoxCommand.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ComboBoxCommand.FormattingEnabled = True
        Me.ComboBoxCommand.Location = New System.Drawing.Point(116, 320)
        Me.ComboBoxCommand.Name = "ComboBoxCommand"
        Me.ComboBoxCommand.Size = New System.Drawing.Size(121, 28)
        Me.ComboBoxCommand.TabIndex = 41
        '
        'ButtonSet
        '
        Me.ButtonSet.Font = New System.Drawing.Font("微軟正黑體", 12.0!)
        Me.ButtonSet.Location = New System.Drawing.Point(313, 352)
        Me.ButtonSet.Name = "ButtonSet"
        Me.ButtonSet.Size = New System.Drawing.Size(64, 28)
        Me.ButtonSet.TabIndex = 40
        Me.ButtonSet.Text = "Set"
        Me.ButtonSet.UseVisualStyleBackColor = True
        '
        'ButtonMove
        '
        Me.ButtonMove.Font = New System.Drawing.Font("微軟正黑體", 12.0!)
        Me.ButtonMove.Location = New System.Drawing.Point(245, 352)
        Me.ButtonMove.Name = "ButtonMove"
        Me.ButtonMove.Size = New System.Drawing.Size(64, 28)
        Me.ButtonMove.TabIndex = 39
        Me.ButtonMove.Text = "Move to Zero"
        Me.ButtonMove.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("微軟正黑體", 12.0!)
        Me.Label4.Location = New System.Drawing.Point(12, 356)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(99, 20)
        Me.Label4.TabIndex = 38
        Me.Label4.Text = "Position 1："
        '
        'ComboBox1stPositions
        '
        Me.ComboBox1stPositions.Font = New System.Drawing.Font("微軟正黑體", 12.0!)
        Me.ComboBox1stPositions.FormattingEnabled = True
        Me.ComboBox1stPositions.Location = New System.Drawing.Point(117, 352)
        Me.ComboBox1stPositions.Name = "ComboBox1stPositions"
        Me.ComboBox1stPositions.Size = New System.Drawing.Size(121, 28)
        Me.ComboBox1stPositions.TabIndex = 36
        '
        'ButtonFindReferenceMark
        '
        Me.ButtonFindReferenceMark.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonFindReferenceMark.Location = New System.Drawing.Point(124, 456)
        Me.ButtonFindReferenceMark.Name = "ButtonFindReferenceMark"
        Me.ButtonFindReferenceMark.Size = New System.Drawing.Size(116, 28)
        Me.ButtonFindReferenceMark.TabIndex = 34
        Me.ButtonFindReferenceMark.Text = "Find Reference Mark"
        Me.ButtonFindReferenceMark.UseVisualStyleBackColor = True
        '
        'ButtonMoveToZero
        '
        Me.ButtonMoveToZero.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonMoveToZero.Location = New System.Drawing.Point(24, 456)
        Me.ButtonMoveToZero.Name = "ButtonMoveToZero"
        Me.ButtonMoveToZero.Size = New System.Drawing.Size(96, 28)
        Me.ButtonMoveToZero.TabIndex = 33
        Me.ButtonMoveToZero.Text = "Move to Zero"
        Me.ButtonMoveToZero.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 4
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRZminus, 3, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRZplus, 2, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRYminus, 3, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRYplus, 2, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRXminus, 3, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.ButtonRXplus, 2, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label8, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Label12, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label13, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.Label14, 0, 3)
        Me.TableLayoutPanel3.Controls.Add(Me.TextBox_Rx, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.TextBox_Ry, 1, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.TextBox_Rz, 1, 3)
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(8, 148)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 4
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(388, 116)
        Me.TableLayoutPanel3.TabIndex = 31
        '
        'ButtonRZminus
        '
        Me.ButtonRZminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRZminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRZminus.Location = New System.Drawing.Point(339, 86)
        Me.ButtonRZminus.Name = "ButtonRZminus"
        Me.ButtonRZminus.Size = New System.Drawing.Size(49, 27)
        Me.ButtonRZminus.TabIndex = 46
        Me.ButtonRZminus.Text = "RZ-"
        Me.ButtonRZminus.UseVisualStyleBackColor = True
        '
        'ButtonRZplus
        '
        Me.ButtonRZplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRZplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRZplus.Location = New System.Drawing.Point(284, 86)
        Me.ButtonRZplus.Name = "ButtonRZplus"
        Me.ButtonRZplus.Size = New System.Drawing.Size(49, 27)
        Me.ButtonRZplus.TabIndex = 45
        Me.ButtonRZplus.Text = "RZ+"
        Me.ButtonRZplus.UseVisualStyleBackColor = True
        '
        'ButtonRYminus
        '
        Me.ButtonRYminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRYminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRYminus.Location = New System.Drawing.Point(339, 55)
        Me.ButtonRYminus.Name = "ButtonRYminus"
        Me.ButtonRYminus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonRYminus.TabIndex = 44
        Me.ButtonRYminus.Text = "RY-"
        Me.ButtonRYminus.UseVisualStyleBackColor = True
        '
        'ButtonRYplus
        '
        Me.ButtonRYplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRYplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRYplus.Location = New System.Drawing.Point(284, 55)
        Me.ButtonRYplus.Name = "ButtonRYplus"
        Me.ButtonRYplus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonRYplus.TabIndex = 43
        Me.ButtonRYplus.Text = "RY+"
        Me.ButtonRYplus.UseVisualStyleBackColor = True
        '
        'ButtonRXminus
        '
        Me.ButtonRXminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRXminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRXminus.Location = New System.Drawing.Point(339, 23)
        Me.ButtonRXminus.Name = "ButtonRXminus"
        Me.ButtonRXminus.Size = New System.Drawing.Size(49, 26)
        Me.ButtonRXminus.TabIndex = 42
        Me.ButtonRXminus.Text = "RX-"
        Me.ButtonRXminus.UseVisualStyleBackColor = True
        '
        'ButtonRXplus
        '
        Me.ButtonRXplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonRXplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonRXplus.Location = New System.Drawing.Point(284, 23)
        Me.ButtonRXplus.Name = "ButtonRXplus"
        Me.ButtonRXplus.Size = New System.Drawing.Size(49, 26)
        Me.ButtonRXplus.TabIndex = 41
        Me.ButtonRXplus.Text = "RX+"
        Me.ButtonRXplus.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label8.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label8.Location = New System.Drawing.Point(43, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(235, 20)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "Angle(u degree)"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label12.Location = New System.Drawing.Point(3, 20)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(29, 20)
        Me.Label12.TabIndex = 30
        Me.Label12.Text = "RX"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label13.Location = New System.Drawing.Point(3, 52)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(29, 20)
        Me.Label13.TabIndex = 31
        Me.Label13.Text = "RY"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label14.Location = New System.Drawing.Point(3, 83)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(29, 20)
        Me.Label14.TabIndex = 32
        Me.Label14.Text = "RZ"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox_Rx
        '
        Me.TextBox_Rx.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Rx.Location = New System.Drawing.Point(43, 23)
        Me.TextBox_Rx.Multiline = True
        Me.TextBox_Rx.Name = "TextBox_Rx"
        Me.TextBox_Rx.Size = New System.Drawing.Size(235, 26)
        Me.TextBox_Rx.TabIndex = 35
        '
        'TextBox_Ry
        '
        Me.TextBox_Ry.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Ry.Location = New System.Drawing.Point(43, 55)
        Me.TextBox_Ry.Multiline = True
        Me.TextBox_Ry.Name = "TextBox_Ry"
        Me.TextBox_Ry.Size = New System.Drawing.Size(235, 25)
        Me.TextBox_Ry.TabIndex = 36
        '
        'TextBox_Rz
        '
        Me.TextBox_Rz.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Rz.Location = New System.Drawing.Point(43, 86)
        Me.TextBox_Rz.Multiline = True
        Me.TextBox_Rz.Name = "TextBox_Rz"
        Me.TextBox_Rz.Size = New System.Drawing.Size(235, 27)
        Me.TextBox_Rz.TabIndex = 37
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 2
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.20618!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.79382!))
        Me.TableLayoutPanel4.Controls.Add(Me.Label15, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.ComboBoxRelDegree, 1, 0)
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(8, 264)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(388, 22)
        Me.TableLayoutPanel4.TabIndex = 32
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label15.Location = New System.Drawing.Point(3, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(140, 20)
        Me.Label15.TabIndex = 30
        Me.Label15.Text = "Angle Step(udeg)"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxRelDegree
        '
        Me.ComboBoxRelDegree.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxRelDegree.FormattingEnabled = True
        Me.ComboBoxRelDegree.Items.AddRange(New Object() {"0.1", "0.5", "1", "5", "10"})
        Me.ComboBoxRelDegree.Location = New System.Drawing.Point(158, 3)
        Me.ComboBoxRelDegree.Name = "ComboBoxRelDegree"
        Me.ComboBoxRelDegree.Size = New System.Drawing.Size(227, 20)
        Me.ComboBoxRelDegree.TabIndex = 24
        Me.ComboBoxRelDegree.Text = "1"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Px, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lblX, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonZminus, 3, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonZplus, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonYplus, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonYminus, 3, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonXplus, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonXminus, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Py, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox_Pz, 1, 3)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(8, 8)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(388, 116)
        Me.TableLayoutPanel1.TabIndex = 28
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label1.Location = New System.Drawing.Point(43, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(235, 20)
        Me.Label1.TabIndex = 27
        Me.Label1.Text = "Position(mm)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox_Px
        '
        Me.TextBox_Px.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox_Px.Location = New System.Drawing.Point(43, 23)
        Me.TextBox_Px.Multiline = True
        Me.TextBox_Px.Name = "TextBox_Px"
        Me.TextBox_Px.Size = New System.Drawing.Size(235, 25)
        Me.TextBox_Px.TabIndex = 26
        '
        'lblX
        '
        Me.lblX.AutoSize = True
        Me.lblX.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.lblX.Location = New System.Drawing.Point(3, 20)
        Me.lblX.Name = "lblX"
        Me.lblX.Size = New System.Drawing.Size(19, 20)
        Me.lblX.TabIndex = 25
        Me.lblX.Text = "X"
        Me.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label2.Location = New System.Drawing.Point(3, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(19, 20)
        Me.Label2.TabIndex = 28
        Me.Label2.Text = "Y"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonZminus
        '
        Me.ButtonZminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonZminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonZminus.Location = New System.Drawing.Point(339, 85)
        Me.ButtonZminus.Name = "ButtonZminus"
        Me.ButtonZminus.Size = New System.Drawing.Size(49, 28)
        Me.ButtonZminus.TabIndex = 19
        Me.ButtonZminus.Text = "Z-"
        Me.ButtonZminus.UseVisualStyleBackColor = True
        '
        'ButtonZplus
        '
        Me.ButtonZplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonZplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonZplus.Location = New System.Drawing.Point(284, 85)
        Me.ButtonZplus.Name = "ButtonZplus"
        Me.ButtonZplus.Size = New System.Drawing.Size(49, 28)
        Me.ButtonZplus.TabIndex = 22
        Me.ButtonZplus.Text = "Z+"
        Me.ButtonZplus.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label3.Location = New System.Drawing.Point(3, 82)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(19, 20)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Z"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonYplus
        '
        Me.ButtonYplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonYplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonYplus.Location = New System.Drawing.Point(284, 54)
        Me.ButtonYplus.Name = "ButtonYplus"
        Me.ButtonYplus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonYplus.TabIndex = 21
        Me.ButtonYplus.Text = "Y+"
        Me.ButtonYplus.UseVisualStyleBackColor = True
        '
        'ButtonYminus
        '
        Me.ButtonYminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonYminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonYminus.Location = New System.Drawing.Point(339, 54)
        Me.ButtonYminus.Name = "ButtonYminus"
        Me.ButtonYminus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonYminus.TabIndex = 18
        Me.ButtonYminus.Text = "Y-"
        Me.ButtonYminus.UseVisualStyleBackColor = True
        '
        'ButtonXplus
        '
        Me.ButtonXplus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonXplus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonXplus.Location = New System.Drawing.Point(284, 23)
        Me.ButtonXplus.Name = "ButtonXplus"
        Me.ButtonXplus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonXplus.TabIndex = 17
        Me.ButtonXplus.Text = "X+"
        Me.ButtonXplus.UseVisualStyleBackColor = True
        '
        'ButtonXminus
        '
        Me.ButtonXminus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonXminus.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ButtonXminus.Location = New System.Drawing.Point(339, 23)
        Me.ButtonXminus.Name = "ButtonXminus"
        Me.ButtonXminus.Size = New System.Drawing.Size(49, 25)
        Me.ButtonXminus.TabIndex = 16
        Me.ButtonXminus.Text = "X-"
        Me.ButtonXminus.UseVisualStyleBackColor = True
        '
        'TextBox_Py
        '
        Me.TextBox_Py.Location = New System.Drawing.Point(43, 54)
        Me.TextBox_Py.Multiline = True
        Me.TextBox_Py.Name = "TextBox_Py"
        Me.TextBox_Py.Size = New System.Drawing.Size(235, 25)
        Me.TextBox_Py.TabIndex = 33
        '
        'TextBox_Pz
        '
        Me.TextBox_Pz.Location = New System.Drawing.Point(43, 85)
        Me.TextBox_Pz.Multiline = True
        Me.TextBox_Pz.Name = "TextBox_Pz"
        Me.TextBox_Pz.Size = New System.Drawing.Size(235, 27)
        Me.TextBox_Pz.TabIndex = 34
        '
        'btnConnect
        '
        Me.btnConnect.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConnect.Location = New System.Drawing.Point(24, 488)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(96, 28)
        Me.btnConnect.TabIndex = 13
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'btnDisconnect
        '
        Me.btnDisconnect.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDisconnect.Location = New System.Drawing.Point(24, 520)
        Me.btnDisconnect.Name = "btnDisconnect"
        Me.btnDisconnect.Size = New System.Drawing.Size(96, 28)
        Me.btnDisconnect.TabIndex = 12
        Me.btnDisconnect.Text = "Disconnect"
        Me.btnDisconnect.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.20618!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.79382!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label7, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ComboBoxRelDistance, 1, 0)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(8, 124)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(388, 22)
        Me.TableLayoutPanel2.TabIndex = 30
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label7.Location = New System.Drawing.Point(3, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(143, 20)
        Me.Label7.TabIndex = 30
        Me.Label7.Text = "Position Step(um)"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxRelDistance
        '
        Me.ComboBoxRelDistance.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxRelDistance.FormattingEnabled = True
        Me.ComboBoxRelDistance.Items.AddRange(New Object() {"1", "10", "100", "1000"})
        Me.ComboBoxRelDistance.Location = New System.Drawing.Point(158, 3)
        Me.ComboBoxRelDistance.Name = "ComboBoxRelDistance"
        Me.ComboBoxRelDistance.Size = New System.Drawing.Size(227, 20)
        Me.ComboBoxRelDistance.TabIndex = 24
        Me.ComboBoxRelDistance.Text = "1000"
        '
        'Timer1
        '
        '
        'userControlSmarPod
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "userControlSmarPod"
        Me.Size = New System.Drawing.Size(835, 555)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents btnDisconnect As System.Windows.Forms.Button
    Friend WithEvents ButtonXminus As System.Windows.Forms.Button
    Friend WithEvents ComboBoxRelDistance As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonZplus As System.Windows.Forms.Button
    Friend WithEvents ButtonYplus As System.Windows.Forms.Button
    Friend WithEvents ButtonZminus As System.Windows.Forms.Button
    Friend WithEvents ButtonYminus As System.Windows.Forms.Button
    Friend WithEvents ButtonXplus As System.Windows.Forms.Button
    Friend WithEvents lblX As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Px As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Py As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Pz As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonRZminus As System.Windows.Forms.Button
    Friend WithEvents ButtonRZplus As System.Windows.Forms.Button
    Friend WithEvents ButtonRYminus As System.Windows.Forms.Button
    Friend WithEvents ButtonRYplus As System.Windows.Forms.Button
    Friend WithEvents ButtonRXminus As System.Windows.Forms.Button
    Friend WithEvents ButtonRXplus As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Rx As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Ry As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Rz As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxRelDegree As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonMoveToZero As System.Windows.Forms.Button
    Friend WithEvents ButtonFindReferenceMark As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1stPositions As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonSet As System.Windows.Forms.Button
    Friend WithEvents ButtonMove As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxCommand As System.Windows.Forms.ComboBox

End Class
