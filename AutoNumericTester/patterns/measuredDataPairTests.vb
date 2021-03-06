﻿Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumericTester.common
Imports System.Text
Imports AutoNumeric.measuredDataPair
Imports KellermanSoftware.CsvReports


Namespace AutoNumeric.Tests

    <TestClass()> Public Class measuredDataPairTests

        Public Enum frames
            NOMINAL
            REAL
        End Enum

        Dim rnd As Random = New Random(Now.Millisecond)
        Dim noisePercentage As Double = 0.001

        Enum testDataEnum As Integer
            ANSWER_ERROR = 0
            DATAS
        End Enum


        Function testDataGeneratorRandom(selection As Integer,
                                         Optional dataCounts As Integer = 100,
                                         Optional isZRestricted As Boolean = False,
                                         Optional radianBound As Single = 1,
                                         Optional noiseRatio As Single = 0.01) As Object
            'error matrix initialize
            Dim answerErrorMatrix As eulerHtmTR = New eulerHtmTR(frames.NOMINAL,
                                                                 frames.REAL)
            Dim __controlVector As Vector(Of Double) =
                CreateVector.Random(Of Double)([Enum].GetValues(GetType(axisEntityEnum)).Length)

            If isZRestricted Then
                __controlVector(axisEntityEnum.Z) = 0
            End If
            For index = axisEntityEnum.A To axisEntityEnum.C
                __controlVector(index) = (__controlVector(index) Mod radianBound) 'less than upper bound
            Next
            answerErrorMatrix.ControlVector = __controlVector


            Dim dataPairs As List(Of measuredDataPair) = New List(Of measuredDataPair)
            'generate nominal positions around a circle
            For index = 0 To dataCounts - 1
                Dim nomialPosition = New PositionVector(CreateVector.Random(Of Double)(3), Nothing)
                Dim realPosition As PositionVector = answerErrorMatrix * nomialPosition
                Dim noise As Vector(Of Double) = CreateVector.Random(Of Double)(3).Normalize(2)

                noise = noise * (realPosition.RawValue.L2Norm() * noiseRatio)

                realPosition += New PositionVector(noise, Nothing)
                dataPairs.Add(New measuredDataPair(nomialPosition.RawValue,
                                                   realPosition.RawValue,
                                                   selection))

            Next

            Return {answerErrorMatrix,
                    dataPairs}

        End Function

        <TestMethod()> Public Sub NewTest()

            Dim dataPair = New measuredDataPair(CreateVector.Dense(Of Double)(3),
                                                CreateVector.Dense(Of Double)(3),
                                                 utilities.selectionEnums.X +
                                                  utilities.selectionEnums.Y +
                                                  utilities.selectionEnums.Z)

            'no error
            Assert.IsTrue(dataPair.OriginalErrorPosition.L2Norm = 0)
        End Sub

        <TestMethod()> Public Sub errorGainTest()
            Dim nomialPosition As PositionVector = New PositionVector(CreateVector.Random(Of Double)(3),
                                                                      frames.NOMINAL)

            'Dim errorMatrix As errorHtm = New errorHtm(frames.NOMINAL,
            '                                           frames.REAL)



            'errorMatrix.ErrorVector = CreateVector.Random(Of Double)(6)

            'Dim realPosition As PositionVector = errorMatrix * nomialPosition

            'Dim dataPair = New measuredDataPair(nomialPosition.RawValue,
            '                                    realPosition.RawValue,
            '                                     utilities.selectionEnums.X +
            '                                     utilities.selectionEnums.Y +
            '                                     utilities.selectionEnums.Z)

            'Dim errorPositionRestore = dataPair.ErrorGain * errorMatrix.ErrorVector
            'Dim errorPositionDifference = errorPositionRestore - dataPair.ErrorPosition
            ''no error
            'Assert.IsTrue(errorPositionDifference.ForAll(AddressOf isInTolerance),
            '              String.Format("given {0}; restore {1}",
            '                            dataPair.ErrorPosition.ToVectorString,
            '                            errorPositionRestore.ToVectorString))
        End Sub

        '<TestMethod()> Public Sub fitErrorVectorTest()
        '    common.tolerance = 0.00000001

        '    'error matrix initialize
        '    Dim errorMatrix As errorHtm = New errorHtm(frames.NOMINAL,
        '                                               frames.REAL)
        '    Dim errorVector = CreateVector.Random(Of Double)([Enum].GetValues(GetType(errorVectorIndexesEnum)).Length)
        '    errorMatrix.ErrorVector = errorVector

        '    Dim nomialCenter As PositionVector = New PositionVector(CreateVector.Random(Of Double)(3),
        '                                                            frames.NOMINAL)
        '    Dim nomialPosition As PositionVector = New PositionVector(frames.NOMINAL)
        '    Dim nomialPositions As List(Of PositionVector) = New List(Of PositionVector)

        '    Dim realPositions As List(Of PositionVector) = New List(Of PositionVector)
        '    Dim realPositionsCalculated As List(Of PositionVector) = New List(Of PositionVector)
        '    Dim realPositionDifference As List(Of PositionVector) = New List(Of PositionVector)

        '    Dim radius As Double = 100
        '    Dim divides As Integer = 20
        '    Dim radianIncrement As Double = ((360 / divides) / 180) * Math.PI
        '    Dim radianAccumulation As Double = 0

        '    Dim dataPairs As List(Of measuredDataPair) = New List(Of measuredDataPair)
        '    'generate nominal positions around a circle
        '    For index = 0 To divides - 1
        '        nomialPosition = nomialCenter.Clone
        '        With nomialPosition
        '            .X = .X + radius * Math.Cos(radianAccumulation)
        '            .Y = .Y + radius * Math.Sin(radianAccumulation)
        '            .Z = .Z + radius * Math.Sin(radianAccumulation)
        '        End With

        '        Dim realPosition As PositionVector = errorMatrix * nomialPosition

        '        nomialPositions.Add(nomialPosition.Clone)
        '        realPositions.Add(realPosition.Clone)
        '        'With realPosition
        '        '    'added some noise
        '        '    Dim sign As Double = (rnd.Next(0, 2) - 0.5) * 2
        '        '    .X += ((.X) * noisePercentage * rnd.NextDouble * sign)
        '        '    sign = (rnd.Next(0, 2) - 0.5) * 2
        '        '    .Y += ((.Y) * noisePercentage * rnd.NextDouble * sign)
        '        '    sign = (rnd.Next(0, 2) - 0.5) * 2
        '        '    .Z += ((.Z) * noisePercentage * rnd.NextDouble * sign)
        '        'End With

        '        dataPairs.Add(New measuredDataPair(nomialPosition.RawValue.Clone,
        '                                           realPosition.RawValue.Clone,
        '                                            utilities.selectionEnums.X + utilities.selectionEnums.Y + utilities.selectionEnums.Z))

        '        radianAccumulation += radianIncrement
        '    Next

        '    Dim calculatedErrorVector As Vector(Of Double) = measuredDataPair.fitErrorVector(dataPairs)

        '    Dim calculatedErrorMatrix As errorHtm = New errorHtm(frames.NOMINAL,
        '                                                         frames.REAL) With {.ErrorVector = calculatedErrorVector}

        '    'calculated
        '    For index = 0 To nomialPositions.Count - 1
        '        realPositionsCalculated.Add(calculatedErrorMatrix * nomialPositions(index))

        '    Next
        '    For index = 0 To realPositions.Count - 1
        '        realPositionDifference.Add(realPositions(index) - realPositionsCalculated(index))
        '    Next


        '    Dim errorVectorDifference = calculatedErrorVector - errorMatrix.ErrorVector

        '    Trace.WriteLine(errorVectorDifference.ToVectorString)


        '    Dim sb As StringBuilder = New StringBuilder
        '    For index = 0 To realPositionDifference.Count - 1
        '        sb.AppendLine(String.Format("{0},{1},{2}",
        '                                    realPositions(index).RawValue.ToVectorString.Replace(vbCrLf, vbTab),
        '                                    realPositionsCalculated(index).RawValue.ToVectorString.Replace(vbCrLf, vbTab),
        '                                    realPositionDifference(index).RawValue.ToVectorString.Replace(vbCrLf, vbTab)))
        '    Next
        '    Trace.WriteLine(sb.ToString)


        '    Assert.IsTrue(realPositionDifference.TrueForAll(Function(__pos As PositionVector)
        '                                                        With __pos
        '                                                            Return isInTolerance(.X) And
        '                                                                isInTolerance(.Y)
        '                                                        End With
        '                                                    End Function))

        'End Sub

        <TestMethod()> Public Sub fitTransformationXY()
            common.tolerance = 0.00000001

            'error matrix initialize
            Dim errorMatrix As eulerHtmTR = New eulerHtmTR(frames.NOMINAL,
                                                           frames.REAL)
            Dim errorVector = CreateVector.Random(Of Double)([Enum].GetValues(GetType(axisEntityEnum)).Length)
            errorMatrix.ControlVector = errorVector

            Dim nomialCenter As PositionVector = New PositionVector(CreateVector.Random(Of Double)(3),
                                                                    frames.NOMINAL)

            Dim nomialPosition As PositionVector = New PositionVector(frames.NOMINAL)
            Dim nomialPositions As List(Of PositionVector) = New List(Of PositionVector)

            Dim realPositions As List(Of PositionVector) = New List(Of PositionVector)
            Dim realPositionsCalculated As List(Of PositionVector) = New List(Of PositionVector)
            Dim realPositionDifference As List(Of PositionVector) = New List(Of PositionVector)

            Dim radius As Double = 100
            Dim divides As Integer = 20
            Dim radianIncrement As Double = ((360 / divides) / 180) * Math.PI
            Dim radianAccumulation As Double = 0

            Dim dataPairs As List(Of measuredDataPair) = New List(Of measuredDataPair)
            'generate nominal positions around a circle
            For index = 0 To divides - 1
                nomialPosition = nomialCenter.Clone
                With nomialPosition
                    .X = .X + radius * Math.Cos(radianAccumulation)
                    .Y = .Y + radius * Math.Sin(radianAccumulation)
                    .Z = .Z + radius * Math.Sin(radianAccumulation)
                End With

                Dim realPosition As PositionVector = errorMatrix * nomialPosition

                nomialPositions.Add(nomialPosition.Clone)
                realPositions.Add(realPosition.Clone)
                'With realPosition
                '    'added some noise
                '    Dim sign As Double = (rnd.Next(0, 2) - 0.5) * 2
                '    .X += ((.X) * noisePercentage * rnd.NextDouble * sign)
                '    sign = (rnd.Next(0, 2) - 0.5) * 2
                '    .Y += ((.Y) * noisePercentage * rnd.NextDouble * sign)
                '    sign = (rnd.Next(0, 2) - 0.5) * 2
                '    .Z += ((.Z) * noisePercentage * rnd.NextDouble * sign)
                'End With

                dataPairs.Add(New measuredDataPair(nomialPosition.RawValue.Clone,
                                                   realPosition.RawValue.Clone,
                                                    utilities.selectionEnums.X Or
                                                    utilities.selectionEnums.Y))

                radianAccumulation += radianIncrement
            Next

            Dim calculatedErrorMatrix As htmEdgeElementary = measuredDataPair.fitTransformation(dataPairs)

            Dim __compare As List(Of measuredDataPair) = New List(Of measuredDataPair)
            For index = 0 To nomialPositions.Count - 1
                __compare.Add(New measuredDataPair((calculatedErrorMatrix * nomialPositions(index)).RawValue,
                                                   realPositions(index).RawValue,
                                                    utilities.selectionEnums.X Or
                                                     utilities.selectionEnums.Y))
            Next

            Trace.Write(measuredDataPair.pairsOutput(__compare))
            Trace.Write(calculatedErrorMatrix.RawValue)
            Trace.Write(errorMatrix.RawValue)

            Assert.IsTrue(__compare.TrueForAll(Function(__data As measuredDataPair) isInTolerance(__data.__dimensionErrorPosition.L2Norm)))
        End Sub

        <TestMethod()>
        Public Sub jacobianTest1()

            Dim __position As Vector(Of Double) = CreateVector.Random(Of Double)(3)
            Dim __currentErrorVector As Vector(Of Double) = CreateVector.Dense(Of Double)(6)


            Trace.WriteLine(__position.ToVectorString)
            Trace.WriteLine(utilities.position2ErrorGain(__position,
                                          __currentErrorVector).ToMatrixString)

        End Sub

        <TestMethod()>
        Sub expectedErrorTest()

            Dim __measurePair As measuredDataPair = New measuredDataPair(CreateVector.Random(Of Double)(3),
                                                                         CreateVector.Random(Of Double)(3))
            Dim __currentErrorVector As Vector(Of Double) = CreateVector.Random(Of Double)(6)

            Trace.WriteLine(__measurePair.OriginalIdealPosition.ToVectorString)
            Trace.WriteLine(__currentErrorVector.ToVectorString)
            'Trace.WriteLine(__measurePair.ExpectedMeasure(__currentErrorVector).ToVectorString)

        End Sub

        <TestMethod()>
        Sub cascadeTest()

            Dim __list As List(Of measuredDataPair) = New List(Of measuredDataPair)
            Dim __control As Vector(Of Double) = CreateVector.Random(Of Double)(6)
            For index = 1 To 10
                __list.Add(New measuredDataPair(CreateVector.Random(Of Double)(3),
                                                CreateVector.Random(Of Double)(3)))
            Next
            Dim result = measuredDataPair.pairsCascade(__list, __control)

            Dim jacobians As Matrix(Of Double) = result(cascadeDefinitionsEnum.JACOBIAN)
            Dim expect As Vector(Of Double) = result(cascadeDefinitionsEnum.EXPECT)
            Dim measures As Vector(Of Double) = result(cascadeDefinitionsEnum.MEASURES)

            Assert.IsTrue(expect.First = __list.First.ExpectedMeasure(__control).First)
            Assert.IsTrue(expect.Last = __list.Last.ExpectedMeasure(__control).Last)

            Assert.IsTrue(measures.First = __list.First.__dimensionErrorPosition.First)
            Assert.IsTrue(measures.Last = __list.Last.__dimensionErrorPosition.Last)

            Assert.IsTrue(jacobians.Row(0).Equals(__list.First.ErrorGain(__control).Row(0)))
            Assert.IsTrue(jacobians.Row(jacobians.RowCount - 1).Equals(__list.Last.ErrorGain(__control).Row(__list.Last.ErrorGain.RowCount - 1)))

        End Sub

        <TestMethod()>
        Sub fitMethodTest2()

            Dim __data As Object = testDataGeneratorRandom(utilities.selectionEnums.X Or
                                                           utilities.selectionEnums.Y,
                                                           100,
                                                           True,
                                                           1.0F,
                                                           0.3)
            Dim __answerTransformation As htmEdgeElementary = __data(testDataEnum.ANSWER_ERROR)

            Dim __calculatedTransformation As htmEdgeElementary =
                measuredDataPair.fitTransformation2(__data(testDataEnum.DATAS))

            Trace.WriteLine(__answerTransformation.RawValue.ToMatrixString)
            Trace.WriteLine(__calculatedTransformation.RawValue.ToMatrixString)

        End Sub

        <TestMethod()>
        Sub fitMethodTest3()
            Dim __csvReader As CsvReader = New CsvReader()
            Dim __list As List(Of measuredDataPair) = New List(Of measuredDataPair)
            Dim __datas As DataTable = __csvReader.CsvFileToDataTable("..\..\testData\ccd_error1.csv")

            For index = 0 To __datas.Rows.Count - 1
                With __datas.Rows(index).ItemArray.Select(Of Double)(Function(__string As String) Single.Parse(__string)).ToList
                    __list.Add(New measuredDataPair(CreateVector.Dense(Of Double)({.Item(2),
                                                                                   .Item(3),
                                                                                   .Item(4),
                                                                                   1}),
                                                    CreateVector.Dense(Of Double)({.Item(0),
                                                                                   .Item(1),
                                                                                   0,
                                                                                   1}),
                                                     utilities.selectionEnums.X Or
                                                     utilities.selectionEnums.Y))

                End With
            Next

            Dim __eu As eulerHtmTR = fitTransformation2(__list)
            Trace.WriteLine(__eu.RawValue)
            Trace.WriteLine(__eu.ControlVector.ToVectorString)

            Dim __list2 As List(Of measuredDataPair) = __list.Select(Of measuredDataPair)(Function(element As measuredDataPair) New measuredDataPair((__eu * New PositionVector(element.OriginalIdealPosition, Nothing)).RawValue,
                                                                                                                                                    element.OriginalRealPosition,
                                                                                                                                                    element.__gainSelection)).ToList
            Trace.WriteLine(measuredDataPair.pairsOutput(__list2))

            Dim __eu2 As eulerHtmTR = fitTransformation2(__list2)
            Trace.WriteLine(__eu2.RawValue)
            Trace.WriteLine(__eu2.ControlVector.ToVectorString)

            Dim __list3 As List(Of measuredDataPair) = __list2.Select(Of measuredDataPair)(Function(element As measuredDataPair) New measuredDataPair((__eu2 * New PositionVector(element.OriginalIdealPosition, Nothing)).RawValue,
                                                                                                                                                    element.OriginalRealPosition,
                                                                                                                                                    element.__gainSelection)).ToList
            Trace.WriteLine(measuredDataPair.pairsOutput(__list3))


        End Sub

    End Class


End Namespace


