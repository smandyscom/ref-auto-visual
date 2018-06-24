Imports MathNet.Numerics.LinearAlgebra
Imports System.Runtime.CompilerServices
Imports System.IO
Imports MathNet.Numerics.Data.Text
Imports System.Math
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class utilities

    ''' <summary>
    ''' 3x3 Matrix
    '''               |  1     0      0     |
    ''' T_rotateX =   |  0  cos(a) -sin(a)  |
    '''               |  0  sin(a)  cos(a)  |
    ''' 
    '''               |  cos(b)  0  sin(b)  |
    ''' T_rotateY =   |    0     1    0     |
    '''               | -sin(b)  0  cos(b)  |
    ''' 
    '''               |  cos(c) -sin(c)  0  |
    ''' T_rotateZ =   |  sin(c)  cos(c)  0  |
    '''               |    0       0     1  |
    ''' </summary>
    ''' <param name="theta_x"></param>
    ''' <param name="theta_y"></param>
    ''' <param name="theta_z"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RotateTransformation(theta_x As Double, theta_y As Double, theta_z As Double) As Matrix(Of Double)
        Dim mat_rotateX As Matrix(Of Double) = CreateMatrix.DenseOfRowArrays(Of Double)({1, 0, 0},
                                                                                        {0, Cos(theta_x), -Sin(theta_x)},
                                                                                        {0, Sin(theta_x), Cos(theta_x)})

        Dim mat_rotateY As Matrix(Of Double) = CreateMatrix.DenseOfRowArrays(Of Double)({Cos(theta_y), 0, Sin(theta_y)},
                                                                                        {0, 1, 0},
                                                                                        {-Sin(theta_y), 0, Cos(theta_y)})

        Dim mat_rotateZ As Matrix(Of Double) = CreateMatrix.DenseOfRowArrays(Of Double)({Cos(theta_z), -Sin(theta_z), 0},
                                                                                        {Sin(theta_z), Cos(theta_z), 0},
                                                                                        {0, 0, 1})
        Return mat_rotateX * mat_rotateY * mat_rotateZ
    End Function


    ''' <summary>
    ''' Error is defined by nominal-real
    ''' Given Nominal Postion , turns into Error Gain Form
    ''' { -1, 0, 0,0, -.Z, .Y,
    ''' 0, -1, 0,.Z, 0, -.X, 
    ''' 0, 0, -1,-.Y, .X, 0, }
    ''' </summary>
    ''' <param name="nominalPostion"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function position2ErrorGain(nominalPostion As PositionVector) As Matrix(Of Double)
        With nominalPostion


            Return CreateMatrix.DenseOfRowArrays(Of Double)({-1, 0, 0, 0, -.Z, .Y},
                                                            {0, -1, 0, .Z, 0, -.X},
                                                            {0, 0, -1, -.Y, .X, 0})

        End With
    End Function
    ''' <summary>
    ''' Error is defined by nominal-real
    ''' Given Nominal Postion , turns into Error Gain Form
    ''' According to nominal position's valid dimension
    ''' </summary>
    ''' <param name="nominalPostion"></param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function position2ErrorGain(nominalPostion As Vector(Of Double)) As Matrix(Of Double)
        Dim __matrix = position2ErrorGain(New PositionVector(nominalPostion,
                                                              Nothing))

        Return __matrix.SubMatrix(0, nominalPostion.Count, 0, __matrix.ColumnCount)
    End Function

    Public Enum selectionEnums As Integer
        X = &H1
        Y = &H2
        Z = &H4
    End Enum
    ''' <summary>
    ''' Select the correspond error gain row
    ''' </summary>
    ''' <param name="nominalPostion"></param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function position2ErrorGain(nominalPostion As Vector(Of Double),
                                              selection As selectionEnums) As Matrix(Of Double)

        Dim __matrix = position2ErrorGain(New PositionVector(nominalPostion,
                                                              Nothing))

        Dim __vectorCollection As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
        Dim values = [Enum].GetValues(GetType(selectionEnums))

        For index = 0 To values.Length - 1
            If selection And values(index) Then
                __vectorCollection.Add(__matrix.Row(index))
            End If
        Next

        Return CreateMatrix.DenseOfRowVectors(Of Double)(__vectorCollection.ToArray)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nominalPosition"></param>
    ''' <param name="errorVector"></param>
    ''' <returns> Jacobian matrix</returns>
    ''' <remarks></remarks>
    Public Shared Function position2ErrorGain(nominalPosition As Vector(Of Double),
                                              errorVector As Vector(Of Double)) As Matrix(Of Double)

        Dim x As Double = nominalPosition(axisEntityEnum.X)
        Dim y As Double = nominalPosition(axisEntityEnum.Y)
        Dim z As Double = nominalPosition(axisEntityEnum.Z)

        Dim cx As Double = Cos(errorVector(axisEntityEnum.A))
        Dim sx As Double = Sin(errorVector(axisEntityEnum.A))

        Dim cy As Double = Cos(errorVector(axisEntityEnum.B))
        Dim sy As Double = Sin(errorVector(axisEntityEnum.B))

        Dim cz As Double = Cos(errorVector(axisEntityEnum.C))
        Dim sz As Double = Sin(errorVector(axisEntityEnum.C))

        Dim a03 As Double = -y * (sx * sz + sy * cx * cz) + z * (sz * sy * cz - sz * cx)
        Dim a04 As Double = x * sy - y * sx * cy - z * cx * cy * cz
        Dim a05 As Double = x * sz * cy + y * (sx * sy * sz + cx * cz) - z * (sx * cz - sy * sz * cx)

        Dim a13 As Double = y * (sx * cz - sy * sz * cx) + z * (sx * sy * sz + cx * cz)
        Dim a14 As Double = x * sy - y * sx * cy - z * cx * cy * sz
        Dim a15 As Double = -x * cy * cz - y * (sx * sy * cz - sz * cx) - z * (sx * sz + cx * sy * cz)

        Dim a23 As Double = (-y * cx + z * sx) * cy
        Dim a24 As Double = x * cy + y * sx * sy + z * sy * cx

        Return CreateMatrix.DenseOfRowArrays(Of Double)({-1, 0, 0, a03, a04, a05},
                                                        {0, -1, 0, a13, a14, a15},
                                                        {0, 0, -1, a23, a24, 0})


    End Function


    Public Shared Function matrix2String(value As Matrix(Of Double)) As String
        Dim sw As StringWriter = New StringWriter()
        DelimitedWriter.Write(Of Double)(sw, value, ",")
        Return sw.ToString
    End Function
    Public Shared Function string2Matrix(value As String) As Matrix(Of Double)
        Dim sr As StringReader = New StringReader(value)
        Return DelimitedReader.Read(Of Double)(sr, False, ",")
    End Function

End Class
