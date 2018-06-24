Imports System.Collections.Concurrent
Public Class alarmMutex '此structure，只有給Alarm用
    '----------------------------------------
    ' All instance 'shared' the same storage
    '----------------------------------------
    '---------------------------------------------
    ' Check On : check if specific sensor is on
    ' Check Off : check if specific sensor is off
    '---------------------------------------------
    '------------------------------------
    ' Following variable used by SetAlarm
    ' by Hsien , 2014/6/3
    '------------------------------------
    Public Enum eErrType
        ERROR_MOTOR = 0
        ERROR_IO = 1
        ERROR_EXTERNAL = 3
        ERROR_NONE
    End Enum

    Public Enum errorReturnMode
        'mode of recovering from error
        ErrNoneMode = &H0         ' 0x0000
        ErrRetryMode = &H1        ' 0x0001do not rising flagIgnore
        ErrIgnoreMode = &H10       ' 0x0010rising flagIgnore
        ErrEndMode = &H100          ' 0x0100
    End Enum

    Private Enum alarmHandlingStateEnum
        LISTENING               ' sensing if any alarm object in queue
        OCCURED
        WAIT_RESPONSE
        RELEASE
    End Enum

    ' control interface
    ReadOnly Property IsAlarmed As Boolean
        ' for monitoring in control thread
        Get
            Return sw
        End Get
    End Property
    ReadOnly Property ReadUserResponse As errorReturnMode
        Get
            ' use flag to ensure the return value
            If (flagIsUserResponsed) Then
                flagIsUserResponsed = False
                Return Me.SelectMode
            Else
                Return errorReturnMode.ErrNoneMode
            End If
        End Get
    End Property
    WriteOnly Property UserResponse As errorReturnMode
        ' for user return
        Set(ByVal value As errorReturnMode)
            SelectMode = value
            flagIsUserResponsed = True
        End Set
    End Property
    'ReadOnly Property IsUserResponseIgnore As Boolean
    '    ' face to the point which firing alarm
    '    Get
    '        If (flagUserResponseIgnore) Then
    '            ' flag hand shaking
    '            flagUserResponseIgnore = False
    '            Return True
    '        Else
    '            Return False
    '        End If
    '    End Get
    'End Property

    ' internal - alarm mutex communication
    Public sw As eSwitch = eSwitch.eOFF  '* General Alarm switch , nor On neither Off , the flag indicate if the object occupied by others
    Public SelectMode As errorReturnMode = errorReturnMode.ErrNoneMode     ' used to response by user

    ' internal - alarm content description , indicated by whose firing alarm
    Public type As eErrType
    Public code As Integer  ' as error code defined in mdlAMONetErrCode.vb , positive value as Input/Alarm , negtive value as Motor/AMONet alarm
    Property ShouldBeOnOff As eCheckStatus  '報警時，顯示此input現在狀態, 給chkon,chkoff共用 , give hint to user " this switch should be on/off"
    Property MotorName As String = ""
    Property AlarmMessage As String = ""
    ' used to control UI's content
    Property PossibleResponse As errorReturnMode = errorReturnMode.ErrRetryMode Or errorReturnMode.ErrIgnoreMode Or errorReturnMode.ErrEndMode
    Property RetryOnly As Boolean
        ' for bridging old alarm annoucing system
        Get
            Return flagRetryOnly
        End Get
        Set(ByVal value As Boolean)
            flagRetryOnly = value
            If (flagRetryOnly) Then
                PossibleResponse = errorReturnMode.ErrRetryMode
            End If
        End Set
    End Property
    Private flagRetryOnly As Boolean = False

    ' internal - 
    'Private flagUserResponseIgnore As Boolean = False
    Private flagIsUserResponsed As Boolean = False     ' the handshaking flag

    Public InProcess As Boolean
    Public state As Boolean '使用都回應是否要重試或忽略chkon,chkoff , indicating if alarm had been ignored by user
    ' if alarm type is retry-only  , then user cannot avoid such alarm

    Delegate Function alarmHandler() As Integer

    ' output delegate
    'Public alarmWaitResponseHandler As alarmHandler
    'Public alarmOccuredHandler As alarmHandler
    'Public alarmReleaseHandler As alarmHandler

    'event
    Public Event alarmOccured(ByVal sender As Object, ByVal e As EventArgs)
    Public Event alarmWaitResponse(ByVal sender As Object, ByVal e As EventArgs)
    Public Event alarmRelease(ByVal sender As Object, ByVal e As EventArgs)

    ' alarm content , used to specific what where how the alarm occured ( alarm message box)
    Public Structure alarmContent
        Public type As eErrType
        Public code As Integer
        Public shouldBeOnOff As eCheckStatus
        Public motorName As String
    End Structure
    Private alarmDetail As alarmContent = New alarmContent
    ReadOnly Property Detail As alarmContent
        Get
            alarmDetail.type = Me.type
            alarmDetail.code = Me.code
            alarmDetail.shouldBeOnOff = Me.ShouldBeOnOff
            alarmDetail.motorName = Me.MotorName
            Return alarmDetail
        End Get
    End Property

    ' internal
    Private alarmHandlingState As alarmHandlingStateEnum = alarmHandlingStateEnum.OCCURED                    ' indicate the current alarm handling state ' by Hsien , 2014/6/3

    Public Function SetAlarm(ByVal alarmCode As Integer, ByVal checkStatus As eCheckStatus, ByVal alarmType As eErrType, ByVal possibleResponse As errorReturnMode) As Boolean
        '------------------------------------
        ' return value:
        ' true : alarm had been setted succfully
        ' false : alarm object had been occupied
        ' by Hsien , 2014/6/3
        '------------------------------------
        If Me.sw Then
            'if alarm had been setted , inpossible to set twice by Hsien , 2014/6/3
            ' implemented the mutex object for different control threads
            Return False
        End If


        Me.sw = eSwitch.eON
        Me.type = alarmType
        Me.code = alarmCode
        Me.ShouldBeOnOff = checkStatus
        Me.PossibleResponse = possibleResponse
        ' 
        Me.flagIsUserResponsed = False

        Return True
    End Function
    Public Function SetAlarm(ByVal alarmCode As Integer, ByVal checkStatus As eCheckStatus, ByVal alarmType As eErrType, Optional ByVal isRetryOnly As Boolean = True) As Boolean
        '------------------------------------
        ' return value:
        ' true : alarm had been setted succfully
        ' false : alarm object had been occupied
        ' by Hsien , 2014/6/3
        '------------------------------------
        If Me.sw Then
            'if alarm had been setted , inpossible to set twice by Hsien , 2014/6/3
            ' implemented the mutex object for different control threads
            Return False
        End If


        Me.sw = eSwitch.eON
        Me.type = alarmType
        Me.code = alarmCode
        Me.ShouldBeOnOff = checkStatus
        Me.RetryOnly = isRetryOnly

        ' motor error , restart system only
        'Me.PossibleResponse = eErrRetMode.ErrEndMode
        ' 
        Me.flagIsUserResponsed = False

        Return True
    End Function
    Public Function SetAlarm(ByVal alarmCode As Integer, ByVal checkStatus As eCheckStatus, ByVal alarmType As eErrType, ByVal motorName As String) As Boolean
        ' especially for motor alarm
        '------------------------------------
        ' return value:
        ' true : alarm had been setted succfully
        ' false : alarm object had been occupied
        ' by Hsien , 2014/6/3
        '------------------------------------
        If Me.sw Then
            'if alarm had been setted , inpossible to set twice by Hsien , 2014/6/3
            ' implemented the mutex object for different control threads
            Return False
        End If

        'If (flagIsUserResponsed) Then
        '    ' user response not handled 
        '    Return False
        'End If


        Me.sw = eSwitch.eON
        Me.type = alarmType
        Me.code = alarmCode
        Me.ShouldBeOnOff = checkStatus
        Me.RetryOnly = True
        Me.MotorName = motorName

        ' motor error , restart system only
        Me.PossibleResponse = errorReturnMode.ErrEndMode
        ' 
        Me.flagIsUserResponsed = False

        Return True
    End Function

    Public Sub alarmHandling()
        '------------------------------------
        ' the alarm handling state-machine
        '------------------------------------
        ' put in top-level of main thread , would call Check_Alarm every scanning 
        ' if alarm object triggered , would run alarm handling routine
        ' by Hsien , 2014/6/3
        '------------------------------------
        Select Case alarmHandlingState
            Case alarmHandlingStateEnum.OCCURED
                '------------------------------------
                ' alarm initializing
                ' by Hsien , 2014/6/3
                '------------------------------------
                flagIsUserResponsed = False
                'flagUserResponseIgnore = False
                'If (alarmOccuredHandler <> Nothing) Then
                '    alarmOccuredHandler()
                'End If
                RaiseEvent alarmOccured(Me, Nothing)

                alarmHandlingState = alarmHandlingStateEnum.WAIT_RESPONSE

            Case alarmHandlingStateEnum.WAIT_RESPONSE  '* 等待錯誤處理
                '------------------------------------
                ' alarm recovering
                ' by Hsien , 2014/6/3
                '------------------------------------
                'If (alarmWaitResponseHandler <> Nothing) Then
                '    alarmWaitResponseHandler()
                'End If

                RaiseEvent alarmWaitResponse(Me, Nothing)

                If flagIsUserResponsed Then
                    alarmHandlingState = alarmHandlingStateEnum.RELEASE
                End If

            Case alarmHandlingStateEnum.RELEASE  '* 檢視按鈕回應值
                '------------------------------------
                ' alarm reset
                ' by Hsien , 2014/6/3
                '------------------------------------
                Me.sw = eSwitch.eOFF

                alarmHandlingState = alarmHandlingStateEnum.OCCURED

                'If (alarmReleaseHandler <> Nothing) Then
                '    alarmReleaseHandler()
                'End If

                RaiseEvent alarmRelease(Me, Nothing)

        End Select

    End Sub

