Imports Automation.Components.Services
Imports System.ComponentModel
Imports Automation.Components.CommandStateMachine
Imports System.Threading

Namespace Components
    Namespace CommandStateMachine

        Public Class alarmContentMotor : Inherits alarmContextBase



            Sub New()
                Me.PossibleResponse = responseWays.ABORT
            End Sub

            Shared Property MotorEnumType As Type = Nothing 'Hsien , 2015.06.18

            Public Overrides Function ToString() As String
                ' append the basic string
                Dim motor As motorControl = CType(Me.Sender, motorControl)

                'complete alarm message , i.e ring,deviceIP,.....motor name....
                Return MyBase.ToString() & vbCrLf &
                    String.Format("({0}):", motor.MotorIndex) &
                    String.Format("[{0}][{1}][{2}]",
                                  motor.ReturnError,
                                  motor.MotionStatus,
                                  motor.ErrorStatus) &
                              String.Format("Command:{0},Feedback:{1}",
                                            motor.CommandPosition,
                                            motor.FeedBackPosition)
            End Function
        End Class

        Public Class motorControl
            Inherits driveBase

#Region "Definitions"
            Public Enum motorCommandEnum As Integer
                NONE = 0                  ' no command in execute
                WAIT_RECALL = 1
                '----------------------------
                GO_HOME = 2
                GO_POSITION = 3
                STOP_FORCELY = 4           ' stop motion immeditatly
                STOP_SLOW_DOWN = 5         ' stop motion in slope
                '---------------------------
                JOG = 6
                GO_POSITION_OPEN_LOOP = 7
                '---------------------------
                'WAIT_MOTION_STOP =8       'Hsien , 2015.01.23 ,   wait hardware stopped signal
                MOTION_PAUSE = 9
                MOTION_RESUME = 10
                '---------------------------
                V_CHANGE = 11            ' Hsien , 2014.09.26
                '---------------------------
                GO_POSITION_COMBINED = 12    ' Hsien , 2014.09.27
                '----------------------------
                'START_MOVE  'master mode
                'START_MOVE_SLAVE    'slave mode
                '----------------------------
                SYNCHRON_MASTER = 13 'master mode
            End Enum
            Public Enum statusEnum As Integer
                ' the internal status 
                ' also the return value to caller of drive()
                'NONE
                EXECUTING = &H0                 ' 0x00
                EXECUTION_END = &H1000            ' 0x1000
                EXECUTION_END_FAIL = &H1100       ' 0x1100 , with sub-code
            End Enum
            Public Enum motionSubStateEnum
                SETUP_AND_GO
                WAITEXECUTION
                WAITTASKDONE
            End Enum

            Public Enum mainFailTypeEnum
                AMAX_ERROR = &H1000000
                DEVICE_ERROR = &H2000000
            End Enum
            Public Enum sencondFailTypeEnum
                ERROR_STOP = &H10000
                RETURN_ERROR = &H20000
                TIME_OUT = &H10000
            End Enum
#End Region
#Region "Properties"
            ' parameter interface
            Property SpeedOverrideRatio As Single = 1                   ' inherited from old library
            ReadOnly Property PulsePerUnit As Double                       ' read from pData
                '-------------------
                '   Hsien , 2014.11.18
                '-------------------
                Get
                    Return __motorSetting.PulsePerUnit
                End Get
            End Property
            WriteOnly Property AllowedSpeedRange As Double '= AmaxVelocityMax      ' v change usage
                Set(value As Double)
                    AMaxM4_Fix_Speed_Range(MotorIndex, value)
                End Set
            End Property

            'calculation tool
            Public Function pulse2Unit(ByVal pulse As Double) As Double
                Return pulse / __motorSetting.PulsePerUnit
            End Function
            Public Function unit2Pulse(ByVal unit As Double) As Double
                Return unit * __motorSetting.PulsePerUnit 'PulsePerUnit
            End Function
            Public Function enum2cPoint(__enum As [Enum]) As cMotorPoint
                'Hsien , 2015.01.19
                ' would be truncated after turns data-pair from (enum,short) to (enum,cMotorPoint)
                Return pData.MotorPoints(PositionDictionary(__enum))
            End Function
            ReadOnly Property Setting As cMotorSetting
                Get
                    Return __motorSetting
                End Get
            End Property
            Protected __motorSetting As cMotorSetting   ' belong to this motor
#Region "Path Table Settings / Simultanous move"
            'used to store motor point ready to execute iterationally , Hsien , 2014.09.27
            ReadOnly Property PointTable As List(Of cMotorPoint) ' = New List(Of cMotorPoint)   'inject the MotorPoints as simultanous commands
                Get
                    Return __pointTable
                End Get
                'Set(value As List(Of cMotorPoint))
                '    __pointTable = value
                'End Set
            End Property
            Protected __pointTable As List(Of cMotorPoint) = New List(Of cMotorPoint)
