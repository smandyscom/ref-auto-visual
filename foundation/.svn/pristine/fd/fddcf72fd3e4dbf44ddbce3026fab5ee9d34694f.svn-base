Imports System.Xml.Serialization
Imports System.IO
Imports System.ComponentModel
Imports Automation.utilitiesUI

<Serializable()>
Public Class shiftDataCollection
    Inherits shiftDataPackBase
    Implements IPersistance
    '--------------------------------------------------------------------------
    '   Regular the Row Data (a datapack per arm pick'n place on PECVD machine)
    '--------------------------------------------------------------------------
    <XmlIgnore()> Overridable Property DataCount As Integer
        Get
            Return DataCollection.Count
        End Get
        Set(value As Integer)
            '-----------------------------------------------
            '   Reload wafer data
            '-----------------------------------------------
            DataCollection.Clear()
            Dim index As Integer = 0
            For index = 0 To value - 1
                DataCollection.Add(Activator.CreateInstance(DataType))
            Next
        End Set
    End Property
    <XmlIgnore()> Overridable Property DataType As Type
        Get
            Return __dataType
        End Get
        Set(value As Type)
            __dataType = value
            For index = 0 To DataCount - 1
                DataCollection(index) = Activator.CreateInstance(__dataType)
            Next
        End Set
    End Property
    Overridable ReadOnly Property IsAnyRemained As Boolean
        Get
            ' scan if any wafer remain
            Return DataCollection.Exists(Function(__data As shiftDataPackBase) __data.IsPositionOccupied)
        End Get
    End Property
    Overridable ReadOnly Property IsAllOccupied As Boolean
        Get
            ' scan if any wafer remain
            Return DataCollection.TrueForAll(Function(__data As shiftDataPackBase) __data.IsPositionOccupied)
        End Get
    End Property

    Public Property DataCollection As List(Of shiftDataPackBase) = New List(Of shiftDataPackBase)
    <XmlIgnore()> Protected __dataType As Type = GetType(shiftDataPackBase)

    Overrides Function Clone() As Object
        '-------------------------------------------
        '   Full Depth copy
        '-------------------------------------------
        'Dim copy As shiftDataCollection = MemberwiseClone() 'error found , moduleAction/moduleCycleTimer linked , doesnt cloned
        '-------------------------
        '   Reference Type Deep Clone
        '-------------------------
        Dim copy As shiftDataCollection = MyBase.Clone()

        With copy
            .DataCollection = New List(Of shiftDataPackBase)        'regenerate reference
            .DataType = Me.DataType
            .DataCount = Me.DataCount
            .Assign(Me)
        End With

        Return copy

    End Function

    Public Overrides Sub Assign(source As Object)
        MyBase.Assign(source)       ' occupied status assignment
        '-------------------------------------
        '   Whole wafer data assignment
        '-------------------------------------
        With CType(source, shiftDataCollection)
            Dim index As Integer
            For index = 0 To .DataCollection.Count - 1
                'CTypeDynamic(WaferDatas(index), WaferDatas(index).GetType).Assign(source.WaferDatas(index))
                Me.DataCollection(index).Assign(.DataCollection(index))
            Next
        End With
    End Sub


#Region "Persistance interface"
    Public Overridable Sub Create(filename As String) Implements IPersistance.Create
        Me.Filename = filename  'memorize the anchor filename
        Me.Save()
    End Sub

    <XmlIgnore()>
    <Browsable(False)>
    Public Property Filename As String Implements IPersistance.Filename

    Public Overridable Sub Load(filename As String) Implements IPersistance.Load
        Try
            Using stream As FileStream = New FileStream(filename, FileMode.Open) '2015.7.23 jk add
                Dim __typeList As List(Of Type) = New List(Of Type)
                collectTypes(__typeList, Me)

                Dim tempLaneData As shiftDataCollection = utilities.getSerializer(Me.GetType, __typeList).Deserialize(stream)

                Me.Assign(tempLaneData) 'value assignment

                Me.Filename = filename  'memorize the anchor filename
            End Using

        Catch ex As Exception
            '-------------------------------------------
            '   Loading failure , may due to file broken
            '-------------------------------------------
            Throw
        End Try

    End Sub

    Public Overridable Sub Save() Implements IPersistance.Save
        '----------------------------------------------------------------------
        '   Save current status into file , filename have to be assigned before
        '----------------------------------------------------------------------
        Try
            Using stream As FileStream = New FileStream(Me.Filename, FileMode.Create, FileAccess.Write) 'create or override

                Dim __typeList As List(Of Type) = New List(Of Type)
                collectTypes(__typeList, Me)

                utilities.getSerializer(Me.GetType, __typeList).Serialize(stream, Me)

            End Using
        Catch ex As Exception
            '-------------------------------------------
            '  Saving failure , may due to file broken
            '-------------------------------------------
            Throw
        End Try

    End Sub

    Shared Function collectTypes(__list As List(Of Type), __nextCollection As shiftDataPackBase) As Integer
        Dim __collection As shiftDataCollection = TryCast(__nextCollection, shiftDataCollection)

        If (__collection IsNot Nothing) Then
            __list.Add(__collection.DataType)
            Return collectTypes(__list, __collection.DataCollection.First)
        Else
            Return 0
        End If

    End Function


#End Region

End Class


