﻿Imports System.Text

Public Class flagController(Of enumType As {Structure, IConvertible})

    Public controllerName As String = ""

    ReadOnly Property FlagElementsArray As List(Of flagElement)
        Get
            'Dim tempArray As List(Of flagElement) = New List(Of flagElement)
            'For i = 0 To flags.Length - 1
            '    tempArray.Add(New flagElement(labels, flags, i))
            'Next
            __flagElementsArray.ForEach(Sub(element As flagElement) element.controllerName = Me.controllerName)
            Return __flagElementsArray
        End Get
    End Property


    ReadOnly Property FlagReference As Boolean()
        Get
            Return flags
        End Get
    End Property
    ReadOnly Property LabelReference As String()
        Get
            Return labels
        End Get
    End Property

    Friend labels As String() = Nothing
    Friend values As Array = Nothing
    Friend flags As Boolean() = New Boolean(([Enum].GetValues(GetType(enumType)).Length) - 1) {}

    Friend __flagElementsArray As List(Of flagElement) = New List(Of flagElement)

    Public Sub New()
        ' intialize string array
        labels = [Enum].GetNames(GetType(enumType))
        values = [Enum].GetValues(GetType(enumType))

        For i = 0 To flags.Length - 1
            __flagElementsArray.Add(New flagElement(labels, flags, i, Me.controllerName))
        Next
    End Sub

    ' interface to external controller
    Public Function writeFlag(ByVal flag As enumType, ByVal value As Boolean) As Integer
        flags(fetchIndex(flag)) = value
        Return 0
    End Function
    Public Function viewFlag(ByVal flag As enumType) As Boolean
        ' view without handahske
        Return flags(fetchIndex(flag))
    End Function
    Public Function readFlag(ByVal flag As enumType) As Boolean
        ' handshake
        If (flags(fetchIndex(flag))) Then
            flags(fetchIndex(flag)) = False
            Return True
        End If
        Return False
    End Function
    Public Function resetFlag(ByVal flag As enumType) As Integer
        flags(fetchIndex(flag)) = False
        Return 0
    End Function
    Public Function setFlag(ByVal flag As enumType) As Integer
        flags(fetchIndex(flag)) = True
        Return 0
    End Function
    Public Sub clearFlags()
        '----------------------
        '   Reset all flags to false
        '----------------------
        For index = 0 To flags.Length - 1
            flags(index) = False
        Next
    End Sub

    Public Overrides Function ToString() As String
        '-----------------------
        '   Reflect values
        '-----------------------
        Dim __sb As StringBuilder = New StringBuilder
        For index = 0 To labels.Count - 1
            __sb.AppendLine(String.Format("Flag Name:{0};Value:{1}",
                                          labels(index),
                                          flags(index)))
        Next
        Return __sb.ToString
    End Function


    Protected Function fetchIndex(ByVal flag As enumType) As Integer
        Dim name As String = [Enum].GetName(flag.GetType, flag)
        If name IsNot Nothing Then
            'represented in enum
            Return Array.IndexOf(labels, name)
        Else
            'represented in index (value)
            Return flag.ToInt32(Nothing)
        End If
    End Function


End Class


Public Class flagElement
    Private labelReference As String()
    Private flagReference As Boolean()
    Private flagIndex As Short
    Friend controllerName As String = ""
    Public Sub New(ByRef _labelReference As String(), ByRef _flagReference As Boolean(), ByVal _flagIndex As Short, Optional controllerName As String = "")
        Me.labelReference = _labelReference
        Me.flagReference = _flagReference
        Me.flagIndex = _flagIndex
        Me.controllerName = controllerName
    End Sub


    Property Flag As Boolean
        Get
            Return flagReference(flagIndex)
        End Get
        Set(value As Boolean)
            flagReference(flagIndex) = value
        End Set
    End Property
    ReadOnly Property Label As String
        Get
            Return labelReference(flagIndex)
        End Get
    End Property
End Class

