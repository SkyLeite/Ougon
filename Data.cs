using System.Runtime.InteropServices;

namespace Ougon
{
    public enum Characters
    {
        Rosa = 12,
        Willard = 6,
        Ronove = 15,
        Virgilia = 8
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Match
    {
        [FieldOffset(0x1c)]
        public Character* p1Character1;

        [FieldOffset(0x24)]
        public Character* p1Character2;

        [FieldOffset(0x20)]
        public Character* p2Character1;

        [FieldOffset(0x28)]
        public Character* p2Character2;

        [FieldOffset(0xb88)]
        public int timer;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Something
    {
        [FieldOffset(0x2a604)]
        public Character* character;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Character
    {
        // Pointer to a team
        [FieldOffset(0x0)]
        public short team;

        // Very possibly an array somehow?
        [FieldOffset(0xC)]
        public Move* currentMove;

        [FieldOffset(0x144)]
        public CharacterInMatch* inMatch;

        [FieldOffset(0x10)]
        public Game* game;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Move
    {
        // These offsets only work for Battler's c.5A for some reason
        [FieldOffset(0x3cc)]
        public byte damage;

        [FieldOffset(0x3cd)]
        public byte knockbackType;

        [FieldOffset(0x3ce)]
        public byte prop3;

        [FieldOffset(0x3cf)]
        public byte hitEffect;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CharacterInMatch
    {
        [FieldOffset(0x41c)]
        public int health;

        // When over 1, the next knockdown stuns
        [FieldOffset(0x40c)]
        public float stun;

        [FieldOffset(0x420)]
        public float meter;

        [FieldOffset(0x3d0)]
        public short positionX;

        [FieldOffset(0x3d4)]
        public short positionY;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Game
    {
        [FieldOffset(0x0B84)]
        public int matchTimer;
    }


    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Team
    {
        [FieldOffset(0x044C)]
        public short positionX;

        [FieldOffset(0x450)]
        public short positionY;

        [FieldOffset(0x498)]
        public short p1Health;

        [FieldOffset(0x550A0)]
        public short* health;
    }
}
