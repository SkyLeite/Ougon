using System.Numerics;
using DearImguiSharp;
using Ougon.Data;
using Reloaded.Hooks.Definitions;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;
using TextCopy;
using igNET = ImGuiNET;

namespace Ougon.GUI;

sealed class Debug
{
    public bool isActive;
    private Hooks.HookService hooks;
    private readonly IReloadedHooks reloadedHooks;
    private readonly InputConfig inputConfig;
    public Context context;
    bool showHitboxes;

    public unsafe Debug(Hooks.HookService hooks, IReloadedHooks reloadedHooks, Context context, InputConfig inputConfig)
    {
        this.hooks = hooks;
        this.context = context;
        this.reloadedHooks = reloadedHooks;
        this.inputConfig = inputConfig;

        Task.Run(() => InitializeImgui());
    }

    public async Task InitializeImgui()
    {
        this.isActive = true;
        SDK.Init(this.reloadedHooks);

        await ImguiHook
            .Create(
                RenderDebugWindow,
                new ImguiHookOptions()
                {
                    Implementations = new List<IImguiHook>()
                    {
                        new ImguiHookDx9(), // `Reloaded.Imgui.Hook.Direct3D9`
                    }
                }
            )
            .ConfigureAwait(false);
    }

    private unsafe void RenderHitbox(GameCharacter* character, Hitbox hitbox, Vector4? color)
    {
        if (color == null)
        {
            color = new Vector4(0, 255, 0, 0.2f);
        }

        var currentFrameTex = new Vector2(
            character->currentFrame->tex_x,
            character->currentFrame->tex_y
        );

        var characterOrigin = new Vector2(
            character->absolutePositionX,
            character->absolutePositionY
        );

        hitbox.x = (short)(hitbox.x * context.match->cameraZoomFactor);
        hitbox.y = (short)(hitbox.y * context.match->cameraZoomFactor);
        hitbox.w = (short)(hitbox.w * context.match->cameraZoomFactor);
        hitbox.h = (short)(hitbox.h * context.match->cameraZoomFactor);

        if (character->isLeftSide)
        {
            hitbox.x = (short)(hitbox.x * -1);
            hitbox.w = (short)(hitbox.w * -1);
            currentFrameTex.X = currentFrameTex.X * -1;
        }

        var one =
            characterOrigin
            + new Vector2(hitbox.x + currentFrameTex.X, hitbox.y + currentFrameTex.Y);
        var two = new Vector2(one.X - hitbox.w, one.Y - hitbox.h);

        one.X += hitbox.w / 2;
        two.X += hitbox.w / 2;
        one.Y += hitbox.h / 2;
        two.Y += hitbox.h / 2;

        if (one.X > two.X)
        {
            var t = one.X;
            one.X = two.X;
            two.X = t;
        }

        if (one.Y > two.Y)
        {
            var t = one.Y;
            one.Y = two.Y;
            two.Y = t;
        }

        if (hitbox.w != 0 && hitbox.h != 0)
        {
            var dl = igNET.ImGui.GetForegroundDrawList();
            dl.AddRectFilled(one, two, igNET.ImGui.GetColorU32((Vector4)color));
        }
    }

