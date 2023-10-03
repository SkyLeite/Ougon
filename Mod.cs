using Ougon_Trainer.Configuration;
using Ougon_Trainer.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Runtime.CompilerServices;
// using Reloaded.Memory;
// using Reloaded.Memory.Pointers;
using System.Diagnostics;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;
using ImGuiNET;
using static ImGuiNET.ImGuiNative;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Imgui;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;

namespace Ougon_Trainer
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly Reloaded.Hooks.Definitions.IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        [Function(CallingConventions.Stdcall)]
        public delegate void Render();
        private IHook<Render> _renderHook;

        [Function(CallingConventions.Stdcall)]
        public delegate void EndScene();
        private IHook<EndScene> _endSceneHook;

        private double FrameInterval; // Time per frame in milliseconds
        private nuint BaseAddress;
        private int vsInitDirectXOffset;
        private int displayAdapterAssignmentOffset;

        public unsafe void MyRender() {
            // Record the start time of the frame
            double startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Call the Render function to render a frame
            _renderHook.OriginalFunction();

            // Calculate how much time has passed since the start of the frame
            double elapsedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime;

            // If the frame took less time than the frame interval, sleep to maintain frame rate
            if (elapsedTime < FrameInterval)
            {
                int sleepTime = (int)(FrameInterval - elapsedTime);
                Thread.Sleep(sleepTime);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct GameStruct
        {
            [FieldOffset(0x4f4)]
            public int* DX9Device;

            [FieldOffset(0x480)]
            public float fps;

            [FieldOffset(0x4b4)]
            public bool debugDisabled;

            [FieldOffset(0x3c)]
            public bool isFullScreen;

            [FieldOffset(0x30)]
            public int isRunning;

            [FieldOffset(0x34)]
            public bool isWindowed;
        }

        public unsafe void MyEndScene()
        {
            var memory = Reloaded.Memory.Memory.Instance;
            //var displayAdapterPointer = (int*)(this.vsInitDirectXOffset + this.displayAdapterAssignmentOffset);
            //var displayAdapterValue = *displayAdapterPointer;
            //var displayAdapter = (int*)(displayAdapterValue + 0x20);
            //var displayAdaptarWhatever = *displayAdapter;

            //_logger.WriteLine($"displayAdaoter: {*this.displayAdapterAddress}");

            //nuint theImportantStructOffset = 0xf17f8;
            //var importantStructAddress = (nuint**)(this.BaseAddress + theImportantStructOffset);
            //memory.ReadWithMarshallingOutParameter<GameStruct>((**importantStructAddress), out var gameStruct);
            //var isWindowed = memory.Read<byte>(importantStructAddress + 0x34);

            //_logger.WriteLine($"fps {memory.Read<float>((**importantStructAddress) + 0x480)}");
            //_logger.WriteLine($"isFullscreen {gameStruct.isFullScreen}");
            //_logger.WriteLine($"isRunning {gameStruct.isRunning}");

            _endSceneHook.OriginalFunction();
        }

        public override void Suspend()
        {
            base.Unload();
            _renderHook.Disable();
        }

        public override void Resume()
        {
            base.Resume();
            _renderHook.Enable();
        }


        private void RenderTestWindow()
        {
            ImGuiNET.ImGui.ShowDemoWindow();
        }

        private async Task InitializeImgui()
        {

            SDK.Init(_hooks);

            await ImguiHook.Create(RenderTestWindow, new ImguiHookOptions()
            {
                Implementations = new List<IImguiHook>()
                {
                    new ImguiHookDx9(), // `Reloaded.Imgui.Hook.Direct3D9`
                }
            }).ConfigureAwait(false);
        }

        unsafe public Mod(ModContext context)
        {

            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            this.FrameInterval = 1000.0 / _configuration.FPSLimit;


            // For more information about this template, please see
            // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

            // If you want to implement e.g. unload support in your mod,
            // and some other neat features, override the methods in ModBase.


            // TODO: Implement some mod logic
            // var meterAddress = baseAddress + 0x0014F3F4;
            // var meterPointer = new Ptr<float>((float*)meterAddress);
            // float * meter = meterPointer;

            var thisProcess = Process.GetCurrentProcess();
            var baseAddress = thisProcess.MainModule.BaseAddress;
            var exeSize = thisProcess.MainModule.ModuleMemorySize;
            this.BaseAddress = (nuint)baseAddress;

            using var scanner = new Scanner((byte*)baseAddress, exeSize);

            var result = scanner.FindPattern("55 8B EC 83 EC 2C A1 ?? ?? ?? ?? 33 C5 89 45 ?? 53");

            var renderPointer = baseAddress + result.Offset;
            if (!result.Found)
                throw new Exception("Signature for getting Render not found.");

            // Create the hook (don't forget to call Activate()).
            _renderHook = _hooks.CreateHook<Render>(MyRender, (long)renderPointer).Activate();

            var endSceneResult = scanner.FindPattern("A1 ?? ?? ?? ?? 8B 80 ?? ?? ?? ?? 50 8B 08 FF 91 A8 ?? ?? ?? 3D 6C 08 76 88 75 ?? 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 59 C3 ?? ?? ?? ?? ?? ?? ?? ?? ??");
            if (!endSceneResult.Found)
                 throw new Exception("EndScene not found");

            var endScenePointer = baseAddress + endSceneResult.Offset;
            _endSceneHook = _hooks.CreateHook<EndScene>(MyEndScene, (long)endScenePointer).Activate();

            var vsInitDirectXResult = scanner.FindPattern("55 8B EC 56 8B 35 ?? ?? ?? ?? 68 ?? ?? ?? ?? E8");
            if (!vsInitDirectXResult.Found)
                 throw new Exception("vsInitDirectX not found");

            this.vsInitDirectXOffset = vsInitDirectXResult.Offset;
            this.displayAdapterAssignmentOffset = 0x1e;
            this.InitializeImgui().Wait();


            // var devicePointer = (int**)(endScenePointer + 4);
            // var device = *devicePointer;

            //_logger.WriteLine((*device).ToString());

            //var igContext = ImGuiNET.ImGuiNative.igCreateContext(null);
            //ImGuiNET.ImGuiNative.ImGui_ImplDX9_Init(device);
            //ImGui_ImplDX9_NewFrame();
            //igShowDemoWindow((byte*)1);
            //igEndFrame();
            //igRender();
            //var data = igGetDrawData();
            //_logger.WriteLine((*data).Valid.ToString());

        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");

            this.FrameInterval = 1000.0 / configuration.FPSLimit; 
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}