﻿Imports System.ComponentModel

Public Module mdlAMOCodes
    '===== jk add ============================================
    'Constanct
    Public Const AmaxPosMax As Int32 = &H7FFFFFF '=+134,217,727， amax馬達位置的最大值，加1之後會變為AmaxPosMin
    Public Const AmaxPosMin As Int32 = -&H8000000 '=-134,217,728 ， amax馬達位置的最小值，減1之後會變為AmaxPosMax
    Public Const AmaxVelocityMax As Double = 6500000    '6.5M pps
    Public Const AmaxVelocityMin As Double = 0.1        '0.1 pps
#Region "Motion Slave Status "
    'Represeted in BIT INDEX
    Public Enum amaxMotionDIO_State As UInteger
        amax_IO_iRDY = &H1      ' servo ready input.
        amax_IO_iALM = &H2      ' servo alarm input.
        amax_IO_iP_EL = &H4     ' positive limit switch.
        amax_IO_iN_EL = &H8     ' negative limit switch.
        amax_IO_iORG = &H10     ' origin(home) switch.
        amax_IO_oDIR = &H20     ' dir output.
        amax_IO_iEMG = &H40     ' emergency signal input.
        amax_IO_iPCS = &H80     ' PCS input.
        amax_IO_oERC = &H100    ' reset driver error counter , deviation counter clear , ERC output.
        amax_IO_iEZ = &H200     ' encoder z-Index signal.
        amax_IO_CLR = &H400     ' reserved
        amax_IO_iLatch = &H800  ' counter latch signal
        amax_IO_iSD = &H1000    ' slow-down signal
        amax_IO_iINP = &H2000   ' in-position signal
        amax_IO_oSVON = &H4000  ' servo-on signal
        amax_IO_oRALM = &H8000  ' reset alarm signal

        amax_IO_iSTA = &H10000  ' start all hardware signal         , Hsien , 2015.01.26
        amax_IO_iSTP = &H20000  ' stop all hardware alarm signal    , Hsien , 2015.01.26

    End Enum
    ' Error Status , fetched by _mnet_m4_error_status( U16 RingNo, U16 DeviceIP , U16 AxisNo, U32 &ErrSt)
    ' Represeted in Bit Index
    <TypeConverter(GetType(errorStatusTypeConvertor))>
    Public Enum errorStatusEnum As UInteger
        NO_ERROR = 0
        STOPPED_COMPARATOR1_MET = &H1
        STOPPED_COMPARATOR2_MET = &H2
        STOPPED_COMPARATOR3_MET = &H4
        STOPPED_COMPARATOR4_MET = &H8

        STOPPED_COMPARATOR5_MET = &H10
        STOPPED_EL_PLUS = &H20
        STOPPED_EL_MINUS = &H40
        STOPPED_ALM_ON = &H80

        STOPPED_STP_ON = &H100
        STOPPED_EMG_ON = &H200
        STOPPED_SD_ON = &H400
        RESERVED_BIT11 = &H800

        RESERVED_BIT12 = &H1000
        STOPPED_OTHER_AXIS = &H2000
        RESERVED_BIT14 = &H4000
        STOPPED_COUNTER_OUT_OF_RANGE = &H8000

        EA_EB_ERROR = &H10000

    End Enum
    Class errorStatusTypeConvertor
        Inherits TypeConverter
        'Hsien , 2015.08.19 , used to translate 

        Dim __conversionDictionary As BiDictionary(Of errorStatusEnum, String) = New BiDictionary(Of errorStatusEnum, String)

        Private Shared valueArray As UInteger() = [Enum].GetValues(GetType(errorStatusEnum))

        Sub New()
            With __conversionDictionary
                .Add(errorStatusEnum.NO_ERROR, My.Resources.NO_ERROR)

                .Add(errorStatusEnum.STOPPED_COMPARATOR1_MET, My.Resources.STOPPED_COMPARATOR1_MET)
                .Add(errorStatusEnum.STOPPED_COMPARATOR2_MET, My.Resources.STOPPED_COMPARATOR2_MET)
                .Add(errorStatusEnum.STOPPED_COMPARATOR3_MET, My.Resources.STOPPED_COMPARATOR3_MET)
                .Add(errorStatusEnum.STOPPED_COMPARATOR4_MET, My.Resources.STOPPED_COMPARATOR4_MET)
                .Add(errorStatusEnum.STOPPED_COMPARATOR5_MET, My.Resources.STOPPED_COMPARATOR5_MET)

                .Add(errorStatusEnum.STOPPED_EL_PLUS, My.Resources.STOPPED_EL_PLUS)
                .Add(errorStatusEnum.STOPPED_EL_MINUS, My.Resources.STOPPED_EL_MINUS)

                .Add(errorStatusEnum.STOPPED_ALM_ON, My.Resources.STOPPED_ALM_ON)

                .Add(errorStatusEnum.STOPPED_STP_ON, My.Resources.STOPPED_STP_ON)
                .Add(errorStatusEnum.STOPPED_EMG_ON, My.Resources.STOPPED_EMG_ON)
                .Add(errorStatusEnum.STOPPED_SD_ON, My.Resources.STOPPED_SD_ON)

                .Add(errorStatusEnum.STOPPED_OTHER_AXIS, My.Resources.STOPPED_OTHER_AXIS)
                .Add(errorStatusEnum.STOPPED_COUNTER_OUT_OF_RANGE, My.Resources.STOPPED_COUNTER_OUT_OF_RANGE)

                .Add(errorStatusEnum.EA_EB_ERROR, My.Resources.EA_EB_ERROR)

            End With
        End Sub

        Dim collectedString As List(Of String) = New List(Of String)

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            'Hsien  , the error code in encoded in bit meaning , had to do some manage
            Dim valueInUinteger As UInteger = CUInt(value)

            collectedString.Clear()
            For index = 0 To valueArray.Length - 1
                Dim eachValue As UInteger = valueInUinteger And (CUInt(&H1) << index)
                Dim eachObject As Object = [Enum].ToObject(GetType(errorStatusEnum), eachValue)

                If (eachValue > 0 AndAlso
                    (eachObject IsNot Nothing And __conversionDictionary.GetByFirst(eachObject).Count > 0)) Then

                    collectedString.Add(__conversionDictionary.GetByFirst(eachObject).First)
                Else
                    'otherwise , normal
                End If
            Next

            If (collectedString.Count = 0) Then
                collectedString.Add(__conversionDictionary.GetByFirst(errorStatusEnum.NO_ERROR).First)
            End If

            Return String.Join("+", collectedString.ToArray)
        End Function
        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
            Dim valueInString As String = value.ToString
            collectedString.Clear()
            collectedString.AddRange(valueInString.Split("+"))

            Dim result As UInteger = 0

            For Each item As String In collectedString
                result += __conversionDictionary.GetBySecond(item).First
            Next

            Return result
        End Function
    End Class
    ' Motion Done Status , fetched by _mnet_m4_motion_done( U16 RingNo, U16 DeviceIP , U16 AxisNo, U16 &MoSt)
    ' Represented in value
    <TypeConverter(GetType(motionStatusTypeConvertor))>
    Public Enum motionStatusEnum As UShort
        _STOP = 0
        RESERVED_1 = 1
        WAIT_UNTIL_ERC_FINISHED = 2
        RESERVED_3 = 3
        CORRECTING_BACKLASH = 4
        RESERVED_5 = 5
        FEEDING_IN_HOME_SPECIAL_SPEED_MOTION = 6
        FEEDING_IN_STRVEL_SPEED = 7
        ACCELERATING = 8
        FEEDING_IN_MAXVEL_SPEED = 9
        DECELERATING = 10
        WAITING_FOR_INP_INPUT = 11
        RESERVED_12
        RESERVED_13
        RESERVED_14
        RESERVED_15
    End Enum
    Class motionStatusTypeConvertor
        Inherits TypeConverter
        'Hsien , 2015.08.19 , used to translate 

        Dim __conversionDictionary As BiDictionary(Of errorStatusEnum, String) = New BiDictionary(Of errorStatusEnum, String)

        Sub New()
            With __conversionDictionary
                .Add(motionStatusEnum._STOP, My.Resources._STOP)
                .Add(motionStatusEnum.RESERVED_1, My.Resources.RESERVED_1)
                .Add(motionStatusEnum.WAIT_UNTIL_ERC_FINISHED, My.Resources.WAIT_UNTIL_ERC_FINISHED)
                .Add(motionStatusEnum.RESERVED_3, My.Resources.RESERVED_3)
                .Add(motionStatusEnum.CORRECTING_BACKLASH, My.Resources.CORRECTING_BACKLASH)
                .Add(motionStatusEnum.RESERVED_5, My.Resources.RESERVED_5)
                .Add(motionStatusEnum.FEEDING_IN_HOME_SPECIAL_SPEED_MOTION, My.Resources.FEEDING_IN_HOME_SPECIAL_SPEED_MOTION)
                .Add(motionStatusEnum.FEEDING_IN_STRVEL_SPEED, My.Resources.FEEDING_IN_STRVEL_SPEED)
                .Add(motionStatusEnum.ACCELERATING, My.Resources.ACCELERATING)
                .Add(motionStatusEnum.FEEDING_IN_MAXVEL_SPEED, My.Resources.FEEDING_IN_MAXVEL_SPEED)
                .Add(motionStatusEnum.DECELERATING, My.Resources.DECELERATING)
                .Add(motionStatusEnum.WAITING_FOR_INP_INPUT, My.Resources.WAITING_FOR_INP_INPUT)

                .Add(motionStatusEnum.RESERVED_12, My.Resources.RESERVED_12)
                .Add(motionStatusEnum.RESERVED_13, My.Resources.RESERVED_13)
                .Add(motionStatusEnum.RESERVED_14, My.Resources.RESERVED_14)
                .Add(motionStatusEnum.RESERVED_15, My.Resources.RESERVED_15)

            End With
        End Sub

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            Return __conversionDictionary.GetByFirst(value).First
        End Function
        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
            Return True
        End Function
        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
            Return __conversionDictionary.GetBySecond(value).First
        End Function
    End Class
    'motion interrupt status , fetched by _mnet_m4_rist_status( U16 RingNo, U16 DeviceIP , U16 AxisNo, U32 &IstSt)
    ' Represeted in BIT INDEX
    Public Enum ristStatusEnum As UInteger
        RESERVED_BIT0 = &H1
        RESERVED_BIT1 = &H2
        RESERVED_BIT2 = &H4
        RESERVED_BIT3 = &H8
        RESERVED_BIT4 = &H10
        RESERVED_BIT5 = &H20
        RESERVED_BIT6 = &H40
        RESERVED_BIT7 = &H80
        COMPARATOR_1_MET_COMMAND_COUNTER = &H100
        COMPARATOR_2_MET_POSITION_COUNTER = &H200
        RESERVED_BIT10 = &H400
        RESERVED_BIT11 = &H800
        RESERVED_BIT12 = &H1000
        RESERVED_BIT13 = &H2000
        COUNT_VALUE_LATCHED_LTC = &H4000
        RESERVED_BIT15 = &H8000
        RESERVED_BIT16 = &H10000
        RESERVED_BIT17 = &H20000
        RESERVED_BIT18 = &H40000
        RESERVED_BIT19 = &H80000
    End Enum
