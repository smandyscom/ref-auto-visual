Imports Automation
Imports Automation.Components.CommandStateMachine

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class clsConveyorLaneMaskV2
    Inherits clsConveyorLaneMask
    Public Overrides Function initMappingAndSetup() As Object
        'remove auto add lane mask 
        MyBase.initMappingAndSetup()
        For index = 0 To Capability - 1
            LaneMask.RemoveAt(LaneMask.Count - 1)
        Next
        Return 0
    End Function
End Class
