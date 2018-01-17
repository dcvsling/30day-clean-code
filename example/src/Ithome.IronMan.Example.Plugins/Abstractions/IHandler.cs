namespace Ithome.IronMan.Example.Plugins
{

    public interface IHandler
    {
        void Handle<T>(T context);
    }
}
