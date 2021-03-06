﻿Imports Automation.Components.CommandStateMachine
Imports System.ComponentModel
Imports Automation
Imports SmarPodAssembly
Imports System.Linq.Expressions
Imports System.Windows.Forms

Public Class userControlSmarPod
    Property SmarPodReference As smarPodControl
    Property PropertyView As userControlPropertyView

    Dim __form As System.Windows.Forms.Form
    'Dim simultanousCommands As List(Of cMotorPoint) = New List(Of cMotorPoint)  'Hsien , 2015.01.25 , support simulatanous command

    Private Sub loadUserControl(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        'UserControlPropertyViewMotor.Drive = Motor
        Me.SplitContainer1.Panel2.Controls.Add(PropertyView)
        PropertyView.Dock = Windows.Forms.DockStyle.Fill

        'For Each pair As KeyValuePair(Of [Enum], Short) In _smarPod.PositionDictionary
        '    ComboBox1stPositions.Items.Add(pair.Key)
        'Next


        Timer1.Interval = 50
        Timer1.Enabled = True
    End Sub



    Private Sub ButtonRelMove_Click(sender As Object, e As EventArgs) Handles _
        ButtonXplus.Click, ButtonXminus.Click,
        ButtonYplus.Click, ButtonYminus.Click,
        ButtonZplus.Click, ButtonZminus.Click,
        ButtonRXplus.Click, ButtonRXminus.Click,
        ButtonRYplus.Click, ButtonRYminus.Click,
        ButtonRZplus.Click, ButtonRZminus.Click

        'SmarPodReference.GetPose()
        'SmarPodReference.mTargetPose.Px = SmarPodReference.__currentPose.Px
        'SmarPodReference.mTargetPose.Py = SmarPodReference.__currentPose.Py
        'SmarPodReference.mTargetPose.Pz = SmarPodReference.__currentPose.Pz
        'SmarPodReference.mTargetPose.Rx = SmarPodReference.__currentPose.Rx
        'SmarPodReference.mTargetPose.Ry = SmarPodReference.__currentPose.Ry
        'SmarPodReference.mTargetPose.Rz = SmarPodReference.__currentPose.Rz

        'Select Case sender.name
        '    Case ButtonXplus.Name : SmarPodReference.mTargetPose.Px += Val(ComboBoxRelDistance.Text)
        '    Case ButtonXminus.Name : SmarPodReference.mTargetPose.Px -= Val(ComboBoxRelDistance.Text)
        '    Case ButtonYplus.Name : SmarPodReference.mTargetPose.Py += Val(ComboBoxRelDistance.Text)
        '    Case ButtonYminus.Name : SmarPodReference.mTargetPose.Py -= Val(ComboBoxRelDistance.Text)
        '    Case ButtonZplus.Name : SmarPodReference.mTargetPose.Pz += Val(ComboBoxRelDistance.Text)
        '    Case ButtonZminus.Name : SmarPodReference.mTargetPose.Pz -= Val(ComboBoxRelDistance.Text)

        '    Case ButtonRXplus.Name : SmarPodReference.mTargetPose.Rx += Val(ComboBoxRelDegree.Text)
        '    Case ButtonRXminus.Name : SmarPodReference.mTargetPose.Rx -= Val(ComboBoxRelDegree.Text)
        '    Case ButtonRYplus.Name : SmarPodReference.mTargetPose.Ry += Val(ComboBoxRelDegree.Text)
        '    Case ButtonRYminus.Name : SmarPodReference.mTargetPose.Ry -= Val(ComboBoxRelDegree.Text)
        '    Case ButtonRZplus.Name : SmarPodReference.mTargetPose.Rz += Val(ComboBoxRelDegree.Text)
        '    Case ButtonRZminus.Name : SmarPodReference.mTargetPose.Rz -= Val(ComboBoxRelDegree.Text)

        'End Select
        'Dim status As Smarpod.Status = _smarPod.Move
        'If status <> Smarpod.Status.SMARPOD_OK Then
        '    MsgBox(String.Format("Move Error : {0}", [Enum].GetName(GetType(Smarpod.Status), status)))
        '    SmarPodReference.GetPose()
        '    SmarPodReference.mTargetPose.Px = SmarPodReference.__currentPose.Px
        '    SmarPodReference.mTargetPose.Py = SmarPodReference.__currentPose.Py
        '    SmarPodReference.mTargetPose.Pz = SmarPodReference.__currentPose.Pz
        '    SmarPodReference.mTargetPose.Rx = SmarPodReference.__currentPose.Rx
        '    SmarPodReference.mTargetPose.Ry = SmarPodReference.__currentPose.Ry
        '    SmarPodReference.mTargetPose.Rz = SmarPodReference.__currentPose.Rz
        'End If

    End Sub
    Private Function GetPropertyName(Of T)(prop As Expression(Of Func(Of T))) As String
        Dim expression = GetMemberInfo(prop)
        Return expression.Member.Name
    End Function
    Private Shared Function GetMemberInfo(method As Expression) As MemberExpression
        Dim lambda As LambdaExpression = TryCast(method, LambdaExpression)
        If lambda Is Nothing Then
            Throw New ArgumentNullException("method")
        End If

        Dim memberExpr As MemberExpression = Nothing

        If lambda.Body.NodeType = ExpressionType.Convert Then
            memberExpr = TryCast(DirectCast(lambda.Body, UnaryExpression).Operand, MemberExpression)
        ElseIf lambda.Body.NodeType = ExpressionType.MemberAccess Then
            memberExpr = TryCast(lambda.Body, MemberExpression)
        End If

        If memberExpr Is Nothing Then
            Throw New ArgumentException("method")
        End If

        Return memberExpr
    End Function

    Private Sub ButtonMoveToZero_Click(sender As Object, e As EventArgs) Handles ButtonMoveToZero.Click
        'With SmarPodReference.mTargetPose
        '    .Px = 0
        '    .Py = 0
        '    .Pz = 0
        '    .Rx = 0
        '    .Ry = 0
        '    .Rz = 0
        'End With
        'Dim status As Smarpod.Status = _smarPod.Move
        'If status <> Smarpod.Status.SMARPOD_OK Then
        '    _smarPod.Move()
        '    MsgBox(String.Format("Move Error : {0}", [Enum].GetName(GetType(Smarpod.Status), status)))
        'End If

    End Sub

    Private Sub ButtonFindReferenceMark_Click(sender As Object, e As EventArgs) Handles ButtonFindReferenceMark.Click
        'SmarPodReference.FindReferenceMarks()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ''SmarPodReference.GetPose()
        'TextBox_Px.Text = Format(SmarPodReference.__commandPose.Px, "0.00")
        'TextBox_Py.Text = Format(SmarPodReference.__commandPose.Py, "0.00")
        'TextBox_Pz.Text = Format(SmarPodReference.__commandPose.Pz, "0.00")
        'TextBox_Rx.Text = Format(SmarPodReference.__commandPose.Rx, "0.000")
        'TextBox_Ry.Text = Format(SmarPodReference.__commandPose.Ry, "0.000")
        'TextBox_Rz.Text = Format(SmarPodReference.__commandPose.Rz, "0.000")
    End Sub

    Private Sub ButtonMove_Click(sender As Object, e As EventArgs) Handles ButtonMove.Click
        'Dim endStatus = SmarPodReference.CommandEndStatus 'reset it
        'If (ComboBox1stPositions.SelectedItem IsNot Nothing) Then
        '    _smarPod.drive(ComboBoxCommand.SelectedItem, ComboBox1stPositions.SelectedItem)
        'Else
        '    _smarPod.drive(ComboBoxCommand.SelectedItem)
        'End If

    End Sub
End Class
