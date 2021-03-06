﻿Imports System.Xml.Serialization
Imports Automation
Imports System.ComponentModel
Imports System.Reflection
Imports System.IO

Public Interface IPersistance
    <XmlIgnore()>
    <Browsable(False)>
    Property Filename As String
    Sub Save()
    Sub Create(filename As String)
    Sub Load(filename As String)
End Interface


<Serializable()>
Public Class settingBase
    Implements IPersistance

    <XmlIgnore()>
    <Browsable(False)>
    Public Overridable Property Filename As String Implements IPersistance.Filename

    Public Overridable Sub Load(filename As String) Implements IPersistance.Load

        'override default filename , once input is valid
        If filename IsNot Nothing AndAlso
            filename.Length <> 0 Then
            Me.Filename = filename
        End If

        If (Not File.Exists(Me.Filename)) Then
            Me.Create(Me.Filename)
            Exit Sub
        End If

        Dim tempData As Object = Activator.CreateInstance(Me.GetType())
        OpenXmlFile(tempData, Me.Filename)

        If (tempData IsNot Nothing) Then
            '-----------------------------------------
            '   Value assignment by reflection
            '-----------------------------------------
            Dim __propertyInfos As PropertyInfo() = Me.GetType.GetProperties()
            For Each __pi As PropertyInfo In __propertyInfos
                'Hsien , 2016.02.02, skip the xmlIgnored attributed property
                If (Not Attribute.IsDefined(__pi, GetType(XmlIgnoreAttribute)) And
                    __pi.GetIndexParameters.Count = 0 And
                    __pi.CanWrite) Then
                    __pi.SetValue(Me, __pi.GetValue(tempData, Nothing), Nothing)
                End If

            Next
            Me.Filename = filename
        Else
            Throw New Exception 'load failed
        End If
    End Sub

    Public Overridable Sub Save() Implements IPersistance.Save
        SaveXmlFile(Me, Filename)
    End Sub
    Public Overridable Sub Create(filename As String) Implements IPersistance.Create
        SaveXmlFile(Me, filename)
        Me.Filename = filename
    End Sub

    Public Event PropertyChanged As EventHandler
    Public Sub applyPropertyChange()
        RaiseEvent PropertyChanged(Me, Nothing)
    End Sub

End Class
