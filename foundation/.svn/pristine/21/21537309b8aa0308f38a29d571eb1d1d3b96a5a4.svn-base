
Imports Automation
Imports Automation.Components.Services
Imports System.Reflection

Public Class userControlSystem
    Property SystemReference As systemControlPrototype
        Get
            Return __systemReference
        End Get
        Set(value As systemControlPrototype)
            If (value IsNot Nothing) Then
                __systemReference = value
                loadSystem(Me, Nothing)
            End If
        End Set
    End Property
    Protected WithEvents __systemReference As systemControlPrototype

    Private userControlControlFlags As userControlFlags = New userControlFlags()
    Private userControlStatusFlags As userControlFlags = New userControlFlags()

    Private Sub loadSystem(sender As Object, e As EventArgs) Handles MyBase.Load
        If (__systemReference Is Nothing) Then
            Exit Sub
        End If

        'System Name
        GroupBoxSystemName.Text = __systemReference.ToString()

        'loading flags into tables
        Dim collectedFlags As List(Of flagElement) = New List(Of flagElement)
        'find all flags in this instance
        Dim fields = __systemReference.GetType.GetFields(BindingFlags.Instance Or
                                                         BindingFlags.NonPublic Or
                                                         BindingFlags.Public Or BindingFlags.GetProperty)
        Dim properties = __systemReference.GetType.GetProperties(BindingFlags.Instance Or
                                                                  BindingFlags.NonPublic Or
                                                                  BindingFlags.Public)
        Dim value As Object = Nothing
        Dim collectedValues As List(Of Object) = New List(Of Object)
        For Each item As FieldInfo In fields
            value = item.GetValue(__systemReference)
            If (value IsNot Nothing) Then
                'recognize its type
                Dim fieldType As Type = value.GetType
                If (fieldType.IsGenericType) Then
                    Dim genericType As Type = fieldType.GetGenericTypeDefinition()
                    Dim firstOccurance As Integer = genericType.Name.IndexOf("`")
                    If (firstOccurance > 0 AndAlso
                        genericType.Name.Remove(firstOccurance) = "flagController") Then
                        'identified
                        value.controllerName = item.Name ' refresh current name
                        collectedValues.Add(value)
                    End If
                End If
            End If
        Next
        For Each item As PropertyInfo In properties
            value = Nothing
            If (item.CanRead And
                item.GetIndexParameters.Length = 0) Then
                value = item.GetValue(__systemReference, Nothing)
            End If
            If (value IsNot Nothing) Then
                'recognize its type
                Dim fieldType As Type = value.GetType
                If (fieldType.IsGenericType) Then
                    Dim genericType As Type = fieldType.GetGenericTypeDefinition()
                    Dim firstOccurance As Integer = genericType.Name.IndexOf("`")
                    If (firstOccurance > 0 AndAlso
                        genericType.Name.Remove(firstOccurance) = "flagController") Then
                        'identified
                        If (Not collectedValues.Exists(Function(__object As Object) (__object.Equals(value)))) Then
                            value.controllerName = item.Name ' refresh current name
                            collectedValues.Add(value)
                        End If
                    End If
                End If
            End If
        Next
        collectedValues.ForEach(Sub(__object As Object) collectedFlags.AddRange(__object.FlagElementsArray()))


        userControlControlFlags.FlagsReference = collectedFlags '__systemReference.relatedFlags
        TabPageControlFlags.Controls.Add(userControlControlFlags)
        'userControlStatusFlags.FlagsReference = __systemReference.statusFlagsReference 'status reference truncated , Hsien , 2014.01.07
        TabPageStatusFlags.Controls.Add(userControlStatusFlags)

        'link alarm user control
        UserControlAlarmObject.AlarmReference = __systemReference.CentralAlarmObject

        If (__systemReference.CentralMessenger Is Nothing) Then
            RichTextBoxMessage.Text += "Message Manager Not Linked"
        Else
            'AddHandler systemReference.CentralMessenger.MessagePoped, AddressOf Me.messagePostHandler
        End If

        '
        Me.PropertyGridSystem.SelectedObject = __systemReference

        ' load components
        '-------------------------------
        ' loading components into tables
        '-------------------------------
        TableLayoutPanelComponents.Controls.Clear()
        Dim componentsControl As UserControlComponent
        For Each comp As driveBase In __systemReference.ComponentsIncluded
            componentsControl = New UserControlComponent
            componentsControl.componentReference = comp
            TableLayoutPanelComponents.Controls.Add(componentsControl)
        Next

        'AddHandler systemReference.ProcessProgressed, AddressOf systemRefresh
        'AddHandler Me.ParentForm.FormClosing, AddressOf Me.removeRefreshHandler

    End Sub

    'Private Sub unloadSystem(sender As Object, e As EventArgs)
    '    '-------
    '    ' try to fix handler call afer handle destroyed issue
    '    '-------
    '    If (systemReference Is Nothing) Then
    '        Exit Sub
    '    End If

    '    RemoveHandler systemReference.ProcessProgressed, AddressOf Me.systemRefresh
    'End Sub

    Private Sub systemRefresh(ByVal sender As Object, ByVal e As EventArgs) Handles TimerRefresh.Tick
        'Handles TimerRefresh.Tick
        '---------------
        '   Used to refresh GUI
        '---------------
        If (Me.IsDisposed) Then
            Exit Sub
        End If

        'reject
        'If (IsReentry) Then
        '    Exit Sub
        'End If

        'IsReentry = True

        'Me.BeginInvoke(New Action(Sub()
        '                         SuspendLayout()


        Me.PropertyGridSystem.Refresh()

        '---------
        ' Flag Status Refresh
        '---------
        userControlControlFlags.Refresh()
        userControlStatusFlags.Refresh()

        For Each uc As Control In TableLayoutPanelComponents.Controls
            uc.Refresh()
        Next

        ResumeLayout()

        'IsReentry = False

        'End Sub))
    End Sub
    Private Sub controlEnter(sender As Object, e As EventArgs) Handles Me.Enter

        TimerRefresh.Enabled = True
        'AddHandler systemReference.ProcessProgressed, AddressOf systemRefresh
    End Sub
    Private Sub controlLeave(sender As Object, e As EventArgs) Handles Me.Leave
        TimerRefresh.Enabled = False
        ' avoid to handle event after uc closed

        'RemoveHandler systemReference.ProcessProgressed, AddressOf Me.systemRefresh
    End Sub

    Private Sub removeRefreshHandler(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Disposed
        'RemoveHandler systemReference.ProcessProgressed, AddressOf Me.systemRefresh
    End Sub

End Class
