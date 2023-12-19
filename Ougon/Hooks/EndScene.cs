using System.Diagnostics;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.Stdcall)]
public delegate void EndScene();

sealed class EndSceneHook : IHook<EndScene>
{
    public EndSceneHook(IReloadedHooks hooks, Context context)
        : base(hooks)
    {
        this.Context = context;
    }

    public override string SignatureBytes =>
        "A1 ?? ?? ?? ?? 8B 80 ?? ?? ?? ?? 50 8B 08 FF 91 A8 ?? ?? ?? 3D 6C 08 76 88 75 ?? 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 59 C3 ?? ?? ?? ?? ?? ?? ?? ?? ??";
    public override EndScene Hook => this.EndScene;

    private readonly Context Context;

    public void EndScene()
    {
        unsafe
        {
            if (this.Context.gameState == null)
            {
                var thisProcess = Process.GetCurrentProcess();
                if (thisProcess.MainModule == null)
                    throw new Exception("Could not find the process' main module");

                var baseAddress = thisProcess.MainModule.BaseAddress;

                var str = (GameState**)(baseAddress + 0xf17f8);
                this.Context.gameState = *str;
            }
        }

        this.OriginalHook.OriginalFunction();
    }
}
