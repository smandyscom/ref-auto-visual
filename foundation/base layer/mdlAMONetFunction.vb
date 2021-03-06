Public Module mdlAMONetFunction
    
    

    '==========================================
    '     Pulse Input/Output Configuration
    '==========================================
    Public Function AMaxM4_PulseConfig(ByVal AxisIP As Integer, ByVal cntFeedback As Boolean, Optional ByVal strErrStatus As String = vbNullString) As Short
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short
        Dim cntSrc As Short

        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        ' Set encoder input mode
        ' pls_iptmode : Setting of encoder feedback pulse input mode ( 0=1X A/B; 1=2X A/B; 2=4X A/B; 3=CW/CCW)
        ' pls_logic : Logic of encoder feedback pulseValue  Meaning (0=Not inverse direction;1= Inverse direction
        AMaxM4_PulseConfig = B_mnet_m4_set_pls_iptmode(RingNo, DeviceIP, PortNo, 2, 0)   ' 4X, Not Inverse
        If AMaxM4_PulseConfig < 0 Then strErrStatus = strErrAmaxMotion(AxisIP, AMaxM4_PulseConfig) : Exit Function

        ' Set pulse command output mode
        AMaxM4_PulseConfig = B_mnet_m4_set_pls_outmode(RingNo, DeviceIP, PortNo, 7)          '7 : CW/CCW
        If AMaxM4_PulseConfig < 0 Then strErrStatus = strErrAmaxMotion(AxisIP, AMaxM4_PulseConfig) : Exit Function

        ' Set the position counters input source
        If cntFeedback = False Then cntSrc = 1
        AMaxM4_PulseConfig = B_mnet_m4_set_feedback_src(RingNo, DeviceIP, PortNo, cntSrc)       '0 External Feedback; 1:  Command Pulse
        If AMaxM4_PulseConfig < 0 Then strErrStatus = strErrAmaxMotion(AxisIP, AMaxM4_PulseConfig) : Exit Function

    End Function
    
    Public Function AMaxM4_HomeConfig(ByVal AxisIP As Integer, ByVal home_mode As Short, ByVal org_logic As Short, ByVal ez_logic As Short, ByVal ez_count As Short, ByVal ERC_Out As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            'Home Logic : Set the home/index logic configuration and homing mode.
            'home_mode : Range: 0~12
            'org_logic : Action logic configuration for ORG signal (0:Low Active, 1: High Active)
            'ez_logic : Action logic configuration for EZ signal(0:Low Active, 1: High Active)
            'ez_count : Range : 0~15
            'erc_out : Set ERC output options.(Clear Servo Error Counter Signal Output) (0:No ERC Out, 1:ERC Out when homing finish)
            AMaxM4_HomeConfig = B_mnet_m4_set_home_config(RingNo, DeviceIP, PortNo, home_mode, org_logic, ez_logic, ez_count, ERC_Out)

            'Enable reset counter when homing is complete
            '0:Don’t reset counter when homing is complete, 1: Reset counter when homing is complete (Default)
            AMaxM4_HomeConfig = B_mnet_m4_enable_home_reset(RingNo, DeviceIP, PortNo, 1)
        End SyncLock
    End Function
    Public Function AMaxM4_HomeMove(ByVal AxisIP As Integer, ByVal Dir As Short, ByVal ORGOffset As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            ' 0 :  Negative (-) homing,   1 :  Positive (+) homing
            'AMaxM4_HomeMove = B_mnet_m4_start_home_move(RingNo, DeviceIP, PortNo, Dir)
            AMaxM4_HomeMove = B_mnet_m4_start_home_search(RingNo, DeviceIP, PortNo, Dir, ORGOffset)
        End SyncLock
    End Function
    '=============================================================
    '=============================================================
    '=============================================================
    Public Function AMaxM4_ServoOn(ByVal AxisIP As Integer, ByVal OnOff As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            '0: In-active; 1:Active
            AMaxM4_ServoOn = B_mnet_m4_set_svon(RingNo, DeviceIP, PortNo, Math.Abs(OnOff))
        End SyncLock
    End Function
    Public Function AMaxM4_ResetALM(ByVal AxisIP As Integer, ByVal OnOff As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            '0: In-active; 1:Active
            AMaxM4_ResetALM = B_mnet_m4_set_ralm(RingNo, DeviceIP, PortNo, Math.Abs(OnOff))
        End SyncLock

    End Function
    '=============================================================
    '=============================================================
    Public Function AMaxM4_P_Change(ByVal AxisIP As Integer, ByVal posPosition As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_P_Change = B_mnet_m4_p_change(RingNo, DeviceIP, PortNo, posPosition)
        End SyncLock
    End Function
    Public Function AMaxM4_AMov(ByVal AxisIP As Integer, ByVal posPosition As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_AMov = B_mnet_m4_start_a_move(RingNo, DeviceIP, PortNo, posPosition)
        End SyncLock
    End Function
    Public Function AMaxM4_AMov2(ByVal AxisIP As Integer, ByVal MPoint As cMotorPoint) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_AMov2 = B_mnet_m4_start_a_move(RingNo, DeviceIP, PortNo, MPoint.Distance)
        End SyncLock
    End Function
    Public Function AMaxM4_RMov(ByVal AxisIP As Integer, ByVal posPosition As Long) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_RMov = B_mnet_m4_start_r_move(RingNo, DeviceIP, PortNo, posPosition)
        End SyncLock
    End Function
    Public Function AMaxM4_RistStatus(ByVal AxisIP As Short, ByRef IntSt As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_RistStatus = B_mnet_m4_rist_status(RingNo, DeviceIP, PortNo, IntSt)
        End SyncLock
    End Function
    Public Function AMaxM4_SetLatchEnable(ByVal AxisIP As Short, ByVal ltc_enable As enableDisable) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_SetLatchEnable = B_mnet_m4_set_ltc_enable(RingNo, DeviceIP, PortNo, ltc_enable) '0:disable,1:enable
        End SyncLock
    End Function
    Public Function AMaxM4_GetLatchData(ByVal AxisIP As Short, ByVal LtcNo As latchNumberEnum, ByRef POS As Double) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_GetLatchData = B_mnet_m4_get_latch_data(RingNo, DeviceIP, PortNo, LtcNo, POS)    'reset latch data
        End SyncLock
    End Function
    Public Function AMaxM4_SetTriggerComparatorPulse(ByVal AxisIP As Short, ByVal PulseMode As Short, ByVal Logic As Short, ByVal PulseWidth As Short) As Short
        'PulseMode
        '0:Pulse(Output)
        '1:Toggle(Output)
        'Logic
        '0:Normal OFF
        '1:Normal ON 
        'PulseWidth
        '0 5 us 
        '1 10 us 
        '2 20 us 
        '3 50 us 
        '4 100 us 
        '5 200 us 
        '6 500 us 
        '7 1000 us 
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SetTriggerComparatorPulse = B_mnet_m4_set_trigger_comparator_pulse(RingNo, DeviceIP, PortNo, PulseMode, Logic, PulseWidth)
        End SyncLock
    End Function
    Public Function AMaxM4_SetTriggerComparatorData(ByVal AxisIP As Short, ByVal Data As Double) As Short '設定被觸發的位置
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_SetTriggerComparatorData = B_mnet_m4_set_trigger_comparator_data(RingNo, DeviceIP, PortNo, Data)
            'iMStatus = B_mnet_m4_set_comparator_data(RingNo, DeviceIP, MSO Mod 4, 1, TrigPos) '設定被觸發的位置
        End SyncLock
    End Function
    Public Function AMaxM4_GetTriggerComparatorData(ByVal AxisIP As Short, ByRef Data As Double) As Short '設定被觸發的位置
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_GetTriggerComparatorData = B_mnet_m4_get_trigger_comparator_data(RingNo, DeviceIP, PortNo, Data)
            'iMStatus = B_mnet_m4_set_comparator_data(RingNo, DeviceIP, MSO Mod 4, 1, TrigPos) '設定被觸發的位置
        End SyncLock
    End Function
    Public Function AMaxM4_SetComparatorData(ByVal AxisIP As Short, ByVal CmpNo As Short, ByVal Data As Double) As Short '設定被觸發的位置
        'CmpNo
        '   1:Comparator 1 (default: + Software limit ) <--通常選這個
        '   2: Comparator 2 (default: - Software limit )
        '   3:General(comparator)
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_SetComparatorData = B_mnet_m4_set_comparator_data(RingNo, DeviceIP, PortNo, CmpNo, Data)
            'iMStatus = B_mnet_m4_set_comparator_data(RingNo, DeviceIP, MSO Mod 4, 1, TrigPos) '設定被觸發的位置
        End SyncLock
    End Function
    Public Function AMaxM4_SetComparatorMode(ByVal AxisIP As Short, ByVal CmpNo As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short, ByVal CmpAction As Short) As Short
        '與AMaxM4_SetTriggerComparator很類似
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SetComparatorMode = B_mnet_m4_set_comparator_mode(RingNo, DeviceIP, PortNo, CmpNo, CmpSrc, CmpMethod, CmpAction)
        End SyncLock
        'CompNo:Comparator Number
        '   1: Comparator 1 (default: + Software limit )
        '   2: Comparator 2 (default: - Software limit )
        '   3:General(comparator)
        'CmpSrc
        '   0:Command(Counter)
        '   1:Feedback(Counter)
        '   2:Error Counter
        'CmpMethod
        '   0:No(compare)
        '   1: 設定值=Counter (Directionless)
        '   2: 設定值=Counter (+Dir)
        '   3: 設定值=Counter (-Dir)
        '   4: 設定值>Counter
        '   5: 設定值<Counter :
        '   6: Use as software limits. When used for software limits, Comparator 1 is a positive direction limit and the comparison method is comparator < comparison source counter. Comparator 2 is the negative limit value and the comparison method is comparator > comparison source counter.
        'CmpAction
        '   0:No(action)
        '   1:Immediately stopre limits. When used for software limits, Comparator 1 is a positive direction limit and the comparison method is comparator < comparison source counter. Comparator 2 is the negative limit value and the comparison method is comparator > comparison source counter.
    End Function
    Public Function AMaxM4_SetTriggerComparator(ByVal AxisIP As Short, ByVal CmpSrc As Short, ByVal CmpMethod As Short) As Short
        '與AMaxM4_SetComparatorMode很類似
        'CmpSrc:0=command , 1=feedback
        'CmpMethod: 
        '   0: No(compare)
        '   1: =Counter (Directionless)
        '   2: =Counter (+Dir)
        '   3: =Counter (-Dir)
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_SetTriggerComparator = B_mnet_m4_set_trigger_comparator(RingNo, DeviceIP, PortNo, CmpSrc, CmpMethod)
        End SyncLock
    End Function
    'Public Function AMaxM4_FixSpeedRange(ByVal AxisIP As Short, ByVal MaxVel As Double) As Short
    '    SyncLock Locking
    '        Dim RingNo As Short, DeviceIP As Short, PortNo As Short
    '        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
    '        AMaxM4_FixSpeedRange = B_mnet_m4_fix_speed_range(RingNo, DeviceIP, PortNo, MaxVel)
    '    End SyncLock
    'End Function
    Public Function AMaxM4_SetSyncOption(ByVal AxisIP As Short, ByVal Mode As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
            AMaxM4_SetSyncOption = B_mnet_m4_set_sync_option(RingNo, DeviceIP, PortNo, Mode)  '觸發致能 2=Start with an internal synchronous start signal.
            '0 Start immediately.
        End SyncLock
    End Function
    Public Function AMaxM4_SetSyncSignalMode(ByVal AxisIP As Short, ByVal Mode As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SetSyncSignalMode = B_mnet_m4_set_sync_signal_mode(RingNo, DeviceIP, PortNo, Mode) '用2，AxisIP回授位置到就觸發
        End SyncLock
        'This function is used to specify the internal synchronous signal output
        'timing. If one of the condition is satisfied, the internal synchronous signal will be triggered.
        'mode:
        '0001:1 When the Comparator 1(command counter) conditions are met. (for Delta conveyor synchronize function)
        '0010:2 When the Comparator 2(position counter) conditions are met.
        '0011:3 When the Comparator 3(error counter) conditions are met.
        '0100:4 When the Comparator 4 conditions are met.
        '0101:5 When the Comparator 5 conditions are met.
        '1000:8 When starting acceleration.
        '1001:9 When ending acceleration.
        '1010:10 When starting deceleration.
        '1011:11 When ending deceleration.
        'Others: Internal synchronous signal output is OFF.
    End Function
    Public Function AMaxM4_SetSyncSignalSource(ByVal AxisIP As Short, ByVal SrcAxis As Short) As Short 'AxisIP 與 SrcAxis一定要同一個模組
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo1, PortNo2 As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(SrcAxis, RingNo, DeviceIP, PortNo2)
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo1)

            AMaxM4_SetSyncSignalSource = B_mnet_m4_set_sync_signal_source(RingNo, DeviceIP, PortNo1, PortNo2) 'PortNo1被PortNo2觸發
        End SyncLock
    End Function
    'Public Function AMaxM4_AbsMode(ByVal AxisIP As Integer, ByVal AbsMode As Integer) As Short
    ' malfunction , by hsien
    '    SyncLock Locking
    '        Dim RingNo As Integer, DeviceIP As Integer, PortNo As Integer

    '        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
    '        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

    '        'AMaxM4_AbsMode = B_mnet_m4_set_abs_mode(RingNo, DeviceIP, PortNo, AbsMode)
    '    End SyncLock
    'End Function
    Public Function AMaxM4_SMovSet(ByVal AxisIP As Integer, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Acc As Double, ByVal Dec As Double, ByVal SAcc As Double, ByVal SDcc As Double) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SMovSet = B_mnet_m4_set_smove_speed(RingNo, DeviceIP, PortNo, StartVel, MaxVel, Acc, Dec, SAcc, SDcc)
        End SyncLock
    End Function
    Public Function AMaxM4_SMovSet2(ByVal AxisIP As Integer, ByVal MPoint As cMotorPoint) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            Dim StartVel, MaxVel, Acc, Dec, SAcc, SDcc As Double
            StartVel = MPoint.StartVelocity
            MaxVel = MPoint.Velocity
            Acc = MPoint.AccelerationTime
            Dec = MPoint.DecelerationTime
            SAcc = 0
            SDcc = 0
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SMovSet2 = B_mnet_m4_set_smove_speed(RingNo, DeviceIP, PortNo, StartVel, MaxVel, Acc, Dec, SAcc, SDcc)
        End SyncLock
    End Function
    Public Function AMaxM4_TMovSet(ByVal AxisIP As Integer, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Acc As Double, ByVal Dec As Double) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_TMovSet = B_mnet_m4_set_tmove_speed(RingNo, DeviceIP, PortNo, StartVel, MaxVel, Acc, Dec)
        End SyncLock
    End Function
    Public Function AMaxM4_VMov(ByVal AxisIP As Integer, ByVal Dir As Short) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            ' 0 :  Negative (-) moving,   1 :  Positive (+) moving
            AMaxM4_VMov = B_mnet_m4_v_move(RingNo, DeviceIP, PortNo, Dir)
        End SyncLock
    End Function
    Public Function AMaxM4_TR_Line2(ByVal AxisIP1 As Integer, ByVal AxisIP2 As Integer, ByVal Dist1 As Long, ByVal Dist2 As Long, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Integer
        SyncLock Locking
            Dim RingNo1 As Integer, RingNo2 As Integer, DeviceIP1 As Integer, DeviceIP2 As Integer, PortNo1 As Integer, PortNo2 As Integer
            Dim AxisArray(2) As Integer
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP1, RingNo1, DeviceIP1, PortNo1)
            Call AMax_Get_Moton_DeviceIP(AxisIP2, RingNo2, DeviceIP2, PortNo2)

            If (RingNo1 <> RingNo2) Or (DeviceIP1 <> DeviceIP2) Then
                AMaxM4_TR_Line2 = ERR_Invalid_Setting : Exit Function
            End If

            'Begin a relative 2-axis linear interpolation for any 2 axes, with S-curve profile
            AxisArray(1) = PortNo1 : AxisArray(2) = PortNo2
            AMaxM4_TR_Line2 = B_mnet_m4_start_tr_line2(RingNo1, DeviceIP1, AxisArray(1), Dist1, Dist2, StartVel, MaxVel, Tacc, Tdec)
        End SyncLock
    End Function
    Public Function AMaxM4_TA_Line2(ByVal AxisIP1 As Integer, ByVal AxisIP2 As Integer, ByVal Dist1 As Long, ByVal Dist2 As Long, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Integer
        SyncLock Locking
            Dim RingNo1 As Integer, RingNo2 As Integer, DeviceIP1 As Integer, DeviceIP2 As Integer, PortNo1 As Integer, PortNo2 As Integer
            Dim AxisArray(2) As Integer
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP1, RingNo1, DeviceIP1, PortNo1)
            Call AMax_Get_Moton_DeviceIP(AxisIP2, RingNo2, DeviceIP2, PortNo2)

            If (RingNo1 <> RingNo2) Or (DeviceIP1 <> DeviceIP2) Then
                AMaxM4_TA_Line2 = ERR_Invalid_Setting : Exit Function
            End If

            'Begin a relative 2-axis linear interpolation for any 2 axes, with S-curve profile
            AxisArray(1) = PortNo1 : AxisArray(2) = PortNo2
            AMaxM4_TA_Line2 = B_mnet_m4_start_ta_line2(RingNo1, DeviceIP1, AxisArray(1), Dist1, Dist2, StartVel, MaxVel, Tacc, Tdec)
        End SyncLock
    End Function
    Public Function AMaxM4_SR_Line2(ByVal AxisIP1 As Integer, ByVal AxisIP2 As Integer, ByVal Dist1 As Long, ByVal Dist2 As Long, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Integer
        SyncLock Locking
            Dim RingNo1 As Integer, RingNo2 As Integer, DeviceIP1 As Integer, DeviceIP2 As Integer, PortNo1 As Integer, PortNo2 As Integer
            Dim AxisArray(2) As Integer
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP1, RingNo1, DeviceIP1, PortNo1)
            Call AMax_Get_Moton_DeviceIP(AxisIP2, RingNo2, DeviceIP2, PortNo2)

            If (RingNo1 <> RingNo2) Or (DeviceIP1 <> DeviceIP2) Then
                AMaxM4_SR_Line2 = ERR_Invalid_Setting : Exit Function
            End If

            'Begin a relative 2-axis linear interpolation for any 2 axes, with S-curve profile
            AxisArray(1) = PortNo1 : AxisArray(2) = PortNo2
            AMaxM4_SR_Line2 = B_mnet_m4_start_sr_line2(RingNo1, DeviceIP1, AxisArray(1), Dist1, Dist2, StartVel, MaxVel, Tacc, Tdec)
        End SyncLock
    End Function
    Public Function AMaxM4_SA_Line2(ByVal AxisIP1 As Integer, ByVal AxisIP2 As Integer, ByVal Dist1 As Long, ByVal Dist2 As Long, ByVal StartVel As Double, ByVal MaxVel As Double, ByVal Tacc As Double, ByVal Tdec As Double) As Integer
        SyncLock Locking
            Dim RingNo1 As Integer, RingNo2 As Integer, DeviceIP1 As Integer, DeviceIP2 As Integer, PortNo1 As Integer, PortNo2 As Integer
            Dim AxisArray(2) As Integer
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP1, RingNo1, DeviceIP1, PortNo1)
            Call AMax_Get_Moton_DeviceIP(AxisIP2, RingNo2, DeviceIP2, PortNo2)

            If (RingNo1 <> RingNo2) Or (DeviceIP1 <> DeviceIP2) Then
                AMaxM4_SA_Line2 = ERR_Invalid_Setting : Exit Function
            End If
            'Begin a relative 2-axis linear interpolation for any 2 axes, with S-curve profile
            AxisArray(1) = PortNo1 : AxisArray(2) = PortNo2
            AMaxM4_SA_Line2 = B_mnet_m4_start_sa_line2(RingNo1, DeviceIP1, AxisArray(1), Dist1, Dist2, StartVel, MaxVel, Tacc, Tdec)
        End SyncLock
    End Function





    Public Function AMaxM4_PauseMotion(ByVal AxisIP As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            ' Pause the executing motion immediately, when the pause command is executed, the motion will be slow down to zero.
            ' ERR_NoError (0) : The API returns success
            AMaxM4_PauseMotion = B_mnet_m4_pause_motion(RingNo, DeviceIP, PortNo)
        End SyncLock
    End Function
    Public Function AMaxM4_ResumeMotion(ByVal AxisIP As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            'Resume motion command that was paused by _mnet_m4_pause_motion.
            AMaxM4_ResumeMotion = B_mnet_m4_resume_motion(RingNo, DeviceIP, PortNo)
        End SyncLock

    End Function
    Public Function AMaxM4_WatchDogMode(ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal wdgMode As Short) As Short
        'Set AMAX-224x SW_Com_Watchdog action when AMax communication is broken.
        ' 0: Ignore (Default); 1: Slow down stop; 2:  Emg stop
        AMaxM4_WatchDogMode = B_mnet_m4_set_com_wdg_mode(RingNo, DeviceIP, wdgMode)

    End Function
    
    Public Function AMaxM4_Set_SD_Enable(ByVal AxisIP As Integer,
                                         ByVal enable As enableEnum,
                                         logic As inputActiveModeEnum,
                                         latch As sdLatchEnum,
                                         mode As sdModeEnum) As Short
        SyncLock Locking
            'Dim Status As Integer
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            Return B_mnet_m4_set_sd(RingNo, DeviceIP, PortNo, enable, logic, latch, mode) 'set sd hi active
        End SyncLock

    End Function
    
    Public Function AMaxM4_Get_Current_Speed(ByVal AxisIP As Integer, ByRef CurrentSpeed As Double) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_Get_Current_Speed = B_mnet_m4_get_current_speed(RingNo, DeviceIP, PortNo, CurrentSpeed)
        End SyncLock
    End Function
    Public Function AMaxM4_EmgStop(ByVal AxisIP As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_EmgStop = B_mnet_m4_emg_stop(RingNo, DeviceIP, PortNo)
        End SyncLock
    End Function
    Public Function AMaxM4_Soft_Emg_Stop(ByVal AxisIP As Integer) As Short
        '-----------------------
        '   Hsien , 2014.10.09
        '-----------------------
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            Return B_mnet_m4_soft_emg_stop(RingNo, DeviceIP, PortNo)
        End SyncLock
    End Function
    Public Function AMaxM4_SlowDownStop(ByVal AxisIP As Integer) As Short
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_SlowDownStop = B_mnet_m4_sd_stop(RingNo, DeviceIP, PortNo)
        End SyncLock
    End Function
    
    Public Function AMaxM4_CmdPos_Reset(ByVal AxisIP As Integer) As Short
        'Reset pulse Command Counter Value to zero.
        'Reset position counter value
        'Reset error counter value
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            Dim returnValue As Short = 0
            returnValue = B_mnet_m4_reset_command(RingNo, DeviceIP, PortNo)
            returnValue = B_mnet_m4_reset_position(RingNo, DeviceIP, PortNo)
            returnValue = B_mnet_m4_reset_error_counter(RingNo, DeviceIP, PortNo)

            Return returnValue
        End SyncLock
    End Function

    Public Function AMaxM4_CommandPosition_Set(ByVal AxisIP As Integer, ByVal nPos As Integer) As Short
        ' Set the Command Counter Value
        ' Set the Position Counter value
        ' Reset error counter
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            AMaxM4_CommandPosition_Set = B_mnet_m4_set_command(RingNo, DeviceIP, PortNo, nPos)
            AMaxM4_CommandPosition_Set = B_mnet_m4_set_position(RingNo, DeviceIP, PortNo, nPos)
            AMaxM4_CommandPosition_Set = B_mnet_m4_reset_error_counter(RingNo, DeviceIP, PortNo)
        End SyncLock
    End Function


    Public Function AMaxM4_MotionDone(ByVal AxisIP As Integer, ByRef MotionStatus As Short) As Short

        '0:Stop, 
        '1:Reserved, 
        '2:Wait until ERC finished, 
        '3:Reserved,
        '4:Correcting Backlash
        '5:Reserved, 
        '6:Feeding in home special speed motion, 
        '7:Feeding in StrVel speed
        '8:Accelerating, 
        '9:Feeding in MaxVel speed(在最大速度段), 
        '10: Decelerating, 
        '11:Waiting for INP input '只有在減速段完成後，才會進入此狀態

        '15:Reserved '=jk ??? 什麼狀態??
        'SyncLock motionStatusQueryLock
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short
        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        AMaxM4_MotionDone = B_mnet_m4_motion_done(RingNo, DeviceIP, PortNo, MotionStatus)
        'End SyncLock
    End Function
    Public Function AMaxM4_ErrorStatus(ByVal AxisIP As Integer, ByRef errorStatus As Integer) As Short
        'SyncLock motionStatusQueryLock
        '---------------------
        '   Hsien , 2014.07.31
        '---------------------
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short
        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_error_status(RingNo, DeviceIP, PortNo, errorStatus)

        'End SyncLock

    End Function
    Public Function AMaxM4_Get_Position(ByVal AxisIP As Integer, ByRef posFeedback As Integer) As Short
        ' Get position
        'SyncLock motionStatusQueryLock
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        AMaxM4_Get_Position = B_mnet_m4_get_position(RingNo, DeviceIP, PortNo, posFeedback)
        'End SyncLock
    End Function
    Public Function AMaxM4_Get_Command(ByVal AxisIP As Integer, ByRef commandFeedback As Integer) As Short
        ' Get position
        'SyncLock motionStatusQueryLock
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        AMaxM4_Get_Command = B_mnet_m4_get_command(RingNo, DeviceIP, PortNo, commandFeedback)
        'End SyncLock
    End Function
    Public Function AMaxM4_Get_Error_Counter(ByVal AxisIP As Integer, ByRef errorFeedback As Integer) As Short
        ' Get position
        'SyncLock motionStatusQueryLock
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        AMaxM4_Get_Error_Counter = B_mnet_m4_get_error_counter(RingNo, DeviceIP, PortNo, errorFeedback)
        'End SyncLock
    End Function




    Public Function AMaxM4_MotionStatusMsg(ByVal Status As Short) As String
        SyncLock Locking
            AMaxM4_MotionStatusMsg = vbNullString
            Select Case Status
                Case 0
                    AMaxM4_MotionStatusMsg = "Stop"
                Case 1
                    AMaxM4_MotionStatusMsg = " Reserved"
                Case 2
                    AMaxM4_MotionStatusMsg = "Wait until ERC finished"
                Case 3
                    AMaxM4_MotionStatusMsg = "Reserved"
                Case 4
                    AMaxM4_MotionStatusMsg = "Correcting Backlash"
                Case 5
                    AMaxM4_MotionStatusMsg = "Reserved"
                Case 6
                    AMaxM4_MotionStatusMsg = "Feeding in home special speed motion"
                Case 7
                    AMaxM4_MotionStatusMsg = "Feeding in StrVel speed"
                Case 8
                    AMaxM4_MotionStatusMsg = "Accelerating"
                Case 9
                    AMaxM4_MotionStatusMsg = "Feeding in MaxVel speed"
                Case 10
                    AMaxM4_MotionStatusMsg = "Decelerating"
                Case 11
                    AMaxM4_MotionStatusMsg = "Waiting for INP input"
                Case 12
                    AMaxM4_MotionStatusMsg = "Reserved"
                Case 13
                    AMaxM4_MotionStatusMsg = "Reserved"
                Case 14
                    AMaxM4_MotionStatusMsg = "Reserved"
                Case 15
                    AMaxM4_MotionStatusMsg = "Reserved"
            End Select
        End SyncLock
    End Function

#Region "various speed/position change"
    Public Function AMaxM4_P_Change_R(ByVal AxisIP As Integer, ByVal position As Integer) As Short
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_p_change_r(RingNo, DeviceIP, PortNo, position)

    End Function
    Public Function AMaxM4_Fix_Speed_Range(ByVal AxisIP As Integer, ByVal maxVel As Double) As Short
        '---------
        '   Hsien , according to manual , use this function to enlarge speed scope before V change
        ' 2014.09.25
        '---------
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_fix_speed_range(RingNo, DeviceIP, PortNo, maxVel)

    End Function
    Public Function AMaxM4_V_Change(ByVal AxisIP As Integer, ByVal newVel As Double, ByVal timeSecond As Double) As Short
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_v_change(RingNo, DeviceIP, PortNo, newVel, timeSecond)

    End Function
#End Region

#Region "path table"
    Public Function AMaxM4_Reset_Path(ByVal AxisIP As Integer) As Short
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_reset_path(RingNo, DeviceIP)

    End Function
    Public Function AMaxM4_Set_Path_Move_Speed(ByVal AxisIP As Integer, ByVal profile As UShort, ByVal startVelocity As Double, ByVal maxVelocity As Double, accTime As Double, decTime As Double) As Short
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_set_path_move_speed(RingNo, DeviceIP, profile, startVelocity, maxVelocity, accTime, decTime)

    End Function
    Public Function AMaxM4_Set_Path_Line_Data(ByVal AxisIP As Integer, ByVal command As UInt16, ByVal distance As Integer, ByVal startVelocity As Double, ByVal maxVelocity As Double, ByVal enableDec As UInt16)

        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_set_path_line_data(RingNo, DeviceIP, New Short() {PortNo}, command, New Integer() {distance}, startVelocity, maxVelocity, enableDec)
    End Function
    Public Function AMaxM4_Start_Path(ByVal AxisIP As Integer) As Short
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_start_path(RingNo, DeviceIP)
    End Function
#End Region

#Region "start/stop move all"
    Public Function AMaxM4_Set_R_Move_All(ByVal AxisIP As Integer(), ByVal distance As Integer()) As Short

        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short() = New Short(AxisIP.Length - 1) {}

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        For index = 0 To AxisIP.Length - 1
            Call AMax_Get_Moton_DeviceIP(AxisIP(index), RingNo, DeviceIP, PortNo(index))
        Next

        Return B_mnet_m4_set_r_move_all(RingNo, DeviceIP, PortNo.Length, PortNo(0), distance(0))      ' pass by reference 
        'For index = 0 To PortNo.Length - 1
        '    B_mnet_m4_set_r_move_all(RingNo, DeviceIP, 1, PortNo(index), distance(index))
        'Next

        'Return 0
        'Return B_mnet_m4_set_r_move_all(RingNo, DeviceIP, AxisIP.Length, PortNo, distance)      ' pass by reference 


    End Function
    Public Function AMaxM4_Set_A_Move_All(ByVal AxisIP As Integer(), ByVal distance As Integer()) As Short

        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short() = New Short(AxisIP.Length - 1) {}

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        'Call AMax_Get_Moton_DeviceIP(AxisIP(0), RingNo, DeviceIP, PortNo)

        For index = 0 To AxisIP.Length - 1
            Call AMax_Get_Moton_DeviceIP(AxisIP(index), RingNo, DeviceIP, PortNo(index))
        Next

        'Return B_mnet_m4_set_a_move_all(RingNo, DeviceIP, 1, PortNo, distance)      ' pass by reference 
        Return B_mnet_m4_set_a_move_all(RingNo, DeviceIP, AxisIP.Length, PortNo(0), distance(0))      ' pass by reference 
    End Function
    Public Function AMaxM4_Start_Move_All(ByVal AxisIP As Integer) As Short

        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

        Return B_mnet_m4_start_move_all(RingNo, DeviceIP, PortNo)      ' pass by reference 
    End Function
    Public Function AMaxM4_Set_Move_All_Stop_Mode(ByVal AxisIP As Integer, ByVal mode As Short, ByVal FallorRise As Short) As Short
        'mode: 0=level, 1:edge
        'FallorRise: 若mode=level, 0=GND作動，1=24V作動。若mode=edge, 0=24V->GND作動，1=GND->24V作動
        Dim RingNo As Short, DeviceIP As Short, PortNo As Short
        Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)
        Return B_mnet_m4_set_moveall_stop_mode(RingNo, DeviceIP, mode, FallorRise)
    End Function
#End Region

    Public Sub boxErrAmaxMotion(ByVal strErrStatus As String)
        ' Show a formatted message box when error
        Dim Msg, Style, Title, Help, Ctxt, Response
        Msg = "AMax Motion Error"                      ' 定義訊息。
        Style = vbOKOnly + vbCritical                  ' 定義按鈕。
        Title = "AMax Motion Error"                    ' 定義標題。
        Help = "DEMO.HLP"                              ' 定義說明檔。
        Ctxt = 1000                                    ' 定義內容代碼。

        Response = MsgBox(strErrStatus, Style, Title)
    End Sub
    Public Function strErrAmaxInit(ByVal RingNo As Short, ByVal DeviceIP As Short, ByVal MStatus As Short) As String
        ' Return a formatted error string , especially for initial phase
        'strErrAmaxInit = "Motion Initialize Error" & Chr(13) & "Ring : " & CStr(RingNo) & ", DeviceIP : " & CStr(DeviceIP) & Chr(13) & AlarmAmet.ErrorMsg(-MStatus)
        strErrAmaxInit = "Motion Initialize Error" & Chr(13) & "Ring : " & CStr(RingNo) & ", DeviceIP : " & CStr(DeviceIP) & Chr(13) & [Enum].GetName(GetType(returnErrorCodes), -MStatus)
    End Function
    Public Function strErrAmaxMotion(ByVal AxisIP As Integer, ByVal MStatus As Short) As String
        ' Return a formatted error string , especailly for working phase
        SyncLock Locking
            Dim RingNo As Short, DeviceIP As Short, PortNo As Short
            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(AxisIP, RingNo, DeviceIP, PortNo)

            strErrAmaxMotion = String.Format("Motion Error: {1}{0}Ring:{2},DeviceIP:{3},PortNo:{4}{0}{5}",
                                             Environment.NewLine,
                                             pData.MotorSettings(AxisIP).MotorName, CStr(RingNo), CStr(DeviceIP), CStr(PortNo), [Enum].GetName(GetType(returnErrorCodes), MStatus))
        End SyncLock
    End Function

End Module
