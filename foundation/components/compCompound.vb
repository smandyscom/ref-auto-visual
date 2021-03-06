﻿Imports Automation.mainIOHardware
Imports Automation.Components.Services

Namespace Components
    Namespace Services

#Region "Compound"
        Public Class flipService : Inherits driveBase
            ' warn : this control is not time-critical , applied on HMI or GUI control only
            ' do NOT applied on machine control
            ' controlled by isEnabled

            ' setup interface
            Property OutputBit As ULong = 0        ' indicating which output bit to trigger
            Property FlipGoal As TimeSpan
                Get
                    Return flipTimer.TimerGoal
                End Get
                Set(value As TimeSpan)
                    flipTimer.TimerGoal = value
                End Set
            End Property

            ' internal
            'Private flipCount As Integer            ' current count as flip
            Private flipStopWatch As Stopwatch = New Stopwatch()
            Private flipTimer As ringTimer = New ringTimer()

            Protected Overrides Function process() As Integer

                ' Timer Ticked | Previous Status | R
                '   0               0               0
                '   0               1               0
                '   1               0               1
                '   1               1               0

                If (flipTimer.IsTimerTicked) Then
                    writeBit(OutputBit, Not readBit(OutputBit))
                End If

                flipTimer.running()

                Return 0
            End Function

            ' gui
            'Public Overrides Function raisingGUI() As Control
            '    Dim uc As userControlFlipService = New userControlFlipService()
            '    uc.flip = Me
            '    Return uc
            'End Function

            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                ' clear output when disabled
                If (Not arg) Then
                    writeBit(OutputBit, False)
                End If

                flipTimer.IsEnabled = arg

                Return MyBase.enableDetail(arg)
            End Function

        End Class

        Public Class sensorControl : Inherits driveBase
            ' counting time-interval between raising edge to fallen edge

            ' set interface
            Property InputBit As ULong
                Get
                    Return RisingEdge.InputBit
                End Get
                Set(ByVal value As ULong)
                    RisingEdge.InputBit = value
                    FallenEdge.InputBit = value
                End Set
            End Property

            ' control interface
            Property PulseCount As UInteger = 0

            Property OnPulseWidth As TimeSpan = TimeSpan.Zero ' as snap shot when fallenedge detected , or state reset
            Property OffPulseWidth As TimeSpan = TimeSpan.Zero ' as snap shot when raisingedge detected , or state reset

            ReadOnly Property IsSensorCovered As Boolean
                Get
                    'Return subState = 10
                    Return readBit(InputBit) 'Hsien , used to decouple from upper control
                    ' means rising edge detected already , wait for fallen edge , which means sensor had covered by something
                    ' otherwise , not covered
                End Get
            End Property


            ' internal 
            ' internal components
            Property RisingEdge As risingEdge = New risingEdge()
            Property FallenEdge As fallenEdge = New fallenEdge()
            Property OnTimer As singleTimer = New singleTimer()     'monitoring sensor-on time
            Property OffTimer As singleTimer = New singleTimer      'monitoring sensor-off time
            Private subState As Short = 0

            Public Function stateReset() As Integer
                'Hsien , 2014.07.05
                OnPulseWidth = OnTimer.TimeElapsed        ' backup the pulse width since last rising edge
                subState = 0                ' reset state
                Return 0
            End Function

            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                '-------
                ' reset
                ' since interrupted process , the pulse width count would reset
                RisingEdge.IsEnabled = arg
                FallenEdge.IsEnabled = arg
                stateReset()
                'If (Not arg) Then
                ' reset the timer
                OnTimer.IsEnabled = True
                OnTimer.IsEnabled = False
                OffTimer.IsEnabled = True   'Hsien , 2015.03.24
                'End If
                'Timer.IsEnabled = arg       ' follow the parent setting , Hsien , 2014.10.13
                Return MyBase.enableDetail(arg)
            End Function

            Protected Overrides Function process() As Integer

                '   >limit  |   fallenedge  |   Result
                '   ----------------------------------
                '   0       |   0           |   Keep Counting
                '   0       |   1           |   pulseCount+=1   ,   recycle
                '   1       |   0           |   ErrorCount+=1
                '   1       |   1           |   ErrorCount+=1   ,   pulseCount+1    ,   recycle
                Select Case subState
                    Case 0
                        ' detect the rising edge
                        If (RisingEdge.IsDetected) Then
                            '--------------------
                            '   Bug Found : using edge methology would failed when device disabled (missing the edge)
                            ' Hsien , 2014.10.21
                            '--------------------
                            OffPulseWidth = OffTimer.TimeElapsed    'take snap shot
                            OnTimer.IsEnabled = True
                            OffTimer.IsEnabled = False
                            OffTimer.resetTimer() 'clear snapshot , Hsien , 2015.02.06

                            subState = 10
                        End If
                    Case 10
                        ' detect the fallen edge
                        If (FallenEdge.IsDetected) Then

                            OnPulseWidth = OnTimer.TimeElapsed        ' take snap shot
                            OnTimer.IsEnabled = False
                            OffTimer.IsEnabled = True
                            OnTimer.resetTimer() ' clear snapshot , Hsien , 2014.10.09

                            '-------------------
                            '   Overflow prevent
                            '-------------------
                            If (PulseCount = UInteger.MaxValue) Then
                                PulseCount = 0
                            End If
                            PulseCount += 1
                            subState = 0
                        End If
                End Select


                ' run the detector
                RisingEdge.running()
                FallenEdge.running()

                Return 0
            End Function


            '------------------------------
            '   Utility tool 
            '------------------------------
            Shared Sub activateSensorControl(ByVal __sensor As sensorControl, ByVal __isActivate As Boolean)
                If (Not __sensor.IsEnabled And __isActivate) Then
                    __sensor.IsEnabled = True
                ElseIf (Not __isActivate) Then
                    __sensor.IsEnabled = False
                End If
            End Sub
        End Class

        Public Class sensorCheckService : Inherits driveBase

            Public Class sensorCheckEventArgs : Inherits EventArgs
                Property Content As sensorCheckCondition
                Sub New(ByVal __sensor As sensorCheckCondition)
                    Content = __sensor
                End Sub
            End Class

            Public Class sensorCheckCondition
                Property SensorBit As ULong
                Property Condition As Boolean     ' the condition should the sensor bit be
                Property IsConditionPassed As Boolean
                Public Sub New(ByVal _sensorBit As ULong, ByVal _condition As Boolean)
                    SensorBit = _sensorBit
                    Condition = _condition
                End Sub
            End Class

            ' setup interface
            Property SensorsNeedToCheck As List(Of sensorCheckCondition) = New List(Of sensorCheckCondition)

            ' handling interface
            Public Event CheckFailed As EventHandler(Of sensorCheckEventArgs)
            Public Event CheckPassed As EventHandler(Of sensorCheckEventArgs)

            ' control interface
            ReadOnly Property IsAllConditionPassed As Boolean
                Get
                    Return flagAllConditionPassed
                End Get
            End Property

            ' internal
            ' Private inProcessLock As Boolean = False
            Private flagAllConditionPassed As Boolean = True  'Kung, 2015.06.11  解決啟動後因預設值而造成先暫停問題
            Private flagTempAllConditionPassed As Boolean = False
            Private tempState As Boolean = False

            Protected Overrides Function process() As Integer
                ' 1. check sensors listed in ...
                ' 2. if condition not matched , rising corresponding handler
                ' similiar to event-drive

                flagTempAllConditionPassed = True
                For Each sensor As sensorCheckCondition In SensorsNeedToCheck
                    tempState = readBit(sensor.SensorBit)
                    sensor.IsConditionPassed = (tempState = sensor.Condition)   ' reflect if condition passed
                    If (tempState <> sensor.Condition) Then
                        RaiseEvent CheckFailed(Me, New sensorCheckEventArgs(sensor))
                        flagTempAllConditionPassed = False  ' condition override
                    Else

                    End If
                Next
                If flagTempAllConditionPassed = True Then
                    RaiseEvent CheckPassed(Me, New sensorCheckEventArgs(Nothing))
                End If

                '---------------
                '   Due to multi thread issue
                '---------------
                flagAllConditionPassed = flagTempAllConditionPassed

                Return 0
            End Function
        End Class



        Public Class genericCheckService : Inherits driveBase

            Public Class genericCheckEventArgs : Inherits EventArgs
                Property Content As genericCheckCondition
                Sub New(__condition As genericCheckCondition)
                    Me.Content = __condition
                End Sub
            End Class

            Public Class genericCheckCondition
                Property Sender As Object           ' sender would be report when check failed/passed
                Property Condition As Func(Of Boolean) = Function() (True)     ' used to do condition check
            End Class

            '------------------------------------
            '   Generated from sensorCheckService
            '       when condition function turns true , raising event of CheckPassed
            '       otherwise , raising event of CheckFailed
            '------------------------------------
            Property ObjectConditionsNeedToCheck As List(Of genericCheckCondition) = New List(Of genericCheckCondition)
            ' handling interface
            Public Event CheckFailed As EventHandler(Of genericCheckEventArgs)
            Public Event CheckPassed As EventHandler(Of genericCheckEventArgs)

            Protected Overrides Function process() As Integer
                '--------------------------------------
                '   Generic check service , could apply on rising edge/status onoff/internal flag tracking/numeric comparation
                '--------------------------------------
                For Each element As genericCheckCondition In ObjectConditionsNeedToCheck
                    If (element.Condition.Invoke()) Then
                        RaiseEvent CheckPassed(Me, New genericCheckEventArgs(element))
                    Else
                        RaiseEvent CheckFailed(Me, New genericCheckEventArgs(element))
                    End If
                Next

                Return MyBase.process()
            End Function

        End Class
