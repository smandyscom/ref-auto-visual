Imports Automation
Imports AutoNumeric
Imports Automation.mainIOHardware

Public Class dispenseControl
    Inherits systemControlPrototype
    Implements IProcedure

    ''' <summary>
    ''' Set Dispention duration
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Arguments As Object Implements IProcedure.Arguments
        Get
            Return readWord(outputAddress.DISP_TIME)
        End Get
        Set(value As Object)
            writeWord(outputAddress.DISP_TIME, CULng(value))
        End Set
    End Property
    Public Property IsProcedureAbort As flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result

    ''' <summary>
    ''' 0. set DISP_AUTO_CONTROL ON
    ''' 1. check on DISP_RESPONSE
    ''' 2. reset DISP_AUTO_CONTROL ON
    ''' 3. check off DISP_RESPONSE
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateExecute() As Integer

        Select Case systemSubState
            Case 0
                If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    writeBit(outputAddress.DISP_AUTO_CONTROL, True)
                    systemSubState += 10
                Else
                    '---------------------
                    ''  Wait invokation
                    '---------------------
                End If
            Case 10
                If readBit(inputAddress.DISP_RESPONSE) Then
                    writeBit(outputAddress.DISP_AUTO_CONTROL, False)
                    systemSubState += 10
                Else
                    '---------------------
                    ''  Wait response
                    '---------------------
                End If
            Case 20
                If Not readBit(inputAddress.DISP_RESPONSE) Then
                    IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                    systemSubState = 0 'rewind
                Else
                    '---------------------
                    ''  Wait signal reset
                    '---------------------
                End If

        End Select

        Return 0
    End Function

    Sub New()
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.EXECUTE
    End Sub

End Class
