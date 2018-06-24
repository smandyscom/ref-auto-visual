Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

''' <summary>
''' 適用於ASA-04-051 阿特斯 勾片
''' 相依於卡匣勾片機構
''' </summary>
''' <remarks></remarks>
Public Class clsHookDecider
    Inherits systemControlPrototype
    Implements IFinishableStation

#Region "External Data declare"
    Public Property SourcePositionInfo As Func(Of shiftDataPackBase)
    Public Property TargetPositionInfo As New shiftDataPackBase               'face to loading arm side
    Public Property SpWaferExist As New sensorControl
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

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
                If UpstreamStations.Exists(Function(upStation As IFinishableStation) (upStation.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False) '清除自己收料結束
                End If
                If SourcePositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    TargetPositionInfo.Assign(SourcePositionInfo.Invoke)
                    TargetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, True)  'inform arm able to pick
                    cStep = ExcuteEnum._100
                End If
            Case ExcuteEnum._100 '等待勾片機構完成
                If UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) = True Then
                    TargetPositionInfo.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False) 'reset module action
                    SourcePositionInfo.Invoke.Assign(TargetPositionInfo)
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True)
                    cStep = ExcuteEnum._0
                ElseIf TargetPositionInfo.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = False Then
                    If TargetPositionInfo.IsPositionOccupied = True Then
                        'If SpWaferExist.IsSensorCovered = True Then
                        SourcePositionInfo.Invoke.Assign(TargetPositionInfo)
                        cStep = ExcuteEnum._0
                        'Else
                        '    Dim ap As New alarmContentSensor
                        '    With ap
                        '        .Sender = Me
                        '        .Inputs = SpWaferExist.InputBit
                        '        .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.IGNORE
                        '        .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                        '        .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() As Boolean
                        '                                                                     Return True
                        '                                                                 End Function
                        '        .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function() As Boolean
                        '                                                                      TargetPositionInfo.IsPositionOccupied = False
                        '                                                                      Return True
                        '                                                                  End Function
                        '        CentralAlarmObject.raisingAlarm(ap)
                        '    End With
                        'End If
                    Else
                        cStep = ExcuteEnum._0
                    End If

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