End Class



Public Class alarmEventArgs : Inherits EventArgs
    Property Content As alarmContextBase
    Sub New(ByVal __content As alarmContextBase)
        Me.Content = __content
    End Sub
End Class

Public Class alarmManager

    ' control interface
    ReadOnly Property IsAlarmed As Boolean
        ' for monitoring in control thread
        Get
            'Return Me.alarmHandlingState <> alarmHandlingStateEnum.LISTENING
            ' justify the definition of alarm : when alarm queue within content
            Return Me.__alarmQueue.Count <> 0 Or alarmHandlingState <> alarmHandlingStateEnum.LISTENING
        End Get
    End Property
    Property UserResponse As alarmContextBase.responseWays
        Get
            Return __userResponse   'expose to upper layer
        End Get
        ' for user return
        Set(ByVal value As alarmContextBase.responseWays)
            __userResponse = value
            flagIsUserResponsed = True
        End Set
    End Property
    ReadOnly Property AlarmQueue As ConcurrentQueue(Of alarmContextBase)
        Get
            Return __alarmQueue
        End Get
    End Property
    ReadOnly Property CurrentAlarm As alarmContextBase
        Get
            Return popedAlarmContext        'report current handling alarm , could be used to check which the owner is
        End Get
    End Property
    'event
    Public Event alarmOccured As EventHandler(Of alarmEventArgs)
    Public Event alarmWaitResponse As EventHandler(Of alarmEventArgs)
    Public Event alarmReleased As EventHandler(Of alarmEventArgs)

    ' internal
    Protected alarmHandlingState As alarmHandlingStateEnum = alarmHandlingStateEnum.LISTENING                    ' indicate the current alarm handling state ' by Hsien , 2014/6/3
    Protected __userResponse As [Enum] 'decoupled with alarmContextBase , Hsien , 2015.01.20'alarmContextBase.responseWays ' used to response by user
    Protected flagIsUserResponsed As Boolean
    'Protected __alarmQueue As Queue(Of alarmContextBase) = New Queue(Of alarmContextBase)
    Protected __alarmQueue As ConcurrentQueue(Of alarmContextBase) = New ConcurrentQueue(Of alarmContextBase)   'for multi-thread purpose , Hsien  ,2105.10.05
    Protected currentAlarmEventArgs As alarmEventArgs = New alarmEventArgs(New alarmContextBase)
    Protected popedAlarmContext As alarmContextBase = New alarmContextBase() With {.Sender = "DEFAULT"} 'the default current alarm pack

    Protected Enum alarmHandlingStateEnum
        LISTENING               ' sensing if any alarm object in queue
        OCCURED
        WAIT_RESPONSE
        RELEASING
    End Enum

