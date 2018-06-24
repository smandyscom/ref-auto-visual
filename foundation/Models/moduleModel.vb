Imports System.Text
Imports System.ComponentModel

Public Interface IModuleSingle
    '---------------------------
    '   The common interface , for the shifter module linked to single position
    '   Used in ASA-03-262 , verifing
    '---------------------------
    Property TargetPositionInfo As Func(Of shiftDataPackBase)

End Interface

Public Interface IModuleMulti
    '---------------------------
    '   The common interface , for the shifter module linked to multi position
    '   Used in ASA-04-005 , PECVD loading/unloading arm part, verifying
    '---------------------------
    Property TargetPositionInfo As List(Of Func(Of shiftDataPackBase))     'as array of delegates , used to query target info collection
End Interface

Public Interface IFinishableStation
    '---------------------------
    '   The common interface , for those station which able to do sequential-finishing job action
    '---------------------------
    Enum controlFlags
        STATION_FINISHED       ' true , the station is on working , false , the station finished job already/not ignite yet
        COMMAND_IGNITE      ' true , the station should start ignite operation
    End Enum

    Property UpstreamStations As List(Of IFinishableStation)                  ' linked to previous station(s) , hsien , 2015.03.18
    Property FinishableFlags As flagController(Of controlFlags)     ' 

End Interface

Public Enum operationSignalsEnum
    __START 'start operation 
    __STOP  'stop operation from Bypass mode , on : aborting , off : abort finished , in idle
End Enum

Public Interface IOperational
    Property OperationSignals As flagController(Of operationSignalsEnum)
End Interface

''' <summary>
''' Used for UI monitoring
''' </summary>
''' <remarks></remarks>
Public Class finishableStationAdaptor

    <DisplayName("名稱")>
    ReadOnly Property Name As String
        Get
            Return __reference.ToString
        End Get
    End Property
    <DisplayName("上游已清料")>
    <[ReadOnly](True)>
    ReadOnly Property IsUpstreamsFinished As Boolean
        Get
            Return __reference.UpstreamStations.TrueForAll(Function(__station) __station.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED))
        End Get
    End Property
    <DisplayName("清料狀態")>
    Property FinishStatus As Boolean
        Get
            Return __reference.FinishableFlags.viewFlag(IFinishableStation.controlFlags.STATION_FINISHED)
        End Get
        Set(value As Boolean)
            __reference.FinishableFlags.writeFlag(IFinishableStation.controlFlags.STATION_FINISHED, value)
        End Set
    End Property
    <DisplayName("上游名稱")>
    ReadOnly Property Upstreams As String
        Get
            Return __upstreams
        End Get
    End Property

    Dim __reference As IFinishableStation = Nothing
    Dim __upstreams As String = ""

    Sub New(__reference As IFinishableStation)
        Me.__reference = __reference

        Dim sb As StringBuilder = New StringBuilder
        For Each item As IFinishableStation In __reference.UpstreamStations
            With sb
                .Append(String.Format("{0},", item.ToString))
            End With
        Next
        __upstreams = sb.ToString
    End Sub


    ''' <summary>
    ''' Finshable Stations Collector
    ''' </summary>
    ''' <param name="__list"></param>
    ''' <param name="__thisNode"></param>
    ''' <param name="__headNode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function collectFinishableStations(__list As List(Of IFinishableStation),
                                                     __thisNode As IFinishableStation,
                                                     __headNode As IFinishableStation) As Integer


        'if this node hadnt collected , put into list
        If (Not __list.Exists(Function(__node As IFinishableStation) (__thisNode.Equals(__node)))) Then
            __list.Add(__thisNode)
        End If

        If (__thisNode.UpstreamStations Is Nothing) Then
            Return 0
        End If

        For index = 0 To __thisNode.UpstreamStations.Count - 1
            'do breadth first searching

            Dim __upstreamNode As IFinishableStation = __thisNode.UpstreamStations(index)

            '----------------------------------------------------------------------
            '   Chain is linked
            '----------------------------------------------------------------------
            If (Not __upstreamNode.Equals(__headNode)) Then
                'if not the head node , keep depth searching
                collectFinishableStations(__list, __upstreamNode, __headNode)
            End If

        Next

        Return 0
    End Function

End Class