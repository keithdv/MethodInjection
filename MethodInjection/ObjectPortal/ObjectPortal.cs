using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Core;
using Autofac.Builder;
using Autofac;
using System.Reflection;
using System.Linq.Expressions;
using Csla.Core;

namespace ObjectPortal
{

    public static class ObjectPortal
    {

        private delegate void Nothing();

        public static void RegisterObjectPortalCreate(this ContainerBuilder builder, Type delegateType)
        {
            RegisterObjectPortalMethodCall(nameof(ObjectPortal<Csla.IBusinessBase>.Create), builder, delegateType);
        }
        public static void RegisterObjectPortalCreateChild(this ContainerBuilder builder, Type delegateType)
        {
            RegisterObjectPortalMethodCall(nameof(ObjectPortal<Csla.IBusinessBase>.CreateChild), builder, delegateType);
        }
        public static void RegisterObjectPortalFetch(this ContainerBuilder builder, Type delegateType)
        {
            RegisterObjectPortalMethodCall(nameof(ObjectPortal<Csla.IBusinessBase>.Fetch), builder, delegateType);
        }
        public static void RegisterObjectPortalFetchChild(this ContainerBuilder builder, Type delegateType)
        {
            RegisterObjectPortalMethodCall(nameof(ObjectPortal<Csla.IBusinessBase>.FetchChild), builder, delegateType);
        }

        private static void RegisterObjectPortalMethodCall(string methodName, ContainerBuilder builder, Type delegateType)
        {

            // Some serious WTF code!

            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new Exception("Only Delegates types allowed.");
            }


            // We assume delegateType is a Delegate. 
            // The business object type we need is the return type of Delegate.Invoke.
            var invoke = delegateType.GetMethod(nameof(Nothing.Invoke)); // Better way to get "Invoke"???

            var boType = invoke.ReturnType;


            if (invoke == null)
            {
                throw new Exception($"Unable to load invoke method on ${delegateType.Name}");
            }

            var parameterCount = invoke.GetParameters().Count();

            // If there is more than one CRITERIA parameter than ObjectPortal will create a Tuple
            //if (parameterCount > 1)
            //{
            //    throw new Exception($"Delegate ${delegateType.Name} cannot have more than one method parameter.");
            //}

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);

            MethodInfo method = null;

