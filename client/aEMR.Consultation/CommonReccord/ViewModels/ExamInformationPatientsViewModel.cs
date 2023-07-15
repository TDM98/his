using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using aEMR.DataContracts;
using System.ServiceModel;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Web.WebView2.Wpf;
using System.Net.Http.Headers;
using Microsoft.Web.WebView2.Core;


namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IExamInformationPatients)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExamInformationPatientsViewModel : ViewModelBase, IExamInformationPatients
        , IHandle<InPatientRegistrationSelectedForConsultation>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
       
        [ImportingConstructor]
        public ExamInformationPatientsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
      
            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);

            //LoginPost();

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
        }

        private static string BaseURL = Globals.ServerConfigSection.CommonItems.ExamCovidAPIBaseURL;
        private static string Token = Globals.ServerConfigSection.CommonItems.BearerToken;
        private static string APIGetHistory = Globals.ServerConfigSection.CommonItems.ExamCovidAPIGetHistory;
        private static string APIGetPreview = Globals.ServerConfigSection.CommonItems.ExamCovidAPIGetPrintPreview;
        private WebView2 _webView;
        public WebView2 webView
        {
            get => _webView;
            set
            {
                _webView = value;
                NotifyOfPropertyChange(() => webView);
            }
        }
        private string _WebViewSource;
        public string WebViewSource
        {
            get => _WebViewSource;
            set
            {
                _WebViewSource = value;
                NotifyOfPropertyChange(() => WebViewSource);
            }
        }

        public void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            webView = (WebView2)sender;
        }
        #region Handle
        public void Handle(InPatientRegistrationSelectedForConsultation message)
        {
            Initwebview();
            if (message != null)
            {
                CheckHistoryByPatientCode(message.Source.Patient.PatientCode);
            }
            //NavigateAsync();
        }
        private void Initwebview()
        {
            if(webView!= null)
            {
                if (webView.CoreWebView2 == null)
                {
                    webView.EnsureCoreWebView2Async();
                }
                else
                {
                    webView.CoreWebView2.NavigateToString("");
                }
            }
        }
        #endregion
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        private ObservableCollection<HistoryExamInfomation> _HistoryExamInfomationList;
        public ObservableCollection<HistoryExamInfomation> HistoryExamInfomationList
        {
            get => _HistoryExamInfomationList;
            set
            {
                _HistoryExamInfomationList = value;
                NotifyOfPropertyChange(() => HistoryExamInfomationList);
            }
        }
        private HistoryExamInfomation _SelectedHistoryExamInfomation;
        public HistoryExamInfomation SelectedHistoryExamInfomation
        {
            get => _SelectedHistoryExamInfomation;
            set
            {
                _SelectedHistoryExamInfomation = value;
                NotifyOfPropertyChange(() => SelectedHistoryExamInfomation);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
       
     
        public static string GetToken()
        {
            //if (String.IsNullOrEmpty(uri) || String.IsNullOrEmpty(requestUri))
            //{
            //    return null;
            //}

            using (var client = new HttpClient())
            {
                try
                {
                    var person = new BodyLogin { Branch="TV2",Email="baolq@medicbaclieu.vn",Password= "170895"};
                    var json = GlobalsNAV.ConvertObjectToJson(person);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    client.BaseAddress = new Uri(BaseURL);
                    var response = client.PostAsync("/api/login",data).Result;
                    if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        var JResult = JObject.Parse(result);
                        string token = JResult["access_token"].ToString();
                        return token;
                      
                    }
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
        public void CheckHistoryByPatientCode(string PatientCode)
        {
            if (String.IsNullOrEmpty(PatientCode))
            {
                return;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(BaseURL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    var response = client.GetAsync(APIGetHistory+ PatientCode).Result;
                    
                    if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        HistoryExamInfomationList = new ObservableCollection<HistoryExamInfomation>();
                        string result = response.Content.ReadAsStringAsync().Result;
                        var JResult = JObject.Parse(result);
                        string token = JResult["data"].ToString();
                        foreach (var item in JResult["data"])
                        {
                            HistoryExamInfomationList.Add(new HistoryExamInfomation(Convert.ToDateTime(item["created_at"]), Convert.ToInt32(item["InformationID"])));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
       
        private async void NavigateAsync(int InfoID)
        {
            //string result = RequestReport();
            if (webView != null)
            {
                var request = webView.CoreWebView2.Environment.CreateWebResourceRequest(BaseURL+APIGetPreview+ InfoID,"GET", null, "Authorization: Bearer "+Token);
                webView.CoreWebView2.NavigateWithWebResourceRequest(request);
            }
        }
        public void DoubleClick(object args)
        {
            if(SelectedHistoryExamInfomation == null)
            {
                return;
            }
            NavigateAsync(SelectedHistoryExamInfomation.InformationID);
        }
    }
    
    public class BodyLogin
    {
        public string Branch { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class HistoryExamInfomation
    {
        public HistoryExamInfomation()
        {

        }
        public HistoryExamInfomation(DateTime _created_at, int _InformationID)
        {
            created_at = _created_at;
            InformationID = _InformationID;
        }
        public DateTime created_at { get; set; }
        public int InformationID { get; set; }
    }
}
