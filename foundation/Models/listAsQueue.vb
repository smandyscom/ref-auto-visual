﻿Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class ListAsQueue(Of T)
    Inherits List(Of T)
    Implements IPersistance

    Public Event CollectionChanged(ByVal sender As Object, ByVal e As EventArgs)
    '-----------------------------
    '   The Queue implemented by List
    '1. List is able to serializing in xml form
    '-----------------------------
    '---------------------------------
    '   Extended Methods
    '---------------------------------
    Public Sub Enqueue(item As T)
        MyBase.Insert(0, item)  'first
        RaiseEvent CollectionChanged(Me, Nothing)
    End Sub
    Public Function Dequeue() As T
        If Count > 0 Then
            Dim __dequeuedObject As T = Me.Last
            Remove(Me.Last)
            RaiseEvent CollectionChanged(Me, Nothing)
            Return __dequeuedObject
        Else
            Return Nothing 'nothing in the list
        End If
    End Function
    Public Function Peek() As T
        If Count > 0 Then
            Return Me.Last
        Else
            Return Nothing 'nothing in the list
        End If
    End Function
    Public Shadows Function Remove(item As T) As Boolean
        Dim result As Boolean = MyBase.Remove(item)
        RaiseEvent CollectionChanged(Me, Nothing)
        Return result
    End Function
    Public Shadows Sub Add(item As T)
        MyBase.Add(item)
        RaiseEvent CollectionChanged(Me, Nothing)
    End Sub

    Public Sub Create(filename As String) Implements IPersistance.Create
        Me.Filename = filename
        Save()
    End Sub


    <XmlIgnore> Public Property Filename As String Implements IPersistance.Filename
    <XmlIgnore> Public Serializer As XmlSerializer  'should given the specific run-time type 

    Public Sub Load(filename As String) Implements IPersistance.Load
        Using sr As StreamReader = New StreamReader(filename)
            Dim __object As ListAsQueue(Of T) = Serializer.Deserialize(sr)
            Me.Clear()
            Me.AddRange(__object)   'likely assignment

            Me.Filename = filename
        End Using
    End Sub

    Public Sub Save() Implements IPersistance.Save
        Using sw As StreamWriter = New StreamWriter(Filename, False) 'override files
            Serializer.Serialize(sw, Me)
        End Using

    End Sub


End Class
