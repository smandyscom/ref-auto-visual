﻿Imports Automation
Imports System.IO
Imports Automation.Components.Services
Imports Automation.Components.CommandStateMachine
Imports System.Text.RegularExpressions

Public Class formMain

    Dim WithEvents messengerReference As messageHandler = Assembly.Instance.CentralMessenger
    Dim WithEvents pauseBlockReference As interceptObject = Assembly.Instance.PauseBlock
    Dim WithEvents alarmManagerReference As alarmManager = Assembly.Instance.CentralAlarmObject  'ready link to __assembly

    Private historyConsoleMaxLines As Integer = 64

    Dim __sensorAreaDatabase As IMessageQuery = New sensorAreaDatabase    'Hsien , used to query sensor area
    Dim __sensorDetailDatabase As sensorDetailDatabase = New sensorDetailDatabase

    Dim usercontrolCogToolBlock As Cognex.VisionPro.ToolBlock.CogToolBlockEditV2

    Private Sub loadMain(sender As Object, e As EventArgs) Handles MyBase.Load

        System.Diagnostics.Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.High  'set me as highest priority

        Dim di As DirectoryInfo = New DirectoryInfo(My.Application.Info.DirectoryPath + "\Data\")
        If (Not di.Exists) Then
            '--------------
            '   Once directory not existed , created one
            '--------------
            di.Create()
        End If

        '------------------------------------------------------
        '   Show the version info , and write into version file
        '------------------------------------------------------
        Me.Text = utilities.StandardTitle
        '------------------------------
        '   Initializing Message Databse
        '------------------------------
        messagePackage.QueryInterface = Nothing
        alarmContentMotor.MotorEnumType = GetType(motorAddress)
        alarmContextBase.QueryInterface = __sensorDetailDatabase
        alarmContentSensor.InputsEnumType = GetType(inputAddress)

        Dim daqNode As moduleDAQ = New moduleDAQ
        With daqNode
            .DeviceList.Add(New moduleInstantAIAO With {.DeviceNumber = 1}) '1720 AO control
            .DeviceList.Add(New moduleInstantAIAO With {.DeviceNumber = 2}) '4716 AI/AO control
        End With

        mainIOHardware.Instance.Load(Nothing)   'load the configuration
        mainIOHardware.Instance.PhysicalHardwareList.Add(New mainIOHardware.subHardwareNode() With {.PhysicalHardware = daqNode})

        mainIOHardware.__initialize()   'initialize io hardware
        mainIOHardware.scanBit(GetType(inputAddress))   'make sure if all address valid
        mainIOHardware.scanBit(GetType(outputAddress))
        '------------------------------------
        '   Linking
        '------------------------------------
        Assembly.Instance.initialize()         ' initializing __aseembly data structure
        AddHandler imageProcessSettingBlock.CameraTriggered, AddressOf linkImageMonitor

        With UserControlMainPanel1
            .AssemblyReference = Assembly.Instance
        End With
        '------------------------
        '   Prepare mainUserPanel
        '------------------------
        TabPageEnginner.Controls.Add(Assembly.Instance.raisingGUI())       'the baisc gui - engineer mode
        Try
            usercontrolCogToolBlock = New Cognex.VisionPro.ToolBlock.CogToolBlockEditV2
            TabPageFirer.Controls.Add(usercontrolCogToolBlock)
        Catch ex As Exception

        End Try

        'message bus initialize
        With userControlMessageHistory
            .messengerReference = Assembly.Instance.CentralMessenger
            .IsValidToShow = userControlMessage.generateMessageFilters({AddressOf .isNonRedundantMessage})
        End With
        With userControlAliveBarMain
            .assemblyReference = Assembly.Instance
        End With
        With UserControl_LightControl1
            .refLightControl = lightControl.Instance
        End With
        With UserControlFrameManagers1
            .AssemblyReference = Assembly.Instance
        End With
        '----------------------------------------------------
        '   After all link established  , then fire the timer
        '----------------------------------------------------
        '--------------------------------
        '   Preset alarm pop
        '--------------------------------
        With __alarmPop
            .buzzerFlagReference = Assembly.Instance.controlFlags
        End With
        With __dialog
            .Controls.Add(__alarmPop)
            .AutoSize = True
            .ControlBox = False
            .Text = "Error Message"
        End With
        '--------------------------------
        '   Preset Tracking monitor
        '--------------------------------
        Assembly.Instance.start()


        TimerRefresh.Enabled = True
    End Sub

    Private Sub uiAliveRefresh(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        ' ui refresh engine
        TextBoxTime.Text = Date.Now().ToString("G")
    End Sub

    Private Sub lockSettingHandler(ByVal sender As Object, ByVal e As EventArgs) Handles pauseBlockReference.InterceptedEvent
        Me.BeginInvoke(Sub() Me.userControlSettingMain.Enabled = True)
    End Sub
    Private Sub unlockSettingHandler(ByVal sender As Object, ByVal e As EventArgs) Handles pauseBlockReference.UninterceptedEvent
        Me.BeginInvoke(Sub() Me.userControlSettingMain.Enabled = False)
    End Sub

    Private Function loginRoutine() As DialogResult

        ' login stage
        Dim currentUser As formLogin.userData = Nothing
        Dim formLogin As formLogin = New formLogin()
        Dim result As DialogResult = Windows.Forms.DialogResult.OK

        While (currentUser Is Nothing)
            result = formLogin.ShowDialog()
            If (result = Windows.Forms.DialogResult.OK) Then
                currentUser = formLogin.currentUser
            Else
                result = Windows.Forms.DialogResult.Cancel
                Exit While
            End If
        End While

        If (result = Windows.Forms.DialogResult.Cancel) Then
            '--------------
            '   Cancel login
            '--------------
            Return Windows.Forms.DialogResult.Cancel
        End If

        ' remove all tab-page , then added them by access level
        MainTabControl.TabPages.Clear()

        If (currentUser.level And formLogin.accessLevelEnum.END_USER) Then
            With UserControlMainPanel1
            End With
            MainTabControl.TabPages.Add(Me.TabPageMainPanel)
            MainTabControl.TabPages.Add(Me.TabPageHistory)
        End If

        If (currentUser.level And formLogin.accessLevelEnum.SERVICE) Then
            MainTabControl.TabPages.Add(Me.TabPageSetting)
            '-------------------------------------------------------------------------------------------------------
            'determine the condition to lock setting page , 
            'i.e AddHandler ascSystem.loaderMainControl.PauseBlock.interceptedEvent, AddressOf Me.lockSettingHandler
            '-------------------------------------------------------------------------------------------------------
        End If

        If (currentUser.level And formLogin.accessLevelEnum.DEVELOPE) Then
            With UserControlMainPanel1
            End With
            MainTabControl.TabPages.Add(TabPageEnginner)
        End If
        ''------------------
        ''   Apply languege
        ''------------------
        utilitiesUI.applyResourceOnAllControls(Me.Controls, New System.ComponentModel.ComponentResourceManager(Me.GetType()))
        __sensorDetailDatabase.swapDataBase()
        Assembly.Instance.sendMessage(internalEnum.LOGIN, currentUser.Account)

        Return Windows.Forms.DialogResult.OK
    End Function

    Private Sub ClickRelogin(sender As Object, e As EventArgs)
        loginRoutine()
    End Sub


    '----------------------------------------------
    '   Alarm handling pop-up
    ''---------------------------------------------
    Dim isAlarmDialogPoped As Boolean = False
    Dim __alarmPop As userControlAlarmPop = New userControlAlarmPop
    Dim __dialog As Form = New Form
    Dim __result As IAsyncResult = Nothing


    Private Sub alarmPopup(ByVal sender As alarmManager, ByVal e As alarmEventArgs) Handles alarmManagerReference.alarmWaitResponse

        If (Not isAlarmDialogPoped) Then
            isAlarmDialogPoped = True   'bit lock

            __alarmPop.AlarmReference = sender

            __result = Me.BeginInvoke(Sub()
                                          With __dialog
                                              .ShowDialog()
                                          End With
                                          Me.EndInvoke(__result)
                                      End Sub)
        End If
    End Sub
    Private Sub alarmDialogClosed() Handles alarmManagerReference.alarmReleased
        isAlarmDialogPoped = False  'bit release
    End Sub
    Private Sub LogInToolStripMenuItem_Click(sender As Object, e As EventArgs)
        loginRoutine()
    End Sub

    Sub linkImageMonitor(sender As Object, e As imageProcessTriggerEventArgs)
        Me.Invoke(Sub()
                      If e.ToolBlock IsNot Nothing And
                                                     usercontrolCogToolBlock IsNot Nothing AndAlso
                                                     (usercontrolCogToolBlock.Subject Is Nothing OrElse Not usercontrolCogToolBlock.Subject.Equals(e.ToolBlock)) Then
                          'avoid redundant linkin
                          usercontrolCogToolBlock.Subject = e.ToolBlock
                      End If
                  End Sub)
    End Sub


End Class
