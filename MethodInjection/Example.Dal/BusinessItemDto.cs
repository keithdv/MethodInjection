using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Example.Dal
{
    public class BusinessItemDto
    {
        public Guid FetchUniqueID { get; set; }
        public Guid UpdateUniqueID { get; set; }
        public Guid Criteria { get; set; }
    }
}
