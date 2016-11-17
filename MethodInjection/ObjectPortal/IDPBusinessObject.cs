using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    internal interface IDPBusinessObject : Csla.Core.ITrackStatus
    {
        ILifetimeScope scope { get; set; } // In the actual implementation we would not use a service locator. Limited by CSLA
    }
}
