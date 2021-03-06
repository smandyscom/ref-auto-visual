﻿Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Spatial.Euclidean
Imports MathNet.Numerics.Data.Text
Imports Automation
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Reflection
Imports System.Text
Imports System.ComponentModel

Public Enum axisEntityEnum As Integer
    X = 0
    Y = 1
    Z = 2
    A = 3
    B = 4
    C = 5
End Enum

Public Enum frameVectorEnum As Integer
    VX = 0
    VY = 1
    VZ = 2
    ''' <summary>
    ''' The Position Vector
    ''' </summary>
    ''' <remarks></remarks>
    P = 3
End Enum

Public MustInherit Class htmEdgeBase

    <XmlIgnore()>
    ReadOnly Property Name As String
        Get
            Return Me.ToString
        End Get
    End Property

    <XmlIgnore()>
    MustOverride Property RawValue As Matrix(Of Double)
    <XmlIgnore()>
    MustOverride ReadOnly Property Value As htmEdgeBase
    ''' <summary>
    ''' give a inverse
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    MustOverride ReadOnly Property Inverse As htmEdgeBase

    <Browsable(False)>
    ReadOnly Property From As [Enum]
        Get
            Return __from
        End Get
    End Property
    <Browsable(False)>
    ReadOnly Property To__ As [Enum]
        Get
            Return __to
        End Get
    End Property
    ''' <summary>
    ''' Depict which two frames this htm related
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    ReadOnly Property DoubleEnds As List(Of [Enum])
        Get
            Return New List(Of [Enum]) From {__from,
                                             __to}
        End Get
    End Property

    Protected __from As [Enum] = Nothing
    Protected __to As [Enum] = Nothing

    ''' <summary>
    ''' 3x3 Matrix
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Overridable Property RotationMatrix As Matrix(Of Double)
        Get
            Return RawValue.SubMatrix(0, 3, 0, 3)
        End Get
        Set(value As Matrix(Of Double))
            'update only for elementray
            Throw New NotImplementedException
        End Set
    End Property

    ''' <summary>
    ''' 4x1 Vector
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public Overridable Property PositionVector As Vector(Of Double)
        Get
            Return FrameVector(frameVectorEnum.P)
        End Get
        Set(ByVal value As Vector(Of Double))
            'update
            FrameVector(frameVectorEnum.P) = value
        End Set
    End Property

    ''' <summary>
    ''' 4x1 Vector
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public Property Origin As PositionVector
        Get
            Return New PositionVector(PositionVector, Me.To__, Me.ToString)
        End Get
        Set(ByVal value As PositionVector)
            'update
            FrameVector(frameVectorEnum.P) = value.RawValue
        End Set
    End Property


    ''' <summary>
    ''' 4x1 Vector
    ''' </summary>
    ''' <param name="axis"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Overridable Property FrameVector(axis As frameVectorEnum) As Vector(Of Double)
        Get
            Return RawValue.Column(axis)
        End Get
        Set(value As Vector(Of Double))
            'update
            Throw New NotImplementedException
        End Set
    End Property


    Public Overrides Function ToString() As String
        Return String.Format("T_{0}_{1}",
                             __from,
                             __to)
    End Function

    ''' <summary>
    ''' Define matrix multiplication
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Operator *(ByVal left As htmEdgeBase, right As htmEdgeBase) As htmEdgeBase
        Return New htmEdgeElementary(left.RawValue * right.RawValue,
                                     right.__from,
                                     left.__to)
    End Operator

End Class

