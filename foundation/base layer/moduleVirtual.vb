Imports Automation.mainIOHardware

Public Class virtualModule
    Inherits hardwareBase

    Property MemorySize As UInteger = 256

    Public Overrides Function dataDispatch() As Integer
        Return 0    'no need to update
        'TODO , or use script to bridge?
    End Function

    Public Overrides Function initialize() As Integer
        Return 0    'no need to initial
    End Function

    Public Overrides Function readValue(addressCodeInByte() As Byte) As ULong
        Return memoryHold(addressCodeInByte(generalAddressCode.BYTE_INDEX_DATA_INDEX))
    End Function

    Public Overrides Sub writeValue(addressCodeInByte() As Byte, value As ULong)
        'zero address is forbidden to write
        If addressCodeInByte(generalAddressCode.BYTE_INDEX_DATA_INDEX) <> 0 Then
            memoryHold(addressCodeInByte(generalAddressCode.BYTE_INDEX_DATA_INDEX)) = value
        End If
    End Sub

    Sub New()
        __hardwareCode = hardwareCodeEnum.VIRTUAL
        memoryHold = New ULong(MemorySize) {}
        For index = 0 To memoryHold.Length - 1
            memoryHold(index) = 0 ' clear all memory
        Next
        memoryHold(0) = &H2
    End Sub

End Class
