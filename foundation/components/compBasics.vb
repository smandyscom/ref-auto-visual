'Option Explicit On
'Imports AMONetSystem.AlarmMutex
Imports System.Reflection
Imports System.Text
Imports Automation.mainIOHardware
#Region "Basic definitions"
Public Delegate Function stateFunction(ByRef state As Integer) As Boolean   'the function prototype of state function
#End Region

Namespace Components
    Namespace Services

        Public MustInherit Class driveBase
            ' the base class on control/sensor (control including output functions)
            ' such device should be put-in timer-driving thread , in ordering to be monitored in regular time-scale

            ' setup interface

            ' indicating the time tick interval in unit of msec
            Property Parent As driveBase 'used to trace parent , 2015.04.04
            Property DeviceName As String = Me.GetType().ToString()

            '-------------------------------------
            '   Alarm/Hierarachy system extensions
            '-------------------------------------
            ReadOnly Property IsSenderBelongToMe(sender As Object) As Boolean
                'trace back if im the parent of sender, Hsien , 2015.09.20
                Get
                    Dim __senderAsDrivebase As driveBase = TryCast(sender, driveBase)
                    Return ((__senderAsDrivebase IsNot Nothing) AndAlso
                        (__senderAsDrivebase.Equals(Me) OrElse IsSenderBelongToMe(__senderAsDrivebase.Parent)))
                End Get
            End Property
            ReadOnly Property IsMyAlarmCurrent As Boolean
                Get
                    Return CentralAlarmObject.CurrentAlarm IsNot Nothing AndAlso
                        (CentralAlarmObject.CurrentAlarm.Sender IsNot Nothing) AndAlso
                        (IsSenderBelongToMe(CentralAlarmObject.CurrentAlarm.Sender)) AndAlso
                        CentralAlarmObject.IsAlarmed 'prevent motor alarm force to drive
                End Get
            End Property
            ReadOnly Property IsMyAlarmInQueue As Boolean
                Get
                    'Hsien , bug found , use 'any' rather 'all' , list.exist() likewise , 2015.12.24
                    Return CentralAlarmObject.AlarmQueue.Count > 0 AndAlso
                        CentralAlarmObject.AlarmQueue.Any(Function(__pack As alarmContextBase) ((__pack.Sender IsNot Nothing) AndAlso (IsSenderBelongToMe(__pack.Sender))))
                End Get
            End Property


            'Property ioInformationDictionary As Dictionary(Of [Enum], String)   ' ready to link , use to query inputs informations
            ' the gui interface to this control
            Public Overridable Function raisingGUI() As Control
                ' 1. generating corresponding GUI
                ' 2. link GUI member and me
                ' 3. return
                Dim uc As userControlPropertyView = New userControlPropertyView()
                uc.Drive = Me
                Return uc
            End Function

            Property IsEnabled As Boolean
                Get
                    Return flagIsEnabled
                End Get
                Set(value As Boolean)
                    enableDetail(value)
                End Set
            End Property
            ' determine whether time-drive process would be execute or ignore
            Protected flagIsEnabled As Boolean = False      ' controlled whether this instance to drive
            Protected Overridable Function enableDetail(ByVal arg As Boolean) As Integer
                ' default enable routine
                ' for combined component , child could enable other associated components here
                flagIsEnabled = arg
                Return 0
            End Function

            ' error report
            Public WithEvents CentralAlarmObject As alarmManager        ' central reporting
            Public WithEvents CentralMessenger As messageHandler   ' as central message bus report gate way
            'Property CentralMessageDataBase As messageDatas


            Public Function running() As Integer
                Try

                    If (flagIsEnabled <> True) Then
                        ' not not enabled  , ignore to drive
                        Return -1
                    End If

                    ' if alarm announced , return here

                    process()

                Catch ex As Exception

                    Throw New Exception(Me.DeviceName + ex.Message, ex)  'Hsien , 2014.10.28 , added more information

                End Try

                Return 0
            End Function

            ' used to be implemented by childs
            Protected Overridable Function process() As Integer
                Return 0
            End Function

            Public Overrides Function ToString() As String
                ' overrides , Hsien , 2014.09.22
                Return Me.DeviceName
            End Function
            Public Overridable Overloads Function ToString(arg As Object) As String
                '------------------------------
                '   Dump all fields/properties
                '------------------------------
                Dim __sb As StringBuilder = New StringBuilder
                __sb.AppendLine(String.Format("{0},Dump Start", Me.DeviceName))
                Me.GetType.GetFields(Reflection.BindingFlags.Instance Or
                                          Reflection.BindingFlags.NonPublic Or
                                          BindingFlags.Public).All(Function(__field As FieldInfo) As Boolean
                                                                       Try
                                                                           __sb.AppendLine(String.Format("Field:{0};Value:{1}",
                                                                                                         __field.Name,
                                                                                                         __field.GetValue(Me)))
                                                                       Catch ex As Exception
                                                                           __sb.AppendLine(String.Format("Field:{0};Cannot be shown", __field.Name))
                                                                       End Try
                                                                       Return True
                                                                   End Function)
                Me.GetType.GetProperties(Reflection.BindingFlags.Instance Or
                   Reflection.BindingFlags.NonPublic Or
                   BindingFlags.Public).All(Function(__property As PropertyInfo) As Boolean
                                                Try
                                                    __sb.AppendLine(String.Format("Property:{0};Value:{1}",
                                                                                  __property.Name,
                                                                                  __property.GetValue(Me, Nothing)))
                                                Catch ex As Exception
                                                    __sb.AppendLine(String.Format("Property:{0};Cannot be shown", __property.Name))
                                                End Try
                                                Return True
                                            End Function)

                __sb.AppendLine(String.Format("{0},Dump End", Me.DeviceName))
                __sb.AppendLine()    'append empty line
                Return __sb.ToString
            End Function

        End Class


