Imports System.Text.RegularExpressions
Imports System.IO

Public Class compSmarPod
    Inherits compHexaPodBase
    Const modelNum As UInteger = 10019
    Dim mSmarPod As New Smarpod
    Dim mPoint As Smarpod_Pose
    Dim taskMotion As Task(Of hexaPodExceptionPack)
    Property frmMain As Windows.Forms.Form
    Property ProjectFileRelativePath As String 'relative path in Data\SCARA_prj\*.sprj
    Public Overrides ReadOnly Property IsConnected As Boolean
        Get
            Return mSmarPod.IsConnected
        End Get
    End Property
    Public Overrides ReadOnly Property IsReferenced As Boolean
        Get
            Return mSmarPod.IsReferenced
        End Get
    End Property
    Public Overrides Function Connect(strIP As String, strPort As String) As UInteger
        Return mSmarPod.Connect(modelNum, strIP, strPort)
    End Function
    Public Overrides Function Disconnect() As Integer
        Try
            mSmarPod.Disconnect()
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try
        Return 0
    End Function


    Protected Overrides Function _Stop() As Integer
        mSmarPod.Stop_()
        Return 0
    End Function
    Protected Overrides Function CommandInCycle() As Boolean

        Return mSmarPod.CommandInCycle
    End Function
    Protected Overrides Function pauseCommand() As Integer
        mSmarPod.Pause()
        Return 0
    End Function

    Protected Overrides Function resumeCommand() As Integer
        mSmarPod.Continue()
        Return 0
    End Function


    Protected Overrides Function go() As hexaPodExceptionPack

        If CommandInCycle() = True Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_TaskIsNotCompleted, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_TaskIsNotCompleted)}
        End If
        If PositionPoint Is Nothing Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_No_Assigning_Point, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_No_Assigning_Point)}
        End If
        taskMotion = Task.Factory.StartNew(Function()
                                               Try
                                                   mSmarPod.Go(New SpelPoint With {
                                                    .X = PositionPoint.X, .Y = PositionPoint.Y,
                                                    .Z = PositionPoint.Z, .U = PositionPoint.U,
                                                    .Local = PositionPoint.Local,
                                                    .Hand = PositionPoint.Hand})
                                               Catch ex As SpelException
                                                   Return New scaraExceptionPack With {.ErrorNumber = ex.ErrorNumber,
                                                                                       .StackTrace = .StackTrace,
                                                                                       .Message = .Message}
                                               End Try
                                               Return Nothing
                                           End Function)
        Return Nothing
    End Function
    Protected Overrides Function gorel() As hexaPodExceptionPack

        If CommandInCycle() = True Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_TaskIsNotCompleted, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_TaskIsNotCompleted)}
        End If
        If PositionPoint Is Nothing Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_No_Assigning_Point, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_No_Assigning_Point)}
        End If
        'mSmarPod.Go("Here +X(10)+Y(10)+Z(-10)+U(10)")
        taskMotion = Task.Factory.StartNew(Function()
                                               Try

                                                   mSmarPod.Go(String.Format("Here +X({0})+Y({1})+Z({2})+U({3})",
                                                                           PositionPoint.X, PositionPoint.Y,
                                                                           PositionPoint.Z, PositionPoint.U))
                                               Catch ex As SpelException
                                                   Return New scaraExceptionPack With {.ErrorNumber = ex.ErrorNumber,
                                                        .StackTrace = .StackTrace,
                                                        .Message = .Message}
                                               End Try
                                               Return Nothing
                                           End Function)
        Return Nothing
    End Function
    Protected Overrides Function home() As hexaPodExceptionPack
        If CommandInCycle() = True Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_TaskIsNotCompleted, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_TaskIsNotCompleted)}
        End If
        taskMotion = Task.Factory.StartNew(Function()
                                               Try
                                                   mSmarPod.Home()
                                               Catch ex As SpelException
                                                   Return New scaraExceptionPack With {.ErrorNumber = ex.ErrorNumber,
                                                                                       .StackTrace = .StackTrace,
                                                                                       .Message = .Message}
                                               End Try
                                               Return Nothing
                                           End Function)
        Return Nothing
    End Function
    Protected Overrides Function jump() As hexaPodExceptionPack

        If CommandInCycle() = True Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_TaskIsNotCompleted, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_TaskIsNotCompleted)}
        End If
        If PointTable.Count = 0 Then
            Return New hexaPodExceptionPack With {.ErrorNumber = returnErrorCodes.ERR_No_Assigning_Point, .StackTrace = "", .Message = [Enum].GetName(GetType(returnErrorCodes), returnErrorCodes.ERR_No_Assigning_Point)}
        End If
        Dim destinationPoint As New SpelPoint With {
                                                        .X = PositionPoint.X, .Y = PositionPoint.Y,
                                                        .Z = PositionPoint.Z, .U = PositionPoint.U,
                                                        .Local = PositionPoint.Local,
                                                        .Hand = PositionPoint.Hand}
        taskMotion = Task.Factory.StartNew(Function()
                                               Try
                                                   If PointTable.Count = 1 Then '只有一個位置
                                                       mSmarPod.Jump(destinationPoint)
                                                   ElseIf PointTable.Count = 2 Then
                                                       Dim LimZ As String = "LimZ " & PointTable(1).Z.ToString
                                                       mSmarPod.Jump(destinationPoint, LimZ)
                                                   End If
                                               Catch ex As SpelException
                                                   Return New scaraExceptionPack With {.ErrorNumber = ex.ErrorNumber,
                                                                                       .StackTrace = .StackTrace,
                                                                                       .Message = .Message}
                                               End Try
                                               Return Nothing
                                           End Function)
        Return Nothing
    End Function
    Public Overrides Function GetPoint(pointIndex As Integer) As hexaPoint
        Dim p As New hexaPoint
        With mSmarPod.GetPoint(pointIndex)
            p.PosX = .X
            p.Y = .Y
            p.Z = .Z
            p.U = .U
            p.Hand = .Hand
            p.Local = .Local
        End With
        Return p

    End Function

    Protected Overrides Function checkMotionDone() As hexaPodExceptionPack
        Return mSmarPod.GetMoveStatus()
    End Function
    Public Overrides Sub showWindow(windows As windowsEnum, sender As Windows.Forms.Form)
        Try
            mSmarPod.ShowWindow(windows, sender)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try

    End Sub
    Public Overrides Sub runDialog(dialog As dialogEnum, sender As Windows.Forms.Form)
        Try
            mSmarPod.RunDialog(dialog, sender)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try
    End Sub


    Public Overrides Sub programMode()
        Try
            mSmarPod.OperationMode = SpelOperationMode.Program
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try

    End Sub
    Public Overrides Sub reset()
        Try
            mSmarPod.Reset()
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try

    End Sub
    Public Overrides Sub teachPoint()
        Try
            mSmarPod.TeachPoint("robot1.pts", 1, "Teach Pick Position")
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace)
        End Try

    End Sub
    Public Sub New()
        MyBase.New()
    End Sub
    Protected Overrides Function GetPosition() As Single()
        Return mSmarPod.GetRobotPos(SpelRobotPosType.World, 0, 0, 0)
    End Function

End Class
