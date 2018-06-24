Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FA
Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra

<TestClass()> Public Class frameTest
    ''' <summary>
    ''' Check if exception
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub solveAbsAxAyTest1()
        With frames.Instance
            'Dim result = .solveAbsAxAy(itemsDefinition.C3_ORIGIN, framesDefinition.C4)
        End With
    End Sub

    ''' <summary>
    ''' Check if exception
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub solveSTest1()
        With frames.Instance
            'move the smart pod
            Dim transformationBallToTarget = New htmEdgeElementary(framesDefinition.BALL,
                                                                   framesDefinition.C3REAL)
            transformationBallToTarget.PositionVector = CreateVector.Dense(Of Double)({100,
                                                                                       200,
                                                                                       300,
                                                                                       1})
            transformationBallToTarget.RotationMatrix = AutoNumeric.utilities.RotateTransformation(Math.PI, Math.PI, Math.PI)

            'transformationBallToTarget.PositionVector = nominalPosition.Current.RawValue
            Dim result = frames.Instance.solveS(transformationBallToTarget)

            Trace.WriteLine(transformationBallToTarget.PositionVector.ToVectorString.Replace(vbCrLf, vbTab))
        End With
    End Sub

End Class