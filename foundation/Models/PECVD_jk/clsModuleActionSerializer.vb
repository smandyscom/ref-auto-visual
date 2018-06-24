Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' module action 有順序性，firstSourcePositionInfo必須完成，secondSourcePositionInfo會複製到targetPositionInfo，
''' 等待targetPositionInfo被完成後，才把targetPositionInfo複製回secondSourcePositionInfo。
''' 使用在ASA-04-013 gintech maia 上料手與clamp之間
''' </summary>
''' <remarks></remarks>
Public Class clsModuleActionSerializer
    Inherits systemControlPrototype

#Region "External Data declare"
    Public Property firstSourcePositionInfo As Func(Of shiftDataPackBase)   'face to conveyor(clamp postion)
    Public Property secondSourcePositionInfo As Func(Of shiftDataPackBase)               'face to conveyor(loadingPicker position)
    Public Property targetPositionInfo As shiftDataPackBase 'use after allocating , Hsien , 2015.05.28 'New shiftDataCollection               'face to loadingPicker
#End Region

    Enum ExcuteEnum
        _0
        _10
        _20
    End Enum 'only for stateIgnite

    Protected Function stateExecute() As Integer
        Excute(systemSubState)
        Return 0
    End Function
    Function Excute(ByRef cStep As ExcuteEnum) As Integer
        Select Case cStep
            Case ExcuteEnum._0
                If firstSourcePositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False AndAlso
                   secondSourcePositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    'clamp finish job

                    targetPositionInfo.Assign(secondSourcePositionInfo.Invoke)
                    cStep = ExcuteEnum._10
                End If
            Case ExcuteEnum._10
                If targetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then 'wait first module action done
                    secondSourcePositionInfo.Invoke.Assign(targetPositionInfo)
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
