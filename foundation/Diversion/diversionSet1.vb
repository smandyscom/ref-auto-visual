﻿Imports System.ComponentModel
Imports System.Drawing.Design
Imports System.Windows.Forms.Design

Public Interface IDiversionCodeFetchable
    ReadOnly Property DiversionCode As ULong
End Interface

Namespace diversionSet1

    Public Class decoder

        ''' <summary>
        ''' Used to do value-type access (writing)
        ''' </summary>
        ''' <param name="bitIndex"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Delegate Sub setResetMethodPrototype(bitIndex As Byte, value As Boolean)

        Public valueSetResetMethod As setResetMethodPrototype = Nothing

        Public Property IsBooleanTypeEnabled(code As Long, mask As Long) As Boolean
            Get
                Return (code And mask) > 0
            End Get
            Set(value As Boolean)
                valueSetResetMethod.Invoke(utilities.findFirstBitOccurance(mask), value)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SelectionCassetteConveyorMotor(code As Long) As conveyorMotorSelection
            Get
                Return [Enum].ToObject(GetType(conveyorMotorSelection), (code And subVersionMaskDefinitions.SELECTION_CASSETTE_CONVEYOR_MOTOR) >>
                    utilities.findFirstBitOccurance(subVersionMaskDefinitions.SELECTION_CASSETTE_CONVEYOR_MOTOR))
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="code"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SelectionLoaderInspection(code As Long) As inspectionDeviceEnum
            Get
                Return [Enum].ToObject(GetType(inspectionDeviceEnum), (code And subVersionMaskDefinitions.SELECTION_LOADER_INSPECTION) >>
                    utilities.findFirstBitOccurance(subVersionMaskDefinitions.SELECTION_LOADER_INSPECTION))
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="code"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SelectionUnloaderInspection(code As Long) As inspectionDeviceEnum
            Get
                Return [Enum].ToObject(GetType(inspectionDeviceEnum), (code And subVersionMaskDefinitions.SELECTION_UNLOADER_INSPECTION) >>
                    utilities.findFirstBitOccurance(subVersionMaskDefinitions.SELECTION_UNLOADER_INSPECTION))
            End Get
        End Property
        Sub New(valueSetResetMethod As setResetMethodPrototype )
            Me.valueSetResetMethod = valueSetResetMethod
        End Sub

    End Class

    <Editor(GetType(customedDropDownEditor), GetType(UITypeEditor))>
    <TypeConverter(GetType(cassetteTypeConvertor))>
    Public Enum cassetteTypeEnum As UShort
        JR_REGULAR = &H1
        JR_REVERSE = &H2
        ACI_REGULAR = &H4
        ACI_REVERSE = &H8
        JR_REGULAR_WITH_PLATE = &H10
        JR_REVERSE_WITH_PLATE = &H20
        SCHMIT100 = &H40
        BACCINI50 = &H80   'Hsien , 2016.04.12
    End Enum

    'Hsien ,  2015.08.31 , mode selection
    <TypeConverter(GetType(mainlineModeTypeConvertor))>
    Public Enum mainlineModeEnum
        ID_MODE = 0
        QUEUE_MODE_NO_ID_VERIFICATION = -1
    End Enum

    Public Enum subVersionMaskDefinitions As Long
        IS_STANDALONE_ENABLED = &H1
        IS_DOORINTERLOCK_DISABLED = &H2
        IS_GEM_ENABLED = &H4
        ''' <summary>
        ''' True(-1) : Queue Mode
        ''' False(0) : ID Mode (Default)
        ''' </summary>
        ''' <remarks></remarks>
        IS_QUEUE_DOMINATION_ENABLED = &H8
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        SELECTION_CASSETTE_CONVEYOR_MOTOR = &HF00
        ''' <summary>
        ''' Affect UI Only
        ''' </summary>
        ''' <remarks></remarks>
        IS_LOADING_CASSETTE_TYPE_READONLY = &H1000
        ''' <summary>
        ''' Affect UI Only
        ''' </summary>
        ''' <remarks></remarks>
        IS_UNLOADING_CASSETTE_TYPE_READONLY = &H2000
        ''' <summary>
        ''' Affect UI Only
        ''' </summary>
        ''' <remarks></remarks>
        SELECTION_LOADING_CASSETTE_TYPE_FORBIDDEN = &HFF0000
        ''' <summary>
        ''' Affect UI Only
        ''' </summary>
        ''' <remarks></remarks>
        SELECTION_UNLOADING_CASSETTE_TYPE_FORBIDDEN = &HFF000000

        SELECTION_LOADER_INSPECTION = &HF00000000
        SELECTION_UNLOADER_INSPECTION = &HF000000000
    End Enum

    Public Enum conveyorMotorSelection As Byte
        DELTA_A2 = 0
        BMU = 1
    End Enum

    Public Enum inspectionDeviceEnum As Byte
        VIVITECH_PVA = 0
        BROKEN_CHECK = 1
    End Enum

    Public Class selectionForbiddenAttribute
        Inherits Attribute

        ReadOnly Property Selection As subVersionMaskDefinitions
            Get
                Return __selection
            End Get
        End Property

        Dim __selection As subVersionMaskDefinitions = subVersionMaskDefinitions.SELECTION_LOADING_CASSETTE_TYPE_FORBIDDEN
        Sub New(__selection As subVersionMaskDefinitions)
            Me.__selection = __selection
        End Sub

    End Class

    Class customedDropDownEditor
        Inherits UITypeEditor

        Dim possibleSelections As UShort = UShort.MaxValue 'default open all selections

        Dim WithEvents __lb As ListBox = Nothing
        Dim __service As IWindowsFormsEditorService = Nothing

        Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
            Return UITypeEditorEditStyle.DropDown
        End Function
        Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object

            Dim __generitcSettingReference As IDiversionCodeFetchable = CType(context.Instance, IDiversionCodeFetchable)

            __service = provider.GetService(GetType(IWindowsFormsEditorService))

            __lb = New ListBox With {.SelectionMode = SelectionMode.One,
                                     .FormattingEnabled = True} ' formatting enabled would use type convertor  , Hsien , 2016.04.18

            Dim __forbiddenMask As subVersionMaskDefinitions = subVersionMaskDefinitions.SELECTION_LOADING_CASSETTE_TYPE_FORBIDDEN

            Select Case context.PropertyDescriptor.Name
                Case "CassetteTypeLoad"
                    __forbiddenMask = subVersionMaskDefinitions.SELECTION_LOADING_CASSETTE_TYPE_FORBIDDEN
                Case "CassetteTypeUnload"
                    __forbiddenMask = subVersionMaskDefinitions.SELECTION_UNLOADING_CASSETTE_TYPE_FORBIDDEN
                Case Else
                    'use attribute to judge
                    Dim selectionAttribute As selectionForbiddenAttribute =
                        TryCast(context.PropertyDescriptor.Attributes(GetType(selectionForbiddenAttribute)), selectionForbiddenAttribute)
                    If selectionAttribute IsNot Nothing Then
                        __forbiddenMask = selectionAttribute.Selection
                    End If
            End Select

            'output
            possibleSelections = Not CUShort((__generitcSettingReference.DiversionCode And __forbiddenMask) >> utilities.findFirstBitOccurance(__forbiddenMask))


            Dim valueArrays As Array = [Enum].GetValues(GetType(cassetteTypeEnum))
            For index = 0 To valueArrays.Length - 1
                If (possibleSelections And valueArrays(index)) > 0 Then
                    'selected
                    __lb.Items.Add([Enum].ToObject(GetType(cassetteTypeEnum), valueArrays(index)))
                End If
            Next

            __service.DropDownControl(__lb)

            If (__lb.SelectedItem Is Nothing) Then
                Return value
            End If

            Return __lb.SelectedItem
        End Function


        Sub itemSelected() Handles __lb.SelectedValueChanged
            __service.CloseDropDown()
        End Sub


    End Class



    Public Class cassetteTypeConvertor
        Inherits EnumConverter      'should use enum type convertor

        Sub New(__type As Type)
            MyBase.New(__type)

            With __biDictionaryChinese
                .Add(cassetteTypeEnum.ACI_REGULAR, My.Resources.CassetteACIRegular)
                .Add(cassetteTypeEnum.ACI_REVERSE, My.Resources.CassetteACIReverse)
                .Add(cassetteTypeEnum.JR_REGULAR, My.Resources.CassetteJRRegular)
                .Add(cassetteTypeEnum.JR_REVERSE, My.Resources.CassetteJRReverse)
                .Add(cassetteTypeEnum.JR_REGULAR_WITH_PLATE, My.Resources.CassetteJRRegularPlate)
                .Add(cassetteTypeEnum.JR_REVERSE_WITH_PLATE, My.Resources.CassetteJRReversePlate)
                .Add(cassetteTypeEnum.SCHMIT100, My.Resources.CassetteSchmit)
                .Add(cassetteTypeEnum.BACCINI50, My.Resources.CassetteBaccini)
            End With
        End Sub

        ''' <summary>
        ''' Used to do enum-string conversion
        ''' </summary>
        ''' <remarks></remarks>
        Shared __biDictionaryChinese As BiDictionary(Of cassetteTypeEnum, String) = New BiDictionary(Of cassetteTypeEnum, String)

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return True
        End Function

        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            Return __biDictionaryChinese.GetByFirst(value).First
        End Function

        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
            Return __biDictionaryChinese.GetBySecond(value).First
        End Function

    End Class

    Class mainlineModeTypeConvertor
        Inherits EnumConverter      'should use enum type convertor

        Sub New(__type As Type)
            MyBase.New(__type)
        End Sub

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return True
        End Function

        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object

            Dim __returnString As String = ""

            Select Case CType(value, mainlineModeEnum)
                Case mainlineModeEnum.ID_MODE
                    __returnString = My.Resources.SettingIDMode
                Case mainlineModeEnum.QUEUE_MODE_NO_ID_VERIFICATION
                    __returnString = My.Resources.SettingQueueMode
            End Select


            Return __returnString

        End Function

        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object

            Dim __enum As mainlineModeEnum = mainlineModeEnum.ID_MODE

            Select Case value
                Case My.Resources.SettingIDMode
                    __enum = mainlineModeEnum.ID_MODE
                Case My.Resources.SettingQueueMode
                    __enum = mainlineModeEnum.QUEUE_MODE_NO_ID_VERIFICATION
            End Select


            Return __enum
        End Function



    End Class


End Namespace
