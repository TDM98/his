/*
 * 20170522 #001 CMN: Added variable to check InPt 5 year HI without paid enough
*/
using System;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class CalcHiBenefitTask:IResult
    {
        public Exception Error { get; private set; }

        private HealthInsurance _hiItem;
        private PaperReferal _referal;
        private long _V_RegistrationType = 0;
        private bool _isEmergInPtReExamination = false;
        
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
        }

        public HealthInsurance HiItem
        {
            get
            {
                return _hiItem;
            }
        }

        public PaperReferal Referal
        {
            get
            {
                return _referal;
            }
        }

        public bool IsCrossRegion
        {
            get;
            set;
        }
        public double HiBenefit
        {
            get;
            set;
        }
        public bool IsEmergency
        {
            get;
            set;
        }
        //HPT 19/08/2015 Begin: Khai báo thêm biến _isHICard_FiveYearsCont
        //Biến _isHICard_FiveYearsCont được truyền giá trị từ hàm gọi đến CalcHiBenefitTask để sử dụng cho hàm Excute bên dưới
        public bool _isHICard_FiveYearsCont
        {
            get;
            set;
        }
        public bool _isChildUnder6YearsOld
        {
            get;
            set;
        }
        public bool _isAllowCrossRegion
        {
            get;
            set;
        }
        //HPT 19/08/2015 End
        /*==== #001 ====*/
        private bool _IsHICard_FiveYearsCont_NoPaid = false;
        public bool IsHICard_FiveYearsCont_NoPaid
        {
            get { return _IsHICard_FiveYearsCont_NoPaid; }
            set
            {
                _IsHICard_FiveYearsCont_NoPaid = value;
            }
        }
        /*==== #001 ====*/
        public CalcHiBenefitTask(HealthInsurance hi, PaperReferal referal, long V_RegistrationType, bool Emergency = false, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false, bool IsHICard_FiveYearsCont_NoPaid = false)
        {
            _hiItem = hi;
            _referal = referal;
            IsEmergency = Emergency;
            _V_RegistrationType = V_RegistrationType;
            _isEmergInPtReExamination = IsEmergInPtReExamination;
            //HPT 19/08/2015 : Truyền giá trị cho biến _isHICard_FiveYearsCont đã khai báo ở trên
            _isHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            _isChildUnder6YearsOld = IsChildUnder6YearsOld;
            _isAllowCrossRegion = IsAllowCrossRegion;
            /*==== #001 ====*/
            this.IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid;
            /*==== #001 ====*/
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();

                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bHasError = false;
                        /*==== #001 ====*/
                        //contract.BeginCalculateHiBenefit(_hiItem, _referal, IsEmergency, V_RegistrationType, _isEmergInPtReExamination, _isHICard_FiveYearsCont, _isChildUnder6YearsOld, _isAllowCrossRegion,
                        contract.BeginCalculateHiBenefit_V2(_hiItem, _referal, IsEmergency, V_RegistrationType, _isEmergInPtReExamination, _isHICard_FiveYearsCont, _isChildUnder6YearsOld, _isAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid,
                        /*==== #001 ====*/
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool crossRegion = false;
                                    /*==== #001 ====*/
                                    //double benefit = contract.EndCalculateHiBenefit(out crossRegion, asyncResult);
                                    double benefit = contract.EndCalculateHiBenefit_V2(out crossRegion, asyncResult);
                                    /*==== #001 ====*/
                                    HiBenefit = benefit;
                                    IsCrossRegion = crossRegion;
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                    bHasError = true;
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = bHasError
                                    });
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = true
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
