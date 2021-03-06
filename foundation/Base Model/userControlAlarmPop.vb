﻿Imports Automation
Imports System.Text
Imports Automation.Components.Services
Imports System.Linq
Imports Automation.Components.CommandStateMachine
Imports System.ComponentModel
Imports Automation.alarmInterpreters

Public Class userControlAlarmPop
    'pop-up style alarm information
    Property AlarmReference As alarmManager
        Get
            Return __alarmReference
        End Get
        Set(value As alarmManager)
            If (value IsNot Nothing) Then
                __alarmReference = value
            End If
        End Set
    End Property

    Public areaMapReference As ISensorAreaVisiable = Nothing 'reference to the presentation
    Public interpreterReference As alarmInterpreterPrototype = alarmInterpreters.generateInterpretor({New alarmInterpreterPrototype(AddressOf alarmConveyorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmSensorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmMotorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmDefaultInterpretor)})
    Public buzzerFlagReference As flagController(Of assemblyArch.controlFlagsEnum)    'linked to Control flag

    'Public cassetteAction As Action(Of alarmContextBase) = Sub() Math.Ceiling(1)

    Dim WithEvents __alarmReference As alarmManager = Nothing
    Dim __reasonDictionary As Dictionary(Of alarmContentSensor.alarmReasonSensor, String) = New Dictionary(Of alarmContentSensor.alarmReasonSensor, String)

    Private Sub loadAlarm(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ButtonIgnore.Enabled = False
        ButtonRetry.Enabled = False
        buttonOption3.Enabled = False
        checkBoxBuzzer.Checked = True

        With __reasonDictionary

            If (Not .ContainsKey(alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF)) Then
                .Add(alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, "SHOULD BE OFF （應為OFF）")
            End If
            If (Not .ContainsKey(alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON)) Then
                .Add(alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON, "SHOULD BE ON （應為ON）")
            End If
        End With


        If (AlarmReference Is Nothing) Then
            Me.TextBoxAlarmMessage.Text = "Alarm Manager Not Linked"
            Exit Sub
        End If

        '------------------------------------
        '   Initiating Presentation
        '------------------------------------
        Try
            '-----------------------------------------------------------------------
            ' Once Alarm occured and in sensor form , target the area to inform user
            '-----------------------------------------------------------------------
            GroupBoxArea.Controls.Clear()
            If (areaMapReference IsNot Nothing) Then
                areaMapReference.SourceInformation = __alarmReference.CurrentAlarm   'setup
                GroupBoxArea.Controls.Add(areaMapReference)          'show
            End If


        Catch ex As Exception
            '-------------------------------------
            ' Prevent Database failed
            '-------------------------------------
        Finally

            Select Case Application.CurrentCulture.ToString
                Case "en-US"
                    'default button memtion , Hsien , 2015.04.27
                    ButtonRetry.Text = "Retry"
                    ButtonIgnore.Text = "Ignore"
                    buttonOption3.Text = "Else"
                Case Else
                    'default button memtion , Hsien , 2015.04.27
                    ButtonRetry.Text = "重試"
                    ButtonIgnore.Text = "忽略"
                    buttonOption3.Text = "其他"
            End Select


            Me.TextBoxAlarmMessage.Text = interpreterReference(__alarmReference.CurrentAlarm)
            Me.TextBoxAlarmMessage.Text += vbCrLf & __alarmReference.CurrentAlarm.AdditionalInfo


            ButtonRetry.Enabled = (__alarmReference.CurrentAlarm.PossibleResponse And alarmContextBase.responseWays.RETRY)
            ButtonIgnore.Enabled = (__alarmReference.CurrentAlarm.PossibleResponse And alarmContextBase.responseWays.IGNORE)
            buttonOption3.Enabled = (__alarmReference.CurrentAlarm.PossibleResponse And alarmContextBase.responseWays.OPTION3)
            ButtonAbort.Enabled = (__alarmReference.CurrentAlarm.PossibleResponse And alarmContextBase.responseWays.ABORT)
        End Try

    End Sub

    '-------------------------------------
    '   
    '-------------------------------------
    Private Sub buttonClick(sender As Button, e As EventArgs) Handles ButtonIgnore.Click,
        ButtonRetry.Click,
        buttonOption3.Click,
        ButtonAbort.Click,
        ButtonHide.Click

        'Hsien , 2015.06.19 , added optional function := ignore and cassette eject
        Select Case sender.Name
            Case ButtonRetry.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.RETRY
            Case ButtonIgnore.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.IGNORE
            Case buttonOption3.Name
                'Me.AlarmReference.UserResponse = alarmContextBase.responseWays.IGNORE
                'cassetteAction(__alarmReference.CurrentAlarm)
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.OPTION3 'regulared , Hsien , 2016.03.10
            Case ButtonAbort.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.ABORT
            Case ButtonHide.Name
                If (Me.ParentForm IsNot Nothing AndAlso
                    Me.ParentForm.IsHandleCreated) Then
                    Me.ParentForm.DialogResult = DialogResult.Cancel
                End If
            Case Else

        End Select
        If utilities.IsInIDEmode = True AndAlso sender.Name = ButtonAbort.Name Then
            Dim response As MsgBoxResult = MsgBox("yes:abort, no:retry", MsgBoxStyle.YesNo)
            If response = MsgBoxResult.No Then
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.RETRY
            End If
        End If
        '---------------------------------------------
        '   Close parent form until next alarm occured
        '---------------------------------------------
        If (Me.FindForm IsNot Nothing) Then
            Me.FindForm.DialogResult = DialogResult.OK
        End If

    End Sub
    
    Private Sub checkBoxBuzzerOffCheckedChanged(sender As Object, e As EventArgs) Handles checkBoxBuzzer.CheckedChanged
        buzzerFlagReference.writeFlag(assemblyArch.controlFlagsEnum.IS_BUZZER_ON, checkBoxBuzzer.Checked)
    End Sub

End Class

Public Interface ISensorAreaVisiable
    '--------------------------------------------------------------------------------------
    'The userControl which used to demostrate sensor area should implement this interface
    ' Hsien , 2015.06.09
    '--------------------------------------------------------------------------------------
    Property SourceInformation As Object
    Sub CloseAreaShow()
End Interface

Public Class alarmInterpreters
    'used to collect alarm interpreters

    Delegate Function alarmInterpreterPrototype(ByVal __alarm As alarmContextBase) As String    'prototype declaration

    Shared __reasonDictionary As Dictionary(Of alarmContentSensor.alarmReasonSensor, String) = New Dictionary(Of alarmContentSensor.alarmReasonSensor, String)

    Public Shared Function alarmConveyorInterpretor(__alarm As alarmContextBase) As String
        Dim sb As StringBuilder = New StringBuilder()

        '-------------------------------------
        '   For Conveyor System ( exception the tray transporter
        '-------------------------------------
        Dim alarmContextAsConveyor As alarmContentConveyor = TryCast(__alarm, alarmContentConveyor)
        If (alarmContextAsConveyor IsNot Nothing) Then

            Select Case alarmContextAsConveyor.Detail
                Case alarmContentConveyor.alarmReasonConveyor.WAFER_JAMMED
                    sb.AppendLine("輸送帶警報--塞片(Wafer Jammed)")
                Case alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
                    sb.AppendLine("輸送帶警報--遺失片(Wafer Loss)")
                Case alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
                    sb.AppendLine("輸送帶警報--未知片(Unknown Wafer)")
                Case Else

            End Select
        End If

        Dim __sensorString As String = alarmSensorInterpretor(__alarm)
        If (__sensorString <> "") Then
            sb.AppendLine(__sensorString)
        End If

        Return sb.ToString
    End Function

    Public Shared Function alarmSensorInterpretor(__alarm As alarmContextBase) As String
        Dim sb As StringBuilder = New StringBuilder()

        '---------------------------------------
        '   For Cylinders
        '---------------------------------------
        Dim senderAsCylinders As driveBase = Nothing

        If (senderAsCylinders Is Nothing) Then
            senderAsCylinders = TryCast(__alarm.Sender, cylinderControlBase)
        End If
        If (senderAsCylinders IsNot Nothing) Then
            '-------------------------
            '   Cylinder Alarm Confirmed
            '-------------------------
            sb.AppendLine("氣壓缸異常(Cylinder Failed)")
        End If


        Dim __sensorAlarmMulti As alarmContextMultiSensors = TryCast(__alarm, alarmContextMultiSensors)

        If (__sensorAlarmMulti IsNot Nothing) Then
            '---------------------------------
            '   Try to interpret multi sensor alarm , Hsien , 2015.05.18
            '---------------------------------
            Try
                For Each __sensor As KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor) In __sensorAlarmMulti.SensorConditionList

                    Dim __name As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType, __sensor.Key.InputBit)
                    sb.AppendLine(String.Format("{0}, ({1}) , {2}{3}",
                                         __name.ToString(),
                                         alarmContentSensor.QueryInterface.query(__name),
                                         vbCrLf,
                                         TypeDescriptor.GetConverter(__sensor.Value).ConvertTo(__sensor.Value, GetType(String))))
                    ' i.e SpF1 , IP80 R0-0-1 , xxx檢知
                    '          應該為ON
                Next
            Catch ex As Exception
                '--------------------------
                '   Unable to query detail 
                '--------------------------
                sb.AppendLine(__alarm.ToString())
            End Try


        Else

            Dim __sensorAlarm As alarmContentSensor = TryCast(__alarm, alarmContentSensor)

            Try
                If (__sensorAlarm IsNot Nothing) Then
                    Dim __name As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType, __sensorAlarm.Inputs)
                    sb.AppendLine(String.Format("{0}, ({1}) , {2}{3}",
                                         __name.ToString(),
                                         alarmContentSensor.QueryInterface.query(__name),
                                         vbCrLf,
                                         TypeDescriptor.GetConverter(__sensorAlarm.Reason).ConvertTo(__sensorAlarm.Reason, GetType(String))))
                End If
            Catch ex As Exception
                '--------------------------
                '   Unable to query detail 
                '--------------------------
                sb.AppendLine(__alarm.ToString())
            End Try

        End If

        Return sb.ToString

    End Function

    Public Shared Function alarmMotorInterpretor(__alarm As alarmContextBase) As String
        '-------------------------
        '   Hsien , 2015.04.14 , as interpreter 
        '-------------------------
        Dim sb As StringBuilder = New StringBuilder()

        Dim __motorAlarm As alarmContentMotor = TryCast(__alarm, alarmContentMotor)

        Try
            If (__motorAlarm IsNot Nothing) Then

                Dim senderAsMotor As motorControl = TryCast(__motorAlarm.Sender, motorControl)

                'motor name , return error , motion status , error status
                sb.AppendLine(String.Format("馬達警報(Motor Failed)：{0}{1}錯誤碼(Error Code):{2}{1}運動狀態(Status):{3}{1}停止原因(Reason):{4}",
                                            [Enum].GetName(alarmContentMotor.MotorEnumType, senderAsMotor.MotorIndex),
                                            vbCrLf,
                                            senderAsMotor.ReturnError.ToString() & "(" & CInt(senderAsMotor.ReturnError) & ")",
                                            (New motionStatusTypeConvertor).ConvertTo(senderAsMotor.MotionStatus, GetType(String)),
                                            (New errorStatusTypeConvertor).ConvertTo(senderAsMotor.ErrorStatus, GetType(String))))
            End If
        Catch ex As Exception
            '--------------------------
            '   Unable to query detail 
            '--------------------------
        End Try

        Return sb.ToString

    End Function

    Public Shared Function alarmDefaultInterpretor(__alarm As alarmContextBase) As String
        Return __alarm.ToString()
    End Function

    Public Shared Function generateInterpretor(__interpretors As alarmInterpreterPrototype()) As alarmInterpreterPrototype
        'the utility tool used to combined interpretors

        'return a combined delegate
        Return Function(__alarm As alarmContextBase) As String
                   For Each __interpretor As alarmInterpreterPrototype In __interpretors
                       Dim __returnString As String = __interpretor.Invoke(__alarm)

                       'once get some string from any interpretor , break the procedure
                       If (__returnString <> "") Then
                           Return __returnString
                       End If
                   Next

                   Return ""
               End Function

    End Function

End Class