#End Region
#Region "System Initialization"
    'used by I16 status= _mnet_m4_set_pls_iptmode(U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 pls_iptmode, U16 pls_logic) 
    Enum pulseInputModeEnum As UShort 'encoder 回授方式
        ABx1 = 0
        ABx2 = 1
        ABx4 = 2
        CW_CCW = 3
    End Enum
    Enum pulseInputLogicEnum As UShort 'encoder 回授方式
        NOT_INVERSE_DIRECTION = 0
        INVERSER_DIRECTION = 1
    End Enum


    'used for _mnet_m4_set_pls_outmode(U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 pls_outmode ) 
    Enum pulseOutputModeEnum As UShort
        OUT_FALLING_DIR_HIGH = 0
        OUT_RISING_DIR_HIGH = 1
        OUT_FALLING_DIR_LOW = 2
        OUT_RISING_DIR_LOW = 3
        CW_CCW_FALLING = 4   'CWCCW High edge
        A_B_Phase = 5
        B_A_Phase = 6
        CW_CCW_RISING = 7   ' common use
    End Enum

    'used for _mnet_m4_set_feedback_src (U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 Src)
    Enum feedBackSourceEnum As UShort
        EXTERNAL_ENCODER = 0
        INTERNAL_COMMAND = 1
    End Enum

    '   for source type input point
    ' HIGH_ACTIVE = NORMAL_CLOSE TYPE SENSOR/SWITCH = B TYPE SENSOR/SWITCH
    ' LOW_ACTIVE = NORMAL_OPEN TYPE SENSOR/SWITCH = A TYPE SENSOR/SWITCH
    ' for I16 status=_mnet_m4_set_alm( U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 alm_logic, U16 alm_mode)
    Enum inputActiveModeEnum As UShort
        HIGH_ACTIVE = 0
        LOW_ACTIVE = 1
    End Enum

    Enum ezActiveModeEnum As UShort
        HIGH_ACTIVE = 1
        LOW_ACTIVE = 0
    End Enum

    Enum outputActiveSettingEnum As Short
        INACTIVE = 0
        ACTIVE = 1
    End Enum

    'for I16 status=_mnet_m4_set_inp( U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 inp_enable, U16 inp_logic)
    Enum enableEnum As UShort
        DISABLE = 0
        ENABLE = 1
    End Enum

    'for I16 status=_mnet_m4_set_alm( U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 alm_logic, U16 alm_mode)
    Enum alarmModeEnum As UShort
        IMMEDIATELY_STOP = 0
        DECELERATE_STOP = 1
    End Enum

    'for U16 status=_mnet_m4_set_erc( U16 RingNo, U16 DeviceIP, U16 AxisNo, U16 erc_logic,U16 erc_on_time, U16 erc_off_time)
    Enum ercOnTimeEnum As UShort
        MS_1_6 = 3
        MS_13 = 4
        MS_52 = 5
        MS_104 = 6
        LEVEL = 7
    End Enum
    Enum ercOffTimeEnum As UShort
        MS_0 = 0
        MS_1_6 = 2
        MS_104 = 3
    End Enum

    'for I16 status=_mnet_m4_set_sd( U16 RingNo, U16 DeviceIP, U16 AxisNo, I16 enable,I16 sd_logic, I16 sd_latch, I16 sd_mode)
    Enum sdLatchEnum As Short
        DO_NOT_LATCH = 0
        LATCH = 1
    End Enum

    'for _mnet_m4_set_sd
    Enum sdModeEnum As Short
        SLOW_DOWN_ONLY = 0
        SLOW_DOWN_STOP = 1
    End Enum
    'for _mnet_m4_set_el
    Enum elModeEnum As UShort
        IMMEDIATELY_STOP = 0
        DECELERATE_STOP = 1
    End Enum
    'for _mnet_m4_set_backlash_comp
    Enum backlashCompModeEnum As Short
        OFF = 0
        BACKLASH_CORRECTION = 1
        SLIP_CORRECTION = 2
    End Enum
    'for _mnet_m4_set_com_wdg_mode
    Enum comWatchDogStopMode As UShort
        IGNORE = 0
        SLOW_DOWN_STOP = 1
        EMG_STOP = 2
    End Enum
    'for _mnet_m4_set_emg_reaction
    Enum emgReaction As UShort
        NO_EFFECT = 0
        SERVO_OFF = 1
    End Enum
