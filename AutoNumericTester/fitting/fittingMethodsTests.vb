Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric.fittingMethods
Imports AutoNumericTester.common
Imports KellermanSoftware.CsvReports

Namespace AutoNumeric.Tests

    <TestClass()> Public Class fittingMethodsTests



        <TestMethod()> Public Sub parabolicFittingTest()

            Dim answerCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.PARABOLIC).Normalize(2)
            Dim pointCounts As Integer = 20

            'X-Y
            Dim pointSets As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            For index = 0 To pointCounts - 1

                Dim __pt As Vector(Of Double) = CreateVector.Random(Of Double)(2)
                __pt(1) = data2D(__pt(0), answerCoeff, fitting2DMethodsEnum.PARABOLIC)

                pointSets.Add(__pt)
            Next

            Dim calculatedCoeff As Vector(Of Double) = data2DFitting(pointSets, fitting2DMethodsEnum.PARABOLIC)

            Assert.IsTrue((calculatedCoeff - answerCoeff).L2Norm < 0.0000001,
                          (calculatedCoeff - answerCoeff).L2Norm)

            'since it is increasing
            Dim peakValue As Double = pointSets.Min(Function(__pt As Vector(Of Double)) Math.Abs(__pt(1)))
            Dim peakPoint As Vector(Of Double) =
                pointSets.Find(Function(___every As Vector(Of Double)) ___every(1) = peakValue)

            Trace.WriteLine(parabolicMaximum(calculatedCoeff))
            Trace.WriteLine(peakPoint(1))
            Trace.WriteLine(parabolicSolver(0, calculatedCoeff))
        End Sub

        <TestMethod()> Public Sub parabolicMaximumTest()
            Assert.Fail()
        End Sub

        <TestMethod()> Public Sub lineFittingTest()

            'line two coefficient
            Dim answerCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.LINE).Normalize(2)
            Dim pointCounts As Integer = 20

            Dim pointSets As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            For index = 0 To pointCounts - 1

                Dim __pt As Vector(Of Double) = CreateVector.Random(Of Double)(2)
                __pt(1) = data2D(__pt(0), answerCoeff, fitting2DMethodsEnum.LINE)

                pointSets.Add(__pt)
            Next


            Dim caculatedCoeff As Vector(Of Double) = CreateVector.Dense(Of Double)(answerCoeff.Count)

            Assert.IsTrue((caculatedCoeff - answerCoeff).L2Norm > 0.001)

            caculatedCoeff = data2DFitting(pointSets, fitting2DMethodsEnum.LINE)

            Assert.IsTrue((caculatedCoeff - answerCoeff).L2Norm < 0.0000001,
                          (caculatedCoeff - answerCoeff).L2Norm)

        End Sub

        <TestMethod()> Public Sub solveRightSingularVectorTest()

            Dim test As Matrix(Of Double) = CreateMatrix.Random(Of Double)(10, 10)

            Dim __v As Vector(Of Double) = solveRightSingularVector(test)

            Assert.Fail()
        End Sub

        <TestMethod()> Public Sub normalVectorFitTest()

            'ax+by+cz+d=0
            'z = (-d-ax-by)/c

            Dim cloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            Dim answerCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(4)

            For index = 0 To 100
                'randomize data points
                Dim __point As Vector(Of Double) = CreateVector.Random(Of Double)(3)
                __point(2) = (-answerCoeff(coeffsDefinition.D) - answerCoeff(coeffsDefinition.A) * __point(0) - answerCoeff(coeffsDefinition.B) * __point(1)) / answerCoeff(coeffsDefinition.C)
                cloud.Add(__point)

            Next

            Dim answer As Vector(Of Double) = answerCoeff.Normalize(2)
            Dim calculated As Vector(Of Double) = data3DFitting(cloud, fitting3DMethodsEnum.PLANE)

            Trace.WriteLine(answer.ToVectorString.Replace(vbCrLf, vbTab))
            Trace.WriteLine(calculated.ToVectorString.Replace(vbCrLf, vbTab))

            Assert.Fail()
        End Sub

        <TestMethod()> Public Sub cloudCenterTest()

            Dim nominalCenter As PositionVector = New PositionVector(CreateVector.Random(Of Double)(3),
                                                               Nothing)
            Dim nomialPosition As PositionVector = New PositionVector(Nothing)

            Dim cloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))



            Dim radius As Double = 100
            Dim divides As Integer = 20
            Dim radianIncrement As Double = ((360 / divides) / 180) * Math.PI
            Dim radianAccumulation As Double = 0

            For index = 0 To divides - 1
                nomialPosition = nominalCenter.Clone
                With nomialPosition
                    .X = .X + radius * Math.Cos(radianAccumulation)
                    .Y = .Y + radius * Math.Sin(radianAccumulation)
                    .Z = .Z + radius * Math.Sin(radianAccumulation)
                End With
                cloud.Add(nomialPosition.RawValue)
                radianAccumulation += radianIncrement
            Next

            Assert.IsTrue((cloudCenter(cloud) - nominalCenter.RawValue.SubVector(0, 3)).L2Norm < 0.0001,
                          String.Format("Calculated:{0},Nominal:{1}",
                                        cloudCenter(cloud).ToVectorString.Replace(vbCrLf, vbTab),
                                        nominalCenter.RawValue.ToVectorString.Replace(vbCrLf, vbTab)))
        End Sub

        <TestMethod()> Public Sub coordinateFindTest()

            Dim answerTransformation As htmEdgeElementary = New htmEdgeElementary(Nothing, Nothing)
            Dim __angles As Vector(Of Double) = CreateVector.Random(Of Double)(3)
            Dim __positions As Vector(Of Double) = CreateVector.Random(Of Double)(3)

            With answerTransformation
                .RotationMatrix = utilities.RotateTransformation(__angles(0),
                                                                 __angles(1),
                                                                 __angles(2))
                .PositionVector = CreateVector.Dense(Of Double)({__positions(0),
                                                                 __positions(1),
                                                                 __positions(2),
                                                                 1})
            End With

            'randomize point cloud
            Dim cloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            For index = 0 To 100
                'since cloud should form as a plane , so there is no z value
                cloud.Add((answerTransformation * New PositionVector(CreateVector.Random(Of Double)(2), Nothing)).RawValue)
            Next

            'affine space?
            Dim calculatedTransformation As Matrix(Of Double) = coordinateFind(cloud)


            Trace.WriteLine(answerTransformation.RawValue.ToMatrixString)
            Trace.WriteLine(calculatedTransformation.ToMatrixString)

            'condition:
            '1. the calculated z-axis should parallel to answer
            '2. the calculated x-axis/y-axis is perpendicular to calculated z-axis

            Dim dotProductValueZ As Double = calculatedTransformation.Column(2).DotProduct(answerTransformation.FrameVector(frameVectorEnum.VZ))
            Dim dotProductValueX As Double = calculatedTransformation.Column(2).DotProduct(calculatedTransformation.Column(0))
            Dim dotProductValueY As Double = calculatedTransformation.Column(2).DotProduct(calculatedTransformation.Column(1))


            Assert.IsTrue(Math.Abs(dotProductValueZ) = 1,
                          dotProductValueZ)
            Assert.IsTrue(dotProductValueX,
                          dotProductValueX)
            Assert.IsTrue(dotProductValueY,
                          dotProductValueY)

        End Sub

        <TestMethod()> Public Sub line3DIntersectionTest()

            Dim line1 As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.LINE).Normalize(2)
            Dim line2 As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.LINE).Normalize(2)
            'make sure they are not parallel

            Dim __v1 As Vector(Of Double) = CreateVector.Dense(Of Double)({-line1(coeffsDefinition.B), line1(coeffsDefinition.A)})
            Dim __v2 As Vector(Of Double) = CreateVector.Dense(Of Double)({-line2(coeffsDefinition.B), line2(coeffsDefinition.A)})

            Assert.IsTrue(__v1.DotProduct(__v2) <> 0)

            'T_local_reference
            Dim givenTransformation As htmEdgeElementary = New htmEdgeElementary(Nothing, Nothing)
            Dim __angles As Vector(Of Double) = CreateVector.Random(Of Double)(3)
            Dim __positions As Vector(Of Double) = CreateVector.Random(Of Double)(3)

            'local to reference
            With givenTransformation
                .RotationMatrix = utilities.RotateTransformation(__angles(0),
                                                                 __angles(1),
                                                                 __angles(2))
                .PositionVector = CreateVector.Dense(Of Double)({__positions(0),
                                                                 __positions(1),
                                                                 __positions(2),
                                                                 1})
            End With
            Dim __intersection As Vector(Of Double) = line2DIntersection(line1, line2)
            __intersection(__intersection.Count - 1) = 0
            Dim answerintersection3D As Vector(Of Double) = (givenTransformation * New PositionVector(__intersection, Nothing)).RawValue
            Trace.WriteLine(__v1.DotProduct(__v2))
            Trace.WriteLine(givenTransformation.RawValue.ToMatrixString)
            '-----------------------------
            '   Test Data generating
            '-----------------------------
            Dim line1Cloud3D As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim line2Cloud3D As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            Dim __pt2D As Vector(Of Double) = CreateVector.Random(Of Double)(2)

            'generate randomize cloud
            ' 0= ax + by+c
            For index = 1 To 100
                __pt2D = CreateVector.Random(Of Double)(2)
                __pt2D(1) = data2D(__pt2D(0), line1, fitting2DMethodsEnum.LINE)
                line1Cloud3D.Add((givenTransformation * New PositionVector(__pt2D, Nothing)).RawValue.SubVector(0, 3))



                __pt2D = CreateVector.Random(Of Double)(2)
                __pt2D(1) = data2D(__pt2D(0), line2, fitting2DMethodsEnum.LINE)
                line2Cloud3D.Add((givenTransformation * New PositionVector(__pt2D, Nothing)).RawValue.SubVector(0, 3))
            Next

            Dim calculatedIntersection3D As Vector(Of Double) = line3DIntersection(line1Cloud3D, line2Cloud3D)

            Trace.WriteLine(answerintersection3D.ToVectorString)
            Trace.WriteLine(calculatedIntersection3D.ToVectorString)


            Assert.Fail()
        End Sub

        <TestMethod()> Public Sub line2DIntersectionTest()

            Dim line1 As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.LINE)
            Dim line2 As Vector(Of Double) = CreateVector.Random(Of Double)(fitting2DMethodsEnum.LINE)

            'make sure they are not parallel
            Assert.IsTrue(line1.SubVector(0, 2).DotProduct(line2.SubVector(0, 2)) <> 0)


            Dim intersection As Vector(Of Double) = line2DIntersection(line1, line2)

            Assert.IsTrue(line1.DotProduct(intersection) < 0.0001, line1.DotProduct(intersection))
            Assert.IsTrue(line2.DotProduct(intersection) < 0.0001, line2.DotProduct(intersection))

            intersection = line2DIntersection(line2, line1)

            Assert.IsTrue(line1.DotProduct(intersection) < 0.0001, line1.DotProduct(intersection))
            Assert.IsTrue(line2.DotProduct(intersection) < 0.0001, line2.DotProduct(intersection))


        End Sub

        <TestMethod()>
        Public Sub paraBolicPlace()

            Dim xRange As Single = 50
            Dim yRange As Single = 50
            Dim xStep As Single = 1
            Dim yStep As Single = 1

            Dim xIncrement As Single = 0
            Dim yIncrement As Single = 0

            Dim dataCloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim answerCoeff As Vector(Of Double) = CreateVector.Random(Of Double)(6)
            answerCoeff = answerCoeff.Normalize(2)

            While yIncrement <= yRange
                While xIncrement <= xRange
                    dataCloud.Add(CreateVector.Dense(Of Double)({xIncrement,
                                                                yIncrement,
                                                                data3D(xIncrement,
                                                                       yIncrement,
                                                                       answerCoeff, fitting3DMethodsEnum.DOUBLE_PARABOLA)}) +
                                                           CreateVector.Random(Of Double)(3).Normalize(2))
                    xIncrement += xStep
                End While
                xIncrement = 0
                yIncrement += yStep
            End While

            Dim calculatedCoeff As Vector(Of Double) = data3DFitting(dataCloud, fitting3DMethodsEnum.DOUBLE_PARABOLA)


            Assert.IsTrue((answerCoeff - calculatedCoeff).L2Norm < 0.00001,
                          String.Format("{0}{1}{2}",
                                        answerCoeff.ToVectorString.Replace(vbCrLf, vbTab),
                                        vbCrLf,
                                        calculatedCoeff.ToVectorString.Replace(vbCrLf, vbTab)))

        End Sub

        <TestMethod()> Public Sub line3DIntersectionTest2()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.826, 201.834, 58.1252}),
                              CreateVector.Dense(Of Double)({361.249, 201.932, 58.1183}),
                              CreateVector.Dense(Of Double)({361.817, 202.132, 58.1201}),
                              CreateVector.Dense(Of Double)({361.294, 202.234, 58.116}),
                              CreateVector.Dense(Of Double)({361.254, 202.332, 58.1201}),
                              CreateVector.Dense(Of Double)({361.592, 202.435, 58.1354}),
                              CreateVector.Dense(Of Double)({361.242, 202.532, 58.1231})})

            __line2.AddRange({CreateVector.Dense(Of Double)({362.041, 200.832, 58.1157}),
                              CreateVector.Dense(Of Double)({362.152, 201.72, 58.1165}),
                              CreateVector.Dense(Of Double)({362.252, 201.718, 58.1175}),
                              CreateVector.Dense(Of Double)({362.335, 201.137, 58.117}),
                              CreateVector.Dense(Of Double)({362.438, 201.283, 58.1162}),
                              CreateVector.Dense(Of Double)({362.551, 201.59, 58.1213}),
                              CreateVector.Dense(Of Double)({362.636, 200.833, 58.1152}),
                              CreateVector.Dense(Of Double)({362.752, 201.708, 58.1155}),
                              CreateVector.Dense(Of Double)({362.85, 201.491, 58.1175})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub


        <TestMethod()> Public Sub line3DIntersectionTest3()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.249, 201.932, 58.1183}),
                           CreateVector.Dense(Of Double)({361.294, 202.234, 58.116}),
                           CreateVector.Dense(Of Double)({361.254, 202.332, 58.1201}),
                           CreateVector.Dense(Of Double)({361.242, 202.532, 58.1231})})

            __line2.AddRange({
                              CreateVector.Dense(Of Double)({362.152, 201.72, 58.1165}),
                              CreateVector.Dense(Of Double)({362.252, 201.718, 58.1175}),
                              CreateVector.Dense(Of Double)({362.752, 201.708, 58.1155})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub

        <TestMethod()> Public Sub line3DIntersectionTest4()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.214, 201.936, 58.1249}),
                              CreateVector.Dense(Of Double)({361.22, 202.034, 58.1242}),
                              CreateVector.Dense(Of Double)({361.22, 202.088, 58.1306}),
                              CreateVector.Dense(Of Double)({361.229, 202.137, 58.1183}),
                              CreateVector.Dense(Of Double)({361.216, 202.185, 58.1262}),
                              CreateVector.Dense(Of Double)({361.201, 202.337, 58.1216}),
                              CreateVector.Dense(Of Double)({361.2, 202.386, 58.1283}),
                              CreateVector.Dense(Of Double)({361.223, 202.488, 58.1198}),
                              CreateVector.Dense(Of Double)({361.194, 202.537, 58.1498}),
                              CreateVector.Dense(Of Double)({361.222, 202.586, 58.1229}),
                              CreateVector.Dense(Of Double)({361.218, 202.635, 58.1224})})

            __line2.AddRange({
                              CreateVector.Dense(Of Double)({362.098, 200.936, 58.1142}),
                              CreateVector.Dense(Of Double)({362.145, 201.315, 58.1147}),
                              CreateVector.Dense(Of Double)({362.198, 200.934, 58.1142}),
                              CreateVector.Dense(Of Double)({362.247, 200.936, 58.1116}),
                              CreateVector.Dense(Of Double)({362.297, 200.937, 58.1144}),
                              CreateVector.Dense(Of Double)({362.347, 200.936, 58.1147}),
                              CreateVector.Dense(Of Double)({362.397, 200.912, 58.1152}),
                              CreateVector.Dense(Of Double)({362.447, 200.916, 58.1106}),
                              CreateVector.Dense(Of Double)({362.497, 200.926, 58.1147}),
                              CreateVector.Dense(Of Double)({362.547, 200.928, 58.1155}),
                              CreateVector.Dense(Of Double)({362.597, 200.931, 58.1175}),
                              CreateVector.Dense(Of Double)({362.646, 201.086, 58.1157}),
                              CreateVector.Dense(Of Double)({362.695, 201.36, 58.1129}),
                              CreateVector.Dense(Of Double)({362.747, 200.926, 58.1119}),
                              CreateVector.Dense(Of Double)({362.797, 200.923, 58.1129}),
                               CreateVector.Dense(Of Double)({362.86, 201.853, 58.1137}),
                               CreateVector.Dense(Of Double)({362.897, 200.916, 58.1137})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub

        <TestMethod()> Public Sub line3DIntersectionTest5()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.214, 201.936, 58.1249}),
                              CreateVector.Dense(Of Double)({361.22, 202.034, 58.1242}),
                              CreateVector.Dense(Of Double)({361.22, 202.088, 58.1306}),
                              CreateVector.Dense(Of Double)({361.229, 202.137, 58.1183}),
                              CreateVector.Dense(Of Double)({361.216, 202.185, 58.1262}),
                              CreateVector.Dense(Of Double)({361.201, 202.337, 58.1216}),
                              CreateVector.Dense(Of Double)({361.2, 202.386, 58.1283}),
                              CreateVector.Dense(Of Double)({361.223, 202.488, 58.1198}),
                              CreateVector.Dense(Of Double)({361.194, 202.537, 58.1498}),
                              CreateVector.Dense(Of Double)({361.222, 202.586, 58.1229}),
                              CreateVector.Dense(Of Double)({361.218, 202.635, 58.1224})})

            __line2.AddRange({
                              CreateVector.Dense(Of Double)({362.098, 200.936, 58.1142}),
                              CreateVector.Dense(Of Double)({362.198, 200.934, 58.1142}),
                              CreateVector.Dense(Of Double)({362.247, 200.936, 58.1116}),
                              CreateVector.Dense(Of Double)({362.297, 200.937, 58.1144}),
                              CreateVector.Dense(Of Double)({362.347, 200.936, 58.1147}),
                              CreateVector.Dense(Of Double)({362.397, 200.912, 58.1152}),
                              CreateVector.Dense(Of Double)({362.447, 200.916, 58.1106}),
                              CreateVector.Dense(Of Double)({362.497, 200.926, 58.1147}),
                              CreateVector.Dense(Of Double)({362.547, 200.928, 58.1155}),
                              CreateVector.Dense(Of Double)({362.597, 200.931, 58.1175}),
                              CreateVector.Dense(Of Double)({362.646, 201.086, 58.1157}),
                              CreateVector.Dense(Of Double)({362.747, 200.926, 58.1119}),
                              CreateVector.Dense(Of Double)({362.797, 200.923, 58.1129}),
                               CreateVector.Dense(Of Double)({362.897, 200.916, 58.1137})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub
        <TestMethod()>
        Public Sub line3DIntersectionTest6()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.214, 201.936, 58.1249}),
                              CreateVector.Dense(Of Double)({361.22, 202.034, 58.1242}),
                              CreateVector.Dense(Of Double)({361.22, 202.088, 58.1306}),
                              CreateVector.Dense(Of Double)({361.229, 202.137, 58.1183}),
                              CreateVector.Dense(Of Double)({361.216, 202.185, 58.1262}),
                              CreateVector.Dense(Of Double)({361.201, 202.337, 58.1216}),
                              CreateVector.Dense(Of Double)({361.2, 202.386, 58.1283}),
                              CreateVector.Dense(Of Double)({361.223, 202.488, 58.1198}),
                              CreateVector.Dense(Of Double)({361.194, 202.537, 58.1498}),
                              CreateVector.Dense(Of Double)({361.222, 202.586, 58.1229}),
                              CreateVector.Dense(Of Double)({361.218, 202.635, 58.1224})})

            __line2.AddRange({
                              CreateVector.Dense(Of Double)({362.098, 200.936, 58.1142}),
                              CreateVector.Dense(Of Double)({362.145, 201.315, 58.1147}),
                              CreateVector.Dense(Of Double)({362.198, 200.934, 58.1142}),
                              CreateVector.Dense(Of Double)({362.247, 200.936, 58.1116}),
                              CreateVector.Dense(Of Double)({362.297, 200.937, 58.1144}),
                              CreateVector.Dense(Of Double)({362.347, 200.936, 58.1147}),
                              CreateVector.Dense(Of Double)({362.397, 200.912, 58.1152}),
                              CreateVector.Dense(Of Double)({362.447, 200.916, 58.1106}),
                              CreateVector.Dense(Of Double)({362.497, 200.926, 58.1147}),
                              CreateVector.Dense(Of Double)({362.547, 200.928, 58.1155}),
                              CreateVector.Dense(Of Double)({362.597, 200.931, 58.1175}),
                              CreateVector.Dense(Of Double)({362.646, 201.086, 58.1157}),
                              CreateVector.Dense(Of Double)({362.695, 201.36, 58.1129}),
                              CreateVector.Dense(Of Double)({362.747, 200.926, 58.1119}),
                              CreateVector.Dense(Of Double)({362.797, 200.923, 58.1129}),
                               CreateVector.Dense(Of Double)({362.897, 200.916, 58.1137})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub

        <TestMethod()>
        Public Sub line3DIntersectionTest7()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

            __line1.AddRange({CreateVector.Dense(Of Double)({361.233, 201.849, 58.1137}),
                              CreateVector.Dense(Of Double)({361.229, 201.878, 58.1252}),
                              CreateVector.Dense(Of Double)({361.23, 201.936, 58.1234}),
                              CreateVector.Dense(Of Double)({361.233, 201.966, 58.1152}),
                              CreateVector.Dense(Of Double)({361.217, 201.995, 58.1132}),
                              CreateVector.Dense(Of Double)({361.235, 202.029, 58.1229}),
                              CreateVector.Dense(Of Double)({361.236, 202.059, 58.1208}),
                              CreateVector.Dense(Of Double)({361.239, 202.117, 58.1252}),
                              CreateVector.Dense(Of Double)({361.242, 202.146, 58.1272}),
                              CreateVector.Dense(Of Double)({361.236, 202.176, 58.1231}),
                              CreateVector.Dense(Of Double)({361.232, 202.205, 58.1236}),
                              CreateVector.Dense(Of Double)({361.239, 202.239, 58.1196}),
                              CreateVector.Dense(Of Double)({361.245, 202.269, 58.1124}),
                              CreateVector.Dense(Of Double)({361.241, 202.298, 58.118}),
                              CreateVector.Dense(Of Double)({361.218, 202.356, 58.1126}),
                              CreateVector.Dense(Of Double)({361.219, 202.415, 58.1139})})

            __line2.AddRange({
                              CreateVector.Dense(Of Double)({362.009, 200.926, 58.1188}),
                              CreateVector.Dense(Of Double)({362.039, 200.931, 58.1142}),
                              CreateVector.Dense(Of Double)({362.069, 200.928, 58.1196}),
                              CreateVector.Dense(Of Double)({362.099, 200.932, 58.1147}),
                              CreateVector.Dense(Of Double)({362.143, 201.846, 58.1206}),
                              CreateVector.Dense(Of Double)({362.159, 200.923, 58.116}),
                              CreateVector.Dense(Of Double)({362.219, 200.929, 58.1188}),
                              CreateVector.Dense(Of Double)({362.279, 200.929, 58.1236}),
                              CreateVector.Dense(Of Double)({362.309, 200.928, 58.1137}),
                              CreateVector.Dense(Of Double)({362.338, 201.232, 58.1132}),
                              CreateVector.Dense(Of Double)({362.369, 200.921, 58.1178}),
                              CreateVector.Dense(Of Double)({362.399, 200.909, 58.1132}),
                              CreateVector.Dense(Of Double)({362.429, 200.906, 58.1144}),
                              CreateVector.Dense(Of Double)({362.459, 200.91, 58.1144}),
                              CreateVector.Dense(Of Double)({362.489, 200.922, 58.1221}),
                               CreateVector.Dense(Of Double)({362.519, 200.92, 58.1188}),
                             CreateVector.Dense(Of Double)({362.549, 200.925, 58.1142}),
                              CreateVector.Dense(Of Double)({362.579, 200.923, 58.1211}),
                              CreateVector.Dense(Of Double)({362.609, 200.925, 58.1165})})

            Trace.WriteLine(line3DIntersection(__line1, __line2).ToVectorString)

        End Sub

        <TestMethod()>
        Public Sub line3DIntersectionTest8()

            Dim __line1 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim __line2 As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            Dim counter As Integer = 0

            Dim __lines As List(Of List(Of Vector(Of Double))) = New List(Of List(Of Vector(Of Double))) From {__line1,
                                                                                                               __line2}

            Dim __csvReader As CsvReader = New CsvReader()

            For Each item As DataTable In {__csvReader.CsvFileToDataTable("..\..\testData\edge4_line1.csv"),
                                           __csvReader.CsvFileToDataTable("..\..\testData\edge4_line2.csv")}
                For index = 0 To item.Rows.Count - 1
                    __lines(counter).Add(CreateVector.DenseOfArray(Of Double)(item.Rows(index).ItemArray.Select(Of Double)(Function(element As String) As Double
                                                                                                                               Return Single.Parse(element)
                                                                                                                           End Function).ToArray()))
                Next
                counter += 1
            Next

            Trace.WriteLine(line3DIntersection(__line1, __line2, 2).ToVectorString)

        End Sub

    End Class



End Namespace


