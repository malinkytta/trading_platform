namespace App;

public class Trade
{
    public List<Item> Items;
    public User Sender;
    public User Receiver;
    public TradeStatus Status;

    public Trade(List<Item> items, User sender, User receiver, TradeStatus status)
    {
        Items = items;
        Sender = sender;
        Receiver = receiver;
        Status = status;
    }
}
