using Ougon.Configuration;
using Ougon.Template;
using Reloaded.Mod.Interfaces;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;

namespace Ougon
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

        private unsafe GameState* _gameState;

        [Function(CallingConventions.Fastcall)]
        public unsafe delegate void LoadMoves(bool *character);
        private IHook<LoadMoves> _loadMovesHook;

        [Function(CallingConventions.Stdcall)]
        public delegate void Render();
        private IHook<Render> _renderHook;

        [Function(CallingConventions.Stdcall)]
        public delegate void EndScene();
        private IHook<EndScene> _endSceneHook;

        [Function(CallingConventions.Cdecl)]
        public unsafe delegate void LoadCharacters(byte* param_1, void* param_2, int* param_3);
        private IHook<LoadCharacters> _loadCharactersHook;

        [Function(CallingConventions.Cdecl)]
        public unsafe delegate void Debug(char* param_1, char* param2);
        private IHook<Debug> _debugHook;

        [Function(CallingConventions.MicrosoftThiscall)]
        public unsafe delegate void CalculateHealth(Something* _this, int param_1);
        private IHook<CalculateHealth> _calculateHealthHook;

        [Function(CallingConventions.MicrosoftThiscall)]
        public unsafe delegate void TickMatch(Match* match, char param_1);
        private IHook<TickMatch> _tickMatchHook;

        private double FrameInterval; // Time per frame in milliseconds
        private nuint BaseAddress;
        private nint _setTimerAddress;
        private Ougon.GUI.Debug? _gui;
        private Context _context;

        // AttackType:
        // 0 = normal hit
        // 1 = grab / command grab
        // 2 = OTG
        private unsafe void MyCalculateHealth(Something* _this, int attackType)
        {
            var character = *(int*)((int)_this + 0x2a604);
            var health = *(short*)(character + 0x2aa9c);
            var grayHealth = *(short*)(character + 0x2aaaa);
            _logger.WriteLineAsync($"[MyCalculateHealth] attackType: {attackType} | baseAddress: {new IntPtr((int)_this).ToString("x")} | characterAddress: {new IntPtr((int)_this + 0x2a604).ToString("x")} | originalHealth: {health} | grayHealth: {grayHealth}");
            _calculateHealthHook.OriginalFunction(_this, attackType);
        }

        public unsafe void MyLoadCharacters(byte* param_1, void* param_2, int* param_3)
        {
            var p = new IntPtr(param_2);
            _logger.WriteLineAsync($"[MyLoadCharacters] param1: {new IntPtr(param_1).ToString("x")} | param_2 address: {new IntPtr(param_2).ToString("x")} | param_3: {*param_3}");

            var char1Pointer = param_1 + 0x40;

            // ushort[] characters = new[] { *char1Pointer, *(char1Pointer + 1), *(char1Pointer + 2), *(char1Pointer + 3) };

            var characters = new List<Character> {};

            for (int i = 0; i < 4; i++) {
                var characterPointer = char1Pointer + (i * 2);
                var characterID = *characterPointer;
                var characterColor = *(characterPointer + 1);

                characters.Add(Character.fromID(characterID, characterColor));
            }

            var player1 = new Player(new List<Character> { characters[0], characters[2] });
            var player2 = new Player(new List<Character> { characters[1], characters[3] });
            this._context.fight = new Fight(player1, player2);

            _loadCharactersHook.OriginalFunction(param_1, param_2, param_3);
        }

        public unsafe void MyDebug(char* param_1, char* param_2)
        {
            _logger.WriteLineAsync($"[MyDebug] param_1: {Marshal.PtrToStringAnsi((IntPtr)param_1) + Marshal.PtrToStringAnsi((IntPtr)param_2)}");
        }

        public unsafe void MyRender()
        {
            // var timerPointer = (int**)(_setTimerAddress + 2);
            // if (timerPointer != null)
            // {
            //     var timerAddress = *timerPointer;
            //     *timerAddress = 3000;
            // }

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

        public unsafe void MyEndScene()
        {
            var str = (GameState**)(BaseAddress + 0xf17f8);
            this._gameState = *str;

            new Ougon.GUI.Debug(_hooks, _gameState, _context);

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


        unsafe public Mod(ModContext context)
        {

            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;
            _context = new Context();
            this.FrameInterval = 1000.0 / _configuration.FPSLimit;

            var thisProcess = Process.GetCurrentProcess();
            if (thisProcess.MainModule == null)
                throw new Exception("Could not find the process' main module");

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

            var loadCharactersResult = scanner.FindPattern("55 8B EC 81 EC 90 00 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 8B 45");
            if (!loadCharactersResult.Found)
                throw new Exception("LoadCharacters not found");

            var loadCharactersPointer = baseAddress + loadCharactersResult.Offset;
            _loadCharactersHook = _hooks.CreateHook<LoadCharacters>(MyLoadCharacters, (long)loadCharactersPointer).Activate();

            var debugResult = scanner.FindPattern("55 8B EC 81 EC 04 01 00 00 A1 ?? ?? ?? ?? 33 C5 89 45 ?? 83 3D ?? ?? ?? ?? 00");
            if (!debugResult.Found)
                throw new Exception("Debug not found");

            var debugPointer = baseAddress + debugResult.Offset;
            _debugHook = _hooks.CreateHook<Debug>(MyDebug, (long)debugPointer).Activate();

            var calculateHealthHook = scanner.FindPattern("55 8B EC 8B 45 ?? 53 56 57 8B F9 8B 8F");
            if (!calculateHealthHook.Found)
                throw new Exception("CalculateHealth not found");

            var calculateHealthPointer = baseAddress + calculateHealthHook.Offset;
            _calculateHealthHook = _hooks.CreateHook<CalculateHealth>(MyCalculateHealth, (long)calculateHealthPointer).Activate();

            var tickMatchHook = scanner.FindPattern("55 8B EC 83 EC 0C 56 8B F1 57");
            if (!tickMatchHook.Found)
                throw new Exception("TickMatch not found");

            var tickMatchPointer = baseAddress + tickMatchHook.Offset;
            _tickMatchHook = _hooks.CreateHook<TickMatch>(MyTickMatch, (long)tickMatchPointer).Activate();
        }

        private unsafe string GetAddr(void* ptr)
        {
            return new IntPtr(ptr).ToString("x");
        }

        private unsafe void MyTickMatch(Match* match, char param_1)
        {
            _context.match = match;
            _tickMatchHook.OriginalFunction(match, param_1);

            if (_context.timerLocked == true)
            {
                _context.match->timer = 10800;
            }
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
