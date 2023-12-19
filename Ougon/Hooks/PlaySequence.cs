using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.MicrosoftThiscall)]
public unsafe delegate void PlaySequence(
    GameCharacter* character,
    int sequenceIndex,
    int existance,
    int zero
);

class PlaySequenceHook : IHook<PlaySequence>
{
    public PlaySequenceHook(IReloadedHooks hooks, Context context) : base(hooks)
    {
        this.Context = context;
    }

    public override string SignatureBytes => "55 8B EC 56 57 8B 7D ?? 8B F1 83 FF FF";
    public override unsafe PlaySequence Hook => this.PlaySequence;

    private readonly Context Context;

    private unsafe void PlaySequence(
        GameCharacter* character,
        int sequenceIndex,
        int existance,
        int zero
    )
    {
        this.OriginalHook.OriginalFunction(character, sequenceIndex, existance, zero);
    }
}
