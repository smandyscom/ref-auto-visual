﻿Imports AutoNumeric
Imports SmarPodAssembly
Imports SmarPodAssembly.smarPodControl.podCommands
Imports Automation
Imports System.ComponentModel
Imports MathNet.Numerics.LinearAlgebra

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class sHtm
    Inherits eulerHtmTR
    Implements IAxis


    Public Event TransformationChanged(ByVal sender As Object, ByVal e As EventArgs)

    Property SafePose As Vector(Of Double) = CreateVector.Dense(Of Double)({0,
                                                                             0,
                                                                             -5.5,
                                                                             0,
                                                                             0,
                                                                             0})
    Property LoadingPose As Vector(Of Double) = CreateVector.Dense(Of Double)({0,
                                                                               0,
                                                                               0,
                                                                               -0.17,
                                                                               0,
                                                                               0})

    <Browsable(False)>
    Public Overrides Property RawValue As MathNet.Numerics.LinearAlgebra.Matrix(Of Double)
        Get
            Return MyBase.RawValue
        End Get
        Set(value As MathNet.Numerics.LinearAlgebra.Matrix(Of Double))
            MyBase.RawValue = value
            'trigger the smarpod moing
            Me.ControlVector = Me.ControlVector
        End Set
    End Property
    <Browsable(False)>
    Public Overrides Property RotationMatrix As MathNet.Numerics.LinearAlgebra.Matrix(Of Double)
        Get
            Return MyBase.RotationMatrix
        End Get
        Set(value As MathNet.Numerics.LinearAlgebra.Matrix(Of Double))
            MyBase.RotationMatrix = value
            'trigger the smarpod moing
            Me.ControlVector = Me.ControlVector
        End Set
    End Property
    <Browsable(False)>
    Public Overrides Property PositionVector As MathNet.Numerics.LinearAlgebra.Vector(Of Double)
        Get
            Return MyBase.PositionVector
        End Get
        Set(value As MathNet.Numerics.LinearAlgebra.Vector(Of Double))
            MyBase.PositionVector = value
            'trigger the smarpod moing
            Me.ControlVector = Me.ControlVector
        End Set
    End Property

    ReadOnly Property PodCommand As podCommandBase
        Get
            Return __podCommand
        End Get
    End Property
    Dim __podCommand As podCommandBase = New podCommandBase

    <Browsable(False)>
    Public Property AxisValue(entity As axisEntityEnum) As Double Implements IAxis.AxisControlValue
        Get
            Return MyBase.ControlVector(entity)
        End Get
        Set(value As Double)
            Dim previous = Me.ControlVector
            previous(entity) = value
            'trigger the control
            Me.ControlVector = previous
        End Set
    End Property
    Public ReadOnly Property AxisFeedbackValue(entity As axisEntityEnum) As Double Implements IAxis.AxisFeedbackValue
        Get
            With Assembly.Instance.__smarPodControl.RealPose
                Select Case entity
                    Case axisEntityEnum.X
                        Return .Px
                    Case axisEntityEnum.Y
                        Return .Py
                    Case axisEntityEnum.Z
                        Return .Pz
                    Case axisEntityEnum.A
                        Return .Rx
                    Case axisEntityEnum.B
                        Return .Ry
                    Case axisEntityEnum.C
                        Return .Rz
                End Select
            End With
        End Get
    End Property
    ''' <summary>
    ''' Integrate coordinate system and actuator
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Property ControlVector As MathNet.Numerics.LinearAlgebra.Vector(Of Double)
        Get
            Return MyBase.ControlVector
        End Get
        Set(value As MathNet.Numerics.LinearAlgebra.Vector(Of Double))
            MyBase.ControlVector = value
            'transform the control vector to podCommnad
            With __podCommand
                .Px = value(axisEntityEnum.X)
                .Py = value(axisEntityEnum.Y)
                .Pz = value(axisEntityEnum.Z)
                .Rx = value(axisEntityEnum.A)
                .Ry = value(axisEntityEnum.B)
                .Rz = value(axisEntityEnum.C)
            End With
            RaiseEvent TransformationChanged(Me, EventArgs.Empty)
        End Set
    End Property

    Shared ReadOnly Property Instance As sHtm
        Get
            If __instance Is Nothing Then
                __instance = New sHtm
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As sHtm = Nothing
    Protected Sub New()
        MyBase.New(framesDefinition.S,
                   framesDefinition.S0)
    End Sub


   
End Class

