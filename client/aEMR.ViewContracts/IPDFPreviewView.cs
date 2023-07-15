/*
 *  20170215 #001 CMN Begin: Add report TransferForm
 *  20181101 #002 TTM: BM 0004220: Bổ sung param cho report toa thuốc.
 *  20181129 #003 TNHX: [BM0005312]: Bổ sung param cho report PhieuMienGiam.
 *  20210430 #004 BLQ: thêm OutPtTreatmentProgramID cho xem in hồ sơ bệnh án ngoại trú
 */
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPDFPreviewView
    {
        string DigitalSignatureResultPath { get; set; }
    }
}
