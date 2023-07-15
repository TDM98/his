using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;

namespace aEMR.ViewModels
{
    [Export(typeof (ILinkInputPCLImaging)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LinkInputPCLImagingViewModel : Conductor<object>, ILinkInputPCLImaging
    {
        public LinkInputPCLImagingViewModel()
        {

        }

        public void hplInputPCLImaging_Click(object sender, RoutedEventArgs e)
        {
            var Conslt = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<IPCLDeptImagingResult>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
    }
}