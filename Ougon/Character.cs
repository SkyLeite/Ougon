namespace Ougon
{
    public class Character
    {
        public byte id { get; set; }
        public string name { get; set; }
        public byte color { get; set; }
        public HashSet<string> sequenceHistory { get; set; } = new HashSet<string>();

        public Character(byte id, string name, byte color)
        {
            this.id = id;
            this.name = name;
            this.color = color;
        }

        public void AddToSequenceHistory(IntPtr sequencePtr)
        {
            this.sequenceHistory.Add($"0x{sequencePtr.ToString("x")}");
        }

        public void ClearSequenceHistory()
        {
            this.sequenceHistory.Clear();
        }

        public static Character fromID(byte id, byte color)
        {
            var name = id switch
            {
                0 => "Battler",
                1 => "Ange",
                2 => "Shannon",
                3 => "Kanon",
                4 => "Lucifer",
                5 => "Chiester 410",
                6 => "Ronove",
                7 => "EVA Beatrice",
                8 => "Virgilia",
                9 => "Beatrice",
                10 => "George",
                11 => "Jessica",
                12 => "Rosa",
                13 => "Erika",
                14 => "Dlanor",
                15 => "Willard",
                16 => "Bernkastel",
                17 => "Lambdadelta",
                _ => id.ToString()
            };

            return new Character(id, name, color);
        }
    }
}
