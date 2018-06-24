Imports System.Drawing.Design
Imports System.Windows.Forms.PropertyGridInternal
Imports System.Windows.Forms.Design
Imports System.ComponentModel
Public Class MotorPointTypeEditor : Inherits UITypeEditor

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object
        Dim formService As IWindowsFormsEditorService = provider.GetService(GetType(IWindowsFormsEditorService))

        'pass value-reference to editorForm , then do editing
        Dim __form As Form = New Form()
        __form.Controls.Add(New PropertyGrid() With {.SelectedObject = value,
                                                   .Dock = DockStyle.Fill})
        formService.ShowDialog(__form)

        'after editing , return the edited object to caller
        Return value

    End Function
  
    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function


End Class
