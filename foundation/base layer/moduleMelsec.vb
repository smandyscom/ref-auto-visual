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
Imports nspMxCOM

''' <summary>
''' Q Serires accomplished with QJ71E71-100 ethernet interface
''' </summary>
''' <remarks></remarks>
Public Class melsecOverEthernet
    Inherits hardwareBase
    Implements IDisposable
    ''' <summary>
    ''' Used to control polling interval
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property PollingInterval As Single = 100.0F    'in ms
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BReadStartAddress() As String
        Get
            Return __bReadStartAddress
        End Get
        Set(ByVal value As String)
            'try parsing numeric address part
            If Integer.TryParse(value.Substring(1), Globalization.NumberStyles.HexNumber, Nothing, __bReadStartAddressNumeric) Then
                __bReadStartAddress = value
            End If
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BWriteStartAddress() As String
        Get
            Return __bWriteStartAddress
        End Get
        Set(ByVal value As String)
            'try pasring numeric address part
            If Integer.TryParse(value.Substring(1), Globalization.NumberStyles.HexNumber, Nothing, __bWriteStartAddressNumeric) Then
                __bWriteStartAddress = value
            End If
        End Set
    End Property

    Friend __bReadStartAddressNumeric As Integer = 0
    Private __bReadStartAddress As String = "B000"
    Friend __bWriteStartAddressNumeric As Integer = 0
    Private __bWriteStartAddress As String = "B100"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property WReadStartWord As String
        Get
            Return String.Format("{0:X}", __wReadStartWord)
        End Get
        Set(value As String)
            __wReadStartWord = Convert.ToInt32(value, 16)
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property WWriteStartWord As String
        Get
            Return String.Format("{0:X}", __wWriteStartWord)
        End Get
        Set(value As String)
            __wWriteStartWord = Convert.ToInt32(value, 16)
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property WReadRange As Integer   '0-(DReadRange-1)
        Get
            Return __wReadRange
        End Get
        Set(value As Integer)
            __wReadRange = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property WWriteRange As Integer  'DReadRange-DWriteRange
        Get
            Return __wWriteRange
        End Get
        Set(value As Integer)
            __wWriteRange = value
        End Set
    End Property
    Property LogicalStationNumber As Integer = 1
    ''' <summary>
    ''' In ms
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ReconnectionInterval As Integer = 1000

    ReadOnly Property ReadBit(categrory As categroryCodeEnum, bitIndex As Integer) As Boolean
        Get
            Return hardwareBase.readBitFromWord(memoryHold(categrory), bitIndex)
        End Get
    End Property
    WriteOnly Property WriteBit(categrory As categroryCodeEnum, bitIndex As Integer) As Boolean
        Set(value As Boolean)
            hardwareBase.writeBitToWord(memoryHold(categrory), bitIndex, value)
        End Set
    End Property
    ReadOnly Property ReadWValue(wordIndex As Integer)
        Get
            Return memoryHold(categroryCodeEnum.W_BASE + wordIndex)
        End Get
    End Property
    WriteOnly Property WriteWValue(wordIndex As Integer) As ULong
        Set(value As ULong)
            memoryHold(categroryCodeEnum.W_BASE) = value
        End Set
    End Property

    Protected __wReadRange As Integer = 40
    Protected __wWriteRange As Integer = 40
    ''' <summary>
    ''' Represented in HEX
    ''' </summary>
    ''' <remarks></remarks>
    Friend __wReadStartWord As Integer = 0
    ''' <summary>
    ''' Represented in HEX
    ''' </summary>
    ''' <remarks></remarks>
    Friend __wWriteStartWord As Integer = &H428

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Enum melsecAddressCodeEnum
        BYTE_INDEX_CATEGRORY = 3
        BYTE_INDEX_DEVICE = 2
    End Enum


    Enum categroryCodeEnum
        B_IN_BASE = 0
        B_OUT_BASE = 1
        ''' <summary>
        ''' The start-index in memory hold
        ''' </summary>
        ''' <remarks></remarks>
        W_BASE = 16
    End Enum


    'the head start node to address 
    Friend tunnel As clsMxComLib = Nothing

    Public Overrides ReadOnly Property Status As hardwareStatusEnum
        Get
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
            Return totalCycleTime
        End Get
    End Property
    Dim totalCycleTime As Double = 0


    Enum masterStateEnum As Integer
        WORKING
        RECONNECTING
    End Enum
    Dim state As masterStateEnum = masterStateEnum.RECONNECTING
    Public Overrides Function dataDispatch() As Integer

        While True
            Select Case state
                Case masterStateEnum.WORKING
                    '----------------------------
                    '   Data Mapping I/O
                    '----------------------------
                    Dim result As Boolean = True
                    With tunnel
                        '----------------
                        '   B Read-in
                        '----------------
                        Dim raw As Long = 0
                        result = result And .Read64(__bReadStartAddress, raw)
                        memoryHold(0) = BitConverter.ToUInt64(BitConverter.GetBytes(raw), 0)
                        '---------------
                        '   B Write-out
                        '---------------
                        raw = BitConverter.ToInt64(BitConverter.GetBytes(memoryHold(1)), 0)
                        result = result And .Write64(__bWriteStartAddress, raw)
                        '---------------
                        '   W Read-in
                        '---------------
                        For index = 0 To __wReadRange - 1
                            result = result And .Read16(String.Format("W{0:X}", __wReadStartWord + index),
                                    memoryHold(16 + index))
                        Next
                        '---------------
                        '   W Write-out
                        '---------------
                        For index = 0 To __wWriteRange - 1
                            Dim temp = BitConverter.GetBytes(memoryHold(16 + __wReadRange + index))
                            Dim value = BitConverter.ToInt16({temp(0),
                                                              temp(1),
                                                              temp(2),
                                                              temp(3)}, 0)
                            result = result And .Write16(String.Format("W{0:X}", __wWriteStartWord + index), value)
                        Next
                    End With
                    'any error , connection failed , redo
                    If Not result Then
                        state = masterStateEnum.RECONNECTING
                    End If
                    Thread.Sleep(PollingInterval)

                Case masterStateEnum.RECONNECTING
                    Try
                        Dim result As Boolean = False
                        While result = False
                            If (tunnel IsNot Nothing) Then
                                tunnel.CLose()
                            End If
                            tunnel = New nspMxCOM.clsMxComLib
                            result = tunnel.Open(LogicalStationNumber) 'virtual station number
                            Thread.Sleep(ReconnectionInterval) 'reconnection cycle time
                        End While
                        state = masterStateEnum.WORKING
                    Catch ex As Runtime.InteropServices.COMException
                        Return 0 'no COM , abort thread
                    End Try
            End Select

        End While

        Return 0

    End Function

    Public Overrides Function initialize() As Integer
        'direct start thread
        Dim __asynchronCaller As Func(Of Integer) = AddressOf dataDispatch
        __asynchronCaller.BeginInvoke(Nothing, Nothing)

        memoryHold = New ULong(16 + __wReadRange + __wWriteRange) {}
        Return 0
    End Function

#Region "program unified interface"
    Function locateIndex(localAddress As Byte()) As Integer

        Return localAddress(melsecAddressCodeEnum.BYTE_INDEX_CATEGRORY) +
            localAddress(melsecAddressCodeEnum.BYTE_INDEX_DEVICE)
    End Function
    Public Overrides Function readValue(localAddress As Byte()) As ULong
        Return memoryHold(locateIndex(localAddress))
    End Function
    Public Overrides Sub writeValue(localAddress As Byte(), value As ULong)
        memoryHold(locateIndex(localAddress)) = value
    End Sub
#End Region

    Sub New()
        __status = hardwareStatusEnum.FAILED
        __hardwareCode = hardwareCodeEnum.MELSEC_ETHERNET
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If

            '---------------
            '   B Write-out/cLEAR ALL
            '---------------
            Dim zero As Integer = 0
            Dim raw = BitConverter.ToInt32(BitConverter.GetBytes(zero), 0)
            Try
                tunnel.Write32(__bWriteStartAddress, raw)
                tunnel.CLose()
            Catch ex As Exception

            End Try
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

End Class


