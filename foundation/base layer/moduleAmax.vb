Imports System.IO
Imports System.Xml.Serialization
Imports Automation.amaxModule
Imports Automation.mainIOHardware
Imports System.Threading

Public Enum amaxModuleTypeEnum As ULong
    LOCAL_OUTPUT = &H0
    LOCAL_INPUT = &H1
    REMOTE = &H2
End Enum
Public Enum amaxBaudrate
    BAUD_2_5M = 0
    BAUD_5M = 1
    BAUD_10M = 2
    BAUD_20M = 3
End Enum
Public Enum amaxAddressCodeEnum
    BYTE_INDEX_TYPE = 4
    BYTE_INDEX_RING = 3
    BYTE_INDEX_DEVICEIP = 2
End Enum


Public Class amaxModule
    Inherits hardwareBase

    Enum moduleHardwareCodeEnum As Byte
        AMAX_224X
        AMAX_1240
        AMAX_2754_1754
        AMAX_2752_1752
        UNKNOWN
    End Enum


    'decribe the concurrent module status
    '-------------------------------------
    '   Setting Values
    '-------------------------------------
    Property IsExpectedConnected As Boolean = False
    Property ExpectedCode As moduleHardwareCodeEnum
        Get
            Return __hardwareCode
        End Get
        Set(value As moduleHardwareCodeEnum)
            __hardwareCode = value
        End Set
    End Property
    ReadOnly Property RealCode As moduleHardwareCodeEnum
        Get
            Return __realCode
        End Get
    End Property
    Friend __realCode As moduleHardwareCodeEnum = moduleHardwareCodeEnum.AMAX_1240  'reflect real type of hardware

    ReadOnly Property IsTypeMatched As Boolean
        Get
            Return __realCode = HardwareCode    'used to verify , hardware code is expected type
        End Get
    End Property
    ReadOnly Property IsMotionDevice As Boolean
        Get
            Return (slaveInfo And slaveInfoLowByte.IS_MOTION_SLAVE) > 0
        End Get
    End Property
    ReadOnly Property IsInitialized As Boolean
        Get
            Return (slaveInfo And slaveInfoLowByte.IS_INITIALED) > 0
        End Get
    End Property
    ReadOnly Property IsDeviceInUse As Boolean
        Get
            Return (slaveInfo And slaveInfoLowByte.IS_DEVICE_IN_USE) > 0
        End Get
    End Property
    ReadOnly Property RingIndex As Short
        Get
            Return parentContainerReference.RingIndex
        End Get
    End Property
    ReadOnly Property IsPhysicalConnected As Boolean
        Get
            Return __isPhysicalConnected
        End Get
    End Property
    ReadOnly Property IsLossed As Boolean
        Get
            Return IsExpectedConnected And (Not __isPhysicalConnected)
        End Get
    End Property
    ReadOnly Property ModuleAddress As ULong
        Get
            Return BitConverter.ToUInt64({hardwareCodeEnum.AMAX_1202_CARD, 0, 0, amaxModuleTypeEnum.REMOTE, RingIndex, slaveIndex, 0, 0}, 0)
        End Get
    End Property

    Dim firmwareVersion As UInteger = 0 'mother fucker wrong type defined by API , use byte would cause corruption , Hsien , 2015.11.26

    Friend parentContainerReference As amaxRing = Nothing   'link to
    Friend slaveInfo As Short = 0   'quried from _mnet_get_slave_info
    Friend slaveIndex As Integer = 0

    Dim returnError As Integer = 0
    Protected Friend __isPhysicalConnected As Boolean = False 'whether connected in real

    Public Overrides Function ToString() As String
        Return String.Format("{0},{1},{2},{3}",
                             ExpectedCode,
                             [Enum].ToObject(GetType(moduleHardwareCodeEnum), HardwareCode).ToString,
                             IsPhysicalConnected,
                             IsLossed)
    End Function

    Public Overrides Function initialize() As Integer

        __status = hardwareStatusEnum.HEALTHY

        'grab slave info
        slaveInfo = B_mnet_get_slave_info(RingIndex, slaveIndex)
        'grab slave version

        If (IsMotionDevice) Then
            returnError = B_mnet_get_fw_version(RingIndex, slaveIndex, firmwareVersion)
            returnError += B_mnet_m4_initial(RingIndex, slaveIndex)    ' equivalent to errorCode = B_mnet_m4_initial(RingNo, DeviceIP)
            returnError += B_mnet_m4_set_com_wdg_mode(RingIndex, slaveIndex, 2)    ' emergency behavior

            If (returnError <> returnErrorCodes.ERR_NoError) Then
                __status = hardwareStatusEnum.FAILED
            End If

            'identify motion slave type
            Select Case [Enum].ToObject(GetType(slaveInfoHighByte), (slaveInfo And &H300) >> 8)    'bit 8-15
                Case slaveInfoHighByte.AMAX_1240
                    __realCode = moduleHardwareCodeEnum.AMAX_1240
                Case slaveInfoHighByte.AMAX_224X
                    __realCode = moduleHardwareCodeEnum.AMAX_224X
                Case Else
                    __realCode = moduleHardwareCodeEnum.UNKNOWN
            End Select

        Else
            'IO slave
            Select Case [Enum].ToObject(GetType(slaveInfoLowByte), slaveInfo And &H7)    'bit 0-2
                Case slaveInfoLowByte.AMAX_2752_1752
                    __realCode = moduleHardwareCodeEnum.AMAX_2752_1752
                Case slaveInfoLowByte.AMAX_2754_1754
                    __realCode = moduleHardwareCodeEnum.AMAX_2754_1754
            End Select
        End If

        Return returnError
    End Function

    Dim tempStatus32x4 As Integer() = New Integer(3) {}
    Dim tempStatus16x4 As UShort() = New UShort(3) {}
    Dim tempStatus8x8 As Byte() = New Byte(7) {}


    ''' <summary>
    ''' The hardware io access routine for motion device
    ''' </summary>
    ''' <remarks></remarks>
    Sub refreshMotionDevice()
        If (IsPhysicalConnected) Then
            If (IsMotionDevice) Then
                'make sure its motion device
                '--------------------------------
                '   Input Status Polling
                '--------------------------------
                tempStatus32x4 = New Integer(3) {}
                returnError = B_mnet_m4_get_io_status_ex(RingIndex, slaveIndex, {0, 1, 2, 3}, tempStatus32x4, 4) 'read motion IO for x,y,z,u channel


                For __index = 0 To tempStatus32x4.Count - 1
                    tempStatus16x4(__index) = (tempStatus32x4(__index) And &HFFFF)  'grab the Lower word
                Next

                Buffer.BlockCopy(tempStatus16x4, 0, tempStatus8x8, 0, 8)
                memoryHold(0) = BitConverter.ToUInt64(tempStatus8x8, 0)
                '---------------------------------
                '   Output Status Polling
                '---------------------------------
            Else
                'reserved for module polling mode
            End If
        Else
            '----------------------------------------------
            '   Device is not connected
            '----------------------------------------------
        End If
    End Sub

    Public Overrides Function dataDispatch() As Integer
        'Hsien , do not keep polling motion device , performance issue?
        Return 0
    End Function



    Sub New()
        memoryHold = New ULong(1) {}    'index-0 for motion input/32-input/32-output
        __hardwareCode = moduleHardwareCodeEnum.UNKNOWN
        'index-1 spared for motion output
    End Sub

    Public Overrides Function readValue(localAddress As Byte()) As ULong
        refreshMotionDevice()   'Hsien , direct access motion when read
        Return memoryHold(0)
    End Function

    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        refreshMotionDevice()   'Hsien , direct access motion when read
        memoryHold(0) = value
    End Sub
