﻿Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Drawing.Design
Imports Automation.mainIOHardware
Imports System.Windows.Forms.Design
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>
''' The singleton base object
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class mainIOHardware
    Inherits hardwareBase
    Implements IPersistance

    ' offer the memory pool for all types of hardware to scanning in/out
    ' data width : 64bits
    ' addressing mode : 64bits
    ' shortcut : each accessing would do once locating procedure


    Property PhysicalHardwareList As List(Of subHardwareNode)
        Get
            Return __physicalHardwareList
        End Get
        Set(value As List(Of subHardwareNode))
            __physicalHardwareList = value
        End Set
    End Property
    Protected Friend __physicalHardwareList As List(Of subHardwareNode) = New List(Of subHardwareNode)

    Enum hardwareCodeEnum As Byte
        VIRTUAL = 0
        AMAX_1202_CARD = 1
        DMT_MODBUS = 2
        TWINCAT_ADS = 3
        MELSEC_ETHERNET = 4
    End Enum


    Class subHardwareNode

        <Editor(GetType(hardwareEditor), GetType(UITypeEditor))>
        Property PhysicalHardware As hardwareBase
            Get
                Return __physicalHardware
            End Get
            Set(value As hardwareBase)
                __physicalHardware = value
            End Set
        End Property
        

        Dim __physicalHardware As hardwareBase


        ReadOnly Property Status As hardwareStatusEnum
            Get
                Return Me.__physicalHardware.Status
            End Get
        End Property
        ReadOnly Property PollingCycle As Double
            Get
                Return Me.__physicalHardware.PollingCycle
            End Get
        End Property
        ReadOnly Property HardwareName As String
            Get
                Return Me.ToString
            End Get
        End Property

        Public Overrides Function ToString() As String
            If (__physicalHardware Is Nothing) Then
                Return "Not Allocated"
            Else
                Return String.Format("{0},{1}",
                                     [Enum].GetName(GetType(hardwareCodeEnum), __physicalHardware.HardwareCode),
                                         __physicalHardware.HardwareCode)
            End If
        End Function
    End Class

#Region "singleton interfaces"
    Shared ReadOnly Property Instance As mainIOHardware
        Get
            Return __instance
        End Get
    End Property
    Private Shared ReadOnly __instance As mainIOHardware = New mainIOHardware

    Private Sub New()
        Filename = Application.StartupPath & "\Data\hardware.conf"
    End Sub

    Shared Function __initialize() As Integer
        __instance.initialize()
        Return 0
    End Function

    Shared Function refresh() As Integer
        'parallel task firing , return regardless if hardware finished works
        __instance.dataDispatch()
        Return 0
    End Function



    '-------------------------------------------------------
    '   Addressing Mode Description:
    'byte() = BitConverter.ToUInt64({31, 0, 3, 0, 2, 0, 0, &H10}
    '                                LowestByte             Highest Byte
    'Byte 7(Highest Byte)   : Hardware Code
    'Byte 6-2               : Hardware Specfic
    'Byte 0(Loweset Byte)   : Bit Index
    'Byte 1                 : Data Index
    '-------------------------------------------------------
    Public Shared Function readDouble(globalAddress As UInt64) As Double
        Return BitConverter.ToDouble(BitConverter.GetBytes(readWord(globalAddress)), 0)
    End Function
    Public Shared Function readWord(globalAddress As UInt64) As UInt64
        'use context to query target word , and return
        Return __instance.readValue(BitConverter.GetBytes(globalAddress))
    End Function
    Public Shared Sub writeWord(globalAddress As UInt64, value As UInt64)
        'use context to find out the memory , and set value
        __instance.writeValue(BitConverter.GetBytes(globalAddress), value)
    End Sub
    Public Shared Sub writeDouble(globalAddress As UInt64, value As Double)
        'turns into UINT64 representation
        writeWord(globalAddress, BitConverter.ToUInt64(BitConverter.GetBytes(value), 0))
    End Sub
    Public Shared Function readBit(globalAddress As UInt64) As Boolean
        'Return instance.readBit(BitConverter.GetBytes(globalAddress))
        Return __instance.readIO(BitConverter.GetBytes(globalAddress))
    End Function
    Public Shared Sub writeBit(globalAddress As UInt64, logicValue As Boolean)
        'base address should not be written , Hsien , 2016.06.03
        If (globalAddress <> 0) Then
            __instance.writeIO(BitConverter.GetBytes(globalAddress), logicValue)
        End If
    End Sub

    ''' <summary>
    ''' Used to scan if all values enumerated in some enumtype in able to query
    ''' </summary>
    ''' <param name="enumtype"></param>
    ''' <remarks></remarks>
    Public Shared Sub scanBit(enumtype As Type)
        For Each item As ULong In CType([Enum].GetValues(enumtype), ULong())
            Try
                __instance.readIO(BitConverter.GetBytes(item))
            Catch ex As Exception
                MessageBox.Show(String.Format("{0}: {1}",
                                              [Enum].GetName(enumtype, item),
                                              ex.ToString))
            End Try
        Next
    End Sub
    Shared Sub shutBits(enumtype As Type)
        For Each item As ULong In CType([Enum].GetValues(enumtype), ULong())
            Try
                __instance.writeIO(BitConverter.GetBytes(item), False)
            Catch ex As Exception
                MessageBox.Show(String.Format("{0}: {1}",
                                              [Enum].GetName(enumtype, item),
                                              ex.ToString))
            End Try
        Next
    End Sub
