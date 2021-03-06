﻿Public Class userControlMaterial

    WriteOnly Property Data As materialData
        Set(value As materialData)
            __data = value
            loadControl()
        End Set
    End Property

    Dim __data As materialData = Nothing
    Dim __dialog As Form = New Form
    Dim __propertyGrid As PropertyGrid = New PropertyGrid
    Dim isInitialized As Boolean = False

    Sub loadControl() Handles MyBase.Load

        If __data Is Nothing Or
            isInitialized Then
            Exit Sub
        End If

        With __propertyGrid
            .SelectedObject = __data
        End With
        With __dialog
            .Controls.Add(__propertyGrid)
            .AutoSize = True
            .AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
            .StartPosition = FormStartPosition.CenterScreen
        End With

        isInitialized = True
    End Sub


    Private Sub raisePropertyWindow(sender As Object, e As EventArgs) Handles PictureBoxBondImage.DoubleClick
        '
        __dialog.Show()
    End Sub

    Private Sub checkBoxIsBondingCheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIsBonding.CheckedChanged
        __data.IsEnagedToBond = True
    End Sub
End Class
