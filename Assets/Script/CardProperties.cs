public class CardProperties
{
    // 0:"単体攻撃",1:"貫通攻撃",2:"列攻撃",3:"爆発攻撃"
    public int type { get; set; }
    public string name { get; set; }
    public int power { get; set; }

    public CardProperties(int type, string name, int power)
    {
        this.type = type;
        this.name = name;
        this.power = power;
    }
}
