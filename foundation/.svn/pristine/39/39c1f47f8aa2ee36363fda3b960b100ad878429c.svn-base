Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 適用於ASA-04-051 50MW 阿特斯 上料區
''' ASA-04-191上料 及 下料
''' </summary>
''' <remarks></remarks>
Public Class clsConveyorLifterFilter
    Inherits systemControlPrototype
    Implements IFinishableStation
#Region "External Data declare"
    Public Property sourcePositionCollection As New List(Of shiftDataCollection)    'face to conveyor lifter side
    Public Property targetPositionInfo As New shiftDataCollection               'face to loading arm side
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
#End Region
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
                For i As Short = 0 To sourcePositionCollection.Count - 1
                    If sourcePositionCollection(i).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False AndAlso
                        UpstreamStations(i).FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                        blnCanPick = False : Exit For
                    End If
                Next
                For i As Short = 0 To UpstreamStations.Count - 1
                    If UpstreamStations(i).FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                        blnAllFinished = False
                    End If
                Next
                If blnAllFinished = True Then blnCanPick = False

                If blnCanPick = True Then
                    targetPositionInfo.DataCollection.Clear()
                    For i As Short = 0 To sourcePositionCollection.Count - 1
                        For j As Short = 0 To sourcePositionCollection(i).DataCollection.Count - 1
                            targetPositionInfo.DataCollection.Add(sourcePositionCollection(i).DataCollection(j).Clone)
                        Next
                    Next
                    targetPositionInfo.DataCollection(0).ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)
                    cStep = ExcuteEnum._100
                End If

            Case ExcuteEnum._100 '等待抓手取完
                If targetPositionInfo.DataCollection(0).ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    ' arm picked , release conveyor
                    For i As Short = 0 To sourcePositionCollection.Count - 1
                        For j As Short = 0 To sourcePositionCollection(i).DataCollection.Count - 1
                            sourcePositionCollection(i).DataCollection(j).IsPositionOccupied =
                            targetPositionInfo.DataCollection(j).IsPositionOccupied
                        Next
                        sourcePositionCollection(i).ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                    Next
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
