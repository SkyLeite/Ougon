using System.Collections.ObjectModel;

namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Match
{
    [FieldOffset(0x1c)]
    public GameCharacter* p1Character1;

    [FieldOffset(0x24)]
    public GameCharacter* p1Character2;

    [FieldOffset(0x20)]
    public GameCharacter* p2Character1;

    [FieldOffset(0x28)]
    public GameCharacter* p2Character2;

    [FieldOffset(0x34)]
    public int* background;

    [FieldOffset(0xB54)]
    public MatchState matchState;

    [FieldOffset(0xB68)]
    public float cameraPositionX;

    [FieldOffset(0xB6C)]
    public float cameraPositionY;

    [FieldOffset(0xB80)]
    public float cameraZoomFactor;

    [FieldOffset(0xb88)]
    public int timer;

    [FieldOffset(0xb64)]
    public bool isPaused;

    public bool isValid()
    {
        return this.p1Character1 != null
            && this.p1Character2 != null
            && this.p2Character1 != null
            && this.p2Character2 != null;
    }

    public GameCharacter*[] player1Characters() =>
        new GameCharacter*[] { this.p1Character1, this.p1Character2 };

    public GameCharacter*[] player2Characters() =>
        new GameCharacter*[] { this.p2Character1, this.p2Character2 };

    // public GameCharacter*[] Characters() => new GameCharacter*[] { this.p1Character1, this.p1Character2, this.p2Character1, this.p2Character2 };

    public Collection<IntPtr> Characters() {
        var list = new Collection<IntPtr>();

        var p1Characters = this.player1Characters();
        var p2Characters = this.player2Characters();

        foreach (var character in p1Characters) {
            list.Add((IntPtr)character);
        }

        foreach (var character in p2Characters) {
            list.Add((IntPtr)character);
        }

        return list;
    }
}

public enum MatchState {
    None = 0,
    RoundStart = 1,
    Ongoing = 3,
    FadeOut = 4,
    ReturningToCharacterSelect = 5,
}
