namespace Example.Lib
{
    public interface IRoot : Csla.IBusinessBase
    {
        IBusinessItemList BusinessItemList { get; set; }

    }
}