using System;

namespace StockApp.BaseClasses
{
    /// <summary>
    /// StockTV ServiceInformation with Name and Portnumber
    /// </summary>
    public class StockTVService : IEquatable<StockTVService>
    {
        public StockTVService(MDnsServiceInfo mDnsInfo)
        {
            ServiceName = mDnsInfo.DomainName.Labels[1];
            Port = mDnsInfo.Port;
        }
        public bool IsPublisherService => ServiceName == "_stockpub";
        public bool IsApplicationService => ServiceName == "_stockapp";
        public string ServiceName { get; private set; }
        public int Port { get; private set; }

        public bool Equals(StockTVService other)
        {
            return this.Port == other?.Port;
        }

        public string GetConnectionString(string ipAddress)
        {
            return $"tcp://{ipAddress}:{Port}";
        }
    }

    
}
