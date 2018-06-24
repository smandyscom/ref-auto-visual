<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class userControlAnalogSetter
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonSet = New System.Windows.Forms.Button()
        Me.ComboBoxSelection = New System.Windows.Forms.ComboBox()
        Me.TextBoxValue = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonSet, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboBoxSelection, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxValue, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(175, 23)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'ButtonSet
        '
        Me.ButtonSet.Location = New System.Drawing.Point(100, 0)
        Me.ButtonSet.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonSet.Name = "ButtonSet"
        Me.ButtonSet.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSet.TabIndex = 0
        Me.ButtonSet.Text = "Set"
        Me.ButtonSet.UseVisualStyleBackColor = True
        '
        'ComboBoxSelection
        '
        Me.ComboBoxSelection.FormattingEnabled = True
        Me.ComboBoxSelection.Location = New System.Drawing.Point(0, 0)
        Me.ComboBoxSelection.Margin = New System.Windows.Forms.Padding(0)
        Me.ComboBoxSelection.Name = "ComboBoxSelection"
        Me.ComboBoxSelection.Size = New System.Drawing.Size(50, 20)
        Me.ComboBoxSelection.TabIndex = 1
        '
        'TextBoxValue
        '
        Me.TextBoxValue.Location = New System.Drawing.Point(50, 0)
        Me.TextBoxValue.Margin = New System.Windows.Forms.Padding(0)
        Me.TextBoxValue.Name = "TextBoxValue"
        Me.TextBoxValue.Size = New System.Drawing.Size(50, 22)
        Me.TextBoxValue.TabIndex = 2
        Me.TextBoxValue.Text = "0"
        '
        'userControlAnalogSetter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "userControlAnalogSetter"
        Me.Size = New System.Drawing.Size(181, 29)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ButtonSet As System.Windows.Forms.Button
    Friend WithEvents ComboBoxSelection As System.Windows.Forms.ComboBox
    Friend WithEvents TextBoxValue As System.Windows.Forms.TextBox

End Class
