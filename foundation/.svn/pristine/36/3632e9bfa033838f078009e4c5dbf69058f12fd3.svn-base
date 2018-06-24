Imports System.Windows.Forms.ListViewItem

Public Class userControlMelsec
    WriteOnly Property MelsecReference As melsecOverEthernet
        Set(value As melsecOverEthernet)
            __melsecReference = value
        End Set
    End Property

    Dim __melsecReference As melsecOverEthernet = Nothing

    Dim __binLabelsList As List(Of Label) = New List(Of Label)
    Dim __boutLabelsList As List(Of Label) = New List(Of Label)

    Dim __winItemList As List(Of ListViewSubItem) = New List(Of ListViewSubItem)
    Dim __woutItemList As List(Of ListViewSubItem) = New List(Of ListViewSubItem)

    Dim Index As Integer = 0

    Sub loadControl() Handles MyBase.Load

        If __melsecReference Is Nothing Then
            Exit Sub
        End If

        GroupBoxMain.Text = __melsecReference.ToString

        'Bin Value
        TableLayoutPanelBin.Controls.Clear()
        For Me.Index = 0 To 63
            Dim __label As Label = New Label With {.Text = String.Format("B{0:X4}", __melsecReference.__bReadStartAddressNumeric + Index),
                                                   .Size = New Size(55, 20),
                                                   .Margin = New Padding(0),
                                                   .Dock = DockStyle.Fill,
                                                   .TextAlign = ContentAlignment.TopLeft}
            __binLabelsList.Add(__label)
            TableLayoutPanelBin.Controls.Add(__label, -1, -1)
        Next
        'Bout Value
        TableLayoutPanelBout.Controls.Clear()
        For Me.Index = 0 To 63
            Dim __label As Label = New Label With {.Text = String.Format("B{0:X4}", __melsecReference.__bWriteStartAddressNumeric + Index),
                                                   .Size = New Size(55, 20),
                                                   .Margin = New Padding(0),
                                                   .Dock = DockStyle.Fill,
                                                   .TextAlign = ContentAlignment.TopLeft}
            __boutLabelsList.Add(__label)
            TableLayoutPanelBout.Controls.Add(__label, -1, -1)
            AddHandler __label.Click, AddressOf labelClickHandler
        Next
        'Win Value
        ListViewWin.Items.Clear()
        For Me.Index = 0 To __melsecReference.WReadRange - 1
            Dim __listViewItem As ListViewItem = New ListViewItem(String.Format("W{0:X4}", __melsecReference.__wReadStartWord + Index))
            Dim __listViewSubItem As ListViewSubItem = New ListViewSubItem With {.Text = "default"}
            __winItemList.Add(__listViewSubItem)
            __listViewItem.SubItems.Add(__listViewSubItem)
            ListViewWin.Items.Add(__listViewItem)
        Next
        'Wout Value
        ListViewWout.Items.Clear()
        For Me.Index = 0 To __melsecReference.WWriteRange - 1
            Dim __listViewItem As ListViewItem = New ListViewItem(String.Format("W{0:X4}", __melsecReference.__wWriteStartWord + Index))
            Dim __listViewSubItem As ListViewSubItem = New ListViewSubItem With {.Text = "default"}
            __woutItemList.Add(__listViewSubItem)
            __listViewItem.SubItems.Add(__listViewSubItem)
            ListViewWout.Items.Add(__listViewItem)
        Next

        TimerScan.Enabled = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub timerTick(sender As Object, e As EventArgs) Handles TimerScan.Tick
        SuspendLayout()

        'polling Bin values
        For Me.Index = 0 To __binLabelsList.Count - 1
            utilitiesUI.controlFollowBoolean(__binLabelsList(Index), __melsecReference.ReadBit(melsecOverEthernet.categroryCodeEnum.B_IN_BASE, Index))
        Next
        'polling Bout values
        For Me.Index = 0 To __boutLabelsList.Count - 1
            utilitiesUI.controlFollowBoolean(__boutLabelsList(Index), __melsecReference.ReadBit(melsecOverEthernet.categroryCodeEnum.B_OUT_BASE, Index))
        Next
        'polling Win values
        For Me.Index = 0 To __winItemList.Count - 1
            __winItemList(Index).Text = __melsecReference.ReadWValue(Index)
        Next
        'polling Wout values
        For Me.Index = 0 To __woutItemList.Count - 1
            'offset read index
            __woutItemList(Index).Text = __melsecReference.ReadWValue(__winItemList.Count + Index)
        Next

        PropertyGridMelsec.Refresh()
        ResumeLayout()
    End Sub

    Sub labelClickHandler(sender As Label, e As EventArgs)
        Dim serialAddress As Integer = __boutLabelsList.IndexOf(sender)
        __melsecReference.WriteBit(melsecOverEthernet.categroryCodeEnum.B_OUT_BASE, serialAddress) =
            Not __melsecReference.ReadBit(melsecOverEthernet.categroryCodeEnum.B_OUT_BASE, serialAddress)
    End Sub


End Class
