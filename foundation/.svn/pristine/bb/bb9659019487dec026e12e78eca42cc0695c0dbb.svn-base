Imports System.ComponentModel
Public Class userControlBufferBase

    WriteOnly Property BufferBaseReference As bufferBase
        Set(value As bufferBase)
            __bufferBaseReference = value
            loadControl()
        End Set
    End Property

    Dim __bufferBaseReference As bufferBase = Nothing

    Sub loadControl() Handles MyBase.Load
        If __bufferBaseReference Is Nothing Then
            Exit Sub
        End If

        PropertyGridBuffer.SelectedObject = __bufferBaseReference
        PropertyGridBuffer.BrowsableAttributes = New System.ComponentModel.AttributeCollection({New CategoryAttribute("BufferBase")}) 'display those property with this kind of attribute
        timerScan.Enabled = True

    End Sub

    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles timerScan.Tick
        PropertyGridBuffer.Refresh()
    End Sub
End Class
