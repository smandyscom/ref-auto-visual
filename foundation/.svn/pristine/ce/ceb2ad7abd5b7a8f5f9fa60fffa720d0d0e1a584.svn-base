Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Drawing.Design
Imports System.Reflection

Public Class utilities

    Public Shared Function findAllRelatingMotorPoints(ByVal sourcePointList As List(Of cMotorPoint), ByVal __p As Predicate(Of cMotorPoint), ByVal enumType As Type) As List(Of [Enum])
        Dim relatingPoints As List(Of cMotorPoint) = sourcePointList.FindAll(__p)

        Dim enumList As List(Of [Enum]) = New List(Of [Enum])
        For index = 0 To relatingPoints.Count - 1
            Dim __obj As Object = [Enum].ToObject(enumType, relatingPoints(index).AxisIndex)
            enumList.Add(__obj)
        Next
        Return enumList
    End Function

    Public Shared Sub floatFormatFilter(ByRef inputString As String, ByRef outputValue As Single, ByVal pattern As String)
        'if enter pressed
        Dim regexObject As Regex = New Regex(pattern)

        'do lower bound , up bound setting
        If (Not regexObject.IsMatch(inputString)) Then
            'not match , eject input , use last setting value
            inputString = outputValue.ToString()
        Else
            outputValue = CSng(inputString)
        End If

    End Sub
    Public Shared Function isFloatFormatPassed(ByVal inputString As String, ByVal pattern As String) As Boolean
        'if enter pressed
        Dim regexObject As Regex = New Regex(pattern)
        Return regexObject.IsMatch(inputString)
    End Function


    Public Shared Function single2Timespan(ByVal seconds As Single) As TimeSpan
        '--------------
        '   in order to solve seriablizing timespace issue
        '--------------
        Return New TimeSpan(0, 0, 0, 0, seconds * 1000)
    End Function

    Public Shared Sub object2Stream(ByVal source As Object, ByRef stream As System.IO.Stream)
        '---------------------------------
        '   Use binary serailzation method
        '---------------------------------
        Dim bf As System.Runtime.Serialization.Formatters.Binary.BinaryFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        bf.Serialize(stream, source)
    End Sub
    Public Shared Function stream2Object(ByVal source As System.IO.Stream) As Object
        '---------------------------------
        '   Use binary serailzation method
        '---------------------------------
        Dim bf As System.Runtime.Serialization.Formatters.Binary.BinaryFormatter = New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        Return bf.Deserialize(source)
    End Function

    Public Shared Function enumObjectsListing(ByVal enumType As Type) As [Enum]()
        '--------------------------------------
        '   Enumurate Enum members by enumType to items
        '--------------------------------------
        Dim valueArray As Array = [Enum].GetValues(enumType)
        Dim result As [Enum]() = New [Enum]() {}

        Array.Resize(result, valueArray.Length)

        For index = 0 To valueArray.Length - 1
            result(index) = [Enum].ToObject(enumType, valueArray(index))
        Next

        Return result
    End Function

    Public Shared Function gatherInvokeListResult(ByVal invokeList As Func(Of Boolean)) As Boolean
        '-------------------------------------
        '   Result &= every conditions
        '-------------------------------------
        Dim overallCondition As Boolean = True

        For Each del As Func(Of Boolean) In invokeList.GetInvocationList()
            overallCondition = overallCondition And del()
        Next

        Return overallCondition

    End Function
    Public Shared Function breakInvokeListResult(ByVal invokeList As Func(Of Boolean)) As Boolean
        '-------------------------------------
        '   if any result failed , break the invoke list
        '-------------------------------------
        For Each del As Func(Of Boolean) In invokeList.GetInvocationList()
            If (Not del()) Then
                Return False
            End If
        Next

        Return True

    End Function
    Public Shared Function getFullParentName(ByVal obj As Components.Services.driveBase) As String
        If obj.Parent Is Nothing Then
            Return obj.DeviceName
        Else
            Return getFullParentName(obj.Parent) & "." & obj.DeviceName
        End If
    End Function

    Public Shared Function convertBuildNumber2Date(__version As Version) As DateTime
        Dim __result As DateTime = New DateTime(2000, 1, 1)
        __result = __result.AddDays(__version.Build)
        __result = __result.AddSeconds(__version.Revision * 2)
        Return __result
    End Function
    '--------------------------------
    'Xml Serializer Cache
    '--------------------------------
    Shared serializersDictionary As Hashtable = New Hashtable() 'have to cache serialzers to prevent memory leak
    'according to : https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx
    'Dynamically Generated Assemblies

    Public Shared Function generateKey(ByVal __list As List(Of Type)) As ULong
        'generate unique key to represent each type of serializer
        Dim __code As ULong = 0

        __list.ForEach(Sub(__type As Type)
                           'maybe generate redundant key , have to improve this mechanism
                           __code += __type.GetHashCode * (__list.IndexOf(__type) + 1)

                       End Sub)

        Return __code
    End Function
    Public Shared Function getSerializer(ByVal mainType As Type, ByVal __list As List(Of Type)) As XmlSerializer

        Dim fullList As List(Of Type) = New List(Of Type)
        fullList.Add(mainType)

        If (__list IsNot Nothing) Then
            fullList.AddRange(__list)
        End If

        Dim key As ULong = generateKey(fullList)

        Dim cachedSerializer As XmlSerializer = TryCast(serializersDictionary(key), XmlSerializer)

        If (cachedSerializer Is Nothing) Then
            'no existed in table , create new one
            If (__list IsNot Nothing) Then
                cachedSerializer = New XmlSerializer(mainType, __list.ToArray)
            Else
                cachedSerializer = New XmlSerializer(mainType)
            End If
            serializersDictionary.Add(key, cachedSerializer)
        End If

        Return cachedSerializer
    End Function

    ''' <summary>
    ''' Given mask represeting constant , return the index(zero-based) where the first valid bit occurance
    ''' E.g 0xF0 -> 4
    ''' </summary>
    ''' <param name="mask"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function findFirstBitOccurance(mask As Long) As Integer
        For index = 0 To 63
            If ((mask >> index) And &H1) > 0 Then
                Return index
            End If
        Next
        Return 0
    End Function

    ''' <summary>
    ''' check this project is in ide enviroment or in exe file
    ''' </summary>
    Public Shared Function IsInIDEmode() As Boolean
        Return System.Diagnostics.Debugger.IsAttached()
    End Function
    ''' <summary>
    ''' Put this on the ban of windows form to inidicate software model and version
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared ReadOnly Property StandardTitle As String
        Get
            Return String.Format("{0:G},{1:G},{2:u}",
                                My.Application.Info.ProductName,
                                My.Application.Info.Version,
                                utilities.convertBuildNumber2Date(My.Application.Info.Version))
        End Get
    End Property

    Shared Function DetailedCompare(Of T)(val1 As T, val2 As T) As List(Of variance)
        Dim variances As List(Of variance) = New List(Of variance)
        Dim fi As FieldInfo() = val1.GetType().GetFields(BindingFlags.Instance Or
                                                                  BindingFlags.NonPublic Or
                                                                  BindingFlags.Public)
        For Each f As FieldInfo In fi
            Dim v As variance = New variance()
            v.Prop = f.Name
            v.valA = f.GetValue(val1)
            v.valB = f.GetValue(val2)
            If (Not v.valA.Equals(v.valB)) Then
                variances.Add(v)
            End If
        Next
        Return variances
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub bitSetUnset32(ByRef memory As Int32, bitIndex As Byte, value As Boolean)
        If value Then
            'set
            memory = memory Or 2 ^ bitIndex
        Else
            'unset
            memory = memory And Not 2 ^ bitIndex
        End If
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub bitSetUnset64(ByRef memory As Int64, bitIndex As Byte, value As Boolean)
        If value Then
            'set
            memory = memory Or 2 ^ bitIndex
        Else
            'unset
            memory = memory And Not 2 ^ bitIndex
        End If
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ulongToUshort(value As ULong) As UShort
        Dim __array = BitConverter.GetBytes(value)
        Return BitConverter.ToUInt16({__array(0),
                                      __array(1)}, 0)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ulongToUinteger(value As ULong) As UInt32
        Dim __array = BitConverter.GetBytes(value)
        Return BitConverter.ToUInt32({__array(0),
                                      __array(1),
                                      __array(2),
                                      __array(3)}, 0)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function listEnumMembers(__type As Type) As List(Of [Enum])
        Dim output As List(Of [Enum]) = New List(Of [Enum])
        Dim values As Array = [Enum].GetValues(__type)
        For index = 0 To values.Length - 1
            output.Add([Enum].ToObject(__type, values(index)))
        Next
        Return output
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getEnumDescription(item As [Enum]) As String
        Return CType(item.GetType.GetMember(item.ToString).First.GetCustomAttributes(GetType(DescriptionAttribute), False).First, DescriptionAttribute).Description
    End Function
