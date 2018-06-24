Imports Automation
Imports MathNet.Numerics.LinearAlgebra
Imports AutoNumeric
Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class scanningDataEventArgs
    Inherits EventArgs

    ReadOnly Property Data As scanningData
        Get
            Return __data
        End Get
    End Property

    Dim __data As scanningData = Nothing

    Sub New(__data As scanningData)
        Me.__data = __data
    End Sub

End Class


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class rectangleSearchRouteSetting
    Inherits settingBase
    Implements IRoute

    ''' <summary>
    ''' The center position/pose
    ''' </summary>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    Property Start As htmEdgeElementary = Nothing

    ''' <summary>
    ''' In mm
    ''' </summary>
    ''' <remarks></remarks>
    Property RangeX As Double = 0.01
    Property RangeY As Double = 0.01
    Property StepX As Double = 0.0005
    Property StepY As Double = 0.0005

    Property Height As Double = 1
    Property OriginOffsetX As Double = 0
    Property OriginOffsetY As Double = 0

    Dim it As List(Of PositionVector).Enumerator = Nothing
    Dim __measurePoints As List(Of PositionVector) = Nothing

    ReadOnly Property SearchPercentage As Single
        Get
            If __measurePoints IsNot Nothing Then
                Return (__measurePoints.IndexOf(it.Current) + 1) / __measurePoints.Count
            Else
                '-----------------
                '   Not Initialized
                '-----------------
                Return 0
            End If
        End Get
    End Property

    ''' <summary>
    ''' Reset Iterator
    ''' </summary>
    ''' <remarks></remarks>
    Sub reset()
        __measurePoints = MeasurePoints
        it = __measurePoints.GetEnumerator
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    <Browsable(False)>
    ReadOnly Property NextTransformation As htmEdgeElementary
        Get
            If it.MoveNext Then

                Dim __nextTransforamtion As htmEdgeElementary = Start.Clone
                __nextTransforamtion.Origin += it.Current
                Return __nextTransforamtion

            Else
                Return Nothing
            End If
        End Get
    End Property
    <XmlIgnore()>
    <Browsable(False)>
    Public ReadOnly Property MeasurePoints As List(Of PositionVector) Implements IRoute.MeasurePoints
        Get

            Dim leftUpCorner As PositionVector = New PositionVector(Start.From) With {.X = OriginOffsetX - RangeX / 2,
                                                                                      .Y = OriginOffsetY - RangeY / 2}

            Dim output As List(Of PositionVector) = New List(Of PositionVector)
            Dim xAccumulation As Double = 0
            Dim yAccumulation As Double = 0

            While yAccumulation <= RangeY
                While xAccumulation <= RangeX
                    Dim __nextPoint As PositionVector = leftUpCorner.Clone
                    With __nextPoint
                        .X += xAccumulation
                        .Y += yAccumulation
                    End With
                    output.Add(__nextPoint)
                    xAccumulation += StepX
                End While
                xAccumulation = 0 'reset
                yAccumulation += StepY
            End While

            Return output
        End Get
    End Property

End Class




''' <summary>
''' 1. move X-stage to ready position (DIE_REAL)
''' 2. let LPC_REAL align to DIE_REAL_DRY at certain height
''' 3. xy-stage searching
''' 4. output:
'''  i  .peak value in the searching region
'''  ii. right/left beam width
'''  iii. right/left beam center 
''' 
''' 
'''after searching
'''inline requirment , the peak value should over 0.75V
'''if passed , do searching
'''after searching , do data fitting (parabolic) along X,Y , then you can get
'''left peak XY
'''right peak XY
'''do dry alignment , align peak XY and yaw
''' 
''' All X-Y coordinate referenced to DIE coordinate
''' </summary>
''' <remarks></remarks>
Public Class energySearch
    Inherits systemControlPrototype
    Implements IProcedure
    Implements IDisposable

    Public Event DataGenerated(ByVal sender As Object, ByVal e As EventArgs)

    Friend setting As rectangleSearchRouteSetting = New rectangleSearchRouteSetting

    Public Property Result As IProcedure.procedureResultEnums Implements IProcedure.Result
    ''' <summary>
    ''' Arguments
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Arguments As Object Implements IProcedure.Arguments
        Get
            Return __materialReference
        End Get
        Set(value As Object)
            __materialReference = value
        End Set
    End Property
    Public Property IsProcedureStarted As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureStarted
    Public Property IsProcedureAbort As New flagController(Of interlockedFlag) Implements IProcedure.IsProcedureAbort

    Dim __materialReference As materialData = Nothing

    Dim leftRawData As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
    Dim rightRawData As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))

    Friend dieCoordinate As htmEdgeElementary = Nothing

    Dim __dataLogger As dataLogger = Nothing

    Friend lpcDieTransformation As htmEdgeElementary = Nothing
    Friend nextTransformation As htmEdgeElementary = Nothing
    ''' <summary>
    ''' 0: Origin/Pose had been aligned in some height
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function stateExecute() As Integer

        If IsProcedureAbort.readFlag(interlockedFlag.POSITION_OCCUPIED) Then
            IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
            systemSubState = 0 'reset
        End If

        Select Case systemSubState
            Case 0
                If IsProcedureStarted.viewFlag(interlockedFlag.POSITION_OCCUPIED) Then
                    'let all axis perfectly aligned
                    'x,y coincidence , z at first stage height

                    'end of logging
                    If __dataLogger IsNot Nothing Then
                        __dataLogger.writeLine("Closed", True)
                    End If
                    __dataLogger = New dataLogger(Me.ToString) 'swap link


                    'enable pd , start scanning
                    mainIOHardware.writeBit(outputAddress.PD_EN, True)

                    'clear old data
                    leftRawData.Clear()
                    rightRawData.Clear()

                    lpcDieTransformation.Origin = New PositionVector(Nothing) With {.X = setting.OriginOffsetX,
                                                                                    .Y = setting.OriginOffsetY,
                                                                                    .Z = setting.Height}

                    With setting
                        sendMessage(internalEnum.GENERIC_MESSAGE, String.Format("Height,{0}Range,{1}Step,{2}",
                                                                                .Height & vbCrLf,
                                                                                .RangeX & vbCrLf,
                                                                                .StepX & vbCrLf))
                    End With

                    frames.Instance.solveS(lpcDieTransformation)
                    systemSubState += 10
                Else
                    '---------------------
                    '
                    '---------------------
                End If
            Case 10
                If Assembly.Instance.CommandEndStatus(controlUnitsEnum.S) =
                     IDrivable.endStatus.EXECUTION_END Then
                    'setup search center
                    setting.Start = lpcDieTransformation.Clone
                    setting.reset()

                    systemSubState += 10
                Else
                    '---------------------------
                    '   
                    '---------------------------
                End If
            Case 20
                nextTransformation = setting.NextTransformation
                If nextTransformation IsNot Nothing Then
                    frames.Instance.solveS(nextTransformation)
                    systemSubState += 10
                Else
                    '----------------------
                    '   Search finished
                    '----------------------
                    systemSubState = 500
                End If
            Case 30
                If Assembly.Instance.CommandEndStatus(controlUnitsEnum.S) =
                     IDrivable.endStatus.EXECUTION_END Then

                    leftRawData.Add(CreateVector.Dense(Of Double)({nextTransformation.Origin.X,
                                                                   nextTransformation.Origin.Y,
                                                                   mainIOHardware.readDouble(inputAddress.PD_LEFT)}))
                    rightRawData.Add(CreateVector.Dense(Of Double)({nextTransformation.Origin.X,
                                                                   nextTransformation.Origin.Y,
                                                                   mainIOHardware.readDouble(inputAddress.PD_RIGHT)}))
                    systemSubState = 20
                Else
                    '-----------------------
                    '   Settling
                    '-----------------------
                End If
            Case 500
                '-----------------
                '   Data Handling
                '-----------------
                Dim generatedData As scanningData = New scanningData(leftRawData,
                                                                     rightRawData)

                __materialReference.scanningDatas(Me.dieCoordinate.From) = generatedData


                RaiseEvent DataGenerated(Me, New scanningDataEventArgs(generatedData)) 'inform gui to update

                sendMessage(internalEnum.GENERIC_MESSAGE, __materialReference.scanningDatas(Me.dieCoordinate.From).ToString)
                'revised the coordinate system
                dieCoordinate.RawValue = dieCoordinate.RawValue * __materialReference.scanningDatas(Me.dieCoordinate.From).RevicedCoordinate



                Result = IProcedure.procedureResultEnums.SUCCESS
                IsProcedureStarted.resetFlag(interlockedFlag.POSITION_OCCUPIED)
                systemSubState = 0

        End Select

        Return 0
    End Function

    Public Sub New(__dieCoordinate As htmEdgeElementary)
        dieCoordinate = __dieCoordinate
        'T_lpcReal_Die , set as I4x4, means alignment
        lpcDieTransformation = New htmEdgeElementary(framesDefinition.LPC_REAL, __dieCoordinate.From)

        Me.systemMainStateFunctions(systemStatesEnum.EXECUTE) = AddressOf stateExecute
        systemMainState = systemStatesEnum.EXECUTE

        Me.setting.Load(String.Format("{0}{1}.xml",
                                      measureProcedureSetting.settingPath,
                                      Me.dieCoordinate.From.ToString))

    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}",
                             Me.DeviceName,
                             Me.dieCoordinate.From.ToString)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If
            setting.Save()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' Redirect Message to Data Logger
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub dataLoggerRedirector(sender As Object, e As messagePackageEventArg) Handles CentralMessenger.MessagePoped
        If e.Message.Sender.Equals(Me) AndAlso
            __dataLogger IsNot Nothing Then
            __dataLogger.writeLine(e.Message.ToString)
        End If
    End Sub

End Class
