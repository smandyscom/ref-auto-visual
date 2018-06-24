Public Interface IXYRTable
    Function Connect() As enStatus
    Function MoveRel(x As Single, y As Single, r As Single) As enStatus
    Function MotionDone() As enStatus
    Function GetPosition(ByRef x As Single, ByRef y As Single, ByRef r As Single) As Integer
    ReadOnly Property PositionX As Double
    ReadOnly Property PositionY As Double
    ReadOnly Property PositionR As Double
    Enum enStatus
        DONE
        FAIL
        BUSY
    End Enum
End Interface


