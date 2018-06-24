Imports Automation

Public Class conveyorTongue
    Inherits clsSynchronizableTransporterPullTypeV2

    Protected Overrides Function dataVerifyAction() As Boolean
        'additional check through loss , Hsien , 2015.09.04 
        Return MyBase.dataVerifyAction() AndAlso
            (systemSubState <> shiftingStates.DATA_PRE_VERIFY) OrElse
             (systemSubState = shiftingStates.DATA_PRE_VERIFY And checkWaferThroughLoss())
    End Function

    Sub alarmOccured(sender As alarmManager, e As alarmEventArgs) Handles CentralAlarmObject.alarmOccured
        If (Me.IsMyAlarmCurrent) Then

            Dim __conveyorAlarm As alarmContentConveyor = TryCast(e.Content, alarmContentConveyor)
            If (__conveyorAlarm IsNot Nothing AndAlso
                __conveyorAlarm.Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS AndAlso
                __conveyorAlarm.Position = 1) Then
                e.Content.AdditionalInfo = "卡匣抽片失敗"
            End If
        End If
    End Sub


End Class
