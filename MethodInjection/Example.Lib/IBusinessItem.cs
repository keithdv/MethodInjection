using Example.Dal;
using ObjectPortal;
using System;

namespace Example.Lib
{
    public interface IBusinessItem : Csla.IBusinessBase
    {
        string Name { get; set; }
        Guid Criteria { get; }
        Guid ScopeID { get;  }
        Guid FetchChildID { get;  }
        Guid UpdatedID { get; }

    }
}