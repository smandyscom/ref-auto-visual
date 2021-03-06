﻿Imports Cognex.VisionPro
Imports Cognex.VisionPro.Implementation.Internal
Imports Cognex.VisionPro.ToolBlock
Imports Cognex.VisionPro.ImageFile
Imports MathNet.Numerics.LinearAlgebra

Public Class CognexVision
    Public Event WorkDone()
    Public Event ToolblockChanged()

    Private __output As CogToolBlockTerminalCollection = Nothing
    Public ReadOnly Property output As CogToolBlockTerminalCollection
        Get
            Return __output
        End Get
    End Property

    Public WithEvents toolblock As CogToolBlock = Nothing

    Private _cameras As CogFrameGrabbers
    Private _cameraList As List(Of Object) = New List(Of Object)
    Private _fifoList As List(Of Object) = New List(Of Object)
    Private trigNum As Integer = 0

    Const VIDEO_FORMAT As String = "Generic GigEVision (Mono)"

    Public Sub New()
        ' check license
        Dim licensedFeatures As CogStringCollection = CogMisc.GetLicensedFeatures(False)
        If licensedFeatures.Count = 0 Then
            MessageBox.Show("No VisionPro Dongle.")
        End If

        ' preload toolblocks
        '_toolblocksList.Add(CogSerializer.LoadObjectFromFile(Application.StartupPath + "\VisionPro\ToolBlocks\CogToolBlock_FindLpcPosAng_RightSide_noImg.vpp"))
    End Sub

    Public Function LoadToolblock(strFileName As String) As Boolean
        toolblock = CogSerializer.LoadObjectFromFile(strFileName)
        RaiseEvent ToolblockChanged()
        Return True
    End Function

    Public Sub Run()
        If toolblock IsNot Nothing Then
            toolblock.Run()
        Else
            MessageBox.Show("Toolblock wasn't selected.")
        End If
    End Sub

    Public Sub Run(strInputImageFile As String)
        If toolblock IsNot Nothing Then
            toolblock.Inputs.Item("OutputImage").Value = CogImageFormFile(strInputImageFile)
            toolblock.Run()
        Else
            MessageBox.Show("Toolblock wasn't selected.")
        End If
    End Sub

    Private Function CogImageFormFile(strFileName As String) As CogImage8Grey
        Dim img As CogImageFileBMP = New CogImageFileBMP()
        img.Open(strFileName, CogImageFileModeConstants.Read)
        Return img(0)
    End Function


    Private Sub Subject_Ran(ByVal sender As Object, ByVal e As System.EventArgs) Handles toolblock.Ran
        Select Case toolblock.RunStatus.Result
            Case CogToolResultConstants.Accept
                __output = toolblock.Outputs
                'MessageBox.Show("Toolblock running success.")
            Case CogToolResultConstants.Error
                MessageBox.Show("Toolblock running error")
            Case CogToolResultConstants.Warning
                MessageBox.Show("Toolblock warning")
            Case CogToolResultConstants.Reject
                MessageBox.Show("Toolblock reject")
        End Select
        RaiseEvent WorkDone()
    End Sub

    Public Function GetCameras() As Integer
        _cameras = New CogFrameGrabbers()
        For index = 0 To _cameras.Count - 1
            _cameraList.Add(New Object)
            _cameraList(index) = _cameras(index)

            _fifoList.Add(New Object)
            _fifoList(index) = CType(_cameraList(index), ICogFrameGrabber).CreateAcqFifo(VIDEO_FORMAT, CogAcqFifoPixelFormatConstants.Format8Grey, 0, True)
        Next

        Return _cameras.Count
    End Function

    Public Function CaptureOneImage(camera_i As Integer) As Bitmap
        Dim img As ICogImage = Nothing
        If _fifoList.Count <> 0 Then
            img = CType(_fifoList(camera_i), ICogAcqFifo).Acquire(trigNum)
            'CogDisplay1.Image = img
        End If
        Return img.ToBitmap()
    End Function


    Enum keysEnum As Integer
        X
        Y
        OutputImage
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="__output"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function output2Vector(__output As CogToolBlockTerminalCollection) As Vector(Of Double)
        '-----------------
        '   Error Rejection
        '-----------------
        If __output Is Nothing OrElse
           Not __output.Contains(keysEnum.X.ToString) OrElse
           Not __output.Contains(keysEnum.Y.ToString) Then
            Return Nothing
        End If

        With __output
            Return CreateVector.Dense(Of Double)({.Item(keysEnum.X.ToString).Value,
                                                  .Item(keysEnum.Y.ToString).Value,
                                                  0,
                                                  1})
        End With
    End Function


    Shared availiablePositionCount As Integer = 10
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="__output"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function output2VectorCollection(__output As CogToolBlockTerminalCollection) As List(Of Vector(Of Double))

        Dim __list As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

        Dim keyX As String = Nothing
        Dim keyY As String = Nothing

        'at most N keys

        With __output
            For index = 0 To availiablePositionCount - 1
                keyX = String.Format("{0}{1}",
                                     keysEnum.X.ToString,
                                     index)
                keyY = String.Format("{0}{1}",
                                     keysEnum.Y.ToString,
                                     index)
                If __output.Contains(keyX) And
                    __output.Contains(keyY) Then
                    __list.Add(CreateVector.Dense(Of Double)({.Item(keyX).Value,
                                                              .Item(keyY).Value,
                                                              0,
                                                              1}))
                Else
                    '--------------
                    '   Key not existed
                    '--------------
                    __list.Add(Nothing)
                End If
            Next
        End With

        Return __list
    End Function

End Class
