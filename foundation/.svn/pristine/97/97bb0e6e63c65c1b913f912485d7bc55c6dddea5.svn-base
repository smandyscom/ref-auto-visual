Public MustInherit Class stringCommunicationBase
    MustOverride Function Send() As String                              'used the command initiator for sending command 
    MustOverride Function Receive(inlet As String) As decodeResults     'used by the command receiver for parsing command
    MustOverride Function AcknowledgeSend() As String                   'used by command receiver for ackowledge sending
    MustOverride Function AcknowledgeReceive(inlet As String) As decodeResults          'used by command initiator for parsing ackowledge message
End Class
Public Enum decodeResults As Integer
    SUCCESS = 0
    ERR_PATTERN_NOT_MATCHED
    ERR_DATA_VERIFIED_FAILED
    ERR_BYTES_CONVERTING_FAILED
    ERR_NOT_IMPLEMENTED
End Enum
