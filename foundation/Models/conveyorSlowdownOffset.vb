﻿Imports Automation

Public Class conveyorSlowdownOffset
    Inherits clsSynchronizableTransporterPullTypeV2
    'the special edition of pulltypev2 , if slots had wafer occupied , the transfer action would do a offset 
    ' then handshake the downstream do normal transfer

    Public offsetPoint As cMotorPoint = Nothing
    Public externalCondition As Func(Of Boolean) = Function() (True)

    Dim offsetState As Integer = 0
    Dim isNeedOffset As Boolean = False

    Protected Overrides Function dataVerifyAction() As Boolean

        '---------------------------
        '   For Pre verfify , do not override
        '---------------------------
        If (ShiftState = shiftingStates.DATA_PRE_VERIFY) Then
            Return MyBase.dataVerifyAction
        End If

        '-------------------------------
        '   Active when post-verifing
        '-------------------------------
        Select Case offsetState
            Case 0
                If (isNeedOffset And
                   __occupiedStatus.IsAllOccupied) Then
                    'wafer existed , and need do offset
                    offsetState = 10
                Else
                    '------------------------------
                    '   No need to offset
                    '------------------------------
                    offsetState = 100
                End If

                isNeedOffset = False    'reset mark
                '--------------------------------------------------
                '   Do slight offset
                '--------------------------------------------------
            Case 10
                'synchron with clamp4/down stream conveyor
                If (externalCondition.Invoke) Then
                    motorMasterConveyor.SlowdownEnable = enableEnum.DISABLE
                    offsetState = 20
                End If
            Case 20
                'do slight transfer
                If (motorMasterConveyor.drive(Components.CommandStateMachine.motorControl.motorCommandEnum.GO_POSITION, offsetPoint) =
                   Components.CommandStateMachine.motorControl.statusEnum.EXECUTION_END) Then
                    isNeedOffset = False  'reset state
                    motorMasterConveyor.SlowdownEnable = enableEnum.ENABLE
                    offsetState = 100
                End If
            Case 100
                'the since on post verification , no need to do check
                offsetState = 0    'reset state
                Return True
        End Select

        Return False

    End Function
    Protected Overrides Function transferProcess() As Boolean

        Dim __transferProcessResult As Boolean = MyBase.transferProcess

        'once transferred done and wafer is incoming , set on the need offset mark
        If (__transferProcessResult And
           IncomingShiftData.IsPositionOccupied) Then
            isNeedOffset = True
        End If

        Return __transferProcessResult
    End Function


End Class
