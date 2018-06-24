Imports Automation
''' <summary>
''' Used to control analog output
''' </summary>
''' <remarks></remarks>
Public Class userControlAnalogSetter

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub loadControl() Handles MyBase.Load

        'clear all
        ComboBoxSelection.Items.Clear()
        utilitiesUI.loadComboBoxItemByEnum(Me.ComboBoxSelection, {outputAddress.W1,
                                                                  outputAddress.W2,
                                                                  outputAddress.W3})

    End Sub

    Sub setValue() Handles ButtonSet.Click
        mainIOHardware.writeDouble(Convert.ToUInt64(ComboBoxSelection.SelectedItem),
                                   Convert.ToDouble(TextBoxValue.Text))
    End Sub
    Sub readValue() Handles ComboBoxSelection.SelectedIndexChanged
        TextBoxValue.Text = mainIOHardware.readDouble(Convert.ToUInt64(ComboBoxSelection.SelectedItem))
    End Sub


End Class
