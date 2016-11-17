using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    [Serializable]
    public class DtoBusinessListBase<T, C> : Csla.BusinessListBase<T, C>, IDPBusinessObject
        where C : Csla.Core.IEditableBusinessObject
        where T:DtoBusinessListBase<T, C>
    {


        ILifetimeScope IDPBusinessObject.scope { get; set; } // In the actual implementation we would not use a service locator. Limited by CSLA

        public void CallMethodDI(string methodName)
        {
            ((IDPBusinessObject)this).CallMethodDI(methodName);
        }

        public void CallMethodDI<P>(string methodName, P parameters)
        {
            ((IDPBusinessObject)this).CallMethodDI(methodName, parameters);
        }

        public T2 CallMethodDI<T2>(string methodName)
        {
            return ((IDPBusinessObject)this).CallMethodDI<T2>(methodName);
        }

        public T2 CallMethodDI<P, T2>(string methodName, P parameters)
        {
            return ((IDPBusinessObject)this).CallMethodDI<P, T2>(methodName, parameters);
        }
    }
}
