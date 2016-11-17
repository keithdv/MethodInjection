using Csla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using ObjectPortal;
using Example.Dal;

namespace Example.Lib
{

    public delegate IBusinessItemList CreateChildBusinessItemList();
    public delegate IBusinessItemList CreateChildBusinessItemListGuid(Guid criteria);
    public delegate IBusinessItemList FetchChildBusinessItemList();
    public delegate IBusinessItemList FetchChildBusinessItemListGuid(Guid criteria);

    [Serializable]
    internal class BusinessItemList : DtoBusinessListBase<BusinessItemList, IBusinessItem>, IBusinessItemList
        , IHandleCreateChildDI<Guid, CreateChildBusinessItemGuid>
        , IHandleUpdateChildDI<UpdateChildBusinessItem>
        , IHandleFetchChildDI<Tuple<FetchChildBusinessItem, IBusinessItemDal>>, IHandleFetchChildDI<Guid, Tuple<FetchChildBusinessItemGuid, IBusinessItemDal>>
    {

        public void AddChild()
        {
            base.CallMethodDI(nameof(AddChildDI));
        }


        // Use Method injection to get the dependency from the base class
        public void AddChildDI(CreateChildBusinessItem createBI)
        {
            var bo = createBI();

            this.Add(bo);


        }

        public IBusinessItem AddCreateChild()
        {
            return base.CallMethodDI<IBusinessItem>(nameof(AddCreateChildDI));
        }

        // Use Method injection to get the dependency from the base class
        public IBusinessItem AddCreateChildDI(CreateChildBusinessItem createBI)
        {
            var bo = createBI();

            this.Add(bo);

            return bo;

        }

        public IBusinessItem AddCreateChild(Guid criteria)
        {
            return base.CallMethodDI<Guid, IBusinessItem>(nameof(AddCreateChildDI), criteria);
        }

        // Use Method injection to get the dependency from the base class
        public IBusinessItem AddCreateChildDI(Guid criteria, CreateChildBusinessItemGuid createBI)
        {
            var bo = createBI(criteria);

            this.Add(bo);

            return bo;

        }

        public void CreateChild(Guid criteria, CreateChildBusinessItemGuid createChild)
        {
            this.Add(createChild(criteria));
        }

        public void FetchChild(Tuple<FetchChildBusinessItem, IBusinessItemDal> fetchbi)
        {
            var dtos = fetchbi.Item2.Fetch();

            foreach (var d in dtos)
            {
                Add(fetchbi.Item1(d));
            }

        }

        public void FetchChild(Guid criteria, Tuple<FetchChildBusinessItemGuid, IBusinessItemDal> fetchbi)
        {

            var dtos = fetchbi.Item2.Fetch(criteria);

            foreach (var d in dtos)
            {
                // We allow the Fetch calls (and delegates) to have multiple parameters
                // But the IHandleXYZ interface can only have one criteria as a parameter
                // with a tuple to handle multiple parameters
                // ObjectPortal will bridge the two by turning the multiple paramters to a tuple

                Add(fetchbi.Item1(criteria, d));
            }

        }

        public void InsertChild(UpdateChildBusinessItem update)
        {
            foreach (var i in this)
            {
                if (i.IsDirty)
                {
                    update(i, Guid.NewGuid());
                }
            }
        }

        public void UpdateChild(UpdateChildBusinessItem update)
        {
            foreach (var i in this)
            {
                if (i.IsDirty)
                {
                    update(i, Guid.NewGuid());
                }
            }
        }
    }
}
