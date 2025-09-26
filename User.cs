namespace App;

public class User
{
    public string Email;
    string _password;

    public User(string email, string password)
    {
        Email = email;
        _password = password;
    }

    public bool TryLogin(string email, string password)
    {
        return Email == email && _password == password;
    }
}
