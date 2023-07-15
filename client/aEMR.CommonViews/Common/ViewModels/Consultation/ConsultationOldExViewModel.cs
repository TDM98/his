namespace aEMR.Common.ViewModels
{
    
    public partial class ConsultationOldViewModel 
    {
        #region button control

        public enum ConsultationState
        {
            //Tao moi chan doan
            NewConsultationState = 1,
            //Hieu chinh chan doan
            EditConsultationState = 2,  
            //tao moi va chinh sua chan doan cua noi tru
            NewAndEditConsultationState = 3,
           
        }


        private ConsultationState _ConsultState = ConsultationState.NewConsultationState;
        public ConsultationState ConsultState
        {
            get
            {
                return _ConsultState;
            }
            set
            {
                //if (_ConsultState != value)
                {
                    _ConsultState = value;
                    NotifyOfPropertyChange(() => ConsultState);
                    switch (ConsultState)
                    {
                        case ConsultationState.NewConsultationState:
                            mNewConsultationState = true ;
                            mEditConsultationState = false;                            
                            break;
                        case ConsultationState.EditConsultationState:
                            mNewConsultationState = false;
                            mEditConsultationState = true;                            
                            break;
                        case ConsultationState.NewAndEditConsultationState:
                            mNewConsultationState = true;
                            mEditConsultationState = true;
                            break;
                    }
                }
            }
        }

        private bool _mNewConsultationState;
        public bool mNewConsultationState
        {
            get
            {
                return _mNewConsultationState;
            }
            set
            {
                if (_mNewConsultationState != value)
                {
                    _mNewConsultationState = value;
                    NotifyOfPropertyChange(() => mNewConsultationState);
                }
            }
        }

        public bool mSaveConsultationState
        {
            get
            {
                return mNewConsultationState && IsShowSummaryContent;
            }
        }

        private bool _mEditConsultationState;
        public bool mEditConsultationState
        {
            get
            {
                return _mEditConsultationState;
            }
            set
            {
                if (_mEditConsultationState != value)
                {
                    _mEditConsultationState = value;
                    NotifyOfPropertyChange(() => mEditConsultationState);
                    NotifyOfPropertyChange(() => mUpdateConsultationState);
                }
            }
        }

        public bool mUpdateConsultationState
        {
            get
            {
                return mEditConsultationState && IsShowSummaryContent;
            }
        }

        public void StateNew()
        {
            btCreateNewIsEnabled = true;
            btCreateNewByOldIsEnabled = true;
            btSaveCreateNewIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(()=>btCreateNewIsEnabled );
            NotifyOfPropertyChange(()=>btCreateNewByOldIsEnabled );
            NotifyOfPropertyChange(()=>btSaveCreateNewIsEnabled );
            NotifyOfPropertyChange(()=>btCancelIsEnabled );
            NotifyOfPropertyChange(()=>FormEditorIsEnabled);

        }

        public void StateNewEdit()
        {
            btCreateNewIsEnabled = true;
            btCreateNewByOldIsEnabled = true;
            btEditIsEnabled = true;
            btUpdateIsEnabled = false;
            btSaveCreateNewIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);

        }

        public void StateNewWaiting()
        {
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = true;
            btCancelIsEnabled = true;
            FormEditorIsEnabled = true;

            btEditIsEnabled = false;
           
            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        public void StateEdit()
        {
            btEditIsEnabled = true;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(() => btEditIsEnabled );
            NotifyOfPropertyChange(() => btUpdateIsEnabled );
            NotifyOfPropertyChange(() => btCancelIsEnabled );
            NotifyOfPropertyChange(() => FormEditorIsEnabled );
        }

        public void StateEditWaiting()
        {
            btEditIsEnabled = false;
            btUpdateIsEnabled = true;
            btCancelIsEnabled = true;
            FormEditorIsEnabled = true;
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;

            NotifyOfPropertyChange(() => btEditIsEnabled);
            NotifyOfPropertyChange(() => btUpdateIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        #endregion

        #region Old Button Control

        private bool _IsEnableButton;
        public bool IsEnableButton
        {
            get { return _IsEnableButton; }
            set
            {
                _IsEnableButton = value;
                NotifyOfPropertyChange(() => IsEnableButton);
                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                NotifyOfPropertyChange(() => btEditIsEnabled);
            }
        }

        private void ButtonForNotDiag(bool bCancel)
        {
            //ban dau neu benh nhan chua co 1 chan doan bat ky nao het va bCancel =false
            //co the dung lai cho khi nhan nut tao moi hoac tao moi dua tren chan doan cu va bCancel=true
            IsEnableButton = false;
            // btCreateNewIsEnabled = false;
            // btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = true;
            //btEditIsEnabled = false;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = bCancel;
        }

        private void ButtonForHasDiag()
        {
            IsEnableButton = true;
            //btCreateNewIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID;
            //btCreateNewByOldIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID && (DiagTrmtItem != null && Globals.PatientAllDetails.RegistrationInfo != null && DiagTrmtItem.PatientServiceRecord != null && (DiagTrmtItem.PatientServiceRecord.PtRegistrationID == Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID || DiagTrmtItem.PatientServiceRecord.PtRegistrationID == PtRegistrationIDLatest.GetValueOrDefault(0)));
            btSaveCreateNewIsEnabled = false;
            //btEditIsEnabled = true && (DiagTrmtItem != null && (DiagTrmtItem.DoctorStaffID == Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) || PermissionManager.IsAdminUser()));
            btUpdateIsEnabled = false;
            btCancelIsEnabled = false;
        }
        #endregion

    }
}