namespace App;

public class Trade
{
    public Item TradedItem;
    public User Sender;
    public User Receiver;
    public TradeStatus Status;

    public Trade(Item item, User sender, User receiver, TradeStatus status)
    {
        TradedItem = item;
        Sender = sender;
        Receiver = receiver;
        Status = status;
    }
}
