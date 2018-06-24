Public Class interceptObject

    Private Enum interceptStateEnum
        LISTEN
        INTERCEPTED
    End Enum

    '------------
    '   Used for process intercetion
    '------------
    ReadOnly Property IsPaused As Boolean
        Get
            Return (pauseState = interceptStateEnum.INTERCEPTED)
        End Get
    End Property

    'internal
    Private pauseState As interceptStateEnum = interceptStateEnum.LISTEN

    Public Event InterceptedEvent As EventHandler
    Public Event UninterceptedEvent As EventHandler
    Public listening As Func(Of Boolean) = New Func(Of Boolean)(Function() (False))
    Public uninterceptListening As Func(Of Boolean) = New Func(Of Boolean)(Function() (False))

    'Private Function dummyFunction() As Boolean
    '    Return False
    'End Function

    Public Function pauseHandling() As Integer
        Select Case pauseState
            Case interceptStateEnum.LISTEN
                'listen request
                If (listening()) Then
                    RaiseEvent InterceptedEvent(Me, Nothing)
                    pauseState = interceptStateEnum.INTERCEPTED
                End If

            Case interceptStateEnum.INTERCEPTED
                'listen if unpause request come
                If (uninterceptListening()) Then
                    RaiseEvent UninterceptedEvent(Me, Nothing)
                    pauseState = interceptStateEnum.LISTEN
                End If

            Case Else
        End Select

        Return 0
    End Function

End Class