''' <summary>
''' Decorator of Matrix(of Double) 
''' Has edge property , depict T_from_to
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class htmEdgeElementary
    Inherits htmEdgeBase
    Implements IPersistance
    Implements ICloneable

    <Browsable(False)>
    Public Overrides ReadOnly Property Inverse As htmEdgeBase
        Get
            Dim output As htmEdgeElementary = New htmEdgeElementary(Me.__to,
                                                                    Me.__from)
            output.RotationMatrix = Me.RotationMatrix.Transpose
            output.Origin = New PositionVector(-1 * output.RotationMatrix * Me.PositionVector.SubVector(0, 3), Nothing)

            Return output
        End Get
    End Property

    <Browsable(False)>
    <XmlIgnore()>
    Public Overrides Property RawValue As Matrix(Of Double)
        Get
            Return __matrixCore
        End Get
        Set(value As Matrix(Of Double))
            __matrixCore = value
        End Set
    End Property
    <Browsable(False)>
    Public Overrides ReadOnly Property Value As htmEdgeBase
        Get
            Return Me
        End Get
    End Property
    ''' <summary>
    ''' Given a 3x3 matrix
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    Public Overrides Property RotationMatrix As Matrix(Of Double)
        Get
            Return MyBase.RotationMatrix
        End Get
        Set(value As Matrix(Of Double))
            __matrixCore.SetSubMatrix(0, value.RowCount, 0, value.ColumnCount, value)
        End Set
    End Property
    ''' <summary>
    ''' Given a 4x1 column vector
    ''' </summary>
    ''' <param name="axis"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    Public Overrides Property FrameVector(axis As frameVectorEnum) As Vector(Of Double)
        Get
            Return MyBase.FrameVector(axis)
        End Get
        Set(value As Vector(Of Double))
            __matrixCore.SetSubMatrix(0, value.Count, axis, 1, value.ToColumnMatrix)
        End Set
    End Property

    ReadOnly Property Tag As Object
        Get
            Return __tag
        End Get
    End Property
    Dim __tag As Object = Nothing

    ''' <summary>
    ''' Used for xml serialization
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property MatrixText As String
        Get
            Return utilities.matrix2String(__matrixCore)
        End Get
        Set(value As String)
            __matrixCore = utilities.string2Matrix(value)
        End Set
    End Property

    Protected __matrixCore As Matrix(Of Double) = Nothing

    Sub New(__matrixCore As Matrix(Of Double),
            __from As [Enum],
            __to As [Enum],
            Optional tag As Object = Nothing)

        Me.__matrixCore = CreateMatrix.DenseIdentity(Of Double)(4)
        Me.__matrixCore.SetSubMatrix(0, 3, 0, 3, __matrixCore.SubMatrix(0, 3, 0, 3).NormalizeColumns(2))
        Me.__matrixCore.SetSubMatrix(0, 3, 3, 1, __matrixCore.SubMatrix(0, 3, 3, 1))
        'Me.__matrixCore = __matrixCore
        Me.__from = __from
        Me.__to = __to

        Me.__tag = tag
    End Sub
    Sub New(__from As [Enum],
            __to As [Enum],
            Optional tag As Object = Nothing)
        Me.New(CreateMatrix.DenseIdentity(Of Double)(4),
                   __from,
                   __to,
                   tag)        'create eye(4)
    End Sub
    ''' <summary>
    ''' Used for serializing
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
        Me.New(Nothing,
               Nothing)
    End Sub
#Region "persistance"
    <XmlIgnore()>
    Shared Property HtmDirectory As String = My.Application.Info.DirectoryPath & "/Data/htms/"
    Public Sub Create(filename As String) Implements IPersistance.Create
        Save()
    End Sub
    <XmlIgnore()>
    <Browsable(False)>
    Public Property Filename As String Implements IPersistance.Filename
        Get
            If (Not Directory.Exists(HtmDirectory)) Then
                Directory.CreateDirectory(HtmDirectory)
            End If
            Return HtmDirectory & Me.ToString() & ".xml"
        End Get
        Set(value As String)
        End Set
    End Property
    Public Overridable Sub Load(filename As String) Implements IPersistance.Load
        Try
            'override default filename , once input is valid
            If filename IsNot Nothing AndAlso
                filename.Length <> 0 Then
                Me.Filename = filename
            End If

            If (Not File.Exists(Me.Filename)) Then
                Me.Create(Nothing)
                Exit Sub
            End If

            'for non-public constructor
            Dim temp As Object = Activator.CreateInstance(Me.GetType, True)
            OpenXmlFile(temp, Me.Filename)

            'rebuild all properties
            For Each item As PropertyInfo In Me.GetType.GetProperties(BindingFlags.Public Or
                                                                 BindingFlags.Instance)
                If item.CanWrite And
                    item.GetIndexParameters.Length = 0 Then

                    item.SetValue(Me, item.GetValue(temp, Nothing), Nothing)
                End If
            Next


        Catch ex As Exception

        End Try
    End Sub
    Public Overridable Sub Save() Implements IPersistance.Save
        Try
            'DelimitedWriter.Write(Of Double)(Filename, __matrixCore, ",")
            SaveXmlFile(Me, Me.Filename)
        Catch ex As Exception

        End Try
    End Sub

#End Region

    Public Overridable Function Clone() As Object Implements ICloneable.Clone
        Return New htmEdgeElementary(Me.__matrixCore.Clone,
                                      Me.__from,
                                      Me.__to)
    End Function

    ''' <summary>
    ''' Returned all the values back to default
    ''' </summary>
    ''' <remarks></remarks>
    Sub reset()
        Me.__matrixCore = CreateMatrix.DenseIdentity(Of Double)(4)
    End Sub

End Class

''' <summary>
''' Composed by elementray htm
''' </summary>
''' <remarks></remarks>
Public Class htmEdgeComposed
    Inherits htmEdgeBase

    ReadOnly Property Chain As List(Of htmEdgeBase)
        Get
            Return __chain
        End Get
    End Property

    Public Overrides ReadOnly Property Inverse As htmEdgeBase
        Get
            Return Value.Inverse
        End Get
    End Property

    ''' <summary>
    ''' Assemble the chain
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Property RawValue As Matrix(Of Double)
        Get
            Return Value.RawValue
        End Get
        Set(value As Matrix(Of Double))
            Throw New NotImplementedException
        End Set
    End Property

    Dim __chain As List(Of htmEdgeBase) = New List(Of htmEdgeBase)

    ''' <summary>
    ''' Returned a assembled elementray htm
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property Value As htmEdgeBase
        Get
            Dim __assembledHtm As htmEdgeElementary = New htmEdgeElementary(Me.__to,
                                                                            Me.__to)
            Dim it As List(Of htmEdgeBase).Enumerator = __chain.GetEnumerator()

            While it.MoveNext()

                Dim node As htmEdgeBase = it.Current
                If Not node.To__.Equals(__assembledHtm.From) Then
                    'need inverse
                    node = node.Inverse
                    'Else
                    '    '-----------
                    '    '   Error Occured , chain is not in sequnece
                    '    '-----------
                    '    Throw New Exception("Chain is not in sequnece")
                End If

                'matrix cascade
                __assembledHtm = __assembledHtm * node

            End While

            Return __assembledHtm
        End Get
    End Property

    Sub New(__chain As List(Of htmEdgeBase),
            __from As [Enum],
            __to As [Enum])
        Me.__chain = __chain
        Me.__from = __from
        Me.__to = __to
    End Sub

End Class

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class PositionVector
    Implements IPersistance
    Implements ICloneable
    Implements IAxis
    Implements IComparable

    <XmlIgnore()>
    ReadOnly Property Quadrant As Integer
        Get
            If X <= 0 And
                Y <= 0 Then
                Return 4
            ElseIf X <= 0 And
                Y > 0 Then
                Return 2
            ElseIf X > 0 And
                Y > 0 Then
                Return 1
            Else
                Return 3
            End If
        End Get
    End Property

    <Browsable(False)>
    <XmlIgnore()>
    Public Property AxisValue(entity As axisEntityEnum) As Double Implements IAxis.AxisControlValue
        Get
            Return __vectorCore(entity)
        End Get
        Set(value As Double)
            __vectorCore(entity) = value
        End Set
    End Property
    <Browsable(False)>
    <XmlIgnore()>
    Public ReadOnly Property AxisFeedbackValue(entity As axisEntityEnum) As Double Implements IAxis.AxisFeedbackValue
        Get
            Return 0
        End Get
    End Property


    ''' <summary>
    ''' Given a 4x1 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    Property RawValue As Vector(Of Double)
        Set(value As Vector(Of Double))
            __vectorCore.SetValues(value.ToArray)
        End Set
        Get
            Return __vectorCore
        End Get
    End Property

    <XmlIgnore()>
    Property X As Double
        Get
            Return __vectorCore.Item(axisEntityEnum.X)
        End Get
        Set(value As Double)
            __vectorCore.Item(axisEntityEnum.X) = value
        End Set
    End Property
    <XmlIgnore()>
    Property Y As Double
        Get
            Return __vectorCore.Item(axisEntityEnum.Y)
        End Get
        Set(value As Double)
            __vectorCore.Item(axisEntityEnum.Y) = value
        End Set
    End Property
    <XmlIgnore()>
    Property Z As Double
        Get
            Return __vectorCore.Item(axisEntityEnum.Z)
        End Get
        Set(value As Double)
            __vectorCore.Item(axisEntityEnum.Z) = value
        End Set
    End Property
    <Browsable(False)>
    ReadOnly Property ReferencedFrame As [Enum]
        Get
            Return __referencedFrame
        End Get
    End Property
    Dim __referencedFrame As [Enum] = Nothing

    ReadOnly Property Name As String
        Get
            Return Me.ToString
        End Get
    End Property

    ''' <summary>
    ''' Used for xml serialization
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(False)>
    Property PositionText As String
        Get
            Return utilities.matrix2String(__vectorCore.ToColumnMatrix)
        End Get
        Set(value As String)
            __vectorCore = CreateVector.Dense(Of Double)(utilities.string2Matrix(value).ToColumnMajorArray)
        End Set
    End Property

    Dim __label As String = ""

    Dim __vectorCore As Vector(Of Double) = CreateVector.DenseOfArray(Of Double)({0,
                                                                                  0,
                                                                                  0,
                                                                                  1})

    Sub New(__vectorCore As Vector(Of Double),
            __referencedFrame As [Enum],
            Optional name As String = "")

        'cannot handling
        If __vectorCore.Count > 4 Then
            Throw New Exception
        Else
            For index = 0 To __vectorCore.Count - 1
                Me.__vectorCore(index) = __vectorCore(index)
            Next
            'for last element , it is always one
            Me.__vectorCore(Me.__vectorCore.Count - 1) = 1
        End If
        Me.__referencedFrame = __referencedFrame
        Me.__label = name
    End Sub
    Sub New(__referencedFrame As [Enum],
           Optional name As String = "")
        Me.New(CreateVector.DenseOfArray(Of Double)({0,
                                                     0,
                                                     0,
                                                     1}),
                                             __referencedFrame,
                                             name)
    End Sub
    Protected Sub New()
        Me.New(Nothing)
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Operator *(left As htmEdgeBase, right As PositionVector) As PositionVector
        Return New PositionVector(left.RawValue * right.RawValue,
                                  left.To__)
    End Operator
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Operator +(left As PositionVector, right As PositionVector) As PositionVector
        Return New PositionVector(left.RawValue + right.RawValue,
                                  left.ReferencedFrame)
    End Operator
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="left"></param>
    ''' <param name="right"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Operator -(left As PositionVector, right As PositionVector) As PositionVector
        Return New PositionVector(left.RawValue - right.RawValue,
                                  left.ReferencedFrame)
    End Operator

