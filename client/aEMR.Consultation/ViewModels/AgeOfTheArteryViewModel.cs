using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.ViewModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IAgeOfTheArtery)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgeOfTheArteryViewModel : ViewModelBase, IAgeOfTheArtery
    {
        public AgeOfTheArteryViewModel()
        {
            CurAgeOfTheArtery = new AgeOfTheArtery();
        }

        private Patient _Patient;
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                _Patient = value;
                NotifyOfPropertyChange(() => Patient);

                if (Patient == null || Patient.PatientID == 0 || Patient.Gender == null || Patient.Age <= 0)
                {
                    Globals.ShowMessage(string.Format("{0}.", eHCMSResources.Z0400_G1_KgNhanDuocDLieuLamViec), eHCMSResources.G0442_G1_TBao);
                    TryClose();
                }
                else
                {
                    CurAgeOfTheArtery.Patient = Patient;
                }
            }
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
                CurAgeOfTheArtery.PtRegistrationID = PtRegistrationID;
                GetCurAgeOfTheArtery();
            }
        }

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
                CurAgeOfTheArtery.V_RegistrationType = V_RegistrationType;
                GetCurAgeOfTheArtery();
            }
        }

        private long _PatientClassID;
        public long PatientClassID
        {
            get
            {
                return _PatientClassID;
            }
            set
            {
                _PatientClassID = value;
                NotifyOfPropertyChange(() => PatientClassID);
                CurAgeOfTheArtery.PatientClassID = PatientClassID;
                GetCurAgeOfTheArtery();
            }
        }
        
        private AgeOfTheArtery _CurAgeOfTheArtery;
        public AgeOfTheArtery CurAgeOfTheArtery
        {
            get
            {
                return _CurAgeOfTheArtery;
            }
            set
            {
                _CurAgeOfTheArtery = value;
                NotifyOfPropertyChange(() => CurAgeOfTheArtery);
            }
        }

        private string _BMIRate;
        public string BMIRate
        {
            get
            {
                return _BMIRate;
            }
            set
            {
                _BMIRate = value;
                NotifyOfPropertyChange(() => BMIRate);
            }
        }

        private bool _IsEdit = false;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        private ComboBox CtrTreatmentBloodPressure;

        public void cboTreatmentBloodPressure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrTreatmentBloodPressure = sender as ComboBox;

            if (CurAgeOfTheArtery != null)
            {
                CurAgeOfTheArtery.IsTreatmentBloodPressure = CtrTreatmentBloodPressure.SelectedIndex == 0 ? false : true;
            }
        }
        public void cboTreatmentBloodPressure_Loaded(object sender, RoutedEventArgs e)
        {
            CtrTreatmentBloodPressure = sender as ComboBox;
        }

        private ComboBox CtrIsSmoke;

        public void cboIsSmoke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrIsSmoke = sender as ComboBox;

            if (CurAgeOfTheArtery != null)
            {
                CurAgeOfTheArtery.IsSmoke = CtrIsSmoke.SelectedIndex == 0 ? false : true;
            }
        }
        public void cboIsSmoke_Loaded(object sender, RoutedEventArgs e)
        {
            CtrIsSmoke = sender as ComboBox;
        }

        private ComboBox CtrDiabetes;

        public void cboDiabetes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrDiabetes = sender as ComboBox;

            if (CurAgeOfTheArtery != null)
            {
                CurAgeOfTheArtery.Diabetes = CtrDiabetes.SelectedIndex == 0 ? false : true;
            }
        }
        public void cboDiabetes_Loaded(object sender, RoutedEventArgs e)
        {
            CtrDiabetes = sender as ComboBox;
        }

        private bool CheckValid(object temp)
        {
            AgeOfTheArtery u = temp as AgeOfTheArtery;
            if (u == null)
            {
                return false;
            }
            if (IsNullNum(u.Height))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.K1862_G1_ChieuCao), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (IsNullNum(u.Weight))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.K1558_G1_CanNang), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (IsNullNum(u.BloodPressure))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T1770_G1_HApmmHg), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (IsNullNum(u.HDL))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T1430_G1_HDL), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (IsNullNum(u.Cholesterol))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T3822_G1_Cholesterol), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return u.Validate();
        }

        private bool IsNullNum(float? num)
        {
            if (num == null || num == 0)
                return true;
            else
                return false;
        }

        public void btnSave()
        {
            if (CurAgeOfTheArtery == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0400_G1_KgNhanDuocDLieuLamViec), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (CheckValid(CurAgeOfTheArtery))
            {
                if (CurAgeOfTheArtery.Patient.Gender == "F")
                {
                    Cal_Point_Female();
                }
                else
                {
                    Cal_Point_Male();
                }
                CurAgeOfTheArtery.CreatedStaff = CurAgeOfTheArtery.LastUpdateStaff = Globals.LoggedUserAccount.Staff;

                if (!IsEdit)
                {
                    SaveCurAgeOfTheArtery();
                }
                else
                {
                    UpdateCurAgeOfTheArtery();
                }
            }
        }

        public void btnEdit()
        {
            btnSave();
        }

        public void btnPrint()
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.ID = CurAgeOfTheArtery.AgeOfTheArteryID;
                proAlloc.eItem = ReportName.XRpt_AgeOfTheArtery;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void SaveCurAgeOfTheArtery()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveAgeOfTheArtery(CurAgeOfTheArtery, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long id = 0;
                            var results = contract.EndSaveAgeOfTheArtery(out id, asyncResult);

                            if (results)
                            {
                                if (id > 0)
                                {
                                    CurAgeOfTheArtery.AgeOfTheArteryID = id;
                                    IsEdit = true;
                                }
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        public void UpdateCurAgeOfTheArtery()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateAgeOfTheArtery(CurAgeOfTheArtery, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndUpdateAgeOfTheArtery(asyncResult);

                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        public void GetCurAgeOfTheArtery()
        {
            if (PtRegistrationID == 0 || V_RegistrationType == 0 || PatientClassID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAgeOfTheArtery_ByPatient(PtRegistrationID, V_RegistrationType, PatientClassID,
                        Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAgeOfTheArtery_ByPatient(asyncResult);

                            if (results != null)
                            {
                                CurAgeOfTheArtery = results;
                                CtrTreatmentBloodPressure.SelectedIndex = Convert.ToInt32(CurAgeOfTheArtery.IsTreatmentBloodPressure);
                                CtrIsSmoke.SelectedIndex = Convert.ToInt32(CurAgeOfTheArtery.IsSmoke);
                                CtrDiabetes.SelectedIndex = Convert.ToInt32(CurAgeOfTheArtery.Diabetes);

                                IsEdit = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void Cal_Point_Female()
        {
            CurAgeOfTheArtery.AgePoint = CheckPointAndReturn((float)CurAgeOfTheArtery.Patient.Age, PointF_Age);

            if (!CurAgeOfTheArtery.IsTreatmentBloodPressure)
            {
                CurAgeOfTheArtery.BloodPressureScore = CheckPointAndReturn((float)CurAgeOfTheArtery.BloodPressure, PointF_BloodPressureNotTreatment);
            }
            else
            {
                CurAgeOfTheArtery.BloodPressureScore = CheckPointAndReturn((float)CurAgeOfTheArtery.BloodPressure, PointF_BloodPressureIsTreatment);
            }

            CurAgeOfTheArtery.HDLScore = CheckPointAndReturn((float)CurAgeOfTheArtery.HDL, Point_HDL);

            CurAgeOfTheArtery.CholesterolScore = CheckPointAndReturn((float)CurAgeOfTheArtery.Cholesterol, PointF_Cholesterol);

            if (!CurAgeOfTheArtery.IsSmoke)
            {
                CurAgeOfTheArtery.SmokeScore = 0;
            }
            else
            {
                CurAgeOfTheArtery.SmokeScore = 3;
            }

            if (!CurAgeOfTheArtery.Diabetes)
            {
                CurAgeOfTheArtery.DiabetesScore = 0;
            }
            else
            {
                CurAgeOfTheArtery.DiabetesScore = 4;
            }

            CurAgeOfTheArtery.TotalScore = Cal_TotalScore(CurAgeOfTheArtery);
            CurAgeOfTheArtery.AgePointOfTheArtery = CalF_AgePointOfTheArtery(CurAgeOfTheArtery.TotalScore);
            CurAgeOfTheArtery.BMI = Cal_BMI((float)CurAgeOfTheArtery.Height, (float)CurAgeOfTheArtery.Weight);
        }

        private void Cal_Point_Male()
        {
            CurAgeOfTheArtery.AgePoint = CheckPointAndReturn((float)CurAgeOfTheArtery.Patient.Age, PointM_Age);

            if (!CurAgeOfTheArtery.IsTreatmentBloodPressure)
            {
                CurAgeOfTheArtery.BloodPressureScore = CheckPointAndReturn((float)CurAgeOfTheArtery.BloodPressure, PointM_BloodPressureNotTreatment);
            }
            else
            {
                CurAgeOfTheArtery.BloodPressureScore = CheckPointAndReturn((float)CurAgeOfTheArtery.BloodPressure, PointM_BloodPressureIsTreatment);
            }

            CurAgeOfTheArtery.HDLScore = CheckPointAndReturn((float)CurAgeOfTheArtery.HDL, Point_HDL);

            CurAgeOfTheArtery.CholesterolScore = CheckPointAndReturn((float)CurAgeOfTheArtery.Cholesterol, PointM_Cholesterol);

            if (!CurAgeOfTheArtery.IsSmoke)
            {
                CurAgeOfTheArtery.SmokeScore = 0;
            }
            else
            {
                CurAgeOfTheArtery.SmokeScore = 4;
            }

            if (!CurAgeOfTheArtery.Diabetes)
            {
                CurAgeOfTheArtery.DiabetesScore = 0;
            }
            else
            {
                CurAgeOfTheArtery.DiabetesScore = 3;
            }

            CurAgeOfTheArtery.TotalScore = Cal_TotalScore(CurAgeOfTheArtery);
            CurAgeOfTheArtery.AgePointOfTheArtery = CalM_AgePointOfTheArtery(CurAgeOfTheArtery.TotalScore);
            CurAgeOfTheArtery.BMI = Cal_BMI((float)CurAgeOfTheArtery.Height, (float)CurAgeOfTheArtery.Weight);
        }


        //Function và List kiểm tra điểm
        #region Func_List_Tinh_Diem
        public class ListPointCheck
        {
            private int _Point;
            private int _Min;
            private int _Max;
            public int Point
            {
                get { return _Point; }
                set { _Point = value; }
            }
            public int Min
            {
                get { return _Min; }
                set { _Min = value; }
            }
            public int Max
            {
                get { return _Max; }
                set { _Max = value; }
            }
        }

        private int CheckPointAndReturn(float value, List<ListPointCheck> lists)
        {
            var point = 0;
            foreach (var item in lists)
            {
                if (item.Min == 0 && item.Max > 0)
                {
                    if (value <= item.Max) point = item.Point;
                }
                else if (item.Min > 0 && item.Max == 0)
                {
                    if (value >= item.Min) point = item.Point;
                }
                else
                {
                    if (value >= item.Min && value <= item.Max) point = item.Point;
                }
            }

            return point;
        }

        private int Cal_TotalScore(AgeOfTheArtery a)
        {
            var point = 0;
            point = a.AgePoint + a.BloodPressureScore + a.HDLScore + a.CholesterolScore + a.SmokeScore + a.DiabetesScore;
            return point;
        }

        private int CalF_AgePointOfTheArtery(int TotalScore)
        {
            var point = 0;
            for (int i = 0; i < 16; i++)
            {
                if (TotalScore == i)
                {
                    point = PointF_AgeOfTheArtery[i];
                }
                else
                {
                    if (TotalScore < 1)
                    {
                        point = 30;
                    }
                    else
                    {
                        if (TotalScore > 14)
                        {
                            point = 80;
                        }

                    }
                }
            }
            return point;
        }

        private int CalM_AgePointOfTheArtery(int TotalScore)
        {
            var point = 0;
            for (int i = 0; i < 18; i++)
            {
                if (TotalScore == i)
                {
                    point = PointM_AgeOfTheArtery[i];
                }
                else
                {
                    if (TotalScore < 0)
                    {
                        point = 30;
                    }
                    else
                    {
                        if (TotalScore > 16)
                        {
                            point = 80;
                        }

                    }
                }
            }
            return point;
        }

        private decimal Cal_BMI(float Height, float Weight)
        {
            float height = Height / 100;

            float bmi = Weight / (height * height);

            if (bmi < 18.50)
            {
                BMIRate = "Người Gầy";
            }
            else
            {
                if (bmi >= 18.50 && bmi <= 24.99)
                {
                    BMIRate = "Bình Thường";
                }
                else
                {
                    if (bmi == 25)
                    {
                        BMIRate = "Thừa Cân";
                    }
                    else
                    {
                        if (bmi > 25 && bmi <= 29.99)
                        {
                            BMIRate = "Tiền Béo Phì";
                        }
                        else
                        {
                            if (bmi >= 30 && bmi <= 34.99)
                            {
                                BMIRate = "Béo Phì Độ I";
                            }
                            else
                            {
                                if (bmi >= 35 && bmi <= 39.99)
                                {
                                    BMIRate = "Béo Phì Độ II";
                                }
                                else
                                {
                                    BMIRate = "Béo Phì Độ III";
                                }
                            }
                        }
                    }

                }
            }
            return (decimal)bmi;
        }
        //List điểm để kiếm tra
        #region List_Diem

        // Điểm tuổi nữ
        #region Female
        private List<ListPointCheck> PointF_Age = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 34 },
            new ListPointCheck() { Point = 2, Min = 35, Max = 39 },
            new ListPointCheck() { Point = 4, Min = 40, Max = 44 },
            new ListPointCheck() { Point = 5, Min = 45, Max = 49 },
            new ListPointCheck() { Point = 7, Min = 50, Max = 54 },
            new ListPointCheck() { Point = 8, Min = 55, Max = 59 },
            new ListPointCheck() { Point = 9, Min = 60, Max = 64 },
            new ListPointCheck() { Point = 10, Min = 65, Max = 69 },
            new ListPointCheck() { Point = 11, Min = 70, Max = 74 },
            new ListPointCheck() { Point = 12, Min = 75, Max = 0 }
        };

        private List<ListPointCheck> PointF_BloodPressureNotTreatment = new List<ListPointCheck>()
        {
            new ListPointCheck() {Point = -3, Min = 0, Max = 120 },
            new ListPointCheck() {Point = 0, Min = 120, Max = 129 },
            new ListPointCheck() {Point = 1, Min = 130, Max = 139 },
            new ListPointCheck() {Point = 2, Min = 140, Max = 149 },
            new ListPointCheck() {Point = 4, Min = 150, Max = 159 },
            new ListPointCheck() {Point = 5, Min = 160, Max = 0 }
        };

        private List<ListPointCheck> PointF_BloodPressureIsTreatment = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 120 },
            new ListPointCheck() { Point = 2, Min = 120, Max = 129 },
            new ListPointCheck() { Point = 3, Min = 130, Max = 139 },
            new ListPointCheck() { Point = 5, Min = 140, Max = 149 },
            new ListPointCheck() { Point = 6, Min = 150, Max = 159 },
            new ListPointCheck() { Point = 7, Min = 160, Max = 0 }
        };

        private List<ListPointCheck> PointF_Cholesterol = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 160 },
            new ListPointCheck() { Point = 1, Min = 160, Max = 199 },
            new ListPointCheck() { Point = 3, Min = 200, Max = 239 },
            new ListPointCheck() { Point = 4, Min = 240, Max = 279 },
            new ListPointCheck() { Point = 5, Min = 280, Max = 0 }
        };
        
        private int[] PointF_AgeOfTheArtery = new int[] { 30, 31, 34, 36, 39, 42, 45, 48, 51, 55, 59, 64, 68, 73, 79, 80 };
        #endregion

        // Điểm tuổi nam
        #region Male
        private List<ListPointCheck> PointM_Age = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 34 },
            new ListPointCheck() { Point = 2, Min = 35, Max = 39 },
            new ListPointCheck() { Point = 5, Min = 40, Max = 44 },
            new ListPointCheck() { Point = 6, Min = 45, Max = 49 },
            new ListPointCheck() { Point = 8, Min = 50, Max = 54 },
            new ListPointCheck() { Point = 10, Min = 55, Max = 59 },
            new ListPointCheck() { Point = 11, Min = 60, Max = 64 },
            new ListPointCheck() { Point = 12, Min = 65, Max = 69 },
            new ListPointCheck() { Point = 14, Min = 70, Max = 74 },
            new ListPointCheck() { Point = 15, Min = 75, Max = 0 }
        };

        private List<ListPointCheck> PointM_BloodPressureNotTreatment = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = -2, Min = 0, Max = 120 },
            new ListPointCheck() { Point = 0, Min = 120, Max = 129 },
            new ListPointCheck() { Point = 1, Min = 130, Max = 139 },
            new ListPointCheck() { Point = 2, Min = 140, Max = 159 },
            new ListPointCheck() { Point = 3, Min = 160, Max = 0 }
        };

        private List<ListPointCheck> PointM_BloodPressureIsTreatment = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 120 },
            new ListPointCheck() { Point = 2, Min = 120, Max = 129 },
            new ListPointCheck() { Point = 3, Min = 130, Max = 139 },
            new ListPointCheck() { Point = 4, Min = 140, Max = 159 },
            new ListPointCheck() { Point = 5, Min = 160, Max = 0 }
        };

        private List<ListPointCheck> PointM_Cholesterol = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 0, Min = 0, Max = 160 },
            new ListPointCheck() { Point = 1, Min = 160, Max = 199 },
            new ListPointCheck() { Point = 2, Min = 200, Max = 239 },
            new ListPointCheck() { Point = 3, Min = 240, Max = 279 },
            new ListPointCheck() { Point = 4, Min = 280, Max = 0 }
        };
        
        private int[] PointM_AgeOfTheArtery = new int[] { 30, 32, 34, 36, 38, 40, 42, 45, 48, 51, 54, 57, 60, 64, 68, 72, 76, 80 };
        #endregion

        private List<ListPointCheck> Point_HDL = new List<ListPointCheck>()
        {
            new ListPointCheck() { Point = 2, Min = 0, Max = 35 },
            new ListPointCheck() { Point = 1, Min = 35, Max = 44 },
            new ListPointCheck() { Point = 0, Min = 45, Max = 49 },
            new ListPointCheck() { Point = -1, Min = 50, Max = 60 },
            new ListPointCheck() { Point = -2, Min = 60, Max = 0 }
        };
        #endregion
        #endregion
    }
}
