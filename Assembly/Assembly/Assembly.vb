Option Strict Off
Imports Automation
Imports Automation.Components.Services
Imports Automation.mainIOHardware
Imports Automation.Components
Imports System.ComponentModel
Imports System.Net
Imports System.IO.Ports
Imports System.Text.RegularExpressions
Imports SmarPodAssembly
Imports Automation.Components.CommandStateMachine
Imports System.Linq
Imports FA.distanceMeter
Imports AutoNumeric

Public Enum controlUnitsEnum As Integer
    X = &H1
    Y = &H2
    Z = &H4
    S = &H8
End Enum

Public Enum dmTaskEnum As Integer
    TASK_EDGE_AUTO_PEAK = 0
    TASK_PLANE_HEIGHT = 1
End Enum

Public Enum workMode As Integer
    CALIBRATION
    BONDING
End Enum


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class controlUnitsEventArgs
    Inherits EventArgs

    ReadOnly Property Unit As controlUnitsEnum
        Get
            Return __unit
        End Get
    End Property
    ReadOnly Property Status As IDrivable.endStatus
        Get
            Return __status
        End Get
    End Property

    Dim __unit As controlUnitsEnum = controlUnitsEnum.X
    Dim __status As IDrivable.endStatus = IDrivable.endStatus.EXECUTING

    Sub New(__unit As controlUnitsEnum,
            status As IDrivable.endStatus)
        Me.__unit = __unit
        Me.__status = status
    End Sub

End Class

