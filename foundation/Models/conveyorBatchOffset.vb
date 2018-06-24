Imports Automation

Public Class conveyorBatchOffset
    Inherits clsSynchronizableTransporterPullTypeV2
    'the special edition of pulltypev2 , if all slots had wafer occupied , the transfer action would do a offset 
    ' then handshake the downstream do normal transfer

    Public offsetPoint As cMotorPoint = Nothing

    Dim offsetState As Integer = 0

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Default : need to offset
    ''' </remarks>
    Dim isNeedOffset As Boolean = True

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function dataVerifyAction() As Boolean

        'dont do shift
        If (ShiftState = shiftingStates.DATA_PRE_VERIFY) Then
            'once transferred done and all slot emptyed , set on the need offset mark

            If Not IsAnyRemained Then
                isNeedOffset = True
            End If

            Return MyBase.dataVerifyAction
        End If
        '--------------------------------
        '   POST VERIFY
        '--------------------------------
        Select Case offsetState
            Case 0
                'once new batch of wafers had placed , need to offset
                If (isNeedOffset) Then
                    offsetState = 10
                Else
                    offsetState = 100
                End If

                isNeedOffset = False 'reset state

            Case 10
                'do slight transfer
                If (motorMasterConveyor.drive(Components.CommandStateMachine.motorControl.motorCommandEnum.GO_POSITION, offsetPoint) =
                   Components.CommandStateMachine.motorControl.statusEnum.EXECUTION_END) Then
                    offsetState = 100
                End If
            Case 100
                offsetState = 0
                Return True
        End Select

        Return False
    End Function


End Class
