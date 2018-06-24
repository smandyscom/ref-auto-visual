Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Numerics.LinearAlgebra.Matrix(Of Double)
Imports AutoNumericTester.common
Imports System.Diagnostics

<TestClass()> Public Class htmEdgeElementaryTest

    Enum frameDefines
        ___FROM
        ___TO
    End Enum

    Dim elementary As htmEdgeElementary = Nothing

    <TestInitialize()>
    Public Sub initialize()
        elementary = New htmEdgeElementary(frameDefines.___FROM, frameDefines.___TO)
    End Sub

    <TestMethod()>
    Public Sub RotationMatrixTest()
        'prepare a random one
        Dim answer As Matrix(Of Double) = CreateMatrix.RandomPositiveDefinite(Of Double)(3)
        'assign
        Assert.IsFalse(elementary.RotationMatrix.Equals(answer))
        elementary.RotationMatrix = answer
        Assert.IsTrue(elementary.RotationMatrix.Equals(answer))

    End Sub
    <TestMethod()>
    Public Sub FrameVectorTest()
        'prepare a random one
        Dim answer As Vector(Of Double) = CreateVector.Random(Of Double)(4)
        'assign
        Dim valueArray = [Enum].GetValues(GetType(frameVectorEnum))
        For index = 0 To valueArray.Length - 1
            Dim obj = [Enum].ToObject(GetType(frameVectorEnum), valueArray(index))
            Assert.IsFalse(elementary.FrameVector(obj).Equals(answer))
            elementary.FrameVector(obj) = answer
            Assert.IsTrue(elementary.FrameVector(obj).Equals(answer))
        Next
    End Sub
    <TestMethod()>
    Public Sub PositionVectorTest()
        'prepare a random one
        Dim answer As Vector(Of Double) = CreateVector.Random(Of Double)(4)
        'assign
        Assert.IsFalse(elementary.PositionVector.Equals(answer))
        elementary.PositionVector = answer
        Assert.IsTrue(elementary.PositionVector.Equals(answer))
    End Sub

    <TestMethod()>
    Public Sub SaveLoadTest()

        Dim rot = CreateMatrix.RandomPositiveDefinite(Of Double)(3)
        Dim pos = CreateVector.Random(Of Double)(4)

        elementary.RotationMatrix = rot
        elementary.PositionVector = pos

        elementary.Save()

        'destroy previous record
        initialize()

        Assert.IsFalse(elementary.RotationMatrix.Equals(rot))
        Assert.IsFalse(elementary.PositionVector.Equals(pos))
        elementary.Load(Nothing)
        Assert.IsTrue((elementary.RotationMatrix - rot).ForAll(AddressOf isInTolerance))
        Assert.IsTrue((elementary.PositionVector - pos).ForAll(AddressOf isInTolerance))

    End Sub

    <TestMethod()>
    Public Sub InverseTest()
        Dim rot = CreateVector.Random(Of Double)(3)
        Dim pos = CreateVector.Random(Of Double)(4)

        elementary.RotationMatrix = utilities.RotateTransformation(rot(0), rot(1), rot(2))
        elementary.Origin = New PositionVector(pos, Nothing)

        System.Diagnostics.Trace.WriteLine((elementary.Inverse * elementary).RawValue.ToMatrixString)

    End Sub

End Class