Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports AutoNumeric.PositionVector
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric

Imports AutoNumericTester.common

<TestClass()> Public Class positionVectorTest


    Enum frameDefines
        __R
    End Enum

    Dim __positionVector As PositionVector = Nothing

    <TestInitialize()>
    Public Sub initialize()
        __positionVector = New PositionVector(frameDefines.__R,
                                              "TEST")
    End Sub

    <TestMethod()> Public Sub SaveLoadTest()

        Dim rnd = CreateVector.Random(Of Double)(4)

        __positionVector.RawValue = rnd
        __positionVector.Save()

        initialize()

        Assert.IsFalse(__positionVector.RawValue.Equals(rnd))
        __positionVector.Load(Nothing)
        Assert.IsTrue((__positionVector.RawValue - rnd).ForAll(AddressOf isInTolerance), (__positionVector.RawValue - rnd).ToString)

    End Sub


    <TestMethod()>
    Public Sub RawValueTest()
        'prepare a random one
        Dim answer As Vector(Of Double) = CreateVector.Random(Of Double)(4)
        'assign
        Assert.IsFalse(__positionVector.RawValue.Equals(answer))
        __positionVector.RawValue = answer
        Assert.IsTrue(__positionVector.RawValue.Equals(answer))

    End Sub



End Class