﻿Imports System.ComponentModel

Public Class alarmContextBase
    ' define basic alarm contents
    ' this class used to describe informations the alarm Mutex need
    ' alarm Mutex would use this to  tip user what to do
    Public Enum responseWays As Integer
        'mode of recovering from error
        NONE = &H0         ' 0x0000
        RETRY = &H1        ' 0x0001do not rising flagIgnore
        IGNORE = &H10       ' 0x0010rising flagIgnore
        ABORT = &H100          ' 0x0100
        OPTION3 = &H1000    'Hsien , 2016.03.10 ' option expanded
    End Enum

    Shared Property QueryInterface As IMessageQuery = Nothing
    Public Shared abortMethod As Func(Of Boolean) = Function() (True)   'hsien , 2015.10.05 , used to link assembly abort 

    Property Sender As Object
    Property PossibleResponse As responseWays = responseWays.IGNORE Or responseWays.RETRY
    Property CallbackResponse As Dictionary(Of responseWays, Func(Of Boolean)) = New Dictionary(Of responseWays, Func(Of Boolean))       ' return value definition: true , handling over , false , on handling

    Property AdditionalInfo As String = ""                         ' indicate physical meaning of this sensor , Hsien , move from sensor to base , 2015.04.09

    Sub New()
        Dim defaultCallback = Function() (True)

        With CallbackResponse
            .Add(responseWays.NONE, defaultCallback)
            .Add(responseWays.IGNORE, defaultCallback)
            .Add(responseWays.RETRY, defaultCallback)
            .Add(responseWays.ABORT, abortMethod)
            .Add(responseWays.OPTION3, defaultCallback)
        End With
    End Sub

    Public Overrides Function ToString() As String
        Try

            Dim senderString As String = ""

            If (Sender Is Nothing) Then
                'Throw New Exception
                senderString = "Sender Cannot Recognize (In alarmContextBase)"
            Else
                senderString = Sender.ToString()
            End If

            Return senderString

        Catch ex As Exception

            Return "Sender Cannot Recognize (In alarmContextBase)" & ex.Message

        End Try
    End Function

End Class



Public Class alarmContentSensor : Inherits alarmContextBase

    <TypeConverter(GetType(alarmReasonTypeConvertor))>
    Enum alarmReasonSensor As Integer
        SHOULD_BE_ON = 1
        SHOULD_BE_OFF = 0
        SHOULD_RISING = 2
        SHOULD_FALLING = 3
        NO_PULSE = 4
        UNKNOWN_PULSE = 5
        PULSE_WIDTH_OVERRED = 6 'Hsien , 2014.10.07
    End Enum
    Class alarmReasonTypeConvertor
        Inherits TypeConverter

        Dim __conversionDictionary As BiDictionary(Of alarmReasonSensor, String) = New BiDictionary(Of alarmReasonSensor, String)
        Sub New()
            With __conversionDictionary
                .Add(alarmReasonSensor.SHOULD_BE_ON, "應該為ON")
                .Add(alarmReasonSensor.SHOULD_BE_OFF, "應該為OFF")
                .Add(alarmReasonSensor.SHOULD_RISING, "應該為上升緣")
                .Add(alarmReasonSensor.SHOULD_FALLING, "應該為下降緣")
                .Add(alarmReasonSensor.NO_PULSE, "無脈波")
                .Add(alarmReasonSensor.UNKNOWN_PULSE, "異常脈波")
                .Add(alarmReasonSensor.PULSE_WIDTH_OVERRED, "脈波寬度過大")

            End With
        End Sub

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            Return __conversionDictionary.GetByFirst(value).First
        End Function
        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
            Return __conversionDictionary.GetBySecond(value).First
        End Function
    End Class

    'make it overridable , so that derived class able to use this property , 2015.11.03
    Overridable Property Inputs As ULong
        Get
            Return __input
        End Get
        Set(value As ULong)
            __input = value
        End Set
    End Property
    Shared Property InputsEnumType As Type = Nothing
    Property Reason As alarmReasonSensor = alarmReasonSensor.SHOULD_BE_ON     ' indicate mechanical status

    Protected __input As ULong = 0

    Public Overrides Function ToString() As String
        Try
            If (InputsEnumType Is Nothing) Then
                Throw New NullReferenceException()
            End If

            Dim inputEnum As [Enum] = [Enum].ToObject(InputsEnumType, Inputs)

            Dim inputString As String = [Enum].GetName(InputsEnumType, inputEnum)
            Dim reasonString As String = Reason.ToString()
            Dim formatString As String = "Sensor {0} : {1} {2} "        ' the default format

            '-----------------------------------
            '   I.e : Sensor Spb1 : Should Be On
            '-----------------------------------
            Return String.Format(formatString,
                                 inputString,
                                 reasonString,
                                 vbCrLf)

        Catch ex As Exception
            '-------------
            ' ex : InputsEnumType not linked
            '-------------
            Return String.Format("Sensor {0} : {1} {2} {3}",
                                 Inputs & "Exception : Inputs Enum Type Not Linked ",
                                 Reason.ToString(),
                                 vbCrLf,
                                 ex.Message)
        End Try

    End Function

End Class
Public Class alarmContentConveyor : Inherits alarmContentSensor
    '----------
    '   Used to offer standard message string:
    ' 1. Wafer Jammed
    ' 2. Wafer Loss
    ' 3. Wafer Unknown
    ' 4. Wafer Broken
    '----------
    Public Enum alarmReasonConveyor
        WAFER_LOSS
        WAFER_JAMMED
        WAFER_UNKNOWN
        WAFER_BROKEN
    End Enum

    Property Position As UInteger = 0       ' indicating the position which occured alarm , added by Hsien , 2014.10.29
    Property Detail As alarmReasonConveyor = alarmReasonConveyor.WAFER_LOSS

    Public Overrides Function ToString() As String

        Dim detailString As String = Detail.ToString()
        Dim formatString As String = "{1}{0}Detail:{0}{2}Position:{3}"

        Return vbCrLf + String.Format(formatString,
                                      vbCrLf,
                                      MyBase.ToString(),
                                      detailString,
                                      Position)

    End Function
End Class

