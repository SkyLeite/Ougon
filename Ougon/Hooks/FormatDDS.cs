using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.Cdecl)]
public unsafe delegate byte* FormatDDS(int* LZLRFile, byte* outBuffer);

sealed class FormatDDSHook : IHook<FormatDDS>
{
    public FormatDDSHook(IReloadedHooks hooks, Context context)
        : base(hooks)
    {
        this.Context = context;
    }

    public override string SignatureBytes => "55 8B EC 83 EC 08 53 56 57 8B 7D";
    public override unsafe FormatDDS Hook => this.FormatDDS;

    private readonly Context Context;

    private unsafe byte* FormatDDS(int* LZLRFile, byte* outBuffer)
    {
        return this.OriginalHook.OriginalFunction(LZLRFile, outBuffer);
    }
}
