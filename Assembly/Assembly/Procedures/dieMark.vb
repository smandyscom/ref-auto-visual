﻿Imports Automation
Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra
Imports Automation.Components.Services
Imports System.ComponentModel
Imports System.Drawing.Design

Public Class dieMarkSetting
    Inherits measureProcedureSetting

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
    Property ImageProcessSetting As imageProcessSettingBlock = New imageProcessSettingBlock


End Class

''' <summary>
''' 1. Move to die center (DIE_REAL)
''' 2. Do camera image process
''' 3. get P1/P2/P3/P4 x,y coordinate
''' 4. move LREAL to P1/P2/P3/P4
''' 5. read z 
''' 6. data process to mark out the real coordinate system it is (using fit method)
''' </summary>
''' <remarks></remarks>
Public Class dieMark
    Inherits measureProcedureType1Base

    Protected ReadOnly Property Setting As dieMarkSetting
        Get
            Return CType(__measureSetting, dieMarkSetting)
        End Get
    End Property

    Friend measuredPadPositionsInR As List(Of PositionVector) = New List(Of PositionVector)
    Friend currentPad As List(Of PositionVector).Enumerator = Nothing

    Friend delayTimer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 100)}

    ' ''' <summary>
    ' ''' Link to some external link
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Protected correspondingErrorMatrix As eulerHtmTR = Nothing

    Enum subStatesEnum As Integer
        IMAGE_PROCESSING = 100
        PAD_HEIGHT_MEASURE = 200
        DATA_PROCESSING = 500
    End Enum

