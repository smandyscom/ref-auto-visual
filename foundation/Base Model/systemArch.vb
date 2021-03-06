﻿Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation.Components
Imports System.Threading
Imports System.Reflection
Imports System.Text

Partial Public MustInherit Class systemControlPrototype : Inherits driveBase

    Public Enum systemStatesEnum
        IGNITE
        IDLE
        PREPARE
        EXECUTE
        ABORT
        ALARM_RECOVER
        STATE_TOTAL_NUM
    End Enum

    Public Event ProcessProgressed As EventHandler

    Property MessengerTimeInterval As TimeSpan
        Get
            Return messageTimer.TimerGoal
        End Get
        Set(value As TimeSpan)
            messageTimer.TimerGoal = value
        End Set
    End Property
    Property ProcessProgreeedTimeInterval As TimeSpan
        Get
            Return processProgressedTimer.TimerGoal
        End Get
        Set(value As TimeSpan)
            processProgressedTimer.TimerGoal = value
        End Set
    End Property

#Region "monitor interface to GUI"
    Overridable ReadOnly Property MainState As systemStatesEnum
        ' return system main state
        Get
            Return systemMainState
        End Get
    End Property
    ReadOnly Property SubState As Object
        Get
            Return systemSubState
        End Get
    End Property
    ReadOnly Property IsAlarmed As Boolean
        Get
            Return CentralAlarmObject.IsAlarmed
        End Get
    End Property
    ReadOnly Property ComponentsIncluded As List(Of driveBase)
        Get
            Return actionComponents
        End Get
    End Property
    ReadOnly Property CycleTime As TimeSpan
        Get
            Return __cycleTime
        End Get
    End Property
    ReadOnly Property AlarmCount As Integer
        Get
            Return CentralAlarmObject.AlarmQueue.Count
        End Get
    End Property
    ReadOnly Property MessageCount As Integer
        Get
            Return CentralMessenger.MessageQueue.Count
        End Get
    End Property
#End Region

    ' initializing functions
    Public initialize As Func(Of Integer) = New Func(Of Integer)(Function() (0))
    'internal- state control
    Protected systemMainStateFunctions As Func(Of Integer)() = New Func(Of Integer)(systemStatesEnum.STATE_TOTAL_NUM) {}
    Protected systemMainState As systemStatesEnum = systemStatesEnum.IGNITE
    Protected systemSubState As Integer = 0
    ' internal - alarm handling control
    Protected systemPreviousMainState As systemStatesEnum = systemStatesEnum.IGNITE
    ' to store all the timerDrive , for iterating executing
    Protected actionComponents As List(Of driveBase) = New List(Of driveBase)
    Protected drivesRunningInvoke As Func(Of Integer) = Function() (0)
    ' the invoke list of drive.running()

    Protected __cycleTime As TimeSpan       ' as snapshot

    ' internal , message timer
    Protected messageTimer As singleTimer = New singleTimer() With {.TimerGoal = New TimeSpan(0, 0, 10),
                                                                    .IsEnabled = True}
    Protected processProgressedTimer As ringTimer = New ringTimer() With {.TimerGoal = New TimeSpan(0, 0, 0, 0, 100),
                                                                          .IsEnabled = True}
    Protected cycleTimer As singleTimer = New singleTimer()

    Public WithEvents PauseBlock As interceptObject = New interceptObject   'recovered hsien , 2015.02.24

#Region "frequecy used sending functions"
    Protected Sub sendAlarmSensor(ByVal input As Integer, ByVal reason As alarmContentSensor.alarmReasonSensor, Optional description As String = "")
        'If (Not CentralAlarmObject.IsAlarmed) Then                      'Hsien , Prevent overrun , 2014.11.24
        CentralAlarmObject.raisingAlarm(New alarmContentSensor() With {.Sender = Me,
                                                                      .PossibleResponse = alarmContextBase.responseWays.RETRY,
                                                                      .Reason = reason,
                                                                      .Inputs = input,
                                                                      .AdditionalInfo = description})
        'End If
    End Sub
    Public Function sendMessage(ByVal key As Object, Optional ByVal __additionalInfomation As String = "") As Integer
        ' 1. get the specific enum object
        ' 2. use object above to query data base , get pattern
        ' 3. use this pattern to general final string , according input argument
        ' 4. queue in message queue
        CentralMessenger.MessageQueue.Enqueue(New messagePackage(Me, key, __additionalInfomation))
        Return 0
    End Function
    Public Function sendMessageTimed(ByVal key As Object, Optional ByVal __additionalInfomation As String = "") As Integer
        If (Not messageTimer.IsEnabled Or messageTimer.IsTimerTicked) Then
            ' first time to send , or reset timer
            messageTimer.IsEnabled = True
            ' send
            sendMessage(key, __additionalInfomation)
        End If

        Return 0
    End Function
