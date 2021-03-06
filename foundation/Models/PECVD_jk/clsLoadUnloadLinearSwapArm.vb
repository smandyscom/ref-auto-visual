﻿Imports Automation.Components.CommandStateMachine
Imports Automation.mainIOHardware
Imports Automation.Components.Services
Imports Automation

Public Interface IWeightData
    Property WeightBeforeProcess As Single
    Property WeightAfterProcess As Single
End Interface
Public Class clsLoadingLinearSwapArm
    Inherits clsLinearSwapArm
    Property FlowNum As Integer '共有幾個水道
    Property IsAction As Boolean = True '是否啟用抓手
    <MonitoringDescription("每隔幾片要就秤重一次(0為除能)")> Property MeasurementPerNum As UInteger '每隔幾片要就秤重一次
    Property _motor As motorControl = MyBase.motor 'for testing and monitor
    Dim waferCount As Integer
    Dim cStep As enumIsAbleSwapStep
    Dim NextSwapCount As Integer '記錄第幾片要交換
    Dim flowCount As Integer '記錄目前換到第幾個lane
    ReadOnly Property [Step] As enumIsAbleSwapStep
        Get
            Return cStep
        End Get
    End Property
    Public Overrides Function initMappingAndSetup() As Integer
        IsAbleSwap = AddressOf _isAbleSwap
        SwapFinishProcess = AddressOf _swapFinishProcess
        Return MyBase.initMappingAndSetup()
    End Function
    Private Function _swapFinishProcess() As Boolean
        If TargetPositionInfoB.Invoke.IsPositionOccupied = True Then
            NextSwapCount += FlowNum + 1
            flowCount += 1
            If flowCount >= FlowNum Then
                flowCount = 0
                NextSwapCount = 0
                waferCount = 0 'reset
            End If
        Else
            NextSwapCount += FlowNum
        End If
        Return True
    End Function
    Private Function _isAbleSwap() As Boolean
        Select Case cStep
            Case enumIsAbleSwapStep._0
                If TargetPositionInfoA.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    'do nothing, waiting module action

                ElseIf ForceSwapCondition.Invoke = True Then
                    cStep = enumIsAbleSwapStep._10

                ElseIf Not IsAction Then 'bypass mode
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action

                ElseIf TargetPositionInfoA.Invoke.IsPositionOccupied = False Then
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                Else
                    waferCount += 1
                    If (
                            waferCount >= MeasurementPerNum AndAlso
                            MeasurementPerNum > 0 AndAlso
                            (waferCount - MeasurementPerNum) >= NextSwapCount
                            ) Then
                        'forceMeasure.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset later....

                        cStep = enumIsAbleSwapStep._10
                    Else
                        TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                    End If
                End If
            Case enumIsAbleSwapStep._10
                If TargetPositionInfoB.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    cStep = enumIsAbleSwapStep._0
                    Return True
                End If

        End Select
        Return False
    End Function
    Enum enumIsAbleSwapStep
        _0
        _10

        _20

    End Enum
End Class
Public Class clsUnloadingLinearSwapArm
    Inherits clsLinearSwapArm
    Property _motor As motorControl = MyBase.motor
    Property IsAction As Boolean = True '是否啟用抓手
    Dim cStep As enumIsAbleSwapStep
    ReadOnly Property [Step] As enumIsAbleSwapStep
        Get
            Return cStep
        End Get
    End Property
    Public Overrides Function initMappingAndSetup() As Integer
        IsAbleSwap = AddressOf _isAbleSwap
        SwapFinishProcess = AddressOf _swapFinishProcess
        Return MyBase.initMappingAndSetup()
    End Function
    Private Function _swapFinishProcess() As Boolean
        Return True
    End Function
    Private Function _isAbleSwap() As Boolean
        Select Case cStep
            Case enumIsAbleSwapStep._0
                If TargetPositionInfoA.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    'do nothing, waiting module action
                ElseIf ForceSwapCondition.Invoke = True Then
                    cStep = enumIsAbleSwapStep._10
                ElseIf Not IsAction Then 'bypass mode
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                ElseIf TargetPositionInfoA.Invoke.IsPositionOccupied = False Then
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                ElseIf CType(TargetPositionInfoA.Invoke, IWeightData).WeightBeforeProcess > 0 Then 'there is weight data in this wafer
                    cStep = enumIsAbleSwapStep._10
                ElseIf TargetPositionInfoA.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    TargetPositionInfoA.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                End If
            Case enumIsAbleSwapStep._10
                If TargetPositionInfoB.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    cStep = 0
                    Return True
                End If
        End Select
        Return False
    End Function
    Enum enumIsAbleSwapStep
        _0
        _10
    End Enum
End Class