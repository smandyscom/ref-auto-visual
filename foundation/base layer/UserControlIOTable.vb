﻿Imports System.Xml
Imports System.Xml.Serialization
Imports System.Text.RegularExpressions
Imports Automation.mainIOHardware
Imports Automation.ioTable

Public Class UserControlIOTable
    Const ROWS_PER_MODULE = 5 '每個modle在data grid view中所占的行數，第一列為Rx-xx

    Dim __ioTableFromXml As New ioTable 'read from xml file
    Dim __ioTable As New ioTable '實際存在的輸入出模組列表

    Dim xmlFilePath As String = Application.StartupPath & "\Data\IOTable.xml"

    Public Function initialize(amaxDevice As amaxCard) As Integer

        '初始化_InDeviceIP_Table:實際的device ip table(不含空的模組)
        Dim inputModulesList As List(Of amaxModule) = amaxDevice.ExpectedModuleCounts(amaxModule.moduleHardwareCodeEnum.AMAX_2752_1752)
        Dim outputModulesList As List(Of amaxModule) = amaxDevice.ExpectedModuleCounts(amaxModule.moduleHardwareCodeEnum.AMAX_2754_1754)

        inputModulesList.Sort(Function(x As amaxModule, y As amaxModule) ((x.RingIndex - y.RingIndex) * 1000 + (x.slaveIndex - y.slaveIndex)))
        outputModulesList.Sort(Function(x As amaxModule, y As amaxModule) ((x.RingIndex - y.RingIndex) * 1000 + (x.slaveIndex - y.slaveIndex)))

        dataGridViewInput.RowCount = inputModulesList.Count * ROWS_PER_MODULE
        dataGridViewOutput.RowCount = outputModulesList.Count * ROWS_PER_MODULE

        'IoTable.initial(amaxInDeviceIP_Table, InNoExist, amaxOutDeviceIP_Table, OutNoExist)
        initializeTable(__ioTable.InputModules, ioTable.IO_MODULE_TYPE.IP, inputModulesList, dataGridViewInput)
        initializeTable(__ioTable.OutputModules, ioTable.IO_MODULE_TYPE.OP, outputModulesList, dataGridViewOutput)

        '讀取xml file 取得io點的名稱及描述
        Try
            OpenXmlFile(__ioTableFromXml, xmlFilePath)
        Catch ex As Exception
            If IO.File.Exists(xmlFilePath) = True Then
                IO.File.Copy(xmlFilePath, xmlFilePath & ".bak", True)
            End If
            '若之前有檔案，則建立備份
        End Try
        'check input module is exit
        '將xml內的資料載入ioTable
        __ioTable.Assign(__ioTableFromXml)

        Return 0
    End Function




    Public Overrides Sub Refresh()
        __ioTable.InputModules.ForEach(Sub(__module As ioModule) __module.IoElements.ForEach(Sub(__elements As ioElement) __elements.refreshRelatingCell()))
        __ioTable.OutputModules.ForEach(Sub(__module As ioModule) __module.IoElements.ForEach(Sub(__elements As ioElement) __elements.refreshRelatingCell()))
        MyBase.Refresh()
    End Sub

