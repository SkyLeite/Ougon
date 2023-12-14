using System.ComponentModel;
using Ougon.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;

namespace Ougon.Configuration
{
    public class Config : Configurable<Config>
    {
        /*
            User Properties:
                - Please put all of your configurable properties here.

            By default, configuration saves as "Config.json" in mod user config folder.
            Need more config files/classes? See Configuration.cs

            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */
        [DisplayName("FPS Limit")]
        [Description(
            "Set a manual FPS limit. Values different from 60 will cause issues. Cannot go above your monitor's refresh rate."
        )]
        [DefaultValue(60)]
        public int FPSLimit { get; set; } = 60;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        //
    }
}
