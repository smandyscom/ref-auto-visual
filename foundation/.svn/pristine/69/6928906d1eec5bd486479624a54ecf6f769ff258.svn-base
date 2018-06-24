Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 不相容 ASA-04-002 IOX 下料 因為多了一個SourceModuleAction
''' ASA-04-191 IOX 下料
''' </summary>
''' <remarks></remarks>
Public Class clsUnloadingPickerFilterV2
    Inherits systemControlPrototype

#Region "External Data declare"
    Public Property SourceModuleAction As Func(Of shiftDataPackBase) 'focus the conveyor module action
    Public Property SourcePositionInfo As New List(Of Func(Of shiftDataPackBase))   'face to conveyor
    Public Property TargetPositionInfo As New shiftDataCollection       'face to arm
    Public Property conveyor As shiftingModel

    ''' <summary>
    ''' ex.輸送帶總長度為5 放片手只放在位置0與1, 將0,1移動完後 到2,3不會繼續移動. 為了解決此問題須再看放片手是否有片在手上,若否,則繼續移動
    ''' </summary>
    ''' <remarks></remarks>
    Public isUnloadingPickerBusy As Func(Of Boolean) = New Func(Of Boolean)(Function() (True)) 'default value is busy, always waiting unloading picker put done.
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
                If SourceModuleAction.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then

                    If SourcePositionInfo.TrueForAll(Function(obj As Func(Of shiftDataPackBase)) (obj.Invoke.IsPositionOccupied = False)) = True AndAlso
                        isUnloadingPickerBusy() = True Then
                        '----------------------------------
                        '   Landing Area had cleared
                        '----------------------------------

                        'ready to be place
                        For i As Short = 0 To SourcePositionInfo.Count - 1
                            TargetPositionInfo.DataCollection(i).Assign(SourcePositionInfo(i).Invoke)
                        Next
                        TargetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)

                        cStep = ExcuteEnum._100
                    Else
                        '----------------------------------
                        '   Any one landing area is occupied
                        '----------------------------------
                        SourceModuleAction.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) '不全空
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
