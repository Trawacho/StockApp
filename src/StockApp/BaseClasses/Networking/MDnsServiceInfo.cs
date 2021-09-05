using Makaretu.Dns;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.BaseClasses
{
    public class MDnsServiceInfo
    {
        public MDnsServiceInfo(DomainName domainName, string ipAddress, int port)
        {
            this.DomainName = domainName;
            this.IPAddress = ipAddress;
            this.Port = port;
            this.Informations = new HashSet<string>();
        }

        public DomainName DomainName { get; private set; }

        public string HostName => DomainName.Labels.First();

        public string ConnectionString => $"tcp://{IPAddress}:{Port}";
        public string IPAddress { get; private set; }
        public int Port { get; private set; }
        public HashSet<string> Informations { get; private set; }
    }
}
