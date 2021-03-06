﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.mainIOHardware

''' <summary>
''' The common interface for those wafer which are recognizable for NG Picker
''' </summary>
''' <remarks></remarks>
Public Interface INgClassifiable

    '1. wafer data
    '2. whether ng
    '3. pick point
    'ReadOnly Property 

    ''' <summary>
    ''' Returned the target point according to inspection result 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property PlacePoint(token As Object) As cMotorPoint
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property PickPoint(token As Object) As cMotorPoint
    ''' <summary>
    ''' Is classfied to pick
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsClassified(token As Object) As Boolean
End Interface


''' <summary>
''' Cooperating with broken check
''' </summary>
''' <remarks></remarks>
Public Class ngPicker
    Inherits systemControlPrototype
    Implements IFinishableStation
    Implements IModuleMulti

    Public Event BrokenWaferRemoved(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' Would monitor multi wafers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetPositionInfos As List(Of Func(Of shiftDataPackBase)) Implements IModuleMulti.TargetPositionInfo

    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations

    'Hsien , open parameter to setting , 2015.07.22
    Property VacummGeneratingTime As TimeSpan = New TimeSpan(0, 0, 1)
    Property VacummBreakTime As TimeSpan = New TimeSpan(0, 0, 1)
    ''' <summary>
    ''' The expected wafer transmission time
    ''' After locked , how long the conveyor would stop
    ''' After conveyor stopped , conveyor would blocked by lock signal
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property SynchronizationTime As TimeSpan = New TimeSpan(0, 0, 0, 0, 700)

    Protected __token As Object = ""

#Region "control members"
    Public motorNgPicker As motorControl = New motorControl
    Public cyDown As cylinderGeneric = New cylinderGeneric() '上下氣缸
    Public doVacuum As ULong = 0
    Public doVBreak As ULong = 0

    ''' <summary>
    ''' Nothing : ngPicker is idle, otherwise , ngPicker is working
    ''' </summary>
    ''' <remarks></remarks>
    Dim currentData As Func(Of shiftDataPackBase) = Nothing '記住是哪個broken reporter發生ng
    Dim timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}
    Dim holdWaferData As shiftDataPackBase = New shiftDataPackBase
    ''' <summary>
    ''' There'r conveyors need synchronization
    ''' </summary>
    ''' <remarks></remarks>
    Dim lockActions As List(Of Action(Of Boolean)) = New List(Of Action(Of Boolean))
