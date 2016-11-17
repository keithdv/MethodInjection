using Example.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Example.DalConcrete
{
    public class RootDal : IRootDal
    {

        IBusinessItemDal listDal;

        public RootDal(IBusinessItemDal listDal)
        {
            this.listDal = listDal;
        }

        public RootDto Fetch()
        {

            RootDto result = new RootDto();

            result.BusinessItemDtos = listDal.Fetch();

            return result;

        }

        public RootDto Fetch(Guid criteria)
        {
            RootDto result = new RootDto();

            result.BusinessItemDtos = listDal.Fetch(criteria);

            return result;
        }

        public void Update(RootDto dto)
        {
        }
    }
}
