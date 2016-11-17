using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPortal
{
    public interface IDependencyPropertyInfo
    {
        string Name { get; }
        Type Type { get; }
    }

    public interface IDependencyPropertyInfo<T> : IDependencyPropertyInfo
    {

    }
}
