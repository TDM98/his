using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;

namespace aEMR.Configuration.RoomType.Views
{
    [Export(typeof(RoomTypeView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RoomTypeView : IRoomType, IDisposable
    {
        //
        private bool disposed = false;

    //Implement IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
            disposed = true;
        }
    }

    // Use C# destructor syntax for finalization code.
    ~RoomTypeView()
    {
        // Simply call Dispose(false).
        Dispose (false);
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        Dispose();
    }

        //

      
    }
}
