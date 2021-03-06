﻿Imports AutoNumeric
Imports System.Xml.Serialization
Imports Automation
Imports System.IO
Imports System.ComponentModel

''' <summary>
''' Singleton
''' S0->R
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class s0Htm
    Inherits htmEdgeElementary
    Implements IAxis

    Public Event TransformationChanged(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' The Ax Control Value From Zero Point
    ''' Also the motorX command Value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public Property AxisValue(entity As axisEntityEnum) As Double Implements IAxis.AxisControlValue
        Get
            'update axfromHome
            __axFromZero = Me.PositionVector(axisEntityEnum.X) - ZeroOffset
            Return __axFromZero
        End Get
        Set(value As Double)
            __axFromZero = value
            'update current status
            Dim previous = MyBase.PositionVector
            previous(axisEntityEnum.X) = __axFromZero + ZeroOffset
            'update whole position vector
            PositionVector = previous
            RaiseEvent TransformationChanged(Me, EventArgs.Empty)
        End Set
    End Property
    Public ReadOnly Property AxisFeedbackValue(entity As axisEntityEnum) As Double Implements IAxis.AxisFeedbackValue
        Get
            With Assembly.Instance
                Return .xMotorControl.pulse2Unit(.xMotorControl.FeedBackPosition) + ZeroOffset
            End With
        End Get
    End Property

    Protected __axFromZero As Double = 0

    ''' <summary>
    ''' Need to be stored
    ''' The home position from reference frame
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ZeroOffset As Double = 0

    Protected Sub New()
        MyBase.New(framesDefinition.S0,
                   framesDefinition.R)
    End Sub

    Shared __instance As s0Htm = New s0Htm

    Public Shared ReadOnly Property Instance As s0Htm
        Get
            If __instance Is Nothing Then
                __instance = New s0Htm
            End If
            Return __instance
        End Get
    End Property

    Public Overrides Sub Load(filename As String)
        MyBase.Load(filename)
        'reconstruct Ax
        Me.AxisValue(axisEntityEnum.X) = 0
    End Sub

    Public Overrides Function Clone() As Object
        Dim output As s0Htm = New s0Htm
        With output
            .RawValue = Me.RawValue.Clone
            .ZeroOffset = Me.ZeroOffset
            .__axFromZero = Me.__axFromZero
        End With

        Return output
    End Function

   
End Class
