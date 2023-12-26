using Ougon.Configuration;
using Ougon.Misc;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.Stdcall)]
public delegate void Render();

sealed class RenderHook : IHook<Render>
{
    private FramePacer FramePacer;

    public RenderHook(
        Reloaded.Hooks.Definitions.IReloadedHooks hooks,
        Config config,
        FramePacer framePacer
    )
        : base(hooks)
    {
        this.Config = config;
        this.FramePacer = framePacer;

        this.FramePacer.FPSLimit = this.Config.FPSLimit;
    }

    public override string SignatureBytes => "55 8B EC 83 EC 2C A1 ?? ?? ?? ?? 33 C5 89 45 ?? 53";
    public override Render Hook => this.Render;

    private readonly Config Config;

    public void Render()
    {
        this.OriginalHook.OriginalFunction();
        this.FramePacer.EndFrame();
    }
}
