Imports System.Runtime.CompilerServices
Imports Automation.mainIOHardware

<Assembly: InternalsVisibleTo("componentMainline")> 
<Assembly: InternalsVisibleTo("UnitTestProjectCompMainline")> 
''' <summary>
''' Used on unloading placing
''' </summary>
''' <remarks></remarks>
Public Class targetLaneStandard

    Enum laneConfiguration
        INDIVIDUAL = 1
        HALF_LANE = 2
    End Enum
   

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event TargetChosen(ByVal sender As Object, ByVal e As EventArgs)
    Public Event TargetScan(ByVal sender As Object, ByVal e As EventArgs)

    Friend Sub onTargetChosen()
        Me.isNotChoosenYet = False  'reset flag
        If (EngagedTimes = Byte.MaxValue) Then
            EngagedTimes = 0 'reset
        End If
        Me.EngagedTimes += 1  'chosen once
        RaiseEvent TargetChosen(Me, Nothing)
    End Sub

    Overridable ReadOnly Property IntegratedList As List(Of shiftDataPackBase)
        Get
            'the list integrated right-half and left-halft , or individual
            Dim __integratedList As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase)
            subGroups.ForEach(Sub(__subGroup As shiftDataCollection) __integratedList.AddRange(__subGroup.DataCollection)) ' integrated

            Select Case subGroups.Count
                Case laneConfiguration.INDIVIDUAL

                    If isReversed Then
                        __integratedList.Reverse()
                    Else
                        '-------------------
                        'not change sequence
                        '-------------------
                    End If

                Case laneConfiguration.HALF_LANE

                    __integratedList.Reverse(0,
                                      subGroups.First.DataCollection.Count) 'reversed the right half

                Case Else
                    Throw New Exception("Lane Configuration Not Supported")
            End Select

            Return __integratedList
        End Get
    End Property

    ''' <summary>
    ''' indicate how many conveyor this set include
    ''' e.g left half + right half = lane set
    ''' 0-index,  right half lane
    ''' </summary>
    ''' <remarks></remarks>
    Public subGroups As List(Of shiftDataCollection) = New List(Of shiftDataCollection)
    ''' <summary>
    ''' Feedback the corresponding position
    ''' </summary>
    ''' <param name="arg"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property Position(Optional arg As Object = Nothing) As cMotorPoint '= Nothing
        Get
            If arg Is Nothing Then
                Return __positionDictionary.First.Value
            Else
                Return __positionDictionary(arg)
            End If
        End Get
        Set(value As cMotorPoint)
            If (arg Is Nothing) Then
                'for single input
                __positionDictionary.Clear()
                __positionDictionary.Add(New Object, value) 'empty key
            Else
                'for multi input
                __positionDictionary.Add(arg, value)
            End If
        End Set
    End Property
    Dim __positionDictionary As Dictionary(Of Object, cMotorPoint) = New Dictionary(Of Object, cMotorPoint)
    ''' <summary>
    ''' (Configuration)Indicate whether data index is reverse arrangement to gripper configuration
    ''' Conveyor 0-Index : indicate most upstream
    ''' Gripper 0-Index : indicate right most gripper
    ''' 'especially for subGroups-Count=1
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend isReversed As Boolean = False
    ''' <summary>
    ''' (Configuration) True , check empty , False , check full
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend isCheckEmptyOrFull As Boolean = False

    Public Overridable ReadOnly Property Score As Integer
        Get
            'high value: high priority ,
            'default : high weight , empty count , low weight , place counts , those one place more times had lower priority
            Dim estamation1 As Integer = Math.Abs(CInt(subGroups.Exists(Function(group As shiftDataCollection) IsSubGroupAbleToEngage(group)))) *
                UShort.MaxValue
            Return estamation1 + AvailiableCounts * Byte.MaxValue - EngagedTimes
        End Get
    End Property
    Protected Friend Overridable ReadOnly Property IsSubGroupAbleToEngage(subGroup As shiftDataCollection) As Boolean
        Get
            'if it need to update new link , handle this event
            RaiseEvent TargetScan(Me, EventArgs.Empty)
            'condition : 
            '1. must synchronized with conveyor
            '2. (all emptyed/fulled)
            '3. or finished with partial fulled
            'the working flag is on 0-index always (protocal)
            Dim partialFull As Boolean = __finishFlag.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED) And
                subGroup.DataCollection.Exists(Function(data As shiftDataPackBase) data.IsPositionOccupied = True) And (Not Me.isCheckEmptyOrFull)

            Return (partialFull Or subGroup.DataCollection.TrueForAll(Function(__data As shiftDataPackBase) (__data.IsPositionOccupied = (Not isCheckEmptyOrFull)))) And
            subGroup.DataCollection.First.ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED)
            'todo , optimization : when lane is on alarm , this condition should be true if landing zone cleared 
        End Get
    End Property

    Protected Friend isNotChoosenYet As Boolean = True

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property EmptyCounts As Integer
        Get
            Return TotalCounts - OccupiedCounts
        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property OccupiedCounts As Integer
        Get
            Return subGroups.Sum(Function(__lane As shiftDataCollection) (__lane.DataCollection.Sum(Function(data As shiftDataPackBase) Math.Abs(CInt(data.IsPositionOccupied)))))
        End Get
    End Property


    ReadOnly Property TotalCounts As Integer
        Get
            Return subGroups.Sum(Function(__lane As shiftDataCollection) __lane.DataCollection.Count)
        End Get
    End Property
    ''' <summary>
    ''' Used to count how many time this set be engaged
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property EngagedTimes As Byte = 0

    ReadOnly Property IsFinished As Boolean
        Get
            Return __finishFlag.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)
        End Get
    End Property

    ReadOnly Property AvailiableCounts As Integer
        Get
            If isCheckEmptyOrFull Then
                'unloader , check empty counts
                Return EmptyCounts
            Else
                'loader , check occupied counts
                Return OccupiedCounts
            End If
        End Get
    End Property

    Dim cachedList As List(Of shiftDataPackBase) = Nothing 'used as temp handle
    Dim __finishFlag As flagController(Of IFinishableStation.controlFlags) = New flagController(Of IFinishableStation.controlFlags) 'never come true 
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="isReversed"></param>
    ''' <param name="isCheckEmpty"></param>
    ''' <remarks></remarks>
    Public Sub New(isReversed As Boolean,
                   isCheckEmpty As Boolean,
                   Optional finishFlag As flagController(Of IFinishableStation.controlFlags) = Nothing)
        Me.isReversed = isReversed
        Me.isCheckEmptyOrFull = isCheckEmpty

        If finishFlag IsNot Nothing Then
            Me.__finishFlag = finishFlag
        End If

    End Sub
End Class


''' <summary>
''' The event arguments
''' </summary>
''' <remarks></remarks>
Public Class waferInfoEventArgs
    Inherits EventArgs

    Public waferInfo As shiftDataPackBase = Nothing

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public MustInherit Class gripperBase
    Public MustOverride ReadOnly Property GrippedData As shiftDataPackBase '= Nothing  'need to initializing
    Friend targetLink As shiftDataPackBase = Nothing 'build when target lane chosen , e.g chosen.group(i).dataCollection(j)

    Public Event DropOnLane(ByVal sender As Object, ByVal e As waferInfoEventArgs)
    Public Event DrainFromLane(ByVal sender As Object, ByVal e As waferInfoEventArgs)


    Friend Sub drop()
        If GrippedData.IsPositionOccupied Then
            'this gripper holds the data
            targetLink.Assign(GrippedData)

            GrippedData.IsPositionOccupied = False ' reset data
            RaiseEvent DropOnLane(Me, New waferInfoEventArgs With {.waferInfo = GrippedData})
        End If
        'for derived class , implement physical drop method
        dropMethod()

        targetLink = Nothing 'cut the link after dropped
    End Sub

    Friend Overridable Sub drain()
        If targetLink.IsPositionOccupied Then
            'the target holds the data
            GrippedData.Assign(targetLink)
            GrippedData.ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED) ' dont transfer MA

            targetLink.IsPositionOccupied = False ' 
            RaiseEvent DrainFromLane(Me, New waferInfoEventArgs With {.waferInfo = targetLink})
            'for derived class , implement physical drop method
            drainMethod()
        End If

        targetLink = Nothing
    End Sub

    MustOverride Sub dropMethod()
    MustOverride Sub drainMethod()
End Class


Public Class gripperStandard
    Inherits gripperBase

    Public vacuumGenerator As ULong = 0
    Public vacuumBreaker As ULong = 0
    Public vacuumSensor As ULong = 0

    Friend Overrides Sub drain()
        MyBase.drain()
        If readBit(vacuumSensor) Then
            'once sensor detected something , triggering
            GrippedData.IsPositionOccupied = True   'dummy data
            drainMethod()
        End If
    End Sub

    Public Overrides Sub drainMethod()
        writeBit(vacuumBreaker, False)
        writeBit(vacuumGenerator, True)
    End Sub

    Public Overrides Sub dropMethod()
        writeBit(vacuumBreaker, True)
        writeBit(vacuumGenerator, False)
    End Sub

    Public Overrides ReadOnly Property GrippedData As shiftDataPackBase
        Get
            Return __gripperData
        End Get
    End Property
    Protected __gripperData As shiftDataPackBase = Nothing

    Sub New(dataType As Type)
        __gripperData = Activator.CreateInstance(dataType)
    End Sub

End Class