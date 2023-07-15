using System.Windows.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class RequestNewView : UserControl
    {
        public RequestNewView()
        {
            InitializeComponent();
            //KMx: Group ở ".xaml" rồi. Không cần group ở đây nữa. Nếu group ở đây thì câu SortOrder ở ".xaml" không thực hiện được (06/12/2014 17:15).
            //grid.GroupBy("RefGenericDrugDetail.BrandNameAndCode");
        }
       
    }
}