''' <summary>
''' Singleton
''' </summary>
''' <remarks></remarks>
Public Class Assembly
    Inherits assemblyArch
    Implements IOperational


    Property WorkMode As workMode
        Get
            Return __workMode
        End Get
        Set(value As workMode)
            Select Case value
                Case FA.workMode.BONDING
                    systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateBonding
                Case FA.workMode.CALIBRATION
                    systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateCalibration
            End Select
            __workMode = value
        End Set
    End Property
    Dim __workMode As workMode = FA.workMode.CALIBRATION

    Public Property OperationSignals As New flagController(Of operationSignalsEnum) Implements IOperational.OperationSignals

    ReadOnly Property CalibrationProcedures As List(Of procedureContext)
        Get
            Return calibrationProcessExecutor.ProcedureCollection
        End Get
    End Property

    Public Event SystemIgnited(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' Occured when XYZ motor start moving
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event UnitStatusChanged(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary>
    ''' Used to query status
    ''' </summary>
    ''' <param name="unit"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property CommandEndStatus(unit As controlUnitsEnum) As IDrivable.endStatus
        Get
            Return controlUnits(unit).CommandEndStatus
        End Get
    End Property

    ReadOnly Property Profile(unit As controlUnitsEnum) As cMotorPoint
        Get
            Return CType(controlUnits(unit), motorControl).PositionPoint
        End Get
    End Property
    ''' <summary>
    ''' Main worker thread available only
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsAllAxesSettled As Boolean
        Get
            Return axesSymbol.TrueForAll(Function(symbol As controlUnitsEnum) CommandEndStatus(symbol) = IDrivable.endStatus.EXECUTION_END)
        End Get
    End Property
    Dim axesSymbol As List(Of controlUnitsEnum) = New List(Of controlUnitsEnum)

    Friend analogChannels As List(Of [Enum]) = New List(Of [Enum])

    Property BondedMaterialData As List(Of materialData) = New List(Of materialData)
    Shared materialTotalCount As Integer = 9
    Dim currentMaterial As List(Of materialData).Enumerator = Nothing

    Friend settingDictionary As Dictionary(Of [Enum], settingBase) = New Dictionary(Of [Enum], settingBase)

#Region "sub systems"
    Dim hmiPauseButton As sensorControl = New sensorControl() With {.InputBit = inputAddress.PB_PAUSE,
                                                                    .IsEnabled = True}
    Dim hmiPauseLight As flipService = New flipService() With {.OutputBit = outputAddress.LP_PAUSE,
                                                               .FlipGoal = New TimeSpan(0, 0, 0, 0, 500)}
    Dim __timer As singleTimer = New singleTimer

    Friend gripperMount As sensorControl = New sensorControl With {.IsEnabled = True,
                                                                .InputBit = inputAddress.GRIP_MOUNT}
    Friend alarmPackGripperMount As alarmContentSensor = New alarmContentSensor With {.Sender = Me,
                                                                                    .Inputs = inputAddress.GRIP_MOUNT,
                                                                                    .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_OFF,
                                                                                    .PossibleResponse = alarmContextBase.responseWays.RETRY}
    Friend alarmPackCollision As alarmContextBase = New alarmContextBase With {.Sender = Me,
                                                                               .AdditionalInfo = "Collision Fault Detected"}
    '-----------------------------
    '   Measurments/Devices
    '--------------------------
    Friend __distanceMeter As distanceMeter = New distanceMeter("192.168.2.100:9601") With {.IsEnabled = True}
    Friend __lightController As lightControl = lightControl.Instance
    '--------------------------
    ' Coordinates control units
    '-----------------------------
    Friend WithEvents xMotorControl As motorControlDrivable = New motorControlDrivable With {.IsEnabled = False}
    Friend WithEvents yMotorControl As motorControlDrivable = New motorControlDrivable With {.IsEnabled = False}
    Friend WithEvents zMotorControl As motorControlDrivable = New motorControlDrivable With {.IsEnabled = False}
    Friend WithEvents __smarPodControl As smarPodControl = New smarPodControl With {.IsEnabled = False}
    Friend gripperClampControl As cylinderGeneric = New cylinderGeneric With {.IsEnabled = True,
                                                                           .ActuatorBit = outputAddress.GRIP_OPEN}
    Dim controlUnits As Dictionary(Of controlUnitsEnum, IDrivable) = New Dictionary(Of controlUnitsEnum, IDrivable)
#End Region

#Region "procedures"

    Dim calibrationProcessExecutor As procedureExecutor = New procedureExecutor With {.IsEnabled = True}
    Dim bondingProcessExecutor As procedureExecutor = New procedureExecutor With {.IsEnabled = True}

    Dim c1CalibrationProcess As cameraCalibrationBase = New cameraCalibrationBase(framesDefinition.C1, framesDefinition.C1REAL) With {.IsEnabled = True}
    Dim c2CalibrationProcess As cameraCalibrationBase = New cameraCalibrationBase(framesDefinition.C2, framesDefinition.C2REAL) With {.IsEnabled = True}
    Dim c3CalibrationProcess As cameraCalibrationBase = New cameraCalibrationBase(framesDefinition.C3, framesDefinition.C3REAL) With {.IsEnabled = True}

    Dim c4CalibrationProcess As c4Calibration = c4Calibration.Instance
    Dim dmCalibrationProcess As dmCalibration = dmCalibration.Instance
    Dim dispCalibrationProcess As dispCalibration = dispCalibration.Instance

    Dim lpcMarkProcess As lpcMark = lpcMark.Instance
    Dim dieMarkProcess As dieMark = dieMark.Instance
    Friend dryAlignProcess As energySearch = New energySearch(frames.Instance.Elementray(framesDefinition.DIE_REAL_DRY_REVISED,
                                                                                       framesDefinition.DIE_REAL_DRY)) With {.IsEnabled = True}

    Dim dispProcess As dispWorking = dispWorking.Instance
    Dim eproxyCuringProcess As eproxyCuring = eproxyCuring.Instance
    Friend wetAlignProcess As energySearch = New energySearch(frames.Instance.Elementray(framesDefinition.DIE_REAL_WET_REVISED,
                                                                                       framesDefinition.DIE_REAL_WET)) With {.IsEnabled = True}
    Dim curingProcess As eproxyCuring = eproxyCuring.Instance

#End Region

    Function initMappingAndSetup() As Integer

        Dim values = [Enum].GetValues(GetType(controlUnitsEnum))
        For index = 0 To values.Length - 1
            axesSymbol.Add([Enum].ToObject(GetType(controlUnitsEnum), values(index)))
        Next

        analogChannels.AddRange({inputAddress.PD_LEFT,
                                   inputAddress.PD_RIGHT,
                                    outputAddress.W1,
                                   outputAddress.W2,
                                    outputAddress.W3})

        '----------------------------
        '   Initialize Material Datas
        '----------------------------
        For index = 0 To materialTotalCount - 1
            Dim __data As materialData = New materialData(index)
            BondedMaterialData.Add(__data)
        Next


        '--------------------
        '   Raise singletons
        '---------------------
        Dim ref = frames.Instance

       

        'mount logger
        basicLogger.MessengerReference = CentralMessenger

        'door interlock , Hsien , 2015.05.14
        messageTimer.TimerGoal = New TimeSpan(0, 0, 1)
        With doorInterlock.SensorsNeedToCheck
        End With

        buzzer.OutputBit = outputAddress.BUZZER
        redTowerLight.OutputBit = outputAddress.LP_R
        yellowTowerLight.OutputBit = outputAddress.LP_Y
        greenTowerLight.OutputBit = outputAddress.LP_G

        pauseButtons.AddRange({hmiPauseButton})
        pauseLights.AddRange({hmiPauseLight})

        PauseBlock.listening = AddressOf pauseSense
        PauseBlock.uninterceptListening = AddressOf unpauseSense

        With xMotorControl
            .MotorIndex = motorAddress.MsX
            'z interlocked
            .SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
            .SlowdownLatch = sdLatchEnum.DO_NOT_LATCH
            '.SlowdownEnable = enableEnum.ENABLE
        End With
        With yMotorControl
            .MotorIndex = motorAddress.MsY
            'z interlocked
            .SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
            .SlowdownLatch = sdLatchEnum.DO_NOT_LATCH
            '.SlowdownEnable = enableEnum.ENABLE
        End With
        With zMotorControl
            .MotorIndex = motorAddress.MsZ
        End With
        With controlUnits
            .Add(controlUnitsEnum.X, xMotorControl)
            .Add(controlUnitsEnum.Y, yMotorControl)
            .Add(controlUnitsEnum.Z, zMotorControl)
            .Add(controlUnitsEnum.S, __smarPodControl)
        End With
        '----------------------------
        'Gather settings
        '----------------------------
        For Each item As measureProcedureType1Base In {c1CalibrationProcess,
                                                       c2CalibrationProcess,
                                                       c3CalibrationProcess,
                                                       c4CalibrationProcess,
                                                       dmCalibrationProcess,
                                                       dispCalibrationProcess,
                                                       lpcMarkProcess,
                                                       dieMarkProcess}
            settingDictionary(item.CorrespondingFrame) = item.MeasureSetting
        Next
        settingDictionary(framesDefinition.DIE_REAL_DRY_REVISED) = dryAlignProcess.setting
        settingDictionary(framesDefinition.DIE_REAL_WET_REVISED) = wetAlignProcess.setting
        settingDictionary(outputAddress.DISP_AUTO_CONTROL) = dispProcess.__dispSetting
        settingDictionary(motorAddress.MpUV) = curingProcess.__cureSetting
        '-------------------------------------
        '   Calibration Procedures
        '-------------------------------------
        With calibrationProcessExecutor.ProcedureCollection
            .Add(New procedureContext(Nothing, c1CalibrationProcess, "C1 Calibration"))
            .Add(New procedureContext(Nothing, c2CalibrationProcess, "C2 Calibration"))
            .Add(New procedureContext(Nothing, c3CalibrationProcess, "C3 Calibration"))
            .Add(New procedureContext(Nothing, c4Calibration.Instance, "C4 Calibration"))
            .Add(New procedureContext(Nothing, dmCalibration.Instance, "Distance Meter Calibration"))
            .Add(New procedureContext(Nothing, dispCalibration.Instance, "Dispention Head Calibration"))
        End With
        '-------------------------------------
        '   Calibration Procedures
        '-------------------------------------
        With bondingProcessExecutor.ProcedureCollection
            .Add(New procedureContext(Nothing, lpcMark.Instance))
            .Add(New procedureContext(Nothing, dieMark.Instance))
            .Add(New procedureContext(New Func(Of materialData)(Function() (currentMaterial.Current)), dryAlignProcess))

            .Add(New procedureContext(Nothing, dispWorking.Instance))
            .Add(New procedureContext(New Func(Of materialData)(Function() (currentMaterial.Current)), eproxyCuring.Instance))
            .Add(New procedureContext(New Func(Of materialData)(Function() (currentMaterial.Current)), wetAlignProcess))
        End With

        dryAlignProcess.Arguments = New materialData(0) ' dummy one
        wetAlignProcess.Arguments = New materialData(0) ' dummy one

        __lightController.IsEnabled = True
        '------------------------
        '   The Common Handler
        '------------------------
        AddHandler imageProcessSettingBlock.CameraTriggered, AddressOf lightAdjustingAndRunToolBlock

        Return 0

    End Function

    Function closeAssembly() As Integer
        '---------------------------
        '   Servo all motor off
        '---------------------------
        For index = 0 To [Enum].GetValues(GetType(motorAddress)).Length - 1
            AMaxM4_ServoOn(index, IS_OFF)
        Next
        Return 0
    End Function
    Function initStartMotors() As Integer
        'release alarm , servo-on , clear register...etc
        '---------------------------
        '   Servo all motor off
        '---------------------------
        Dim remainedErrorStatus As Integer
        For index = 0 To [Enum].GetValues(GetType(motorAddress)).Length - 1
            AMaxM4_ServoOn(index, pData.MotorSettings(index).ServoOnLevel)
            AMaxM4_ResetALM(index, IS_ON)
            AMaxM4_ErrorStatus(index, remainedErrorStatus)
        Next

        Threading.Thread.Sleep(1000)    'sleep for a while

        For index = 0 To [Enum].GetValues(GetType(motorAddress)).Length - 1
            AMaxM4_ResetALM(index, IS_OFF)
        Next

        AMaxM4_ServoOn(motorAddress.MsX, IS_ON)

        Return 0
    End Function

    Shared ReadOnly Property Instance As Assembly
        Get
            If __instance Is Nothing Then
                __instance = New Assembly
            End If
            Return __instance
        End Get
    End Property
    Shared WithEvents __instance As Assembly = Nothing

    Protected Sub New()
        Me.initialize = CType([Delegate].Combine(Me.initialize,
                                                 New Func(Of Integer)(AddressOf initLinkPause),
                                                 New Func(Of Integer)(AddressOf initMappingAndSetup),
                                                 New Func(Of Integer)(AddressOf initSubsystemInitialize),
                                                 New Func(Of Integer)(AddressOf initStartMotors)),
                                             Global.System.Func(Of Integer))

        'Hsien , define shutdown script
        Me.shutdown = [Delegate].Combine(New Func(Of Integer)(AddressOf closeDumpAlarms2Message),
                                         New Func(Of Integer)(AddressOf closeFlushAllMessages),
                                         New Func(Of Integer)(AddressOf closeAssembly),
                                         New Func(Of Integer)(Function() As Integer
                                                                  eproxyCuringProcess.Dispose()
                                                                  shutBits(GetType(outputAddress))
                                                                  Return 0
                                                              End Function))

        'state function configuration
        systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        systemMainStateFunctions(systemStatesEnum.IDLE) = AddressOf stateIdle
        systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateBonding
        systemMainState = systemStatesEnum.IGNITE

    End Sub

#Region "state functions"

    ''' <summary>
    ''' Return all axes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                '---------------------------------
                '   Wait until all hardware online
                '---------------------------------
                If (mainIOHardware.Instance.PhysicalHardwareList.TrueForAll(Function(__hardware As subHardwareNode) __hardware.Status = hardwareStatusEnum.HEALTHY)) Then
                    sendMessage(statusEnum.GENERIC_MESSAGE, "All hardware connected")
                    systemSubState = 10
                Else
                    '----------------------------------------
                    '   Some hardware failed , wait reconnect
                    '----------------------------------------
                End If
            Case 10
                If controlFlags.viewFlag(controlFlagsEnum.ABLE_IGNITE) And
                    gripperMount.OffTimer.TimeElapsed.TotalMilliseconds > 100 Then

                    controlUnits.Values.ToList.ForEach(Sub(driver As IDrivable) CType(driver, driveBase).IsEnabled = True)
                    'return z,smardpod first
                    zMotorControl.drive(motorControl.motorCommandEnum.GO_HOME)
                    __smarPodControl.drive(smarPodControl.podCommands.GO_HOME)

                    __distanceMeter.drive(distanceMeter.dmCommands.CONNECT)
                    writeBit(outputAddress.SYNRINGE, True) 'activate synringe


                    systemSubState = 20

                ElseIf controlFlags.viewFlag(controlFlagsEnum.ABLE_IGNITE) And
                    gripperMount.OffTimer.TimeElapsed.TotalMilliseconds < 100 Then
                    'alarm
                    CentralAlarmObject.raisingAlarm(alarmPackGripperMount)
                Else

                    '---------------------
                    '   Wait Command
                    '---------------------
                End If
            Case 20
                If zMotorControl.CommandEndStatus = IDrivable.endStatus.EXECUTION_END And
                    __smarPodControl.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then

                    'avoid collision
                    sHtm.Instance.ControlVector = sHtm.Instance.SafePose.Clone

                    yMotorControl.drive(motorControl.motorCommandEnum.GO_HOME)
                    xMotorControl.drive(motorControl.motorCommandEnum.GO_HOME)

                    systemSubState = 30
                Else
                    '--------------
                    '
                    '--------------
                End If
            Case 30
                If IsAllAxesSettled Then
                    systemSubState = 100
                Else
                    '--------------
                    '
                    '--------------
                End If
                '----------------------------
                '   Phase2 , to zero position
                '----------------------------
            Case 100
                'xyz go to zero position
                zMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION, pData.MotorPoints(motorPoints.MsZ_ZERO).Clone)
                systemSubState = 110
            Case 110
                If IsAllAxesSettled Then

                    xMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION, pData.MotorPoints(motorPoints.MsX_ZERO).Clone)
                    yMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION, pData.MotorPoints(motorPoints.MsY_ZERO).Clone)

                    systemSubState = 120
                Else
                    '--------------
                    '   
                    '--------------
                End If
            Case 120
                If IsAllAxesSettled  Then
                    'set to be zero as referenced position
                    For Each item As motorControlDrivable In {xMotorControl,
                                                               yMotorControl,
                                                               zMotorControl}
                        AMaxM4_CmdPos_Reset(item.MotorIndex)
                    Next

                    With frames.Instance
                        .CurrentMovingItem = framesDefinition.LREAL
                        .CurrentRItem = itemsDefinition.CHOKE_CORNER1
                    End With
                    curingProcess.FinishableFlags.setFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)

                    systemSubState = 500
                Else
                    '---------------
                    '   
                    '---------------
                End If
                '-------------------------------------
                '   Final
                '-------------------------------------
            Case 500
                If IsAllAxesSettled And
                    __distanceMeter.CommandEndStatus = IDrivable.endStatus.EXECUTION_END Then
                    systemSubState += 10
                Else
                    '---------------------------------
                    '   Settling
                    '---------------------------------
                End If
            Case 510
                If __distanceMeter.drive(distanceMeter.dmCommands.ZR, {dmTaskEnum.TASK_EDGE_AUTO_PEAK}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState += 10
                Else
                    '-------------------
                    '   Communicating
                    '-------------------
                End If
            Case 520
                If __distanceMeter.drive(distanceMeter.dmCommands.ZR, {dmTaskEnum.TASK_PLANE_HEIGHT}) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState += 10
                Else
                    '-------------------
                    '   Communicating
                    '-------------------
                End If
            Case 530
                If Not curingProcess.FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState += 10
                End If
            Case 540
                RaiseEvent SystemIgnited(Me, EventArgs.Empty)
                controlFlags.resetFlag(controlFlagsEnum.ABLE_IGNITE)
                systemMainState = systemStatesEnum.IDLE
        End Select

        Return 0
    End Function
    Function stateIdle() As Integer

        If OperationSignals.readFlag(operationSignalsEnum.__START) Then
            systemMainState = systemStatesEnum.EXECUTE
        End If

        Return 0
    End Function
    ''' <summary>
    ''' The modelling procedure collection
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateCalibration() As Integer
        systemMainState = calibrationProcessExecutor.MainState
        Return 0
    End Function

    ''' <summary>
    ''' The bonding procedure
    ''' 0. wait loading
    ''' 1. lpc mark
    ''' 2. die mark
    ''' 3. dry alignment
    ''' 4. dispenstion
    ''' 5. wet alignment
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateBonding() As Integer

        Select Case systemSubState
            Case 0
                If currentMaterial.MoveNext Then
                    'check if bonded checked

                    If currentMaterial.Current.IsEnagedToBond Then
                        'avoid collision
                        sHtm.Instance.ControlVector = sHtm.Instance.SafePose.Clone

                        'do further procedure
                        systemSubState += 10 ' go further
                    Else
                        'skip , do next bond
                    End If

                Else
                    '--------------------
                    '   All material done
                    '--------------------
                    systemMainState = systemStatesEnum.IDLE
                End If
            Case 10
                If CommandEndStatus(controlUnitsEnum.S) = IDrivable.endStatus.EXECUTION_END Then
                    With frames.Instance
                        .CurrentMovingItem = framesDefinition.S0
                        .CurrentRItem = itemsDefinition.C3_ORIGIN
                    End With
                    systemSubState += 10
                Else
                    '---------------
                    '   Settling
                    '---------------
                End If
            Case 20
                If IsAllAxesSettled Then
                    'drive to loading pose
                    sHtm.Instance.ControlVector = sHtm.Instance.LoadingPose.Clone
                    systemSubState += 10
                Else
                    '---------------
                    '   Settling
                    '---------------
                End If
            Case 30
                If CommandEndStatus(controlUnitsEnum.S) = IDrivable.endStatus.EXECUTION_END Then

                    'setup die coordinate
                    'clear :
                    'lpc_real
                    'die_real
                    'DIE_REAL_DRY
                    'DIE_REAL_DRY_REVISED
                    'DIE_REAL_WET
                    'DIE_REAL_WET_REVISED
                    dieHtm.Instance.DieIndex = currentMaterial.Current.IndexInArray
                    With frames.Instance
                        For Each item As htmEdgeElementary In {.Elementray(framesDefinition.LPC_REAL, framesDefinition.LPC),
                                                               .Elementray(framesDefinition.DIE_REAL_DRY, framesDefinition.DIE),
                                                               .Elementray(framesDefinition.DIE_REAL_DRY_REVISED, framesDefinition.DIE_REAL_DRY),
                                                               .Elementray(framesDefinition.DIE_REAL_WET, framesDefinition.DIE_REAL_DRY_REVISED),
                                                               .Elementray(framesDefinition.DIE_REAL_WET_REVISED, framesDefinition.DIE_REAL_WET)}
                            item.reset() 'reset real coordinate
                        Next

                    End With

                    'release gripper
                    writeBit(outputAddress.GRIP_VAC, False)
                    systemSubState += 10
                Else
                    '---------------
                    '   Settling
                    '---------------
                End If
            Case 40
                '------------------------
                '   Wait user respond
                '------------------------
                If gripperMount.OnTimer.TimeElapsed.TotalMilliseconds > 100 Then
                    bondingProcessExecutor.OperationSignals.setFlag(operationSignalsEnum.__START)
                    systemSubState += 10
                Else
                    'alarm
                    alarmPackGripperMount.Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON
                    CentralAlarmObject.raisingAlarm(alarmPackGripperMount)
                End If
            Case 50
                If bondingProcessExecutor.MainState = systemStatesEnum.IDLE Then
                    '---------------------
                    '   Done , do next one
                    '---------------------
                    systemSubState = 0
                Else
                    '----------------
                    'Working
                    '----------------
                End If

        End Select


        Return 0
    End Function
#End Region

    Dim WithEvents xManager As s0Htm = s0Htm.Instance
    Dim WithEvents yzManager As c4htm = c4htm.Instance
    Dim WithEvents sManager As sHtm = sHtm.Instance
    Sub transformationChanged(sender As Object, e As EventArgs) Handles xManager.TransformationChanged,
        yzManager.TransformationChanged,
        sManager.TransformationChanged

        'unable to be controlled before ignite phase
        If (MainState = systemStatesEnum.IGNITE And SubState = 0) And
           Not sender.Equals(sManager) Then
            Exit Sub
        End If

        Dim unit As controlUnitsEnum = controlUnitsEnum.X

        If sender.Equals(xManager) Then

            '------------------------------
            '   Prevent Collision Happend
            '------------------------------
            Dim collisionCondition As Boolean = False
            For Each item As framesDefinition In {framesDefinition.LPC_REAL,
                                                  framesDefinition.BALL,
                                                  framesDefinition.DISP_HEAD_REAL}
                With frames.Instance
                    collisionCondition = collisionCondition Or
                        .MovingItemCurrentPosition(item).Z <= .objectsDictionary(itemsDefinition.ANTI_COLLISION_EDGE_1).Z
                End With
            Next
            If collisionCondition Then
                CentralAlarmObject.raisingAlarm(alarmPackCollision)
                Exit Sub 'do not moving X
            End If

            With Profile(controlUnitsEnum.X)
                .DistanceInUnit = xManager.AxisValue(AutoNumeric.axisEntityEnum.X)
            End With
            xMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION)

            unit = controlUnitsEnum.X

        ElseIf sender.Equals(yzManager) Then
            With Profile(controlUnitsEnum.Y)
                .DistanceInUnit = yzManager.AxisValue(AutoNumeric.axisEntityEnum.Y)
            End With
            With Profile(controlUnitsEnum.Z)
                .DistanceInUnit = yzManager.AxisValue(AutoNumeric.axisEntityEnum.Z)
            End With
            yMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION)
            zMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION)

            unit = controlUnitsEnum.Y Or controlUnitsEnum.Z

        ElseIf sender.Equals(sManager) Then
            __smarPodControl.drive(smarPodControl.podCommands.GO_POSITION, sHtm.Instance.PodCommand)
            unit = controlUnitsEnum.S
        End If

        'inform relatives
        RaiseEvent UnitStatusChanged(Me, New controlUnitsEventArgs(unit, IDrivable.endStatus.EXECUTING))

    End Sub
    Sub controlDone(sender As IDrivable, e As EventArgs) Handles xMotorControl.CommandExecuted,
         yMotorControl.CommandExecuted,
         zMotorControl.CommandExecuted,
         __smarPodControl.CommandExecuted

        Dim unit As controlUnitsEnum = controlUnitsEnum.X

        If sender.Equals(xMotorControl) Then
            unit = controlUnitsEnum.X
        ElseIf sender.Equals(yMotorControl) Then
            unit = controlUnitsEnum.Y
        ElseIf sender.Equals(zMotorControl) Then
            unit = controlUnitsEnum.Z
        ElseIf sender.Equals(__smarPodControl) Then
            unit = controlUnitsEnum.S
        End If
        'inform relatives
        RaiseEvent UnitStatusChanged(sender, New controlUnitsEventArgs(unit,
                                                                       sender.CommandEndStatus))

    End Sub

    Public Overrides Sub alarmOccuredHandler(sender As alarmManager, e As alarmEventArgs)
        'mount retry option for motors
        Dim alarmMotor = TryCast(e.Content, alarmContentMotor)
        If alarmMotor IsNot Nothing Then
            alarmMotor.PossibleResponse += alarmContextBase.responseWays.RETRY
        End If

        MyBase.alarmOccuredHandler(sender, e)
    End Sub

    ''' <summary>
    ''' Execute the image process
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub lightAdjustingAndRunToolBlock(sender As Object, e As imageProcessTriggerEventArgs)
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   'light adjusting
                                                   With lightControl.Instance
                                                       .Intensity(e.LightChannel) = e.LightIntensity
                                                       .Intensity(e.LightChannel2) = e.LightIntensity2
                                                       .IsCommunicating = True
                                                       While .IsCommunicating
                                                           'do nothing , just polling
                                                           Threading.Thread.Yield()
                                                       End While
                                                   End With
                                                   'trigger tool block
                                                   e.ToolBlock.Run()
                                               End Sub)
    End Sub


End Class