End Class
Public Class variance
    Public Prop As String
    Public valA As Object
    Public valB As Object
End Class
Public Class utilitiesUI
    '----------------------------
    '   Categroried UI related utilities here
    '----------------------------
    Public Shared Sub textBoxKeepScrollBottom(ByRef textBox As TextBoxBase)
        '---------------------
        ' keep scoll on bottom
        '---------------------
        textBox.SelectionStart = textBox.Text.Length
        textBox.ScrollToCaret()
    End Sub
    Public Shared Sub textBoxLimitMaxLine(ByRef textBox As TextBoxBase, ByVal maxLines As Integer)
        '-------------------------------------------
        '    Discard oldest message if count reached to prevent buffer overload
        '-------------------------------------------
        If (textBox.Lines.Length > maxLines) Then
            'Hsien  ,2015.12.30 , mechanism replaced
            textBox.Lines = textBox.Lines.Skip(textBox.Lines.Length - maxLines).ToArray() 'discards first N lines
            'textBox.Text = String.Join(vbCrLf, textBox.Lines, 1, textBox.Lines.Count - 1)   'defect : delete only one line, once multi lines comein , the buffer would increase gradually
        End If
    End Sub
    Public Shared Function labelFollowBoolean(ByRef label As Label, ByVal value As Boolean) As Integer
        'generic solution
        If (value) Then
            label.BackColor = Color.Green
        Else
            label.BackColor = Color.Red
        End If

        Return 0
    End Function
    Public Shared Function controlFollowBoolean(ByRef __control As Control,
                                                ByVal value As Boolean) As Integer


        'generic solution
        If (value) Then
            __control.BackColor = Color.Yellow
        Else
            __control.BackColor = Control.DefaultBackColor
        End If

        Return 0
    End Function
    Public Shared Function controlFollowBooleanColor(ByRef __control As Control,
                                                ByVal value As Boolean,
                                                Optional ByVal __trueColor As Color = Nothing,
                                                Optional ByVal __falseColor As Color = Nothing) As Integer

        If (__trueColor.IsEmpty) Then
            __trueColor = Color.Yellow
        End If

        If (__falseColor.IsEmpty) Then
            __falseColor = Control.DefaultBackColor
        End If

        'generic solution
        If (value) Then
            __control.BackColor = __trueColor
        Else
            __control.BackColor = __falseColor
        End If

        Return 0
    End Function

    Public Shared Sub applyResourceOnAllControls(ByVal controlList As System.Windows.Forms.Control.ControlCollection, ByVal resource As System.ComponentModel.ComponentResourceManager)
        ' applying on all controls
        For Each uc As Control In controlList
            uc.SuspendLayout()
            resource.ApplyResources(uc, uc.Name, Application.CurrentCulture)
            If (uc.Controls.Count <> 0) Then
                If (uc.GetType().BaseType = GetType(UserControl)) Then
                    applyResourceOnAllControls(uc.Controls, New System.ComponentModel.ComponentResourceManager(uc.GetType()))
                Else
                    applyResourceOnAllControls(uc.Controls, resource)
                End If
            ElseIf TypeOf uc Is MenuStrip Then
                applyResourceOnToolStripItemCollection(TryCast(uc, MenuStrip).Items, resource)
            End If
            uc.ResumeLayout()
        Next
    End Sub
    Private Shared Sub applyResourceOnToolStripItemCollection(collection As ToolStripItemCollection, resource As System.ComponentModel.ComponentResourceManager)
        For Each item As ToolStripItem In collection
            resource.ApplyResources(item, item.Name, Application.CurrentCulture)
            If TypeOf item Is ToolStripDropDownItem Then
                applyResourceOnToolStripItemCollection(TryCast(item, ToolStripDropDownItem).DropDownItems, resource)
            End If
        Next
    End Sub


    Public Shared Sub loadComboBoxItemByEnum(ByVal cb As ComboBox, ByVal arg As Type)
        For index = 0 To [Enum].GetValues(arg).Length - 1
            cb.Items.Add(CTypeDynamic([Enum].ToObject(arg, [Enum].GetValues(arg)(index)), arg))
        Next
    End Sub
    Public Shared Sub loadComboBoxItemByEnum(ByVal cb As ComboBox, ByVal arg As [Enum]())
        For index = 0 To arg.Length - 1
            cb.Items.Add(arg(index))
        Next
    End Sub

    Public Shared Function findControlsWithTargetType(ByVal searchRoot As Control, targetType As Type) As List(Of Control)
        'find all Controls in target type
        'Hsien , 2015.10.05

        Dim __collectionOfControls As List(Of Control) = New List(Of Control)

        For Each __control As Control In searchRoot.Controls
            'once this instance is in target type
            If (targetType.IsInstanceOfType(__control)) Then
                __collectionOfControls.Add(__control)
            End If

            __collectionOfControls.AddRange(findControlsWithTargetType(__control, targetType))   'recursivly find next level

        Next

        Return __collectionOfControls
    End Function



    Class trueFalseTypeConvertor1
        Inherits TypeConverter
        Public Overrides Function GetStandardValuesSupported(context As ITypeDescriptorContext) As Boolean
            Return True
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="context"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetStandardValues(context As ITypeDescriptorContext) As TypeConverter.StandardValuesCollection
            Return New TypeConverter.StandardValuesCollection(New List(Of Boolean) From {True, False})
        End Function
        ''' <summary>
        ''' String to boolean
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="culture"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object

            If (value = My.Resources.ENABLED) Then
                Return True
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' Boolean to String
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="culture"></param>
        ''' <param name="value"></param>
        ''' <param name="destinationType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object

            If (value) Then
                Return My.Resources.ENABLED
            Else
                Return My.Resources.DISABLED
            End If

        End Function

        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
            Return True
        End Function
    End Class

    Class toStringTypeConvertor
        Inherits TypeConverter

        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            Return value.ToString
        End Function

    End Class
    ''' <summary>
    ''' The generic proeprty grid editor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class popupPropertyGridEditor
        Inherits UITypeEditor

        Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

            Dim __dialog As Form = New Form
            Dim __pg As PropertyGrid = New PropertyGrid With {.Dock = DockStyle.Fill}
            With __dialog
                .Text = context.PropertyDescriptor.DisplayName
                .AutoSize = True
                .StartPosition = FormStartPosition.CenterScreen
                .Controls.Add(__pg)
                __pg.SelectedObject = value
                .ShowDialog()
            End With

            Return value
        End Function
        Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.Modal
        End Function

    End Class

    Public Class pathAttribute
        Inherits Attribute

        ReadOnly Property SubPath As String
            Get
                Return String.Format("{0}{1}",
                                     My.Application.Info.DirectoryPath,
                                     __subPath)
            End Get
        End Property

        Dim __subPath As String = ""

        Sub New(subPath As String)
            Me.__subPath = subPath
        End Sub

    End Class
    Public Class filterAttribute
        Inherits Attribute

        ReadOnly Property Filter As String
            Get
                Return __filter
            End Get
        End Property

        Dim __filter As String = "xml files (*.xml)|*.xml"

        Sub New(filter As String)
            __filter = filter
        End Sub

    End Class

    Public Class FileNamesEditor
        Inherits UITypeEditor
        Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

            Using da As OpenFileDialog = New OpenFileDialog
                With da

                    .InitialDirectory = CType(context.PropertyDescriptor.Attributes.Item(GetType(pathAttribute)), pathAttribute).SubPath
                    .Filter = CType(context.PropertyDescriptor.Attributes.Item(GetType(filterAttribute)), filterAttribute).Filter
                    .AddExtension = True
                    .RestoreDirectory = True
                    If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                        Return .SafeFileName
                    End If
                End With
                Return value
            End Using
        End Function
        Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.Modal
        End Function


    End Class

    ''' <summary>
    ''' For list of keyvalue pairs
    ''' </summary>
    ''' <remarks></remarks>
    Class keyPairsEditor
        Inherits UITypeEditor

        Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

            Using __form As Form = New Form

                Dim dgv As DataGridView = New DataGridView
                Dim __collection As List(Of Object) = New List(Of Object)
                For index = 0 To value.Count - 1
                    __collection.Add(value(index).Value)
                Next

                dgv.DataSource = __collection
                __form.Controls.Add(dgv)

                __form.ShowDialog()

            End Using

            Return value
        End Function

        Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.Modal
        End Function

    End Class

