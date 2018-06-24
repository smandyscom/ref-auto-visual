Imports System.Text

Public Class userControlMessage

    Property MaxLines As Integer = 64

    Public WithEvents messengerReference As messageHandler

    Delegate Function MessageFilterPrototpe(sender As messageHandler, e As messagePackageEventArg) As Boolean

    Public IsValidToShow As MessageFilterPrototpe = Function() (True)    'used reject unnessaary messages
    Public MessageFormator As Func(Of messageHandler, messagePackageEventArg, String) = Function(sender As messageHandler, e As messagePackageEventArg) (vbCrLf & e.Message.ToString)    'used to generate formatted string

    Private Sub historyMessageHandler(ByVal sender As messageHandler, ByVal e As messagePackageEventArg) Handles messengerReference.MessagePoped

        If (Me.IsDisposed) Then
            Exit Sub
        End If

        'filter out unnessary messages
        If Not IsValidToShow.Invoke(sender, e) Then
            Exit Sub
        End If

        '---------------------------
        ' Once this message is not equal to last mesage  , post on the history panel
        '---------------------------
        Me.Invoke(Sub()
                      RichTextBoxMessage.Text += MessageFormator(sender, e)
                      '-------------------------------------------
                      '    Discard oldest message if count reached to prevent buffer overload
                      '-------------------------------------------
                      utilitiesUI.textBoxLimitMaxLine(RichTextBoxMessage, MaxLines)
                      '----------------------
                      ' keep scoll on bottom
                      '---------------------
                      utilitiesUI.textBoxKeepScrollBottom(RichTextBoxMessage)

                  End Sub)

    End Sub

    '-----------------------------------------
    'the filter utilities , Hsien , 2015.09.18
    '-----------------------------------------
    Dim lastMessage As messagePackage = New messagePackage(Me, Me, "")    'Hsien , 2015.7.28
    Dim thisMessage As StringBuilder = New StringBuilder()

    Function isNonRedundantMessage(sender As messageHandler, e As messagePackageEventArg) As Boolean
        Dim condition As Boolean = Not lastMessage.Equals(e.Message)
        lastMessage = e.Message   'backup message
        Return condition    'true : not redundant , false : redundnat message
    End Function
    Shared Function isStatusMessage(sender As messageHandler, e As messagePackageEventArg) As Boolean
        Return e.Message.PrimaryKey.Equals(statusEnum.GENERIC_MESSAGE)
    End Function

    Public Shared Function generateMessageFilters(__filters As MessageFilterPrototpe()) As MessageFilterPrototpe
        'the utility tool used to combined messageFilters

        'return a combined delegate
        Return Function(sender As messageHandler, e As messagePackageEventArg) As Boolean
                   Dim __result As Boolean = True
                   For Each __eachFilter As MessageFilterPrototpe In __filters
                       __result = __eachFilter.Invoke(sender, e) And __result
                   Next

                   Return __result
               End Function

    End Function


End Class
