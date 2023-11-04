namespace Ougon {
  public class Character {
    public string name { get; set; }
    public byte color { get; set; }

    public Character(string name, byte color) {
        this.name = name;
        this.color = color;
    }

    public static Character fromID(byte id, byte color) {
        var name = id switch {
            0 => "Battler",
            1 => "Ange",
            2 => "Shannon",
            3 => "Kanon",
            4 => "Lucifer",
            5 => "Chiester 410",
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

        return new Character(name, color);
    }
  }
}