#Region "persistance"
    Private Function ExportIOTableToCsvFile(_stream As System.IO.Stream, delimiter As String) As Integer
        Dim sw As New System.IO.StreamWriter(_stream)
        sw.WriteLine(String.Format("{1}{0}{2}{0}{3}", delimiter, "Number", "Symbol", "Description"))
        For Each _ListModule As List(Of ioTable.ioModule) In {__ioTable.InputModules, __ioTable.OutputModules}
            For Each _module As ioTable.ioModule In _ListModule
                For Each _dio As ioTable.ioElement In _module.IoElements
                    sw.WriteLine(String.Format("{1}{0}{2}{0}{3}", delimiter, _dio.AddressInIpOpForm, _dio.Symbol, _dio.Description))
                Next
            Next
        Next
        sw.Close()
        Return 0
    End Function
    Private Function ImportIOTableFromCsvFile(_filePath As String, delimiter As String) As Integer
        Try
            Dim allLines As List(Of String) = System.IO.File.ReadAllLines(_filePath, System.Text.Encoding.Default).ToList
            For Each _ListModule As List(Of ioTable.ioModule) In {__ioTable.InputModules, __ioTable.OutputModules}
                For Each _module As ioTable.ioModule In _ListModule
                    For Each _dio As ioTable.ioElement In _module.IoElements
                        Dim s As String = allLines.Find(Function(obj As String)
                                                            If Regex.IsMatch(obj, "^" & _dio.AddressInIpOpForm) Then Return True
                                                            Return False
                                                        End Function)
                        If s <> "" Then
                            Dim _string() = s.Split(delimiter)
                            If _string.Count >= 3 Then
                                _dio.Symbol = _string(1)
                                _dio.Description = _string(2)
                            End If
                        End If
                    Next
                Next
            Next
            '顯示在data grid view中
            'initialDataGridViewDIO(dataGridViewInput, __ioTable.InputModules)
            'initialDataGridViewDIO(dataGridViewOutput, __ioTable.OutputModules)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace.ToString)
        End Try


        Return 0
    End Function

    Private Sub importClick(sender As Object, e As EventArgs) Handles ToolStripMenuItemImportCSVComma.Click, ToolStripMenuItemImportCSVSemiComma.Click
        Dim _openFileDialog As New OpenFileDialog
        With _openFileDialog
            .InitialDirectory = My.Application.Info.DirectoryPath
            .Filter = "csv files|*.csv" '|EXE files|*.exe|All Files|*.*"
            .RestoreDirectory = True
            If (.ShowDialog = DialogResult.OK) Then
                Dim delimiter As String = ","
                Select Case sender.name
                    Case ToolStripMenuItemImportCSVComma.Name
                        delimiter = ","
                    Case ToolStripMenuItemImportCSVSemiComma.Name
                        delimiter = ";"
                End Select
                ImportIOTableFromCsvFile(_openFileDialog.FileName, delimiter)
                SaveXmlFile(__ioTable, xmlFilePath)
            End If

        End With
    End Sub

    Private Sub exportClick(sender As Object, e As EventArgs) Handles ToolStripMenuItemExportCSVComma.Click, ToolStripMenuItemExportCSVSemiComma.Click
        'mdlXmlSaveLoad.SaveXmlFile(IoTable, My.Application.Info.DirectoryPath & "\Data\" + "IOTable1.xml")
        Dim _saveFileDialog As New SaveFileDialog
        With _saveFileDialog
            .Title = "Export IO Table as a text file"
            .InitialDirectory = My.Application.Info.DirectoryPath
            .Filter = "csv files|*.csv" '|EXE files|*.exe|All Files|*.*"
            .RestoreDirectory = True
            .ShowDialog(Me)
        End With
        If _saveFileDialog.FileName <> "" Then
            Dim delimiter As String = ","
            Select Case sender.name
                Case ToolStripMenuItemExportCSVComma.Name
                    delimiter = ","
                Case ToolStripMenuItemExportCSVSemiComma.Name
                    delimiter = ";"
            End Select
            ExportIOTableToCsvFile(_saveFileDialog.OpenFile(), delimiter)
        End If
    End Sub

#End Region


    Private Function findElementByCell(sender As DataGridView, e As System.Windows.Forms.DataGridViewCellEventArgs) As ioElement
        'use cell as key to get which the element is 
        Dim cell As DataGridViewCell = sender.Item(e.ColumnIndex, e.RowIndex)
        Dim element As ioElement = Nothing

        For Each __moduleGroup As List(Of ioModule) In {__ioTable.InputModules,
                                                          __ioTable.OutputModules}
            __moduleGroup.ForEach(Sub(__module As ioModule)

                                      'had found , break iterating
                                      If (element IsNot Nothing) Then
                                          Exit Sub
                                      End If

                                      element = __module.IoElements.Find(Function(__element As ioElement) (__element.relatingCell.Equals(cell)))

                                  End Sub)
        Next

        Return element
    End Function

