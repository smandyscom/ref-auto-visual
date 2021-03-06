﻿Imports System.Runtime.InteropServices
Imports System.Threading
Imports Automation.Components.Services
Imports System.Windows.Forms
Imports Automation
Imports System.ComponentModel
Imports SmarPodAssembly.SmarpodApiFuncs
Imports System.IO
Imports Automation.Components.CommandStateMachine

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class smarpodSetting
    Inherits settingBase

    Property Address As String = "192.168.2.200"
    Property Port As Integer = 5000
    Property Model As UInteger = 10019

    Property SensorMode As sensorModeEnum = sensorModeEnum.SMARPOD_SENSORS_POWERSAVE
    Property MaxFrequency As UInteger = 5000
    'Property UI parts
    Property FrefMethod As FREFMETHOD = FrefMethod.METHOD_ZSAFE
    Property FrefXDirection As FREF_DIRECTION = FREF_DIRECTION.X
    Property FrefYDirection As FREF_DIRECTION = FREF_DIRECTION.Y
    Property FrefZDirection As FREF_DIRECTION = FREF_DIRECTION.Z
    Property PivotMode As PIVOTMODES = PIVOTMODES.SMARPOD_PIVOT_RELATIVE
    ''' <summary>
    ''' In Hertz
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property FrefAndCalFrequency As UInt32 = 5000

    Property IsSpeedControlEnabled As enableDisableEnum = enableDisableEnum.ENABLE
    Property IsAccerlerationControlEnabled As enableDisableEnum = enableDisableEnum.ENABLE

End Class