#Region "persistance"
    <XmlIgnore()>
    Shared Property PositionsDirectory As String = My.Application.Info.DirectoryPath & "/Data/ptns/"

    Public Sub Create(filename As String) Implements IPersistance.Create
        Save()
    End Sub
    <XmlIgnore()>
    <Browsable(False)>
    Public Property Filename As String Implements IPersistance.Filename
        Get
            If (Not Directory.Exists(PositionsDirectory)) Then
                Directory.CreateDirectory(PositionsDirectory)
            End If
            Return PositionsDirectory & Me.ToString()
        End Get
        Set(value As String)
            'Throw New NotImplementedException
        End Set
    End Property

    Public Sub Load(filename As String) Implements IPersistance.Load
        Try
            'override default filename , once input is valid
            If filename IsNot Nothing AndAlso
                filename.Length <> 0 Then
                Me.Filename = filename
            End If

            If (Not File.Exists(Me.Filename)) Then
                Me.Create(Nothing)
                Exit Sub
            End If

            Dim temp As PositionVector = New PositionVector
            OpenXmlFile(temp, Me.Filename)

            'rebuild all properties
            For Each item As PropertyInfo In Me.GetType.GetProperties(BindingFlags.Public Or
                                                                 BindingFlags.Instance)
                If item.CanWrite And
                    item.GetIndexParameters.Length = 0 Then

                    item.SetValue(Me, item.GetValue(temp, Nothing), Nothing)
                End If
            Next

        Catch ex As Exception

        End Try
    End Sub

    Public Sub Save() Implements IPersistance.Save
        Try
            SaveXmlFile(Me, Me.Filename)
        Catch ex As Exception

        End Try
    End Sub
#End Region

    ''' <summary>
    ''' As P_r pattern
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return String.Format("P_{0}_{1}",
                             Me.__referencedFrame,
                             Me.__label)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Clone() As Object Implements ICloneable.Clone
        Return New PositionVector(Me.__vectorCore.Clone, Me.__referencedFrame)
    End Function

    ''' <summary>
    ''' Sorting in quadrant meaning
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        With CType(obj, PositionVector)
            Return Me.Quadrant - .Quadrant
        End With
    End Function

  
End Class