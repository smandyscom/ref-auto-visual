﻿Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports Automation
Imports System.Threading
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports Automation.mainIOHardware
Imports Automation.Components.Services.genericCheckService

Public Class assemblyArch
    Inherits systemControlPrototype


    Public Event SystemStarted As EventHandler  'occured when stateIgnite executed
    Public Event SystemClosed As EventHandler   'occured when process returned

    Public Event SystemUp(ByVal sender As Object, ByVal e As EventArgs)
    Public Event SystemDown(ByVal sender As Object, ByVal e As EventArgs)

    Public Enum controlFlagsEnum
        ABLE_IGNITE         ' used to bridge with user-interface
        PAUSE_PRESSED       'Hsien , 2015.02.25
        IS_ABORT_SYSTEM
        IS_BUZZER_ON
    End Enum

    Property controlFlags As flagController(Of controlFlagsEnum) = New flagController(Of controlFlagsEnum)
    ReadOnly Property WorkingThreadState As ThreadState
        Get
            Return workingThread.ThreadState
        End Get
    End Property

    Protected workingThread As Thread = New Thread(AddressOf running) With {.Priority = ThreadPriority.AboveNormal,
                                                                          .Name = "Main Worker Thread"}

    Dim shutDownReason As closeEvent.closeReasonEnum = closeEvent.closeReasonEnum.NORMAL_SHUTDOWN
    ''' <summary>
    ''' Up : Non-pause/alarm
    ''' </summary>
    ''' <remarks></remarks>
    Dim isSystemUp As Boolean = False

#Region "control members"
    Protected emergencyStopSense As sensorCheckService = New sensorCheckService() With {.IsEnabled = True}
    Protected hardwareFailedSense As genericCheckService = New genericCheckService With {.IsEnabled = True}
    Protected basicLogger As logHandler = Nothing
    Protected alarmLogger As logHandler = Nothing 'used to filter-out alarm log only

    'hsien , 2015.04.03
    Protected buzzer As flipService = New flipService() With {.FlipGoal = New TimeSpan(0, 0, 1)}
    Protected redTowerLight As flipService = New flipService() With {.FlipGoal = New TimeSpan(0, 0, 1)}
    Protected yellowTowerLight As flipService = New flipService() With {.FlipGoal = New TimeSpan(0, 0, 1)}
    Protected greenTowerLight As flipService = New flipService() With {.FlipGoal = New TimeSpan(0, 0, 1)}
    Protected blueTowerLight As flipService = New flipService With {.FlipGoal = New TimeSpan(0, 0, 1)}

    Public doorInterlock As sensorCheckService = New sensorCheckService With {.IsEnabled = False}
    '-------------------------------------
    '   Air Sensing
    '-------------------------------------
    Public airMonitor As sensorCheckService = New sensorCheckService With {.IsEnabled = True} 'jk 2015.10.27 remove withevent to prevent fieldinfo could not find this object. using AddHandler in sub New()
    '-------------------------------------
    '   Pause Sensing
    '-------------------------------------
    Protected pauseButtons As List(Of sensorControl) = New List(Of sensorControl)
    Protected pauseLights As List(Of flipService) = New List(Of flipService)
