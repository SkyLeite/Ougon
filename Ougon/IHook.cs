using System.Diagnostics;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;

namespace Ougon.Hooks;

abstract class IHook<TFunction>  {
    public abstract string SignatureBytes { get; }
    public abstract TFunction Hook { get; }

    public Reloaded.Hooks.Definitions.IHook<TFunction> OriginalHook { get; set; }

    public IReloadedHooks Hooks { get; set; }

    protected IHook(IReloadedHooks hooks) {
        this.Hooks = hooks;
        this.OriginalHook = this.CreateHook();
    }

    public void Enable() {
        this.OriginalHook.Enable();
    }

    public void Disable() {
        this.OriginalHook.Disable();
    }

    public unsafe Reloaded.Hooks.Definitions.IHook<TFunction> CreateHook() {
        var thisProcess = Process.GetCurrentProcess();
        if (thisProcess.MainModule == null)
            throw new Exception("Could not find the process' main module");

        var exeSize = thisProcess.MainModule.ModuleMemorySize;

        var baseAddress = thisProcess.MainModule.BaseAddress;
        using var scanner = new Scanner((byte*)baseAddress, exeSize);

        var playSequenceHook = scanner.FindPattern(this.SignatureBytes);
        if (!playSequenceHook.Found)
            throw new Exception("Playsequence Not Found");

        var playSequencePointer = baseAddress + playSequenceHook.Offset;
        return this.Hooks.CreateHook<TFunction>(this.Hook, (long)playSequencePointer).Activate();
    }
}
