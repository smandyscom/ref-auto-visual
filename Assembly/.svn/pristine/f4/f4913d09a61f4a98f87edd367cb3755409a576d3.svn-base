Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FA

<TestClass()> Public Class c4HtmTest

    Dim rnd As Random = New Random(Now.Millisecond)

    <TestMethod()> Public Sub saveLoadTest()
        Dim ans = rnd.NextDouble

        With c4htm.Instance
            .YZeroOffset = ans
            .Save()
            'reset
            .YZeroOffset = 0
            Assert.IsTrue(.YZeroOffset = 0)
            .Load(Nothing)
            Assert.IsTrue(.YZeroOffset = ans)
        End With
    End Sub

End Class