#End Region
#Region "Motion Slave Position Compare and Latch "
    'for _mnet_m4_get_latch_data
    Enum latchNumberEnum As Short
        COMMAND_COUNTER = 1
        FEEDBACK_COUNTER = 2
        ERROR_COUNTER = 3
    End Enum

    'for _mnet_m4_enable_soft_limit
    Enum softLimitAction As Byte
        IMMEDIATELY_STOP = 1
        DECELERATE_STOP = 2
    End Enum

    'for _mnet_m4_set_comparator_mode
    'for _mnet_m4_set_comparator_data
    'for _mnet_m4_get_comparator_data
    Enum comparatorNumber As Short
        COMPARATOR_1 = 1
        COMPARATOR_2 = 2
        COMPARATOR_GENERAL = 3
    End Enum
    Enum comparatorSource As Short
        COMMAND_COUNTER = 0
        FEEDBACK_COUNTER = 1
        ERROR_COUNTER = 2
    End Enum
    Enum comparatorMethod As Short
        NO_COMPARE = 0
        EQUAL_COUNTER = 1
        EQUAL_COUNTER_DIR_PLUS = 2
        EQUAL_COUNTER_DIR_MINUS = 3
        REAL_LESS_THAN_SETTED = 4
        REAL_GREATER_THAN_SETTED = 5
    End Enum
    Enum comparatorAction As Short
        NO_ACTION = 0
        IMMEDIATELY_STOP = 1
        DECELERATE_STOP = 2
    End Enum
    'for _mnet_m4_set_trigger_comparator_pulse
    Enum triggerPulseMode As UShort
        PULSE_OUTPUT = 0
        TOGGLE_OUTPUT = 1
        LEVEL_OUTOUT = 2
    End Enum
    Enum triggerPulseLogic As UShort
        NORMAL_ON = 0
        NORMAL_OFF = 1
    End Enum
    Enum triggerPulseWidth As UShort
        US_5 = 0
        US_10 = 1
        US_20 = 2
        US_50 = 3
        US_100 = 4
        US_200 = 5
        US_500 = 6
        US_1000 = 7
    End Enum
    'for I16 status=_mnet_m4_set_sync_option(U16 RingNo, U16 DeviceIP , U16 AxisNo, U16 mode)
    Enum syncMode As UShort
        IMMEDIATELY_START = 0
        START_ON_INPUT = 1
        START_WITH_INTERNAL_SYNCHRONOUS_SIGNAL = 2
        START_WITH_SPECIFIC_AXIS_STOP = 3
    End Enum
    Enum syncSignalMode As UShort
        COMPARATOR1_MET_COMMAND_COUNTER = 1
        COMPARATOR2_MET_POSITION_COUNTER = 2
        COMPARATOR3_MET = 3
        COMPARATOR4_MET = 4
        COMPARATOR5_MET = 5
        WHEN_START_ACCELERATION = 8
        WHEN_END_ACCELERATION = 9
        WHEN_START_DECELERATION = 10
        WHEN_END_DECELERATION = 11
        OFF = 12
    End Enum
    Enum syncSource As UShort
        FORM_X_AXIS = 0
        FORM_Y_AXIS = 1
        FORM_Z_AXIS = 2
        FORM_U_AXIS = 3
    End Enum