#Region "public interfaces"
    'put in sequential control process ( the possible alarm happening point)
    Public Function raisingAlarm(ByVal __content As alarmContextBase) As Integer
        Me.__alarmQueue.Enqueue(__content)
        Return 0
    End Function
    'put in routine process
    Public Sub alarmHandling()
        '------------------------------------
        ' the alarm handling state-machine
        '------------------------------------
        ' put in top-level of main thread , would call Check_Alarm every scanning 
        ' if alarm object triggered , would run alarm handling routine
        ' by Hsien , 2014/6/3
        '------------------------------------
        Select Case alarmHandlingState
            Case alarmHandlingStateEnum.LISTENING
                If (__alarmQueue.Count <> 0 AndAlso __alarmQueue.TryDequeue(popedAlarmContext)) Then
                    '--------------------------------------------
                    '   Alarm sensed ,  state transit
                    '   poped the first alarm content to handling
                    '--------------------------------------------
                    alarmHandlingState = alarmHandlingStateEnum.OCCURED
                    'popedAlarmContext = __alarmQueue.Dequeue()
                    '---------
                    ' Prepare alarm event args
                    '---------
                    currentAlarmEventArgs.Content = popedAlarmContext
                End If
            Case alarmHandlingStateEnum.OCCURED
                '------------------------------------
                ' alarm initializing
                ' by Hsien , 2014/6/3
                '------------------------------------
                flagIsUserResponsed = False
                RaiseEvent alarmOccured(Me, currentAlarmEventArgs)       ' raising event with poped alarmContent

                alarmHandlingState = alarmHandlingStateEnum.WAIT_RESPONSE
            Case alarmHandlingStateEnum.WAIT_RESPONSE  '* 等待錯誤處理
                '------------------------------------
                ' alarm recovering
                ' by Hsien , 2014/6/3
                '------------------------------------
                RaiseEvent alarmWaitResponse(Me, currentAlarmEventArgs)   ' the heart-beat of alarm

                If flagIsUserResponsed Then
                    alarmHandlingState = alarmHandlingStateEnum.RELEASING
                End If

            Case alarmHandlingStateEnum.RELEASING  '* 檢視回應
                '------------------------------------
                ' alarm reset
                ' by Hsien , 2014/6/3
                '------------------------------------

                ' do custom responsing call back

                If (popedAlarmContext.CallbackResponse(Me.__userResponse).Invoke) Then
                    RaiseEvent alarmReleased(Me, currentAlarmEventArgs)
                    alarmHandlingState = alarmHandlingStateEnum.LISTENING     'rewind , the state transit should as last  , Hsien , 2015.01.06
                End If

        End Select

    End Sub
#End Region


End Class