using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Example.Dal
{
    public interface IBusinessItemDal
    {

        List<BusinessItemDto> Fetch();
        List<BusinessItemDto> Fetch(Guid criteria);

        void Update(BusinessItemDto dto);

    }
}
