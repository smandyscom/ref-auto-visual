﻿Imports Automation
Imports System.ComponentModel


Public Interface IProcedure

    Enum procedureResultEnums As Integer
        SUCCESS
        ''' <summary>
        ''' Not successed but not afftected next procedure
        ''' </summary>
        ''' <remarks></remarks>
        FAILED
        ''' <summary>
        ''' Cannot go further procedure
        ''' </summary>
        ''' <remarks></remarks>
        BREAK
    End Enum

    Property IsProcedureStarted As flagController(Of interlockedFlag)
    ''' <summary>
    ''' Abort process once raised
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property IsProcedureAbort As flagController(Of interlockedFlag)
    ''' <summary>
    ''' Set before trigger
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Arguments As Object

    Property Result As procedureResultEnums
End Interface

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class procedureContext

    ReadOnly Property ProcedureName As String
        Get
            Return __procedureName
        End Get
    End Property

    Property IsEngaged As Boolean = False

    ReadOnly Property Result As IProcedure.procedureResultEnums
        Get
            Return procedureReference.Result
        End Get
    End Property

    Dim presettedArguments As Object
    Dim procedureReference As IProcedure = Nothing
    Dim __procedureName As String = ""
    ''' <summary>
    ''' True:Running
    ''' False : Finished/Not Started
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Property Run As Boolean
        Get
            Return procedureReference.IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED)
        End Get
        Set(value As Boolean)
            If presettedArguments IsNot Nothing Then
                procedureReference.Arguments = presettedArguments
            End If
            procedureReference.IsProcedureStarted.writeFlag(interlockedFlag.POSITION_OCCUPIED,
                                                            IsEngaged)
        End Set
    End Property

    Sub New(__presettedArguments As Object,
            procedureReference As IProcedure,
        Optional __procedureName As String = "")
        Me.presettedArguments = __presettedArguments
        Me.procedureReference = procedureReference
        Me.__procedureName = __procedureName
    End Sub

End Class


Public Class procedureExecutor
    Inherits systemControlPrototype
    Implements IOperational

    ReadOnly Property ProcedureCollection As List(Of procedureContext)
        Get
            Return __procedureCollection
        End Get
    End Property

    Public Property OperationSignals As New flagController(Of operationSignalsEnum) Implements IOperational.OperationSignals

    Dim __procedureCollection As List(Of procedureContext) = New List(Of procedureContext)
    Dim __currentProcedure As List(Of procedureContext).Enumerator

    Function stateIdle() As Integer
        If OperationSignals.readFlag(operationSignalsEnum.__START) Then
            systemMainState = systemStatesEnum.EXECUTE
        End If
        Return 0
    End Function

    Function stateExecute() As Integer

        Select Case systemSubState
            Case 0
                __currentProcedure = __procedureCollection.GetEnumerator
            Case 10
                If __currentProcedure.MoveNext Then
                    __currentProcedure.Current.Run = True
                    systemSubState = 20
                Else
                    '----------------
                    '   All procedures ran out
                    '----------------
                    systemSubState = 500
                End If
            Case 20
                With __currentProcedure.Current
                    If Not .Run And
                        .Result <> IProcedure.procedureResultEnums.BREAK Then
                        'success/failed

                        systemSubState = 10

                    ElseIf Not .Run And
                        .Result = IProcedure.procedureResultEnums.BREAK Then
                        'series procedure breaks
                        systemMainState = systemStatesEnum.IDLE
                    Else
                        '-----------------
                        ' Procedure Executing
                        '-----------------
                    End If

                End With

            Case 500
                systemMainState = systemStatesEnum.IDLE
        End Select

        Return 0

    End Function

    Sub New()
        Me.systemMainStateFunctions(systemStatesEnum.IDLE) = AddressOf stateIdle
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IDLE
    End Sub

End Class