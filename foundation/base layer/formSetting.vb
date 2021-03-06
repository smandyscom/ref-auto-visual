﻿Imports System.IO
Imports System.Xml.Serialization
'Imports System.Net.NetworkInformation
'Imports System.Net
'Imports System.Threading
Imports System.Collections.Generic
'Imports System.Web.Mvc
Imports Automation.utilities
Imports Automation.mainIOHardware

Public Class formSetting

    Property PointFilename As String = My.Application.Info.DirectoryPath & "\Data\" + "MotionPosData.xml"      'hsien ,  2015.01.30 , offer the ability to redirect target file

    Dim amaxDevice As amaxCard '= CType(physicalHardwareList.Find(Function(__hardware As hardwareNode) (__hardware.PhysicalHardware.hardwareCode And hardwareCodeEnum.AMAX_1202_CARD)).PhysicalHardware, amaxCardContext)
    Dim errorAxisIndex As Integer = 0
    Dim errorMessage As String = ""

    Structure AMONetMotion
        '------------------------------------
        ' All unit in mm
        'remarked by Hsien , 2014/5/28
        '------------------------------------
        Public ipAxis As Short 'eMotor 'remarked by Hsien , 2014/5/28

        Public MotionStatus As Integer
        Public speed As Double

        Public velDistanceMM As Double
        Public velStartVelMM As Double
        Public velMaxVelMM As Double
        Public velAccMM As Double
        Public velDecMM As Double
        Public velSAccMM As Double
        Public velDAccMM As Double

        Public posCommand As Long
        Public posFeedback As Long
        Public posErrorCounter As Long

    End Structure

    Structure AMONetMotionPulse
        '------------------------------------
        ' All unit in pulse
        'remarked by Hsien , 2014/5/28
        '------------------------------------
        'Public recStepMM2Pulse() As Double
        'Public recStepHomingPulse(,) As Double
        'Public recStepPositionPulse(,,) As Double

        Public velDistancePulse As Double
        Public velStartVelPulse As Double
        Public velMaxVelPulse As Double
        Public velAccPulse As Double
        Public velDecPulse As Double
        Public velSAccPulse As Double
        Public velSDecPulse As Double
    End Structure

    'Dim txt As Control
    Const ScreenBitShow As Short = 64          '* Bits per Page
    Const OutPort As Boolean = True
    Const InPort As Boolean = False
    Dim OutBitPage As Byte = 0              ' current bit page
    Dim InBitPage As Byte = 0               ' current bit page

    Dim MStatus As Integer = 0
    'Public velProfile(0 To 8) As Double
    Dim velProfile() As Double = New Double(8) {}
    Dim MStep_Type As Integer = 0
    Dim MServo_Type As Integer = 0

    Dim SlowVelocity_f As Integer = 0
    Dim MStep_btnGoHome_f As Boolean = 0
    Dim MStep_btnGoToPosition_f As Boolean = 0

    Dim selectedPoint As Short '取得選中的馬達位置在MPoint中的Index值

    Dim ImageAnalysisPath As String = ""

    Dim __AMONetMotion As AMONetMotion = New AMONetMotion()               'used by frmSetting ' by Hsien , 2014/6/4
    Dim __AMONetMotionPulse As AMONetMotionPulse = New AMONetMotionPulse()          'used by frmSetting , by Hsien , 2014/6/4

#Region "find use variable"
    Private targetAxisIndex As Short
    Private targetPointName As String