#End Region

#Region "Ring Operation"
    Enum communicationStatus As Short
        DISCONNECTED = &H0
        CONNECTED = &H1
        SLAVE_ERROR = &H2
        STOPPED = &H3
    End Enum
    Enum ringStatus As UShort
        AXIS_COMMAND_EMPTY = &H1
        RESERVED_BIT1 = &H2
        INPUT_CHANGE = &H4
        IO_DEVICE_ERROR = &H8

        AXIS_DEVICE_ERROR = &H10
        MASTER_SETTING_ERROR = &H20
        MASTER_OPERATING_ERROR = &H40
        RESERVED_BIT7 = &H80

        RESERVED_BIT8 = &H100
        NEW_AXIS_COMMAND = &H200
        NEW_AXIS_DATA = &H400
        RESERVED_BIT11 = &H800

        IO_CYCLE_BUSY = &H1000
        SOFT_RESET = &H2000
        AXIS_CYCLE_BUSY = &H4000
        RESERVED_BIT15 = &H8000
    End Enum
    Enum slaveInfoHighByte As Byte
        AMAX_1220 = &H0
        AMAX_224X = &H1
        AMAX_2710 = &H2
        AMAX_1240 = &H3
    End Enum
    Enum slaveInfoLowByte As Byte

        'represented in value
        AMAX_2754_1754 = &H0
        AMAX_2730 = &H1
        AMAX_2756_1756 = &H2    'conflicted with manual , 2015.11.30
        AMAX_2752_1752 = &H4

        ' folloowing represented in bit index
        IS_MOTION_SLAVE = &H8

        IS_INITIALED = &H10
        RESERVED_BIT9 = &H20
        RESERVED_BIT10 = &H40
        IS_DEVICE_IN_USE = &H80

    End Enum
