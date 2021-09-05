using Makaretu.Dns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace StockApp.BaseClasses
{
    public class StockTVs : ICollection<StockTV>, IEnumerable<StockTV>
    {
        #region EventHandler

        public event StockTVCollectionChangedEventHandler StockTVCollectionAdded;

        protected void RaiseStockTVCollectionAdded(StockTV stockTV)
        {
            var handler = StockTVCollectionAdded;
            handler?.Invoke(this, new StockTVCollectionChangedEventArgs(stockTV));
        }

        public event StockTVCollectionChangedEventHandler StockTVCollectionRemoved;

        protected void RaiseStockTVCollectionRemoved(StockTV stockTV)
        {
            var handler = StockTVCollectionRemoved;
            handler?.Invoke(this, new StockTVCollectionChangedEventArgs(stockTV));
        }

        #endregion

        #region private Fields

        private readonly object _lock = new();
        private readonly List<StockTV> stockTVs;
        readonly ServiceDiscovery serviceDiscovery;
        readonly DomainName stockAppDNS = new("_stockapp._tcp.local");
        readonly DomainName stockPubDNS = new("_stockpub._tcp.local");
        DispatcherTimer discoveryTimer;

        #endregion

        #region Implement ICollection

        public int Count => ((ICollection<StockTV>)stockTVs).Count;

        public bool IsReadOnly => ((ICollection<StockTV>)stockTVs).IsReadOnly;

        public bool Contains(StockTV item)
        {
            return stockTVs.Contains(item);
        }

        public void CopyTo(StockTV[] array, int arrayIndex)
        {
            lock (_lock)
                stockTVs.CopyTo(array, arrayIndex);
        }

        public int IndexOf(StockTV item)
        {
            return stockTVs.IndexOf(item);
        }

        public void Add(StockTV item)
        {
            lock (_lock)
            {
                if (!stockTVs.Any(s => s.IPAddress == item.IPAddress))
                {
                    stockTVs.Add(item);
                    RaiseStockTVCollectionAdded(item);
                }
            }
        }

        public bool Remove(StockTV item)
        {
            bool _removed = false;

            lock (_lock)
                _removed = stockTVs.Remove(item);

            if (_removed)
                RaiseStockTVCollectionRemoved(item);

            return _removed;

        }

        public void Clear()
        {
            lock (_lock)
                stockTVs.Clear();

            RaiseStockTVCollectionRemoved(null);
        }

        IEnumerator<StockTV> IEnumerable<StockTV>.GetEnumerator()
        {
            return stockTVs.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return stockTVs.GetEnumerator();
        }

        public StockTV this[int index]
        {
            get => ((IList<StockTV>)stockTVs)[index];
            set => ((IList<StockTV>)stockTVs)[index] = value;
        }


        #endregion

        #region Konstruktor

        public StockTVs()
        {
            stockTVs = new List<StockTV>();
            serviceDiscovery = new ServiceDiscovery();

            serviceDiscovery.ServiceInstanceDiscovered += Discovery_ServiceInstanceDiscovered;
            serviceDiscovery.ServiceInstanceShutdown += Discovery_ServiceInstanceShutdown;
        }

        #endregion

        #region Discovery-events

        private void Discovery_ServiceInstanceShutdown(object sender, ServiceInstanceShutdownEventArgs args)
        {
            var dnsInfo = new MDnsServiceInfo(args.ServiceInstanceName,
                                        args.Message.AdditionalRecords.OfType<ARecord>().FirstOrDefault()?.Address.ToString() ?? "",
                                        args.Message.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault()?.Port ?? 0);

            Remove(dnsInfo);
        }

        private void Discovery_ServiceInstanceDiscovered(object sender, ServiceInstanceDiscoveryEventArgs args)
        {
            var dnsInfo = new MDnsServiceInfo(args.ServiceInstanceName,
                                         args.Message.AdditionalRecords.OfType<ARecord>().FirstOrDefault()?.Address.ToString() ?? "",
                                         args.Message.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault()?.Port ?? 0);

            foreach (var item in args.Message.AdditionalRecords.OfType<TXTRecord>())
            {
                foreach (var s in item.Strings)
                {
                    dnsInfo.Informations.Add(s);
                }
            }

            Add(dnsInfo);
        }

        #endregion

        /// <summary>
        /// Starts discovering for StockTV and cleanup if StockTV is no longer online/avail
        /// </summary>
        internal void StartDiscovery()
        {
            if (discoveryTimer?.IsEnabled ?? false) return;

            if (discoveryTimer == null)
                discoveryTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(2)
                };

            discoveryTimer.Tick += (o, e) =>
            {
                var app = string.Join(".", stockAppDNS.Labels.Take(2));
                var pub = string.Join(".", stockPubDNS.Labels.Take(2));
                serviceDiscovery.QueryServiceInstances(app);
                serviceDiscovery.QueryServiceInstances(pub);
                RemoveOutdatedStockTV();
            };
            discoveryTimer.Start();
        }

        /// <summary>
        /// <br>For each <see cref="StockTV"/> where <see cref="StockTV.IsOutdated"/> is TRUE, </br>
        /// <br>the <see cref="StockTV.Stop"/> is called and the object is removed from internal list</br>
        /// <para>this function is called from <see cref="discoveryTimer"/></para>
        /// </summary>
        private void RemoveOutdatedStockTV()
        {
            int x = 0;
            lock (_lock)
            {
                foreach (StockTV item in this.Where(x => x.IsOutdated()))
                    item.Stop();

                x = this.stockTVs.RemoveAll(t => t.IsOutdated());
            }

            if (x > 0)
                RaiseStockTVCollectionRemoved(null);
        }

        /// <summary>
        /// Stops discovering for StockTV
        /// </summary>
        internal void StopDiscovery()
        {
            discoveryTimer?.Stop();
        }

        /// <summary>
        /// Stops discoerving and for each TV the network activities
        /// </summary>
        internal void StopAllServices()
        {
            StopDiscovery();
            lock (_lock)
                foreach (StockTV tv in this)
                {
                    tv.Stop();
                }
        }

        /// <summary>
        /// Adds new StockTV to collection if parameter fits StockTV-Service
        /// </summary>
        /// <param name="mDnsInfo"></param>
        private void Add(MDnsServiceInfo mDnsInfo)
        {
            if (!mDnsInfo.DomainName.Labels.Contains("_stockapp") && !mDnsInfo.DomainName.Labels.Contains("_stockpub"))
                return;

            Add(new StockTV(mDnsInfo));

            this.First(s => s.IPAddress == mDnsInfo.IPAddress).SetStockTVService(mDnsInfo);
        }

        /// <summary>
        /// Removes StockTVService from StockTV with same IPAddress in Collection. Removes StockTV if no more Service exists
        /// </summary>
        /// <param name="dnsInfo"></param>
        private void Remove(MDnsServiceInfo dnsInfo)
        {
            this.FirstOrDefault(s => s.IPAddress == dnsInfo.IPAddress)?.RemoveStockTVService(dnsInfo);
        }
    }
}
