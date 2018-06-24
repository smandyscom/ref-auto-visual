Imports Microsoft.VisualBasic

''' <summary>
''' Used to describe the position vector referenced frame
''' </summary>
''' <remarks></remarks>
Public Class reference
    Inherits Attribute

    ReadOnly Property ReferencedFrame As framesDefinition
        Get
            Return __referencedFrame
        End Get
    End Property

    Dim __referencedFrame As framesDefinition = framesDefinition.R
    Sub New(__referencedFrame As framesDefinition)
        Me.__referencedFrame = __referencedFrame
    End Sub

End Class

Public Enum itemsDefinition As Byte



    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P1 = 0
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P2 = 1
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P3 = 2
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P4 = 3
    ''' <summary>
    ''' Origin of DIE
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P5 = 4
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P6 = 5
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_P7 = 6
    <reference(framesDefinition.DIE_REAL_DRY)>
        DIE_CENTER
    <reference(framesDefinition.DIE_REAL_DRY_REVISED)>
        DIE_P5_NON_CONTACT
    ''' <summary>
    ''' The working point for dispention
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.DIE_REAL_DRY_REVISED)>
        DIE_DISP_READY
    ''' <summary>
    ''' The working point for dispention
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.DIE_REAL_DRY_REVISED)>
        DIE_DISP_WORK


    <reference(framesDefinition.R)>
        CHOKE_CENTER
    <reference(framesDefinition.R)>
        CHOKE_CORNER1
    <reference(framesDefinition.R)>
        CHOKE_CORNER2
    <reference(framesDefinition.R)>
        CHOKE_CORNER3

    <reference(framesDefinition.C1REAL)>
        C1_ORIGIN
    <reference(framesDefinition.C2REAL)>
        C2_ORIGIN
    <reference(framesDefinition.C3REAL)>
        C3_ORIGIN
    ''' <summary>
    ''' The ready point for C1-Disp calibration
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.C1REAL)>
        C1_DISP_READY
    ''' <summary>
    ''' The ready point for C2-Disp calibration
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.C2REAL)>
        C2_DISP_READY

    '---------------------
    '   LPC
    '---------------------
    <reference(framesDefinition.LPC_REAL)>
        LPC_R1
    <reference(framesDefinition.LPC_REAL)>
        LPC_R2
    <reference(framesDefinition.LPC_REAL)>
        LPC_R3
    <reference(framesDefinition.LPC_REAL)>
        LPC_R4
    <reference(framesDefinition.LPC_REAL)>
        LPC_H1
    <reference(framesDefinition.LPC_REAL)>
        LPC_H2
    <reference(framesDefinition.LPC_REAL)>
        LPC_F1
    <reference(framesDefinition.LPC_REAL)>
        LPC_F2
    <reference(framesDefinition.LPC_REAL)>
        LPC_F3

    <reference(framesDefinition.LPC_REAL)>
        LPC_F4
    <reference(framesDefinition.LPC_REAL)>
        LPC_F5

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.R)>
       ANTI_COLLISION_EDGE_1

    ''' <summary>
    '''Moving Items Origins
    ''' </summary>
    ''' <remarks></remarks>
    <reference(framesDefinition.S0)>
        S0_ORIGIN
    <reference(framesDefinition.C4)>
        C4_ORIGIN
    <reference(framesDefinition.LREAL)>
        LREAL_ORIGIN
    <reference(framesDefinition.DISP_HEAD_REAL)>
        DISP_HEAD_REAL_ORIGIN
    <reference(framesDefinition.BALL)>
        BALL_ORIGIN
    <reference(framesDefinition.LPC_REAL)>
        LPC_REAL_ORIGIN

End Enum

