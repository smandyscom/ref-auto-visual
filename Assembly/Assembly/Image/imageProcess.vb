﻿Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric
Imports Automation
Imports Cognex.VisionPro.ToolBlock
Imports Cognex.VisionPro

Imports System.Xml.Serialization
Imports System.IO
Imports Cognex.VisionPro.ImageFile

Imports System.ComponentModel
Imports System.Drawing.Design

Public Class imageProcessEndEventArgs
    Inherits EventArgs

    ReadOnly Property OutputPositionsUnit As List(Of Vector(Of Double))
        Get
            Return __unitCoords
        End Get
    End Property
    ReadOnly Property OutputPositionsPixel As List(Of Vector(Of Double))
        Get
            Return __pixelCoords
        End Get
    End Property

    ''' <summary>
    ''' As 4x1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property OutputPositionUnit(Optional index As Integer = 0) As Vector(Of Double)
        Get
            Return __unitCoords(index)
        End Get
    End Property
    ''' <summary>
    ''' As 4x1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property OutputPositionPixel(Optional index As Integer = 0) As Vector(Of Double)
        Get
            Return __pixelCoords(index)
        End Get
    End Property

    ReadOnly Property Result As CogToolResultConstants
        Get
            Return __result.Result
        End Get
    End Property

    Dim __output As CogToolBlockTerminalCollection = Nothing
    Dim __result As ICogRunStatus = Nothing

    ''' <summary>
    ''' As 4x1
    ''' </summary>
    ''' <remarks></remarks>
    Dim __pixelCoords As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
    ''' <summary>
    ''' As 4x1
    ''' </summary>
    ''' <remarks></remarks>
    Dim __unitCoords As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))


    Sub New(ByVal outputPosition As CogToolBlockTerminalCollection,
            result As ICogRunStatus)

        Me.__output = outputPosition
        Me.__result = result

        For Each item As Vector(Of Double) In CognexVision.output2VectorCollection(Me.__output)
            __pixelCoords.Add(item)
            With __unitCoords
                If item IsNot Nothing Then
                    .Add(pixelCoord2unitCoord(item))
                Else
                    .Add(Nothing)
                End If
            End With
        Next

    End Sub

    Shared pixel2mmRatio As Double = 0.00275
    Shared xPixels As Double = 2590
    Shared yPixels As Double = 1942

    ''' <summary>
    ''' Default Origin : Left-Up Corner
    ''' Output Origin : FOV Center
    ''' Default Y-axis : to bottom side of FOV
    ''' Output Y-axis : to upper side of FOV
    ''' </summary>
    ''' <param name="pixelCoord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function pixelCoord2unitCoord(pixelCoord As Vector(Of Double)) As Vector(Of Double)
        Dim pixelCoordX = pixelCoord(0) - xPixels / 2
        Dim pixelCoordY = pixelCoord(1) - yPixels / 2
        Return CreateVector.Dense(Of Double)({pixelCoordX * pixel2mmRatio,
                                              pixelCoordY * pixel2mmRatio,
                                              0,
                                              1})
    End Function

End Class

''' <summary>
''' Used to trigger camera
''' </summary>
''' <remarks></remarks>
Public Class imageProcessTriggerEventArgs
    Inherits EventArgs

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event ImageProcessDone(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' The cognex tool block object
    ''' Contains some specific image process flow
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ToolBlock As CogToolBlock
        Get
            Return __toolBlock
        End Get
    End Property
    ReadOnly Property LightChannel As Integer
        Get
            Return __lightChannel
        End Get
    End Property
    ReadOnly Property LightIntensity As Integer
        Get
            Return __lightIntensity
        End Get
    End Property
    ReadOnly Property LightChannel2 As Integer
        Get
            Return __lightChannel2
        End Get
    End Property
    ReadOnly Property LightIntensity2 As Integer
        Get
            Return __lightIntensity2
        End Get
    End Property

    Dim WithEvents __toolBlock As CogToolBlock = Nothing
    Dim __lightChannel As Integer = 0
    Dim __lightIntensity As Integer = 0
    Dim __lightChannel2 As Integer = 0
    Dim __lightIntensity2 As Integer = 0

    Sub New(__toolBlock As CogToolBlock,
            __lightChannel As Integer,
            __lightIntensity As Integer,
            __lightChannel2 As Integer,
            __lightIntesity2 As Integer)

        Me.__toolBlock = __toolBlock
        Me.__lightChannel = __lightChannel
        Me.__lightIntensity = __lightIntensity

        Me.__lightChannel2 = __lightChannel2
        Me.__lightIntensity2 = __lightIntesity2
    End Sub

    ''' <summary>
    ''' Regular the interface
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub toolBlockExecuted(sender As Object, e As EventArgs) Handles __toolBlock.Ran

        With __toolBlock
            RaiseEvent ImageProcessDone(Me, New imageProcessEndEventArgs(.Outputs,
                                                                         .RunStatus))
            'output image , tagged with toolblock name
            If .Outputs IsNot Nothing AndAlso
                .Outputs.Contains(CognexVision.keysEnum.OutputImage.ToString) Then
                'Using cif As CogImageFile = New CogImageFile()
                '    cif.Open(String.Format("{0}{1}_{2}.bmp",
                '                           imageSavePath,
                '                           Now.ToString("yyyyMMddhhmmss"),
                '                           .Name), CogImageFileModeConstants.Write)
                '    cif.Append(.Outputs.Item(CognexVision.keysEnum.OutputImage.ToString).Value)
                '    cif.Close()
                'End Using
            End If
        End With
    End Sub

    Shared imageSavePath As String = My.Application.Info.DirectoryPath & "/Data/images/"

