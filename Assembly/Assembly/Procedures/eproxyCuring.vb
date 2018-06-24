Imports Automation
Imports AutoNumeric
Imports Automation.Components
Imports Automation.Components.Services
Imports System.ComponentModel

Public Class cureSetting
    Inherits settingBase

    Property UVCureDuration As Integer = 10
    ''' <summary>
    ''' Specification : 4W/cm2 (irradiance
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Description("W/cm2")>
    Property Irradiance As Single = 4.0F
    <Description("In Percentage")>
    Property IrisLevel As Integer = 20

    Property PostCureWaitTime As Integer = 10

    Property Mode As uvCuringMode = uvCuringMode.ABSOLUTE

    Public Overrides Property Filename As String
        Get
            Return String.Format("{0}{1}.xml",
                                 measureProcedureSetting.settingPath,
                                 Me.ToString)
        End Get
        Set(value As String)
            'nothing
        End Set
    End Property

End Class

''' <summary>
''' 1. curing
''' 2. recording the variance of energy
''' </summary>
''' <remarks></remarks>
Public Class eproxyCuring
    Inherits systemControlPrototype
    Implements IProcedure
    Implements IDisposable
    Implements IFinishableStation



    Friend __cureSetting As cureSetting = New cureSetting

    Public Property Arguments As Object Implements IProcedure.Arguments
        Get
            Return Nothing
        End Get
        Set(value As Object)
            currentCuringData = value

            __nextCuringPosition = pData.MotorPoints(motorPoints.MpUV_FIRST).Clone
            __nextCuringPosition.DistanceInUnit += pData.MotorPoints(motorPoints.MpUV_PITCH).DistanceInUnit *
                CType(value, Func(Of materialData)).Invoke.IndexInArray
        End Set
    End Property
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property IsProcedureAbort As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort
    Public Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result

    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations


    Dim __nextCuringPosition As cMotorPoint = Nothing

    Dim __dummyMaterial As materialData = New materialData(8)
    Dim currentCuringData As Func(Of materialData) = Function() (__dummyMaterial)
#Region "control members"
    Dim uvControlMotor As motorControlDrivable = New motorControlDrivable With {.IsEnabled = True}
    Dim uvCureStation As uvCure = New uvCure With {.IsEnabled = True}
    Dim __postCuringTimer As singleTimer = New singleTimer
    Dim readyPosition As cMotorPoint = Nothing
#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState += 10
                Else
                    '---------------
                    '   Waiting
                    '---------------
                End If
            Case 10
                If uvControlMotor.drive(motorControlDrivable.motorCommandEnum.GO_HOME) =
                     IDrivable.endStatus.EXECUTION_END Then
                    systemSubState += 10
                Else
                    '---------------
                    '   Homing
                    '---------------
                End If
            Case 20
                If uvControlMotor.drive(motorControlDrivable.motorCommandEnum.GO_POSITION, readyPosition) =
                     IDrivable.endStatus.EXECUTION_END Then

                    FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                    systemMainState = systemStatesEnum.EXECUTE
                Else
                    '---------------
                    '   Positioning
                    '---------------
                End If
        End Select

        Return 0
    End Function


    ''' <summary>
    ''' 1. move motor to corresponding position
    ''' 2. start curing
    ''' 3. record reading during curing(both channel
    ''' 4. until curing finished
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateExecute() As Integer

        If IsProcedureAbort.readFlag(interlockedFlag.POSITION_OCCUPIED) Then
            IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            systemSubState = 0 'reset
        End If

        With currentCuringData.Invoke

            If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                .cureDatas(dataKeysDefine.VOLTAGE_LEFT).Push = mainIOHardware.readDouble(inputAddress.PD_LEFT)
                .cureDatas(dataKeysDefine.VOLTAGE_RIGHT).Push = mainIOHardware.readDouble(inputAddress.PD_RIGHT)

            Else
                '---------------------
                '   Do not read
                '---------------------
            End If

            Select Case systemSubState
                Case 0
                    If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                        systemSubState += 10
                    Else

                    End If
                Case 10
                    If uvControlMotor.drive(motorControlDrivable.motorCommandEnum.GO_POSITION, __nextCuringPosition) =
                         IDrivable.endStatus.EXECUTION_END Then
                        'start curing

                        With __cureSetting
                            Select Case .Mode
                                Case uvCuringMode.ABSOLUTE
                                    uvCureStation.Arguments = {.Irradiance,
                                                               .UVCureDuration,
                                                               .Mode}
                                Case uvCuringMode.RELATIVE
                                    uvCureStation.Arguments = {.IrisLevel,
                                                               .UVCureDuration,
                                                               .Mode}
                            End Select
                        End With
                        uvCureStation.IsProcedureStarted.setFlag(interlockedFlag.POSITION_OCCUPIED)

                        .cureDatas(dataKeysDefine.VOLTAGE_LEFT).setCureParameters(curingDataEachChannel.cureParametersEnum.PRE_POWER)
                        .cureDatas(dataKeysDefine.VOLTAGE_RIGHT).setCureParameters(curingDataEachChannel.cureParametersEnum.PRE_POWER)

                        systemSubState = 100
                    Else
                        '-----------------------------
                        '   Settling
                        '-----------------------------
                    End If
                Case 100
                    If Not uvCureStation.IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then

                        .cureDatas(dataKeysDefine.VOLTAGE_LEFT).setCureParameters(curingDataEachChannel.cureParametersEnum.POST_POWER_1)
                        .cureDatas(dataKeysDefine.VOLTAGE_RIGHT).setCureParameters(curingDataEachChannel.cureParametersEnum.POST_POWER_2)

                        __postCuringTimer.TimerGoal = New TimeSpan(0, 0, __cureSetting.PostCureWaitTime)
                        __postCuringTimer.IsEnabled = True

                        systemSubState = 400
                    Else
                        '-------------------------------------
                        '   Curing , Record Left/Right Reading
                        '-------------------------------------
                    End If
                Case 400
                    '-------------------------------
                    '   Stablizing Left
                    '-------------------------------
                    If .cureDatas(dataKeysDefine.VOLTAGE_LEFT).IsStabled Then
                        .cureDatas(dataKeysDefine.VOLTAGE_LEFT).setCureParameters(curingDataEachChannel.cureParametersEnum.POST_POWER_2)
                        systemSubState = 450
                    Else
                        '---------------------------
                        '   Left Stablizing
                        '---------------------------
                    End If

                Case 450
                    '-------------------------------
                    '   Stablizing Right
                    '-------------------------------
                    If .cureDatas(dataKeysDefine.VOLTAGE_RIGHT).IsStabled Then
                        .cureDatas(dataKeysDefine.VOLTAGE_RIGHT).setCureParameters(curingDataEachChannel.cureParametersEnum.POST_POWER_2)
                        systemSubState = 500
                    Else
                        '---------------------------
                        '   Right Stablizing
                        '---------------------------
                    End If
                Case 500
                    If uvControlMotor.drive(motorControlDrivable.motorCommandEnum.GO_POSITION, readyPosition) =
                         IDrivable.endStatus.EXECUTION_END Then
                        systemSubState += 10
                    Else
                        '-----------------------------
                        '   Settling
                        '-----------------------------
                    End If
                Case 510
                    If __postCuringTimer.IsTimerTicked Then
                        'release gripper , 
                        systemSubState += 10
                    Else
                        '--------------
                        '   Counting Down
                        '--------------
                    End If
                Case 520
                    If Assembly.Instance.gripperClampControl.drive(cylinderGeneric.cylinderCommands.GO_A_END) =
                         IDrivable.endStatus.EXECUTION_END Then
                        systemSubState += 10
                    Else
                        '--------------
                        '
                        '--------------
                    End If
                Case 530
                    IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                    systemSubState = 0

            End Select

        End With


        Return 0
    End Function


#Region "singleton interface"
    ''' <summary>
    ''' Singalton pattern
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property Instance As eproxyCuring
        Get
            If __instance Is Nothing Then
                __instance = New eproxyCuring
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As eproxyCuring = Nothing
#End Region


    Protected Sub New()
        __cureSetting.Load(Nothing)
        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initMappingAndSetup),
                                           New Func(Of Integer)(AddressOf initSubsystemInitialize))

        Me.systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.IGNITE

        Me.IsEnabled = True
    End Sub
    Function initMappingAndSetup() As Integer
        Me.uvControlMotor.MotorIndex = motorAddress.MpUV
        Me.readyPosition = pData.MotorPoints(motorPoints.MpUV_READY)

        Me.Arguments = currentCuringData ' dummy at begiining
        Return 0
    End Function


#Region "IDisposable Support"
    Private disposedValue As Boolean ' 偵測多餘的呼叫

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                uvCureStation.Dispose()
            End If
            __cureSetting.Save()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
