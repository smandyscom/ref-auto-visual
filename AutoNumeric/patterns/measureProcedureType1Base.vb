﻿Imports Automation
Imports MathNet.Numerics.LinearAlgebra
Imports System.Xml.Serialization
Imports System.IO

Public Enum compenstationMethodEnums As Integer
    ''' <summary>
    ''' For measurements frame calibration
    ''' </summary>
    ''' <remarks></remarks>
    AS_OBSERVER = 0
    ''' <summary>
    ''' For objects frame alignment
    ''' </summary>
    ''' <remarks></remarks>
    AS_PASSIVE_OBJECT = 1
End Enum


Public Class measureProcedureSetting
    Inherits settingBase

    Property TolerancePosition As Double = 0.001
    Property ToleranceRotation As Double = 0.0001

    Property TryingGoal As Integer = 3


    Public Shared settingPath As String = My.Application.Info.DirectoryPath & "\Data\measure\"
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

End Class

''' <summary>
''' Offer the basic data procedure flow
''' Output: error matrix updating
''' </summary>
''' <remarks></remarks>
Public MustInherit Class measureProcedureType1Base
    Inherits systemControlPrototype
    Implements IProcedure
    Implements IDisposable

    ReadOnly Property CorrespondingFrame As [Enum]
        Get
            Return correspondingErrorMatrix.From
        End Get
    End Property

    Public Overridable Property Arguments As Object Implements IProcedure.Arguments
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property IsProcedureAbort As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort
    Public Overridable Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result

    Property IsOutputProcedue As Boolean = False
    ReadOnly Property MeasureSetting As measureProcedureSetting
        Get
            Return __measureSetting
        End Get
    End Property
    Protected __measureSetting As measureProcedureSetting = New measureProcedureSetting


    ReadOnly Property ProcedureState As Integer
        Get
            Return __procedureState
        End Get
    End Property
    ReadOnly Property LastNorm As Double
        Get
            Return __lastNorm
        End Get
    End Property
    ''' <summary>
    ''' Used to store data pair
    ''' Derived class should output this
    ''' </summary>
    ''' <remarks></remarks>
    Protected dataPairCollection As List(Of measuredDataPair) = New List(Of measuredDataPair)

    ''' <summary>
    ''' Link to some external link
    ''' </summary>
    ''' <remarks></remarks>
    Protected correspondingErrorMatrix As eulerHtmTR = Nothing

    Protected tryingTimes As Integer = 0

    Protected __procedureState As Integer = 0
    Protected lastProcedureState As Integer = 0
    Protected MustOverride Function measureProcedure(ByRef state As Integer) As Boolean
    Protected Overridable Function preparationProcedure(ByRef state As Integer) As Boolean
        Return True
    End Function
    Protected Overridable Function abortProcedure(ByRef state As Integer) As Boolean
        Return True
    End Function

    Protected errorMatrix As eulerHtmTR = New eulerHtmTR(Nothing, Nothing)
    '---------------------------------
    'able to redirect by derived class
    '---------------------------------
    Protected Overridable Function dataHandlingMethod(__dataCollection As List(Of measuredDataPair)) As htmEdgeElementary
        Return measuredDataPair.fitTransformation2(__dataCollection)
    End Function

    Protected compenstationMethod As compenstationMethodEnums = compenstationMethodEnums.AS_OBSERVER

    Dim alarmPackErrorNotConverage As alarmContextBase = New alarmContextBase With {.Sender = Me}

    Dim lastSystemSubState As Integer = 0
    Dim __dataLogger As dataLogger = Nothing

    Dim __count As Integer = 0
    Dim __average As Double = 0
    Dim __deviation As Double = 0
    Dim __lastNorm As Double = Double.MaxValue

    Protected ReadOnly Property IsMeasureAccepted As Boolean
        Get
            Return errorMatrix.ControlVector.L2Norm < __measureSetting.ToleranceRotation
        End Get
    End Property

    Function stateMeasure() As Integer

        If IsOutputProcedue Then
            'record procedure state
            If lastProcedureState <> __procedureState Then
                sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Procedure:{0}", __procedureState))
                lastProcedureState = __procedureState
            End If
            If lastSystemSubState <> systemSubState Then
                sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Sub-state:{0}", systemSubState))
                lastSystemSubState = systemSubState
            End If
        Else
            '-------------------------------
            '   No need to output
            '-------------------------------
        End If

        If IsProcedureAbort.readFlag(interlockedFlag.POSITION_OCCUPIED) Then
            __lastNorm = Double.MaxValue
            IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            systemSubState = 0 'reset
            __procedureState = 0

        End If

        'through the measure procedure , get the ideal position and real position(measured one)
        'turns into error gain/error form (error  = ideal-real)
        'interating above procedure , until enough data count got
        'processing datas, error vector/error matrix calculated
        'update error matrix
        'check if norm of error vector is smaller than low threshold
        ' if yes , quit procedure
        ' if no  , iterating

        Select Case systemSubState
            Case 0
                If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    tryingTimes = 0 'reset

                    If __dataLogger IsNot Nothing Then
                        __dataLogger.writeLine("Closed", True)
                    End If
                    __dataLogger = New dataLogger(Me.DeviceName) ' initialize new log

                    sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Initial Control Vector,{0}",
                                                       correspondingErrorMatrix.ControlVector.ToVectorString.Replace(vbCrLf, vbTab)))

                    systemSubState += 10
                Else
                    '---------------
                    '   Wait Trigger
                    '---------------
                End If
            Case 10
                If preparationProcedure(__procedureState) Then
                    __procedureState = 0 'reset
                    systemSubState = 100
                Else
                    '------------
                    '   Preparating
                    '------------
                End If
            Case 100
                'measure procedures
                If measureProcedure(__procedureState) Then
                    __procedureState = 0 'reset

                    'procedure would output dataPairCollection
                    'T_real_real1
                    __average = measuredDataPair.averageErrorLength(dataPairCollection)
                    __deviation = measuredDataPair.deviationErrorLength(dataPairCollection)
                    __count = dataPairCollection.Count
                    '----------------------------------------
                    '   Record
                    '----------------------------------------
                    sendMessage(internalEnum.GENERIC_MESSAGE,
                                  String.Format("Average,{1}{0}Deviation,{2}{0}Count,{3}{0}Data Pairs,{4}{0}",
                                                vbCrLf,
                                                __average,
                                                __deviation,
                                                __count,
                                                measuredDataPair.pairsOutput(dataPairCollection)))
                    '-------------------------
                    'not converged , but trying times not over the given value
                    'keep trying
                    '-------------------------
                    errorMatrix.RawValue = dataHandlingMethod(dataPairCollection).RawValue

                    If Not IsMeasureAccepted And
                        tryingTimes < __measureSetting.TryingGoal Then

                        'cascade current error matrix
                        sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Before,{0}", correspondingErrorMatrix.ControlVector.ToVectorString.Replace(vbCrLf, vbTab)))

                        Select Case compenstationMethod
                            Case compenstationMethodEnums.AS_OBSERVER
                                'inversion
                                errorMatrix.RawValue = errorMatrix.RawValue.Inverse
                            Case compenstationMethodEnums.AS_PASSIVE_OBJECT
                                'no need transform
                        End Select

                        'compensation
                        correspondingErrorMatrix.RawValue = correspondingErrorMatrix.RawValue * errorMatrix.RawValue

                        sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("After,{0}", correspondingErrorMatrix.ControlVector.ToVectorString.Replace(vbCrLf, vbTab)))
                        sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Error Vector,{0}", errorMatrix.ControlVector.ToVectorString.Replace(vbCrLf, vbTab)))
                        sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Error Vector Norm,{0}", errorMatrix.ControlVector.L2Norm))

                        tryingTimes += 1
                    Else
                        'converged/trying times reached , procedure done
                        systemSubState = 500
                    End If

                    __lastNorm = errorMatrix.ControlVector.L2Norm 'update
                    dataPairCollection.Clear()

                Else
                    '--------------------------
                    '   Measuring
                    '--------------------------
                End If
            Case 500
                If abortProcedure(__procedureState) Then
                    '------------------
                    '   Procedure Done
                    '------------------
                    If IsMeasureAccepted Then
                        '----------------
                        '   Well done
                        '----------------
                        Result = IProcedure.procedureResultEnums.SUCCESS
                    Else
                        'not coveraged , report error
                        Result = IProcedure.procedureResultEnums.BREAK
                    End If

                    IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED) ' reset flag
                    systemSubState = 0
                Else
                    '--------------------
                    '   Procedure running
                    '--------------------
                End If

        End Select

        Return 0

    End Function

    Sub New(__compenstationMethod As compenstationMethodEnums,
             derivedSettingBlock As measureProcedureSetting,
             __correspondingErrorMatrix As eulerHtmTR)

        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateMeasure
        Me.systemMainState = systemStatesEnum.EXECUTE

        Me.compenstationMethod = __compenstationMethod
        Me.correspondingErrorMatrix = __correspondingErrorMatrix

        Me.__measureSetting = derivedSettingBlock
        Me.__measureSetting.Load(Nothing)

        Me.IsEnabled = True
    End Sub

    ''' <summary>
    ''' Redirect Message to Data Logger
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub dataLoggerRedirector(sender As Object, e As messagePackageEventArg) Handles CentralMessenger.MessagePoped
        If e.Message.Sender.Equals(Me) AndAlso
            __dataLogger IsNot Nothing Then
            __dataLogger.writeLine(e.Message.ToString)
        End If
    End Sub


#Region "IDisposable Support"
    Protected disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If
            __measureSetting.Save()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
