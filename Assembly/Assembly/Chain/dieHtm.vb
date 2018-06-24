Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra
Imports System.Xml.Serialization

Public Class dieHtm
    Inherits htmEdgeElementary

    Public Event OriginChanged(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' In mm
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DiePitch As Double = 23

    <XmlIgnore()>
    Property DieIndex As Integer
        Get
            Return __dieIndex
        End Get
        Set(value As Integer)
            Me.PositionVector = CreateVector.Dense(Of Double)({FirstDiePosition - value * DiePitch,
                                                               Me.PositionVector(axisEntityEnum.Y),
                                                               Me.PositionVector(axisEntityEnum.Z),
                                                               1})
            __dieIndex = value
            RaiseEvent OriginChanged(Me, EventArgs.Empty)
        End Set
    End Property
    Dim __dieIndex As Integer = 0

    ''' <summary>
    ''' X coordinate of first Die
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property FirstDiePosition As Double = 707

    Protected Sub New()
        MyBase.New(framesDefinition.DIE,
                   framesDefinition.R)
    End Sub

    Shared __instance As dieHtm = New dieHtm
    Public Shared ReadOnly Property Instance As dieHtm
        Get
            If __instance Is Nothing Then
                __instance = New dieHtm
            End If
            Return __instance
        End Get
    End Property

End Class
