﻿Imports Automation
Imports AutoNumeric
Imports System.Linq
Imports MathNet.Numerics.LinearAlgebra
Imports FA.dmCalibrationSetting
Imports FA.distanceMeter
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Text

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class dmCalibrationSetting
    Inherits measureProcedureSetting

    ''' <summary>
    ''' Generate shift-search route
    ''' </summary>
    ''' <remarks></remarks>
    <Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
    Class edgeSearchSetting
        Implements IRoute

        <Browsable(False)>
        <XmlIgnore()>
        Property StartPoint As PositionVector
            Get
                Return __startPoint
            End Get
            Set(value As PositionVector)
                __startPoint = value
            End Set
        End Property
        ''' <summary>
        ''' In R-Frame
        ''' </summary>
        ''' <remarks></remarks>
        Dim __startPoint As PositionVector = New PositionVector(framesDefinition.R)

        Property StartOffset As Double = 0.3

        Property SearchDirection As axisEntityEnum = axisEntityEnum.X
        Property ShiftDirection As axisEntityEnum = axisEntityEnum.Y

        Property SearchDepth As Double = 3
        Property ShiftPitch As Double = 0.1
        Property ShiftCount As Integer = 10

        ''' <summary>
        ''' In mm/sec
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Speed As Double = 1
        Property ReturnSpeed As Double = 10

        Property StartSpeed As Double = 0.1
        Property AccerlerationTime As Double = 0.05
        Property DecerlerationTime As Double = 0.05


        <XmlIgnore()>
        <Browsable(False)>
        Public ReadOnly Property MeasurePoints As List(Of PositionVector) Implements IRoute.MeasurePoints
            Get
                Dim output As List(Of PositionVector) = New List(Of PositionVector)
                Dim routeStart As PositionVector = __startPoint.Clone
                Dim routeEnd As PositionVector = __startPoint.Clone
                routeEnd.AxisValue(SearchDirection) += SearchDepth

                Dim counter As Integer = 0
                While counter <= ShiftCount
                    output.AddRange({routeStart.Clone,
                                     routeEnd.Clone})

                    routeStart.AxisValue(ShiftDirection) += ShiftPitch
                    routeEnd.AxisValue(ShiftDirection) += ShiftPitch
                    counter += 1
                End While

                Return output
            End Get
        End Property

        Friend pointCloud As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

    End Class


    <Browsable(False)>
    <XmlIgnore()>
    Friend ReadOnly Property ImageProcesses As Dictionary(Of framesDefinition, imageProcessSettingBlock)
        Get
            Dim output As Dictionary(Of framesDefinition, imageProcessSettingBlock) = New Dictionary(Of framesDefinition, imageProcessSettingBlock)
            output(framesDefinition.C1REAL) = C1ImageProcess
            output(framesDefinition.C2REAL) = C2ImageProcess
            output(framesDefinition.C4) = C4ImageProcess
            Return output
        End Get
    End Property
    <Browsable(False)>
   <XmlIgnore()>
    Friend ReadOnly Property RouteProcesses As Dictionary(Of framesDefinition, edgeSearchSetting)
        Get
            Dim output As Dictionary(Of framesDefinition, edgeSearchSetting) = New Dictionary(Of framesDefinition, edgeSearchSetting)
            output(framesDefinition.C1REAL) = C1RouteProcess
            output(framesDefinition.C2REAL) = C2RouteProcess
            Return output
        End Get
    End Property

    Property C1ImageProcess As imageProcessSettingBlock = New imageProcessSettingBlock
    Property C2ImageProcess As imageProcessSettingBlock = New imageProcessSettingBlock
    Property C4ImageProcess As imageProcessSettingBlock = New imageProcessSettingBlock

    Property C1RouteProcess As edgeSearchSetting = New edgeSearchSetting
    Property C2RouteProcess As edgeSearchSetting = New edgeSearchSetting

    Sub New()

    End Sub

End Class

