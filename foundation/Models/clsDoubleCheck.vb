Imports Automation
Imports Automation.Components.Services

Public Class clsDoubleCheck
    Inherits systemControlPrototype
    Implements IModuleSingle

    Property IsBypassed As Boolean = False  ';Hsien , 2016.03.23
    Public doubleSheetSensor As sensorControl = New sensorControl With {.IsEnabled = False}

    Public Property TargetPositionInfo As Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo

    Dim alarmPackWaferHadDoubled As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                                      .Inputs = doubleSheetSensor.InputBit,
                                                                                      .PossibleResponse = alarmContextBase.responseWays.RETRY,
                                                                                      .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                                                                      .AdditionalInfo = "疊片檢出，請移除多餘片後重試（Double wafers had detected）"}

    Function stateExecute() As Integer

        Select Case systemSubState
            Case 0
                With TargetPositionInfo.Invoke
                    If (.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) AndAlso
                       .IsPositionOccupied) Then

                        If (IsBypassed) Then
                            'do not action
                            .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                        Else
                            'normally work
                            systemSubState = 10
                        End If

                    ElseIf (.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)) Then
                        .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)  'release conveyor if no wafer
                    Else
                        '-----------------
                        '   Wait
                        '-----------------
                    End If

                End With
            Case 10
                If Not doubleSheetSensor.IsSensorCovered Then
                    CentralAlarmObject.raisingAlarm(alarmPackWaferHadDoubled)
                Else
                    '-----------------
                    '   Check Passed
                    '-----------------
                    TargetPositionInfo.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                    systemSubState = 0
                End If

            Case Else

        End Select


        Return 0
    End Function

    Sub New()
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE
    End Sub

End Class
