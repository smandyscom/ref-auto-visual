Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FA
Imports System.IO
Imports FA.s0Htm
<TestClass()> Public Class s0HtmTest

    Dim rnd As Random = New Random(Now.Millisecond)

    <TestMethod()> Public Sub saveLoadTest()
        With s0Htm.Instance
            Dim ans = rnd.NextDouble
            .ZeroOffset = ans
            .Save()
            'reset
            .ZeroOffset = 0
            Assert.IsTrue(.ZeroOffset = 0)
            .Load(Nothing)
            Assert.IsTrue(.ZeroOffset = ans)
        End With
    End Sub

    <TestMethod()> Public Sub axisValueTest()
        With s0Htm.Instance
            Dim ans = rnd.NextDouble
            .AxisValue(AutoNumeric.axisEntityEnum.X) = ans
            Assert.IsTrue(.AxisValue(AutoNumeric.axisEntityEnum.X) = ans)
        End With
    End Sub

End Class