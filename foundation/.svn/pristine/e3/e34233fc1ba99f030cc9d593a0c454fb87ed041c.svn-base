Imports Automation
Imports Automation.Components.Services

Public Class UserControlTitleWarning
    Dim WarningQueue As New Concurrent.ConcurrentQueue(Of warningMessagePackage)

    Dim lstWarning As New List(Of warningMessagePackage) '越新(time stamp)的warning會放在後面
    Dim lstRemoveWarning As New List(Of warningMessagePackage)
    Public WithEvents messengerReference As messageHandler
    Delegate Function MessageFilterPrototpe(sender As messageHandler, e As messagePackageEventArg) As Boolean
    Public IsValidToShow As MessageFilterPrototpe = Function() (True)    'used reject unnessaary messages
    Property switchTimeInterval As New TimeSpan(TimeSpan.TicksPerSecond * 3) '每個訊息輪播的時間
    Property timeoutInterval As New TimeSpan(TimeSpan.TicksPerSecond * 2) '每個訊息超過3秒沒送，就代表關掉，意指若只有單個訊息，而超過3秒沒收到相同字串的新訊息，則代表要清除此訊息。
    Dim currentWarning As warningMessagePackage
    Dim popTime As Date
    Property blinkInterval As New TimeSpan(TimeSpan.TicksPerSecond * 0.5)
    Dim previousBlinkTime As Date = Now
    Const DEFAULT_STRING = "Delta Automation"
    Private Sub BasicLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        Timer1.Interval = 100
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        If WarningQueue.Count > 0 Then '解析新進的warning message 看是否要加入至lstWarning或從lstWarning移除
            Dim newWarning As warningMessagePackage = Nothing
            If WarningQueue.TryDequeue(newWarning) = True Then
                Select Case newWarning.PrimaryKey
                    Case statusEnum.WARNING_MESSAGE_ADD
                        If lstWarning.Contains(newWarning) = False Then lstWarning.Add(newWarning)
                    Case statusEnum.WARNING_MESSAGE_REMOVE
                        lstWarning.Remove(newWarning)
                End Select
            End If
        End If

        '==== show warning ======
        If lstWarning.Count > 0 Then
            If lstWarning.Contains(currentWarning) = False Then 'if currentWarning is nothing , it would return false
                currentWarning = lstWarning(0)
                LabelBanner.Text = currentWarning.AdditionalInfo
                popTime = Now

            ElseIf DateDiff(DateInterval.Second, popTime, Now) > switchTimeInterval.Seconds Then '檢查目前的warning顯示了多少秒，若超過秒數，則移至下一個warning
                Dim i As Integer = lstWarning.IndexOf(currentWarning)
                i += 1
                If i < lstWarning.Count Then currentWarning = lstWarning(i) Else currentWarning = lstWarning(0)
                LabelBanner.Text = currentWarning.AdditionalInfo
                popTime = Now

            End If

            'blink tile bar
            If DateDiff(DateInterval.Second, previousBlinkTime, Now) > blinkInterval.Seconds Then
                If LabelBanner.BackColor = Color.White Then LabelBanner.BackColor = Color.Yellow Else LabelBanner.BackColor = Color.White
                previousBlinkTime = Now
            End If
        Else
            LabelBanner.Text = DEFAULT_STRING
            LabelBanner.BackColor = Color.White
        End If



    End Sub
    Private Sub MessageHandler(ByVal sender As messageHandler, ByVal e As messagePackageEventArg) Handles messengerReference.MessagePoped

        If (Me.IsDisposed) Then
            Exit Sub
        End If

        'filter out unnessary messages
        If Not IsValidToShow.Invoke(sender, e) Then
            Exit Sub
        End If
        If TryCast(e.Message, warningMessagePackage) Is Nothing Then Exit Sub
        WarningQueue.Enqueue(e.Message)
    End Sub
End Class
