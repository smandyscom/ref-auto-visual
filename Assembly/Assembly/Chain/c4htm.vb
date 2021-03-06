﻿Imports AutoNumeric
Imports System.Xml.Serialization

''' <summary>
''' Singleton
''' C4->Y0REAL
''' </summary>
''' <remarks></remarks>
Public Class c4htm
    Inherits htmEdgeElementary
    Implements IAxis

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event TransformationChanged(ByVal sender As Object, ByVal e As EventArgs)


    <XmlIgnore()>
    Public Property AxisValue(entity As axisEntityEnum) As Double Implements IAxis.AxisControlValue
        Get
            Select Case entity
                Case axisEntityEnum.X
                    Throw New NotImplementedException
                Case axisEntityEnum.Y
                    'update ayFromHome
                    __ayFromZero = -Me.PositionVector(axisEntityEnum.Y) - YZeroOffset
                    Return __ayFromZero
                Case axisEntityEnum.Z
                    'update azFromHome
                    __azFromZero = -Me.PositionVector(axisEntityEnum.Z) - ZZeroOffset
                    Return __azFromZero
                Case Else
                    Throw New NotImplementedException
            End Select
        End Get
        Set(value As Double)

            Dim previous = Me.PositionVector

            Select Case entity
                Case axisEntityEnum.X
                    Throw New NotImplementedException
                Case axisEntityEnum.Y
                    __ayFromZero = value
                    'update
                    previous(axisEntityEnum.Y) = (__ayFromZero + YZeroOffset) * -1
                Case axisEntityEnum.Z
                    __azFromZero = value
                    'update
                    previous(axisEntityEnum.Z) = (__azFromZero + ZZeroOffset) * -1
            End Select

            Me.PositionVector = previous

            RaiseEvent TransformationChanged(Me, EventArgs.Empty)

        End Set
    End Property
    <XmlIgnore()>
    Public ReadOnly Property AxisFeedbackValue(entity As axisEntityEnum) As Double Implements IAxis.AxisFeedbackValue
        Get
            With Assembly.Instance
                Select Case entity
                    Case axisEntityEnum.X
                        Throw New NotImplementedException
                    Case axisEntityEnum.Y
                        Return (.yMotorControl.pulse2Unit(.yMotorControl.FeedBackPosition) + YZeroOffset) * -1
                    Case axisEntityEnum.Z
                        Return (.zMotorControl.pulse2Unit(.zMotorControl.FeedBackPosition) + ZZeroOffset) * -1
                End Select
            End With
        End Get
    End Property

    Dim __ayFromZero As Double = 0
    Dim __azFromZero As Double = 0

    ''' <summary>
    ''' Need to be stored
    ''' The home position reference to previous frame
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property YZeroOffset As Double = 0
    Property ZZeroOffset As Double = 0

    Protected Sub New()
        MyBase.New(framesDefinition.C4,
                   framesDefinition.Y0REAL)
    End Sub

    Shared __instance As c4htm = Nothing
    Public Shared ReadOnly Property Instance As c4htm
        Get
            If __instance Is Nothing Then
                __instance = New c4htm
            End If
            Return __instance
        End Get
    End Property

    Public Overrides Sub Load(filename As String)
        MyBase.Load(filename)
        'reconstruct Ay,Az
        Me.AxisValue(axisEntityEnum.Y) = 0
        Me.AxisValue(axisEntityEnum.Z) = 0
    End Sub

    Public Overrides Function Clone() As Object
        Dim output As c4htm = New c4htm
        With output
            .RawValue = Me.RawValue.Clone
            .YZeroOffset = Me.YZeroOffset
            .ZZeroOffset = Me.ZZeroOffset
            .__ayFromZero = Me.__ayFromZero
            .__azFromZero = Me.__azFromZero
        End With
        Return output
    End Function

    
End Class
