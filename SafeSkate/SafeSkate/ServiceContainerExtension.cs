﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Extension;
using Unity.Injection;

namespace SafeSkate
{
    public class ServiceContainerExtension : UnityContainerExtension
    {
        private string serverIp;
        private int updatePort;
        private int queryPort;
        public ServiceContainerExtension(string serverIp, int updatePort, int queryPort) 
        {
            this.serverIp = serverIp;
            this.updatePort = updatePort;
            this.queryPort = queryPort;
        }
        protected override void Initialize()
        {
            ServiceTypeProvider.ServerIp = serverIp;
            ServiceTypeProvider.UpdatePort = updatePort;
            ServiceTypeProvider.QueryPort = queryPort;
            this.Container.RegisterInstance(ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy);
        }
    }
}