#End Region

    End Namespace
End Namespace


Public Class debouncedSensorControl
    Inherits sensorControl

    ''' <summary>
    ''' In milli-second
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DebouncedOnTime As Integer = 100
    Property DebouncedOffTime As Integer = 100
    Property PulseWidthOnTime As Integer = 100

    Public Event DebouncedOn(ByVal sender As Object, ByVal e As EventArgs)
    Public Event DebouncedOff(ByVal sender As Object, ByVal e As EventArgs)

    Public Event PulseWidthOn(ByVal sender As Object, ByVal e As EventArgs)

    Protected Overrides Function process() As Integer
        MyBase.process()

        If Me.OnTimer.TimeElapsed.TotalMilliseconds > DebouncedOnTime Then
            RaiseEvent DebouncedOn(Me, EventArgs.Empty)
        ElseIf OffTimer.TimeElapsed.TotalMilliseconds > DebouncedOffTime Then
            RaiseEvent DebouncedOff(Me, EventArgs.Empty)
        End If

        If PulseCount > 0 And
            OnPulseWidth.TotalMilliseconds > PulseWidthOnTime Then
            PulseCount = 0 'reset
            RaiseEvent PulseWidthOn(Me, EventArgs.Empty)
        End If

        Return 0
    End Function

End Class