#End Region

    Protected Overrides Function process() As Integer
        '--------------------
        ' the common control process for a machine assembly
        '--------------------
        Try
            RaiseEvent SystemStarted(Me, Nothing)

            While (Not Me.controlFlags.viewFlag(controlFlagsEnum.IS_ABORT_SYSTEM))
                '---------------
                '   System scanning time monitoring
                '---------------
                __cycleTime = cycleTimer.TimeElapsed
                cycleTimer.IsEnabled = True
                '-----------------
                '   Main procedure
                '-----------------
                drivesRunningInvoke()   'stations runs here
                If (Not PauseBlock.IsPaused And Not CentralAlarmObject.IsAlarmed) Then
                    stateControl()          'state control , used to ignite sub-systems
                    If Not isSystemUp Then
                        RaiseEvent SystemUp(Me, EventArgs.Empty)
                        isSystemUp = True
                    End If
                ElseIf isSystemUp Then
                    RaiseEvent SystemDown(Me, EventArgs.Empty)
                    isSystemUp = False
                End If
                processProgress()       'used to refresh gui
                CentralMessenger.messageHandling()
                CentralAlarmObject.alarmHandling()  'the central handling routine , Hsien , 2015.01.23
                '--------------------------
                '   Handling the pause event in assembly
                '--------------------------
                PauseBlock.pauseHandling()
                Thread.Yield()

            End While
        Catch ex As Exception
            '---------------------------------------------
            '   Catch every exception occured when working
            '---------------------------------------------
            sendMessage(exceptionEnum.PROGRAM_ERROR, "Message : " + ex.Message + ", StackTrace : " + ex.StackTrace)
            shutDownReason = closeEvent.closeReasonEnum.PROGRAM_ERROR_OCCURED
        End Try

        '--------------------------------
        '   Secondary Try-Catch Protection
        '--------------------------------
        Try
            sendMessage(statusEnum.SYSTEM_DOWN, shutDownReason.ToString())
            Me.shutdown()
        Catch ex As Exception
            '------------------------------------
            '   Error occured when shutdown
            '------------------------------------
            'Hsien , 2015.07.28 , use "using" to ensure logger would be closed
            Using tempLogger As New logHandler(".fatal")
                Dim fatalMessage As messagePackageEventArg = New messagePackageEventArg()
                fatalMessage.Message = New messagePackage(Me, exceptionEnum.PROGRAM_ERROR, Me.DeviceName + ".close()" + ex.Message)
                tempLogger.logMessageToFile(Nothing, fatalMessage)
            End Using

        Finally
            '-----------------------------------------------------------
            '   Whatever exception occured or not , will do this section
            '-----------------------------------------------------------
            basicLogger.Dispose()
            RaiseEvent SystemClosed(Me, New closeEvent(shutDownReason)) 'used to shutdown UI
        End Try

        Return 0

    End Function

