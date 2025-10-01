using App;

string usersFilePath = "users.csv";
string itemsFilePath = "items.csv";

List<User> users = new List<User>();
List<Item> items = new List<Item>();

bool isRunning = true;
User? activeUser = null;
Menu currentMenu = Menu.None;

//Kolla om user-filen finns, annars hoppa över
if (File.Exists(usersFilePath))
{
    // Läs in alla rader från users.csv och spara i userLines (en array av strängar)
    string[] userLines = File.ReadAllLines(usersFilePath);

    foreach (string userLine in userLines)
    {
        // Dela upp raden efter kommatecken
        string[] fields = userLine.Split(",");

        // Skapa en User och lägg till i listan
        users.Add(new User(fields[0], fields[1], fields[2]));
    }
}

// Kolla om item-filen finns, annars hoppa över
if (File.Exists(itemsFilePath))
{
    // Läs in alla rader från items.csv och spara i itemLines (array av strängar)
    string[] itemLines = File.ReadAllLines(itemsFilePath);

    foreach (string itemLine in itemLines)
    {
        // Dela upp raden efter kommatecken
        string[] fields = itemLine.Split(",");

        string ownerName = fields[2];

        // Hitta rätt User som matchar ownerName
        foreach (User user in users)
        {
            if (user.Name == ownerName)
            {
                // Skapa ett Item och koppla ihop med användaren
                items.Add(new Item(fields[0], fields[1], user));
                break; //sluta leta, vi hittade rätt
            }
        }
    }
}

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
            if (RegisterUser(users, usersFilePath))
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
                            UploadItem(items, activeUser, itemsFilePath);
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
static bool RegisterUser(List<User> users, string usersFilePath)
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

        File.AppendAllLines(usersFilePath, newUser); // lägger till den nya användaren i listan

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
static void UploadItem(List<Item> items, User activeUser, string itemsFilePath)
{
    Console.Clear();

    Console.Write("Title: ");
    string title = Console.ReadLine();

    Console.Write("Description: ");
    string desc = Console.ReadLine();

    items.Add(new Item(title, desc, activeUser)); //Kopplar den inloggade användaren till item
    string owner = activeUser.Name;

    string[] newItem = { $"{title},{desc},{owner}" };
    File.AppendAllLines(itemsFilePath, newItem);

    Console.WriteLine($"Item '{title}' uploaded successfully!");
    Console.ReadLine();
}

// Visar alla items som inte tillhör den inloggade användaren.
static void ShowItems(List<Item> items, User? activeUser)
{
    Console.Clear();

    if (items.Count == 0)
    {
        Console.WriteLine("There are no items uploaded yet.");
        Console.WriteLine("Press ENTER to return.");
        Console.ReadLine();
        return;
    }
    else
    {
        // Används både som räknare för att skriva ut listan snyggt men även för att hålla koll på vems items som syns
        int i = 0;
        foreach (Item item in items)
        {
            // visa bara andras grejer
            if (item.Owner.Name != activeUser.Name)
            {
                Console.WriteLine(
                    $"{i + 1}] Name: {item.Name}. Description: {item.Description}. Owner: {item.Owner.Name}"
                );
                // +1 så utskriften börjar på 1, inte 0

                i++;
            }
        }

        // Om i inte ökas, så är det ens egna items som finns i listan och man hamnar här
        if (i == 0)
        {
            Console.WriteLine("No items yet");
        }
    }
    Console.WriteLine("Press ENTER to return.");
    Console.ReadLine();
}
