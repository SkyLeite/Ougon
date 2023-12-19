using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.MicrosoftThiscall)]
public unsafe delegate void TickMatch(Match* match, char param_1);

class TickMatchHook : IHook<TickMatch>
{
    public TickMatchHook(IReloadedHooks hooks, Context context) : base(hooks)
    {
        this.Context = context;
    }

    public override string SignatureBytes => "55 8B EC 83 EC 0C 56 8B F1 57";
    public override unsafe TickMatch Hook => this.TickMatch;

    private readonly Context Context;

    private unsafe void TickMatch(Match* match, char param_1)
    {
        this.Context.match = match;
        this.OriginalHook.OriginalFunction(match, param_1);

        if (this.Context.timerLocked == true)
        {
            this.Context.match->timer = 10800;
        }
    }
}
