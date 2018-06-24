Imports Automation
Imports Automation.Components.CommandStateMachine

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class clsConveyorLaneMask
    Inherits clsSynchronizableTransporterPullTypeV2 '繼承可同動串聯的輸送帶，並加上可設定lane Mask的功能
#Region "Data declare"
    Property LaneMask As List(Of Boolean) = New List(Of Boolean)     '可設定哪個位置有片，即可用在類似RENA上料系統
    Property LaneIndex As Integer '指出輸送帶上做到哪個Lane
    Property myLanePointTable As New List(Of List(Of cMotorPoint)) '自己輸送帶的pointTable集合

#End Region
#Region "Exteranl Data delcare"

    'Public isResendAction As Func(Of Boolean) = Function() (False) '填入判斷式，是否要重送action旗標
    'Public moveSelfCondition As Func(Of Boolean) = Function() (False)  '是否自我移動的條件
    'Public startShiftMove As Func(Of Boolean) = Function() (True) '填入開始移動方式，可能是同動或自己動
    'Public checkMotionDone As Func(Of Boolean) = Function() (True)
#End Region
    Protected Overrides Function isAbleTransfer() As Boolean
        If moveSelfCondition() = True Then
            Return True
        Else
            Return MyBase.isAbleTransfer()
        End If

    End Function
    Private Function moveSelfCondition() As Boolean
        If LaneIndex < LaneMask.Count AndAlso
            LaneMask(LaneIndex) = False Then
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' 實做 單軸 或 多軸同動的功能，若是被動軸則等待同動旗標完成
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function transferProcess() As Boolean
        MyPointTable = myLanePointTable(LaneIndex)
        If MyBase.transferProcess() = True Then '移動完，LaneIndex+1
            LaneIndex += 1 : LaneIndex = LaneIndex Mod LaneMask.Count
            Return True
        End If
        Return False
    End Function

    Public Overrides Function initMappingAndSetup() As Object
        '----------------------------
        '   Hsien , Lane Mask Count follow capability
        '----------------------------
        For index = 0 To Capability - 1
            LaneMask.Add(True)
        Next

        Return MyBase.initMappingAndSetup()
    End Function

    'seems redundant , Hsien , 2015.04.27
    'Public Overrides Function setMove(ByRef pointTable As List(Of cMotorPoint)) As Integer '設定移動前所要設定的程序，如point table, velocity profile
    '    If Flags.viewFlag(FlagsEnum.SYNC_WITH_UP_STREAM_NODE) = True Then
    '        CType(UpstreamNode, clsSynchronizableTransporterPullTypeV2).setMove(pointTable) '如果需要與上段輸送帶同動，則呼叫upstreamNode的setMove
    '    End If
    '    pointTable.AddRange(myLanePointTable(LaneIndex))
    '    Return 0
    'End Function
End Class

#Region "必須放在外面的"
#If 1 = 0 Then
'上料區 舌頭段輸送帶可以用此class
'上料區 傳送段輸送帶可以用此class
Public Class TestBench
    Dim unloadingConveyorTongue As synchronizableTransporterPullTypeJk = New synchronizableTransporterPullTypeJk()
    Dim unloadingBuffer As clsUnloadingBuffer = New clsUnloadingBuffer
    Sub Test()
        With unloadingConveyorTongue
            .highestPriority = Function() As Boolean '下料輸送帶 buffer滿片時，必須自己動
                                   If unloadingBuffer.controlFlags.viewFlag(clsUnloadingBuffer.controlFlagsEnum.IS_FULL) = True Then
                                       Return True
                                   End If
                                   Return False
                               End Function

            '.highestPriority = Function() (True)


        End With
    End Sub
End Class
#End If

#End Region