            // ObjectPortal concrete has two fetch methods
            // One that takes parameters and one that doesn't
            // Chose the right one based on the parameters our delegate
            if (parameterCount == 0)
            {
                method = opType.GetMethod(methodName, new Type[0]);
            }
            else
            {



                List<Type> types = new List<Type>();

                int count = 0;

                foreach (var p in invoke.GetParameters().ToList())
                {
                    types.Add(p.ParameterType);
                    count += 1;
                }

                method = opType.GetMethods().Where(x => x.Name == methodName && x.GetParameters().Count() == count).First();
                method = method.MakeGenericMethod(types.ToArray());

            }


            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, method), delegateType);

            }).As(delegateType);

        }

        public static void RegisterObjectPortalUpdate(this ContainerBuilder builder, Type boType)
        {
            ObjectPortalUpdate(builder, boType, typeof(Update<>), nameof(ObjectPortal<Csla.IBusinessBase>.Update));
        }

        public static void RegisterObjectPortalUpdateChild(this ContainerBuilder builder, Type boType)
        {
            ObjectPortalUpdate(builder, boType, typeof(UpdateChild<>), nameof(ObjectPortal<Csla.IBusinessBase>.UpdateChild));
        }

        private static void ObjectPortalUpdate(ContainerBuilder builder, Type boType, Type objectPortalDelegate, string updateMethodName)
        {

            // Some serious WTF code!

            if (boType == null)
            {
                throw new ArgumentNullException(nameof(boType));
            }

            if (typeof(Delegate).IsAssignableFrom(boType))
            {
                // This is a delegate type
                ObjectPortalUpdate_Delegate(builder, boType, updateMethodName);
                return;
            }

            // Business object type
            // Update with no parameters

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);
            var delegateType = objectPortalDelegate.MakeGenericType(boType);

            MethodInfo updateMethod = null;

            updateMethod = opType.GetMethod(updateMethodName, new Type[1] { boType });

            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, updateMethod), delegateType);

            }).As(delegateType);

        }

        private static void ObjectPortalUpdate_Delegate(this ContainerBuilder builder, Type delegateType, string updateMethodName)
        {

            // Some serious WTF code!

            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new Exception("Only Delegates types allowed.");
            }


            // We assume delegateType is a Delegate. 
            // The business object type we need is the return type of Delegate.Invoke.
            var invoke = delegateType.GetMethod(nameof(Nothing.Invoke)); // Better way to get "Invoke"???



            if (invoke == null)
            {
                throw new Exception($"Unable to load invoke method on ${delegateType.Name}");
            }

            var parameterCount = invoke.GetParameters().Count();

            if (parameterCount == 0 || parameterCount > 2)
            {
                throw new Exception($"Delegate ${delegateType.Name} cannot have 1 or 2 method parameter.");
            }

            var boType = invoke.GetParameters()[0].ParameterType;

            // Need to resolve an IObjectPortal<BusinessObjectType>
            var opType = typeof(IObjectPortal<>).MakeGenericType(boType);

            MethodInfo updateMethod = null;

            // ObjectPortal concrete has two fetch methods
            // One that takes parameters and one that doesn't
            // Chose the right one based on the parameters our delegate
            if (parameterCount == 1)
            {
                updateMethod = opType.GetMethod(updateMethodName, new Type[0]);
            }
            else
            {

                // parameterCount = 1
                updateMethod = opType.GetMethods().Where(x => x.Name == updateMethodName && x.IsGenericMethod).First();

                // We need to match the delegateType signature
                // So Fetch<C> needs to be Fetch<Criteria>
                // Again we look to our invoke for this
                var criteriaType = invoke.GetParameters()[1].ParameterType;
                updateMethod = updateMethod.MakeGenericMethod(new Type[1] { criteriaType });

            }

            builder.Register((c) =>
            {
                var portal = c.Resolve(opType);

                return Convert.ChangeType(Delegate.CreateDelegate(delegateType, portal, updateMethod), delegateType);

            }).As(delegateType);

        }

    }

    public delegate void Update<T>(T Bo) where T : Csla.Core.ITrackStatus;
    public delegate void Update<T, C>(T Bo, C criteria) where T : Csla.Core.ITrackStatus;
    public delegate void UpdateChild<T>(T Bo) where T : Csla.Core.ITrackStatus;
    public delegate void UpdateChild<T, C>(T Bo, C criteria) where T : Csla.Core.ITrackStatus;

    /// <summary>
    /// Abstract BO object creating, fetching and updating each other
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPortal<T> : Csla.Server.ObjectFactory, IObjectPortal<T>
        where T : Csla.Core.ITrackStatus
    {



        Func<T> createT;

        [NonSerialized]
        ILifetimeScope scope;

        public ObjectPortal(Func<T> createT, ILifetimeScope scope)
        {
            this.createT = () =>
            {
                var newT = createT();

                // Tag on the scope
                var dp = newT as IDPBusinessObject; // In the actual implementation we would not use a service locator. Limited by CSLA
                dp.scope = scope;

                return newT;
            };

            this.scope = scope;
        }

        public T Create()
        {
            var result = createT();

            base.MarkNew(result);

            var create = result as IHandleCreate;


            if (create == null)
            {

                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleCreateDI<>).Name).FirstOrDefault();


                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleCreateDI<object>.Create), @interface, o => new object[1] { o });
                }
                // Allow no create() to be called on the new object
            }
            else
            {
                create.Create();
            }

            return result;

        }


        public T Create<C>(C criteria)
        {

            var result = createT();
            base.MarkNew(result);

            var create = result as IHandleCreate<C>;


            if (create == null)
            {
                var criteriaType = criteria.GetType();
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleCreateDI<,>).Name && x.GenericTypeArguments[0] == criteriaType).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleCreateDI<object, object>.Create), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    throw new InvalidOperationException("No IHandleCreate<C> found");
                }
            }
            else
            {
                create.Create(criteria);
            }


            return result;

        }

        public T CreateChild()
        {
            var result = createT();

            base.MarkAsChild(result);
            base.MarkNew(result);

            var create = result as IHandleCreateChild;

            if (create == null)
            {
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleCreateChildDI<>).Name).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleCreateChildDI<object>.CreateChild), @interface, o => new object[] { o });
                }
                // Allow create with no criteria to not have an IHandle
            }
            else
            {
                create.CreateChild();
            }

            return result;

        }

        public T CreateChild<C>(C criteria)
        {
            var result = createT();
            base.MarkAsChild(result);
            base.MarkNew(result);

            var create = result as IHandleCreateChild<C>;

            if (create == null)
            {
                var criteriaType = criteria.GetType();
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleCreateChildDI<,>).Name && x.GenericTypeArguments[0] == criteriaType).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleCreateChildDI<object, object>.CreateChild), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    throw new InvalidOperationException("IHandleCreateChild<C> not implemented");
                }
            }
            else
            {
                create.CreateChild(criteria);
            }

            return result;

        }

        public T Fetch()
        {
            var result = createT();
            base.MarkOld(result);

            var fetch = result as IHandleFetch;


            if (fetch == null)
            {
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleFetchDI<>).Name).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleFetchDI<object>.Fetch), @interface, o => new object[1] { o });
                }
                else
                {
                    throw new InvalidOperationException($"IHandleFetch not found on {typeof(T).Name}");
                }
            }
            else
            {
                fetch.Fetch();
            }

            return result;

        }


        public T Fetch<C>(C criteria)
        {

            var result = createT();
            base.MarkOld(result);

            var fetch = result as IHandleFetch<C>;


            if (fetch == null)
            {
                var criteriaType = criteria.GetType();
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleFetchDI<,>).Name && x.GenericTypeArguments[0] == criteriaType).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleFetchDI<object, object>.Fetch), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    throw new InvalidOperationException($"IHandleFetch<C> not found on {typeof(T).Name}");
                }

            }
            else
            {
                fetch.Fetch(criteria);
            }

            return result;

        }

        public T Fetch<C1, C2>(C1 criteria1, C2 criteria2)
        {

            var tupleCreateMethod = typeof(Tuple).GetMethods().Where(x => x.Name == nameof(Tuple.Create) && x.GetParameters().Count() == 2).First();

            var tuple = tupleCreateMethod
                .MakeGenericMethod(new Type[2] { typeof(C1), typeof(C2) })
                .Invoke(null, new object[2] { criteria1, criteria2 });

            return (T)this.Fetch(tuple);

        }

        public T Fetch<C1, C2, C3>(C1 criteria1, C2 criteria2, C3 criteria3)
        {
            throw new NotImplementedException();
        }

        public T FetchChild()
        {
            var result = createT();
            base.MarkOld(result);
            base.MarkAsChild(result);

            var fetch = result as IHandleFetchChild;


            if (fetch == null)
            {
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleFetchChildDI<>).Name).FirstOrDefault();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleFetchChildDI<object>.FetchChild), @interface, o => new object[] { o });
                }
                else
                {
                    throw new InvalidOperationException($"IHandleFetchChild not found on {typeof(T).Name}");
                }
            }
            else
            {
                fetch.FetchChild();
            }

            return result;

        }

        public T FetchChild<C>(C criteria)
        {
            var result = createT();
            base.MarkOld(result);
            base.MarkAsChild(result);

            var fetch = result as IHandleFetchChild<C>;


            if (fetch == null)
            {
                var criteriaType = criteria.GetType();
                var @interface = result.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleFetchChildDI<,>).Name && x.GenericTypeArguments[0] == criteriaType).First();

                if (@interface != null)
                {
                    return _CallMethod(result, nameof(IHandleFetchChildDI<object, object>.FetchChild), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    throw new InvalidOperationException($"IHandleFetchChild<C> not found on {typeof(T).Name}");
                }
            }
            else
            {
                fetch.FetchChild(criteria);
            }

            return result;

        }

        public T FetchChild<C1, C2>(C1 criteria1, C2 criteria2)
        {
            // We allow the Fetch calls (and delegates) to have multiple parameters
            // But the IHandleXYZ interface can only have one criteria as a parameter
            // with a tuple to handle multiple parameters
            // Convert the multiple parameters to a tuple

            // Convert to a Tuple as that is what IHandle will expect, only one Criteria parameter allowed
            var tupleCreateMethod = typeof(Tuple).GetMethods().Where(x => x.Name == nameof(Tuple.Create) && x.GetParameters().Count() == 2).First();

            var tuple = tupleCreateMethod
                .MakeGenericMethod(new Type[2] { typeof(C1), typeof(C2) })
                .Invoke(null, new object[2] { criteria1, criteria2 });

            // Right now "tuple" is an object 
            // Convert to correct tuple type

            // var tupleType = typeof(Tuple<,>).MakeGenericType(new Type[2] { typeof(C1), typeof(C2) });

            return (T)this.FetchChild(tuple);

        }

        public T FetchChild<C1, C2, C3>(C1 criteria1, C2 criteria2, C3 criteria3)
        {
            throw new NotImplementedException();
        }

        private T _CallMethod(T result, string methodName, Type interfaceType, Func<object, object[]> returnInvokeParams)
        {
            // Thought - For root operations should this create a new scope?? Then when will it be disposed???
            // Example: For 2-Tier applications how will an Update use a single .InstancePerLifetimeScope sql connection and transaction??

            // This is also where we would enforce create authorization rules


            Type dependencyType = interfaceType.GenericTypeArguments[interfaceType.GenericTypeArguments.Count() - 1]; // grab the last parameter

            if (interfaceType != null && dependencyType != null)
            {
                if (!scope.IsRegistered(dependencyType))
                {

                    if (typeof(Delegate).IsAssignableFrom(dependencyType))
                    {
                        throw new NotImplementedException();
                    }
                    else if (dependencyType.IsGenericType)
                    { // Bad way of seeing if it is a Tuple.
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

                        var method = interfaceType.GetMethod(methodName);

                        method.Invoke(result, returnInvokeParams(tuple));
                    }
                }
                else if (scope.IsRegistered(dependencyType))
                {
                    var dep = scope.Resolve(dependencyType);

                    var method = interfaceType.GetMethod(methodName);

                    method.Invoke(result, returnInvokeParams(dep));
                }
                else
                {
                    throw new ObjectPortalOperationNotSupportedException("Fetch with criteria. Dependencies not registered");
                }
            }
            else
            {
                throw new ObjectPortalOperationNotSupportedException("Fetch with criteria not supported");
            }

            return result;

        }


        public void Update(T bo)
        {
            var update = bo as IHandleUpdate;

            if (update != null)
            {
                if (bo.IsDirty)
                {
                    if (bo.IsNew)
                    {
                        update.Insert();
                    }
                    else
                    {
                        update.Update();
                    }
                }
            }

            ITrackStatus ts = bo as ITrackStatus;

            if (ts.IsDirty)
            {
                var @interface = bo.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleUpdateDI<>).Name).First();

                if (ts.IsNew)
                {
                    _CallMethod(bo, nameof(IHandleUpdateDI<object>.Insert), @interface, o => new object[] { o });
                }
                else
                {
                    _CallMethod(bo, nameof(IHandleUpdateDI<object>.Update), @interface, o => new object[] { o });
                }
            }


        }

        public void Update<C>(T bo, C criteria)
        {
            var update = bo as IHandleUpdate<C>;

            if (update != null)
            {
                if (bo.IsDirty)
                {
                    if (bo.IsNew)
                    {
                        update.Insert(criteria);
                    }
                    else
                    {
                        update.Update(criteria);
                    }
                }
            }

            ITrackStatus ts = bo as ITrackStatus;

            if (ts.IsDirty)
            {
                var @interface = bo.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleUpdateDI<>).Name && x.GenericTypeArguments[0] == criteria.GetType()).First();

                if (ts.IsNew)
                {
                    _CallMethod(bo, nameof(IHandleUpdateDI<object, object>.Insert), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    _CallMethod(bo, nameof(IHandleUpdateDI<object, object>.Update), @interface, o => new object[] { criteria, o });
                }
            }

        }

        public void UpdateChild(T bo)
        {
            var update = bo as IHandleUpdateChild;

            if (update != null)
            {
                if (bo.IsDirty)
                {
                    if (bo.IsNew)
                    {
                        update.InsertChild();
                    }
                    else
                    {
                        update.UpdateChild();
                    }
                }
            }

            ITrackStatus ts = bo as ITrackStatus;

            if (ts.IsDirty)
            {
                var @interface = bo.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleUpdateChildDI<>).Name).First();

                if (ts.IsNew)
                {
                    _CallMethod(bo, nameof(IHandleUpdateChildDI<object>.InsertChild), @interface, o => new object[] { o });
                }
                else
                {
                    _CallMethod(bo, nameof(IHandleUpdateChildDI<object>.UpdateChild), @interface, o => new object[] { o });
                }
            }

        }

        public void UpdateChild<C>(T bo, C criteria)
        {
            var update = bo as IHandleUpdateChild<C>;

            if (update != null)
            {
                if (bo.IsDirty)
                {
                    if (bo.IsNew)
                    {
                        update.InsertChild(criteria);
                    }
                    else
                    {
                        update.UpdateChild(criteria);
                    }
                }
            }

            ITrackStatus ts = bo as ITrackStatus;

            if (ts.IsDirty)
            {
                var @interface = bo.GetType().GetInterfaces().Where(x => x.Name == typeof(IHandleUpdateChildDI<,>).Name && x.GenericTypeArguments[0] == criteria.GetType()).First();

                if (ts.IsNew)
                {
                    _CallMethod(bo, nameof(IHandleUpdateChildDI<object, object>.InsertChild), @interface, o => new object[] { criteria, o });
                }
                else
                {
                    _CallMethod(bo, nameof(IHandleUpdateChildDI<object, object>.UpdateChild), @interface, o => new object[] { criteria, o });
                }
            }

        }
    }


    [Serializable]
    public class ObjectPortalOperationNotSupportedException : Exception
    {
        public ObjectPortalOperationNotSupportedException() { }
        public ObjectPortalOperationNotSupportedException(string message) : base(message) { }
        public ObjectPortalOperationNotSupportedException(string message, Exception inner) : base(message, inner) { }
        protected ObjectPortalOperationNotSupportedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
