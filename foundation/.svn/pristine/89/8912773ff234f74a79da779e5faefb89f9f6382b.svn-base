Public Class userControlFlags
    Property FlagsReference As List(Of flagElement)
        Get
            Return __flagReference
        End Get
        Set(value As List(Of flagElement))
            __flagReference = value
            userControlFlagsLoad(Me, Nothing)
        End Set
    End Property
    'Property labelReference As String()
    Private controlsNeedToReferesh As List(Of Control) = New List(Of Control)

    Private __flagReference As List(Of flagElement)

    Public Overrides Sub Refresh()
        ' refresh all sub controls
        For Each uc As Control In controlsNeedToReferesh
            uc.Refresh()
        Next

        MyBase.Refresh()
    End Sub



    Private Sub userControlFlagsLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        If (FlagsReference Is Nothing) Then
            Exit Sub
        End If

        '---------------
        '   Hsien , reset all status
        '---------------
        controlsNeedToReferesh.Clear()
        TableLayoutPanelFlagControl.Controls.Clear()

        Dim element As userControlFlagElement
        For Each flag As flagElement In FlagsReference
            element = New userControlFlagElement
            element.FlagElementReference = flag
            controlsNeedToReferesh.Add(element)

            ' addin tables
            TableLayoutPanelFlagControl.Controls.Add(element)
        Next
    End Sub
End Class