#End Region

            ' setup interface
            Property MotorIndex As Integer
                Get
                    Return __motorIndex
                End Get
                Set(value As Integer)
                    '--------------
                    '   BAD: coupled with pData
                    '--------------
                    If (pData.MotorSettings(value) IsNot Nothing) Then
                        __motorIndex = value
                        __motorSetting = pData.MotorSettings(value)
                        'PositionToleranceInUnit = 0.5  'default 0.5 units , i.e 0.5mm , 0.5deg
                    End If

                End Set
            End Property
            Protected __motorIndex As Integer = -1  ' to avoid non-mapping error happed , Hsien , 2016.03.09
            Public Delegate Function commandFunctionPrototype(ByRef subState As Short) As statusEnum
            <Browsable(False)>
            Property CommandFunctionDictionary As Dictionary(Of [Enum], commandFunctionPrototype) = New Dictionary(Of [Enum], commandFunctionPrototype)    ' managed all command functions
            Property PositionDictionary As Dictionary(Of [Enum], Short) = New Dictionary(Of [Enum], Short)  ' mapping the local position index to global position index
            Property TimeoutLimit As TimeSpan
                Get
                    Return timeOutTimer.TimerGoal
                End Get
                Set(value As TimeSpan)
                    timeOutTimer.TimerGoal = value
                End Set
            End Property

            ReadOnly Property ReturnError As returnErrorCodes
                Get
                    Return [Enum].ToObject(GetType(returnErrorCodes), Me.__returnStatus)
                End Get
            End Property
            ReadOnly Property MotionStatus As motionStatusEnum
                Get
                    Return [Enum].ToObject(GetType(motionStatusEnum), Me.__motionStatus)
                End Get
            End Property
            ReadOnly Property ErrorStatus As errorStatusEnum
                Get
                    Return [Enum].ToObject(GetType(errorStatusEnum), Me.__errorStatus)
                End Get
            End Property

            ' monitor interface
            ReadOnly Property ElapsedTime As TimeSpan
                Get
                    'Return timeOutCount
                    Return timeOutTimer.TimeElapsed
                End Get
            End Property
            ReadOnly Property ExecuteStatus As Object
                Get
                    Return commandInExecute
                End Get
            End Property
            ReadOnly Property CommandPosition As Integer
                Get
                    AMaxM4_Get_Command(Me.MotorIndex, Me.commandCounter)
                    Return commandCounter
                End Get
            End Property
            ReadOnly Property FeedBackPosition As Integer
                Get
                    AMaxM4_Get_Position(Me.MotorIndex, Me.feedBackCounter)
                    Return feedBackCounter
                End Get
            End Property
            ReadOnly Property ErrorPosition As Integer
                Get
                    AMaxM4_Get_Error_Counter(Me.MotorIndex, Me.errorCounter)
                    Return errorCounter
                End Get
            End Property
            Property AcceptableErrorStatus As errorStatusEnum
                Get
                    Return __accepatableErrorStatus
                End Get
                Set(ByVal value As errorStatusEnum)
                    __accepatableErrorStatus = value
                End Set
            End Property
            ReadOnly Property CurrentSpeed As Double
                Get
                    AMaxM4_Get_Current_Speed(Me.MotorIndex, __currentSpeed)
                    Return __currentSpeed
                End Get
            End Property

            ReadOnly Property CommandSubState As motionSubStateEnum
                Get
                    Return __commandSubState
                End Get
            End Property
            '------------------------------------
            '   Individual axis Slow-Down switch setting
            '   Hsien , 2015.01.19
            '------------------------------------
            Property SlowdownEnable As enableEnum
                Get
                    Return __sdEnabled
                End Get
                Set(value As enableEnum)
                    __sdEnabled = value
                    __returnStatus = AMaxM4_Set_SD_Enable(Me.MotorIndex,
                                             __sdEnabled,
                                             __motorSetting.SlowDownLevel,
                                             __sdLatchSetting,
                                             __sdModeSetting)

                End Set
            End Property
            Property SlowdownLatch As sdLatchEnum
                Get
                    Return __sdLatchSetting
                End Get
                Set(value As sdLatchEnum)
                    __sdLatchSetting = value
                    __returnStatus = AMaxM4_Set_SD_Enable(Me.MotorIndex,
                                            __sdEnabled,
                                            __motorSetting.SlowDownLevel,
                                            __sdLatchSetting,
                                            __sdModeSetting)
                End Set
            End Property
            Property SlowdownMode As sdModeEnum
                Get
                    Return __sdModeSetting
                End Get
                Set(value As sdModeEnum)
                    __sdModeSetting = value
                    __returnStatus = AMaxM4_Set_SD_Enable(Me.MotorIndex,
                                            __sdEnabled,
                                            __motorSetting.SlowDownLevel,
                                            __sdLatchSetting,
                                            __sdModeSetting)
                End Set
            End Property
            '---------------------------------------
            '   Latch Related 
            '---------------------------------------
            ReadOnly Property IsLatched As Boolean
                Get
                    'isLatched = not equal of previous/latched
                    'fetch latched data
                    AMaxM4_GetLatchData(Me.__motorIndex, latchNumberEnum.FEEDBACK_COUNTER, __latchedValue)
                    Return previousLatchedValue <> __latchedValue
                End Get
            End Property
            ''' <summary>
            ''' In Unit
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            ReadOnly Property LatchedValue As Double
                Get
                    Return Me.pulse2Unit(__latchedValue)
                End Get
            End Property
            '------------------------------------
            '   Synchron Master Mode switch setting
            '   Hsien , 2015.03.24
            '------------------------------------
            Property SynchronMode As syncMode = syncMode.IMMEDIATELY_START
            Property SynchonSignalMode As syncSignalMode = syncSignalMode.WHEN_START_ACCELERATION
            '------------------------------------
            '   Start All/Stop All hardware setting
            '   Hsien , 2015.01.19
            '------------------------------------
            Property StartAllMode As moveAllMode = moveAllMode.LEVEL
            Property StartAllRisingOrFalling As moveAllFallOrRise = moveAllFallOrRise.RISING_OR_HIGH
            Property StopAllMode As moveAllMode = moveAllMode.LEVEL
            Property StopAllRisingOrFalling As moveAllFallOrRise = moveAllFallOrRise.FALLING_OR_LOW