#End Region

    Private Sub frmMotionTestImageList()
        'Dim i As Integer
        'picEnvironment.Image = imlDeltaLogo.Images(1)
        btnExit.Image = imlfrmMotionTest.Images.Item("Exit.ICO")
        '=== IO Control ===




        '=== Motion Control ===
        btnMotion0.Image = imlfrmMotionTest.Images.Item("SMinus.ICO")
        btnMotion1.Image = imlfrmMotionTest.Images.Item("SPlus.ICO")
        btnMotion2.Image = imlfrmMotionTest.Images.Item("Stop.ICO")
    End Sub
    'Private Sub Input_State_Detect()
    '    'UserControlIOTable1.Input_State_Detect()
    'End Sub
    'Private Sub Output_State_Detect()
    '    'UserControlIOTable1.Output_State_Detect()
    'End Sub

    Private Sub lblRing0_DeviceIP1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim i As Integer = sender.Tag
        Me.Controls("lblRing0_DeviceIP" & i).Text = "Kelly" & CStr(i)
    End Sub

    Private Sub lblRing_DeviceIP_NameSet()
        Dim i As Integer = 0

        btnStartRing0.Tag = 0 : btnStartRing1.Tag = 1

        For i = 0 To 63
            ' Ring 0 : Device IP
            Me.gboxAMONet0.Controls("lblRing0_DeviceIP" & i).Text = i
            Me.gboxAMONet0.Controls("lblRing0_DeviceIP" & i).Tag = i
            Me.gboxAMONet0.Controls("lblRing0_DeviceIP" & i).BackColor = Color.Silver
            Me.gboxAMONet0.Controls("lblRing0_DeviceIP" & i).ForeColor = Color.DarkBlue
            ' Ring 1 : Device IP
            Me.gboxAMONet1.Controls("lblRing1_DeviceIP" & i).Text = i
            Me.gboxAMONet1.Controls("lblRing1_DeviceIP" & i).Tag = i
            Me.gboxAMONet1.Controls("lblRing1_DeviceIP" & i).BackColor = Color.Silver
            Me.gboxAMONet1.Controls("lblRing1_DeviceIP" & i).ForeColor = Color.DarkBlue
        Next
        '=== Motion ===
        For i = 0 To 3
            Me.gboxVelocityProfile.Controls("txtvelocityprofile" & i).Tag = i
            Me.gboxMotion.Controls("btnMotion" & i).Tag = i
        Next i
        For i = 0 To 2
            Me.gboxPosition.Controls("lblPosition" & i).Tag = i
        Next
        For i = 0 To 15
            Me.gboxStatus.Controls("shpStatus" & i).Tag = i
            Me.gboxStatus.Controls("lblStatus" & i).Tag = i
        Next i
    End Sub

    Private Sub lblRing0_DeviceIP0_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblRing0_DeviceIP0.MouseDown, lblRing0_DeviceIP1.MouseDown, lblRing0_DeviceIP2.MouseDown, lblRing0_DeviceIP3.MouseDown, lblRing0_DeviceIP4.MouseDown, lblRing0_DeviceIP5.MouseDown, lblRing0_DeviceIP6.MouseDown, lblRing0_DeviceIP7.MouseDown, _
                                                lblRing0_DeviceIP8.MouseDown, lblRing0_DeviceIP9.MouseDown, lblRing0_DeviceIP10.MouseDown, lblRing0_DeviceIP11.MouseDown, lblRing0_DeviceIP12.MouseDown, lblRing0_DeviceIP13.MouseDown, lblRing0_DeviceIP14.MouseDown, lblRing0_DeviceIP15.MouseDown, _
                                                lblRing0_DeviceIP16.MouseDown, lblRing0_DeviceIP17.MouseDown, lblRing0_DeviceIP18.MouseDown, lblRing0_DeviceIP19.MouseDown, lblRing0_DeviceIP20.MouseDown, lblRing0_DeviceIP21.MouseDown, lblRing0_DeviceIP22.MouseDown, lblRing0_DeviceIP23.MouseDown, _
                                                lblRing0_DeviceIP24.MouseDown, lblRing0_DeviceIP25.MouseDown, lblRing0_DeviceIP26.MouseDown, lblRing0_DeviceIP27.MouseDown, lblRing0_DeviceIP28.MouseDown, lblRing0_DeviceIP29.MouseDown, lblRing0_DeviceIP30.MouseDown, lblRing0_DeviceIP31.MouseDown, _
                                                lblRing0_DeviceIP32.MouseDown, lblRing0_DeviceIP33.MouseDown, lblRing0_DeviceIP34.MouseDown, lblRing0_DeviceIP35.MouseDown, lblRing0_DeviceIP36.MouseDown, lblRing0_DeviceIP37.MouseDown, lblRing0_DeviceIP38.MouseDown, lblRing0_DeviceIP39.MouseDown, _
                                                lblRing0_DeviceIP40.MouseDown, lblRing0_DeviceIP41.MouseDown, lblRing0_DeviceIP42.MouseDown, lblRing0_DeviceIP43.MouseDown, lblRing0_DeviceIP44.MouseDown, lblRing0_DeviceIP45.MouseDown, lblRing0_DeviceIP46.MouseDown, lblRing0_DeviceIP47.MouseDown, _
                                                lblRing0_DeviceIP48.MouseDown, lblRing0_DeviceIP49.MouseDown, lblRing0_DeviceIP50.MouseDown, lblRing0_DeviceIP51.MouseDown, lblRing0_DeviceIP52.MouseDown, lblRing0_DeviceIP53.MouseDown, lblRing0_DeviceIP54.MouseDown, lblRing0_DeviceIP55.MouseDown, _
                                                lblRing0_DeviceIP56.MouseDown, lblRing0_DeviceIP57.MouseDown, lblRing0_DeviceIP58.MouseDown, lblRing0_DeviceIP59.MouseDown, lblRing0_DeviceIP60.MouseDown, lblRing0_DeviceIP61.MouseDown, lblRing0_DeviceIP62.MouseDown, lblRing0_DeviceIP63.MouseDown
        '===== program =====
        Dim i As Short = sender.tag
        Me.gboxAMONet0.Controls("lblRing0_DeviceIP" & i).BackColor = Color.Red
        lblAMONetRing0Type.Text = ""
        lblAMONetRing0SubType.Text = ""

    End Sub

    Private Sub lblRing0_DeviceIP0_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblRing0_DeviceIP0.MouseUp, lblRing0_DeviceIP1.MouseUp, lblRing0_DeviceIP2.MouseUp, lblRing0_DeviceIP3.MouseUp, lblRing0_DeviceIP4.MouseUp, lblRing0_DeviceIP5.MouseUp, lblRing0_DeviceIP6.MouseUp, lblRing0_DeviceIP7.MouseUp, _
                                            lblRing0_DeviceIP8.MouseUp, lblRing0_DeviceIP9.MouseUp, lblRing0_DeviceIP10.MouseUp, lblRing0_DeviceIP11.MouseUp, lblRing0_DeviceIP12.MouseUp, lblRing0_DeviceIP13.MouseUp, lblRing0_DeviceIP14.MouseUp, lblRing0_DeviceIP15.MouseUp, _
                                            lblRing0_DeviceIP16.MouseUp, lblRing0_DeviceIP17.MouseUp, lblRing0_DeviceIP18.MouseUp, lblRing0_DeviceIP19.MouseUp, lblRing0_DeviceIP20.MouseUp, lblRing0_DeviceIP21.MouseUp, lblRing0_DeviceIP22.MouseUp, lblRing0_DeviceIP23.MouseUp, _
                                            lblRing0_DeviceIP24.MouseUp, lblRing0_DeviceIP25.MouseUp, lblRing0_DeviceIP26.MouseUp, lblRing0_DeviceIP27.MouseUp, lblRing0_DeviceIP28.MouseUp, lblRing0_DeviceIP29.MouseUp, lblRing0_DeviceIP30.MouseUp, lblRing0_DeviceIP31.MouseUp, _
                                            lblRing0_DeviceIP32.MouseUp, lblRing0_DeviceIP33.MouseUp, lblRing0_DeviceIP34.MouseUp, lblRing0_DeviceIP35.MouseUp, lblRing0_DeviceIP36.MouseUp, lblRing0_DeviceIP37.MouseUp, lblRing0_DeviceIP38.MouseUp, lblRing0_DeviceIP39.MouseUp, _
                                            lblRing0_DeviceIP40.MouseUp, lblRing0_DeviceIP41.MouseUp, lblRing0_DeviceIP42.MouseUp, lblRing0_DeviceIP43.MouseUp, lblRing0_DeviceIP44.MouseUp, lblRing0_DeviceIP45.MouseUp, lblRing0_DeviceIP46.MouseUp, lblRing0_DeviceIP47.MouseUp, _
                                            lblRing0_DeviceIP48.MouseUp, lblRing0_DeviceIP49.MouseUp, lblRing0_DeviceIP50.MouseUp, lblRing0_DeviceIP51.MouseUp, lblRing0_DeviceIP52.MouseUp, lblRing0_DeviceIP53.MouseUp, lblRing0_DeviceIP54.MouseUp, lblRing0_DeviceIP55.MouseUp, _
                                            lblRing0_DeviceIP56.MouseUp, lblRing0_DeviceIP57.MouseUp, lblRing0_DeviceIP58.MouseUp, lblRing0_DeviceIP59.MouseUp, lblRing0_DeviceIP60.MouseUp, lblRing0_DeviceIP61.MouseUp, lblRing0_DeviceIP62.MouseUp, lblRing0_DeviceIP63.MouseUp
        '===== program =====
        With amaxDevice.RingInfos(0).ModuleInfos(sender.Tag)

            If (.IsMotionDevice) Then
                lblAMONetRing0Type.Text = "Motion Device"
            Else
                lblAMONetRing0Type.Text = "I/O Device"
            End If
            lblAMONetRing0SubType.Text = .RealCode.ToString

        End With
    End Sub

    Private Sub lblRing1_DeviceIP0_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblRing1_DeviceIP0.MouseDown, lblRing1_DeviceIP1.MouseDown, lblRing1_DeviceIP2.MouseDown, lblRing1_DeviceIP3.MouseDown, lblRing1_DeviceIP4.MouseDown, lblRing1_DeviceIP5.MouseDown, lblRing1_DeviceIP6.MouseDown, lblRing1_DeviceIP7.MouseDown, _
                                                lblRing1_DeviceIP8.MouseDown, lblRing1_DeviceIP9.MouseDown, lblRing1_DeviceIP10.MouseDown, lblRing1_DeviceIP11.MouseDown, lblRing1_DeviceIP12.MouseDown, lblRing1_DeviceIP13.MouseDown, lblRing1_DeviceIP14.MouseDown, lblRing1_DeviceIP15.MouseDown, _
                                                lblRing1_DeviceIP16.MouseDown, lblRing1_DeviceIP17.MouseDown, lblRing1_DeviceIP18.MouseDown, lblRing1_DeviceIP19.MouseDown, lblRing1_DeviceIP20.MouseDown, lblRing1_DeviceIP21.MouseDown, lblRing1_DeviceIP22.MouseDown, lblRing1_DeviceIP23.MouseDown, _
                                                lblRing1_DeviceIP24.MouseDown, lblRing1_DeviceIP25.MouseDown, lblRing1_DeviceIP26.MouseDown, lblRing1_DeviceIP27.MouseDown, lblRing1_DeviceIP28.MouseDown, lblRing1_DeviceIP29.MouseDown, lblRing1_DeviceIP30.MouseDown, lblRing1_DeviceIP31.MouseDown, _
                                                lblRing1_DeviceIP32.MouseDown, lblRing1_DeviceIP33.MouseDown, lblRing1_DeviceIP34.MouseDown, lblRing1_DeviceIP35.MouseDown, lblRing1_DeviceIP36.MouseDown, lblRing1_DeviceIP37.MouseDown, lblRing1_DeviceIP38.MouseDown, lblRing1_DeviceIP39.MouseDown, _
                                                lblRing1_DeviceIP40.MouseDown, lblRing1_DeviceIP41.MouseDown, lblRing1_DeviceIP42.MouseDown, lblRing1_DeviceIP43.MouseDown, lblRing1_DeviceIP44.MouseDown, lblRing1_DeviceIP45.MouseDown, lblRing1_DeviceIP46.MouseDown, lblRing1_DeviceIP47.MouseDown, _
                                                lblRing1_DeviceIP48.MouseDown, lblRing1_DeviceIP49.MouseDown, lblRing1_DeviceIP50.MouseDown, lblRing1_DeviceIP51.MouseDown, lblRing1_DeviceIP52.MouseDown, lblRing1_DeviceIP53.MouseDown, lblRing1_DeviceIP54.MouseDown, lblRing1_DeviceIP55.MouseDown, _
                                                lblRing1_DeviceIP56.MouseDown, lblRing1_DeviceIP57.MouseDown, lblRing1_DeviceIP58.MouseDown, lblRing1_DeviceIP59.MouseDown, lblRing1_DeviceIP60.MouseDown, lblRing1_DeviceIP61.MouseDown, lblRing1_DeviceIP62.MouseDown, lblRing1_DeviceIP63.MouseDown
        Dim i As Short = sender.Tag
        Me.gboxAMONet1.Controls("lblRing1_DeviceIP" & i).BackColor = Color.Red
        lblAMONetRing1Type.Text = ""
        lblAMONetRing1SubType.Text = ""

    End Sub
    Private Sub lblRing1_DeviceIP0_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblRing1_DeviceIP0.MouseUp, lblRing1_DeviceIP1.MouseUp, lblRing1_DeviceIP2.MouseUp, lblRing1_DeviceIP3.MouseUp, lblRing1_DeviceIP4.MouseUp, lblRing1_DeviceIP5.MouseUp, lblRing1_DeviceIP6.MouseUp, lblRing1_DeviceIP7.MouseUp, _
                                                lblRing1_DeviceIP8.MouseUp, lblRing1_DeviceIP9.MouseUp, lblRing1_DeviceIP10.MouseUp, lblRing1_DeviceIP11.MouseUp, lblRing1_DeviceIP12.MouseUp, lblRing1_DeviceIP13.MouseUp, lblRing1_DeviceIP14.MouseUp, lblRing1_DeviceIP15.MouseUp, _
                                                lblRing1_DeviceIP16.MouseUp, lblRing1_DeviceIP17.MouseUp, lblRing1_DeviceIP18.MouseUp, lblRing1_DeviceIP19.MouseUp, lblRing1_DeviceIP20.MouseUp, lblRing1_DeviceIP21.MouseUp, lblRing1_DeviceIP22.MouseUp, lblRing1_DeviceIP23.MouseUp, _
                                                lblRing1_DeviceIP24.MouseUp, lblRing1_DeviceIP25.MouseUp, lblRing1_DeviceIP26.MouseUp, lblRing1_DeviceIP27.MouseUp, lblRing1_DeviceIP28.MouseUp, lblRing1_DeviceIP29.MouseUp, lblRing1_DeviceIP30.MouseUp, lblRing1_DeviceIP31.MouseUp, _
                                                lblRing1_DeviceIP32.MouseUp, lblRing1_DeviceIP33.MouseUp, lblRing1_DeviceIP34.MouseUp, lblRing1_DeviceIP35.MouseUp, lblRing1_DeviceIP36.MouseUp, lblRing1_DeviceIP37.MouseUp, lblRing1_DeviceIP38.MouseUp, lblRing1_DeviceIP39.MouseUp, _
                                                lblRing1_DeviceIP40.MouseUp, lblRing1_DeviceIP41.MouseUp, lblRing1_DeviceIP42.MouseUp, lblRing1_DeviceIP43.MouseUp, lblRing1_DeviceIP44.MouseUp, lblRing1_DeviceIP45.MouseUp, lblRing1_DeviceIP46.MouseUp, lblRing1_DeviceIP47.MouseUp, _
                                                lblRing1_DeviceIP48.MouseUp, lblRing1_DeviceIP49.MouseUp, lblRing1_DeviceIP50.MouseUp, lblRing1_DeviceIP51.MouseUp, lblRing1_DeviceIP52.MouseUp, lblRing1_DeviceIP53.MouseUp, lblRing1_DeviceIP54.MouseUp, lblRing1_DeviceIP55.MouseUp, _
                                                lblRing1_DeviceIP56.MouseUp, lblRing1_DeviceIP57.MouseUp, lblRing1_DeviceIP58.MouseUp, lblRing1_DeviceIP59.MouseUp, lblRing1_DeviceIP60.MouseUp, lblRing1_DeviceIP61.MouseUp, lblRing1_DeviceIP62.MouseUp, lblRing1_DeviceIP63.MouseUp
        '===== program =====
        With amaxDevice.RingInfos(1).ModuleInfos(sender.Tag)

            If (.IsMotionDevice) Then
                lblAMONetRing0Type.Text = "Motion Device"
            Else
                lblAMONetRing0Type.Text = "I/O Device"
            End If
            lblAMONetRing0SubType.Text = .RealCode.ToString

        End With
    End Sub

    Private Sub AMax_LED_ActiveTable()
        '----------------------------------------------------
        'Hsien , due to HAL , re-write this section
        '----------------------------------------------------
        'Lime : Correct Device
        'Magenta : Incorrrect Device but connected
        'Silver : Incorrect Device 
        Dim amaxDevice As amaxCard = CType(Instance.PhysicalHardwareList.Find(Function(__hardware As subHardwareNode) (__hardware.PhysicalHardware.HardwareCode And hardwareCodeEnum.AMAX_1202_CARD)).PhysicalHardware, amaxCard)
        For ringIndex = 0 To amaxDevice.RingInfos.Count - 1
            With amaxDevice.RingInfos(ringIndex)
                'for each ring...
                Dim __groupBox As GroupBox = Me.tabRemoteTable.Controls(String.Format("gboxAMONet{0}", ringIndex))

                'todo , find no groupBox

                For deviceIndex = 0 To .ModuleInfos.Count - 1
                    'for each device
                    Dim __control As Control = __groupBox.Controls(String.Format("lblRing{0}_DeviceIP{1}",
                                                                                 ringIndex,
                                                                                 deviceIndex))

                    With .ModuleInfos(deviceIndex)

                        If (.IsTypeMatched And .IsPhysicalConnected) Then
                            'matched and connected
                            __control.BackColor = Color.Lime
                        ElseIf (.IsLossed) Then
                            __control.BackColor = Color.Magenta
                        Else
                            'dont care
                            __control.BackColor = Color.Silver
                        End If

                    End With

                Next

            End With
        Next


    End Sub
    Private Sub VelProfileSet()
        Dim strErrStatus As String
        With __AMONetMotion
            .velDistanceMM = Val(txtVelocityProfile0.Text)
            .velStartVelMM = Val(txtVelocityProfile1.Text)
            .velMaxVelMM = Val(txtVelocityProfile2.Text)
            .velAccMM = Val(txtVelocityProfile3.Text)
            .velDecMM = .velAccMM

            .velSAccMM = 0
            .velDAccMM = 0
        End With

        With __AMONetMotionPulse
            .velDistancePulse = __AMONetMotion.velDistanceMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velStartVelPulse = __AMONetMotion.velStartVelMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velMaxVelPulse = __AMONetMotion.velMaxVelMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velAccPulse = __AMONetMotion.velAccMM
            .velDecPulse = .velAccPulse
            .velSAccPulse = 0
            .velSDecPulse = 0
            '=== Set an S-curve Velocity profile move ===
            Dim RingNo As Integer, DeviceIP As Integer, PortNo As Integer

            Call AMax_Get_Moton_DeviceIP(__AMONetMotion.ipAxis, RingNo, DeviceIP, PortNo)  '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            'If amaxConfiguration.RingEnabled_f(RingNo) = True And amaxConfiguration.MotionEnabled_f(RingNo) = True Then
            If amaxDevice.RingInfos(RingNo).CommunicationStatus = communicationStatus.CONNECTED Then
                MStatus = AMaxM4_SMovSet(__AMONetMotion.ipAxis, .velStartVelPulse, .velMaxVelPulse, .velAccPulse, .velDecPulse, .velSAccPulse, .velSDecPulse)
                If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
            End If
        End With
    End Sub

    Private Sub txtVelocityProfile0_TextChanged(ByVal sender As TextBox, ByVal e As System.EventArgs) Handles txtVelocityProfile0.TextChanged, txtVelocityProfile1.TextChanged, txtVelocityProfile2.TextChanged, txtVelocityProfile3.TextChanged

        Try
            Dim intLen As Integer = 0


            'hsien , do not use tag to recognize instance anymore , 2015.03.26
            'Dim i As Integer, intLen As Integer
            'i = sender.Tag        'Get the Index
            'If IsNumeric(Me.gboxVelocityProfile.Controls("txtvelocityprofile" & i).Text) = False Then
            '    intLen = Len(Me.gboxVelocityProfile.Controls("txtvelocityprofile" & i).Text)
            '    If intLen > 0 Then
            '        Me.gboxVelocityProfile.Controls("txtvelocityprofile" & i).Text =
            '            Microsoft.VisualBasic.Strings.Left(Me.gboxVelocityProfile.Controls("txtvelocityprofile" & i).Text, intLen - 1)
            '    End If
            'End If

            'solved , hsien , 2015.03.26
            If IsNumeric(sender.Text) = False Then
                intLen = Len(sender.Text)
                If intLen > 0 Then
                    sender.Text =
                        Microsoft.VisualBasic.Strings.Left(sender.Text, intLen - 1)
                End If
            End If


        Catch ex As Exception
            'going to solve exception here , hsien 2015.03.28
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub btnMotion0_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles btnMotion0.MouseDown, btnMotion1.MouseDown, btnMotion2.MouseDown, btnMotion3.MouseDown, btnMotion4.MouseDown
        'Dim OnOff As Integer, 
        Dim strErrStatus As String
        Dim index As Integer

        index = sender.tag

        Call VelProfileSet()
        With __AMONetMotionPulse
            Select Case sender.name
                Case "btnMotion0" '往負方向移動
                    If rdoMoveMode0.Checked = True Then '相對移動
                        MStatus = AMaxM4_RMov(__AMONetMotion.ipAxis, -.velDistancePulse)
                        If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    End If
                    If rdoMoveMode1.Checked = True Then '連續移動
                        MStatus = AMaxM4_VMov(__AMONetMotion.ipAxis, eDir.Neg)
                        If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    End If
                Case "btnMotion1" '往正方向移動
                    If rdoMoveMode0.Checked = True Then '相對移動
                        MStatus = AMaxM4_RMov(__AMONetMotion.ipAxis, .velDistancePulse)
                        If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    End If
                    If rdoMoveMode1.Checked = True Then '連續移動
                        MStatus = AMaxM4_VMov(__AMONetMotion.ipAxis, eDir.Pos)
                        If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    End If
                Case "btnMotion2" '緊急停止
                    MStep_btnGoHome_f = False : MStep_btnGoToPosition_f = False
                    cboMStepPart.Enabled = True : cboMStepName.Enabled = True
                    MStatus = AMaxM4_EmgStop(__AMONetMotion.ipAxis)
                    If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                Case "btnMotion3" '伺服致能/去能
                    'MStatus = AMaxM4_ioServoOn(__AMONetMotion.ipAxis, OnOff)
                    'If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    If Me.readMotorStatus(__AMONetMotion.ipAxis, amaxMotionDIO_State.amax_IO_oSVON) Then
                        MStatus = AMaxM4_ServoOn(__AMONetMotion.ipAxis, 0)
                        SettingLogSave(String.Format("ServoOFF {0} Motor", cboMStepName.SelectedItem))
                    Else
                        MStatus = AMaxM4_ServoOn(__AMONetMotion.ipAxis, 1)
                        SettingLogSave(String.Format("ServoON {0} Motor", cboMStepName.SelectedItem))
                    End If
                    If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                Case "btnMotion4" '重置錯誤
                    btnMotion4.Enabled = False
                    'MStatus = AMaxM4_ioResetALM(__AMONetMotion.ipAxis, OnOff)
                    'If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    If Me.readMotorStatus(__AMONetMotion.ipAxis, amaxMotionDIO_State.amax_IO_oRALM) Then
                        MStatus = AMaxM4_ResetALM(__AMONetMotion.ipAxis, 0)
                    Else
                        MStatus = AMaxM4_ResetALM(__AMONetMotion.ipAxis, 1)
                    End If
                    If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
                    btnMotion4.Enabled = True
            End Select
        End With
    End Sub

    Private Sub btnMotion0_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles btnMotion0.MouseUp, btnMotion1.MouseUp
        Dim strErrStatus As String
        If rdoMoveMode1.Checked Then
            MStatus = AMaxM4_Soft_Emg_Stop(__AMONetMotion.ipAxis)
            If MStatus < 0 Then strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus) : Call boxErrAmaxMotion(strErrStatus) : Exit Sub
        End If
    End Sub
    Private Sub btnPositionReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPositionReset.Click
        Dim strErrStatus As String
        With __AMONetMotion

            MStatus = AMaxM4_CmdPos_Reset(.ipAxis)
            If MStatus < 0 Then
                strErrStatus = strErrAmaxMotion(.ipAxis, MStatus)
                Call boxErrAmaxMotion(strErrStatus)
                Exit Sub
            End If

            MStatus = AMaxM4_Get_Command(.ipAxis, .posCommand)
            AMaxM4_Get_Position(.ipAxis, .posFeedback)
            AMaxM4_Get_Error_Counter(.ipAxis, .posErrorCounter)

            If MStatus < 0 Then
                strErrStatus = strErrAmaxMotion(.ipAxis, MStatus)
                Call boxErrAmaxMotion(strErrStatus)
                Exit Sub
            End If

        End With
    End Sub
    

    Private Sub btnRecord0_Click(ByVal sender As Button, ByVal e As System.EventArgs) Handles btnMoveToMotionPosition.Click,
        btnGetVelocityProfile.Click,
        btnRecord2.Click

        Select Case sender.Name
            Case btnMoveToMotionPosition.Name ' Move
                Try
                    'Hsien , 2015.06.03 , had to match  motorAxis and PointName
                    targetAxisIndex = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)
                    targetPointName = msgStepRecord.CurrentRow.HeaderCell.Value 'Hsien , 2015.06.24
                    selectedPoint = pData.MotorPoints.FindIndex(AddressOf findPointNameAndAxisIndex)

                    'If Strings.Right(pData.MotorPoints.Item(i).sName, 3) = "回原點" Then 'home
                    If pData.MotorPoints(selectedPoint).IsHomePoint Then 'remarked by Hsien , 2014/5/30
                        MStep_btnGoHome_f = True
                    Else 'move point
                        ' set-up the flag xxx_f ,  then back ground timer would invocate moving procedure
                        MStep_btnGoToPosition_f = True
                    End If
                    'Disable the Go-Command & cbo box selection
                    cboMStepPart.Enabled = False : cboMStepName.Enabled = False
                    SettingLogSave(String.Format("Go to {0} MotorPoint", targetPointName))
                Catch ex As Exception
                    MessageBox.Show(ex.Message) 'Hsien , 2015.06.22
                End Try


            Case btnGetVelocityProfile.Name
                '--------------------------------------------------------------------
                ' Get Velocity-Profile :  Copy the Velocity-Profile to msgStepRecord
                '--------------------------------------------------------------------
                Application.DoEvents()
                With msgStepRecord
                    For i = 0 To 2 'Only velocity-parameters
                        msgStepRecord.CurrentRow.Cells(i + 2).Style.ForeColor = Color.Black   ' Set
                        msgStepRecord.CurrentRow.Cells(i + 2).Value = Me.gboxVelocityProfile.Controls("txtVelocityProfile" & i).Text
                    Next i

                End With
                Call GetMStepVelocityProfileFromGrid()

            Case btnRecord2.Name 'Save
                Call msgMStepRecordSave()
                Call SaveXmlFile(pData, PointFilename)   'Hsien , 2015.01.30 , redircting
                Call msgMStepRecordGet(__AMONetMotion.ipAxis)
        End Select

    End Sub

    Private Sub msgStepRecord_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles msgStepRecord.CellDoubleClick
        '-----------------------------------------
        '   Use double click to prevent mis-operate for point type switch
        ' Hsien , 2015.06.23
        '-----------------------------------------
        btnMoveToMotionPosition.Enabled = False
        btnGetVelocityProfile.Enabled = False
        If e.ColumnIndex < 0 Or e.RowIndex < 0 Then Exit Sub
        With sender(e.ColumnIndex, e.RowIndex)
            If .GetType.ToString = "System.Windows.Forms.DataGridViewButtonCell" Then
                If .Value = "+" Then : .Value = "-"
                ElseIf .Value = "-" Then : .Value = "+"
                ElseIf .Value = "Abs" Then : .Value = "Rel"
                ElseIf .Value = "Rel" Then : .Value = "Abs"
                End If
                .Style.ForeColor = Color.Red
            End If
        End With
    End Sub


    Private Sub NumericValuesOnly(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Select Case Asc(e.KeyChar)
            Case AscW(ControlChars.Cr) 'Enter key
                e.Handled = True '將 Handled 設為 true 可以取消 KeyPress 事件
            Case AscW(ControlChars.Back) 'Backspace
            Case 45, 46, 48 To 57 'Negative sign, Decimal and Numbers
            Case Else ' Everything else
                e.Handled = True
        End Select
    End Sub

    Private Sub msgStepRecord_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles msgStepRecord.EditingControlShowing
        ' Add the new event to the current text control in the grid
        AddHandler e.Control.KeyPress, AddressOf NumericValuesOnly
    End Sub
    Private Sub msgStepRecord_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles msgStepRecord.LostFocus
        msgStepRecord.ClearSelection()
    End Sub

    Private Sub msgStepRecord_RowHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles msgStepRecord.RowHeaderMouseClick
        msgStepRecord.Tag = e.RowIndex
        btnMoveToMotionPosition.Enabled = True
        btnGetVelocityProfile.Enabled = True
    End Sub
    Private Sub msgStepRecord_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles msgStepRecord.CellBeginEdit
        msgStepRecord(e.ColumnIndex, e.RowIndex).Tag = msgStepRecord(e.ColumnIndex, e.RowIndex).Value
    End Sub
    Private Sub msgStepRecord_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles msgStepRecord.CellEndEdit
        If msgStepRecord(e.ColumnIndex, e.RowIndex).Value <> msgStepRecord(e.ColumnIndex, e.RowIndex).Tag Then
            msgStepRecord(e.ColumnIndex, e.RowIndex).Style.ForeColor = Color.Red
        End If
    End Sub

    Private Function CheckMotorStatusTextSet() As Boolean
        'Dim OnOff As Long
        'Dim strErrStatus As String

        'MStatus = AMaxM4_ioServoOn(__AMONetMotion.ipAxis, OnOff)

        'If MStatus < 0 Then
        '    strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus)
        '    'Call boxErrAmaxMotion(strErrStatus) ': Exit Sub
        '    Return False    'Hsien , 2015.01.20
        'End If

        'Direct reflect real I/O status , Hsien , 2015.07.16
        Select Case Application.CurrentCulture.ToString
            Case "en-US"
                'Hsien , 2015.07.16
                If Me.readMotorStatus(__AMONetMotion.ipAxis, amaxMotionDIO_State.amax_IO_oSVON) Then
                    btnMotion3.Text = "Servo On"
                    btnMotion3.BackColor = Color.LightGreen
                Else
                    btnMotion3.Text = "Servo Off"
                    btnMotion3.BackColor = SystemColors.Control
                End If
            Case Else
                If Me.readMotorStatus(__AMONetMotion.ipAxis, amaxMotionDIO_State.amax_IO_oSVON) Then
                    btnMotion3.Text = "伺服致能中"
                    btnMotion3.BackColor = Color.LightGreen
                Else
                    btnMotion3.Text = "伺服去能"
                    btnMotion3.BackColor = SystemColors.Control
                End If
        End Select

        'no need to judge default servo on level , Hsien , 2015.07.16
        'If pData.MotorSettings(__AMONetMotion.ipAxis).ServoOnLevel = eActive.ActHi Then        'Servo on/off

        '    If OnOff = 1 Then
        '        btnMotion3.Text = "伺服致能中"
        '        btnMotion3.BackColor = Color.LightGreen
        '    Else
        '        btnMotion3.Text = "伺服去能"
        '        btnMotion3.BackColor = SystemColors.Control
        '    End If

        'Else
        '    If OnOff = 0 Then
        '        btnMotion3.Text = "電流驅動中"
        '        btnMotion3.BackColor = Color.LightGreen
        '    Else
        '        btnMotion3.Text = "電流斷路"
        '        btnMotion3.BackColor = SystemColors.Control
        '    End If
        'End If

        Return True

    End Function
    Private Sub VelocityProfileSet()
        Dim strErrStatus As String

        With __AMONetMotion
            .velDistanceMM = Val(txtVelocityProfile0.Text)
            .velStartVelMM = Val(txtVelocityProfile1.Text)
            .velMaxVelMM = Val(txtVelocityProfile2.Text)
            .velAccMM = Val(txtVelocityProfile3.Text)
            .velSAccMM = 0
            .velDAccMM = .velSAccMM
        End With

        With __AMONetMotionPulse
            .velDistancePulse = __AMONetMotion.velDistanceMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velStartVelPulse = __AMONetMotion.velStartVelMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velMaxVelPulse = __AMONetMotion.velMaxVelMM * pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit
            .velAccPulse = __AMONetMotion.velAccMM
            .velDecPulse = .velAccPulse 'AMONetMot.velDecMM
            .velSAccPulse = 0
            .velSDecPulse = 0
            Dim RingNo As Integer, DeviceIP As Integer, PortNo As Integer
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(__AMONetMotion.ipAxis, RingNo, DeviceIP, PortNo)

            If amaxDevice.RingInfos(RingNo).CommunicationStatus = communicationStatus.CONNECTED Then
                MStatus = AMaxM4_SMovSet(__AMONetMotion.ipAxis, .velStartVelPulse, .velMaxVelPulse, .velAccPulse, .velDecPulse, .velSAccPulse, .velSDecPulse)
                If MStatus < 0 Then
                    'report error here , if hardware configure conflicted ,  remarked by Hsien , 2014/5/27
                    strErrStatus = strErrAmaxMotion(__AMONetMotion.ipAxis, MStatus)
                    Call boxErrAmaxMotion(strErrStatus)
                    Exit Sub
                End If
            End If
        End With

    End Sub

    Private Sub cboMStepPart_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboMStepPart.SelectedIndexChanged
        'Call MStepZone2Type(cboMStepPart.Text)     'remarked by Hsien , 2014/5/30
        cboMStepName.Items.Clear()
        For Each motorSetting As cMotorSetting In pData.MotorSettings.FindAll(AddressOf findAllPartWithCbo)
            cboMStepName.Items.Add(motorSetting.MotorName)
        Next
        cboMStepName.SelectedIndex = 0

        'AMONetMot.ipAxis = MStepType2ipAxis(cboMStepPart.SelectedIndex, cboMStepName.SelectedIndex)
        __AMONetMotion.ipAxis = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)

        Call msgStepRecordHeaderTextSetting(__AMONetMotion.ipAxis) ' Set Grid
        Call msgMStepRecordGet(__AMONetMotion.ipAxis)   ''Get Data from Saved File
        '=== Set the Jog-Velocity to Step-Motor ===
        Call VelocityProfileSet()        '===== Check Servo/Braker-Status =====
        Application.DoEvents()
        'If pData.MotorSettings(AMONetMot.ipAxis).DrvOnLevel = eActive.ActHi Then btnMotion3.Enabled = True Else btnMotion3.Enabled = False
    End Sub

    Private Sub cboMStepName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboMStepName.SelectedIndexChanged
        'AMONetMot.ipAxis = MStepType2ipAxis(cboMStepPart.SelectedIndex, cboMStepName.SelectedIndex)
        __AMONetMotion.ipAxis = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)

        Call msgStepRecordHeaderTextSetting(__AMONetMotion.ipAxis) ' Set Grid
        Call msgMStepRecordGet(__AMONetMotion.ipAxis)   ''Get Data from Saved File
        msgStepRecord.Tag = Nothing
        '=== Set the Jog-Velocity to Step-Motor ===
        Call VelocityProfileSet()
        Application.DoEvents()
        '===== Check Servo-Status =====
        'If pData.MotorSettings(AMONetMot.ipAxis).DrvOnLevel = eActive.ActHi Then btnMotion3.Enabled = True Else btnMotion3.Enabled = False
    End Sub

    'Private Sub btnStartRing0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartRing0.Click, btnStartRing1.Click
    '    Dim i As Short = sender.tag
    '    'Remote 裝置頁的Ring0按鈕
    '    If AMax_iniRing(0, amaxConfiguration.Baudrate) = True Then Call AMax_iniMotion(0)
    '    If AMax_iniRing(1, amaxConfiguration.Baudrate) = True Then Call AMax_iniMotion(1)

    '    Call AMax_LED_ActiveTable(i)
    '    Call Output_State_Detect()
    'End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        timerRefresh.Enabled = False
        timerRefresh.Interval = 1000
        ' Remarked by Hsien , 2014.05.26
        'frmMain.Enabled = True
        'blnInSettingPage = False
        SettingLogSave("Close FormSetting")
        Me.Close()
    End Sub
    Private Sub tmrMain_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerRefresh.Tick
        'Dim i As Short
        'Dim SenIndex As Short
        'Dim blnSenOk As Boolean
        SuspendLayout()

        userControlIOTable1.Refresh()
        AMax_LED_ActiveTable()

        Call ShowScan()
        Select Case tabGeneral.SelectedTab.Name
            'Case "tabIOTable"
            '    Call Input_State_Detect() '偵測輸入狀態
            Case "tabMotionTable" 'motor
                Call MotorStatusShow()
                '------------------------------------
                ' why go home when this tab selected?
                'remarked by Hsien , 2014/5/30
                '------------------------------------
                If MStep_btnGoHome_f = True Then
                    If MAmax_GoHome(pData.MotorPoints(selectedPoint).AxisIndex, 45) = True Then
                        MStep_btnGoHome_f = False
                        btnMoveToMotionPosition.Enabled = True : cboMStepPart.Enabled = True : cboMStepName.Enabled = True
                    End If
                End If
                'End If
                If MStep_btnGoToPosition_f = True Then
                    ' wait until go to position over
                    MStep_btnGoToPosition_f = Not MAmax_GoToPosition(pData.MotorPoints(selectedPoint), 30)

                    If MStep_btnGoToPosition_f = False Then
                        btnMoveToMotionPosition.Enabled = True : cboMStepPart.Enabled = True : cboMStepName.Enabled = True
                    End If

                End If
                ' Reset the Enable-Flag
                '               If alarm.msw = True And alarm.code < 0 And Amax.numErrAxis = AMONetMot.ipAxis Then
                If (alarm.type = alarmMutex.eErrType.ERROR_MOTOR) And
                    alarm.code < 0 And
                    errorAxisIndex = __AMONetMotion.ipAxis Then

                    MStep_btnGoHome_f = False
                    MStep_btnGoToPosition_f = False
                    alarm.type = alarmMutex.eErrType.ERROR_NONE

                    boxErrAmaxMotion(errorMessage) '顯示馬達錯誤

                    '--- reset alarm ----
                    alarm.sw = eSwitch.eOFF
                    btnMoveToMotionPosition.Enabled = True
                    alarm.code = 0 'reset alarm code
                    cboMStepPart.Enabled = True
                    cboMStepName.Enabled = True
                End If

                Call CheckMotorStatusTextSet()
        End Select

        ResumeLayout()
    End Sub
    Private Sub MotorStatusShow(Optional ByRef blnShowMsg As Boolean = False)

        'Dim DeviceIP, RingNo, PortNo As Short
        'Dim lngStatus As Integer
        Dim i As Short
        Dim iSensor As amaxMotionDIO_State
        Dim iMStatus As Short
        Dim posCommand As Integer
        Dim posFeedback As Integer
        Dim posErrorCounter As Integer
        Dim CurrentSpeed As Double
        Dim MotionStatus As Short
        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        'Call AMax_Get_Moton_DeviceIP(__AMONetMotion.ipAxis, RingNo, DeviceIP, PortNo)

        'iMStatus = B_mnet_m4_get_io_status(RingNo, DeviceIP, PortNo, lngStatus)

        With pData.MotorSettings(__AMONetMotion.ipAxis)
            Dim value As ULong = readWord(BitConverter.ToUInt64({0,
                                            0,
                                            .DeviceIp,
                                            .RingIndex,
                                            amaxModuleTypeEnum.REMOTE,
                                            0,
                                            0,
                                            hardwareCodeEnum.AMAX_1202_CARD}, 0))
            iMStatus = BitConverter.ToInt16(BitConverter.GetBytes(value), .AxisIndex * 2)
            'X: occupied byte0, byte1
            'Y: occupied byte2, byte3
            'so on ...


            For i = 0 To 15
                Select Case i
                    Case 0 : iSensor = amaxMotionDIO_State.amax_IO_iRDY
                    Case 1 : iSensor = amaxMotionDIO_State.amax_IO_iALM
                    Case 2 : iSensor = amaxMotionDIO_State.amax_IO_iP_EL
                    Case 3 : iSensor = amaxMotionDIO_State.amax_IO_iN_EL
                    Case 4 : iSensor = amaxMotionDIO_State.amax_IO_iORG
                    Case 5 : iSensor = amaxMotionDIO_State.amax_IO_oDIR
                    Case 6 : iSensor = amaxMotionDIO_State.amax_IO_iEMG
                    Case 7 : iSensor = amaxMotionDIO_State.amax_IO_iPCS
                    Case 8 : iSensor = amaxMotionDIO_State.amax_IO_oERC
                    Case 9 : iSensor = amaxMotionDIO_State.amax_IO_iEZ
                    Case 10 : iSensor = amaxMotionDIO_State.amax_IO_CLR
                    Case 11 : iSensor = amaxMotionDIO_State.amax_IO_iLatch
                    Case 12 : iSensor = amaxMotionDIO_State.amax_IO_iSD
                    Case 13 : iSensor = amaxMotionDIO_State.amax_IO_iINP
                    Case 14 : iSensor = amaxMotionDIO_State.amax_IO_oSVON
                    Case 15 : iSensor = amaxMotionDIO_State.amax_IO_oRALM
                End Select

                Dim result As Boolean = (iMStatus And iSensor)

                If result Then
                    Me.Controls.Find("shpStatus" & i.ToString, True)(0).BackColor = Color.Red
                Else
                    Me.Controls.Find("shpStatus" & i.ToString, True)(0).BackColor = SystemColors.Control
                End If

                If (iSensor = amaxMotionDIO_State.amax_IO_oSVON) Then
                    utilitiesUI.controlFollowBooleanColor(btnMotion3, result, Color.LimeGreen)
                End If

            Next
            '==== servo on show ===============
            'MStatus = AMaxM4_ioServoOn(__AMONetMotion.ipAxis, OnOff)
            'If OnOff = IS_ON Then
            '    btnMotion3.BackColor = Color.LimeGreen
            'Else
            '    btnMotion3.UseVisualStyleBackColor = True
            'End If
            ''======= position show ===========
            iMStatus = B_mnet_m4_get_command(.RingIndex, .DeviceIp, .AxisIndex, posCommand)
            If iMStatus < 0 Then
                If blnShowMsg = True Then
                    '------------------------------------
                    ' showed on the customed message box
                    'remarked by Hsien , 2014/5/26
                    '------------------------------------
                    'MyMsgBox(strErrAmaxMotion(AMONetMot.ipAxis, iMStatus))
                End If
            End If

            iMStatus = B_mnet_m4_get_position(.RingIndex, .DeviceIp, .AxisIndex, posFeedback)
            If iMStatus < 0 Then
                If blnShowMsg = True Then
                    'MyMsgBox(strErrAmaxMotion(AMONetMot.ipAxis, iMStatus))
                End If
            End If

            iMStatus = B_mnet_m4_get_error_counter(.RingIndex, .DeviceIp, .AxisIndex, posErrorCounter)
            If iMStatus < 0 Then
                If blnShowMsg = True Then
                    'MyMsgBox(strErrAmaxMotion(AMONetMot.ipAxis, iMStatus))
                End If
            End If

            iMStatus = B_mnet_m4_get_current_speed(.RingIndex, .DeviceIp, .AxisIndex, CurrentSpeed)
            If iMStatus < 0 Then
                If blnShowMsg = True Then
                    'MyMsgBox(strErrAmaxMotion(AMONetMot.ipAxis, iMStatus))
                End If
            End If

            iMStatus = B_mnet_m4_motion_done(.RingIndex, .DeviceIp, .AxisIndex, MotionStatus)
            If iMStatus < 0 Then
                If blnShowMsg = True Then
                    'MyMsgBox(strErrAmaxMotion(AMONetMot.ipAxis, iMStatus))
                End If
            End If
            Dim errStatus As Integer
            Static tmr As New Stopwatch
            AMaxM4_ErrorStatus(__AMONetMotion.ipAxis, errStatus)

        If lblMotorErrorStatus.Text = My.Resources.NO_ERROR OrElse lblMotorErrorStatus.Text = "" Then
            lblMotorErrorStatus.Text = (New errorStatusTypeConvertor).ConvertTo(errStatus, GetType(String))
            tmr.Restart()
        ElseIf tmr.ElapsedMilliseconds > 3000 Then
            lblMotorErrorStatus.Text = ""
        End If
        If lblMotorErrorStatus.Text = My.Resources.NO_ERROR OrElse lblMotorErrorStatus.Text = "" Then
            lblMotorErrorStatus.BackColor = SystemColors.Control
        Else
            lblMotorErrorStatus.BackColor = Color.Tomato
        End If

            lblPosition0.Text = Format(posCommand / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit, "0.00")
            lblPosition1.Text = Format(posFeedback / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit, "0.00")
            lblPosition2.Text = Format(posErrorCounter / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit, "0.00")
            lblMotionVelocity.Text = Format(CurrentSpeed / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit, "0.00")

            lblMotionStatus.Text = MotionStatus.ToString & ":" & AMaxM4_MotionStatusMsg(MotionStatus)

        End With

        ' latch 
        Dim LatchData_Cmd As Double = 0
        Dim LatchData_feedback As Double = 0
        AMaxM4_GetLatchData(__AMONetMotion.ipAxis, 1, LatchData_Cmd) ' ltc_no: 1-cmd counter, 2-feedback counter, 3-Error counter
        TextBox_LatchDataCommand.Text = Format((LatchData_Cmd / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit), "0.0000")
        AMaxM4_GetLatchData(__AMONetMotion.ipAxis, 2, LatchData_feedback) ' ltc_no: 1-cmd counter, 2-feedback counter, 3-Error counter
        TextBox_LatchDataFeedback.Text = Format((LatchData_feedback / pData.MotorSettings(__AMONetMotion.ipAxis).PulsePerUnit), "0.0000")

    End Sub

    Private Sub ShowScan()
        Static i As Short
        Static iMax As Short
        Static sec_old As Short
        Static C As String
        Dim sec_now As Short
        i = i + 1
        sec_now = Second(Now)
        If sec_now <> sec_old Then
            sec_old = sec_now
            iMax = i
            i = 0
        End If
        Select Case C
            Case "/" : C = "—"
            Case "—" : C = "\"
            Case "\" : C = "|"
            Case "|" : C = "/"
            Case Else : C = "/"
        End Select

        Me.Text = "Control Table" & " " & C
    End Sub


