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
        , IHandleFetchDI<FetchChildBusinessItemList>, IHandleFetchDI<Guid, FetchChildBusinessItemListCriteria>, IHandleUpdateDI<UpdateChild<IBusinessItemList>>
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

        public void Fetch(Guid criteria, FetchChildBusinessItemListCriteria fetchList)
        {
            BusinessItemList = fetchList(new Criteria() { Guid = criteria });
        }

        public void Insert(UpdateChild<IBusinessItemList> updateList)
        {
            updateList(BusinessItemList);
        }

        public void Update(UpdateChild<IBusinessItemList> updateList)
        {
            updateList(BusinessItemList);
        }
    }
}
