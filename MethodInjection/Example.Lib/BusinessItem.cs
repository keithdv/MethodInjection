using Autofac;
using Csla;
using Example.Dal;
using ObjectPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Csla.Core;
using Csla.Rules;

namespace Example.Lib
{

    public delegate IBusinessItem CreateChildBusinessItem();
    public delegate IBusinessItem CreateChildBusinessItemGuid(Guid criteria);
    public delegate IBusinessItem FetchChildBusinessItem(BusinessItemDto dto);

    // We allow the Fetch calls (and delegates) to have multiple parameters
    // But the IHandleXYZ interface can only have one criteria as a parameter
    // with a tuple to handle multiple parameters
    // ObjectPortal will bridge the two by turning the multiple paramters to a tuple
    public delegate IBusinessItem FetchChildBusinessItemGuid((Guid g, BusinessItemDto dto) criteria);

    public delegate void UpdateChildBusinessItem(IBusinessItem bo, Guid criteria);

    [Serializable]
    internal class BusinessItem : DPBusinessBase<BusinessItem>, IBusinessItem
        , IHandleCreateChild<Guid>
        , IHandleFetchChildDI<BusinessItemDto, IBusinessItemDal>
        , IHandleFetchChildDI<(Guid g, BusinessItemDto dto), IBusinessItemDal>
        , IHandleUpdateChildDI<Guid, IBusinessItemDal>
    {

        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
        public string Name
        {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        public static readonly PropertyInfo<Guid> CriteriaProperty = RegisterProperty<Guid>(c => c.Criteria);
        public Guid Criteria
        {
            get { return GetProperty(CriteriaProperty); }
            set { SetProperty(CriteriaProperty, value); }
        }

        public static readonly PropertyInfo<Guid> UniqueIDProperty = RegisterProperty<Guid>(c => c.FetchChildID);
        public Guid FetchChildID
        {
            get { return GetProperty(UniqueIDProperty); }
            set { SetProperty(UniqueIDProperty, value); }
        }


        public static readonly PropertyInfo<Guid> UpdatedIDProperty = RegisterProperty<Guid>(c => c.UpdatedID);
        public Guid UpdatedID
        {
            get { return GetProperty(UpdatedIDProperty); }
            set { SetProperty(UpdatedIDProperty, value); }
        }

        public static readonly PropertyInfo<Guid> ScopeIDProperty = RegisterProperty<Guid>(c => c.ScopeID);
        public Guid ScopeID
        {
            get { return GetProperty(ScopeIDProperty); }
            set { SetProperty(ScopeIDProperty, value); }
        }

        public void FetchChild(BusinessItemDto dto, IBusinessItemDal dal) // I only need the dependency within this method
        {
            MarkAsChild();
            this.FetchChildID = dto.FetchUniqueID;
        }


        // We allow the Fetch calls (and delegates) to have multiple parameters
        // But the IHandleXYZ interface can only have one criteria as a parameter
        // with a tuple to handle multiple parameters
        // ObjectPortal will bridge the two by turning the multiple paramters to a tuple
        public void FetchChild((Guid g, BusinessItemDto dto) criteria, IBusinessItemDal dal) // I only need the dependency within this method
        {
            MarkAsChild();
            this.FetchChildID = criteria.dto.FetchUniqueID;
            this.Criteria = criteria.g;

        }

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();
            BusinessRules.AddRule(new DependencyBusinessRule(NameProperty));
            BusinessRules.AddRule(new DependencyBusinessRuleTuple(NameProperty));
        }

        public void CreateChild(Guid criteria)
        {
            this.Criteria = criteria;
        }

        public void InsertChild(Guid criteria, IBusinessItemDal dal)
        {

            var dto = new BusinessItemDto();
            dal.Update(dto);

            using (BypassPropertyChecks)
            {
                this.UpdatedID = dto.UpdateUniqueID;
            }

        }

        public void UpdateChild(Guid criteria, IBusinessItemDal dal)
        {
            var dto = new BusinessItemDto();
            dal.Update(dto);

            using (BypassPropertyChecks)
            {
                this.UpdatedID = dto.UpdateUniqueID;
            }
        }


        internal class DependencyBusinessRule : BusinessRuleDIBase<IBusinessItemDal>
        {

            public DependencyBusinessRule(IPropertyInfo nameProperty) : base(nameProperty)
            {
                InputProperties.Add(nameProperty);
            }

            public override void Execute(RuleContext context, IBusinessItemDal dependencies)
            {
                if (dependencies == null)
                {
                    context.AddErrorResult("Did not recieve dependency!");
                }
            }

        }

        internal class DependencyBusinessRuleTuple : BusinessRuleDIBase<(IObjectPortal<IBusinessItem>, IBusinessItemDal)>
        {

            public DependencyBusinessRuleTuple(IPropertyInfo nameProperty) : base(nameProperty)
            {
                InputProperties.Add(nameProperty);
            }

            public override void Execute(RuleContext context, (IObjectPortal<IBusinessItem>, IBusinessItemDal) dependencies)
            {

                if (dependencies.Item1 == null || dependencies.Item2 == null)
                {
                    context.AddErrorResult("Did not recieve dependency!");
                }
            }

        }

    }
}
