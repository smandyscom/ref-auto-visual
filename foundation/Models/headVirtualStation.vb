﻿Imports Automation

''' <summary>
'''     'used to control autoLoading/unloading flow
''' </summary>
''' <remarks></remarks>
Public Class headVirtualStation
    Inherits systemControlPrototype
    Implements IFinishableStation

    Public operationSignals As flagController(Of operationSignalsEnum) = New flagController(Of operationSignalsEnum)

    Public Property FinishableFlags As flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags

    Public Property LastStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    '------------------------
    '   Hsien , 2015.06.17 , complished
    '------------------------
    Public Event AllStationRised(ByVal sender As Object, ByVal e As EventArgs)
    Public Event AllStationFalled(ByVal sender As Object, ByVal e As EventArgs)

    Function stateIdle() As Integer

        operationSignals.resetFlag(operationSignalsEnum.__STOP)
        If (operationSignals.readFlag(operationSignalsEnum.__START)) Then
            systemMainState = systemStatesEnum.EXECUTE
        End If

        Return 0
    End Function
    Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                If (operationSignals.viewFlag(operationSignalsEnum.__STOP)) Then
                    Me.FinishableFlags.setFlag(IFinishableStation.controlFlags.STATION_FINISHED)
                    systemSubState = 10
                End If
            Case 10
                '---------------------
                '   Wait last station finished
                '---------------------
                If (LastStations.Count > 0 AndAlso
                   LastStations.TrueForAll(Function(__station As IFinishableStation) (__station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)))) Then
                    RaiseEvent AllStationRised(Me, Nothing)  '   Hsien , 2015.06.17 , used to close cassette station
                    Me.FinishableFlags.resetFlag(IFinishableStation.controlFlags.STATION_FINISHED)
                    systemSubState = 100
                End If
            Case 100
                If (LastStations.Count > 0 AndAlso
                   LastStations.TrueForAll(Function(__station As IFinishableStation) (Not __station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)))) Then
                    RaiseEvent AllStationFalled(Me, Nothing) 'Hsien , 2015.06.17
                    systemMainState = systemStatesEnum.IDLE
                End If
            Case Else

        End Select

        Return 0
    End Function


    Sub New()
        Me.FinishableFlags = New flagController(Of IFinishableStation.controlFlags)
        Me.LastStations = New List(Of IFinishableStation)
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub

    Function initMappingAndSetup() As Integer

        systemMainStateFunctions(systemStatesEnum.IDLE) = AddressOf stateIdle
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IDLE

        Return 0
    End Function


End Class
