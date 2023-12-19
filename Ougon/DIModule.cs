using Ninject.Modules;
using Ougon.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Mod.Interfaces;

namespace Ougon.DI;

class Module : NinjectModule
{
    private readonly ModContext modContext;

    public Module(ModContext modContext) {
        this.modContext = modContext;
    }

    public override void Load()
    {
        if (this.modContext.Hooks == null)
            throw new Exception("Hooks is null");

        Bind<IModLoader>().ToConstant(this.modContext.ModLoader);
        Bind<IReloadedHooks>().ToConstant(this.modContext.Hooks);
        Bind<ILogger>().ToConstant(this.modContext.Logger);
        Bind<IMod>().ToConstant(this.modContext.Owner);
        Bind<Configuration.Config>().ToConstant(this.modContext.Configuration);
        Bind<IModConfig>().ToConstant(this.modContext.ModConfig);

        Bind<Context>().ToSelf().InSingletonScope();
        Bind<Hooks.HookService>().ToSelf().InSingletonScope();
        Bind<GUI.Debug>().ToSelf().InSingletonScope();
    }
}
