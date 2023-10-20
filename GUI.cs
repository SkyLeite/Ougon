using Reloaded.Hooks.Definitions;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;

namespace Ougon.GUI
{
    class Debug
    {
        private unsafe GameState* _gameState;
        public Context _context;
        private readonly IReloadedHooks? _hooks;

        public unsafe Debug(IReloadedHooks hooks, GameState* gameState, Context context)
        {
            _gameState = gameState;
            _hooks = hooks;
            _context = context;

            this.InitializeImgui().Wait();
        }


        private async Task InitializeImgui()
        {
            SDK.Init(_hooks);

            await ImguiHook.Create(RenderDebugWindow, new ImguiHookOptions()
            {
                Implementations = new List<IImguiHook>()
                {
                    new ImguiHookDx9(), // `Reloaded.Imgui.Hook.Direct3D9`
                }
            }).ConfigureAwait(false);
        }

        private unsafe void RenderDebugWindow()
        {
            ImGuiNET.ImGui.Begin("Debug");
            ImGuiNET.ImGui.Text($"FPS: {_gameState->fps.ToString("0.##")}");

            if (_context.match == null)
            {
                ImGuiNET.ImGui.Text($"Not currently in a match");
            }
            else
            {
                ImGuiNET.ImGui.Text("Match");
                ImGuiNET.ImGui.SameLine();
                ImGuiNET.ImGui.Text($"Address: {new IntPtr(_context.match).ToString("x")}");

                {
                    ImGuiNET.ImGui.BeginGroup();
                    ImGuiNET.ImGui.SliderInt("Timer", ref _context.match->timer, 0, 10800);
                    ImGuiNET.ImGui.SameLine();
                    ImGuiNET.ImGui.Checkbox("Lock", ref _context.timerLocked);
                    ImGuiNET.ImGui.EndGroup();
                }


                ImGuiNET.ImGui.Text("Player 1");
                {
                    ImGuiNET.ImGui.BeginGroup();
                    ImGuiNET.ImGui.Text($"C1 Address: {new IntPtr(_context.match->p1Character1).ToString("x")}");
                    ImGuiNET.ImGui.Text($"C2 Address: {new IntPtr(_context.match->p1Character2).ToString("x")}");
                    ImGuiNET.ImGui.InputInt("Character 1 Health##1", ref _context.match->p1Character1->inMatch->health);
                    ImGuiNET.ImGui.InputFloat("Character 1 Meter##1", ref _context.match->p1Character1->inMatch->meter);
                    ImGuiNET.ImGui.InputInt("Character 2 Health##1", ref _context.match->p1Character2->inMatch->health);
                    ImGuiNET.ImGui.InputFloat("Character 2 Meter##1", ref _context.match->p1Character2->inMatch->meter);
                    ImGuiNET.ImGui.EndGroup();
                }

                ImGuiNET.ImGui.Text("Player 2");
                {
                    ImGuiNET.ImGui.BeginGroup();
                    ImGuiNET.ImGui.Text($"C1 Address: {new IntPtr(_context.match->p2Character1).ToString("x")}");
                    ImGuiNET.ImGui.Text($"C2 Address: {new IntPtr(_context.match->p2Character2).ToString("x")}");
                    ImGuiNET.ImGui.InputInt("Character 1 Health##2", ref _context.match->p2Character1->inMatch->health);
                    ImGuiNET.ImGui.InputFloat("Character 1 Meter##2", ref _context.match->p2Character1->inMatch->meter);
                    ImGuiNET.ImGui.InputInt("Character 2 Health##2", ref _context.match->p2Character2->inMatch->health);
                    ImGuiNET.ImGui.InputFloat("Character 2 Meter##2", ref _context.match->p2Character2->inMatch->meter);
                    ImGuiNET.ImGui.EndGroup();
                }
            }

            ImGuiNET.ImGui.End();
        }
    }
}
