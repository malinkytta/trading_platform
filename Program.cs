using App;

List<User> users = new List<User>();

bool isRunning = true;
User? activeUser = null;
Menu currentMenu = Menu.None;

while (isRunning)
{
    switch (currentMenu)
    {
        case Menu.None:
            Console.Clear();

            Console.WriteLine("----- Welcome to the Trading Platform -----");
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
            break;

        case Menu.Main:
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
