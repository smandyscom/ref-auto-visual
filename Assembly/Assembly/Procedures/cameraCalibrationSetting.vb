﻿Imports Automation
Imports System.Xml.Serialization
Imports System.IO
Imports AutoNumeric
Imports System.ComponentModel
Imports System.Drawing.Design

''' <summary>
''' Supports measuring route generating
''' </summary>
''' <remarks></remarks>
Public Interface IRoute
    ReadOnly Property MeasurePoints As List(Of PositionVector)
End Interface

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class cameraCalibrationSettingBase
    Inherits measureProcedureSetting
    Implements IRoute


    ''' <summary>
    ''' Calibration goal
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property RealReference As framesDefinition
        Get
            Return __realReference
        End Get
    End Property
    ReadOnly Property NominalReference As framesDefinition
        Get
            Return __nominalReference
        End Get
    End Property

    Protected __realReference As framesDefinition = framesDefinition.C1REAL
    Protected __nominalReference As framesDefinition = framesDefinition.C1

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
    Property ImageProcessSetting As imageProcessSettingBlock = New imageProcessSettingBlock

    Sub New(__realReference As framesDefinition,
            __nominalReference As framesDefinition)

        Me.__realReference = __realReference
        Me.__nominalReference = __nominalReference

    End Sub

    ''' <summary>
    ''' For loading use
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}",
                             __realReference.ToString,
                             Me.GetType.ToString)
    End Function

    <Browsable(False)>
    <XmlIgnore()>
    Public Overridable ReadOnly Property MeasurePoints As List(Of PositionVector) Implements IRoute.MeasurePoints
        Get
            Return Nothing
        End Get
    End Property
End Class


Public Class cameraCalibrationSettingRectangle
    Inherits cameraCalibrationSettingBase
    Implements IRoute


    Property LeftUpCorner As String
        Get
            Return __leftUpCorner.PositionText
        End Get
        Set(value As String)
            __leftUpCorner.PositionText = value
        End Set
    End Property
    ''' <summary>
    ''' Backend data
    ''' </summary>
    ''' <remarks></remarks>
    Friend __leftUpCorner As PositionVector = New PositionVector(Nothing)

    Property XPitch As Double = 1
    Property XCounts As Integer = 5
    Property YPitch As Double = 1
    Property YCounts As Integer = 5
    Property ZPitch As Double = 0
    Property ZCounts As Integer = 1

    ''' <summary>
    ''' Given a nominal pitch angle to stimulate error factor
    ''' In radien
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property SurfacePitchSwing As Double = 0.005
    Property SurfaceRollSwing As Double = 0.005

    Sub New(__realReference As framesDefinition,
            __nominalReference As framesDefinition)
        MyBase.New(__realReference,
                   __nominalReference)
    End Sub
    Public Sub New()

    End Sub
    <Browsable(False)>
    <XmlIgnore()>
    Public Overrides ReadOnly Property MeasurePoints As List(Of PositionVector)
        Get
            Dim outputList As List(Of PositionVector) = New List(Of PositionVector)
            Dim outputPoint As PositionVector = New PositionVector(__leftUpCorner.RawValue, __realReference)

            'initial point
            outputList.Add(outputPoint.Clone)

            'initialate transformation
            Dim __transformation As htmEdgeElementary = New htmEdgeElementary(__realReference, __realReference)

            '-------------------------------------------
            '   Do Plus/Minus Swing to search best angle
            '-------------------------------------------
            Dim swingSequence As List(Of Double()) = New List(Of Double()) From {New Double() {-1, 0},
                                                                                New Double() {1, 0},
                                                                                 New Double() {0, 0},
                                                                                 New Double() {0, -1},
                                                                                 New Double() {0, 1}}
            For index = 0 To swingSequence.Count - 1
                swingSequence(index)(0) *= SurfacePitchSwing
                swingSequence(index)(1) *= SurfaceRollSwing
            Next

            'remove repeative sequence
            For index = 0 To swingSequence.Count - 1
                If index > swingSequence.Count - 1 Then
                    Exit For
                End If
                'left the unique one
                Dim item = swingSequence(index)
                swingSequence.RemoveAll(Function(pair As Double())
                                            Return pair(0) = item(0) And pair(1) = item(1)
                                        End Function)
                swingSequence.Add(item)
            Next


            For index = 0 To swingSequence.Count - 1

                Dim pitch As Double = swingSequence(index)(0)
                Dim roll As Double = swingSequence(index)(1)

                __transformation.RotationMatrix =
                    AutoNumeric.utilities.RotateTransformation(pitch,
                                                                roll, 0)

                For zIndex = 0 To ZCounts - 1
                    For yIndex = 0 To YCounts - 1
                        For xIndex = 0 To XCounts - 1
                            'as fifo
                            outputList.Add(__transformation * outputPoint)
                            outputPoint.X += XPitch
                        Next
                        outputPoint.X = __leftUpCorner.X 'x value reset
                        outputPoint.Y += YPitch
                    Next
                    outputPoint.Y = __leftUpCorner.Y 'y value reset
                    outputPoint.Z += ZPitch
                Next
            Next
            outputPoint.Z = __leftUpCorner.Z 'z value reset

            Return outputList
        End Get
    End Property
End Class


Public Class cameraCalibrationCircle
    Inherits cameraCalibrationSettingBase

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property CenterText As String
        Get
            Return centerPosition.PositionText
        End Get
        Set(value As String)
            centerPosition = New PositionVector(__realReference)
            centerPosition.PositionText = value
        End Set
    End Property
    Friend centerPosition As PositionVector = New PositionVector(Nothing)

    ''' <summary>
    ''' In mm
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Radius As Double = 2
    ''' <summary>
    ''' How many points should be generated
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Divides As Integer = 10

    <XmlIgnore()>
    Public Overrides ReadOnly Property MeasurePoints As List(Of PositionVector)
        Get
            Dim outputList As List(Of PositionVector) = New List(Of PositionVector)
            Dim outputPoint As PositionVector = Nothing

            Dim radIncrement As Double = 2 * Math.PI / Divides
            Dim radAccumulate As Double = 0

            For index = 0 To Divides - 1
                outputPoint = New PositionVector(RealReference)
                With outputPoint
                    .X = centerPosition.X + Radius * Math.Cos(radAccumulate)
                    .Y = centerPosition.Y + Radius * Math.Sin(radAccumulate)
                    .Z = 0
                End With
                radAccumulate += radIncrement

                outputList.Add(outputPoint)
            Next
            Return outputList
        End Get
    End Property

    Sub New(__realReference As framesDefinition,
           __nominalReference As framesDefinition)
        MyBase.New(__realReference,
                   __nominalReference)
    End Sub
    Protected Sub New()

    End Sub

End Class