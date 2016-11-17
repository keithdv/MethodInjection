using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ObjectPortal
{
    public interface IHandleCreate
    {

        void Create();

    }

    public interface IHandleCreate<C>
    {

        void Create(C criteria);

    }

    public interface IHandleCreateDI<D>
    {

        void Create(D Dependencies);

    }

    public interface IHandleCreateDI<C, D>
    {

        void Create(C criteria, D dependencies);

    }

    public interface IHandleCreateChild
    {

        void CreateChild();

    }

    public interface IHandleCreateChild<C>
    {

        void CreateChild(C criteria);

    }

    public interface IHandleCreateChildDI<D>
    {

        void CreateChild(D Dependencies);

    }

    public interface IHandleCreateChildDI<C, D>
    {

        void CreateChild(C criteria, D dependencies);

    }

    public interface IHandleFetch
    {

        void Fetch();

    }

    public interface IHandleFetch<C>
    {

        void Fetch(C criteria);

    }

    public interface IHandleFetchDI<D>
    {

        void Fetch(D Dependencies);

    }

    public interface IHandleFetchDI<C, D>
    {

        void Fetch(C criteria, D dependencies);

    }

    public interface IHandleFetchChild
    {

        void FetchChild();

    }

    public interface IHandleFetchChild<C>
    {

        void FetchChild(C criteria);

    }

    public interface IHandleFetchChildDI<D>
    {

        void FetchChild(D Dependencies);

    }

    public interface IHandleFetchChildDI<C, D>
    {

        void FetchChild(C criteria, D dependencies);

    }

    public interface IHandleUpdate
    {
        void Insert();
        void Update();
    }

    public interface IHandleUpdate<C>
    {
        void Insert(C criteria);
        void Update(C criteria);
    }

    public interface IHandleUpdateDI<D>
    {
        void Insert(D dependency);
        void Update(D dependency);
    }


    public interface IHandleUpdateDI<C, D>
    {
        void Insert(C criteria, D dependency);
        void Update(C criteria, D dependency);
    }

    public interface IHandleUpdateChild
    {
        void InsertChild();
        void UpdateChild();
    }

    public interface IHandleUpdateChild<C>
    {
        void InsertChild(C criteria);
        void UpdateChild(C criteria);
    }

    public interface IHandleUpdateChildDI<D>
    {
        void InsertChild(D dependency);
        void UpdateChild(D dependency);
    }


    public interface IHandleUpdateChildDI<C, D>
    {
        void InsertChild(C criteria, D dependency);
        void UpdateChild(C criteria, D dependency);
    }

}
