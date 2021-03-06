﻿Imports AutoNumeric
Public Class userControlFrameControlElement

    WriteOnly Property AxisReference As IAxis
        Set(value As IAxis)
            __axisReference = value
            loadControl()
        End Set
    End Property
    WriteOnly Property AxisEntity As axisEntityEnum
        Set(value As axisEntityEnum)
            __axisEntity = value
            loadControl()
        End Set
    End Property

    Dim __axisReference As IAxis = Nothing
    Dim __axisEntity As axisEntityEnum = axisEntityEnum.X

    Sub loadControl() Handles Me.Load

        timerScan.Enabled = False

        If __axisReference Is Nothing Then
            Exit Sub
        End If

        LabelName.Text = __axisEntity.ToString
        timerScan.Enabled = True

    End Sub

    Sub jogControl(sender As Button, e As EventArgs) Handles ButtonForward.Click,
        ButtonBackward.Click

        Dim value = CDbl(TextBoxJogValue.Text)

        Select Case sender.Name
            Case ButtonForward.Name
            Case ButtonBackward.Name
                'reverse
                value *= -1
        End Select

        __axisReference.AxisControlValue(__axisEntity) += value

    End Sub

    Private Sub timerTicks(sender As Object, e As EventArgs) Handles timerScan.Tick
        TextBoxCurrentValue.Text = String.Format("{0:F5}", __axisReference.AxisControlValue(__axisEntity))
        TextBoxRealValue.Text = String.Format("{0:F5}", __axisReference.AxisFeedbackValue(__axisEntity))
    End Sub
End Class
