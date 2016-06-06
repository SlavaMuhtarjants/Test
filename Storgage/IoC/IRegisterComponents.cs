namespace Weezlabs.Storgage.IoC
{
    using Castle.Windsor;

    public interface IRegisterComponents
    {
        void Register(IWindsorContainer container);
    }
}