End Class


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Editor(GetType(utilitiesUI.popupPropertyGridEditor), GetType(UITypeEditor))>
Public Class imageProcessSettingBlock
    Inherits settingBase

    Protected __toolBlock As CogToolBlock = Nothing

    ''' <summary>
    ''' In percentage representation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property LightDensity As Double = 1.0F
    ''' <summary>
    ''' Indicate which channel used
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property LightChannel As Integer = 0

    ''' <summary>
    ''' In percentage representation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property LightDensity2 As Double = 1.0F
    ''' <summary>
    ''' Indicate which channel used
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property LightChannel2 As Integer = 0

    <utilitiesUI.path("\Data\vpps\")>
    <utilitiesUI.filter("vpp files (*.vpp)|*.vpp")>
    <Editor(GetType(utilitiesUI.FileNamesEditor), GetType(UITypeEditor))>
    Public Property VppFilename As String
        Get
            If Not Directory.Exists(vppPath) Then
                Directory.CreateDirectory(vppPath)
            End If
            If Not File.Exists(VppFullFilename) Then
                File.Create(VppFullFilename)
            End If
            Return __vppFilename
        End Get
        Set(value As String)

            If value <> __vppFilename Then
                'Reload cog block
                Try
                    __toolBlock = TryCast(CogSerializer.LoadObjectFromFile(vppPath & value), CogToolBlock)
                    __vppFilename = value
                Catch ex As Exception

                End Try
            Else
                '------------------------
                '   Nothing Changed
                '------------------------
            End If

        End Set
    End Property
    <Browsable(False)>
    ReadOnly Property VppFullFilename As String
        Get
            Return vppPath & __vppFilename
        End Get
    End Property
    Shared vppPath As String = My.Application.Info.DirectoryPath & "\Data\vpps\"
    Protected __vppFilename As String = "default.vpp"

    Public Overrides Function ToString() As String
        Return __vppFilename
    End Function

#Region "agent interface"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns>
    ''' True : image process is working
    ''' False : image process done
    ''' </returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    ReadOnly Property IsImageProcessDone As Boolean
        Get
            If __isImageProcessDone Then
                __isImageProcessDone = False 'reset when read
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Protected __isImageProcessDone As Boolean = False


    ''' <summary>
    ''' 4x1 coordinate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    ReadOnly Property Coordinates As List(Of Vector(Of Double))
        Get
            Return doneHandle.OutputPositionsUnit
        End Get
    End Property

    ReadOnly Property Result As CogToolResultConstants
        Get
            If doneHandle Is Nothing Then
                Return CogToolResultConstants.Error
            End If
            Return doneHandle.Result
        End Get
    End Property

    ''' <summary>
    ''' Used to trigger external procedure
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Shared Event CameraTriggered(ByVal sender As Object, ByVal e As EventArgs)

    Public Sub onCameraTriggered()
        triggerHandle = New imageProcessTriggerEventArgs(Me.__toolBlock,
                                                         Me.LightChannel,
                                                         Me.LightDensity,
                                                         Me.LightChannel2,
                                                         Me.LightDensity2)
        RaiseEvent CameraTriggered(Me, triggerHandle)
    End Sub

    Friend WithEvents triggerHandle As imageProcessTriggerEventArgs = Nothing
    Friend doneHandle As imageProcessEndEventArgs = Nothing

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub imageProcessDone(sender As Object, e As imageProcessEndEventArgs) Handles triggerHandle.ImageProcessDone
        Me.doneHandle = e
        __isImageProcessDone = True ' flag set
    End Sub


#End Region

End Class