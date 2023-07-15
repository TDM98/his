using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PrintStationParameter : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PrintStationParameter object.

        /// <param name="parsID">Initial value of the ParsID property.</param>
        /// <param name="parsDateTime">Initial value of the ParsDateTime property.</param>
        /// <param name="printStationNumber">Initial value of the PrintStationNumber property.</param>
        /// <param name="fromSequenceNumber">Initial value of the FromSequenceNumber property.</param>
        /// <param name="toSequenceNumber">Initial value of the ToSequenceNumber property.</param>
        /// <param name="inputMaskForReceiptNo">Initial value of the InputMaskForReceiptNo property.</param>
        public static PrintStationParameter CreatePrintStationParameter(long parsID, DateTime parsDateTime, Byte printStationNumber, Int64 fromSequenceNumber, Int64 toSequenceNumber, String inputMaskForReceiptNo)
        {
            PrintStationParameter printStationParameter = new PrintStationParameter();
            printStationParameter.ParsID = parsID;
            printStationParameter.ParsDateTime = parsDateTime;
            printStationParameter.PrintStationNumber = printStationNumber;
            printStationParameter.FromSequenceNumber = fromSequenceNumber;
            printStationParameter.ToSequenceNumber = toSequenceNumber;
            printStationParameter.InputMaskForReceiptNo = inputMaskForReceiptNo;
            return printStationParameter;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long ParsID
        {
            get
            {
                return _ParsID;
            }
            set
            {
                if (_ParsID != value)
                {
                    OnParsIDChanging(value);
                    ////ReportPropertyChanging("ParsID");
                    _ParsID = value;
                    RaisePropertyChanged("ParsID");
                    OnParsIDChanged();
                }
            }
        }
        private long _ParsID;
        partial void OnParsIDChanging(long value);
        partial void OnParsIDChanged();





        [DataMemberAttribute()]
        public DateTime ParsDateTime
        {
            get
            {
                return _ParsDateTime;
            }
            set
            {
                OnParsDateTimeChanging(value);
                ////ReportPropertyChanging("ParsDateTime");
                _ParsDateTime = value;
                RaisePropertyChanged("ParsDateTime");
                OnParsDateTimeChanged();
            }
        }
        private DateTime _ParsDateTime;
        partial void OnParsDateTimeChanging(DateTime value);
        partial void OnParsDateTimeChanged();





        [DataMemberAttribute()]
        public Byte PrintStationNumber
        {
            get
            {
                return _PrintStationNumber;
            }
            set
            {
                OnPrintStationNumberChanging(value);
                ////ReportPropertyChanging("PrintStationNumber");
                _PrintStationNumber = value;
                RaisePropertyChanged("PrintStationNumber");
                OnPrintStationNumberChanged();
            }
        }
        private Byte _PrintStationNumber;
        partial void OnPrintStationNumberChanging(Byte value);
        partial void OnPrintStationNumberChanged();





        [DataMemberAttribute()]
        public Int64 FromSequenceNumber
        {
            get
            {
                return _FromSequenceNumber;
            }
            set
            {
                OnFromSequenceNumberChanging(value);
                ////ReportPropertyChanging("FromSequenceNumber");
                _FromSequenceNumber = value;
                RaisePropertyChanged("FromSequenceNumber");
                OnFromSequenceNumberChanged();
            }
        }
        private Int64 _FromSequenceNumber;
        partial void OnFromSequenceNumberChanging(Int64 value);
        partial void OnFromSequenceNumberChanged();





        [DataMemberAttribute()]
        public Int64 ToSequenceNumber
        {
            get
            {
                return _ToSequenceNumber;
            }
            set
            {
                OnToSequenceNumberChanging(value);
                ////ReportPropertyChanging("ToSequenceNumber");
                _ToSequenceNumber = value;
                RaisePropertyChanged("ToSequenceNumber");
                OnToSequenceNumberChanged();
            }
        }
        private Int64 _ToSequenceNumber;
        partial void OnToSequenceNumberChanging(Int64 value);
        partial void OnToSequenceNumberChanged();





        [DataMemberAttribute()]
        public String InputMaskForReceiptNo
        {
            get
            {
                return _InputMaskForReceiptNo;
            }
            set
            {
                OnInputMaskForReceiptNoChanging(value);
                ////ReportPropertyChanging("InputMaskForReceiptNo");
                _InputMaskForReceiptNo = value;
                RaisePropertyChanged("InputMaskForReceiptNo");
                OnInputMaskForReceiptNoChanged();
            }
        }
        private String _InputMaskForReceiptNo;
        partial void OnInputMaskForReceiptNoChanging(String value);
        partial void OnInputMaskForReceiptNoChanged();





        [DataMemberAttribute()]
        public String ParsNotes
        {
            get
            {
                return _ParsNotes;
            }
            set
            {
                OnParsNotesChanging(value);
                ////ReportPropertyChanging("ParsNotes");
                _ParsNotes = value;
                RaisePropertyChanged("ParsNotes");
                OnParsNotesChanged();
            }
        }
        private String _ParsNotes;
        partial void OnParsNotesChanging(String value);
        partial void OnParsNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_KEEPTRAC_REL_HOSFM_PRINTSTA", "KeepTrackPriting")]
        public ObservableCollection<KeepTrackPriting> KeepTrackPritings
        {
            get;
            set;
        }

        #endregion
    }
}
