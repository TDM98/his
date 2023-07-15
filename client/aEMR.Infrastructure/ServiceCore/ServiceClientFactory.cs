using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.Infrastructure.ServiceCore
{
    public abstract class ServiceClientFactory<T> : IServiceClientFactory<T> where T : class
    {
        private ChannelFactory<T> _channelFactory;
        private T _channel;

        #region IServiceClientFactory<T> Members

        public T ServiceInstance
        {
            get
            {
                if (_channelFactory == null || this._channel == null)
                {
                    InitializeServiceClient();
                }
                return this._channel;
            }
        }

        /// <summary>
        /// Create a channel to server
        /// </summary>
        private void InitializeServiceClient()
        {
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (designTime)
            //if (DesignerProperties.IsInDesignTool)
            {
                // TxD 31/05/2018: The following NO LONGER appropriate under WPF so commented OUT
                //                  if something goes wrong please REVIEW.
                //var uri = new Uri(Application.Current.Host.Source, EndPointAddress);
                //_channelFactory = new ChannelFactory<T>(EndPointName, new EndpointAddress(uri, null));
            }
            else
            {
                _channelFactory = new ChannelFactory<T>(EndPointName);
            }

            // TxD Added the following to enable the adding of Custom 
            // Header(s) to the WCF Data packet sent between Client and Server
            _channelFactory.Endpoint.Behaviors.Add(new CustomBehavior());

            _channel = GetNewChannelFromCurrentFactory();

        }

        private T GetNewChannelFromCurrentFactory()
        {
            var newChannel = _channelFactory.CreateChannel();

            var channelAsCommunicationObject = (newChannel as ICommunicationObject);
            if (channelAsCommunicationObject != null)
            {
                channelAsCommunicationObject.Faulted += ChannelFaulted;
            }

            return newChannel;
        }

        private void ChannelFaulted(object sender, EventArgs e)
        {
            var communicationObject = (this._channel as ICommunicationObject);
            if (communicationObject != null)
            {
                communicationObject.Abort();

                this._channel = this.GetNewChannelFromCurrentFactory();
            }
        }

        public abstract string EndPointName
        {
            get;
        }

        public abstract string EndPointAddress
        {
            get;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            var communicationObject = (this._channel as ICommunicationObject);
            if (communicationObject != null)
            {
                communicationObject.Faulted -= this.ChannelFaulted;
                try
                {
                    communicationObject.Close();
                }
                catch (CommunicationException)
                {
                    communicationObject.Abort();
                }
                catch (TimeoutException)
                {
                    communicationObject.Abort();
                }
                catch (Exception)
                {
                    communicationObject.Abort();
                    throw;
                }
            }
        }

        #endregion
    }
}
