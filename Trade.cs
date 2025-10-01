namespace App;

public class Trade
{
    public List<Item> Items;
    public User Sender;
    public User Receiver;
    public TradeStatus Status;

    public Trade(List<Item> items, User sender, User reciever, TradeStatus status)
    {
        Items = items;
        Sender = sender;
        Receiver = reciever;
        Status = status;
    }
}