End Class

Public Class amaxRing
    Inherits hardwareBase


    'indicate ring status
    Property ModuleInfos As List(Of amaxModule) = New List(Of amaxModule)(64)   'fixed capacity

    '-----------------------------------------------
    '   Monitoring value
    '-----------------------------------------------
    ReadOnly Property RingStatus As ringStatus
        Get
            Return __ringStatus
        End Get
    End Property
    ReadOnly Property RingCyclicTime As Double
        Get
            Return __ringcyclicTime
        End Get
    End Property
    ReadOnly Property CommunicationStatus As communicationStatus
        Get
            Return __communicationStatus
        End Get
    End Property
    ReadOnly Property RingIndex As Short
        Get
            Return __ringIndex
        End Get
    End Property
    '------------------------------
    '   Setting Values
    '------------------------------
    Property RetryTimes As Short = 10
    Property Baud As amaxBaudrate = amaxBaudrate.BAUD_20M
    ''' <summary>
    ''' When expected to be connect , return status error when fail to initialize
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property IsExpectedConnected As Boolean = False

    Dim returnError As Integer = 0
    Dim actionTable As UInt64 = 0

    Dim __isInitialized As Boolean = False

    Protected Friend __ringIndex As Short = 0 '0 or 1
    Protected Friend __ringStatus As ringStatus = mdlAMOCodes.ringStatus.AXIS_COMMAND_EMPTY
    Protected Friend __communicationStatus As communicationStatus = mdlAMOCodes.communicationStatus.DISCONNECTED
    Protected Friend __ringcyclicTime As Double = 0 'queried from _mnet_get_ring_cyclic_time

    Dim tempStatus32x64 As UInteger() = New UInteger(63) {}

    Public Overrides Sub buildRawSeed()
        For index = 0 To ModuleInfos.Capacity - 1
            With ModuleInfos(index)
                .slaveIndex = index
                .parentContainerReference = Me
            End With
        Next
    End Sub

    Public Overrides Function initialize() As Integer

        'start ring
        '=========================================
        ' (0) :ERROR_NoError The API returns success
        ' (-4) : ERROR_Invalid_Ring make sure the Ring is active
        '=========================================


        'set retry times
        returnError = B_mnet_set_retry_times(RingIndex, RetryTimes)
        ' 1 : Soft reset ring to the initial status.
        returnError += B_mnet_reset_ring(RingIndex)             '*Reset First: ERROR_Invalid_Ring make sure the Ring is active
        returnError += B_mnet_set_ring_config(RingIndex, Baud)
        ' 3 : Start ring communication
        returnError += B_mnet_start_ring(RingIndex)
        ' 4 : Enable watch dog & Set the ring continue error and error rate
        B_mnet_enable_soft_watchdog(RingIndex, Nothing)       'Enable Watch Dog
        returnError += B_mnet_set_ring_quality_param(RingIndex, 50, 100)
        ' 5 : Get the active ring status
        ' ( 0 Ring Disconnected; 01 Ring Connect; 02 Slave Error; 03 Ring is Stop )
        __communicationStatus = B_mnet_get_com_status(RingIndex)
        'ring slave status , scanning
        returnError += B_mnet_get_ring_active_table(RingIndex, actionTable)

        If (returnError <> returnErrorCodes.ERR_NoError And
            IsExpectedConnected) Then
            __status = hardwareStatusEnum.FAILED
        End If

        '----------------------
        'modules initialization
        '----------------------
        returnError = 0 'reset
        For index = 0 To ModuleInfos.Capacity - 1
            With ModuleInfos(index)
                .__isPhysicalConnected = (((actionTable >> index) And &H1) > 0)

                If (.IsPhysicalConnected) Then
                    returnError += .initialize()
                End If
            End With
        Next

        If (returnError <> returnErrorCodes.ERR_NoError) Then
            __status = hardwareStatusEnum.PARTIAL_FAILED
        End If

        Return returnError
    End Function

    Public Overrides Function dataDispatch() As Integer


        'assumed had initialed by card
        If (CommunicationStatus = CommunicationStatus.CONNECTED) Then
            '-------------------------------------
            '   Output Status Mapping
            '-------------------------------------  
            For index = 0 To tempStatus32x64.Count - 1
                'tempStatus32x64(index) = memoryHold(ioEnum.OUTPUT + index) And UInteger.MaxValue  'mask the lower DWORD
                If (ModuleInfos(index).RealCode.Equals(moduleHardwareCodeEnum.AMAX_2754_1754)) Then
                    tempStatus32x64(index) = ModuleInfos(index).memoryHold(0) And UInteger.MaxValue 'mask the lower DWORD
                End If
            Next
            returnError = B_mnet_io_memory_output(__ringIndex, tempStatus32x64(0)) '
            '-------------------------------------
            '   Input Status Mapping
            '-------------------------------------  
            returnError += B_mnet_io_memory_input(__ringIndex, tempStatus32x64(0))
            For index = 0 To tempStatus32x64.Count - 1
                'memoryHold(ioEnum.INPUT + index) = tempStatus32x64(index)
                If (ModuleInfos(index).RealCode.Equals(moduleHardwareCodeEnum.AMAX_2752_1752)) Then
                    ModuleInfos(index).memoryHold(0) = tempStatus32x64(index)
                End If
            Next
            '-------------------------------------
            '   Motion Device Polling
            '-------------------------------------
            ModuleInfos.ForEach(Sub(__module As amaxModule) __module.dataDispatch())

        Else
            '---------------------------
            '   Ring is not activated
            '---------------------------
        End If

        If (returnError <> 0) Then
            __status = hardwareStatusEnum.FAILED
        End If

        Return returnError

    End Function





    Public Overrides Function readValue(localAddress As Byte()) As ULong
        Return ModuleInfos(localAddress(amaxAddressCodeEnum.BYTE_INDEX_DEVICEIP)).readValue(localAddress)
    End Function

    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        ModuleInfos(localAddress(amaxAddressCodeEnum.BYTE_INDEX_DEVICEIP)).writeValue(localAddress, value)
    End Sub

