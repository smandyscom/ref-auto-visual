﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
''' <summary>
''' ASA-04-051 50MW 阿特斯
''' 上料輸送帶舉升
''' </summary>
''' <remarks></remarks>
Public Class clsConveyorLifter
    Inherits systemControlPrototype
    Implements IFinishableStation

#Region "Device declare"
    ''' <summary>
    ''' 上下氣缸
    ''' a side: down, b side: up
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property cyUp As cylinderGeneric = New cylinderGeneric
    ''' <summary>
    ''' 橫移氣缸
    ''' a side: open, b side: close
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property cyShift As cylinderGeneric = New cylinderGeneric
#End Region
#Region "External Data declare"
    Property SourcePositionModuleAction As Func(Of shiftDataPackBase) '檢查module action的位置，位於輸送帶的最尾端
    Property sourcePositionCollection As New List(Of Func(Of shiftDataPackBase)) '被舉升的位置的集合
    Property targetPositionCollection As New shiftDataCollection 'face to arm bridge
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
#End Region

#Region "Data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}
    Protected systemSubState2 As Integer = 0 '給第2個substate用
#End Region

    Enum stateIgniteEnum
        _0
        _10
        _20
        _30
        _40
        _100

    End Enum
    Enum ExcuteEnum
        _0
        _10
        _20
        _30
        _40
        _41
        _42
        _50
        _60
        _70
        _100
        _110
        _120
        _130
        _140
        _150
        _200
        _210
        _220
        _300
        _400
        _500
        _600
        _610
        _620
        _630
        _640
        _650
        _660

        _25

        _2

        _15

        _17

        _12

    End Enum

    Protected Function stateIgnite() As Integer
        Ignite(systemSubState)
        Return 0
    End Function
    Private Function Ignite(ByRef cStep As stateIgniteEnum) As Integer
        Select Case cStep
            Case stateIgniteEnum._0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    cStep = stateIgniteEnum._10
                End If
            Case stateIgniteEnum._10
                If cyShift.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    cStep = stateIgniteEnum._20
                End If
            Case stateIgniteEnum._20
                If cyUp.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    cStep = stateIgniteEnum._30
                End If
            Case stateIgniteEnum._30 'cyshift close
                If cyShift.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    cStep = stateIgniteEnum._100
                End If
            Case stateIgniteEnum._100
                FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                systemMainState = systemStatesEnum.EXECUTE
                'systemSubState = 0 '不需要，會自動歸0
                systemSubState2 = 0
        End Select
        Return 0
    End Function
    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Excute2(systemSubState2)
        Return 0
    End Function
    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0 '查看module action 
                If SourcePositionModuleAction.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    If SourcePositionModuleAction.Invoke.IsPositionOccupied = True Then '最後一個位置有片
                        cStep = ExcuteEnum._10
                    Else
                        SourcePositionModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                    End If
                    '查看上游站是否已收料結束
                ElseIf UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True)
                    cStep = ExcuteEnum._2
                End If
            Case ExcuteEnum._2
                If UpstreamStations.Exists(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False) '清除自己收料結束

                    cStep = ExcuteEnum._0
                End If

            Case ExcuteEnum._10 '
                If cyUp.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then
                    cStep = ExcuteEnum._20
                End If

            Case ExcuteEnum._20 'data moving
                SourcePositionModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                For i As Short = 0 To targetPositionCollection.DataCount - 1 'copy wafer infomation to data queue
                    targetPositionCollection.DataCollection(i).Assign(sourcePositionCollection(i).Invoke)
                    sourcePositionCollection(i).Invoke.IsPositionOccupied = False
                    'sourcePositionCollection(i).Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                Next
                targetPositionCollection.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True) '通知抓手取片
                cStep = ExcuteEnum._30
            Case ExcuteEnum._30 'wait wafers are picked
                If targetPositionCollection.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    For i As Short = 0 To targetPositionCollection.DataCount - 1 'copy wafer infomation to data queue
                        targetPositionCollection.DataCollection(i).IsPositionOccupied = False 'clear each wafer
                    Next
                    cStep = ExcuteEnum._40
                End If
            Case ExcuteEnum._40
                If cyShift.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then 'open
                    cStep = ExcuteEnum._50
                End If
            Case ExcuteEnum._50
                If cyUp.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                    cStep = ExcuteEnum._60
                End If
            Case ExcuteEnum._60
                If cyShift.drive(cylinderControlBase.cylinderCommands.GO_B_END) = IDrivable.endStatus.EXECUTION_END Then 'open
                    cStep = ExcuteEnum._0
                End If
        End Select
        Return 0
    End Function
    Function Excute2(ByRef cStep As Integer) As Integer
        Select Case cStep
            Case 0 'check conveyors's  module action, auto clear module action
                If SourcePositionModuleAction.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True AndAlso
                    SourcePositionModuleAction.Invoke.IsPositionOccupied = False Then
                    SourcePositionModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                End If
        End Select
        Return 0
    End Function

    Function initMappingAndSetup() As Integer

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function
    Private Sub cyDrive(cy As cylinderGeneric, ByVal command As cylinderGeneric.cylinderCommands)
        If cy Is Nothing Then Exit Sub
        cy.drive(command)
    End Sub

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub
End Class
