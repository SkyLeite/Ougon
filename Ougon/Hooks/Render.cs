using Ougon.Configuration;
using Reloaded.Hooks.Definitions.X86;

namespace Ougon.Hooks;

[Function(CallingConventions.Stdcall)]
public delegate void Render();

class RenderHook : IHook<Render>
{
    public RenderHook(Reloaded.Hooks.Definitions.IReloadedHooks hooks, Config config) : base(hooks) {
        this.Config = config;
    }

    public override string SignatureBytes => "55 8B EC 83 EC 2C A1 ?? ?? ?? ?? 33 C5 89 45 ?? 53";
    public override Render Hook => this.Render;

    private readonly Config Config;

    public void Render()
    {
        var frameInterval = 1000.0 / this.Config.FPSLimit;

        // Record the start time of the frame
        double startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        // Call the Render function to render a frame
        this.OriginalHook.OriginalFunction();

        // Calculate how much time has passed since the start of the frame
        double elapsedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime;

        // If the frame took less time than the frame interval, sleep to maintain frame rate
        if (elapsedTime < frameInterval)
        {
            int sleepTime = (int)(frameInterval - elapsedTime);
            Thread.Sleep(sleepTime);
        }
    }
}