#Region "find predicate"
    Private Function findPartAndNameWithCbo(ByVal motorSetting As cMotorSetting) As Boolean
        '------------------------------------
        ' find the item which's name and part matched with combo box
        'remarked by Hsien , 2014/5/30
        '------------------------------------
        Return (motorSetting.MotorName = cboMStepName.SelectedItem) And (motorSetting.Station = cboMStepPart.SelectedItem)
    End Function

    Private Function findAllPartWithCbo(ByVal motorSetting As cMotorSetting) As Boolean
        '------------------------------------
        ' find the item which's stationGroup matched with combo box
        'remarked by Hsien , 2014/5/30
        '------------------------------------
        Return motorSetting.Station = cboMStepPart.SelectedItem
    End Function

    Private Function findAllAxisIndex(ByVal motorPoint As cMotorPoint) As Boolean
        '------------------------------------
        ' by Hsien , 2014/5/30
        '------------------------------------
        Return motorPoint.AxisIndex = targetAxisIndex
    End Function

    Private Function findPointNameAndAxisIndex(ByVal motorPoint As cMotorPoint) As Boolean
        '------------------------------------
        ' by Hsien , 2014/5/30
        '------------------------------------
        'Return motorPoint.PointName = targetPointName
        Return (motorPoint.PointName = targetPointName) And findAllAxisIndex(motorPoint)    'hsien , 2015.06.03 , to solve the issue when different axis use the same point name
    End Function

