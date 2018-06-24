Imports Cognex.VisionPro
Imports Cognex.VisionPro.Implementation.Internal
Imports Cognex.VisionPro.ToolBlock
Imports Cognex.VisionPro.ImageFile


Public Class UserControl_CognexVision    
    Private WithEvents ref_CogVision As CognexVision = Nothing

    Public Sub Reference(ByRef refCog As CognexVision)
        ref_CogVision = refCog
        If Checkbox_showtb.Checked Then
            CogToolBlockEditV21.Subject = ref_CogVision.toolblock
        End If
    End Sub

    Private Sub ran() Handles ref_CogVision.WorkDone
        CogRecordDisplay1.Image = ref_CogVision.toolblock.Inputs.Item("OutputImage").Value
        CogRecordDisplay1.Record = ref_CogVision.toolblock.CreateLastRunRecord()
    End Sub

    Private Sub toolblockChanged() Handles ref_CogVision.ToolblockChanged
        If Checkbox_showtb.Checked Then
            CogToolBlockEditV21.Subject = ref_CogVision.toolblock
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles Checkbox_showtb.CheckedChanged
        If Checkbox_showtb.Checked Then
            ' Show toolblock sequence            
            If ref_CogVision IsNot Nothing Then
                CogToolBlockEditV21.Subject = ref_CogVision.toolblock
                CogToolBlockEditV21.Visible = True
            End If            
        Else
            CogToolBlockEditV21.Subject = Nothing
            CogToolBlockEditV21.Visible = False
        End If
    End Sub

End Class

'Public Class UserControl_CognexVision

'    '
'    Private WithEvents ref_tooblock As CogToolBlock = Nothing
'    '


'    Property fileName_toolblock As String
'        Get
'            Return __fileName_toolblock
'        End Get
'        Set(value As String)
'            __fileName_toolblock = value
'        End Set
'    End Property

'    Public ReadOnly Property dX As Double
'        Get
'            Return _dX
'        End Get
'    End Property
'    Public ReadOnly Property dY As Double
'        Get
'            Return _dY
'        End Get
'    End Property
'    Public ReadOnly Property dRadian As Double
'        Get
'            Return _dRadian
'        End Get
'    End Property

'    Private __fileName_toolblock As String
'    Private _dX As Double = 0
'    Private _dY As Double = 0
'    Private _dRadian As Double = 0

'    Private WithEvents currentToolblock As CogToolBlock
'    Private imgTool As CogImageFileBMP = New CogImageFileBMP()
'    Private _cameras As CogFrameGrabbers
'    Private _cameraList As List(Of Object) = New List(Of Object)
'    Private _fifoList As List(Of Object) = New List(Of Object)
'    Private trigNum As Integer = 0

'    Const VIDEO_FORMAT As String = "Generic GigEVision (Mono)"

'    Private Sub UserControl_CogToolblock_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        ' check license
'        Dim licensedFeatures As CogStringCollection = CogMisc.GetLicensedFeatures(False)
'        If licensedFeatures.Count = 0 Then
'            MessageBox.Show("No VisionPro Dongle.")
'        End If
'    End Sub

'    Public Sub Run()
'        currentToolblock = CogSerializer.LoadObjectFromFile(__fileName_toolblock)
'        currentToolblock.Run()
'    End Sub

'    Public Sub RunWithImageFile(strFileName As String)
'        currentToolblock = CogSerializer.LoadObjectFromFile(__fileName_toolblock)
'        currentToolblock.Inputs.Item("OutputImage").Value = LoadImageFormFile(strFileName)
'        currentToolblock.Run()
'    End Sub

'    Private Sub Subject_Ran(ByVal sender As Object, ByVal e As System.EventArgs) Handles ref_tooblock.Ran
'        Select Case currentToolblock.RunStatus.Result
'            Case CogToolResultConstants.Accept
'                _dX = currentToolblock.Outputs.Item("X").Value
'                '_dX = currentToolblock.Outputs(0).Value
'                _dY = currentToolblock.Outputs.Item("Y").Value
'                _dRadian = currentToolblock.Outputs.Item("Angle").Value
'                Dim strTest As String() = currentToolblock.Outputs.GetFormattedTerminalStrings()

'                MessageBox.Show(strTest(0) + ";" + strTest(1))

'                CogRecordDisplay1.Image = currentToolblock.Inputs.Item("OutputImage").Value
'                'CogRecordDisplay1.Record = CogToolBlockEditV21.Subject.Tools.Item("CogPixelMapTool1").CreateLastRunRecord
'                CogRecordDisplay1.Record = currentToolblock.CreateLastRunRecord()
'            Case CogToolResultConstants.Error
'                MessageBox.Show("Toolblock running error")
'            Case CogToolResultConstants.Warning
'                MessageBox.Show("Toolblock warning")
'            Case CogToolResultConstants.Reject
'                MessageBox.Show("Toolblock reject")
'        End Select

'    End Sub

'    Private Function LoadImageFormFile(strFileName As String) As CogImage8Grey
'        Dim img As CogImageFileBMP = New CogImageFileBMP()
'        img.Open(strFileName, CogImageFileModeConstants.Read)
'        Return img(0)
'    End Function

'    Public Function GetCameras() As Integer
'        _cameras = New CogFrameGrabbers()
'        For index = 0 To _cameras.Count - 1
'            _cameraList.Add(New Object)
'            _cameraList(index) = _cameras(index)

'            _fifoList.Add(New Object)
'            _fifoList(index) = CType(_cameraList(index), ICogFrameGrabber).CreateAcqFifo(VIDEO_FORMAT, CogAcqFifoPixelFormatConstants.Format8Grey, 0, True)
'        Next

'        Return _cameras.Count
'    End Function

'    Public Function CaptureOneImage(camera_i As Integer) As Bitmap
'        Dim img As ICogImage = Nothing
'        If _fifoList.Count <> 0 Then
'            img = CType(_fifoList(camera_i), ICogAcqFifo).Acquire(trigNum)
'            CogDisplay1.Image = img
'        End If
'        Return img.ToBitmap()
'    End Function

'    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
'        If CheckBox1.Checked Then
'            ' Show toolblock sequence
'            CogToolBlockEditV21.Subject = currentToolblock
'        Else
'            CogToolBlockEditV21.Subject = Nothing
'        End If
'    End Sub
'End Class