''' <summary>
''' Tranlation Unit in mm
''' Rotation Unit in rad
''' </summary>
''' <remarks></remarks>
<EditorBrowsable(EditorBrowsableState.Always)>
<Editor(GetType(MotorPointTypeEditor), GetType(System.Drawing.Design.UITypeEditor))>
Public Class podCommandBase
    Implements ICloneable

    ReadOnly Property IsReached(target As podCommandBase) As Boolean
        Get
            Return (target - Me).Length < Tolerance
        End Get
    End Property
    ReadOnly Property Length As Double
        Get
            Dim __length As Double = 0
            For Each item As Double In {Px,
                                        Py,
                                        Pz,
                                        Rx,
                                        Ry,
                                        Rz}
                __length += Math.Pow(Math.Abs(Px), 2)
            Next

            Return Math.Sqrt(__length)
        End Get
    End Property


    Property Tolerance As Double = 0.0001

    Property Px As Double = 0
    Property Py As Double = 0
    Property Pz As Double = 0
    Property Rx As Double = 0
    Property Ry As Double = 0
    Property Rz As Double = 0

    ''' <summary>
    ''' In mm/s**2
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Accerleration As Double = 10
    ''' <summary>
    ''' In mm/s
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Speed As Double = 10

    Property HoldingTime As UInteger = holdTime.SMARPOD_HOLDTIME_INFINITE

    Public Function Clone() As Object Implements ICloneable.Clone
        Return Me.MemberwiseClone()
    End Function


    ReadOnly Property AccerlerationInMs2 As Double
        Get
            Return Accerleration / meter2mm
        End Get
    End Property
    ReadOnly Property SpeedInMs As Double
        Get
            Return Speed / meter2mm
        End Get
    End Property

    ''' <summary>
    ''' The native type structure for smarpod api
    '''  meter - mm
    '''  degree - rad
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property SmarpodPose As Smarpod_Pose
        Get
            Return New Smarpod_Pose With {.positionX = Px / meter2mm,
                                          .positionY = Py / meter2mm,
                                          .positionZ = Pz / meter2mm,
                                          .rotationX = rad2deg(Rx),
                                          .rotationY = rad2deg(Ry),
                                          .rotationZ = rad2deg(Rz)}
        End Get
        Set(value As Smarpod_Pose)
            With value
                Px = .positionX * meter2mm
                Py = .positionY * meter2mm
                Pz = .positionZ * meter2mm
                Rx = deg2rad(.rotationX)
                Ry = deg2rad(.rotationY)
                Rz = deg2rad(.rotationZ)
            End With
        End Set
    End Property

    Protected Function deg2rad(deg As Double) As Double
        Return (deg / 180) * Math.PI
    End Function
    Protected Function rad2deg(rad As Double) As Double
        Return (rad / Math.PI) * 180
    End Function
    Protected meter2mm As Integer = 1000

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return String.Format("{0},{1},{2},{3},{4},{5}",
                             Px,
                             Py,
                             Pz,
                             Rx,
                             Ry,
                             Rz)
    End Function

    ''' <summary>
    ''' Define pose difference
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Operator -(ByVal left As podCommandBase, right As podCommandBase) As podCommandBase
        Return New podCommandBase With {.Px = left.Px - right.Px,
                                        .Py = left.Py - right.Py,
                                        .Pz = left.Pz - right.Pz,
                                        .Rx = left.Rx - right.Rx,
                                        .Ry = left.Ry - right.Ry,
                                        .Rz = left.Rz - right.Rz}
    End Operator

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class smarPodControl
    Inherits systemControlPrototype
    Implements IDrivable
    Implements IDisposable

    ''' <summary>
    ''' Event driven mechanism
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event CommandExecuted(ByVal sender As Object, ByVal e As EventArgs)

    Property Configuration As smarpodSetting = New smarpodSetting

    Public Enum podCommands As Integer
        NONE = 0
        GO_HOME = 2
        GO_POSITION = 3
        CALIBRATE = 20
    End Enum

    Public Enum podSubStates As Integer
        DO_EXECUTION = 0
        DOING_EXECUTION = 1
        END_OF_EXECUTION
    End Enum

    Protected __commandInExecute As podCommands = podCommands.NONE
    Protected __commandDriveState As IDrivable.drivenState = IDrivable.drivenState.LISTENING
    Protected __commandSubState As podSubStates = podSubStates.DO_EXECUTION ' shared by all command functions , command function should rewind this in the end of execution
    Protected __commandEndStatus As IDrivable.endStatus = IDrivable.endStatus.EXECUTING
    Protected __commandDictionary As Dictionary(Of podCommands, IDrivable.commandFunctionPrototype) = New Dictionary(Of podCommands, IDrivable.commandFunctionPrototype)
    Protected __commandPose As podCommandBase = New podCommandBase

    Protected __returnValue As UInt32 = Status.SMARPOD_OK
    ''' <summary>
    ''' ID Fetched from connection procedure
    ''' </summary>
    ''' <remarks></remarks>
    Dim __smarpodId As UInteger = 0

    Public Property TimeoutLimit As TimeSpan Implements IDrivable.TimeoutLimit

#Region "Properties"
    ' 操作狀態
    Public ReadOnly Property ReturnValue As Status
        Get
            Return [Enum].ToObject(GetType(Status), __returnValue)
        End Get
    End Property
    Public ReadOnly Property MoveStatus As moveStatusEnum
        Get
            Dim __moveStatus As moveStatusEnum = moveStatusEnum.SMARPOD_STOPPED
            Smarpod_GetMoveStatus(__smarpodId,
                                  __moveStatus)
            Return __moveStatus
        End Get
    End Property
    ''' <summary>
    ''' Fetch detailed status description
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property StatusInfo As String
        Get
            Dim __ptr As IntPtr
            SmarpodApiFuncs.Smarpod_GetStatusInfo(__returnValue, __ptr)
            Return Marshal.PtrToStringAnsi(__ptr)
        End Get
    End Property
    ReadOnly Property SmarPodId As UInteger
        Get
            Return __smarpodId
        End Get
    End Property
    ''' <summary>
    ''' Record last command
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property CommandPose As podCommandBase
        Get
            Return __commandPose
        End Get
        Set(value As podCommandBase)
            __commandPose = value
        End Set
    End Property
    ''' <summary>
    ''' Direct fetch real pose
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property RealPose As podCommandBase
        Get
            Dim __realPose As podCommandBase = New podCommandBase
            Dim __pose As Smarpod_Pose = Nothing
            Smarpod_GetPose(__smarpodId,
                            __pose)
            __realPose.SmarpodPose = __pose
            Return __realPose
        End Get
    End Property

    Public ReadOnly Property CommandDrivenState As IDrivable.drivenState Implements IDrivable.CommandDrivenState
        Get
            Return __commandDriveState
        End Get
    End Property
    Public ReadOnly Property CommandEndStatus As IDrivable.endStatus Implements IDrivable.CommandEndStatus
        Get
            If __commandDriveState = IDrivable.drivenState.WAIT_RECALL Then
                __commandInExecute = podCommands.NONE
                __commandDriveState = IDrivable.drivenState.LISTENING
            End If
            Return __commandEndStatus
        End Get
    End Property
    Public ReadOnly Property CommandInExecute As Object Implements IDrivable.CommandInExecute
        Get
            Return __commandEndStatus
        End Get
    End Property
#End Region


    Protected Overrides Function process() As Integer
        '---------------------------
        '   Driven state-mahcine
        '---------------------------

        If __commandInExecute.Equals(podCommands.NONE) Then
            Return 0
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING,
                IDrivable.drivenState.WAIT_RECALL
                Return 0
            Case IDrivable.drivenState.EXECUTING
                '--------------------------------
                '   Executing
                '--------------------------------
                __commandEndStatus = __commandDictionary(__commandInExecute)()
                If (__commandEndStatus And IDrivable.endStatus.EXECUTION_END) Then
                    __commandSubState = podSubStates.DO_EXECUTION       'rewind
                    __commandDriveState = IDrivable.drivenState.WAIT_RECALL
                    RaiseEvent CommandExecuted(Me, EventArgs.Empty)

                    If (__commandEndStatus = IDrivable.endStatus.EXECUTION_END_FAIL) Then
                        '----------------------------
                        'error occured
                        'raise alarm and rewind state
                        '----------------------------
                        'bug patch , to prevent missed end failed condition , Hsien  ,2016.05.16
                        Dim alarmPackage = New alarmContextBase With {.Sender = Me}
                        With alarmPackage
                            .CallbackResponse(alarmContextBase.responseWays.ABORT) = alarmContextBase.abortMethod   'Hsien , setup before raise alarm  , 2015.10.12
                            .AdditionalInfo = String.Format("CommandPose{0},RealPose:{1},StatusInfo:{2},MoveStatus:{3}",
                                                            CommandPose,
                                                            RealPose,
                                                            StatusInfo,
                                                            MoveStatus)
                        End With
                        CentralAlarmObject.raisingAlarm(alarmPackage)       'raising alarm
                        'stop motor and reset all status
                    End If
                Else
                    '-------------------------
                    '   Executing
                    '-------------------------
                End If

        End Select

        Return 0
    End Function
    Public Function drive(command As [Enum], Optional __arg As Object = Nothing) As IDrivable.endStatus Implements IDrivable.drive

        'direct break and response dummy result
        If (Not IsEnabled) Then
            Return __commandEndStatus
        End If

        'Hsien , should cast into right type
        Dim __commandInPodCommand As podCommands = [Enum].ToObject(GetType(podCommands), command)
        Dim __podPose As podCommandBase = TryCast(__arg, podCommandBase)
        If __commandInPodCommand.Equals(Nothing) Then
            'error , cannot casting , command reject
            Return IDrivable.endStatus.EXECUTION_END_FAIL
        End If

        Select Case __commandDriveState
            Case IDrivable.drivenState.LISTENING
                '------------------------------
                '   Able to accept command
                '-------------------------------
                __commandInExecute = __commandInPodCommand
                If __podPose IsNot Nothing Then
                    __commandPose = __podPose
                End If
                __commandDriveState = IDrivable.drivenState.EXECUTING
            Case IDrivable.drivenState.WAIT_RECALL
                '------------------------
                '   Last command had beed executed , this cycle used to rewind
                '------------------------
                __commandDriveState = IDrivable.drivenState.LISTENING
                __commandInExecute = podCommands.NONE
                Return __commandEndStatus
            Case IDrivable.drivenState.EXECUTING
                '--------------------
                '   Do nothing
                '--------------------
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

#Region "command functions"

    Function goHomeCommand() As IDrivable.endStatus

        Select Case __commandSubState
            Case podSubStates.DO_EXECUTION
                Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                           __returnValue = Smarpod_FindReferenceMarks(Me.__smarpodId)
                                                           While True
                                                               If MoveStatus <> moveStatusEnum.SMARPOD_REFERENCING Then
                                                                   __commandSubState = podSubStates.END_OF_EXECUTION
                                                                   Exit While
                                                               End If
                                                           End While
                                                       End Sub)
                __commandSubState = podSubStates.DOING_EXECUTION
            Case podSubStates.DOING_EXECUTION
                '----------------------
                '   Just waiting
                '----------------------
            Case podSubStates.END_OF_EXECUTION
                Dim __isReferenced As Integer = 0
                __returnValue = Smarpod_IsReferenced(Me.__smarpodId, __isReferenced)
                If __isReferenced = 0 Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                Else
                    Return IDrivable.endStatus.EXECUTION_END
                End If
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function
    Function goPositionCommand() As IDrivable.endStatus

        Select Case __commandSubState
            Case podSubStates.DO_EXECUTION
                'check if pose is reachable
                Dim __isReachable As Integer = 0
                __returnValue = Smarpod_IsPoseReachable(Me.__smarpodId,
                                                        __commandPose.SmarpodPose,
                                                        __isReachable)

                If __returnValue <> Status.SMARPOD_OK Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If

                If __isReachable = 0 Then
                    __returnValue = Status.SMARPOD_POSE_UNREACHABLE_ERROR
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If

                'speed,acce setup
                __returnValue = Smarpod_SetSpeed(__smarpodId,
                                                 Configuration.IsSpeedControlEnabled,
                                                 __commandPose.SpeedInMs)
                If __returnValue <> Status.SMARPOD_OK Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                __returnValue = Smarpod_SetAcceleration(__smarpodId,
                                                        Configuration.IsAccerlerationControlEnabled,
                                                        __commandPose.AccerlerationInMs2)
                If __returnValue <> Status.SMARPOD_OK Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If

                'start moving
                Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                           __returnValue = Smarpod_Move(Me.__smarpodId,
                                                                                         __commandPose.SmarpodPose,
                                                                                         __commandPose.HoldingTime,
                                                                                         waitForCompletion.SYCHRON_MODE)
                                                           While True
                                                               If MoveStatus <> moveStatusEnum.SMARPOD_MOVING Then
                                                                   __commandSubState = podSubStates.END_OF_EXECUTION
                                                                   Exit While
                                                               End If
                                                           End While
                                                       End Sub)
                __commandSubState = podSubStates.DOING_EXECUTION
            Case podSubStates.DOING_EXECUTION
                '----------------------
                '   Just waiting
                '----------------------
            Case podSubStates.END_OF_EXECUTION
                Return IDrivable.endStatus.EXECUTION_END

        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

    Function calibrationCommand() As IDrivable.endStatus

        Select Case __commandSubState
            Case podSubStates.DO_EXECUTION
                Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                           __returnValue = Smarpod_Calibrate(Me.__smarpodId)
                                                           __commandSubState = podSubStates.END_OF_EXECUTION
                                                       End Sub)
            Case podSubStates.END_OF_EXECUTION
                If __returnValue <> Status.SMARPOD_OK Then
                    Return IDrivable.endStatus.EXECUTION_END_FAIL
                End If
                Return IDrivable.endStatus.EXECUTION_END
        End Select

        Return IDrivable.endStatus.EXECUTING
    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If
            SmarpodApiFuncs.Smarpod_Close(__smarpodId)
            Configuration.Save()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "GUI Reflection"
    Public Overrides Function raisingGUI() As Control
        Dim uc As userControlDrivable = New userControlDrivable
        With uc
            .Component = Me
            .PropertyView = New userControlPropertyView With {.Drive = Me}
        End With
        Return uc
    End Function
    Public Function getCommands() As ICollection Implements IDrivable.getCommands
        Return __commandDictionary.Keys
    End Function
#End Region


    Sub New()

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initMappingAndSetup))
        With __commandDictionary
            .Add(podCommands.GO_HOME, AddressOf goHomeCommand)
            .Add(podCommands.GO_POSITION, AddressOf goPositionCommand)
            .Add(podCommands.CALIBRATE, AddressOf calibrationCommand)
        End With
    End Sub
    Function initMappingAndSetup() As Integer

        Try

            '' reflect dll info
            Dim major As UInteger = 0
            Dim minor As UInteger = 0
            Dim update As UInteger = 0
            __returnValue += SmarpodApiFuncs.Smarpod_GetDLLVersion(major, minor, update)
            sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("SmarPod Version:{0},{1},{2}",
                                                                   major,
                                                                   minor,
                                                                   update))
            'Open and link
            __returnValue += SmarpodApiFuncs.Smarpod_Open(__smarpodId,
                                                         Configuration.Model,
                                                        String.Format("network:{0}:{1}",
                                                                      Configuration.Address,
                                                                      Configuration.Port),
                                                        "")
            '--------------
            'Configurations ,load setting and configuration
            '--------------
            With Configuration

                'try fetching smarpod_setting.xml
                Dim fi As FileInfo = New FileInfo(My.Application.Info.DirectoryPath & "/Data/smarpod_setting.xml")
                If fi.Exists Then
                    .Load(fi.FullName)
                Else
                    .Create(fi.FullName)
                End If

                __returnValue += Smarpod_SetMaxFrequency(__smarpodId, .MaxFrequency)
                __returnValue += Smarpod_SetSensorMode(__smarpodId, .SensorMode)

                __returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_FREF_METHOD, .FrefMethod)
                '__returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_FREF_XDIRECTION, .FrefXDirection)
                '__returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_FREF_YDIRECTION, .FrefYDirection)
                '__returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_FREF_ZDIRECTION, .FrefZDirection)
                __returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_PIVOT_MODE, .PivotMode)
                __returnValue += Smarpod_Set_ui(__smarpodId, PROPERTYSYMBOLS.SMARPOD_FREF_AND_CAL_FREQUENCY, .FrefAndCalFrequency)

                If __returnValue <> Status.SMARPOD_OK Then
                    'some error occured
                    MessageBox.Show("SmardPod Error")
                End If
            End With
        Catch ex As Exception

        End Try

        Return 0
    End Function

    Protected Overrides Function enableDetail(arg As Boolean) As Integer
        If (arg) Then
            'if enabled , set default command end status as EXECUTING
            __commandEndStatus = IDrivable.endStatus.EXECUTING
        Else
            'if disabled , set default command end status as END (As Dummy Response)
            __commandEndStatus = IDrivable.endStatus.EXECUTION_END
        End If
        Return MyBase.enableDetail(arg)
    End Function

End Class