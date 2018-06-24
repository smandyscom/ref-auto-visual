Imports Automation

Partial Public Class dataBaseMain : Implements IMessageQuery

    Protected mainDataBase As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)  'if value is dictionary , keep iterating with enum-value , if not , return type-string
    Protected secondaryDictionary As Dictionary(Of [Enum], String) = New Dictionary(Of [Enum], String)

    Protected mainDataBaseTemp As Dictionary(Of Object, Object) = New Dictionary(Of Object, Object)
    Protected secondaryDictionaryTemp As Dictionary(Of [Enum], String) = New Dictionary(Of [Enum], String)

    Protected Function searchInPrimary(ByVal key As Object) As String
        Dim result As Object = Nothing

        If (Not mainDataBase.TryGetValue(key, result)) Then
            '------------------
            '   Fetched nothing
            '------------------
            Return Nothing
        End If

        Return result
    End Function
    Protected Function searchInSecondary(ByVal key As Object) As String
        Dim result As Object = Nothing

        Dim keyAsEnum As [Enum] = TryCast(key, [Enum])
        If (keyAsEnum Is Nothing) Then
            '------------------
            '   Fetched nothing
            '------------------
            Return Nothing
        End If

        If (Not secondaryDictionary.TryGetValue(keyAsEnum, result)) Then
            '------------------
            '   Fetched nothing
            '------------------
            Return Nothing
        End If

        Return result
    End Function
    Protected Function traverseMessageParallelMode(ByVal key As Object) As String
        '--------------------------------------------------------------------------------------------------------
        '   Main key : type-info , or enum-type
        '    if main search result is dictionary , and main key is not type-info , do further search by key-value
        '   otherwise , main search result is not dictionary , output the result
        '--------------------------------------------------------------------------------------------------------

        Dim keyAsType As Type = TryCast(key, Type)
        Dim primaryResult As String = ""
        Dim secondaryResult As String = ""

        '--------------
        '   If key is not type info , try to cast it
        '--------------
        If (keyAsType IsNot Nothing) Then
            primaryResult = searchInPrimary(key)
        Else
            primaryResult = searchInPrimary(key.GetType())
        End If

        secondaryResult = searchInSecondary(key)

        If (secondaryResult IsNot Nothing And primaryResult IsNot Nothing) Then
            '---------------
            '   Both found in database
            '---------------
            Return String.Format("[{0}]{1}", primaryResult, secondaryResult)
        ElseIf (secondaryResult IsNot Nothing And primaryResult Is Nothing) Then
            '---------------
            '   Secondary found only
            '---------------
            Return secondaryResult
        ElseIf (secondaryResult Is Nothing And primaryResult IsNot Nothing) Then
            '---------------
            '   Primary found only
            '---------------
            Return primaryResult
        Else
            '---------------
            '   missing both
            '---------------
            Return key.ToString()
        End If

    End Function

    Public Function query(ByVal keyChain As Object) As Object Implements IMessageQuery.query
        Return traverseMessageParallelMode(keyChain)
    End Function












#Region "Discarded"
    Protected Function traverseMessageTypeMode(ByVal primaryKey As Object) As String
        '--------------------------------------------------------------------------------------------------------
        '   Main key : type-info , or enum-type
        '    if main search result is dictionary , and main key is not type-info , do further search by key-value
        '   otherwise , main search result is not dictionary , output the result
        '--------------------------------------------------------------------------------------------------------
        'Try
        Dim mainResult As Object = Nothing


        If (Not mainDataBase.TryGetValue(primaryKey, mainResult)) Then
            'Return "Key Not Found : " + primaryKey.ToString()
            Return primaryKey.ToString()
        End If

        Dim primaryKeyAsTypeInfo As Type = TryCast(primaryKey, Type)
        If (primaryKeyAsTypeInfo IsNot Nothing) Then
            '------------------------------------------------------------
            '   Key is Type Info(without further Value) , the tranverse would end in mainDictionary
            '------------------------------------------------------------
            '---------------------
            '   Success fetched
            '---------------------
            Return mainResult.ToString()
        End If
        '------------------------------------------
        '   Try Casting main result into dictionary
        '------------------------------------------
        Dim secondaryDictionary As Dictionary(Of Object, Object) = TryCast(mainResult, Dictionary(Of Object, Object))   'try search if theres more content
        If (secondaryDictionary Is Nothing) Then
            '---------------------
            '   Unable to fetch further dictionary , return its value
            '---------------------
            'Return mainResult.ToString() + "Secondary Dictionary Missing"
            Return mainResult.ToString()
        End If
        Dim secondaryResult As Object = Nothing
        If (Not secondaryDictionary.TryGetValue(primaryKey, secondaryResult)) Then
            '---------------------
            '   Key not found in secondary
            '---------------------
            'Return "Key Not Found In Secondary Dictionary" + primaryKey.ToString()
            Return primaryKey.ToString()
        End If
        '---------------------
        '   Success fetched
        '---------------------
        Return secondaryResult

    End Function
    Protected Function traverseMessageEnumMode(ByVal primaryKey As Object) As String
        '--------------------------------------------------------------------------------------------------------
        '   Main key : type-info , or enum-type
        '    if main search result is dictionary , and main key is not type-info , do further search by key-value
        '   otherwise , main search result is not dictionary , output the result
        '--------------------------------------------------------------------------------------------------------
        'Try
        Dim mainResult As Object = Nothing


        If (Not mainDataBase.TryGetValue(primaryKey, mainResult)) Then
            Return "Key Not Found In Main Dictionary: " + primaryKey.ToString()
        End If

        Dim primaryKeyAsTypeInfo As [Enum] = TryCast(primaryKey, [Enum])
        If (primaryKeyAsTypeInfo Is Nothing) Then
            '------------------------------------------------------------
            '   Key is Type Info(without further Value) , the tranverse would end in mainDictionary
            '------------------------------------------------------------
            '---------------------
            '   Success fetched
            '---------------------
            Return String.Format("[{0}]", mainResult.ToString())
        End If

        Dim secondaryResult As Object = Nothing
        If (Not secondaryDictionary.TryGetValue(primaryKey, secondaryResult)) Then
            '---------------------
            '   Key not found in secondary
            '---------------------
            Return "Key Not Found In Secondary Dictionary" + primaryKey.ToString()
        End If
        '---------------------
        '   Success fetched
        '---------------------
        Return String.Format("[{0}]:{1}", mainResult.ToString(), secondaryResult.ToString())

    End Function
#End Region

End Class