#End Region

    'hsien , 2015.04.02 , used to select velocity profile
    Public Enum velocityProfileEnum As Short
        S_CURVE = 0
        T_CURVE = 1
    End Enum


#Region "Return Error Codes"

    Enum returnErrorCodes As Short
        ERR_NoError = 0
        ERR_EventError = -1
        ERR_LinkError = -2
        ERR_MNET_Ring_Used = -3
        ERR_Invalid_Ring = -4
        ERR_Invalid_Slave = -5
        ERR_Invalid_Hardware = -6
        ERR_Value_Out_Range = -8
        ERR_Invalid_Setting = -9

        ERR_Axis_Communication = -11
        ERR_Axis_command = -12
        ERR_Axis_Receive = -13
        ERR_Invalid_Operating_Velocity = -14
        ERR_PosOutOfRange = -15
        ERR_Invalid_MaxVel = -16
        ERR_Speed_Change = -17
        ERR_SlowDown_Point = -18
        ERR_Invalid_DIO = -19
        ERR_Invalid_Comparator = -20
        ERR_Comparator_Config = -21
        ERR_CompareSourceError = -22
        ERR_CompareActionError = -23
        ERR_CompareMethodError = -24
        ERR_ComparatorRead = -25
        ERR_LimitOutOfRange = -26
        '//Added by W.Y.Z on 2012.08.15
        ERR_Invalid_DIO_Channel = -27

        ERR_Latch_Config = -30
        ERR_LatchError = -31
        ERR_LatchRead = -32
        ERR_HomeConfig = -35

        '/////////////////////G94 BUS ERROR///////////////////
        ERR_G94_RECEIVE_TimeOut = -36
        ERR_G94_CPURead = -37

        '/////////////////////M4 ERROR///////////////////
        ERR_M4_CPLDRead = -46
        ERR_M4_RegisterRead = -47
        ERR_M4_CPLDWrite = -48
        ERR_M4_RegisterWrite = -49
        ERR_M4_InvalidAxisNo = -50
        ERR_M4_MOFStatusErr = -51
        ERR_M4_InvalidAxisSelect = -52
        ERR_M4_MPGmode = -53
        ERR_M4_InvalidMpgEnable = -54
        ERR_M4_MOFConfigmode = -55
        ERR_M4_SpeedError = -56
        ERR_M4_AxisArrayError = -57

        ERR_Invalid_DeviceNumber = -58
        ERR_LoadDriver_Failed = -59
        ERR_Resource_Failed = -60
        ERR_Invalid_InputPulseMode = -61
        ERR_Invalid_Logic = -62
        ERR_Invalid_OutputPulseMode = -63
        ERR_Invalid_FeedbackSource = -64
        ERR_Invalid_ALMMode = -65
        ERR_Invalid_ERCActiveTime = -66
        ERR_Invalid_ERCUnactiveTime = -67
        ERR_Invalid_SDMode = -68
        ERR_Invalid_HomeMode = -69
        ERR_Invalid_EZCount = -70
        ERR_Invalid_ERCOperation = -71
        ERR_Invalid_LatchNumber = -72
        ERR_Device_NotOpened = -73
        ERR_Watchdog_Started = -74
        ERR_ConfigFileOpenError = -75
        ERR_Invalid_Position_Change = -76
        ERR_Invalid_StartVel = -77
        ERR_Invalid_AccTime = -78
        ERR_Invalid_DecTime = -79
        ERR_Invalid_Ratio = -80
        ERR_Invalid_ELMode = -81
        ERR_Invalid_AccRange = -82
        ERR_Invalid_DecRange = -83
        ERR_Invalid_Memory = -84
        ERR_Invalid_DIOValue = -85
        ERR_Invalid_ORGLogic = -86
        ERR_Invalid_EZLogic = -87
        ERR_Invalid_LatchSetting = -88
        ERR_Invalid_RelPosition = -89
        ERR_Invalid_Baudrate = -90
        ERR_No_Device_Initialized = -91
        ERR_DeviceBusy = -92
        ERR_Invalid_Table_Size = -93
        ERR_Invalid_Compare_Pulse_Mode = -94
        ERR_Invalid_Compare_Pulse_Width = -95
        ERR_Invalid_Compare_Pulse_Logic = -96
        ERR_Function_NotSupport = -97
        ERR_Invalid_ORGOffset = -98
        ERR_Invalid_FwMemoryMode = -99

        '/////////////////////AMAX-2240 ERROR///////////////////
        ERR_AMAX2240 = -200
        ERR_Invalid_Center_Position = ERR_AMAX2240 - 1
        ERR_Invalid_End_Position = ERR_AMAX2240 - 2
        ERR_Invalid_Path_Cmd_Function = ERR_AMAX2240 - 3
        ERR_Invalid_Compare_Start_Data = ERR_AMAX2240 - 4
        ERR_Invalid_Compare_Interval_Data = ERR_AMAX2240 - 5
        '//Added by W.Y.Z on 2012.08.15
        ERR_Invalid_RepeatAxis = ERR_AMAX2240 - 6
        ERR_Invalid_ZeroDistance = ERR_AMAX2240 - 7
        'ERR_Invalid_PrivateID = ERR_AMAX2240 = 8
        ERR_Invalid_PrivateID = ERR_AMAX2240 - 8        ' Error found , Hsien , 2014.09.18


        '/////////////////////AMAX-2240 FIRMWARE ERROR///////////////////
        ERR_AMAX2240_FIRM = -250
        '// Command execution error
        ERR_CommandCodeError = ERR_AMAX2240_FIRM - 1
        ERR_CommandCountExceed = ERR_AMAX2240_FIRM - 2
        ERR_CommandAddedCountError = ERR_AMAX2240_FIRM - 3
        ERR_TerminalSymbolError = ERR_AMAX2240_FIRM - 4
        ERR_CmpoutBufferIsFull = ERR_AMAX2240_FIRM - 5
        ERR_InterruptButNoData = ERR_AMAX2240_FIRM - 6      'Engineer Error code

        '// Motion command error
        ERR_MotionHaveDone = ERR_AMAX2240_FIRM - 7
        ERR_SurplusPulseNotEnough = ERR_AMAX2240_FIRM - 8
        ERR_InvalidSpeedSection = ERR_AMAX2240_FIRM - 9
        ERR_InterpolationMove = ERR_AMAX2240_FIRM - 10
        ERR_InvalidSpeedPattern = ERR_AMAX2240_FIRM - 11
        ERR_AMAX_FWDownloadExceed = ERR_AMAX2240_FIRM - 12
        ERR_AMAX_FWUpdateFailed = ERR_AMAX2240_FIRM - 13
        ERR_CntBufferIsFull = ERR_AMAX2240_FIRM - 14
        ERR_CntMoveIsBusy = ERR_AMAX2240_FIRM - 15
        ERR_CntBufferIsNull = ERR_AMAX2240_FIRM - 16
        ERR_FwMemoryAllocateError = ERR_AMAX2240_FIRM - 17
        '//Added by W.Y.Z on 2012.08.15
        ERR_PassWrdErrorFirstly = ERR_AMAX2240_FIRM - 18
        ERR_PassWrdErrorAgain = ERR_AMAX2240_FIRM - 19
        ERR_PassWrdErrorthrice = ERR_AMAX2240_FIRM - 20

        '// AMAX2710 error code
        ERR_AMAX2710_ERROR = -300
        ERR_Module_Not_Initialize = ERR_AMAX2710_ERROR - 1
        ERR_Module_Initialize_Fail = ERR_AMAX2710_ERROR - 2
        ERR_AI_Channel_Error = ERR_AMAX2710_ERROR - 3
        ERR_AI_Gain_Invalid = ERR_AMAX2710_ERROR - 4
        ERR_AO_Channel_Error = ERR_AMAX2710_ERROR - 5
        ERR_AO_Range_Error = ERR_AMAX2710_ERROR - 6
        ERR_AO_Value_Invalid = ERR_AMAX2710_ERROR - 7
        ERR_BUFFER_TOO_SMALL = ERR_AMAX2710_ERROR - 8
        ERR_MODE_UNMATCHED = ERR_AMAX2710_ERROR - 9

        '//AMAX1220 error code Added by W.Y.Z on 2012.08.15
        ERR_AMAX1220_ERROR = -400
        ERR_M2_INVALID_AXISNO = ERR_AMAX1220_ERROR - 1
        ERR_M2_INVALID_GPID = ERR_AMAX1220_ERROR - 2
        ERR_M2_CannotFindInvalidGPID = ERR_AMAX1220_ERROR - 3
        ERR_M2_Axis_Already_In_GP = ERR_AMAX1220_ERROR - 4
        ERR_M2_Axis_not_exist_inGp = ERR_AMAX1220_ERROR - 5
        ERR_M2_Invalid_Dist_Array = ERR_AMAX1220_ERROR - 6
        ERR_M2_Invlaid_Center_Array = ERR_AMAX1220_ERROR - 7
        ERR_M2_Invalid_Axis_Count = ERR_AMAX1220_ERROR - 8
        ERR_M2_Invalid_Axis_Index = ERR_AMAX1220_ERROR - 9


        '-----------------------------------
        '   Not showed on oringinal API , but defined in previous codes
        '-----------------------------------
        ERR_Origin_Limit = -150
        ERR_Position_Move = -151
        ERR_ioALM = -152
        ERR_Ring_St_Disconnected = -153
        ERR_Positive_Limit = -154
        ERR_Negative_Limit = -155
        ERR_In_Position = -156
        ERR_Software_Limit = -157
        ERR_Setting_Position = -158
        ERR_Over_Position = -159

        ERR_Execution_Time_Out = -160     ' defined by Hsien , 2014.09.27
        ERR_No_Corresponding_Point = -161     ' defined by Hsien , 2014.10.06
        ERR_No_Assigning_Point = -162     ' defined by Hsien , 2014.10.06

    End Enum

