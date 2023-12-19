namespace Ougon.Hooks;

sealed class HookService
{
    public RenderHook Render;
    public EndSceneHook EndScene;
    public LoadCharactersHook LoadCharacters;
    public DebugHook Debug;
    public CalculateHealthHook CalculateHealth;
    public TickMatchHook TickMatch;
    public FormatDDSHook FormatDDS;
    public PlaySequenceHook PlaySequence;

    public unsafe HookService(
        RenderHook render,
        EndSceneHook endScene,
        LoadCharactersHook loadCharacters,
        DebugHook debug,
        CalculateHealthHook calculateHealth,
        TickMatchHook tickMatch,
        FormatDDSHook formatDDS,
        PlaySequenceHook playSequence
    )
    {
        this.Render = render;
        this.EndScene = endScene;
        this.LoadCharacters = loadCharacters;
        this.Debug = debug;
        this.CalculateHealth = calculateHealth;
        this.TickMatch = tickMatch;
        this.FormatDDS = formatDDS;
        this.PlaySequence = playSequence;
    }

    public void Enable()
    {
        this.Render.Enable();
        this.EndScene.Enable();
        this.LoadCharacters.Enable();
        this.Debug.Enable();
        this.CalculateHealth.Enable();
        this.TickMatch.Enable();
        this.FormatDDS.Enable();
        this.PlaySequence.Enable();
    }

    public void Disable()
    {
        this.Render.Disable();
        this.EndScene.Disable();
        this.LoadCharacters.Disable();
        this.Debug.Disable();
        this.CalculateHealth.Disable();
        this.TickMatch.Disable();
        this.FormatDDS.Disable();
        this.PlaySequence.Disable();
    }
}
