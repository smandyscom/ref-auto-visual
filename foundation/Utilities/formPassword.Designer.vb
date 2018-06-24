<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formPassword
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxOld = New System.Windows.Forms.TextBox()
        Me.TextBoxNew = New System.Windows.Forms.TextBox()
        Me.TextBoxAgain = New System.Windows.Forms.TextBox()
        Me.ButtonEnter = New System.Windows.Forms.Button()
        Me.ButtonLogIn = New System.Windows.Forms.Button()
        Me.ButtonExit = New System.Windows.Forms.Button()
        Me.TextBoxLogIn = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxStatus = New System.Windows.Forms.TextBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSize = True
        Me.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.TextBoxOld)
        Me.GroupBox1.Controls.Add(Me.TextBoxNew)
        Me.GroupBox1.Controls.Add(Me.TextBoxAgain)
        Me.GroupBox1.Controls.Add(Me.ButtonEnter)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 78)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(178, 154)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "New password setting"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(29, 86)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(33, 12)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Again"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(29, 58)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(26, 12)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "New"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(29, 30)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(22, 12)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Old"
        '
        'TextBoxOld
        '
        Me.TextBoxOld.Location = New System.Drawing.Point(72, 27)
        Me.TextBoxOld.Name = "TextBoxOld"
        Me.TextBoxOld.Size = New System.Drawing.Size(100, 22)
        Me.TextBoxOld.TabIndex = 6
        Me.TextBoxOld.UseSystemPasswordChar = True
        '
        'TextBoxNew
        '
        Me.TextBoxNew.Location = New System.Drawing.Point(72, 55)
        Me.TextBoxNew.Name = "TextBoxNew"
        Me.TextBoxNew.Size = New System.Drawing.Size(100, 22)
        Me.TextBoxNew.TabIndex = 5
        Me.TextBoxNew.UseSystemPasswordChar = True
        '
        'TextBoxAgain
        '
        Me.TextBoxAgain.Location = New System.Drawing.Point(72, 83)
        Me.TextBoxAgain.Name = "TextBoxAgain"
        Me.TextBoxAgain.Size = New System.Drawing.Size(100, 22)
        Me.TextBoxAgain.TabIndex = 4
        Me.TextBoxAgain.UseSystemPasswordChar = True
        '
        'ButtonEnter
        '
        Me.ButtonEnter.Location = New System.Drawing.Point(72, 111)
        Me.ButtonEnter.Name = "ButtonEnter"
        Me.ButtonEnter.Size = New System.Drawing.Size(100, 22)
        Me.ButtonEnter.TabIndex = 3
        Me.ButtonEnter.Text = "Change"
        Me.ButtonEnter.UseVisualStyleBackColor = True
        '
        'ButtonLogIn
        '
        Me.ButtonLogIn.Location = New System.Drawing.Point(196, 34)
        Me.ButtonLogIn.Name = "ButtonLogIn"
        Me.ButtonLogIn.Size = New System.Drawing.Size(75, 28)
        Me.ButtonLogIn.TabIndex = 1
        Me.ButtonLogIn.Text = "Enter"
        Me.ButtonLogIn.UseVisualStyleBackColor = True
        '
        'ButtonExit
        '
        Me.ButtonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonExit.Location = New System.Drawing.Point(196, 105)
        Me.ButtonExit.Name = "ButtonExit"
        Me.ButtonExit.Size = New System.Drawing.Size(75, 78)
        Me.ButtonExit.TabIndex = 2
        Me.ButtonExit.Text = "Exit"
        Me.ButtonExit.UseVisualStyleBackColor = True
        '
        'TextBoxLogIn
        '
        Me.TextBoxLogIn.Location = New System.Drawing.Point(84, 34)
        Me.TextBoxLogIn.Name = "TextBoxLogIn"
        Me.TextBoxLogIn.Size = New System.Drawing.Size(100, 22)
        Me.TextBoxLogIn.TabIndex = 0
        Me.TextBoxLogIn.UseSystemPasswordChar = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(41, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(40, 12)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Log In:"
        '
        'TextBoxStatus
        '
        Me.TextBoxStatus.Location = New System.Drawing.Point(12, 238)
        Me.TextBoxStatus.Name = "TextBoxStatus"
        Me.TextBoxStatus.ReadOnly = True
        Me.TextBoxStatus.Size = New System.Drawing.Size(272, 22)
        Me.TextBoxStatus.TabIndex = 5
        '
        'formPassword
        '
        Me.AcceptButton = Me.ButtonLogIn
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.ButtonExit
        Me.ClientSize = New System.Drawing.Size(296, 261)
        Me.Controls.Add(Me.TextBoxStatus)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextBoxLogIn)
        Me.Controls.Add(Me.ButtonExit)
        Me.Controls.Add(Me.ButtonLogIn)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "formPassword"
        Me.Text = "Log In"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxOld As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxNew As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxAgain As System.Windows.Forms.TextBox
    Friend WithEvents ButtonEnter As System.Windows.Forms.Button
    Friend WithEvents ButtonLogIn As System.Windows.Forms.Button
    Friend WithEvents ButtonExit As System.Windows.Forms.Button
    Friend WithEvents TextBoxLogIn As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBoxStatus As System.Windows.Forms.TextBox
End Class
