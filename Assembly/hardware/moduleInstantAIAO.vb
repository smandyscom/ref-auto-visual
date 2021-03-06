﻿Imports Automation
Imports Automation.BDaq
Imports System.ComponentModel
Imports System.Drawing.Design

Enum instantAIAOCodeEnum As Integer
    BYTE_INDEX_DEVICENUM = 4
    ''' <summary>
    ''' AI : 0
    ''' AO : 1
    ''' </summary>
    ''' <remarks></remarks>
    BYTE_INDEX_CATEGRORY = 3
    BYTE_INDEX_CHANNEL = 2
End Enum

Public Class moduleDAQ
    Inherits hardwareBase

    Property DeviceList As List(Of moduleInstantAIAO) = New List(Of moduleInstantAIAO)

    Public Overrides Function dataDispatch() As Integer

        While True
            DeviceList.ForEach(Sub(device As moduleInstantAIAO) device.dataDispatch())
        End While

        Return 0
    End Function

    Public Overrides Function initialize() As Integer

        DeviceList.ForEach(Sub(device As moduleInstantAIAO) device.initialize())

        Threading.ThreadPool.QueueUserWorkItem(AddressOf dataDispatch)

        Return 0
    End Function

    Public Overrides Function readValue(addressCodeInByte() As Byte) As ULong
        Return locateDevice(addressCodeInByte).readValue(addressCodeInByte)
    End Function

    Public Overrides Sub writeValue(addressCodeInByte() As Byte, value As ULong)
        locateDevice(addressCodeInByte).writeValue(addressCodeInByte, value)
    End Sub

    Function locateDevice(localAddress As Byte()) As moduleInstantAIAO
        Return DeviceList.Find(Function(__device As moduleInstantAIAO) (__device.DeviceNumber = localAddress(instantAIAOCodeEnum.BYTE_INDEX_DEVICENUM)))
    End Function

    Sub New()
        'as temprorary code
        Me.__hardwareCode = &H10
    End Sub
End Class

Public Class moduleInstantAIAO
    Inherits hardwareBase
    ''' <summary>
    ''' Default the demo device
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DeviceNumber As Integer = 1

    <Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
    ReadOnly Property InputsController As InstantAiCtrl
        Get
            Return __inputsController
        End Get
    End Property

    Dim __inputsController As InstantAiCtrl = New InstantAiCtrl
    Dim __outputsController As InstantAoCtrl = New InstantAoCtrl

    Dim __inputsChannelCounts As Integer = 16
    Dim __outputsChannelCounts As Integer = 16
    Dim returnValue As BDaq.ErrorCode = ErrorCode.Success

    Dim tempBufferIn As Double()
    Dim tempBufferOut As Double()

    Public Overrides Function dataDispatch() As Integer

        If __inputsController.Initialized Then
            returnValue = __inputsController.Read(0, __inputsChannelCounts, tempBufferIn)
            If returnValue = ErrorCode.Success Then
                For index = 0 To __inputsChannelCounts - 1
                    memoryHold(index) =
                        BitConverter.ToUInt64(BitConverter.GetBytes(tempBufferIn(index)), 0)
                Next
            End If
        Else
            '------------------------
            '   Not existed
            '------------------------
        End If

        If __outputsController.Initialized Then
            For index = 0 To __outputsController.ChannelCount - 1
                tempBufferOut(index) = BitConverter.ToDouble(BitConverter.GetBytes(memoryHold(index + __inputsChannelCounts)), 0)
            Next
            __outputsController.Write(0, __outputsChannelCounts, tempBufferOut)
        Else
            '---------------------------
            '
            '---------------------------
        End If

        Threading.Thread.Sleep(__pollingCycle)
        Return 0
    End Function

    Public Overrides Function initialize() As Integer
        Try
            __inputsController.SelectedDevice = New DeviceInformation(DeviceNumber)
            __inputsChannelCounts = __inputsController.ChannelCount
            tempBufferIn = New Double(__inputsChannelCounts) {}
            For Each item As AiChannel In __inputsController.Channels
                With item
                    .SignalType = AiSignalType.SingleEnded
                    .ValueRange = ValueRange.V_0To10
                End With
            Next

        Catch ex As Exception
            'may AI/AO not existed 
        End Try
        Try
            __outputsController.SelectedDevice = New DeviceInformation(DeviceNumber)
            __outputsChannelCounts = __outputsController.ChannelCount
            tempBufferOut = New Double(__outputsChannelCounts) {}

        Catch ex As Exception

        End Try
        memoryHold = New ULong(__inputsChannelCounts + __outputsChannelCounts) {}
        Return 0
    End Function

    Enum categroryCodeEnum As Byte
        AI = 0
        AO = 1
    End Enum


    Public Overrides Function readValue(addressCodeInByte() As Byte) As ULong
        Select Case addressCodeInByte(instantAIAOCodeEnum.BYTE_INDEX_CATEGRORY)
            Case categroryCodeEnum.AI
                Return memoryHold(addressCodeInByte(instantAIAOCodeEnum.BYTE_INDEX_CHANNEL))
            Case categroryCodeEnum.AO
                Return memoryHold(addressCodeInByte(instantAIAOCodeEnum.BYTE_INDEX_CHANNEL) + __inputsChannelCounts)
        End Select

        Return 0
    End Function

    ''' <summary>
    ''' AO only
    ''' </summary>
    ''' <param name="addressCodeInByte"></param>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    Public Overrides Sub writeValue(addressCodeInByte() As Byte, value As ULong)
        memoryHold(addressCodeInByte(instantAIAOCodeEnum.BYTE_INDEX_CHANNEL) + __inputsChannelCounts) =
            value
    End Sub

    Sub New()
        Me.__pollingCycle = New TimeSpan(0, 0, 0, 0, 50)
    End Sub



End Class

