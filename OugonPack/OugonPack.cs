namespace OugonPack;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public class OugonPack
{
    private const int TABLE_SIZE = 24;
    private static readonly uint[] Table = new uint[TABLE_SIZE]
    {
        0x1,     0x3,     0x7,      0xf,      0x1f,     0x3f,
        0x7f,    0xff,    0x1ff,    0x3ff,    0x7ff,    0xfff,
        0x1fff,  0x3fff,  0x7fff,   0xffff,   0x1ffff,  0x3ffff,
        0x7ffff, 0xfffff, 0x1fffff, 0x3fffff, 0x7fffff, 0xffffff
    };

    private uint bitMarker;
    private uint bitData;
    private uint[] bitNext;

    private uint Do401030()
    {
        uint sv = 0x200;
        uint retval = 0;

        do
        {
            uint value = bitMarker & bitData;
            if (value != 0)
            {
                retval |= sv;
            }

            bitMarker >>= 1;
            if (bitMarker == 0)
            {
                bitMarker = 0x80000000;
                bitData = bitNext[0];
                bitNext = bitNext[1..];
            }

            sv >>= 1;
        } while (sv != 0);

        return retval;
    }

    private uint Do401080()
    {
        uint count = 0;
        uint sv = 0;
        uint ebx = 0;

        while (true)
        {
            uint value = bitMarker & bitData;

            bitMarker >>= 1;
            if (bitMarker == 0)
            {
                bitMarker = 0x80000000;
                bitData = bitNext[0];
                bitNext = bitNext[1..];
            }

            if (value == 0)
            {
                break;
            }

            count++;
        }

        sv = (uint)(1 << (int)count);
        ebx = 0;

        do
        {
            uint value = bitMarker & bitData;
            if (value != 0)
            {
                ebx |= sv;
            }

            bitMarker >>= 1;
            if (bitMarker == 0)
            {
                bitMarker = 0x80000000;
                bitData = bitNext[0];
                bitNext = bitNext[1..];
            }

            sv >>= 1;
        } while (sv != 0);

        return Table[count] + ebx;
    }

    public bool Decompress(byte[] data, out byte[] decompressedData)
    {

            Debug.WriteLine("hello");
            Debug.Flush();
        uint v1;
        uint v2;
        byte[] input;
        uint readData;
        byte[] dest;
        byte[] destOrig;

        if (!data.AsSpan(0, 4).SequenceEqual(new byte[] { 0x4C, 0x5A, 0x4C, 0x52 }))
        {
            decompressedData = null;
            return false;
        }

            Debug.WriteLine("hello");
            Debug.Flush();

        v1 = BitConverter.ToUInt32(data, 4);
        v2 = BitConverter.ToUInt32(data, 8);

            Debug.WriteLine("hello");
            Debug.Flush();

        bitMarker = 0x80000000;
        bitNext = new uint[(data.Length - 12) / 4];
        Buffer.BlockCopy(data, 12, bitNext, 0, bitNext.Length * 4);
        bitData = bitNext[0];
        bitNext = bitNext[1..];

            Debug.WriteLine("hello");
            Debug.Flush();

        input = data.AsSpan((int)v2).ToArray();

            Debug.WriteLine("hello");
            Debug.Flush();

        destOrig = new byte[v1 + 0x10000];
        dest = destOrig;

        readData = 0;

            Debug.WriteLine("hello");
            Debug.Flush();

        while (readData < v1)
        {


            uint value = bitMarker & bitData;

            bitMarker >>= 1;
            if (bitMarker == 0)
            {
                bitMarker = 0x80000000;
                bitData = bitNext[0];
                bitNext = bitNext[1..];
            }


            if (value != 0)
            {
                uint count1 = Do401080();

                while (count1-- > 0)
                {

                    uint count2 = Do401080();
                    uint count3 = Do401030();


                    //byte[] src = dest.AsSpan(1000).ToArray();
                    var srcSegment = new ArraySegment<byte>(dest, (int)(count3 * 4), dest.Length - (int)(count3 * 4));
                    byte[] src = srcSegment.Array;

                    for (int i = 0; i < count2; i++)
                    {
                        
                        Buffer.BlockCopy(src, i * 4, dest, 0, 4);
                        Array.Copy(dest, 4, dest, 0, dest.Length - 4);
                        Array.Resize(ref dest, dest.Length - 4);
                    }

                    readData += count2 * 4;
                }
            }
            else
            {
                uint count = Do401080();

                Buffer.BlockCopy(input, 0, dest, 0, (int)(count * 4));
                dest = dest.AsSpan((int)(count * 4)).ToArray();
                input = input.AsSpan((int)(count * 4)).ToArray();

                readData += count * 4;
            }
        }

        decompressedData = destOrig;
        return true;
    }

    public byte[] LoadAndDecomp(string filepath) {
        byte[] compressedData = File.ReadAllBytes(filepath);
        if (Decompress(compressedData, out byte[] decompressedData))
        {
            // Use the decompressed data
            Console.WriteLine("Decompression successful!");
            Console.WriteLine($"Decompressed data size: {decompressedData.Length} bytes");
            return decompressedData;
        }
        else
        {
            Console.WriteLine("Decompression failed!");
            return new byte[] {};
        }
    }
}
