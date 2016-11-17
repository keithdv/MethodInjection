using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;
using Autofac;
using Csla.Core;
using System.Reflection;

namespace ObjectPortal
{
    public abstract class BusinessRuleDIBase<T> : Csla.Rules.BusinessRule
    {

        public BusinessRuleDIBase() : base() { }

        public BusinessRuleDIBase(IPropertyInfo pi) : base(pi) { }


        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            var db = context.Target as IDPBusinessObject;
            var scope = db.scope; // In the actual implementation we would not use a service locator. Limited by CSLA
            var dependencyType = typeof(T);

            if (!scope.IsRegistered(dependencyType) && dependencyType.IsGenericType) // Bad way of seeing if it is a Tuple.
            {
                List<object> dependencies = new List<object>();

                foreach (var depType in dependencyType.GenericTypeArguments)
                {
                    dependencies.Add(scope.Resolve(depType));
                }

                object tuple = null;
                MethodInfo tupleCreateMethod = null;

                switch (dependencyType.GenericTypeArguments.Count())
                {
                    case 2:
                        tupleCreateMethod = typeof(Tuple).GetMethods().Where(x => x.IsGenericMethod && x.GetGenericArguments().Count() == 2).First();
                        tuple = tupleCreateMethod
                            .MakeGenericMethod(new Type[2] { dependencyType.GenericTypeArguments[0], dependencyType.GenericTypeArguments[1] })
                            .Invoke(null, new object[2] { dependencies[0], dependencies[1] });
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }


                Execute(context, (T)tuple);


            }
            else if (scope.IsRegistered(dependencyType))
            {
                var dep = scope.Resolve(dependencyType);

                Execute(context, (T)dep);

            }
            else
            {
                throw new ObjectPortalOperationNotSupportedException("Fetch with criteria. Dependencies not registered");
            }
        }


        public abstract void Execute(RuleContext context, T dependencies);


    }
}
