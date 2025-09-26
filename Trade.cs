namespace App;

public class Trade
{
    public List<Item> Items;
    public User Sender;
    public User Receiver;
    public string Status;

    public Trade(List<Item> items, User sender, User reciever, string status)
    {
        Items = items;
        Sender = sender;
        Receiver = reciever;
        Status = status;
    }
}
