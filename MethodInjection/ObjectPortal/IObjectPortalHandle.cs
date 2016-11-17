using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    public interface IObjectPortalHandleFetch
    {

        void Fetch();

    }

    public interface IObjectPortalHandleFetch<T>
    {

        void Fetch(T criteria);

    }

}
