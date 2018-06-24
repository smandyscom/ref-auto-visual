Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 相容昱晶IOX 下料 因為多了一個SourceModuleAction
''' 相依於clsConveyorLaneMask
''' </summary>
''' <remarks></remarks>
Public Class clsUnloadingPickerFilter
    Inherits systemControlPrototype

#Region "External Data declare"
    Public Property SourcePositionInfo As New List(Of Func(Of shiftDataPackBase))   'face to conveyor
    Public Property TargetPositionInfo As New shiftDataCollection       'face to arm
    Public Property conveyor As shiftingModel
#End Region

    Enum ExcuteEnum
        _0
        _100
    End Enum 'only for stateIgnite
    Protected Function stateIgnite() As Integer
        systemMainState = systemStatesEnum.EXECUTE
        Return 0
    End Function
    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Return 0
    End Function
    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0
                If SourcePositionInfo(0).Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then

                    If SourcePositionInfo.TrueForAll(Function(obj As Func(Of shiftDataPackBase)) (obj.Invoke.IsPositionOccupied = False)) = True Then
                        '----------------------------------
                        '   Landing Area had cleared
                        '----------------------------------

                        'ready to be place
                        'For i As Short = 0 To SourcePositionInfo.Count - 1
                        '    TargetPositionInfo.DataCollection(i).Assign(SourcePositionInfo(i).Invoke)
                        'Next
                        TargetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)

                        cStep = ExcuteEnum._100
                    Else
                        '----------------------------------
                        '   Any one landing area is occupied
                        '----------------------------------
                        SourcePositionInfo(0).Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) '不全空
                    End If
                Else
                    '----------------------
                    '   Wait Conveyor Response
                    '-----------------------
                End If
            Case ExcuteEnum._100
                If TargetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then

                    For i As Short = 0 To SourcePositionInfo.Count - 1
                        SourcePositionInfo(i).Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                        SourcePositionInfo(i).Invoke.Assign(TargetPositionInfo.DataCollection(i))
                        'SourcePositionInfo(i).Invoke.IsPositionOccupied = True  'Hsien , 2015.05.05 ,assume all wafer had placed
                    Next

                    cStep = ExcuteEnum._0
                Else
                    '-----------------------------
                    '   Wait arm finish its JOB
                    '-----------------------------
                End If
        End Select
        Return 0
    End Function

    Function initMappingAndSetup()
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE
        Return 0
    End Function

    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub


End Class
