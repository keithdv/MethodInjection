using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ObjectPortal;
using Example.Dal;

namespace Example.Lib
{
    public class LibModule : Autofac.Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Root>().As<IRoot>();
            builder.RegisterType<BusinessItem>().As<IBusinessItem>();
            builder.RegisterType<BusinessItemList>().As<IBusinessItemList>();

            // Need to find a way to make this generic
            // Delegates and generics do not play nice together!!

            //builder.Register<FetchRoot>((c) =>
            //{
            //    var portal = c.Resolve<IObjectPortal<IRoot>>();
            //    return () => portal.Fetch(); // C# lets you implicitly convert a lamda to a delegate...can't do this anywhere else!
            //});

            builder.Register<Func<BusinessItemDto, IBusinessItem>>(c =>
            {
                var portal = c.Resolve<IObjectPortal<IBusinessItem>>();

                return (d) => portal.Fetch(d);

            });

            // Update - Best I came up with
            builder.RegisterObjectPortalFetch(typeof(FetchRoot));
            builder.RegisterObjectPortalFetch(typeof(FetchRootGuid));

            // Update Nov 21st
            // You no longer need to register all of the factory delegates
            // ObjectPortal will automatically recognize and create them for you on the fly
            // However I like the approach of registering them
            // Because I think it will be better peformance
            
            // With both cases there's a catch
            // if you forget to register them you won't get an error
            // Autofac will automatically create a delegate for you
            // but it will not call the ObjectPortal method for you

            //builder.RegisterObjectPortalFetchChild(typeof(FetchChildBusinessItem));
            //builder.RegisterObjectPortalFetchChild(typeof(FetchChildBusinessItemGuid));
            ////builder.RegisterObjectPortalFetchChild(typeof(FetchChildBusinessItemList));
            //builder.RegisterObjectPortalFetchChild(typeof(FetchChildBusinessItemListGuid));

            ///// I don't think registering these are neccessary
            ///// Can't ObjectPortal realize that the dependency you are asking for is a Delegate
            ///// and construct the delegate on the fly??
            //builder.RegisterObjectPortalCreateChild(typeof(CreateChildBusinessItemList));
            //builder.RegisterObjectPortalCreateChild(typeof(CreateChildBusinessItem));
            //builder.RegisterObjectPortalCreateChild(typeof(CreateChildBusinessItemListGuid));
            //builder.RegisterObjectPortalCreateChild(typeof(CreateChildBusinessItemGuid));

            builder.RegisterObjectPortalUpdate(typeof(IRoot));
            builder.RegisterObjectPortalUpdateChild(typeof(IBusinessItemList));
            builder.RegisterObjectPortalUpdateChild(typeof(UpdateChildBusinessItem));

        }

    }
}
