﻿Imports Automation

<Serializable()>
Public Class userControlLane
    Property IsMirror As Boolean = False    'control the flow direction
    Property IsElementModuleActionVisualizing As Boolean
        Get
            Return __isElementModuleActionVisualizing
        End Get
        Set(value As Boolean)
            __isElementModuleActionVisualizing = value

            For Each __control As Control In tableLayoutPanelLane.Controls
                Dim __element As userControlElement = TryCast(__control, userControlElement)
                If (__element IsNot Nothing) Then
                    __element.IsModuleActionVisualizing = value
                End If
            Next

        End Set
    End Property
    Dim __isElementModuleActionVisualizing As Boolean = True

    Public Sub LaneReference(value As Func(Of shiftDataCollection)) 'as reference 

        If (value Is Nothing) Then
            Exit Sub
        End If

        If Me.InvokeRequired Then
            'Backgroud Thread
            Me.Invoke(Sub()
                          UILayout(value)
                      End Sub)
        Else
            'UI Thread
            UILayout(value)
        End If

    End Sub
    Private Sub UILayout(value As Func(Of shiftDataCollection))
        timerScan.Enabled = False
        tableLayoutPanelLane.Controls.Clear()
        GC.Collect()

        __laneData = value
        laneLoad(Me, Nothing)
    End Sub
    Property IsLaneOccupiedVisual As Boolean
        Get
            Return __isLaneOccupiedVisual
        End Get
        Set(value As Boolean)
            __isLaneOccupiedVisual = value
        End Set
    End Property
    Dim _elementNumber As Integer = 1
    Property elementNumber As Integer
        Set(value As Integer)
            _elementNumber = value
            tempLaneData = New shiftDataCollection With {.DataCount = _elementNumber}
            tableLayoutPanelLane.Controls.Clear()
            For index As Integer = 0 To _elementNumber - 1
                establishElements(index)
            Next
        End Set
        Get
            Return _elementNumber
        End Get
    End Property
    Property IsLaneModuleActionVisual As Boolean
        Get
            Return __isLaneModuleActionVisual
        End Get
        Set(value As Boolean)
            __isLaneModuleActionVisual = value
        End Set
    End Property

    Dim __laneData As Func(Of shiftDataCollection) = Function() (tempLaneData)

    Dim __isLaneOccupiedVisual As Boolean = False
    Dim __isLaneModuleActionVisual As Boolean = False

    Dim tempLaneData As shiftDataCollection = New shiftDataCollection With {.DataCount = 1}

    Private Sub laneLoad(sender As Object, e As EventArgs) Handles MyBase.Load

        If __laneData Is Nothing Then
            Exit Sub
        End If



        tableLayoutPanelLane.Controls.Clear()

        If (IsMirror) Then
            For index = __laneData.Invoke.DataCollection.Count - 1 To 0 Step -1
                establishElements(index)
            Next
        Else
            For index = 0 To __laneData.Invoke.DataCollection.Count - 1
                establishElements(index)
            Next
        End If

        'timerScan.Enabled = True

    End Sub

    Sub establishElements(______index As Integer)
        Try
            Dim __element As userControlElement = New userControlElement With {.IsModuleActionVisualizing = IsElementModuleActionVisualizing}
            Dim __index As Integer = ______index
            __element.ShiftDataReference = Function()
                                               If __laneData.Invoke.DataCollection.Count > __index Then
                                                   Return __laneData.Invoke.DataCollection(__index)
                                               Else
                                                   Return New shiftDataPackBase
                                               End If
                                           End Function
            __element.LabelElement.Text = __index ' mark the location number
            tableLayoutPanelLane.Controls.Add(__element)
            tableLayoutPanelLane.PerformLayout()
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace.ToString)
        End Try
    End Sub

End Class