#End Region



#Region "persistance interface "
    Shared __serializer As XmlSerializer = New XmlSerializer(GetType(mainIOHardware), {GetType(subHardwareNode),
                                                                                       GetType(amaxCard),
                                                                                       GetType(dmtModbusInterface),
                                                                                       GetType(virtualModule),
                                                                                       GetType(melsecOverEthernet)})
    Public Sub Create(filename As String) Implements IPersistance.Create
        Me.__physicalHardwareList.Add(New subHardwareNode With {.PhysicalHardware = New virtualModule})
        Save()
    End Sub

    <XmlIgnore()>
    <[ReadOnly](True)>
    Public Property Filename As String Implements IPersistance.Filename

    Public Sub Load(filename As String) Implements IPersistance.Load

        'override default filename , once input is valid
        If filename IsNot Nothing AndAlso
            filename.Length <> 0 Then
            Me.Filename = filename
        End If

        If (Not Directory.Exists(My.Application.Info.DirectoryPath & "\Data")) Then
            Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Data")
        End If

        If (Not File.Exists(Me.Filename)) Then
            Me.Create(Nothing)
            Exit Sub
        End If

        Try
            Using sr As StreamReader = New StreamReader(Me.Filename)
                Me.PhysicalHardwareList = CType(__serializer.Deserialize(sr), mainIOHardware).PhysicalHardwareList
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
            Throw
        End Try
    End Sub

    Public Sub Save() Implements IPersistance.Save
        Try
            Using sw As StreamWriter = New StreamWriter(Filename)
                __serializer.Serialize(sw, Me)
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
            Throw
        End Try
    End Sub
#End Region

#Region "intiailzed/run/access"

    Public Overrides Function dataDispatch() As Integer
        Return 0
    End Function

    Public Overrides Function initialize() As Integer
        'filename encoding
        Me.PhysicalHardwareList.ForEach(Sub(__hardware As subHardwareNode)
                                            With __hardware
                                                .PhysicalHardware.initialize()
                                                If (.PhysicalHardware.Status = hardwareStatusEnum.HEALTHY) Then
                                                    'initialized successfully
                                                    Dim __asynchronCaller As Func(Of Integer) = AddressOf .PhysicalHardware.dataDispatch
                                                    __asynchronCaller.BeginInvoke(Nothing, Nothing)
                                                End If
                                            End With
                                        End Sub)
        Return 0
    End Function

    Function locateHardware(addressCodeInByte() As Byte) As hardwareBase
        Return Me.__physicalHardwareList.Find(Function(__hardware As subHardwareNode) (__hardware.PhysicalHardware.HardwareCode = addressCodeInByte(hardwareBase.generalAddressCode.BYTE_INDEX_HARDWARE_CODE))).PhysicalHardware
    End Function
    Public Overrides Function readValue(addressCodeInByte() As Byte) As ULong
        'use context to query target word , and return
        Return locateHardware(addressCodeInByte).readValue(addressCodeInByte)
    End Function

    Public Overrides Sub writeValue(addressCodeInByte() As Byte, value As ULong)
        'use context to find out the memory , and set value
        locateHardware(addressCodeInByte).writeValue(addressCodeInByte, value)
    End Sub
