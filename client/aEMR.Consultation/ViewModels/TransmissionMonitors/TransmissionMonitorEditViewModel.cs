using System;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Threading;
using aEMR.ServiceClient;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using System.Windows.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Text;
using aEMR.Common;
using System.Windows.Data;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using System.ComponentModel;
using eHCMS.Services.Core.Base;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Service.Core.Common;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts.Configuration;
using System.Xml;
/*
* 20220110 #001 BLQ: Create New
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ITransmissionMonitorEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TransmissionMonitorEditViewModel : ViewModelBase, ITransmissionMonitorEdit
    {
        [ImportingConstructor]
        public TransmissionMonitorEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            FromTimeHour = Globals.GetViewModel<IMinHourControl>();
            FromTimeHour.DateTime = Globals.GetCurServerDateTime().Date;
            ToTimeHour = Globals.GetViewModel<IMinHourControl>();
            ToTimeHour.DateTime = Globals.GetCurServerDateTime().Date;
            LoadStaffCollection();
        }
        public void InitTransmissionMonitor(bool IsNew)
        {
            if (IsNew)
            {
                CurTransmissionMonitor.TransmissionMonitorID = 0;
                CurTransmissionMonitor.TransAmount = 0;
                CurTransmissionMonitor.TransSpeed = 0;
            }
            else
            {
                TransSpeed = IntToRoman(CurTransmissionMonitor.TransSpeed);
                FromTimeHour.DateTime = CurTransmissionMonitor.StartTime;
                ToTimeHour.DateTime = CurTransmissionMonitor.EndTime;
            }
        }
        private IMinHourControl _FromTimeHour;

        public IMinHourControl FromTimeHour
        {
            get
            {
                return _FromTimeHour;
            }
            set
            {
                if (_FromTimeHour == value)
                    return;
                _FromTimeHour = value;
                NotifyOfPropertyChange(() => FromTimeHour);
            }
        }
        private IMinHourControl _ToTimeHour;

        public IMinHourControl ToTimeHour
        {
            get
            {
                return _ToTimeHour;
            }
            set
            {
                if (_ToTimeHour == value)
                    return;
                _ToTimeHour = value;
                NotifyOfPropertyChange(() => ToTimeHour);
            }
        }
        private ObservableCollection<Staff> _Staffs;
        public ObservableCollection<Staff> Staffs
        {
            get
            {
                return _Staffs;
            }
            set
            {
                if (_Staffs != value)
                {
                    _Staffs = value;
                    NotifyOfPropertyChange(() => Staffs);
                }
            }
        }

        private Staff _gSelectedStaff;
        public Staff gSelectedStaff
        {
            get
            {
                return _gSelectedStaff;
            }
            set
            {
                if (_gSelectedStaff == value) return;
                _gSelectedStaff = value;
                NotifyOfPropertyChange(() => gSelectedStaff);
            }
        }
        private TransmissionMonitor _CurTransmissionMonitor;
        public TransmissionMonitor CurTransmissionMonitor
        {
            get
            {
                return _CurTransmissionMonitor;
            }
            set
            {
                if (_CurTransmissionMonitor != value)
                {
                    _CurTransmissionMonitor = value;
                    NotifyOfPropertyChange(() => CurTransmissionMonitor);
                }
            }
        }
        private string _TransSpeed;
        public string TransSpeed
        {
            get
            {
                return _TransSpeed;
            }
            set
            {
                if (_TransSpeed != value)
                {
                    _TransSpeed = value;
                    NotifyOfPropertyChange(() => TransSpeed);
                }
            }
        }
        private void LoadStaffCollection()
        {
            Staffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null && (!x.IsStopUsing)).ToList());
            gSelectedStaff = Staffs != null ? Staffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
        }
        public void Staff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(Staffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void Staff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void SaveTM()
        {
            if (CurTransmissionMonitor == null)
            {
                return;
            }
            if (CheckValidTransmissionMonitor())
            {
                SaveTransmissionMonitor();
            }
          
            //string temp = "MMM";
            //if (ValidationOfRomanNumerals(temp))
            //{
            //    int tempResInt = RomanToInteger(temp);
            //    string tempResRoman = IntToRoman(tempResInt);
            //}
            //else
            //{
            //    MessageBox.Show("Sai định dạng số la mã");
            //}
            //int countSuccess = 0;
            //int countFail = 0;
            //for(int i = 1; i < 4000; i++)
            //{
            //    string tempResRoman = IntToRoman(i);
            //    if (ValidationOfRomanNumerals(tempResRoman))
            //    {
            //        //Console.WriteLine(tempResRoman + " - " + RomanToInteger(tempResRoman));
            //        if(i == RomanToInteger(tempResRoman))
            //        {
            //            countSuccess++;
            //        }
            //    }
            //    else
            //    {
            //        countFail++;
            //    }
            //}
            //Console.WriteLine("Đúng: " + countSuccess + ".Sai: " + countFail);
        }
        private bool CheckValidTransmissionMonitor()
        {
            if (CurTransmissionMonitor.TransAmount < 1)
            {
                MessageBox.Show("Chưa nhập Số ml truyền");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TransSpeed))
            {
                MessageBox.Show("Chưa nhập Tốc độ truyền");
                return false;
            }
            if (!ValidationOfRomanNumerals(TransSpeed))
            {
                MessageBox.Show("Tốc độ truyền nhập sai định dạng");
                return false;
            }
            if(FromTimeHour.DateTime == null)
            {
                MessageBox.Show("Chưa nhập giờ bắt đầu");
                return false;
            }
            if (ToTimeHour.DateTime == null)
            {
                MessageBox.Show("Chưa nhập giờ kết thúc");
                return false;
            }
            if (ToTimeHour.DateTime.Value <= FromTimeHour.DateTime.Value)
            {
                MessageBox.Show("Giờ kết thúc không được nhỏ hơn hoặc bằng giờ bắt đầu");
                return false;
            }
            if (gSelectedStaff == null)
            {
                MessageBox.Show("Chưa chọn điều dưỡng thực hiện");
                return false;
            }
            return true;
        }
        private void SaveTransmissionMonitor()
        {
            CurTransmissionMonitor.StartTime = FromTimeHour.DateTime.Value;
            CurTransmissionMonitor.EndTime = ToTimeHour.DateTime.Value;
            CurTransmissionMonitor.TransSpeed = RomanToInteger(TransSpeed);
            CurTransmissionMonitor.StaffID = gSelectedStaff.StaffID;
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveTransmissionMonitor(CurTransmissionMonitor, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var result = contract.EndSaveTransmissionMonitor(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK );
                                    this.TryClose();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.DlgShowBusyIndicator();
                }
            });
            t.Start();
        }
        public static bool ValidationOfRomanNumerals(string str)
        {
            string strRegex = @"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(str))
                return (true);
            else
                return (false);
        }
        private static Dictionary<char, int> RomanToIntMap = new Dictionary<char, int>()
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };
        public static int RomanToInteger(string roman)
        {
            int number = 0;
            for (int i = 0; i < roman.Length; i++)
            {
                if (i + 1 < roman.Length && RomanToIntMap[roman[i]] < RomanToIntMap[roman[i + 1]])
                {
                    number -= RomanToIntMap[roman[i]];
                }
                else
                {
                    number += RomanToIntMap[roman[i]];
                }
            }
            return number;
            
        }
        private static Dictionary<string, int> IntToRomanMap = new Dictionary<string, int>()
        {
            {"I", 1},
            {"IV", 4},
            {"V", 5},
            {"IX", 9},
            {"X", 10},
            {"XL", 40},
            {"L", 50},
            {"XC", 90},
            {"C", 100},
            {"CD", 400},
            {"D", 500},
            {"CM", 900},
            {"M", 1000}
        };
        public string IntToRoman(int num)
        {
            string romanResult = "";

            foreach (var item in IntToRomanMap.Reverse())
            {
                if (num <= 0) break;
                while (num >= item.Value)
                {
                    romanResult += item.Key;
                    num -= item.Value;
                }
            }
            return romanResult;
        }
    }
}