#End Region


    Private Sub formSettingLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SettingLogSave("Load FormSetting")
        'Hsien , build link from HAL
        amaxDevice = CType(Instance.PhysicalHardwareList.Find(Function(__hardware As subHardwareNode) (__hardware.PhysicalHardware.HardwareCode And hardwareCodeEnum.AMAX_1202_CARD)).PhysicalHardware, amaxCard)

        utilitiesUI.applyResourceOnAllControls(Me.Controls, New System.ComponentModel.ComponentResourceManager(Me.GetType()))   'Hsien , 2015.06.03

        Dim i As Integer
        '==== 設定ComboBox的型式 ======================================
        cboMStepPart.DropDownStyle = ComboBoxStyle.DropDownList
        cboMStepName.DropDownStyle = ComboBoxStyle.DropDownList
        ''====Initial General Tab=====================================
        Call frmMotionTestImageList()
        '=== Set the State of the Ring-Button and DeviceIP ===
        Call lblRing_DeviceIP_NameSet()

        For i = 0 To 1
            Me.tabRemoteTable.Controls("gboxAMONet" & i).Controls("btnStartRing" & i).Enabled = False ' Reset initially
        Next
        '==========================
        '初始化馬達速度參數DataGridView
        Call msgStepRecordInitSet(msgStepRecord, 5, 1)      'setup basic attribute of data grid view remarked by Hsien , 2014/5/30

        '------------------------------------
        'loading texts , remarked by Hsien , 2014/5/30
        '------------------------------------
        cboMStepPart.Items.Clear()
        cboMStepName.Items.Clear()

        '------------------------------------------------------------------------
        '   Hsien , 2015.01.30 , used to reject exception
        '-------------------------------------------------------------------------
        If (pData.MotorSettings.Count = 0) Then
            MessageBox.Show("Error : pData.MotorSettings had no any content.")
            Exit Sub
        End If

        For Each motorSetting As cMotorSetting In pData.MotorSettings
            '------------------------------------
            ' adding items if not existed in cbo
            'remarked by Hsien , 2014/5/30
            '------------------------------------
            If (Not cboMStepPart.Items.Contains(motorSetting.Station)) Then
                cboMStepPart.Items.Add(motorSetting.Station)
            End If
        Next
        cboMStepPart.SelectedIndex = 0

        __AMONetMotion.ipAxis = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo) 'test ok  , remarked by Hsien , 2014/5/30
        Call msgStepRecordHeaderTextSetting(__AMONetMotion.ipAxis) ' 設定標頭
        Call msgMStepRecordGet(__AMONetMotion.ipAxis)   '取得馬達位置點的內容


        'utilitiesUI.applyResourceOnAllControls(Me.Controls, New System.ComponentModel.ComponentResourceManager(Me.GetType()))

        '------------------------------
        '   Draw the page , Hsien , 2015.01.20
        '------------------------------
        userControlIOTable1.initialize(amaxDevice)


        timerRefresh.Enabled = True

        ' Hsien , MStep used by goToPosition and GoHome only
        ' 2014/08/26
        MStep.initialize()

        '-----------------------------------------------
        '   Show up current editing file
        '-----------------------------------------------
        TextBoxCurrentFile.Text = PointFilename


        '-------------------------------------------------
        '   PLC Page Preparation
        '-------------------------------------------------
        'scan all dmt slave
        Dim slaveList As List(Of dmtModbusSlave) = New List(Of dmtModbusSlave)
        Dim dmtList As List(Of subHardwareNode) = Instance.PhysicalHardwareList.FindAll(Function(__subHardware As subHardwareNode) __subHardware.PhysicalHardware.GetType.Equals(GetType(dmtModbusInterface)))
        dmtList.ForEach(Sub(__subHardware As subHardwareNode)
                            With CType(__subHardware.PhysicalHardware, dmtModbusInterface)
                                .MasterList.ForEach(Sub(__master As dmtModbusMaster)
                                                        slaveList.AddRange(__master.SlaveList)
                                                    End Sub)
                            End With
                        End Sub)

        For Each item As dmtModbusSlave In slaveList
            Dim __page As TabPage = New TabPage With {.Text = item.SlaveName}
            Dim __control As userControlDMTSlave = New userControlDMTSlave With {.SlaveReference = item}
            __page.Controls.Add(__control)
            tabGeneral.TabPages.Add(__page)
        Next
        '-------------------------------------------
        '   Melsec Page Preparation
        '-------------------------------------------
        Dim melsecList As List(Of melsecOverEthernet) = New List(Of melsecOverEthernet)
        Instance.PhysicalHardwareList.FindAll(Function(__subHardware As subHardwareNode) __subHardware.PhysicalHardware.GetType.Equals(GetType(melsecOverEthernet))).ForEach(Sub(node As subHardwareNode)
                                                                                                                                                                                 melsecList.Add(node.PhysicalHardware)
                                                                                                                                                                             End Sub)
        For Each item As melsecOverEthernet In melsecList
            Dim __page As TabPage = New TabPage With {.Text = "MELSEC"}
            Dim __control As userControlMelsec = New userControlMelsec With {.MelsecReference = item}
            __page.Controls.Add(__control)
            tabGeneral.TabPages.Add(__page)
        Next



    End Sub
    
    Private Sub btnGetPosition_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetPosition.Click
        '------------------------------------
        ' need to confirm later
        'remarked by Hsien , 2014/5/26
        '------------------------------------

        With msgStepRecord
            If .CurrentCell.ColumnIndex = 1 Then
                If .CurrentRow.Index <= .RowCount Then
                    .Rows(.CurrentRow.Index).Cells(.CurrentCell.ColumnIndex).Value = Val(lblPosition0.Text) 'Feedback position by Hsien , 2014/5/30
                End If
            End If
        End With
    End Sub
