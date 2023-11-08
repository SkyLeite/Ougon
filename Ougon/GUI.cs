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

                if (player == null) {
                    return;
                }

                ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                if (ImGui.TreeNodeStr($"Player {playerIndex}"))
                {
                    int i = 0;
                    foreach (GameCharacter* character in characters) {
                        var _character = player.characters[i];
                        var id = _character.id;
                        var name = _character.name;
                        var color = _character.color;

                        _character.AddToSequenceHistory((nint)character->currentSequence);

                        ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                        if (ImGui.TreeNodeStr($"{name} ({color})")) {
                            RenderPointer("", character);
                            ImGui.InputInt($"Health##{playerIndex}-{i}", ref character->inMatch->health, 0, 1, 0);
                            ImGui.InputFloat($"Meter##{playerIndex}-{i}", ref character->inMatch->meter, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Stun##{playerIndex}-{i}", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.InputFloat($"Position X##{playerIndex}-{i}", ref character->inMatch->positionX, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Position Y##{playerIndex}-{i}", ref character->inMatch->positionY, 0.0f, 0.0f, "%.3f", 0);

                            if (ImGui.TreeNodeStr("Moves"))
                            {
                                RenderSequence("Idle", playerIndex, 1, character->_5);

                                RenderSequence("5->2", playerIndex, 3, character->_5to2);

                                RenderSequence("Crouching", playerIndex, 4, character->_2);

                                RenderSequence("3->1", playerIndex, 5, character->_3to1);
                                RenderSequence("2->5", playerIndex, 6, character->_2to5);

                                RenderSequence("Walk forward", playerIndex, 7, character->_6);
                                RenderSequence("Walk backward", playerIndex, 8, character->_4);

                                RenderSequence("Neutral jump", playerIndex, 9, character->_8);
                                RenderSequence("Forward jump", playerIndex, 10, character->_9);
                                RenderSequence("Backward jump", playerIndex, 11, character->_7);

                                RenderSequence("Dash recovery", playerIndex, 12, character->_66to5);
                                RenderSequence("Backdash", playerIndex, 13, character->_44);

                                RenderSequence("Grab", playerIndex, 14, character->grab);
                                RenderSequence("Grab Whiff", playerIndex, 15, character->grabWhiff);

                                RenderSequence("Call tag", playerIndex, 16, character->callTag);
                                RenderSequence("Tag out", playerIndex, 17, character->tagOut);

                                if (ImGui.TreeNodeStr("A")) {
                                    RenderSequence("Far 5A", playerIndex, 1, character->far5A);
                                    RenderSequence("Close 5A", playerIndex, 2, character->close5A);
                                    RenderSequence("2A", playerIndex, 3, character->_2A);
                                    RenderSequence("8A", playerIndex, 4, character->j8A);
                                    RenderSequence("9A", playerIndex, 5, character->j9A);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("B")) {
                                    RenderSequence("Far 5B", playerIndex, 1, character->far5B);
                                    RenderSequence("Close 5B", playerIndex, 2, character->close5B);
                                    RenderSequence("6B", playerIndex, 3, character->_6B);
                                    RenderSequence("2B", playerIndex, 4, character->_2B);
                                    RenderSequence("8B", playerIndex, 5, character->j8B);
                                    RenderSequence("9B", playerIndex, 6, character->j9B);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("C")) {
                                    RenderSequence("Far 5C", playerIndex, 1, character->far5C);
                                    RenderSequence("Close 5C", playerIndex, 2, character->close5C);
                                    RenderSequence("6C", playerIndex, 3, character->_6C);
                                    RenderSequence("2C", playerIndex, 4, character->_2C);
                                    RenderSequence("8C", playerIndex, 5, character->j8C);
                                    RenderSequence("9C", playerIndex, 6, character->j9C);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("Special Moves")) {
                                    if (ImGui.TreeNodeStr("Special 1")) {
                                        RenderSequence("A", playerIndex, 1, character->specialMove1A);
                                        RenderSequence("B", playerIndex, 2, character->specialMove1B);
                                        RenderSequence("C", playerIndex, 3, character->specialMove1C);
                                        RenderSequence("EX", playerIndex, 4, character->specialMove1X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 2")) {
                                        RenderSequence("A", playerIndex, 1, character->specialMove2A);
                                        RenderSequence("B", playerIndex, 2, character->specialMove2B);
                                        RenderSequence("C", playerIndex, 3, character->specialMove2C);
                                        RenderSequence("EX", playerIndex, 4, character->specialMove2X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 3")) {
                                        RenderSequence("A", playerIndex, 1, character->specialMove3A);
                                        RenderSequence("B", playerIndex, 2, character->specialMove3B);
                                        RenderSequence("C", playerIndex, 3, character->specialMove3C);
                                        RenderSequence("EX", playerIndex, 4, character->specialMove3X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 4")) {
                                        RenderSequence("A", playerIndex, 1, character->specialMove4A);
                                        RenderSequence("B", playerIndex, 2, character->specialMove4B);
                                        RenderSequence("C", playerIndex, 3, character->specialMove4C);
                                        RenderSequence("EX", playerIndex, 4, character->specialMove4X);
                                        ImGui.TreePop();
                                    }

                                    ImGui.TreePop();
                                }

                                RenderSequence("Level 2 Super", playerIndex, 18, character->super2);
                                RenderSequence("Meta Declare", playerIndex, 19, character->metaDeclare);
                                RenderSequence("Meta Super", playerIndex, 20, character->metaSuper);

                                ImGui.TreePop();
                            }

                            if (ImGui.TreeNodeStr("Animation history")) {
                                var buttonSize = new ImVec2();
                                buttonSize.X = 100;
                                buttonSize.Y = 20;
                                if (ImGui.Button("Clear", buttonSize)) {
                                    _character.ClearSequenceHistory();
                                }

                                ImVec2 available = new ImVec2();
                                ImGui.GetContentRegionAvail(available);
                                available.Y = 100;

                                ImGui.BeginChildStr($"AnimationHistory#{name}{color}{i}", available, true, (int)ImGuiWindowFlags.HorizontalScrollbar);
                                foreach (var sequence in _character.sequenceHistory) {
                                    ImGui.Text(sequence);
                                }
                                ImGui.EndChild();

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
            if (ImGui.TreeNodeStr(name))
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
