Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FA

<TestClass()> Public Class settingTest

    <TestMethod()> Public Sub TestMethod1()

        Dim setting As cameraCalibrationSettingRectangle = New cameraCalibrationSettingRectangle(framesDefinition.C1REAL, framesDefinition.C1)

        setting.XCounts = 500

        setting.Save()

        setting = New cameraCalibrationSettingRectangle(framesDefinition.C1REAL, framesDefinition.C1)

        setting.Load(Nothing)

        Assert.IsTrue(setting.XCounts = 500)

    End Sub

End Class