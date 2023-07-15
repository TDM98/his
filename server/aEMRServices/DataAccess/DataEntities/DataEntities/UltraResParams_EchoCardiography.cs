/*
 * 20170413 #001 CMN: Bỏ tính tự động Gd.Max từ Vel.Max
 * 20190718 TTM: Thay đổi 001 (Phòng mạch BS Huân cần chức năng này).
*/
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UltraResParams_EchoCardiography : NotifyChangedBase, IEditableObject
    {
        public UltraResParams_EchoCardiography()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            UltraResParams_EchoCardiography info = obj as UltraResParams_EchoCardiography;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UltraResParams_EchoCardiographyID > 0 && this.UltraResParams_EchoCardiographyID == info.UltraResParams_EchoCardiographyID;
        }
        private UltraResParams_EchoCardiography _tempUltraResParams_EchoCardiography;

        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempUltraResParams_EchoCardiography = (UltraResParams_EchoCardiography)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempUltraResParams_EchoCardiography)
                CopyFrom(_tempUltraResParams_EchoCardiography);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(UltraResParams_EchoCardiography p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new UltraResParams_EchoCardiography object.

        /// <param name="UltraResParams_EchoCardiographyID">Initial value of the UltraResParams_EchoCardiographyID property.</param>
        
        public static UltraResParams_EchoCardiography CreateUltraResParams_EchoCardiography(Byte UltraResParams_EchoCardiographyID, String UltraResParams_EchoCardiographyName)
        {
            UltraResParams_EchoCardiography UltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
            UltraResParams_EchoCardiography.UltraResParams_EchoCardiographyID = UltraResParams_EchoCardiographyID;
            
            return UltraResParams_EchoCardiography;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UltraResParams_EchoCardiographyID
        {
            get
            {
                return _UltraResParams_EchoCardiographyID;
            }
            set
            {
                if (_UltraResParams_EchoCardiographyID != value)
                {
                    OnUltraResParams_EchoCardiographyIDChanging(value);
                    _UltraResParams_EchoCardiographyID = value;
                    RaisePropertyChanged("UltraResParams_EchoCardiographyID");
                    OnUltraResParams_EchoCardiographyIDChanged();
                }
            }
        }
        private long _UltraResParams_EchoCardiographyID;
        partial void OnUltraResParams_EchoCardiographyIDChanging(long value);
        partial void OnUltraResParams_EchoCardiographyIDChanged();

        public bool Tab1_TM_Changed = false;

        [DataMemberAttribute()]
        public bool Tab1_TM_Update_Required = false;

        [DataMemberAttribute()]
        public double? TM_Vlt_Ttr
        {
            get
            {
                return _TM_Vlt_Ttr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Vlt_Ttr)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Vlt_TtrChanging(value);
                _TM_Vlt_Ttr = value;
                RaisePropertyChanged("TM_Vlt_Ttr");
                OnTM_Vlt_TtrChanged();
            }
        }
        private double? _TM_Vlt_Ttr;
        partial void OnTM_Vlt_TtrChanging(double? value);
        partial void OnTM_Vlt_TtrChanged();




        [DataMemberAttribute()]
        public double? TM_Dktt_Ttr
        {
            get
            {
                return _TM_Dktt_Ttr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Dktt_Ttr)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Dktt_TtrChanging(value);
                _TM_Dktt_Ttr = value;
                RaisePropertyChanged("TM_Dktt_Ttr");
                OnTM_Dktt_TtrChanged();
            }
        }
        private double? _TM_Dktt_Ttr;
        partial void OnTM_Dktt_TtrChanging(double? value);
        partial void OnTM_Dktt_TtrChanged();




        [DataMemberAttribute()]
        public double? TM_Tstt_Ttr
        {
            get
            {
                return _TM_Tstt_Ttr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Tstt_Ttr)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Tstt_TtrChanging(value);
                _TM_Tstt_Ttr = value;
                RaisePropertyChanged("TM_Tstt_Ttr");
                OnTM_Tstt_TtrChanged();
            }
        }
        private double? _TM_Tstt_Ttr;
        partial void OnTM_Tstt_TtrChanging(double? value);
        partial void OnTM_Tstt_TtrChanged();




        [DataMemberAttribute()]
        public double? TM_Vlt_Tt
        {
            get
            {
                return _TM_Vlt_Tt;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Vlt_Tt)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Vlt_TtChanging(value);
                _TM_Vlt_Tt = value;
                RaisePropertyChanged("TM_Vlt_Tt");
                OnTM_Vlt_TtChanged();
            }
        }
        private double? _TM_Vlt_Tt;
        partial void OnTM_Vlt_TtChanging(double? value);
        partial void OnTM_Vlt_TtChanged();




        [DataMemberAttribute()]
        public double? TM_Dktt_Tt
        {
            get
            {
                return _TM_Dktt_Tt;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Dktt_Tt)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Dktt_TtChanging(value);
                _TM_Dktt_Tt = value;
                RaisePropertyChanged("TM_Dktt_Tt");
                OnTM_Dktt_TtChanged();
            }
        }
        private double? _TM_Dktt_Tt;
        partial void OnTM_Dktt_TtChanging(double? value);
        partial void OnTM_Dktt_TtChanged();




        [DataMemberAttribute()]
        public double? TM_Tstt_Tt
        {
            get
            {
                return _TM_Tstt_Tt;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Tstt_Tt)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_Tstt_TtChanging(value);
                _TM_Tstt_Tt = value;
                RaisePropertyChanged("TM_Tstt_Tt");
                OnTM_Tstt_TtChanged();
            }
        }
        private double? _TM_Tstt_Tt;
        partial void OnTM_Tstt_TtChanging(double? value);
        partial void OnTM_Tstt_TtChanged();




        [DataMemberAttribute()]
        public double? TM_Pxcr
        {
            get
            {
                return _TM_Pxcr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Pxcr)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_PxcrChanging(value);
                _TM_Pxcr = value;
                RaisePropertyChanged("TM_Pxcr");
                OnTM_PxcrChanged();
            }
        }
        private double? _TM_Pxcr;
        partial void OnTM_PxcrChanging(double? value);
        partial void OnTM_PxcrChanged();




        [DataMemberAttribute()]
        public double? TM_Pxtm
        {
            get
            {
                return _TM_Pxtm;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Pxtm)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_PxtmChanging(value);
                _TM_Pxtm = value;
                RaisePropertyChanged("TM_Pxtm");
                OnTM_PxtmChanged();
            }
        }
        private double? _TM_Pxtm;
        partial void OnTM_PxtmChanging(double? value);
        partial void OnTM_PxtmChanged();




        [DataMemberAttribute()]
        public double? TM_RV
        {
            get
            {
                return _TM_RV;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_RV)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_RVChanging(value);
                _TM_RV = value;
                RaisePropertyChanged("TM_RV");
                OnTM_RVChanged();
            }
        }
        private double? _TM_RV;
        partial void OnTM_RVChanging(double? value);
        partial void OnTM_RVChanged();




        [DataMemberAttribute()]
        public double? TM_Ao
        {
            get
            {
                return _TM_Ao;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Ao)
                {
                    Tab1_TM_Changed = true;
                }

                OnTM_AoChanging(value);
                _TM_Ao = value;
                RaisePropertyChanged("TM_Ao");
                OnTM_AoChanged();
            }
        }
        private double? _TM_Ao;
        partial void OnTM_AoChanging(double? value);
        partial void OnTM_AoChanged();




        [DataMemberAttribute()]
        public double? TM_La
        {
            get
            {
                return _TM_La;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_La)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_LaChanging(value);
                _TM_La = value;
                RaisePropertyChanged("TM_La");
                OnTM_LaChanged();
            }
        }
        private double? _TM_La;
        partial void OnTM_LaChanging(double? value);
        partial void OnTM_LaChanged();




        [DataMemberAttribute()]
        public double? TM_Ssa
        {
            get
            {
                return _TM_Ssa;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TM_Ssa)
                {
                    Tab1_TM_Changed = true;
                }
                OnTM_SsaChanging(value);
                _TM_Ssa = value;
                RaisePropertyChanged("TM_Ssa");
                OnTM_SsaChanged();
            }
        }
        private double? _TM_Ssa;
        partial void OnTM_SsaChanging(double? value);
        partial void OnTM_SsaChanged();


        public bool Tab2_2D_Changed = false;

        [DataMemberAttribute()]
        public bool Tab2_2D_Update_Required = false;

        [DataMemberAttribute()]
        public long V_2D_Situs
        {
            get
            {
                return _V_2D_Situs;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_2D_Situs)
                {
                    Tab2_2D_Changed = true;
                }
                OnV_2D_SitusChanging(value);
                if (_V_2D_Situs!=value)
                {
                    _V_2D_Situs = value;
                    RaisePropertyChanged("V_2D_Situs");

                    if (_L2D_Situs != null)
                    {
                        _L2D_Situs.LookupID = V_2D_Situs;
                    }
                }
                OnV_2D_SitusChanged();
            }
        }
        private long _V_2D_Situs;
        partial void OnV_2D_SitusChanging(long value);
        partial void OnV_2D_SitusChanged();


        [DataMemberAttribute()]
        public Lookup L2D_Situs
        {
            get
            {
                return _L2D_Situs;
            }
            set
            {
                OnL2D_SitusChanging(value);
                if (_L2D_Situs != value)
                {
                    bool bNotifyRelatedFields = false;
                    // TxD 24/05/2015: Changing from Solitus to something else
                    if (_L2D_Situs != null && _L2D_Situs.LookupID == (long)AllLookupValues.V_EchoCardio_2D_Situs.V_Solitus && value.LookupID != (long)AllLookupValues.V_EchoCardio_2D_Situs.V_Solitus)
                    {
                        TwoD_Ivc = "";
                        TwoD_Svc = "";
                        TwoD_Pv = "";
                        bNotifyRelatedFields = true;
                    }
                     _L2D_Situs = value;
                    RaisePropertyChanged("L2D_Situs");
                    OnL2D_SitusChanged();

                    // TxD 24/05/2015: Changing to be Solitus so SET some default values for the following
                    if (_L2D_Situs != null && _L2D_Situs.LookupID == (long)AllLookupValues.V_EchoCardio_2D_Situs.V_Solitus)
                    {
                        TwoD_Ivc = "Vào nhĩ phải";
                        TwoD_Svc = "Vào nhĩ phải";
                        TwoD_Pv = "Vào nhĩ trái";
                        bNotifyRelatedFields = true;
                    }

                    if (bNotifyRelatedFields)
                    {
                        RaisePropertyChanged("TwoD_Ivc");
                        RaisePropertyChanged("TwoD_Svc");
                        RaisePropertyChanged("TwoD_Pv");
                    }

                    if (L2D_Situs != null)
                    {
                        _V_2D_Situs = L2D_Situs.LookupID;
                    }   
                }
            }
        }
        private Lookup _L2D_Situs;
        partial void OnL2D_SitusChanging(Lookup value);
        partial void OnL2D_SitusChanged();


        [DataMemberAttribute()]
        public string TwoD_Veins
        {
            get
            {
                return _TwoD_Veins;
            }
            set
            {
                OnTwoD_VeinsChanging(value);
                _TwoD_Veins = value;
                RaisePropertyChanged("TwoD_Veins");
                OnTwoD_VeinsChanged();
            }
        }
        private string _TwoD_Veins;
        partial void OnTwoD_VeinsChanging(string value);
        partial void OnTwoD_VeinsChanged();


        [DataMemberAttribute()]
        public string TwoD_Ivc
        {
            get
            {
                return _TwoD_Ivc;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Ivc)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_IvcChanging(value);
                _TwoD_Ivc = value;
                RaisePropertyChanged("TwoD_Ivc");
                OnTwoD_IvcChanged();
            }
        }
        private string _TwoD_Ivc;
        partial void OnTwoD_IvcChanging(string value);
        partial void OnTwoD_IvcChanged();




        [DataMemberAttribute()]
        public string TwoD_Svc
        {
            get
            {
                return _TwoD_Svc;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Svc)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_SvcChanging(value);
                _TwoD_Svc = value;
                RaisePropertyChanged("TwoD_Svc");
                OnTwoD_SvcChanged();
            }
        }
        private string _TwoD_Svc;
        partial void OnTwoD_SvcChanging(string value);
        partial void OnTwoD_SvcChanged();


        [DataMemberAttribute()]
        public string TwoD_Tvi
        {
            get
            {
                return _TwoD_Tvi;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Tvi)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_TviChanging(value);
                _TwoD_Tvi = value;
                RaisePropertyChanged("TwoD_Tvi");
                OnTwoD_TviChanged();
            }
        }
        private string _TwoD_Tvi;
        partial void OnTwoD_TviChanging(string value);
        partial void OnTwoD_TviChanged();


        [DataMemberAttribute()]
        public long V_2D_Lsvc
        {
            get
            {
                return _V_2D_Lsvc;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_2D_Lsvc)
                {
                    Tab2_2D_Changed = true;
                }
                OnV_2D_LsvcChanging(value);
                _V_2D_Lsvc = value;
                RaisePropertyChanged("V_2D_Lsvc");
                OnV_2D_LsvcChanged();
            }
        }
        private long _V_2D_Lsvc;
        partial void OnV_2D_LsvcChanging(long value);
        partial void OnV_2D_LsvcChanged();


        [DataMemberAttribute()]
        public short V_2D_Azygos
        {
            get
            {
                return _V_2D_Azygos;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_2D_Azygos)
                {
                    Tab2_2D_Changed = true;
                }
                OnV_2D_AzygosChanging(value);
                _V_2D_Azygos = value;
                RaisePropertyChanged("V_2D_Azygos");
                OnV_2D_AzygosChanged();
            }
        }
        private short _V_2D_Azygos;
        partial void OnV_2D_AzygosChanging(short value);
        partial void OnV_2D_AzygosChanged();

        [DataMemberAttribute()]
        public string TwoD_Pv
        {
            get
            {
                return _TwoD_Pv;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Pv)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_PvChanging(value);
                _TwoD_Pv = value;
                RaisePropertyChanged("TwoD_Pv");
                OnTwoD_PvChanged();
            }
        }
        private string _TwoD_Pv;
        partial void OnTwoD_PvChanging(string value);
        partial void OnTwoD_PvChanged();


        [DataMemberAttribute()]
        public string TwoD_Azygos
        {
            get
            {
                return _TwoD_Azygos;
            }
            set
            {
                OnTwoD_AzygosChanging(value);
                _TwoD_Azygos = value;
                RaisePropertyChanged("TwoD_Azygos");
                OnTwoD_AzygosChanged();
            }
        }
        private string _TwoD_Azygos;
        partial void OnTwoD_AzygosChanging(string value);
        partial void OnTwoD_AzygosChanged();


        [DataMemberAttribute()]
        public string TwoD_Atria
        {
            get
            {
                return _TwoD_Atria;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Atria)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_AtriaChanging(value);
                _TwoD_Atria = value;
                RaisePropertyChanged("TwoD_Atria");
                OnTwoD_AtriaChanged();
            }
        }
        private string _TwoD_Atria;
        partial void OnTwoD_AtriaChanging(string value);
        partial void OnTwoD_AtriaChanged();


        [DataMemberAttribute()]
        public string TwoD_Valves
        {
            get
            {
                return _TwoD_Valves;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Valves)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_ValvesChanging(value);
                _TwoD_Valves = value;
                RaisePropertyChanged("TwoD_Valves");
                OnTwoD_ValvesChanged();
            }
        }
        private string _TwoD_Valves;
        partial void OnTwoD_ValvesChanging(string value);
        partial void OnTwoD_ValvesChanged();


        [DataMemberAttribute()]
        public double? TwoD_Cd
        {
            get
            {
                return _TwoD_Cd;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Cd)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_CdChanging(value);
                _TwoD_Cd = value;
                RaisePropertyChanged("TwoD_Cd");
                OnTwoD_CdChanged();
            }
        }
        private double? _TwoD_Cd;
        partial void OnTwoD_CdChanging(double? value);
        partial void OnTwoD_CdChanged();


        [DataMemberAttribute()]
        public double? TwoD_Ma
        {
            get
            {
                return _TwoD_Ma;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Ma)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_MaChanging(value);
                _TwoD_Ma = value;
                RaisePropertyChanged("TwoD_Ma");
                OnTwoD_MaChanged();
            }
        }
        private double? _TwoD_Ma;
        partial void OnTwoD_MaChanging(double? value);
        partial void OnTwoD_MaChanged();


        [DataMemberAttribute()]
        public double? TwoD_MitralArea
        {
            get
            {
                return _TwoD_MitralArea;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_MitralArea)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_MitralAreaChanging(value);
                _TwoD_MitralArea = value;
                RaisePropertyChanged("TwoD_MitralArea");
                OnTwoD_MitralAreaChanged();
            }
        }
        private double? _TwoD_MitralArea;
        partial void OnTwoD_MitralAreaChanging(double? value);
        partial void OnTwoD_MitralAreaChanged();


        [DataMemberAttribute()]
        public double? TwoD_Ta
        {
            get
            {
                return _TwoD_Ta;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Ta)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_TaChanging(value);
                _TwoD_Ta = value;
                RaisePropertyChanged("TwoD_Ta");
                OnTwoD_TaChanged();
            }
        }
        private double? _TwoD_Ta;
        partial void OnTwoD_TaChanging(double? value);
        partial void OnTwoD_TaChanged();

        
        [DataMemberAttribute()]
        public bool TwoD_LSVC
        {
            get
            {
                return _TwoD_LSVC;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_LSVC)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_LSVCChanging(value);
                _TwoD_LSVC = value;
                RaisePropertyChanged("TwoD_LSVC");
                OnTwoD_LSVCChanged();
            }
        }
        private bool _TwoD_LSVC;
        partial void OnTwoD_LSVCChanging(bool value);
        partial void OnTwoD_LSVCChanged();


        [DataMemberAttribute()]
        public string TwoD_Ventricles
        {
            get
            {
                return _TwoD_Ventricles;
            }
            set
            {
                OnTwoD_VentriclesChanging(value);
                _TwoD_Ventricles = value;
                RaisePropertyChanged("TwoD_Ventricles");
                OnTwoD_VentriclesChanged();
            }
        }
        private string _TwoD_Ventricles;
        partial void OnTwoD_VentriclesChanging(string value);
        partial void OnTwoD_VentriclesChanged();


        [DataMemberAttribute()]
        public string TwoD_Aorte
        {
            get
            {
                return _TwoD_Aorte;
            }
            set
            {
                OnTwoD_AorteChanging(value);
                _TwoD_Aorte = value;
                RaisePropertyChanged("TwoD_Aorte");
                OnTwoD_AorteChanged();
            }
        }
        private string _TwoD_Aorte;
        partial void OnTwoD_AorteChanging(string value);
        partial void OnTwoD_AorteChanged();


        [DataMemberAttribute()]
        public double? TwoD_Asc
        {
            get
            {
                return _TwoD_Asc;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Asc)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_AscChanging(value);
                _TwoD_Asc = value;
                RaisePropertyChanged("TwoD_Asc");
                OnTwoD_AscChanged();
            }
        }
        private double? _TwoD_Asc;
        partial void OnTwoD_AscChanging(double? value);
        partial void OnTwoD_AscChanged();


        [DataMemberAttribute()]
        public double? TwoD_Cr
        {
            get
            {
                return _TwoD_Cr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Cr)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_CrChanging(value);
                _TwoD_Cr = value;
                RaisePropertyChanged("TwoD_Cr");
                OnTwoD_CrChanged();
            }
        }
        private double? _TwoD_Cr;
        partial void OnTwoD_CrChanging(double? value);
        partial void OnTwoD_CrChanged();


        [DataMemberAttribute()]
        public double? TwoD_Is
        {
            get
            {
                return _TwoD_Is;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Is)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_IsChanging(value);
                _TwoD_Is = value;
                RaisePropertyChanged("TwoD_Is");
                OnTwoD_IsChanged();
            }
        }
        private double? _TwoD_Is;
        partial void OnTwoD_IsChanging(double? value);
        partial void OnTwoD_IsChanged();


        [DataMemberAttribute()]
        public double? TwoD_Abd
        {
            get
            {
                return _TwoD_Abd;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Abd)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_AbdChanging(value);
                _TwoD_Abd = value;
                RaisePropertyChanged("TwoD_Abd");
                OnTwoD_AbdChanged();
            }
        }
        private double? _TwoD_Abd;
        partial void OnTwoD_AbdChanging(double? value);
        partial void OnTwoD_AbdChanged();


        [DataMemberAttribute()]
        public double? TwoD_D2
        {
            get
            {
                return _TwoD_D2;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_D2)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_D2Changing(value);
                _TwoD_D2 = value;
                RaisePropertyChanged("TwoD_D2");
                OnTwoD_D2Changed();
            }
        }
        private double? _TwoD_D2;
        partial void OnTwoD_D2Changing(double? value);
        partial void OnTwoD_D2Changed();


        [DataMemberAttribute()]
        public double? TwoD_Ann
        {
            get
            {
                return _TwoD_Ann;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Ann)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_AnnChanging(value);
                _TwoD_Ann = value;
                RaisePropertyChanged("TwoD_Ann");
                OnTwoD_AnnChanged();
            }
        }
        private double? _TwoD_Ann;
        partial void OnTwoD_AnnChanging(double? value);
        partial void OnTwoD_AnnChanged();


        [DataMemberAttribute()]
        public double? TwoD_Tap
        {
            get
            {
                return _TwoD_Tap;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != TwoD_Tap)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_TapChanging(value);
                _TwoD_Tap = value;
                RaisePropertyChanged("TwoD_Tap");
                OnTwoD_TapChanged();
            }
        }
        private double? _TwoD_Tap;
        partial void OnTwoD_TapChanging(double? value);
        partial void OnTwoD_TapChanged();


        [DataMemberAttribute()]
        public double? TwoD_Rpa
        {
            get
            {
                return _TwoD_Rpa;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Rpa)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_RpaChanging(value);
                _TwoD_Rpa = value;
                RaisePropertyChanged("TwoD_Rpa");
                OnTwoD_RpaChanged();
            }
        }
        private double? _TwoD_Rpa;
        partial void OnTwoD_RpaChanging(double? value);
        partial void OnTwoD_RpaChanged();


        [DataMemberAttribute()]
        public double? TwoD_Lpa
        {
            get
            {
                return _TwoD_Lpa;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Lpa)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_LpaChanging(value);
                _TwoD_Lpa = value;
                RaisePropertyChanged("TwoD_Lpa");
                OnTwoD_LpaChanged();
            }
        }
        private double? _TwoD_Lpa;
        partial void OnTwoD_LpaChanging(double? value);
        partial void OnTwoD_LpaChanged();


        [DataMemberAttribute()]
        public string TwoD_Pericarde
        {
            get
            {
                return _TwoD_Pericarde;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Pericarde)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_PericardeChanging(value);
                _TwoD_Pericarde = value;
                RaisePropertyChanged("TwoD_Pericarde");
                OnTwoD_PericardeChanged();
            }
        }
        private string _TwoD_Pericarde;
        partial void OnTwoD_PericardeChanging(string value);
        partial void OnTwoD_PericardeChanged();


        [DataMemberAttribute()]
        public string TwoD_Pa
        {
            get
            {
                return _TwoD_Pa;
            }
            set
            {
                OnTwoD_PaChanging(value);
                _TwoD_Pa = value;
                RaisePropertyChanged("TwoD_Pa");
                OnTwoD_PaChanged();
            }
        }
        private string _TwoD_Pa;
        partial void OnTwoD_PaChanging(string value);
        partial void OnTwoD_PaChanged();


        [DataMemberAttribute()]
        public string TwoD_Others
        {
            get
            {
                return _TwoD_Others;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _TwoD_Others)
                {
                    Tab2_2D_Changed = true;
                }
                OnTwoD_OthersChanging(value);
                _TwoD_Others = value;
                RaisePropertyChanged("TwoD_Others");
                OnTwoD_OthersChanged();
            }
        }
        private string _TwoD_Others;
        partial void OnTwoD_OthersChanging(string value);
        partial void OnTwoD_OthersChanged();


        public bool Tab3_Doppler_Changed = false;

        [DataMemberAttribute()]
        public bool Tab3_Doppler_Update_Required = false;

        [DataMemberAttribute()]
        public double? DOPPLER_Mitral_VelMax
        {
            get
            {
                return _DOPPLER_Mitral_VelMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_VelMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_VelMaxChanging(value);
                _DOPPLER_Mitral_VelMax = value;
                /*==== #001 ====*/
                if (_DOPPLER_Mitral_VelMax.HasValue && _DOPPLER_Mitral_VelMax > 0)
                {
                    DOPPLER_Mitral_GdMax = _DOPPLER_Mitral_VelMax.Value * _DOPPLER_Mitral_VelMax.Value * 4;
                }
                /*==== #001 ====*/
                RaisePropertyChanged("DOPPLER_Mitral_VelMax");
                OnDOPPLER_Mitral_VelMaxChanged();
            }
        }
        private double? _DOPPLER_Mitral_VelMax;
        partial void OnDOPPLER_Mitral_VelMaxChanging(double? value);
        partial void OnDOPPLER_Mitral_VelMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Mitral_GdMax
        {
            get
            {
                return _DOPPLER_Mitral_GdMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_GdMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_GdMaxChanging(value);
                _DOPPLER_Mitral_GdMax = value;
                RaisePropertyChanged("DOPPLER_Mitral_GdMax");
                OnDOPPLER_Mitral_GdMaxChanged();
            }
        }
        private double? _DOPPLER_Mitral_GdMax;
        partial void OnDOPPLER_Mitral_GdMaxChanging(double? value);
        partial void OnDOPPLER_Mitral_GdMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Mitral_Ms
        {
            get
            {
                return _DOPPLER_Mitral_Ms;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_Ms)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_MsChanging(value);
                _DOPPLER_Mitral_Ms = value;
                RaisePropertyChanged("DOPPLER_Mitral_Ms");
                OnDOPPLER_Mitral_MsChanged();
            }
        }
        private double? _DOPPLER_Mitral_Ms;
        partial void OnDOPPLER_Mitral_MsChanging(double? value);
        partial void OnDOPPLER_Mitral_MsChanged();


        [DataMemberAttribute()]
        public bool V_DOPPLER_Mitral_Mr
        {
            get
            {
                return _V_DOPPLER_Mitral_Mr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Mitral_Mr)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnV_DOPPLER_Mitral_MrChanging(value);
                _V_DOPPLER_Mitral_Mr = value;
                RaisePropertyChanged("V_DOPPLER_Mitral_Mr");
                OnV_DOPPLER_Mitral_MrChanged();

                if (!_V_DOPPLER_Mitral_Mr)
                {
                    LDOPPLER_Mitral_Grade = null;
                }
            }
        }
        private bool _V_DOPPLER_Mitral_Mr;
        partial void OnV_DOPPLER_Mitral_MrChanging(bool value);
        partial void OnV_DOPPLER_Mitral_MrChanged();


        [DataMemberAttribute()]
        public string DOPPLER_Mitral_Ea
        {
            get
            {
                return _DOPPLER_Mitral_Ea;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_Ea)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_EaChanging(value);
                if (_DOPPLER_Mitral_Ea != value)
                {
                    _DOPPLER_Mitral_Ea = value;
                    RaisePropertyChanged("DOPPLER_Mitral_Ea");
                    OnDOPPLER_Mitral_EaChanged();                    
                }
            }
        }
        private string _DOPPLER_Mitral_Ea;
        partial void OnDOPPLER_Mitral_EaChanging(string value);
        partial void OnDOPPLER_Mitral_EaChanged();


        [DataMemberAttribute()]
        public int V_DOPPLER_Mitral_Ea
        {
            get
            {
                return _V_DOPPLER_Mitral_Ea;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Mitral_Ea)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnV_DOPPLER_Mitral_EaChanging(value);
                if (_V_DOPPLER_Mitral_Ea != value)
                {
                    _V_DOPPLER_Mitral_Ea = value;
                    RaisePropertyChanged("V_DOPPLER_Mitral_Ea");
                    OnV_DOPPLER_Mitral_EaChanged();
                    _LDOPPLER_Mitral_Ea = (AllLookupValues.ChoiceEnum)V_DOPPLER_Mitral_Ea;    
                }
                
            }
        }
        private int _V_DOPPLER_Mitral_Ea;
        partial void OnV_DOPPLER_Mitral_EaChanging(int value);
        partial void OnV_DOPPLER_Mitral_EaChanged();

        [DataMemberAttribute()]
        public AllLookupValues.ChoiceEnum LDOPPLER_Mitral_Ea
        {
            get
            {
                return _LDOPPLER_Mitral_Ea;
            }
            set
            {
                OnLDOPPLER_Mitral_EaChanging(value);
                if (_LDOPPLER_Mitral_Ea != value)
                {
                    _LDOPPLER_Mitral_Ea = value;
                    RaisePropertyChanged("LDOPPLER_Mitral_Ea");
                    OnLDOPPLER_Mitral_EaChanged();
                    _V_DOPPLER_Mitral_Ea = (int)LDOPPLER_Mitral_Ea;
                }
            }
        }
        private AllLookupValues.ChoiceEnum _LDOPPLER_Mitral_Ea;
        partial void OnLDOPPLER_Mitral_EaChanging(AllLookupValues.ChoiceEnum value);
        partial void OnLDOPPLER_Mitral_EaChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Mitral_Moy
        {
            get
            {
                return _DOPPLER_Mitral_Moy;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_Moy)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_MoyChanging(value);
                _DOPPLER_Mitral_Moy = value;
                RaisePropertyChanged("DOPPLER_Mitral_Moy");
                OnDOPPLER_Mitral_MoyChanged();
            }
        }
        private double? _DOPPLER_Mitral_Moy;
        partial void OnDOPPLER_Mitral_MoyChanging(double? value);
        partial void OnDOPPLER_Mitral_MoyChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Mitral_Sm
        {
            get
            {
                return _DOPPLER_Mitral_Sm;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Mitral_Sm)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Mitral_SmChanging(value);
                _DOPPLER_Mitral_Sm = value;
                RaisePropertyChanged("DOPPLER_Mitral_Sm");
                OnDOPPLER_Mitral_SmChanged();
            }
        }
        private double? _DOPPLER_Mitral_Sm;
        partial void OnDOPPLER_Mitral_SmChanging(double? value);
        partial void OnDOPPLER_Mitral_SmChanged();


        //[DataMemberAttribute()]
        //public long V_DOPPLER_Mitral_Grade
        //{
        //    get
        //    {
        //        return _V_DOPPLER_Mitral_Grade;
        //    }
        //    set
        //    {
        //        if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Mitral_Grade)
        //        {
        //            Tab3_Doppler_Changed = true;
        //        }
        //        OnV_DOPPLER_Mitral_GradeChanging(value);
        //        _V_DOPPLER_Mitral_Grade = value;
        //        RaisePropertyChanged("V_DOPPLER_Mitral_Grade");
        //        OnV_DOPPLER_Mitral_GradeChanged();
        //        if (LDOPPLER_Mitral_Grade != null)
        //        {
        //            _LDOPPLER_Mitral_Grade.LookupID = V_DOPPLER_Mitral_Grade;
        //        }
        //    }
        //}
        //private long _V_DOPPLER_Mitral_Grade;
        //partial void OnV_DOPPLER_Mitral_GradeChanging(long value);
        //partial void OnV_DOPPLER_Mitral_GradeChanged();

        [DataMemberAttribute()]
        public Lookup LDOPPLER_Mitral_Grade
        {
            get
            {
                return _LDOPPLER_Mitral_Grade;
            }
            set
            {
                OnLDOPPLER_Mitral_GradeChanging(value);
                _LDOPPLER_Mitral_Grade = value;
                RaisePropertyChanged("LDOPPLER_Mitral_Grade");
                OnLDOPPLER_Mitral_GradeChanged();
                //if (LDOPPLER_Mitral_Grade!=null)
                //{
                //    _V_DOPPLER_Mitral_Grade = LDOPPLER_Mitral_Grade.LookupID;
                //}
            }
        }
        private Lookup _LDOPPLER_Mitral_Grade;
        partial void OnLDOPPLER_Mitral_GradeChanging(Lookup value);
        partial void OnLDOPPLER_Mitral_GradeChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_VelMax
        {
            get
            {
                return _DOPPLER_Aortic_VelMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_VelMax)
                {
                    Tab3_Doppler_Changed = true;
                }

                OnDOPPLER_Aortic_VelMaxChanging(value);
                _DOPPLER_Aortic_VelMax = value;
                /*==== #001 ====*/
                if (_DOPPLER_Aortic_VelMax.HasValue && _DOPPLER_Aortic_VelMax > 0)
                {
                    DOPPLER_Aortic_GdMax = _DOPPLER_Aortic_VelMax.Value * _DOPPLER_Aortic_VelMax.Value * 4;
                }
                /*==== #001 ====*/
                RaisePropertyChanged("DOPPLER_Aortic_VelMax");
                OnDOPPLER_Aortic_VelMaxChanged();
            }
        }
        private double? _DOPPLER_Aortic_VelMax;
        partial void OnDOPPLER_Aortic_VelMaxChanging(double? value);
        partial void OnDOPPLER_Aortic_VelMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_GdMax
        {
            get
            {
                return _DOPPLER_Aortic_GdMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_GdMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_GdMaxChanging(value);
                _DOPPLER_Aortic_GdMax = value;
                RaisePropertyChanged("DOPPLER_Aortic_GdMax");
                OnDOPPLER_Aortic_GdMaxChanged();
            }
        }
        private double? _DOPPLER_Aortic_GdMax;
        partial void OnDOPPLER_Aortic_GdMaxChanging(double? value);
        partial void OnDOPPLER_Aortic_GdMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_As
        {
            get
            {
                return _DOPPLER_Aortic_As;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_As)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_AsChanging(value);
                _DOPPLER_Aortic_As = value;
                RaisePropertyChanged("DOPPLER_Aortic_As");
                OnDOPPLER_Aortic_AsChanged();
            }
        }
        private double? _DOPPLER_Aortic_As;
        partial void OnDOPPLER_Aortic_AsChanging(double? value);
        partial void OnDOPPLER_Aortic_AsChanged();


        [DataMemberAttribute()]
        public bool V_DOPPLER_Aortic_Ar
        {
            get
            {
                return _V_DOPPLER_Aortic_Ar;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Aortic_Ar)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnV_DOPPLER_Aortic_ArChanging(value);
                _V_DOPPLER_Aortic_Ar = value;
                RaisePropertyChanged("V_DOPPLER_Aortic_Ar");
                OnV_DOPPLER_Aortic_ArChanged();

                if (!_V_DOPPLER_Aortic_Ar)
                {
                    LDOPPLER_Aortic_Grade = null;
                }
            }
        }
        private bool _V_DOPPLER_Aortic_Ar;
        partial void OnV_DOPPLER_Aortic_ArChanging(bool value);
        partial void OnV_DOPPLER_Aortic_ArChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_Moy
        {
            get
            {
                return _DOPPLER_Aortic_Moy;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_Moy)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_MoyChanging(value);
                _DOPPLER_Aortic_Moy = value;
                RaisePropertyChanged("DOPPLER_Aortic_Moy");
                OnDOPPLER_Aortic_MoyChanged();
            }
        }
        private double? _DOPPLER_Aortic_Moy;
        partial void OnDOPPLER_Aortic_MoyChanging(double? value);
        partial void OnDOPPLER_Aortic_MoyChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_SAo
        {
            get
            {
                return _DOPPLER_Aortic_SAo;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_SAo)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_SAoChanging(value);
                _DOPPLER_Aortic_SAo = value;
                RaisePropertyChanged("DOPPLER_Aortic_SAo");
                OnDOPPLER_Aortic_SAoChanged();
            }
        }
        private double? _DOPPLER_Aortic_SAo;
        partial void OnDOPPLER_Aortic_SAoChanging(double? value);
        partial void OnDOPPLER_Aortic_SAoChanged();


        //[DataMemberAttribute()]
        //public long V_DOPPLER_Aortic_Grade
        //{
        //    get
        //    {
        //        return _V_DOPPLER_Aortic_Grade;
        //    }
        //    set
        //    {
        //        if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Aortic_Grade)
        //        {
        //            Tab3_Doppler_Changed = true;
        //        }
        //        OnV_DOPPLER_Aortic_GradeChanging(value);
        //        _V_DOPPLER_Aortic_Grade = value;
        //        RaisePropertyChanged("V_DOPPLER_Aortic_Grade");
        //        OnV_DOPPLER_Aortic_GradeChanged();
        //        if (LDOPPLER_Aortic_Grade != null)
        //        {
        //            _LDOPPLER_Aortic_Grade.LookupID = V_DOPPLER_Aortic_Grade;
        //        }
        //    }
        //}
        //private long _V_DOPPLER_Aortic_Grade;
        //partial void OnV_DOPPLER_Aortic_GradeChanging(long value);
        //partial void OnV_DOPPLER_Aortic_GradeChanged();

        [DataMemberAttribute()]
        public Lookup LDOPPLER_Aortic_Grade
        {
            get
            {
                return _LDOPPLER_Aortic_Grade;
            }
            set
            {
                OnLDOPPLER_Aortic_GradeChanging(value);
                _LDOPPLER_Aortic_Grade = value;
                RaisePropertyChanged("LDOPPLER_Aortic_Grade");
                OnLDOPPLER_Aortic_GradeChanged();
                //if (LDOPPLER_Aortic_Grade != null)
                //{
                //    _V_DOPPLER_Aortic_Grade = LDOPPLER_Aortic_Grade.LookupID;
                //}
            }
        }
        private Lookup _LDOPPLER_Aortic_Grade;
        partial void OnLDOPPLER_Aortic_GradeChanging(Lookup value);
        partial void OnLDOPPLER_Aortic_GradeChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_PHT
        {
            get
            {
                return _DOPPLER_Aortic_PHT;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_PHT)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_PHTChanging(value);
                _DOPPLER_Aortic_PHT = value;
                RaisePropertyChanged("DOPPLER_Aortic_PHT");
                OnDOPPLER_Aortic_PHTChanged();
            }
        }
        private double? _DOPPLER_Aortic_PHT;
        partial void OnDOPPLER_Aortic_PHTChanging(double? value);
        partial void OnDOPPLER_Aortic_PHTChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_Dfo
        {
            get
            {
                return _DOPPLER_Aortic_Dfo;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_Dfo)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_DfoChanging(value);
                _DOPPLER_Aortic_Dfo = value;
                RaisePropertyChanged("DOPPLER_Aortic_Dfo");
                OnDOPPLER_Aortic_DfoChanged();
            }
        }
        private double? _DOPPLER_Aortic_Dfo;
        partial void OnDOPPLER_Aortic_DfoChanging(double? value);
        partial void OnDOPPLER_Aortic_DfoChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_Edtd
        {
            get
            {
                return _DOPPLER_Aortic_Edtd;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_Edtd)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_EdtdChanging(value);
                _DOPPLER_Aortic_Edtd = value;
                RaisePropertyChanged("DOPPLER_Aortic_Edtd");
                OnDOPPLER_Aortic_EdtdChanged();
            }
        }
        private double? _DOPPLER_Aortic_Edtd;
        partial void OnDOPPLER_Aortic_EdtdChanging(double? value);
        partial void OnDOPPLER_Aortic_EdtdChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Aortic_ExtSpat
        {
            get
            {
                return _DOPPLER_Aortic_ExtSpat;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Aortic_ExtSpat)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Aortic_ExtSpatChanging(value);
                _DOPPLER_Aortic_ExtSpat = value;
                RaisePropertyChanged("DOPPLER_Aortic_ExtSpat");
                OnDOPPLER_Aortic_ExtSpatChanged();
            }
        }
        private double? _DOPPLER_Aortic_ExtSpat;
        partial void OnDOPPLER_Aortic_ExtSpatChanging(double? value);
        partial void OnDOPPLER_Aortic_ExtSpatChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Tricuspid_VelMax
        {
            get
            {
                return _DOPPLER_Tricuspid_VelMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Tricuspid_VelMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Tricuspid_VelMaxChanging(value);
                _DOPPLER_Tricuspid_VelMax = value;
                /*==== #001 ====*/
                if (_DOPPLER_Tricuspid_VelMax.HasValue && _DOPPLER_Tricuspid_VelMax.Value > 0)
                {
                    DOPPLER_Tricuspid_GdMax = _DOPPLER_Tricuspid_VelMax.Value * _DOPPLER_Tricuspid_VelMax.Value * 4;
                }
                /*==== #001 ====*/
                RaisePropertyChanged("DOPPLER_Tricuspid_VelMax");
                OnDOPPLER_Tricuspid_VelMaxChanged();
            }
        }
        private double? _DOPPLER_Tricuspid_VelMax;
        partial void OnDOPPLER_Tricuspid_VelMaxChanging(double? value);
        partial void OnDOPPLER_Tricuspid_VelMaxChanged();


        [DataMemberAttribute()]
        public bool V_DOPPLER_Tricuspid_Tr
        {
            get
            {
                return _V_DOPPLER_Tricuspid_Tr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Tricuspid_Tr)
                {
                    Tab3_Doppler_Changed = true;
                }

                OnV_DOPPLER_Tricuspid_TrChanging(value);
                _V_DOPPLER_Tricuspid_Tr = value;
                RaisePropertyChanged("V_DOPPLER_Tricuspid_Tr");
                OnV_DOPPLER_Tricuspid_TrChanged();

                if (!_V_DOPPLER_Tricuspid_Tr)
                {
                    LDOPPLER_Tricuspid_Grade = null;
                }
            }
        }
        private bool _V_DOPPLER_Tricuspid_Tr;
        partial void OnV_DOPPLER_Tricuspid_TrChanging(bool value);
        partial void OnV_DOPPLER_Tricuspid_TrChanged();


        //[DataMemberAttribute()]
        //public long V_DOPPLER_Tricuspid_Grade
        //{
        //    get
        //    {
        //        return _V_DOPPLER_Tricuspid_Grade;
        //    }
        //    set
        //    {
        //        if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Tricuspid_Grade)
        //        {
        //            Tab3_Doppler_Changed = true;
        //        }
        //        OnV_DOPPLER_Tricuspid_GradeChanging(value);
        //        _V_DOPPLER_Tricuspid_Grade = value;
        //        RaisePropertyChanged("V_DOPPLER_Tricuspid_Grade");
        //        OnV_DOPPLER_Tricuspid_GradeChanged();
        //    }
        //}
        //private long _V_DOPPLER_Tricuspid_Grade;
        //partial void OnV_DOPPLER_Tricuspid_GradeChanging(long value);
        //partial void OnV_DOPPLER_Tricuspid_GradeChanged();


        [DataMemberAttribute()]
        public Lookup LDOPPLER_Tricuspid_Grade
        {
            get
            {
                return _LDOPPLER_Tricuspid_Grade;
            }
            set
            {
                OnLDOPPLER_Tricuspid_GradeChanging(value);
                _LDOPPLER_Tricuspid_Grade = value;
                RaisePropertyChanged("LDOPPLER_Tricuspid_Grade");
                OnLDOPPLER_Tricuspid_GradeChanged();
                //if (LDOPPLER_Tricuspid_Grade != null)
                //{
                //    _V_DOPPLER_Tricuspid_Grade = LDOPPLER_Tricuspid_Grade.LookupID;
                //}
            }
        }
        private Lookup _LDOPPLER_Tricuspid_Grade;
        partial void OnLDOPPLER_Tricuspid_GradeChanging(Lookup value);
        partial void OnLDOPPLER_Tricuspid_GradeChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Tricuspid_GdMax
        {
            get
            {
                return _DOPPLER_Tricuspid_GdMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Tricuspid_GdMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Tricuspid_GdMaxChanging(value);
                _DOPPLER_Tricuspid_GdMax = value;
                RaisePropertyChanged("DOPPLER_Tricuspid_GdMax");
                OnDOPPLER_Tricuspid_GdMaxChanged();
            }
        }
        private double? _DOPPLER_Tricuspid_GdMax;
        partial void OnDOPPLER_Tricuspid_GdMaxChanging(double? value);
        partial void OnDOPPLER_Tricuspid_GdMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Tricuspid_Paps
        {
            get
            {
                return _DOPPLER_Tricuspid_Paps;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Tricuspid_Paps)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Tricuspid_PapsChanging(value);
                _DOPPLER_Tricuspid_Paps = value;
                RaisePropertyChanged("DOPPLER_Tricuspid_Paps");
                OnDOPPLER_Tricuspid_PapsChanged();
            }
        }
        private double? _DOPPLER_Tricuspid_Paps;
        partial void OnDOPPLER_Tricuspid_PapsChanging(double? value);
        partial void OnDOPPLER_Tricuspid_PapsChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Tricuspid_Moy
        {
            get
            {
                return _DOPPLER_Tricuspid_Moy;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Tricuspid_Moy)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Tricuspid_MoyChanging(value);
                _DOPPLER_Tricuspid_Moy = value;
                RaisePropertyChanged("DOPPLER_Tricuspid_Moy");
                OnDOPPLER_Tricuspid_MoyChanged();
            }
        }
        private double? _DOPPLER_Tricuspid_Moy;
        partial void OnDOPPLER_Tricuspid_MoyChanging(double? value);
        partial void OnDOPPLER_Tricuspid_MoyChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Pulmonary_VelMax
        {
            get
            {
                return _DOPPLER_Pulmonary_VelMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_VelMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_VelMaxChanging(value);
                _DOPPLER_Pulmonary_VelMax = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_VelMax");
                /*==== #001 ====*/
                if (_DOPPLER_Pulmonary_VelMax > 0)
                {
                    DOPPLER_Pulmonary_GdMax = _DOPPLER_Pulmonary_VelMax * _DOPPLER_Pulmonary_VelMax * 4;
                }
                /*==== #001 ====*/
                OnDOPPLER_Pulmonary_VelMaxChanged();
            }
        }
        private double? _DOPPLER_Pulmonary_VelMax;
        partial void OnDOPPLER_Pulmonary_VelMaxChanging(double? value);
        partial void OnDOPPLER_Pulmonary_VelMaxChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Pulmonary_GdMax
        {
            get
            {
                return _DOPPLER_Pulmonary_GdMax;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_GdMax)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_GdMaxChanging(value);
                _DOPPLER_Pulmonary_GdMax = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_GdMax");
                OnDOPPLER_Pulmonary_GdMaxChanged();
            }
        }
        private double? _DOPPLER_Pulmonary_GdMax;
        partial void OnDOPPLER_Pulmonary_GdMaxChanging(double? value);
        partial void OnDOPPLER_Pulmonary_GdMaxChanged();


        [DataMemberAttribute()]
        public bool V_DOPPLER_Pulmonary_Pr
        {
            get
            {
                return _V_DOPPLER_Pulmonary_Pr;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Pulmonary_Pr)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnV_DOPPLER_Pulmonary_PrChanging(value);
                _V_DOPPLER_Pulmonary_Pr = value;
                RaisePropertyChanged("V_DOPPLER_Pulmonary_Pr");
                OnV_DOPPLER_Pulmonary_PrChanged();

                if (!_V_DOPPLER_Pulmonary_Pr)
                {
                    LDOPPLER_Pulmonary_Grade = null;
                }
            }
        }
        private bool _V_DOPPLER_Pulmonary_Pr;
        partial void OnV_DOPPLER_Pulmonary_PrChanging(bool value);
        partial void OnV_DOPPLER_Pulmonary_PrChanged();


        //[DataMemberAttribute()]
        //public long V_DOPPLER_Pulmonary_Grade
        //{
        //    get
        //    {
        //        return _V_DOPPLER_Pulmonary_Grade;
        //    }
        //    set
        //    {
        //        if (UltraResParams_EchoCardiographyID > 0 && value != _V_DOPPLER_Pulmonary_Grade)
        //        {
        //            Tab3_Doppler_Changed = true;
        //        }
        //        OnV_DOPPLER_Pulmonary_GradeChanging(value);
        //        _V_DOPPLER_Pulmonary_Grade = value;
        //        RaisePropertyChanged("V_DOPPLER_Pulmonary_Grade");
        //        OnV_DOPPLER_Pulmonary_GradeChanged();
        //    }
        //}
        //private long _V_DOPPLER_Pulmonary_Grade;
        //partial void OnV_DOPPLER_Pulmonary_GradeChanging(long value);
        //partial void OnV_DOPPLER_Pulmonary_GradeChanged();

        [DataMemberAttribute()]
        public Lookup LDOPPLER_Pulmonary_Grade
        {
            get
            {
                return _LDOPPLER_Pulmonary_Grade;
            }
            set
            {
                OnLDOPPLER_Pulmonary_GradeChanging(value);
                _LDOPPLER_Pulmonary_Grade = value;
                RaisePropertyChanged("LDOPPLER_Pulmonary_Grade");
                OnLDOPPLER_Pulmonary_GradeChanged();
                //if (LDOPPLER_Pulmonary_Grade != null)
                //{
                //    _V_DOPPLER_Pulmonary_Grade = LDOPPLER_Pulmonary_Grade.LookupID;
                //}
            }
        }
        private Lookup _LDOPPLER_Pulmonary_Grade;
        partial void OnLDOPPLER_Pulmonary_GradeChanging(Lookup value);
        partial void OnLDOPPLER_Pulmonary_GradeChanged();

        [DataMemberAttribute()]
        public double? DOPPLER_Pulmonary_Moy
        {
            get
            {
                return _DOPPLER_Pulmonary_Moy;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_Moy)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_MoyChanging(value);
                _DOPPLER_Pulmonary_Moy = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_Moy");
                OnDOPPLER_Pulmonary_MoyChanged();
            }
        }
        private double? _DOPPLER_Pulmonary_Moy;
        partial void OnDOPPLER_Pulmonary_MoyChanging(double? value);
        partial void OnDOPPLER_Pulmonary_MoyChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Pulmonary_Papm
        {
            get
            {
                return _DOPPLER_Pulmonary_Papm;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_Papm)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_PapmChanging(value);
                _DOPPLER_Pulmonary_Papm = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_Papm");
                OnDOPPLER_Pulmonary_PapmChanged();
            }
        }
        private double? _DOPPLER_Pulmonary_Papm;
        partial void OnDOPPLER_Pulmonary_PapmChanging(double? value);
        partial void OnDOPPLER_Pulmonary_PapmChanged();


        [DataMemberAttribute()]
        public double? DOPPLER_Pulmonary_Papd
        {
            get
            {
                return _DOPPLER_Pulmonary_Papd;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_Papd)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_PapdChanging(value);
                _DOPPLER_Pulmonary_Papd = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_Papd");
                OnDOPPLER_Pulmonary_PapdChanged();
            }
        }
        private double? _DOPPLER_Pulmonary_Papd;
        partial void OnDOPPLER_Pulmonary_PapdChanging(double? value);
        partial void OnDOPPLER_Pulmonary_PapdChanged();


        [DataMemberAttribute()]
        public string DOPPLER_Pulmonary_Orthers
        {
            get
            {
                return _DOPPLER_Pulmonary_Orthers;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _DOPPLER_Pulmonary_Orthers)
                {
                    Tab3_Doppler_Changed = true;
                }
                OnDOPPLER_Pulmonary_OrthersChanging(value);
                _DOPPLER_Pulmonary_Orthers = value;
                RaisePropertyChanged("DOPPLER_Pulmonary_Orthers");
                OnDOPPLER_Pulmonary_OrthersChanged();
            }
        }
        private string _DOPPLER_Pulmonary_Orthers;
        partial void OnDOPPLER_Pulmonary_OrthersChanging(string value);
        partial void OnDOPPLER_Pulmonary_OrthersChanged();


        public bool Tab4_Conclusion_Changed = false;

        [DataMemberAttribute()]
        public bool Tab4_Conclusion_Update_Required = false;

        [DataMemberAttribute()]
        public string Conclusion
        {
            get
            {
                return _Conclusion;
            }
            set
            {
                if (UltraResParams_EchoCardiographyID > 0 && value != _Conclusion)
                {
                    Tab4_Conclusion_Changed = true;
                }
                OnConclusionChanging(value);
                _Conclusion = value;
                RaisePropertyChanged("Conclusion");
                OnConclusionChanged();
            }
        }
        private string _Conclusion;
        partial void OnConclusionChanging(string value);
        partial void OnConclusionChanged();




        [DataMemberAttribute()]
        public long PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                OnPCLImgResultIDChanging(value);
                _PCLImgResultID = value;
                RaisePropertyChanged("PCLImgResultID");
                OnPCLImgResultIDChanged();
            }
        }
        private long _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(long value);
        partial void OnPCLImgResultIDChanged();




        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                OnDoctorStaffIDChanging(value);
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
                OnDoctorStaffIDChanged();
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();


	    [DataMemberAttribute()]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                if (_CreateDate != value)
                {
                    OnCreateDateChanging(value);
                    _CreateDate = value;
                    RaisePropertyChanged("CreateDate");
                    OnCreateDateChanged();
                }
            }
        }
        private DateTime _CreateDate;
        partial void OnCreateDateChanging(DateTime value);
        partial void OnCreateDateChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Staff VStaff
        {
            get
            {
                return _VStaff;
            }
            set
            {
                if (_VStaff != value)
                {
                    OnVStaffChanging(value);
                    _VStaff = value;
                    RaisePropertyChanged("VStaff");
                    OnVStaffChanged();
                }
            }
        }
        private Staff _VStaff;
        partial void OnVStaffChanging(Staff value);
        partial void OnVStaffChanged();
        #endregion


        [DataMemberAttribute()]
        public Int32 TabIndex
        {
            get
            {
                return _TabIndex;
            }
            set
            {
                if (_TabIndex != value)
                {
                    OnTabIndexChanging(value);
                    _TabIndex = value;
                    RaisePropertyChanged("TabIndex");
                    OnTabIndexChanged();
                }
            }
        }
        private Int32 _TabIndex;
        partial void OnTabIndexChanging(Int32 value);
        partial void OnTabIndexChanged();



        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
