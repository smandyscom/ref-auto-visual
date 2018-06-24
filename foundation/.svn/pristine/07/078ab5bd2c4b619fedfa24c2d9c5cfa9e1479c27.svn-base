Imports System.Windows.Forms.ListViewItem

Public Class userControlDMTSlave

    WriteOnly Property SlaveReference As dmtModbusSlave
        Set(value As dmtModbusSlave)
            __slaveReference = value
        End Set
    End Property

    Dim __slaveReference As dmtModbusSlave = Nothing

    Dim __dSubitemList As List(Of ListViewSubItem) = New List(Of ListViewSubItem)
    Dim __xSubitemList As List(Of Label) = New List(Of Label)
    Dim __ySubitemList As List(Of Label) = New List(Of Label)

    Sub loadControl() Handles MyBase.Load

        If (__slaveReference Is Nothing) Then
            Exit Sub
        End If

        GroupBoxTitle.Text = __slaveReference.SlaveName

        'X value 
        TableLayoutPanelX.Controls.Clear()
        __xSubitemList.Clear()
        For __index = 0 To __slaveReference.XRange - 1

            'Hsien ,  delta-plc had stupid and incorporate address mode , 2016.02.05
            'Dim formattion As String = ""
            'Dim width As Integer = 8
            'Dim preFix As Integer = Math.Ceiling(__index \ 8)
            'Dim postFix As Integer = __index Mod 8

            'Select Case __slaveReference.SlaveType
            '    Case seriesEnum.DVP
            '        '8-bit width
            '        formattion = "X{0}{1}"
            '        width = 8
            '    Case seriesEnum.AH
            '        '16-bits width
            '        formattion = "X{0}.{1}"
            '        width = 16
            'End Select

            ''e.g , X20 , the seventeenth input address
            Dim __label As Label = New Label With {.Text = String.Format("X{0}",
                                                                          __slaveReference.serialBit2DeltaFormat(__index)),
                                                   .Size = New Size(50, 20),
                                                   .Margin = New Padding(0),
                                                   .TextAlign = ContentAlignment.TopLeft}
            __xSubitemList.Add(__label)
            TableLayoutPanelX.Controls.Add(__label, -1, -1)
        Next
        'Y value 
        TableLayoutPanelY.Controls.Clear()
        __ySubitemList.Clear()
        For __index = 0 To __slaveReference.YRange - 1
            Dim __label As Label = New Label With {.Text = String.Format("Y{0}",
                                                                          __slaveReference.serialBit2DeltaFormat(__index)),
                                                   .Size = New Size(50, 20),
                                                   .Margin = New Padding(0),
                                                   .TextAlign = ContentAlignment.TopLeft}
            __ySubitemList.Add(__label)
            TableLayoutPanelY.Controls.Add(__label, -1, -1)
            AddHandler __label.Click, AddressOf YclickHandler 'click to flip value
        Next

        'D value listview initialize
        ListViewDValue.Items.Clear()
        __dSubitemList.Clear()
        For __index = 0 To (__slaveReference.DReadRange + __slaveReference.DWriteRange) - 1
            Dim __listViewItem As ListViewItem = New ListViewItem(String.Format("D{0}", __index))
            Dim __listViewSubItem As ListViewSubItem = New ListViewSubItem With {.Text = "default"}
            __dSubitemList.Add(__listViewSubItem)
            __listViewItem.SubItems.Add(__listViewSubItem)
            ListViewDValue.Items.Add(__listViewItem)
        Next

        'master infomation post on
        If (__slaveReference.parentRaference IsNot Nothing) Then
            PropertyGridMasterInfo.SelectedObject = __slaveReference.parentRaference
        End If


        '' initialize header
        With ListViewDValue
            .AutoArrange = True
            .View = View.Details
            .GridLines = True
            .Columns.Add("D Address", -1, HorizontalAlignment.Left)
            .Columns.Add("Value（HEX）", -2, HorizontalAlignment.Left)
        End With

        TimerScan.Enabled = True
    End Sub


    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles TimerScan.Tick
        SuspendLayout()

        'polling D Values
        For __index = 0 To (__slaveReference.DReadRange + __slaveReference.DWriteRange) - 1
            __dSubitemList(__index).Text = String.Format("0x{0:X}", __slaveReference.ReadDeviceValue(dmtModbusSlave.categroryCodeEnum.D, __index))
        Next
        For __index = 0 To __slaveReference.XRange - 1
            utilitiesUI.controlFollowBoolean(__xSubitemList(__index), (__slaveReference.ReadDeviceValue(dmtModbusSlave.categroryCodeEnum.X, 0) >> __index) And &H1)
        Next
        For __index = 0 To __slaveReference.YRange - 1
            utilitiesUI.controlFollowBoolean(__ySubitemList(__index), __slaveReference.ReadDeviceBit(dmtModbusSlave.categroryCodeEnum.Y, __index))
        Next
        PropertyGridMasterInfo.Refresh()

        ResumeLayout()
    End Sub
    ''' <summary>
    ''' Flip Current Status
    ''' </summary>
    ''' <remarks></remarks>
    Sub YclickHandler(sender As Label, e As EventArgs)
        Dim serialBit As Integer = __ySubitemList.IndexOf(sender)
        __slaveReference.WriteDeviceBit(dmtModbusSlave.categroryCodeEnum.Y, serialBit) =
             Not __slaveReference.ReadDeviceBit(dmtModbusSlave.categroryCodeEnum.Y, serialBit)
    End Sub


End Class
