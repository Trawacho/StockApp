using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StockApp.BaseClasses
{
    public class StockTV : IEquatable<StockTV>, IComparable<StockTV>
    {
        #region EventHandler

        public event TVSettingsSentEventHandler TVSettingsSent;
        protected void RaiseTVSettingsSent()
        {
            var handler = TVSettingsSent;
            handler?.Invoke(this, EventArgs.Empty);
        }



        private event StockTVServiceChangedEventHandler StockTVServiceAdded;
        protected void RaiseStockTVServiceAdded(StockTVService service)
        {
            var handler = StockTVServiceAdded;
            handler?.Invoke(this, new StockTVServiceChangedEventArgs(service));
        }

        private event StockTVServiceChangedEventHandler StockTVServiceRemoved;
        protected void RaiseStockTVServiceRemove(StockTVService service)
        {
            var handler = StockTVServiceRemoved;
            handler?.Invoke(this, new StockTVServiceChangedEventArgs(service));
        }


        public event StockTVResultChangedHandler StockTVResultChanged;
        protected void RaiseStockTVResultChanged()
        {
            var handler = StockTVResultChanged;
            handler?.Invoke(this, new StockTVResultChangedEventArgs(TVResult));
        }


        public event PropertyChangedEventHandler StockTVSettingsChanged;
        protected void RaiseStockTVSettingsChanged([CallerMemberName] string propertyName = null)
        {
            var handler = StockTVSettingsChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void TvSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseStockTVSettingsChanged(e.PropertyName);
        }


        public event StockTVOnlineChangedEventHandler StockTVOnlineChanged;
        protected void RaiseStockTVOnlineChanged()
        {
            var handler = StockTVOnlineChanged;
            handler?.Invoke(this, this.IsOnline);
        }

        #endregion

        #region IEquatable- and ICompareable Implementation

        /// <summary>
        /// True if Hostname is equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StockTV other)
        {
            return this.HostName.Equals(other.HostName) &&
                this.IPAddress.Equals(other.IPAddress) &&
                this.TVSettings.Bahn.Equals(other.TVSettings.Bahn);
        }

        public int CompareTo(StockTV other)
        {
            var equalBahn = TVSettings.Bahn.CompareTo(other.TVSettings.Bahn);
            if (equalBahn != 0)
                return equalBahn;

            var equalHostName = this.HostName.CompareTo(other.HostName);
            if (equalHostName != 0)
                return equalHostName;

            return this.IPAddress.CompareTo(other.IPAddress);

        }

        #endregion

        #region Fields and Properties

        private readonly object _lock = new();
        private StockTVService ApplicationService;
        private StockTVAppClient appClient;

        private StockTVService PublisherService;
        private StockTVSubscriberClient subscriberClient;

        private readonly List<string> _infos;
        public IEnumerable<string> Informationen => _infos;

        public string FW { get; private set; }

        public string IPAddress { private set; get; }
        public string HostName { private set; get; }
        public DomainName DomainName { private set; get; }

        public DateTime LastMDnsUpdate { get; private set; }

        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            private set
            {
                if (_isOnline == value) return;

                _isOnline = value;

                if (_isOnline)
                {
                    __wasOnline = true;
                    TVSettingsGet();
                }

                RaiseStockTVOnlineChanged();

            }
        }
        /// <summary>
        /// Needed for <see cref="IsOutdated"/>
        /// </summary>
        private bool __wasOnline;

        #endregion

        #region Konstruktor
        public StockTV()
        {
            _infos = new List<string>();
            tvSettings = StockTVSettings.GetDefault(GameModis.Training);
            tvSettings.PropertyChanged += TvSettings_PropertyChanged;
        }

       

        /// <summary>
        /// for design only
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="ipaddress"></param>
        public StockTV(string hostname, string ipaddress) : this()
        {
            this.HostName = hostname;
            this.IPAddress = ipaddress;
        }

        /// <summary>
        /// Default-Konstruktor
        /// </summary>
        /// <param name="mDnsInfo"></param>
        public StockTV(MDnsServiceInfo mDnsInfo) : this()
        {
            IPAddress = mDnsInfo.IPAddress;
            HostName = mDnsInfo.DomainName.Labels.First();
            DomainName = mDnsInfo.DomainName;
            UpdateInfos(mDnsInfo.Informations);
        }

        #endregion

        #region StockTV mDNS-Service Implementation

        private void UpdateInfos(IEnumerable<string> infos)
        {
            LastMDnsUpdate = DateTime.Now;
            foreach (var i in infos)
            {
                if (!_infos.Contains(i))
                {
                    _infos.Add(i);
                    if (i.StartsWith("pkg"))
                    {
                        FW = i.Split('=')[1];
                    }
                }
            }
        }

        public void SetStockTVService(MDnsServiceInfo mDnsInfo)
        {
            UpdateInfos(mDnsInfo.Informations);

            var service = new StockTVService(mDnsInfo);
            if (service == null) return;

            lock (_lock)
            {
                if (service.IsApplicationService)
                {
                    if (ApplicationService == null || !ApplicationService.Equals(service))
                    {
                        ApplicationService = service;
                        RaiseStockTVServiceAdded(service);

                        if (appClient == null)
                        {
                            appClient = new StockTVAppClient(IPAddress, service.Port, HostName);
                            appClient.StockTVOnlineChanged += AppClient_StockTVOnlineChanged;
                            appClient.Start();
                        }
                    }
                }
                else if (service.IsPublisherService)
                {
                    if (PublisherService == null || !PublisherService.Equals(service))
                    {
                        PublisherService = service;
                        RaiseStockTVServiceAdded(service);

                        if (subscriberClient == null)
                        {
                            subscriberClient = new StockTVSubscriberClient(IPAddress, PublisherService.Port);
                            subscriberClient.StockTVResultChanged += SubscriberClient_StockTVResultChanged;
                            subscriberClient.Start();
                        }
                    }
                }
            }
        }

        public void RemoveStockTVService(MDnsServiceInfo mDnsInfo)
        {
            var service = new StockTVService(mDnsInfo);
            lock (_lock)
            {
                if (service.IsApplicationService)
                {
                    ApplicationService = null;
                    StopAppClient();
                }
                else if (service.IsPublisherService)
                {
                    PublisherService = null;
                    StopSubscriberClient();
                }
            }
            RaiseStockTVServiceRemove(service);
        }

        #endregion

        #region StockTVResult

        /// <summary>
        /// Called from Event within <see cref="subscriberClient"/> 
        /// </summary>
        /// <param name="stockTVResult"></param>
        private void SubscriberClient_StockTVResultChanged(object sender, StockTVResult stockTVResult)
        {
            //Create a copy to use, not reference
            TVResult = new StockTVResult(subscriberClient.StockTVResult);
        }

        /// <summary>
        /// Returns <see cref="StockTVResult"/> after sending a request to StockTV
        /// </summary>
        /// <returns></returns>
        public void TVResultGet()
        {
            appClient?.AddCommand(StockTVCommand.GetResultCommand((byteArray) => TVResult = new StockTVResult(byteArray)));
        }

        public void TVResultReset()
        {
            appClient?.AddCommand(StockTVCommand.ResetCommand());
        }

        private StockTVResult tvResult;
        /// <summary>
        /// Updated through the <see cref="subscriberClient"/>
        /// </summary>
        public StockTVResult TVResult
        {
            get => tvResult;
            private set
            {
                if (tvResult == value) return;

                tvResult = value;
                RaiseStockTVResultChanged();
            }
        }

        #endregion

        #region StockTVSettings

        private readonly StockTVSettings tvSettings;

        public StockTVSettings TVSettings
        {
            get => tvSettings;
        }


        public void TVSettingsGet()
        {
            appClient?.AddCommand(StockTVCommand.GetSettingsCommand((byteArray) =>
            {
                TVSettings.SetValues(byteArray);
                Debug.WriteLine($"Get TVSettings {IPAddress} -> [{TVSettings}]");
            }));
        }

        public void TVSettingsSend()
        {
            Debug.WriteLine($"Send TVSettings {IPAddress} -> [{TVSettings}]");
            appClient?.AddCommand(StockTVCommand.SendSettingsCommand(TVSettings));
            RaiseTVSettingsSent();
        }



        #endregion

        /// <summary>
        /// Updates the <see cref="IsOnline"/> state
        /// <br>If state is true, the <see cref="TVSettingsGet"/>-Method gets called</br>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="IsOnline"></param>
        private void AppClient_StockTVOnlineChanged(object sender, bool IsOnline)
        {
            this.IsOnline = IsOnline;
        }


        public void SendTeamNames(IEnumerable<StockTVBegegnung> begegnungen)
        {
            appClient?.AddCommand(StockTVCommand.SendBegegnungenCommand(begegnungen));
        }

        /// <summary>
        /// Stops the <see cref="subscriberClient"/> and removes the event <see cref="StockTVResult_StockTVResultChanged(StockTVResult)"/>
        /// </summary>
        private void StopSubscriberClient()
        {
            if (subscriberClient != null)
            {
                subscriberClient.Stop();
                subscriberClient.StockTVResultChanged -= SubscriberClient_StockTVResultChanged;
                subscriberClient = null;
            }
        }

        /// <summary>
        /// Stops the <see cref="appClient"/>
        /// </summary>
        private void StopAppClient()
        {
            if (appClient != null)
            {
                appClient.StockTVOnlineChanged -= AppClient_StockTVOnlineChanged;
                appClient.Stop();
                appClient = null;
            }

            IsOnline = false;
        }

        /// <summary>
        /// Stops the <seealso cref="subscriberClient"/> and the <see cref="appClient"/>
        /// </summary>
        public void Stop()
        {
            StopSubscriberClient();
            StopAppClient();
        }

        /// <summary>
        /// TRUE if <see cref="IsOnline"/> is true or <see cref="LastMDnsUpdate"/> was within the last 10s
        /// </summary>
        /// <returns></returns>
        public bool IsOutdated()
        {
            bool _tooOld = (DateTime.Now - LastMDnsUpdate).TotalMilliseconds > 10000;

            if (!__wasOnline)
            {
                if (!_tooOld)
                {
                    IsOnline = true;
                }
            }
            else
            {
                if (_tooOld && IsOnline)
                {
                    IsOnline = false;
                }
            }

            return !IsOnline;
            //if (!__wasOnline && !_tooOld)
            //{
            //    IsOnline = true;
            //}

            //if (IsOnline) return false;

            //return (__wasOnline && _tooOld);

        }


    }
}
