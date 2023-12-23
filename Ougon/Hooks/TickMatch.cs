using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Mod.Interfaces;

namespace Ougon.Hooks;

[Function(CallingConventions.MicrosoftThiscall)]
public unsafe delegate void TickMatch(Match* match, char param1);

sealed class TickMatchHook : IHook<TickMatch>
{
    public TickMatchHook(IReloadedHooks hooks, Context context, ILogger logger)
        : base(hooks)
    {
        this.Context = context;
        this.Logger = logger;
    }

                                           // 55 8B EC 83 EC ? 56 8B F1 57 89 75 F4
    public override string SignatureBytes => "55 8B EC 83 EC 0C 56 8B F1 57";
    public override unsafe TickMatch Hook => this.TickMatch;

    private readonly Context Context;

    public ILogger Logger { get; }

    private unsafe void TickMatch(Match* match, char param1)
    {
        this.Context.match = match;

        this.OriginalHook.OriginalFunction(match, param1);

        if (
            match->Characters().All(character => ((GameCharacter*)character)->hitstopFrameCount == 0 && ((GameCharacter*)character)->superFreezeFrameCount == 0)
        ) {
            var character = this.Context.match->player1Characters()[0];

            if (this.Context.FrameHistory.Count > 60 || character->currentSequence->IsIdle()) {
                this.Context.FrameHistory.Reset();
            }

            if (character->currentSequence->IsIdle()) {
                if (this.Context.FrameHistory.Count > 0) {
                    this.Context.FrameHistory.Add((nint)character->currentFrame);
                }
            } else {
                this.Context.FrameHistory.Add((nint)character->currentFrame);
            }
        }

        // foreach (var character in match->Characters()) {
        //     if (character->hitstopFrameCount == 0) {
        //         this.Context.shouldUpdate = true;
        //         break;
        //     }
        // }

        if (this.Context.timerLocked == true)
        {
            this.Context.match->timer = 10800;
        }
    }
}
