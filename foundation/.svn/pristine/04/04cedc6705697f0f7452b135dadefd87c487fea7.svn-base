Public Class userControlCassetteTongueBuffer

    WriteOnly Property Reference As ICassetteTongueBuffer
        Set(value As ICassetteTongueBuffer)
            __reference = value
            loadControl()
        End Set
    End Property

    WriteOnly Property LayoutType As machineTypeEnum
        Set(value As machineTypeEnum)
            'reload table layout panel sequence
            'default: Left-Cassette (Right-mainequipment)
            UserControlCassette1.IsMirror = (value = machineTypeEnum.LEFT_MAINEQUIPMENT)
            If value = machineTypeEnum.LEFT_MAINEQUIPMENT Then
                With TableLayoutPanel1.Controls
                    .Clear()
                    LabelBufferCount.Anchor = AnchorStyles.Top Or AnchorStyles.Left
                    .Add(TableLayoutPanelTongueBuffer, 0, 0)
                    .Add(UserControlCassette1, 1, 1)
                End With

            End If
            loadControl()
        End Set
    End Property
    ''' <summary>
    ''' Flow direction , the tongue item sequence
    ''' False(default): left(0) to right(2)
    ''' True: right(0) to left(2)
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    WriteOnly Property IsFlowMirror As Boolean
        Set(value As Boolean)
            For Each item As userControlLane In {UserControlLane1}
                item.IsMirror = value
            Next
        End Set
    End Property

    WriteOnly Property IsEnableModuleAction As Boolean
        Set(value As Boolean)
            With UserControlLane1
                .IsElementModuleActionVisualizing = value
                .Enabled = value
            End With
        End Set
    End Property
    WriteOnly Property Title As String
        Set(value As String)
            Me.UserControlCassette1.GroupBoxTitle.Text = value
        End Set
    End Property

    Dim __reference As ICassetteTongueBuffer = Nothing
    Dim __tempTongue As shiftDataCollection = New shiftDataCollection With {.DataType = GetType(shiftDataPackBase), .DataCount = 3}

    Sub loadControl() Handles MyBase.Load

        If __reference Is Nothing Then
            Me.UserControlLane1.LaneReference(Function() (__tempTongue))
            Exit Sub
        End If

        Me.UserControlCassette1.CassetteReference = __reference.Cassette
        Me.UserControlLane1.LaneReference(Function() (__reference.Tongue.OccupiedStatus))

        'enable
        timerScan.Enabled = True
    End Sub


    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles timerScan.Tick
        With __reference
            utilitiesUI.controlFollowBoolean(UserControlLane1, CType(.Tongue, IFinishableStation).FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED))
            LabelBufferCount.Text = .BufferCounts
            utilitiesUI.controlFollowBoolean(UserControlCassette1, .Cassette._cassetteTransport._FinishableFlag.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED))
        End With
    End Sub
End Class

''' <summary>
''' Offered the regulated interface for UI
''' </summary>
''' <remarks></remarks>
Public Interface ICassetteTongueBuffer

    ReadOnly Property Cassette As cassetteSystemBase
    ReadOnly Property Tongue As shiftingModel
    ReadOnly Property BufferCounts As Integer
End Interface