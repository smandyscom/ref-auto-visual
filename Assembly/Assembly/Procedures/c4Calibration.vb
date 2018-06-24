Imports AutoNumeric
Imports System.ComponentModel
Imports Automation
Imports System.Drawing.Design
Imports System.Xml.Serialization

Public Class c4CalibrationSetting
    Inherits cameraCalibrationSettingRectangle

    ''' <summary>
    ''' Addtional image processing block
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
    Property C3ImageProcessSetting As imageProcessSettingBlock = New imageProcessSettingBlock

    Friend __ballPosition As PositionVector = New PositionVector(framesDefinition.R)

    ''' <summary>
    ''' Do ball position offset
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public Overrides ReadOnly Property MeasurePoints As List(Of PositionVector)
        Get
            Dim __measurePointsInR As List(Of PositionVector) = New List(Of PositionVector)

            'change its frames from Y0 to R
            For Each item As PositionVector In MyBase.MeasurePoints
                __measurePointsInR.Add(New PositionVector(item.RawValue, framesDefinition.R))
            Next

            __measurePointsInR.ForEach(Sub(__point As PositionVector)
                                           With __point
                                               .X += __ballPosition.X
                                               .Y += __ballPosition.Y
                                               .Z += __ballPosition.Z
                                           End With
                                       End Sub)

            Return __measurePointsInR
        End Get
    End Property

    Sub New()
        MyBase.New(framesDefinition.Y0REAL,
                   framesDefinition.Y0)
    End Sub

End Class
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class c4Calibration
    Inherits cameraCalibrationBase

    ''' <summary>
    ''' As local origin
    ''' </summary>
    ''' <remarks></remarks>
    Private Property BallPosition As PositionVector
        Get
            Return Setting.__ballPosition
        End Get
        Set(value As PositionVector)
            Setting.__ballPosition = value
        End Set
    End Property
    Protected Shadows ReadOnly Property Setting As c4CalibrationSetting
        Get
            Return CType(__measureSetting, c4CalibrationSetting)
        End Get
    End Property


    Protected Friend Overrides Sub moveAction()
        'move current position under views of C4
        frames.Instance.solveAbsAxAy(idealPosition.Current, framesDefinition.C4)
    End Sub

    ''' <summary>
    ''' In order to sort out Y0REAL , 
    ''' turns ideal points on Y0
    ''' turns real point on Y0REAL
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend Overrides Function postHandlingAction(nominalPosition As PositionVector, measuredPosition As PositionVector) As measuredDataPair
        With frames.Instance
            'the calculated value from known chain
            Dim ideal As PositionVector = .Transformation(framesDefinition.R, framesDefinition.Y0REAL) * BallPosition
            'the measured value from the unknown chain
            Dim real As PositionVector = .Transformation(framesDefinition.C4, framesDefinition.Y0REAL) * New PositionVector(measuredPosition.RawValue, framesDefinition.C4)
            Return MyBase.postHandlingAction(ideal, real)
        End With
    End Function

    Protected Overrides Function preparationProcedure(ByRef state As Integer) As Boolean
        '-------------------------
        '   Take Real Ball Position On FOV of C3
        '-------------------------
        Select Case state
            Case 0
                With frames.Instance
                    .CurrentMovingItem = framesDefinition.C4
                    .CurrentRItem = itemsDefinition.C3_ORIGIN
                End With
                state = 10
            Case 10
                If Assembly.Instance.IsAllAxesSettled Then
                    CType(__measureSetting, c4CalibrationSetting).C3ImageProcessSetting.onCameraTriggered()
                    state = 100
                Else
                    '------------------
                    '   Axes Moving
                    '------------------
                End If
            Case 100
                '--------------
                '   Finished when camera worked 
                '--------------
                With CType(__measureSetting, c4CalibrationSetting).C3ImageProcessSetting
                    If .IsImageProcessDone Then

                        If .Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                            '------------------------------------
                            '   Calculating out the real position
                            '------------------------------------
                            BallPosition = frames.Instance.Transformation(framesDefinition.C3REAL, framesDefinition.R) *
                                New PositionVector(CType(__measureSetting, c4CalibrationSetting).C3ImageProcessSetting.Coordinates.First, framesDefinition.C3REAL)
                            Return True
                        Else
                            '---------------------
                            '   Failed , TODO
                            '---------------------
                        End If

                    End If
                End With

        End Select

        Return False
    End Function

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As c4Calibration
        Get
            If __instance Is Nothing Then
                __instance = New c4Calibration
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As c4Calibration = Nothing
#End Region

    Protected Sub New()
        '------------------------
        '   Loading Setting Files
        '------------------------
        MyBase.New(framesDefinition.Y0, framesDefinition.Y0REAL)
        MyBase.__measureSetting = New c4CalibrationSetting
        __measureSetting.Load(Nothing)
        BallPosition = frames.Instance.objectsDictionary(itemsDefinition.C3_ORIGIN)
    End Sub

End Class
