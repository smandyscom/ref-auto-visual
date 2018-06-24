Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Spatial.Euclidean
Imports AutoNumeric
Imports AutoNumeric.fittingMethods.coeffsDefinition
Imports FA.dataKeysDefine
Imports MathNet.Numerics.Statistics
Imports System.Text
Imports System.ComponentModel

Public Enum dataKeysDefine As Integer
    X = 0
    Y = 1
    VOLTAGE_LEFT = 2
    VOLTAGE_RIGHT = 3

End Enum


''' <summary>
''' The bonded material data , going to report
''' </summary>
''' <remarks></remarks>
Public Class materialData

    'polarizing value W1/W2/W3

    'image
    'intensity

    'raw dry/wet

    'peak value
    'width

    'eproxy diameter/position

    'dry/wet
    Friend scanningDatas As Dictionary(Of framesDefinition, scanningData) =
         New Dictionary(Of framesDefinition, scanningData)

    Friend cureDatas As Dictionary(Of dataKeysDefine, curingDataEachChannel) =
        New Dictionary(Of dataKeysDefine, curingDataEachChannel)

    Property IsEnagedToBond As Boolean = True

    'TODO , curing data
    ReadOnly Property IndexInArray As Integer
        Get
            Return __indexInArray
        End Get
    End Property
    Dim __indexInArray As Integer = 0

    Sub New(__index As Integer)
        __indexInArray = __index

        cureDatas(VOLTAGE_LEFT) = New curingDataEachChannel
        cureDatas(VOLTAGE_RIGHT) = New curingDataEachChannel

    End Sub
End Class


''' <summary>
''' Given raw data 
''' Output:
''' 1. parabolic profiles
''' 2. peak values ( left/right)
''' </summary>
''' <remarks></remarks>
Public Class scanningData

    ''' <summary>
    ''' In mm , the norminal distance between right-left
    ''' </summary>
    ''' <remarks></remarks>
    Shared WriteOnly Property NominalChannelDistance As Double
        Set(value As Double)
            __nominalChannelDistance = value
            __nominalRight = CreateVector.Dense(Of Double)({__nominalChannelDistance,
                                                                              0})
            __nominalMiddle = __nominalMiddle / 2
        End Set
    End Property

    Shared __nominalChannelDistance As Double = 1.75
    Shared __nominalRight As Vector(Of Double) = CreateVector.Dense(Of Double)({-1 * __nominalChannelDistance, 0})
    Shared __nominalLeft As Vector(Of Double) = CreateVector.Dense(Of Double)({0, 0})

    Shared __nominalMiddle As Vector(Of Double) = __nominalRight / 2

    Dim __rightCoordinate As Vector(Of Double) = Nothing
    Dim __leftCoordinate As Vector(Of Double) = Nothing

    ''' <summary>
    ''' According to scanned data
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property RevicedCoordinate As Matrix(Of Double)
        Get
            Return __revicedCoordinate
        End Get
    End Property
    ReadOnly Property ChannelData(__key As dataKeysDefine) As channelData
        Get
            Return __channelDatas(__key)
        End Get
    End Property
    Dim __revicedCoordinate As Matrix(Of Double) = CreateMatrix.DenseIdentity(Of Double)(4)

    Dim __channelDatas As Dictionary(Of dataKeysDefine, channelData) =
        New Dictionary(Of dataKeysDefine, channelData)

    ''' <summary>
    ''' For each data :
    ''' 0:X
    ''' 1:Y
    ''' 2:Reading
    ''' </summary>
    ''' <param name="leftRawData"></param>
    ''' <param name="rightRawData"></param>
    ''' <remarks></remarks>
    Sub New(leftRawData As List(Of Vector(Of Double)),
            rightRawData As List(Of Vector(Of Double)))

        __channelDatas(VOLTAGE_LEFT) = New channelData(leftRawData)
        __channelDatas(VOLTAGE_RIGHT) = New channelData(rightRawData)

        'error + nominal
        __rightCoordinate = __channelDatas(VOLTAGE_RIGHT).Center + __nominalRight
        __leftCoordinate = __channelDatas(VOLTAGE_LEFT).Center + __nominalLeft

        '(right-left).nomalize  = X axis (revised)
        Dim zAxis As Vector(Of Double) = CreateVector.Dense(Of Double)({0,
                                                                        0,
                                                                        1})
        Dim xAxis As Vector(Of Double) = (__leftCoordinate - __rightCoordinate).Normalize(2)
        xAxis = CreateVector.Dense(Of Double)({xAxis(X),
                                               xAxis(Y),
                                               0}) 'to 3d
        Dim yAxis As Vector(Of Double) = Vector3D.OfVector(zAxis).CrossProduct(Vector3D.OfVector(xAxis)).ToVector.Normalize(2)

        'take average of error
        Dim offset As Vector(Of Double) = __channelDatas(VOLTAGE_LEFT).Center

        With __revicedCoordinate
            .SetSubMatrix(0, xAxis.Count, axisEntityEnum.X, 1, xAxis.ToColumnMatrix)
            .SetSubMatrix(0, yAxis.Count, axisEntityEnum.Y, 1, yAxis.ToColumnMatrix)
            .SetSubMatrix(0, zAxis.Count, axisEntityEnum.Z, 1, zAxis.ToColumnMatrix)
            .SetSubMatrix(0, offset.Count, .ColumnCount - 1, 1, offset.ToColumnMatrix)
        End With

    End Sub

    Public Overrides Function ToString() As String
        Dim sb As StringBuilder = New StringBuilder

        For Each item As dataKeysDefine In {VOLTAGE_LEFT,
                                            VOLTAGE_RIGHT}
            sb.AppendLine(item.ToString)
            sb.Append(__channelDatas(item))
        Next
        sb.AppendLine("Revised Coordinate")
        sb.AppendLine(__revicedCoordinate.ToMatrixString)

        Return sb.ToString
    End Function