#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As dieMark
        Get
            If __instance Is Nothing Then
                __instance = New dieMark
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As dieMark = Nothing
#End Region

    Shared pointSequence As List(Of itemsDefinition) = New List(Of itemsDefinition) From {itemsDefinition.DIE_P1,
                                                                                          itemsDefinition.DIE_P2,
                                                                                              itemsDefinition.DIE_P3,
                                                                                              itemsDefinition.DIE_P4,
                                                                                              itemsDefinition.DIE_P5,
                                                                                              itemsDefinition.DIE_P6,
                                                                                              itemsDefinition.DIE_P7}
    Protected Sub New()
        MyBase.New(compenstationMethodEnums.AS_PASSIVE_OBJECT,
                   New dieMarkSetting,
                   frames.Instance.Elementray(framesDefinition.DIE_REAL_DRY, framesDefinition.DIE))
    End Sub


    Protected Overrides Function measureProcedure(ByRef state As Integer) As Boolean
        Select Case state
            Case 0
                'reset DIE_REAL_DRY
                measuredPadPositionsInR.Clear()

                With frames.Instance
                    .CurrentMovingItem = framesDefinition.C4
                    .CurrentRItem = itemsDefinition.DIE_CENTER
                End With
                state = 10
            Case 10
                If Assembly.Instance.IsAllAxesSettled Then
                    Setting.ImageProcessSetting.onCameraTriggered()
                    state = subStatesEnum.IMAGE_PROCESSING
                Else
                    '------------------------
                    '   Settling
                    '------------------------
                End If
            Case subStatesEnum.IMAGE_PROCESSING
                With Setting.ImageProcessSetting
                    If .IsImageProcessDone Then
                        If .Result = Cognex.VisionPro.CogToolResultConstants.Accept Then
                            'collect coordinates
                            Setting.ImageProcessSetting.Coordinates.ForEach(Sub(__point As Vector(Of Double))
                                                                                If __point IsNot Nothing Then
                                                                                    measuredPadPositionsInR.Add(New PositionVector(__point, framesDefinition.C4))
                                                                                End If
                                                                            End Sub)

                            'transform to reference position
                            For index = 0 To measuredPadPositionsInR.Count - 1
                                measuredPadPositionsInR(index) = frames.Instance.Transformation(framesDefinition.C4, framesDefinition.R) * measuredPadPositionsInR(index)
                            Next

                            currentPad = measuredPadPositionsInR.GetEnumerator
                            state = subStatesEnum.PAD_HEIGHT_MEASURE
                        Else
                            ''----------------------------
                            ''TODO image processing failed
                            ''----------------------------
                            'Result = IProcedure.procedureResultEnums.FAILED
                            'Return True
                        End If
                    Else
                        '-------------------------
                        '   Image processing
                        '-------------------------
                    End If
                End With

            Case subStatesEnum.PAD_HEIGHT_MEASURE
                If currentPad.MoveNext Then

                    frames.Instance.solveAbsAxAy(currentPad.Current, framesDefinition.LREAL)
                    state += 10
                Else
                    '-------------------------------
                    '   All measured
                    ''------------------------------
                    state = subStatesEnum.DATA_PROCESSING
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 10
                If Assembly.Instance.IsAllAxesSettled Then
                    state += 10
                Else
                    '------------------------
                    '   Settling
                    '------------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 20
                If Assembly.Instance.__distanceMeter.drive(distanceMeter.dmCommands.RT, {distanceMeter.dmOnOff.__ON}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    delayTimer.IsEnabled = True
                    state += 10
                Else
                    '-------------------------------
                    '   Communicating
                    ''------------------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 30
                If delayTimer.IsTimerTicked Then
                    state += 10
                Else
                    '--------------------
                    '   Counting
                    '--------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 40
                If Assembly.Instance.__distanceMeter.drive(distanceMeter.dmCommands.RT, {distanceMeter.dmOnOff.__OFF}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    state += 10
                Else
                    '-------------------------------
                    '   Communicating
                    ''------------------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 50
                If Assembly.Instance.__distanceMeter.drive(distanceMeter.dmCommands.TM, {distanceMeter.dmOnOff.__ON}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    delayTimer.IsEnabled = True
                    state += 10
                Else
                    '-------------------------------
                    '   Communicating
                    ''------------------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 60
                If delayTimer.IsTimerTicked Then
                    state += 10
                Else
                    '--------------------
                    '   Counting
                    '--------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 70
                If Assembly.Instance.__distanceMeter.drive(distanceMeter.dmCommands.TM, {distanceMeter.dmOnOff.__OFF}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    state += 10
                Else
                    '-------------------------------
                    '   Communicating
                    ''------------------------------
                End If
            Case subStatesEnum.PAD_HEIGHT_MEASURE + 80
                If Assembly.Instance.__distanceMeter.drive(distanceMeter.dmCommands.MS, {dmTaskEnum.TASK_PLANE_HEIGHT}) =
                     IDrivable.endStatus.EXECUTION_END And
                    Assembly.Instance.__distanceMeter.IsMeasureValueAvailable Then

                    Dim measuredPosition As PositionVector = New PositionVector(framesDefinition.LREAL) With {.Z = Assembly.Instance.__distanceMeter.MeasureValue}

                    'complete the Z-value
                    currentPad.Current.Z =
                        (frames.Instance.Transformation(framesDefinition.LREAL, framesDefinition.R) * measuredPosition).Z

                    'do next point measurement
                    state = subStatesEnum.PAD_HEIGHT_MEASURE
                Else
                    '----------------------
                    '
                    '----------------------
                End If
            Case subStatesEnum.DATA_PROCESSING
                '-------------------------------
                '   Data Process
                '-------------------------------

                'transform to DIE_REAL_DRY position
                For index = 0 To measuredPadPositionsInR.Count - 1
                    measuredPadPositionsInR(index) = frames.Instance.Transformation(framesDefinition.R, framesDefinition.DIE_REAL_DRY) * measuredPadPositionsInR(index)
                Next

                For index = 0 To itemsDefinition.DIE_P4
                    Dim idealValue As Vector(Of Double) = frames.Instance.objectsDictionary(pointSequence(index)).RawValue
                    MyBase.dataPairCollection.Add(New measuredDataPair(idealValue,
                                                                 measuredPadPositionsInR(index).RawValue,
                                                                 AutoNumeric.utilities.selectionEnums.X Or
                                                                 AutoNumeric.utilities.selectionEnums.Y Or
                                                                 AutoNumeric.utilities.selectionEnums.Z))

                Next
                'P5/P6 takes X/Y only
                For index = itemsDefinition.DIE_P5 To itemsDefinition.DIE_P6
                    Dim idealValue As Vector(Of Double) = frames.Instance.objectsDictionary(pointSequence(index)).RawValue
                    MyBase.dataPairCollection.Add(New measuredDataPair(idealValue,
                                                                 measuredPadPositionsInR(index).RawValue,
                                                                 AutoNumeric.utilities.selectionEnums.X Or
                                                                 AutoNumeric.utilities.selectionEnums.Y))
                Next

                Return True
        End Select


        Return False
    End Function


End Class
