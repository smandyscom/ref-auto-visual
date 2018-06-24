Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' ASA-04-191上料 及 下料 base on ASA-04-051 50MW 阿特斯 上料區, but not suitable for older version
''' </summary>
''' <remarks></remarks>
Public Class clsConveyorCombineFilter
    Inherits systemControlPrototype
    Implements IFinishableStation
#Region "External Data declare"
    Public Property sourcePositionCollection As New List(Of shiftDataCollection)    'face to conveyor lifter side
    Public Property targetPositionInfo As New shiftDataCollection               'face to loading arm side
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property Flags As New flagController(Of flagEnum)
#End Region
#Region "internal data declare"
    ''' <summary>
    ''' remember which sourcePosition's module action is ON
    ''' </summary>
    ''' <remarks></remarks>
    Dim sourcePositionModuleActionCollection As New List(Of shiftDataCollection)
#End Region
    Enum flagEnum
        ALLOW_HALF_EMPTY
    End Enum
    Enum ExcuteEnum
        _0
        _100
    End Enum 'only for stateIgnite
    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Return 0
    End Function
    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0 '檢查 conveyor lifter module action
                '是否全部都module action
                Dim blnCanPick As Boolean = True
                Dim blnAllFinished As Boolean = True
                If Flags.viewFlag(flagEnum.ALLOW_HALF_EMPTY) = False Then
                    If sourcePositionCollection.Exists(Function(obj As shiftDataCollection)
                                                           Return Not obj.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)
                                                       End Function) = True Then
                        blnCanPick = False
                    End If
                Else 'allow half empty
                    If sourcePositionCollection.Exists(Function(obj As shiftDataCollection)
                                                           Return obj.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)
                                                       End Function) = True AndAlso
                       sourcePositionCollection.TrueForAll(Function(obj As shiftDataCollection)
                                                               If obj.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                                                                   Return True
                                                               ElseIf obj.IsAnyRemained = False Then
                                                                   Return True
                                                               End If
                                                               Return False
                                                           End Function) = True Then
                        blnCanPick = True
                    Else
                        blnCanPick = False
                    End If
                End If

                For i As Short = 0 To UpstreamStations.Count - 1
                    If UpstreamStations(i).FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                        blnAllFinished = False
                    End If
                Next
                If UpstreamStations.Count = 0 Then blnAllFinished = False
                If blnAllFinished = True Then blnCanPick = False

                If blnCanPick = True Then
                    'targetPositionInfo.DataCollection.Clear()
                    sourcePositionModuleActionCollection.Clear() 'clear this list
                    Dim targetIndex As Integer = 0
                    With sourcePositionCollection
                        For sourceIndex As Integer = 0 To .Count - 1 'for each sourcePosition
                            If .Item(sourceIndex).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                                sourcePositionModuleActionCollection.Add(.Item(sourceIndex)) 'add this sourcePosition to module action list
                                For dataIndex As Integer = 0 To .Item(sourceIndex).DataCount - 1
                                    targetPositionInfo.DataCollection(targetIndex).Assign(sourcePositionCollection(sourceIndex).DataCollection(dataIndex))
                                    targetIndex += 1
                                Next
                            Else 'skip this sourcePosition and clear these positions of targetPositionInfo
                                For dataIndex As Integer = 0 To .Item(sourceIndex).DataCount - 1
                                    targetPositionInfo.DataCollection(targetIndex).IsPositionOccupied = False
                                    targetIndex += 1
                                Next
                            End If
                        Next
                    End With
                    targetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True) 'trigger arm to pick or place
                    cStep = ExcuteEnum._100
                End If

            Case ExcuteEnum._100 '等待抓手取放完
                If targetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    ' arm picked/placed , release conveyor
                    Dim targetIndex As Integer = 0
                    With sourcePositionCollection
                        For sourceIndex As Integer = 0 To .Count - 1 'for each sourcePosition
                            If sourcePositionModuleActionCollection.Contains(.Item(sourceIndex)) = True Then 'find is the sourcePosition in the list of module action
                                .Item(sourceIndex).ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)  'reset module action
                                For dataIndex As Integer = 0 To .Item(sourceIndex).DataCount - 1
                                    sourcePositionCollection(sourceIndex).DataCollection(dataIndex).Assign(targetPositionInfo.DataCollection(targetIndex))
                                    targetIndex += 1
                                Next
                            Else  'skip this sourcePosition
                                targetIndex += .Item(sourceIndex).DataCount
                            End If
                        Next
                    End With
                    cStep = ExcuteEnum._0
                End If
        End Select
        Return 0
    End Function

    Function initMappingAndSetup()

        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function

    Public Sub New()

        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE

        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub


End Class
