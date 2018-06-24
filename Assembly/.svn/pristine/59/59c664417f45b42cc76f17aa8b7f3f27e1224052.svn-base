Public Class userControlChannelData

    WriteOnly Property DataSource As channelData
        Set(value As channelData)
            __dataSource = value
            loadControl()
        End Set
    End Property

    Dim __dataSource As channelData = Nothing

    Sub loadControl() Handles MyBase.Load

        If __dataSource Is Nothing Then
            Exit Sub
        End If

        Me.Invoke(Sub()
                      UserControlIntensityMapData.Data = __dataSource.RawData
                      PropertyGridData.SelectedObject = __dataSource
                  End Sub)
    End Sub

End Class
