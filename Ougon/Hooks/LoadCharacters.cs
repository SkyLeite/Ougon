using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Mod.Interfaces;

namespace Ougon.Hooks;

[Function(CallingConventions.Cdecl)]
public unsafe delegate void LoadCharacters(byte* param1, void* param2, int* param3);

sealed class LoadCharactersHook : IHook<LoadCharacters>
{
    public LoadCharactersHook(IReloadedHooks hooks, Context context, ILogger logger)
        : base(hooks)
    {
        this.Context = context;
        this.Logger = logger;
    }

    public override string SignatureBytes =>
        "55 8B EC 81 EC 90 00 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 8B 45";
    public override unsafe LoadCharacters Hook => this.LoadCharacters;

    private readonly Context Context;
    private readonly ILogger Logger;

    public unsafe void LoadCharacters(byte* param1, void* param2, int* param3)
    {
        var p = new IntPtr(param2);

        this.Logger.WriteLineAsync(
            $"[MyLoadCharacters] param1: {new IntPtr(param1).ToString("x")} | param_2 address: {new IntPtr(param2).ToString("x")} | param_3: {*param3}"
        );

        var char1Pointer = param1 + 0x40;

        // ushort[] characters = new[] { *char1Pointer, *(char1Pointer + 1), *(char1Pointer + 2), *(char1Pointer + 3) };

        var characters = new List<Character> { };

        for (int i = 0; i < 4; i++)
        {
            var characterPointer = char1Pointer + (i * 2);
            var characterID = *characterPointer;
            var characterColor = *(characterPointer + 1);

            characters.Add(Character.fromID(characterID, characterColor));
        }

        var player1 = new Player(new List<Character> { characters[0], characters[2] });
        var player2 = new Player(new List<Character> { characters[1], characters[3] });
        this.Context.fight = new Fight(player1, player2);

        this.OriginalHook.OriginalFunction(param1, param2, param3);
    }
}
