using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ObjectPortal
{
    internal static class IDPBusinessObjectExtensions
    {


        public static void CallMethodDI(this IDPBusinessObject bo, string methodName)
        {
            var method = bo.GetType().GetMethod(methodName);
            var paramCount = method.GetParameters().Count();

            if (paramCount != 1)
            {
                throw new Exception($"Method {methodName} must have 1 method parameter as dependencies.");
            }

            CallMethodDI(bo, method, (o) => new object[] { o });
        }

        public static void CallMethodDI<P>(this IDPBusinessObject bo, string methodName, P parameters)
        {
            var method = bo.GetType().GetMethod(methodName);
            var paramCount = method.GetParameters().Count();

            if (paramCount != 2)
            {
                throw new Exception($"Method {methodName} must have 2 method parameter.");
            }

            CallMethodDI(bo, method, (o) => new object[] { parameters, o });

        }

        public static T2 CallMethodDI<T2>(this IDPBusinessObject bo, string methodName)
        {
            var method = bo.GetType().GetMethods().Where(x => x.Name == methodName && x.GetParameters().Count() == 1).FirstOrDefault();


            if (method == null)
            {
                throw new InvalidOperationException($"Method signature could not be matched for {methodName} with no parameters");
            }

            var paramCount = method.GetParameters().Count();

            if (paramCount != 1)
            {
                throw new Exception($"Method {methodName} must have 1 method parameter as dependencies.");
            }

            return (T2)CallMethodDI(bo, method, (o) => new object[] { o });
        }

        public static T2 CallMethodDI<P, T2>(this IDPBusinessObject bo, string methodName, P parameters)
        {
            var methods = bo.GetType().GetMethods().Where(x => x.Name == methodName);

            MethodInfo method = null;

            foreach (var m in methods)
            {
                var p = m.GetParameters();

                // Two parameters - Criteria and DIs
                if (p.Count() == 2 && p[0].ParameterType == typeof(P))
                {
                    method = m;
                    break;
                }

            }

            if (method == null)
            {
                throw new InvalidOperationException($"Method signature of (Parameter, Dependency) could not be matched for {methodName} and parameter type ${typeof(P).Name}");
            }

            var paramCount = method.GetParameters().Count();

            if (paramCount != 2)
            {
                throw new Exception($"Method {methodName} must have 2 method parameter.");
            }

            return (T2)CallMethodDI(bo, method, (o) => new object[] { parameters, o });

        }

        private static object CallMethodDI(IDPBusinessObject bo, MethodInfo method, Func<object, object[]> returnInvokeParams)
        {


            var @params = method.GetParameters();
            var scope = ((IDPBusinessObject)bo).scope;


            var dependencyType = @params[@params.Count() - 1].ParameterType; // Assume dependency to be the last parameter

            if (typeof(Delegate).IsAssignableFrom(dependencyType))
            {
                return method.Invoke(bo, returnInvokeParams(ObjectPortal.CreateDelegate(dependencyType, scope)));
            }
            else if (!scope.IsRegistered(dependencyType) && dependencyType.IsGenericType)
            { // Bad way of seeing if it is a Tuple.
                List<object> dependencies = new List<object>();

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


                return method.Invoke(bo, returnInvokeParams(tuple));

            }
            else if (scope.IsRegistered(dependencyType))
            {
                var dep = scope.Resolve(dependencyType);


                return method.Invoke(bo, returnInvokeParams(dep));
            }
            else
            {
                throw new ObjectPortalOperationNotSupportedException("Fetch with criteria. Dependencies not registered");
            }

        }

    }
}
