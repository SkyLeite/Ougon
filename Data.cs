using System.Runtime.InteropServices;

namespace Ougon
{
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

        [FieldOffset(0xb88)]
        public int timer;

        [FieldOffset(0xb64)]
        public bool isPaused;

        public bool isValid() {
            return this.p1Character1 != null && this.p1Character2 != null && this.p2Character1 != null && this.p2Character2 != null;
        }

        public GameCharacter*[] player1Characters() => new GameCharacter*[] { this.p1Character1, this.p1Character2 };
        public GameCharacter*[] player2Characters() => new GameCharacter*[] { this.p2Character1, this.p2Character2 };
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Something
    {
        [FieldOffset(0x2a604)]
        public GameCharacter* character;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct GameCharacter
    {
        // Pointer to a team
        [FieldOffset(0x0)]
        public short team;

        // Very possibly an array somehow?
        [FieldOffset(0xC)]
        public Move* currentMove;

        [FieldOffset(0x144)]
        public GameCharacterInMatch* inMatch;

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
    public unsafe struct GameCharacterInMatch
    {
        [FieldOffset(0x41c)]
        public int health;

        // When over 1, the next knockdown stuns
        [FieldOffset(0x40c)]
        public float stun;

        [FieldOffset(0x420)]
        public float meter;

        [FieldOffset(0x3d0)]
        public float positionX;

        [FieldOffset(0x3d4)]
        public float positionY;
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