#End Region

    Protected Function stateControl() As Integer
        Try
            systemPreviousMainState = systemMainState

            systemMainStateFunctions(systemMainState)()

            'state transit , rewind substate ( NOTE: state-transit happened only when NO-ALARM OCCURED in flow
            If (systemMainState <> systemPreviousMainState) Then
                systemSubState = 0
            End If

        Catch ex As Exception

            Throw New Exception(Me.DeviceName & ", stateControl()" & ex.Message & systemMainState & systemSubState & ex.StackTrace, ex)  'Hsien , 2014.10.28 , reflect exception

        End Try

        Return 0

    End Function
    Protected Friend Function processProgress() As Integer
        '----------------------
        '   Used to trigger GUI, logger
        '----------------------

        If (processProgressedTimer.IsTimerTicked) Then
            RaiseEvent ProcessProgressed(Me, Nothing)
            processProgressedTimer.IsEnabled = True    'restart timer , Hsien , 2015.07.29
        End If

        Return 0
    End Function
    Protected Overrides Function process() As Integer
        '----------------------------
        'standard system control flow
        '----------------------------

        drivesRunningInvoke()

        If (CentralAlarmObject.IsAlarmed OrElse
            PauseBlock.IsPaused) Then
            Return 0
        End If

        stateControl()
        processProgress()

        Return 0
    End Function


    Sub New()
        ' generating default initializing invoke list
        Me.initialize = [Delegate].Combine(New Func(Of Integer)(AddressOf Me.initAddAllDrive),
                                           New Func(Of Integer)(AddressOf Me.initLinkAlarm),
                                           New Func(Of Integer)(AddressOf Me.initLinkMessenger))
    End Sub

#Region "initializing templates"
    Public Function initAddAllDrive() As Integer

        ' get field infos of the Form1-Class
        Dim fieldInfos As FieldInfo() = Me.GetType().GetFields(System.Reflection.BindingFlags.Instance Or
                                                               BindingFlags.NonPublic Or
                                                               BindingFlags.Public)
        'find-out those field with "WithEvents"
        Dim propertyInfos As PropertyInfo() = Me.GetType.GetProperties(BindingFlags.Instance Or
                                                                        BindingFlags.NonPublic Or
                                                                         BindingFlags.Public)
        'clear all things , Hsien , so that able to restart system
        actionComponents.Clear()
        drivesRunningInvoke = [Delegate].RemoveAll(drivesRunningInvoke, drivesRunningInvoke)

        ' iterating fields to grab out fields from specific type(MyTestClass)
        ' 1. marked the name
        ' 2. and added into list
        For Each eachField As FieldInfo In fieldInfos

            Dim instanceHandle As driveBase = TryCast(eachField.GetValue(Me), driveBase)
            If (instanceHandle IsNot Nothing AndAlso
                instanceHandle.Parent Is Nothing) Then
                'make sure this device had no parent (not been added by others yet
                instanceHandle.DeviceName = eachField.Name
                actionComponents.Add(instanceHandle)

                instanceHandle.Parent = Me  'Hsien , 2015.04.04
                ' added invoke list
                drivesRunningInvoke = [Delegate].Combine(drivesRunningInvoke, New Func(Of Integer)(AddressOf instanceHandle.running))
            End If

        Next
        For Each item As PropertyInfo In propertyInfos
            If item.GetIndexParameters.Length = 0 AndAlso
                item.CanRead AndAlso
                item.CanWrite AndAlso
                item.PropertyType.IsSubclassOf(GetType(driveBase)) Then
                'for those readonly property

                Dim instanceHandle As driveBase = TryCast(item.GetValue(Me, Nothing), driveBase)
                If instanceHandle IsNot Nothing AndAlso
                    instanceHandle.Parent Is Nothing Then
                    'make sure this device had no parent (not been added by others yet
                    instanceHandle.DeviceName = item.Name
                    actionComponents.Add(instanceHandle)

                    instanceHandle.Parent = Me  'Hsien , 2015.04.04
                    ' added invoke list
                    drivesRunningInvoke = [Delegate].Combine(drivesRunningInvoke, New Func(Of Integer)(AddressOf instanceHandle.running))
                End If
            End If
        Next


        'done
        Return 0
    End Function
    Public Function initLinkAlarm() As Integer
        For Each drive As driveBase In actionComponents
            drive.CentralAlarmObject = Me.CentralAlarmObject
        Next
        Return 0
    End Function
    Public Function initLinkMessenger() As Integer
        For Each drive As driveBase In actionComponents
            drive.CentralMessenger = Me.CentralMessenger
        Next
        Return 0
    End Function
    Public Function initLinkPause() As Integer
        'Hsien , 2015.02.24
        For Each subSystem As driveBase In actionComponents
            Dim systemReference As systemControlPrototype = TryCast(subSystem, systemControlPrototype)
            If (Not systemReference Is Nothing) Then
                systemReference.PauseBlock = Me.PauseBlock    ' link child systems pause blocks
            End If
        Next
        Return 0
    End Function
    Protected Function initEnableAllDrives() As Integer
        'enable all drive
        For Each drive As driveBase In actionComponents
            drive.IsEnabled = True
        Next
        Return 0
    End Function
    Protected Function initDisableAllDrives() As Integer
        'disable all drive
        For Each drive As driveBase In actionComponents
            drive.IsEnabled = False
        Next
        Return 0
    End Function
    ''---------------------------------------------------------
    ''link initializing routines
    ''0. link pause blocks , central messenger , central alarm , settings
    ''1. link subSystem initialize
    ''2. mapping and setup drives
    ''3. load and apply settings (emergency stop)
    ''--------------------------------------------------------
    Protected Function initSubsystemInitialize() As Integer
        For Each subSystem As driveBase In actionComponents
            Dim systemReference As systemControlPrototype = TryCast(subSystem, systemControlPrototype)
            If (Not systemReference Is Nothing) Then
                systemReference.initialize()    ' initializing each sub-system
            End If
        Next
        Return 0
    End Function
