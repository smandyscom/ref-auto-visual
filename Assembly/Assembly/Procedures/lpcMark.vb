﻿Imports AutoNumeric
Imports Cognex.VisionPro.ToolBlock
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric.utilities
Imports System.Xml.Serialization
Imports System.IO
Imports System.ComponentModel

''' <summary>
''' Feature(surface) measurement procedure and related settings
''' </summary>
''' <remarks></remarks>
Public Class lpcFeatureMeasureSetting
    Inherits imageProcessSettingBlock

    <XmlIgnore()>
    Protected Property CameraFrameReal As framesDefinition
        Get
            Return __cameraFrameReal
        End Get
        Set(value As framesDefinition)
            __cameraFrameReal = value
            Select Case __cameraFrameReal
                Case framesDefinition.C1REAL
                    __cameraFrameIdeal = framesDefinition.C1
                    __dimensionSelection = selectionEnums.Y Or selectionEnums.Z
                    __readyPosition = itemsDefinition.C1_ORIGIN
                Case framesDefinition.C2REAL
                    __cameraFrameIdeal = framesDefinition.C2
                    __dimensionSelection = selectionEnums.X Or selectionEnums.Z
                    __readyPosition = itemsDefinition.C2_ORIGIN
                Case framesDefinition.C3REAL
                    __cameraFrameIdeal = framesDefinition.C3
                    __dimensionSelection = selectionEnums.X Or selectionEnums.Y
                    __readyPosition = itemsDefinition.C3_ORIGIN
            End Select
        End Set
    End Property

    <XmlIgnore()>
    Property EnabledFeatures As List(Of Boolean) = New List(Of Boolean)

    Protected __readyPosition As itemsDefinition = itemsDefinition.C1_ORIGIN
    Protected __featurePosition As itemsDefinition = itemsDefinition.LPC_F1

    Protected __cameraFrameIdeal As framesDefinition = framesDefinition.C1

    Dim __cameraFrameReal As framesDefinition = framesDefinition.C1REAL

    Protected __interestedFeatures As List(Of itemsDefinition) = New List(Of itemsDefinition)

    Protected __dimensionSelection As Integer = selectionEnums.X Or selectionEnums.Y

    Friend __outputList As List(Of measuredDataPair) = New List(Of measuredDataPair)

    ''' <summary>
    ''' Rotate about X-axis, unit in radien
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Pitch As Double = 0
    ''' <summary>
    ''' Rotate about Y-axis, unit in radien
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Roll As Double = 0

    <XmlIgnore()>
    <Browsable(False)>
    ReadOnly Property FeaturePositionTransformation As htmEdgeElementary
        Get
            Dim targetTransformation As htmEdgeElementary = New htmEdgeElementary(framesDefinition.LPC_REAL,
                                                                                  CameraFrameReal)
            Dim featurePointCoordinate As htmEdgeElementary = New htmEdgeElementary(framesDefinition.LPC_REAL,
                                                                                    framesDefinition.LPC_REAL) With {.PositionVector = frames.Instance.objectsDictionary(__featurePosition).RawValue}

            With targetTransformation
                'go to the nominal pose
                .RotationMatrix = frames.Instance.Transformation(framesDefinition.LPC, __cameraFrameIdeal).RotationMatrix *
                    AutoNumeric.utilities.RotateTransformation(Pitch, Roll, 0)

            End With
            ''origin coincidence
            targetTransformation = targetTransformation * featurePointCoordinate.Inverse
            Return targetTransformation
        End Get
    End Property

    Function procedure(ByRef state As Integer) As Boolean
        With frames.Instance
            Select Case state
                Case 0
                    'move to ready position for X stage
                    .CurrentMovingItem = framesDefinition.S0
                    .CurrentRItem = __readyPosition
                    Me.__outputList.Clear()
                    state += 10
                Case 10
                    'align the feature center
                    If Assembly.Instance.IsAllAxesSettled Then
                        'would auto compensate pose-error , move real frame to meet the nominal pose
                        .solveS(FeaturePositionTransformation)
                        state += 10
                    Else
                        '--------------------
                        '   Settling
                        '--------------------
                    End If
                Case 20
                    If Assembly.Instance.CommandEndStatus(controlUnitsEnum.S) =
                        Automation.IDrivable.endStatus.EXECUTION_END Then

                        'initiating measuring procedure
                        '-------------------------------------------------------
                        '   Image Processing
                        '-------------------------------------------------------
                        onCameraTriggered()
                        state = 100
                    Else
                        '--------------
                        '   Moving
                        '--------------
                    End If
                Case 100
                    If IsImageProcessDone And
                        Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                        'take measured value/nominal value into list
                        'there's one dimension missing on camera


                        If Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                            For index = 0 To __interestedFeatures.Count - 1
                                If EnabledFeatures(index) Then
                                    'error on LPC-REAL_Frame
                                    Dim nominalValue = .objectsDictionary(__interestedFeatures(index))
                                    Dim realValue = .Transformation(CameraFrameReal, framesDefinition.LPC_REAL) * New PositionVector(Coordinates(index), CameraFrameReal)

                                    __outputList.Add(New measuredDataPair(nominalValue.RawValue,
                                                                        realValue.RawValue,
                                                                        __dimensionSelection))
                                Else
                                    '-------------
                                    ''   Dont take into account
                                    '-------------
                                End If


                            Next
                        Else
                            '--------------------
                            'Image Process Failed
                            '--------------------
                        End If


                        '------------------------
                        '   Back to safe position
                        '------------------------
                        sHtm.Instance.ControlVector = sHtm.Instance.SafePose.Clone
                        state = 500
                    Else
                        '------------------
                        '   Camera working
                        '-----------------
                    End If
                    '-----------------------------
                    '   Wait Smodpod Return
                    '-----------------------------
                Case 500
                    If Assembly.Instance.CommandEndStatus(controlUnitsEnum.S) =
                         Automation.IDrivable.endStatus.EXECUTION_END Then
                        Return True
                    Else
                        '----------------
                        '
                        '----------------
                    End If
            End Select

        End With

        Return False
    End Function

    Property FeaturePoint As itemsDefinition
        Get
            Return __featurePosition
        End Get
        Set(value As itemsDefinition)
            Me.__featurePosition = value
            __interestedFeatures.Clear()

            'set camera frame
            Select Case value
                Case itemsDefinition.LPC_F1,
                    itemsDefinition.LPC_F4
                    CameraFrameReal = framesDefinition.C1REAL
                Case itemsDefinition.LPC_F2,
                    itemsDefinition.LPC_F5
                    CameraFrameReal = framesDefinition.C2REAL
                Case itemsDefinition.LPC_F3
                    CameraFrameReal = framesDefinition.C3REAL
            End Select

            EnabledFeatures.Clear()
            'set interseted features
            Select Case value
                Case itemsDefinition.LPC_F1
                    With __interestedFeatures
                        .Add(itemsDefinition.LPC_R1)
                        .Add(itemsDefinition.LPC_R2)
                    End With
                    With EnabledFeatures
                        .Add(True)
                        .Add(True)
                    End With
                Case itemsDefinition.LPC_F2
                    With __interestedFeatures
                        .Add(itemsDefinition.LPC_R2)
                        .Add(itemsDefinition.LPC_R3)
                    End With
                    With EnabledFeatures
                        .Add(True)
                        .Add(True)
                    End With
                Case itemsDefinition.LPC_F3
                    With __interestedFeatures
                        .Add(itemsDefinition.LPC_R1)
                        .Add(itemsDefinition.LPC_R4)
                        .Add(itemsDefinition.LPC_R2)
                        .Add(itemsDefinition.LPC_R3)
                        .Add(itemsDefinition.LPC_H1)
                        .Add(itemsDefinition.LPC_H2)
                    End With
                    With EnabledFeatures
                        .Add(True)
                        .Add(True)
                        .Add(True)
                        .Add(True)
                        .Add(True)
                        .Add(True)
                    End With
                    '-----------------------
                    'the oppsitie side of F1
                    '-----------------------
                Case itemsDefinition.LPC_F4
                    With __interestedFeatures
                        .Add(itemsDefinition.LPC_R2)
                        .Add(itemsDefinition.LPC_R3)
                    End With
                    With EnabledFeatures
                        .Add(True)
                        .Add(True)
                    End With
                Case itemsDefinition.LPC_F5
                    With __interestedFeatures
                        .Add(itemsDefinition.LPC_R1)
                        .Add(itemsDefinition.LPC_R4)
                    End With
                    With EnabledFeatures
                        .Add(True)
                        .Add(True)
                    End With
            End Select


        End Set
    End Property

