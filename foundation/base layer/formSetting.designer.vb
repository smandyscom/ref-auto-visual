﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formSetting
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請不要使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formSetting))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.timerRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.btnExit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.TextBoxCurrentFile = New System.Windows.Forms.TextBox()
        Me.imlfrmMotionTest = New System.Windows.Forms.ImageList(Me.components)
        Me.tabMotionTable = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_LatchDataCommand = New System.Windows.Forms.TextBox()
        Me.Btn_LatchDisable = New System.Windows.Forms.Button()
        Me.Btn_LatchEnable = New System.Windows.Forms.Button()
        Me.ButtonService = New System.Windows.Forms.Button()
        Me.lblStepPartTitle1 = New System.Windows.Forms.Label()
        Me.lblStepPartTitle0 = New System.Windows.Forms.Label()
        Me.cboMStepName = New System.Windows.Forms.ComboBox()
        Me.cboMStepPart = New System.Windows.Forms.ComboBox()
        Me.gboxStepControl = New System.Windows.Forms.GroupBox()
        Me.lblMotorErrorStatus = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblMotionStatus = New System.Windows.Forms.Label()
        Me.lblMotionVelocity = New System.Windows.Forms.Label()
        Me.gboxStatus = New System.Windows.Forms.GroupBox()
        Me.shpStatus15 = New System.Windows.Forms.Label()
        Me.shpStatus14 = New System.Windows.Forms.Label()
        Me.shpStatus13 = New System.Windows.Forms.Label()
        Me.shpStatus12 = New System.Windows.Forms.Label()
        Me.shpStatus11 = New System.Windows.Forms.Label()
        Me.shpStatus10 = New System.Windows.Forms.Label()
        Me.shpStatus9 = New System.Windows.Forms.Label()
        Me.shpStatus8 = New System.Windows.Forms.Label()
        Me.shpStatus7 = New System.Windows.Forms.Label()
        Me.shpStatus6 = New System.Windows.Forms.Label()
        Me.shpStatus5 = New System.Windows.Forms.Label()
        Me.shpStatus4 = New System.Windows.Forms.Label()
        Me.shpStatus3 = New System.Windows.Forms.Label()
        Me.shpStatus2 = New System.Windows.Forms.Label()
        Me.shpStatus1 = New System.Windows.Forms.Label()
        Me.shpStatus0 = New System.Windows.Forms.Label()
        Me.lblStatus15 = New System.Windows.Forms.Label()
        Me.lblStatus14 = New System.Windows.Forms.Label()
        Me.lblStatus13 = New System.Windows.Forms.Label()
        Me.lblStatus12 = New System.Windows.Forms.Label()
        Me.lblStatus11 = New System.Windows.Forms.Label()
        Me.lblStatus10 = New System.Windows.Forms.Label()
        Me.lblStatus9 = New System.Windows.Forms.Label()
        Me.lblStatus8 = New System.Windows.Forms.Label()
        Me.lblStatus7 = New System.Windows.Forms.Label()
        Me.lblStatus6 = New System.Windows.Forms.Label()
        Me.lblStatus5 = New System.Windows.Forms.Label()
        Me.lblStatus4 = New System.Windows.Forms.Label()
        Me.lblStatus3 = New System.Windows.Forms.Label()
        Me.lblStatus2 = New System.Windows.Forms.Label()
        Me.lblStatus1 = New System.Windows.Forms.Label()
        Me.lblStatus0 = New System.Windows.Forms.Label()
        Me.gboxMotion = New System.Windows.Forms.GroupBox()
        Me.btnMotion4 = New System.Windows.Forms.Button()
        Me.btnMotion3 = New System.Windows.Forms.Button()
        Me.btnMotion2 = New System.Windows.Forms.Button()
        Me.btnMotion1 = New System.Windows.Forms.Button()
        Me.btnMotion0 = New System.Windows.Forms.Button()
        Me.gboxPosition = New System.Windows.Forms.GroupBox()
        Me.btnGetPosition = New System.Windows.Forms.Button()
        Me.btnPositionReset = New System.Windows.Forms.Button()
        Me.lblPosition2 = New System.Windows.Forms.Label()
        Me.lblPosition1 = New System.Windows.Forms.Label()
        Me.lblPosition0 = New System.Windows.Forms.Label()
        Me.lblPositionTitle2 = New System.Windows.Forms.Label()
        Me.lblPositionTitle1 = New System.Windows.Forms.Label()
        Me.lblPositionTitle0 = New System.Windows.Forms.Label()
        Me.gboxVelocityProfile = New System.Windows.Forms.GroupBox()
        Me.lblVelocityProfileTitle0 = New System.Windows.Forms.Label()
        Me.gboxMoveMode = New System.Windows.Forms.GroupBox()
        Me.rdoMoveMode1 = New System.Windows.Forms.RadioButton()
        Me.rdoMoveMode0 = New System.Windows.Forms.RadioButton()
        Me.lblVelocityProfileUnit3 = New System.Windows.Forms.Label()
        Me.lblVelocityProfileUnit2 = New System.Windows.Forms.Label()
        Me.lblVelocityProfileUnit1 = New System.Windows.Forms.Label()
        Me.lblVelocityProfileUnit0 = New System.Windows.Forms.Label()
        Me.txtVelocityProfile3 = New System.Windows.Forms.TextBox()
        Me.txtVelocityProfile2 = New System.Windows.Forms.TextBox()
        Me.txtVelocityProfile1 = New System.Windows.Forms.TextBox()
        Me.txtVelocityProfile0 = New System.Windows.Forms.TextBox()
        Me.lblVelocityProfileTitle3 = New System.Windows.Forms.Label()
        Me.lblVelocityProfileTitle2 = New System.Windows.Forms.Label()
        Me.lblVelocityProfileTitle1 = New System.Windows.Forms.Label()
        Me.gboxRecord = New System.Windows.Forms.GroupBox()
        Me.btnGetVelocityProfile = New System.Windows.Forms.Button()
        Me.btnRecord2 = New System.Windows.Forms.Button()
        Me.btnMoveToMotionPosition = New System.Windows.Forms.Button()
        Me.msgStepRecord = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tabIOTable = New System.Windows.Forms.TabPage()
        Me.userControlIOTable1 = New Automation.UserControlIOTable()
        Me.tabRemoteTable = New System.Windows.Forms.TabPage()
        Me.gboxAMONet1 = New System.Windows.Forms.GroupBox()
        Me.btnStartRing1 = New System.Windows.Forms.Button()
        Me.lblAMONetRing1SubType = New System.Windows.Forms.Label()
        Me.lblAMONetRing1Type = New System.Windows.Forms.Label()
        Me.lblAMONetRing1SubTypeTitle = New System.Windows.Forms.Label()
        Me.lblAMONetRing1TypeTitle = New System.Windows.Forms.Label()
        Me.lblErrorSlave1 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP63 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP62 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP61 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP60 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP59 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP58 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP57 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP56 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP55 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP54 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP53 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP52 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP51 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP50 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP49 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP48 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP47 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP46 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP45 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP44 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP43 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP42 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP41 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP40 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP39 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP38 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP37 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP36 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP35 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP34 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP33 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP32 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP31 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP30 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP29 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP28 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP27 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP26 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP25 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP24 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP23 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP22 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP21 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP20 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP19 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP18 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP17 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP16 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP15 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP14 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP13 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP12 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP11 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP10 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP9 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP8 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP7 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP6 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP5 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP4 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP3 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP2 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP1 = New System.Windows.Forms.Label()
        Me.lblRing1_DeviceIP0 = New System.Windows.Forms.Label()
        Me.gboxAMONet0 = New System.Windows.Forms.GroupBox()
        Me.btnStartRing0 = New System.Windows.Forms.Button()
        Me.lblAMONetRing0SubType = New System.Windows.Forms.Label()
        Me.lblAMONetRing0Type = New System.Windows.Forms.Label()
        Me.lblAMONetRing0SubTypeTitle = New System.Windows.Forms.Label()
        Me.lblAMONetRing0TypeTitle = New System.Windows.Forms.Label()
        Me.lblErrorSlave0 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP63 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP62 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP61 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP60 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP59 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP58 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP57 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP56 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP55 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP54 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP53 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP52 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP51 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP50 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP49 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP48 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP47 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP46 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP45 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP44 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP43 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP42 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP41 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP40 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP39 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP38 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP37 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP36 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP35 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP34 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP33 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP32 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP31 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP30 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP29 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP28 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP27 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP26 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP25 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP24 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP23 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP22 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP21 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP20 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP19 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP18 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP17 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP16 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP15 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP14 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP13 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP12 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP11 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP10 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP9 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP8 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP7 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP6 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP5 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP4 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP3 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP2 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP1 = New System.Windows.Forms.Label()
        Me.lblRing0_DeviceIP0 = New System.Windows.Forms.Label()
        Me.tabGeneral = New System.Windows.Forms.TabControl()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox_LatchDataFeedback = New System.Windows.Forms.TextBox()
        Me.Panel1.SuspendLayout()
        Me.tabMotionTable.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.gboxStepControl.SuspendLayout()
        Me.gboxStatus.SuspendLayout()
        Me.gboxMotion.SuspendLayout()
        Me.gboxPosition.SuspendLayout()
        Me.gboxVelocityProfile.SuspendLayout()
        Me.gboxMoveMode.SuspendLayout()
        Me.gboxRecord.SuspendLayout()
        CType(Me.msgStepRecord, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabIOTable.SuspendLayout()
        Me.tabRemoteTable.SuspendLayout()
        Me.gboxAMONet1.SuspendLayout()
        Me.gboxAMONet0.SuspendLayout()
        Me.tabGeneral.SuspendLayout()
        Me.SuspendLayout()
        '
        'timerRefresh
        '
        Me.timerRefresh.Interval = 10
        '
        'btnExit
        '
        Me.btnExit.BackColor = System.Drawing.Color.LightGray
        resources.ApplyResources(Me.btnExit, "btnExit")
        Me.btnExit.Name = "btnExit"
        Me.btnExit.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.TextBoxCurrentFile)
        Me.Panel1.Controls.Add(Me.btnExit)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'TextBoxCurrentFile
        '
        resources.ApplyResources(Me.TextBoxCurrentFile, "TextBoxCurrentFile")
        Me.TextBoxCurrentFile.Name = "TextBoxCurrentFile"
        Me.TextBoxCurrentFile.ReadOnly = True
        '
        'imlfrmMotionTest
        '
        Me.imlfrmMotionTest.ImageStream = CType(resources.GetObject("imlfrmMotionTest.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlfrmMotionTest.TransparentColor = System.Drawing.Color.Transparent
        Me.imlfrmMotionTest.Images.SetKeyName(0, "ARW04UP.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(1, "ARW04DN.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(2, "ARW04LT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(3, "ARW04RT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(4, "ARW05UP.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(5, "ARW05DN.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(6, "ARW05LT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(7, "ARW05RT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(8, "ARW06UP.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(9, "ARW06DN.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(10, "ARW06LT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(11, "ARW06RT.ICO")
        Me.imlfrmMotionTest.Images.SetKeyName(12, "Stop.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(13, "End.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(14, "Exit.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(15, "SPlus.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(16, "SMinus.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(17, "PageUp.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(18, "PageDown.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(19, "DownLoad.ico")
        Me.imlfrmMotionTest.Images.SetKeyName(20, "UpLoad.ico")
        '
        'tabMotionTable
        '
        resources.ApplyResources(Me.tabMotionTable, "tabMotionTable")
        Me.tabMotionTable.BackColor = System.Drawing.SystemColors.Control
        Me.tabMotionTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.tabMotionTable.Controls.Add(Me.GroupBox1)
        Me.tabMotionTable.Controls.Add(Me.ButtonService)
        Me.tabMotionTable.Controls.Add(Me.lblStepPartTitle1)
        Me.tabMotionTable.Controls.Add(Me.lblStepPartTitle0)
        Me.tabMotionTable.Controls.Add(Me.cboMStepName)
        Me.tabMotionTable.Controls.Add(Me.cboMStepPart)
        Me.tabMotionTable.Controls.Add(Me.gboxStepControl)
        Me.tabMotionTable.Controls.Add(Me.gboxRecord)
        Me.tabMotionTable.Name = "tabMotionTable"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.TextBox_LatchDataFeedback)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TextBox_LatchDataCommand)
        Me.GroupBox1.Controls.Add(Me.Btn_LatchDisable)
        Me.GroupBox1.Controls.Add(Me.Btn_LatchEnable)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'TextBox_LatchDataCommand
        '
        resources.ApplyResources(Me.TextBox_LatchDataCommand, "TextBox_LatchDataCommand")
        Me.TextBox_LatchDataCommand.Name = "TextBox_LatchDataCommand"
        Me.TextBox_LatchDataCommand.ReadOnly = True
        '
        'Btn_LatchDisable
        '
        resources.ApplyResources(Me.Btn_LatchDisable, "Btn_LatchDisable")
        Me.Btn_LatchDisable.Name = "Btn_LatchDisable"
        Me.Btn_LatchDisable.UseVisualStyleBackColor = True
        '
        'Btn_LatchEnable
        '
        resources.ApplyResources(Me.Btn_LatchEnable, "Btn_LatchEnable")
        Me.Btn_LatchEnable.Name = "Btn_LatchEnable"
        Me.Btn_LatchEnable.UseVisualStyleBackColor = True
        '
        'ButtonService
        '
        resources.ApplyResources(Me.ButtonService, "ButtonService")
        Me.ButtonService.Name = "ButtonService"
        Me.ButtonService.UseVisualStyleBackColor = True
        '
        'lblStepPartTitle1
        '
        Me.lblStepPartTitle1.BackColor = System.Drawing.Color.Cornsilk
        Me.lblStepPartTitle1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblStepPartTitle1, "lblStepPartTitle1")
        Me.lblStepPartTitle1.Name = "lblStepPartTitle1"
        '
        'lblStepPartTitle0
        '
        Me.lblStepPartTitle0.BackColor = System.Drawing.Color.Cornsilk
        Me.lblStepPartTitle0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblStepPartTitle0, "lblStepPartTitle0")
        Me.lblStepPartTitle0.Name = "lblStepPartTitle0"
        '
        'cboMStepName
        '
        Me.cboMStepName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMStepName.FormattingEnabled = True
        resources.ApplyResources(Me.cboMStepName, "cboMStepName")
        Me.cboMStepName.Name = "cboMStepName"
        Me.cboMStepName.Sorted = True
        '
        'cboMStepPart
        '
        Me.cboMStepPart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMStepPart.FormattingEnabled = True
        resources.ApplyResources(Me.cboMStepPart, "cboMStepPart")
        Me.cboMStepPart.Name = "cboMStepPart"
        Me.cboMStepPart.Sorted = True
        '
        'gboxStepControl
        '
        Me.gboxStepControl.Controls.Add(Me.lblMotorErrorStatus)
        Me.gboxStepControl.Controls.Add(Me.Label9)
        Me.gboxStepControl.Controls.Add(Me.lblMotionStatus)
        Me.gboxStepControl.Controls.Add(Me.lblMotionVelocity)
        Me.gboxStepControl.Controls.Add(Me.gboxStatus)
        Me.gboxStepControl.Controls.Add(Me.gboxMotion)
        Me.gboxStepControl.Controls.Add(Me.gboxPosition)
        Me.gboxStepControl.Controls.Add(Me.gboxVelocityProfile)
        resources.ApplyResources(Me.gboxStepControl, "gboxStepControl")
        Me.gboxStepControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.gboxStepControl.Name = "gboxStepControl"
        Me.gboxStepControl.TabStop = False
        '
        'lblMotorErrorStatus
        '
        Me.lblMotorErrorStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblMotorErrorStatus, "lblMotorErrorStatus")
        Me.lblMotorErrorStatus.Name = "lblMotorErrorStatus"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'lblMotionStatus
        '
        Me.lblMotionStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblMotionStatus, "lblMotionStatus")
        Me.lblMotionStatus.Name = "lblMotionStatus"
        '
        'lblMotionVelocity
        '
        Me.lblMotionVelocity.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblMotionVelocity, "lblMotionVelocity")
        Me.lblMotionVelocity.Name = "lblMotionVelocity"
        '
        'gboxStatus
        '
        Me.gboxStatus.Controls.Add(Me.shpStatus15)
        Me.gboxStatus.Controls.Add(Me.shpStatus14)
        Me.gboxStatus.Controls.Add(Me.shpStatus13)
        Me.gboxStatus.Controls.Add(Me.shpStatus12)
        Me.gboxStatus.Controls.Add(Me.shpStatus11)
        Me.gboxStatus.Controls.Add(Me.shpStatus10)
        Me.gboxStatus.Controls.Add(Me.shpStatus9)
        Me.gboxStatus.Controls.Add(Me.shpStatus8)
        Me.gboxStatus.Controls.Add(Me.shpStatus7)
        Me.gboxStatus.Controls.Add(Me.shpStatus6)
        Me.gboxStatus.Controls.Add(Me.shpStatus5)
        Me.gboxStatus.Controls.Add(Me.shpStatus4)
        Me.gboxStatus.Controls.Add(Me.shpStatus3)
        Me.gboxStatus.Controls.Add(Me.shpStatus2)
        Me.gboxStatus.Controls.Add(Me.shpStatus1)
        Me.gboxStatus.Controls.Add(Me.shpStatus0)
        Me.gboxStatus.Controls.Add(Me.lblStatus15)
        Me.gboxStatus.Controls.Add(Me.lblStatus14)
        Me.gboxStatus.Controls.Add(Me.lblStatus13)
        Me.gboxStatus.Controls.Add(Me.lblStatus12)
        Me.gboxStatus.Controls.Add(Me.lblStatus11)
        Me.gboxStatus.Controls.Add(Me.lblStatus10)
        Me.gboxStatus.Controls.Add(Me.lblStatus9)
        Me.gboxStatus.Controls.Add(Me.lblStatus8)
        Me.gboxStatus.Controls.Add(Me.lblStatus7)
        Me.gboxStatus.Controls.Add(Me.lblStatus6)
        Me.gboxStatus.Controls.Add(Me.lblStatus5)
        Me.gboxStatus.Controls.Add(Me.lblStatus4)
        Me.gboxStatus.Controls.Add(Me.lblStatus3)
        Me.gboxStatus.Controls.Add(Me.lblStatus2)
        Me.gboxStatus.Controls.Add(Me.lblStatus1)
        Me.gboxStatus.Controls.Add(Me.lblStatus0)
        Me.gboxStatus.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me.gboxStatus, "gboxStatus")
        Me.gboxStatus.Name = "gboxStatus"
        Me.gboxStatus.TabStop = False
        '
        'shpStatus15
        '
        Me.shpStatus15.BackColor = System.Drawing.Color.White
        Me.shpStatus15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus15, "shpStatus15")
        Me.shpStatus15.Name = "shpStatus15"
        '
        'shpStatus14
        '
        Me.shpStatus14.BackColor = System.Drawing.Color.White
        Me.shpStatus14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus14, "shpStatus14")
        Me.shpStatus14.Name = "shpStatus14"
        '
        'shpStatus13
        '
        Me.shpStatus13.BackColor = System.Drawing.Color.White
        Me.shpStatus13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus13, "shpStatus13")
        Me.shpStatus13.Name = "shpStatus13"
        '
        'shpStatus12
        '
        Me.shpStatus12.BackColor = System.Drawing.Color.White
        Me.shpStatus12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus12, "shpStatus12")
        Me.shpStatus12.Name = "shpStatus12"
        '
        'shpStatus11
        '
        Me.shpStatus11.BackColor = System.Drawing.Color.White
        Me.shpStatus11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus11, "shpStatus11")
        Me.shpStatus11.Name = "shpStatus11"
        '
        'shpStatus10
        '
        Me.shpStatus10.BackColor = System.Drawing.Color.White
        Me.shpStatus10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus10, "shpStatus10")
        Me.shpStatus10.Name = "shpStatus10"
        '
        'shpStatus9
        '
        Me.shpStatus9.BackColor = System.Drawing.Color.White
        Me.shpStatus9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus9, "shpStatus9")
        Me.shpStatus9.Name = "shpStatus9"
        '
        'shpStatus8
        '
        Me.shpStatus8.BackColor = System.Drawing.Color.White
        Me.shpStatus8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus8, "shpStatus8")
        Me.shpStatus8.Name = "shpStatus8"
        '
        'shpStatus7
        '
        Me.shpStatus7.BackColor = System.Drawing.Color.White
        Me.shpStatus7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus7, "shpStatus7")
        Me.shpStatus7.Name = "shpStatus7"
        '
        'shpStatus6
        '
        Me.shpStatus6.BackColor = System.Drawing.Color.White
        Me.shpStatus6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus6, "shpStatus6")
        Me.shpStatus6.Name = "shpStatus6"
        '
        'shpStatus5
        '
        Me.shpStatus5.BackColor = System.Drawing.Color.White
        Me.shpStatus5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus5, "shpStatus5")
        Me.shpStatus5.Name = "shpStatus5"
        '
        'shpStatus4
        '
        Me.shpStatus4.BackColor = System.Drawing.Color.White
        Me.shpStatus4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus4, "shpStatus4")
        Me.shpStatus4.Name = "shpStatus4"
        '
        'shpStatus3
        '
        Me.shpStatus3.BackColor = System.Drawing.Color.White
        Me.shpStatus3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus3, "shpStatus3")
        Me.shpStatus3.Name = "shpStatus3"
        '
        'shpStatus2
        '
        Me.shpStatus2.BackColor = System.Drawing.Color.White
        Me.shpStatus2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus2, "shpStatus2")
        Me.shpStatus2.Name = "shpStatus2"
        '
        'shpStatus1
        '
        Me.shpStatus1.BackColor = System.Drawing.Color.White
        Me.shpStatus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus1, "shpStatus1")
        Me.shpStatus1.Name = "shpStatus1"
        '
        'shpStatus0
        '
        Me.shpStatus0.BackColor = System.Drawing.Color.White
        Me.shpStatus0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.shpStatus0, "shpStatus0")
        Me.shpStatus0.Name = "shpStatus0"
        '
        'lblStatus15
        '
        resources.ApplyResources(Me.lblStatus15, "lblStatus15")
        Me.lblStatus15.Name = "lblStatus15"
        '
        'lblStatus14
        '
        resources.ApplyResources(Me.lblStatus14, "lblStatus14")
        Me.lblStatus14.Name = "lblStatus14"
        '
        'lblStatus13
        '
        resources.ApplyResources(Me.lblStatus13, "lblStatus13")
        Me.lblStatus13.Name = "lblStatus13"
        '
        'lblStatus12
        '
        resources.ApplyResources(Me.lblStatus12, "lblStatus12")
        Me.lblStatus12.Name = "lblStatus12"
        '
        'lblStatus11
        '
        resources.ApplyResources(Me.lblStatus11, "lblStatus11")
        Me.lblStatus11.Name = "lblStatus11"
        '
        'lblStatus10
        '
        resources.ApplyResources(Me.lblStatus10, "lblStatus10")
        Me.lblStatus10.Name = "lblStatus10"
        '
        'lblStatus9
        '
        resources.ApplyResources(Me.lblStatus9, "lblStatus9")
        Me.lblStatus9.Name = "lblStatus9"
        '
        'lblStatus8
        '
        resources.ApplyResources(Me.lblStatus8, "lblStatus8")
        Me.lblStatus8.Name = "lblStatus8"
        '
        'lblStatus7
        '
        resources.ApplyResources(Me.lblStatus7, "lblStatus7")
        Me.lblStatus7.Name = "lblStatus7"
        '
        'lblStatus6
        '
        resources.ApplyResources(Me.lblStatus6, "lblStatus6")
        Me.lblStatus6.Name = "lblStatus6"
        '
        'lblStatus5
        '
        resources.ApplyResources(Me.lblStatus5, "lblStatus5")
        Me.lblStatus5.Name = "lblStatus5"
        '
        'lblStatus4
        '
        resources.ApplyResources(Me.lblStatus4, "lblStatus4")
        Me.lblStatus4.Name = "lblStatus4"
        '
        'lblStatus3
        '
        resources.ApplyResources(Me.lblStatus3, "lblStatus3")
        Me.lblStatus3.Name = "lblStatus3"
        '
        'lblStatus2
        '
        resources.ApplyResources(Me.lblStatus2, "lblStatus2")
        Me.lblStatus2.Name = "lblStatus2"
        '
        'lblStatus1
        '
        resources.ApplyResources(Me.lblStatus1, "lblStatus1")
        Me.lblStatus1.Name = "lblStatus1"
        '
        'lblStatus0
        '
        resources.ApplyResources(Me.lblStatus0, "lblStatus0")
        Me.lblStatus0.Name = "lblStatus0"
        '
        'gboxMotion
        '
        Me.gboxMotion.Controls.Add(Me.btnMotion4)
        Me.gboxMotion.Controls.Add(Me.btnMotion3)
        Me.gboxMotion.Controls.Add(Me.btnMotion2)
        Me.gboxMotion.Controls.Add(Me.btnMotion1)
        Me.gboxMotion.Controls.Add(Me.btnMotion0)
        Me.gboxMotion.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me.gboxMotion, "gboxMotion")
        Me.gboxMotion.Name = "gboxMotion"
        Me.gboxMotion.TabStop = False
        '
        'btnMotion4
        '
        resources.ApplyResources(Me.btnMotion4, "btnMotion4")
        Me.btnMotion4.Name = "btnMotion4"
        Me.btnMotion4.UseVisualStyleBackColor = True
        '
        'btnMotion3
        '
        resources.ApplyResources(Me.btnMotion3, "btnMotion3")
        Me.btnMotion3.Name = "btnMotion3"
        Me.btnMotion3.UseVisualStyleBackColor = True
        '
        'btnMotion2
        '
        Me.btnMotion2.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.btnMotion2, "btnMotion2")
        Me.btnMotion2.Name = "btnMotion2"
        Me.btnMotion2.UseVisualStyleBackColor = True
        '
        'btnMotion1
        '
        resources.ApplyResources(Me.btnMotion1, "btnMotion1")
        Me.btnMotion1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnMotion1.Name = "btnMotion1"
        Me.btnMotion1.UseVisualStyleBackColor = True
        '
        'btnMotion0
        '
        resources.ApplyResources(Me.btnMotion0, "btnMotion0")
        Me.btnMotion0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnMotion0.Name = "btnMotion0"
        Me.btnMotion0.UseVisualStyleBackColor = True
        '
        'gboxPosition
        '
        Me.gboxPosition.Controls.Add(Me.btnGetPosition)
        Me.gboxPosition.Controls.Add(Me.btnPositionReset)
        Me.gboxPosition.Controls.Add(Me.lblPosition2)
        Me.gboxPosition.Controls.Add(Me.lblPosition1)
        Me.gboxPosition.Controls.Add(Me.lblPosition0)
        Me.gboxPosition.Controls.Add(Me.lblPositionTitle2)
        Me.gboxPosition.Controls.Add(Me.lblPositionTitle1)
        Me.gboxPosition.Controls.Add(Me.lblPositionTitle0)
        Me.gboxPosition.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me.gboxPosition, "gboxPosition")
        Me.gboxPosition.Name = "gboxPosition"
        Me.gboxPosition.TabStop = False
        '
        'btnGetPosition
        '
        resources.ApplyResources(Me.btnGetPosition, "btnGetPosition")
        Me.btnGetPosition.Name = "btnGetPosition"
        Me.btnGetPosition.UseVisualStyleBackColor = True
        '
        'btnPositionReset
        '
        resources.ApplyResources(Me.btnPositionReset, "btnPositionReset")
        Me.btnPositionReset.Name = "btnPositionReset"
        Me.btnPositionReset.UseVisualStyleBackColor = True
        '
        'lblPosition2
        '
        Me.lblPosition2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblPosition2, "lblPosition2")
        Me.lblPosition2.Name = "lblPosition2"
        '
        'lblPosition1
        '
        Me.lblPosition1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblPosition1, "lblPosition1")
        Me.lblPosition1.Name = "lblPosition1"
        '
        'lblPosition0
        '
        Me.lblPosition0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblPosition0, "lblPosition0")
        Me.lblPosition0.Name = "lblPosition0"
        '
        'lblPositionTitle2
        '
        resources.ApplyResources(Me.lblPositionTitle2, "lblPositionTitle2")
        Me.lblPositionTitle2.Name = "lblPositionTitle2"
        '
        'lblPositionTitle1
        '
        resources.ApplyResources(Me.lblPositionTitle1, "lblPositionTitle1")
        Me.lblPositionTitle1.Name = "lblPositionTitle1"
        '
        'lblPositionTitle0
        '
        resources.ApplyResources(Me.lblPositionTitle0, "lblPositionTitle0")
        Me.lblPositionTitle0.Name = "lblPositionTitle0"
        '
        'gboxVelocityProfile
        '
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileTitle0)
        Me.gboxVelocityProfile.Controls.Add(Me.gboxMoveMode)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileUnit3)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileUnit2)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileUnit1)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileUnit0)
        Me.gboxVelocityProfile.Controls.Add(Me.txtVelocityProfile3)
        Me.gboxVelocityProfile.Controls.Add(Me.txtVelocityProfile2)
        Me.gboxVelocityProfile.Controls.Add(Me.txtVelocityProfile1)
        Me.gboxVelocityProfile.Controls.Add(Me.txtVelocityProfile0)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileTitle3)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileTitle2)
        Me.gboxVelocityProfile.Controls.Add(Me.lblVelocityProfileTitle1)
        Me.gboxVelocityProfile.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me.gboxVelocityProfile, "gboxVelocityProfile")
        Me.gboxVelocityProfile.Name = "gboxVelocityProfile"
        Me.gboxVelocityProfile.TabStop = False
        '
        'lblVelocityProfileTitle0
        '
        resources.ApplyResources(Me.lblVelocityProfileTitle0, "lblVelocityProfileTitle0")
        Me.lblVelocityProfileTitle0.Name = "lblVelocityProfileTitle0"
        '
        'gboxMoveMode
        '
        Me.gboxMoveMode.Controls.Add(Me.rdoMoveMode1)
        Me.gboxMoveMode.Controls.Add(Me.rdoMoveMode0)
        Me.gboxMoveMode.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(Me.gboxMoveMode, "gboxMoveMode")
        Me.gboxMoveMode.Name = "gboxMoveMode"
        Me.gboxMoveMode.TabStop = False
        '
        'rdoMoveMode1
        '
        resources.ApplyResources(Me.rdoMoveMode1, "rdoMoveMode1")
        Me.rdoMoveMode1.Name = "rdoMoveMode1"
        Me.rdoMoveMode1.UseVisualStyleBackColor = True
        '
        'rdoMoveMode0
        '
        resources.ApplyResources(Me.rdoMoveMode0, "rdoMoveMode0")
        Me.rdoMoveMode0.Checked = True
        Me.rdoMoveMode0.Name = "rdoMoveMode0"
        Me.rdoMoveMode0.TabStop = True
        Me.rdoMoveMode0.UseVisualStyleBackColor = True
        '
        'lblVelocityProfileUnit3
        '
        resources.ApplyResources(Me.lblVelocityProfileUnit3, "lblVelocityProfileUnit3")
        Me.lblVelocityProfileUnit3.Name = "lblVelocityProfileUnit3"
        '
        'lblVelocityProfileUnit2
        '
        resources.ApplyResources(Me.lblVelocityProfileUnit2, "lblVelocityProfileUnit2")
        Me.lblVelocityProfileUnit2.Name = "lblVelocityProfileUnit2"
        '
        'lblVelocityProfileUnit1
        '
        resources.ApplyResources(Me.lblVelocityProfileUnit1, "lblVelocityProfileUnit1")
        Me.lblVelocityProfileUnit1.Name = "lblVelocityProfileUnit1"
        '
        'lblVelocityProfileUnit0
        '
        resources.ApplyResources(Me.lblVelocityProfileUnit0, "lblVelocityProfileUnit0")
        Me.lblVelocityProfileUnit0.Name = "lblVelocityProfileUnit0"
        '
        'txtVelocityProfile3
        '
        resources.ApplyResources(Me.txtVelocityProfile3, "txtVelocityProfile3")
        Me.txtVelocityProfile3.Name = "txtVelocityProfile3"
        '
        'txtVelocityProfile2
        '
        resources.ApplyResources(Me.txtVelocityProfile2, "txtVelocityProfile2")
        Me.txtVelocityProfile2.Name = "txtVelocityProfile2"
        '
        'txtVelocityProfile1
        '
        resources.ApplyResources(Me.txtVelocityProfile1, "txtVelocityProfile1")
        Me.txtVelocityProfile1.Name = "txtVelocityProfile1"
        '
        'txtVelocityProfile0
        '
        resources.ApplyResources(Me.txtVelocityProfile0, "txtVelocityProfile0")
        Me.txtVelocityProfile0.Name = "txtVelocityProfile0"
        '
        'lblVelocityProfileTitle3
        '
        resources.ApplyResources(Me.lblVelocityProfileTitle3, "lblVelocityProfileTitle3")
        Me.lblVelocityProfileTitle3.Name = "lblVelocityProfileTitle3"
        '
        'lblVelocityProfileTitle2
        '
        resources.ApplyResources(Me.lblVelocityProfileTitle2, "lblVelocityProfileTitle2")
        Me.lblVelocityProfileTitle2.Name = "lblVelocityProfileTitle2"
        '
        'lblVelocityProfileTitle1
        '
        resources.ApplyResources(Me.lblVelocityProfileTitle1, "lblVelocityProfileTitle1")
        Me.lblVelocityProfileTitle1.Name = "lblVelocityProfileTitle1"
        '
        'gboxRecord
        '
        Me.gboxRecord.BackColor = System.Drawing.SystemColors.Control
        Me.gboxRecord.Controls.Add(Me.btnGetVelocityProfile)
        Me.gboxRecord.Controls.Add(Me.btnRecord2)
        Me.gboxRecord.Controls.Add(Me.btnMoveToMotionPosition)
        Me.gboxRecord.Controls.Add(Me.msgStepRecord)
        resources.ApplyResources(Me.gboxRecord, "gboxRecord")
        Me.gboxRecord.ForeColor = System.Drawing.SystemColors.ControlText
        Me.gboxRecord.Name = "gboxRecord"
        Me.gboxRecord.TabStop = False
        '
        'btnGetVelocityProfile
        '
        Me.btnGetVelocityProfile.BackColor = System.Drawing.SystemColors.ControlLight
        resources.ApplyResources(Me.btnGetVelocityProfile, "btnGetVelocityProfile")
        Me.btnGetVelocityProfile.Name = "btnGetVelocityProfile"
        Me.btnGetVelocityProfile.UseVisualStyleBackColor = True
        '
        'btnRecord2
        '
        Me.btnRecord2.BackColor = System.Drawing.SystemColors.Control
        resources.ApplyResources(Me.btnRecord2, "btnRecord2")
        Me.btnRecord2.Name = "btnRecord2"
        Me.btnRecord2.UseVisualStyleBackColor = True
        '
        'btnMoveToMotionPosition
        '
        Me.btnMoveToMotionPosition.BackColor = System.Drawing.SystemColors.Control
        resources.ApplyResources(Me.btnMoveToMotionPosition, "btnMoveToMotionPosition")
        Me.btnMoveToMotionPosition.Name = "btnMoveToMotionPosition"
        Me.btnMoveToMotionPosition.UseVisualStyleBackColor = True
        '
        'msgStepRecord
        '
        Me.msgStepRecord.AllowUserToResizeColumns = False
        Me.msgStepRecord.AllowUserToResizeRows = False
        Me.msgStepRecord.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.msgStepRecord.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Times New Roman", 12.0!)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.msgStepRecord.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.msgStepRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.msgStepRecord.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5, Me.Column6, Me.Column7, Me.Column8})
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Times New Roman", 12.0!)
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.msgStepRecord.DefaultCellStyle = DataGridViewCellStyle3
        resources.ApplyResources(Me.msgStepRecord, "msgStepRecord")
        Me.msgStepRecord.Name = "msgStepRecord"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Times New Roman", 12.0!)
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.msgStepRecord.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.msgStepRecord.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
        Me.msgStepRecord.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.msgStepRecord.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.msgStepRecord.RowTemplate.Height = 24
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomRight
        Me.Column1.DefaultCellStyle = DataGridViewCellStyle2
        Me.Column1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        resources.ApplyResources(Me.Column1, "Column1")
        Me.Column1.Name = "Column1"
        Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'Column2
        '
        resources.ApplyResources(Me.Column2, "Column2")
        Me.Column2.Name = "Column2"
        Me.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Column3
        '
        resources.ApplyResources(Me.Column3, "Column3")
        Me.Column3.Name = "Column3"
        Me.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Column4
        '
        resources.ApplyResources(Me.Column4, "Column4")
        Me.Column4.Name = "Column4"
        '
        'Column5
        '
        resources.ApplyResources(Me.Column5, "Column5")
        Me.Column5.Name = "Column5"
        '
        'Column6
        '
        resources.ApplyResources(Me.Column6, "Column6")
        Me.Column6.Name = "Column6"
        '
        'Column7
        '
        resources.ApplyResources(Me.Column7, "Column7")
        Me.Column7.Name = "Column7"
        '
        'Column8
        '
        resources.ApplyResources(Me.Column8, "Column8")
        Me.Column8.Name = "Column8"
        '
        'tabIOTable
        '
        resources.ApplyResources(Me.tabIOTable, "tabIOTable")
        Me.tabIOTable.BackColor = System.Drawing.SystemColors.Control
        Me.tabIOTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.tabIOTable.Controls.Add(Me.userControlIOTable1)
        Me.tabIOTable.Name = "tabIOTable"
        '
        'userControlIOTable1
        '
        resources.ApplyResources(Me.userControlIOTable1, "userControlIOTable1")
        Me.userControlIOTable1.Name = "userControlIOTable1"
        '
        'tabRemoteTable
        '
        resources.ApplyResources(Me.tabRemoteTable, "tabRemoteTable")
        Me.tabRemoteTable.BackColor = System.Drawing.SystemColors.Control
        Me.tabRemoteTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.tabRemoteTable.Controls.Add(Me.gboxAMONet1)
        Me.tabRemoteTable.Controls.Add(Me.gboxAMONet0)
        Me.tabRemoteTable.Name = "tabRemoteTable"
        '
        'gboxAMONet1
        '
        Me.gboxAMONet1.BackColor = System.Drawing.SystemColors.Control
        Me.gboxAMONet1.Controls.Add(Me.btnStartRing1)
        Me.gboxAMONet1.Controls.Add(Me.lblAMONetRing1SubType)
        Me.gboxAMONet1.Controls.Add(Me.lblAMONetRing1Type)
        Me.gboxAMONet1.Controls.Add(Me.lblAMONetRing1SubTypeTitle)
        Me.gboxAMONet1.Controls.Add(Me.lblAMONetRing1TypeTitle)
        Me.gboxAMONet1.Controls.Add(Me.lblErrorSlave1)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP63)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP62)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP61)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP60)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP59)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP58)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP57)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP56)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP55)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP54)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP53)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP52)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP51)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP50)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP49)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP48)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP47)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP46)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP45)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP44)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP43)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP42)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP41)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP40)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP39)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP38)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP37)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP36)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP35)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP34)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP33)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP32)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP31)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP30)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP29)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP28)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP27)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP26)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP25)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP24)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP23)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP22)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP21)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP20)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP19)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP18)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP17)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP16)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP15)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP14)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP13)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP12)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP11)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP10)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP9)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP8)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP7)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP6)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP5)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP4)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP3)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP2)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP1)
        Me.gboxAMONet1.Controls.Add(Me.lblRing1_DeviceIP0)
        resources.ApplyResources(Me.gboxAMONet1, "gboxAMONet1")
        Me.gboxAMONet1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.gboxAMONet1.Name = "gboxAMONet1"
        Me.gboxAMONet1.TabStop = False
        '
        'btnStartRing1
        '
        resources.ApplyResources(Me.btnStartRing1, "btnStartRing1")
        Me.btnStartRing1.Name = "btnStartRing1"
        Me.btnStartRing1.UseVisualStyleBackColor = True
        '
        'lblAMONetRing1SubType
        '
        Me.lblAMONetRing1SubType.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblAMONetRing1SubType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing1SubType, "lblAMONetRing1SubType")
        Me.lblAMONetRing1SubType.Name = "lblAMONetRing1SubType"
        '
        'lblAMONetRing1Type
        '
        Me.lblAMONetRing1Type.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblAMONetRing1Type.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing1Type, "lblAMONetRing1Type")
        Me.lblAMONetRing1Type.Name = "lblAMONetRing1Type"
        '
        'lblAMONetRing1SubTypeTitle
        '
        Me.lblAMONetRing1SubTypeTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAMONetRing1SubTypeTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing1SubTypeTitle, "lblAMONetRing1SubTypeTitle")
        Me.lblAMONetRing1SubTypeTitle.Name = "lblAMONetRing1SubTypeTitle"
        '
        'lblAMONetRing1TypeTitle
        '
        Me.lblAMONetRing1TypeTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAMONetRing1TypeTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing1TypeTitle, "lblAMONetRing1TypeTitle")
        Me.lblAMONetRing1TypeTitle.Name = "lblAMONetRing1TypeTitle"
        '
        'lblErrorSlave1
        '
        Me.lblErrorSlave1.BackColor = System.Drawing.Color.SandyBrown
        Me.lblErrorSlave1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblErrorSlave1, "lblErrorSlave1")
        Me.lblErrorSlave1.Name = "lblErrorSlave1"
        '
        'lblRing1_DeviceIP63
        '
        Me.lblRing1_DeviceIP63.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP63.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP63, "lblRing1_DeviceIP63")
        Me.lblRing1_DeviceIP63.Name = "lblRing1_DeviceIP63"
        '
        'lblRing1_DeviceIP62
        '
        Me.lblRing1_DeviceIP62.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP62.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP62, "lblRing1_DeviceIP62")
        Me.lblRing1_DeviceIP62.Name = "lblRing1_DeviceIP62"
        '
        'lblRing1_DeviceIP61
        '
        Me.lblRing1_DeviceIP61.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP61.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP61, "lblRing1_DeviceIP61")
        Me.lblRing1_DeviceIP61.Name = "lblRing1_DeviceIP61"
        '
        'lblRing1_DeviceIP60
        '
        Me.lblRing1_DeviceIP60.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP60.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP60, "lblRing1_DeviceIP60")
        Me.lblRing1_DeviceIP60.Name = "lblRing1_DeviceIP60"
        '
        'lblRing1_DeviceIP59
        '
        Me.lblRing1_DeviceIP59.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP59.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP59, "lblRing1_DeviceIP59")
        Me.lblRing1_DeviceIP59.Name = "lblRing1_DeviceIP59"
        '
        'lblRing1_DeviceIP58
        '
        Me.lblRing1_DeviceIP58.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP58.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP58, "lblRing1_DeviceIP58")
        Me.lblRing1_DeviceIP58.Name = "lblRing1_DeviceIP58"
        '
        'lblRing1_DeviceIP57
        '
        Me.lblRing1_DeviceIP57.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP57, "lblRing1_DeviceIP57")
        Me.lblRing1_DeviceIP57.Name = "lblRing1_DeviceIP57"
        '
        'lblRing1_DeviceIP56
        '
        Me.lblRing1_DeviceIP56.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP56.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP56, "lblRing1_DeviceIP56")
        Me.lblRing1_DeviceIP56.Name = "lblRing1_DeviceIP56"
        '
        'lblRing1_DeviceIP55
        '
        Me.lblRing1_DeviceIP55.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP55.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP55, "lblRing1_DeviceIP55")
        Me.lblRing1_DeviceIP55.Name = "lblRing1_DeviceIP55"
        '
        'lblRing1_DeviceIP54
        '
        Me.lblRing1_DeviceIP54.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP54.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP54, "lblRing1_DeviceIP54")
        Me.lblRing1_DeviceIP54.Name = "lblRing1_DeviceIP54"
        '
        'lblRing1_DeviceIP53
        '
        Me.lblRing1_DeviceIP53.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP53.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP53, "lblRing1_DeviceIP53")
        Me.lblRing1_DeviceIP53.Name = "lblRing1_DeviceIP53"
        '
        'lblRing1_DeviceIP52
        '
        Me.lblRing1_DeviceIP52.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP52.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP52, "lblRing1_DeviceIP52")
        Me.lblRing1_DeviceIP52.Name = "lblRing1_DeviceIP52"
        '
        'lblRing1_DeviceIP51
        '
        Me.lblRing1_DeviceIP51.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP51.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP51, "lblRing1_DeviceIP51")
        Me.lblRing1_DeviceIP51.Name = "lblRing1_DeviceIP51"
        '
        'lblRing1_DeviceIP50
        '
        Me.lblRing1_DeviceIP50.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP50.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP50, "lblRing1_DeviceIP50")
        Me.lblRing1_DeviceIP50.Name = "lblRing1_DeviceIP50"
        '
        'lblRing1_DeviceIP49
        '
        Me.lblRing1_DeviceIP49.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP49.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP49, "lblRing1_DeviceIP49")
        Me.lblRing1_DeviceIP49.Name = "lblRing1_DeviceIP49"
        '
        'lblRing1_DeviceIP48
        '
        Me.lblRing1_DeviceIP48.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP48.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP48, "lblRing1_DeviceIP48")
        Me.lblRing1_DeviceIP48.Name = "lblRing1_DeviceIP48"
        '
        'lblRing1_DeviceIP47
        '
        Me.lblRing1_DeviceIP47.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP47.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP47, "lblRing1_DeviceIP47")
        Me.lblRing1_DeviceIP47.Name = "lblRing1_DeviceIP47"
        '
        'lblRing1_DeviceIP46
        '
        Me.lblRing1_DeviceIP46.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP46.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP46, "lblRing1_DeviceIP46")
        Me.lblRing1_DeviceIP46.Name = "lblRing1_DeviceIP46"
        '
        'lblRing1_DeviceIP45
        '
        Me.lblRing1_DeviceIP45.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP45.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP45, "lblRing1_DeviceIP45")
        Me.lblRing1_DeviceIP45.Name = "lblRing1_DeviceIP45"
        '
        'lblRing1_DeviceIP44
        '
        Me.lblRing1_DeviceIP44.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP44.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP44, "lblRing1_DeviceIP44")
        Me.lblRing1_DeviceIP44.Name = "lblRing1_DeviceIP44"
        '
        'lblRing1_DeviceIP43
        '
        Me.lblRing1_DeviceIP43.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP43.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP43, "lblRing1_DeviceIP43")
        Me.lblRing1_DeviceIP43.Name = "lblRing1_DeviceIP43"
        '
        'lblRing1_DeviceIP42
        '
        Me.lblRing1_DeviceIP42.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP42.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP42, "lblRing1_DeviceIP42")
        Me.lblRing1_DeviceIP42.Name = "lblRing1_DeviceIP42"
        '
        'lblRing1_DeviceIP41
        '
        Me.lblRing1_DeviceIP41.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP41.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP41, "lblRing1_DeviceIP41")
        Me.lblRing1_DeviceIP41.Name = "lblRing1_DeviceIP41"
        '
        'lblRing1_DeviceIP40
        '
        Me.lblRing1_DeviceIP40.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP40.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP40, "lblRing1_DeviceIP40")
        Me.lblRing1_DeviceIP40.Name = "lblRing1_DeviceIP40"
        '
        'lblRing1_DeviceIP39
        '
        Me.lblRing1_DeviceIP39.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP39.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP39, "lblRing1_DeviceIP39")
        Me.lblRing1_DeviceIP39.Name = "lblRing1_DeviceIP39"
        '
        'lblRing1_DeviceIP38
        '
        Me.lblRing1_DeviceIP38.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP38.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP38, "lblRing1_DeviceIP38")
        Me.lblRing1_DeviceIP38.Name = "lblRing1_DeviceIP38"
        '
        'lblRing1_DeviceIP37
        '
        Me.lblRing1_DeviceIP37.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP37.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP37, "lblRing1_DeviceIP37")
        Me.lblRing1_DeviceIP37.Name = "lblRing1_DeviceIP37"
        '
        'lblRing1_DeviceIP36
        '
        Me.lblRing1_DeviceIP36.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP36.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP36, "lblRing1_DeviceIP36")
        Me.lblRing1_DeviceIP36.Name = "lblRing1_DeviceIP36"
        '
        'lblRing1_DeviceIP35
        '
        Me.lblRing1_DeviceIP35.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP35.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP35, "lblRing1_DeviceIP35")
        Me.lblRing1_DeviceIP35.Name = "lblRing1_DeviceIP35"
        '
        'lblRing1_DeviceIP34
        '
        Me.lblRing1_DeviceIP34.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP34.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP34, "lblRing1_DeviceIP34")
        Me.lblRing1_DeviceIP34.Name = "lblRing1_DeviceIP34"
        '
        'lblRing1_DeviceIP33
        '
        Me.lblRing1_DeviceIP33.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP33.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP33, "lblRing1_DeviceIP33")
        Me.lblRing1_DeviceIP33.Name = "lblRing1_DeviceIP33"
        '
        'lblRing1_DeviceIP32
        '
        Me.lblRing1_DeviceIP32.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP32.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP32, "lblRing1_DeviceIP32")
        Me.lblRing1_DeviceIP32.Name = "lblRing1_DeviceIP32"
        '
        'lblRing1_DeviceIP31
        '
        Me.lblRing1_DeviceIP31.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP31.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP31, "lblRing1_DeviceIP31")
        Me.lblRing1_DeviceIP31.Name = "lblRing1_DeviceIP31"
        '
        'lblRing1_DeviceIP30
        '
        Me.lblRing1_DeviceIP30.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP30.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP30, "lblRing1_DeviceIP30")
        Me.lblRing1_DeviceIP30.Name = "lblRing1_DeviceIP30"
        '
        'lblRing1_DeviceIP29
        '
        Me.lblRing1_DeviceIP29.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP29.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP29, "lblRing1_DeviceIP29")
        Me.lblRing1_DeviceIP29.Name = "lblRing1_DeviceIP29"
        '
        'lblRing1_DeviceIP28
        '
        Me.lblRing1_DeviceIP28.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP28.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP28, "lblRing1_DeviceIP28")
        Me.lblRing1_DeviceIP28.Name = "lblRing1_DeviceIP28"
        '
        'lblRing1_DeviceIP27
        '
        Me.lblRing1_DeviceIP27.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP27, "lblRing1_DeviceIP27")
        Me.lblRing1_DeviceIP27.Name = "lblRing1_DeviceIP27"
        '
        'lblRing1_DeviceIP26
        '
        Me.lblRing1_DeviceIP26.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP26.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP26, "lblRing1_DeviceIP26")
        Me.lblRing1_DeviceIP26.Name = "lblRing1_DeviceIP26"
        '
        'lblRing1_DeviceIP25
        '
        Me.lblRing1_DeviceIP25.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP25.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP25, "lblRing1_DeviceIP25")
        Me.lblRing1_DeviceIP25.Name = "lblRing1_DeviceIP25"
        '
        'lblRing1_DeviceIP24
        '
        Me.lblRing1_DeviceIP24.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP24, "lblRing1_DeviceIP24")
        Me.lblRing1_DeviceIP24.Name = "lblRing1_DeviceIP24"
        '
        'lblRing1_DeviceIP23
        '
        Me.lblRing1_DeviceIP23.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP23, "lblRing1_DeviceIP23")
        Me.lblRing1_DeviceIP23.Name = "lblRing1_DeviceIP23"
        '
        'lblRing1_DeviceIP22
        '
        Me.lblRing1_DeviceIP22.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP22, "lblRing1_DeviceIP22")
        Me.lblRing1_DeviceIP22.Name = "lblRing1_DeviceIP22"
        '
        'lblRing1_DeviceIP21
        '
        Me.lblRing1_DeviceIP21.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP21, "lblRing1_DeviceIP21")
        Me.lblRing1_DeviceIP21.Name = "lblRing1_DeviceIP21"
        '
        'lblRing1_DeviceIP20
        '
        Me.lblRing1_DeviceIP20.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP20.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP20, "lblRing1_DeviceIP20")
        Me.lblRing1_DeviceIP20.Name = "lblRing1_DeviceIP20"
        '
        'lblRing1_DeviceIP19
        '
        Me.lblRing1_DeviceIP19.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP19, "lblRing1_DeviceIP19")
        Me.lblRing1_DeviceIP19.Name = "lblRing1_DeviceIP19"
        '
        'lblRing1_DeviceIP18
        '
        Me.lblRing1_DeviceIP18.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP18, "lblRing1_DeviceIP18")
        Me.lblRing1_DeviceIP18.Name = "lblRing1_DeviceIP18"
        '
        'lblRing1_DeviceIP17
        '
        Me.lblRing1_DeviceIP17.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP17, "lblRing1_DeviceIP17")
        Me.lblRing1_DeviceIP17.Name = "lblRing1_DeviceIP17"
        '
        'lblRing1_DeviceIP16
        '
        Me.lblRing1_DeviceIP16.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP16, "lblRing1_DeviceIP16")
        Me.lblRing1_DeviceIP16.Name = "lblRing1_DeviceIP16"
        '
        'lblRing1_DeviceIP15
        '
        Me.lblRing1_DeviceIP15.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP15, "lblRing1_DeviceIP15")
        Me.lblRing1_DeviceIP15.Name = "lblRing1_DeviceIP15"
        '
        'lblRing1_DeviceIP14
        '
        Me.lblRing1_DeviceIP14.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP14, "lblRing1_DeviceIP14")
        Me.lblRing1_DeviceIP14.Name = "lblRing1_DeviceIP14"
        '
        'lblRing1_DeviceIP13
        '
        Me.lblRing1_DeviceIP13.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP13, "lblRing1_DeviceIP13")
        Me.lblRing1_DeviceIP13.Name = "lblRing1_DeviceIP13"
        '
        'lblRing1_DeviceIP12
        '
        Me.lblRing1_DeviceIP12.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP12, "lblRing1_DeviceIP12")
        Me.lblRing1_DeviceIP12.Name = "lblRing1_DeviceIP12"
        '
        'lblRing1_DeviceIP11
        '
        Me.lblRing1_DeviceIP11.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP11, "lblRing1_DeviceIP11")
        Me.lblRing1_DeviceIP11.Name = "lblRing1_DeviceIP11"
        '
        'lblRing1_DeviceIP10
        '
        Me.lblRing1_DeviceIP10.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP10, "lblRing1_DeviceIP10")
        Me.lblRing1_DeviceIP10.Name = "lblRing1_DeviceIP10"
        '
        'lblRing1_DeviceIP9
        '
        Me.lblRing1_DeviceIP9.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP9, "lblRing1_DeviceIP9")
        Me.lblRing1_DeviceIP9.Name = "lblRing1_DeviceIP9"
        '
        'lblRing1_DeviceIP8
        '
        Me.lblRing1_DeviceIP8.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP8, "lblRing1_DeviceIP8")
        Me.lblRing1_DeviceIP8.Name = "lblRing1_DeviceIP8"
        '
        'lblRing1_DeviceIP7
        '
        Me.lblRing1_DeviceIP7.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP7, "lblRing1_DeviceIP7")
        Me.lblRing1_DeviceIP7.Name = "lblRing1_DeviceIP7"
        '
        'lblRing1_DeviceIP6
        '
        Me.lblRing1_DeviceIP6.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP6, "lblRing1_DeviceIP6")
        Me.lblRing1_DeviceIP6.Name = "lblRing1_DeviceIP6"
        '
        'lblRing1_DeviceIP5
        '
        Me.lblRing1_DeviceIP5.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP5, "lblRing1_DeviceIP5")
        Me.lblRing1_DeviceIP5.Name = "lblRing1_DeviceIP5"
        '
        'lblRing1_DeviceIP4
        '
        Me.lblRing1_DeviceIP4.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP4, "lblRing1_DeviceIP4")
        Me.lblRing1_DeviceIP4.Name = "lblRing1_DeviceIP4"
        '
        'lblRing1_DeviceIP3
        '
        Me.lblRing1_DeviceIP3.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP3, "lblRing1_DeviceIP3")
        Me.lblRing1_DeviceIP3.Name = "lblRing1_DeviceIP3"
        '
        'lblRing1_DeviceIP2
        '
        Me.lblRing1_DeviceIP2.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP2, "lblRing1_DeviceIP2")
        Me.lblRing1_DeviceIP2.Name = "lblRing1_DeviceIP2"
        '
        'lblRing1_DeviceIP1
        '
        Me.lblRing1_DeviceIP1.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP1, "lblRing1_DeviceIP1")
        Me.lblRing1_DeviceIP1.Name = "lblRing1_DeviceIP1"
        '
        'lblRing1_DeviceIP0
        '
        Me.lblRing1_DeviceIP0.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing1_DeviceIP0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing1_DeviceIP0, "lblRing1_DeviceIP0")
        Me.lblRing1_DeviceIP0.Name = "lblRing1_DeviceIP0"
        '
        'gboxAMONet0
        '
        Me.gboxAMONet0.BackColor = System.Drawing.SystemColors.Control
        Me.gboxAMONet0.Controls.Add(Me.btnStartRing0)
        Me.gboxAMONet0.Controls.Add(Me.lblAMONetRing0SubType)
        Me.gboxAMONet0.Controls.Add(Me.lblAMONetRing0Type)
        Me.gboxAMONet0.Controls.Add(Me.lblAMONetRing0SubTypeTitle)
        Me.gboxAMONet0.Controls.Add(Me.lblAMONetRing0TypeTitle)
        Me.gboxAMONet0.Controls.Add(Me.lblErrorSlave0)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP63)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP62)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP61)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP60)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP59)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP58)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP57)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP56)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP55)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP54)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP53)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP52)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP51)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP50)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP49)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP48)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP47)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP46)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP45)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP44)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP43)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP42)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP41)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP40)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP39)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP38)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP37)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP36)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP35)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP34)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP33)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP32)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP31)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP30)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP29)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP28)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP27)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP26)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP25)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP24)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP23)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP22)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP21)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP20)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP19)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP18)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP17)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP16)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP15)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP14)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP13)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP12)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP11)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP10)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP9)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP8)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP7)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP6)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP5)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP4)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP3)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP2)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP1)
        Me.gboxAMONet0.Controls.Add(Me.lblRing0_DeviceIP0)
        resources.ApplyResources(Me.gboxAMONet0, "gboxAMONet0")
        Me.gboxAMONet0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.gboxAMONet0.Name = "gboxAMONet0"
        Me.gboxAMONet0.TabStop = False
        '
        'btnStartRing0
        '
        resources.ApplyResources(Me.btnStartRing0, "btnStartRing0")
        Me.btnStartRing0.Name = "btnStartRing0"
        Me.btnStartRing0.UseVisualStyleBackColor = True
        '
        'lblAMONetRing0SubType
        '
        Me.lblAMONetRing0SubType.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblAMONetRing0SubType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing0SubType, "lblAMONetRing0SubType")
        Me.lblAMONetRing0SubType.Name = "lblAMONetRing0SubType"
        '
        'lblAMONetRing0Type
        '
        Me.lblAMONetRing0Type.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblAMONetRing0Type.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing0Type, "lblAMONetRing0Type")
        Me.lblAMONetRing0Type.Name = "lblAMONetRing0Type"
        '
        'lblAMONetRing0SubTypeTitle
        '
        Me.lblAMONetRing0SubTypeTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAMONetRing0SubTypeTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing0SubTypeTitle, "lblAMONetRing0SubTypeTitle")
        Me.lblAMONetRing0SubTypeTitle.Name = "lblAMONetRing0SubTypeTitle"
        '
        'lblAMONetRing0TypeTitle
        '
        Me.lblAMONetRing0TypeTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAMONetRing0TypeTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblAMONetRing0TypeTitle, "lblAMONetRing0TypeTitle")
        Me.lblAMONetRing0TypeTitle.Name = "lblAMONetRing0TypeTitle"
        '
        'lblErrorSlave0
        '
        Me.lblErrorSlave0.BackColor = System.Drawing.Color.SandyBrown
        Me.lblErrorSlave0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblErrorSlave0, "lblErrorSlave0")
        Me.lblErrorSlave0.Name = "lblErrorSlave0"
        '
        'lblRing0_DeviceIP63
        '
        Me.lblRing0_DeviceIP63.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP63.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP63, "lblRing0_DeviceIP63")
        Me.lblRing0_DeviceIP63.Name = "lblRing0_DeviceIP63"
        '
        'lblRing0_DeviceIP62
        '
        Me.lblRing0_DeviceIP62.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP62.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP62, "lblRing0_DeviceIP62")
        Me.lblRing0_DeviceIP62.Name = "lblRing0_DeviceIP62"
        '
        'lblRing0_DeviceIP61
        '
        Me.lblRing0_DeviceIP61.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP61.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP61, "lblRing0_DeviceIP61")
        Me.lblRing0_DeviceIP61.Name = "lblRing0_DeviceIP61"
        '
        'lblRing0_DeviceIP60
        '
        Me.lblRing0_DeviceIP60.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP60.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP60, "lblRing0_DeviceIP60")
        Me.lblRing0_DeviceIP60.Name = "lblRing0_DeviceIP60"
        '
        'lblRing0_DeviceIP59
        '
        Me.lblRing0_DeviceIP59.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP59.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP59, "lblRing0_DeviceIP59")
        Me.lblRing0_DeviceIP59.Name = "lblRing0_DeviceIP59"
        '
        'lblRing0_DeviceIP58
        '
        Me.lblRing0_DeviceIP58.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP58.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP58, "lblRing0_DeviceIP58")
        Me.lblRing0_DeviceIP58.Name = "lblRing0_DeviceIP58"
        '
        'lblRing0_DeviceIP57
        '
        Me.lblRing0_DeviceIP57.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP57, "lblRing0_DeviceIP57")
        Me.lblRing0_DeviceIP57.Name = "lblRing0_DeviceIP57"
        '
        'lblRing0_DeviceIP56
        '
        Me.lblRing0_DeviceIP56.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP56.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP56, "lblRing0_DeviceIP56")
        Me.lblRing0_DeviceIP56.Name = "lblRing0_DeviceIP56"
        '
        'lblRing0_DeviceIP55
        '
        Me.lblRing0_DeviceIP55.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP55.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP55, "lblRing0_DeviceIP55")
        Me.lblRing0_DeviceIP55.Name = "lblRing0_DeviceIP55"
        '
        'lblRing0_DeviceIP54
        '
        Me.lblRing0_DeviceIP54.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP54.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP54, "lblRing0_DeviceIP54")
        Me.lblRing0_DeviceIP54.Name = "lblRing0_DeviceIP54"
        '
        'lblRing0_DeviceIP53
        '
        Me.lblRing0_DeviceIP53.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP53.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP53, "lblRing0_DeviceIP53")
        Me.lblRing0_DeviceIP53.Name = "lblRing0_DeviceIP53"
        '
        'lblRing0_DeviceIP52
        '
        Me.lblRing0_DeviceIP52.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP52.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP52, "lblRing0_DeviceIP52")
        Me.lblRing0_DeviceIP52.Name = "lblRing0_DeviceIP52"
        '
        'lblRing0_DeviceIP51
        '
        Me.lblRing0_DeviceIP51.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP51.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP51, "lblRing0_DeviceIP51")
        Me.lblRing0_DeviceIP51.Name = "lblRing0_DeviceIP51"
        '
        'lblRing0_DeviceIP50
        '
        Me.lblRing0_DeviceIP50.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP50.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP50, "lblRing0_DeviceIP50")
        Me.lblRing0_DeviceIP50.Name = "lblRing0_DeviceIP50"
        '
        'lblRing0_DeviceIP49
        '
        Me.lblRing0_DeviceIP49.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP49.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP49, "lblRing0_DeviceIP49")
        Me.lblRing0_DeviceIP49.Name = "lblRing0_DeviceIP49"
        '
        'lblRing0_DeviceIP48
        '
        Me.lblRing0_DeviceIP48.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP48.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP48, "lblRing0_DeviceIP48")
        Me.lblRing0_DeviceIP48.Name = "lblRing0_DeviceIP48"
        '
        'lblRing0_DeviceIP47
        '
        Me.lblRing0_DeviceIP47.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP47.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP47, "lblRing0_DeviceIP47")
        Me.lblRing0_DeviceIP47.Name = "lblRing0_DeviceIP47"
        '
        'lblRing0_DeviceIP46
        '
        Me.lblRing0_DeviceIP46.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP46.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP46, "lblRing0_DeviceIP46")
        Me.lblRing0_DeviceIP46.Name = "lblRing0_DeviceIP46"
        '
        'lblRing0_DeviceIP45
        '
        Me.lblRing0_DeviceIP45.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP45.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP45, "lblRing0_DeviceIP45")
        Me.lblRing0_DeviceIP45.Name = "lblRing0_DeviceIP45"
        '
        'lblRing0_DeviceIP44
        '
        Me.lblRing0_DeviceIP44.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP44.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP44, "lblRing0_DeviceIP44")
        Me.lblRing0_DeviceIP44.Name = "lblRing0_DeviceIP44"
        '
        'lblRing0_DeviceIP43
        '
        Me.lblRing0_DeviceIP43.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP43.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP43, "lblRing0_DeviceIP43")
        Me.lblRing0_DeviceIP43.Name = "lblRing0_DeviceIP43"
        '
        'lblRing0_DeviceIP42
        '
        Me.lblRing0_DeviceIP42.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP42.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP42, "lblRing0_DeviceIP42")
        Me.lblRing0_DeviceIP42.Name = "lblRing0_DeviceIP42"
        '
        'lblRing0_DeviceIP41
        '
        Me.lblRing0_DeviceIP41.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP41.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP41, "lblRing0_DeviceIP41")
        Me.lblRing0_DeviceIP41.Name = "lblRing0_DeviceIP41"
        '
        'lblRing0_DeviceIP40
        '
        Me.lblRing0_DeviceIP40.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP40.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP40, "lblRing0_DeviceIP40")
        Me.lblRing0_DeviceIP40.Name = "lblRing0_DeviceIP40"
        '
        'lblRing0_DeviceIP39
        '
        Me.lblRing0_DeviceIP39.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP39.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP39, "lblRing0_DeviceIP39")
        Me.lblRing0_DeviceIP39.Name = "lblRing0_DeviceIP39"
        '
        'lblRing0_DeviceIP38
        '
        Me.lblRing0_DeviceIP38.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP38.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP38, "lblRing0_DeviceIP38")
        Me.lblRing0_DeviceIP38.Name = "lblRing0_DeviceIP38"
        '
        'lblRing0_DeviceIP37
        '
        Me.lblRing0_DeviceIP37.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP37.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP37, "lblRing0_DeviceIP37")
        Me.lblRing0_DeviceIP37.Name = "lblRing0_DeviceIP37"
        '
        'lblRing0_DeviceIP36
        '
        Me.lblRing0_DeviceIP36.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP36.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP36, "lblRing0_DeviceIP36")
        Me.lblRing0_DeviceIP36.Name = "lblRing0_DeviceIP36"
        '
        'lblRing0_DeviceIP35
        '
        Me.lblRing0_DeviceIP35.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP35.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP35, "lblRing0_DeviceIP35")
        Me.lblRing0_DeviceIP35.Name = "lblRing0_DeviceIP35"
        '
        'lblRing0_DeviceIP34
        '
        Me.lblRing0_DeviceIP34.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP34.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP34, "lblRing0_DeviceIP34")
        Me.lblRing0_DeviceIP34.Name = "lblRing0_DeviceIP34"
        '
        'lblRing0_DeviceIP33
        '
        Me.lblRing0_DeviceIP33.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP33.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP33, "lblRing0_DeviceIP33")
        Me.lblRing0_DeviceIP33.Name = "lblRing0_DeviceIP33"
        '
        'lblRing0_DeviceIP32
        '
        Me.lblRing0_DeviceIP32.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP32.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP32, "lblRing0_DeviceIP32")
        Me.lblRing0_DeviceIP32.Name = "lblRing0_DeviceIP32"
        '
        'lblRing0_DeviceIP31
        '
        Me.lblRing0_DeviceIP31.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP31.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP31, "lblRing0_DeviceIP31")
        Me.lblRing0_DeviceIP31.Name = "lblRing0_DeviceIP31"
        '
        'lblRing0_DeviceIP30
        '
        Me.lblRing0_DeviceIP30.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP30.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP30, "lblRing0_DeviceIP30")
        Me.lblRing0_DeviceIP30.Name = "lblRing0_DeviceIP30"
        '
        'lblRing0_DeviceIP29
        '
        Me.lblRing0_DeviceIP29.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP29.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP29, "lblRing0_DeviceIP29")
        Me.lblRing0_DeviceIP29.Name = "lblRing0_DeviceIP29"
        '
        'lblRing0_DeviceIP28
        '
        Me.lblRing0_DeviceIP28.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP28.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP28, "lblRing0_DeviceIP28")
        Me.lblRing0_DeviceIP28.Name = "lblRing0_DeviceIP28"
        '
        'lblRing0_DeviceIP27
        '
        Me.lblRing0_DeviceIP27.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP27, "lblRing0_DeviceIP27")
        Me.lblRing0_DeviceIP27.Name = "lblRing0_DeviceIP27"
        '
        'lblRing0_DeviceIP26
        '
        Me.lblRing0_DeviceIP26.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP26.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP26, "lblRing0_DeviceIP26")
        Me.lblRing0_DeviceIP26.Name = "lblRing0_DeviceIP26"
        '
        'lblRing0_DeviceIP25
        '
        Me.lblRing0_DeviceIP25.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP25.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP25, "lblRing0_DeviceIP25")
        Me.lblRing0_DeviceIP25.Name = "lblRing0_DeviceIP25"
        '
        'lblRing0_DeviceIP24
        '
        Me.lblRing0_DeviceIP24.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP24, "lblRing0_DeviceIP24")
        Me.lblRing0_DeviceIP24.Name = "lblRing0_DeviceIP24"
        '
        'lblRing0_DeviceIP23
        '
        Me.lblRing0_DeviceIP23.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP23, "lblRing0_DeviceIP23")
        Me.lblRing0_DeviceIP23.Name = "lblRing0_DeviceIP23"
        '
        'lblRing0_DeviceIP22
        '
        Me.lblRing0_DeviceIP22.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP22, "lblRing0_DeviceIP22")
        Me.lblRing0_DeviceIP22.Name = "lblRing0_DeviceIP22"
        '
        'lblRing0_DeviceIP21
        '
        Me.lblRing0_DeviceIP21.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP21, "lblRing0_DeviceIP21")
        Me.lblRing0_DeviceIP21.Name = "lblRing0_DeviceIP21"
        '
        'lblRing0_DeviceIP20
        '
        Me.lblRing0_DeviceIP20.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP20.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP20, "lblRing0_DeviceIP20")
        Me.lblRing0_DeviceIP20.Name = "lblRing0_DeviceIP20"
        '
        'lblRing0_DeviceIP19
        '
        Me.lblRing0_DeviceIP19.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP19, "lblRing0_DeviceIP19")
        Me.lblRing0_DeviceIP19.Name = "lblRing0_DeviceIP19"
        '
        'lblRing0_DeviceIP18
        '
        Me.lblRing0_DeviceIP18.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP18, "lblRing0_DeviceIP18")
        Me.lblRing0_DeviceIP18.Name = "lblRing0_DeviceIP18"
        '
        'lblRing0_DeviceIP17
        '
        Me.lblRing0_DeviceIP17.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP17, "lblRing0_DeviceIP17")
        Me.lblRing0_DeviceIP17.Name = "lblRing0_DeviceIP17"
        '
        'lblRing0_DeviceIP16
        '
        Me.lblRing0_DeviceIP16.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP16, "lblRing0_DeviceIP16")
        Me.lblRing0_DeviceIP16.Name = "lblRing0_DeviceIP16"
        '
        'lblRing0_DeviceIP15
        '
        Me.lblRing0_DeviceIP15.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP15, "lblRing0_DeviceIP15")
        Me.lblRing0_DeviceIP15.Name = "lblRing0_DeviceIP15"
        '
        'lblRing0_DeviceIP14
        '
        Me.lblRing0_DeviceIP14.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP14, "lblRing0_DeviceIP14")
        Me.lblRing0_DeviceIP14.Name = "lblRing0_DeviceIP14"
        '
        'lblRing0_DeviceIP13
        '
        Me.lblRing0_DeviceIP13.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP13, "lblRing0_DeviceIP13")
        Me.lblRing0_DeviceIP13.Name = "lblRing0_DeviceIP13"
        '
        'lblRing0_DeviceIP12
        '
        Me.lblRing0_DeviceIP12.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP12, "lblRing0_DeviceIP12")
        Me.lblRing0_DeviceIP12.Name = "lblRing0_DeviceIP12"
        '
        'lblRing0_DeviceIP11
        '
        Me.lblRing0_DeviceIP11.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP11, "lblRing0_DeviceIP11")
        Me.lblRing0_DeviceIP11.Name = "lblRing0_DeviceIP11"
        '
        'lblRing0_DeviceIP10
        '
        Me.lblRing0_DeviceIP10.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP10, "lblRing0_DeviceIP10")
        Me.lblRing0_DeviceIP10.Name = "lblRing0_DeviceIP10"
        '
        'lblRing0_DeviceIP9
        '
        Me.lblRing0_DeviceIP9.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP9, "lblRing0_DeviceIP9")
        Me.lblRing0_DeviceIP9.Name = "lblRing0_DeviceIP9"
        '
        'lblRing0_DeviceIP8
        '
        Me.lblRing0_DeviceIP8.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP8, "lblRing0_DeviceIP8")
        Me.lblRing0_DeviceIP8.Name = "lblRing0_DeviceIP8"
        '
        'lblRing0_DeviceIP7
        '
        Me.lblRing0_DeviceIP7.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP7, "lblRing0_DeviceIP7")
        Me.lblRing0_DeviceIP7.Name = "lblRing0_DeviceIP7"
        '
        'lblRing0_DeviceIP6
        '
        Me.lblRing0_DeviceIP6.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP6, "lblRing0_DeviceIP6")
        Me.lblRing0_DeviceIP6.Name = "lblRing0_DeviceIP6"
        '
        'lblRing0_DeviceIP5
        '
        Me.lblRing0_DeviceIP5.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP5, "lblRing0_DeviceIP5")
        Me.lblRing0_DeviceIP5.Name = "lblRing0_DeviceIP5"
        '
        'lblRing0_DeviceIP4
        '
        Me.lblRing0_DeviceIP4.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP4, "lblRing0_DeviceIP4")
        Me.lblRing0_DeviceIP4.Name = "lblRing0_DeviceIP4"
        '
        'lblRing0_DeviceIP3
        '
        Me.lblRing0_DeviceIP3.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP3, "lblRing0_DeviceIP3")
        Me.lblRing0_DeviceIP3.Name = "lblRing0_DeviceIP3"
        '
        'lblRing0_DeviceIP2
        '
        Me.lblRing0_DeviceIP2.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP2, "lblRing0_DeviceIP2")
        Me.lblRing0_DeviceIP2.Name = "lblRing0_DeviceIP2"
        '
        'lblRing0_DeviceIP1
        '
        Me.lblRing0_DeviceIP1.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP1, "lblRing0_DeviceIP1")
        Me.lblRing0_DeviceIP1.Name = "lblRing0_DeviceIP1"
        '
        'lblRing0_DeviceIP0
        '
        Me.lblRing0_DeviceIP0.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblRing0_DeviceIP0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblRing0_DeviceIP0, "lblRing0_DeviceIP0")
        Me.lblRing0_DeviceIP0.Name = "lblRing0_DeviceIP0"
        '
        'tabGeneral
        '
        Me.tabGeneral.Controls.Add(Me.tabRemoteTable)
        Me.tabGeneral.Controls.Add(Me.tabIOTable)
        Me.tabGeneral.Controls.Add(Me.tabMotionTable)
        resources.ApplyResources(Me.tabGeneral, "tabGeneral")
        Me.tabGeneral.Name = "tabGeneral"
        Me.tabGeneral.SelectedIndex = 0
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'TextBox_LatchDataFeedback
        '
        resources.ApplyResources(Me.TextBox_LatchDataFeedback, "TextBox_LatchDataFeedback")
        Me.TextBox_LatchDataFeedback.Name = "TextBox_LatchDataFeedback"
        Me.TextBox_LatchDataFeedback.ReadOnly = True
        '
        'formSetting
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        resources.ApplyResources(Me, "$this")
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ControlBox = False
        Me.Controls.Add(Me.tabGeneral)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "formSetting"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.tabMotionTable.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.gboxStepControl.ResumeLayout(False)
        Me.gboxStepControl.PerformLayout()
        Me.gboxStatus.ResumeLayout(False)
        Me.gboxStatus.PerformLayout()
        Me.gboxMotion.ResumeLayout(False)
        Me.gboxPosition.ResumeLayout(False)
        Me.gboxPosition.PerformLayout()
        Me.gboxVelocityProfile.ResumeLayout(False)
        Me.gboxVelocityProfile.PerformLayout()
        Me.gboxMoveMode.ResumeLayout(False)
        Me.gboxMoveMode.PerformLayout()
        Me.gboxRecord.ResumeLayout(False)
        CType(Me.msgStepRecord, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabIOTable.ResumeLayout(False)
        Me.tabIOTable.PerformLayout()
        Me.tabRemoteTable.ResumeLayout(False)
        Me.gboxAMONet1.ResumeLayout(False)
        Me.gboxAMONet0.ResumeLayout(False)
        Me.tabGeneral.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents timerRefresh As System.Windows.Forms.Timer
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents imlfrmMotionTest As System.Windows.Forms.ImageList
    Friend WithEvents tabMotionTable As System.Windows.Forms.TabPage
    Friend WithEvents lblStepPartTitle1 As System.Windows.Forms.Label
    Friend WithEvents lblStepPartTitle0 As System.Windows.Forms.Label
    Friend WithEvents cboMStepName As System.Windows.Forms.ComboBox
    Friend WithEvents cboMStepPart As System.Windows.Forms.ComboBox
    Friend WithEvents gboxStepControl As System.Windows.Forms.GroupBox
    Friend WithEvents gboxStatus As System.Windows.Forms.GroupBox
    Friend WithEvents shpStatus15 As System.Windows.Forms.Label
    Friend WithEvents shpStatus14 As System.Windows.Forms.Label
    Friend WithEvents shpStatus13 As System.Windows.Forms.Label
    Friend WithEvents shpStatus12 As System.Windows.Forms.Label
    Friend WithEvents shpStatus11 As System.Windows.Forms.Label
    Friend WithEvents shpStatus10 As System.Windows.Forms.Label
    Friend WithEvents shpStatus9 As System.Windows.Forms.Label
    Friend WithEvents shpStatus8 As System.Windows.Forms.Label
    Friend WithEvents shpStatus7 As System.Windows.Forms.Label
    Friend WithEvents shpStatus6 As System.Windows.Forms.Label
    Friend WithEvents shpStatus5 As System.Windows.Forms.Label
    Friend WithEvents shpStatus4 As System.Windows.Forms.Label
    Friend WithEvents shpStatus3 As System.Windows.Forms.Label
    Friend WithEvents shpStatus2 As System.Windows.Forms.Label
    Friend WithEvents shpStatus1 As System.Windows.Forms.Label
    Friend WithEvents shpStatus0 As System.Windows.Forms.Label
    Friend WithEvents lblStatus15 As System.Windows.Forms.Label
    Friend WithEvents lblStatus14 As System.Windows.Forms.Label
    Friend WithEvents lblStatus13 As System.Windows.Forms.Label
    Friend WithEvents lblStatus12 As System.Windows.Forms.Label
    Friend WithEvents lblStatus11 As System.Windows.Forms.Label
    Friend WithEvents lblStatus10 As System.Windows.Forms.Label
    Friend WithEvents lblStatus9 As System.Windows.Forms.Label
    Friend WithEvents lblStatus8 As System.Windows.Forms.Label
    Friend WithEvents lblStatus7 As System.Windows.Forms.Label
    Friend WithEvents lblStatus6 As System.Windows.Forms.Label
    Friend WithEvents lblStatus5 As System.Windows.Forms.Label
    Friend WithEvents lblStatus4 As System.Windows.Forms.Label
    Friend WithEvents lblStatus3 As System.Windows.Forms.Label
    Friend WithEvents lblStatus2 As System.Windows.Forms.Label
    Friend WithEvents lblStatus1 As System.Windows.Forms.Label
    Friend WithEvents lblStatus0 As System.Windows.Forms.Label
    Friend WithEvents gboxMotion As System.Windows.Forms.GroupBox
    Friend WithEvents btnMotion4 As System.Windows.Forms.Button
    Friend WithEvents btnMotion3 As System.Windows.Forms.Button
    Friend WithEvents btnMotion2 As System.Windows.Forms.Button
    Friend WithEvents btnMotion1 As System.Windows.Forms.Button
    Friend WithEvents btnMotion0 As System.Windows.Forms.Button
    Friend WithEvents gboxPosition As System.Windows.Forms.GroupBox
    Friend WithEvents btnGetPosition As System.Windows.Forms.Button
    Friend WithEvents btnPositionReset As System.Windows.Forms.Button
    Friend WithEvents lblPosition2 As System.Windows.Forms.Label
    Friend WithEvents lblPosition1 As System.Windows.Forms.Label
    Friend WithEvents lblPosition0 As System.Windows.Forms.Label
    Friend WithEvents lblPositionTitle2 As System.Windows.Forms.Label
    Friend WithEvents lblPositionTitle1 As System.Windows.Forms.Label
    Friend WithEvents lblPositionTitle0 As System.Windows.Forms.Label
    Friend WithEvents gboxVelocityProfile As System.Windows.Forms.GroupBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileTitle0 As System.Windows.Forms.Label
    Friend WithEvents lblMotionVelocity As System.Windows.Forms.Label
    Friend WithEvents lblMotionStatus As System.Windows.Forms.Label
    Friend WithEvents gboxMoveMode As System.Windows.Forms.GroupBox
    Friend WithEvents rdoMoveMode1 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoMoveMode0 As System.Windows.Forms.RadioButton
    Friend WithEvents lblVelocityProfileUnit3 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileUnit2 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileUnit1 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileUnit0 As System.Windows.Forms.Label
    Friend WithEvents txtVelocityProfile3 As System.Windows.Forms.TextBox
    Friend WithEvents txtVelocityProfile2 As System.Windows.Forms.TextBox
    Friend WithEvents txtVelocityProfile1 As System.Windows.Forms.TextBox
    Friend WithEvents txtVelocityProfile0 As System.Windows.Forms.TextBox
    Friend WithEvents lblVelocityProfileTitle3 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileTitle2 As System.Windows.Forms.Label
    Friend WithEvents lblVelocityProfileTitle1 As System.Windows.Forms.Label
    Friend WithEvents gboxRecord As System.Windows.Forms.GroupBox
    Friend WithEvents btnGetVelocityProfile As System.Windows.Forms.Button
    Friend WithEvents btnRecord2 As System.Windows.Forms.Button
    Friend WithEvents btnMoveToMotionPosition As System.Windows.Forms.Button
    Friend WithEvents msgStepRecord As System.Windows.Forms.DataGridView
    Friend WithEvents tabIOTable As System.Windows.Forms.TabPage
    Friend WithEvents tabRemoteTable As System.Windows.Forms.TabPage
    Friend WithEvents gboxAMONet1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnStartRing1 As System.Windows.Forms.Button
    Friend WithEvents lblAMONetRing1SubType As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing1Type As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing1SubTypeTitle As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing1TypeTitle As System.Windows.Forms.Label
    Friend WithEvents lblErrorSlave1 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP63 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP62 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP61 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP60 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP59 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP58 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP57 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP56 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP55 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP54 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP53 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP52 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP51 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP50 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP49 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP48 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP47 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP46 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP45 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP44 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP43 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP42 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP41 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP40 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP39 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP38 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP37 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP36 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP35 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP34 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP33 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP32 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP31 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP30 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP29 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP28 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP27 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP26 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP25 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP24 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP23 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP22 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP21 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP20 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP19 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP18 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP17 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP16 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP15 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP14 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP13 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP12 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP11 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP10 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP9 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP8 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP7 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP6 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP5 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP4 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP3 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP2 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP1 As System.Windows.Forms.Label
    Friend WithEvents lblRing1_DeviceIP0 As System.Windows.Forms.Label
    Friend WithEvents gboxAMONet0 As System.Windows.Forms.GroupBox
    Friend WithEvents btnStartRing0 As System.Windows.Forms.Button
    Friend WithEvents lblAMONetRing0SubType As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing0Type As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing0SubTypeTitle As System.Windows.Forms.Label
    Friend WithEvents lblAMONetRing0TypeTitle As System.Windows.Forms.Label
    Friend WithEvents lblErrorSlave0 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP63 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP62 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP61 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP60 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP59 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP58 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP57 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP56 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP55 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP54 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP53 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP52 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP51 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP50 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP49 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP48 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP47 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP46 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP45 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP44 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP43 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP42 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP41 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP40 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP39 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP38 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP37 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP36 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP35 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP34 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP33 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP32 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP31 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP30 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP29 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP28 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP27 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP26 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP25 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP24 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP23 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP22 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP21 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP20 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP19 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP18 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP17 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP16 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP15 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP14 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP13 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP12 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP11 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP10 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP9 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP8 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP7 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP6 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP5 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP4 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP3 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP2 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP1 As System.Windows.Forms.Label
    Friend WithEvents lblRing0_DeviceIP0 As System.Windows.Forms.Label
    Friend WithEvents tabGeneral As System.Windows.Forms.TabControl
    Friend WithEvents TextBoxCurrentFile As System.Windows.Forms.TextBox
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents Column2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ButtonService As System.Windows.Forms.Button
    Friend WithEvents userControlIOTable1 As Automation.UserControlIOTable
    Friend WithEvents lblMotorErrorStatus As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_LatchDataCommand As System.Windows.Forms.TextBox
    Friend WithEvents Btn_LatchDisable As System.Windows.Forms.Button
    Friend WithEvents Btn_LatchEnable As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBox_LatchDataFeedback As System.Windows.Forms.TextBox
End Class
