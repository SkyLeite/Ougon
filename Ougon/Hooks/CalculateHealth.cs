using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Mod.Interfaces;

namespace Ougon.Hooks;

[Function(CallingConventions.MicrosoftThiscall)]
public unsafe delegate void CalculateHealth(Something* that, int attackType);

sealed class CalculateHealthHook : IHook<CalculateHealth>
{
    public CalculateHealthHook(IReloadedHooks hooks, ILogger logger)
        : base(hooks)
    {
        this.Logger = logger;
    }

    public override string SignatureBytes => "55 8B EC 8B 45 ?? 53 56 57 8B F9 8B 8F";
    public override unsafe CalculateHealth Hook => this.CalculateHealth;

    private readonly ILogger Logger;

    // AttackType:
    // 0 = normal hit
    // 1 = grab / command grab
    // 2 = OTG
    private unsafe void CalculateHealth(Something* that, int attackType)
    {
        var character = *(int*)((int)that + 0x2a604);
        var health = *(short*)(character + 0x2aa9c);
        var grayHealth = *(short*)(character + 0x2aaaa);
        this.Logger.WriteLineAsync(
            $"[MyCalculateHealth] attackType: {attackType} | baseAddress: {new IntPtr((int)that).ToString("x")} | characterAddress: {new IntPtr((int)that + 0x2a604).ToString("x")} | originalHealth: {health} | grayHealth: {grayHealth}"
        );
        this.OriginalHook.OriginalFunction(that, attackType);
    }
}
