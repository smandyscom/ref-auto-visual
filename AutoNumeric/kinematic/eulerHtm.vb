Imports MathNet.Numerics.LinearAlgebra
Imports System.Xml.Serialization

''' <summary>
''' Tranlation before Rotation
''' XYZ->A->B->C
''' Offers IK as well
''' </summary>
''' <remarks></remarks>
Public Class eulerHtmTR
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
    ReadOnly Property ControlVectorString
        Get
            Return ControlVector.ToVectorString.Replace(vbCrLf, ";")
        End Get
    End Property
   
    ''' <summary>
    ''' Offered the forward/inverse kinematic
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Overridable Property ControlVector As Vector(Of Double)
        Get
            'solve inverse kinematic
            Return CreateVector.Dense(Of Double)({__matrixCore(0, 3),
                                                  __matrixCore(1, 3),
                                                  __matrixCore(2, 3),
                                                  Math.Atan2(__matrixCore(2, 1), __matrixCore(2, 2)),
                                                  Math.Asin(-__matrixCore(2, 0)),
                                                  Math.Atan2(__matrixCore(1, 0), __matrixCore(0, 0))})
        End Get
        Set(value As Vector(Of Double))
            'solve forward kinematic
            Dim rotation = utilities.RotateTransformation(value(axisEntityEnum.A),
                                                          value(axisEntityEnum.B),
                                                          value(axisEntityEnum.C))
            MyBase.RotationMatrix = rotation
            MyBase.PositionVector = CreateVector.Dense(Of Double)({value(axisEntityEnum.X),
                                                                   value(axisEntityEnum.Y),
                                                                   value(axisEntityEnum.Z),
                                                                   1})
            RaiseEvent ErrorContentUpdated(Me, EventArgs.Empty)
        End Set
    End Property


    Sub New(__from As [Enum],
           __to As [Enum])
        MyBase.New(__from, __to)
    End Sub
    ''' <summary>
    ''' Used for serializing
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
        Me.New(Nothing,
               Nothing)
    End Sub

End Class

''' <summary>
''' Rotation Before Translation
''' TODO
''' </summary>
''' <remarks></remarks>
Public Class eulerHtmRT
    Inherits htmEdgeElementary

    Sub New(__from As [Enum],
           __to As [Enum])
        MyBase.New(__from, __to)
    End Sub

End Class

