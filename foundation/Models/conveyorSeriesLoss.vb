﻿Imports Automation
Imports Automation.Components.Services
Imports Automation.mainIOHardware
''' <summary>
''' The special edition to handle wafer lossed check
''' </summary>
''' <remarks></remarks>
Public Class conveyorSeriesLoss
    Inherits clsSynchronizableTransporterPullTypeV2

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property CheckState As Integer
        Get
            Return __checkState
        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    WriteOnly Property SerialLossThreshold As Integer
        Set(value As Integer)
            __serialLossThreshold = value
        End Set
    End Property

    Protected __checkState As Integer = 0
    Dim alarmPackLoss As alarmContentConveyor = New alarmContentConveyor With {.Sender = Me,
                                                                           .Reason = alarmContentSensor.alarmReasonSensor.SHOULD_BE_ON,
                                                                               .Detail = alarmContentConveyor.alarmReasonConveyor.WAFER_LOSS,
                                                                           .PossibleResponse = alarmContextBase.responseWays.IGNORE Or alarmContextBase.responseWays.RETRY}
    Dim lossedPosition As Integer = 0
    Dim lossedSensor As sensorControl = Nothing

    Protected Overrides Function dataVerifyAction() As Boolean

        If (systemSubState = shiftingStates.DATA_POST_VERIFY) Then
            Return checkWaferCoverLossAndUnknown() 'check loss and unknown
        End If

        If (Not checkWaferThroughJammed()) Then
            Return False    'first priority check 
        End If

        Select Case __checkState
            Case 0
                If (checkLoss()) Then
                    'passed
                    serialLossCounter = 0 'reset
                    __checkState = 1000
                Else
                    'failed to check
                    serialLossCounter += 1

                    If (serialLossCounter > __serialLossThreshold) Then
                        'alarm
                        With alarmPackLoss
                            .Inputs = lossedSensor.InputBit
                            .Position = lossedPosition
                        End With
                        CentralAlarmObject.raisingAlarm(alarmPackLoss)
                    Else
                        'skip , set isOccupied off
                        __occupiedStatus.DataCollection(lossedPosition).IsPositionOccupied = False
                        __checkState = 1000
                    End If

                End If

            Case 1000
                writeBit(outputBitMoving, False) 'hsien , not equal to default value , 2015.07.09
                __checkState = 0 ' rewind
                Return True 'loss check passed
        End Select

        Return False

    End Function


    Dim serialLossCounter As Integer = 0
    Friend __serialLossThreshold As Integer = 2

    Protected Function checkLoss() As Boolean
        For Each pair As KeyValuePair(Of Integer, sensorControl) In checkListWaferCoverSensors

            Dim sensor As sensorControl = pair.Value     'Hsien , 2014.10.29 , used to re-check

            '------------------------------
            '   Loss (Supposed to be coverd)
            '------------------------------
            If (__occupiedStatus.DataCollection(pair.Key).IsPositionOccupied And
                (Not pair.Value.IsSensorCovered)) Then

                'cached the loss information
                lossedPosition = pair.Key
                lossedSensor = pair.Value

                Return False ' failed
            Else
                '-----------------
                '
                '----------------
            End If
        Next

        Return True 'passed
    End Function

    Public Overrides Function initMappingAndSetup() As Object

        With alarmPackLoss
            .CallbackResponse(alarmContextBase.responseWays.RETRY) = Function() (True)
            .CallbackResponse(alarmContextBase.responseWays.IGNORE) = Function()
                                                                          '-------------------------------------------------------------
                                                                          ' Treat as disappered , need to confirm physical sensor again
                                                                          ' -------------------------------------------------------------
                                                                          serialLossCounter = 0 'reset
                                                                          Return True
                                                                      End Function
        End With

        Return MyBase.initMappingAndSetup()
    End Function

End Class
