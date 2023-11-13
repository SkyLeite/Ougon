using Reloaded.Hooks.Definitions;
using igNET = ImGuiNET;
using Reloaded.Imgui.Hook;
using Reloaded.Imgui.Hook.Implementations;
using DearImguiSharp;
using TextCopy;
using Ougon.Template;
using System.Runtime.InteropServices;
using System.Numerics;
using SharpDX.Direct3D9;
using Reloaded.Memory.Pointers;

namespace Ougon.GUI
{
    class Debug
    {
        private unsafe GameState* _gameState;
        public Context _context;
        private ModContext _modContext;
        private nint fontTexId;
        private readonly IReloadedHooks? _hooks;
        private readonly Dictionary<string, Texture> _loaded = new Dictionary<string, Texture> ();

        public unsafe Debug(IReloadedHooks hooks, GameState* gameState, Context context, ModContext modContext)
        {
            _gameState = gameState;
            _hooks = hooks;
            _context = context;
            _modContext = modContext;

            this.InitializeImgui();
        }


        private async Task InitializeImgui()
        {
            SDK.Init(_hooks);

            await ImguiHook.Create(RenderDebugWindow, new ImguiHookOptions()
            {
                EnableViewports = false,
                Implementations = new List<IImguiHook>()
                {
                    new ImguiHookDx9(), // `Reloaded.Imgui.Hook.Direct3D9`
                }
            });
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
                                RenderSequence(character, "Idle", playerIndex, 1, character->_5);

                                RenderSequence(character, "5->2", playerIndex, 3, character->_5to2);

                                RenderSequence(character, "Crouching", playerIndex, 4, character->_2);

                                RenderSequence(character, "3->1", playerIndex, 5, character->_3to1);
                                RenderSequence(character, "2->5", playerIndex, 6, character->_2to5);

                                RenderSequence(character, "Walk forward", playerIndex, 7, character->_6);
                                RenderSequence(character, "Walk backward", playerIndex, 8, character->_4);

                                RenderSequence(character, "Neutral jump", playerIndex, 9, character->_8);
                                RenderSequence(character, "Forward jump", playerIndex, 10, character->_9);
                                RenderSequence(character, "Backward jump", playerIndex, 11, character->_7);

                                RenderSequence(character, "Dash recovery", playerIndex, 12, character->_66to5);
                                RenderSequence(character, "Backdash", playerIndex, 13, character->_44);

                                RenderSequence(character, "Grab", playerIndex, 14, character->grab);
                                RenderSequence(character, "Grab Whiff", playerIndex, 15, character->grabWhiff);

                                RenderSequence(character, "Call tag", playerIndex, 16, character->callTag);
                                RenderSequence(character, "Tag out", playerIndex, 17, character->tagOut);

                                if (ImGui.TreeNodeStr("A")) {
                                    RenderSequence(character, "Far 5A", playerIndex, 1, character->far5A);
                                    RenderSequence(character, "Close 5A", playerIndex, 2, character->close5A);
                                    RenderSequence(character, "2A", playerIndex, 3, character->_2A);
                                    RenderSequence(character, "8A", playerIndex, 4, character->j8A);
                                    RenderSequence(character, "9A", playerIndex, 5, character->j9A);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("B")) {
                                    RenderSequence(character, "Far 5B", playerIndex, 1, character->far5B);
                                    RenderSequence(character, "Close 5B", playerIndex, 2, character->close5B);
                                    RenderSequence(character, "6B", playerIndex, 3, character->_6B);
                                    RenderSequence(character, "2B", playerIndex, 4, character->_2B);
                                    RenderSequence(character, "8B", playerIndex, 5, character->j8B);
                                    RenderSequence(character, "9B", playerIndex, 6, character->j9B);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("C")) {
                                    RenderSequence(character, "Far 5C", playerIndex, 1, character->far5C);
                                    RenderSequence(character, "Close 5C", playerIndex, 2, character->close5C);
                                    RenderSequence(character, "6C", playerIndex, 3, character->_6C);
                                    RenderSequence(character, "2C", playerIndex, 4, character->_2C);
                                    RenderSequence(character, "8C", playerIndex, 5, character->j8C);
                                    RenderSequence(character, "9C", playerIndex, 6, character->j9C);

                                    ImGui.TreePop();
                                }

                                if (ImGui.TreeNodeStr("Special Moves")) {
                                    if (ImGui.TreeNodeStr("Special 1")) {
                                        RenderSequence(character, "A", playerIndex, 1, character->specialMove1A);
                                        RenderSequence(character, "B", playerIndex, 2, character->specialMove1B);
                                        RenderSequence(character, "C", playerIndex, 3, character->specialMove1C);
                                        RenderSequence(character, "EX", playerIndex, 4, character->specialMove1X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 2")) {
                                        RenderSequence(character, "A", playerIndex, 1, character->specialMove2A);
                                        RenderSequence(character, "B", playerIndex, 2, character->specialMove2B);
                                        RenderSequence(character, "C", playerIndex, 3, character->specialMove2C);
                                        RenderSequence(character, "EX", playerIndex, 4, character->specialMove2X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 3")) {
                                        RenderSequence(character, "A", playerIndex, 1, character->specialMove3A);
                                        RenderSequence(character, "B", playerIndex, 2, character->specialMove3B);
                                        RenderSequence(character, "C", playerIndex, 3, character->specialMove3C);
                                        RenderSequence(character, "EX", playerIndex, 4, character->specialMove3X);
                                        ImGui.TreePop();
                                    }

                                    if (ImGui.TreeNodeStr("Special 4")) {
                                        RenderSequence(character, "A", playerIndex, 1, character->specialMove4A);
                                        RenderSequence(character, "B", playerIndex, 2, character->specialMove4B);
                                        RenderSequence(character, "C", playerIndex, 3, character->specialMove4C);
                                        RenderSequence(character, "EX", playerIndex, 4, character->specialMove4X);
                                        ImGui.TreePop();
                                    }

                                    ImGui.TreePop();
                                }

                                RenderSequence(character, "Level 2 Super", playerIndex, 18, character->super2);
                                RenderSequence(character, "Meta Declare", playerIndex, 19, character->metaDeclare);
                                RenderSequence(character, "Meta Super", playerIndex, 20, character->metaSuper);

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

        private unsafe void RenderPointer(string label, void* pointer) {
            var address = new IntPtr(pointer).ToString("x");
            var displayString = $"{label} 0x{new IntPtr(pointer).ToString("x")}";
            var size = new ImVec2();
            size.X = 100;
            size.Y = 20;
            if (ImGui.Button(displayString, size)) {
                ClipboardService.SetTextAsync(address);
            }
        }

        // public unsafe static void Image(IntPtr userTextureId, ImVec2 size, Vector2 uv0, Vector2 uv1, Vector4 tintCol, Vector4 borderCol)
        // {
        //     ImGuiWindow window = ImGui.GetCurrentWindow();
        //     if (window.SkipItems)
        //         return;

        //     var bbMin = new ImVec2.__Internal();
        //     bbMin.x = window.DC.CursorPos.X;
        //     bbMin.y = window.DC.CursorPos.Y;

        //     var bbMax = new ImVec2.__Internal();
        //     bbMax.x = bbMin.X + size.X;
        //     bbMax.y = bbMin.Y + size.Y;

        //     var bb = new ImRect();
        //     bb.Min = bbMin;
        //     bb.Max = bbMax;

        //     if (borderCol.W > 0.0f)
        //         bb.Max.X += 2;
        //         bb.Max.Y += 2;

        //     ImGui.__Internal.ItemSizeRect(new IntPtr(&bb), -1.0f);
        //     if (!ImGui.ItemAdd(bb, 0, null, 0))
        //         return;

        //     var drawList = ImGui.GetWindowDrawList();

        //     if (borderCol.W > 0.0f)
        //     {
        //         drawList.AddRect(bbMin, bbMax, ImGui.GetColorU32(borderCol), 0.0f);
        //         drawList.AddImage(userTextureId, bbMin + new Vector2(1, 1), bbMax - new Vector2(1, 1), uv0, uv1, ImGui.GetColorU32(tintCol));
        //     }
        //     else
        //     {
        //         ImGui.__Internal.ImDrawListAddImage(
        //             new IntPtr(&drawList),
        //             userTextureId,
        //             bbMin,
        //             bbMax,
        //             uv0,
        //             uv1,
        //             igNET.ImGui.GetColorU32(tintCol)
        //         );
        //         drawList.AddImage(userTextureId, bbMin, bbMax, uv0, uv1, ImGui.GetColorU32(tintCol));
        //     }
        // }

        private unsafe void RenderSequence(GameCharacter* character, string name, int playerIndex, int i, Sequence* sequence)
        {
            if (ImGui.TreeNodeStr(name))
            {
                // RenderPointer("", sequence);
                var frameI = 0;
                var damage = 0;

                foreach (var frame in sequence->frames())
                {
                    if (frame->duration < 1 || frame->sprite_id > 999) {
                        continue;
                    }

                    var sprite = GameCharacter.GetSpriteFromID(character, frame->sprite_id);
                    if (sprite != null) {
                        // RenderPointer("spr:", sprite);
                        // RenderPointer("lzlr:", sprite->lzlr);
                        // ImGui.Text($"ddssize: {sprite->lzlr->DDSSize}");
                        // ImGui.Text($"ddsoffset: {sprite->lzlr->DDSOffset}");

                        // var ddsAddr = IntPtr.Add(new IntPtr(sprite->lzlr), sprite->lzlr->DDSOffset);
                        // var height = *(int *)IntPtr.Add(ddsAddr, 0xC);
                        // var width = *(int *)IntPtr.Add(ddsAddr, 0x10);
                        // ImGui.Text($"width: {width} | height: {height}");
                        // RenderPointer("dds:", (void *)ddsAddr);

                        var formatStr = $"{name}-{playerIndex}-{frameI}";
                        Texture? texture;
                        _loaded.TryGetValue(formatStr, out texture);
                        if (_context.FormatDDS != null && texture == null) {
                            Console.WriteLine($"Loading {formatStr}");
                            texture = LZLRFile.GetTexture(_gameState, sprite->lzlr, _context.FormatDDS);
                            _loaded.Add(formatStr, texture);
                            // string[] paths = { @"C:\data", $"{name}-{playerIndex}-{frameI}.dds" };
                            // if (!File.Exists(Path.Combine(paths))) {
                            //     File.WriteAllBytes(Path.Combine(paths), texture);
                            // }
                            // ImGui.ImageButton(texture.NativePointer, imgSize, uv0, uv1, 0, tintCol, borderCol);

                            // string[] paths = { @"C:\data", $"{name}-{playerIndex}-{frameI}.dds" };
                            // if (!File.Exists(Path.Combine(paths))) {
                            //     SharpDX.Direct3D9.Texture.ToFile(texture, Path.Combine(paths), SharpDX.Direct3D9.ImageFileFormat.Dds);
                            // }
                            //ImGui.Image(texture.NativePointer, imgSize, uv0, uv1, tintCol, borderCol);
                        }

                        if (texture != null) {
                            var textdesc = texture.GetLevelDescription(0);

                            ImGuiWindow window = ImGui.GetCurrentWindow();
                            var drawList = igNET.ImGui.GetWindowDrawList();

                            // Calculate the position and size of your image
                            Vector2 position = new Vector2(10, 10); // Replace with your desired position
                            Vector2 size = new Vector2(128, 128);    // Replace with your desired size

                            var cursorPos = new Vector2(window.DC.CursorPos.X, window.DC.CursorPos.Y);
                            var bbMin = cursorPos;
                            var bbMax = cursorPos + size;

                            // Draw the image using DrawList.AddImage
                            igNET.ImGuiNative.ImDrawList_AddImage(
                                drawList,
                                texture.NativePointer,
                                bbMin,
                                bbMax,
                                new Vector2(0, 0),
                                new Vector2(1, 1),
                                igNET.ImGui.GetColorU32(Vector4.Zero) // Tint color (white in this case)
                            );

                            ImGui.Text($"tex w {textdesc.Width} h {textdesc.Height} f {textdesc.Format} type {textdesc.Type} usage {textdesc.Usage} pool {textdesc.Pool}  {texture.NativePointer.ToString("x")} {texture.GetSurfaceLevel(0).NativePointer.ToString("x")}");
                            igNET.ImGui.Image(texture.NativePointer, size);

                            // Draw the image using DrawList.AddImage
                            igNET.ImGuiNative.ImDrawList_AddImage(
                                drawList,
                                texture.NativePointer,
                                new Vector2(position.X + (frameI * size.X), position.Y + (frameI * size.Y)),
                                new Vector2(position.X + size.X, position.Y + size.Y),
                                new Vector2(0, 0),
                                new Vector2(1, 1),
                                igNET.ImGui.GetColorU32(Vector4.Zero) // Tint color (white in this case)
                            );


                            // {
                            //     igNET.ImGui.BeginChild($"Sprite##{formatStr}");
                            //     igNET.ImGui.Image(texture.NativePointer, new Vector2(textdesc.Width, textdesc.Height));
                            //     igNET.ImGui.EndChild();
                            // }
                        }
                    }

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

        private unsafe void RenderHitbox(string name, string moveName, int playerIndex, int frameI, int i, ref Hitbox hitbox)
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
            var context = ImGui.GetCurrentContext();
            Console.WriteLine(new IntPtr(context.ActiveId).ToString("x"));
            bool isDefaultOpen = true;

            ImGui.ShowMetricsWindow(ref isDefaultOpen);

            var foo = true;
            if (ImGui.Begin("Font", ref foo, 0))
            {
                var io = context.IO;
                if (io != null)
                {
                    this.fontTexId = io.Fonts.TexID;
                    var texW = io.Fonts.TexWidth;
                    var texH = io.Fonts.TexHeight;

                    var drawList = igNET.ImGui.GetForegroundDrawList();

                    var vMin = igNET.ImGui.GetWindowContentRegionMin();
                    var vMax = igNET.ImGui.GetWindowContentRegionMax();

                    var windowPos = igNET.ImGui.GetWindowPos();
                    vMin.X += windowPos.X;
                    vMin.Y += windowPos.Y;
                    vMax.X += windowPos.X;
                    vMax.Y += windowPos.Y;

                    var windowdl = igNET.ImGui.GetWindowDrawList();
                    // windowdl.AddImage(this.fontTexId, vMax, vMin);

                    Console.WriteLine($"tex 0x{new IntPtr(this.fontTexId).ToString("x")}");
                    drawList.AddRect(vMin, vMax, igNET.ImGui.GetColorU32(new Vector4(255, 255, 0, 255)));
                    // drawList.AddImage(texID, vMin, vMax, Vector2.Zero, Vector2.One, igNET.ImGui.GetColorU32(new Vector4(255, 255, 255, 255)));
                    igNET.ImGui.Image(this.fontTexId, new Vector2(texW, texH), Vector2.Zero, Vector2.One, new Vector4(255, 255, 255, 255), Vector4.Zero);
                } else {
                    Console.WriteLine("IO or Fonts null");
                }

                ImGui.End();
            }

            // if (ImGui.Begin("Forgery in Black", ref isDefaultOpen, 0))
            // {
            //     var defaultWindowSize = new ImVec2();
            //     defaultWindowSize.X = 300;
            //     defaultWindowSize.Y = 400;
            //     ImGui.SetWindowSizeVec2(defaultWindowSize, (int)ImGuiCond.FirstUseEver);

            //     if (ImGui.CollapsingHeaderBoolPtr("General", ref isDefaultOpen, 0))
            //     {
            //         if (_context.TextureAddresses != IntPtr.Zero)
            //         {
            //             ImGui.Text("Texture");
            //             igNET.ImGui.Image(_context.TextureAddresses, new Vector2(128, 512), Vector2.Zero, Vector2.One, Vector4.One, Vector4.One);
            //         }

            //         var size = new ImVec2();
            //         size.X = 128;
            //         size.Y = 512;

            //         var uv0 = new ImVec2();
            //         uv0.X = 0;
            //         uv0.Y = 0;

            //         var uv1 = new ImVec2();
            //         uv1.X = 1;
            //         uv1.Y = 1;

            //         var t = new ImVec4();
            //         t.X = 1;
            //         t.Y = 1;
            //         t.Z = 1;
            //         t.W = 1;

            //         // ImGui.Image(_context.TextureAddresses, size, uv0, uv1, t, t);

            //         ImGui.Text($"FPS: {_gameState->fps.ToString("0.##")}");
            //     }

            //     if (_context.match != null && _context.match->isValid())
            //     {
            //         if (ImGui.CollapsingHeaderBoolPtr("Current Match", ref isDefaultOpen, 0))
            //         {
            //             RenderPointer("Match:", _context.match);

            //             {
            //                 ImGui.BeginGroup();
            //                 ImGui.SliderInt("Timer", ref _context.match->timer, 0, 10800, "", 0);
            //                 ImGui.SameLine(0, 0);
            //                 ImGui.Checkbox("Lock", ref _context.timerLocked);
            //                 ImGui.EndGroup();
            //             }


            //             ImGui.Separator();

            //             RenderPlayer(1, _context.match->player1Characters());
            //             ImGui.Separator();
            //             RenderPlayer(2, _context.match->player2Characters());
            //         }
            //     }

            //     ImGui.ShowStyleSelector("style selector");
            //     // ImGui.ShowDemoWindow(ref isDefaultOpen);
            //     igNET.ImGui.ShowDemoWindow(ref isDefaultOpen);
            // }

            // ImGui.End();
        }
    }
}