#Region "initializing/Finalizing"
    Function closeDumpAllDriveStatusHeavy()
        '---------------------------------------------
        '   Dump All drive's public/property fields
        '----------------------------------------------
        'Dim __sb As StringBuilder = New StringBuilder
        'Me.forEach(Sub(__drive As driveBase)
        '               __sb.AppendLine(String.Format("{0},Dump Start", __drive.DeviceName))
        '               __drive.GetType.GetFields(Reflection.BindingFlags.Instance Or
        '                                         Reflection.BindingFlags.NonPublic Or
        '                                         BindingFlags.Public).All(Function(__field As FieldInfo) As Boolean
        '                                                                      Try
        '                                                                          __sb.AppendLine(String.Format("Field:{0};Value:{1}",
        '                                                                                                        __field.Name,
        '                                                                                                        __field.GetValue(__drive)))
        '                                                                      Catch ex As Exception
        '                                                                          __sb.AppendLine(String.Format("Field:{0};Cannot be shown", __field.Name))
        '                                                                      End Try
        '                                                                      Return True
        '                                                                  End Function)
        '               __drive.GetType.GetProperties(Reflection.BindingFlags.Instance Or
        '                  Reflection.BindingFlags.NonPublic Or
        '                  BindingFlags.Public).All(Function(__property As PropertyInfo) As Boolean
        '                                               Try
        '                                                   __sb.AppendLine(String.Format("Property:{0};Value:{1}",
        '                                                                                 __property.Name,
        '                                                                                 __property.GetValue(__drive, Nothing)))
        '                                               Catch ex As Exception
        '                                                   __sb.AppendLine(String.Format("Property:{0};Cannot be shown", __property.Name))
        '                                               End Try
        '                                               Return True
        '                                           End Function)

        '               __sb.AppendLine(String.Format("{0},Dump End", __drive.DeviceName))
        '               __sb.AppendLine()    'append empty line
        '           End Sub)

        sendMessage(internalEnum.GENERIC_MESSAGE, Me.ToString(Nothing))
        Return 0
    End Function
    Function closeDumAllDriveStatusLight()
        Return sendMessage(internalEnum.GENERIC_MESSAGE, Me.ToString("Light"))
    End Function
    Function closeDumpAlarms2Message()
        '----------------------------------
        '   Flush all alarmContext in subsystems and sendinto message bus
        '---------------------------------
        Dim __alarmContent As alarmContextBase = Nothing

        For Each subSystem As driveBase In actionComponents
            Dim systemReference As systemControlPrototype = TryCast(subSystem, systemControlPrototype)
            If (Not systemReference Is Nothing) Then
                If (subSystem.CentralAlarmObject.AlarmQueue.Count > 0 And
                    subSystem.CentralAlarmObject.AlarmQueue.TryDequeue(__alarmContent)) Then
                    'sendMessage(GetType(alarmGeneric), subSystem.CentralAlarmObject.AlarmQueue.Dequeue().ToString())
                    sendMessage(GetType(alarmGeneric), __alarmContent.ToString())   'Hsien , 2015.10.05
                End If
            End If
        Next
        Return 0
    End Function
    Function closeFlushAllMessages()
        'clear all messages inqueue
        While (CentralMessenger.MessageQueue.Count <> 0)
            CentralMessenger.messageHandling()
        End While
        Return 0
    End Function
    'Function closeSubsystems()
    '    'close systems
    '    For Each subSystem As driveBase In actionComponents
    '        Dim systemReference As systemControlPrototype = TryCast(subSystem, systemControlPrototype)
    '        If (Not systemReference Is Nothing) Then
    '            systemReference.close()
    '        End If
    '    Next
    '    Return 0
    'End Function
    '-------------------------------
    '   Common shutdown procedure:
    '   1. dump alarms to message queue
    '   2. flush all messages
    '   3' save settings
    '   4. close subsystems
    '-------------------------------
    Protected shutdown As Func(Of Integer) = New Func(Of Integer)(Function() (0))

    Public Overridable Function start() As Integer
        '------------
        ' fire thread
        '------------
        'Me.workingThread = New Thread(AddressOf Me.running)
        'Me.workingThread.Priority = ThreadPriority.Highest
        'Me.workingThread.Name = "Main Worker Thread"
        Me.workingThread.Start()
        sendMessage(internalEnum.VERSION, My.Application.Info.Version.ToString())   'Hsien , 2014.10.29
        Return 0
    End Function
    ''---------------------------------------------------------
    ''link initializing routines
    ''0. link pause blocks , central messenger , central alarm , settings
    ''1. link subSystem initialize
    ''2. mapping and setup drives
    ''3. load and apply settings (emergency stop)
    ''--------------------------------------------------------
    'Protected Function initSubsystemInitialize() As Integer
    '    For Each subSystem As driveBase In actionComponents
    '        Dim systemReference As systemControlPrototype = TryCast(subSystem, systemControlPrototype)
    '        If (Not systemReference Is Nothing) Then
    '            systemReference.initialize()    ' initializing each sub-system
    '        End If
    '    Next
    '    Return 0
    'End Function
    Protected Function loadSetting(ByRef __settings As settingBase, __settingName As String) As Integer
        '-------------------
        '   the routine to load setting
        '   1. load setting from file (deserialze)
        '   2. applyPropertyChange (raising events)
        '-------------------
        Dim fi As FileInfo = New FileInfo(My.Application.Info.DirectoryPath + "\Data\" + __settingName)

        If (fi.Exists) Then
            __settings.Load(fi.FullName)
        Else
            sendMessage(internalEnum.GENERIC_MESSAGE, fi.FullName & ": Missing")
            __settings.Create(fi.FullName)
            __settings.Load(fi.FullName)
        End If

        __settings.applyPropertyChange()            'apply on settings

        Return 0
    End Function

#End Region

    Sub New()
        'the dummy state function , prevent not-implemented exception
        systemMainState = systemStatesEnum.IGNITE
        systemMainStateFunctions(systemStatesEnum.IGNITE) = Function() (0)

        CentralAlarmObject = New alarmManager
        CentralMessenger = New messageHandler
        PauseBlock = New interceptObject

        'basicLogger = New logHandler(".log") With {.IsAsynchronWriteLog = True}
        'basicLogger.MessengerReference = CentralMessenger

        controlFlags.setFlag(controlFlagsEnum.IS_BUZZER_ON)

        alarmContextBase.abortMethod = Function() (Me.controlFlags.setFlag(assemblyArch.controlFlagsEnum.IS_ABORT_SYSTEM) = 0)  'Hsien , 2015.10.12 , the default abort system method
        AddHandler airMonitor.CheckFailed, AddressOf airAbnormal 'jk 2015.10.27 addhandler here
        AddHandler emergencyStopSense.CheckFailed, AddressOf emergencySensed
        hardwareFailedSense.ObjectConditionsNeedToCheck.Add(New genericCheckCondition With {.Sender = Me,
                                                                                            .Condition = AddressOf hardwareCheckCondition})
        AddHandler hardwareFailedSense.CheckFailed, AddressOf hardwareFailedSensed

        'Hsien , default allocating
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initAllocateLogger))

        IsEnabled = True
    End Sub


