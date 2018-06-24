Imports Automation
Imports AutoNumeric
Imports AutoNumeric.utilities
Imports System.Xml.Serialization
Imports MathNet.Numerics.LinearAlgebra


Public Class dispCalibrationCameraSetting
    Inherits imageProcessSettingBlock

    Friend measureFrame As framesDefinition = framesDefinition.C1REAL
    Friend readyPoint As itemsDefinition = itemsDefinition.C1_DISP_READY
    Friend processPoint As itemsDefinition = itemsDefinition.C1_ORIGIN
    ''' <summary>
    ''' Which dimension cannot be detected
    ''' </summary>
    ''' <remarks></remarks>
    Friend zeroDimensionInDISP As axisEntityEnum = axisEntityEnum.X
    <XmlIgnore()>
    WriteOnly Property FeaturePoint As itemsDefinition
        Set(value As itemsDefinition)
            Select Case value
                Case itemsDefinition.C1_ORIGIN
                    measureFrame = framesDefinition.C1REAL
                    readyPoint = itemsDefinition.C1_DISP_READY
                    zeroDimensionInDISP = axisEntityEnum.X
                Case itemsDefinition.C2_ORIGIN
                    measureFrame = framesDefinition.C2REAL
                    readyPoint = itemsDefinition.C2_DISP_READY
                    zeroDimensionInDISP = axisEntityEnum.Y
            End Select
            processPoint = value
        End Set
    End Property

End Class

''' <summary>
''' Including two camera
''' </summary>
''' <remarks></remarks>
Public Class dispCalibrationSetting
    Inherits measureProcedureSetting

    Property C1Setting As dispCalibrationCameraSetting = New dispCalibrationCameraSetting With {.FeaturePoint = itemsDefinition.C1_ORIGIN}
    Property C2Setting As dispCalibrationCameraSetting = New dispCalibrationCameraSetting With {.FeaturePoint = itemsDefinition.C2_ORIGIN}

    <XmlIgnore()>
    ReadOnly Property FeatureMeasureSettings As Dictionary(Of itemsDefinition, dispCalibrationCameraSetting)
        Get
            If __featureMeasureSettings Is Nothing Then
                __featureMeasureSettings = New Dictionary(Of itemsDefinition, dispCalibrationCameraSetting)
                __featureMeasureSettings(itemsDefinition.C1_ORIGIN) = C1Setting
                __featureMeasureSettings(itemsDefinition.C2_ORIGIN) = C2Setting
            End If

            Return __featureMeasureSettings
        End Get
    End Property
    Protected __featureMeasureSettings As Dictionary(Of itemsDefinition, dispCalibrationCameraSetting) = Nothing
    Friend currentFeatureProcedure As Dictionary(Of itemsDefinition, dispCalibrationCameraSetting).Enumerator = Nothing

    Public Overrides Sub Load(filename As String)
        MyBase.Load(filename)
        C1Setting.FeaturePoint = itemsDefinition.C1_ORIGIN
        C2Setting.FeaturePoint = itemsDefinition.C2_ORIGIN
    End Sub
End Class

Public Class dispCalibration
    Inherits measureProcedureType1Base

    Protected ReadOnly Property Setting As dispCalibrationSetting
        Get
            Return CType(__measureSetting, dispCalibrationSetting)
        End Get
    End Property

    Protected Sub New()
        MyBase.New(compenstationMethodEnums.AS_PASSIVE_OBJECT,
                   New dispCalibrationSetting,
                   frames.Instance.Elementray(framesDefinition.DISP_HEAD_REAL, framesDefinition.DISP_HEAD))
    End Sub

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As dispCalibration
        Get
            If __instance Is Nothing Then
                __instance = New dispCalibration
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As dispCalibration = Nothing
#End Region

    ''' <summary>
    ''' 0: move to c1 origin , take measuring
    ''' 1: move to c2 origin , take measuring
    ''' 2: cal
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function measureProcedure(ByRef state As Integer) As Boolean

        Select Case state
            Case 0
                Setting.currentFeatureProcedure = Setting.FeatureMeasureSettings.GetEnumerator
                state += 10
            Case 10
                If Setting.currentFeatureProcedure.MoveNext Then
                    With frames.Instance
                        .CurrentMovingItem = framesDefinition.DISP_HEAD_REAL
                        .CurrentRItem = Setting.currentFeatureProcedure.Current.Value.readyPoint
                    End With
                    state += 10
                Else
                    '---------------
                    '   All procedure proceded
                    '---------------
                    Return True
                End If
            Case 20
                If Assembly.Instance.IsAllAxesSettled Then
                    With frames.Instance
                        .CurrentMovingItem = framesDefinition.DISP_HEAD_REAL
                        .CurrentRItem = Setting.currentFeatureProcedure.Current.Value.processPoint
                    End With
                    state += 10
                Else
                    '-------------------------------
                    '   Settling
                    '-------------------------------
                End If
            Case 30
                If Assembly.Instance.IsAllAxesSettled Then
                    Setting.currentFeatureProcedure.Current.Value.onCameraTriggered()
                    state += 10
                Else
                    '-------------------
                    '   Settling
                    '-------------------
                End If
            Case 40
                With Setting.currentFeatureProcedure.Current.Value
                    If .IsImageProcessDone Then

                        If .Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                            'output to data pair collection
                            'origin of DISP comparasion
                            Dim measuredPositionInDisp As PositionVector =
                                frames.Instance.Transformation(.measureFrame, framesDefinition.DISP_HEAD_REAL) * New PositionVector(.Coordinates.First, .measureFrame)
                            measuredPositionInDisp.RawValue(.zeroDimensionInDISP) = 0

                            MyBase.dataPairCollection.Add(New measuredDataPair(New PositionVector(Nothing).RawValue,
                                                                               measuredPositionInDisp.RawValue))

                            'back to zero point
                            c4htm.Instance.AxisValue(axisEntityEnum.Z) = 0
                            state += 10
                        Else
                            '----------------------
                            '   Not Acceptable
                            '----------------------
                        End If
                    Else
                        '------------------------
                        '   Not Done Yet
                        '------------------------
                    End If
                End With
            Case 50
                If Assembly.Instance.IsAllAxesSettled Then
                    state = 10 ' rewind
                Else
                    '-------------------------------
                    '   Settling
                    '-------------------------------
                End If

        End Select

        Return False
    End Function
    ''' <summary>
    ''' Average the errors
    ''' </summary>
    ''' <param name="__dataCollection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function dataHandlingMethod(__dataCollection As List(Of measuredDataPair)) As htmEdgeElementary
        Dim output As htmEdgeElementary = New htmEdgeElementary(Nothing,
                                                                Nothing)
        Dim __compensatedOrigin As PositionVector = New PositionVector(Nothing)
        With __compensatedOrigin
            .X = -1 * __dataCollection.Last.OriginalErrorPosition(axisEntityEnum.X) 'c2 taks x/z
            .Y = -1 * __dataCollection.First.OriginalErrorPosition(axisEntityEnum.Y) 'c1 takes y/z
            .Z = -1 * measuredDataPair.averageErrorVector(__dataCollection)(axisEntityEnum.Z)
        End With
        output.Origin = __compensatedOrigin
        Return output
    End Function
End Class
