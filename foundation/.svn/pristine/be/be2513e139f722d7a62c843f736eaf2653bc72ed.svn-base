Imports Automation
Imports Automation.Components.CommandStateMachine
Imports Automation.Components.Services
Imports Automation.Components
Imports System.ComponentModel

Class testAttribute
    Inherits Attribute



End Class

Public Class bufferBase
    Inherits systemControlPrototype
    Implements IModuleMulti
    Implements IFinishableStation

    <Category("BufferBase")>
<DisplayName("除能")>
<TypeConverter(GetType(utilitiesUI.trueFalseTypeConvertor1))>
    Property IsBypassed As Boolean = False

    <DisplayName("物理容量")>
                <Category("BufferBase")>
    Property PhysicalCapacity As Integer
        Get
            Return __physicalCapacity
        End Get
        Set(value As Integer)
            If __logicalCapacity > value Then
                'shrink logical capacity if need
                __logicalCapacity = value
            End If
            __physicalCapacity = value
        End Set
    End Property
    <DisplayName("邏輯容量")>
            <Category("BufferBase")>
    Property LogicalCapacity As Integer
        Get
            Return __logicalCapacity
        End Get
        Set(value As Integer)
            If value <= __physicalCapacity Then
                __logicalCapacity = value
            Else
                '------------------
                'otherwise , reject
                '------------------
            End If
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("目前存量")>
        <Category("BufferBase")>
    ReadOnly Property CurrentStoredAmount As Integer
        Get
            Return currentIndex
        End Get
    End Property

    Dim __physicalCapacity As Integer = 15
    Dim __logicalCapacity As Integer = 15

#Region "control members"
    Public __liftMotorControl As motorControlDrivable = New motorControlDrivable With {.IsEnabled = True}
    Public __pointTop As cMotorPoint = Nothing
    ''' <summary>
    ''' Definition: upward/store
    ''' </summary>
    ''' <remarks></remarks>
    Public __pointPitch As cMotorPoint = Nothing

    'Dim sensorCollection As List(Of sensorControl) = New List(Of sensorControl)
    Public bottomSensor As sensorControl = New sensorControl With {.IsEnabled = True}

    Dim dataCollection As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase)
#End Region

    Public Property FinishableFlags As New flagController(Of IFinishableStation.controlFlags) Implements IFinishableStation.FinishableFlags
    Public Property UpstreamStations As List(Of IFinishableStation) Implements IFinishableStation.UpstreamStations
    Public Property TargetPositionInfos As List(Of Func(Of shiftDataPackBase)) Implements IModuleMulti.TargetPositionInfo

    ''' <summary>
    ''' Top : index-0
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function location(index As Integer) As cMotorPoint
        Dim output As cMotorPoint = __pointTop.Clone
        output.DistanceInUnit += __pointPitch.DistanceInUnit * index
        Return output
    End Function
    Protected currentIndex As Integer = PhysicalCapacity - 1 'bottom

    Enum decisionEnums As Integer
        STORE = 100
        RELEASE = 200
        ''' <summary>
        ''' Reset MA only
        ''' </summary>
        ''' <remarks></remarks>
        PASS = 10
        ''' <summary>
        ''' MA holding
        ''' </summary>
        ''' <remarks></remarks>
        HOLD = 0
    End Enum

    Overridable Function makeDecision() As decisionEnums
        Return decisionEnums.PASS
    End Function

    Function stateIgnite() As Integer

        Select Case systemSubState
            Case 0
                If FinishableFlags.viewFlag(IFinishableStation.controlFlags.COMMAND_IGNITE) Then
                    systemSubState = 10
                Else
                    '---------------------
                    '   Wait Command
                    '---------------------
                End If
            Case 10
                If __liftMotorControl.drive(motorControl.motorCommandEnum.GO_HOME) =
                     IDrivable.endStatus.EXECUTION_END Then
                    currentIndex = PhysicalCapacity - 1 'bottom
                    systemSubState = 20
                Else

                End If
                '-----------------------------
                '   Search The First Occurance
                'From Bottom To Top
                '-----------------------------
            Case 20
                If __liftMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION,
                                            location(currentIndex)) =
                                        IDrivable.endStatus.EXECUTION_END Then
                    systemSubState = 30
                Else
                    '-----------------
                    '   Settling
                    '-----------------
                End If
            Case 30
                If bottomSensor.IsSensorCovered Or
                    currentIndex = 0 Then
                    '------------------
                    'First Occurance Searched
                    '------------------
                    systemSubState = 500
                ElseIf bottomSensor.OffTimer.TimeElapsed.TotalMilliseconds > 100 And
                  (currentIndex > 0) Then
                    '---------
                    'inexisted
                    '---------
                    'do next searching
                    currentIndex -= 1 'drive go down/ stack move upward
                    systemSubState = 20
                End If
            Case 500
                FinishableFlags.resetFlag(IFinishableStation.controlFlags.COMMAND_IGNITE)
                systemMainState = systemStatesEnum.EXECUTE
        End Select

        Return 0
    End Function

    Function stateExecute() As Integer

        Select Case systemSubState
            Case decisionEnums.HOLD
                '-------------------------------------------
                '   Otherwise , making a effective decision
                '-------------------------------------------
                If IsBypassed Then
                    systemSubState = decisionEnums.PASS
                Else
                    systemSubState = makeDecision()
                End If
            Case decisionEnums.PASS
                TargetPositionInfos.First.Invoke.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = decisionEnums.HOLD
            Case decisionEnums.STORE
                '---------------
                '   Store Action
                '---------------
                If currentIndex >= __logicalCapacity - 1 Then
                    'impossible to store
                    systemSubState = decisionEnums.PASS
                ElseIf __liftMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION,
                                            location(currentIndex + 1)) Then

                    'data storage
                    dataCollection(currentIndex).Assign(TargetPositionInfos.First.Invoke)
                    currentIndex += 1

                    systemSubState = decisionEnums.PASS
                Else
                    '--------------------
                    '   Motor Settling
                    '--------------------
                End If
            Case decisionEnums.RELEASE
                '---------------
                '   Release Action
                '---------------
                If currentIndex = 0 Then
                    'impossible to release
                    systemSubState = decisionEnums.PASS
                ElseIf __liftMotorControl.drive(motorControl.motorCommandEnum.GO_POSITION,
                                            location(currentIndex - 1)) Then

                    'data releasing
                    TargetPositionInfos.First.Invoke.Assign(dataCollection(currentIndex))
                    currentIndex -= 1

                    systemSubState = decisionEnums.PASS
                Else
                    '--------------------
                    '   Motor Settling
                    '--------------------
                End If
        End Select


        Return 0
    End Function

    Sub New(seed As shiftDataPackBase)
        Me.systemMainStateFunctions(systemStatesEnum.IGNITE) = AddressOf stateIgnite
        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        Me.systemMainState = systemStatesEnum.IGNITE

        For index = 0 To PhysicalCapacity - 1
            dataCollection.Add(seed.Clone)
        Next

        Me.initialize = [Delegate].Combine(Me.initialize,
                                           New Func(Of Integer)(AddressOf initMappingAndSetup))

    End Sub

    Function initMappingAndSetup() As Integer
        With __liftMotorControl
            .SlowdownEnable = enableEnum.ENABLE
            .SlowdownLatch = sdLatchEnum.LATCH
            .SlowdownMode = sdModeEnum.SLOW_DOWN_STOP
        End With
        Return 0
    End Function


End Class
