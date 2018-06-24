Imports Automation

Public Class sensorAreaDatabase
    Implements IMessageQuery

    Dim __dictionary As Dictionary(Of ULong, String) = New Dictionary(Of ULong, String)

    Sub New()
        '__dictionary.add(inputAddress.Start,"LabelArea人機")
        '    __dictionary.Add(inputAddress.Pause1, "LabelArea人機")
        '    __dictionary.Add(inputAddress.PB_End, "LabelArea人機")
        '    __dictionary.Add(inputAddress.Air, "LabelArea人機")
        '    '
        '    '
        '    '
        '    '
        '    __dictionary.Add(inputAddress.SpA1A, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1B, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1C, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1D, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1H, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1I, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1J, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1K, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1L, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1M, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1N, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1O, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1P, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1Q, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SpA1W, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.VsA1D, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.LoadPause, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.alarm, "LabelAreaB1")
        '    __dictionary.Add(inputAddress.VsA1A, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.VsA1B, "LabelAreaA1")
        '    '
        '    '
        '    '
        '    '
        '    __dictionary.Add(inputAddress.A1a0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1a1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1b0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1b1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1c0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1c1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1d0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.A1d1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.B1a0, "LabelAreaB1")
        '    __dictionary.Add(inputAddress.B1a1, "LabelAreaB1")
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    __dictionary.Add(inputAddress.PVA_READY, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_BUSY, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_DATAOK, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_INS_TEST, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_RESULT0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_RESULT1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_RESULT2, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.PVA_RESULT3, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_READY, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_BUSY, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_DATAOK, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_RESULT0, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_RESULT1, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_RESULT2, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_RESULT3, "LabelAreaA1")
        '    __dictionary.Add(inputAddress.SAW_INS_TEST, "LabelAreaA1")
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    '
        '    __dictionary.Add(inputAddress.BPD_In1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.BPD_InM1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.BPD_Out1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.BPD_OutM1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.BPD1_Pause, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SxD1A, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SxD1B, "LabelAreaD1")
        '    '
        '    __dictionary.Add(inputAddress.SpD1A, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1B, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1C, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1D, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1E, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1F, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1G, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1H, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1I, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1J, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1K, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1L, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SpD1M, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SPD1N, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SPD1O, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.SPD1P, "LabelAreaD1")
        '    '
        '    __dictionary.Add(inputAddress.SpE1B, "LabelAreaE1")
        '    __dictionary.Add(inputAddress.SpE1C, "LabelAreaE1")
        '    __dictionary.Add(inputAddress.SpE1D, "LabelAreaE1")
        '    __dictionary.Add(inputAddress.SpE1E, "LabelAreaE1")
        '    '
        '    '
        '    '
        '    __dictionary.Add(inputAddress.D1a0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1a1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1b0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1b1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1c0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1c1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1d0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1d1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1e0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1e1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1f0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1f1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1g0, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.D1g1, "LabelAreaD1")
        '    __dictionary.Add(inputAddress.E1a0, "LabelAreaE1")
        '    __dictionary.Add(inputAddress.E1a1, "LabelAreaE1")

    End Sub

    Public Function query(keyChain As Object) As Object Implements IMessageQuery.query
        Return __dictionary(keyChain)
    End Function
End Class

