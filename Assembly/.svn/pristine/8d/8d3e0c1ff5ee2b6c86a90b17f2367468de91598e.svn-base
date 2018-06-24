Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FA
Imports AutoNumeric

<TestClass()> Public Class energyAlignmentTest

    <TestMethod()> Public Sub settingTestMethod1()

        Dim __searchingSetting As rectangleSearchRouteSetting = New rectangleSearchRouteSetting()

        With __searchingSetting
            .Start = New AutoNumeric.htmEdgeElementary(Nothing,
                                                       Nothing)
            .OriginOffsetX = 2
            .OriginOffsetY = 2
            .Height = 0.02

            .RangeX = 5
            .StepX = 1
            .RangeY = 5
            .StepY = 1
        End With

        Dim ___points As List(Of PositionVector) = __searchingSetting.MeasurePoints

        ___points.ForEach(Sub(__pt As PositionVector) Trace.WriteLine(__pt.RawValue.ToVectorString.Replace(vbCrLf, vbTab)))

        Trace.WriteLine("Origins")
        __searchingSetting.reset()
        Dim nt As htmEdgeElementary = __searchingSetting.NextTransformation
        While nt IsNot Nothing
            Trace.WriteLine(nt.Origin.RawValue.ToVectorString.Replace(vbCrLf, vbTab))
            nt = __searchingSetting.NextTransformation
        End While


        Assert.IsTrue(___points.TrueForAll(Function(__pt As PositionVector) __pt.Z = __searchingSetting.Height))

    End Sub

End Class