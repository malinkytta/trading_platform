namespace App;

public class Item
{
    public string Name;
    public string Description;
    public User Owner;

    public Item(string name, string description, User owner)
    {
        Name = name;
        Description = description;
        Owner = owner;
    }
}
