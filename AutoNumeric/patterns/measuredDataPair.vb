﻿Imports MathNet.Numerics.LinearAlgebra
Imports System.Text
Imports AutoNumeric.utilities

Public Class measuredDataPair

    Friend Enum cascadeDefinitionsEnum As Byte
        JACOBIAN = 0
        MEASURES
        EXPECT = 2
    End Enum

    ''' <summary>
    ''' 3x1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property OriginalErrorPosition As Vector(Of Double)
        Get
            Return __originalErrorPosition
        End Get
    End Property
    ''' <summary>
    ''' 3x6
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ErrorGain As Matrix(Of Double)
        Get
            Return __errorGain
        End Get
    End Property
    ''' <summary>
    ''' 3x6 , the f(errorVector,idealPosition)
    ''' </summary>
    ''' <param name="errorVector"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ErrorGain(errorVector As Vector(Of Double)) As Matrix(Of Double)
        Get
            Dim __jacobian As Matrix(Of Double) = utilities.position2ErrorGain(__originalIdealPosition,
                                                                               errorVector)
            Dim value = [Enum].GetValues(GetType(utilities.selectionEnums))
            Dim availableRows As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            'take the avialbale dimension only
            For index = 0 To value.Length - 1
                If __gainSelection And value(index) Then
                    'if dimension selected
                    availableRows.Add(__jacobian.Row(index))
                End If
            Next

            Return CreateMatrix.DenseOfRowVectors(Of Double)(availableRows.ToArray)
        End Get
    End Property
    ''' <summary>
    ''' 3x1
    ''' </summary>
    ''' <param name="errorVector"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ExpectedMeasure(errorVector As Vector(Of Double)) As Vector(Of Double)
        Get
            Dim __tx As eulerHtmTR = New eulerHtmTR(Nothing, Nothing)
            __tx.ControlVector = errorVector

            Dim value = [Enum].GetValues(GetType(utilities.selectionEnums))

            Dim __idealPosition As Vector(Of Double) = CreateVector.Dense(Of Double)(4, 1)
            __idealPosition.SetSubVector(0, __originalIdealPosition.Count, __originalIdealPosition)

            Dim __expectedError As Vector(Of Double) = (CreateMatrix.DenseIdentity(Of Double)(4) - __tx.RawValue) * __idealPosition
            Dim availableValues As List(Of Double) = New List(Of Double)
            'take the avialbale dimension only
            For index = 0 To value.Length - 1
                If __gainSelection And value(index) Then
                    'if dimension selected
                    availableValues.Add(__expectedError(index))
                End If
            Next
            Return CreateVector.DenseOfEnumerable(Of Double)(availableValues)

        End Get
    End Property


    ReadOnly Property OriginalIdealPosition As Vector(Of Double)
        Get
            Return __originalIdealPosition
        End Get
    End Property
    ReadOnly Property OriginalRealPosition As Vector(Of Double)
        Get
            Return __originalRealPosition
        End Get
    End Property
    ReadOnly Property ErrorLength As Double
        Get
            Return __dimensionErrorPosition.L2Norm
        End Get
    End Property

    Friend __errorGain As Matrix(Of Double) = Nothing
    ''' <summary>
    ''' Definition : ideal-real
    ''' </summary>
    ''' <remarks></remarks>
    Friend __originalErrorPosition As Vector(Of Double) = Nothing

    Friend __originalIdealPosition As Vector(Of Double) = Nothing
    Friend __originalRealPosition As Vector(Of Double) = Nothing

    Friend __dimensionIdealPosition As Vector(Of Double) = Nothing
    Friend __dimensionRealPosition As Vector(Of Double) = Nothing
    Friend __dimensionErrorPosition As Vector(Of Double) = Nothing
    Friend __gainSelection As Integer = 0
    ''' <summary>
    ''' Turns data pair into ErrorGain Collection and ErrorValue Collection
    ''' </summary>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function pairsCascade(collection As List(Of measuredDataPair),
                                        Optional errorVector As Vector(Of Double) = Nothing) As Object

        If errorVector Is Nothing Then
            errorVector = CreateVector.Dense(Of Double)(6)
        End If

        'maps function
        Dim measures = collection.Select(Of Matrix(Of Double))(Function(element As measuredDataPair, index As Integer) element.__dimensionErrorPosition.ToColumnMatrix).ToArray
        Dim jacobians = collection.Select(Of Matrix(Of Double))(Function(element As measuredDataPair, index As Integer) element.ErrorGain(errorVector)).ToArray
        Dim expectedMeasures = collection.Select(Of Matrix(Of Double))(Function(element As measuredDataPair, index As Integer) element.ExpectedMeasure(errorVector).ToColumnMatrix).ToArray

        Dim measuredValue As Matrix(Of Double) = measures.Aggregate(Function(element1 As Matrix(Of Double), element2 As Matrix(Of Double)) element1.Stack(element2))
        Dim jacobianMatrix As Matrix(Of Double) = jacobians.Aggregate(Function(element1 As Matrix(Of Double), element2 As Matrix(Of Double)) element1.Stack(element2))
        Dim expectedMeasure As Matrix(Of Double) = expectedMeasures.Aggregate(Function(element1 As Matrix(Of Double), element2 As Matrix(Of Double)) element1.Stack(element2))

        Return {jacobianMatrix,
                CreateVector.Dense(Of Double)(measuredValue.ToColumnMajorArray),
                CreateVector.Dense(Of Double)(expectedMeasure.ToColumnMajorArray)}
    End Function
    ''' <summary>
    ''' Turns data pair into fitted error vector
    ''' </summary>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function fitErrorVector(collection As List(Of measuredDataPair)) As Vector(Of Double)
        'cascade datas
        Dim cascade = measuredDataPair.pairsCascade(collection)
        Dim errorGains As Matrix(Of Double) = CType(cascade(cascadeDefinitionsEnum.JACOBIAN), Matrix(Of Double))
        Dim errorPositions As Vector(Of Double) = CType(cascade(cascadeDefinitionsEnum.MEASURES), Vector(Of Double))

        'errorGain may not full rank (neither column nor row)
        'do pseudoinverse to analyze the error vector
        Return errorGains.PseudoInverse * errorPositions
    End Function

    ''' <summary>
    ''' The non-linear fitting
    ''' </summary>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function fitTransformation2(collection As List(Of measuredDataPair)) As htmEdgeElementary

        Dim cache As Object = Nothing
        Dim x_next As Vector(Of Double) = CreateVector.Dense(Of Double)(6) 'zero vector initially
        Dim increment As Vector(Of Double) = CreateVector.Dense(Of Double)(6) 'zero vector initially

        Do
            x_next -= increment
            cache = pairsCascade(collection, x_next)

            increment = CType(cache(cascadeDefinitionsEnum.JACOBIAN), Matrix(Of Double)).PseudoInverse *
              (CType(cache(cascadeDefinitionsEnum.EXPECT), Vector(Of Double)) - CType(cache(cascadeDefinitionsEnum.MEASURES), Vector(Of Double)))

        Loop Until increment.L2Norm < iterationTolerance

        Return New eulerHtmTR(Nothing, Nothing) With {.ControlVector = x_next}
    End Function


    Shared iterationTolerance As Double = 0.000001
    Shared iterationGoal As Integer = 100
    Shared iterationCounter As Integer = 0
    Shared lastNorm As Double = Double.MaxValue
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function fitTransformation(collection As List(Of measuredDataPair)) As htmEdgeElementary
        iterationCounter = 0
        Dim output As eulerHtmTR = New eulerHtmTR(Nothing,
                                                                Nothing)
        Dim __revisedCollection As List(Of measuredDataPair) = New List(Of measuredDataPair)(collection)
        '-----------------------------
        'use full xyz rotate/translate
        '-----------------------------
        Dim errorMatrix As eulerHtmTR = New eulerHtmTR(Nothing,
                                               Nothing)

        lastNorm = Double.MaxValue 'reset
        While True

            __revisedCollection.Clear()
            For Each item As measuredDataPair In collection
                __revisedCollection.Add(New measuredDataPair((output * New PositionVector(item.OriginalIdealPosition, Nothing)).RawValue,
                                                             item.OriginalRealPosition,
                                                              item.__gainSelection))
            Next
            errorMatrix.ControlVector = fitErrorVector(__revisedCollection)


            If errorMatrix.ControlVector.L2Norm > lastNorm Or
                iterationCounter = iterationGoal Then
                'iteration end
                Exit While
            Else
                'compensation
                output.RawValue = errorMatrix.RawValue * output.RawValue
            End If

            lastNorm = errorMatrix.ControlVector.L2Norm
            iterationCounter += 1

        End While

        Return output

    End Function


    Shared Function pairsOutput(collection As List(Of measuredDataPair)) As String
        Dim sb As StringBuilder = New StringBuilder
        sb.AppendLine("Real,Ideal,Error")
        collection.ForEach(Sub(pair As measuredDataPair)
                               With sb
                                   'real ideal error
                                   .AppendLine(String.Format("{0},{1},{2}",
                                                             pair.__dimensionRealPosition.ToVectorString.Replace(vbCrLf, vbTab),
                                                             pair.OriginalIdealPosition.ToVectorString.Replace(vbCrLf, vbTab),
                                                             pair.__dimensionErrorPosition.ToVectorString.Replace(vbCrLf, vbTab)))
                               End With
                           End Sub)

        Return sb.ToString
    End Function

    ''' <summary>
    ''' Ideal : the value calculated throught the known chain
    ''' Real : the value measured in unknown chain
    ''' </summary>
    ''' <param name="originalIdealPosition"></param>
    ''' <param name="originalRealPosition"></param>
    ''' <param name="selection"></param>
    ''' <remarks></remarks>
    Sub New(originalIdealPosition As Vector(Of Double),
            originalRealPosition As Vector(Of Double),
           Optional selection As utilities.selectionEnums = selectionEnums.X Or
           selectionEnums.Y Or
           selectionEnums.Z)

        Dim value = [Enum].GetValues(GetType(utilities.selectionEnums))

        Dim idealValues As List(Of Double) = New List(Of Double)
        Dim realValues As List(Of Double) = New List(Of Double)

        For index = 0 To value.Length - 1
            If selection And value(index) Then
                'if dimension selected
                idealValues.Add(originalIdealPosition(index))
                realValues.Add(originalRealPosition(index))
            End If
        Next

        Me.__originalIdealPosition = originalIdealPosition
        Me.__originalRealPosition = originalRealPosition
        Me.__originalErrorPosition = Me.__originalIdealPosition - Me.__originalRealPosition

        Me.__gainSelection = selection
        Me.__dimensionIdealPosition = CreateVector.DenseOfArray(Of Double)(idealValues.ToArray)
        Me.__dimensionRealPosition = CreateVector.DenseOfArray(Of Double)(realValues.ToArray)
        Me.__dimensionErrorPosition = __dimensionIdealPosition - __dimensionRealPosition
        Me.__errorGain = utilities.position2ErrorGain(originalIdealPosition,
                                                      selection)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function averageErrorVector(collection As List(Of measuredDataPair)) As Vector(Of Double)

        Dim __sum As Vector(Of Double) = CreateVector.Dense(Of Double)(collection.First.OriginalErrorPosition.Count)

        collection.ForEach(Sub(__data As measuredDataPair) __sum += __data.OriginalErrorPosition)

        Return __sum / collection.Count
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function averageErrorLength(collection As List(Of measuredDataPair)) As Double

        Dim __sum As Double = 0

        collection.ForEach(Sub(__data As measuredDataPair) __sum += __data.ErrorLength)

        Return __sum / collection.Count
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function deviationErrorLength(collection As List(Of measuredDataPair)) As Double

        Dim __sample As List(Of Double) = New List(Of Double)

        collection.ForEach(Sub(__data As measuredDataPair) __sample.Add(__data.ErrorLength))

        Return MathNet.Numerics.Statistics.Statistics.StandardDeviation(__sample)
    End Function
End Class