End Class


Public Class channelData

    Property Profile As Vector(Of Double)
        Get
            Return __profile
        End Get
        Set(value As Vector(Of Double))
            __profile = value
            'according to parabolic profile
            __peakCoordinateFitted = CreateVector.Dense(Of Double)({-__profile(B) / (2 * __profile(A)),
                                                            -__profile(E) / (2 * __profile(D))})
        End Set
    End Property
    ReadOnly Property Center As Vector(Of Double)
        Get
            'once profile is accpeted , use fitted coordinate
            'otherwise , use the real value
            Return __peakCoordinateDetected
        End Get
    End Property
    ReadOnly Property PeakValuteDetected As Double
        Get
            Return __peakValueDetected
        End Get
    End Property
    ReadOnly Property PeakValuteFitted As Double
        Get
            Return __peakValueFitted
        End Get
    End Property

    ReadOnly Property BeamWidthX As Double
        Get
            Return solveX.L1Norm
        End Get
    End Property
    ReadOnly Property BeamWidthY As Double
        Get
            Return solveY.L1Norm
        End Get
    End Property
    <Browsable(False)>
    ReadOnly Property RawData As List(Of Vector(Of Double))
        Get
            Return __rawData
        End Get
    End Property

    ''' <summary>
    ''' 0: X
    ''' 1: Y
    ''' 2: VOLTAGE
    ''' </summary>
    ''' <remarks></remarks>
    Dim __rawData As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
    Dim rangeX As Vector(Of Double) = Nothing
    Dim rangeY As Vector(Of Double) = Nothing

    Dim amptitude As Double = 1 '3db
    Dim solveX As Vector(Of Double) = Nothing
    Dim solveY As Vector(Of Double) = Nothing

    ''' <summary>
    ''' 6x1 vector
    ''' ax^2 + bx + cy^2 + dy + ez + f = 0
    ''' </summary>
    ''' <remarks></remarks>
    Dim __profile As Vector(Of Double) = Nothing
    Dim __peakValueDetected As Double = Nothing
    Dim __peakValueFitted As Double = Nothing

    Dim __peakCoordinateFitted As Vector(Of Double) = Nothing
    Dim __peakCoordinateDetected As Vector(Of Double) = Nothing

    Dim __isProfileAccepted As Boolean = False

    Sub New(__rawData As List(Of Vector(Of Double)))

        '--------------
        '
        '--------------
        Me.__rawData = __rawData
        rangeX = CreateVector.Dense(Of Double)({__rawData.Min(Function(data As Vector(Of Double)) data(X)),
                                                __rawData.Max(Function(data As Vector(Of Double)) data(X))})
        rangeY = CreateVector.Dense(Of Double)({__rawData.Min(Function(data As Vector(Of Double)) data(Y)),
                                                __rawData.Max(Function(data As Vector(Of Double)) data(Y))})


        __profile = fittingMethods.data3DFitting(__rawData, fittingMethods.fitting3DMethodsEnum.DOUBLE_PARABOLA)

        '--------------------
        '   Find out real peak
        '--------------------
        __peakValueDetected =
            __rawData.Max(Function(data As Vector(Of Double)) data.Last)
        __peakCoordinateDetected =
            CreateVector.DenseOfVector(Of Double)(__rawData.Find(Function(element As Vector(Of Double)) element.Last = __peakValueDetected)).SubVector(0, 2)

        'X: -b/2a , Y: -d/2c
        __peakCoordinateFitted = CreateVector.Dense(Of Double)({-__profile(B) / 2 * __profile(A),
                                                                -__profile(D) / 2 * __profile(C)})
        __peakValueFitted = fittingMethods.data3D(__peakCoordinateFitted(X),
                                                __peakCoordinateFitted(Y),
                                                __profile, fittingMethods.fitting3DMethodsEnum.DOUBLE_PARABOLA)



        'verify
        ' the peak value is happened within searching range
        'solve corresponding X/Y , check if X/Y is in the scanning range
        __isProfileAccepted = __peakCoordinateFitted(X) >= rangeX.First And
            __peakCoordinateFitted(X) <= rangeX.Last And
            __peakCoordinateFitted(Y) >= rangeY.First And
            __peakCoordinateFitted(Y) <= rangeY.Last And (__peakValueFitted > 0)


        'calculate beam width
        'solve x1,x2,y1,y2 on -3db peak value
        Dim __const As Double = __profile(C) * __peakCoordinateFitted(Y) ^ 2 +
            __profile(D) * __peakCoordinateFitted(Y) +
            __profile(E) * __peakValueFitted * amptitude +
            __profile(F)

        solveX = fittingMethods.parabolicSolver(0,
                                                CreateVector.Dense(Of Double)({__profile(A),
                                                                               __profile(B),
                                                                               0,
                                                                               __const}))
        __const = __profile(A) * __peakCoordinateFitted(X) ^ 2 +
            __profile(B) * __peakCoordinateFitted(X) +
            __profile(E) * __peakValueFitted * amptitude +
            __profile(F)

        solveY = fittingMethods.parabolicSolver(0,
                                                CreateVector.Dense(Of Double)({__profile(C),
                                                                               __profile(D),
                                                                               0,
                                                                               __const}))



    End Sub

    Public Overrides Function ToString() As String
        Dim sb As StringBuilder = New StringBuilder
        sb.AppendLine("Raw Data")
        Me.__rawData.ForEach(Sub(__data As Vector(Of Double)) sb.AppendLine(__data.ToVectorString.Replace(vbCrLf, ",")))
        sb.AppendLine(String.Format("Profile,{0}", __profile.ToVectorString.Replace(vbCrLf, ",")))
        sb.AppendLine(String.Format("Center,{0}", Center.ToVectorString.Replace(vbCrLf, ",")))
        sb.AppendLine(String.Format("Peak Value Detected,{0}", __peakValueDetected))
        sb.AppendLine(String.Format("Peak Value Fitted,{0}", __peakValueFitted))
        sb.AppendLine(String.Format("Beam Width X,{0}", BeamWidthX))
        sb.AppendLine(String.Format("Beam Width Y,{0}", BeamWidthY))
        sb.AppendLine(String.Format("Is Profile Accepted,{0}", __isProfileAccepted))

        Return sb.ToString
    End Function

End Class


Public Class curingDataEachChannel

    Public Enum cureParametersEnum As Integer
        START_POWER = 0
        PRE_POWER
        POST_POWER_1
        POST_POWER_2
    End Enum

    Sub setCureParameters(knot As cureParametersEnum)
        __cureParametersKnots(knot) = __rawReadings.Count - 1 'current index
    End Sub

    ReadOnly Property CureParameters(knot As cureParametersEnum) As Double
        Get
            Return __rawReadings(__cureParametersKnots(knot))
        End Get
    End Property

    WriteOnly Property Push As Double
        Set(value As Double)
            __rawReadings.Add(value)
            __movingReadings.Push(value)
        End Set
    End Property
    ReadOnly Property IsStabled As Boolean
        Get
            Return __movingReadings.StandardDeviation < deviationThreshold
        End Get
    End Property

    Protected __cureParametersKnots As List(Of Integer) = New List(Of Integer)
    Protected __rawReadings As List(Of Double) = New List(Of Double)

    Protected __movingReadings As MovingStatistics = New MovingStatistics(movingWindow)

    Shared movingWindow As Integer = 100
    Shared deviationThreshold As Double = 0.1

    Sub New()
        For index = 0 To Automation.utilities.enumObjectsListing(GetType(cureParametersEnum)).Length - 1
            __cureParametersKnots.Add(0)
        Next
    End Sub

End Class