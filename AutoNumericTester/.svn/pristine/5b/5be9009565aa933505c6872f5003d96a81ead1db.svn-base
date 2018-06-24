Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AutoNumeric
<TestClass()> Public Class dataLoggerTest

    <TestMethod()> Public Sub dataLoggerTest1()

        Dim __dataLogger As dataLogger = New dataLogger(Me.ToString)

        For index = 0 To 100
            __dataLogger.write(index.ToString)
        Next
        __dataLogger.writeLine("Closed", True)

    End Sub

    <TestMethod()> Public Sub dataLoggerTest2()
        Dim __dataLogger As dataLogger = Nothing
        For count = 0 To 5
            __dataLogger = New dataLogger(Me.ToString)

            For index = 0 To 100
                __dataLogger.writeLine(index.ToString)
            Next
            __dataLogger.writeLine("Closed", True)
            '__dataLogger.Dispose()
        Next
    End Sub

End Class