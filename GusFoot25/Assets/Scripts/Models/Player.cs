public class Player {
    public string Name;
    public int Age;
    public string Position;    // e.g. "GK", "DEF", "MID", "FWD"
    public int OverallRating;  // 0–100
    public int Value;          // transfer value

    public Player(string name, int age, string position, int rating, int value) {
        Name = name;
        Age = age;
        Position = position;
        OverallRating = rating;
        Value = value;
    }
}