#End Region

End Class


Public MustInherit Class hardwareBase

    Enum hardwareStatusEnum
        'LOST    'enabled , but encountered error
        HEALTHY = &H10 'worked well
        PARTIAL_FAILED = &H11   'part of hardware failed
        FAILED = &H20 'suspend
    End Enum


    Protected Friend memoryHold As UInt64()   'used to store hardware polled memory pool , use the memory block type rather list

    ReadOnly Property HardwareCode As Byte
        Get
            Return __hardwareCode
        End Get
    End Property
    Protected __hardwareCode As Byte = mainIOHardware.hardwareCodeEnum.VIRTUAL    ' used to identify 
    'status
    Overridable ReadOnly Property PollingCycle As Double
        Get
            Return __pollingCycle.TotalMilliseconds
        End Get
    End Property

    Overridable ReadOnly Property Status As hardwareStatusEnum
        Get
            Return __status
        End Get
    End Property
    Protected Friend __status As hardwareStatusEnum = hardwareStatusEnum.HEALTHY

    Public MustOverride Function initialize() As Integer                   ' the hardware initialization procedure
    Public MustOverride Function dataDispatch() As Integer                  ' the running method
    Public Overridable Sub buildRawSeed()
        'Xml Deseriaiizer may cause extra items if constructor would allocate default item
        'use this function to put default seed on list
    End Sub

    Enum generalAddressCode As Integer
        BYTE_INDEX_BIT_INDEX = 0
        BYTE_INDEX_DATA_INDEX = 1
        BYTE_INDEX_HARDWARE_CODE = 7
    End Enum

    Public MustOverride Function readValue(addressCodeInByte As Byte()) As UInt64 'use context to query target word , and return
    Public MustOverride Sub writeValue(addressCodeInByte As Byte(), value As UInt64) 'use context to find out the memory , and set value

    Public Function readIO(addressCodeInByte As Byte()) As Boolean
        Return readBitFromWord(readValue(addressCodeInByte), addressCodeInByte(generalAddressCode.BYTE_INDEX_BIT_INDEX))
    End Function
    Public Sub writeIO(addressCodeInByte As Byte(), logicValue As Boolean)
        Dim lastValue = readValue(addressCodeInByte)
        writeBitToWord(lastValue, addressCodeInByte(generalAddressCode.BYTE_INDEX_BIT_INDEX), logicValue)
        writeValue(addressCodeInByte, lastValue)
    End Sub

    Protected __stopWatch As Stopwatch = New Stopwatch
    Protected __pollingCycle As TimeSpan = New TimeSpan(0, 0, 0, 0, 100)
    Protected Shared Function readBitFromWord(wordValue As UInt64, bitIndex As Byte) As Boolean
        'bit index data : 0-31 (zero based)
        Return ((wordValue And CULng((2 ^ bitIndex))) > 0)
    End Function
    Protected Shared Sub writeBitToWord(ByRef wordValue As UInt64, bitIndex As Byte, logicValue As Boolean)
        'bitInex : ,zero-based addressing mode
        If (bitIndex > 63) Then
            Throw New Exception(String.Format("Arguments bitIndex Overflowed , {0}", bitIndex))
        End If

        If (logicValue) Then
            'set
            wordValue = wordValue Or CULng(2 ^ (bitIndex))
        Else
            'unset
            wordValue = wordValue And (Not CULng(2 ^ (bitIndex)))
        End If
    End Sub

End Class





Class hardwareEditor
    Inherits UITypeEditor

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

        Dim __returnObject As Object = Nothing  'used to keep object
        'Dim formService As IWindowsFormsEditorService = provider.GetService(GetType(IWindowsFormsEditorService))
        Using __dialog As Form = New Form
            Using __control As userControlHardwareNode = New userControlHardwareNode With {.HardwareNodeReference = value}
                __dialog.Controls.Add(__control)
                __dialog.AutoSize = True
                __dialog.AutoSizeMode = AutoSizeMode.GrowAndShrink
                __dialog.StartPosition = FormStartPosition.CenterScreen
                __dialog.ShowDialog()
                __returnObject = __control.HardwareNodeReference
            End Using
        End Using

        Return __returnObject
    End Function

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

End Class