#Region "Data grid view relating event handlers"
    Private Sub editDescriptions(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dataGridViewOutput.CellMouseClick, dataGridViewInput.CellMouseClick

        If e.RowIndex Mod ROWS_PER_MODULE = 0 Then
            Exit Sub '避開顯示模組("Rx-xx)的那一列
        End If

        'right click used to edit
        If e.Button = Windows.Forms.MouseButtons.Right Then

            Dim Rst As String = String.Empty
            Dim bEdited As Boolean = False
            Dim moduleIndex As Short = e.RowIndex \ ROWS_PER_MODULE
            Dim _ioModule As ioTable.ioModule '= IoTable.OutputModules(moduleIndex)
            Dim _dio As ioTable.ioElement '= _ioModule.DIOs((e.RowIndex Mod ROWS_PER_MODULE - 1) * dgvDO.ColumnCount + e.ColumnIndex)
            Dim _dgv As DataGridView = TryCast(sender, DataGridView)
            Dim _dioName As String
            Select Case _dgv.Name
                Case dataGridViewInput.Name
                    _ioModule = __ioTable.InputModules(moduleIndex)
                    _dioName = "DI"
                Case Else
                    _ioModule = __ioTable.OutputModules(moduleIndex)
                    _dioName = "DO"
            End Select
            _dio = _ioModule.IoElements((e.RowIndex Mod ROWS_PER_MODULE - 1) * _dgv.ColumnCount + e.ColumnIndex)
            Rst = InputBox(String.Format("Please edit the {0} name", _dioName),
                                        String.Format("{0} Edit", _dioName),
                                        _dio.Symbol)
            If String.IsNullOrEmpty(Rst) = False AndAlso String.Compare(Rst, _dio.Symbol) <> 0 Then  ' 
                _dio.Symbol = Rst : bEdited = True
                _dgv(e.ColumnIndex, e.RowIndex).Value = String.Format("{0}  {1}", _dio.AddressInIpOpForm, _dio.Symbol)
            End If

            Rst = InputBox(String.Format("Please edit the {0} description", _dioName), String.Format("{0} Edit", _dioName), _dio.Description)
            If String.IsNullOrEmpty(Rst) = False AndAlso String.Compare(Rst, _dio.Description) <> 0 Then
                _dio.Description = Rst : bEdited = True
            End If

            If bEdited = True Then
                SaveXmlFile(__ioTable, My.Application.Info.DirectoryPath & "\Data\" & "IOTable.xml")
            End If
        End If
    End Sub
    Private Sub flipOutput(sender As DataGridView, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataGridViewOutput.CellClick
        If e.RowIndex Mod ROWS_PER_MODULE = 0 Then
            Exit Sub '避開顯示模組("Rx-xx)的那一列
        End If

        'use cell as key to get which the element is , then flip its value , Hsien , 2015.12.18
        Dim element As ioElement = findElementByCell(sender, e)
        writeBit(element.AddressInMemoryPoolForm, Not readBit(element.AddressInMemoryPoolForm))
    End Sub
    Private Sub showDescriptionOnBar(sender As DataGridView, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataGridViewInput.CellMouseEnter, dataGridViewOutput.CellMouseEnter
        If e.RowIndex Mod ROWS_PER_MODULE = 0 Then
            Exit Sub '避開顯示模組("Rx-xx)的那一列
        End If

        Dim foundIoElement As ioElement = findElementByCell(sender, e)
        With foundIoElement
            textBoxIODescription.Text = String.Format("{0},{1},{2}",
                                                      .AddressInIpOpForm,
                                                      .Symbol,
                                                      .Description)
        End With

    End Sub
#End Region





    ''' <summary>
    ''' Given innerData , iType , expectedModules , would initialize data grid view
    ''' </summary>
    ''' <param name="innerData"></param>
    ''' <param name="iType"></param>
    ''' <param name="expectedModules"></param>
    ''' <param name="viewTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function initializeTable(innerData As List(Of ioModule),
                                           iType As IO_MODULE_TYPE,
                                           expectedModules As List(Of amaxModule),
                                           viewTable As DataGridView) As Integer
        'initialize I/O table (fill in data only)
        innerData.Clear()

        For moduleIndex = 0 To expectedModules.Count - 1

            With expectedModules(moduleIndex)


                Dim moduleRowStartIndex As Integer = moduleIndex * ROWS_PER_MODULE

                '--------------------
                '   Make Banner
                '--------------------
                viewTable.Item(0, moduleRowStartIndex).Value = String.Format("R{0}-{1:00}", .RingIndex, .slaveIndex)

                Dim __ioModule As ioModule = New ioModule With {.Ring = expectedModules(moduleIndex).RingIndex,
                                                                                .DeviceIP = expectedModules(moduleIndex).slaveIndex,
                                                                                .Type = iType}


                'listTable.Last.DIOs.Clear()

                'i.e first input module(device ip : 3) -> 1
                '       
                Dim sequenceInTable As Integer = expectedModules.IndexOf(expectedModules(moduleIndex))


                '--------------------------------
                '   Prepare DIO
                '--------------------------------
                For bitIndex = 0 To BITS_PER_MODULE - 1

                    Dim ioSequence As Integer = sequenceInTable * BITS_PER_MODULE + bitIndex

                    Dim element As ioElement = New ioElement With {.AddressInMemoryPoolForm = BitConverter.ToUInt64({bitIndex,
                                                                                                      0,
                                                                                                      expectedModules(moduleIndex).slaveIndex,
                                                                                                      expectedModules(moduleIndex).RingIndex,
                                                                                                      amaxModuleTypeEnum.REMOTE,
                                                                                                      0,
                                                                                                      0,
                                                                                                      hardwareCodeEnum.AMAX_1202_CARD}, 0),
                                                                   .AddressInIpOpForm = String.Format("{0}{1:X}{2}",
                                                                                       iType.ToString,
                                                                                       ioSequence \ 8,
                                                                                       bitIndex Mod 8),
                                                                   .Symbol = "",
                                                                   .Description = "",
                                                                   .relatingCell = viewTable.Item(bitIndex Mod 8, moduleRowStartIndex + 1 + bitIndex \ viewTable.ColumnCount)}
                    __ioModule.IoElements.Add(element)
                Next

                innerData.Add(__ioModule)

                '------------------------------------------------------
                '   Data Grid View Initialization
                '------------------------------------------------------


            End With
        Next

        Return 0
    End Function

End Class

<Serializable> Public Class ioTable
    Public Const BITS_PER_MODULE = 32 '每個module有幾個點位
    Public Property InputModules As New List(Of ioModule)
    Public Property OutputModules As New List(Of ioModule)


    Public Function Assign(source As ioTable) As Integer
        For Each listModules As List(Of ioTable.ioModule) In {InputModules, OutputModules}
            For Each _Module As ioTable.ioModule In listModules
                Dim xmlListModule As List(Of ioModule)
                If listModules Is InputModules Then
                    xmlListModule = source.InputModules
                Else
                    xmlListModule = source.OutputModules
                End If

                Dim xmlModule As ioTable.ioModule =
                    xmlListModule.Find(Function(obj As ioTable.ioModule) (obj.Ring = _Module.Ring And obj.DeviceIP = _Module.DeviceIP))

                If xmlModule IsNot Nothing Then
                    For i = 0 To _Module.IoElements.Count - 1
                        Dim _dio As ioElement = _Module.IoElements(i)
                        Dim _xmlDio As ioElement = xmlModule.IoElements.Find(Function(obj As ioElement) (obj.AddressInIpOpForm = _dio.AddressInIpOpForm))
                        If _xmlDio IsNot Nothing Then
                            _dio.Symbol = _xmlDio.Symbol
                            _dio.Description = _xmlDio.Description
                        End If
                    Next
                End If
            Next

        Next
        Return 0
    End Function


    Class ioModule
        <XmlAttribute("Ring")> Public Property Ring As Short
        <XmlAttribute("DeviceIP")> Public Property DeviceIP As Short
        <XmlAttribute("Type")> Public Property Type As IO_MODULE_TYPE
        <XmlElement("DIO")> Public Property IoElements As New List(Of ioElement)
    End Class
    Class ioElement
        <XmlAttribute("Serial")> Public Property AddressInMemoryPoolForm As ULong '程式內用流水號    , Hsien , renamed and type redefine
        ''' <summary> 'IP/OP，如IP00, OP10等</summary>
        <XmlAttribute("Num")> Public Property AddressInIpOpForm As String 'eg , IP,OP
        <XmlAttribute("Symbol")> Public Property Symbol As String '代號，如CyA1, SpB1
        <XmlAttribute("Description")> Public Property Description As String '描述

        ''' <summary>
        ''' Combined IP/OP and Symbol
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property CellValue As String
            Get
                Return String.Format("{0},{1}",
                                           AddressInIpOpForm,
                                           Symbol)
            End Get
        End Property
        Friend relatingCell As DataGridViewCell = Nothing 'used to build one-way link

        Friend Sub refreshRelatingCell()
            With relatingCell
                If (.Value <> CellValue) Then
                    .Value = CellValue
                End If

                'status refreshing
                If (readBit(AddressInMemoryPoolForm)) Then
                    .Style.BackColor = Color.LimeGreen
                Else
                    .Style.BackColor = Control.DefaultBackColor
                End If

                .Style.SelectionBackColor = .Style.BackColor    'Hsien , the selection color is different to back color

            End With
        End Sub
    End Class
    Enum IO_MODULE_TYPE
        IP
        OP
    End Enum

End Class
