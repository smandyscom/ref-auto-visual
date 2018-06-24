Imports Automation
Imports Automation.Components.Services
Imports AutoNumeric
Imports MathNet.Numerics.LinearAlgebra

Public Class userControlMainPanel

    WriteOnly Property AssemblyReference As Assembly
        Set(value As Assembly)
            __assemblyReference = value
            pauseBlockReference = value.PauseBlock
            messengerReference = value.CentralMessenger
            BasicLoadMainPanel(Me, EventArgs.Empty)
        End Set
    End Property

    Dim confirmForm As formConfirm = New formConfirm
    Dim calibrationProcedureForm As Form = New Form

    Dim WithEvents __assemblyReference As Assembly = Nothing
    Dim WithEvents pauseBlockReference As interceptObject = Nothing
    Dim WithEvents messengerReference As messageHandler = Nothing

    Dim WithEvents dryAlignerReference As energySearch = Nothing
    Dim WithEvents wetAlignerReference As energySearch = Nothing


    Dim gripperControls As List(Of Button) = New List(Of Button)

    Dim __dummyChannelLeft As channelData = Nothing
    Dim __dummyChannelRight As channelData = Nothing

    Property MaxLines As Integer = 8

    Private Sub BasicLoadMainPanel(sender As Object, e As EventArgs) Handles MyBase.Load
        If __assemblyReference Is Nothing Or
            TimerRefresh.Enabled Then
            Exit Sub
        End If

        For Each item As userControlChannelData In {UserControlChannelDataLeft,
                                                    UserControlChannelDataRight}
            Dim __coeff As Vector(Of Double) = CreateVector.Random(Of Double)(AutoNumeric.fittingMethods.fitting3DMethodsEnum.DOUBLE_PARABOLA).Normalize(2)
            __coeff = __coeff.PointwiseAbs()
            __coeff.Item(AutoNumeric.fittingMethods.coeffsDefinition.E) = -1 * Math.Abs(__coeff.Item(AutoNumeric.fittingMethods.coeffsDefinition.E)) ' be positive
            Dim __datas As List(Of Vector(Of Double)) = New List(Of Vector(Of Double))
            For x = -19 To 19
                For y = -19 To 19
                    __datas.Add(CreateVector.Dense(Of Double)({x,
                                                               y,
                                                               AutoNumeric.fittingMethods.data3D(x, y, __coeff, fittingMethods.fitting3DMethodsEnum.DOUBLE_PARABOLA)}))
                Next
            Next
            item.DataSource = New channelData(__datas)
        Next


        'message bus initialize
        With UserControlMessageMainPanel
            .messengerReference = __assemblyReference.CentralMessenger
            .IsValidToShow = userControlMessage.generateMessageFilters({AddressOf .isNonRedundantMessage,
                                                                        AddressOf Me.isAlarmOrStatusMessage})
            .MessageFormator = Function(__sender As messageHandler, __e As messagePackageEventArg) (String.Format(vbCrLf & "{0} {1}", __e.Message.TimeStamp, __e.Message.AdditionalInfo))
        End With

        '---------------------------
        '   Gripper Control
        '---------------------------
        ButtonGRIPVAC.Tag = outputAddress.GRIP_VAC
        ButtonGRIPOPEN.Tag = outputAddress.GRIP_OPEN
        ButtonDisp.Tag = outputAddress.DISP_MAN_CONTROL
        ButtonPdEnable.Tag = outputAddress.PD_EN
        ButtonLdr1.Tag = outputAddress.LSR_DR1_DIS
        ButtonLdr2.Tag = outputAddress.LSR_DR2_DIS
        ButtonSyringe.Tag = outputAddress.SYNRINGE


        gripperControls.AddRange({ButtonGRIPVAC,
                                  ButtonGRIPOPEN,
                                  ButtonDisp,
                                  ButtonPdEnable,
                                  ButtonLdr1,
                                  ButtonLdr2,
                                  ButtonSyringe})
        gripperControls.ForEach(Sub(__button As Button) __button.Text = __button.Tag.ToString)

        With DataGridViewErrorMatrix
            .DataSource = frames.Instance.ErrorMatrixs
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
            For Each item As DataGridViewColumn In .Columns
                item.Visible = False
            Next
            'except these column...
            For Each item As String In {"Name",
                                        "LastUpdateTime",
                                        "ControlVectorString"}
                .Columns(item).Visible = True
            Next
            .Columns("Name").DisplayIndex = 0 'move to very first index
            For Each item As eulerHtmTR In frames.Instance.ErrorMatrixs
                AddHandler item.ErrorContentUpdated, AddressOf updateErrorContent
            Next

            '---------------------
            '   Addin Reset Button
            '---------------------
            Dim matrixResetColumn As DataGridViewButtonColumn = New DataGridViewButtonColumn
            .Columns.Add(matrixResetColumn)
            With matrixResetColumn
                .Text = "Reset"
                .UseColumnTextForButtonValue = True
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End With
            .RowHeadersVisible = False
        End With
        With TableLayoutPanelAnalog.Controls
            .Clear()
            Assembly.Instance.analogChannels.ForEach(Sub(__item As [Enum])
                                                         Dim __monitor = New userControlAnalogMonitor
                                                         __monitor.Tag = __item
                                                         .Add(__monitor)
                                                     End Sub)
            .Add(New userControlAnalogSetter)
        End With
        With TableLayoutPanelBondMaterial.Controls
            .Clear()
            For index = 0 To Assembly.Instance.BondedMaterialData.Count - 1
                .Add(New userControlMaterial With {.Data = Assembly.Instance.BondedMaterialData(index)})
            Next

        End With

        '---------------------------------
        '   Initializing Calibration Form
        '---------------------------------
        Dim __usercontrolProcedure As userControlProcedureContextList = New userControlProcedureContextList
        With __usercontrolProcedure
            .ProcedureListReference = __assemblyReference.CalibrationProcedures
        End With
        With calibrationProcedureForm
            .Controls.Add(__usercontrolProcedure)
            .Size = New Size(320, 240)
            .StartPosition = FormStartPosition.CenterParent
        End With

        'link alinger reference
        dryAlignerReference = __assemblyReference.dryAlignProcess
        wetAlignerReference = __assemblyReference.wetAlignProcess

        TimerRefresh.Enabled = True
    End Sub
    Function isAlarmOrStatusMessage(sender As messageHandler, e As messagePackageEventArg) As Boolean
        Return e.Message.PrimaryKey.Equals(GetType(alarmGeneric)) Or e.Message.PrimaryKey.Equals(statusEnum.GENERIC_MESSAGE)
    End Function
    Sub pauseHandler() Handles pauseBlockReference.InterceptedEvent
        Me.Invoke(Sub()
                      ButtonPointTeach.Enabled = True
                      ButtonPause.BackColor = Color.Yellow
                  End Sub)
    End Sub
    Sub unpauseHandler() Handles pauseBlockReference.UninterceptedEvent
        Me.Invoke(Sub()
                      ButtonPointTeach.Enabled = False
                      ButtonPause.BackColor = DefaultBackColor
                  End Sub)
    End Sub


    Private Sub systemShutDownHandler(sender As Object, e As EventArgs) Handles __assemblyReference.SystemClosed
        Me.Invoke(Sub()

                      Try

                          confirmForm = New formConfirm()
                          confirmForm.DialogResult = DialogResult.Cancel
                          confirmForm.Message = String.Format("System Shudown , Reason : ({0:G})", e.ToString())
                          confirmForm.ButtonCancel.Enabled = False
                          If (confirmForm.ShowDialog() = DialogResult.OK) Then
                              My.Application.ApplicationContext.MainForm.Close()
                              End
                          End If

                      Catch ex As Exception

                          Throw New Exception("systemShutDownHandler(sender As Object, e As closeEvent)" + ex.Message, ex)

                      End Try

                  End Sub)
    End Sub

    Private flipStatus As Boolean = False
    Private Sub flashButton(sender As Object, e As EventArgs) Handles TimerFlash.Tick
        utilitiesUI.controlFollowBooleanColor(ButtonIgnite, flipStatus, Color.LimeGreen, SystemColors.Control)
        flipStatus = Not flipStatus
    End Sub

    Private Sub BasicButtonClick(sender As Object, e As EventArgs) Handles ButtonShutdown.Click,
        ButtonPause.Click,
        ButtonCalibration.Click,
        ButtonBonding.Click,
        ButtonFinish.Click,
        ButtonPointTeach.Click,
        ButtonIgnite.Click

        Select Case sender.Name
            '-------------
            '   System Part
            '-------------
            Case ButtonIgnite.Name
                ButtonIgnite.Enabled = False
                ButtonIgnite.BackColor = DefaultBackColor
                TimerFlash.Enabled = False
                __assemblyReference.controlFlags.setFlag(assemblyArch.controlFlagsEnum.ABLE_IGNITE)
                '---------------------------------------------------------------------------------------
                'i.e    'systemReference.unloaderMainControl.controlFlags.setFlag(unloaderSystemControl.controlFlagsEnum.ABLE_IGNITE)
                '---------------------------------------------------------------------------------------
            Case ButtonShutdown.Name
                confirmForm.DialogResult = DialogResult.Cancel
                confirmForm.Message = "Do You Really Want To Shutdown System ?"
                If (confirmForm.ShowDialog() = DialogResult.OK) Then
                    __assemblyReference.controlFlags.setFlag(assemblyArch.controlFlagsEnum.IS_ABORT_SYSTEM)
                End If
            Case ButtonPause.Name
                __assemblyReference.controlFlags.setFlag(assemblyArch.controlFlagsEnum.PAUSE_PRESSED)
            Case ButtonFinish.Name
                Dim confirmForm As formConfirm = New formConfirm With {.DialogResult = DialogResult.Cancel,
                                    .Message = "Confirm to finish auto run?", .StartPosition = FormStartPosition.CenterScreen}
                If (confirmForm.ShowDialog = DialogResult.OK) Then
                    __assemblyReference.OperationSignals.setFlag(operationSignalsEnum.__STOP)
                End If
            Case ButtonPointTeach.Name
                Dim __login As formPassword = New formPassword With {.StartPosition = FormStartPosition.CenterScreen}

                If (__login.ShowDialog = DialogResult.OK) Then
                    'rising motor setting form
                    Dim _formSetting As formSetting = New formSetting()
                    _formSetting.ShowDialog()
                    _formSetting.Dispose()
                Else
                    'cancel or incorrect , Hsien , 2015.06.16
                End If
                '----------------------------------------
                '   Procedure Control
                '----------------------------------------
            Case ButtonCalibration.Name
                'raise selection window
                calibrationProcedureForm.ShowDialog()
                __assemblyReference.WorkMode = workMode.CALIBRATION
                __assemblyReference.OperationSignals.setFlag(operationSignalsEnum.__START)

            Case ButtonBonding.Name
                __assemblyReference.WorkMode = workMode.BONDING
                __assemblyReference.OperationSignals.setFlag(operationSignalsEnum.__START)

            Case ButtonFinish.Name
                __assemblyReference.OperationSignals.setFlag(operationSignalsEnum.__STOP)

        End Select
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub ioButtonClick(sender As Button, e As EventArgs) Handles ButtonGRIPVAC.Click,
        ButtonGRIPOPEN.Click,
        ButtonDisp.Click,
        ButtonPdEnable.Click,
        ButtonLdr1.Click,
        ButtonLdr2.Click,
        ButtonSyringe.Click

        'flip
        mainIOHardware.writeBit(sender.Tag,
                                Not mainIOHardware.readBit(sender.Tag))

    End Sub


    Private Sub timerRefreshTick(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        gripperControls.ForEach(Sub(__button As Button)
                                    utilitiesUI.controlFollowBooleanColor(__button, mainIOHardware.readBit(__button.Tag),
                                                                          Color.Green,
                                                                          Control.DefaultBackColor)
                                End Sub)


        '----------------------------------
        '   Button Interlocks
        '----------------------------------
        ButtonCalibration.Enabled = (__assemblyReference.MainState = systemControlPrototype.systemStatesEnum.IDLE)
        ButtonBonding.Enabled = (__assemblyReference.MainState = systemControlPrototype.systemStatesEnum.IDLE)
        ButtonFinish.Enabled = (__assemblyReference.MainState = systemControlPrototype.systemStatesEnum.EXECUTE And
                                __assemblyReference.WorkMode = workMode.BONDING)


    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub updateErrorContent()
        Me.Invoke(Sub()
                      With DataGridViewErrorMatrix
                          .Update()
                          .Refresh()
                      End With
                  End Sub)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Sub resetMatrixContent(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewErrorMatrix.CellClick
        If e.ColumnIndex = DataGridViewErrorMatrix.ColumnCount - 1 Then
            With CType(DataGridViewErrorMatrix.DataSource, List(Of eulerHtmTR)).Item(e.RowIndex)
                .reset()
            End With
        End If
    End Sub
    ''' <summary>
    ''' Update Align Data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Sub updateAlignData(sender As Object, e As scanningDataEventArgs) Handles dryAlignerReference.DataGenerated,
        wetAlignerReference.DataGenerated

        UserControlChannelDataLeft.DataSource = e.Data.ChannelData(dataKeysDefine.VOLTAGE_LEFT)
        UserControlChannelDataRight.DataSource = e.Data.ChannelData(dataKeysDefine.VOLTAGE_RIGHT)

    End Sub

End Class
