﻿Imports Automation


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class inSerieDispatcher
    Inherits systemControlPrototype
    Implements IModuleSingle

    ''' <summary>
    ''' Link to the very beginning
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetPositionInfo As Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo

    ''' <summary>
    ''' Register the module into work table
    ''' </summary>
    ''' <remarks></remarks>
    Public Function createModuleHandle() As shiftDataPackBase
        Dim data As shiftDataPackBase = TargetPositionInfo.Invoke.Clone 'use as seed

        workTable.Add(data)
        '__moduleSocket = Function() (data)

        Return data
    End Function


    Friend workTable As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase)
    Friend __iterator As List(Of shiftDataPackBase).Enumerator

    Function stateExecute() As Integer


        Select Case systemSubState
            Case 0
                With TargetPositionInfo.Invoke

                    If (.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then

                        __iterator = workTable.GetEnumerator()
                        __iterator.MoveNext()
                        With __iterator.Current
                            .Assign(TargetPositionInfo.Invoke)
                            .ModuleAction.setFlag(interlockedFlag.POSITION_OCCUPIED)
                        End With
                        systemSubState = 100
                    Else
                        '-------
                        'wait MA
                        '-------
                    End If

                End With

                '------------------------------------
                '   Loop until all module serie done
                '------------------------------------
            Case 100
                If (Not __iterator.Current.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then

                    Dim lastData As shiftDataPackBase = __iterator.Current

                    If (__iterator.MoveNext) Then
                        __iterator.Current.Assign(lastData) 'data transmission
                        __iterator.Current.ModuleAction.setFlag(interlockedFlag.POSITION_OCCUPIED)  'invoke to work
                    Else
                        'loop end
                        systemSubState = 110
                    End If
                Else
                    'current module on working
                End If
            Case 110
                With TargetPositionInfo.Invoke
                    .Assign(workTable.Last)
                    .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)  'report work done
                    systemSubState = 0
                End With

            Case Else

        End Select

        Return 0
    End Function

    Sub New()
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE
    End Sub

    Protected Overrides Function process() As Integer

        '----------------------------
        'wont be interfered by pause/alarm
        '----------------------------

        drivesRunningInvoke()

        stateControl()
        processProgress()

        Return 0

    End Function

End Class

Public Class testShiftData
    Inherits shiftDataPackBase

    Public derivedField As String = ""

    Public Overrides Sub Assign(source As Object)
        derivedField = source.derivedField
        MyBase.Assign(source)
    End Sub

End Class
Public Class inSerieDispatcherTest

    Dim testData As testShiftData = New testShiftData
    Dim __inSerieDispatcher As inSerieDispatcher = New inSerieDispatcher With {.CentralAlarmObject = New alarmManager,
                                                                               .PauseBlock = New interceptObject,
                                                                               .CentralMessenger = New messageHandler,
                                                                               .IsEnabled = True}
    Dim testSequenceState As Integer = 0

    Public Sub TestRoutine()

        __inSerieDispatcher.TargetPositionInfo = Function() testData

        Dim __module1Handle As Func(Of shiftDataPackBase) = Nothing
        Dim __module2Handle As Func(Of shiftDataPackBase) = Nothing

        '__inSerieDispatcher.createModuleHandle(__module1Handle)
        Debug.Assert(__inSerieDispatcher IsNot Nothing)

        '__inSerieDispatcher.createModuleHandle(__module2Handle)
        Debug.Assert(Not __module1Handle.Equals(__module2Handle))
        Debug.Assert(Not __module1Handle.Invoke.Equals(__module2Handle.Invoke))

        While True
            __inSerieDispatcher.running()

            Select Case testSequenceState
                Case 0
                    testData.derivedField = (New Random(DateTime.Now.Millisecond)).Next.ToString
                    testData.ModuleAction.setFlag(interlockedFlag.POSITION_OCCUPIED)
                    testSequenceState = 10
                Case 10
                    If (__module1Handle.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then

                        Debug.Assert(testData.derivedField = CType(__module1Handle.Invoke, testShiftData).derivedField)   'data should be passed
                        Debug.Assert(testData.derivedField <> CType(__module2Handle.Invoke, testShiftData).derivedField)   'data should not be passed yet

                        Debug.Assert(Not __module2Handle.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED))

                        __module1Handle.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)

                        testSequenceState = 20
                    End If
                Case 20
                    If (__module2Handle.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then

                        Debug.Assert(testData.derivedField = CType(__module2Handle.Invoke, testShiftData).derivedField)   'data should be passed
                        Debug.Assert(Not __module1Handle.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED))

                        CType(__module2Handle.Invoke, testShiftData).derivedField = (New Random(DateTime.Now.Millisecond)).Next.ToString
                        __module2Handle.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)

                        Debug.Assert(testData.derivedField <> CType(__module2Handle.Invoke, testShiftData).derivedField)

                        testSequenceState = 30
                    End If
                Case 30
                    If (Not testData.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then

                        Debug.Assert(testData.derivedField = CType(__module2Handle.Invoke, testShiftData).derivedField)   'data should be passed
                        Debug.Assert(CType(__module1Handle.Invoke, testShiftData).derivedField <> CType(__module2Handle.Invoke, testShiftData).derivedField)   'data should be passed

                        testSequenceState = 500
                    End If

                Case 500
                    Console.WriteLine("")
                Case Else

            End Select



        End While


    End Sub


End Class
