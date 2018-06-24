Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel
Public Module mdlAMONetInit
    '------------------------
    ' MARCO definition
    '------------------------
    Public Const IS_OFF As Boolean = False
    Public Const IS_ON As Boolean = True
    
    Public Locking As New Object            ' used as syncLock object
    
    Public Sub AMax_Get_Moton_DeviceIP(ByVal AxisIP As Short, ByRef RingNo As Short, ByRef DeviceIP As Short, ByRef PortNo As Short)
        ' Given AxisIP , return RingNo , DeviceIP , PortNo
        '' Motor : 0~63 for Ring0, 64~127 as Ring1
        RingNo = pData.MotorSettings(AxisIP).RingIndex
        DeviceIP = pData.MotorSettings(AxisIP).DeviceIp
        PortNo = pData.MotorSettings(AxisIP).AxisIndex
    End Sub
End Module
