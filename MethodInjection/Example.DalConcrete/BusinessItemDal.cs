using Example.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Example.DalConcrete
{
    public class BusinessItemDal : IBusinessItemDal
    {
        public List<BusinessItemDto> Fetch()
        {

            var BusinessItemDtos = new List<BusinessItemDto>();

            BusinessItemDtos.Add(new BusinessItemDto() { FetchUniqueID = Guid.NewGuid() });
            BusinessItemDtos.Add(new BusinessItemDto() { FetchUniqueID = Guid.NewGuid() });

            return BusinessItemDtos;
        }

        public List<BusinessItemDto> Fetch(Guid criteria)
        {

            var BusinessItemDtos = new List<BusinessItemDto>();

            BusinessItemDtos.Add(new BusinessItemDto() { FetchUniqueID = Guid.NewGuid(), Criteria = criteria });
            BusinessItemDtos.Add(new BusinessItemDto() { FetchUniqueID = Guid.NewGuid(), Criteria = criteria });

            return BusinessItemDtos;
        }

        public void Update(BusinessItemDto dto)
        {
            dto.UpdateUniqueID = Guid.NewGuid();
        }

    }
}
