namespace Ougon.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct GameCharacter
{
    [FieldOffset(0xC)]
    public Sequence* currentSequence;

    [FieldOffset(0x60)]
    public Frame* currentFrame;

    [FieldOffset(0x64)]
    public Frame* nextFrame;

    [FieldOffset(0x2aa68)]
    public bool isLeftSide;

    [FieldOffset(0x1A)]
    public ushort currentSpriteID;

    [FieldOffset(0xD4)]
    public float absolutePositionX;

    [FieldOffset(0xD8)]
    public float absolutePositionY;

    [FieldOffset(0x51C)]
    // [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 4096)]
    public Sprite* sprites;

    [FieldOffset(0x144)]
    public GameCharacterInMatch* inMatch;

    [FieldOffset(0x520)]
    public Sequence* idle;

    [FieldOffset(0x524)]
    public Sequence* _5;

    [FieldOffset(0x528)]
    public Sequence* _5to2;

    [FieldOffset(0x52C)]
    public Sequence* yawn;

    [FieldOffset(0x530)]
    public Sequence* _2;

    [FieldOffset(0x534)]
    public Sequence* _3to1;

    [FieldOffset(0x538)]
    public Sequence* _2to5;

    [FieldOffset(0x540)]
    public Sequence* _6;

    [FieldOffset(0x544)]
    public Sequence* _4;

    [FieldOffset(0x548)]
    public Sequence* _8;

    [FieldOffset(0x54C)]
    public Sequence* _9;

    [FieldOffset(0x54E)]
    public Sequence* _7;

    [FieldOffset(0x564)]
    public Sequence* tagIn;

    [FieldOffset(0x570)]
    public Sequence* _66to5;

    [FieldOffset(0x574)]
    public Sequence* _44;

    [FieldOffset(0x610)]
    public Sequence* far5A;

    [FieldOffset(0x614)]
    public Sequence* close5A;

    [FieldOffset(0x61c)]
    public Sequence* far5B;

    [FieldOffset(0x620)]
    public Sequence* close5B;

    [FieldOffset(0x624)]
    public Sequence* _6B;

    [FieldOffset(0x628)]
    public Sequence* far5C;

    [FieldOffset(0x62c)]
    public Sequence* close5C;

    [FieldOffset(0x630)]
    public Sequence* _6C;

    [FieldOffset(0x634)]
    public Sequence* _2A;

    [FieldOffset(0x63c)]
    public Sequence* _2B;

    [FieldOffset(0x640)]
    public Sequence* _3B;

    [FieldOffset(0x644)]
    public Sequence* _2C;

    [FieldOffset(0x64c)]
    public Sequence* j8A;

    [FieldOffset(0x650)]
    public Sequence* j8B;

    [FieldOffset(0x654)]
    public Sequence* j8C;

    [FieldOffset(0x658)]
    public Sequence* j9A;

    [FieldOffset(0x65c)]
    public Sequence* j9B;

    [FieldOffset(0x65e)]
    public Sequence* j9C;

    [FieldOffset(0x668)]
    public Sequence* j2C;

    [FieldOffset(0x6F4)]
    public Sequence* grab;

    [FieldOffset(0x6F8)]
    public Sequence* grabWhiff;

    [FieldOffset(0x674)]
    public Sequence* callTag;

    [FieldOffset(0x678)]
    public Sequence* tagOut; // Battler's 236A

    [FieldOffset(0x688)]
    public Sequence* specialMove1A; // Battler's 236A

    [FieldOffset(0x68C)]
    public Sequence* specialMove1B; // Battler's 236B

    [FieldOffset(0x690)]
    public Sequence* specialMove1C; // Battler's 236C

    [FieldOffset(0x694)]
    public Sequence* specialMove1X; // Battler's 236AB

    [FieldOffset(0x698)]
    public Sequence* specialMove2A; // Battler's 623A

    [FieldOffset(0x69C)]
    public Sequence* specialMove2B; // Battler's 623B

    [FieldOffset(0x6A0)]
    public Sequence* specialMove2C; // Battler's 623C

    [FieldOffset(0x6A4)]
    public Sequence* specialMove2X; // Battler's 623AB

    [FieldOffset(0x6A8)]
    public Sequence* specialMove3A; // Battler's 214A

    [FieldOffset(0x6AC)]
    public Sequence* specialMove3B; // Battler's 214B

    [FieldOffset(0x6B0)]
    public Sequence* specialMove3C; // Battler's 214C

    [FieldOffset(0x6B4)]
    public Sequence* specialMove3X; // Battler's 214AB

    [FieldOffset(0x6B8)]
    public Sequence* specialMove4A; // Battler's j.214A

    [FieldOffset(0x6BC)]
    public Sequence* specialMove4B; // Battler's j.214B

    [FieldOffset(0x6C0)]
    public Sequence* specialMove4C; // Battler's j.214C

    [FieldOffset(0x6C4)]
    public Sequence* specialMove4X; // Battler's j.214AB

    [FieldOffset(0x6D0)]
    public Sequence* super2; // Battler's 236236C

    [FieldOffset(0x6D4)]
    public Sequence* metaSuper; // Battler's 236236C

    [FieldOffset(0x780)]
    public Sequence* metaDeclare;

    [FieldOffset(0x2a620)]
    public int existance;

    public static Sequence*[] GetSequences(GameCharacter* character)
    {
        var length = 248;
        var charAddr = new IntPtr(character);
        var firstSeq = IntPtr.Add(charAddr, 0x524);
        Sequence*[] sequenceArray = new Sequence*[length];

        for (int i = 0; i < length; i++)
        {
            sequenceArray[i] = (Sequence*)*((nint*)IntPtr.Add(firstSeq, 0x4 * i));
        }

        return sequenceArray;
    }

    public static unsafe int GetSequenceIndex(GameCharacter* character, string sequenceStr)
    {
        int characterBaseAddress = (int)character;

        int offset = (int)Marshal.OffsetOf<GameCharacter>(sequenceStr);

        return (offset - 0x520) / 4;
    }

    public static Sprite* GetSpriteFromID(GameCharacter* character, ushort sprite_id)
    {
        var spriteArray = new IntPtr(character->sprites);
        var spriteOffset = sprite_id * 4;
        var spriteAddr = (Sprite**)IntPtr.Add(spriteArray, spriteOffset);

        Console.WriteLine(
            $"id: {sprite_id} arr: {spriteArray.ToString("x")} offset: {spriteOffset} {spriteOffset.ToString("x")} Sprite addres: {new IntPtr(spriteAddr).ToString("x")}"
        );

        return *spriteAddr;
    }
}
