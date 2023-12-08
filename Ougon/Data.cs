using System.Runtime.InteropServices;
using SharpDX.Direct3D9;
using static Ougon.Mod;

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
    public unsafe struct Sprite {
        [FieldOffset(0x8)]
        public LZLRFile* lzlr;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct LZLRFile {
        [FieldOffset(0x0)]
        public char* LZLR;

        [FieldOffset(0x4)]
        public int DDSSize;

        [FieldOffset(0x8)]
        public short DDSOffset;

        public static Texture? GetTexture(GameState * gameState, LZLRFile* lzlr, FormatDDS formatDDS) {
            var lzlrAddress = new IntPtr(lzlr);
            var ddsPtr = IntPtr.Add(lzlrAddress, lzlr->DDSOffset);

            try
            {
                var ddsData = formatDDS((int*)lzlr, null);
                var size = lzlr->DDSSize;
                byte[] arr = new byte[size];
                Marshal.Copy((IntPtr)ddsData, arr, 0, size);

                var device = SharpDX.Direct3D9.Device.FromPointer<SharpDX.Direct3D9.Device>(gameState->DX9Device);
                int width = 128;
                int height = 512;
                int mipLevels = 1;
                var usage = SharpDX.Direct3D9.Usage.None;
                var format = SharpDX.Direct3D9.Format.A8R8G8B8;
                var pool = SharpDX.Direct3D9.Pool.Managed;
                var filter = (SharpDX.Direct3D9.Filter)1;
                var mipFilter = SharpDX.Direct3D9.Filter.Box;
                var colorKey = 0;
                var texture = SharpDX.Direct3D9.Texture.FromMemory(device, arr, width, height, mipLevels, usage, format, pool, filter, mipFilter, colorKey);
                return texture;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }
    }


    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct GameCharacter
    {
        // Pointer to a team
        [FieldOffset(0x0)]
        public short team;

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

        [FieldOffset(0x10)]
        public Game* game;

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

        [FieldOffset(0x54C)]
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

        public static Sequence*[] GetSequences(GameCharacter* character) {
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

        public static Sprite* GetSpriteFromID(GameCharacter* character, ushort sprite_id) {
            var spriteArray = new IntPtr(character->sprites);
            var spriteOffset = sprite_id * 4;
            var spriteAddr = (Sprite**)IntPtr.Add(spriteArray, spriteOffset);

            Console.WriteLine($"id: {sprite_id} arr: {spriteArray.ToString("x")} offset: {spriteOffset} {spriteOffset.ToString("x")} Sprite addres: {new IntPtr(spriteAddr).ToString("x")}");

            return *spriteAddr;
        }
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpriteInfo
    {
        public int width;
        public int height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Hitbox
    {
        public short x;
        public short y;
        public short w;
        public short h;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, Size = 96)]
    public struct Frame
    {
        public ushort sprite_id;
        public ushort duration;
        public ushort unknown02;
        public short tex_x;
        public short tex_y;

        public ushort unknown05;
        public ushort unknown06;
        public ushort unknown07;
        public ushort unknown08;
        public ushort unknown09;
        public ushort unknown0a;
        public ushort unknown0b;
        public ushort unknown0c;
        public ushort unknown0d;

        public Hitbox attackbox;

        public Hitbox hitbox1;
        public Hitbox hitbox2;
        public Hitbox hitbox3;

        public ushort unknown1e;
        public ushort unknown1f;
        public ushort unknown20;
        public ushort unknown21;

        public struct Attack
        {
            public byte damage;
            public byte prop1;
            public ushort prop2;
            public ushort prop3;
            public ushort prop4;
        }

        public Attack attack;

        public ushort unknown26;
        public ushort unknown27;
        public ushort unknown28;
        public ushort unknown29;
        public ushort unknown2a;
        public ushort unknown2b;
        public ushort unknown2c;
        public ushort unknown2d;
        public ushort unknown2e;
        public ushort unknown2f;
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

    [StructLayout(LayoutKind.Explicit)]
    public struct SequenceHeader
    {
        [FieldOffset(0x0)]
        public int movementState;

        [FieldOffset(0x4)]
        public int movementState2;

        [FieldOffset(0x8)]
        public int movementState3;

        [FieldOffset(0x40)]
        public uint frame_count;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Sequence {
        [FieldOffset(0x0)]
        public SequenceHeader header;

        [FieldOffset(0x44)]
        public Frame frame0;

        [FieldOffset(0x44 + (0x60 * 1))]
        public Frame frame1;

        [FieldOffset(0x44 + (0x60 * 2))]
        public Frame frame2;

        [FieldOffset(0x44 + (0x60 * 3))]
        public Frame frame3;

        [FieldOffset(0x44 + (0x60 * 4))]
        public Frame frame4;

        [FieldOffset(0x44 + (0x60 * 5))]
        public Frame frame5;

        [FieldOffset(0x44 + (0x60 * 6))]
        public Frame frame6;

        [FieldOffset(0x44 + (0x60 * 7))]
        public Frame frame7;

        [FieldOffset(0x44 + (0x60 * 8))]
        public Frame frame8;

        [FieldOffset(0x44 + (0x60 * 9))]
        public Frame frame9;

        [FieldOffset(0x44 + (0x60 * 10))]
        public Frame frame10;

        [FieldOffset(0x44 + (0x60 * 11))]
        public Frame frame11;

        [FieldOffset(0x44 + (0x60 * 12))]
        public Frame frame12;

        [FieldOffset(0x44 + (0x60 * 13))]
        public Frame frame13;

        [FieldOffset(0x44 + (0x60 * 14))]
        public Frame frame14;

        [FieldOffset(0x44 + (0x60 * 15))]
        public Frame frame15;

        [FieldOffset(0x44 + (0x60 * 16))]
        public Frame frame16;

        [FieldOffset(0x44 + (0x60 * 17))]
        public Frame frame17;

        [FieldOffset(0x44 + (0x60 * 18))]
        public Frame frame18;

        [FieldOffset(0x44 + (0x60 * 19))]
        public Frame frame19;

        [FieldOffset(0x44 + (0x60 * 20))]
        public Frame frame20;

        public unsafe Frame*[] frames() {
            var length = 21;
            Frame*[] frameArray = new Frame*[length];

            fixed (Frame* pFrame0 = &frame0)
            fixed (Frame* pFrame1 = &frame1)
            fixed (Frame* pFrame2 = &frame2)
            fixed (Frame* pFrame3 = &frame3)
            fixed (Frame* pFrame4 = &frame4)
            fixed (Frame* pFrame5 = &frame5)
            fixed (Frame* pFrame6 = &frame6)
            fixed (Frame* pFrame7 = &frame7)
            fixed (Frame* pFrame8 = &frame8)
            fixed (Frame* pFrame9 = &frame9)
            fixed (Frame* pFrame10 = &frame10)
            fixed (Frame* pFrame11 = &frame11)
            fixed (Frame* pFrame12 = &frame12)
            fixed (Frame* pFrame13 = &frame13)
            fixed (Frame* pFrame14 = &frame14)
            fixed (Frame* pFrame15 = &frame15)
            fixed (Frame* pFrame16 = &frame16)
            fixed (Frame* pFrame17 = &frame17)
            fixed (Frame* pFrame18 = &frame18)
            fixed (Frame* pFrame19 = &frame19)
            fixed (Frame* pFrame20 = &frame20)
            {
                frameArray[0] = pFrame0;
                frameArray[1] = pFrame1;
                frameArray[2] = pFrame2;
                frameArray[3] = pFrame3;
                frameArray[4] = pFrame4;
                frameArray[5] = pFrame5;
                frameArray[6] = pFrame6;
                frameArray[7] = pFrame7;
                frameArray[8] = pFrame8;
                frameArray[9] = pFrame9;
                frameArray[10] = pFrame10;
                frameArray[11] = pFrame11;
                frameArray[12] = pFrame12;
                frameArray[13] = pFrame13;
                frameArray[14] = pFrame14;
                frameArray[15] = pFrame15;
                frameArray[16] = pFrame16;
                frameArray[17] = pFrame17;
                frameArray[18] = pFrame18;
                frameArray[19] = pFrame19;
                frameArray[20] = pFrame20;
            }

            return frameArray;
        }
    };
}
