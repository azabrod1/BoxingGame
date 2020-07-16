using System;
using Autofac;
using Boxing.FighterRating;
using Main;

namespace Utilities
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<FighterCache>().As<IFighterRating>();

            //builder.RegisterAssemblyTypes(Assembly.Load(nameof(DemoLibrary)))
            //    .Where(t => t.Namespace.Contains("Utilities"))
            //    .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));

            return builder.Build();
        }
    }
}
