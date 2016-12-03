using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ObjectPortal
{

    public interface IValueTupleDependency
    {
        object CreateValueTuple();
    }

    public interface IValueTupleDependency<T> : IValueTupleDependency
    {
    }

    public class ValueTupleDependency<T> : IValueTupleDependency<T>
    {

        ILifetimeScope scope;
        Type dependencyType = typeof(T);

        public ValueTupleDependency(ILifetimeScope scope)
        {
            this.scope = scope;
        }


        public object CreateValueTuple()
        {

            List<object> dependencies = new List<object>();

            // Resolve each dependency within the Tuple from the scope
            foreach (var depType in dependencyType.GenericTypeArguments)
            {
                if (typeof(Delegate).IsAssignableFrom(depType))
                {
                    dependencies.Add(ObjectPortal.CreateDelegate(depType, scope));
                }
                else
                {
                    dependencies.Add(scope.Resolve(depType));
                }
            }

            return CreateValueTuple(dependencies.ToArray(), dependencyType.GenericTypeArguments);

        }

        MethodInfo tupleCreateMethod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencies"></param>
        /// <param name="dependencyTypes">Seperate because the concrete type may not match the requested type</param>
        /// <returns></returns>
        public object CreateValueTuple(object[] dependencies, Type[] dependencyTypes)
        {

            if(dependencies == null) { throw new ArgumentNullException(nameof(dependencies)); }
            if(dependencyTypes == null) { throw new ArgumentNullException(nameof(dependencyTypes)); }

            if(dependencies.Length == 0 || dependencies.Length != dependencyTypes.Length)
            {
                throw new ArgumentNullException("Invalid dependency array length");
            }

            if(dependencies.Length > 9)
            {
                throw new ArgumentNullException("Too many dependency properties");
            }

            var len = dependencies.Length;

            // Get the correct Create<> method
            if (tupleCreateMethod == null)
            {
                tupleCreateMethod = typeof(ValueTuple)
                                                .GetMethods()
                                                .Where(x => x.IsGenericMethod && x.GetGenericArguments().Count() == len).First();
                tupleCreateMethod = tupleCreateMethod.MakeGenericMethod(dependencyTypes);
            }

            var tuple = tupleCreateMethod.Invoke(null, dependencies);

            return (T) tuple;

        }
    }
}
