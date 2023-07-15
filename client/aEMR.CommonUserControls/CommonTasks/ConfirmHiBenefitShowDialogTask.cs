using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

using aEMR.ViewContracts;

namespace aEMR.CommonTasks
{
    public class ConfirmHiBenefitDialogTask : IResult
    {
        public ConfirmHiBenefitEvent confirmHiBenefitEvent { get; set; }

        double _OriginalHiBenefit ;
        double _HiBenefit ;
        long _HiId ;
        long _PatientId ;
        bool _OriginalIsCrossRegion ;
        bool _IsAllowCrossRegion;
        bool _VisibilityCbxAllowCrossRegion;
        int _ShowOpts_To_OJR_Card = 0;
        int _PreSelected_OJR_Option = 0;
                
        //vm.SetCrossRegion(calcHiTask.IsCrossRegion);

        // TxD 04/01/2015: Added V_RegistrationType to determine Rebate level for InPt according to new 2015 HI Rule
        private long _V_RegistrationType = 0;

        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
        }


        public static IWindowManager WindowManager;

        public ConfirmHiBenefitDialogTask(double OriginalHiBenefit ,
                                            double HiBenefit ,
                                            long HiId ,
                                            long PatientId ,
                                            bool OriginalIsCrossRegion,
                                            long V_RegistrationType,
                                            bool IsAllowCrossRegion,
                                            bool VisibilityCbxAllowCrossRegion,
                                            int ShowOpts_To_OJR_Card = 0,
                                            int PreSelected_OJR_Option = 0) 
        {
            _OriginalHiBenefit = OriginalHiBenefit;
            _HiBenefit = HiBenefit;
            _HiId = HiId;
            _PatientId = PatientId;
            _OriginalIsCrossRegion = OriginalIsCrossRegion;
            _V_RegistrationType = V_RegistrationType;
            _IsAllowCrossRegion = IsAllowCrossRegion;
            _VisibilityCbxAllowCrossRegion = VisibilityCbxAllowCrossRegion;
            _ShowOpts_To_OJR_Card = ShowOpts_To_OJR_Card;
            _PreSelected_OJR_Option = PreSelected_OJR_Option;

            //Globals.EventAggregator.Subscribe(this);
        }       

        public void Execute(ActionExecutionContext context)
        {
            //var ApptCheck = Globals.GetViewModel<IPrescriptionApptCheck>();
            //var ApptCheck = IoC.Get<IConfirmHiBenefit>();
            Action<IConfirmHiBenefit> onInitDlg = (ApptCheck) =>
            {
               ApptCheck.OriginalHiBenefit = _HiBenefit;
               ApptCheck.HiBenefit = _HiBenefit;

               if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU)
               {
                   ApptCheck.RebatePercentageLevel1 = _HiBenefit;
               }

               ApptCheck.HiId = _HiId;
               ApptCheck.PatientId = _PatientId;
               ApptCheck.OriginalIsCrossRegion = _OriginalIsCrossRegion;
               ApptCheck.SetCrossRegion(_OriginalIsCrossRegion);
               ApptCheck.IsAllowCrossRegion = _IsAllowCrossRegion;
               ApptCheck.VisibilityCbxAllowCrossRegion = _VisibilityCbxAllowCrossRegion;
               ApptCheck.ShowOpts_To_OJR_Card = _ShowOpts_To_OJR_Card;
               ApptCheck.PreSelected_OJR_Option = _PreSelected_OJR_Option;
            };
            
            var confirmHIDlgVM = 
                        GlobalsNAV.ShowDialog_V2<IConfirmHiBenefit>(false, null, onInitDlg);

            confirmHiBenefitEvent = confirmHIDlgVM.confirmHiBenefitEvent;

            Completed(this, new ResultCompletionEventArgs());

            // TxD 21/07/2018: Commented OUT the following because IDeactivate DOESNOT WORK for WPF somehow
            //                 Instead we just retrieve the details from the ComfirmDlg itself after closing because execution actually STOP at Globals.ShowDialog_V2
            //                 so we can safely get the details right below the ShowDialog then call Completed.

            //var deActive = instance as IDeactivate;
            //if (deActive == null)
            //{
            //    Completed(this, new ResultCompletionEventArgs());
            //}
            //else 
            //{
            //    deActive.Deactivated += new EventHandler<DeactivationEventArgs>((o, e) => 
            //    {
            //        if (e.WasClosed) 
            //        {
            //            confirmHiBenefitEvent = ((IConfirmHiBenefit)o).confirmHiBenefitEvent;
            //            Completed(this, new ResultCompletionEventArgs());
            //        }
            //    });
            //}
        }

        //void deActive_Deactivated(object sender, DeactivationEventArgs e)
        //{
        //    if (e.WasClosed)
        //    {
        //        confirmHiBenefitEvent = ((IConfirmHiBenefit)sender).confirmHiBenefitEvent;
        //        Completed(this,new ResultCompletionEventArgs());
        //    }
        //}
                
        public event EventHandler<ResultCompletionEventArgs> Completed;


    }
}
