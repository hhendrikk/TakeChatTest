namespace Server
{
    using Autofac;
    using Contracts;
    using Core;
    using Services;

    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder container)
        {
            container.RegisterType<IdGuidGenerator>().As<IIdGenerator>().SingleInstance();
            container.RegisterType<SocketStore>().As<ISocketStore>().SingleInstance();
            container.RegisterType<ConnectionManager>().As<IConnectionManager>().SingleInstance();
            container.RegisterType<RepositoryMemory>().As<IRepository>().SingleInstance();
            container.RegisterType<CommandTranslate>().As<ICommandTranslate>().SingleInstance();
            container.RegisterType<CommandActions>().As<ICommandActions>().SingleInstance();
            container.RegisterType<ChatService>().As<IChatService>().InstancePerLifetimeScope();
            container.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

        }
    }
}