#Region "persistance"
    Shared settingPath As String = My.Application.Info.DirectoryPath & "\Data\feature\"
    <XmlIgnore()>
    Public Overrides Property Filename As String
        Get
            If Not Directory.Exists(settingPath) Then
                Directory.CreateDirectory(settingPath)
            End If

            Return String.Format("{0}{1}.xml",
                                 settingPath,
                                 Me.ToString)
        End Get
        Set(value As String)
            'nothing to do
        End Set
    End Property

    Public Overrides Sub Load(filename As String)
        MyBase.Load(filename)
        Try
            Me.applyPropertyChange() ' after loaded , transmit setting
        Catch ex As Exception
        End Try
    End Sub
    Public Overrides Sub Save()
        MyBase.Save()
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}",
                             Me.__featurePosition.ToString,
                             Me.GetType.ToString)
    End Function
#End Region

End Class


Public Class lpcMeasureSetting
    Inherits measureProcedureSetting

    <XmlIgnore()>
    ReadOnly Property FeatureMeasureSettings As Dictionary(Of itemsDefinition, lpcFeatureMeasureSetting)
        Get
            If __featureMeasureSettings Is Nothing Then
                __featureMeasureSettings = New Dictionary(Of itemsDefinition, lpcFeatureMeasureSetting)
                __featureMeasureSettings(itemsDefinition.LPC_F1) = F1Setting
                __featureMeasureSettings(itemsDefinition.LPC_F2) = F2Setting
                __featureMeasureSettings(itemsDefinition.LPC_F3) = F3Setting
                '__featureMeasureSettings(itemsDefinition.LPC_F4) = F4Setting
                '__featureMeasureSettings(itemsDefinition.LPC_F5) = F5Setting
            End If
            Return __featureMeasureSettings
        End Get
    End Property
    Protected __featureMeasureSettings As Dictionary(Of itemsDefinition, lpcFeatureMeasureSetting) = Nothing
    Friend currentFeatureProcedure As Dictionary(Of itemsDefinition, lpcFeatureMeasureSetting).Enumerator = Nothing

    Property F1Setting As lpcFeatureMeasureSetting = New lpcFeatureMeasureSetting
    Property F2Setting As lpcFeatureMeasureSetting = New lpcFeatureMeasureSetting
    Property F3Setting As lpcFeatureMeasureSetting = New lpcFeatureMeasureSetting
    Property F4Setting As lpcFeatureMeasureSetting = New lpcFeatureMeasureSetting
    Property F5Setting As lpcFeatureMeasureSetting = New lpcFeatureMeasureSetting

    Public Overrides Sub Load(filename As String)
        MyBase.Load(filename)
        F1Setting.FeaturePoint = itemsDefinition.LPC_F1
        F2Setting.FeaturePoint = itemsDefinition.LPC_F2
        F3Setting.FeaturePoint = itemsDefinition.LPC_F3
        F4Setting.FeaturePoint = itemsDefinition.LPC_F4
        F5Setting.FeaturePoint = itemsDefinition.LPC_F5

    End Sub

