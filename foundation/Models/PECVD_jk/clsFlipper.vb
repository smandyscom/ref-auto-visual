﻿Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services

Public Class clsFlipper
    Inherits shiftingModel
    Implements IFinishableStation

    Enum FlagsEnum
        IsEnable
        IsAllowEmptyIn  'hsien , 2015.06.29 , going to fix loading issue
    End Enum

    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}
    Enum IgniteEnum

        _0

        _10

        _20

        _30

        _100

        _40

        _50

        _110

    End Enum
    Enum isAbleTransferEnum
        _0
        _5  'Hsien , used to solve emtpy-in issue , 2015.06.29
        _10

        _20

        _30

        _40

        _50

        _60

        _100

        _200

        _210

        _220

    End Enum
    Enum transferProcessEnum
        _0

        _10

        _20

        _30

        _40

        _50

        _60

        _100

    End Enum
#Region "data declare"
    Public Property TargetPositionIn As Func(Of shiftDataPackBase)
    Public Property TargetPositionOut As Func(Of shiftDataPackBase)
    Public flags As flagController(Of FlagsEnum) = New flagController(Of FlagsEnum)
    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As New List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property InConveyor As New List(Of shiftingModel) '偵測該輸送帶是否全空
    Dim isEnableFlip As Boolean
    'Public Property TargetPositionInfo As Func(Of ICollection(Of shiftDataPackBase)) Implements IModuleMulti.TargetPositionInfo
    'Protected __waferCapability As Integer = 6 - 2  ' how many wafer could store in this flipper, excluding input and output positions.
#End Region
#Region "Device declare"
    Property motorFlip As motorControl = New motorControl
    Property SpFlipIn As sensorControl = New sensorControl '翻轉入料處sensor
    Property SpFlipOut As sensorControl = New sensorControl '翻轉出料處sensor
    Property cyShift As cylinderGeneric = New cylinderGeneric 'b side: close, a side: open
