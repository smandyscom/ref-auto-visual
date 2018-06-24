Imports MathNet.Numerics.LinearAlgebra
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports MathNet.Numerics.Data.Text


Public Module MatrixOperation

    Public Sub test()
        Dim mat_A As Matrix(Of Double)
        Dim mat_B As Matrix(Of Double)
        Dim mat_C As Matrix(Of Double)
        Dim vec_A1 As Vector(Of Double)
        Dim vec_A2 As Vector(Of Double)
        Dim vec_A3 As Vector(Of Double)

        Dim array_Temp As Double(,) = {{1.0, 2.0}, {3.0, 4.0}}
        Dim mat1 As Matrix(Of Double)
        mat1 = CreateMatrix.DenseOfArray(Of Double)(array_Temp)

        '
        vec_A1 = CreateVector.Random(Of Double)(3)
        vec_A2 = CreateVector.Random(Of Double)(3)
        vec_A3 = CreateVector.Random(Of Double)(3)

        '       | Ve_A1(0) Ve_A2(0) Ve_A3(0) |
        ' A =   | Ve_A1(1) Ve_A2(1) Ve_A3(1) |
        '       | Ve_A1(2) Ve_A2(2) Ve_A3(2) |
        mat_A = CreateMatrix.DenseOfColumnVectors(Of Double)(vec_A1, vec_A2, vec_A3)

        '       | Ve_A1(0) Ve_A1(1) Ve_A1(2) |
        ' B =   | Ve_A2(0) Ve_A2(1) Ve_A2(2) |
        '       | Ve_A3(0) Ve_A3(1) Ve_A3(2) |
        mat_B = CreateMatrix.DenseOfRowVectors(Of Double)(vec_A1, vec_A2, vec_A3)

        '       | 2.0  2.0  2.0 |
        ' C =   | 2.0  2.0  2.0 |
        '       | 2.0  2.0  2.0 |
        mat_C = CreateMatrix.Dense(Of Double)(3, 3, 2.0)

        ' B = A + C
        mat_B = mat_A.Add(mat_C)

        '       | 0  3  6 |
        ' D =   | 1  4  7 |
        '       | 2  5  8 |
        Dim mat_D As Matrix(Of Double) = CreateMatrix.Dense(Of Double)(3, 3, {0, 1, 2, 3, 4, 5, 6, 7, 8})
        mat_D = Matrix(Of Double).Build.Dense(3, 3, {0, 1, 2, 3, 4, 5, 6, 7, 8})     ' the same

        '                   |  0   3   6 |
        ' E =|    D   | =   |  1   4   7 |
        '    | newRow |     |  2   5   8 |
        '                   | 10  20  30 |
        Dim newRow As Matrix(Of Double) = Matrix(Of Double).Build.Dense(1, 3, {10, 20, 30})
        Dim mat_E As Matrix(Of Double) = mat_D.Stack(newRow)

        '                        |  0   3   6  10 |
        ' F =| D   newCol |  =   |  1   4   7  20 |
        '                        |  2   5   8  30 |
        Dim newCol As Matrix(Of Double) = Matrix(Of Double).Build.Dense(3, 1, {10, 20, 30})
        Dim mat_F As Matrix(Of Double) = mat_D.Append(newCol)

        '       |  0   1   2 |
        ' FT =  |  3   4   5 |
        '       |  6   7   8 |
        '       | 10  20  30 |
        Dim mat_F_T As Matrix(Of Double) = mat_F.Transpose()

        '                 |  5   0   0 |
        ' mat_Diagonal =  |  0   5   0 |
        '                 |  0   0   5 |                
        Dim mat_Diagonal As Matrix(Of Double) = Matrix(Of Double).Build.Diagonal(3, 3, 5.0)
        Dim mat_Inv As Matrix(Of Double) = mat_Diagonal.Inverse()

        Dim mat_G As Matrix(Of Double) = Matrix(Of Double).Build.Dense(3, 3, {1, 0, 0, 0, 2, 0, 0, 0, 5})
        Dim mat_G_inv As Matrix(Of Double) = mat_G.Inverse()

        ' 
        Dim mat_F_pseudoInv As Matrix(Of Double) = PseudoInverse(mat_F)

        ' H = D x F
        'Dim mat_H As Matrix(Of Double) = mat_D.Multiply(mat_F)
        Dim mat_H As Matrix(Of Double) = mat_D * mat_F

        Dim mat_I As Matrix(Of Double) = RotateTransformation(Math.PI / 2, 0, 0)

        Dim mat_J As Matrix(Of Double) = mat_I * TranslateTransformation(0, 50, 0)

        Dim ttt As CalibrationChain = New CalibrationChain
        ttt.initialize()
    End Sub


    Public Function PseudoInverse(matrice As Matrix(Of Double)) As Matrix(Of Double)
        ' Reference: https://github.com/mathnet/mathnet-numerics/issues/432

        Dim svd As Factorization.Svd(Of Double) = matrice.Svd(True)
        Dim mat_W As Matrix(Of Double) = svd.W
        Dim vec_s As Vector(Of Double) = svd.S
        Dim torlerance As Double = Math.Max(matrice.RowCount, matrice.ColumnCount) * matrice.L2Norm * MathNet.Numerics.Precision.DoublePrecision

        For Each si In vec_s
            If si < torlerance Then
                si = 0
            Else
                si = 1 / si
            End If
        Next

        mat_W.SetDiagonal(vec_s)

        Return (svd.U * mat_W * svd.VT).Transpose()
    End Function

    '               |  1     0      0     0  |
    ' T_rotateX =   |  0  cos(a) -sin(a)  0  |
    '               |  0  sin(a)  cos(a)  0  |
    '               |  0     0      0     1  |

    '               |  cos(b)  0  sin(b)  0  |
    ' T_rotateY =   |    0     1    0     0  |
    '               | -sin(b)  0  cos(b)  0  |
    '               |    0     0    0     1  |

    '               |  cos(c) -sin(c)  0  0  |
    ' T_rotateZ =   |  sin(c)  cos(c)  0  0  |
    '               |    0       0     1  0  |
    '               |    0       0     0  1  |
    Public Function RotateTransformation(theta_x As Double, theta_y As Double, theta_z As Double) As Matrix(Of Double)
        Dim mat_rotateX As Matrix(Of Double) = Matrix(Of Double).Build.Dense(4, 4, {1, 0, 0, 0, 0, Math.Cos(theta_x), Math.Sin(theta_x), 0, 0, -Math.Sin(theta_x), Math.Cos(theta_x), 0, 0, 0, 0, 1})
        Dim mat_rotateY As Matrix(Of Double) = Matrix(Of Double).Build.Dense(4, 4, {Math.Cos(theta_y), 0, -Math.Sin(theta_y), 0, 0, 1, 0, 0, Math.Sin(theta_y), 0, Math.Cos(theta_y), 0, 0, 0, 0, 1})
        Dim mat_rotateZ As Matrix(Of Double) = Matrix(Of Double).Build.Dense(4, 4, {Math.Cos(theta_z), Math.Sin(theta_z), 0, 0, -Math.Sin(theta_z), Math.Cos(theta_z), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1})

        Return mat_rotateX * mat_rotateY * mat_rotateZ
    End Function

    '                  |  1  0  0  tx  |
    ' T_translation =  |  0  1  0  ty  |
    '                  |  0  0  1  tz  |
    '                  |  0  0  0   1  |
    Public Function TranslateTransformation(trans_x As Double, trans_y As Double, trans_z As Double) As Matrix(Of Double)
        Return Matrix(Of Double).Build.Dense(4, 4, {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, trans_x, trans_y, trans_z, 1})
    End Function

    ''' <summary>
    ''' Inverse Kinematics of smarpod with Pivot mode relative
    ''' </summary>
    ''' <param name="htm"> (4x4) homogenous transformation matrix</param>
    ''' <returns> (6x1) matrix</returns>
    ''' <remarks></remarks>
    Public Function smarpod_relative_inverse(htm As Matrix(Of Double)) As Matrix(Of Double)
        Dim x, y, z, theta_x, theta_y, theta_z As Double
        x = htm(0, 3)
        y = htm(1, 3)
        z = htm(2, 3)
        theta_x = Math.Atan2(htm(2, 1), htm(2, 2))
        theta_y = Math.Asin(-htm(2, 0))
        theta_z = Math.Atan2(htm(1, 0), htm(0, 0))

        Return Matrix(Of Double).Build.Dense(6, 1, {x, y, z, theta_x, theta_y, theta_z})
    End Function

    ''' <summary>
    ''' Forward Kinematics of smarpod with Pivot mode relative
    ''' </summary>
    ''' <param name="mat_pos"> (6x1) matrix of smarpod posture</param>
    ''' <returns> (4x4) htm of smarpod </returns>
    ''' <remarks></remarks>
    Public Function smarpod_relative_forward(mat_pos As Matrix(Of Double)) As Matrix(Of Double)
        Return RotateTransformation(mat_pos(3, 0), mat_pos(4, 0), mat_pos(5, 0)) * TranslateTransformation(mat_pos(0, 0), mat_pos(1, 0), mat_pos(2, 0))
    End Function
    Public Function smarpod_relative_forward(x As Double, y As Double, z As Double, theta_x As Double, theta_y As Double, theta_z As Double) As Matrix(Of Double)
        Return RotateTransformation(theta_x, theta_y, theta_z) * TranslateTransformation(x, y, z)
    End Function

    ''' <summary>
    ''' Forward Kinematics of smarpod with Pivot mode fixed
    ''' </summary>
    ''' <param name="mat_pos">(6x1) matrix of smarpod posture</param>
    ''' <returns> (4x4) htm of smarpod </returns>
    ''' <remarks></remarks>
    Public Function smarpod_fixed_forward(mat_pos As Matrix(Of Double)) As Matrix(Of Double)
        Return TranslateTransformation(mat_pos(0, 0), mat_pos(1, 0), mat_pos(2, 0)) * RotateTransformation(mat_pos(3, 0), mat_pos(4, 0), mat_pos(5, 0))
    End Function
    Public Function smarpod_fixed_forward(x As Double, y As Double, z As Double, theta_x As Double, theta_y As Double, theta_z As Double) As Matrix(Of Double)
        Return TranslateTransformation(x, y, z) * RotateTransformation(theta_x, theta_y, theta_z)
    End Function

    Public Function LoadMatrix(strFileName As String) As Matrix(Of Double)
        Try
            Return DelimitedReader.Read(Of Double)(strFileName, False, ",")
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try        
        Return Nothing
    End Function

    Public Function SaveMatrix(strFileName As String, mat As Matrix(Of Double)) As Boolean
        Try
            DelimitedWriter.Write(Of Double)(strFileName, mat, ",")
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return False
    End Function

End Module

