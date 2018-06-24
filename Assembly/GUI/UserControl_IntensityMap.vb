Imports System.Drawing
Imports MathNet.Numerics.LinearAlgebra

Public Class UserControl_IntensityMap

    Private _StartPositionX As Double = 0.0
    Public Property StartPositionX As Double
        Get
            Return _StartPositionX
        End Get
        Set(value As Double)
            _StartPositionX = value
            If _gridsPanel IsNot Nothing Then
                _gridsPanel.StartX = value
                If _gridsPanel.Created Then
                    Me.Invoke(Sub() _gridsPanel.Refresh())
                End If
            End If
        End Set
    End Property

    Private _EndPositionX As Double = 10.0
    Public Property EndPositionX As Double
        Get
            Return _EndPositionX
        End Get
        Set(value As Double)
            _EndPositionX = value
            If _gridsPanel IsNot Nothing Then
                _gridsPanel.EndX = value
                If _gridsPanel.Created Then
                    Me.Invoke(Sub() _gridsPanel.Refresh())
                End If
            End If
        End Set
    End Property

    Private _StartPositionY As Double = 0.0
    Public Property StartPositionY As Double
        Get
            Return _StartPositionY
        End Get
        Set(value As Double)
            _StartPositionY = value
            If _gridsPanel IsNot Nothing Then
                _gridsPanel.StartY = value
                If _gridsPanel.Created Then
                    Me.Invoke(Sub() _gridsPanel.Refresh())
                End If
            End If
        End Set
    End Property

    Private _EndPositionY As Double = 10.0
    Public Property EndPositionY As Double
        Get
            Return _EndPositionY
        End Get
        Set(value As Double)
            _EndPositionY = value
            If _gridsPanel IsNot Nothing Then
                _gridsPanel.EndY = value
                If _gridsPanel.Created Then
                    Me.Invoke(Sub() _gridsPanel.Refresh())
                End If
            End If
        End Set
    End Property

    Private _data As List(Of Vector(Of Double))
    WriteOnly Property Data As List(Of Vector(Of Double))
        Set(value As List(Of Vector(Of Double)))
            _data = value
            If _data IsNot Nothing Then
                Try
                    _gridsPanel.MaxValue = _data.Max(Function(vec As Vector(Of Double)) vec.Item(2))
                    _gridsPanel.MinValue = _data.Min(Function(vec As Vector(Of Double)) vec.Item(2))
                    StartPositionX = _data.Min(Function(vec As Vector(Of Double)) vec.Item(0))
                    EndPositionX = _data.Max(Function(vec As Vector(Of Double)) vec.Item(0))
                    StartPositionY = _data.Min(Function(vec As Vector(Of Double)) vec.Item(1))
                    EndPositionY = _data.Max(Function(vec As Vector(Of Double)) vec.Item(1))

                    ' refresh color bar
                    _colorbar.strMax = CStr(_data.Max(Function(vec As Vector(Of Double)) vec.Item(2)))
                    _colorbar.strMin = CStr(_data.Min(Function(vec As Vector(Of Double)) vec.Item(2)))
                    _colorbar.strMid = CStr((_data.Max(Function(vec As Vector(Of Double)) vec.Item(2)) + _data.Min(Function(vec As Vector(Of Double)) vec.Item(2))) / 2)
                    Me.Invoke(Sub() _colorbar.Refresh())

                    _gridsPanel.dataList = value
                Catch ex As Exception
                End Try
            End If
        End Set
    End Property

    Private _gridsPanel As GridPanel = New GridPanel(Me.Width, Me.Height)
    Private _colorbar As ColorBar = New ColorBar(70, Me.Height - 60)

    Private Sub UserControl_IntensityMap_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _colorbar = New ColorBar(50, Me.Height - 60)
        _colorbar.Location = New Point(8, 10)
        Panel2.Controls.Add(_colorbar)

        _gridsPanel = New GridPanel(Panel1.Width, Panel1.Height)

        Panel1.Controls.Add(_gridsPanel)
        _gridsPanel.Dock = DockStyle.Fill

    End Sub
End Class