#Region "monitors"
    ReadOnly Property ConnectedModules As Integer
        Get
            Return ModuleInfos.FindAll(Function(__module As amaxModule) __module.IsPhysicalConnected).Count
        End Get
    End Property
    ReadOnly Property LossedModules As Integer
        Get
            Return ModuleInfos.FindAll(Function(__module As amaxModule) __module.IsLossed).Count
        End Get
    End Property
#End Region

End Class

Public Class amaxCard
    Inherits hardwareBase
    Implements IDisposable


    Public cardIndex As Short = 0
    Dim cardCounts As Short = 0

    Property RingInfos As List(Of amaxRing) = New List(Of amaxRing)(2) 'ring-0 ' ring-1
    Property PollingInterval As Single = 0.0F    'in ms
    Dim returnError As Integer = 0


    Public Overrides Function initialize() As Integer

        '---------------------------------------
        '   Load configuration/verification list
        '---------------------------------------
        pData.Load(pData.Filename)  'using default filename to loa

        '---------------------------------------
        '   Build the correct connection 
        '---------------------------------------
        buildRawSeed()
        '---------------------------------------
        '   Amax Card/Ring/Module initialization
        '---------------------------------------
        'open card
        'open ring , and do scanning
        'device content verification and warning
        'initialize all motion device
        cardCounts = B_mnet_initial()

        If (cardCounts < 1) Then
            __status = hardwareStatusEnum.FAILED
        End If

        returnError = 0
        'ring N initialization
        RingInfos.ForEach(Sub(__ring As amaxRing) returnError += __ring.initialize())

        '-----------------------------------------
        '   Amax Motion Device initialization
        '-----------------------------------------  
        pData.MotorSettings.ForEach(Sub(__setting As cMotorSetting) __setting.applyConfiguration())

        If (returnError <> returnErrorCodes.ERR_NoError) Then
            __status = hardwareStatusEnum.PARTIAL_FAILED
        End If

        Return returnError
    End Function

    Public Overrides Function dataDispatch() As Integer

        'once stop fuse not break
        While True
            'lio polling
            __pollingCycle = __stopWatch.Elapsed
            __stopWatch.Restart()


            returnError = B_1202_lio_write(0, (BitConverter.GetBytes(memoryHold(0)))(0))   'Hsien , found overflowed issue
            Dim byteArray2 As Byte() = BitConverter.GetBytes(B_1202_lio_read(0))
            memoryHold(1) = BitConverter.ToUInt64({byteArray2(0), 0, 0, 0, 0, 0, 0, 0}, 0)  'overflow


            If (returnError <> returnErrorCodes.ERR_NoError) Then
                Exit While ' card failed , worst condition
            End If

            RingInfos.ForEach(Sub(__ring As amaxRing)
                                  If (__ring.Status = hardwareStatusEnum.HEALTHY) Then
                                      __ring.dataDispatch()
                                  Else
                                      'ring failed , do not polling that
                                      __status = hardwareStatusEnum.PARTIAL_FAILED
                                  End If
                              End Sub)

            Thread.Sleep(PollingInterval)

        End While

        Return 0
    End Function

    Sub New()
        __hardwareCode = hardwareCodeEnum.AMAX_1202_CARD
        memoryHold = New ULong(1) {}    'hold the local OUPUT/local INPUT
    End Sub
    Overrides Sub buildRawSeed()

        '-----------------------------------------------------
        '   Part I , if the net is empty , rebuild the network
        '-----------------------------------------------------
        If (RingInfos.Count <> RingInfos.Capacity) Then
            RingInfos.Clear()
            RingInfos.Add(New amaxRing With {.__ringIndex = 0})
            RingInfos.Add(New amaxRing With {.__ringIndex = 1})
        End If
        RingInfos.ForEach(Sub(__ring As amaxRing)
                              If (__ring.ModuleInfos.Count <> __ring.ModuleInfos.Capacity) Then
                                  __ring.ModuleInfos.Clear()
                                  For index = 0 To __ring.ModuleInfos.Capacity - 1
                                      __ring.ModuleInfos.Add(New amaxModule)
                                  Next
                              End If
                          End Sub)

        '-------------------------------------------------------------
        ' Part II , build the connection
        '   Due to allocating on deserializing of XML serializer issue
        '   had to late allocating , Hsien , 2015.11.30
        '-------------------------------------------------------------
        RingInfos(0).__ringIndex = 0
        RingInfos(1).__ringIndex = 1
        RingInfos.ForEach(Sub(__ring As amaxRing) __ring.buildRawSeed())
    End Sub


    Public Overrides Function readValue(localAddress As Byte()) As ULong

        Select Case localAddress(amaxAddressCodeEnum.BYTE_INDEX_TYPE)
            Case amaxModuleTypeEnum.LOCAL_INPUT
                Return memoryHold(1)
            Case amaxModuleTypeEnum.LOCAL_OUTPUT
                Return memoryHold(0)
            Case amaxModuleTypeEnum.REMOTE
                Return RingInfos(localAddress(amaxAddressCodeEnum.BYTE_INDEX_RING)).readValue(localAddress)
            Case Else
        End Select

        Return 0
    End Function

    Public Overloads Sub writeValue(ByVal mainFrame As amaxModuleTypeEnum, ringIndex As Byte, slaveIndex As Byte, value As ULong)
        Select Case mainFrame
            Case amaxModuleTypeEnum.LOCAL_INPUT
                memoryHold(1) = value
            Case amaxModuleTypeEnum.LOCAL_OUTPUT
                memoryHold(0) = value
            Case amaxModuleTypeEnum.REMOTE
                RingInfos(ringIndex).ModuleInfos(slaveIndex).memoryHold(0) = value
            Case Else
        End Select
    End Sub
    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        writeValue(localAddress(amaxAddressCodeEnum.BYTE_INDEX_TYPE), localAddress(amaxAddressCodeEnum.BYTE_INDEX_RING), localAddress(amaxAddressCodeEnum.BYTE_INDEX_DEVICEIP), value)

    End Sub


