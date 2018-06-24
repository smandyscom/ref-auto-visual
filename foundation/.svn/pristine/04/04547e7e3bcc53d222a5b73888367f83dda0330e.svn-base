Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 相依於clsConveyorLaneMask
''' </summary>
''' <remarks></remarks>
Public Class clsLoadingPickerFilter
    Inherits systemControlPrototype

#Region "External Data declare"
    Public Property SourcePositionInfo As List(Of Func(Of shiftDataPackBase)) = New List(Of Func(Of shiftDataPackBase))   'face to conveyor side
    Public Property SourcePositionInfoModuleAction As Func(Of shiftDataPackBase)
    Public Property TargetPositionInfo As New shiftDataCollection               'face to loading arm side
    Public Property ConveyorLaneMaskReference As clsConveyorLaneMask            'ready link to conveyorLaneMask
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
            Case ExcuteEnum._0
                If SourcePositionInfoModuleAction.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    For i As Short = 0 To SourcePositionInfo.Count - 1
                        TargetPositionInfo.DataCollection(i).Assign(SourcePositionInfo(i).Invoke)
                    Next
                    If ConveyorLaneMaskReference.LaneIndex = 0 Then
                        '--------------------------------------------------------------------------------
                        '   Pick condition : Conveyor Lane Mask had count a round , and any wafer existed
                        '--------------------------------------------------------------------------------
                        If SourcePositionInfo.Exists(Function(obj As Func(Of shiftDataPackBase)) (obj.Invoke.IsPositionOccupied = True)) = True Then
                            'ready to be pick
                            'For i As Short = 0 To SourcePositionInfo.Count - 1
                            '    TargetPositionInfo.DataCollection(i).Assign(SourcePositionInfo(i).Invoke)
                            'Next
                            TargetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)  'inform arm able to pick
                            cStep = ExcuteEnum._100
                        Else
                            SourcePositionInfoModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) '全空
                        End If

                    Else
                        SourcePositionInfoModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) '尚未移動到整排lane , release the conveyor
                    End If
                Else
                    '-----------------------
                    '   Wait Conveyor to Trigger
                    '-----------------------
                End If
            Case ExcuteEnum._100
                If TargetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    SourcePositionInfoModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                    ' arm picked , release conveyor
                    For i As Short = 0 To SourcePositionInfo.Count - 1
                        'SourcePositionInfo(i).Invoke.IsPositionOccupied = False
                        SourcePositionInfo(i).Invoke.Assign(TargetPositionInfo.DataCollection(i)) '2016.3.12 jk prevent loading picker did not picked.
                        SourcePositionInfo(i).Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
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
