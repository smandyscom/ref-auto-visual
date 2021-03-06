﻿Imports MathNet.Numerics.LinearAlgebra
Imports System.Xml.Serialization
Imports System.IO
Imports AutoNumeric
Imports FA.framesDefinition
Imports System.Runtime.CompilerServices
Imports System.ComponentModel
<Assembly: InternalsVisibleTo("AssemblyTester")> 



''' <summary>
''' List all Frame Tag
''' </summary>
''' <remarks></remarks>
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

    LPC
    LPC_REAL
    DIE
    ''' <summary>
    ''' Found by image-processing
    ''' </summary>
    ''' <remarks></remarks>
    DIE_REAL_DRY
    ''' <summary>
    ''' Found by energy-searching
    ''' </summary>
    ''' <remarks></remarks>
    DIE_REAL_DRY_REVISED
    ''' <summary>
    ''' Referenced to DIE_REAL_DRY_REVISED , nominal Y-offset
    ''' </summary>
    ''' <remarks></remarks>
    DIE_REAL_WET
    DIE_REAL_WET_REVISED

    BALL


    DISP_HEAD
    DISP_HEAD_REAL

End Enum


''' <summary>
''' Depict machine frame structure
''' </summary>
''' <remarks></remarks>
Public Class frames
    Inherits kinematicGraphBase
    Implements IDisposable

    ReadOnly Property MovingItemCurrentPosition As PositionVector
        Get
            Return Transformation(CurrentMovingItem, framesDefinition.R) * New PositionVector(CurrentMovingItem)
        End Get
    End Property
    ReadOnly Property MovingItemCurrentPosition(item As framesDefinition) As PositionVector
        Get
            Return Transformation(item, framesDefinition.R) * New PositionVector(CurrentMovingItem)
        End Get
    End Property

    Public Property CurrentRItem As itemsDefinition
        Get
            Return __currentRItem
        End Get
        Set(ByVal value As itemsDefinition)
            __currentRItem = value
            Select Case __currentMovingItem
                Case BALL
                    Dim __transformation As htmEdgeElementary = Transformation(BALL, objectsDictionary(__currentRItem).ReferencedFrame).Value
                    __transformation.PositionVector = objectsDictionary(__currentRItem).RawValue
                    solveS(__transformation, manipulationOptionsEnum.POSITION_ONLY)
                Case LPC_REAL
                    Dim __transformation As htmEdgeElementary = Nothing

                    Select Case __currentRItem
                        Case itemsDefinition.LPC_F1,
                            itemsDefinition.LPC_F2,
                            itemsDefinition.LPC_F3,
                            itemsDefinition.LPC_F4,
                            itemsDefinition.LPC_F5
                            With CType(lpcMark.Instance.MeasureSetting, lpcMeasureSetting)
                                __transformation = .FeatureMeasureSettings(__currentRItem).FeaturePositionTransformation
                            End With
                        Case Else
                            __transformation = New htmEdgeElementary(LPC_REAL, objectsDictionary(__currentRItem).ReferencedFrame)
                            __transformation.PositionVector = objectsDictionary(__currentRItem).RawValue
                    End Select
                    solveS(__transformation) ' pose and orientation
                Case Else
                    solveAbsAxAy(__currentRItem, __currentMovingItem)
            End Select

        End Set
    End Property
    ReadOnly Property CurrentRObject As PositionVector
        Get
            Return objectsDictionary(__currentRItem)
        End Get
    End Property
    ReadOnly Property HtmsNeedReload As List(Of htmEdgeBase)
        Get
            Return __htmsNeedReload
        End Get
    End Property
    ReadOnly Property ErrorMatrixs As List(Of eulerHtmTR)
        Get
            Return New List(Of eulerHtmTR) From {Elementray(C1REAL, C1),
                                                 Elementray(C2REAL, C2),
                                                 Elementray(C3REAL, C3),
                                                 Elementray(Y0REAL, Y0),
                                                 Elementray(LREAL, L),
                                                 Elementray(LPC_REAL, LPC),
                                                 Elementray(DIE_REAL_DRY, DIE),
                                                 Elementray(DIE_REAL_DRY_REVISED, DIE_REAL_DRY),
                                                 Elementray(DIE_REAL_WET_REVISED, DIE_REAL_WET),
                                                 Elementray(DISP_HEAD_REAL, DISP_HEAD)}
        End Get
    End Property

    Public Property CurrentMovingItem As framesDefinition
        Get
            Return __currentMovingItem
        End Get
        Set(ByVal value As framesDefinition)
            Select Case value
                Case LREAL,
                    C4,
                    S0,
                    DISP_HEAD_REAL,
                    BALL,
                    LPC_REAL

                    __currentMovingItem = value
                Case Else
                    'reject
            End Select

        End Set
    End Property

    Dim __currentMovingItem As framesDefinition = S0
    Dim __currentRItem As itemsDefinition = itemsDefinition.CHOKE_CENTER


    ''' <summary>
    ''' Store the positions of objects on reference frame
    ''' </summary>
    ''' <remarks></remarks>
    Friend objectsDictionary As Dictionary(Of itemsDefinition, PositionVector) = New Dictionary(Of itemsDefinition, PositionVector)

    Dim __htmsNeedReload As List(Of htmEdgeBase) = New List(Of htmEdgeBase)


    ''' <summary>
    ''' Match the moving item origin to target item
    ''' Given item , solve the nominal axis value
    ''' </summary>
    ''' <param name="rItem"></param>
    ''' <param name="movingItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function solveAbsAxAy(rItem As itemsDefinition, movingItem As framesDefinition) As Object
        '----------------------------------
        '  Coordinate Transformation to R
        '----------------------------------
        Return solveAbsAxAy(objectsDictionary(rItem), movingItem)
    End Function

    ''' <summary>
    ''' Move moving item to targetPosition represented in R-frame
    ''' </summary>
    ''' <param name="targetPosition">
    ''' Position In R-Frame
    ''' </param>
    ''' <param name="movingItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function solveAbsAxAy(targetPosition As PositionVector, movingItem As framesDefinition) As Object
        Select Case movingItem
            Case LREAL,
                C4,
                S0,
                DISP_HEAD_REAL

                'do transformation
                Dim targetPositionInR As PositionVector = Transformation(targetPosition.ReferencedFrame,
                                                                         framesDefinition.R) * targetPosition
                'initial value
                Dim xAxisValue As Double = s0Htm.Instance.AxisValue(axisEntityEnum.X)
                Dim yAxisValue As Double = c4htm.Instance.AxisValue(axisEntityEnum.Y)
                Dim zAxisValue As Double = c4htm.Instance.AxisValue(axisEntityEnum.Z)

                Dim currentTransformation As htmEdgeBase = Nothing

                Dim relativeTranslation As Vector(Of Double) = Nothing

                Do
                    currentTransformation = ForwardKinematic(xAxisValue, yAxisValue, zAxisValue, movingItem)

                    'moving item to R
                    relativeTranslation =
                      targetPositionInR.RawValue.SubVector(0, 3) - currentTransformation.Origin.RawValue.SubVector(0, 3)

                    xAxisValue += relativeTranslation(axisEntityEnum.X)
                    yAxisValue += relativeTranslation(axisEntityEnum.Y)
                    zAxisValue += relativeTranslation(axisEntityEnum.Z)
                Loop Until relativeTranslation.L2Norm < 0.0001


                'update Ax,Ay (moving?
                s0Htm.Instance.AxisValue(axisEntityEnum.X) = xAxisValue
                If Not movingItem.Equals(S0) Then
                    c4htm.Instance.AxisValue(axisEntityEnum.Y) = yAxisValue
                End If
                'Z-axis moving
                Select Case movingItem
                    Case DISP_HEAD_REAL
                        c4htm.Instance.AxisValue(axisEntityEnum.Z) = zAxisValue
                        '----------------------
                        '   C4/LREAL Working Distance Difference
                        '----------------------
                    Case LREAL
                        'moving the L-carriage rather L_REAL , relative moving
                        c4htm.Instance.AxisValue(axisEntityEnum.Z) = Transformation(L, C4).Origin.Z
                    Case C4
                        c4htm.Instance.AxisValue(axisEntityEnum.Z) = 0 'back to zero
                End Select

                Return {s0Htm.Instance.AxisValue(axisEntityEnum.X),
                        c4htm.Instance.AxisValue(axisEntityEnum.Y)}

            Case Else
                'not available Y item
                Throw New InvalidDataException
        End Select
    End Function

    ''' <summary>
    ''' Solve Forward kinematics without moving, T_c4_r(ax,ay,az)
    ''' </summary>
    ''' <param name="ax"></param>
    ''' <param name="ay"></param>
    ''' <param name="az"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ForwardKinematic(ax As Double, ay As Double, az As Double, Optional movingItem As framesDefinition = C4) As htmEdgeBase
        Get
            Dim transformationS0toR As s0Htm = s0Htm.Instance.Clone
            transformationS0toR.AxisValue(axisEntityEnum.X) = ax
            Dim transformationY0RealtoS0 = Transformation(framesDefinition.Y0REAL,
                                                          framesDefinition.S0)
            Dim transformationC4toY0Real As c4htm = c4htm.Instance.Clone
            transformationC4toY0Real.AxisValue(axisEntityEnum.Y) = ay
            transformationC4toY0Real.AxisValue(axisEntityEnum.Z) = az

            Return transformationS0toR *
                transformationY0RealtoS0 *
                transformationC4toY0Real *
                Transformation(movingItem, framesDefinition.C4)
        End Get
    End Property

    Public Enum manipulationOptionsEnum
        POSITION_ONLY = &H1
        ROTATION_ONLY = &H2
        BOTH = &H3
    End Enum


    ''' <summary>
    ''' Manipulate some frame on specific reference
    ''' Match/Align the origin/orientation
    '''  If Both/Rotation given , would try to align the 3-coordinate in the same direction
    ''' </summary>
    ''' <remarks></remarks>
    Function solveS(transformationToolToWorkpiece As htmEdgeBase,
                    Optional manipulationOptions As manipulationOptionsEnum = manipulationOptionsEnum.BOTH) As Object

        'chain,  T_s_s0 = T_r_s0 * T_w_r * T_s_w
        'target transformation (T_w_r
        Dim transformationStoTool = Transformation(framesDefinition.S, transformationToolToWorkpiece.From)
        Dim transformationWorkpiecetoS0 = Transformation(transformationToolToWorkpiece.To__, framesDefinition.S0)
        Dim transformationStoS0 = transformationWorkpiecetoS0 * transformationToolToWorkpiece * transformationStoTool


        'update S control vector
        With sHtm.Instance
            Select Case manipulationOptions
                Case manipulationOptionsEnum.POSITION_ONLY
                    .PositionVector = transformationStoS0.PositionVector
                Case manipulationOptionsEnum.ROTATION_ONLY
                    .RotationMatrix = transformationStoS0.RotationMatrix
                Case manipulationOptionsEnum.BOTH
                    'update and trigger
                    .RawValue = transformationStoS0.RawValue
            End Select

            Return .ControlVector
        End With

    End Function


    Protected Sub New()

        're-construct chain from persistance
        With Me.__htmEdgeList
            .Add(New htmEdgeElementary(C1, R))
            .Add(New htmEdgeElementary(C2, R))
            .Add(New htmEdgeElementary(C3, R))

            .Add(New eulerHtmTR(C1REAL, C1))
            .Add(New eulerHtmTR(C2REAL, C2))
            .Add(New eulerHtmTR(C3REAL, C3))

            .Add(s0Htm.Instance)
            .Add(sHtm.Instance)

            .Add(New htmEdgeElementary(Y0, S0))
            .Add(New eulerHtmTR(Y0REAL, Y0))

            .Add(c4htm.Instance)

            .Add(New htmEdgeElementary(L, C4))
            .Add(New eulerHtmTR(LREAL, L))

            .Add(New htmEdgeElementary(LPC, S))
            .Add(New eulerHtmTR(LPC_REAL, LPC))

            .Add(New htmEdgeElementary(DISP_HEAD, C4))
            .Add(New eulerHtmTR(DISP_HEAD_REAL, DISP_HEAD))

            .Add(dieHtm.Instance)
            .Add(New eulerHtmTR(DIE_REAL_DRY, DIE))
            .Add(New eulerHtmTR(DIE_REAL_DRY_REVISED, DIE_REAL_DRY))
            .Add(New htmEdgeElementary(DIE_REAL_WET, DIE_REAL_DRY_REVISED))
            .Add(New eulerHtmTR(DIE_REAL_WET_REVISED, DIE_REAL_WET))

            .Add(New htmEdgeElementary(BALL, S))

            With __htmsNeedReload
                .AddRange(Me.__htmEdgeList.FindAll(Function(__htm As htmEdgeElementary) __htm.GetType.Equals(GetType(htmEdgeElementary))))
                .AddRange({s0Htm.Instance,
                           c4htm.Instance,
                           dieHtm.Instance})
                .AddRange({Me.Elementray(C1REAL, C1),
                           Me.Elementray(C2REAL, C2),
                           Me.Elementray(C3REAL, C3),
                           Me.Elementray(Y0REAL, Y0),
                           Me.Elementray(LREAL, L),
                           Me.Elementray(DISP_HEAD_REAL, DISP_HEAD)})

                .ForEach(Sub(__htm As htmEdgeElementary) __htm.Load(Nothing))
            End With
        End With
        'reconstruct object list
        Dim valueArray = [Enum].GetValues(GetType(itemsDefinition))
        For index = 0 To valueArray.Length - 1
            Dim item As itemsDefinition = [Enum].ToObject(GetType(itemsDefinition), valueArray(index))
            Dim attribute = CType(item.GetType.GetMember(item.ToString).First.GetCustomAttributes(GetType(reference), False).First, reference)
            objectsDictionary(item) = New PositionVector(attribute.ReferencedFrame, item.ToString)
        Next
        'reload
        For Each item As PositionVector In objectsDictionary.Values
            item.Load(Nothing)
        Next

    End Sub

    Shared ReadOnly Property Instance As frames
        Get
            If __instance Is Nothing Then
                __instance = New frames
            End If
            Return __instance
        End Get
    End Property
    Shared __instance As frames = Nothing

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If
            'for those marked persistance , do save()
            __htmsNeedReload.ForEach(Sub(htm As htmEdgeElementary) htm.Save())
            For Each item As PositionVector In objectsDictionary.Values
                item.Save()
            Next
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

