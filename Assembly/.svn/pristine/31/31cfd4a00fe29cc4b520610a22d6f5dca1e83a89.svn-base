Imports Automation
Imports AutoNumeric
Imports Automation.Components.Services
Imports Automation.mainIOHardware

Public Class dispSetting
    Inherits settingBase

    ''' <summary>
    ''' Unit : ms
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DispentionTime As Integer
        Get
            Return __dispentionTime.TotalMilliseconds
        End Get
        Set(value As Integer)
            __dispentionTime = New TimeSpan(0, 0, 0, 0, value)
        End Set
    End Property
    Friend __dispentionTime As TimeSpan = New TimeSpan(0, 0, 0, 0, 100)

    Public Overrides Property Filename As String
        Get
            Return String.Format("{0}{1}.xml",
                                 measureProcedureSetting.settingPath,
                                 Me.ToString)
        End Get
        Set(value As String)
            'nothing
        End Set
    End Property

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class dispWorking
    Inherits systemControlPrototype
    Implements IProcedure
    Implements IDisposable

    Friend __dispSetting As dispSetting = New dispSetting

    Public Property Arguments As Object Implements IProcedure.Arguments
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property IsProcedureAbort As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort

    Public Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result

    Dim __dispense As dispenseControl = New dispenseControl With {.IsEnabled = True}
    Dim __timer As singleTimer = New singleTimer
    ''' <summary>
    ''' 1. move to ready position
    ''' 2. move to working position
    ''' 3. dispention start
    ''' 4. wait until dispention end (simple handshake with plc)
    ''' 5. move to ready position
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateExecute() As Integer

        If IsProcedureAbort.readFlag(interlockedFlag.POSITION_OCCUPIED) Then
            IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            systemSubState = 0 'reset
        End If

        'keep updating dispention time
        __dispense.Arguments = __dispSetting.DispentionTime

        With frames.Instance
            Select Case systemSubState
                Case 0
                    If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                        .CurrentMovingItem = framesDefinition.DISP_HEAD_REAL
                        .CurrentRItem = itemsDefinition.DIE_DISP_READY


                        systemSubState += 10
                    Else
                        '---------------------------------
                        ' 
                        '---------------------------------
                    End If
                Case 10
                    If Assembly.Instance.IsAllAxesSettled Then
                        .CurrentMovingItem = framesDefinition.DISP_HEAD_REAL
                        .CurrentRItem = itemsDefinition.DIE_DISP_WORK
                        systemSubState += 10
                    Else
                        '---------------------------------
                        ' Settling
                        '---------------------------------
                    End If
                Case 20
                    If Assembly.Instance.IsAllAxesSettled Then
                        'activate dispensing
                        __dispense.IsProcedureStarted.setFlag(interlockedFlag.POSITION_OCCUPIED)
                        systemSubState += 10
                    Else
                        '---------------------------------
                        ' Settling
                        '---------------------------------
                    End If
                Case 30
                    If Not __dispense.IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                        With __timer
                            .TimerGoal = New TimeSpan(0, 0, 0, 0, 100)
                            .IsEnabled = True
                        End With
                        systemSubState += 10
                    Else
                        '---------------------------------
                        ' Count Down
                        '---------------------------------
                    End If
                Case 40
                    If __timer.IsTimerTicked Then
                        c4htm.Instance.AxisValue(axisEntityEnum.Z) = 0 'return
                        systemSubState += 10
                    Else
                        '----------------------
                        '   Working
                        '----------------------
                    End If
                Case 50
                    If Assembly.Instance.IsAllAxesSettled Then
                        systemSubState = 500
                    Else
                        '---------------------------------
                        ' Settling
                        '---------------------------------
                    End If
                Case 500
                    'writeBit(outputAddress.SYNRINGE, False) 'activate synringe
                    IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                    systemSubState = 0
            End Select
        End With


        Return 0
    End Function



#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As dispWorking
        Get
            If __instance Is Nothing Then
                __instance = New dispWorking
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As dispWorking = Nothing
#End Region

    Protected Sub New()
        Result = IProcedure.procedureResultEnums.SUCCESS
        __dispSetting.Load(Nothing)
        Me.IsEnabled = True
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.EXECUTE
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

            End If
            __dispSetting.Save()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
