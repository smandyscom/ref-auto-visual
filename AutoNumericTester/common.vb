Public Class common

    Friend Shared tolerance As Double = 0.001
    ''' <summary>
    ''' For all entities , the error should be less than tolerance
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function isInTolerance(value As Double) As Boolean
        Return value < tolerance
    End Function
End Class
