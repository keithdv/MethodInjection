using Csla;
using System;
using Autofac;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectPortal;
using Example.Dal;

namespace Example.Lib
{

    public delegate IRoot FetchRoot();
    public delegate IRoot FetchRootGuid(Guid criteria);

    [Serializable]
    internal class Root : DPBusinessBase<Root>, IRoot
        , IHandleCreateDI<CreateChildBusinessItemList>, IHandleCreateDI<Guid, CreateChildBusinessItemListGuid>
        , IHandleFetchDI<FetchChildBusinessItemList>, IHandleFetchDI<Guid, FetchChildBusinessItemListGuid>, IHandleUpdateDI<ObjectPortalUpdateChild<IBusinessItemList>>
    {


        public static readonly PropertyInfo<IBusinessItemList> BusinessItemListProperty = RegisterProperty<IBusinessItemList>(c => c.BusinessItemList);
        public IBusinessItemList BusinessItemList
        {
            get { return GetProperty(BusinessItemListProperty); }
            set { SetProperty(BusinessItemListProperty, value); }
        }

        public void Create(CreateChildBusinessItemList createList)
        {
            BusinessItemList = createList();
        }

        public void Create(Guid criteria, CreateChildBusinessItemListGuid createList)
        {
            BusinessItemList = createList(criteria);
        }

        public void Fetch(FetchChildBusinessItemList fetchList)
        {
            BusinessItemList = fetchList();
        }

        public void Fetch(Guid criteria, FetchChildBusinessItemListGuid fetchList)
        {
            BusinessItemList = fetchList(criteria);
        }

        public void Insert(ObjectPortalUpdateChild<IBusinessItemList> updateList)
        {
            updateList(BusinessItemList);
        }

        public void Update(ObjectPortalUpdateChild<IBusinessItemList> updateList)
        {
            updateList(BusinessItemList);
        }
    }
}
