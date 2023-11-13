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
                            ImGui.InputInt($"Health", ref character->inMatch->health, 0, 1, 0);
                            ImGui.InputFloat($"Meter", ref character->inMatch->meter, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Stun", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.InputFloat($"Position X", ref character->inMatch->positionX, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Position Y", ref character->inMatch->positionY, 0.0f, 0.0f, "%.3f", 0);

                            if (ImGui.TreeNodeStr("Moves"))
                            {
                                RenderSequence("Idle", character->_5);

                                RenderSequence("5->2", character->_5to2);

                                RenderSequence("Crouching", character->_2);

                                RenderSequence("3->1", character->_3to1);
                                RenderSequence("2->5", character->_2to5);

                                RenderSequence("Walk forward", character->_6);
                                RenderSequence("Walk backward", character->_4);

                                RenderSequence("Neutral jump", character->_8);
                                RenderSequence("Forward jump", character->_9);
                                RenderSequence("Backward jump", character->_7);

                                RenderSequence("Dash recovery", character->_66to5);
                                RenderSequence("Backdash", character->_44);

                                RenderSequence("Grab", character->grab);
                                RenderSequence("Grab Whiff", character->grabWhiff);

                                RenderSequence("Call tag", character->callTag);
                                RenderSequence("Tag out", character->tagOut);

                                if (ImGui.TreeNodeStr("A")) {
                                    RenderSequence("Far 5A", character->far5A);
                                    RenderSequence("Close 5A", character->close5A);
                                    RenderSequence("2A", character->_2A);
                                    RenderSequence("8A", character->j8A);
                                    RenderSequence("9A", character->j9A);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("B")) {
                                    RenderSequence("Far 5B", character->far5B);
                                    RenderSequence("Close 5B", character->close5B);
                                    RenderSequence("6B", character->_6B);
                                    RenderSequence("2B", character->_2B);
                                    RenderSequence("8B", character->j8B);
                                    RenderSequence("9B", character->j9B);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("C")) {
                                    RenderSequence("Far 5C", character->far5C);
                                    RenderSequence("Close 5C", character->close5C);
                                    RenderSequence("6C", character->_6C);
                                    RenderSequence("2C", character->_2C);
                                    RenderSequence("8C", character->j8C);
                                    RenderSequence("9C", character->j9C);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("Special Moves")) {
                                    if (ImGui.TreeNodeStr("Special 1")) {
                                        RenderSequence("A", character->specialMove1A);
                                        RenderSequence("B", character->specialMove1B);
                                        RenderSequence("C", character->specialMove1C);
                                        RenderSequence("EX", character->specialMove1X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 2")) {
                                        RenderSequence("A", character->specialMove2A);
                                        RenderSequence("B", character->specialMove2B);
                                        RenderSequence("C", character->specialMove2C);
                                        RenderSequence("EX", character->specialMove2X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 3")) {
                                        RenderSequence("A", character->specialMove3A);
                                        RenderSequence("B", character->specialMove3B);
                                        RenderSequence("C", character->specialMove3C);
                                        RenderSequence("EX", character->specialMove3X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 4")) {
                                        RenderSequence("A", character->specialMove4A);
                                        RenderSequence("B", character->specialMove4B);
                                        RenderSequence("C", character->specialMove4C);
                                        RenderSequence("EX", character->specialMove4X);
                                        ImGui.TreePop();
                                    }

                                    ImGui.TreePop();
                                }

                                RenderSequence("Level 2 Super", character->super2);
                                RenderSequence("Meta Declare", character->metaDeclare);
                                RenderSequence("Meta Super", character->metaSuper);

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

        private static unsafe void RenderSequence(string name, Sequence* sequence)
        {
            if (ImGui.TreeNodeStr(name))
            {
                RenderPointer("", sequence);
                var frameI = 0;

                foreach (var frame in sequence->frames())
                {
                    if (ImGui.TreeNodePtr(frameI, $"Frame {frameI}"))
                    {
                        int durationInt = frame->duration;
                        ImGui.InputInt($"Duration", ref durationInt, 0, 1, 0);
                        frame->duration = (ushort)durationInt;

                        int damageInt = frame->attack.damage * 3;
                        ImGui.InputInt($"Damage", ref damageInt, 0, 1, 0);
                        frame->attack.damage = (byte)(damageInt / 3);

                        RenderHitbox("Attackbox", ref frame->attackbox);
                        RenderHitbox("Hurtbox 1", ref frame->hitbox1);
                        RenderHitbox("Hurtbox 2", ref frame->hitbox2);
                        RenderHitbox("Hurtbox 3", ref frame->hitbox3);

                        ImGui.TreePop();
                    }

                    frameI += 1;
                }

                ImGui.TreePop();
            }
        }

        private static unsafe void RenderHitbox(string name, ref Hitbox hitbox)
        {

            if (ImGui.TreeNodeStr(name)) {
                int attackbox1x = hitbox.x;
                int attackbox1y = hitbox.y;
                int attackbox1w = hitbox.w;
                int attackbox1h = hitbox.h;
                ImGui.InputInt($"X", ref attackbox1x, 0, 1, 0);
                ImGui.InputInt($"Y", ref attackbox1y, 0, 1, 0);
                ImGui.InputInt($"Width", ref attackbox1w, 0, 1, 0);
                ImGui.InputInt($"Height", ref attackbox1h, 0, 1, 0);
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
