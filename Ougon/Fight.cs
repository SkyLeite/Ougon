namespace Ougon {
  class Fight {
    public Player player1;
    public Player player2;
    public int timer;

    public Fight(Player player1, Player player2) {
        this.player1 = player1;
        this.player2 = player2;
    }

    public Character FindCharacter(byte id, byte color) {
        var characters = player1.characters.Concat(player2.characters);
        return characters.FirstOrDefault(character => character.id == id && character.color == color, null);
    }
  }
}
