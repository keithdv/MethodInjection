using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Lib
{
    public class CriteriaBase
    {
        public Guid Guid { get; set; }
    }

    public class Criteria : CriteriaBase { }

}
