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
}