Public Class GridPanel
    Inherits Panel

    Public MaxValue As Double = 10.0
    Public MinValue As Double = 0.0

    Public leftSpace As Integer = 40
    Public rightSpace As Integer = 10
    Public upSpace As Integer = 5
    Public downSpace As Integer = 20
    Public StartX As Double = 0.0
    Public EndX As Double = 10.0
    Public StartY As Double = 0.0
    Public EndY As Double = 10.0

    Private _width As Integer
    Private _height As Integer

    Private _labelXSpace As Integer = 10
    Private _labelYSpace As Integer = 20

    Private labelX As Label = New Label()
    Private labelY As Label = New Label()

    Private _gridwidth As Integer = 0.0
    Private _gridheight As Integer = 0.0
    Private _bDraw As Boolean = False

    Private _aryData As Double(,)

    Private _dataList As List(Of Vector(Of Double))
    WriteOnly Property dataList As List(Of Vector(Of Double))
        Set(value As List(Of Vector(Of Double)))
            _dataList = value
            If _dataList IsNot Nothing Then
                drawMap()
                Me.Invoke(Sub() Me.Refresh())
            End If
        End Set
    End Property

    Public Sub New(width As Integer, height As Integer)
        MyBase.New()
        _width = width
        _height = height

        labelX.Text = "X(mm)"
        labelY.Text = "Y(mm)"
        labelX.Width = 40
        labelY.Width = 40

        labelY.Location = New Point(5, height / 2 + 10)
        labelX.Location = New Point(width / 2 + 40, height - _labelXSpace - 10)

        Me.Controls.Add(labelX)
        Me.Controls.Add(labelY)
    End Sub

    Private Sub drawMap()
        _bDraw = False
        Dim xcount As Integer = 1
        Dim ycount As Integer = 1

        Do While xcount < _dataList.Count AndAlso _dataList.Count > 1 AndAlso _dataList.Item(xcount).Item(1) = _dataList.Item(xcount - 1).Item(1)
            xcount = xcount + 1
        Loop

        If xcount = 1 Then
            Do While ycount < _dataList.Count AndAlso _dataList.Count > 1 AndAlso _dataList.Item(ycount).Item(0) = _dataList.Item(ycount - 1).Item(0)
                ycount = ycount + 1
            Loop
            xcount = _dataList.Count / ycount
        Else
            ycount = _dataList.Count / xcount
        End If

        _gridwidth = Math.Floor((Me.Width - leftSpace - rightSpace) / xcount)
        _gridheight = Math.Floor((Me.Height - downSpace - upSpace) / ycount)

        _width = _gridwidth * (xcount) + leftSpace + rightSpace + _labelYSpace
        _height = _gridheight * (ycount) + downSpace + upSpace + _labelXSpace
        Me.Invoke(Sub() Me.Refresh())

        ReDim _aryData(xcount - 1, ycount - 1)
        For j = 0 To ycount - 1
            For i = 0 To xcount - 1
                _aryData(i, j) = _dataList.Item(j * xcount + i).Item(2)
            Next
        Next
        _bDraw = True
    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        Dim brush As System.Drawing.SolidBrush = New SolidBrush(Color.White)
        e.Graphics.FillRectangle(brush, 0, 0, Me.Width, Me.Height)

        Dim pen1 As New Pen(Color.Black, 1.5)

        Dim line_left As Integer = leftSpace + _labelYSpace
        Dim line_up As Integer = upSpace
        Dim line_down As Integer = _height - downSpace - _labelXSpace
        Dim line_right As Integer = _width - rightSpace
        Dim gridplot_width As Integer = _width - leftSpace - rightSpace - _labelYSpace
        Dim gridplot_height As Integer = _height - upSpace - downSpace - _labelXSpace

        Dim format As StringFormat = New StringFormat
        format.Alignment = StringAlignment.Center
        format.LineAlignment = StringAlignment.Center

        e.Graphics.DrawLine(pen1, line_left - 1, line_up, line_left - 1, line_down)
        e.Graphics.DrawLine(pen1, line_left, line_down, line_right, line_down)

        e.Graphics.DrawString(EndY.ToString("0.0000"), Me.Font, Brushes.Black, line_left - 30, line_up + 5, format)
        e.Graphics.DrawString(((StartY + EndY) / 2).ToString("0.0000"), Me.Font, Brushes.Black, line_left - 30, line_up + gridplot_height / 2, format)
        e.Graphics.DrawString(StartY.ToString("0.0000"), Me.Font, Brushes.Black, line_left - 30, line_up + gridplot_height - 5, format)

        e.Graphics.DrawString(StartX.ToString("0.0000"), Me.Font, Brushes.Black, line_left, line_down + 15, format)
        e.Graphics.DrawString(((StartX + EndX) / 2).ToString("0.0000"), Me.Font, Brushes.Black, line_left + gridplot_width / 2 - 10, line_down + 15, format)
        e.Graphics.DrawString(EndX.ToString("0.0000"), Me.Font, Brushes.Black, line_left + gridplot_width - 15, line_down + 15, format)

        If _bDraw = True Then
            For i = 0 To _aryData.GetUpperBound(0)
                For j = 0 To _aryData.GetUpperBound(1)
                    brush.Color = SetColor(_aryData(i, j), MaxValue, MinValue, 220)
                    e.Graphics.FillRectangle(brush, leftSpace + _labelYSpace + (_gridwidth) * i, upSpace + (_gridheight) * (_aryData.GetUpperBound(1) - j), _gridwidth, _gridheight)
                Next
            Next
        End If
        brush.Dispose()
        pen1.Dispose()
        format.Dispose()
    End Sub

    Private Function SetColor(value As Double, max As Double, min As Double, maxGray As Integer) As Color
        Dim pct As Double = 0.0
        If max = min Then
            pct = 0.0
        Else
            pct = (value - min) / (max - min)
        End If
        If pct > 1 Then
            pct = 1
        ElseIf pct < 0 Then
            pct = 0
        End If

        If (pct >= 0) And (pct < 0.25) Then
            Return Color.FromArgb(0, pct * maxGray * 4, maxGray)
        ElseIf (pct >= 0.25) And (pct < 0.5) Then
            Return Color.FromArgb(0, maxGray, maxGray - (pct - 0.25) * maxGray * 4)
        ElseIf (pct >= 0.5) And (pct < 0.75) Then
            Return Color.FromArgb((pct - 0.5) * maxGray * 4, maxGray, 0)
        Else
            Return Color.FromArgb(maxGray, maxGray - (pct - 0.75) * maxGray * 4, 0)
        End If

    End Function