#End Region


    Public Overrides Function raisingGUI() As Control
        Dim uc As userControlSystem = New userControlSystem()
        uc.SystemReference = Me
        Return uc
    End Function

    '----------------------------------------
    '   Hierarachy Functions
    '----------------------------------------
    Sub forEach(__action As Action(Of driveBase))

        'traverse all childs
        actionComponents.ForEach(Sub(__drive As driveBase)
                                     __action(__drive)  'first-order call back
                                     Dim __system As systemControlPrototype = TryCast(__drive, systemControlPrototype)
                                     If (__system IsNot Nothing) Then
                                         __system.forEach(__action) 'second-order call back
                                     End If
                                 End Sub)

    End Sub

    Public Overrides Function ToString(arg As Object) As String
        'append my string and iterate all childs

        Dim __sb As StringBuilder = New StringBuilder

        If (arg IsNot Nothing AndAlso
           arg.ToString = "Light") Then
            '--------------------------------------------------------
            '   Light Dump , dump systemStates only
            '---------------------------------------------------------
            __sb.AppendLine(String.Format("Station Name:{0} ,  Light Dump Start", utilities.getFullParentName(Me)))
            __sb.AppendLine(String.Format("systemMainState , {0};systemSubState , {1}",
                                          MainState,
                                          SubState))
            Me.forEach(Sub(__drive As driveBase)
                           Dim __system As systemControlPrototype = TryCast(__drive, systemControlPrototype)
                           If (__system IsNot Nothing) Then
                               __sb.AppendLine(__system.ToString("Light"))
                           End If
                       End Sub)
            __sb.AppendLine(String.Format("Light Dump End"))
        Else
            '--------------------------------------------------------
            '   Heavy Dump
            '---------------------------------------------------------
            __sb.AppendLine(MyBase.ToString(Nothing)) 'append myself
            Me.forEach(Sub(__drive As driveBase) __sb.AppendLine(__drive.ToString(Nothing)))
        End If

        Return __sb.ToString
    End Function
#Region "Warning control"
    Property listWarning As New List(Of warningMessagePackage)
    Public Function sendWarningMessage(ByVal warningMessage As warningMessagePackage) As Integer
        If listWarning.Contains(warningMessage) = True Then Return 0
        warningMessage.Sender = Me
        warningMessage.PrimaryKey = statusEnum.WARNING_MESSAGE_ADD
        CentralMessenger.MessageQueue.Enqueue(warningMessage)
        listWarning.Add(warningMessage)
        Return 0
    End Function
    Public Function removeWarningMessage(ByVal warningMessage As warningMessagePackage) As Integer
        If listWarning.Contains(warningMessage) = False Then Return 0
        warningMessage.Sender = Me
        warningMessage.PrimaryKey = statusEnum.WARNING_MESSAGE_REMOVE
        CentralMessenger.MessageQueue.Enqueue(warningMessage)
        listWarning.Remove(warningMessage)
        Return 0
    End Function
    Public Function removeAllWarningMessage() As Integer
        For Each w As warningMessagePackage In listWarning
            w.PrimaryKey = statusEnum.WARNING_MESSAGE_REMOVE
            CentralMessenger.MessageQueue.Enqueue(w)
        Next
        listWarning.Clear()
        Return 0
    End Function
#End Region


    ''' <summary>
    ''' Shut all physical devices , e.g cylinder/motor
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub isEnableAllPhysical(node As systemControlPrototype, isEnable As Boolean)

        Dim __shutableDevices As List(Of Type) = New List(Of Type) From {GetType(cylinderControlBase),
                                                                         GetType(motorControl)}

        'shut those cylinder/motor
        'do further searching if system control prototype
        For Each item As driveBase In node.actionComponents
            If __shutableDevices.Exists(Function(__type As Type) item.GetType.IsSubclassOf(__type) Or item.GetType.Equals(__type)) Then
                item.IsEnabled = isEnable
            End If
            'do further recursive
            If item.GetType.IsSubclassOf(GetType(systemControlPrototype)) Then
                isEnableAllPhysical(item, isEnable)
            End If
        Next


    End Sub

End Class