#Region "Wrapper Functions"
    ReadOnly Property ExpectedModuleCounts(ByVal arg As moduleHardwareCodeEnum) As List(Of amaxModule)
        'used to count how many modules is type of arg in expect
        Get
            Dim __resultList As List(Of amaxModule) = New List(Of amaxModule)
            __resultList.AddRange(RingInfos(0).ModuleInfos.FindAll(Function(__module As amaxModule) __module.ExpectedCode = arg))
            __resultList.AddRange(RingInfos(1).ModuleInfos.FindAll(Function(__module As amaxModule) __module.ExpectedCode = arg))
            Return __resultList
        End Get
    End Property
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                '處置 Managed 狀態 (Managed 物件)。
            End If

            Dim tempStatus32x64 As UInteger() = New UInteger(63) {}

            'clear all ouput
            B_mnet_io_memory_output(0, tempStatus32x64(0)) '
            B_mnet_io_memory_output(1, tempStatus32x64(0)) '

            '釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下面的 Finalize()。
            '將大型欄位設定為 null。
            'pData.Save()'do not auto save
            pData.MotorSettings.ForEach(Sub(__motor As cMotorSetting) AMaxM4_ServoOn(pData.MotorSettings.IndexOf(__motor), 0))  'servo off all motors

            RingInfos.ForEach(Sub(__ring As amaxRing) B_mnet_stop_ring(__ring.RingIndex))
            B_mnet_close()

        End If

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