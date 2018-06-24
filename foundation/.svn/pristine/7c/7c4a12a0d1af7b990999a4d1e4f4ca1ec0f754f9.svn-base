Imports Automation
Imports Automation.Components.CommandStateMachine
Imports System.Text
Imports Automation.Components.Services
Imports Automation.alarmInterpreters

Public Class userControlAlarm
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

    Public interpreterReference As alarmInterpreterPrototype = alarmInterpreters.generateInterpretor({New alarmInterpreterPrototype(AddressOf alarmConveyorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmSensorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmMotorInterpretor),
                                                                                                      New alarmInterpreterPrototype(AddressOf alarmDefaultInterpretor)})    'reformed , unified with alarmPop , 2015.08.19

    Private isAlarmMonitorInvoked As Boolean = False

    Private WithEvents __alarmReference As alarmManager = Nothing


    Private Sub alarmWaitingHandler(ByVal sender As Object, ByVal e As alarmEventArgs) Handles __alarmReference.alarmWaitResponse
        Try
            If (Me.ParentForm Is Nothing OrElse
                Not Me.ParentForm.IsHandleCreated OrElse
                Me.ParentForm.IsDisposed) Then
                __alarmReference = Nothing  'cut the link
                Exit Sub
            End If

            If (isAlarmMonitorInvoked) Then
                Exit Sub
            End If

            Me.Invoke(Sub()
                          isAlarmMonitorInvoked = True

                          TextBoxAlarm.Text = interpreterReference(e.Content) 'Hsien ,2015.04.20
                          TextBoxAlarm.Text += e.Content.AdditionalInfo 'Hsien , 2015.08.20

                          ButtonRetry.Enabled = (e.Content.PossibleResponse And alarmContextBase.responseWays.RETRY)
                          ButtonIgnore.Enabled = (e.Content.PossibleResponse And alarmContextBase.responseWays.IGNORE)
                          ButtonEnd.Enabled = (e.Content.PossibleResponse And alarmContextBase.responseWays.ABORT)

                          Me.TimerFlash.Enabled = True

                      End Sub)

        Catch ex As Exception
            '------------------------------
            '
            '------------------------------
        End Try

    End Sub
    Private Sub alarmReleaseHandler(ByVal sender As Object, ByVal e As EventArgs) Handles __alarmReference.alarmReleased
        Try
            If (Me.ParentForm Is Nothing OrElse
                Not Me.ParentForm.IsHandleCreated OrElse
                Me.ParentForm.IsDisposed) Then
                __alarmReference = Nothing  'cut the link
                Exit Sub
            End If

            Me.Invoke(Sub()
                          TextBoxAlarm.Clear()

                          ButtonRetry.Enabled = False
                          ButtonIgnore.Enabled = False
                          ButtonEnd.Enabled = False

                          Me.TimerFlash.Enabled = False
                          Me.BackColor = DefaultBackColor

                          isAlarmMonitorInvoked = False
                      End Sub)
        Catch ex As Exception
            '------------------------------
            '
            '------------------------------
        End Try

    End Sub

    Private Sub alarmResponse(sender As Button, e As EventArgs) Handles ButtonEnd.Click, ButtonIgnore.Click, ButtonRetry.Click

        Select Case sender.Name
            Case ButtonRetry.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.RETRY
            Case ButtonIgnore.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.IGNORE
            Case ButtonEnd.Name
                Me.AlarmReference.UserResponse = alarmContextBase.responseWays.ABORT
            Case Else

        End Select

    End Sub

    Private Sub flash(sender As Object, e As EventArgs) Handles TimerFlash.Tick
        If (Me.BackColor = DefaultBackColor) Then
            Me.BackColor = Color.Red
        Else
            Me.BackColor = DefaultBackColor
        End If
    End Sub


    '-----------------------------------------
    '   The interpreters
    '-----------------------------------------
    'Public Function basicAlarmContextInterpreter(sender As Object, e As alarmEventArgs) As String

    '    Static __reasonDictionary As Dictionary(Of alarmContentSensor.alarmReasonSensor, String) '= New Dictionary(Of alarmContentSensor.alarmReasonSensor, String)

    '    Dim __alarm As alarmContextBase = TryCast(e.Content, alarmContextBase)

    '    'reason dicitionary initialize
    '    If (__reasonDictionary Is Nothing) Then
    '        __reasonDictionary = New Dictionary(Of alarmContentSensor.alarmReasonSensor, String)

    '        __reasonDictionary.Add(alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF, "SHOULD BE OFF （應為OFF）")
    '        __reasonDictionary.Add(alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON, "SHOULD BE ON （應為ON）")

    '    End If

    '    '-------------------------
    '    '   Hsien , 2015.04.14 , as interpreter 
    '    '-------------------------
    '    Dim __returnMessage As String = ""
    '    Dim sb As StringBuilder = New StringBuilder()

    '    'default button memtion , Hsien , 2015.04.27
    '    ButtonRetry.Text = "重試"
    '    ButtonIgnore.Text = "忽略"

    '    Dim senderAsTransporter As transporterBase = TryCast(__alarm.Sender, transporterBase)

    '    If (senderAsTransporter IsNot Nothing) Then
    '        '-----------------------------
    '        '   Alarm From Transporter
    '        '-----------------------------
    '        Dim interpretedString As String = ""

    '        '------------------------
    '        '   Translating transporter's name
    '        '------------------------
    '        Try
    '            interpretedString = alarmContentSensor.QueryInterface.query(senderAsTransporter)
    '        Catch ex As Exception
    '            interpretedString = senderAsTransporter.ToString
    '        End Try


    '        Dim __lossOrUnknownAlarm As alarmContentConveyor = TryCast(__alarm, alarmContentConveyor)

    '        If (__lossOrUnknownAlarm IsNot Nothing) Then
    '            '----------------
    '            'the ignite state
    '            '----------------
    '            Dim inputEnum As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType, __lossOrUnknownAlarm.Inputs)
    '            Dim inputString As String = [Enum].GetName(alarmContentSensor.InputsEnumType, inputEnum)


    '            If (senderAsTransporter.MainState = systemControlPrototype.systemStatesEnum.IGNITE) Then

    '                Select Case __lossOrUnknownAlarm.Detail
    '                    Case alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
    '                        interpretedString = interpretedString & vbCrLf & String.Format("感測器{0}偵測到載盤，與資料不符", inputString) & vbCrLf '& "重試: 確認無載盤" & vbCrLf & "忽略: 確認有載盤"

    '                        'override button memtion , Hsien , 2015.04.27
    '                        ButtonRetry.Text = "確認無載盤"
    '                        ButtonIgnore.Text = "確認有載盤"
    '                    Case alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
    '                        interpretedString = interpretedString & vbCrLf & String.Format("感測器{0}未偵測到載盤，與資料不符", inputString) & vbCrLf '& "重試: 確認無載盤" & vbCrLf & "忽略: 確認有載盤"

    '                        'override button memtion , Hsien , 2015.04.27
    '                        ButtonRetry.Text = "確認有載盤"
    '                        ButtonIgnore.Text = "確認無載盤"
    '                    Case Else

    '                End Select

    '            Else
    '                '-------------------------
    '                '   Not on ignite stage
    '                '-------------------------
    '                Select Case __lossOrUnknownAlarm.Detail
    '                    Case alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
    '                        interpretedString = interpretedString & vbCrLf & String.Format("離開定位錯誤（感測器{0}）", inputString) & vbCrLf '& "重試: 確認無載盤" & vbCrLf & "忽略: 確認有載盤"

    '                        'override button memtion , Hsien , 2015.04.27
    '                        ButtonRetry.Text = "確認無載盤"
    '                        ButtonIgnore.Text = "確認有載盤"
    '                    Case alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
    '                        interpretedString = interpretedString & vbCrLf & String.Format("到達定位錯誤（感測器{0}）", inputString) & vbCrLf '& "重試: 確認無載盤" & vbCrLf & "忽略: 確認有載盤"

    '                        'override button memtion , Hsien , 2015.04.27
    '                        ButtonRetry.Text = "確認有載盤"
    '                        ButtonIgnore.Text = "確認無載盤"
    '                    Case Else

    '                End Select
    '            End If

    '        End If

    '        sb.AppendLine(interpretedString)    'Hsien , 2015.05.28
    '    End If
    '    '------------------------------------
    '    '   End of transporter alarm
    '    '------------------------------------

    '    '-------------------------------------
    '    '   For Conveyor System ( exception the tray transporter
    '    '-------------------------------------
    '    Dim alarmContextAsConveyor As alarmContentConveyor = TryCast(__alarm, alarmContentConveyor)
    '    If (alarmContextAsConveyor IsNot Nothing AndAlso
    '        (senderAsTransporter Is Nothing)) Then

    '        Select Case alarmContextAsConveyor.Detail
    '            Case alarmContentConveyor.alarmReasonConveyor.WAFER_JAMMED
    '                sb.AppendLine("輸送帶警報 -- 塞片")
    '            Case alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS
    '                sb.AppendLine("輸送帶警報 -- 遺失片")
    '            Case alarmContentConveyor.alarmReasonConveyor.WAFER_UNKNOWN
    '                sb.AppendLine("輸送帶警報 -- 未知片")
    '            Case Else

    '        End Select
    '    End If

    '    '---------------------------------------
    '    '   For Cylinders
    '    '---------------------------------------
    '    Dim senderAsCylinders As driveBase = TryCast(__alarm.Sender, cylinderControl)
    '    If (senderAsCylinders Is Nothing) Then
    '        senderAsCylinders = TryCast(__alarm.Sender, cylinderControlTwin)
    '    End If
    '    If (senderAsCylinders Is Nothing) Then
    '        senderAsCylinders = TryCast(__alarm.Sender, cylinderControlBase)
    '    End If
    '    If (senderAsCylinders IsNot Nothing) Then
    '        '-------------------------
    '        '   Cylinder Alarm Confirmed
    '        '-------------------------
    '        sb.AppendLine("氣壓缸異常")
    '    End If


    '    Dim __sensorAlarmMulti As alarmContextMultiSensors = TryCast(__alarm, alarmContextMultiSensors)

    '    If (__sensorAlarmMulti IsNot Nothing) Then
    '        '---------------------------------
    '        '   Try to interpret multi sensor alarm , Hsien , 2015.05.18
    '        '---------------------------------
    '        Try
    '            For Each __sensor As KeyValuePair(Of sensorControl, alarmContentSensor.alarmReasonSensor) In __sensorAlarmMulti.SensorConditionList

    '                Dim __name As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType, __sensor.Key.InputBit)
    '                sb.AppendLine(String.Format("{0}, ({1}) , {2}{3}",
    '                                     __name.ToString(),
    '                                     alarmContentSensor.QueryInterface.query(__name),
    '                                     vbCrLf,
    '                                     __reasonDictionary(__sensor.Value)))
    '                ' i.e SpF1 , IP80 R0-0-1 , xxx檢知
    '                '          應該為ON
    '            Next
    '        Catch ex As Exception
    '            '--------------------------
    '            '   Unable to query detail , return default message
    '            '--------------------------
    '            sb.AppendLine(__alarm.ToString())
    '        End Try


    '    Else

    '        Dim __sensorAlarm As alarmContentSensor = TryCast(__alarm, alarmContentSensor)

    '        Try
    '            If (__sensorAlarm IsNot Nothing) Then
    '                Dim __name As [Enum] = [Enum].ToObject(alarmContentSensor.InputsEnumType, __sensorAlarm.Inputs)
    '                sb.AppendLine(String.Format("{0}, ({1}) , {2}{3}",
    '                                     __name.ToString(),
    '                                     alarmContentSensor.QueryInterface.query(__name),
    '                                     vbCrLf,
    '                                     __reasonDictionary(__sensorAlarm.Reason)))
    '            End If
    '        Catch ex As Exception
    '            '--------------------------
    '            '   Unable to query detail , return default message
    '            '--------------------------
    '            sb.AppendLine(__alarm.ToString())
    '        End Try

    '    End If

    '    Dim __motorAlarm As alarmContentMotor = TryCast(__alarm, alarmContentMotor)

    '    Try
    '        If (__motorAlarm IsNot Nothing) Then
    '            sb.AppendLine(String.Format("馬達警報：{0}{1}{2}",
    '                                        [Enum].GetName(alarmContentMotor.MotorEnumType, CType(__motorAlarm.Sender, motorControl).MotorIndex),
    '                                        vbCrLf,
    '                                        __motorAlarm.ToString))
    '        End If
    '    Catch ex As Exception
    '    End Try


    '    If (sb.Length = 0) Then
    '        sb.Append(__alarm.ToString())
    '    End If

    '    sb.AppendLine(__alarm.AdditionalInfo)   'Hsien ,  2015.05.14

    '    Return sb.ToString

    'End Function

End Class
