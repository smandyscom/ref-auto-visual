Imports AutoNumeric

Public Class userControlFrameManagers

    WriteOnly Property AssemblyReference As Assembly
        Set(value As Assembly)
            __assemblyReference = value
            loadControl()
        End Set
    End Property

    Dim WithEvents __assemblyReference As Assembly = Nothing

    Sub loadControl() Handles MyBase.Load

        If __assemblyReference Is Nothing Then
            Exit Sub
        End If

        With UserControlFrameControlElementX
            .AxisEntity = AutoNumeric.axisEntityEnum.X
            .AxisReference = s0Htm.Instance
        End With
        With UserControlFrameControlElementY
            .AxisEntity = AutoNumeric.axisEntityEnum.Y
            .AxisReference = c4htm.Instance
        End With
        With UserControlFrameControlElementZ
            .AxisEntity = AutoNumeric.axisEntityEnum.Z
            .AxisReference = c4htm.Instance
        End With

        Dim counter As Integer = 0
        For Each item As userControlFrameControlElement In {UserControlFrameControlElementSX,
                                                            UserControlFrameControlElementSY,
                                                            UserControlFrameControlElementSZ,
                                                            UserControlFrameControlElementSA,
                                                            UserControlFrameControlElementSB,
                                                            UserControlFrameControlElementSC}
            item.AxisEntity = [Enum].ToObject(GetType(axisEntityEnum), counter)
            item.AxisReference = sHtm.Instance
            counter += 1
        Next

        Dim __itemList As Array = Automation.utilities.enumObjectsListing(GetType(itemsDefinition))
        Dim __positions As List(Of positionNode) = New List(Of positionNode)

        For Each item As itemsDefinition In __itemList
            __positions.Add(New positionNode(item))
        Next
        With Me.DataGridViewCoordinates
            .DataSource = __positions
            .RowHeadersVisible = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        End With

        timerScan.Enabled = True
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub axesMoving(sender As Object, e As controlUnitsEventArgs) Handles __assemblyReference.UnitStatusChanged
        'this control is not loaded yet
        Me.Invoke(Sub()
                      GroupBoxAxisControl.Enabled = Assembly.Instance.IsAllAxesSettled
                      GroupBoxSmarpod.Enabled = Assembly.Instance.IsAllAxesSettled
                  End Sub)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub smarpodPoseControl(sender As Button, e As EventArgs) Handles ButtonLoadPose.Click,
        ButtonSafePose.Click

        With sHtm.Instance
            Select Case sender.Name
                Case ButtonLoadPose.Name
                    .ControlVector = .LoadingPose
                Case ButtonSafePose.Name
                    .ControlVector = .SafePose
            End Select
        End With

    End Sub


    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles timerScan.Tick
        With Me.DataGridViewCoordinates
            .Update()
            .Refresh()
        End With
    End Sub
End Class
''' <summary>
''' Used to visualize current position for each item
''' </summary>
''' <remarks></remarks>
Public Class positionNode

    ReadOnly Property Name As String
        Get
            Return __name
        End Get
    End Property
    ReadOnly Property CurrentPosition As String
        Get
            Return (frames.Instance.Transformation(__item.ReferencedFrame, framesDefinition.R) * __item).RawValue.ToVectorString.Replace(vbCrLf, ",")
        End Get
    End Property

    Dim __item As PositionVector = Nothing
    Dim __name As String = Nothing
    Sub New(____item As itemsDefinition)
        __item = frames.Instance.objectsDictionary(____item)
        __name = ____item.ToString
    End Sub

End Class