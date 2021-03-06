﻿Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AutoNumeric

<TestClass()> Public Class kinematicGraphBaseTest

    ''' <summary>
    ''' R->F1->F2->F3->F4
    ''' </summary>
    ''' <remarks></remarks>
    Enum frames
        R
        F1
        F2
        F3
        F4
    End Enum

    ''' <summary>
    ''' R->F1->F2->F3->F4
    ''' </summary>
    ''' <remarks></remarks>
    Enum frames2
        R
        C1
        C2
        C3
        S0
        S
        Y0
        C4
        L
    End Enum

    Public Enum framesDefinition As Byte
        R = 0
        C1
        C2
        C3
        C1REAL
        C2REAL
        C3REAL
        S0
        S
        Y0
        Y0REAL
        C4
        L
        LREAL

        DIE
        LPC
    End Enum


    ''' <summary>
    ''' For single chain
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub traverseTest1()
        Dim __kinematic As kinematicGraphBase = New kinematicGraphBase

        With __kinematic.HtmEdgeList
            .Add(New htmEdgeElementary(frames.F4, frames.F3))
            .Add(New htmEdgeElementary(frames.F3, frames.F2))
            .Add(New htmEdgeElementary(frames.F2, frames.F1))
            .Add(New htmEdgeElementary(frames.F1, frames.R))
        End With

        Dim T_f4_r = __kinematic.htmTraverse(frames.F4, frames.R)
        Dim T_f4_f1 = __kinematic.htmTraverse(frames.F4, frames.F2)

    End Sub

    ''' <summary>
    ''' For tree-like chain
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub traverseTest2()
        Dim __kinematic As kinematicGraphBase = New kinematicGraphBase

        With __kinematic.HtmEdgeList
            .Add(New htmEdgeElementary(frames.F4, frames.F3))
            .Add(New htmEdgeElementary(frames.F3, frames.F1))
            .Add(New htmEdgeElementary(frames.F2, frames.F1))
            .Add(New htmEdgeElementary(frames.F1, frames.R))
        End With

        Dim T_f4_r = __kinematic.htmTraverse(frames.F4, frames.R)
        Dim T_f4_f1 = __kinematic.htmTraverse(frames.F4, frames.F2)

    End Sub

    ''' <summary>
    ''' For tree-like chain
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub traverseTest4()
        Dim __kinematic As kinematicGraphBase = New kinematicGraphBase

        With __kinematic.HtmEdgeList
            .Add(New htmEdgeElementary(frames2.C1, frames2.R))
            .Add(New htmEdgeElementary(frames2.C2, frames2.R))
            .Add(New htmEdgeElementary(frames2.C3, frames2.R))
            .Add(New htmEdgeElementary(frames2.S0, frames2.R))
            .Add(New htmEdgeElementary(frames2.S, frames2.S0))
            .Add(New htmEdgeElementary(frames2.Y0, frames2.S0))
            .Add(New htmEdgeElementary(frames2.C4, frames2.Y0))
            .Add(New htmEdgeElementary(frames2.L, frames2.C4))

        End With

        Dim T_s_c1 As List(Of htmEdgeBase) = __kinematic.htmTraverse(frames2.S, frames2.C1)

        Dim T_s_c1_htm = __kinematic.Transformation(frames2.S, frames2.C1)

        Dim __value = T_s_c1_htm.Value

        Dim __v__s As PositionVector = New PositionVector(frames2.S)

        Dim __v__c = T_s_c1_htm * __v__s
    End Sub

    <TestMethod()> Public Sub ElementrayTest()
        Dim __kinematic As kinematicGraphBase = New kinematicGraphBase
        Dim tr = New htmEdgeElementary(frames.F4, frames.F3)

        With __kinematic.HtmEdgeList
            .Add(tr)
        End With
        Dim fnd = __kinematic.Elementray(frames.F4, frames.F3)
        Assert.IsTrue(fnd.Equals(tr))
    End Sub

    ''' <summary>
    ''' For tree-like chain
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> Public Sub traverseTest3()
        Dim __kinematic As kinematicGraphBase = New kinematicGraphBase

        With __kinematic.HtmEdgeList
            .Add(New htmEdgeElementary(framesDefinition.C1, framesDefinition.R))
            .Add(New htmEdgeElementary(framesDefinition.C2, framesDefinition.R))
            .Add(New htmEdgeElementary(framesDefinition.C3, framesDefinition.R))

            '.Add(New errorHtm(framesDefinition.C1REAL, framesDefinition.C1))
            '.Add(New errorHtm(framesDefinition.C2REAL, framesDefinition.C2))
            '.Add(New errorHtm(framesDefinition.C3REAL, framesDefinition.C3))

            .Add(New htmEdgeElementary(framesDefinition.S0, framesDefinition.R))
            .Add(New htmEdgeElementary(framesDefinition.S, framesDefinition.S0))

            .Add(New htmEdgeElementary(framesDefinition.Y0, framesDefinition.S0))
            '.Add(New errorHtm(framesDefinition.Y0REAL, framesDefinition.Y0))

            '.Add(New errorHtm(framesDefinition.C4, framesDefinition.Y0REAL))

            .Add(New htmEdgeElementary(framesDefinition.L, framesDefinition.C4))
            '.Add(New errorHtm(framesDefinition.LREAL, framesDefinition.L))

            .Add(New htmEdgeElementary(framesDefinition.DIE, framesDefinition.R))
            .Add(New htmEdgeElementary(framesDefinition.LPC, framesDefinition.S))
        End With

        Dim result = __kinematic.htmTraverse(framesDefinition.S, framesDefinition.C3REAL)
        Assert.IsTrue(result.Count = 4)

        result = __kinematic.htmTraverse(framesDefinition.LREAL, framesDefinition.C3REAL)
        Assert.IsTrue(result.Count = 8)

        result = __kinematic.htmTraverse(framesDefinition.LPC, framesDefinition.DIE)
        Assert.IsTrue(result.Count = 4)

        Dim tr = __kinematic.Transformation(framesDefinition.S, framesDefinition.C3REAL)

        Assert.IsTrue(tr.RawValue IsNot Nothing)

    End Sub

End Class