    private unsafe void RenderPlayer(int playerIndex, GameCharacter*[] characters)
    {
        var player = playerIndex switch
        {
            1 => context.fight?.player1,
            2 => context.fight?.player2,
            _ => null
        };

        if (player == null)
        {
            return;
        }

        ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
        if (ImGui.TreeNodeStr($"Player {playerIndex}"))
        {
            var io = ImGui.GetIO();
            var viewport = ImGui.GetMainViewport();

            int i = 0;
            foreach (GameCharacter* character in characters)
            {
                var _character = player.characters[i];
                var id = _character.id;
                var name = _character.name;
                var color = _character.color;

                _character.AddToSequenceHistory((nint)character->currentSequence);
                ImGui.Text($"CurrentFrameID: {character->currentFrame->sprite_id}");

                var currentFrame = character->currentFrame;

                ImGui.SetNextItemOpen(true, (int)ImGuiCond.Once);
                if (ImGui.TreeNodeStr($"{name} ({color})"))
                {
                    RenderPointer("", character);
                    ImGui.InputInt($"Health", ref character->inMatch->health, 0, 1, 0);
                    ImGui.InputFloat(
                        $"Meter",
                        ref character->inMatch->meter,
                        0.0f,
                        0.0f,
                        "%.3f",
                        0
                    );
                    ImGui.InputFloat($"Stun", ref character->inMatch->stun, 0.0f, 0.0f, "%.3f", 0);

                    ImGui.InputFloat(
                        $"Position X",
                        ref character->inMatch->positionX,
                        0.0f,
                        0.0f,
                        "%.3f",
                        0
                    );
                    ImGui.InputFloat(
                        $"Position Y",
                        ref character->inMatch->positionY,
                        0.0f,
                        0.0f,
                        "%.3f",
                        0
                    );

                    ImGui.Text($"Absolute Position X: {character->absolutePositionX}");
                    ImGui.Text($"Absolute Position Y: {character->absolutePositionY}");
                    ImGui.Text($"Is Left Side: {character->isLeftSide}");

                    if (ImGui.TreeNodeStr("Moves"))
                    {
                        RenderSequence("Idle", character, character->_5, "_5");

                        RenderSequence("5->2", character, character->_5to2, "_5to2");

                        RenderSequence("Crouching", character, character->_2, "_2");

                        RenderSequence("3->1", character, character->_3to1, "_3to1");
                        RenderSequence("2->5", character, character->_2to5, "_2to5");

                        RenderSequence("Walk forward", character, character->_6, "_6");
                        RenderSequence("Walk backward", character, character->_4, "_4");

                        RenderSequence("Neutral jump", character, character->_8, "_8");
                        RenderSequence("Forward jump", character, character->_9, "_9");
                        RenderSequence("Backward jump", character, character->_7, "_7");

                        RenderSequence("Dash recovery", character, character->_66to5, "_66to5");
                        RenderSequence("Backdash", character, character->_44, "_44");

                        RenderSequence("Grab", character, character->grab, "grab");
                        RenderSequence("Grab Whiff", character, character->grabWhiff, "grabWhiff");

                        RenderSequence("Call tag", character, character->callTag, "callTag");
                        RenderSequence("Tag out", character, character->tagOut, "tagOut");

                        if (ImGui.TreeNodeStr("A"))
                        {
                            RenderSequence("Far 5A", character, character->far5A, "far5A");
                            RenderSequence("Close 5A", character, character->close5A, "close5A");
                            RenderSequence("2A", character, character->_2A, "_2A");
                            RenderSequence("8A", character, character->j8A, "j8A");
                            RenderSequence("9A", character, character->j9A, "j9A");

                            ImGui.TreePop();
                        }

                        if (ImGui.TreeNodeStr("B"))
                        {
                            RenderSequence("Far 5B", character, character->far5B, "far5B");
                            RenderSequence("Close 5B", character, character->close5B, "close5B");
                            RenderSequence("6B", character, character->_6B, "_6B");
                            RenderSequence("2B", character, character->_2B, "_2B");
                            RenderSequence("8B", character, character->j8B, "j8B");
                            RenderSequence("9B", character, character->j9B, "j9B");

                            ImGui.TreePop();
                        }

                        if (ImGui.TreeNodeStr("C"))
                        {
                            RenderSequence("Far 5C", character, character->far5C, "far5C");
                            RenderSequence("Close 5C", character, character->close5C, "close5C");
                            RenderSequence("6C", character, character->_6C, "_6C");
                            RenderSequence("2C", character, character->_2C, "_2C");
                            RenderSequence("8C", character, character->j8C, "j8C");
                            RenderSequence("9C", character, character->j9C, "j9C");

                            ImGui.TreePop();
                        }

                        if (ImGui.TreeNodeStr("Special Moves"))
                        {
                            if (ImGui.TreeNodeStr("Special 1"))
                            {
                                RenderSequence(
                                    "A",
                                    character,
                                    character->specialMove1A,
                                    "specialMove1A"
                                );
                                RenderSequence(
                                    "B",
                                    character,
                                    character->specialMove1B,
                                    "specialMove1B"
                                );
                                RenderSequence(
                                    "C",
                                    character,
                                    character->specialMove1C,
                                    "specialMove1C"
                                );
                                RenderSequence(
                                    "EX",
                                    character,
                                    character->specialMove1X,
                                    "specialMove1X"
                                );
                                ImGui.TreePop();
                            }

                            if (ImGui.TreeNodeStr("Special 2"))
                            {
                                RenderSequence(
                                    "A",
                                    character,
                                    character->specialMove2A,
                                    "specialMove2A"
                                );
                                RenderSequence(
                                    "B",
                                    character,
                                    character->specialMove2B,
                                    "specialMove2B"
                                );
                                RenderSequence(
                                    "C",
                                    character,
                                    character->specialMove2C,
                                    "specialMove2C"
                                );
                                RenderSequence(
                                    "EX",
                                    character,
                                    character->specialMove2X,
                                    "specialMove2X"
                                );
                                ImGui.TreePop();
                            }

                            if (ImGui.TreeNodeStr("Special 3"))
                            {
                                RenderSequence(
                                    "A",
                                    character,
                                    character->specialMove3A,
                                    "specialMove3A"
                                );
                                RenderSequence(
                                    "B",
                                    character,
                                    character->specialMove3B,
                                    "specialMove3B"
                                );
                                RenderSequence(
                                    "C",
                                    character,
                                    character->specialMove3C,
                                    "specialMove3C"
                                );
                                RenderSequence(
                                    "EX",
                                    character,
                                    character->specialMove3X,
                                    "specialMove3X"
                                );
                                ImGui.TreePop();
                            }

                            if (ImGui.TreeNodeStr("Special 4"))
                            {
                                RenderSequence(
                                    "A",
                                    character,
                                    character->specialMove4A,
                                    "specialMove4A"
                                );
                                RenderSequence(
                                    "B",
                                    character,
                                    character->specialMove4B,
                                    "specialMove4B"
                                );
                                RenderSequence(
                                    "C",
                                    character,
                                    character->specialMove4C,
                                    "specialMove4C"
                                );
                                RenderSequence(
                                    "EX",
                                    character,
                                    character->specialMove4X,
                                    "specialMove4X"
                                );
                                ImGui.TreePop();
                            }

                            ImGui.TreePop();
                        }

                        RenderSequence("Level 2 Super", character, character->super2, "super2");
                        RenderSequence(
                            "Meta Declare",
                            character,
                            character->metaDeclare,
                            "metaDeclare"
                        );
                        RenderSequence("Meta Super", character, character->metaSuper, "metaSuper");

                        ImGui.TreePop();
                    }

                    if (ImGui.TreeNodeStr("Animation history"))
                    {
                        using (var buttonSize = new ImVec2())
                        {
                            buttonSize.X = 100;
                            buttonSize.Y = 20;
                            if (ImGui.Button("Clear", buttonSize))
                            {
                                _character.ClearSequenceHistory();
                            }
                        }

                        using (ImVec2 available = new ImVec2())
                        {
                            ImGui.GetContentRegionAvail(available);
                            available.Y = 100;

                            ImGui.BeginChildStr(
                                $"AnimationHistory#{name}{color}{i}",
                                available,
                                true,
                                (int)ImGuiWindowFlags.HorizontalScrollbar
                            );
                        }

                        foreach (var sequence in _character.sequenceHistory)
                        {
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

    private static unsafe void RenderPointer(string label, void* pointer)
    {
        var address = new IntPtr(pointer).ToString("x");
        var displayString = $"{label} 0x{new IntPtr(pointer).ToString("x")}";
        using (var size = new ImVec2())
        {
            size.X = 100;
            size.Y = 20;
            if (ImGui.Button(displayString, size))
            {
                ClipboardService.SetTextAsync(address);
            }
        }
        ;
    }

    private unsafe void RenderSequence(
        string name,
        GameCharacter* character,
        Sequence* sequence,
        string sequenceStr
    )
    {
        if (ImGui.TreeNodeStr(name))
        {
            RenderPointer("", sequence);
            var frameI = 0;

            using (var buttonSize = new ImVec2())
            {
                buttonSize.X = 100;
                buttonSize.Y = 20;
                if (ImGui.Button("Run", buttonSize))
                {
                    var sequenceIndex = GameCharacter.GetSequenceIndex(character, sequenceStr);
                    this.hooks.PlaySequence.Hook(character, sequenceIndex, 13, 0);
                }
            }
            ;

            foreach (var frame in sequence->frames())
            {
                if (frame->duration < 1)
                {
                    continue;
                }

                if (ImGui.TreeNodePtr(frameI, $"Frame {frameI}"))
                {
                    RenderPointer("", frame);
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
        if (ImGui.TreeNodeStr(name))
        {
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
        this.inputConfig.Render();

        bool isDefaultOpen = true;
        ImGui.Begin("Debug", ref isDefaultOpen, 0);

        using (var defaultWindowSize = new ImVec2())
        {
            defaultWindowSize.X = 300;
            defaultWindowSize.Y = 400;
            ImGui.SetWindowSizeVec2(defaultWindowSize, (int)ImGuiCond.FirstUseEver);
        }
        ;

        if (ImGui.CollapsingHeaderBoolPtr("General", ref isDefaultOpen, 0))
        {
            ImGui.Text($"FPS: {this.context.gameState->fps.ToString("0.##")}");
        }

        if (context.match != null && context.match->isValid())
        {
            ImGui.Checkbox("Show hitboxes", ref this.showHitboxes);

            if (this.showHitboxes)
            {
                var p1Characters = context.match->player1Characters();
                var p2Characters = context.match->player2Characters();

                foreach (GameCharacter* character in p1Characters)
                {
                    RenderHitbox(
                        character,
                        character->currentFrame->attackbox,
                        new Vector4(255, 0, 0, 0.2f)
                    );
                    RenderHitbox(character, character->currentFrame->hitbox1, null);
                    RenderHitbox(character, character->currentFrame->hitbox2, null);
                    RenderHitbox(character, character->currentFrame->hitbox3, null);
                }

                foreach (GameCharacter* character in p2Characters)
                {
                    RenderHitbox(
                        character,
                        character->currentFrame->attackbox,
                        new Vector4(255, 0, 0, 0.2f)
                    );
                    RenderHitbox(character, character->currentFrame->hitbox1, null);
                    RenderHitbox(character, character->currentFrame->hitbox2, null);
                    RenderHitbox(character, character->currentFrame->hitbox3, null);
                }
            }

            if (ImGui.CollapsingHeaderBoolPtr("Current Match", ref isDefaultOpen, 0))
            {
                RenderPointer("Match:", context.match);
                ImGui.Text($"Camera Position: {context.match->cameraPositionX}");
                ImGui.Text($"Camera Zoom: {context.match->cameraZoomFactor}");

                {
                    ImGui.BeginGroup();
                    ImGui.SliderInt("Timer", ref context.match->timer, 0, 10800, "", 0);
                    ImGui.SameLine(0, 0);
                    ImGui.Checkbox("Lock", ref context.timerLocked);
                    ImGui.EndGroup();
                }

                ImGui.Separator();

                RenderPlayer(1, context.match->player1Characters());
                ImGui.Separator();
                RenderPlayer(2, context.match->player2Characters());
            }
        }

        ImGui.End();
    }
}
