Imports Automation
Imports Automation.Components.Services

Public Class brokenCheck
    Inherits systemControlPrototype
    Implements IModuleSingle

    Enum brokenResult As Integer
        OK = 1
        BROKEN = 0
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event BrokenFound(ByVal sender As Object, ByVal e As EventArgs)

    Public Property TargetPositionInfo As Func(Of shiftDataPackBase) Implements IModuleSingle.TargetPositionInfo

    Public additionalBrokenCondition As Func(Of Boolean) = Function() (False)   'return value : true , treat as broken , false , treat as normal

    Dim __delay As TimeSpan = New TimeSpan(0, 0, 0, 0, 100)
    Dim timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 5)}

    Dim extensionActionState As Integer = 0
    Public extensionAction As stateFunction = Function() (True) 'the  extension action after broken had found

    Protected __token As Object = ""

#Region "Device declare"
    Public spCrack1 As sensorControl = New sensorControl
    Public spCrack2 As sensorControl = New sensorControl
    Public spCrack3 As sensorControl = New sensorControl
    Public spCrack4 As sensorControl = New sensorControl
    Public spOccupied As sensorControl = New sensorControl
#End Region

    Protected Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                If TargetPositionInfo.Invoke.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then 'action

                    If spOccupied.IsEnabled Then
                        systemSubState = 10
                    ElseIf TargetPositionInfo.Invoke.IsPositionOccupied Then
                        'wafer existed
                        systemSubState = 20
                    Else
                        'no wafer
                        systemSubState = 200
                    End If
                Else
                    '---------------------
                    '   Conveyor is working
                    '---------------------
                End If
            Case 10
                If spOccupied.IsSensorCovered Then
                    '-------------------------
                    '   there's real wafer
                    '-------------------------
                    If (Not TargetPositionInfo.Invoke.IsPositionOccupied) Then
                        sendMessage(internalEnum.GENERIC_MESSAGE, "Unknown wafer on broken check")
                        TargetPositionInfo.Invoke.IsPositionOccupied = True '
                    End If

                    systemSubState = 20
                Else
                    '--------------------
                    '   Send a warning
                    '--------------------
                    If (TargetPositionInfo.Invoke.IsPositionOccupied) Then
                        sendMessage(internalEnum.GENERIC_MESSAGE, "Wafer lossed on broken check")
                        TargetPositionInfo.Invoke.IsPositionOccupied = False 'reset  , Hsien  ,2015.10.21, prevent 
                    End If

                    systemSubState = 200 'there is no wafer,  clear action flag directly
                End If
                '-----------------------
                '   Set a timeout
                '-----------------------
            Case 20
                timer.TimerGoal = __delay
                timer.IsEnabled = True
                systemSubState = 30
            Case 30
                If spCrack1.IsSensorCovered And
                    spCrack2.IsSensorCovered And
                    spCrack3.IsSensorCovered And
                    spCrack4.IsSensorCovered And
                   Not additionalBrokenCondition.Invoke Then
                    '-------------------
                    '   Passed
                    '-------------------
                    CType(TargetPositionInfo.Invoke, IInspection).InspectionResult(__token) = brokenResult.OK
                    systemSubState = 200
                ElseIf timer.IsTimerTicked Then
                    '---------------------------
                    '   Treat as NG
                    '---------------------------   
                    CType(TargetPositionInfo.Invoke, IInspection).InspectionResult(__token) = brokenResult.BROKEN
                    systemSubState = 100
                End If

            Case 100
                If (extensionAction(extensionActionState)) Then
                    extensionActionState = 0    'reset

                    RaiseEvent BrokenFound(Me, Nothing)  'hsien , event added

                    systemSubState = 200
                End If

                '---------------------------
                '   Finished
                '---------------------------
            Case 200 'finish
                'release
                TargetPositionInfo.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = 0
        End Select
        Return 0
    End Function

    Public Sub New(Optional targetPositionInvoke As Func(Of shiftDataPackBase) = Nothing,
            Optional token As Object = "")

        Me.__token = token

        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE
    End Sub

End Class