End Class

Public Class ColorBar
    Inherits Panel

    Public strMax As String = "1"
    Public strMin As String = "0.5"
    Public strMid As String = "0"

    Private barWidth As Integer
    Private barHeight As Integer

    Public Sub New(width As Integer, height As Integer)
        MyBase.New()
        Me.Width = width
        Me.Height = height

        barWidth = width - 30
        If barWidth <= 0 Then
            barWidth = 1
        End If
        barHeight = height
    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        'Dim brush As System.Drawing.SolidBrush = New SolidBrush(SystemColors.Control)
        Dim brush As System.Drawing.SolidBrush = New SolidBrush(Color.White)
        e.Graphics.FillRectangle(brush, 0, 0, Me.Width, Me.Height)

        Dim _brush1 As System.Drawing.Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(New Rectangle(0, 0, barWidth, Me.Height / 4), Color.FromArgb(220, 0, 0), Color.FromArgb(220, 220, 0), 90)
        Dim _brush2 As System.Drawing.Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(New Rectangle(0, Me.Height / 4 - 1, barWidth, Me.Height / 4), Color.FromArgb(220, 220, 0), Color.FromArgb(0, 220, 0), 90)
        Dim _brush3 As System.Drawing.Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(New Rectangle(0, Me.Height / 2 - 1, barWidth, Me.Height / 4), Color.FromArgb(0, 220, 0), Color.FromArgb(0, 220, 220), 90)
        Dim _brush4 As System.Drawing.Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(New Rectangle(0, 3 * Me.Height / 4 - 1, barWidth, Me.Height / 4), Color.FromArgb(0, 220, 220), Color.FromArgb(0, 0, 220), 90)

        e.Graphics.FillRectangle(_brush1, 0, 0, barWidth, CInt(Me.Height / 4))
        e.Graphics.FillRectangle(_brush2, 0, CInt(Me.Height / 4), barWidth, CInt(Me.Height / 4))
        e.Graphics.FillRectangle(_brush3, 0, CInt(Me.Height / 2), barWidth, CInt(Me.Height / 4))
        e.Graphics.FillRectangle(_brush4, 0, CInt(3 * Me.Height / 4), barWidth, CInt(Me.Height / 4))

        brush.Dispose()
        _brush1.Dispose()
        _brush2.Dispose()
        _brush3.Dispose()
        _brush4.Dispose()

        e.Graphics.DrawString(strMax, Me.Font, Brushes.Black, barWidth + 2, 0)
        e.Graphics.DrawString(strMin, Me.Font, Brushes.Black, barWidth + 2, barHeight - 10)
        e.Graphics.DrawString(strMid, Me.Font, Brushes.Black, barWidth + 2, barHeight / 2 - 5)

        brush.Dispose()
        _brush1.Dispose()
        _brush2.Dispose()
        _brush3.Dispose()
        _brush4.Dispose()
    End Sub

End Class

