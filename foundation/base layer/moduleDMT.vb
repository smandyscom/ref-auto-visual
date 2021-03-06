﻿Imports System.Text
Imports System.Linq
Imports System.Threading
Imports System.IO
Imports System.Xml.Serialization
Imports System.Net
Imports Automation.mainIOHardware
Imports System.ComponentModel
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Net.NetworkInformation

#Region "arguments definition"
Public Enum baudEnum As Integer
    B9600 = 9600
    B19200 = 19200
    B38400 = 38400
    B57600 = 57600
    B115200 = 115200
    B921000 = 921000
End Enum
Public Enum dataLengthEnum As Integer
    LEN_7 = 7
    LEN_8 = 8
End Enum
Public Enum parityEnum As Integer
    'in ASCII definition
    NONE = &H4E 'as  N
    EVEN = &H45 'as  E
    ODD = &H4F  'as  O
End Enum
Public Enum stopBitsEnum As Integer
    BITS_1 = 1
    BITS_2 = 2
End Enum
Public Enum modbusModeEnum As Integer
    ASCII = 1
    RTU = 2
End Enum
Public Enum seriesEnum As Integer
    DVP
    RTU
    AH
End Enum
Public Enum commuicationTypeEnum As Integer
    SERIAL = 0
    ETHERNET = 1
End Enum
Public Enum coilStatusEnum As UInt32
    COIL_ON = 1
    COIL_OFF = 0
End Enum
Public Enum connectionInterfaceEnum As Integer
    SERIAL = 0
    SOCKET = 1
End Enum



#End Region

