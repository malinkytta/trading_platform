using App;

List<User> users = new List<User>();
List<Item> items = new List<Item>();

string userPath = "users.csv";

if (File.Exists(userPath))
{
    string[] users_scv = File.ReadAllLines(userPath);
    foreach (string user in users_scv)
    {
        string[] split_userdata = user.Split(",");
        users.Add(new User(split_userdata[0], split_userdata[1], split_userdata[2]));
    }
}

bool isRunning = true;
User? activeUser = null;
Menu currentMenu = Menu.None;

while (isRunning)
{
    switch (currentMenu)
    {
        case Menu.None:
            Console.Clear();

            Console.WriteLine("----- Welcome to the Trading Platform -----\n");

            Console.WriteLine("Please select an option: ");
            Console.WriteLine("1] Register new account");
            Console.WriteLine("2] Login");
            Console.WriteLine("3] Exit the application");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    currentMenu = Menu.Register;
                    break;

                case "2":
                    currentMenu = Menu.Login;
                    break;

                case "3":
                    currentMenu = Menu.Exit;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Press ENTER to try again.");
                    Console.ReadLine();
                    currentMenu = Menu.None;
                    break;
            }
            break;

        case Menu.Register:

            // RegisterUser(users) returnerar en bool (sant/falskt värde)
            // true = lyckad registrering, tillbaka till startmeny.
            // false = misslyckad registering, stanna kvar i samma meny så man kan försöka igen.
            if (RegisterUser(users, userPath))
            {
                currentMenu = Menu.None;
            }
            else
            {
                currentMenu = Menu.Register;
            }

            break;

        case Menu.Login:

            //LoginUser(users) returnerar en User om lyckad inloggning, annars null.
            // Om vi får en User, spara som activeUser och gå till huvudmenyn, annars till startsidan.
            activeUser = LoginUser(users);
            if (activeUser != null)
            {
                currentMenu = Menu.Main;
            }
            else
            {
                currentMenu = Menu.None;
            }
            break;

        case Menu.Main:

            Console.Clear();
            Console.WriteLine("----- Main Menu -----");
            Console.WriteLine($"Logged in as: {activeUser?.Name}\n");
            Console.WriteLine("Please select an option: ");

            Console.WriteLine("1] Items");
            Console.WriteLine("2] Trades");
            Console.WriteLine("3] Log out");
            Console.WriteLine("4] Exit");

            input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("----- Items -----");
                    Console.WriteLine("1] Upload a new item");
                    Console.WriteLine("2] Browse items");

                    input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            // Skapar ett nytt item kopplat till den inloggade användaren
                            UploadItem(items, activeUser);
                            break;
                        case "2":
                            // Visar alla andras items
                            ShowItems(items, activeUser);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press ENTER to try again.");
                            Console.ReadLine();
                            break;
                    }
                    break;

                case "2":
                    break;

                case "3":
                    currentMenu = Menu.Logout;
                    break;

                case "4":
                    currentMenu = Menu.Exit;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Press ENTER to try again.");
                    Console.ReadLine();
                    break;
            }

            break;

        case Menu.Exit:
            isRunning = false;
            break;

        case Menu.Logout:
            activeUser = null;
            currentMenu = Menu.None;
            break;
    }
}

// Försöker registrera ny användare.
// Returnerar true om registreringen lyckades, annars false.
static bool RegisterUser(List<User> users, string userPath)
{
    {
        Console.Clear();
        Console.WriteLine("----- Register new user -----");

        Console.Write("Name: ");
        string name = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        if (name == "" || email == "" || password == "")
        {
            Console.Clear();
            Console.WriteLine("All fields are required. Please try again.");
            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
            return false;
        }

        Console.Clear();

        string[] newUser = { $"{name},{email},{password}" }; // skapar en array av strängar
        File.AppendAllLines(userPath, newUser); // lägger till den nya användaren i listan
        users.Add(new User(name, email, password));

        Console.WriteLine("New user created successfully!");
        Console.WriteLine("Press ENTER to return to the menu.");

        Console.ReadLine();
        return true;
    }
}

// Försöker logga in användare med e-post och lösenord.
// Returnerar User om inloggning lyckats, annars null.
static User? LoginUser(List<User> users)
{
    Console.Clear();
    Console.WriteLine("----- Login -----");

    Console.Write("Email: ");
    string email = Console.ReadLine();

    Console.Write("Password: ");
    string password = Console.ReadLine();

    Console.Clear();

    foreach (User user in users)
    {
        if (user.TryLogin(email, password))
        {
            return user;
        }
    }
    Console.WriteLine("Login failed. Press ENTER to return to the menu.");
    Console.ReadLine();

    return null;
}

// Låter den inloggade användaren ladda upp ett nytt item.
static void UploadItem(List<Item> items, User activeUser)
{
    Console.Clear();

    Console.Write("Title: ");
    string title = Console.ReadLine();

    Console.Write("Description: ");
    string desc = Console.ReadLine();

    items.Add(new Item(title, desc, activeUser)); //Kopplar den inloggade användaren till item

    Console.WriteLine($"Item '{title}' uploaded successfully!");
    Console.ReadLine();
}

// Visar alla items som inte tillhör den inloggade användaren.
static void ShowItems(List<Item> items, User? activeUser)
{
    Console.Clear();
    if (items.Count == 0) //om det inte finns några items så kommer den in här
    {
        Console.WriteLine("There are no items uploaded yet.");
    }
    else
    {
        Console.WriteLine("All items:");
        int i = 1;
        foreach (Item item in items)
        {
            if (item.Owner != activeUser) // visa inte items som ägs av den inloggade användaren
            {
                Console.WriteLine(
                    $"{i}] Name: {item.Name}. Description: {item.Description}. Owner: {item.Owner.Name}"
                );
            }
            i++;
        }
    }
    Console.WriteLine("Press ENTER to return.");
    Console.ReadLine();
}