#End Region

    Dim apHomingSensorCovered As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                                   .PossibleResponse = alarmContextBase.responseWays.RETRY,
                                                                                   .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF}
    Public Enum flipPositions
        OFFSET
        FLIP
    End Enum
    ''' <summary>
    ''' 歸原點時，若發生任何Alarm，馬達motorFlip會立即停止(Pause功能)，解除後會重新運作(Resume)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function stateIgnite() As Integer 'motor homing , cylinder initial position
        Ignite(systemSubState)
        Return 0
    End Function
    Private Function Ignite(ByRef cStep As IgniteEnum) As Integer
        Select Case cStep
            Case IgniteEnum._0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) = True Then
                    cStep = IgniteEnum._10
                End If
            Case IgniteEnum._10
                isEnableFlip = flags.viewFlag(FlagsEnum.IsEnable) 'remember enable flag
                If isEnableFlip = True Then

                    If (cyShift IsNot Nothing) Then
                        cyShift.drive(cylinderControlBase.cylinderCommands.GO_B_END)  '夾
                    End If

                Else

                    If (cyShift IsNot Nothing) Then
                        cyShift.drive(cylinderControlBase.cylinderCommands.GO_A_END) '開
                    End If

                End If
                cStep = IgniteEnum._20
            Case IgniteEnum._20
                If cyShift Is Nothing OrElse cyShift.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    cStep = IgniteEnum._30
                End If
            Case IgniteEnum._30
                If SpFlipIn.IsSensorCovered = True OrElse SpFlipOut.IsSensorCovered = True Then
                    cStep = IgniteEnum._100 'alarm
                Else
                    motorFlip.drive(motorControl.motorCommandEnum.GO_HOME)
                    cStep = IgniteEnum._40
                End If
            Case IgniteEnum._40
                If SpFlipIn.IsSensorCovered = True OrElse SpFlipOut.IsSensorCovered = True Then
                    'stop flipper motor
                    cStep = IgniteEnum._100 'alarm
                ElseIf (motorFlip.CommandEndStatus = motorControl.statusEnum.EXECUTION_END) Then
                    cStep = IgniteEnum._50
                End If
            Case IgniteEnum._50
                If motorFlip.drive(motorControl.motorCommandEnum.GO_POSITION, flipPositions.OFFSET) = motorControl.statusEnum.EXECUTION_END Then
                    systemMainState = systemStatesEnum.EXECUTE
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.COMMAND_IGNITE, False)
                    'systemSubState = 0 '不需要，會自動歸0
                End If
            Case IgniteEnum._100 'stop flipper motor
                If motorFlip.drive(motorControl.motorCommandEnum.STOP_SLOW_DOWN) = motorControl.statusEnum.EXECUTION_END Then
                    cStep = IgniteEnum._110
                End If
            Case IgniteEnum._110
                '必須等到所有條件成立才能繼續歸Home
                With apHomingSensorCovered '只有重試
                    If SpFlipIn.IsSensorCovered = True Then
                        .Inputs = SpFlipIn.InputBit
                        CentralAlarmObject.raisingAlarm(apHomingSensorCovered)
                    ElseIf SpFlipOut.IsSensorCovered = True Then
                        .Inputs = SpFlipOut.InputBit
                        CentralAlarmObject.raisingAlarm(apHomingSensorCovered)
                    Else
                        cStep = IgniteEnum._0
                    End If
                End With
        End Select
        Return 0
    End Function
    Protected Overrides Function isAbleTransfer() As Boolean 'shiftingModel中定義必須實作出的函式
        Return _isAbleTransfer(actionState)
    End Function
    Private Function _isAbleTransfer(ByRef cStep As isAbleTransferEnum) As Boolean 'shiftingModel中定義必須實作出的函式
        Select Case cStep
            Case isAbleTransferEnum._0
                '收料結束檢查
                If UpstreamStations.Count > 0 AndAlso
                    UpstreamStations.TrueForAll(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = True)) AndAlso
                    IsAnyRemained = False AndAlso InConveyor.TrueForAll(Function(obj As shiftingModel) (obj.IsAnyRemained = False)) Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, True)
                    cStep = isAbleTransferEnum._200
                    '檢查進、出料處任務 (翻轉可能會跨越兩條輸送帶，所以要看兩個module action)
                ElseIf TargetPositionIn.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True AndAlso
                    TargetPositionOut.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True Then
                    If flags.viewFlag(FlagsEnum.IsEnable) = False Then
                        cStep = isAbleTransferEnum._100
                    ElseIf IsAnyRemained = False AndAlso TargetPositionIn.Invoke.IsPositionOccupied = False Then
                        'nothing left but triggered
                        cStep = isAbleTransferEnum._100
                    Else
                        'working condition , Hsien , 2015.06.29
                        'cStep = isAbleTransferEnum._10
                        cStep = isAbleTransferEnum._5
                    End If
                ElseIf TargetPositionOut.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) = True AndAlso UpstreamStations.Count > 0 Then
                    Dim blnAllFinish As Boolean = True
                    For i As Short = 0 To UpstreamStations.Count - 1
                        If UpstreamStations(i).FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False Then
                            blnAllFinish = False
                            Exit For
                        End If
                    Next
                    If blnAllFinish = True Then
                        If flags.viewFlag(FlagsEnum.IsEnable) = False Then
                            cStep = isAbleTransferEnum._100
                        ElseIf IsAnyRemained = False AndAlso TargetPositionIn.Invoke.IsPositionOccupied = False Then
                            cStep = isAbleTransferEnum._100
                        Else
                            cStep = isAbleTransferEnum._10
                        End If
                    End If
                End If
            Case isAbleTransferEnum._5
                If (flags.viewFlag(FlagsEnum.IsAllowEmptyIn)) Then
                    cStep = isAbleTransferEnum._10  'the original route
                Else
                    If (TargetPositionIn.Invoke.IsPositionOccupied) Then
                        cStep = isAbleTransferEnum._10
                    Else
                        'not allow emtpy in : release the source convyor , wait next trigger
                        TargetPositionIn.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                        cStep = isAbleTransferEnum._0   'not allow to move
                    End If
                End If
            Case isAbleTransferEnum._10 '移動前的檢查
                '檢查進出料處sensor是否符合。Note: 輸送帶都會檢查，可不必再檢查。若要檢查，會寫得較複雜。
                '檢查cyPush必須回原點: 直接再驅動一次，查看是否到位。
                'If cyPush.drive(cylinderControlBase.cylinderCommands.GO_A_END) = IDrivable.endStatus.EXECUTION_END Then
                If SpFlipOut.IsSensorCovered = True Then
                    apHomingSensorCovered.Inputs = SpFlipOut.InputBit
                    CentralAlarmObject.raisingAlarm(apHomingSensorCovered)
                Else
                    IncomingShiftData.Assign(TargetPositionIn.Invoke) 'assignment
                    Return True
                End If
            Case isAbleTransferEnum._100 '直接回應不移動
                TargetPositionIn.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                TargetPositionOut.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                cStep = isAbleTransferEnum._210

                '==== 收料結束 ======
            Case isAbleTransferEnum._200
                '若還有wafer進來，仍要翻轉，解除收料結束
                If TargetPositionIn.Invoke.IsPositionOccupied = True Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False)
                    cStep = isAbleTransferEnum._210
                ElseIf UpstreamStations.Exists(Function(obj As IFinishableStation) (obj.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) = False)) Then
                    FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, False)
                    cStep = isAbleTransferEnum._210
                Else
                    'auto clear module action
                    TargetPositionIn.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                    TargetPositionOut.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                End If
            Case isAbleTransferEnum._210
                isEnableFlip = flags.viewFlag(FlagsEnum.IsEnable)
                If isEnableFlip = True Then

                    If (cyShift IsNot Nothing) Then
                        cyShift.drive(cylinderControlBase.cylinderCommands.GO_B_END) '夾
                    End If

                Else

                    If (cyShift IsNot Nothing) Then
                        cyShift.drive(cylinderControlBase.cylinderCommands.GO_A_END) '開
                    End If

                End If
                cStep = isAbleTransferEnum._220
            Case isAbleTransferEnum._220
                If cyShift Is Nothing OrElse cyShift.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    cStep = isAbleTransferEnum._0
                End If

        End Select
        Return False
    End Function

    'Hsien  , correction 2015.07.08
    Sub alarmOccursHandler() Handles CentralAlarmObject.alarmOccured, PauseBlock.InterceptedEvent
        'If (MainState = systemStatesEnum.IGNITE) Then
        motorFlip.drive(motorControl.motorCommandEnum.MOTION_PAUSE)
        'End If
    End Sub

    Sub alarmReleaseHandler() Handles CentralAlarmObject.alarmReleased, PauseBlock.UninterceptedEvent
        'If (MainState = systemStatesEnum.IGNITE) Then
        motorFlip.drive(motorControl.motorCommandEnum.MOTION_RESUME)
        'End If
    End Sub

    Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf initMappingAndSetup))
    End Sub
    Function initMappingAndSetup()
        flags.writeFlag(FlagsEnum.IsAllowEmptyIn, True) '2015.7.16 jk add let allow empty in true as default
        '預先定義歸Home時，若進出料的位置有片，則馬達立即停止歸Home，並報警
        With apHomingSensorCovered
            .Sender = Me    'Hsien , 2015.05.14
            .PossibleResponse = alarmContextBase.responseWays.RETRY
            .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF
        End With

        'cyPush.StrokeGoal = New TimeSpan(0, 0, 5) '設定推片氣缸timeout時間

        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives()
        Return 0
    End Function
    Protected Overrides Function dataVerifyAction() As Boolean
        Return checkWaferCoverLossAndUnknown() And checkWaferThroughJammed() 'And checkWaferThroughLoss() 'And checkWaferThroughUnknown()
    End Function
    Protected Overrides Function transferProcess() As Boolean
        Return _transferProcess(actionState)
    End Function
    Private Function _transferProcess(ByRef cStep As transferProcessEnum) As Boolean
        Select Case cStep
            Case transferProcessEnum._0 'motor flip
                If motorFlip.drive(motorControl.motorCommandEnum.GO_POSITION, flipPositions.FLIP) = motorControl.statusEnum.EXECUTION_END Then

                    TargetPositionIn.Invoke.IsPositionOccupied = False 'clear incoming position

                    TargetPositionOut.Invoke.Assign(OccupiedStatus.DataCollection.Last)

                    TargetPositionIn.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                    TargetPositionOut.Invoke.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED, False)
                    Return True
                End If
        End Select
        Return False
    End Function
    Private Sub cyDrive(cy As cylinderControlBase, ByVal command As cylinderControlBase.cylinderCommands)
        cy.drive(command)
    End Sub

End Class

#Region "必須放在外面的"
#If 1 = 0 Then
Public Class TestBench
    Sub Test()
        Dim MyFlipper As clsFlipper = New clsFlipper
        MyFlipper.shiftDataType = GetType(waferDataGintechPECVD)
    End Sub
End Class
#End If

#End Region