Public Class sensorDetailDatabase
    Implements IMessageQuery

    Dim __dictionary As Dictionary(Of ULong, String) = New Dictionary(Of ULong, String)



    Sub New()
        '__dictionary.add(inputAddress.Start,"人機啟動鈕,IP00,R1-0-0")
        '   __dictionary.Add(inputAddress.Pause1, "人機暫停鈕,IP01,R0-0-1")
        '   __dictionary.Add(inputAddress.PB_End, "人機結束鈕,IP02,R0-0-2")
        '   __dictionary.Add(inputAddress.Air, "人機氣壓源檢知,IP03,R0-0-3")
        '   '
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.SpA1A, "A1區手臂放置點,IP10,R0-0-8")
        '   __dictionary.Add(inputAddress.SpA1B, "A1區通過,IP11,R0-0-9")
        '   __dictionary.Add(inputAddress.SpA1C, "A1區Buffer內,IP12,R0-0-10")
        '   __dictionary.Add(inputAddress.SpA1D, "A1區片有無,IP13,R0-0-11")
        '   __dictionary.Add(inputAddress.SpA1H, "A1區slow down,IP14,R0-0-12")
        '   __dictionary.Add(inputAddress.SpA1I, "A1區抓取區片子有無,IP15,R0-0-13")
        '   __dictionary.Add(inputAddress.SpA1J, "A1區抓取區片子有無,IP16,R0-0-14")
        '   __dictionary.Add(inputAddress.SpA1K, "A1區抓取區片子有無,IP17,R0-0-15")
        '   __dictionary.Add(inputAddress.SpA1L, "A1區抓取區片子有無,IP20,R0-0-16")
        '   __dictionary.Add(inputAddress.SpA1M, "A1區抓取區片子有無,IP21,R0-0-17")
        '   __dictionary.Add(inputAddress.SpA1N, "A1區片子有無,IP22,R0-0-18")
        '   __dictionary.Add(inputAddress.SpA1O, "A1區片子有無,IP23,R0-0-19")
        '   __dictionary.Add(inputAddress.SpA1P, "A1區片子有無,IP24,R0-0-20")
        '   __dictionary.Add(inputAddress.SpA1Q, "A1區片子有無,IP25,R0-0-21")
        '   __dictionary.Add(inputAddress.SpA1W, "A1區疊片檢知,IP26,R0-0-22")
        '   __dictionary.Add(inputAddress.VsA1D, "A1區NG手檢知,IP27,R0-0-23")
        '   __dictionary.Add(inputAddress.LoadPause, "A1區下料命令暫停,IP30,R0-0-24")
        '   __dictionary.Add(inputAddress.alarm, "B1區主設備報警,IP31,R0-0-25")
        '   __dictionary.Add(inputAddress.VsA1A, "A1區秤重手檢知,IP32,R0-0-26")
        '   __dictionary.Add(inputAddress.VsA1B, "A1區秤重手檢知,IP33,R0-0-27")
        '   '
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.A1a0, "A1區上檢知,IP40,R0-1-0")
        '   __dictionary.Add(inputAddress.A1a1, "A1區下檢知,IP41,R0-1-1")
        '   __dictionary.Add(inputAddress.A1b0, "A1區上檢知,IP42,R0-1-2")
        '   __dictionary.Add(inputAddress.A1b1, "A1區下檢知,IP43,R0-1-3")
        '   __dictionary.Add(inputAddress.A1c0, "A1區磅秤蓋開,IP44,R0-1-4")
        '   __dictionary.Add(inputAddress.A1c1, "A1區磅秤蓋蓋,IP45,R0-1-5")
        '   __dictionary.Add(inputAddress.A1d0, "A1區NG上檢知,IP46,R0-1-6")
        '   __dictionary.Add(inputAddress.A1d1, "A1區NG下檢知,IP47,R0-1-7")
        '   __dictionary.Add(inputAddress.B1a0, "B1區抓手氣缸上檢知,IP50,R0-1-8")
        '   __dictionary.Add(inputAddress.B1a1, "B1區抓手氣缸下檢知,IP51,R0-1-9")
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.SpC1A, "C1區入料輸送帶檢知,IP60,R0-1-16")
        '   __dictionary.Add(inputAddress.SpC1B, "C1區入料輸送帶檢知,IP61,R0-1-17")
        '   __dictionary.Add(inputAddress.SpC1C, "C1區入料輸送帶檢知,IP62,R0-1-18")
        '   __dictionary.Add(inputAddress.SpC1D, "C1區入料輸送帶檢知,IP63,R0-1-19")
        '   __dictionary.Add(inputAddress.SpC1E, "C1區入料輸送帶檢知,IP64,R0-1-20")
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.SxF1, "門禁前門1,IP70,R0-1-24")
        '   __dictionary.Add(inputAddress.SxF2, "門禁前門2,IP71,R0-1-25")
        '   __dictionary.Add(inputAddress.SxR1, "門禁右門1,IP72,R0-1-26")
        '   __dictionary.Add(inputAddress.SxL1, "門禁右門2,IP73,R0-1-27")
        '   __dictionary.Add(inputAddress.SxB1, "門禁右門3,IP74,R0-1-28")
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.PVA_READY, "A1區PVA_READY,IP80,R0-2-0")
        '   __dictionary.Add(inputAddress.PVA_BUSY, "A1區PVA_BUSY,IP81,R0-2-1")
        '   __dictionary.Add(inputAddress.PVA_DATAOK, "A1區PVA_DATAOK,IP82,R0-2-2")
        '   __dictionary.Add(inputAddress.PVA_INS_TEST, "A1區PVA_INS_TEST,IP83,R0-2-3")
        '   __dictionary.Add(inputAddress.PVA_RESULT0, "A1區PVA_RESULT0,IP84,R0-2-4")
        '   __dictionary.Add(inputAddress.PVA_RESULT1, "A1區PVA_RESULT1,IP85,R0-2-5")
        '   __dictionary.Add(inputAddress.PVA_RESULT2, "A1區PVA_RESULT2,IP86,R0-2-6")
        '   __dictionary.Add(inputAddress.PVA_RESULT3, "A1區PVA_RESULT3,IP87,R0-2-7")
        '   __dictionary.Add(inputAddress.SAW_READY, "A1區SAW_READY,IP90,R0-2-8")
        '   __dictionary.Add(inputAddress.SAW_BUSY, "A1區SAW_BUSY,IP91,R0-2-9")
        '   __dictionary.Add(inputAddress.SAW_DATAOK, "A1區SAW_DATAOK,IP92,R0-2-10")
        '   __dictionary.Add(inputAddress.SAW_RESULT0, "A1區SAW_RESULT0,IP93,R0-2-11")
        '   __dictionary.Add(inputAddress.SAW_RESULT1, "A1區SAW_RESULT1,IP94,R0-2-12")
        '   __dictionary.Add(inputAddress.SAW_RESULT2, "A1區SAW_RESULT2,IP95,R0-2-13")
        '   __dictionary.Add(inputAddress.SAW_RESULT3, "A1區SAW_RESULT3,IP96,R0-2-14")
        '   __dictionary.Add(inputAddress.SAW_INS_TEST, "A1區SAW_INS_TEST,IP97,R0-2-15")
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.BPD_In1, "D1區手動入卡匣鈕,IPC0,R0-5-0")
        '   __dictionary.Add(inputAddress.BPD_InM1, "D1區移動入卡匣鈕,IPC1,R0-5-1")
        '   __dictionary.Add(inputAddress.BPD_Out1, "D1區手動出卡匣鈕,IPC2,R0-5-2")
        '   __dictionary.Add(inputAddress.BPD_OutM1, "D1區移動出卡匣鈕,IPC3,R0-5-3")
        '   __dictionary.Add(inputAddress.BPD1_Pause, "D1區暫停鈕,IPC4,R0-5-4")
        '   __dictionary.Add(inputAddress.SxD1A, "D1區D區門禁1,IPC5,R0-5-5")
        '   __dictionary.Add(inputAddress.SxD1B, "D1區D區門禁2,IPC6,R0-5-6")
        '   '
        '   __dictionary.Add(inputAddress.SpD1A, "D1區入料到位檢知,IPD0,R0-5-8")
        '   __dictionary.Add(inputAddress.SpD1B, "D1區入料到位檢知,IPD1,R0-5-9")
        '   __dictionary.Add(inputAddress.SpD1C, "D1區入料到位檢知,IPD2,R0-5-10")
        '   __dictionary.Add(inputAddress.SpD1D, "D1區入料到位檢知,IPD3,R0-5-11")
        '   __dictionary.Add(inputAddress.SpD1E, "D1區到位檢知,IPD4,R0-5-12")
        '   __dictionary.Add(inputAddress.SpD1F, "D1區到位檢知,IPD5,R0-5-13")
        '   __dictionary.Add(inputAddress.SpD1G, "D1區入料到位檢知,IPD6,R0-5-14")
        '   __dictionary.Add(inputAddress.SpD1H, "D1區入料到位檢知,IPD7,R0-5-15")
        '   __dictionary.Add(inputAddress.SpD1I, "D1區吹氣分片檢知,IPE0,R0-5-16")
        '   __dictionary.Add(inputAddress.SpD1J, "D1區出料到位檢知,IPE1,R0-5-17")
        '   __dictionary.Add(inputAddress.SpD1K, "D1區入料到位檢知,IPE2,R0-5-18")
        '   __dictionary.Add(inputAddress.SpD1L, "D1區出料滿料檢知,IPE3,R0-5-19")
        '   __dictionary.Add(inputAddress.SpD1M, "D1區出料滿料檢知,IPE4,R0-5-20")
        '   __dictionary.Add(inputAddress.SPD1N, "D1區板片檢知,IPE5,R0-5-21")
        '   __dictionary.Add(inputAddress.SPD1O, "D1區卡匣檢知,IPE6,R0-5-22")
        '   __dictionary.Add(inputAddress.SPD1P, "D1區卡匣檢知,IPE7,R0-5-23")
        '   '
        '   __dictionary.Add(inputAddress.SpE1B, "E1區掉片檢知,IPF1,R0-5-25")
        '   __dictionary.Add(inputAddress.SpE1C, "E1區掉片檢知,IPF2,R0-5-26")
        '   __dictionary.Add(inputAddress.SpE1D, "E1區抓手片子有無,IPF3,R0-5-27")
        '   __dictionary.Add(inputAddress.SpE1E, "E1區抓手片子有無,IPF4,R0-5-28")
        '   '
        '   '
        '   '
        '   __dictionary.Add(inputAddress.D1a0, "D1區入料分料擋點伸,IP100,R0-6-0")
        '   __dictionary.Add(inputAddress.D1a1, "D1區入料分料擋點縮,IP101,R0-6-1")
        '   __dictionary.Add(inputAddress.D1b0, "D1區入料分料擋點伸,IP102,R0-6-2")
        '   __dictionary.Add(inputAddress.D1b1, "D1區入料分料擋點縮,IP103,R0-6-3")
        '   __dictionary.Add(inputAddress.D1c0, "D1區入料分料擋點伸,IP104,R0-6-4")
        '   __dictionary.Add(inputAddress.D1c1, "D1區入料分料擋點縮,IP105,R0-6-5")
        '   __dictionary.Add(inputAddress.D1d0, "D1區,IP106,R0-6-6")
        '   __dictionary.Add(inputAddress.D1d1, "D1區,IP107,R0-6-7")
        '   __dictionary.Add(inputAddress.D1e0, "D1區,IP110,R0-6-8")
        '   __dictionary.Add(inputAddress.D1e1, "D1區,IP111,R0-6-9")
        '   __dictionary.Add(inputAddress.D1f0, "D1區,IP112,R0-6-10")
        '   __dictionary.Add(inputAddress.D1f1, "D1區,IP113,R0-6-11")
        '   __dictionary.Add(inputAddress.D1g0, "D1區,IP114,R0-6-12")
        '   __dictionary.Add(inputAddress.D1g1, "D1區,IP115,R0-6-13")
        '   __dictionary.Add(inputAddress.E1a0, "E1區取片汽缸伸,IP116,R0-6-14")
        '   __dictionary.Add(inputAddress.E1a1, "E1區取片汽缸縮,IP117,R0-6-15")
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '
        '

    End Sub

    Sub EnglishVersion()


    End Sub
    Sub swapDataBase()
        __dictionary.Clear()
        EnglishVersion()
    End Sub
    Public Function query(keyChain As Object) As Object Implements IMessageQuery.query
        If __dictionary.ContainsKey(keyChain) = True Then
            Return __dictionary(keyChain)
        Else
            'cannot find  the area index

            Return Nothing
        End If

    End Function
End Class