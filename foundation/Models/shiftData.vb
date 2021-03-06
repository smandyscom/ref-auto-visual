﻿Imports Automation
Imports Automation.Components.Services
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Reflection

<Serializable()>
Public Class shiftDataPackBase
    Implements ICloneable
    Implements IValueAssignable
    
    '例如 每片wafer身上所帶的資料
    ' this class is able to be serialized , Hsien , 2015.02.03
    'due to value type in queue cannot assignment issue
    <Browsable(False)>
    Property IsPositionOccupied As Boolean = False
    <XmlIgnore()>
    <NonSerialized()>
    Public ModuleAction As flagController(Of interlockedFlag) = New flagController(Of interlockedFlag)
    <XmlIgnore()>
    <NonSerialized()>
    Public ModuleCycleTimer As singleTimer = New singleTimer

    Property DataId As String = "default"  'Hsien , 2017.06.19 , set as base facility

    '--------------------------------------------------------
    '   This type is supported clone operation (shallow copy
    '---------------------------------------------------------
    Public Overridable Function Clone() As Object Implements ICloneable.Clone
        '----------------------
        '   Hsien , 2015.01.13 , the reference type should do the deep clone
        '----------------------
        Dim copy As shiftDataPackBase = MemberwiseClone() '複製所有實值型別
        '以下要自行加入的參考型別複製
        With copy
            .ModuleAction = New flagController(Of interlockedFlag)
            .ModuleAction.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            .ModuleCycleTimer = New singleTimer()
        End With

        Return copy
    End Function

    Public Overridable Sub Assign(source As Object) Implements IValueAssignable.Assign
        '----------------------
        '   Hsien , 2015.02.03
        '----------------------
        With DirectCast(source, shiftDataPackBase)
            Me.IsPositionOccupied = .IsPositionOccupied
            Me.ModuleAction.writeFlag(interlockedFlag.POSITION_OCCUPIED,
                                      .ModuleAction.viewFlag(interlockedFlag.POSITION_OCCUPIED))
            Me.DataId = .DataId
        End With

    End Sub

    <XmlIgnore()>
    <Browsable(False)>
    Overridable ReadOnly Property Description As String
        Get
            Return Me.ToString
        End Get
    End Property

End Class


Public Interface IValueAssignable
    '----------------------------------
    '   Value could be assigned from source instance to target instance
    '----------------------------------
    Sub Assign(ByVal source As Object)
End Interface


Public Class shiftDataEventArgs
    Inherits EventArgs

    ReadOnly Property Data As shiftDataPackBase
        Get
            Return __data
        End Get
    End Property

    Dim __data As shiftDataPackBase = Nothing

    Sub New(__data As shiftDataPackBase)
        Me.__data = __data
    End Sub

End Class