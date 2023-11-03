using Reloaded.Hooks.Definitions;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;
using Reloaded.Mod.Interfaces;
using DearImguiSharp;

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

        private unsafe void RenderPlayer(int playerIndex, Character*[] characters)
        {
                ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                if (ImGui.TreeNodePtr(playerIndex, $"Player {playerIndex.ToString()}"))
                {
                    int i = 0;
                    foreach (Character* character in characters) {
                        ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                        if (ImGui.TreeNodePtr(i, $"Character {i + 1}")) {
                            ImGui.Text($"Address: {new IntPtr(character).ToString("x")}");
                            ImGui.InputInt($"Health##{playerIndex}-{i}", ref character->inMatch->health, 0, 1, 0);
                            ImGui.InputFloat($"Meter##{playerIndex}-{i}", ref character->inMatch->meter, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Stun##{playerIndex}-{i}", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.TreePop();
                        }

                        i = i + 1;
                    }

                    ImGui.TreePop();
                }
        }

        private unsafe void RenderDebugWindow()
        {
            bool isDefaultOpen = true;
            ImGui.Begin("Debug", ref isDefaultOpen, 0);

            var defaultWindowSize = new ImVec2();
            defaultWindowSize.X = 300;
            defaultWindowSize.Y = 400;
            ImGui.SetWindowSizeVec2(defaultWindowSize, (int)ImGuiCond.FirstUseEver);

            if (ImGui.CollapsingHeaderBoolPtr("General", ref isDefaultOpen, 0)) {
                ImGui.Text($"FPS: {_gameState->fps.ToString("0.##")}");
            }

            if (_context.match != null && _context.match->isValid())
            {
                if (ImGui.CollapsingHeaderBoolPtr("Current Match", ref isDefaultOpen, 0))
                {
                    ImGui.Text($"Match Address: {new IntPtr(_context.match).ToString("x")}");

                    {
                        ImGui.BeginGroup();
                        ImGui.SliderInt("Timer", ref _context.match->timer, 0, 10800, "", 0);
                        ImGui.SameLine(0, 0);
                        ImGui.Checkbox("Lock", ref _context.timerLocked);
                        ImGui.EndGroup();
                    }


                    ImGui.Separator();

                    RenderPlayer(1, _context.match->player1Characters());
                    ImGui.Separator();
                    RenderPlayer(2, _context.match->player2Characters());
                }
            }

            ImGui.End();
        }
    }
}
