using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ObjectPortal
{
    public interface IObjectPortal<T>
        where T : Csla.Core.ITrackStatus
    {

        T Create();
        T Create<C>(C criteria);
        T CreateChild();
        T CreateChild<C>(C criteria);
        T Fetch();

        T Fetch<C>(C criteria);
        T FetchChild();
        T FetchChild<C>(C criteria);
        void Update(T bo);
        void Update<C>(T bo, C criteria);
        void UpdateChild(T bo);
        void UpdateChild<C>(T bo, C criteria);
    }
}
