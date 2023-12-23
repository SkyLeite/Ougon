using DearImguiSharp;
using Ougon.Data;

namespace Ougon.GUI;

sealed record Colors {
    public static ImVec4 startupFrame = new ImVec4 { X = 0.03f, Y = 0.7f, Z = 0.5f, W = 1 };
    public static ImVec4 activeFrame = new ImVec4 { X = 0.7f, Y = 0.2f, Z = 0.4f, W = 1 };
    public static ImVec4 recoveryFrame = new ImVec4 { X = 0.02f, Y = 0.4f, Z = 0.7f, W = 1 };
    public static ImVec4 blank = new ImVec4 { X = 0, Y = 0, Z = 0, W = 1 };
}

sealed class FrameData {
    private Context Context;

    private unsafe Sequence* CurrentSequence = null;

    public FrameData(Context context)
    {
        this.Context = context;
    }

    public unsafe void Render() {
        if (this.Context.match == null) {
            return;
        }


        var character = this.Context.match->player1Characters()[0];

        if (character == null) {
            return;
        }

        if (character->currentFrame == null) {
            return;
        }

        bool isDefaultOpen = true;
        var columns = 60;

        this.CurrentSequence = character->currentSequence;

        // if (this.Context.FrameHistory.Count > columns || character->currentSequence->IsIdle()) {
        //     if (this.Context.shouldUpdate) {
        //         this.Context.FrameHistory.Reset();
        //     }
        // }

        // if (this.Context.shouldUpdate) {
        //     if (character->currentSequence->IsIdle()) {
        //         if (this.Context.FrameHistory.Count > 0) {
        //             this.Context.FrameHistory.Add((nint)character->currentFrame);
        //             this.Context.shouldUpdate = false;
        //         }
        //     } else {
        //         this.Context.FrameHistory.Add((nint)character->currentFrame);
        //         this.Context.shouldUpdate = false;
        //     }
        // }

        var windowFlags = 0;
        windowFlags |= (int)ImGuiWindowFlags.NoResize;
        windowFlags |= (int)ImGuiWindowFlags.NoCollapse;
        windowFlags |= (int)ImGuiWindowFlags.NoNav;
        windowFlags |= (int)ImGuiWindowFlags.NoBringToFrontOnFocus;
        windowFlags |= (int)ImGuiWindowFlags.NoDocking;
        windowFlags |= (int)ImGuiWindowFlags.NoMouseInputs;
        windowFlags |= (int)ImGuiWindowFlags.NoScrollbar;

        using (var windowPos = new ImVec2 { X = 80, Y = 400 })
        using (var windowPivot = new ImVec2 { X = 0, Y = 0 })
        ImGui.SetNextWindowPos(windowPos, 1, windowPivot);

        using (var windowSize = new ImVec2 { X = 800, Y = 50 })
        ImGui.SetNextWindowSize(windowSize, 1);

        if (ImGui.Begin("Player 1", ref isDefaultOpen, windowFlags)) {
            var tableFlags = 0;
            tableFlags |= (int)ImGuiTableFlags.Borders;

            using (var tableSize = new ImVec2 { X = 800, Y = 20})
            if (ImGui.BeginTable("Player 1 FrameData", columns, tableFlags, tableSize, 100)) {
                ImGui.TableNextRow(0, 20);

                for (int column = 0; column < columns; column++) {
                    ImGui.TableSetColumnIndex(column);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("");

                    var color = Colors.blank;

                    try {
                        if (character->currentSequence->IsIdle() == false)
                        {
                            color = this.Context.FrameHistory.Kind(column) switch
                            {
                                FrameKind.Startup => Colors.startupFrame,
                                FrameKind.Active => Colors.activeFrame,
                                FrameKind.Recovery => Colors.recoveryFrame,
                                _ => Colors.blank
                            };
                        }
                    } catch (ArgumentOutOfRangeException) {
                    }

                    ImGui.TableSetBgColor((int)ImGuiTableBgTarget.CellBg, ImGui.GetColorU32Vec4(color), column);
                }

                ImGui.EndTable();
            }

            ImGui.End();
        };
    }
}
