﻿Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric.errorVectorIndexesEnum
Imports System.Xml.Serialization

''' <summary>
''' Indicate the index meaning of error vector
''' </summary>
''' <remarks></remarks>
Enum errorVectorIndexesEnum
    SX = 0
    SY
    SZ
    EX
    EY
    EZ = 5
End Enum
''' <summary>
''' The error matrix
''' </summary>
''' <remarks></remarks>
Public Class errorHtm
    Inherits htmEdgeElementary

    ''' <summary>
    ''' Raised when error had been identified
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event ErrorContentUpdated(ByVal sender As Object, ByVal e As EventArgs)

    <XmlIgnore()>
    ReadOnly Property LastUpdateTime As String
        Get
            Return __lastUpdateTime.ToString("MMddhhmmss")
        End Get
    End Property
    Dim __lastUpdateTime As Date = Now

    ''' <summary>
    ''' In radien
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    ReadOnly Property ErrorVectorString
        Get
            Return ErrorVector.ToVectorString.Replace(vbCrLf, ";")
        End Get
    End Property

    ''' <summary>
    ''' The 6x1 vector
    ''' in sequence of ex,ey,ez,sx,sy,sz
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Property ErrorVector As Vector(Of Double)
        Get
            Return CreateVector.Dense(Of Double)({__matrixCore(0, 3),
                                                  __matrixCore(1, 3),
                                                  __matrixCore(2, 3),
                                                  __matrixCore(2, 1),
                                                  __matrixCore(0, 2),
                                                  __matrixCore(1, 0)})
        End Get
        Set(value As Vector(Of Double))
            __matrixCore(2, 1) = value(EX)
            __matrixCore(1, 2) = -value(EX)

            __matrixCore(0, 2) = value(EY)
            __matrixCore(2, 0) = -value(EY)

            __matrixCore(1, 0) = value(EZ)
            __matrixCore(0, 1) = -value(EZ)

            __matrixCore(0, 3) = value(SX)
            __matrixCore(1, 3) = value(SY)
            __matrixCore(2, 3) = value(SZ)

            __lastUpdateTime = Now
            RaiseEvent ErrorContentUpdated(Me, EventArgs.Empty)
        End Set
    End Property


    Sub New(__from As [Enum],
            __to As [Enum])
        MyBase.New(__from, __to)
    End Sub

End Class
