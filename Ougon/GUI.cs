using Reloaded.Hooks.Definitions;
using igNET = ImGuiNET;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;
using DearImguiSharp;
using TextCopy;
using System.Numerics;

namespace Ougon.GUI
{
    class Debug
    {
        private unsafe GameState* _gameState;
        public Context _context;
        private readonly IReloadedHooks? _hooks;
        Vector2 initialPosition = new Vector2(0, 0);
        Vector2 hitboxPositionMax = new Vector2(0, 0);
        bool showHitboxes = false;

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

        private unsafe void RenderPlayerCenter(GameCharacter* character) {
            var minimum = new Vector2(character->absolutePositionX, character->absolutePositionY);
            var maximum = new Vector2(character->absolutePositionX, 216 + character->inMatch->positionY);

            var dl = igNET.ImGui.GetForegroundDrawList();
            dl.AddRect(minimum, maximum, igNET.ImGui.GetColorU32(new Vector4(255, 255, 255, 255)));
        }

        private unsafe void RenderHitbox(GameCharacter* character, Hitbox hitbox, Vector4? color) {

            if (color == null) {
                color = new Vector4(0, 255, 0, 0.2f);
            }

            var currentFrameTex = new Vector2(character->currentFrame->tex_x, character->currentFrame->tex_y);

            var characterOrigin = new Vector2(character->absolutePositionX, character->absolutePositionY);

            hitbox.x = (short)(hitbox.x * _context.match->cameraZoomFactor);
            hitbox.y = (short)(hitbox.y * _context.match->cameraZoomFactor);
            hitbox.w = (short)(hitbox.w * _context.match->cameraZoomFactor);
            hitbox.h = (short)(hitbox.h * _context.match->cameraZoomFactor);

            if (character->isLeftSide) {
                hitbox.x = (short)(hitbox.x * -1);
                hitbox.w = (short)(hitbox.w * -1);
                currentFrameTex.X = currentFrameTex.X * -1;
            }

            var one = characterOrigin + new Vector2(hitbox.x + currentFrameTex.X, hitbox.y + currentFrameTex.Y);
            var two = new Vector2(one.X - hitbox.w, one.Y - hitbox.h);

            one.X += hitbox.w / 2;
            two.X += hitbox.w / 2;
            one.Y += hitbox.h / 2;
            two.Y += hitbox.h / 2;

            if (one.X > two.X) {
                var t = one.X;
                one.X = two.X;
                two.X = t;
            }

            if (one.Y > two.Y) {
                var t = one.Y;
                one.Y = two.Y;
                two.Y = t;
            }

            if (hitbox.w != 0 && hitbox.h != 0) {
                var dl = igNET.ImGui.GetForegroundDrawList();
                dl.AddRectFilled(one, two, igNET.ImGui.GetColorU32((Vector4)color));
            }
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
                    var io = ImGui.GetIO();
                    var viewport = ImGui.GetMainViewport();

                    int i = 0;
                    foreach (GameCharacter* character in characters) {
                        var _character = player.characters[i];
                        var id = _character.id;
                        var name = _character.name;
                        var color = _character.color;

                        _character.AddToSequenceHistory((nint)character->currentSequence);
                        ImGui.Text($"CurrentFrameID: {character->currentFrame->sprite_id}");

                        var currentFrame = character->currentFrame;

                        RenderPlayerCenter(character);

                        ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                        if (ImGui.TreeNodeStr($"{name} ({color})")) {
                            RenderPointer("", character);
                            ImGui.InputInt($"Health", ref character->inMatch->health, 0, 1, 0);
                            ImGui.InputFloat($"Meter", ref character->inMatch->meter, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Stun", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.InputFloat($"Position X", ref character->inMatch->positionX, 0.0f, 0.0f, "%.3f", 0);
                            ImGui.InputFloat($"Position Y", ref character->inMatch->positionY, 0.0f, 0.0f, "%.3f", 0);

                            ImGui.Text($"Absolute Position X: {character->absolutePositionX}");
                            ImGui.Text($"Absolute Position Y: {character->absolutePositionY}");
                            ImGui.Text($"Is Left Side: {character->isLeftSide}");

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
                    if (frame->duration < 1) {
                        continue;
                    }

                    if (ImGui.TreeNodePtr(frameI, $"Frame {frameI}"))
                    {
                        int durationInt = frame->duration;
                        ImGui.InputInt($"Duration", ref durationInt, 0, 1, 0);
                        frame->duration = (ushort)durationInt;

                        int damageInt = frame->attack.damage * 3;
                        ImGui.InputInt($"Damage", ref damageInt, 0, 1, 0);
                        frame->attack.damage = (byte)(damageInt / 3);

                        ImGui.Text($"Tex X: {frame->tex_x}");
                        ImGui.Text($"Tex Y: {frame->tex_y}");
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
            bool isDefaultOpen2 = true;
            ImGui.Begin("Debug", ref isDefaultOpen, 0);

            var defaultWindowSize = new ImVec2();
            defaultWindowSize.X = 300;
            defaultWindowSize.Y = 400;
            ImGui.SetWindowSizeVec2(defaultWindowSize, (int)ImGuiCond.FirstUseEver);
            ImGui.ShowDemoWindow(ref isDefaultOpen2);

            var dl = igNET.ImGui.GetForegroundDrawList();
            dl.AddRect(initialPosition, hitboxPositionMax, igNET.ImGui.GetColorU32(new Vector4(255, 255, 255, 255)));

            ImGui.DragFloat("MinimumX", ref initialPosition.X, 0, 0, 10800, "%.3f", 0);
            ImGui.DragFloat("MinimumY", ref initialPosition.Y, 0, 0, 10800, "%.3f", 0);

            ImGui.DragFloat("MaximumX", ref hitboxPositionMax.X, 0, 0, 10800, "%.3f", 0);
            ImGui.DragFloat("MaximumY", ref hitboxPositionMax.Y, 0, 0, 10800, "%.3f", 0);

            if (ImGui.CollapsingHeaderBoolPtr("General", ref isDefaultOpen, 0)) {
                ImGui.Text($"FPS: {_gameState->fps.ToString("0.##")}");
            }

            if (_context.match != null && _context.match->isValid())
            {
                ImGui.Checkbox("Show hitboxes", ref this.showHitboxes);

                if (this.showHitboxes) {
                    var p1Characters = _context.match->player1Characters();
                    var p2Characters = _context.match->player2Characters();

                    foreach (GameCharacter* character in p1Characters) {
                        RenderHitbox(character, character->currentFrame->attackbox, new Vector4(255, 0, 0, 0.2f));
                        RenderHitbox(character, character->currentFrame->hitbox1, null);
                        RenderHitbox(character, character->currentFrame->hitbox2, null);
                        RenderHitbox(character, character->currentFrame->hitbox3, null);
                    }

                    foreach (GameCharacter* character in p2Characters) {
                        RenderHitbox(character, character->currentFrame->attackbox, new Vector4(255, 0, 0, 0.2f));
                        RenderHitbox(character, character->currentFrame->hitbox1, null);
                        RenderHitbox(character, character->currentFrame->hitbox2, null);
                        RenderHitbox(character, character->currentFrame->hitbox3, null);
                    }
                }

                if (ImGui.CollapsingHeaderBoolPtr("Current Match", ref isDefaultOpen, 0))
                {
                    RenderPointer("Match:", _context.match);
                    ImGui.Text($"Camera Position: {_context.match->cameraPositionX}");
                    ImGui.Text($"Camera Zoom: {_context.match->cameraZoomFactor}");

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
