using Ninject;
using Ougon.Configuration;
using Ougon.GUI;
using Ougon.Template;
using Reloaded.Mod.Interfaces;

namespace Ougon
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        private readonly StandardKernel kernel;
        private readonly ILogger logger;
        private readonly IModConfig modConfig;
        private readonly Hooks.HookService ougonHooks;
        private readonly Debug debugUi;

        public override void Suspend()
        {
            base.Unload();
            this.ougonHooks.Disable();
        }

        public override void Resume()
        {
            base.Resume();
            this.ougonHooks.Enable();
        }

        public unsafe Mod(ModContext context)
        {
            var diModule = new DI.Module(context);
            this.kernel = new StandardKernel(diModule);

            this.logger = this.kernel.Get<ILogger>();
            this.modConfig = this.kernel.Get<IModConfig>();
            this.ougonHooks = this.kernel.Get<Hooks.HookService>();
            this.debugUi = this.kernel.Get<GUI.Debug>();
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            this.kernel.Rebind<Configuration.Config>().ToConstant(configuration);
            this.logger.WriteLine($"[{this.modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}
