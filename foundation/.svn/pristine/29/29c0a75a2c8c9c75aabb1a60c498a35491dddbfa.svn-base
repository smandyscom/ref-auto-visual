Imports Automation.mainIOHardware

''' <summary>
''' Offer the facility to edit main hardware node
''' </summary>
''' <remarks></remarks>
Public Class userControlHardwareNode
    Public Property HardwareNodeReference As hardwareBase
        Get
            Return __hardwareReference
        End Get
        Set(ByVal value As hardwareBase)
            __hardwareReference = value
            loadControl()
        End Set
    End Property
    Private __hardwareReference As hardwareBase = Nothing

    Sub loadControl() Handles MyBase.Load

        'reload options if no content inside
        If (ComboBoxHardwareType.Items.Count = 0) Then
            utilitiesUI.loadComboBoxItemByEnum(ComboBoxHardwareType, GetType(hardwareCodeEnum))
        End If

        If (__hardwareReference Is Nothing) Then
            Exit Sub
        End If


        If (__hardwareReference IsNot Nothing) Then
            ComboBoxHardwareType.SelectedItem = [Enum].ToObject(GetType(hardwareCodeEnum), (__hardwareReference.HardwareCode))
            PropertyGridHardwareDetail.SelectedObject = __hardwareReference
        End If

        TimerScan.Enabled = True    'start scanning

    End Sub

    Private Sub swapHardware(sender As Object, e As EventArgs) Handles ComboBoxHardwareType.SelectedValueChanged
        'allocating condition:
        '1. __hardware not allocated
        '2. the item combobox selected is different to __hardware

        If __hardwareReference Is Nothing OrElse
            Not ComboBoxHardwareType.SelectedItem.Equals([Enum].ToObject(GetType(hardwareCodeEnum), __hardwareReference.HardwareCode)) Then

            Select Case CType(ComboBoxHardwareType.SelectedItem, hardwareCodeEnum)
                Case hardwareCodeEnum.AMAX_1202_CARD
                    __hardwareReference = New amaxCard
                Case hardwareCodeEnum.DMT_MODBUS
                    __hardwareReference = New dmtModbusInterface
                Case hardwareCodeEnum.TWINCAT_ADS
                Case hardwareCodeEnum.VIRTUAL
                    __hardwareReference = New virtualModule
                Case hardwareCodeEnum.MELSEC_ETHERNET
                    __hardwareReference = New melsecOverEthernet
                Case Else

            End Select

            __hardwareReference.buildRawSeed() 'Hsine , 2015.12.17

            PropertyGridHardwareDetail.SelectedObject = __hardwareReference

            If (Not TimerScan.Enabled) Then
                TimerScan.Enabled = True
            End If

        End If
    End Sub
    Private Sub TimerScanTick(sender As Object, e As EventArgs) Handles TimerScan.Tick

        With __hardwareReference

            LabelHardwareCode.Text = Hex(.HardwareCode)
            LabelStatus.Text = .Status.ToString


        End With

    End Sub
End Class
