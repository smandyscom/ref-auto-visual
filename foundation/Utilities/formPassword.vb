﻿Imports System.IO

Public Class formPassword
    Dim __password As String = "1234"   'default password , Hsien  ,2015.06.16
    Private Sub loadForm(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim __fi As FileInfo = New FileInfo(My.Application.Info.DirectoryPath + "\Data\" + "password.dat")

        Try
            If (__fi.Exists) Then
                __password = File.ReadAllText(My.Application.Info.DirectoryPath + "\Data\" + "password.dat")
            Else
                'use default password
                TextBoxStatus.Text = My.Resources.PASSWORD_INEXISTED_USE_DEFAULT
            End If
        Catch ex As Exception
            TextBoxStatus.Text = My.Resources.PASSWORD_INEXISTED_USE_DEFAULT
        End Try

        TextBoxLogIn.Clear()    'Hsien , 2016.03.25 , missed to clear password
    End Sub

    Sub buttonClick(sender As Button, e As EventArgs) Handles ButtonEnter.Click, ButtonExit.Click, ButtonLogIn.Click

        Select Case sender.Name
            Case ButtonLogIn.Name

                If (TextBoxLogIn.Text = __password) Then
                    DialogResult = Windows.Forms.DialogResult.OK    'successfully
                Else
                    'show incorrect password
                    TextBoxStatus.Text = My.Resources.PASSWORD_INCORRECT
                End If

            Case ButtonExit.Name

                DialogResult = Windows.Forms.DialogResult.Cancel

            Case ButtonEnter.Name

                If (TextBoxOld.Text = __password AndAlso
                   TextBoxNew.Text = TextBoxAgain.Text) Then

                    __password = TextBoxNew.Text
                    File.WriteAllText(My.Application.Info.DirectoryPath + "\Data\" + "password.dat", __password)

                    TextBoxStatus.Text = My.Resources.PASSWORD_SAVED
                End If

            Case Else

        End Select


    End Sub


End Class