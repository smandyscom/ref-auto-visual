﻿Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo("AutoNumericTester")> 


''' <summary>
''' Offer basic facilities to build/traverse a kinematic graph
''' </summary>
''' <remarks></remarks>
Public Class kinematicGraphBase

    ''' <summary>
    ''' Given source frame(from) and destionation frame(to) , would return a 
    ''' consequence transformation
    ''' </summary>
    ''' <param name="__from"></param>
    ''' <param name="__to"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Transformation(__from As [Enum], __to As [Enum]) As htmEdgeBase
        Get
            If __from.Equals(__to) Then
                'Identity transformation
                Return New htmEdgeElementary(__from,
                                             __from)
            End If

            Return New htmEdgeComposed(htmTraverse(__from, __to),
                                       __from,
                                       __to)
        End Get
    End Property
    ''' <summary>
    ''' Used for query elementray 
    ''' </summary>
    ''' <param name="__from"></param>
    ''' <param name="__to"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Elementray(__from As [Enum], __to As [Enum]) As htmEdgeElementary
        Get
            Return __htmEdgeList.Find(Function(__htm As htmEdgeBase)
                                          Return TryCast(__htm, htmEdgeElementary) IsNot Nothing And
                                              __htm.From.Equals(__from) And
                                              __htm.To__.Equals(__to)
                                      End Function)
        End Get
    End Property


    ReadOnly Property HtmEdgeList As List(Of htmEdgeBase)
        Get
            Return __htmEdgeList
        End Get
    End Property

    ''' <summary>
    ''' Used to store original HTMs
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend __htmEdgeList As List(Of htmEdgeBase) = New List(Of htmEdgeBase)

    ''' <summary>
    ''' The Traverse method to find a htm from a frame to another
    '''  a series of htms , zero-index indicate 'destination'
    ''' Treated as non-direction graph
    ''' </summary>
    ''' <param name="start"></param>
    ''' <param name="destination"></param>
    ''' <returns> the path traversed </returns>
    ''' <remarks></remarks>
    Protected Friend Function htmTraverse(start As [Enum], destination As [Enum],
                         Optional remainedEdges As List(Of htmEdgeBase) = Nothing) As List(Of htmEdgeBase)
        '1. find all candinates
        '2. go further traverse each candinates
        '3. do nessary inversion of htm

        If remainedEdges Is Nothing Then
            '----------------------------
            '   Initiate Remained Edges
            '----------------------------
            remainedEdges = __htmEdgeList
        End If

        Dim startEdges As List(Of htmEdgeBase) =
            remainedEdges.FindAll(Function(__htm As htmEdgeBase) __htm.DoubleEnds.Contains(start))

        'if candinates linked with the tranverse destination , finish traversing
        Dim destinitionEdges As htmEdgeBase = startEdges.Find(Function(__htm As htmEdgeBase) __htm.DoubleEnds.Contains(destination))

        If destinitionEdges IsNot Nothing Then
            '------------------------
            '   Destiniton Edge Found
            '------------------------
            Return New List(Of htmEdgeBase) From {destinitionEdges}
        Else

            'organized remained edge
            Dim __remainedEdge As List(Of htmEdgeBase) = New List(Of htmEdgeBase)(remainedEdges)
            'remove parallel edges , then do further searching
            __remainedEdge.RemoveAll(Function(__htm As htmEdgeBase) (startEdges.Contains(__htm)))

            For Each __htm As htmEdgeBase In startEdges

                Dim nextStart As [Enum] = Nothing

                '--------------------------------
                '   Start From Another End
                '--------------------------------
                If __htm.From.Equals(start) Then
                    nextStart = __htm.To__
                Else
                    nextStart = __htm.From
                End If

                Dim furtherPath As List(Of htmEdgeBase) =
                    htmTraverse(nextStart, destination, __remainedEdge)

                If furtherPath IsNot Nothing Then
                    Dim __subPath As List(Of htmEdgeBase) = New List(Of htmEdgeBase)(furtherPath)
                    __subPath.Add(__htm)
                    Return __subPath
                Else
                    '-------------------------
                    ' no further path
                    'do neighbor traversing
                    '-------------------------
                End If
            Next
        End If

        'no candinates
        'no further path for all candinates
        Return Nothing

    End Function


End Class