End Class

''' <summary>
''' 1.Align F1 to C1 , measure R1,R2
''' 2.Align F2 to C2 , meausre R2,R3
''' 3.Align F3 to C3 (Orientation need) , measure R1,R2,R3,H1,H2
''' 4. Do error fitting
''' 5. Do step 1-4 until error vector converged 
''' </summary>
''' <remarks></remarks>
Public Class lpcMark
    Inherits measureProcedureType1Base
    Implements IDisposable

    Protected ReadOnly Property Setting As lpcMeasureSetting
        Get
            Return CType(__measureSetting, lpcMeasureSetting)
        End Get
    End Property

    Protected featureProcedureState As Integer = 0
    ''' <summary>
    ''' 1. move X
    ''' 2. align center by S
    ''' 3. measure features , output pairs
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function measureProcedure(ByRef state As Integer) As Boolean
        With Setting

            Select Case state
                Case 0
                    .currentFeatureProcedure =
                        .FeatureMeasureSettings.GetEnumerator
                    state = 10
                Case 10
                    If .currentFeatureProcedure.MoveNext Then
                        state = 100
                    Else
                        '-----------------------
                        '   All feature executed
                        '-----------------------
                        Return True
                    End If
                Case 100
                    With .currentFeatureProcedure.Current.Value
                        If .procedure(featureProcedureState) Then
                            featureProcedureState = 0 'rewind
                            dataPairCollection.AddRange(.__outputList)
                            state = 10
                        Else
                            '-----------------------
                            '   Procedure is running
                            '-----------------------    
                        End If
                    End With
            End Select
        End With

        Return False

    End Function

    Protected Sub New()
        'all calculation used to mark the lpcreal
        MyBase.New(compenstationMethodEnums.AS_PASSIVE_OBJECT,
                   New lpcMeasureSetting,
                   frames.Instance.Elementray(framesDefinition.LPC_REAL, framesDefinition.LPC))

    End Sub


    Protected Overrides Function process() As Integer
        If IsProcedureAbort.viewFlag(Automation.interlockedFlag.POSITION_OCCUPIED) Then
            featureProcedureState = 0 'reset
        End If
        Return MyBase.process()
    End Function

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As lpcMark
        Get
            If __instance Is Nothing Then
                __instance = New lpcMark
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As lpcMark = Nothing
#End Region

End Class
