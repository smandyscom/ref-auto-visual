'---------------------------------------------------------------------------------------------------------------------
'   The Common Message Query used in the class which may need to query message , i.e messagePackage , alarmContextBase
' Hsien , 2014.10.10
'---------------------------------------------------------------------------------------------------------------------
Public Interface IMessageQuery
    '-----------------------------------------
    '   The Base Class for query data facility
    '-----------------------------------------
    Function query(ByVal keyChain As Object) As Object
End Interface
