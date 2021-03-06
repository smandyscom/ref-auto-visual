﻿Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Numerics.LinearAlgebra.Factorization
Imports MathNet.Spatial.Euclidean
Imports AutoNumeric.fittingMethods.coeffsDefinition

Public Class fittingMethods


    Public Enum coeffsDefinition As Integer
        A = 0
        B = 1
        C = 2
        D = 3
        E = 4
        F = 5
    End Enum

    Public Enum fitting2DMethodsEnum As Integer
        ''' <summary>
        ''' 0 = ax+by+c
        ''' </summary>
        ''' <remarks></remarks>
        LINE = 3
        ''' <summary>
        ''' 0 = ax^2+ bx + cy + d
        ''' </summary>
        ''' <remarks></remarks>
        PARABOLIC = 4
    End Enum


    Shared Function data2DFitting(datas As List(Of Vector(Of Double)),
                                  mode As fitting2DMethodsEnum,
                                  Optional regressionGoal As Integer = 1) As Vector(Of Double)


        Dim coeffVectors As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

        Dim diversity As List(Of Double) = New List(Of Double)
        Dim diversityAverage As Double = 0
        Dim diversityDeviation As Double = 0

        Dim minimumSolutionVector As Vector(Of Double) = Nothing

        Dim regressionCounter As Integer = 0

        Dim eachPoint As Vector(Of Double) = CreateVector.Dense(Of Double)(mode, 1)


        'stack these vectors
        For Each item As Vector(Of Double) In datas

            Select Case mode
                Case fitting2DMethodsEnum.LINE
                    eachPoint.SetSubVector(0, fitting2DMethodsEnum.LINE - 1, item)
                Case fitting2DMethodsEnum.PARABOLIC
                    eachPoint.SetValues({item.First ^ 2,
                                         item.First,
                                         item(1),
                                         1})
            End Select

            coeffVectors.Add(eachPoint.Clone)
        Next

        'regression ?
        Do
            'prepare coeff
            Dim coeff As Matrix(Of Double) = CreateMatrix.Dense(Of Double)(1, mode)
            coeffVectors.ForEach(Sub(__v As Vector(Of Double)) coeff = coeff.Stack(__v.ToRowMatrix))
            coeff = coeff.RemoveRow(0)

            minimumSolutionVector = solveRightSingularVector(coeff).Normalize(2) 'solve the best fit

            'minimum solution is the best othogonal vector to each coeff
            'calculate remained vector
            diversity.Clear()
            coeffVectors.ForEach(Sub(__v As Vector(Of Double)) diversity.Add(Math.Abs(__v.Normalize(2).DotProduct(minimumSolutionVector))))
            'sigma calculation
            diversityAverage = diversity.Average
            diversityDeviation = MathNet.Numerics.Statistics.Statistics.StandardDeviation(diversity)

            'find out those data points out of 2x deviation (68-95-99.7 rule)
            coeffVectors.RemoveAll(Function(__v As Vector(Of Double))
                                       Return Math.Abs(__v.Normalize(2).DotProduct(minimumSolutionVector)) > (diversityAverage + diversityDeviation)
                                   End Function)

            regressionCounter += 1 'do next regression


        Loop Until regressionCounter = regressionGoal


        Return minimumSolutionVector

    End Function

    ''' <summary>
    ''' 0 = ax^2+ bx + cy + d
    ''' maximum x , 2ax+b = 0 , so x =-b/2a
    ''' </summary>
    ''' <param name="coeffs">3x1 vector</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function parabolicMaximum(coeffs As Vector(Of Double)) As Double
        Return -coeffs(B) / (2 * coeffs(A))
    End Function
    ''' <summary>
    ''' 0 = ax^2+ bx + cy + d
    ''' </summary>
    ''' <param name="y"></param>
    ''' <param name="profile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function parabolicSolver(y As Double, profile As Vector(Of Double)) As Vector(Of Double)
        Dim __C As Double = profile(C) * y + profile(D)
        Dim discriminant As Double = profile(B) ^ 2 - 4 * profile(A) * __C

        'no real solve
        If discriminant < 0 Then
            Return Nothing
        End If

        Dim output As Vector(Of Double) = CreateVector.Dense(Of Double)({-profile(coeffsDefinition.B) + Math.Sqrt(discriminant),
                                                                         -profile(coeffsDefinition.B) - Math.Sqrt(discriminant)})
        output = output / (2 * profile(A))


        Return output
    End Function


    Shared Function solveRightSingularVector(__matrix As Matrix(Of Double)) As Vector(Of Double)
        Dim __svdResult As Svd(Of Double) = __matrix.Svd()
        'find smallest singular value
        Return __svdResult.VT.Transpose.Column((__svdResult.S.AbsoluteMinimumIndex()))
    End Function

    Enum fitting3DMethodsEnum As Integer
        PLANE = 4
        DOUBLE_PARABOLA = 6
    End Enum


    ''' <summary>
    ''' input : 3x1 vector
    ''' so far fitting the plane
    ''' ax+by+cz+d = 0
    ''' ax^2 + bx + cy^2 + dy + ez + f = 0
    ''' </summary>
    ''' <param name="point3DCloud"></param>
    ''' <returns>the surface coefficient</returns>
    ''' <remarks></remarks>
    Shared Function data3DFitting(point3DCloud As List(Of Vector(Of Double)),
                                   mode As fitting3DMethodsEnum,
                                   Optional regressionGoal As Integer = 1) As Vector(Of Double)


        Dim coeffVectors As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

        Dim diversity As List(Of Double) = New List(Of Double)
        Dim diversityAverage As Double = 0
        Dim diversityDeviation As Double = 0

        Dim minimumSolutionVector As Vector(Of Double) = Nothing

        Dim regressionCounter As Integer = 0

        Dim eachPoint As Vector(Of Double) = CreateVector.Dense(Of Double)(mode, 1)

        'stack these vectors
        For Each item As Vector(Of Double) In point3DCloud

            Select Case mode
                Case fitting3DMethodsEnum.PLANE
                    eachPoint.SetSubVector(0, item.Count, item)
                Case fitting3DMethodsEnum.DOUBLE_PARABOLA
                    eachPoint.SetValues({item.First ^ 2,
                                         item.First,
                                         item(1) ^ 2,
                                         item(1),
                                         item.Last,
                                         1})
            End Select

            coeffVectors.Add(eachPoint.Clone)
        Next

        'regression ?
        Do
            'prepare coeff
            Dim coeff As Matrix(Of Double) = CreateMatrix.Dense(Of Double)(1, mode)
            coeffVectors.ForEach(Sub(__v As Vector(Of Double)) coeff = coeff.Stack(__v.ToRowMatrix))
            coeff = coeff.RemoveRow(0)

            minimumSolutionVector = solveRightSingularVector(coeff).Normalize(2) 'solve the best fit

            'calculate the the orthogonal vector to minimum solution
            diversity.Clear()
            coeffVectors.ForEach(Sub(__v As Vector(Of Double)) diversity.Add(Math.Abs(__v.Normalize(2).DotProduct(minimumSolutionVector))))
            'sigma calculation
            diversityAverage = diversity.Average
            diversityDeviation = MathNet.Numerics.Statistics.Statistics.StandardDeviation(diversity)

            'find out those data points out of 2x deviation (68-95-99.7 rule)
            coeffVectors.RemoveAll(Function(__v As Vector(Of Double))
                                       Return Math.Abs(__v.Normalize(2).DotProduct(minimumSolutionVector)) > (diversityAverage + diversityDeviation)
                                   End Function)

            regressionCounter += 1 'do next regression

        Loop Until regressionCounter = regressionGoal


        Return minimumSolutionVector

    End Function


    Public Shared Function data3D(x As Double, y As Double, profile As Vector(Of Double), Optional mode As fitting3DMethodsEnum = fitting3DMethodsEnum.PLANE) As Double

        Select Case mode
            Case fitting3DMethodsEnum.PLANE
                Return 0
            Case fitting3DMethodsEnum.DOUBLE_PARABOLA
                'z = (ax^2 + bx + cy^2 + dy  + f)/-e
                Return (profile(coeffsDefinition.A) * (x ^ 2) +
                    profile(coeffsDefinition.B) * x +
                    profile(coeffsDefinition.C) * (y ^ 2) +
                    profile(coeffsDefinition.D) * y +
                    profile(coeffsDefinition.F)) / (-profile(coeffsDefinition.E))
            Case Else
                Return 0
        End Select

    End Function

    Public Shared Function data2D(x As Double, profile As Vector(Of Double), Optional mode As fitting2DMethodsEnum = fitting2DMethodsEnum.LINE) As Double

        Select Case mode
            Case fitting2DMethodsEnum.LINE
                'ax+by+c = 0
                'y = (ax+c)/-b
                Return (profile(A) * x + profile(C)) / (-profile(B))
            Case fitting2DMethodsEnum.PARABOLIC
                ' 0 = ax^2+ bx + cy + d
                'y = (ax2+bx+d)/-c
                Return (profile(A) * x * x + profile(B) * x + profile(D)) / (-profile(C))
            Case Else
                Return 0
        End Select

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="point3DCloud"></param>
    ''' <returns>3x1 point</returns>
    ''' <remarks></remarks>
    Shared Function cloudCenter(point3DCloud As List(Of Vector(Of Double))) As Vector(Of Double)

        Dim __accumulatedVector As Vector(Of Double) = CreateVector.Dense(Of Double)({0,
                                                                                      0,
                                                                                      0})

        For Each item As Vector(Of Double) In point3DCloud
            __accumulatedVector += item.SubVector(0, 3)
        Next

        Return __accumulatedVector / point3DCloud.Count

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="point3DCloud"></param>
    ''' <returns>4x4 homogenous matrix</returns>
    ''' <remarks></remarks>
    Shared Function coordinateFind(point3DCloud As List(Of Vector(Of Double))) As Matrix(Of Double)
        '1. find normal vector (Z-axis)
        '2. calculate origin (temp use)
        '3. use first point-origin vector as Y-axis(temp use)
        '4. cross out the X-axis
        '5. X-Z cross out Y-axis

        Dim zAxis As Vector(Of Double) = data3DFitting(point3DCloud, fitting3DMethodsEnum.PLANE).SubVector(0, 3).Normalize(2)
        Dim __origin As Vector(Of Double) = cloudCenter(point3DCloud)
        Dim tempYAxis As Vector(Of Double) = (point3DCloud.First.SubVector(0, __origin.Count) - __origin).Normalize(2)

        'cross out x-axis
        Dim xAxis As Vector(Of Double) = Vector3D.OfVector(tempYAxis).CrossProduct(Vector3D.OfVector(zAxis)).ToVector.Normalize(2)
        Dim yAxis As Vector(Of Double) = Vector3D.OfVector(zAxis).CrossProduct(Vector3D.OfVector(xAxis)).ToVector.Normalize(2)

        Dim output As Matrix(Of Double) = CreateMatrix.DenseOfColumnVectors(Of Double)(xAxis,
                                                                                       yAxis,
                                                                                       zAxis,
                                                                                       __origin)
        'cascade homogenous vector
        Return output.Stack(CreateMatrix.DenseOfRowArrays(Of Double)({0, 0, 0, 1}))
    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="line1Cloud"></param>
    ''' <param name="line2Cloud"></param>
    ''' <returns>3x1 position vector in Reference frame</returns>
    ''' <remarks></remarks>
    Shared Function line3DIntersection(line1Cloud As List(Of Vector(Of Double)),
                                       line2Cloud As List(Of Vector(Of Double)),
                                       Optional regression As Integer = 1) As Vector(Of Double)
        '1. merge two cloud and find-out it local coordinate system
        '2. transform these point to local coordinate 
        '3. take the X,Y part only
        '5. do line1 ,a,b coefficient fitting
        '6. do line2 ,a,b coefficient fitting

        Dim mergedCloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
        mergedCloud.AddRange(line1Cloud)
        mergedCloud.AddRange(line2Cloud)

        Dim localCoordinate As htmEdgeElementary = New htmEdgeElementary(coordinateFind(mergedCloud),
                                                                        coordinateEnums.LOCAL,
                                                                        coordinateEnums.REFERENCE)

        Dim line1CloudInLocal As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
        Dim line2CloudInLocal As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

        Trace.WriteLine(localCoordinate.RotationMatrix.ToMatrixString)
        Trace.WriteLine((localCoordinate.RotationMatrix * localCoordinate.RotationMatrix.Transpose).ToMatrixString)

        '------------------------------
        'turns into representation of R
        'and only take X,Y
        '------------------------------
        Dim localCoordinateInverse As htmEdgeElementary = localCoordinate.Inverse
        For Each item As Vector(Of Double) In line1Cloud
            line1CloudInLocal.Add((localCoordinateInverse * (New PositionVector(item, coordinateEnums.LOCAL))).RawValue)
        Next
        For Each item As Vector(Of Double) In line2Cloud
            line2CloudInLocal.Add((localCoordinateInverse * (New PositionVector(item, coordinateEnums.LOCAL))).RawValue)
        Next

        Trace.WriteLine(line1CloudInLocal.Last.ToVectorString)
        Trace.WriteLine(line2CloudInLocal.Last.ToVectorString)


        Dim line1 As Vector(Of Double) = data2DFitting(line1CloudInLocal, fitting2DMethodsEnum.LINE, regression)
        Dim line2 As Vector(Of Double) = data2DFitting(line2CloudInLocal, fitting2DMethodsEnum.LINE, regression)

        Dim __v1 As Vector(Of Double) = CreateVector.Dense(Of Double)({-line1(coeffsDefinition.B), line1(coeffsDefinition.A)})
        Dim __v2 As Vector(Of Double) = CreateVector.Dense(Of Double)({-line2(coeffsDefinition.B), line2(coeffsDefinition.A)})

        Trace.WriteLine(__v1.DotProduct(__v2))

        '---------------
        '   Equations solver
        '---------------
        Dim intersectionInLocal As Vector(Of Double) = line2DIntersection(line1, line2)
        intersectionInLocal(intersectionInLocal.Count - 1) = 0 ' the z value should be zero
        Trace.WriteLine(intersectionInLocal.ToVectorString)

        Return (localCoordinate * New PositionVector(intersectionInLocal, coordinateEnums.LOCAL)).RawValue
    End Function

    Enum coordinateEnums
        LOCAL
        REFERENCE
    End Enum

    ''' <summary>
    ''' 0= ax + by + c
    ''' </summary>
    ''' <param name="line1"></param>
    ''' <param name="line2"></param>
    ''' <returns> 2x1 intersection coordinate </returns>
    ''' <remarks></remarks>
    Shared Function line2DIntersection(line1 As Vector(Of Double), line2 As Vector(Of Double)) As Vector(Of Double)
        '---------------
        '   Equations solver
        '---------------
        Dim solution As Vector(Of Double) = Vector3D.OfVector(line1).CrossProduct(Vector3D.OfVector(line2)).ToVector()

        'last element should be 1
        Return solution / solution.Last

    End Function

End Class