Friend Module moduleDMT

    Sub New()
        'the initializer would be called when use module first time
        hDMTDll = LoadLibrary("DMT.dll")    'dmt.dll should existed in execute directory , Hsien , 2015.11.16
    End Sub


    Dim hDMTDll As System.IntPtr ' handle of a loaded dll , used for dynamic link 

    ' About .Net P/Invoke:
    
    ' Declare Auto Function ABC Lib "XXX.dll" (ByVal a As Integer, ByVal b As Integer) As Integer

    ' indicates that "ABC" function is imported from XXX.dll
    ' XXX.dll exports a function of the same name with "ABC"
    ' the return type and the parameter's data type of "ABC" 
    ' must be identical with the function exported from XXX.dll

    Declare Auto Function LoadLibrary Lib "kernel32.dll" (ByVal dllPath As String) As IntPtr
    Declare Auto Function FreeLibrary Lib "kernel32.dll" (ByVal hDll As IntPtr) As Boolean

    '// Data Access
    Declare Auto Function RequestData Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal func_code As Integer, ByRef sendbuf As Byte, ByVal sendlen As Integer) As Integer
    Declare Auto Function ResponseData Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByRef slave_addr As Integer, ByRef func_code As Integer, ByRef recvbuf As Byte) As Integer

    '// Serial Communication
    Public Function OpenModbusSerial(ByVal conn_num As Integer, ByVal baud_rate As baudEnum, ByVal data_len As dataLengthEnum, ByVal parity As parityEnum, ByVal stop_bits As stopBitsEnum, ByVal modbus_mode As modbusModeEnum) As Integer
        'as wrapper
        'hDMTDll = LoadLibrary("DMT.dll")    
        Return OpenModbusSerial(conn_num, baud_rate, data_len, ChrW(parity), stop_bits, modbus_mode)
    End Function
    Private Declare Auto Function OpenModbusSerial Lib "DMT.dll" (ByVal conn_num As Integer, ByVal baud_rate As Integer, ByVal data_len As Integer, ByVal parity As Char, ByVal stop_bits As Integer, ByVal modbus_mode As Integer) As Integer

    Declare Auto Sub CloseSerial Lib "DMT.dll" (ByVal conn_num As Integer)
    Declare Auto Function GetLastSerialErr Lib "DMT.dll" () As Integer
    Declare Auto Sub ResetSerialErr Lib "DMT.dll" ()

    '// Socket Communication
    Declare Auto Function OpenModbusTCPSocket Lib "DMT.dll" (ByVal conn_num As Integer, ByVal ipaddr As Integer) As Integer
    Declare Auto Sub CloseSocket Lib "DMT.dll" (ByVal conn_num As Integer)
    Declare Auto Function GetLastSocketErr Lib "DMT.dll" () As Integer
    Declare Auto Sub ResetSocketErr Lib "DMT.dll" ()
    Declare Auto Function ReadSelect Lib "DMT.dll" (ByVal conn_num As Integer, ByVal millisecs As Integer) As Integer

    '// MODBUS Address Calculation
    Public Function DevToAddrW(ByVal series As seriesEnum, ByVal device As String, ByVal qty As Integer) As Integer
        Return DevToAddrW([Enum].GetName(GetType(seriesEnum), series), device, qty)
    End Function
    Declare Auto Function DevToAddrW Lib "DMT.dll" (ByVal series As String, ByVal device As String, ByVal qty As Integer) As Integer

    '// Wrapped MODBUS Funcion : 0x01
    Declare Auto Function ReadCoilsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer

    '// Wrapped MODBUS Funcion : 0x02
    Function __ReadInputsW(ByVal comm_type As commuicationTypeEnum, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
        Return ReadInputsW(comm_type, conn_num, slave_addr, dev_addr, qty, data_r, req, res)
    End Function
    Friend Declare Auto Function ReadInputsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer

    '// Wrapped MODBUS Funcion : 0x03
    Declare Auto Function ReadHoldRegsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
    Declare Auto Function ReadHoldRegs32W Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer

    '// Wrapped MODBUS Funcion : 0x04
    Declare Auto Function ReadInputRegsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_r As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer

    '// Wrapped MODBUS Funcion : 0x05		   
    Declare Auto Function WriteSingleCoilW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer

    '// Wrapped MODBUS Funcion : 0x06
    Declare Auto Function WriteSingleRegW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
    Declare Auto Function WriteSingleReg32W Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
    '// Wrapped MODBUS Funcion : 0x0F
    Declare Auto Function WriteMultiCoilsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
    Function __WriteMultiCoilsW(ByVal comm_type As commuicationTypeEnum, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
        Return WriteMultiCoilsW(comm_type, conn_num, slave_addr, dev_addr, qty, data_w, req, res)
    End Function

    '// Wrapped MODBUS Funcion : 0x10
    Declare Auto Function WriteMultiRegsW Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer
    Declare Auto Function WriteMultiRegs32W Lib "DMT.dll" (ByVal comm_type As Integer, ByVal conn_num As Integer, ByVal slave_addr As Integer, ByVal dev_addr As Integer, ByVal qty As Integer, ByRef data_w As UInt32, ByVal req As StringBuilder, ByVal res As StringBuilder) As Integer


    Public dmtLockObject As Object = New Object
End Module

Enum dmtAddressCodeEnum
    BYTE_INDEX_COMM_NO = 5  'Hsien , 2016.01.08
    BYTE_INDEX_SLAVE_NO = 4
    BYTE_INDEX_CATEGRORY = 3
    BYTE_INDEX_DEVICE = 2
End Enum

Public Class dmtModbusInterface
    Inherits hardwareBase

    'the head start node to address 

    '----------------------------------
    '   Slave Table
    '----------------------------------
    Property MasterList As List(Of dmtModbusMaster) = New List(Of dmtModbusMaster)
    Public Overrides ReadOnly Property Status As hardwareStatusEnum
        Get
            If (MasterList.TrueForAll(Function(__master As dmtModbusMaster) (__master.Status = hardwareStatusEnum.FAILED))) Then
                Me.__status = hardwareStatusEnum.FAILED
            ElseIf (MasterList.Exists(Function(__master As dmtModbusMaster) (__master.Status = hardwareStatusEnum.FAILED))) Then
                Me.__status = hardwareStatusEnum.PARTIAL_FAILED
            Else
                Me.__status = hardwareStatusEnum.HEALTHY
            End If

            Return Me.__status
        End Get
    End Property
    ''' <summary>
    ''' Reflect average polling cycle for all master
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property PollingCycle As Double
        Get
            totalCycleTime = 0
            MasterList.ForEach(Sub(__master As dmtModbusMaster) totalCycleTime += __master.PollingCycle)
            Return totalCycleTime / MasterList.Count
        End Get
    End Property
    Dim totalCycleTime As Double = 0

    Public Overrides Function dataDispatch() As Integer
        Return 0
    End Function

    Public Overrides Function initialize() As Integer
        'open master
        MasterList.ForEach(Sub(__master As dmtModbusMaster) __master.initialize())  'link parent
        Return 0
    End Function

    Function locateMaster(localAddress As Byte()) As dmtModbusMaster
        Return MasterList.Find(Function(__master As dmtModbusMaster) (__master.CommunicationPort = localAddress(dmtAddressCodeEnum.BYTE_INDEX_COMM_NO)))
    End Function

    Public Overrides Function readValue(addressCodeInByte() As Byte) As ULong
        Return locateMaster(addressCodeInByte).readValue(addressCodeInByte)
    End Function

    Public Overrides Sub writeValue(addressCodeInByte() As Byte, value As ULong)
        locateMaster(addressCodeInByte).writeValue(addressCodeInByte, value)
    End Sub

    Sub New()
        __status = hardwareStatusEnum.FAILED
        __hardwareCode = hardwareCodeEnum.DMT_MODBUS
    End Sub
End Class

Public Class dmtModbusMaster
    Inherits hardwareBase
    Implements IDisposable


    '----------------------------------
    '   Master Configurations
    '----------------------------------
    Property ConnectionInterface As connectionInterfaceEnum
        Get
            'hidingProperties()
            Return __connectionInterface
        End Get
        Set(value As connectionInterfaceEnum)
            __connectionInterface = value

            Select Case __connectionInterface
                Case connectionInterfaceEnum.SERIAL
                    __closeMethod = AddressOf CloseSerial
                Case connectionInterfaceEnum.SOCKET
                    __closeMethod = AddressOf CloseSocket
            End Select
        End Set
    End Property
    Property CommunicationPort As Integer  'i.e COM'1'
        Get
            Return __communicationPort
        End Get
        Set(value As Integer)
            __communicationPort = value
            __hardwareCode = hardwareCodeEnum.DMT_MODBUS + CommunicationPort   'encoded commnication port
        End Set
    End Property
    <Category("Serial")> <Browsable(True)> Property Baud As baudEnum = baudEnum.B115200
    <Category("Serial")> <Browsable(True)> Property DataLength As dataLengthEnum = dataLengthEnum.LEN_8
    <Category("Serial")> <Browsable(True)> Property Parity As parityEnum = parityEnum.EVEN
    <Category("Serial")> <Browsable(True)> Property StopBits As stopBitsEnum = stopBitsEnum.BITS_1
    <Category("Serial")> <Browsable(True)> Property Mode As modbusModeEnum = modbusModeEnum.RTU
    <Category("Socket")> <Browsable(True)> Property ConnectionIp As String = "192.168.0.1"
    '----------------------------------
    '   Slave Table
    '----------------------------------

    Property SlaveList As List(Of dmtModbusSlave) = New List(Of dmtModbusSlave)
    Property PollingInterval As Single = 10.0F    'in ms
    Property ReconnectInterval As Integer = 5000    'in ms
    'Dim stopFuse As Boolean = False 'used to control the infinite polling loop


    Dim __connectionInterface As connectionInterfaceEnum = connectionInterfaceEnum.SERIAL
    Dim __communicationPort As Integer = 5
    Dim __closeMethod As Action(Of Integer) = Nothing
    ''' <summary>
    ''' Used for hide unnessary option when connection interface changed
    ''' </summary>
    ''' <remarks></remarks>
    Sub hidingProperties()
        Dim __pd As PropertyDescriptorCollection = TypeDescriptor.GetProperties(Me.GetType())

        For Each __name As String In {"Baud",
                                      "DataLength",
                                      "Parity",
                                      "StopBits",
                                      "Mode"}
            Dim ba As BrowsableAttribute = CType(__pd.Item(__name).Attributes.Item(GetType(BrowsableAttribute)), BrowsableAttribute)
            Dim fi As FieldInfo = ba.GetType.GetField("browsable", BindingFlags.NonPublic Or BindingFlags.Instance)
            fi.SetValue(ba, __connectionInterface = connectionInterfaceEnum.SERIAL)
        Next

        Dim __ba As BrowsableAttribute = CType(__pd.Item("ConnectionIp").Attributes.Item(GetType(BrowsableAttribute)), BrowsableAttribute)
        Dim __fi As FieldInfo = __ba.GetType.GetField("browsable", BindingFlags.NonPublic Or BindingFlags.Instance)
        __fi.SetValue(__ba, __connectionInterface = connectionInterfaceEnum.SOCKET)
    End Sub

    Public Overrides Function initialize() As Integer
        ''load configuration table ( used to allocate how many slave had)


        SlaveList.ForEach(Sub(__slave As dmtModbusSlave)
                              __slave.parentRaference = Me
                              __slave.initialize()
                          End Sub)  'link parent

        Dim returnError As Integer = 0
        Select Case Me.ConnectionInterface
            Case connectionInterfaceEnum.SERIAL
                returnError = OpenModbusSerial(Me.CommunicationPort, Me.Baud, Me.DataLength, Me.Parity, Me.StopBits, Me.Mode)
            Case connectionInterfaceEnum.SOCKET
                returnError = OpenModbusTCPSocket(Me.CommunicationPort, BitConverter.ToInt32(IPAddress.Parse(Me.ConnectionIp).GetAddressBytes(), 0))
        End Select

        If (returnError = -1) Then
            state = masterStateEnum.RECONNECTING
            Me.__status = hardwareStatusEnum.FAILED
        Else
            state = masterStateEnum.WORKING
        End If

        Dim __asynchronCaller As Func(Of Integer) = AddressOf dataDispatch
        __asynchronCaller.BeginInvoke(Nothing, Nothing)

        Return 0
    End Function


    Enum masterStateEnum As Integer
        WORKING
        RECONNECTING
    End Enum

    Shared connectLockObject As Object = New Object
    Dim state As masterStateEnum = masterStateEnum.WORKING
    Public Overrides Function dataDispatch() As Integer

        While True

            Select Case state
                Case masterStateEnum.WORKING
                    '-----------------------------
                    '
                    '-----------------------------

                    __pollingCycle = __stopWatch.Elapsed
                    __stopWatch.Restart()

                    SlaveList.ForEach(Sub(__slave As dmtModbusSlave) __slave.dataDispatch())

                    If (SlaveList.Exists(Function(__slave As dmtModbusSlave) (__slave.Status = hardwareStatusEnum.FAILED))) Then
                        'monitor each module
                        __status = hardwareStatusEnum.FAILED
                        __closeMethod.Invoke(Me.CommunicationPort)  'close port
                        state = masterStateEnum.RECONNECTING
                    Else
                        __status = hardwareStatusEnum.HEALTHY
                    End If


                    Thread.Sleep(PollingInterval)
                Case masterStateEnum.RECONNECTING
                    '-------------------------------------------
                    '
                    '-------------------------------------------    
                    Dim returnError As Integer = -1
                    Select Case Me.ConnectionInterface
                        Case connectionInterfaceEnum.SERIAL
                            SyncLock dmtLockObject  'prevent collision with another device
                                returnError = OpenModbusSerial(Me.CommunicationPort, Me.Baud, Me.DataLength, Me.Parity, Me.StopBits, Me.Mode)
                            End SyncLock
                        Case connectionInterfaceEnum.SOCKET
                            'connect only when ping success 
                            Dim __ping As Ping = New Ping()
                            If (__ping.Send(Me.ConnectionIp).Status = IPStatus.Success) Then
                                SyncLock dmtLockObject  'prevent collision with another device
                                    returnError = OpenModbusTCPSocket(Me.CommunicationPort, BitConverter.ToInt32(IPAddress.Parse(Me.ConnectionIp).GetAddressBytes(), 0))
                                End SyncLock
                            End If
                    End Select

                    If (returnError <> -1) Then
                        'successed reconnected
                        state = masterStateEnum.WORKING
                    Else
                        Thread.Sleep(ReconnectInterval) 'delay a while to reconnect
                    End If
            End Select

        End While

        Return 0
    End Function

    Function locateSlave(localAddress As Byte()) As dmtModbusSlave
        Return SlaveList.Find(Function(__slave As dmtModbusSlave) (__slave.SlaveAddress = localAddress(dmtAddressCodeEnum.BYTE_INDEX_SLAVE_NO)))
    End Function

    Public Overrides Function readValue(localAddress As Byte()) As ULong
        Return locateSlave(localAddress).readValue(localAddress)
    End Function

    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        locateSlave(localAddress).writeValue(localAddress, value)
    End Sub

    Sub New()
        __status = hardwareStatusEnum.FAILED
        __hardwareCode = hardwareCodeEnum.DMT_MODBUS + CommunicationPort   'encoded commnication port
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '處置 Managed 狀態 (Managed 物件)。
            End If
            __closeMethod(Me.CommunicationPort)
            '釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下面的 Finalize()。
            '將大型欄位設定為 null。
        End If
        'close the connection

        Me.disposedValue = True
    End Sub

    '只有當上面的 Dispose(ByVal disposing As Boolean) 有可釋放 Unmanaged 資源的程式碼時，才覆寫 Finalize()。
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

End Class

Public Class dmtModbusSlave
    Inherits hardwareBase
    'memory hold address table:
    '0:X
    '1:Y
    '2-N:D
    Public Enum categroryCodeEnum As Byte
        X = &H0
        Y = &H1
        D = &H2
        'D_WRITE = &H3
    End Enum

    Property SlaveType As seriesEnum
        Get
            Return __slaveType
        End Get
        Set(value As seriesEnum)
            __slaveType = value

            Select Case value
                Case seriesEnum.DVP
                    '8-bit width
                    formattion = "{0}{1}"
                    width = 8
                Case seriesEnum.AH
                    '16-bits width
                    formattion = "{0}.{1}"
                    width = 16
            End Select

            __xStartAddress = "X" & serialBit2DeltaFormat(0)
            __yStartAddress = "Y" & serialBit2DeltaFormat(0)
        End Set
    End Property
    Property SlaveAddress As Integer = 0

    '-----------------------------------------------------
    '   Memory Range Definition (could be set at setup-time)
    '-----------------------------------------------------
    Property XRange As Integer = 32
    Property YRange As Integer = 32
    Property DReadRange As Integer   '0-(DReadRange-1)
        Get
            Return __dReadRange
        End Get
        Set(value As Integer)
            __dReadRange = value
            __dWriteStartWord = String.Format("D{0}", __dReadRange)
        End Set
    End Property
    Property DWriteRange As Integer  'DReadRange-DWriteRange
        Get
            Return __dWriteRange
        End Get
        Set(value As Integer)
            __dWriteRange = value
        End Set
    End Property
    Property SlaveName As String = ""

    ReadOnly Property DReadStartWord As String
        Get
            Return __dReadStartWord
        End Get
    End Property
    ReadOnly Property DWriteStartWord As String
        Get
            Return __dWriteStartWord
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="__categrory"></param>
    ''' <param name="deviceIndex"></param>
    ''' <value></value>
    ''' <returns> The value of corresponding device </returns>
    ''' <remarks></remarks>
    ReadOnly Property ReadDeviceValue(ByVal __categrory As categroryCodeEnum, deviceIndex As Integer) As ULong
        Get
            Dim __queryAddress As ULong = 0
            Dim __queryAddresInByteArray As Byte() = BitConverter.GetBytes(__queryAddress)
            __queryAddresInByteArray(dmtAddressCodeEnum.BYTE_INDEX_CATEGRORY) = __categrory
            __queryAddresInByteArray(dmtAddressCodeEnum.BYTE_INDEX_DEVICE) = deviceIndex
            Return readValue(__queryAddresInByteArray)
        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="categrory"></param>
    ''' <param name="serialAddress"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ReadDeviceBit(categrory As categroryCodeEnum, serialAddress As Integer) As Boolean
        Get
            Return hardwareBase.readBitFromWord(memoryHold(categrory), serialAddress)
        End Get
    End Property
    WriteOnly Property WriteDeviceBit(categrory As categroryCodeEnum, serialAddress As Integer) As Boolean
        Set(value As Boolean)
            hardwareBase.writeBitToWord(memoryHold(categrory), serialAddress, value)
        End Set
    End Property


    Dim __dReadRange As Integer = 8
    Dim __dWriteRange As Integer = 8
    Dim __dReadStartWord As String = "D0"
    Dim __dWriteStartWord As String = "D32"
    Dim __xStartAddress As String = "X0.0"
    Dim __yStartAddress As String = "Y0.0"
    Dim __slaveType As seriesEnum

    'Hsien ,  delta-plc had stupid and incorporate address mode , 2016.02.05
    'e.g , X20 , the seventeenth input address
    Dim formattion As String = "X{0}{1}"
    Dim width As Integer = 8

    Friend parentRaference As dmtModbusMaster

    Public Overrides Function initialize() As Integer
        'allocating memory accroding to setting bavlue
        'calculate sum

        memoryHold = New ULong(2 + __dReadRange + __dWriteRange) {}
        __dataBuffer = New UInteger(63) {}

        Return 0    'no need to initialize
    End Function

    Dim __devAddress As Integer = 0
    Dim __dataBuffer As UInteger() '= New UInteger(memoryRangeEnum.MAX_RANGE - 1) {}

    Dim __request As StringBuilder = New StringBuilder(1024)
    Dim __response As StringBuilder = New StringBuilder(1024)
    Dim returnError As Integer = 0

    Public Overrides Function dataDispatch() As Integer

        __status = hardwareStatusEnum.HEALTHY

        If (parentRaference Is Nothing) Then
            __status = hardwareStatusEnum.FAILED
            Return 0
        End If


        SyncLock dmtLockObject  'prevent collision with another device
            'polling X
            __request.Clear()
            __response.Clear()

            __devAddress = DevToAddrW(SlaveType.ToString, __xStartAddress, XRange)
            returnError = __ReadInputsW(parentRaference.ConnectionInterface, parentRaference.CommunicationPort, SlaveAddress, __devAddress, XRange, __dataBuffer(0), __request, __response)

            If (returnError < 0) Then
                __status = hardwareStatusEnum.FAILED
            Else
                'once no reading error
                For index = 0 To __dataBuffer.Count - 1
                    hardwareBase.writeBitToWord(memoryHold(categroryCodeEnum.X), index, __dataBuffer(index) > 0)    'logic value mapping
                Next
            End If

            __request.Clear()
            __response.Clear()

            'polling Y 
            For index = 0 To __dataBuffer.Count - 1
                __dataBuffer(index) = hardwareBase.readBitFromWord(memoryHold(categroryCodeEnum.Y), index)
            Next

            __devAddress = DevToAddrW(SlaveType.ToString, __yStartAddress, YRange)
            returnError = __WriteMultiCoilsW(parentRaference.ConnectionInterface, parentRaference.CommunicationPort, SlaveAddress, __devAddress, YRange, __dataBuffer(0), __request, __response)
            '---------------
            'polling D(READ)
            '---------------
            __request.Clear()
            __response.Clear()

            __devAddress = DevToAddrW(SlaveType, __dReadStartWord, __dReadRange)
            returnError = ReadHoldRegsW(parentRaference.ConnectionInterface, parentRaference.CommunicationPort, SlaveAddress, __devAddress, __dReadRange, __dataBuffer(0), __request, __response)

            If (returnError < 0) Then
                __status = hardwareStatusEnum.FAILED
            Else
                For index = 0 To DReadRange - 1
                    'write in memory
                    memoryHold(categroryCodeEnum.D + index) = __dataBuffer(index)
                Next
            End If
            '---------------
            'polling D_WRITE
            '---------------
            For index = 0 To __dWriteRange - 1
                __dataBuffer(index) = memoryHold(2 + index + DReadRange) And UInteger.MaxValue    'avoiding data overflow
            Next
            __request.Clear()
            __response.Clear()

            __devAddress = DevToAddrW(SlaveType, __dWriteStartWord, __dWriteRange)
            returnError = WriteMultiRegsW(parentRaference.ConnectionInterface, parentRaference.CommunicationPort, SlaveAddress, __devAddress, __dWriteRange, __dataBuffer(0), __request, __response)
        End SyncLock

        If (returnError < 0) Then
            __status = hardwareStatusEnum.FAILED
        End If

        Return 0

    End Function


#Region "program unified interface"
    Function locateIndex(localAddress As Byte()) As Integer
        Return localAddress(dmtAddressCodeEnum.BYTE_INDEX_CATEGRORY) +
            localAddress(dmtAddressCodeEnum.BYTE_INDEX_DEVICE)
    End Function
    Public Overrides Function readValue(localAddress As Byte()) As ULong
        Return memoryHold(locateIndex(localAddress))
    End Function
    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        memoryHold(locateIndex(localAddress)) = value
    End Sub
#End Region


#Region "Address conversion"
    Friend Function serialBit2DeltaFormat(bit As Integer) As String
        Return String.Format(formattion, bit \ width, bit Mod width)
    End Function
#End Region

End Class