#End Region

    Protected Function stateIgnite() As Integer
        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState = 5
                End If
            Case 5
                If cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 10
                End If
            Case 10
                If motorNgPicker.drive(motorControl.motorCommandEnum.GO_HOME) = motorControl.statusEnum.EXECUTION_END Then
                    systemSubState = 500
                End If
            Case 500
                If motorNgPicker.drive(motorControl.motorCommandEnum.GO_POSITION,
                                      CType(TargetPositionInfos.First.Invoke, INgClassifiable).PlacePoint(Me.__token)) =
                    motorControl.statusEnum.EXECUTION_END Then

                    FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                    systemMainState = systemStatesEnum.EXECUTE
                End If
        End Select
        Return 0
    End Function

    Protected Function stateExecute() As Integer

        '--------------------------------------------------------
        '   For those Module Action Satisifid following conditions , release conveyor
        '1. occupied but not NG
        '2. not occupied
        '--------------------------------------------------------
        If currentData Is Nothing Or
            lockActions.Count = 0 Then
            '---------------------------------
            ' Keep loosing conveyor:
            '1. Not sychroniztion-need system
            '2. Ng Picker is idle
            '---------------------------------
            TargetPositionInfos.ForEach(Sub(data As Func(Of shiftDataPackBase))
                                            With data.Invoke
                                                If .ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) And
                                                    ((.IsPositionOccupied And (Not CType(data.Invoke, INgClassifiable).IsClassified(Me.__token)) Or (Not .IsPositionOccupied))) Then
                                                    'releasing
                                                    .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                                                End If
                                            End With
                                        End Sub)
        Else
            '------------------------------------------------------------
            '   Ng picker is working And it's a sychroniztion-need system
            '------------------------------------------------------------
        End If

        Select Case systemSubState
            Case 0
                '檢查進料處錄出料處任務

                '----------------------------
                'search which one had been ng
                '----------------------------
                currentData = TargetPositionInfos.Find(Function(data As Func(Of shiftDataPackBase))
                                                           With data.Invoke
                                                               Return .ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) And
                                                                   .IsPositionOccupied And
                                                                  (CType(data.Invoke, INgClassifiable).IsClassified(Me.__token))
                                                           End With
                                                       End Function)

                If (currentData IsNot Nothing) Then
                    'lock all further movements of conveyor
                    lockActions.ForEach(Sub(__action As Action(Of Boolean)) __action.Invoke(True))
                    With timer
                        .TimerGoal = SynchronizationTime
                        .IsEnabled = True
                    End With

                    systemSubState = 10
                Else
                    '------------------------
                    '   candinates not found
                    '------------------------
                End If
            Case 10
                If timer.IsTimerTicked Then
                    systemSubState = 20
                Else
                    '-----------------
                    '   Synchronizing with conveyors
                    '-----------------
                End If
            Case 20 '移至取片位置
                If motorNgPicker.drive(motorControl.motorCommandEnum.GO_POSITION, CType(currentData.Invoke, INgClassifiable).PickPoint(Me.__token)) =
                    motorControl.statusEnum.EXECUTION_END Then

                    systemSubState = 30
                End If

            Case 30 '氣缸下
                If cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then
                    '開真空
                    writeBit(doVacuum, True)
                    timer.TimerGoal = VacummGeneratingTime    'Hsien , 2015.08.06
                    timer.IsEnabled = True
                    systemSubState = 40

                End If

            Case 40 '真空確認或timeout當作吸到
                If timer.IsTimerTicked Then
                    systemSubState = 50
                Else
                    '------------------
                    '   Counting
                    '------------------
                End If
            Case 50 '氣缸上
                If cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then

                    systemSubState = 60
                End If
            Case 60
                If motorNgPicker.drive(motorControl.motorCommandEnum.GO_POSITION, CType(currentData.Invoke, INgClassifiable).PlacePoint(Me.__token)) =
                    motorControl.statusEnum.EXECUTION_END Then

                    '----------------------------------
                    'Release conveyor , and reset datas
                    '----------------------------------
                    With currentData.Invoke
                        .IsPositionOccupied = False
                        .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                        holdWaferData = .Clone 'copy
                    End With

                    'release all conveyors
                    lockActions.ForEach(Sub(__action As Action(Of Boolean)) __action.Invoke(False))
                    currentData = Nothing 'used as releasing signal

                    systemSubState = 100
                End If


                '--------------------------------
                '   Place movement
                '--------------------------------
            Case 100
                If cyDown.drive(cylinderControlBase.cylinderCommands.GO_B_END) =
                     IDrivable.endStatus.EXECUTION_END Then

                    writeBit(doVacuum, False) '關真空
                    writeBit(doVBreak, True)

                    timer.TimerGoal = VacummBreakTime
                    timer.IsEnabled = True    'reset , Hsien , 2015.08.06

                    systemSubState = 110

                End If
            Case 110
                If timer.IsTimerTicked = True Then
                    writeBit(doVBreak, False)
                    systemSubState = 500
                End If
                '--------------------------------
                '   End Place movement
                '--------------------------------

            Case 500
                If cyDown.drive(cylinderControlBase.cylinderCommands.GO_A_END) =
                     IDrivable.endStatus.EXECUTION_END Then

                    systemSubState = 0
                End If


        End Select
        Return 0
    End Function

    Sub alarmOccursHandler(sender As Object, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured, PauseBlock.InterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) Then
            motorNgPicker.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        End If
    End Sub
    Sub alarmReleaseHandler(sender As Object, e As alarmEventArgs) Handles CentralAlarmObject.alarmReleased, PauseBlock.UninterceptedEvent
        If (MainState = systemStatesEnum.IGNITE) Then
            motorNgPicker.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        End If
    End Sub

    Function initMappingAndSetup() As Integer
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE

        Return 0
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="lockActions"> For those system need conveyor synchronization</param>
    ''' <remarks></remarks>
    Public Sub New(Optional token As Object = Nothing,
                   Optional lockActions As List(Of Action(Of Boolean)) = Nothing)

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf Me.initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf Me.initEnableAllDrives))

        Me.__token = token 'Hsien , 2016.09.12 , need token to identify exclusive data

        If lockActions IsNot Nothing Then
            Me.lockActions.AddRange(lockActions)
        End If
    End Sub

End Class

