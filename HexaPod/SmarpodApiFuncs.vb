﻿Imports System.Runtime.InteropServices

''' <summary>
'''  in meters,degrees
''' </summary>
''' <remarks></remarks>
Public Structure Smarpod_Pose
    Public positionX As Double
    Public positionY As Double
    Public positionZ As Double
    Public rotationX As Double
    Public rotationY As Double
    Public rotationZ As Double
End Structure

Public Class SmarpodApiFuncs
    Enum PROPERTYSYMBOLS
        SMARPOD_FREF_METHOD = 1000
        SMARPOD_FREF_ZDIRECTION = 1002
        SMARPOD_FREF_XDIRECTION = 1003
        SMARPOD_FREF_YDIRECTION = 1004
        SMARPOD_PIVOT_MODE = 1010
        SMARPOD_FREF_AND_CAL_FREQUENCY = 1020
    End Enum
    Enum FREFMETHOD As UInt32
        METHOD_DEFAULT = 0
        METHOD_SEQUENTIAL = 1
        METHOD_ZSAFE = 2
        METHOD_XYSAFE = 3
    End Enum
    Enum FREF_DIRECTION As UInt32
        X = &H1
        Y = &H2
        Z = &H4
        POSITIVE = &H100
        NEGTIVE = &H200
        REVERSE = &H1000
    End Enum
    Enum PIVOTMODES As UInt32
        SMARPOD_PIVOT_RELATIVE = 0
        SMARPOD_PIVOT_FIXED = 1
    End Enum
    Enum Status As UInt32
        SMARPOD_OK = 0
        SMARPOD_OTHER_ERROR = 1
        SMARPOD_SYSTEM_NOT_INITIALIZED_ERROR = 2
        SMARPOD_NO_SYSTEMS_FOUND_ERROR = 3
        SMARPOD_INVALID_PARAMETER_ERROR = 4
        SMARPOD_COMMUNICATION_ERROR = 5
        SMARPOD_UNKNOWN_PROPERTY_ERROR = 6
        SMARPOD_RESOURCE_TOO_OLD_ERROR = 7
        SMARPOD_FEATURE_UNAVAILABLE_ERROR = 8
        SMARPOD_INVALID_SYSTEM_LOCATOR_ERROR = 9
        SMARPOD_QUERYBUFFER_SIZE_ERROR = 10
        SMARPOD_COMMUNICATION_TIMEOUT_ERROR = 11
        SMARPOD_DRIVER_ERROR = 12

        SMARPOD_STATUS_CODE_UNKNOWN_ERROR = 500
        SMARPOD_INVALID_ID_ERROR = 501
        SMARPOD_INITIALIZED_ERROR = 502
        SMARPOD_HARDWARE_MODEL_UNKNOWN_ERROR = 503
        SMARPOD_WRONG_COMM_MODE_ERROR = 504
        SMARPOD_NOT_INITIALIZED_ERROR = 505
        SMARPOD_INVALID_SYSTEM_ID_ERROR = 506
        SMARPOD_NOT_ENOUGH_CHANNELS_ERROR = 507
        SMARPOD_INVALID_CHANNEL_ERROR = 508
        SMARPOD_CHANNEL_USED_ERROR = 509
        SMARPOD_SENSORS_DISABLED_ERROR = 510
        SMARPOD_WRONG_SENSOR_TYPE_ERROR = 511
        SMARPOD_SYSTEM_CONFIGURATION_ERROR = 512
        SMARPOD_SENSOR_NOT_FOUND_ERROR = 513
        SMARPOD_STOPPED_ERROR = 514
        SMARPOD_BUSY_ERROR = 515

        SMARPOD_NOT_REFERENCED_ERROR = 550
        SMARPOD_POSE_UNREACHABLE_ERROR = 551
        SMARPOD_COMMAND_OVERRIDDEN_ERROR = 552
        SMARPOD_ENDSTOP_REACHED_ERROR = 553
        SMARPOD_NOT_STOPPED_ERROR = 554
        SMARPOD_COULD_NOT_REFERENCE_ERROR = 555
    End Enum


    Enum sensorModeEnum As UInt32
        SMARPOD_SENSORS_DISABLED = 0
        SMARPOD_SENSORS_ENABLED = 1
        SMARPOD_SENSORS_POWERSAVE = 2
    End Enum
    Enum moveStatusEnum As UInt32
        SMARPOD_STOPPED = 0
        SMARPOD_HOLDING = 1
        SMARPOD_MOVING = 2
        SMARPOD_CALIBRATING = 3
        SMARPOD_REFERENCING = 4
    End Enum
    Enum holdTime As UInt32
        SMARPOD_HOLDTIME_INFINITE = 60000
    End Enum
    Enum waitForCompletion As Int32
        ''' <summary>
        ''' Smarpod_Move does not return until all positioners have stopped moving
        ''' </summary>
        ''' <remarks></remarks>
        SYCHRON_MODE = 1
        ''' <summary>
        ''' Smarpod_Move function returns immediately
        ''' </summary>
        ''' <remarks></remarks>
        ASYNCHRON_MODE = 0
    End Enum
    Enum enableDisableEnum As Int32
        ENABLE = 1
        DISABLE = 0
    End Enum



    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetDLLVersion")> _
    Public Shared Function Smarpod_GetDLLVersion(ByRef major As UInteger, ByRef minor As UInteger, ByRef update As UInteger) As UInt32
    End Function

    ''' <summary>
    ''' Return Texture Description
    ''' </summary>
    ''' <param name="status"></param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetStatusInfo")> _
    Public Shared Function Smarpod_GetStatusInfo(status As UInteger, ByRef name As IntPtr) As UInt32
    End Function

    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetModels")> _
    Public Shared Function Smarpod_GetModels(ByRef modelList As UInteger, ByRef ioListSize As UInteger) As UInt32
    End Function

    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetModelName")> _
    Public Shared Function Smarpod_GetModelName(model As UInteger, ByRef name As IntPtr) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Open")> _
    Public Shared Function Smarpod_Open(ByRef smarpodId As UInteger, ByVal model As UInteger, ByVal sysLocator As String, options As String) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Close")> _
    Public Shared Function Smarpod_Close(ByVal smarpodId As UInteger) As UInt32
    End Function

    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_FindSystems")> _
    Public Shared Function Smarpod_FindSystems(ByRef options As String, ByRef outBuffer As Char, ByRef ioBufferSize As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetSystemLocator")> _
    Public Shared Function Smarpod_GetSystemLocator(ByVal smarpodId As UInteger, ByRef outBuffer As String, ByRef ioBufferSize As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_ConfigureSystem")> _
    Public Shared Function Smarpod_ConfigureSystem(ByVal smarpodId As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Set_ui")> _
    Public Shared Function Smarpod_Set_ui(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByVal value As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Set_i")> _
    Public Shared Function Smarpod_Set_i(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByVal value As Integer) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Set_d")> _
    Public Shared Function Smarpod_Set_d(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByVal value As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Get_ui")> _
    Public Shared Function Smarpod_Get_ui(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByRef value As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Get_i")> _
    Public Shared Function Smarpod_Get_i(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByRef value As Integer) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Get_d")> _
    Public Shared Function Smarpod_Get_d(ByVal smarpodId As UInteger, ByVal _property As UInteger, ByRef value As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_SetSensorMode")> _
    Public Shared Function Smarpod_SetSensorMode(ByVal smarpodId As UInteger, ByVal mode As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetSensorMode")> _
    Public Shared Function Smarpod_GetSensorMode(ByVal smarpodId As UInteger, ByRef mode As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_SetMaxFrequency")> _
    Public Shared Function Smarpod_SetMaxFrequency(ByVal smarpodId As UInteger, ByVal mode As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetMaxFrequency")> _
    Public Shared Function Smarpod_GetMaxFrequency(ByVal smarpodId As UInteger, ByRef mode As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_SetSpeed")> _
    Public Shared Function Smarpod_SetSpeed(ByVal smarpodId As UInteger, ByVal speedControl As Integer, ByVal speed As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetSpeed")> _
    Public Shared Function Smarpod_GetSpeed(ByVal smarpodId As UInteger, ByRef speedControl As Integer, ByRef speed As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_SetAcceleration")> _
    Public Shared Function Smarpod_SetAcceleration(ByVal smarpodId As UInteger, ByVal accelControl As Integer, ByVal acceleration As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetAcceleration")> _
    Public Shared Function Smarpod_GetAcceleration(ByVal smarpodId As UInteger, ByRef accelControl As Integer, ByRef acceleration As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_FindReferenceMarks")> _
    Public Shared Function Smarpod_FindReferenceMarks(ByVal smarpodId As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Calibrate")> _
    Public Shared Function Smarpod_Calibrate(ByVal smarpodId As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_IsReferenced")> _
    Public Shared Function Smarpod_IsReferenced(ByVal smarpodId As UInteger, ByRef referenced As Integer) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_SetPivot")> _
    Public Shared Function Smarpod_SetPivot(ByVal smarpodId As UInteger, ByRef pivot As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetPivot")> _
    Public Shared Function Smarpod_GetPivot(ByVal smarpodId As UInteger, ByRef pivot As Double) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_IsPoseReachable")> _
    Public Shared Function Smarpod_IsPoseReachable(ByVal smarpodId As UInteger, ByRef pose As Smarpod_Pose, ByRef reachable As Integer) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetPose")> _
    Public Shared Function Smarpod_GetPose(ByVal smarpodId As UInteger, ByRef pose As Smarpod_Pose) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_GetMoveStatus")> _
    Public Shared Function Smarpod_GetMoveStatus(ByVal smarpodId As UInteger, ByRef status As moveStatusEnum) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Move")> _
    Public Shared Function Smarpod_Move(ByVal smarpodId As UInteger, ByRef pose As Smarpod_Pose, ByVal holdTime As UInteger, ByVal waitForCompletion As Integer) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_Stop")> _
    Public Shared Function Smarpod_Stop(ByVal smarpodId As UInteger) As UInt32
    End Function
    <DllImport("SmarPod.DLL", CallingConvention:=CallingConvention.Cdecl, CharSet:=System.Runtime.InteropServices.CharSet.Ansi, EntryPoint:="Smarpod_StopAndHold")> _
    Public Shared Function Smarpod_StopAndHold(ByVal smarpodId As UInteger, ByVal holdTime As UInteger) As UInt32
    End Function

#Region "Wrapper"
    Public Shared Property PropertyUInteger(id As UInteger, item As PROPERTYSYMBOLS) As UInteger
        Get
            Dim value As UInteger = 0
            Smarpod_Get_ui(id, item, value)
            Return value
        End Get
        Set(ByVal value As UInteger)
            Smarpod_Set_ui(id, item, value)
        End Set
    End Property
#End Region

End Class
