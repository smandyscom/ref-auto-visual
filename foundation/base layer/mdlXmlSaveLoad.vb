﻿Imports System.IO
Imports System.Xml.Serialization

Public Module mdlXmlSaveLoad

    Public Sub SaveXmlFile(ByVal obj As Object, ByVal fullFileName As String)
        'Serialize object to a text file.

        'use "using" to ensure resource would close after operation
        Using objStreamWriter As StreamWriter = New StreamWriter(fullFileName)
            Try
                Dim x As New XmlSerializer(obj.GetType)
                x.Serialize(objStreamWriter, obj)
            Catch ex As Exception
                MessageBox.Show(String.Format("SaveXmlFileError , file:{0} , ex:{1}",
                                              fullFileName,
                                              ex.Message))
            End Try

        End Using
    End Sub

    Public Sub OpenXmlFile(ByRef obj As Object, ByVal fullFileName As String) 'As Integer
        '--------------------------------------------------------
        'Deserialize text file to given type
        ' Hsien 
        '--------------------------------------------------------

        Using sr As StreamReader = New StreamReader(fullFileName)
            Try
                Dim xs As System.Xml.Serialization.XmlSerializer = New System.Xml.Serialization.XmlSerializer(obj.GetType())
                obj = xs.Deserialize(sr)

            Catch ex As Exception
                '判斷錯誤取回的型別
                If ex.GetType Is GetType(System.IO.DirectoryNotFoundException) Then 'err.number=76, 找不到檔案或目錄的一部分時所擲回的例外狀況。
                    MkDir(My.Application.Info.DirectoryPath & "\Data\")
                End If
                MsgBox("OpenXmlFile Error!" & vbCrLf & _
                       "FileName=" & fullFileName & vbCrLf & _
                       "Error Number=" & Err.Number & vbCrLf &
                       "Error Type=" & ex.GetType.ToString & vbCrLf & _
                       ex.Message)

            End Try
        End Using


    End Sub


End Module