#Region "moved from mdlAMONetPosReadWrite.vb"
    Public Sub msgMStepRecordGet(ByVal AxisIP As Integer)
        '********************
        '馬達軸各點位資料顯示
        '********************
        Dim iColumn As Integer, iRow As Integer
        Dim i As Short, ppm As Double
        With __AMONetMotion
            Me.msgStepRecord.AllowUserToAddRows = False '***
            With pData
                For i = 0 To .MotorPoints.Count - 1
                    If .MotorPoints.Item(i).AxisIndex = AxisIP Then '檢查是否為此軸馬達位置點
                        ppm = pData.MotorSettings(AxisIP).PulsePerUnit
                        Me.msgStepRecord.RowCount = iRow + 1 '列數加一
                        Me.msgStepRecord.Rows.Item(iRow).HeaderCell.Value = .MotorPoints.Item(i).PointName '位置點名稱
                        'If Strings.Right(.MotorPoints.Item(i).sName, 3) = "回原點" Then
                        'remarked by Hsien , 2014/5/28
                        If .MotorPoints(i).IsHomePoint Then
                            If .MotorPoints.Item(i).PointType = 0 Then Me.msgStepRecord(0, iRow).Value = "-" Else Me.msgStepRecord(0, iRow).Value = "+"
                        Else
                            If .MotorPoints.Item(i).PointType = 0 Then Me.msgStepRecord(0, iRow).Value = "Abs" Else Me.msgStepRecord(0, iRow).Value = "Rel"
                        End If
                        Me.msgStepRecord(1, iRow).Value = Format(.MotorPoints.Item(i).Distance / ppm, "0.####")
                        Me.msgStepRecord(2, iRow).Value = Format(.MotorPoints.Item(i).StartVelocity / ppm, "0.00")
                        Me.msgStepRecord(3, iRow).Value = Format(.MotorPoints.Item(i).Velocity / ppm, "0.00")
                        Me.msgStepRecord(4, iRow).Value = .MotorPoints.Item(i).AccelerationTime
                        Me.msgStepRecord(5, iRow).Value = .MotorPoints.Item(i).DecelerationTime    ' Hsien , 2015.01.20

                        For iColumn = 0 To Me.msgStepRecord.ColumnCount - 1
                            Me.msgStepRecord(iColumn, iRow).Style.ForeColor = Color.Black
                        Next
                        iRow += 1
                    End If
                Next
            End With
            Me.msgStepRecord.ForeColor = Color.Black
        End With
    End Sub
    Public Sub msgMStepRecordSave()
        '------------------------------------
        ' Write fresh data in dataGridView into pData
        ' 1. find all point data associating with selected axis
        ' 2. row by row write in pData
        ' by Hsien , 2014/5/30
        '------------------------------------
        targetAxisIndex = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)  ' finding corresponding axis-index
        Dim tempList As List(Of cMotorPoint) = pData.MotorPoints.FindAll(AddressOf findAllAxisIndex) ' find out corresponding axis point
        Dim refMotorPoint As cMotorPoint ' ' by Hsien , 2014/5/30
        Dim ppm As Double = pData.MotorSettings(targetAxisIndex).PulsePerUnit
        Dim oldMotorPoint As cMotorPoint
        ' row scanning and write-in by Hsien , 2014/5/30
        For rowIndex = 0 To Me.msgStepRecord.RowCount - 1
            targetPointName = Me.msgStepRecord.Rows.Item(rowIndex).HeaderCell.Value
            refMotorPoint = pData.MotorPoints.Find(AddressOf findPointNameAndAxisIndex)
            If refMotorPoint Is Nothing Then
                MsgBox(String.Format("cannot find the refMotorPoint {0}", targetPointName))
            Else
                oldMotorPoint = refMotorPoint.Clone
                ' write-in by Hsien , 2014/5/30
                If Me.msgStepRecord(0, rowIndex).Value = "-" Then
                    refMotorPoint.PointType = 0
                End If
                If Me.msgStepRecord(0, rowIndex).Value = "+" Then
                    refMotorPoint.PointType = 1
                End If
                If Me.msgStepRecord(0, rowIndex).Value = "Abs" Then
                    refMotorPoint.PointType = 0
                End If
                If Me.msgStepRecord(0, rowIndex).Value = "Rel" Then
                    refMotorPoint.PointType = 1
                End If

                refMotorPoint.Distance = Me.msgStepRecord(1, rowIndex).Value * ppm
                If CSng(Me.msgStepRecord(2, rowIndex).Value) > CSng(Me.msgStepRecord(3, rowIndex).Value) Then
                    MessageBox.Show("The value of start velocity is more than velocity, it would not be saved!")
                ElseIf CSng(Me.msgStepRecord(2, rowIndex).Value) > 3000 Then
                    MessageBox.Show("The value of start velocity is over 30000 mm/s, it would not be saved!")
                Else
                    refMotorPoint.StartVelocity = Me.msgStepRecord(2, rowIndex).Value * ppm
                End If

                If CSng(Me.msgStepRecord(3, rowIndex).Value) < 3000 Then
                    refMotorPoint.Velocity = Me.msgStepRecord(3, rowIndex).Value * ppm
                Else
                    MessageBox.Show("The value of velocity is over 30000 mm/s, it would not be saved!")
                End If
                refMotorPoint.AccelerationTime = Me.msgStepRecord(4, rowIndex).Value
                refMotorPoint.DecelerationTime = Me.msgStepRecord(5, rowIndex).Value    'Hsien , 2015.01.20

                Dim variance = utilities.DetailedCompare(oldMotorPoint, refMotorPoint)
                If variance.Count > 0 Then
                    SettingLogSave(String.Format("MotorPoint {0} parameters setting change", oldMotorPoint.PointName))
                    variance.ForEach(Sub(var As variance)
                                         If var.Prop.Contains("Velocity") Or var.Prop.Contains("Distance") Then
                                             var.valA = var.valA / ppm
                                             var.valB = var.valB / ppm
                                         End If
                                         SettingLogSave(String.Format("{0}: {1} -> {2}", var.Prop, var.valA, var.valB))
                                     End Sub)
                End If

            End If
        Next


    End Sub
    Public Sub SettingLogSave(ByVal CommData As String)
        Dim strDirName As String
        Dim strFileName As String
        Dim tDate As String, tTime As String
        Dim strStringToSave As String



        Try
            '=== Get Date & Time ===
            tDate = Format(Date.Now.Year, "0000") & "-" & Format(Date.Now.Month, "00") & "-" & Format(Date.Now.Day, "00")
            tTime = Format(Date.Now.Hour, "00") & ":" & Format(Date.Now.Minute, "00") & ":" & Format(Date.Now.Second, "00") & "." & Format(Date.Now.Millisecond, "000")

            '=== Set the File Name by using Date ===
            strDirName = CStr(My.Application.Info.DirectoryPath) & "\" & "\FormSettingLog"
            If Dir(strDirName, vbDirectory) = "" Then MkDir(strDirName)

            strFileName = strDirName & "\" & "FormSetting_" & tDate & ".txt"

            '=== Save Msg ===
            strStringToSave = tDate & " , " & tTime & " , " & Replace(CommData, vbCrLf, " ") + Environment.NewLine
            System.IO.File.AppendAllText(strFileName, strStringToSave)

        Catch ex As Exception
            MessageBox.Show("FormSetting log save error")
        End Try


    End Sub
    Public Sub GetMStepVelocityProfileFromGrid()
        Dim iColumn As Integer
        Dim iRow As Integer

        If IsNumeric(Me.msgStepRecord.Tag) = False Then Exit Sub
        iRow = Me.msgStepRecord.Tag
        With Me.msgStepRecord

            If CInt(Me.msgStepRecord.Tag) = 1 Then '回原點 (0: 負方向回原點, 1 : 正方向回原點 )
                For iColumn = 0 To 4
                    If iColumn = 0 Then Me.velProfile(iColumn) = IIf(Me.msgStepRecord(iColumn, 0).Value = "+", 1, 0)
                    If iColumn >= 2 Then
                        If IsNumeric(Me.msgStepRecord(iColumn, 0).Value) Then Me.velProfile(iColumn) = CDbl(Me.msgStepRecord(iColumn, 0).Value)
                    End If
                Next iColumn
            Else ' 位置 :  (0 : 絕對位置移動, 1: 相對位置移動)
                For iColumn = 0 To 4
                    If iColumn = 0 Then
                        Me.velProfile(iColumn) = IIf(Me.msgStepRecord(iColumn, iRow).Value = "Abs", 0, 1)
                    Else
                        If IsNumeric(Me.msgStepRecord(iColumn, iRow).Value) Then Me.velProfile(iColumn) = CDbl(Me.msgStepRecord(iColumn, iRow).Value)
                    End If
                Next iColumn
            End If

        End With
    End Sub
    Public Sub msgStepRecordInitSet(ByVal msgObj As DataGridView, ByVal intColNum As Integer, ByVal intRowNum As Integer)
        Dim i As Integer
        Dim ColWidth As Long '平均分配欄寬用

        With msgObj
            '==== 設定列數及行數 ====
            .ColumnCount = intColNum
            .RowCount = intRowNum
            '======設定每列標頭 =====
            ColWidth = 190
            For i = 0 To .RowCount - 1
                .RowHeadersWidth = ColWidth
                .Rows.Item(i).Resizable = DataGridViewTriState.True '可改變大小
                If i = 0 Then
                    .Rows.Item(i).HeaderCell.ToolTipText = "Homing"
                Else
                    .Rows.Item(i).HeaderCell.ToolTipText = .Rows.Item(i).HeaderCell.Value    'P0,P1,P2......
                End If
            Next
            '=====平均分配欄寬，每欄都靠右對齊，第一欄為置中=====
            ColWidth = 110
            For i = 0 To .ColumnCount - 1
                .Columns.Item(i).Resizable = DataGridViewTriState.True
                .Columns.Item(i).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                If i = 0 Then
                    .Columns.Item(i).Width = 60
                    .Columns.Item(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                ElseIf i = 2 Or i = 3 Then
                    .Columns.Item(i).Width = ColWidth
                    .Columns.Item(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                ElseIf i = 4 Then
                    .Columns.Item(i).Width = 90
                    .Columns.Item(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                Else
                    .Columns.Item(i).Width = ColWidth
                    .Columns.Item(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                End If
            Next
        End With

    End Sub
    Public Sub msgStepRecordHeaderTextSetting(ByVal ipAxis As Short)
        '設定標頭欄位名稱
        Dim i As Integer
        Dim AxPoint As New List(Of cMotorPoint)
        Dim numPosition As Short

        With Me.msgStepRecord
            With pData
                '得到此軸的馬達所有點位數
                For i = 0 To .MotorPoints.Count - 1
                    If .MotorPoints(i).AxisIndex = ipAxis Then
                        numPosition = numPosition + 1
                    End If
                Next
            End With
            '設定欄數及列數
            '.ColumnCount = 5
            .ColumnCount = 6            'Hsien , 2015.01.20 , deceleration added
            .RowCount = numPosition + 1
            Me.msgStepRecord.MultiSelect = False     ' No multi-selection

                       '=== Row Header Text ===
            .Columns.Item(0).HeaderCell.Value = My.Resources.PointType

            '.Columns.Item(1).HeaderCell.Value = "位置" + Chr(13) + pData.MotorSettings(ipAxis).Unit.ToString()
            .Columns.Item(1).HeaderCell.Value = My.Resources.Position + pData.MotorSettings(ipAxis).Unit.ToString() + ")"

            Me.lblVelocityProfileUnit0.Text = pData.MotorSettings(ipAxis).Unit.ToString()
            Me.lblVelocityProfileUnit1.Text = pData.MotorSettings(ipAxis).Unit.ToString() + "/s"
            Me.lblVelocityProfileUnit2.Text = pData.MotorSettings(ipAxis).Unit.ToString() + "/s"
            .Columns.Item(2).HeaderCell.Value = My.Resources.StartVelocity & "(" + Me.lblVelocityProfileUnit1.Text & ")"
            .Columns.Item(3).HeaderCell.Value = My.Resources.Velocity & "(" + Me.lblVelocityProfileUnit2.Text & ")"
            .Columns.Item(4).HeaderCell.Value = My.Resources.AccelerationTime & "(sec.)"
            .Columns.Item(5).HeaderCell.Value = My.Resources.DecelerationTime & "(sec.)" 'Hsien , 2015.01.20 , deceleration added


        End With
    End Sub
#End Region
#Region "Home and GoToPositionOperation"
    Public Const NumberOfStep = 150
    Public MStep As SteppingMotor ' = New SteppingMotor()
    'Public alarm As AlarmMutex ' = New AlarmMutex()
    '#Const kclsTmrDogTest = True
#Const KTIME_TICK_USING_FRAMEWORK = True       ' As compiler switch
    '#Const KTIME_TICK_USING_GETTICKCOUNT = True
    '-------------------------------------------------------------------------------------

    Public Class cTimer
#Region "field"   ' 摺疊與隱藏 Visual Basic 檔案中的程式碼區段。
        '#If KTIME_TICK_USING_GETTICKCOUNT = True Then
        '    Private Declare Function Get_TimeTick Lib "kernel32" Alias "GetTickCount" () As UInt32
        '#Else
        '    Private Declare Ansi Function Get_TimeTick Lib "winmm.dll" Alias "timeGetTime" () As UInt32
        '#End If

        '    <DllImport("winmm.dll", EntryPoint:="timeGetTime", SetLastError:=True)> _
#If Not KTIME_TICK_USING_FRAMEWORK = True Then
#If KTIME_TICK_USING_GETTICKCOUNT = True Then
    ' Interval about 16ms
    ' Shared: 靜態成員每一個執行個體共用該成員，而不是每個執行個體各自保留一份成員複本
    '           且不需建立物件，就可以直接使用列別名稱來存取和呼叫，Ex:Math.Abs(...)
    <DllImport("kernel32.dll", EntryPoint:="GetTickCount")> _
    Public Shared Function Get_TimeTick() As UInt32
    End Function
#Else
    ' Interval = 1ms @.Net 2010
    <DllImport("winmm.dll", EntryPoint:="timeGetTime")> _
    Public Shared Function Get_TimeTick() As UInt32
    End Function
#End If
#End If
        Public Const kTickPreSecond As Single = 1000.0F
        Private Const kMAX_TMR_Round As UInt32 = UInt32.MaxValue    ' CUInt((2 ^ 32) - 1)    ' 429,4967,295(FFFFFFFFh)
        ' 用來存放屬性值的區域變數
        Private dwStartT As UInt32      ' 開始時間
        Private dwFinishT As UInt32     ' 目標時間
        Private bOverRound As Boolean   ' 目標時間跨過 0點旗標(Next Round)
        Private bDue As Boolean         ' 計時終了
        Friend iGU16 As Int16

        Public Event TimeDue(ByVal iNowTick As UInt32)    ' Event Function
#End Region
#Region "methods"
        Public Sub New(Optional ByVal lVal As UInt32 = 0)    ' 建立建構函式 Constructor
            dwStartT = lVal
            Reset()
        End Sub
        'Protected Overrides Sub Finalize()
        '    dwStartT = 0
        'End Sub
        Public Sub Reset()
            iGU16 = 0
            dwStartT = 0
            dwFinishT = 0
            bDue = False
            bOverRound = False
        End Sub
        Friend Function tFriend(ByVal Par1 As Long, ByVal Par2 As Integer) As Boolean
            tFriend = (Par1 = Par2)
        End Function
        Public Shared ReadOnly Property TimerCount() As UInt32
            ' Shared: 靜態成員每一個執行個體共用該成員，而不是每個執行個體各自保留一份成員複本
            '           且不需建立物件，就可以直接使用列別名稱來存取和呼叫，Ex:Math.Abs(...)
            Get
#If kclsTmrDogTest = True Then
            Return CUInt(Val(Form1.TextBox1.Text)) And kMAX_TMR_Round
#ElseIf KTIME_TICK_USING_FRAMEWORK = True Then
                Dim ui32 As UInt32, iTick32 As Integer
                Try
                    iTick32 = Environment.TickCount And Int32.MaxValue ' Interval about 16ms.
                    ui32 = CUInt(iTick32)
                Catch ex As OverflowException   ' Environment.TickCount 由 MaxValue -> MinValue
                    ui32 = Integer.MaxValue
                    ui32 = CUInt(ui32 - iTick32)
                    'ui32 = CUInt(ui32 + System.Math.Abs(iTick32))
                Catch ex As Exception
                    MsgBox("TimerCount Error:(" & ex.Message & ")t=" & iTick32)
                End Try
                Return (ui32 And kMAX_TMR_Round)
#Else
            Return Get_TimeTick() And kMAX_TMR_Round
#End If
            End Get
        End Property
        Public Function reStart() As UInt32
            Dim u64 As UInt64

            u64 = CULng(kMAX_TMR_Round + 1 + dwFinishT - dwStartT) And kMAX_TMR_Round
            TimeDelay = CUInt(u64)
            Return CUInt(u64)
        End Function
        Public Sub DelayS(ByVal sngDLy As Single)
            If sngDLy < 0 Then sngDLy = 0
            Call SetDelay(CUInt(sngDLy * 1000))
        End Sub
        Public Function DelayS() As Boolean
            DelayS = WaitDelay()
        End Function
        Public Sub SetDelay(Optional ByVal dwDLy As UInt32 = 0)
            dwStartT = TimerCount        ' 取得現在的 TimeTick 值
            Try
                bOverRound = False
                dwFinishT = (dwStartT + dwDLy)      ' 目標時間(可能溢位)
            Catch ex As OverflowException
                Dim u64 As UInt64 = dwStartT
                bOverRound = True                   ' 目標時間跨越 0點(目標 > MaxRound)
                u64 += dwDLy
                dwFinishT = CUInt(u64 And kMAX_TMR_Round)   ' 設成下一計數週期的值(Next Round)
                'Catch ex As DivideByZeroException
            Catch ex As Exception
                MsgBox("SetDelay Error:(" & ex.Message & ")s=" & dwStartT & ",e=" & dwFinishT & "ms," & dwDLy)
                bOverRound = False
                dwFinishT = dwStartT
                'Finally    無論是否有例外，最會都會執行此區塊。
            End Try
        End Sub
        Private Function WaitDelay() As Boolean
            Dim dwNow As UInt32, bTMp As Boolean

            dwNow = TimerCount

            If (bOverRound) Then    ' 目標時間跨過 0點(Next time-Round)
                ' 現在時間 >= 目標時間 & 現在時間 < 開始時間(下一個時間計數週期才生效)
                bTMp = (dwNow >= dwFinishT) And (dwNow < dwStartT)  ' Now < Start and Now >= Finish
            Else
                ' 現在時間 >= 目標時間 or 現在時間 < 開始時間(現在時間已跨入新的計數週期)
                bTMp = (dwNow >= dwFinishT) Or (dwNow < dwStartT)   ' Now >= Finish or Now < Statr
            End If

            If bDue = False And bTMp = True Then
                RaiseEvent TimeDue(dwNow) ' 引發事件
            End If

            bDue = bTMp
            Return bDue
        End Function
#End Region
#Region "properties"
        Public WriteOnly Property TimeDelay() As UInt32
            ' Public WriteOnly Property TimeDelay() As UInt32
            ' WriteOnly or Readonly 
            'Get
            '    Return 0
            'End Get
            Set(ByVal dwDLy As UInt32)
                SetDelay(dwDLy)
            End Set
        End Property
        Public ReadOnly Property isTimeUp() As Boolean 'Expired
            Get
                Return WaitDelay()
            End Get
        End Property
        Public ReadOnly Property Remaining() As UInt32  ' 還剩多少時間
            Get
                Dim u64 As UInt64 = CULng(kMAX_TMR_Round + 1 + dwFinishT - TimerCount) And kMAX_TMR_Round
                Dim uTmr64 As ULong = CULng(kMAX_TMR_Round + 1 + dwFinishT - dwStartT) And kMAX_TMR_Round

                If (u64 >= uTmr64) Then u64 = 0
                Return CUInt(u64)
            End Get
        End Property
        Public ReadOnly Property Elasped() As UInt32    ' 已經過多少時間
            Get
                Dim u64 As UInt64 = CULng(kMAX_TMR_Round + 1 - dwStartT)
                u64 += TimerCount
                Return CUInt(u64 And kMAX_TMR_Round)
            End Get
        End Property
#End Region
    End Class


    Public Structure SteppingMotor
        '------------------------------------
        ' indicate executing sequence for each motor
        '   remarked by Hsien , 2014/5/28
        '------------------------------------
        Public StepGoHome_p() As Integer
        Public StepGoToPosition_p() As Integer
        Public StepVMove_p() As Integer
        Public StepAMove_p() As Integer
        Public StepRMove_p() As Integer
        Public StepGoToPosition2_p() As Integer
        Public StepGoToPositionPV2_p() As Integer
        Public StepGoToRobotPosition_p() As Integer
        Public StepCheckAlarm_p() As Integer
        Public strStep() As String

        Public Function initialize()
            '------------------------------------
            ' 1. allocating memory according to number of motors
            ' 2. clear machine-state control data
            ' by Hsien , 2014/6/4
            '------------------------------------
            ReDim Me.strStep(pData.MotorSettings.Count)

            For i = 0 To pData.MotorSettings.Count - 1
                'MStep.strStep(DirectCast([Enum].Parse(GetType(eMotorIndex), [Enum].GetName(GetType(eMotorIndex), i)), eMotorIndex)) = [Enum].GetName(GetType(eMotorIndex), i)
                Me.strStep(i) = pData.MotorSettings(i).MotorName
            Next

            'clear machine-state control data by Hsien , 2014/6/4
            '-----------------------------------
            ' Clear motors' status
            'remarked by Hsien , 2014/5/28
            '-----------------------------------
            'Dim i As Integer
            'Call MStepMotorStructure()

            '------------------------------------
            ' to eliminate redundant void function(void)
            'remarked by Hsien , 2014/5/28
            '------------------------------------
            With Me

                ReDim .StepGoHome_p(NumberOfStep)
                ReDim .StepGoToPosition_p(NumberOfStep)
                ReDim .StepVMove_p(NumberOfStep)
                ReDim .StepAMove_p(NumberOfStep)
                ReDim .StepRMove_p(NumberOfStep)
                ReDim .StepGoToPosition2_p(NumberOfStep)
                ReDim .StepGoToPositionPV2_p(NumberOfStep)
                ReDim .StepGoToRobotPosition_p(NumberOfStep)
                ReDim .StepCheckAlarm_p(NumberOfStep)

                For i = 0 To NumberOfStep
                    .StepGoHome_p(i) = 0
                    .StepGoToPosition_p(i) = 0
                    .StepVMove_p(i) = 0
                    .StepAMove_p(i) = 0
                    .StepRMove_p(i) = 0
                    .StepGoToPosition2_p(i) = 0
                    .StepGoToPositionPV2_p(i) = 0
                    .StepGoToRobotPosition_p(i) = 0
                    .StepCheckAlarm_p(i) = 0
                Next i
            End With
            Return 0
        End Function
    End Structure
    Public alarm As alarmMutex = New alarmMutex()

    Public Function MAmax_GoHome(ByVal nAxis As Integer, Optional ByVal tScanTime As Single = 40, Optional ByVal blnReset As Boolean = False) As Boolean
        '---------------------------------
        '   Home returning layer function 
        '---------------------------------
        'Static tmr(NumberOfStep) As cTimer
        'Static wdgMode_f As Boolean
        Dim Status As Integer = 0
        Dim MoveStatus As Integer
        Dim Sv As Double
        Dim Vel As Double
        Dim Tacc As Double
        Dim Tdec As Double = 0  'Hsien , 2015.05.12
        Dim OrgOffset As Long
        Dim iDir As Short
        Dim i As Short
        Static tmr As Automation.Components.Services.singleTimer = New Automation.Components.Services.singleTimer With {.TimerGoal = New TimeSpan(0, 0, 1)}

        'MAmax_GoHome = False

        If blnReset Then
            MStep.StepGoHome_p(nAxis) = 0
            'Exit Function
            Return False
        End If

        '是否為此馬達軸發生錯誤，若是則跳開 
        If MAmax_CheckAlarm(nAxis) = False Then
            'Exit Function
            Return False
        End If

        If alarm.code < 0 AndAlso
            (alarm.sw = eSwitch.eON OrElse alarm.type = alarmMutex.eErrType.ERROR_MOTOR) AndAlso
            errorAxisIndex = nAxis Then
            Return False
        End If

        'If wdgMode_f = False Then
        '    Status = AMaxM4_WatchDogSet(2)
        '    wdgMode_f = True
        'End If

        'If tmr(nAxis) Is Nothing Then
        '    tmr(nAxis) = New cTimer '若尚未初始化，則new一次
        'End If

        '從結構MPoint找到此軸的位置點且名稱右方5個字元為" Home"的點位
        With pData
            For i = 0 To .MotorPoints.Count - 1
                '           If Strings.Right(MPoints(i).sName, 3) = "回原點" And MPoints(i).iAxis = nAxis Then
                '------------------------------------
                ' search for the point with attribute "isHomeUsed"
                'remarked by Hsien , 2014/5/27
                '------------------------------------
                If .MotorPoints(i).IsHomePoint = True And .MotorPoints(i).AxisIndex = nAxis Then
                    With .MotorPoints(i)
                        Sv = .StartVelocity
                        Vel = .Velocity
                        Tacc = .AccelerationTime
                        Tdec = .DecelerationTime
                        iDir = .PointType
                        OrgOffset = .Distance
                    End With
                    Exit For 'Once point searched , no more to do further searching move 'remarked by Hsien , 2014/5/27
                End If
            Next
        End With


        With MStep
            Select Case MStep.StepGoHome_p(nAxis)
                Case 0 '* Set the Velocity of Homing
                    Status = AMaxM4_SMovSet(nAxis, Sv, Vel, Tacc, Tdec, 0, 0)
                    If Status = 0 Then
                        .StepGoHome_p(nAxis) = 5
                        'Else
                        ' stuck in this state
                    End If
                Case 5
                    Status = AMaxM4_HomeConfig(nAxis, 1, pData.MotorSettings(nAxis).HomeLevel, 0, 0, 0) ' Initial ORG : A-Type
                    If Status = 0 Then
                        .StepGoHome_p(nAxis) = 105 '.StepGoHome_p(nAxis) = 5
                        'Else
                        ' stuck in this state
                    End If
                Case 105 '* Start to Go Home
                    Status = AMaxM4_HomeMove(nAxis, iDir, OrgOffset)
                    If Status = 0 Then
                        .StepGoHome_p(nAxis) = 115
                    End If
                    'Case 110 ' Set Detecting Time
                    '    tmr(nAxis).DelayS(tScanTime)
                    '    .StepGoHome_p(nAxis) = 115
                Case 115 '* Check Finish( 0:Stop )
                    Status = AMaxM4_MotionDone(nAxis, MoveStatus)
                    If Status = 0 And MoveStatus = 0 Then
                        ' move status = 0 means stop
                        tmr.IsEnabled = True
                        .StepGoHome_p(nAxis) = 120
                    End If

                    'Status = AMaxM4_MotionDone(nAxis, MoveStatus)
                    'If Status = 0 And MoveStatus = 0 Then
                    '    tmr(nAxis).DelayS(0.1)
                    '    .StepGoHome_p(nAxis) = 120
                    '    'ElseIf Status = 0 Then
                    'Else    ' by Hsien , 2014.06.09
                    '    If tmr(nAxis).DelayS Then
                    '        Status = ERR_Origin_Limit
                    '        ' should stop , Hsien , 2014.06.09
                    '    End If
                    'End If
                Case 120 ' Homing Finish
                    If tmr.IsTimerTicked = True Then
                        Status = AMaxM4_CmdPos_Reset(nAxis)
                        'MAmax_GoHome = True
                        .StepGoHome_p(nAxis) = 0
                        Return True ' Home procedure done successfully , Hsien , 2014.06.09
                        'End If
                    End If
            End Select
            'PROC_MOTOR_ERR:

            '=== Show Status Alarm ===
            If Status < 0 Then
                If alarm.sw = eSwitch.eOFF Then
                    alarm.sw = eSwitch.eON
                    'alarm.msw = True
                    alarm.type = alarmMutex.eErrType.ERROR_MOTOR
                    alarm.code = Status
                    alarm.RetryOnly = True
                    errorMessage = strErrAmaxMotion(nAxis, alarm.code)
                    errorAxisIndex = nAxis
                End If
            End If
        End With

        Return False    ' Hsien  ,2014.06.09

    End Function
    Public Function MAmax_GoToPosition(ByVal nPosition As cMotorPoint, Optional ByVal tScanTime As Single = 20, Optional ByVal blnReset As Boolean = False, Optional ByVal sngRatio As Single = 1) As Boolean
        '-----------------------------------
        ' single axis p2p movement execution
        '-----------------------------------
        'Static tmr(NumberOfStep) As cTimer
        'Static wdgMode_f As Boolean
        Dim Status As Integer = 0
        Dim MoveStatus As Integer = 0
        Dim posFeedback As Integer = 0
        Dim posCommand As Integer = 0
        Dim Sv As Double = 0
        Dim Vel As Double = 0
        Dim Tacc As Double = 0

        'MAmax_GoToPosition = False

        With MStep
            If blnReset = True Then
                .StepGoToPosition_p(nPosition.AxisIndex) = 0
                Return False
            End If

            'MAmax_GoToPosition = False
            'If tmr(nPosition.iAxis) Is Nothing Then
            '    tmr(nPosition.iAxis) = New cTimer
            'End If

            ' remarked by Hsien  , 2014.06.09 , redundant check
            ''==== 是否為此馬達軸發生錯誤，若是則跳開 ==========
            ' otherwise ,  starting reset alarm procedure
            If MAmax_CheckAlarm(nPosition.AxisIndex) = False Then
                Return False
            End If

            ' if there's any error in system , operation abort
            If alarm.code < 0 And alarm.sw <> eSwitch.eOFF Then
                'Exit Function
                Return False
            End If

            '=====  0:  Ignore stop =====
            'If wdgMode_f = False Then
            '    Status = AMaxM4_WatchDogSet(0)
            '    wdgMode_f = True
            'End If

            Select Case .StepGoToPosition_p(nPosition.AxisIndex)
                Case 0  '* Set the Velocity of Homing , and launch motion
                    Sv = nPosition.StartVelocity * sngRatio
                    Vel = nPosition.Velocity * sngRatio
                    Tacc = nPosition.AccelerationTime

                    Status = AMaxM4_SMovSet(nPosition.AxisIndex, Sv, Vel, Tacc, nPosition.DecelerationTime, 0, 0)

                    If Status = 0 Then
                        If nPosition.PointType = 0 Then  ' Abs-Move
                            Status = AMaxM4_AMov(nPosition.AxisIndex, CInt(nPosition.Distance))
                        Else ' Rel-Move
                            Status = AMaxM4_RMov(nPosition.AxisIndex, CInt(nPosition.Distance))
                        End If
                    End If

                    'If (nPosition.iAxis = allMotor.MSUE1) Then
                    '    monitorCountEnd = Environment.TickCount
                    '    Console.Write("y")
                    'End If

                    If Status = 0 Then
                        .StepGoToPosition_p(nPosition.AxisIndex) = 15
                        ' Else :
                        ' stuck in this state
                    End If

                    'Case 5 '* Check Abs or Rel Move
                    '    If nPosition.iType = 0 Then  ' Abs-Move
                    '        Status = AMaxM4_AMov(nPosition.iAxis, CInt(nPosition.Dist))
                    '    Else ' Rel-Move
                    '        Status = AMaxM4_RMov(nPosition.iAxis, CInt(nPosition.Dist))
                    '    End If
                    '    If Status = 0 Then
                    '        '.StepGoToPosition_p(nPosition.iAxis) = 10
                    '        .StepGoToPosition_p(nPosition.iAxis) = 15   ' Hsien  ,2014.06.09
                    '        ' Else : 
                    '        ' stuck in this state
                    '    End If

                    'Case 10 ' Set Detecting Time
                    '    'If tScanTime > 0 Then
                    '    tmr(nPosition.iAxis).DelayS(tScanTime)  ' edited by Hsien , 2014.06.09
                    '    ' tScanTime equal to expect
                    '    'Else
                    '    'tmr(nPosition.iAxis).DelayS(20)
                    '    'End If
                    '    .StepGoToPosition_p(nPosition.iAxis) = 15

                Case 15 '* Check Move-Finish
                    'Status = AMaxM4_MotionDone(nPosition.iAxis, MoveStatus)
                    Status = AMaxM4_MotionDone(nPosition.AxisIndex, MoveStatus)
                    If Status = 0 And MoveStatus = 0 Then
                        '    If (nPosition.iType = 0) Then
                        'abs()
                        .StepGoToPosition_p(nPosition.AxisIndex) = 30
                        'Else
                        '    'rel()
                        '    'not check motion done
                        '    'todo()
                        '    .StepGoToPosition_p(nPosition.iAxis) = 0
                        '    Return True
                        'End If
                    End If
                    'If Status = 0 And MoveStatus = 0 Then
                    '    'tmr(nPosition.iAxis).DelayS(3)
                    '    .StepGoToPosition_p(nPosition.iAxis) = 20
                    'Else :
                    ' stuck in this state
                    'Else
                    '    If tmr(nPosition.iAxis).DelayS = True Then
                    '        Status = ERR_Position_Move
                    '    End If
                    'End If

                    'Case 20 ' Grab command
                    '    Status = 
                    '    'If Status < 0 Then
                    '    '    GoTo PROC_MOTOR_ERR
                    '    'End If
                    '    If Status = 0 Then
                    '        .StepGoToPosition_p(nPosition.iAxis) = 25
                    '    End If
                    'Case 25 ' Grab position
                    '    Status = AMaxM4_Position_Get(nPosition.iAxis, posFeedback)

                    '    If Status = 0 Then
                    '        .StepGoToPosition_p(nPosition.iAxis) = 30
                    '    End If
                Case 30 '==== 檢查命令位置與回授位置是否小於0.5mm  ,若是，則回報成功 , Hsien , 2014.06.09
                    Status = 0
                    Status += AMaxM4_Get_Command(nPosition.AxisIndex, posCommand)
                    Status += AMaxM4_Get_Position(nPosition.AxisIndex, posFeedback)
                    If Status = 0 Then
                        If Math.Abs(posCommand - posFeedback) < 0.5 * pData.MotorSettings(nPosition.AxisIndex).PulsePerUnit Then
                            'position success
                            'MAmax_GoToPosition = True
                            .StepGoToPosition_p(nPosition.AxisIndex) = 0
                            Return True     ' all process done successfully , Hsien , 2014.06.09
                            'Else '* Did not reach the setting position
                            ' position failed
                            'If tmr(nPosition.iAxis).DelayS = True Then
                            '    Status = ERR_Setting_Position
                            'End If
                        End If
                    End If
            End Select

            'PROC_MOTOR_ERR:
            '=== Show Status Alarm ===
            If Status < 0 Then
                If alarm.sw = eSwitch.eOFF Then
                    alarm.sw = eSwitch.eON
                    'alarm.msw = True
                    alarm.type = alarmMutex.eErrType.ERROR_MOTOR
                    alarm.code = Status
                    alarm.RetryOnly = True
                    errorMessage = strErrAmaxMotion(nPosition.AxisIndex, alarm.code)
                    errorAxisIndex = nPosition.AxisIndex
                End If
            End If

            Return False ' Hsien , 2014.06.09 , to regular function structure

        End With
    End Function
    Public Function MAmax_CheckAlarm(ByVal nAxis As Integer) As Boolean
        ' return value:
        ' true : no alarm
        ' false : alarm happened , or reseting alarm
        Static tmr(NumberOfStep) As cTimer
        'Dim ioAlarm As Long
        Dim Status As Integer

        MAmax_CheckAlarm = False

        If tmr(nAxis) Is Nothing Then
            tmr(nAxis) = New cTimer '若尚未初始化，則new一次
        End If

        '=== Show Status Alarm ===
        With MStep
            Select Case .StepCheckAlarm_p(nAxis)
                Case 0 '* Show ioAlarm

                    If Not Me.readMotorStatus(nAxis, amaxMotionDIO_State.amax_IO_iALM) Then
                        MAmax_CheckAlarm = True
                    ElseIf Me.readMotorStatus(nAxis, amaxMotionDIO_State.amax_IO_iALM) Then
                        ' starting reset alarm procedure , Hsien  , 2014.06.09
                        If alarm.sw = eSwitch.eOFF Then
                            'alarm.msw = True
                            alarm.type = alarmMutex.eErrType.ERROR_MOTOR
                            alarm.sw = eSwitch.eON
                            alarm.code = returnErrorCodes.ERR_ioALM
                            alarm.RetryOnly = True
                            errorMessage = strErrAmaxMotion(nAxis, alarm.code)
                            errorAxisIndex = nAxis
                            .StepCheckAlarm_p(nAxis) = 5
                        End If
                    End If

                Case 5 '* Reset the Alarm
                    If alarm.sw = eSwitch.eOFF Then
                        Status = AMaxM4_ResetALM(nAxis, 1)
                        tmr(nAxis).DelayS(1)
                        If Status = 0 Then
                            .StepCheckAlarm_p(nAxis) = 10
                        End If
                    End If

                Case 10
                    If tmr(nAxis).DelayS = True Then
                        Status = AMaxM4_ResetALM(nAxis, 0)
                        .StepGoHome_p(nAxis) = 0                ' Reset Home Process
                        .StepGoToPosition_p(nAxis) = 0          ' Reset Position Process
                        .StepCheckAlarm_p(nAxis) = 0            ' Reset Alarm Process
                    End If
            End Select

            'PROC_MOTOR_ERR:

            '=== Show Status Alarm ===
            If Status < 0 Then
                If alarm.sw = eSwitch.eOFF Then
                    alarm.sw = eSwitch.eON
                    alarm.type = alarmMutex.eErrType.ERROR_MOTOR
                    'alarm.msw = True
                    alarm.code = Status
                    alarm.RetryOnly = True
                    errorMessage = strErrAmaxMotion(nAxis, alarm.code)
                    errorAxisIndex = nAxis
                End If
            End If

        End With
    End Function
#End Region



    Private Sub ButtonService_Click(sender As Object, e As EventArgs) Handles ButtonService.Click
        Static blnPassed As Boolean = False
        Dim frmLogin As LoginForm = New LoginForm With {.strPassword = "4526107", .Text = "Service Login Form"}
        If blnPassed = False Then
            frmLogin.ShowDialog()
            blnPassed = frmLogin.blnPassed
            If blnPassed = False Then Exit Sub
        End If
        Dim __dialog As Form = New Form
        Dim _tabControl As TabControl = New TabControl
        Dim TabPage1 = New TabPage()
        Dim TabPage2 = New TabPage()


        Dim __propertyGrid As PropertyGrid = New PropertyGrid With {.SelectedObject = pData.MotorSettings(__AMONetMotion.ipAxis),
                                                                    .Dock = DockStyle.Fill,
                                                                    .Font = New Font(Me.Font.Name, 12, Me.Font.Style, Me.Font.Unit)}
        Dim __propertyGrid2 As PropertyGrid = New PropertyGrid With {.Dock = DockStyle.Fill,
                                                                    .Font = New Font(Me.Font.Name, 12, Me.Font.Style, Me.Font.Unit)}


        If IsNumeric(msgStepRecord.Tag) = True Then
            Dim iRow = CInt(msgStepRecord.Tag)    '* Get the Row-Index
            targetAxisIndex = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)
            targetPointName = msgStepRecord.Rows.Item(iRow).HeaderCell.Value
            selectedPoint = pData.MotorPoints.FindIndex(AddressOf findPointNameAndAxisIndex)
            If selectedPoint <> -1 Then
                __propertyGrid2.SelectedObject = pData.MotorPoints(selectedPoint)
            End If
        End If
        With TabPage1
            .Text = "motor"
            .AutoScroll = True
            .Controls.Add(__propertyGrid)
        End With
        With TabPage2
            .Text = "point"
            .AutoScroll = True
            .Controls.Add(__propertyGrid2)
        End With
        With _tabControl
            .Dock = DockStyle.Fill
            .Controls.AddRange({TabPage1, TabPage2})
        End With
        With __dialog
            .Text = "Axis Number= " & __AMONetMotion.ipAxis
            __dialog.Controls.Add(_tabControl)
            __dialog.Size = New Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2)
            __dialog.AutoSize = True
            __dialog.ShowDialog()
        End With
        If MessageBox.Show("do you need to config this motor?", " ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
            'MsgBox("No")
        Else
            'MsgBox("Yes")
            pData.MotorSettings(__AMONetMotion.ipAxis).applyConfiguration()
        End If

    End Sub

    ''' <summary>
    ''' Hsien , the wrapper to read io status of motor , via HAL
    ''' </summary>
    ''' <param name="axisIndex"></param>
    ''' <param name="io"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function readMotorStatus(axisIndex As Integer, io As amaxMotionDIO_State) As Boolean
        Dim __ring As Integer = 0
        Dim __device As Integer = 0
        Dim __port As Integer = 0
        AMax_Get_Moton_DeviceIP(axisIndex, __ring, __device, __port)
        Return readBit(BitConverter.ToUInt64({__port * 16 + Math.Log(io, 2),
                                           0,
                                            __device,
                                            __ring,
                                            amaxModuleTypeEnum.REMOTE,
                                            0,
                                            0,
                                            hardwareCodeEnum.AMAX_1202_CARD}, 0))
    End Function

    Private Sub Btn_LatchEnable_Click(sender As Object, e As EventArgs) Handles Btn_LatchEnable.Click
        Try
            targetAxisIndex = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)
            AMaxM4_SetLatchEnable(__AMONetMotion.ipAxis, 1)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Btn_LatchDisable_Click(sender As Object, e As EventArgs) Handles Btn_LatchDisable.Click
        Try
            targetAxisIndex = pData.MotorSettings.FindIndex(AddressOf findPartAndNameWithCbo)
            AMaxM4_SetLatchEnable(__AMONetMotion.ipAxis, 0)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

End Class


