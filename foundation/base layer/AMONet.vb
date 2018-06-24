Option Strict Off
Option Explicit On
Imports System.Runtime.InteropServices
'
'Define Structures for AIO
<StructLayout(LayoutKind.Sequential)> _
Public Structure GAINLIST
    Public usGainCde As Int16
    Public fMaxGainVal As Single
    Public fMinGainVal As Single
    Public fUpLimit As Single
    Public fDownLimit As Single
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=30)> _
    Public StrGina() As Byte
End Structure

<StructLayout(LayoutKind.Sequential)> _
Public Structure PT_AIOFeature
    Public dwModuleID As Int32
    Public usMaxAISingle As Int16
    Public usMaxAIDiff As Int16
    Public usNumADBit As Int16
    Public usNumAIByte As Int16
    Public AIVolGainArray As IntPtr       'GainList for AI Voltage
    Public AICurGainArray As IntPtr 'GainList for AI Current
    Public usMaxAOChan As Int16
    Public usNumDABit As Int16
    Public usNumDAByte As Int16
    Public AOVolGainArray As IntPtr
    Public AOCurGainArray As IntPtr
End Structure

Module MNET
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' ADVANTECH SOFTWARE
    ' Motion.NET API
    ' Release version 1.0 Beta
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Const Ring_St_Disconnected As Short = &H0S
    Public Const Ring_St_Connected As Short = &H1S
    Public Const Ring_St_Slave_Error As Short = &H2S
    Public Const Ring_St_Idle As Short = &H3S
    Public Const Ring_St_Error As Short = &H4S

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Error Code
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Const ERR_NoError As Short = 0
    Public Const ERR_EventError As Short = -1
    Public Const ERR_LinkError As Short = -2
    Public Const ERR_MNET_Ring_Used As Short = -3
    Public Const ERR_Invalid_Ring As Short = -4
    Public Const ERR_Invalid_Slave As Short = -5
    Public Const ERR_Invalid_Hardware As Short = -6
    Public Const ERR_Value_Out_Range As Short = -8
    Public Const ERR_Invalid_Setting As Short = -9

    Public Const ERR_Axis_Communication As Short = -11
    Public Const ERR_Axis_command As Short = -12
    Public Const ERR_Axis_Receive As Short = -13
    Public Const ERR_Invalid_Operating_Velocity As Short = -14
    Public Const ERR_PosOutOfRange As Short = -15
    Public Const ERR_Invalid_MaxVel As Short = -16
    Public Const ERR_Speed_Change As Short = -17
    Public Const ERR_SlowDown_Point As Short = -18
    Public Const ERR_Invalid_DIO As Short = -19
    Public Const ERR_Invalid_Comparator As Short = -20
    Public Const ERR_Comparator_Config As Short = -21
    Public Const ERR_CompareSourceError As Short = -22
    Public Const ERR_CompareActionError As Short = -23
    Public Const ERR_CompareMethodError As Short = -24
    Public Const ERR_ComparatorRead As Short = -25
    Public Const ERR_LimitOutOfRange As Short = -26
    '//Added by W.Y.Z on 2012.08.15
    Public Const ERR_Invalid_DIO_Channel As Short = -27

    Public Const ERR_Latch_Config As Short = -30
    Public Const ERR_LatchError As Short = -31
    Public Const ERR_LatchRead As Short = -32
    Public Const ERR_HomeConfig As Short = -35

    '/////////////////////G94 BUS ERROR///////////////////
    Public Const ERR_G94_RECEIVE_TimeOut As Short = -36
    Public Const ERR_G94_CPURead As Short = -37

    '/////////////////////M4 ERROR///////////////////
    Public Const ERR_M4_CPLDRead As Short = -46
    Public Const ERR_M4_RegisterRead As Short = -47
    Public Const ERR_M4_CPLDWrite As Short = -48
    Public Const ERR_M4_RegisterWrite As Short = -49
    Public Const ERR_M4_InvalidAxisNo As Short = -50
    Public Const ERR_M4_MOFStatusErr As Short = -51
    Public Const ERR_M4_InvalidAxisSelect As Short = -52
    Public Const ERR_M4_MPGmode As Short = -53
    Public Const ERR_M4_InvalidMpgEnable As Short = -54
    Public Const ERR_M4_MOFConfigmode As Short = -55
    Public Const ERR_M4_SpeedError As Short = -56
    Public Const ERR_M4_AxisArrayError As Short = -57

    Public Const ERR_Invalid_DeviceNumber As Short = -58
    Public Const ERR_LoadDriver_Failed As Short = -59
    Public Const ERR_Resource_Failed As Short = -60
    Public Const ERR_Invalid_InputPulseMode As Short = -61
    Public Const ERR_Invalid_Logic As Short = -62
    Public Const ERR_Invalid_OutputPulseMode As Short = -63
    Public Const ERR_Invalid_FeedbackSource As Short = -64
    Public Const ERR_Invalid_ALMMode As Short = -65
    Public Const ERR_Invalid_ERCActiveTime As Short = -66
    Public Const ERR_Invalid_ERCUnactiveTime As Short = -67
    Public Const ERR_Invalid_SDMode As Short = -68
    Public Const ERR_Invalid_HomeMode As Short = -69
    Public Const ERR_Invalid_EZCount As Short = -70
    Public Const ERR_Invalid_ERCOperation As Short = -71
    Public Const ERR_Invalid_LatchNumber As Short = -72
    Public Const ERR_Device_NotOpened As Short = -73
    Public Const ERR_Watchdog_Started As Short = -74
    Public Const ERR_ConfigFileOpenError As Short = -75
    Public Const ERR_Invalid_Position_Change As Short = -76
    Public Const ERR_Invalid_StartVel As Short = -77
    Public Const ERR_Invalid_AccTime As Short = -78
    Public Const ERR_Invalid_DecTime As Short = -79
    Public Const ERR_Invalid_Ratio As Short = -80
    Public Const ERR_Invalid_ELMode As Short = -81
    Public Const ERR_Invalid_AccRange As Short = -82
    Public Const ERR_Invalid_DecRange As Short = -83
    Public Const ERR_Invalid_Memory As Short = -84
    Public Const ERR_Invalid_DIOValue As Short = -85
    Public Const ERR_Invalid_ORGLogic As Short = -86
    Public Const ERR_Invalid_EZLogic As Short = -87
    Public Const ERR_Invalid_LatchSetting As Short = -88
    Public Const ERR_Invalid_RelPosition As Short = -89
    Public Const ERR_Invalid_Baudrate As Short = -90
    Public Const ERR_No_Device_Initialized As Short = -91
    Public Const ERR_DeviceBusy As Short = -92
    Public Const ERR_Invalid_Table_Size As Short = -93
    Public Const ERR_Invalid_Compare_Pulse_Mode As Short = -94
    Public Const ERR_Invalid_Compare_Pulse_Width As Short = -95
    Public Const ERR_Invalid_Compare_Pulse_Logic As Short = -96
    Public Const ERR_Function_NotSupport As Short = -97
    Public Const ERR_Invalid_ORGOffset As Short = -98
    Public Const ERR_Invalid_FwMemoryMode As Short = -99

    '/////////////////////AMAX-2240 ERROR///////////////////
    Public Const ERR_AMAX2240 As Short = -200
    Public Const ERR_Invalid_Center_Position As Short = ERR_AMAX2240 - 1
    Public Const ERR_Invalid_End_Position As Short = ERR_AMAX2240 - 2
    Public Const ERR_Invalid_Path_Cmd_Function As Short = ERR_AMAX2240 - 3
    Public Const ERR_Invalid_Compare_Start_Data As Short = ERR_AMAX2240 - 4
    Public Const ERR_Invalid_Compare_Interval_Data As Short = ERR_AMAX2240 - 5
    '//Added by W.Y.Z on 2012.08.15
    Public Const ERR_Invalid_RepeatAxis As Short = ERR_AMAX2240 - 6
    Public Const ERR_Invalid_ZeroDistance As Short = ERR_AMAX2240 - 7
    Public Const ERR_Invalid_PrivateID As Short = ERR_AMAX2240 = 8

    '/////////////////////AMAX-2240 FIRMWARE ERROR///////////////////
    Public Const ERR_AMAX2240_FIRM As Short = -250
    '// Command execution error
    Public Const ERR_CommandCodeError As Short = ERR_AMAX2240_FIRM - 1
    Public Const ERR_CommandCountExceed As Short = ERR_AMAX2240_FIRM - 2
    Public Const ERR_CommandAddedCountError As Short = ERR_AMAX2240_FIRM - 3
    Public Const ERR_TerminalSymbolError As Short = ERR_AMAX2240_FIRM - 4
    Public Const ERR_CmpoutBufferIsFull As Short = ERR_AMAX2240_FIRM - 5
    Public Const ERR_InterruptButNoData As Short = ERR_AMAX2240_FIRM - 6      'Engineer Error code

    '// Motion command error
    Public Const ERR_MotionHaveDone As Short = ERR_AMAX2240_FIRM - 7
    Public Const ERR_SurplusPulseNotEnough As Short = ERR_AMAX2240_FIRM - 8
    Public Const ERR_InvalidSpeedSection As Short = ERR_AMAX2240_FIRM - 9
    Public Const ERR_InterpolationMove As Short = ERR_AMAX2240_FIRM - 10
    Public Const ERR_InvalidSpeedPattern As Short = ERR_AMAX2240_FIRM - 11
    Public Const ERR_AMAX_FWDownloadExceed As Short = ERR_AMAX2240_FIRM - 12
    Public Const ERR_AMAX_FWUpdateFailed As Short = ERR_AMAX2240_FIRM - 13
    Public Const ERR_CntBufferIsFull As Short = ERR_AMAX2240_FIRM - 14
    Public Const ERR_CntMoveIsBusy As Short = ERR_AMAX2240_FIRM - 15
    Public Const ERR_CntBufferIsNull As Short = ERR_AMAX2240_FIRM - 16
    Public Const ERR_FwMemoryAllocateError As Short = ERR_AMAX2240_FIRM - 17
    '//Added by W.Y.Z on 2012.08.15
    Public Const ERR_PassWrdErrorFirstly As Short = ERR_AMAX2240_FIRM - 18
    Public Const ERR_PassWrdErrorAgain As Short = ERR_AMAX2240_FIRM - 19
    Public Const ERR_PassWrdErrorthrice As Short = ERR_AMAX2240_FIRM - 20

    '// AMAX2710 error code
    Public Const ERR_AMAX2710_ERROR As Short = -300
    Public Const ERR_Module_Not_Initialize As Short = ERR_AMAX2710_ERROR - 1
    Public Const ERR_Module_Initialize_Fail As Short = ERR_AMAX2710_ERROR - 2
    Public Const ERR_AI_Channel_Error As Short = ERR_AMAX2710_ERROR - 3
    Public Const ERR_AI_Gain_Invalid As Short = ERR_AMAX2710_ERROR - 4
    Public Const ERR_AO_Channel_Error As Short = ERR_AMAX2710_ERROR - 5
    Public Const ERR_AO_Range_Error As Short = ERR_AMAX2710_ERROR - 6
    Public Const ERR_AO_Value_Invalid As Short = ERR_AMAX2710_ERROR - 7
    Public Const ERR_BUFFER_TOO_SMALL As Short = ERR_AMAX2710_ERROR - 8
    Public Const ERR_MODE_UNMATCHED As Short = ERR_AMAX2710_ERROR - 9

    '//AMAX1220 error code Added by W.Y.Z on 2012.08.15
    Public Const ERR_AMAX1220_ERROR As Short = -400
    Public Const ERR_M2_INVALID_AXISNO As Short = ERR_AMAX1220_ERROR - 1
    Public Const ERR_M2_INVALID_GPID As Short = ERR_AMAX1220_ERROR - 2
    Public Const ERR_M2_CannotFindInvalidGPID As Short = ERR_AMAX1220_ERROR - 3
    Public Const ERR_M2_Axis_Already_In_GP As Short = ERR_AMAX1220_ERROR - 4
    Public Const ERR_M2_Axis_not_exist_inGp As Short = ERR_AMAX1220_ERROR - 5
    Public Const ERR_M2_Invalid_Dist_Array As Short = ERR_AMAX1220_ERROR - 6
    Public Const ERR_M2_Invlaid_Center_Array As Short = ERR_AMAX1220_ERROR - 7
    Public Const ERR_M2_Invalid_Axis_Count As Short = ERR_AMAX1220_ERROR - 8
    Public Const ERR_M2_Invalid_Axis_Index As Short = ERR_AMAX1220_ERROR - 9

    'AdmMNet Version
    Declare Function B_mnet_get_version Lib "AMONet.dll" Alias "_mnet_get_version" (ByRef Version As Byte) As Short
    Declare Function B_mnet_initial Lib "AMONet.dll" Alias "_mnet_initial" () As Short
    Declare Function B_mnet_close Lib "AMONet.dll" Alias "_mnet_close" () As Short
    Declare Function B_mnet_set_retry_times Lib "AMONet.dll" Alias "_mnet_set_retry_times" (ByVal RingNo As Short, ByVal RetryTimes As Short) As Short

    'Hsien , 2015.11.18 , missed function call
    Declare Function B_1202_open Lib "AMONet.dll" Alias "_1202_open" (ByRef ExistCard As Short) As Short
    Declare Function B_1202_close Lib "AMONet.dll" Alias "_1202_close" (ByVal CardNo As Short) As Short
    Declare Function B_1202_lio_read Lib "AMONet.dll" Alias "_1202_lio_read" (ByVal CardNo As Short) As Short
    Declare Function B_1202_lio_write Lib "AMONet.dll" Alias "_1202_lio_write" (ByVal CardNo As Short, ByVal Value As Byte) As Short

    'Ring Status
    Declare Function B_mnet_enable_soft_watchdog Lib "AMONet.dll" Alias "_mnet_enable_soft_watchdog" (ByVal RingNo As Short, ByRef UserEvent As Integer) As Short
    Declare Function B_mnet_set_ring_quality_param Lib "AMONet.dll" Alias "_mnet_set_ring_quality_param" (ByVal RingNo As Short, ByVal ContinueErr As Short, ByVal ErrorRate As Short) As Short

    'Ring Status
    Declare Function B_mnet_get_ring_status Lib "AMONet.dll" Alias "_mnet_get_ring_status" (ByVal RingNo As Short, ByRef RingStatus As Short) As Short
    Declare Function B_mnet_get_com_status Lib "AMONet.dll" Alias "_mnet_get_com_status" (ByVal RingNo As Short) As Short

    'Ring Operation
    Declare Function B_mnet_set_ring_config Lib "AMONet.dll" Alias "_mnet_set_ring_config" (ByVal RingNo As Short, ByVal BaudRate As Short) As Short
    Declare Function B_mnet_reset_ring Lib "AMONet.dll" Alias "_mnet_reset_ring" (ByVal RingNo As Short) As Short

    'hsien , corrected data type , 2015.11.18
    Declare Function B_mnet_get_ring_active_table Lib "AMONet.dll" Alias "_mnet_get_ring_active_table" (ByVal RingNo As Short, ByRef ActTable As UInt64) As Short
    'Declare Function B_mnet_get_ring_active_table Lib "AMONet.dll" Alias "_mnet_get_ring_active_table" (ByVal RingNo As Short, ByRef ActTable As Integer) As Short

    Declare Function B_mnet_get_slave_info Lib "AMONet.dll" Alias "_mnet_get_slave_info" (ByVal RingNo As Short, ByVal SlaveIP As Short) As Short
    Declare Function B_mnet_start_ring Lib "AMONet.dll" Alias "_mnet_start_ring" (ByVal RingNo As Short) As Short
    Declare Function B_mnet_stop_ring Lib "AMONet.dll" Alias "_mnet_stop_ring" (ByVal RingNo As Short) As Short
    Declare Function B_mnet_get_error_device Lib "AMONet.dll" Alias "_mnet_get_error_device" (ByVal RingNo As Short) As Short

    'Hsien , error found , the argument 3 should pass by reference 
    '    Declare Function B_mnet_get_fw_version Lib "AMONet.dll" Alias "_mnet_get_fw_version" (ByVal RingNo As Short, ByVal SlaveIP As Short, ByVal FwVersion As String) As Short
    Declare Function B_mnet_get_fw_version Lib "AMONet.dll" Alias "_mnet_get_fw_version" (ByVal RingNo As Short, ByVal SlaveIP As Short, ByRef FwVersion As Byte) As Short


    'Io Operation
    Declare Function B_mnet_io_output Lib "AMONet.dll" Alias "_mnet_io_output" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PortNo As Short, ByVal Value As Short) As Short
    Declare Function B_mnet_io_input Lib "AMONet.dll" Alias "_mnet_io_input" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PortNo As Short) As Short

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '1 Axis Slave Operation
    Declare Function B_mnet_m1_initial Lib "AMONet.dll" Alias "_mnet_m1_initial" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_loadconfig Lib "AMONet.dll" Alias "_mnet_m1_loadconfig" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal szFileName As String) As Short

    ''Pulse In/Out
    Declare Function B_mnet_m1_set_pls_outmode Lib "AMONet.dll" Alias "_mnet_m1_set_pls_outmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal pls_outmode As Short) As Short
    Declare Function B_mnet_m1_set_pls_iptmode Lib "AMONet.dll" Alias "_mnet_m1_set_pls_iptmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal pls_iptmode As Short, ByVal pls_logic As Short) As Short
    Declare Function B_mnet_m1_set_feedback_src Lib "AMONet.dll" Alias "_mnet_m1_set_feedback_src" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Src As Short) As Short


    ''Motion Interface I/O
    Declare Function B_mnet_m1_set_alm Lib "AMONet.dll" Alias "_mnet_m1_set_alm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal alm_logic As Short, ByVal alm_mode As Short) As Short
    Declare Function B_mnet_m1_set_inp Lib "AMONet.dll" Alias "_mnet_m1_set_inp" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal inp_enable As Short, ByVal inp_logic As Short) As Short
    Declare Function B_mnet_m1_set_erc Lib "AMONet.dll" Alias "_mnet_m1_set_erc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal erc_logic As Short, ByVal erc_on_time As Short, ByVal erc_off_time As Short) As Short
    Declare Function B_mnet_m1_set_erc_on Lib "AMONet.dll" Alias "_mnet_m1_set_erc_on" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m1_set_autoerc Lib "AMONet.dll" Alias "_mnet_m1_set_autoerc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m1_set_ralm Lib "AMONet.dll" Alias "_mnet_m1_set_ralm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m1_set_sd Lib "AMONet.dll" Alias "_mnet_m1_set_sd" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Enable As Short, ByVal sd_logic As Short, ByVal sd_latch As Short, ByVal sd_mode As Short) As Short
    Declare Function B_mnet_m1_set_svon Lib "AMONet.dll" Alias "_mnet_m1_set_svon" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m1_set_pcs Lib "AMONet.dll" Alias "_mnet_m1_set_pcs" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal pcs_logic As Short) As Short

    Declare Function B_mnet_m1_dio_output Lib "AMONet.dll" Alias "_mnet_m1_dio_output" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DoNO As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m1_dio_input Lib "AMONet.dll" Alias "_mnet_m1_dio_input" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DoNO As Short) As Short

    ''Stop
    Declare Function B_mnet_m1_sd_stop Lib "AMONet.dll" Alias "_mnet_m1_sd_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_emg_stop Lib "AMONet.dll" Alias "_mnet_m1_emg_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short

    ''IO Monitor
    Declare Function B_mnet_m1_get_io_status Lib "AMONet.dll" Alias "_mnet_m1_get_io_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef io_sts As Integer) As Short

    '' Motion Done
    Declare Function B_mnet_m1_motion_done Lib "AMONet.dll" Alias "_mnet_m1_motion_done" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef motdone As Short) As Short

    '' Single Axis Motion
    Declare Function B_mnet_m1_set_tmove_speed Lib "AMONet.dll" Alias "_mnet_m1_set_tmove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m1_set_smove_speed Lib "AMONet.dll" Alias "_mnet_m1_set_smove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal SVacc As Double, ByVal SVdec As Double) As Short
    Declare Function B_mnet_m1_v_change Lib "AMONet.dll" Alias "_mnet_m1_v_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal NewVel As Double, ByVal TimeSecond As Double) As Short
    Declare Function B_mnet_m1_fix_speed_range Lib "AMONet.dll" Alias "_mnet_m1_fix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal MaxVel As Double) As Short
    Declare Function B_mnet_m1_unfix_speed_range Lib "AMONet.dll" Alias "_mnet_m1_unfix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_set_move_ratio Lib "AMONet.dll" Alias "_mnet_m1_set_move_ratio" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal MoveRatio As Double) As Short


    Declare Function B_mnet_m1_v_move Lib "AMONet.dll" Alias "_mnet_m1_v_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Direction As Short) As Short
    Declare Function B_mnet_m1_start_r_move Lib "AMONet.dll" Alias "_mnet_m1_start_r_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Distance As Integer) As Short
    Declare Function B_mnet_m1_start_a_move Lib "AMONet.dll" Alias "_mnet_m1_start_a_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Position As Integer) As Short
    Declare Function B_mnet_m1_p_change Lib "AMONet.dll" Alias "_mnet_m1_p_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Position As Integer) As Short

    ''Simultaneous Axis motion
    Declare Function B_mnet_m1_set_r_move_all Lib "AMONet.dll" Alias "_mnet_m1_set_r_move_all" (ByVal TotalDevice As Short, ByRef RingNoArray As Short, ByRef SlaveNoArray As Short, ByRef DistArray As Integer) As Short
    Declare Function B_mnet_m1_set_a_move_all Lib "AMONet.dll" Alias "_mnet_m1_set_a_move_all" (ByVal TotalDevice As Short, ByRef RingNoArray As Short, ByRef SlaveNoArray As Short, ByRef PosArray As Integer) As Short
    Declare Function B_mnet_m1_set_sync_stop_mode Lib "AMONet.dll" Alias "_mnet_m1_set_sync_stop_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal stop_mode As Short) As Short

    ''Position Compare and latch
    Declare Function B_mnet_m1_set_comparator_mode Lib "AMONet.dll" Alias "_mnet_m1_set_comparator_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal CmpNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short, ByVal CmpAction As Short) As Short
    Declare Function B_mnet_m1_set_comparator_data Lib "AMONet.dll" Alias "_mnet_m1_set_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal CmpNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m1_set_trigger_comparator Lib "AMONet.dll" Alias "_mnet_m1_set_trigger_comparator" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short) As Short
    Declare Function B_mnet_m1_set_trigger_comparator_data Lib "AMONet.dll" Alias "_mnet_m1_set_trigger_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m1_get_comparator_data Lib "AMONet.dll" Alias "_mnet_m1_get_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef POS As Double) As Short

    Declare Function B_mnet_m1_set_soft_limit Lib "AMONet.dll" Alias "_mnet_m1_set_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PLimit As Integer, ByVal NLimit As Integer) As Short
    Declare Function B_mnet_m1_disable_soft_limit Lib "AMONet.dll" Alias "_mnet_m1_disable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_enable_soft_limit Lib "AMONet.dll" Alias "_mnet_m1_enable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Action As Short) As Short
    Declare Function B_mnet_m1_set_ltc_logic Lib "AMONet.dll" Alias "_mnet_m1_set_ltc_logic" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal let_logic As Short) As Short
    Declare Function B_mnet_m1_get_latch_data Lib "AMONet.dll" Alias "_mnet_m1_get_latch_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal LtcNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m1_start_soft_ltc Lib "AMONet.dll" Alias "_mnet_m1_start_soft_ltc" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short

    ''Counter Operating
    Declare Function B_mnet_m1_get_command Lib "AMONet.dll" Alias "_mnet_m1_get_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef CMD As Integer) As Short
    Declare Function B_mnet_m1_set_command Lib "AMONet.dll" Alias "_mnet_m1_set_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal CMD As Integer) As Short
    Declare Function B_mnet_m1_reset_command Lib "AMONet.dll" Alias "_mnet_m1_reset_command" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_get_position Lib "AMONet.dll" Alias "_mnet_m1_get_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef POS As Integer) As Short
    Declare Function B_mnet_m1_set_position Lib "AMONet.dll" Alias "_mnet_m1_set_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal POS As Integer) As Short
    Declare Function B_mnet_m1_reset_position Lib "AMONet.dll" Alias "_mnet_m1_reset_position" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m1_get_error_counter Lib "AMONet.dll" Alias "_mnet_m1_get_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef ErrCnt As Integer) As Short
    Declare Function B_mnet_m1_reset_error_counter Lib "AMONet.dll" Alias "_mnet_m1_reset_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short

    Declare Function B_mnet_m1_get_current_speed Lib "AMONet.dll" Alias "_mnet_m1_get_current_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef CurSpeed As Double) As Short


    '' Homing
    Declare Function B_mnet_m1_set_home_config Lib "AMONet.dll" Alias "_mnet_m1_set_home_config" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal home_mode As Short, ByVal org_logic As Short, ByVal ez_logic As Short, ByVal ez_count As Short, ByVal ERC_Out As Short) As Short

    Declare Function B_mnet_m1_start_home_move Lib "AMONet.dll" Alias "_mnet_m1_start_home_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Direction As Short) As Short



    ''
    '' AMAX2240
    ''

    '' Axis Slave Operation
    Declare Function B_mnet_m4_initial Lib "AMONet.dll" Alias "_mnet_m4_initial" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m4_set_com_wdg_mode Lib "AMONet.dll" Alias "_mnet_m4_set_com_wdg_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal StopMode As Short) As Short
    Declare Function B_mnet_m4_loadconfig Lib "AMONet.dll" Alias "_mnet_m4_loadconfig" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal szFileName As String) As Short
    Declare Function B_mnet_m4_set_fw_memory Lib "AMONet.dll" Alias "_mnet_m4_set_fw_memory" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Mode As Short) As Short

    ''Pulse In/Out
    Declare Function B_mnet_m4_set_pls_outmode Lib "AMONet.dll" Alias "_mnet_m4_set_pls_outmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal pls_outmode As Short) As Short
    Declare Function B_mnet_m4_set_pls_iptmode Lib "AMONet.dll" Alias "_mnet_m4_set_pls_iptmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal pls_iptmode As Short, ByVal pls_logic As Short) As Short
    Declare Function B_mnet_m4_set_feedback_src Lib "AMONet.dll" Alias "_mnet_m4_set_feedback_src" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Src As Short) As Short


    ''Motion Interface I/O
    Declare Function B_mnet_m4_set_alm Lib "AMONet.dll" Alias "_mnet_m4_set_alm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal alm_logic As Short, ByVal alm_mode As Short) As Short
    Declare Function B_mnet_m4_set_inp Lib "AMONet.dll" Alias "_mnet_m4_set_inp" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal inp_enable As Short, ByVal inp_logic As Short) As Short
    Declare Function B_mnet_m4_set_erc Lib "AMONet.dll" Alias "_mnet_m4_set_erc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal erc_logic As Short, ByVal erc_on_time As Short, ByVal erc_off_time As Short) As Short
    Declare Function B_mnet_m4_set_erc_on Lib "AMONet.dll" Alias "_mnet_m4_set_erc_on" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m4_set_autoerc Lib "AMONet.dll" Alias "_mnet_m4_set_autoerc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m4_set_ralm Lib "AMONet.dll" Alias "_mnet_m4_set_ralm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m4_set_sd Lib "AMONet.dll" Alias "_mnet_m4_set_sd" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Enable As Short, ByVal sd_logic As Short, ByVal sd_latch As Short, ByVal sd_mode As Short) As Short
    Declare Function B_mnet_m4_set_svon Lib "AMONet.dll" Alias "_mnet_m4_set_svon" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m4_set_el Lib "AMONet.dll" Alias "_mnet_m4_set_el" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal el_mode As Short) As Short
    'dan.yang 2012.12.29
    Declare Function B_mnet_m4_set_abs_mode Lib "AMONet.dll" Alias "_mnet_m4_set_abs_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal abs_mode As Short) As Short


    ''Stop
    Declare Function B_mnet_m4_sd_stop Lib "AMONet.dll" Alias "_mnet_m4_sd_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_emg_stop Lib "AMONet.dll" Alias "_mnet_m4_emg_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_pause_motion Lib "AMONet.dll" Alias "_mnet_m4_pause_motion" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_resume_motion Lib "AMONet.dll" Alias "_mnet_m4_resume_motion" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    ''IO Monitor
    Declare Function B_mnet_m4_get_io_status Lib "AMONet.dll" Alias "_mnet_m4_get_io_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef io_sts As Integer) As Short

    '' Motion Done
    Declare Function B_mnet_m4_motion_done Lib "AMONet.dll" Alias "_mnet_m4_motion_done" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef motdone As Short) As Short
    Declare Function B_mnet_m4_error_status Lib "AMONet.dll" Alias "_mnet_m4_error_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef ErrSt As Integer) As Short
    Declare Function B_mnet_m4_rist_status Lib "AMONet.dll" Alias "_mnet_m4_rist_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef IntSt As Integer) As Short

    '' Single Axis Motion
    Declare Function B_mnet_m4_set_tmove_speed Lib "AMONet.dll" Alias "_mnet_m4_set_tmove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_set_smove_speed Lib "AMONet.dll" Alias "_mnet_m4_set_smove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal SVacc As Double, ByVal SVdec As Double) As Short
    Declare Function B_mnet_m4_v_change Lib "AMONet.dll" Alias "_mnet_m4_v_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal NewVel As Double, ByVal TimeSecond As Double) As Short
    Declare Function B_mnet_m4_fix_speed_range Lib "AMONet.dll" Alias "_mnet_m4_fix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal MaxVel As Double) As Short
    Declare Function B_mnet_m4_unfix_speed_range Lib "AMONet.dll" Alias "_mnet_m4_unfix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_set_move_ratio Lib "AMONet.dll" Alias "_mnet_m4_set_move_ratio" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal MoveRatio As Double) As Short
    Declare Function B_mnet_m4_set_backlash_comp Lib "AMONet.dll" Alias "_mnet_m4_set_backlash_comp" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal BcompPulse As Short, ByVal Mode As Short) As Short
    Declare Function B_mnet_m4_set_suppress_vibration Lib "AMONet.dll" Alias "_mnet_m4_set_suppress_vibration" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal ReverseTime As Short, ByVal ForwardTime As Short) As Short
    Declare Function B_mnet_m4_v_change_all Lib "AMONet.dll" Alias "_mnet_m4_v_change_all" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNoArray As Short(), ByVal NewVelArray As Double(), ByVal TimeSecArray As Double(), ByVal Count As Short) As Short

    '' Speed Motion

    Declare Function B_mnet_m4_v_move Lib "AMONet.dll" Alias "_mnet_m4_v_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short
    Declare Function B_mnet_m4_start_r_move Lib "AMONet.dll" Alias "_mnet_m4_start_r_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Distance As Integer) As Short
    Declare Function B_mnet_m4_start_a_move Lib "AMONet.dll" Alias "_mnet_m4_start_a_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Position As Integer) As Short
    Declare Function B_mnet_m4_p_change Lib "AMONet.dll" Alias "_mnet_m4_p_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Position As Integer) As Short
    Declare Function B_mnet_m4_p_change_r Lib "AMONet.dll" Alias "_mnet_m4_p_change_r" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Position As Integer) As Short
    Declare Function B_mnet_m4_p_change_all Lib "AMONet.dll" Alias "_mnet_m4_p_change_all" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNoArray As Short(), ByVal PositionArray As Integer(), ByVal Count As Short) As Short
    Declare Function B_mnet_m4_p_change_r_all Lib "AMONet.dll" Alias "_mnet_m4_p_change_r_all" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNoArray As Short(), ByVal RelPositionArray As Integer(), ByVal Count As Short) As Short


    ''Position Compare and latch
    Declare Function B_mnet_m4_set_comparator_mode Lib "AMONet.dll" Alias "_mnet_m4_set_comparator_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short, ByVal CmpAction As Short) As Short
    Declare Function B_mnet_m4_set_comparator_data Lib "AMONet.dll" Alias "_mnet_m4_set_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m4_get_comparator_data Lib "AMONet.dll" Alias "_mnet_m4_get_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m4_set_trigger_comparator Lib "AMONet.dll" Alias "_mnet_m4_set_trigger_comparator" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short) As Short
    Declare Function B_mnet_m4_set_trigger_comparator_data Lib "AMONet.dll" Alias "_mnet_m4_set_trigger_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m4_get_trigger_comparator_data Lib "AMONet.dll" Alias "_mnet_m4_get_trigger_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m4_set_trigger_comparator_pulse Lib "AMONet.dll" Alias "_mnet_m4_set_trigger_comparator_pulse" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal PulseMode As Short, ByVal Logic As Short, ByVal PulseWidth As Short) As Short
    Declare Function B_mnet_m4_set_trigger_comparator_table Lib "AMONet.dll" Alias "_mnet_m4_set_trigger_comparator_table" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal TableArray As Double(), ByVal TableSize As Integer) As Short
    Declare Function B_mnet_m4_set_trigger_comparator_auto Lib "AMONet.dll" Alias "_mnet_m4_set_trigger_comparator_auto" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StartData As Double, ByVal EndData As Double, ByVal Interval As Integer) As Short
    Declare Function B_mnet_m4_reset_trigger_comparator_level Lib "AMONet.dll" Alias "_mnet_m4_reset_trigger_comparator_level" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_get_trigger_comparator_level Lib "AMONet.dll" Alias "_mnet_m4_get_trigger_comparator_level" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef CmpLevel As Short) As Short

	  ''Internal synchronous motion
    Declare Function B_mnet_m4_set_sync_option Lib "AMONet.dll" Alias "_mnet_m4_set_sync_option" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal mode As Short) As Short
    Declare Function B_mnet_m4_set_sync_signal_source Lib "AMONet.dll" Alias "_mnet_m4_set_sync_signal_source" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal mode As Short) As Short
    Declare Function B_mnet_m4_set_sync_signal_mode Lib "AMONet.dll" Alias "_mnet_m4_set_sync_signal_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal mode As Short) As Short

    Declare Function B_mnet_m4_set_soft_limit Lib "AMONet.dll" Alias "_mnet_m4_set_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal PLimit As Integer, ByVal NLimit As Integer) As Short
    Declare Function B_mnet_m4_disable_soft_limit Lib "AMONet.dll" Alias "_mnet_m4_disable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_enable_soft_limit Lib "AMONet.dll" Alias "_mnet_m4_enable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Action As Short) As Short
    Declare Function B_mnet_m4_set_ltc_logic Lib "AMONet.dll" Alias "_mnet_m4_set_ltc_logic" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal let_logic As Short) As Short
    Declare Function B_mnet_m4_set_ltc_enable Lib "AMONet.dll" Alias "_mnet_m4_set_ltc_enable" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal ltc_enable As Short) As Short

    Declare Function B_mnet_m4_steplose_check Lib "AMONet.dll" Alias "_mnet_m4_steplose_check" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Tolerance As Short) As Short
    Declare Function B_mnet_m4_get_latch_data Lib "AMONet.dll" Alias "_mnet_m4_get_latch_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal LtcNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m4_start_soft_ltc Lib "AMONet.dll" Alias "_mnet_m4_start_soft_ltc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    ''Counter Operating
    Declare Function B_mnet_m4_get_command Lib "AMONet.dll" Alias "_mnet_m4_get_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef CMD As Integer) As Short
    Declare Function B_mnet_m4_set_command Lib "AMONet.dll" Alias "_mnet_m4_set_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CMD As Integer) As Short
    Declare Function B_mnet_m4_reset_command Lib "AMONet.dll" Alias "_mnet_m4_reset_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_get_position Lib "AMONet.dll" Alias "_mnet_m4_get_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef POS As Integer) As Short
    Declare Function B_mnet_m4_set_position Lib "AMONet.dll" Alias "_mnet_m4_set_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal POS As Integer) As Short
    Declare Function B_mnet_m4_reset_position Lib "AMONet.dll" Alias "_mnet_m4_reset_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_get_error_counter Lib "AMONet.dll" Alias "_mnet_m4_get_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef ErrCnt As Integer) As Short
    Declare Function B_mnet_m4_reset_error_counter Lib "AMONet.dll" Alias "_mnet_m4_reset_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    Declare Function B_mnet_m4_get_current_speed Lib "AMONet.dll" Alias "_mnet_m4_get_current_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef CurSpeed As Double) As Short


    '' Homing
    Declare Function B_mnet_m4_set_home_config Lib "AMONet.dll" Alias "_mnet_m4_set_home_config" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal home_mode As Short, ByVal org_logic As Short, ByVal ez_logic As Short, ByVal ez_count As Short, ByVal ERC_Out As Short) As Short

    Declare Function B_mnet_m4_start_home_move Lib "AMONet.dll" Alias "_mnet_m4_start_home_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short

    Declare Function B_mnet_m4_start_home_search Lib "AMONet.dll" Alias "_mnet_m4_start_home_search" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short, ByVal ORGOffset As Integer) As Short

    Declare Function B_mnet_m4_start_home_z Lib "AMONet.dll" Alias "_mnet_m4_start_home_z" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short
    Declare Function B_mnet_m4_enable_home_reset Lib "AMONet.dll" Alias "_mnet_m4_enable_home_reset" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Enable As Short) As Short


    Declare Function B_mnet_m4_start_tr_move_xy Lib "AMONet.dll" Alias "_mnet_m4_start_tr_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_move_xy Lib "AMONet.dll" Alias "_mnet_m4_start_ta_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_move_xy Lib "AMONet.dll" Alias "_mnet_m4_start_sr_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_move_xy Lib "AMONet.dll" Alias "_mnet_m4_start_sa_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_tr_move_zu Lib "AMONet.dll" Alias "_mnet_m4_start_tr_move_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistZ As Integer, ByVal DistU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_move_zu Lib "AMONet.dll" Alias "_mnet_m4_start_ta_move_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosZ As Integer, ByVal PosU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_move_zu Lib "AMONet.dll" Alias "_mnet_m4_start_sr_move_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistZ As Integer, ByVal DistU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_move_zu Lib "AMONet.dll" Alias "_mnet_m4_start_sa_move_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosZ As Integer, ByVal PosU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_tr_line2 Lib "AMONet.dll" Alias "_mnet_m4_start_tr_line2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_line2 Lib "AMONet.dll" Alias "_mnet_m4_start_ta_line2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_line2 Lib "AMONet.dll" Alias "_mnet_m4_start_sr_line2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_line2 Lib "AMONet.dll" Alias "_mnet_m4_start_sa_line2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_tr_line3 Lib "AMONet.dll" Alias "_mnet_m4_start_tr_line3" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal DistZ As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_line3 Lib "AMONet.dll" Alias "_mnet_m4_start_ta_line3" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal PosZ As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_line3 Lib "AMONet.dll" Alias "_mnet_m4_start_sr_line3" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal DistZ As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_line3 Lib "AMONet.dll" Alias "_mnet_m4_start_sa_line3" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal PosZ As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_tr_line4 Lib "AMONet.dll" Alias "_mnet_m4_start_tr_line4" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal DistZ As Integer, ByVal DistU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_line4 Lib "AMONet.dll" Alias "_mnet_m4_start_ta_line4" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal PosZ As Integer, ByVal PosU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_line4 Lib "AMONet.dll" Alias "_mnet_m4_start_sr_line4" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal DistZ As Integer, ByVal DistU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_line4 Lib "AMONet.dll" Alias "_mnet_m4_start_sa_line4" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal PosZ As Integer, ByVal PosU As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short



    Declare Function B_mnet_m4_start_tr_arc_xy Lib "AMONet.dll" Alias "_mnet_m4_start_tr_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_ta_arc_xy Lib "AMONet.dll" Alias "_mnet_m4_start_ta_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sr_arc_xy Lib "AMONet.dll" Alias "_mnet_m4_start_sr_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sa_arc_xy Lib "AMONet.dll" Alias "_mnet_m4_start_sa_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short


    Declare Function B_mnet_m4_start_tr_arc_zu Lib "AMONet.dll" Alias "_mnet_m4_start_tr_arc_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCz As Integer, ByVal OffsetCu As Integer, ByVal OffsetEz As Integer, ByVal OffsetEu As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_ta_arc_zu Lib "AMONet.dll" Alias "_mnet_m4_start_ta_arc_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cz As Integer, ByVal Cu As Integer, ByVal Ez As Integer, ByVal Eu As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sr_arc_zu Lib "AMONet.dll" Alias "_mnet_m4_start_sr_arc_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCz As Integer, ByVal OffsetCu As Integer, ByVal OffsetEz As Integer, ByVal OffsetEu As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sa_arc_zu Lib "AMONet.dll" Alias "_mnet_m4_start_sa_arc_zu" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cz As Integer, ByVal Cu As Integer, ByVal Ez As Integer, ByVal Eu As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short


    Declare Function B_mnet_m4_start_tr_arc2 Lib "AMONet.dll" Alias "_mnet_m4_start_tr_arc2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_ta_arc2 Lib "AMONet.dll" Alias "_mnet_m4_start_ta_arc2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sr_arc2 Lib "AMONet.dll" Alias "_mnet_m4_start_sr_arc2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_sa_arc2 Lib "AMONet.dll" Alias "_mnet_m4_start_sa_arc2" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_start_tr_arc_3p Lib "AMONet.dll" Alias "_mnet_m4_start_tr_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal OffsetRx As Integer, ByVal OffsetRy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_ta_arc_3p Lib "AMONet.dll" Alias "_mnet_m4_start_ta_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal Rx As Integer, ByVal Ry As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sr_arc_3p Lib "AMONet.dll" Alias "_mnet_m4_start_sr_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal OffsetRx As Integer, ByVal OffsetRy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m4_start_sa_arc_3p Lib "AMONet.dll" Alias "_mnet_m4_start_sa_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef AxisArray As Short, ByVal Rx As Integer, ByVal Ry As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short


    Declare Function B_mnet_m4_set_path_move_speed Lib "AMONet.dll" Alias "_mnet_m4_set_path_move_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal TorS As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m4_set_path_arc_data Lib "AMONet.dll" Alias "_mnet_m4_set_path_arc_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray As Short(), ByVal CmdFunc As Short, ByVal CenArray As Integer(), ByVal EndArray As Integer(), ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal EnableDec As Short) As Short
    Declare Function B_mnet_m4_set_path_line_data Lib "AMONet.dll" Alias "_mnet_m4_set_path_line_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray As Short(), ByVal CmdFunc As Short, ByVal DistArray As Integer(), ByVal StrVel As Double, ByVal MaxVel As Double, ByVal EnableDec As Short) As Short

    Declare Function B_mnet_m4_start_path Lib "AMONet.dll" Alias "_mnet_m4_start_path" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m4_reset_path Lib "AMONet.dll" Alias "_mnet_m4_reset_path" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short
    Declare Function B_mnet_m4_get_path_status Lib "AMONet.dll" Alias "_mnet_m4_get_path_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByRef CurIndex As Integer, ByRef CurCmdFunc As Short, ByRef StockCmdCount As Integer, ByRef FreeSpaceCount As Integer) As Short


    '' Analog Input & Output Function
    Declare Function B_mnet_aio_initial Lib "AMONet.dll" Alias "_mnet_aio_initial" (ByVal RingNo As Short, ByVal DeviceIP As Short) As Short
    Declare Function B_mnet_aio_get_feature Lib "AMONet.dll" Alias "_mnet_aio_get_feature" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByRef pBuf As PT_AIOFeature) As Short

    Declare Function B_mnet_ai_set_con_mode Lib "AMONet.dll" Alias "_mnet_ai_set_con_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal ptrChanConfig As Byte(), ByVal usSize As Integer) As Short
    Declare Function B_mnet_ai_get_con_mode Lib "AMONet.dll" Alias "_mnet_ai_get_con_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal ptrChanConfig As Byte(), ByRef usSize As Integer) As Short

    Declare Function B_mnet_ai_config Lib "AMONet.dll" Alias "_mnet_ai_config" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByVal Gain As Short) As Short
    Declare Function B_mnet_ai_binary_in Lib "AMONet.dll" Alias "_mnet_ai_binary_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByRef Data As Short) As Short
    Declare Function B_mnet_ai_voltage_in Lib "AMONet.dll" Alias "_mnet_ai_voltage_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByRef Data As Single) As Short
    Declare Function B_mnet_ai_current_in Lib "AMONet.dll" Alias "_mnet_ai_current_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByRef Data As Single) As Short
    Declare Function B_mnet_mai_config Lib "AMONet.dll" Alias "_mnet_mai_config" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal NumChan As Short, ByVal StartChan As Short, ByVal Gain As Short()) As Short
    Declare Function B_mnet_mai_binary_in Lib "AMONet.dll" Alias "_mnet_mai_binary_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal NumChan As Short, ByVal StartChan As Short, ByVal Data As Short()) As Short
    Declare Function B_mnet_mai_voltage_in Lib "AMONet.dll" Alias "_mnet_mai_voltage_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal NumChan As Short, ByVal StartChan As Short, ByVal fData As Single()) As Short
    Declare Function B_mnet_mai_current_in Lib "AMONet.dll" Alias "_mnet_mai_current_in" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal NumChan As Short, ByVal StartChan As Short, ByVal fData As Single()) As Short

    Declare Function B_mnet_ao_config Lib "AMONet.dll" Alias "_mnet_ao_config" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByVal Range As Short) As Short
    Declare Function B_mnet_ao_binary_out Lib "AMONet.dll" Alias "_mnet_ao_binary_out" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByVal Data As Short) As Short
    Declare Function B_mnet_ao_voltage_out Lib "AMONet.dll" Alias "_mnet_ao_voltage_out" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByVal Data As Single) As Short
    Declare Function B_mnet_ao_current_out Lib "AMONet.dll" Alias "_mnet_ao_current_out" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal Chan As Short, ByVal Data As Single) As Short

    ''Simultaneous Axis motion
    'Declare Function B_mnet_m4_set_r_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_r_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DistArray As Integer) As Short
    'Declare Function B_mnet_m4_set_a_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_a_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef PosArray As Integer) As Short
    'Declare Function B_mnet_m4_set_v_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_v_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DirArray As Short) As Short
    '-----------------------------------------------
    '   Delta Hsien , 2015.01.23 , correct data-type
    '-----------------------------------------------
    Declare Function B_mnet_m4_set_r_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_r_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DistArray As Integer) As Short
    Declare Function B_mnet_m4_set_a_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_a_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef PosArray As Integer) As Short
    Declare Function B_mnet_m4_set_v_move_all Lib "AMONet.dll" Alias "_mnet_m4_set_v_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DirArray As Integer) As Short

    Declare Function B_mnet_m4_start_move_all Lib "AMONet.dll" Alias "_mnet_m4_start_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal MasterNo As Short) As Short
    Declare Function B_mnet_m4_stop_move_all Lib "AMONet.dll" Alias "_mnet_m4_stop_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal MasterNo As Short) As Short
    'Break
    Declare Function B_mnet_m4_set_break_on Lib "AMONet.dll" Alias "_mnet_m4_set_break_on" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal OnOff As Short) As Short
    Declare Function B_mnet_m4_set_auto_break Lib "AMONet.dll" Alias "_mnet_m4_set_auto_break" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_get_auto_break_status Lib "AMONet.dll" Alias "_mnet_m4_get_auto_break_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef Status As Integer) As Short
    Declare Function B_mnet_m4_set_svon_brktime Lib "AMONet.dll" Alias "_mnet_m4_set_svon_brktime" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal data As Integer) As Short
    Declare Function B_mnet_m4_set_svoff_brktime Lib "AMONet.dll" Alias "_mnet_m4_set_svoff_brktime" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal data As Integer) As Short
    Declare Function B_mnet_m4_get_svon_break_time Lib "AMONet.dll" Alias "_mnet_m4_get_svon_break_time" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef data As Integer) As Short
    Declare Function B_mnet_m4_get_svoff_break_time Lib "AMONet.dll" Alias "_mnet_m4_get_svoff_break_time" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef data As Integer) As Short

    'EEPROM
    Declare Function B_mnet_m4_get_eeprom Lib "AMONet.dll" Alias "_mnet_m4_get_eeprom" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PrivateID As Short, ByVal PassWrd_1 As UInteger, ByVal PassWrd_2 As UInteger, ByRef Data_1 As UInteger, ByRef Data_2 As UInteger) As Short
    Declare Function B_mnet_m4_set_eeprom Lib "AMONet.dll" Alias "_mnet_m4_set_eeprom" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PrivateID As Short, ByVal PassWrd_1 As UInteger, ByVal PassWrd_2 As UInteger, ByVal Data_1 As UInteger, ByVal Data_2 As UInteger) As Short

    '================2 axis module [yuzhi.wang 06/08/2012]====================================================='
    Declare Function B_mnet_m2_initial Lib "AMONet.dll" Alias "_mnet_m2_initial" (ByVal RingNo As Short, ByVal SlaveNo As Short) As Short

    ''Pulse In/Out
    Declare Function B_mnet_m2_set_pls_outmode Lib "AMONet.dll" Alias "_mnet_m2_set_pls_outmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal pls_outmode As Short) As Short
    Declare Function B_mnet_m2_set_pls_iptmode Lib "AMONet.dll" Alias "_mnet_m2_set_pls_iptmode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal pls_iptmode As Short, ByVal pls_logic As Short) As Short
    Declare Function B_mnet_m2_set_feedback_src Lib "AMONet.dll" Alias "_mnet_m2_set_feedback_src" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Src As Short) As Short
    Declare Function B_mnet_m2_set_svon Lib "AMONet.dll" Alias "_mnet_m2_set_svon" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short

    ''Motion Interface I/O
    Declare Function B_mnet_m2_set_alm Lib "AMONet.dll" Alias "_mnet_m2_set_alm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal alm_logic As Short, ByVal alm_mode As Short) As Short
    Declare Function B_mnet_m2_set_inp Lib "AMONet.dll" Alias "_mnet_m2_set_inp" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal inp_enable As Short, ByVal inp_logic As Short) As Short
    Declare Function B_mnet_m2_set_erc Lib "AMONet.dll" Alias "_mnet_m2_set_erc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal erc_logic As Short, ByVal erc_on_time As Short, ByVal erc_off_time As Short) As Short
    Declare Function B_mnet_m2_set_erc_on Lib "AMONet.dll" Alias "_mnet_m2_set_erc_on" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m2_set_autoerc Lib "AMONet.dll" Alias "_mnet_m2_set_autoerc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m2_set_ralm Lib "AMONet.dll" Alias "_mnet_m2_set_ralm" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal on_off As Short) As Short
    Declare Function B_mnet_m2_set_sd Lib "AMONet.dll" Alias "_mnet_m2_set_sd" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Enable As Short, ByVal sd_logic As Short, ByVal sd_latch As Short, ByVal sd_mode As Short) As Short
    Declare Function B_mnet_m2_set_el Lib "AMONet.dll" Alias "_mnet_m2_set_el" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal el_mode As Short) As Short
    Declare Function B_mnet_m2_set_pcs Lib "AMONet.dll" Alias "_mnet_m2_set_pcs" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal pcs_logic As Short) As Short
    'dan.yang 2012.12.29
    Declare Function B_mnet_m2_set_abs_mode Lib "AMONet.dll" Alias "_mnet_m2_set_abs_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal abs_mode As Short) As Short

    Declare Function B_mnet_m2_dio_output Lib "AMONet.dll" Alias "_mnet_m2_dio_output" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DoNo As Short, ByVal ON_OFF As Short) As Short
    Declare Function B_mnet_m2_dio_input Lib "AMONet.dll" Alias "_mnet_m2_dio_input" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DiNo As Short) As Short
    Declare Function B_mnet_m2_dio_channel_output Lib "AMONet.dll" Alias "_mnet_m2_dio_channel_output" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Channel As Short, ByVal OutData As Short) As Short
    Declare Function B_mnet_m2_dio_channel_input Lib "AMONet.dll" Alias "_mnet_m2_dio_channel_input" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Channel As Short, ByRef InData As Short) As Short

    Declare Function B_mnet_m2_sd_stop Lib "AMONet.dll" Alias "_mnet_m2_sd_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_emg_stop Lib "AMONet.dll" Alias "_mnet_m2_emg_stop" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    'IO Monitor
    Declare Function B_mnet_m2_get_io_status Lib "AMONet.dll" Alias "_mnet_m2_get_io_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef io_sts As Integer) As Short

    'Motion Status
    Declare Function B_mnet_m2_motion_done Lib "AMONet.dll" Alias "_mnet_m2_motion_done" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef motdone As Short) As Short

    'Single Axis Speed
    Declare Function B_mnet_m2_set_tmove_speed Lib "AMONet.dll" Alias "_mnet_m2_set_tmove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_set_smove_speed Lib "AMONet.dll" Alias "_mnet_m2_set_smove_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal SVacc As Double, ByVal SVdec As Double) As Short
    Declare Function B_mnet_m2_v_change Lib "AMONet.dll" Alias "_mnet_m2_v_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal NewVel As Double, ByVal TimeSecond As Double) As Short
    Declare Function B_mnet_m2_fix_speed_range Lib "AMONet.dll" Alias "_mnet_m2_fix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal MaxVel As Double) As Short
    Declare Function B_mnet_m2_unfix_speed_range Lib "AMONet.dll" Alias "_mnet_m2_unfix_speed_range" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_p_change Lib "AMONet.dll" Alias "_mnet_m2_p_change" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Position As Integer) As Short

    'Single Axis Motion
    Declare Function B_mnet_m2_v_move Lib "AMONet.dll" Alias "_mnet_m2_v_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short
    Declare Function B_mnet_m2_start_r_move Lib "AMONet.dll" Alias "_mnet_m2_start_r_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Distance As Integer) As Short
    Declare Function B_mnet_m2_start_a_move Lib "AMONet.dll" Alias "_mnet_m2_start_a_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Position As Integer) As Short

    'Simultaneous Axis motion
    'Declare Function B_mnet_m2_set_r_move_all Lib "AMONet.dll" Alias "_mnet_m2_set_r_move_all" (ByVal TotalDevice As Short, ByRef RingNoArray As Short, ByRef SlaveNoArray As Short, ByRef DistArray As Integer) As Short
    'Declare Function B_mnet_m2_set_a_move_all Lib "AMONet.dll" Alias "_mnet_m2_set_a_move_all" (ByVal TotalDevice As Short, ByRef RingNoArray As Short, ByRef SlaveNoArray As Short, ByRef PosArray As Integer) As Short
    Declare Function B_mnet_m2_set_r_move_all Lib "AMONet.dll" Alias "_mnet_m2_set_r_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DistArray As Integer) As Short
    Declare Function B_mnet_m2_set_a_move_all Lib "AMONet.dll" Alias "_mnet_m2_set_a_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef PosArray As Integer) As Short
    Declare Function B_mnet_m2_set_v_move_all Lib "AMONet.dll" Alias "_mnet_m2_set_v_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal TotalDevice As Short, ByRef SlaveNoArray As Short, ByRef DirArray As Short) As Short
    Declare Function B_mnet_m2_start_move_all Lib "AMONet.dll" Alias "_mnet_m2_start_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal MasterNo As Short) As Short
    Declare Function B_mnet_m2_stop_move_all Lib "AMONet.dll" Alias "_mnet_m2_stop_move_all" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal MasterNo As Short) As Short
    Declare Function B_mnet_m2_set_sync_stop_mode Lib "AMONet.dll" Alias "_mnet_m2_set_sync_stop_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal stop_mode As Short) As Short

    'Position Compare and Latch
    Declare Function B_mnet_m2_set_comparator_mode Lib "AMONet.dll" Alias "_mnet_m2_set_comparator_mode" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short, ByVal CmpAction As Short) As Short
    Declare Function B_mnet_m2_set_comparator_data Lib "AMONet.dll" Alias "_mnet_m2_set_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m2_set_trigger_comparator Lib "AMONet.dll" Alias "_mnet_m2_set_trigger_comparator" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short) As Short
    Declare Function B_mnet_m2_set_trigger_comparator_data Lib "AMONet.dll" Alias "_mnet_m2_set_trigger_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Data As Double) As Short
    Declare Function B_mnet_m2_get_comparator_data Lib "AMONet.dll" Alias "_mnet_m2_get_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CmpNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m2_get_trigger_comparator_data Lib "AMONet.dll" Alias "_mnet_m2_get_trigger_comparator_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m2_set_trigger_comparator_pulse Lib "AMONet.dll" Alias "_mnet_m2_set_trigger_comparator_pulse" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal PulseMode As Short, ByVal Logic As Short, ByVal PulseWidth As Short) As Short
    Declare Function B_mnet_m2_reset_trigger_comparator_level Lib "AMONet.dll" Alias "_mnet_m2_reset_trigger_comparator_level" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    'Soft Limit
    Declare Function B_mnet_m2_set_soft_limit Lib "AMONet.dll" Alias "_mnet_m2_set_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal PLimit As Integer, ByVal NLimit As Integer) As Short
    Declare Function B_mnet_m2_disable_soft_limit Lib "AMONet.dll" Alias "_mnet_m2_disable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_enable_soft_limit Lib "AMONet.dll" Alias "_mnet_m2_enable_soft_limit" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Action As Short) As Short

    'Latch
    Declare Function B_mnet_m2_set_ltc_logic Lib "AMONet.dll" Alias "_mnet_m2_set_ltc_logic" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal let_logic As Short) As Short
    Declare Function B_mnet_m2_get_latch_data Lib "AMONet.dll" Alias "_mnet_m2_get_latch_data" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal LtcNo As Short, ByRef POS As Double) As Short
    Declare Function B_mnet_m2_start_soft_ltc Lib "AMONet.dll" Alias "_mnet_m2_start_soft_ltc" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    'Counter Operating
    Declare Function B_mnet_m2_get_command Lib "AMONet.dll" Alias "_mnet_m2_get_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef CMD As Integer) As Short
    Declare Function B_mnet_m2_set_command Lib "AMONet.dll" Alias "_mnet_m2_set_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal CMD As Integer) As Short
    Declare Function B_mnet_m2_reset_command Lib "AMONet.dll" Alias "_mnet_m2_reset_command" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_get_position Lib "AMONet.dll" Alias "_mnet_m2_get_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef POS As Integer) As Short
    Declare Function B_mnet_m2_set_position Lib "AMONet.dll" Alias "_mnet_m2_set_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal POS As Integer) As Short
    Declare Function B_mnet_m2_reset_position Lib "AMONet.dll" Alias "_mnet_m2_reset_position" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_get_error_counter Lib "AMONet.dll" Alias "_mnet_m2_get_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef ErrCnt As Integer) As Short
    Declare Function B_mnet_m2_reset_error_counter Lib "AMONet.dll" Alias "_mnet_m2_reset_error_counter" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short

    Declare Function B_mnet_m2_get_current_speed Lib "AMONet.dll" Alias "_mnet_m2_get_current_speed" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef CurSpeed As Double) As Short
    Declare Function B_mnet_m2_set_move_ratio Lib "AMONet.dll" Alias "_mnet_m2_set_move_ratio" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal MoveRatio As Double) As Short

    'Home
    Declare Function B_mnet_m2_set_home_config Lib "AMONet.dll" Alias "_mnet_m2_set_home_config" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal home_mode As Short, ByVal org_logic As Short, ByVal ez_logic As Short, ByVal ez_count As Short, ByVal ERC_Out As Short) As Short
    Declare Function B_mnet_m2_start_home_move Lib "AMONet.dll" Alias "_mnet_m2_start_home_move" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short
    Declare Function B_mnet_m2_start_home_escape Lib "AMONet.dll" Alias "_mnet_m2_start_home_escape" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal Direction As Short) As Short

    'Line
    Declare Function B_mnet_m2_start_tr_move_xy Lib "AMONet.dll" Alias "_mnet_m2_start_tr_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_ta_move_xy Lib "AMONet.dll" Alias "_mnet_m2_start_ta_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sr_move_xy Lib "AMONet.dll" Alias "_mnet_m2_start_sr_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal DistX As Integer, ByVal DistY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sa_move_xy Lib "AMONet.dll" Alias "_mnet_m2_start_sa_move_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal PosX As Integer, ByVal PosY As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    'Arc
    Declare Function B_mnet_m2_start_tr_arc_xy Lib "AMONet.dll" Alias "_mnet_m2_start_tr_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_ta_arc_xy Lib "AMONet.dll" Alias "_mnet_m2_start_ta_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sr_arc_xy Lib "AMONet.dll" Alias "_mnet_m2_start_sr_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sa_arc_xy Lib "AMONet.dll" Alias "_mnet_m2_start_sa_arc_xy" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_tr_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_start_tr_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray() As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_ta_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_start_ta_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray() As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sr_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_start_sr_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray() As Short, ByVal OffsetCx As Integer, ByVal OffsetCy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_start_sa_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_start_sa_arc_3p" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisArray() As Short, ByVal Cx As Integer, ByVal Cy As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal Direction As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    'Compare
    'Declare Function B_mnet_m2_set_trigger_comparator_table Lib "AMONet.dll" Alias "_mnet_m2_set_trigger_comparator_table" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef TableArray As Double, ByVal TableSize As Integer) As Short
    Declare Function B_mnet_m2_set_trigger_comparator_auto Lib "AMONet.dll" Alias "_mnet_m2_set_trigger_comparator_auto" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal StartData As Double, ByVal EndData As Double, ByVal Interval As Integer) As Short

    'Break
    Declare Function B_mnet_m2_set_break_on Lib "AMONet.dll" Alias "_mnet_m2_set_break_on" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal OnOff As Short) As Short
    Declare Function B_mnet_m2_set_auto_break Lib "AMONet.dll" Alias "_mnet_m2_set_auto_break" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m2_get_auto_break_status Lib "AMONet.dll" Alias "_mnet_m2_get_auto_break_status" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef Status As Integer) As Short
    Declare Function B_mnet_m2_set_svon_brktime Lib "AMONet.dll" Alias "_mnet_m2_set_svon_brktime" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal data As Integer) As Short
    Declare Function B_mnet_m2_set_svoff_brktime Lib "AMONet.dll" Alias "_mnet_m2_set_svoff_brktime" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal data As Integer) As Short
    Declare Function B_mnet_m2_get_svon_break_time Lib "AMONet.dll" Alias "_mnet_m2_get_svon_break_time" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef data As Integer) As Short
    Declare Function B_mnet_m2_get_svoff_break_time Lib "AMONet.dll" Alias "_mnet_m2_get_svoff_break_time" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef data As Integer) As Short

    'Group Motion
    Declare Function B_mnet_m2_gp_add_axis Lib "AMONet.dll" Alias "_mnet_m2_gp_add_axis" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByRef GpID As Short) As Short
    Declare Function B_mnet_m2_gp_remove_axis Lib "AMONet.dll" Alias "_mnet_m2_gp_remove_axis" (ByVal RingNo As Short, ByVal SlaveNo As Short, ByVal AxisNo As Short, ByVal GpID As Short) As Short
    Declare Function B_mnet_m2_gp_reset Lib "AMONet.dll" Alias "_mnet_m2_gp_reset" (ByVal RingNo As Short, ByVal GpID As Short) As Short

    Declare Function B_mnet_m2_gp_start_tr_line Lib "AMONet.dll" Alias "_mnet_m2_gp_start_tr_line" (ByVal RingNo As Short, ByVal GpID As Short, ByVal OffsetDistArray() As Integer, ByVal ElementCnt As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal IsConti As Byte) As Short
    Declare Function B_mnet_m2_gp_start_sr_line Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sr_line" (ByVal RingNo As Short, ByVal GpID As Short, ByVal OffsetDistArray() As Integer, ByVal ElementCnt As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal IsConti As Byte) As Short
    Declare Function B_mnet_m2_gp_start_ta_line Lib "AMONet.dll" Alias "_mnet_m2_gp_start_ta_line" (ByVal RingNo As Short, ByVal GpID As Short, ByVal DistArray() As Integer, ByVal ElementCnt As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal IsConti As Byte) As Short
    Declare Function B_mnet_m2_gp_start_sa_line Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sa_line" (ByVal RingNo As Short, ByVal GpID As Short, ByVal DistArray() As Integer, ByVal ElementCnt As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double, ByVal IsConti As Byte) As Short

    Declare Function B_mnet_m2_gp_start_tr_arc Lib "AMONet.dll" Alias "_mnet_m2_gp_start_tr_arc" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal OffSetCen_X As Integer, ByVal OffSetCen_Y As Integer, ByVal OffSetEnd_X As Integer, ByVal OffSetEnd_Y As Integer, ByVal DIR As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_sr_arc Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sr_arc" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal OffSetCen_X As Integer, ByVal OffSetCen_Y As Integer, ByVal OffSetEnd_X As Integer, ByVal OffSetEnd_Y As Integer, ByVal DIR As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_ta_arc Lib "AMONet.dll" Alias "_mnet_m2_gp_start_ta_arc" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal Cen_X As Integer, ByVal Cen_Y As Integer, ByVal End_X As Integer, ByVal End_Y As Integer, ByVal DIR As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_sa_arc Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sa_arc" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal Cen_X As Integer, ByVal Cen_Y As Integer, ByVal End_X As Integer, ByVal End_Y As Integer, ByVal DIR As Short, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m2_gp_start_tr_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_gp_start_tr_arc_3p" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal OffsetRx As Integer, ByVal OffsetRy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_sr_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sr_arc_3p" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal OffsetRx As Integer, ByVal OffsetRy As Integer, ByVal OffsetEx As Integer, ByVal OffsetEy As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_ta_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_gp_start_ta_arc_3p" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal Rx As Integer, ByVal Ry As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short
    Declare Function B_mnet_m2_gp_start_sa_arc_3p Lib "AMONet.dll" Alias "_mnet_m2_gp_start_sa_arc_3p" (ByVal RingNo As Short, ByVal GpID As Short, ByVal AxisArray() As Short, ByVal Rx As Integer, ByVal Ry As Integer, ByVal Ex As Integer, ByVal Ey As Integer, ByVal StrVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Short

    Declare Function B_mnet_m2_gp_stop_dec Lib "AMONet.dll" Alias "_mnet_m2_gp_stop_dec" (ByVal RingNo As Short, ByVal GpID As Short) As Short
    Declare Function B_mnet_m2_gp_stop_emg Lib "AMONet.dll" Alias "_mnet_m2_gp_stop_emg" (ByVal RingNo As Short, ByVal GpID As Short) As Short

    'Added by W.Y.Z on 2013.11.21
    Declare Function B_mnet_m4_soft_emg_stop Lib "AMONet.dll" Alias "_mnet_m4_soft_emg_stop" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short) As Short
    Declare Function B_mnet_m4_get_io_status_ex Lib "AMONet.dll" Alias "_mnet_m4_get_io_status_ex" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNoArray() As Short, ByVal IO_StatusArray() As Integer, ByVal Count As Short) As Short
    Declare Function B_mnet_m4_get_command_ex Lib "AMONet.dll" Alias "_mnet_m4_get_command_ex" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNoArray() As Short, ByVal CmdArray() As Integer, ByVal Count As Short) As Short
    Declare Function B_mnet_m4_get_position_ex Lib "AMONet.dll" Alias "_mnet_m4_get_position_ex" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNoArray() As Short, ByVal PosArray() As Integer, ByVal Count As Short) As Short
    Declare Function B_mnet_m4_motion_done_ex Lib "AMONet.dll" Alias "_mnet_m4_motion_done_ex" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNoArray() As Short, ByVal MoStArray() As Short, ByVal Count As Short) As Short

    Declare Function B_mnet_m2_error_status Lib "AMONet.dll" Alias "_mnet_m2_error_status" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByRef ErrSt As Integer) As Short
    Declare Function B_mnet_m2_rist_status Lib "AMONet.dll" Alias "_mnet_m2_rist_status" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByRef IntSt As Integer) As Short
    Declare Function B_mnet_m2_start_home_search Lib "AMONet.dll" Alias "_mnet_m2_start_home_search" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByVal Dir As Short, ByVal ORGOffet As Integer) As Short
    Declare Function B_mnet_m2_start_home_z Lib "AMONet.dll" Alias "_mnet_m2_start_home_z" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByVal Dir As Short) As Short
    Declare Function B_mnet_m2_enable_home_reset Lib "AMONet.dll" Alias "_mnet_m2_enable_home_reset" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByVal Enable As Short) As Short
    Declare Function B_mnet_m2_enable_interrupt Lib "AMONet.dll" Alias "_mnet_m2_enable_interrupt" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal AxisNo As Short, ByVal IntEn As Integer) As Short
End Module
