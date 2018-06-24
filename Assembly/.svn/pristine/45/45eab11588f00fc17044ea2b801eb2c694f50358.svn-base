Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric
Imports AutoNumeric.fittingMethods.coeffsDefinition
Imports KellermanSoftware.CsvReports
Imports FA


<TestClass()> Public Class materialTest

    Function dataGenerator(coeff As Vector(Of Double)) As List(Of Vector(Of Double))
        Dim output As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
        Dim __generator As FA.rectangleSearchRouteSetting = New FA.rectangleSearchRouteSetting
        __generator.Start = New htmEdgeElementary(Nothing, Nothing)
        __generator.MeasurePoints.ForEach(Sub(__data As PositionVector)

                                              With __data
                                                  Dim ___data As Vector(Of Double) =
                                                      CreateVector.Dense(Of Double)({.X,
                                                                                     .Y,
                                                                                     fittingMethods.data3D(.X, .Y, coeff, fittingMethods.fitting3DMethodsEnum.DOUBLE_PARABOLA)})
                                                  output.Add(___data)
                                              End With

                                          End Sub)


        Return output
    End Function


    <TestMethod()> Public Sub TestMethod1()

        Dim __leftCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(6).Normalize(2)
        __leftCoeff = __leftCoeff.PointwiseAbs()
        __leftCoeff(E) = -1 * __leftCoeff(E)
        __leftCoeff(B) = 0
        __leftCoeff(D) = 0 'force to centralized
        __leftCoeff = -1 * __leftCoeff

        Dim __rightCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(6).Normalize(2)
        __rightCoeff(B) = 0
        __rightCoeff(D) = 0 'force to centralized


        Dim __leftRaw As List(Of Vector(Of Double)) = dataGenerator(__leftCoeff)
        Dim __rightRaw As List(Of Vector(Of Double)) = dataGenerator(__rightCoeff)

        Dim __testData As FA.materialData = New FA.materialData(0)
        __testData.scanningDatas(FA.framesDefinition.DIE_REAL_DRY) = New FA.scanningData(__leftRaw,
                                                                                         __rightRaw)


    End Sub
    <TestMethod()>
    Public Sub TestMethod2()
        '
        Dim __table As DataTable = Nothing
        Dim __list As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
        Dim __csvReader As CsvReader = New CsvReader()
        __table = __csvReader.CsvFileToDataTable("Data\test\4.csv")

        'loading into vector list
        For index = 0 To __table.Rows.Count - 1
            Dim _____list = __table.Rows(index).ItemArray.Select(Of Double)(Function(element As String) As Double
                                                                                Return Single.Parse(element)
                                                                            End Function).ToArray
            __list.Add(CreateVector.DenseOfArray(Of Double)(_____list))
        Next

        Dim __channelData As channelData = New channelData(__list)

    End Sub

End Class