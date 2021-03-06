﻿Imports Automation

Public Class userControlFrameManager1

    WriteOnly Property AssemblyReference As Assembly
        Set(value As Assembly)
            __assemblyReference = value
        End Set
    End Property

    Dim WithEvents __assemblyReference As Assembly = Assembly.Instance

    'select ritem, moving
    Sub loadControl() Handles MyBase.Load

        utilitiesUI.loadComboBoxItemByEnum(ComboBoxRItems, GetType(itemsDefinition))
        With ComboBoxMovingItems.Items
            .Add(framesDefinition.S0)
            .Add(framesDefinition.C4)
            .Add(framesDefinition.LREAL)
            .Add(framesDefinition.DISP_HEAD_REAL)
            .Add(framesDefinition.BALL)
            .Add(framesDefinition.LPC_REAL)
        End With
        ComboBoxRItems.SelectedItem = itemsDefinition.CHOKE_CENTER
        ComboBoxMovingItems.SelectedItem = framesDefinition.S0
    End Sub

    Sub comboBoxItemSelected(sender As ComboBox, e As EventArgs) Handles ComboBoxRItems.SelectedValueChanged,
        ComboBoxMovingItems.SelectedValueChanged

        Select Case sender.Name
            Case ComboBoxRItems.Name
                frames.Instance.CurrentRItem = sender.SelectedItem
                PropertyGridRItem.SelectedObject = frames.Instance.CurrentRObject
            Case ComboBoxMovingItems.Name
                frames.Instance.CurrentMovingItem = sender.SelectedItem
        End Select

    End Sub

    Protected WriteOnly Property IsComboBoxEnabled As Boolean
        Set(value As Boolean)
            For Each item As ComboBox In {ComboBoxRItems,
                                     ComboBoxMovingItems}
                item.Enabled = value
            Next
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub stageStatusChanged(sender As Object, e As controlUnitsEventArgs) Handles __assemblyReference.UnitStatusChanged
        Me.Invoke(Sub() IsComboBoxEnabled = Not (e.Status = IDrivable.endStatus.EXECUTING))
    End Sub

End Class