#End Region

    'Hsien , function argument type changed from ULong -> UInteger , 2015.11.23
    Declare Function B_mnet_io_memory_output Lib "AMONet.dll" Alias "_mnet_io_memory_output" (ByVal RingNo As Integer, ByRef DataOutArray As UInteger) As Short
    Declare Function B_mnet_io_memory_input Lib "AMONet.dll" Alias "_mnet_io_memory_input" (ByVal RingNo As Integer, ByRef DataInArray As UInteger) As Short


#Region "Path Table"
    '-----------
    '   Hsien , 2014.09.27
    '-----------
    Enum velocityProfileType As UShort
        T_CURVE = 0
        S_CURVE = 1
    End Enum

    Enum commandFunctionLineEnum As UShort
        START_R_MOVE = 1
        START_R_LINE2 = 2
        START_R_LINE3 = 4
        START_R_LINE4 = 5
        START_A_MOVE = 10
        START_A_LINE2 = 11
        START_A_LINE3 = 13
        START_A_LINE4 = 14
    End Enum

    Enum commandFunctionCircularEnum As UShort
        START_R_ARC2 = 3
        START_A_ARC2 = 12
    End Enum

    Enum directionircularEnum As UShort
        CW = 0
        CCW = 1
    End Enum

    Enum enableDisable As UShort
        DISBALE = 0
        ENABLE = 1
    End Enum

    Enum currentCommandFunction As UShort
        NO_COMMAND = 0
        START_R_MOVE = 1
        START_R_LINE2 = 2
        START_R_ARC2 = 3
        START_R_LINE3 = 4
        START_R_LINE4 = 5
    End Enum