#Region "alarm/pause handlers"


    Overridable Sub alarmOccuredHandler(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured
        '-------------------------
        '   Cancel the green light
        '-------------------------
        writeBit(greenTowerLight.OutputBit, False)

        sendMessage(GetType(alarmGeneric), e.Content.ToString())    'message send
    End Sub
    Overridable Sub alarmWaitResponseHandler(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmWaitResponse
        '------------------
        '   Buzzer
        '------------------
        If (Me.buzzer.IsEnabled <> Me.controlFlags.viewFlag(controlFlagsEnum.IS_BUZZER_ON)) Then
            Me.buzzer.FlipGoal = New TimeSpan(0, 0, 0, 1)  'Hsien , 2015.07.15
            Me.buzzer.IsEnabled = Me.controlFlags.viewFlag(controlFlagsEnum.IS_BUZZER_ON)
        End If
        '-------------------
        '   Lights
        '-------------------
        If (Not redTowerLight.IsEnabled) Then
            redTowerLight.IsEnabled = True
        End If

    End Sub
    Overridable Sub alarmReleaseHandler(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmReleased
        buzzer.IsEnabled = False
        redTowerLight.IsEnabled = False
        writeBit(greenTowerLight.OutputBit, True)

        sendMessage(GetType(alarmGeneric), String.Format("{0} Selected", sender.UserResponse.ToString))     'hsien , 2015.10.05, User response logged
    End Sub

    Overridable Function pauseSense() As Boolean
        Dim isAnyButtonPressed As Boolean =
            pauseButtons.Exists(Function(__button As sensorControl) (__button.PulseCount > 0 And __button.OnPulseWidth.TotalMilliseconds > 50))

        pauseButtons.ForEach(Sub(__button As sensorControl) __button.PulseCount = 0)

        Dim doorCondition As Boolean = (doorInterlock.IsAllConditionPassed Or Not doorInterlock.IsEnabled)


        Return controlFlags.readFlag(controlFlagsEnum.PAUSE_PRESSED) OrElse
            Not doorCondition OrElse
            isAnyButtonPressed
    End Function
    Overridable Function unpauseSense() As Boolean
        Dim isAnyButtonPressed As Boolean =
            pauseButtons.Exists(Function(__button As sensorControl) (__button.PulseCount > 0 And __button.OnPulseWidth.TotalMilliseconds > 50))

        pauseButtons.ForEach(Sub(__button As sensorControl) __button.PulseCount = 0)


        Dim doorCondition As Boolean = (doorInterlock.IsAllConditionPassed Or Not doorInterlock.IsEnabled)

        If (Not doorCondition) Then
            Dim __theDoorNotClosed As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType,
                                                              doorInterlock.SensorsNeedToCheck.Find(Function(__sensor As sensorCheckService.sensorCheckCondition) (Not __sensor.IsConditionPassed)).SensorBit)
            sendMessageTimed(statusEnum.GENERIC_MESSAGE, String.Format(My.Resources.DOOR_NOT_CLOSED,
                                                                       __theDoorNotClosed.ToString))
        End If



        Return (controlFlags.readFlag(controlFlagsEnum.PAUSE_PRESSED) Or isAnyButtonPressed) AndAlso
            doorCondition

    End Function
    Overridable Sub pauseHandler() Handles PauseBlock.InterceptedEvent
        '---------------------
        '   Flash yellow light
        '---------------------
        If (Not yellowTowerLight.IsEnabled) Then
            yellowTowerLight.IsEnabled = True
        End If

        pauseLights.ForEach(Sub(__light As flipService) writeBit(__light.OutputBit, True))

        sendMessage(statusEnum.GENERIC_MESSAGE, My.Resources.SYSTEM_PAUSE)

    End Sub
    Overridable Sub unpauseHandler() Handles PauseBlock.UninterceptedEvent
        '---------------------
        '   Stop yellow light
        '---------------------
        yellowTowerLight.IsEnabled = False

        pauseLights.ForEach(Sub(__light As flipService) writeBit(__light.OutputBit, False))

        sendMessage(statusEnum.GENERIC_MESSAGE, My.Resources.SYSTEM_RESUME)

    End Sub

    Sub airAbnormal(sender As Object, e As sensorCheckService.sensorCheckEventArgs) 'Handles airMonitor.CheckFailed 'jk 2015.10.27 remove this handles, use addhandler later
        If (Not CentralAlarmObject.IsAlarmed) Then
            Dim ap As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                       .Inputs = e.Content.SensorBit,
                                                                       .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                                                        .AdditionalInfo = My.Resources.AIR_ABNORMAL}
            CentralAlarmObject.raisingAlarm(ap)
        End If
    End Sub
    Sub emergencySensed(sender As Object, e As sensorCheckService.sensorCheckEventArgs)
        Me.controlFlags.setFlag(controlFlagsEnum.IS_ABORT_SYSTEM)
        shutDownReason = closeEvent.closeReasonEnum.EMERGENCY_STOP
    End Sub
    Function hardwareCheckCondition() As Boolean
        Return mainIOHardware.Instance.PhysicalHardwareList.TrueForAll(Function(__hardware As subHardwareNode) __hardware.Status = hardwareStatusEnum.HEALTHY)
    End Function
    Sub hardwareFailedSensed(sender As Object, e As genericCheckEventArgs)
        If (Not CentralAlarmObject.IsAlarmed) Then

            Dim __failedHardware As subHardwareNode =
                mainIOHardware.Instance.PhysicalHardwareList.Find(Function(__hardware As subHardwareNode) __hardware.Status <> hardwareStatusEnum.HEALTHY)

            Dim ap As alarmContextBase = New alarmContextBase
            With ap
                .Sender = Me
                .PossibleResponse = alarmContextBase.responseWays.RETRY Or alarmContextBase.responseWays.ABORT
                .AdditionalInfo = String.Format(My.Resources.HARDWARE_FAILED, __failedHardware.HardwareName)
                CentralAlarmObject.raisingAlarm(ap)
            End With

        End If
    End Sub

#End Region

    ''' <summary>
    ''' make it possible to override logger initilaizing procedure
    ''' </summary>
    ''' <remarks></remarks>
    Function initAllocateLogger() As Integer

        If (Not FileIO.FileSystem.FileExists(logHandler.TestLogFilePath(".log"))) Then
            basicLogger = New logHandler(".log") With {.IsAsynchronWriteLog = True}
        Else
            basicLogger = New logHandler(utilities.getFullParentName(Me) & ".log") With {.IsAsynchronWriteLog = True}
        End If
        alarmLogger = New logHandler(".alarm.log") With {.IsAsynchronWriteLog = True}
        alarmLogger.ContentFilter = Function(sender As Object, e As messagePackageEventArg) (e.Message.PrimaryKey.Equals(GetType(alarmGeneric)))

        basicLogger.MessengerReference = CentralMessenger

        With alarmLogger
            .MessengerReference = CentralMessenger
        End With

        Return 0
    End Function

End Class

Public Class closeEvent : Inherits EventArgs
    Public Enum closeReasonEnum
        EMERGENCY_STOP
        PROGRAM_ERROR_OCCURED
        NORMAL_SHUTDOWN
    End Enum

    Property Reason As closeReasonEnum = closeReasonEnum.NORMAL_SHUTDOWN

    Public Sub New(ByVal arg As closeReasonEnum)
        Reason = arg
    End Sub

    Public Overrides Function ToString() As String
        Return Reason.ToString()
    End Function

End Class