#Region "Element"
        Public Class risingEdge : Inherits driveBase

            ' setup interface
            Property InputBit As ULong = 0

            ' control interface
            ReadOnly Property IsDetected As Boolean
                Get
                    Return flagRisingEdgeDetected
                End Get
            End Property

            ' internal
            Private flagRisingEdgeDetected As Boolean = False
            Private lastState As Boolean = False
            Private state As Boolean = False

            ' internal
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                ' flag reset
                lastState = False       'Hsien , 2014.10.21 , used to solve the missing edge situation
                flagRisingEdgeDetected = False
                Return MyBase.enableDetail(arg)
            End Function
            Protected Overrides Function process() As Integer

                '--------------------------------------
                '   T/F Table
                '--------------------------------------
                '   State   LastState   Result
                '   0       0           0
                '   0       1           0
                '   1       0           1
                '   1       1           0
                '----------------------------------------
                state = readBit(InputBit)
                flagRisingEdgeDetected = (Not lastState) And state
                lastState = state

                Return 0
            End Function

        End Class
        Public Class fallenEdge : Inherits driveBase

            ' setup interface
            Property InputBit As ULong = 0

            ' control interface
            ReadOnly Property IsDetected As Boolean
                Get
                    Return flagFallenEdgeDetected
                End Get
            End Property

            ' internal
            Private flagFallenEdgeDetected As Boolean = False
            Private lastState As Boolean = False
            Private state As Boolean = False

            ' internal
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                ' flag reset
                flagFallenEdgeDetected = False
                Return MyBase.enableDetail(arg)
            End Function
            Protected Overrides Function process() As Integer

                '--------------------------------------
                '   T/F Table
                '--------------------------------------
                '   State   LastState   Result
                '   0       0           0
                '   0       1           1
                '   1       0           0
                '   1       1           0
                '----------------------------------------
                state = readBit(InputBit)
                flagFallenEdgeDetected = (Not state) And lastState
                lastState = state

                Return 0
            End Function

        End Class

        Public Class singleTimer : Inherits driveBase

            ' setup interface
            Property TimerGoal As TimeSpan = New TimeSpan(0, 0, 0, 0, 30)

            ' control interface
            ReadOnly Property IsTimerTicked As Boolean
                Get
                    Return timerStopWatch.Elapsed >= TimerGoal
                End Get
            End Property
            ReadOnly Property TimeElapsed As TimeSpan
                Get
                    Return timerStopWatch.Elapsed
                End Get
            End Property

            ' internal
            Private timerStopWatch As Stopwatch = New Stopwatch()

            Public Sub resetTimer()
                ' Hsien  , 2014.10.09 clear snapshot
                timerStopWatch.Reset()
            End Sub
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                'reset
                If (arg) Then
                    timerStopWatch.Restart()
                Else
                    timerStopWatch.Stop()
                End If
                Return MyBase.enableDetail(arg)
            End Function

        End Class

        Public Class singleTimerContinueType
            Inherits driveBase

            ' setup interface
            Property TimerGoal As TimeSpan = New TimeSpan(0, 0, 0, 0, 30)

            ' control interface
            ReadOnly Property IsTimerTicked As Boolean
                Get
                    Return timerStopWatch.Elapsed >= TimerGoal
                End Get
            End Property
            ReadOnly Property TimeElapsed As TimeSpan
                Get
                    Return timerStopWatch.Elapsed
                End Get
            End Property

            ' internal
            Private timerStopWatch As Stopwatch = New Stopwatch()

            Public Sub resetTimer()
                ' Hsien  , 2014.10.09 clear elapsed time
                timerStopWatch.Reset()
            End Sub
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                'reset
                If (arg) Then
                    timerStopWatch.Start()  'continue measuring
                Else
                    timerStopWatch.Stop()
                End If
                Return MyBase.enableDetail(arg)
            End Function

        End Class

        Public Class ringTimer : Inherits driveBase

            ' setup interface
            Property TimerGoal As TimeSpan = New TimeSpan(0, 0, 0, 0, 30)

            ' control interface
            ReadOnly Property IsTimerTicked As Boolean
                Get
                    Return flagTimerTicked
                End Get
            End Property
            ReadOnly Property TimeElapsed As TimeSpan
                Get
                    Return timerStopWatch.Elapsed
                End Get
            End Property

            Private timerStopWatch As Stopwatch = New Stopwatch()
            Private flagTimerTicked As Boolean = False

            Protected Overrides Function process() As Integer

                flagTimerTicked = (timerStopWatch.Elapsed >= TimerGoal)

                If (timerStopWatch.Elapsed >= TimerGoal) Then
                    timerStopWatch.Restart()
                End If

                Return 0
            End Function
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                'reset
                flagTimerTicked = False


                If (arg) Then
                    timerStopWatch.Restart()
                Else
                    timerStopWatch.Stop()
                End If

                Return MyBase.enableDetail(arg)
            End Function
        End Class

        Public Class risingEdgeGeneric : Inherits driveBase
            '-----------------------------
            '   Similiar to raisingEdge , but replaced implict DI_State function to generic Condition delegate
            '-----------------------------
            Property Argument As Object             ' Hsien , 2014.10.29 used as data storage
            Property StateInput As Func(Of Boolean) = New Func(Of Boolean)(Function() As Boolean
                                                                               Return True
                                                                           End Function)
            ' control interface
            ReadOnly Property IsDetected As Boolean
                Get
                    Return flagRisingEdgeDetected
                End Get
            End Property

            ' internal
            Private flagRisingEdgeDetected As Boolean = False
            Private lastState As Boolean = False
            Private state As Boolean = False

            ' internal
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                ' flag reset
                lastState = False       'Hsien , 2014.10.21 , used to solve the missing edge situation
                flagRisingEdgeDetected = False
                Return MyBase.enableDetail(arg)
            End Function

            Protected Overrides Function process() As Integer
                '--------------------------------------
                '   T/F Table
                '--------------------------------------
                '   State   LastState   Result
                '   0       0           0
                '   0       1           0
                '   1       0           1
                '   1       1           0
                '----------------------------------------
                state = StateInput.Invoke()
                flagRisingEdgeDetected = (Not lastState) And state
                lastState = state

                Return 0

            End Function

        End Class
        Public Class fallenEdgeGeneric : Inherits driveBase

            ' setup interface
            Property Argument As Object             ' Hsien , 2014.10.29 used as data storage
            Property StateInput As Func(Of Boolean) = New Func(Of Boolean)(Function() As Boolean
                                                                               Return True
                                                                           End Function)
            ' control interface
            ReadOnly Property IsDetected As Boolean
                Get
                    Return flagFallenEdgeDetected
                End Get
            End Property

            ' internal
            Private flagFallenEdgeDetected As Boolean = False
            Private lastState As Boolean = False
            Private state As Boolean = False

            ' internal
            Protected Overrides Function enableDetail(ByVal arg As Boolean) As Integer
                ' flag reset
                flagFallenEdgeDetected = False
                Return MyBase.enableDetail(arg)
            End Function
            Protected Overrides Function process() As Integer

                '--------------------------------------
                '   T/F Table
                '--------------------------------------
                '   State   LastState   Result
                '   0       0           0
                '   0       1           1
                '   1       0           0
                '   1       1           0
                '----------------------------------------
                state = StateInput.Invoke()
                flagFallenEdgeDetected = (Not state) And lastState
                lastState = state

                Return 0
            End Function

        End Class

#End Region


    End Namespace
End Namespace