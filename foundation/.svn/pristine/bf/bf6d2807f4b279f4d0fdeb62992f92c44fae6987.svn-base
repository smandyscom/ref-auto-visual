Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.mainIOHardware
''' <summary>
''' detect the unlock signal --> send pause flag, and unlock the door
''' wait unlock signal pressed again, try to lock the door, if not , 
''' </summary>
''' <remarks></remarks>
Public Class clsSaftyGuard
    Inherits systemControlPrototype


#Region "Device declare"
    Property ButtonUnlockBits As New List(Of ULong) ' 解鎖鍵的集合
    Property LampUnlockBits As New List(Of ULong) '解鎖燈的集合
    Property LockBits As New List(Of ULong)
    Property DoorBits As New List(Of ULong)
#End Region
#Region "External Data declare"


#End Region
#Region "Internal Data declare"
    Dim tmr As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 500)}
#End Region

    Protected Function stateIgnite() As Integer
        systemMainState = systemStatesEnum.EXECUTE
        Return 0
    End Function
    Protected Function stateExecute() As Integer
        Select Case systemSubState
            Case 0
                SetAllBits(LockBits, True) '激磁所有門鎖
                tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 0.5)
                systemSubState = 10
            Case 10 '查看門禁狀態
                If tmr.IsTimerTicked = True Then
                    If IsAnyBits(DoorBits, False) = True Then '任一門被開啟
                        'SetAllBits(LockBits, False) '開鎖
                        'SetAllBits(LampUnlockBits, True) '門鎖燈亮 代表需要使用者按下此鍵再確認
                        'systemSubState = 20
                    Else
                        SetAllBits(LampUnlockBits, False) '解鎖燈關，代表正常運作
                        systemSubState = 100
                    End If
                End If
            Case 20 '等待使用者按下解鎖鍵 來上鎖
                If IsAnyBits(ButtonUnlockBits, True) = True Then
                    tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 0.3) : tmr.IsEnabled = True
                    systemSubState = 30
                End If
            Case 30
                If tmr.IsTimerTicked = True Then
                    If IsAllBits(ButtonUnlockBits, False) = True Then
                        systemSubState = 0
                    End If
                End If

            Case 100 '所有門已關上，等待使用者按下解鎖需求
                If IsAnyBits(ButtonUnlockBits, True) = True Then
                    SetAllBits(LampUnlockBits, True)
                    SetAllBits(LockBits, False)
                    tmr.TimerGoal = New TimeSpan(TimeSpan.TicksPerSecond * 0.5) : tmr.IsEnabled = True
                    systemSubState = 110
                End If
            Case 110
                If tmr.IsTimerTicked = True Then
                    If IsAllBits(ButtonUnlockBits, False) = True Then
                        systemSubState = 20
                    End If
                End If

        End Select
        Return 0
    End Function
    Private Function IsAnyBits(bits As List(Of ULong), IsOn As Boolean) As Boolean
        Return bits.Exists(Function(obj As ULong) readBit(obj) = IsOn)
    End Function
    Private Function IsAllBits(bits As List(Of ULong), IsOn As Boolean) As Boolean
        Return bits.TrueForAll(Function(obj As ULong) readBit(obj) = IsOn)
    End Function
    Private Sub SetAllBits(Bits As List(Of ULong), IsOn As Boolean)
        Bits.ForEach(Sub(obj As ULong) writeBit(obj, IsOn))
    End Sub
    Function initMappingAndSetup()
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.IGNITE
        initEnableAllDrives() 'enable 此class裡所有的driveBase
        Return 0
    End Function
    Protected Overrides Function process() As Integer
        drivesRunningInvoke()

        stateControl()
        processProgress()

        Return 0
    End Function
    Public Sub New()
        Me.initialize = [Delegate].Combine(Me.initialize, New Func(Of Integer)(AddressOf Me.initMappingAndSetup))
    End Sub

End Class
