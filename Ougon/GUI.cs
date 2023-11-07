using Reloaded.Hooks.Definitions;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;
using DearImguiSharp;
using TextCopy;

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

        private unsafe void RenderPlayer(int playerIndex, GameCharacter*[] characters)
        {
                var player = playerIndex switch {
                    1 => _context.fight?.player1,
                    2 => _context.fight?.player2,
                    _ => null
                };

                ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                if (ImGui.TreeNodePtr(playerIndex, $"Player {playerIndex.ToString()}"))
                {
                    int i = 0;
                    foreach (GameCharacter* character in characters) {
                        ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);

                        var name = player == null ? "Character" : player.characters[i].name;
                        var color = player == null ? 0 : player.characters[i].color;

                        if (ImGui.TreeNodePtr(i, $"{name} ({color})")) {
                            RenderPointer("", character);
                            ImGui.InputInt($"Health##{playerIndex}-{i}", ref character->inMatch->health, 0, 1, 0);
                            ImGui.InputFloat($"Meter##{playerIndex}-{i}", ref character->inMatch->meter, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Stun##{playerIndex}-{i}", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.InputFloat($"Position X##{playerIndex}-{i}", ref character->inMatch->positionX, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Position Y##{playerIndex}-{i}", ref character->inMatch->positionY, 0.0f, 0.0f, "%.3f", 0);

                            if (ImGui.TreeNodePtr(1, "Moves"))
                            {
                                RenderSequence("Idle", playerIndex, 1, character->_5);

                                RenderSequence("6->4", playerIndex, 2, character->_6to4);
                                RenderSequence("5->2", playerIndex, 3, character->_5to2);

                                RenderSequence("Crouching", playerIndex, 4, character->_2);

                                RenderSequence("3->1", playerIndex, 5, character->_3to1);
                                RenderSequence("2->5", playerIndex, 6, character->_2to5);

                                RenderSequence("Walk forward", playerIndex, 7, character->_6);
                                RenderSequence("Walk backward", playerIndex, 8, character->_4);

                                RenderSequence("Neutral jump", playerIndex, 9, character->_8);
                                RenderSequence("Forward jump", playerIndex, 10, character->_9);
                                RenderSequence("Backward jump", playerIndex, 11, character->_7);

                                RenderSequence("Dash", playerIndex, 12, character->_66);
                                RenderSequence("Backdash", playerIndex, 13, character->_44);

                                if (ImGui.TreeNodePtr(14, "A")) {
                                    RenderSequence("Far 5A", playerIndex, 1, character->far5A);
                                    RenderSequence("Close 5A", playerIndex, 2, character->close5A);
                                    RenderSequence("2A", playerIndex, 3, character->_2A);
                                    RenderSequence("8A", playerIndex, 4, character->j8A);
                                    RenderSequence("9A", playerIndex, 5, character->j9A);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodePtr(15, "B")) {
                                    RenderSequence("Far 5B", playerIndex, 1, character->far5B);
                                    RenderSequence("Close 5B", playerIndex, 2, character->close5B);
                                    RenderSequence("6B", playerIndex, 3, character->_6B);
                                    RenderSequence("2B", playerIndex, 4, character->_2B);
                                    RenderSequence("8B", playerIndex, 5, character->j8B);
                                    RenderSequence("9B", playerIndex, 6, character->j9B);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodePtr(16, "C")) {
                                    RenderSequence("Far 5C", playerIndex, 1, character->far5C);
                                    RenderSequence("Close 5C", playerIndex, 2, character->close5C);
                                    RenderSequence("6C", playerIndex, 3, character->_6C);
                                    RenderSequence("2C", playerIndex, 4, character->_2C);
                                    RenderSequence("8C", playerIndex, 5, character->j8C);
                                    RenderSequence("9C", playerIndex, 6, character->j9C);

                                    ImGui.TreePop();
                                }

                                RenderSequence("Grab", playerIndex, 17, character->grab);
                                RenderSequence("Grab whiff", playerIndex, 18, character->grabWhiff);
                                RenderSequence("Dizzy", playerIndex, 19, character->dizzy);
                                RenderSequence("Appeal", playerIndex, 20, character->appeal);
                                RenderSequence("Attack Touch", playerIndex, 21, character->attackTouch);

                                if (ImGui.TreeNodePtr(17, "Unknown")) {
                                    int sequenceIndex = 0;
                                    foreach (var sequence in GameCharacter.GetSequences(character)) {
                                        RenderSequence($"Unknown 0x{new IntPtr(sequence).ToString("x")} {(0x524 + (0x4 * sequenceIndex)).ToString("x")}", playerIndex, sequenceIndex, sequence);
                                        sequenceIndex += 1;
                                    }

                                    ImGui.TreePop();
                                }

                                ImGui.TreePop();
                            }

                            ImGui.TreePop();
                        }

                        i = i + 1;
                    }

                    ImGui.TreePop();
                }
        }

        private static unsafe void RenderPointer(string label, void* pointer) {
            var address = new IntPtr(pointer).ToString("x");
            var displayString = $"{label} 0x{new IntPtr(pointer).ToString("x")}";
            var size = new ImVec2();
            size.X = 100;
            size.Y = 20;
            if (ImGui.Button(displayString, size)) {
                ClipboardService.SetTextAsync(address);
            }
        }

        private static unsafe void RenderSequence(string name, int playerIndex, int i, Sequence* sequence)
        {
            if (ImGui.TreeNodePtr(i, name))
            {
                RenderPointer("", sequence);
                var frameI = 0;
                var damage = 0;

                foreach (var frame in sequence->frames())
                {
                    if (ImGui.TreeNodePtr(frameI, $"Frame {frameI}"))
                    {
                        int durationInt = frame->duration;
                        ImGui.InputInt($"Duration##{playerIndex}-{name}-{frameI}", ref durationInt, 0, 1, 0);
                        frame->duration = (ushort)durationInt;

                        int damageInt = frame->attack.damage * 3;
                        ImGui.InputInt($"Damage##{playerIndex}-{name}-{frameI}", ref damageInt, 0, 1, 0);
                        frame->attack.damage = (byte)(damageInt / 3);

                        RenderHitbox("Attackbox", name, playerIndex, frameI, 1, ref frame->attackbox);
                        RenderHitbox("Hurtbox 1", name, playerIndex, frameI, 2, ref frame->hitbox1);
                        RenderHitbox("Hurtbox 2", name, playerIndex, frameI, 3, ref frame->hitbox2);
                        RenderHitbox("Hurtbox 3", name, playerIndex, frameI, 4, ref frame->hitbox3);

                        if (damageInt > 0)  {
                            damage += damageInt;
                        }

                        ImGui.TreePop();
                    }

                    frameI += 1;
                }

                ImGui.Text($"Damage: {damage} {damage * 3}");

                ImGui.TreePop();
            }
        }

        private static unsafe void RenderHitbox(string name, string moveName, int playerIndex, int frameI, int i, ref Hitbox hitbox)
        {

            if (ImGui.TreeNodePtr(i, name)) {
                int attackbox1x = hitbox.x;
                int attackbox1y = hitbox.y;
                int attackbox1w = hitbox.w;
                int attackbox1h = hitbox.h;
                ImGui.InputInt($"X##{playerIndex}-{name}-{moveName}-{frameI}-{i}x", ref attackbox1x, 0, 1, 0);
                ImGui.InputInt($"Y##{playerIndex}-{name}-{moveName}-{frameI}-{i}y", ref attackbox1y, 0, 1, 0);
                ImGui.InputInt($"Width##{playerIndex}-{name}-{moveName}-{frameI}-{i}w", ref attackbox1w, 0, 1, 0);
                ImGui.InputInt($"Height##{playerIndex}-{name}-{moveName}-{frameI}-{i}h", ref attackbox1h, 0, 1, 0);
                hitbox.x = (short)attackbox1x;
                hitbox.y = (short)attackbox1y;
                hitbox.w = (short)attackbox1w;
                hitbox.h = (short)attackbox1h;

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
                    RenderPointer("Match:", _context.match);

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