#End Region

#Region "start/stop move All"
    '===== 2013.12 加入 for amax-1240 =============
    ''Simultaneous Axis motion
    Declare Function B_mnet_m4_set_moveall_start_mode Lib "AMONet.dll" Alias "_mnet_m4_set_moveall_start_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal startmode As Short, ByVal start_FallorRise As Short) As Short
    Declare Function B_mnet_m4_get_moveall_start_mode Lib "AMONet.dll" Alias "_mnet_m4_get_moveall_start_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByRef startmode As Short, ByRef start_FallorRise As Short) As Short
    Declare Function B_mnet_m4_set_moveall_stop_mode Lib "AMONet.dll" Alias "_mnet_m4_set_moveall_stop_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal stopmode As Short, ByVal stop_FallorRise As Short) As Short
    Declare Function B_mnet_m4_get_moveall_stop_mode Lib "AMONet.dll" Alias "_mnet_m4_get_moveall_stop_mode" (ByVal RingNo As Short, ByVal DeviceIP As Short, ByRef stopmode As Short, ByRef stop_FallorRise As Short) As Short

    '--------------------------------------
    '   Used for start/stop all mode , Hsien  ,2015.01.26
    '--------------------------------------
    Public Enum moveAllMode As UShort
        LEVEL = 0
        EDGE = 1
    End Enum
    Public Enum moveAllFallOrRise As UShort
        FALLING_OR_LOW
        RISING_OR_HIGH
    End Enum

#End Region

    Public Enum pointTypeEnum As Short
        ABS = 0
        REL = 1
        HOME_MINUS = 0
        HOME_PLUS = 1
    End Enum

End Module
