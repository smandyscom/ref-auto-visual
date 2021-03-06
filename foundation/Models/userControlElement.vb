﻿Imports System.ComponentModel
Imports Automation

Public Class userControlElement
    'used to display shiftDataPackBase
    'Hsien , 2015.03.27
    Property IsModuleActionVisualizing As Boolean
        Get
            'Return LabelModuleAction.Visible
            Return Me.Controls.Contains(LabelModuleAction)
        End Get
        Set(value As Boolean)
            'LabelModuleAction.Visible = value
            If (value) Then
                Me.Controls.Add(LabelModuleAction)
            Else
                Me.Controls.Remove(LabelModuleAction)
            End If

        End Set
    End Property
    Public Property ShiftDataReference As Func(Of shiftDataPackBase)  'as reference
        Get
            Return __shiftData
        End Get
        Set(value As Func(Of shiftDataPackBase))
            __shiftData = value
            ElementLoad(Me, Nothing)
        End Set
    End Property

    Dim __shiftData As Func(Of shiftDataPackBase)


    Private Sub elementContentShow(sender As Object, e As EventArgs) Handles LabelElement.Click
        'monitor the element data
        If (CType(e, MouseEventArgs).Button = Windows.Forms.MouseButtons.Right) Then
            Dim __propretyGrid As PropertyGrid = New PropertyGrid With {.SelectedObject = __shiftData.Invoke}
            Dim __dialog As Form = New Form
            With __dialog
                .Height = 600
                .Width = 800
                .Controls.Add(__propretyGrid)
                __propretyGrid.Dock = DockStyle.Fill
                Dim buttonCancel As New Button() With {.Text = "Cancel"} '2016.3.12 add cancel button for ESC hotkey to close form quickly
                .Controls.Add(buttonCancel)
                .CancelButton = buttonCancel
            End With
            __dialog.ShowDialog()
        Else
            __shiftData.Invoke.IsPositionOccupied = Not __shiftData.Invoke.IsPositionOccupied
        End If
    End Sub


    Private Sub timerRefresh(sender As Object, e As EventArgs) Handles timeScan.Tick
        'Hsien , 2015.04.22 , added module action
        If __shiftData Is Nothing OrElse
            __shiftData.Invoke Is Nothing Then
            Exit Sub
        End If
        utilitiesUI.controlFollowBooleanColor(LabelElement, __shiftData.Invoke.IsPositionOccupied, Color.Green)
        utilitiesUI.controlFollowBooleanColor(LabelModuleAction, __shiftData.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED), Color.GreenYellow)
    End Sub

    Private Sub ElementLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        If __shiftData Is Nothing Then
            Exit Sub
        End If
        timeScan.Enabled = True
    End Sub

    Private Sub moduleActionChange(sender As Object, e As EventArgs) Handles LabelModuleAction.Click
        __shiftData.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED,
                                                  Not __shiftData.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED))
    End Sub

End Class
