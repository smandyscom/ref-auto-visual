Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO

Public Module mdlAMONetPosDefine
    Public Const MinPos As Integer = -134217728
    Public Const MaxPos As Integer = 134217727
    Public pData As cPositionPara = New cPositionPara


    Public Enum UnitEnum
        MM = 0
        DEG = 1
    End Enum

    Public Enum eActive
        ActLo = 0
        ActHi = 1
    End Enum
    Public Enum eDir
        Neg = 0
        Pos = 1
    End Enum

    Public Enum eSwitch  '報警處理狀態
        eOFF = 0
        eON = 1
    End Enum

    Public Enum eCheckStatus  '報警處理狀態
        eOFF = 0
        eON = 1
    End Enum

    <Serializable()>
    <EditorBrowsable(EditorBrowsableState.Always)>
    <Editor(GetType(MotorPointTypeEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Class cMotorPoint
        Implements ICloneable

        '點位的內容
        <DisplayName("點位名稱")> Property PointName As String = "default"
        <DisplayName("座標形式")> Property PointType As pointTypeEnum = pointTypeEnum.ABS '點位模式: 0=Abs,1=Rel ,  Home模式: 0=-, 1=+
        <DisplayName("對應軸編號")> Property AxisIndex As Short = 0 'the serial number of motor , 'turn back to basic data type ,remarked by Hsien , 2014/5/28 , eMotor '馬達編號
        '-------------------------------------
        'Unit in pulses
        '-------------------------------------
        <DisplayName("起始速度（脈波）")> Property StartVelocity As Double = 1.0F 'Start velocity , in pulses/sec
        <DisplayName("終端速度（脈波）")> Property Velocity As Double = 10.0F 'Max velocity       in pulses/sec
        <DisplayName("軸座標/距離（脈波）")> Property Distance As Double = 10.0F ' Coordinate value referenced to origin point in pulses
        '------------------------------------
        'Unit in secs
        '------------------------------------
        <DisplayName("加速時間")> Property AccelerationTime As Double = 0.1F ' Accerleration time in sec
        <DisplayName("減速時間")> Property DecelerationTime As Double = 0.1F ' Decerleration time , Hsien . 2015.01.20
        <DisplayName("S曲線加速時間")> Property SShapeAccelerationTime As Double = 0.0F ' S-Shape Accerleration time in sec
        <DisplayName("S曲線減速時間")> Property SShapeDecelerationTime As Double = 0.0F ' S-Shape Decerleration time , Hsien . 2015.01.23

        <DisplayName("原點/非原點")> Property IsHomePoint As Boolean = False 'indicate if this point is used to homing remarked by Hsien , 2014/5/27

        <DisplayName("速度曲線")> Property VelocityProfile As velocityProfileEnum = velocityProfileEnum.S_CURVE  'indicate which profile used to apply
        <DisplayName("更多資訊")> Property moreInfo As String = ""  'add more infomation from assembly

        <DisplayName("整定誤差容許量（脈波）")>
        Property PositionTolerance As Double = 800.0F          'unit in pulses, used in potion with check
        <DisplayName("整定累計")>
        Property WindowLength As Integer = 100 ' should not be less than 2
        '----------------------------------------
        '   Attributes represented in UNIT , reference to motorSetting
        '----------------------------------------
        <XmlIgnore()>
        <DisplayName("起始速度（單位）")>
        Property StartVelocityInUnit As Double
            Get
                Return pulseToUnit(StartVelocity)
            End Get
            Set(value As Double)
                StartVelocity = unitToPulse(value)
            End Set
        End Property
        <XmlIgnore()>
        <DisplayName("終端速度（單位）")>
        Property VelocityInUnit As Double
            Get
                Return pulseToUnit(Velocity)
            End Get
            Set(value As Double)
                Velocity = unitToPulse(value)
            End Set
        End Property
        <XmlIgnore()>
        <DisplayName("軸座標/距離（單位）")>
        Property DistanceInUnit As Double
            Get
                Return pulseToUnit(Distance)
            End Get
            Set(value As Double)
                Distance = unitToPulse(value)
            End Set
        End Property
        <XmlIgnore()>
        <DisplayName("整定誤差容許量（單位）")>
        Property PositionToleranceInUnit As Double
            Get
                Return pulseToUnit(PositionTolerance)
            End Get
            Set(value As Double)
                PositionTolerance = unitToPulse(value)
            End Set
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            'Hsien , 2014.11.20 , clone definition
            Dim copy As cMotorPoint = MemberwiseClone()
            copy.PointName = String.Copy(Me.PointName)
            Return copy
        End Function
        '------------------------------
        '   Expansion
        '   Hsien , 2014.11.26
        '------------------------------
        Private Function pulseToUnit(ByVal pulse As Double) As Double
            If (pData.MotorSettings(Me.AxisIndex) Is Nothing Or
                pData.MotorSettings(Me.AxisIndex).PulsePerUnit = 0) Then
                '-------------------
                '   Error , setting not found
                '-------------------
                Return 0
            End If
            Return pulse / pData.MotorSettings(Me.AxisIndex).PulsePerUnit
        End Function
        Private Function unitToPulse(ByVal unit As Double) As Double
            If (pData.MotorSettings(Me.AxisIndex) Is Nothing) Then
                '-------------------
                '   Error , setting not found
                '-------------------
                Return 0
            End If

            Return unit * pData.MotorSettings(Me.AxisIndex).PulsePerUnit
        End Function

        ReadOnly Property IsAccumulationEnough As Boolean
            Get
                Return __accumulatedPosition.Count = (WindowLength - 1)
            End Get
        End Property
        ''' <summary>
        ''' The Moving Average
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()>
        ReadOnly Property AveragePosition As Double
            Get
                Return __accumulatedPosition.Average
            End Get
        End Property

        ''' <summary>
        ''' The Local Maximum
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()>
        ReadOnly Property MaxPosition As Double
            Get
                Return __accumulatedPosition.Max
            End Get
        End Property

        ''' <summary>
        ''' The Local Minimum
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()>
        ReadOnly Property MinPosition As Double
            Get
                Return __accumulatedPosition.Min
            End Get
        End Property

        ''' <summary>
        ''' Input the next position
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        <XmlIgnore()>
        WriteOnly Property NextPosition As Double
            Set(value As Double)
                __accumulatedPosition.Enqueue(value)
                'one out one in, if fulfilled
                If __accumulatedPosition.Count = WindowLength Then
                    __accumulatedPosition.Dequeue()
                End If
            End Set
        End Property
        Friend __accumulatedPosition As ListAsQueue(Of Double) = New ListAsQueue(Of Double)
        '------------------
        '   In order to recognize in collection editor
        '   Hsien , 2015.03.17
        '------------------
        Public Overrides Function ToString() As String
            Return PointName
        End Function

    End Class

    Public Class cMotorSetting
        Implements ICloneable

        '-----------------------------------------
        ' setting parameters for individual motor , configuration is specialized for AMAX system
        ' Hsien 
        '-----------------------------------------
        '馬達的參數
        <Category("名稱/站別")> <DisplayName("馬達名稱")> Property MotorName As String = "default motor" '馬達名稱
        <Category("名稱/站別")> <DisplayName("站別名稱")> Property Station As String = "default station"
        '------------------------------------
        'the axis address in AMONet , remarked by Hsien , 2014/5/28
        '------------------------------------
        <Category("Ring-Device-Axis設定")> <DisplayName("Ring標號")> Property RingIndex As Short 'RingNo
        <Category("Ring-Device-Axis設定")> <DisplayName("Device標號")> Property DeviceIp As Short 'IpNo           device ip
        <Category("Ring-Device-Axis設定")> <DisplayName("軸標號")> Property AxisIndex As Short  'AxisNo

        <DisplayName("SERVO-ON準位")> Property ServoOnLevel As outputActiveSettingEnum 'drive on 極性, GND=0=24G, VDD=1=24V 
        <DisplayName("Encoder方向")> Property EncoderDir As Short '0:Not inverse direction , 1:Inverse direction 

        <DisplayName("回授訊號形式")> Property PulseInputMode As pulseInputModeEnum  '0: 1X A/B  ,1:  2X A/B,2:  4X A/B  ,3:  CW/CCW  
        <DisplayName("輸出脈波形式")> Property PulseOutputMode As pulseOutputModeEnum '用1P的方式設定馬達移動方向
        <DisplayName("回授訊號來源")> Property FeedBackSource As feedBackSourceEnum '0:External Feedback, 1:Command pulse
        <DisplayName("ALARM準位")> Property AlarmLevel As inputActiveModeEnum
        <DisplayName("HOME準位")> Property HomeLevel As inputActiveModeEnum = inputActiveModeEnum.LOW_ACTIVE
        <DisplayName("In-Pos致能")> Property InPosEnabled As enableEnum 'in position訊號是否要致能
        <DisplayName("In-Pos準位")> Property InPosLevel As inputActiveModeEnum 'in position極性
        <DisplayName("Latch準位")> Property LatchLevel As inputActiveModeEnum
        <DisplayName("Slowdown準位")> Property SlowDownLevel As inputActiveModeEnum = inputActiveModeEnum.LOW_ACTIVE

        <DisplayName("脈波數/單位")> Property PulsePerUnit As Double = 8192           ' ppm : pulses per mm

        <DisplayName("單位形式")> Property Unit As UnitEnum  'added by  Hsien , 2014/5/30

        '------------------
        '   In order to recognize in collection editor
        '   Hsien , 2015.03.17
        '------------------
        Public Overrides Function ToString() As String
            Return String.Format("{0},{1}",
                                 Station,
                                 MotorName)
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function

        Public Function applyConfiguration() As Integer
            Return applyConfiguration(Me)  'wrapper
        End Function
        Public Shared Function applyConfiguration(setting As cMotorSetting) As Integer '初始化AMAX-224X
            '-------------------------------------------
            ' Inject the pData into physical hardware layer of AMONet setting
            ' Hsien 
            '-------------------------------------------
            Dim returnError As Integer = 0

            With setting

                '-----------------------------------------------------------
                ' Drain out motor setting from pData.MotorSettings
                ' then call native AMONet motor set functions to set-up motors
                ' All setting was preseted in XML file
                ' Hsien
                '--------------------------------------------------------------

                '--------------------------------------------------------------
                '設定編碼器輸入的形式
                ' pls_iptmode : Setting of encoder feedback pulse input mode ( 0=1X A/B; 1=2X A/B; 2=4X A/B; 3=CW/CCW)
                ' pls_logic : Logic of encoder feedback pulseValue  Meaning (0=Not inverse direction;1= Inverse direction
                '---------------------------------------------------------------
                returnError += B_mnet_m4_set_pls_iptmode(.RingIndex, .DeviceIp, .AxisIndex, .PulseInputMode, .EncoderDir)
                '-----------------------------------------------
                '設定脈波輸出的格式
                '0 OUT/DIR, OUT Falling edge, DIR+ is high level
                '1 OUT/DIR, OUT Rising edge, DIR+ is high level
                '2 OUT/DIR, OUT Falling edge, DIR+ is low level
                '3 OUT/DIR, OUT Rising edge, DIR+ is low level
                '4 OUT/DIR, OUT low active, DIR+ is high level
                '5 A/B Phase
                '6 B/A Phase
                '7 CW/CCW
                '------------------------------------------------
                returnError += B_mnet_m4_set_pls_outmode(.RingIndex, .DeviceIp, .AxisIndex, .PulseOutputMode)

                '-----------------------------------------------
                '設定回授的來源
                '0 External Feedback; 1:  Command Pulse
                '-----------------------------------------------
                returnError += B_mnet_m4_set_feedback_src(.RingIndex, .DeviceIp, .AxisIndex, .FeedBackSource)

                '------------------------------------------------
                '0: Low Active 1: High Active
                '0: Motor immediately stops(Default) 1: Motor decelerates then stops
                ' Alarm Logic : Set alarm logic and operating mode
                '-------------------------------------------------
                returnError += B_mnet_m4_set_alm(.RingIndex, .DeviceIp, .AxisIndex, .AlarmLevel, alarmModeEnum.IMMEDIATELY_STOP)

                '------------------------------------------------------------------------------------
                '設定原點邏輯(Set the home/index logic configuration and homing mode.)
                'home_mode : Range: 0~12
                'org_logic : Action logic configuration for ORG signal (0:Low Active, 1: High Active)
                'ez_logic : Action logic configuration for EZ signal(0:Low Active, 1: High Active)
                'ez_count : Range : 0~15
                'erc_out : Set ERC output options.(Clear Servo Error Counter Signal Output) (0:No ERC Out, 1:ERC Out when homing finish)
                '-------------------------------------------------------------------------------------
                returnError += B_mnet_m4_set_home_config(.RingIndex, .DeviceIp, .AxisIndex, 1, .HomeLevel, ezActiveModeEnum.LOW_ACTIVE, 0, enableEnum.DISABLE)

                '---------------------------------------------------------------------------------------------------
                '回原點完成後是否重置計數器(Enable reset counter when homing is complete)
                ' 0:Don’t reset counter when homing is complete, 1: Reset counter when homing is complete (Default)
                '----------------------------------------------------------------------------------------------------
                returnError += B_mnet_m4_enable_home_reset(.RingIndex, .DeviceIp, .AxisIndex, 1)

                '--------------------------------------------------------------------------------------
                '極限停止(End limit, indicate the limit of motion in plus direction or minus direction)
                '0: Motor immediately stops(Default) 1: Motor decelerates then stops
                '---------------------------------------------------------------------------------------
                returnError += B_mnet_m4_set_el(.RingIndex, .DeviceIp, .AxisIndex, 0) '0: motor immediately stops (default)

                '---------------------------------------------------------------------------------------
                '設定In Position訊號(Set Servo In Position Signal)
                '0:Disabled (Default) 1:Enabled
                '0:Low Active 1:High Active
                '----------------------------------------------------------------------------------------
                returnError += B_mnet_m4_set_inp(.RingIndex, .DeviceIp, .AxisIndex, .InPosEnabled, .InPosLevel)

                '-----------------------------------------------------------------------------------------
                '設定鎖定邏輯( when the Latch is triggered, the command counter, feedback counter and error counter will be latched.)
                '0: Low Active; 1: High Active
                '-----------------------------------------------------------------------------------------
                returnError += B_mnet_m4_set_ltc_logic(.RingIndex, .DeviceIp, .AxisIndex, .LatchLevel)

                '------------------------------------------------------------------------------------------
                '設定SD(Signal Deceleration) logic and operating mode (Disabled : Default )
                ' This signal can be used as a deceleration signal or a deceleration stop signal, according to the software settings.
                '------------------------------------------------------------------------------------------
                returnError += B_mnet_m4_set_sd(.RingIndex, .DeviceIp, .AxisIndex, 0, .SlowDownLevel, 0, 0) 'set sd hi active

                '--------------------------------
                '設定ABS模式 , Hsien , 2015.08.21
                '--------------------------------
                returnError += B_mnet_m4_set_abs_mode(.RingIndex, .DeviceIp, .AxisIndex, 1) '0: feedback , 1:command

                '--------------
                '使馬達伺服致能
                '--------------
                returnError += B_mnet_m4_set_svon(.RingIndex, .DeviceIp, .AxisIndex, .ServoOnLevel) 'Servo on

                '------------------------------------------------------------------------
                '設定 ERC(Clear Servo Error Counter Signal Output) logic and timing
                'Call B_mnet_m4_set_erc_on(RingNo, DeviceIP, PortNo, 1)   '1: Active (ON)
                '重置命令位置
                '-------------------------------------------------------------------------
                returnError += B_mnet_m4_reset_command(.RingIndex, .DeviceIp, .AxisIndex)

                '----------------
                '重置真實位置()
                '----------------
                returnError += B_mnet_m4_reset_position(.RingIndex, .DeviceIp, .AxisIndex)

                '--------------
                '重置錯誤計數器
                '--------------
                returnError += B_mnet_m4_reset_error_counter(.RingIndex, .DeviceIp, .AxisIndex)

                Return returnError
            End With

        End Function

    End Class

    Public Class cPositionPara
        Implements ICloneable
        Implements IPersistance

        '------------------------------------
        ' Regulared data type
        'remarked by Hsien , 2014/5/28
        '------------------------------------
        <DisplayName("馬達設定")> Property MotorSettings As List(Of cMotorSetting) = New List(Of cMotorSetting)  '取得enum的個數，記錄馬達基本參數
        <DisplayName("教點設定")> Property MotorPoints As List(Of cMotorPoint) = New List(Of cMotorPoint)

        '-----------------------------------
        '   Offered the Possibility to Clone
        '   hsien  ,2015.10.05
        '-----------------------------------
        Public Function Clone() As Object Implements ICloneable.Clone
            Dim __copy As cPositionPara = New cPositionPara
            With __copy
                Me.MotorSettings.ForEach(Sub(__setting As cMotorSetting) .MotorSettings.Add(__setting.Clone))
                Me.MotorPoints.ForEach(Sub(__point As cMotorPoint) .MotorPoints.AddRange(__point.Clone))
            End With
            Return __copy
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            'todo , compare all values , to check if values are in equal

            Return MyBase.Equals(obj)
        End Function

        Shared __xmlSerializer As XmlSerializer = New XmlSerializer(GetType(cPositionPara))

#Region "persistance interface"
        Public Sub Create(filename As String) Implements IPersistance.Create
            Me.Filename = filename
            Save()
        End Sub

        <Browsable(False)>
        Public Property Filename As String Implements IPersistance.Filename

        Public Sub Load(filename As String) Implements IPersistance.Load
            Using sr As StreamReader = New StreamReader(filename)
                Dim tempData As cPositionPara = __xmlSerializer.Deserialize(sr)
                '-----------------------------
                '   Value Assignation
                '-----------------------------
                MotorSettings.Clear()
                MotorSettings.AddRange(tempData.MotorSettings)

                MotorPoints.Clear()
                MotorPoints.AddRange(tempData.MotorPoints)
            End Using

            Me.Filename = filename
        End Sub

        Public Sub Save() Implements IPersistance.Save
            Using sw As StreamWriter = New StreamWriter(Me.Filename)
                __xmlSerializer.Serialize(sw, Me)
            End Using
        End Sub
        Sub New()
            Me.Filename = My.Application.Info.DirectoryPath & "\Data\" & "MotionPosData.xml"
        End Sub
#End Region

    End Class 'Class cPara


    Public Function fetchAllCommandPositions() As List(Of Integer)
        '----------------------------------
        '   Used to fetch all command position , and build a list
        ' Hsien , 2015.10.05
        '----------------------------------
        Dim RingNo As Short
        Dim DeviceIP As Short
        Dim PortNo As Short

        Dim __commandPosition As Integer = 0

        Dim __result As List(Of Integer) = New List(Of Integer)

        For index = 0 To pData.MotorSettings.Count - 1

            '=== Get the RingNo, DeviceIP and PortNo from Look-Up Table ===
            Call AMax_Get_Moton_DeviceIP(index, RingNo, DeviceIP, PortNo)
            B_mnet_m4_get_command(RingNo, DeviceIP, PortNo, __commandPosition)
            __result.Add(__commandPosition)
        Next

        Return __result
    End Function

End Module