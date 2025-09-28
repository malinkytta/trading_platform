using App;

List<User> users = new List<User>();
List<Item> items = new List<Item>();

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

            if (RegisterUser(users))
            {
                currentMenu = Menu.None;
            }
            else
            {
                currentMenu = Menu.Register;
            }

            break;

        case Menu.Login:

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
                            UploadItem(items, activeUser);
                            break;
                        case "2":
                            ShowItems(items, activeUser);
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

static bool RegisterUser(List<User> users)
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
        users.Add(new User(name, email, password));

        Console.WriteLine("New user created successfully!");
        Console.WriteLine("Press ENTER to return to the menu.");

        Console.ReadLine();
        return true;
    }
}

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
    Console.WriteLine("Login failed. Press ENTER to retun to the menu.");
    Console.ReadLine();

    return null;
}

static User? UploadItem(List<Item> items, User activeUser)
{
    Console.Clear();

    Console.Write("Title: ");
    string title = Console.ReadLine();

    Console.Write("Description: ");
    string desc = Console.ReadLine();

    items.Add(new Item(title, desc, activeUser));

    Console.WriteLine($"Item '{title}' uploaded successfully!");
    Console.ReadLine();
    return activeUser;
}

static void ShowItems(List<Item> items, User? activeUser)
{
    Console.Clear();
    if (items.Count == 0)
    {
        Console.WriteLine("There are no items uploaded yet.");
    }
    else
    {
        Console.WriteLine("All items:");
        int i = 1;
        foreach (Item item in items)
        {
            if (item.Owner != activeUser)
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