''' <summary>
''' Procedures
''' 1. Using C1,C2,C3 Fetch edge line (Double Point)
''' 2. Using DM Cut-Measuring Edges , get N data points (3D)
''' 3. Transform these points , referenced on C1-Frame (X,Y values)
''' 4. Calculate Point-Line Distance , along Y-Axis of C1 , get N differential distance , make a standard variation as reference
''' 5. Average these value , get the Z-offset
''' </summary>
''' <remarks></remarks>
Public Class dmCalibration
    Inherits measureProcedureType1Base

    Protected ReadOnly Property Setting As dmCalibrationSetting
        Get
            Return CType(__measureSetting, dmCalibrationSetting)
        End Get
    End Property

    'Friend setting As dmCalibrationSetting = New dmCalibrationSetting

    Dim currentImageProcess As Dictionary(Of framesDefinition, imageProcessSettingBlock).Enumerator = Nothing
    Dim currentEdgeProcess As Dictionary(Of framesDefinition, edgeSearchSetting).Enumerator = Nothing

    Dim currentRoutePoint As List(Of PositionVector).Enumerator = Nothing

    Friend cornerByCamerasInR As Vector(Of Double) = Nothing

    ''' <summary>
    ''' Cached the image results
    ''' </summary>
    ''' <remarks></remarks>
    Dim imageResultsInReference As Dictionary(Of framesDefinition, List(Of Vector(Of Double))) =
        New Dictionary(Of framesDefinition, List(Of Vector(Of Double)))

    Enum subStatesEnum As Integer
        READY = 0
        EDGE_IMAGE = 100

        EDGE_START = 200
        ROUTE_START = 300

        DATA_PROCESS = 400
        PROCESS_DONE = 500
    End Enum

    Protected Sub New()
        MyBase.New(compenstationMethodEnums.AS_PASSIVE_OBJECT,
                   New dmCalibrationSetting,
                   frames.Instance.Elementray(framesDefinition.LREAL, framesDefinition.L))
    End Sub

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As dmCalibration
        Get
            If __instance Is Nothing Then
                __instance = New dmCalibration
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As dmCalibration = Nothing
#End Region

    ''' <summary>
    ''' Output data pairs
    ''' 1.
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function measureProcedure(ByRef state As Integer) As Boolean
        Select Case state
            Case subStatesEnum.READY
                With frames.Instance
                    .CurrentMovingItem = framesDefinition.C4
                    .CurrentRItem = itemsDefinition.CHOKE_CORNER1
                End With
                state += 10
            Case 10
                If Assembly.Instance.IsAllAxesSettled Then
                    currentImageProcess = Setting.ImageProcesses.GetEnumerator
                    state = subStatesEnum.EDGE_IMAGE
                Else
                    '----------------
                    '   Settling
                    '----------------
                End If
                '------------------------------
                '   Edge Image
                '------------------------------
            Case subStatesEnum.EDGE_IMAGE
                If currentImageProcess.MoveNext Then
                    state += 10
                Else
                    '--------------------
                    '   All Process Done
                    '--------------------
                    'output and check
                    sendMessage(internalEnum.GENERIC_MESSAGE,
                                String.Format("C1,{0}{3}C2,{1}{3}C4,{2}",
                                              imageResultsInReference(framesDefinition.C1REAL).First.ToVectorString.Replace(vbCrLf, vbTab),
                                              imageResultsInReference(framesDefinition.C2REAL).First.ToVectorString.Replace(vbCrLf, vbTab),
                                              imageResultsInReference(framesDefinition.C4).First.ToVectorString.Replace(vbCrLf, vbTab),
                                              vbCrLf))

                    'calculated out the corner coordinate , update
                    imageResultsInReference(framesDefinition.C1REAL).First().Item(axisEntityEnum.X) = 0
                    imageResultsInReference(framesDefinition.C2REAL).First().Item(axisEntityEnum.Y) = 0
                    imageResultsInReference(framesDefinition.C4).First().Item(axisEntityEnum.Z) = 0
                    'transform measure values into LREAL frame (ideal value)
                    cornerByCamerasInR = CreateVector.Dense(Of Double)({0,
                                                                     0,
                                                                     0,
                                                                     0})
                    For Each pair As KeyValuePair(Of framesDefinition, List(Of Vector(Of Double))) In imageResultsInReference
                        cornerByCamerasInR += pair.Value.First
                    Next
                    'the average
                    cornerByCamerasInR /= 2
                    cornerByCamerasInR(cornerByCamerasInR.Count - 1) = 1 'reset the scale factor
                    'update
                    frames.Instance.objectsDictionary(itemsDefinition.CHOKE_CORNER1).RawValue = cornerByCamerasInR

                    Dim __startPoint As PositionVector = New PositionVector(cornerByCamerasInR, framesDefinition.R)

                    With Setting
                        With .C1RouteProcess
                            .StartPoint = __startPoint.Clone
                            .StartPoint.Y += .StartOffset
                            .StartPoint.X += .StartOffset

                            '.StartPoint.X -= .SearchDepth / 2
                        End With
                        With .C2RouteProcess
                            .StartPoint = __startPoint.Clone
                            .StartPoint.Y += .StartOffset
                            .StartPoint.X += .StartOffset

                            '.StartPoint.Y -= .SearchDepth / 2
                        End With
                    End With

                    currentEdgeProcess = Setting.RouteProcesses.GetEnumerator
                    state = subStatesEnum.EDGE_START
                End If
                '-------------------
                '   Image Trigger
                '-------------------
            Case subStatesEnum.EDGE_IMAGE + 10
                currentImageProcess.Current.Value.onCameraTriggered()
                state += 10
            Case subStatesEnum.EDGE_IMAGE + 20
                With currentImageProcess.Current
                    If .Value.IsImageProcessDone Then

                        If .Value.Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                            'transform to reference frame
                            Me.imageResultsInReference(.Key) = .Value.Coordinates

                            'transform to reference frame
                            Dim __collection As List(Of Vector(Of Double)) = imageResultsInReference(.Key)
                            For index = 0 To __collection.Count - 1
                                If __collection(index) IsNot Nothing Then
                                    __collection(index) = (frames.Instance.Transformation(.Key, framesDefinition.R) *
                                                                 New PositionVector(__collection(index), .Key)).RawValue
                                Else
                                    '-----------------
                                    '   Corresponding slot data inexisted
                                    '-----------------
                                End If


                            Next

                            state = subStatesEnum.EDGE_IMAGE
                        Else
                            '----------------------
                            '   Process failed
                            '----------------------
                        End If
                    Else
                        '---------------------------
                        '   Image Processing
                        '---------------------------
                    End If
                End With
                '-----------------------------
                '   Edge Searching Start
                '-----------------------------
            Case subStatesEnum.EDGE_START
                If currentEdgeProcess.MoveNext Then
                    currentEdgeProcess.Current.Value.pointCloud.Clear()
                    currentRoutePoint = currentEdgeProcess.Current.Value.MeasurePoints.GetEnumerator
                    state = subStatesEnum.ROUTE_START
                Else
                    '---------------------
                    '   All Route Executed
                    '---------------------
                    state = subStatesEnum.DATA_PROCESS
                End If
                '--------------------------------
                '   Route Execution
                '--------------------------------
            Case subStatesEnum.ROUTE_START
                If currentRoutePoint.MoveNext Then
                    'move to start
                    With frames.Instance
                        .solveAbsAxAy(currentRoutePoint.Current, framesDefinition.LREAL)
                    End With
                    state += 10
                Else
                    '---------------------------
                    '   No Route able to execute, check another edge
                    '---------------------------
                    state = subStatesEnum.EDGE_START
                End If
            Case subStatesEnum.ROUTE_START + 10
                If Assembly.Instance.IsAllAxesSettled Then
                    state += 10
                Else
                    '---------------------
                    '   Settling
                    '---------------------
                End If
            Case subStatesEnum.ROUTE_START + 20
                If Assembly.Instance.__distanceMeter.drive(dmCommands.RT, {dmOnOff.__ON}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    state += 10
                Else
                    '-------------------
                    '   Communication
                    '-------------------
                End If
            Case subStatesEnum.ROUTE_START + 30
                If Assembly.Instance.__distanceMeter.drive(dmCommands.RT, {dmOnOff.__OFF}) =
                     IDrivable.endStatus.EXECUTION_END Then

                    currentRoutePoint.MoveNext() ' 

                    For Each item As controlUnitsEnum In {controlUnitsEnum.X,
                                                          controlUnitsEnum.Y}
                        With Assembly.Instance.Profile(item)
                            .VelocityInUnit = currentEdgeProcess.Current.Value.Speed
                            .StartVelocityInUnit = currentEdgeProcess.Current.Value.StartSpeed
                            .AccelerationTime = currentEdgeProcess.Current.Value.AccerlerationTime
                            .DecelerationTime = currentEdgeProcess.Current.Value.DecerlerationTime
                        End With
                    Next



                    frames.Instance.solveAbsAxAy(currentRoutePoint.Current, framesDefinition.LREAL)
                    state += 10

                Else
                    '-------------------
                    '   Communication
                    '-------------------
                End If
            Case subStatesEnum.ROUTE_START + 40
                If Assembly.Instance.IsAllAxesSettled Then

                    Assembly.Instance.Profile(controlUnitsEnum.X).VelocityInUnit = currentEdgeProcess.Current.Value.ReturnSpeed
                    Assembly.Instance.Profile(controlUnitsEnum.Y).VelocityInUnit = currentEdgeProcess.Current.Value.ReturnSpeed

                    state += 10
                Else
                    '-------------------
                    '   Settling
                    '-------------------
                End If
            Case subStatesEnum.ROUTE_START + 50
                If Assembly.Instance.__distanceMeter.drive(dmCommands.MS, {dmTaskEnum.TASK_EDGE_AUTO_PEAK}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    state += 10
                Else
                    '-------------------
                    '   Communication
                    '-------------------
                End If
            Case subStatesEnum.ROUTE_START + 60
                With Assembly.Instance

                    If .xMotorControl.IsLatched And
                        .yMotorControl.IsLatched And
                      Assembly.Instance.__distanceMeter.IsMeasureValueAvailable Then

                        'the measure point in LREAL-Frame
                        Dim measuredPointInLreal As PositionVector = New PositionVector(framesDefinition.LREAL) With {.Z =
                            Assembly.Instance.__distanceMeter.MeasureValue}

                        'follow the real Z-axis height
                        Dim transformationLRealToR As htmEdgeElementary = frames.Instance.ForwardKinematic(.xMotorControl.LatchedValue,
                                                                                                           .yMotorControl.LatchedValue,
                                                                                                           c4htm.Instance.AxisValue(axisEntityEnum.Z),
                                                                                                           framesDefinition.LREAL)
                        'calculating measure point in R
                        Dim measuredPointInR As PositionVector = transformationLRealToR *
                                                                                                    measuredPointInLreal

                        currentEdgeProcess.Current.Value.pointCloud.Add(measuredPointInR.RawValue.SubVector(0, 3))
                    Else
                        '--------------------
                        '   Not scanned the edge point
                        '--------------------
                    End If

                    state = subStatesEnum.ROUTE_START

                End With
            Case subStatesEnum.DATA_PROCESS
                'calculated out peak position by distance meter
                'In LREAL Frame (real value)

                'output point cloud
                Dim sb As StringBuilder = New StringBuilder
                sb.AppendLine(String.Format("Point Clouds,{0},{1}",
                                            Setting.C1RouteProcess.pointCloud.Count,
                                            Setting.C2RouteProcess.pointCloud.Count))
                For Each item As List(Of Vector(Of Double)) In {Setting.C1RouteProcess.pointCloud,
                                                                Setting.C2RouteProcess.pointCloud}
                    For Each __point As Vector(Of Double) In item
                        sb.AppendLine(__point.ToVectorString.Replace(vbCrLf, ","))
                    Next
                Next
                sendMessage(internalEnum.GENERIC_MESSAGE, sb.ToString)

                '--------------------------
                '   Once collecting failed
                '--------------------------
                If Setting.RouteProcesses(framesDefinition.C1REAL).pointCloud.Count = 0 Or
                    Setting.RouteProcesses(framesDefinition.C2REAL).pointCloud.Count = 0 Then
                    IsProcedureAbort.setFlag(interlockedFlag.POSITION_OCCUPIED) 'self-terminating
                    Return False
                End If

                Dim fittedCornerInR As Vector(Of Double) = AutoNumeric.fittingMethods.line3DIntersection(Setting.RouteProcesses(framesDefinition.C1REAL).pointCloud,
                                                                                                         Setting.RouteProcesses(framesDefinition.C2REAL).pointCloud,
                                                                                                         3)

                sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Fitted Corner:{0}", fittedCornerInR.ToVectorString.Replace(vbCrLf, vbTab)))
                sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Camera Corner:{0}", cornerByCamerasInR.ToVectorString.Replace(vbCrLf, vbTab)))

                Dim transformationRtoLReal = frames.Instance.Transformation(framesDefinition.R,
                                                                        framesDefinition.LREAL)

                'real
                Dim cornerByDistanceMeterInLReal As Vector(Of Double) = (transformationRtoLReal *
                                                                         New PositionVector(fittedCornerInR, framesDefinition.R)).RawValue
                'calculated out peak position by cameras (R2)
                'In LREAL Frame
                'ideal
                Dim cornerByCamerasInLreal As Vector(Of Double) = (transformationRtoLReal *
                                                                 New PositionVector(cornerByCamerasInR, framesDefinition.R)).RawValue

                MyBase.dataPairCollection.Clear()
                MyBase.dataPairCollection.Add(New measuredDataPair(cornerByCamerasInLreal,
                                                                   cornerByDistanceMeterInLReal))

                Return True
        End Select

        Return False
    End Function

    Protected Overrides Function dataHandlingMethod(__dataCollection As List(Of measuredDataPair)) As htmEdgeElementary
        'speed reset
        Assembly.Instance.Profile(controlUnitsEnum.X).VelocityInUnit = pData.MotorPoints(motorPoints.MsX_ZERO).VelocityInUnit
        Assembly.Instance.Profile(controlUnitsEnum.Y).VelocityInUnit = pData.MotorPoints(motorPoints.MsY_ZERO).VelocityInUnit

        Dim output As htmEdgeElementary = New htmEdgeElementary(Nothing, Nothing)
        output.Origin = New PositionVector(measuredDataPair.averageErrorVector(__dataCollection), Nothing)
        'which origin is compensated
        Return output
    End Function
End Class


