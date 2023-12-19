using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Mod.Interfaces;

namespace Ougon.Hooks;

[Function(CallingConventions.Cdecl)]
public unsafe delegate void Debug(char* param_1, char* param2);

class DebugHook : IHook<Debug>
{
    public DebugHook(IReloadedHooks hooks, ILogger logger) : base(hooks)
    {
        this.Logger = logger;
    }

    public override string SignatureBytes => "55 8B EC 81 EC 04 01 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 83 3D ?? ?? ?? ?? 00";
    public override unsafe Debug Hook => this.Debug;

    private readonly ILogger Logger;

    public unsafe void Debug(char* param_1, char* param_2)
    {
        this.Logger.WriteLineAsync(
            $"[MyDebug] param_1: {Marshal.PtrToStringAnsi((IntPtr)param_1) + Marshal.PtrToStringAnsi((IntPtr)param_2)}"
        );
    }

}