End Class


'reference from stackoverflow : http://stackoverflow.com/questions/255341/getting-key-of-value-of-a-generic-dictionary#255630
Public Class BiDictionary(Of TFirst, TSecond)
    Private firstToSecond As IDictionary(Of TFirst, IList(Of TSecond)) = New Dictionary(Of TFirst, IList(Of TSecond))()
    Private secondToFirst As IDictionary(Of TSecond, IList(Of TFirst)) = New Dictionary(Of TSecond, IList(Of TFirst))()

    Private Shared EmptyFirstList As IList(Of TFirst) = New TFirst(-1) {}
    Private Shared EmptySecondList As IList(Of TSecond) = New TSecond(-1) {}

    Public Sub Add(first As TFirst, second As TSecond)
        Dim firsts As IList(Of TFirst) = Nothing
        Dim seconds As IList(Of TSecond) = Nothing
        If Not firstToSecond.TryGetValue(first, seconds) Then
            seconds = New List(Of TSecond)()
            firstToSecond(first) = seconds
        End If
        If Not secondToFirst.TryGetValue(second, firsts) Then
            firsts = New List(Of TFirst)()
            secondToFirst(second) = firsts
        End If
        seconds.Add(second)
        firsts.Add(first)
    End Sub

    ' Note potential ambiguity using indexers (e.g. mapping from int to int)
    ' Hence the methods as well...
    Default Public ReadOnly Property Item(first As TFirst) As IList(Of TSecond)
        Get
            Return GetByFirst(first)
        End Get
    End Property

    Default Public ReadOnly Property Item(second As TSecond) As IList(Of TFirst)
        Get
            Return GetBySecond(second)
        End Get
    End Property

    Public Function GetByFirst(first As TFirst) As IList(Of TSecond)
        Dim list As IList(Of TSecond) = Nothing
        If Not firstToSecond.TryGetValue(first, list) Then
            Return EmptySecondList
        End If
        Return New List(Of TSecond)(list)
        ' Create a copy for sanity
    End Function

    Public Function GetBySecond(second As TSecond) As IList(Of TFirst)
        Dim list As IList(Of TFirst) = Nothing
        If Not secondToFirst.TryGetValue(second, list) Then
            Return EmptyFirstList
        End If
        Return New List(Of TFirst)(list)
        ' Create a copy for sanity
    End Function
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================