#End Region

            ''' <summary>
            ''' Event driven mechanism
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            ''' <remarks></remarks>
            Public Event CommandExecuted(ByVal sender As Object, ByVal e As EventArgs)

            Property PositionPoint As cMotorPoint 'Hsien , 2015.12.21, used when single position moving going to execute
                Get
                    If __pointTable.Count = 0 Then
                        __pointTable.Add(New cMotorPoint With {.AxisIndex = Me.MotorIndex})
                    End If

                    Return __pointTable.First
                End Get
                Set(value As cMotorPoint)
                    __pointTable.Clear()
                    __pointTable.Add(value)
                End Set
            End Property
            ' First Byte(MSB) - AMAXERROR or DEVICEERROR
            ' Seconde Byte - categrory
            ' Last WORD - reason
            Protected __motionStatus As UShort = 0
            Protected __returnStatus As Short = 0
            Protected __errorStatus As UInteger = 0
            Protected __accepatableErrorStatus As UInteger = 0           'Hsien , 2015.01.19 , the stop reason filter

            Protected timeOutTimer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 30)}
            Protected __timer As singleTimer = New singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}
            Protected __sdEnabled As enableEnum = enableEnum.DISABLE
            Protected __sdLatchSetting As sdLatchEnum = sdLatchEnum.DO_NOT_LATCH
            Protected __sdModeSetting As sdModeEnum = sdModeEnum.SLOW_DOWN_ONLY

            Protected commandCounter As Integer = 0
            Protected feedBackCounter As Integer = 0
            Protected errorCounter As Integer = 0
            Protected __currentSpeed As Double = 0

            Protected previousLatchedValue As Double = 0
            Protected __latchedValue As Double = 0

            ' control interface
            ReadOnly Property CommandEndStatus As statusEnum
                '--------------------
                '   Use to implement auto-reset mechanism
                ' Hsien , 2015.01.10
                '--------------------
                Get
                    If (commandInExecute.Equals(motorCommandEnum.WAIT_RECALL)) Then
                        resetCommand()
                    End If
                    Return __commandEndStatus
                End Get
            End Property
            Friend __commandEndStatus As statusEnum = statusEnum.EXECUTING
            'internal

            Protected __commandSubState As Short = 0                          ' shared by all command functions , command function should rewind this in the end of execution
            Protected commandInExecute As [Enum]
            Protected commandBeforePause As [Enum]          ' used to back the command in execute before motion paused , Hsien , 2014.09.14


            Public alarmPackage As alarmContextBase = Nothing
            'initializing
            Public Sub New()
                With CommandFunctionDictionary
                    .Add(motorCommandEnum.GO_HOME, AddressOf homeCommand)
                    .Add(motorCommandEnum.GO_POSITION, AddressOf synchronMasterMoveCommand)

                    .Add(motorCommandEnum.GO_POSITION_OPEN_LOOP, AddressOf synchronMasterMoveCommand)
                    .Add(motorCommandEnum.JOG, AddressOf jogCommand)

                    .Add(motorCommandEnum.STOP_FORCELY, Function() (statusEnum.EXECUTION_END))
                    .Add(motorCommandEnum.V_CHANGE, AddressOf vchangeCommand)    'Hsien , 2014.09.26

                    .Add(motorCommandEnum.GO_POSITION_COMBINED, AddressOf combinedOpenPositionCommand)    ' Hsien , 2014.09.26

                    '-------------------------------
                    '   Simultanous control
                    '-------------------------------
                    'CommandFunctionDictionary.Add(motorCommandEnum.START_MOVE, AddressOf simultanousMasterMoveCommand)
                    'CommandFunctionDictionary.Add(motorCommandEnum.START_MOVE_SLAVE, AddressOf simultanousSlaveMove)    'Hsien , 2015.01.23
                    '--------------------------------
                    '   Stop/Pause/Resume motion
                    '--------------------------------
                    '.Add(motorCommandEnum.WAIT_MOTION_STOP, AddressOf amaxWaitMotionStop)          'Hsien , 2015.01.23
                    .Add(motorCommandEnum.MOTION_PAUSE, Function() (statusEnum.EXECUTION_END))    ' Hsien , 2014.09.26
                    .Add(motorCommandEnum.MOTION_RESUME, Function() (statusEnum.EXECUTION_END))    ' Hsien , 2014.09.26
                    .Add(motorCommandEnum.STOP_SLOW_DOWN, AddressOf slowDownCommand)    ' Hsien ,2014.10.09

                    .Add(motorCommandEnum.SYNCHRON_MASTER, AddressOf synchronMasterMoveCommand)    ' Hsien ,2015.03.24
                End With

                commandInExecute = motorCommandEnum.NONE

                '------------------------------------
                '   Once not activated , start to run
                '------------------------------------
                If taskExecutor.Status = Tasks.TaskStatus.Created Then
                    taskExecutor.Start()
                End If
            End Sub

            '-------------------
            ' control interface
            '-------------------
            Public Function drive(ByVal command As motorCommandEnum, ByVal globalCommandPosition As cMotorPoint) As statusEnum
                'the extension alias function , hsien , 2015.01.05
                If (commandInExecute.Equals(motorCommandEnum.NONE)) Then
                    'avoid to corrupt the thread-shared data structure - pointTable , Hsien , 2016.02.15
                    PositionPoint = globalCommandPosition
                End If
                If __commandEndStatus = statusEnum.EXECUTION_END And Not commandInExecute.Equals(motorCommandEnum.NONE) And Me.MotorIndex = 4 Then
                    Console.WriteLine("")
                End If

                Return drive(command)
            End Function

            Public Function drive(ByVal command As [Enum], Optional ByVal commandPosition As [Enum] = Nothing) As statusEnum

                'Hsien , direct break and response dummy result , 2016.06.15
                If (Not IsEnabled) Then
                    Return __commandEndStatus
                End If

                '-------------------------------------------------------------
                '   when command UNDETERMINED , will do initializing procedure
                '-------------------------------------------------------------
                ' first priority command : STOP_FORCELY
                If (command.Equals(motorCommandEnum.STOP_FORCELY)) Then
                    '-----------------------------
                    ' highest priority command
                    'direct firing command
                    '------------------------------
                    __commandSubState = motionSubStateEnum.SETUP_AND_GO
                    commandInExecute = motorCommandEnum.NONE
                    __commandEndStatus = statusEnum.EXECUTION_END

                    'Hsien , 2014.10.09 , Slow down stop need time to down , have to use drive-command sequence control
                    '__returnStatus = AMaxM4_SlowDownStop(MotorIndex)
                    __returnStatus = AMaxM4_Soft_Emg_Stop(MotorIndex)

                    '----------------
                    '   Wait until motor stopped
                    '----------------
                    While (__motionStatus <> 0)
                        __returnStatus = AMaxM4_MotionDone(MotorIndex, __motionStatus)
                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If
                    End While

                    __returnStatus = AMaxM4_ErrorStatus(MotorIndex, __errorStatus)

                    Return statusEnum.EXECUTION_END

                End If

                '------------------
                '   PAUSE AND RESUME
                '------------------
                If (command.Equals(motorCommandEnum.MOTION_PAUSE) _
                    And Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE)) Then
                    pauseCommand()
                    commandBeforePause = commandInExecute
                    commandInExecute = motorCommandEnum.MOTION_PAUSE
                    Return statusEnum.EXECUTING
                End If

                If (commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) _
                   And Not command.Equals(motorCommandEnum.MOTION_RESUME)) Then
                    ' only resume command able to release pause command
                    Return statusEnum.EXECUTING 'reject
                End If

                If (Not commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) _
                  And command.Equals(motorCommandEnum.MOTION_RESUME)) Then
                    ' only resume command able to release pause command
                    Return statusEnum.EXECUTING 'reject , Hsien , 2014.10.20 , found that in section lock , the pause-resume pair may be decoupled
                End If

                If (commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE) _
                    And command.Equals(motorCommandEnum.MOTION_RESUME)) Then
                    resumeCommand()
                    commandInExecute = commandBeforePause
                    'Return statusEnum.EXECUTION_END
                    Return statusEnum.EXECUTING
                End If
                '------------------------
                '   STOP SLOWLY , Hsien  2015.01.10
                '-------------------------
                If (command.Equals(motorCommandEnum.STOP_SLOW_DOWN) And
                    (Not commandInExecute.Equals(motorCommandEnum.STOP_SLOW_DOWN)) And
                    (Not commandInExecute.Equals(motorCommandEnum.WAIT_RECALL))) Then
                    '-----------------------------
                    ' high priority command ,would override previous in-execute command
                    'Hsien , 2015.01.10
                    '------------------------------
                    resetCommand()
                    commandInExecute = motorCommandEnum.STOP_SLOW_DOWN
                    Return statusEnum.EXECUTING
                End If


                ' when command is executed to the end , do not accecpt next command 
                ' caller should determine if recall by recognizing return states in this device
                If (commandInExecute.Equals(motorCommandEnum.WAIT_RECALL)) Then
                    '----------------------------------
                    ' last command had been executed
                    '   1. rewind execution status
                    '   2. return the execution result 
                    '----------------------------------
                    commandInExecute = motorCommandEnum.NONE
                    Return __commandEndStatus
                End If

                If (Not commandInExecute.Equals(motorCommandEnum.NONE)) Then
                    ' command on execute , reject command override
                Else
                    If (commandPosition IsNot Nothing) Then
                        '----------------------------------------------------------------
                        'Hsien , 2014.11.14
                        'point indicated , fetch point data from pData tank
                        '----------------------------------------------------------------
                        Dim positionIndex As Integer = 0        ' position command use
                        If (PositionDictionary.TryGetValue(commandPosition, positionIndex)) Then
                            PositionPoint = pData.MotorPoints(positionIndex)
                        Else
                            '---------------------------------
                            '   Key Not Found , Reject Command
                            '---------------------------------
                            ' raising alarm : exception
                            __returnStatus = returnErrorCodes.ERR_No_Assigning_Point

                            alarmPackage = New alarmContentMotor With {.Sender = Me}
                            alarmPackage.CallbackResponse(alarmContextBase.responseWays.ABORT) = alarmContextBase.abortMethod   'Hsien , setup before raise alarm  , 2015.10.12
                            CentralAlarmObject.raisingAlarm(alarmPackage)
                            Return statusEnum.EXECUTION_END_FAIL
                        End If
                    Else
                        '----------------------------------------------------------------
                        'Hsien , 2014.11.14
                        '   No corresponding point indicated , use  positionPoint instead
                        '   if positionPoint not assigned , motor would generate error when executing
                        '----------------------------------------------------------------
                    End If

                    commandInExecute = command

                End If

                Return statusEnum.EXECUTING

            End Function

            ' internal
            Protected Overrides Function process() As Integer

                '---------------------------------
                'if alarmed , pause all motions? , answer : no , controlled by Host
                '---------------------------------

                '--------------------------
                '   drive the state-machine
                '--------------------------
                If (commandInExecute.Equals(motorCommandEnum.NONE) Or
                    commandInExecute.Equals(motorCommandEnum.WAIT_RECALL) Or
                    commandInExecute.Equals(motorCommandEnum.MOTION_PAUSE)) Then
                    ' performance considering
                    Return 0
                End If

                __commandEndStatus = CommandFunctionDictionary(commandInExecute)(__commandSubState)
                If (__commandEndStatus And statusEnum.EXECUTION_END) Then
                    ' bitwise operation
                    ' reset command
                    __commandSubState = motionSubStateEnum.SETUP_AND_GO
                    commandInExecute = motorCommandEnum.WAIT_RECALL
                    RaiseEvent CommandExecuted(Me, EventArgs.Empty)
                End If

                If (__commandEndStatus = statusEnum.EXECUTION_END_FAIL) Then
                    '----------------------------
                    'error occured
                    'raise alarm and rewind state
                    '----------------------------

                    'bug patch , to prevent missed end failed condition , Hsien  ,2016.05.16
                    If (alarmPackage Is Nothing) Then
                        alarmPackage = New alarmContentMotor With {.Sender = Me}
                    End If
                    alarmPackage.CallbackResponse(alarmContextBase.responseWays.ABORT) = alarmContextBase.abortMethod   'Hsien , setup before raise alarm  , 2015.10.12
                    CentralAlarmObject.raisingAlarm(alarmPackage)       'raising alarm
                    'stop motor and reset all status


                End If

                Return 0
            End Function

#Region "command functions"

            '--------------------------------
            '   Solo commands
            '--------------------------------
            Protected Function homeCommand(ByRef subState As Short) As statusEnum
                ' home process sub state-mahcine
                ' return as state
                Select Case subState
                    Case motionSubStateEnum.SETUP_AND_GO

                        Dim homePoint As cMotorPoint

                        ' clear state of MAmax_HoHome state-machine

                        'searching the corresponding point
                        homePoint = pData.MotorPoints.Find(Function(__point As cMotorPoint) (__point.IsHomePoint And __point.AxisIndex = __motorIndex))

                        __pointTable.Clear()
                        __pointTable.Add(homePoint)   'used to fetch waitStop command

                        If (homePoint Is Nothing) Then
                            __returnStatus = returnErrorCodes.ERR_No_Corresponding_Point
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                        ' set-up velocity profile
                        With homePoint

                            If (homePoint.VelocityProfile = velocityProfileEnum.S_CURVE) Then
                                __returnStatus = AMaxM4_SMovSet(__motorIndex,
                                                                .StartVelocity,
                                                                .Velocity,
                                                                .AccelerationTime,
                                                                .DecelerationTime,
                                                                0,
                                                                0)
                            Else
                                __returnStatus = AMaxM4_TMovSet(__motorIndex,
                                                                .StartVelocity,
                                                                .Velocity,
                                                                .AccelerationTime,
                                                                .DecelerationTime)
                            End If

                            If (__returnStatus <> 0) Then
                                Return statusEnum.EXECUTION_END_FAIL
                            End If


                            ' set-up home mode
                            __returnStatus = AMaxM4_HomeConfig(__motorIndex,
                                                               1,
                                                               __motorSetting.HomeLevel,
                                                               0,
                                                               0,
                                                               0)
                            If (__returnStatus <> 0) Then
                                Return statusEnum.EXECUTION_END_FAIL
                            End If


                            ' start home moving
                            __returnStatus = AMaxM4_HomeMove(__motorIndex,
                                                             .PointType,
                                                             .Distance)
                            If (__returnStatus <> 0) Then
                                Return statusEnum.EXECUTION_END_FAIL
                            End If
                        End With


                        '----------------------------
                        '   Use asynchronous thread to examine
                        '----------------------------
                        examinationList.Clear()
                        examinationList.AddRange({AddressOf amaxWaitMotionStop,
                                                  AddressOf amaxWaitFinePosition,
                                                  AddressOf amaxClearPosition})

                        subState = motionSubStateEnum.WAITEXECUTION
                    Case motionSubStateEnum.WAITEXECUTION
                        If taskList.TryAdd(Me, Nothing) Then
                            subState = motionSubStateEnum.WAITTASKDONE
                        End If
                    Case motionSubStateEnum.WAITTASKDONE
                        If Not taskList.ContainsKey(Me) Then
                            Return __result
                        End If

                End Select

                Return statusEnum.EXECUTING

            End Function
            Protected Function jogCommand(ByRef subState As Short) As statusEnum

                'Hsien , 2014.07.14
                ' follow the teaching speed
                If (PositionPoint Is Nothing) Then
                    __returnStatus = returnErrorCodes.ERR_No_Corresponding_Point
                    Return statusEnum.EXECUTION_END_FAIL
                End If

                If (PositionPoint.VelocityProfile = velocityProfileEnum.S_CURVE) Then
                    __returnStatus = AMaxM4_SMovSet2(Me.MotorIndex, PositionPoint)
                Else
                    __returnStatus = AMaxM4_TMovSet(Me.MotorIndex, PositionPoint.StartVelocity, PositionPoint.Velocity, PositionPoint.AccelerationTime, PositionPoint.DecelerationTime)
                End If

                If (__returnStatus <> 0) Then
                    Return statusEnum.EXECUTION_END_FAIL
                End If

                Dim dir As Short = 0
                If (PositionPoint.Distance > 0) Then
                    dir = 1
                Else
                    dir = 0
                End If

                '-----------
                '   1: forward
                '   2: backward
                ''----------
                __returnStatus = AMaxM4_VMov(Me.MotorIndex, dir) ' according to point setting

                If (__returnStatus <> 0) Then
                    Return statusEnum.EXECUTION_END_FAIL
                End If


                '------------------------------------------------
                '   Prevent JOG error , Hsien , 2015.10.16
                '------------------------------------------------
                __returnStatus = AMaxM4_MotionDone(MotorIndex, __motionStatus)
                __returnStatus = AMaxM4_ErrorStatus(MotorIndex, __errorStatus)

                If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
                    Return statusEnum.EXECUTION_END_FAIL
                End If

                If (__motionStatus = motionStatusEnum._STOP AndAlso
                    (__errorStatus And (Not __accepatableErrorStatus))) Then
                    '-----------------------------------
                    '   motion stopped , and not acceptable
                    '   Error Occured
                    '-----------------------------------
                    Return statusEnum.EXECUTION_END_FAIL    'abnormal stopped
                End If


                Return statusEnum.EXECUTION_END
            End Function
            Protected Function vchangeCommand(ByRef subState As Short) As statusEnum
                '--------------
                '   Hsien , 2014.09.26
                '--------------
                ' not works , need some time to let this setting avaialble
                '__returnStatus = AMaxM4_Fix_Speed_Range(Me.MotorIndex, Me.AllowedSpeedRange)

                'Hsien , 2014.11.14
                ' follow the assigned point
                If (PositionPoint Is Nothing) Then
                    __returnStatus = returnErrorCodes.ERR_No_Corresponding_Point
                    Return statusEnum.EXECUTION_END_FAIL
                End If


                __returnStatus = AMaxM4_V_Change(Me.MotorIndex _
                                                 , PositionPoint.Velocity _
                                                 , PositionPoint.AccelerationTime)
                If (__returnStatus <> 0) Then
                    Return statusEnum.EXECUTION_END_FAIL
                End If

                Return statusEnum.EXECUTION_END
            End Function
            Protected Function combinedOpenPositionCommand(ByRef subState As Short) As statusEnum

                '---------------------------
                '   Run combined velocity-position profile at once , according to pointsStored in pathtable
                '---------------------------

                Select Case subState
                    Case motionSubStateEnum.SETUP_AND_GO
                        ' reset register of slave
                        __returnStatus = AMaxM4_Reset_Path(Me.MotorIndex)

                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                        ' setup speed ( unknown purpose)
                        __returnStatus = AMaxM4_Set_Path_Move_Speed(Me.MotorIndex _
                                                                    , velocityProfileType.S_CURVE _
                                                                    , AmaxVelocityMin _
                                                                    , AmaxVelocityMax _
                                                                    , __pointTable(0).AccelerationTime _
                                                                    , __pointTable(0).DecelerationTime)
                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                        ' inject (queue-in) points into register
                        For Each __point As cMotorPoint In PointTable

                            If (__point.PointType = pointTypeEnum.ABS) Then 'abs move
                                __returnStatus = AMaxM4_Set_Path_Line_Data(Me.MotorIndex _
                                                                           , commandFunctionLineEnum.START_A_MOVE _
                                                                           , __point.Distance _
                                                                           , __point.StartVelocity _
                                                                           , __point.Velocity _
                                                                           , enableDisable.ENABLE)
                            Else
                                __returnStatus = AMaxM4_Set_Path_Line_Data(Me.MotorIndex _
                                                                           , commandFunctionLineEnum.START_R_MOVE _
                                                                           , __point.Distance _
                                                                           , __point.StartVelocity _
                                                                           , __point.Velocity _
                                                                           , enableDisable.ENABLE)
                            End If

                            If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
                                Return statusEnum.EXECUTION_END_FAIL
                            End If

                        Next

                        'execute
                        __returnStatus = AMaxM4_Start_Path(Me.MotorIndex)

                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If


                        examinationList.Clear()
                        examinationList.AddRange({AddressOf amaxWaitMotionStop,
                                                  AddressOf amaxWaitFinePosition})

                        subState = motionSubStateEnum.WAITEXECUTION
                    Case motionSubStateEnum.WAITEXECUTION
                        If taskList.TryAdd(Me, Nothing) Then
                            subState = motionSubStateEnum.WAITTASKDONE
                        End If
                    Case motionSubStateEnum.WAITTASKDONE
                        If Not taskList.ContainsKey(Me) Then
                            Return __result
                        End If

                End Select


                Return statusEnum.EXECUTING

            End Function
            ''-------------------------------
            ''   Simultanous control
            ''-------------------------------
            'Protected Function simultanousMasterMoveCommand(ByRef subState As Short) As statusEnum
            '    'Hsien , 2014.10.3
            '    ' move with slave motors
            '    Select Case subState
            '        Case motionSubStateEnum.SETUP_AND_GO
            '            '---------------------
            '            'Support Model - 124X Only
            '            'Hsien , 2015.01.26
            '            '---------------------
            '            __returnStatus = AMaxM4_Set_Move_All_Stop_Mode(Me.MotorIndex, StopAllMode, StopAllRisingOrFalling)

            '            If (__returnStatus <> 0) Then
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '            'SimultanouseSetting = SimultanouseSetting
            '            __returnStatus = tableSetup()
            '            If (__returnStatus <> 0) Then
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If
            '            '--------------------------------------------
            '            '   Command content must be setted before
            '            '--------------------------------------------
            '            __returnStatus = AMaxM4_Start_Move_All(Me.MotorIndex)

            '            If (__returnStatus <> 0) Then
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '            timeOutTimer.IsEnabled = True
            '            subState = motionSubStateEnum.WAITEXECUTION

            '        Case motionSubStateEnum.WAITEXECUTION
            '            'Hsien , 2015.03.23 ' should wait all simultanous move stop

            '            Dim isAllMotionDone As Boolean = PointTable.TrueForAll(Function(obj As cMotorPoint) As Boolean
            '                                                                       Dim __________motionStatus As UShort
            '                                                                       'accumulate all return status
            '                                                                       __returnStatus = __returnStatus Or AMaxM4_MotionDone(obj.AxisIndex, __________motionStatus)
            '                                                                       Return (__________motionStatus = motionStatusEnum._STOP) ' check if motion stopped
            '                                                                   End Function)
            '            '__returnStatus = AMaxM4_MotionDone(MotorIndex, __motionStatus)

            '            If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '            'If (__motionStatus = 0) Then
            '            If (isAllMotionDone) Then
            '                '-----------------------------------
            '                '   motion(all) stopped , go futher check (for all motor status
            '                '-----------------------------------
            '                For Each __point As cMotorPoint In PointTable

            '                    __returnStatus = AMaxM4_ErrorStatus(__point.AxisIndex, __errorStatus)

            '                    If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
            '                        Return statusEnum.EXECUTION_END_FAIL
            '                    End If

            '                    If (__errorStatus = 0 Or
            '                        __errorStatus = __accepatableErrorStatus) Then
            '                        '--------------------
            '                        '   Done Successfully , rewind
            '                        '--------------------
            '                        subState = motionSubStateEnum.SETUP_AND_GO
            '                        Return statusEnum.EXECUTION_END
            '                    Else
            '                        '---------------
            '                        '   Abnormally stopped
            '                        '---------------
            '                        Return statusEnum.EXECUTION_END_FAIL
            '                    End If

            '                Next

            '            ElseIf (timeOutTimer.IsTimerTicked) Then
            '                '----------------
            '                '   Error Occured
            '                '----------------
            '                __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '    End Select


            '    Return statusEnum.EXECUTING
            'End Function
            'Protected Function simultanousSlaveMove(ByRef subState As Short) As statusEnum
            '    'need check , Hsien , 2015.01.23
            '    'command fired by other motors , polling self-status until stopped

            '    Select Case subState
            '        Case motionSubStateEnum.SETUP_AND_GO
            '            '------------------------------
            '            '   Wait trigger
            '            '------------------------------
            '            __returnStatus = AMaxM4_MotionDone(Me.MotorIndex, __motionStatus)
            '            If (__returnStatus <> 0) Then
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '            If (__motionStatus = motionStatusEnum.ACCELERATING) Then
            '                subState = motionSubStateEnum.WAITEXECUTION
            '            End If

            '        Case motionSubStateEnum.WAITEXECUTION

            '            __returnStatus = AMaxM4_MotionDone(MotorIndex, __motionStatus)
            '            If (__motionStatus = 0) Then
            '                '--------------------
            '                '   Done , check reason
            '                '--------------------
            '                __returnStatus = AMaxM4_ErrorStatus(MotorIndex, __errorStatus)
            '                If (__errorStatus = 0 Or
            '                    (__errorStatus And __accepatableErrorStatus)) Then
            '                    '----------------------------------
            '                    '   Hsien , 2015.01.19
            '                    '   normal stopped , or stopped by acceptable reason are in expect
            '                    '----------------------------------
            '                    Return statusEnum.EXECUTION_END
            '                Else
            '                    Return statusEnum.EXECUTION_END_FAIL
            '                End If
            '            ElseIf (timeOutTimer.IsTimerTicked) Then
            '                '---------------------
            '                '   Wait until timeout
            '                '---------------------
            '                Return statusEnum.EXECUTION_END_FAIL
            '            End If

            '    End Select

            '    Return statusEnum.EXECUTING
            'End Function
            '--------------------------------
            '   Stop/Pause/Resume motion
            '--------------------------------
            Protected Function slowDownCommand(ByRef subState As Short) As statusEnum

                Select Case subState
                    Case motionSubStateEnum.SETUP_AND_GO
                        __returnStatus = AMaxM4_SlowDownStop(Me.MotorIndex)
                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                        'Hsien , raised only when wait stop method not invoked yet , 2016.02.15
                        If Not taskList.ContainsKey(Me) Then
                            'make sure the monitor thread not raised , Hsien , 2016.08.18
                            examinationList.Clear()
                            examinationList.Add(AddressOf amaxWaitMotionStop)

                            subState = motionSubStateEnum.WAITEXECUTION
                        Else
                            subState = motionSubStateEnum.WAITTASKDONE
                        End If
                    Case motionSubStateEnum.WAITEXECUTION
                        If taskList.TryAdd(Me, Nothing) Then
                            subState = motionSubStateEnum.WAITTASKDONE
                        End If
                    Case motionSubStateEnum.WAITTASKDONE
                        If Not taskList.ContainsKey(Me) Then
                            Return __result
                        End If

                End Select

                Return statusEnum.EXECUTING
            End Function

            Protected examinationList As List(Of Func(Of statusEnum)) = New List(Of Func(Of statusEnum))

            '-----------------------------------
            '   Thread Schedulaing
            '-----------------------------------
            ''' <summary>
            ''' Unit in ms
            ''' </summary>
            ''' <remarks></remarks>
            Protected travelTime As Integer = 100
            ''' <summary>
            ''' 0.5  Coefficient  1
            ''' </summary>
            ''' <remarks></remarks>
            Protected travelTimeCoefficient As Single = 0.8
            Shared taskExecutor As Tasks.Task = New Tasks.Task(AddressOf motorControl.taskQueryMotionStatus)
            Shared taskList As Concurrent.ConcurrentDictionary(Of motorControl, Object) = New Concurrent.ConcurrentDictionary(Of motorControl, Object)
            Shared dummyHandle As Object = Nothing

            Shared Sub taskQueryMotionStatus()
                While True
                    For Each item As motorControl In taskList.Keys
                        With item
                            If .examinationList.Count = 0 Then
                                'task complete
                                'once task done , remove from list
                                taskList.TryRemove(item, dummyHandle)

                            Else
                                'poll status
                                'break condition:
                                '1. first function returned EXECUTION_FAILED
                                '2. or last function returned EXECUTION_END
                                .__result = .examinationList.First.Invoke
                                Select Case .__result
                                    Case statusEnum.EXECUTION_END
                                        'remove current one
                                        .examinationList.Remove(.examinationList.First)
                                    Case statusEnum.EXECUTION_END_FAIL
                                        .examinationList.Clear() ' failed , no need to do further examination 
                                    Case statusEnum.EXECUTING
                                        '------------------
                                        '   Executing
                                        '------------------
                                End Select
                            End If
                        End With
                    Next
                    '------------------
                    '
                    '------------------
                    Thread.Yield()
                End While
            End Sub

            Friend __result As statusEnum = statusEnum.EXECUTING

            Protected Function amaxWaitMotionStop() As statusEnum
                '--------------------------------------------------------------------
                '   The  Motion Stop Check Routine both for Single/Multi PTP moving of amax
                '--------------------------------------------------------------------


                'Hsien , 2015.03.23 ' should wait all simultanous move stop


                Dim isAllMotionDone As Boolean = __pointTable.TrueForAll(Function(obj As cMotorPoint) As Boolean
                                                                             Dim eachMotionStatus As UShort
                                                                             'accumulate all return status
                                                                             __returnStatus = __returnStatus Or AMaxM4_MotionDone(obj.AxisIndex, eachMotionStatus)
                                                                             Return (eachMotionStatus = motionStatusEnum._STOP) ' check if motion stopped
                                                                         End Function)


                If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
                    Return statusEnum.EXECUTION_END_FAIL
                End If



                If (isAllMotionDone) Then
                    '-----------------------------------
                    '   motion(all) stopped , go futher check (for all motor status
                    '-----------------------------------
                    For Each __point As cMotorPoint In __pointTable

                        __returnStatus = AMaxM4_ErrorStatus(__point.AxisIndex, __errorStatus)


                        If (__returnStatus <> returnErrorCodes.ERR_NoError) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                        If (__errorStatus And (Not __accepatableErrorStatus)) <> 0 Then
                            '---------------
                            '   Abnormally stopped
                            '---------------
                            'corrected alarm message , Hsien , 2015.11.02
                            If __point.AxisIndex = Me.__motorIndex Then
                                'my axis error
                                alarmPackage = New alarmContentMotor With {.Sender = Me}
                            Else
                                'slave axis error
                                alarmPackage = New alarmContentMotor With {.Sender = New motorControl With {.MotorIndex = __point.AxisIndex,
                                                                                                            .__errorStatus = __errorStatus,
                                                                                                            .__motionStatus = 0}}
                            End If
                            Return statusEnum.EXECUTION_END_FAIL
                        End If

                    Next

                    '--------------------
                    '   Done Successfully , rewind
                    '--------------------
                    'all axis had no erros, Hsien , 2015.11.2
                    Return statusEnum.EXECUTION_END
                End If

                Return statusEnum.EXECUTING
            End Function
            Protected Function amaxWaitFinePosition() As statusEnum
                '--------------------------------------
                '   Hsien , 2015.11.02 , all position error have to converge below tolerance
                '--------------------------------------

                'once timer not enabled , start counting
                If Not timeOutTimer.IsEnabled Then
                    timeOutTimer.IsEnabled = True
                End If

                Dim isAllAxisApproched As Boolean = __pointTable.TrueForAll(Function(__point As cMotorPoint) As Boolean
                                                                                With __point
                                                                                    __returnStatus = AMaxM4_Get_Command(.AxisIndex, commandCounter)
                                                                                    __returnStatus = AMaxM4_Get_Position(.AxisIndex, feedBackCounter)
                                                                                    'inject next position
                                                                                    .NextPosition = feedBackCounter

                                                                                    Return .IsAccumulationEnough AndAlso
                                                                                        Math.Abs(commandCounter - .MaxPosition) < .PositionTolerance AndAlso
                                                                                        Math.Abs(commandCounter - .MinPosition) < .PositionTolerance
                                                                                End With
                                                                            End Function)


                If __returnStatus <> 0 OrElse
                    timeOutTimer.IsTimerTicked Then
                    '-------------------------------
                    '   Hardware failed or time's up
                    '-------------------------------    
                    __returnStatus = returnErrorCodes.ERR_Execution_Time_Out
                    timeOutTimer.IsEnabled = False
                    Return statusEnum.EXECUTION_END_FAIL
                ElseIf (isAllAxisApproched) Then
                    '---------------------
                    '   System in position
                    '---------------------
                    timeOutTimer.IsEnabled = False
                    Return statusEnum.EXECUTION_END
                End If

                Return statusEnum.EXECUTING
            End Function
            Protected Function amaxClearPosition() As statusEnum

                If Not __timer.IsEnabled Then
                    __timer.TimerGoal = New TimeSpan(0, 0, 1)
                    __timer.IsEnabled = True
                ElseIf __timer.IsTimerTicked Then
                    __returnStatus = AMaxM4_CmdPos_Reset(Me.__motorIndex)

                    If __returnStatus <> 0 Then
                        Return statusEnum.EXECUTION_END_FAIL
                    Else
                        Return statusEnum.EXECUTION_END
                    End If
                End If

                Return statusEnum.EXECUTING
            End Function
            '-------------------------------
            '   Sync motion
            '-------------------------------
            Protected Function synchronMasterMoveCommand(ByRef subState As Short) As statusEnum
                'Hsien , 2014.10.3
                ' move with slave motors
                Select Case subState
                    Case motionSubStateEnum.SETUP_AND_GO

                        '---------------------
                        'Support Model - 124X Only
                        'Hsien , 2015.01.26
                        '---------------------

                        'strategy:
                        '1. classfied points in pointTable into those one within in the same 1240 and others
                        '2. for those ones in the same 1240 , setup the synchron option
                        '3. for those ones in other 1240 , drive by go to position simualtanously


                        'reset , sync option
                        __pointTable.ForEach(Sub(__point As cMotorPoint) AMaxM4_SetSyncOption(__point.AxisIndex, syncMode.IMMEDIATELY_START))

                        If (__pointTable.Count > 1) Then
                            'there's other axis need to drive
                            __returnStatus = AMaxM4_SetSyncSignalMode(Me.MotorIndex, SynchonSignalMode)
                            If (__returnStatus <> 0) Then
                                Return statusEnum.EXECUTION_END_FAIL
                            End If
                        End If

                        '---------------------------
                        '   Latch Setup
                        'TODO , should suitable for multi-axis sychronized mode
                        '---------------------------
                        AMaxM4_SetLatchEnable(Me.__motorIndex, enableDisable.DISBALE) 'reset latch
                        AMaxM4_GetLatchData(Me.__motorIndex, latchNumberEnum.FEEDBACK_COUNTER, previousLatchedValue)

                        '=== 同步啟動初始化 ======
                        For Each __point As cMotorPoint In __pointTable
                            With __point
                                '---------------------------
                                '   Speed Setup
                                '---------------------------
                                If (.VelocityProfile = velocityProfileEnum.S_CURVE) Then
                                    __returnStatus = AMaxM4_SMovSet(.AxisIndex,
                                                                    .StartVelocity,
                                                                    .Velocity,
                                                                    .AccelerationTime,
                                                                    .DecelerationTime,
                                                                    .SShapeAccelerationTime,
                                                                    .SShapeDecelerationTime)
                                Else
                                    __returnStatus = AMaxM4_TMovSet(.AxisIndex,
                                                                    .StartVelocity,
                                                                    .Velocity,
                                                                    .AccelerationTime,
                                                                    .DecelerationTime)
                                End If

                                If (__returnStatus <> 0) Then
                                    Return statusEnum.EXECUTION_END_FAIL
                                End If

                                '--------------------------------------------------------
                                'setup synchron parameters , for those in the same device
                                '--------------------------------------------------------
                                If (.AxisIndex <> Me.MotorIndex And
                                    (pData.MotorSettings(.AxisIndex).DeviceIp = Me.__motorSetting.DeviceIp)) Then

                                    __returnStatus = AMaxM4_SetSyncSignalSource(.AxisIndex, Me.MotorIndex)
                                    If (__returnStatus <> 0) Then
                                        Return statusEnum.EXECUTION_END_FAIL
                                    End If

                                    __returnStatus = AMaxM4_SetSyncOption(.AxisIndex, SynchronMode)
                                    If (__returnStatus <> 0) Then
                                        Return statusEnum.EXECUTION_END_FAIL
                                    End If

                                    'slave axis motion setup
                                    If .PointType = pointTypeEnum.ABS Then  ' Abs-Move
                                        __returnStatus = AMaxM4_AMov(.AxisIndex, CInt(.Distance))
                                    Else ' Rel-Move
                                        __returnStatus = AMaxM4_RMov(.AxisIndex, CInt(.Distance))
                                    End If
                                    If (__returnStatus <> 0) Then
                                        Return statusEnum.EXECUTION_END_FAIL
                                    End If
                                Else
                                    '---------------
                                    '   My point
                                    '---------------
                                End If

                            End With
                        Next

                        '--------------------------------------------
                        '   Command content must be setted before
                        ' drive to go position
                        '--------------------------------------------
                        'find out those points satisfied:
                        '1. my point
                        '2. the point asssociated to the axis on the different device
                        Dim directDrivePoints As List(Of cMotorPoint) =
                            __pointTable.FindAll(Function(__point As cMotorPoint) ((__point.AxisIndex = Me.MotorIndex) Or
                                                                                   (pData.MotorSettings(__point.AxisIndex).DeviceIp <> Me.__motorSetting.DeviceIp) Or
                                                                                   (pData.MotorSettings(__point.AxisIndex).RingIndex <> Me.__motorSetting.RingIndex)))

                        directDrivePoints.ForEach(Sub(__point As cMotorPoint)
                                                      If (__point.PointType = pointTypeEnum.ABS) Then
                                                          __returnStatus = AMaxM4_AMov(__point.AxisIndex, CInt(__point.Distance))
                                                      Else
                                                          __returnStatus = AMaxM4_RMov(__point.AxisIndex, CInt(__point.Distance))
                                                      End If
                                                  End Sub)
                        If (__returnStatus <> 0) Then
                            Return statusEnum.EXECUTION_END_FAIL
                        End If


                        examinationList.Clear()
                        examinationList.Add(AddressOf amaxWaitMotionStop)
                        Select Case CType(commandInExecute, motorCommandEnum)
                            Case motorCommandEnum.GO_POSITION
                                examinationList.Add(AddressOf amaxWaitFinePosition)
                            Case motorCommandEnum.GO_POSITION_OPEN_LOOP
                                'no check fine position
                        End Select

                        '----------------------
                        'travel time estimation
                        ' Distance/Velocity * 1000 (since counting in ms) * coefficient
                        '----------------------
                        Dim __distance As Integer = 0
                        Select Case __pointTable.First.PointType
                            Case pointTypeEnum.ABS
                                __distance = Math.Abs(Me.CommandPosition - CInt(__pointTable.First.Distance))
                            Case pointTypeEnum.REL
                                __distance = Math.Abs(__pointTable.First.Distance)
                        End Select
                        'travelTime = CInt(Math.Truncate((__distance / __pointTable.First.Velocity) * 1000 * travelTimeCoefficient))
                        __timer.TimerGoal = New TimeSpan(0, 0, 0, 0, CInt(Math.Truncate((__distance / __pointTable.First.Velocity) * 1000 * travelTimeCoefficient)))
                        __timer.IsEnabled = True

                        subState = motionSubStateEnum.WAITEXECUTION

                    Case motionSubStateEnum.WAITEXECUTION
                        If (__timer.IsTimerTicked Or
                            SlowdownEnable = enableEnum.ENABLE) AndAlso
                        taskList.TryAdd(Me, Nothing) Then
                            'since slowdown enabled , the travel time is not constant
                            subState = motionSubStateEnum.WAITTASKDONE
                        End If
                    Case motionSubStateEnum.WAITTASKDONE
                        If Not taskList.ContainsKey(Me) Then
                            Return __result
                        Else
                            '-------------------
                            '   Waiting done
                            '-------------------
                        End If
                End Select

                Return statusEnum.EXECUTING

            End Function

            Protected Function pauseCommand() As Integer
                AMaxM4_PauseMotion(Me.MotorIndex)
                Return 0
            End Function
            Protected Function resumeCommand() As Integer
                AMaxM4_ResumeMotion(Me.MotorIndex)
                Return 0
            End Function

            Protected Overridable Function resetCommand() As Integer
                ' reset motor into initial stage (wait command)
                __commandSubState = 0
                If commandInExecute.Equals(motorCommandEnum.GO_POSITION) Then
                    Console.WriteLine()
                End If
                commandInExecute = motorCommandEnum.NONE
                'otherwise , next command may be skipped
                'Hsien  2014.07.14
                Return 0
            End Function

#End Region

            Public Overrides Function raisingGUI() As Control
                Return New userControlMotor With {.Motor = Me,
                                                 .PropertyView = MyBase.raisingGUI}
            End Function

            Protected Overrides Function enableDetail(arg As Boolean) As Integer
                If (arg) Then
                    'if enabled , set default command end status as EXECUTING
                    __commandEndStatus = statusEnum.EXECUTING
                Else
                    'if disabled , set default command end status as END (As Dummy Response)
                    __commandEndStatus = statusEnum.EXECUTION_END
                End If
                Return MyBase.enableDetail(arg)
            End Function

        End Class
    End Namespace


    Public Class motorControlDrivable
        Inherits motorControl
        Implements IDrivable
        'the IDrivable regular version of  motor control , Hsien , 2015.06.04

        Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
            Get
                If (CommandInExecute.Equals(motorControl.motorCommandEnum.WAIT_RECALL)) Then
                    Return IDrivable.drivenState.WAIT_RECALL
                ElseIf (CommandInExecute.Equals(motorControl.motorCommandEnum.NONE)) Then
                    Return IDrivable.drivenState.LISTENING
                Else
                    Return IDrivable.drivenState.EXECUTING
                End If
            End Get
        End Property

        Public Shadows ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
            Get
                'used interpreter to bridge
                Return __commandEndStatusInterpreter(MyBase.CommandEndStatus)
            End Get
        End Property

        Public Shadows ReadOnly Property CommandInExecute As Object Implements IDrivable.CommandInExecute
            Get
                Return MyBase.commandInExecute
            End Get
        End Property

        Public Shadows Function drive(command As [Enum], Optional ByVal commandPosition As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive
            'used shadows to hide base method , Hsien , 2015.06.04

            'explicit use overload function
            If (commandPosition IsNot Nothing AndAlso
                commandPosition.GetType() = GetType(cMotorPoint)) Then
                Return __commandEndStatusInterpreter(MyBase.drive(command, CType(commandPosition, cMotorPoint)))
            End If

            Return __commandEndStatusInterpreter(MyBase.drive(command, commandPosition))
        End Function

        Public Function getCommands() As ICollection Implements IDrivable.getCommands
            Return CommandFunctionDictionary.Keys
        End Function

        Public Property TimeoutLimit1 As TimeSpan Implements IDrivable.TimeoutLimit 'use less


        'used to interpret command
        Dim __commandEndStatusInterpreter As Dictionary(Of motorControl.statusEnum, IDrivable.endStatus) = New Dictionary(Of motorControl.statusEnum, IDrivable.endStatus)

        Sub New()
            With __commandEndStatusInterpreter
                .Add(statusEnum.EXECUTING, IDrivable.endStatus.EXECUTING)
                .Add(statusEnum.EXECUTION_END, IDrivable.endStatus.EXECUTION_END)
                .Add(statusEnum.EXECUTION_END_FAIL, IDrivable.endStatus.EXECUTION_END_FAIL)
            End With
        End Sub

        Public Overrides Function raisingGUI() As Control
            '----------------------------------------------------
            '   Hsien , 2015.02.05
            '----------------------------------------------------
            Dim uc As userControlDrivable = New userControlDrivable
            With uc
                .Component = Me
                .PropertyView = New userControlPropertyView With {.Drive = Me}
            End With
            Return uc
        End Function

    End Class


End Namespace
