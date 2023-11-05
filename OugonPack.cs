using System.Text;

namespace Ougon {
    class OugonPack {
        static unsafe uint bit_marker;
        static unsafe uint bit_data;
        static unsafe uint bit_next;
        private static readonly uint[] table = {
            0x1,
            0x3,
            0x7,
            0xf,
            0x1f,
            0x3f,
            0x7f,
            0xff,
            0x1ff,
            0x3ff,
            0x7ff,
            0xfff,
            0x1fff,
            0x3fff,
            0x7fff,
            0xffff,
            0x1ffff,
            0x3ffff,
            0x7ffff,
            0xfffff,
            0x1fffff,
            0x3fffff,
            0x7fffff,
            0xffffff
        };

        public unsafe static uint Do_401030()
        {
            uint sv = 0x200;
            uint retval = 0;
            do
            {
                uint value = bit_marker & bit_data;
                if (value != 0)
                {
                    retval |= sv;
                }
                bit_marker >>= 1;
                if (bit_marker == 0)
                {
                    bit_marker = 0x80000000;
                    bit_data = bit_next++;
                }
                sv >>= 1;
            } while (sv != 0);
            return retval;
        }

        public unsafe static uint Do_401080()
        {
            int count = 0;
            int sv = 0;
            int ebx = 0;
            while (true)
            {
                uint value = bit_marker & bit_data;
                bit_marker >>= 1;
                if (bit_marker == 0)
                {
                    bit_marker = 0x80000000;
                    bit_data = bit_next++;
                }
                if (value == 0)
                {
                    break;
                }
                count++;
            }
            sv = 1 << count;
            ebx = 0;
            do
            {
                uint value = bit_marker & bit_data;
                if (value != 0)
                {
                    ebx |= sv;
                }
                bit_marker >>= 1;
                if (bit_marker == 0)
                {
                    bit_marker = 0x80000000;
                    bit_data = bit_next++;
                }
                sv >>= 1;
            } while (sv != 0);
            return (uint)(table[count] + ebx);
        }

        static unsafe bool Decompress(byte[] data, uint size, out byte[]? ddest, out uint dsize) {
            uint v1;
            uint v2;
            byte[] input;
            uint read_data;
            byte[] dest, dest_orig;
            if (!data.SequenceEqual(Encoding.ASCII.GetBytes("LZLR")))
            {
                ddest = null;
                dsize = 0;
                return false;
            }
            v1 = BitConverter.ToUInt32(data, 4);
            v2 = BitConverter.ToUInt32(data, 8);
            var bit_marker = 0x80000000;
            bit_next = BitConverter.ToUInt32(data, 12);
            bit_data = BitConverter.ToUInt32(data, 16);
            input = data.Skip((int)v2).ToArray();
            dest_orig = new byte[v1 + 0x10000];
            dest = dest_orig;
            read_data = 0;
            while (read_data < v1)
            {
                uint value = bit_marker & bit_data;
                bit_marker >>= 1;
                if (bit_marker == 0)
                {
                    bit_marker = 0x80000000;
                    bit_data = BitConverter.ToUInt32(data, (int)(20 + (bit_next - BitConverter.ToUInt32(data, 20)) * 4));
                    bit_next++;
                }
                if (value != 0)
                {
                    uint count1 = Do_401080();
                    while (count1-- > 0)
                    {
                        uint count2 = Do_401080();
                        uint count3 = Do_401030();
                        byte[] src = dest.Skip(-(int)(count3 * 4)).ToArray();
                        for (uint i = 0; i < count2; ++i)
                        {
                            Buffer.BlockCopy(src, 0, dest, 0, 4);
                            src = src.Skip(4).ToArray();
                            dest = dest.Skip(4).ToArray();
                        }
                        read_data += count2 * 4;
                    }
                }
                else
                {
                    uint count = Do_401080();
                    Buffer.BlockCopy(input, 0, dest, 0, (int)(count * 4));
                    dest = dest.Skip((int)(count * 4)).ToArray();
                    input = input.Skip((int)(count * 4)).ToArray();
                    read_data += count * 4;
                }
            }
            ddest = dest_orig;
            dsize = v1;
            return true;
        }

        public static bool LoadAndDecomp(string filename, string base_path, out byte[]? ddata, out uint dsize)
        {
            string my_fname = Path.Combine(base_path, filename);
            FileStream? file = null;
            byte[]? data = null;
            byte[]? uncomp = null;
            uint size = 0;
            uint uncomp_size = 0;

            try
            {
                file = new FileStream(my_fname, FileMode.Open, FileAccess.Read);
                size = (uint)file.Length;
                data = new byte[size];
                file.Read(data, 0, (int)size);
            }
            catch (IOException)
            {
                ddata = null;
                dsize = 0;
                return false;
            }
            finally
            {
                file?.Close();
            }

            if (Decompress(data, size, out uncomp, out uncomp_size))
            {
                ddata = uncomp;
                dsize = uncomp_size;
                return true;
            } else {
                ddata = null;
                dsize = 0;
                return false;
            }
        }
    }
}
