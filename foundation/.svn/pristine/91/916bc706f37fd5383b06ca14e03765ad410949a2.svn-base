Imports Automation
Public Class userControlCassette
    Enum slotIndex As Integer
        LEFT_MOST = 0
        MIDDLE = 1
        RIGHT_MOST = 2
    End Enum

    Property CassetteReference As cassetteSystemBase
        Get
            Return __cassetteRefernce
        End Get
        Set(value As cassetteSystemBase)
            If (value IsNot Nothing) Then
                __cassetteRefernce = value
                loadUsercontrol(Me, Nothing) 'Hsien , 2015.06.02
            End If
        End Set
    End Property
    WriteOnly Property IsMirror As Boolean
        Set(value As Boolean)
            TableLayoutPanelSlot.Controls.Remove(ButtonEject)
            TableLayoutPanelSlot.Controls.Remove(PanelFullAndId)
            If (value) Then
                'Cassette Eject on the Right
                TableLayoutPanelSlot.Controls.Add(ButtonEject, slotIndex.RIGHT_MOST, 0)
                TableLayoutPanelSlot.Controls.Add(PanelFullAndId, slotIndex.LEFT_MOST, 0)
            Else
                'Cassette Eject on the left
                TableLayoutPanelSlot.Controls.Add(ButtonEject, slotIndex.LEFT_MOST, 0)
                TableLayoutPanelSlot.Controls.Add(PanelFullAndId, slotIndex.RIGHT_MOST, 0)
            End If
        End Set
    End Property

    Public Shared failureIDPattern As String = "ReadFail"

    '----------------------------
    '   1. whether the unloading cassette fulled
    '   2. the cassette ID (if any)
    '   3. the cassette slot count
    '   4. the eject control
    '   5. (the inlet cassette short-coming)

    Dim WithEvents __cassetteRefernce As cassetteSystemBase = Nothing


    Dim __flip As Boolean = False
    Private Sub timerFlashTick(sender As Object, e As EventArgs) Handles TimerFlash.Tick
        utilitiesUI.controlFollowBoolean(LabelFlash, __flip)
        __flip = Not __flip
    End Sub

    Private Sub timerScanTick(sender As Object, e As EventArgs) Handles TimerScan.Tick
        With __cassetteRefernce
            If (.commonFlags.viewFlag(flagsInLoaderUnloader.CasUnloadFull_f)) Then
                cassetteFulledAction(.commonFlags.viewFlag(flagsInLoaderUnloader.CasUnloadFull_f))
            ElseIf (Not .commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f)) Then
                cassetteEmptyedAction(Not .commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f))    'if cassette not ready , mind the user
            Else
                LabelFlash.Text = ""
                TimerFlash.Enabled = False
                LabelFlash.BackColor = DefaultBackColor
            End If
            ButtonEject.Enabled = .commonFlags.viewFlag(flagsInLoaderUnloader.CasOn_UD_ConveyerReady_f) And (Not .commonFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f))

            'Hsien  ,present rest number and current count
            With __cassetteRefernce._cassetteLift
                LabelCount.Text = String.Format("{0}/{1}",
                                                .cntWafer.ToString,
                                                (.GoalCount - .cntWafer))
            End With
        End With
    End Sub

    Sub cassetteFulledAction(isCassetteFulled As Boolean)
        If (isCassetteFulled) Then
            LabelFlash.Text = My.Resources.CassetteFull
            TimerFlash.Enabled = True
        Else
            LabelFlash.Text = ""
            TimerFlash.Enabled = False
            LabelFlash.BackColor = DefaultBackColor
        End If
    End Sub
    'Hsien , 2015.07.15 , used to mind user
    Sub cassetteEmptyedAction(isCassetteEmptyed As Boolean)
        If (isCassetteEmptyed) Then
            LabelFlash.Text = My.Resources.CassetteEmpty
            TimerFlash.Enabled = True
        Else
            LabelFlash.Text = ""
            TimerFlash.Enabled = False
            LabelFlash.BackColor = DefaultBackColor
        End If
    End Sub

    'Hsien , 2015.06.11
    Sub cassetteIdRenewedHandler(sender As cassetteSystemBase, e As EventArgs) Handles __cassetteRefernce.CassetteIdRefreshed
        Me.Invoke(Sub()
                      LabelCassetteId.Text = sender.CassetteId      'Hsien , 2015.06.26

                      ' Hsien , once reading failure,  reflect the warning
                      utilitiesUI.controlFollowBooleanColor(LabelCassetteId, sender.CassetteId.Contains(failureIDPattern), Color.Red)

                  End Sub)
    End Sub

    Private Sub loadUsercontrol(sender As Object, e As EventArgs) Handles MyBase.Load
        If (__cassetteRefernce Is Nothing) Then
            Exit Sub
        End If

        TimerScan.Enabled = True    'start polling


    End Sub

    'Hsien , 2015.06.16 
    Private Sub ButtonEjectClick(sender As Object, e As EventArgs) Handles ButtonEject.Click
        If (Not __cassetteRefernce.commonFlags.viewFlag(flagsInLoaderUnloader.CasCollect_f)) Then
            __cassetteRefernce.commonFlags.setFlag(flagsInLoaderUnloader.CasCollect_f)
        End If
    End Sub
End Class
