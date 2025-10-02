using App;

string usersFilePath = "users.csv";
string itemsFilePath = "items.csv";
string tradesFilePath = "trades.csv";

List<User> users = new List<User>();
List<Item> items = new List<Item>();
List<Trade> trades = new List<Trade>();

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

if (File.Exists(tradesFilePath))
{
    string[] tradeLines = File.ReadAllLines(tradesFilePath);

    foreach (string tradeLine in tradeLines)
    {
        string[] fields = tradeLine.Split(",");

        string senderName = fields[0];
        string receiverName = fields[1];
        string itemsList = fields[2];
        string statusText = fields[3];

        User sender = null;
        User receiver = null;

        // Hitta sender/receiver
        foreach (User user in users)
        {
            if (user.Name == senderName)
            {
                sender = user; // hittade sender
            }
            if (user.Name == receiverName)
            {
                receiver = user; // hittade receiver
            }

            if (sender != null && receiver != null)
            {
                break; // båda hittade, bryt loopen
            }
        }

        List<Item> tradedItems = new List<Item>();

        string[] itemNames = itemsList.Split("|");

        foreach (string itemName in itemNames)
        {
            foreach (Item item in items)
            {
                if (item.Name == itemName)
                {
                    tradedItems.Add(item);
                }
            }
        }

        TradeStatus currentStatus = TradeStatus.None;
        switch (statusText)
        {
            case "Pending":
                currentStatus = TradeStatus.Pending;
                break;

            case "Approved":
            {
                currentStatus = TradeStatus.Approved;
                break;
            }
            case "Denied":
            {
                currentStatus = TradeStatus.Denied;
                break;
            }
        }
        // lägg till trade om allt finns
        if (sender != null && receiver != null && tradedItems.Count > 0)
        {
            trades.Add(new Trade(tradedItems, sender, receiver, currentStatus));
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
                    Console.WriteLine("3] Go back to Main Menu");

                    input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            // Skapar ett nytt item kopplat till den inloggade användaren
                            UploadItem(items, activeUser, itemsFilePath);
                            break;
                        case "2":
                            // Visar alla andras items
                            ShowItems(items, activeUser, trades, tradesFilePath);
                            break;

                        case "3":
                            currentMenu = Menu.Main;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press ENTER to try again.");
                            Console.ReadLine();
                            break;
                    }
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("----- Trades -----\n");
                    Console.WriteLine("1] Pending (incoming)");
                    Console.WriteLine("2] Approved");
                    Console.WriteLine("3] Completed");
                    Console.WriteLine("4] Back to Main Menu");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            HandlePending(trades, activeUser, tradesFilePath, items, itemsFilePath);
                            break;

                        case "2":
                            break;

                        case "3":
                            break;

                        case "4":
                            currentMenu = Menu.Main;
                            break;

                        default:
                            break;
                    }
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
void ShowItems(List<Item> items, User activeUser, List<Trade> trades, string tradesFilePath)
{
    Console.Clear();

    // Lista för andras items
    List<Item> availableItems = new List<Item>();
    int i = 1;

    foreach (Item item in items)
    {
        if (item.Owner.Name != activeUser.Name)
        {
            bool hasPending = false;

            // kolla om item redan ligger i en pending trade
            foreach (Trade trade in trades)
            {
                if (trade.Status == TradeStatus.Pending)
                {
                    foreach (Item traded in trade.Items)
                    {
                        if (traded == item)
                        {
                            hasPending = true;
                            break;
                        }
                    }
                    if (hasPending)
                        break;
                }
            }

            if (!hasPending)
            {
                Console.WriteLine(
                    $"{i}] Name: {item.Name}. Description: {item.Description}. Owner: {item.Owner.Name}"
                );
                availableItems.Add(item);
                i++;
            }
        }
    }

    if (availableItems.Count == 0)
    {
        Console.WriteLine("No items from other users yet.");
        Console.WriteLine("Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("\nEnter number to request a trade, or press ENTER to go back:");
    string input = Console.ReadLine();

    if (input == "")
    {
        return;
    }

    if (!int.TryParse(input, out int inputChoice))
    {
        Console.WriteLine("Invalid choice. Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    if (inputChoice < 1 || inputChoice > availableItems.Count)
    {
        Console.WriteLine("Invalid choice. Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    Item wantedItem = availableItems[inputChoice - 1];
    CreateTrade(trades, activeUser, wantedItem, tradesFilePath, activeUser, items);
}

// Skapar en ny trade mellan sender och itemets ägare.
// Sparar också traden i filen.
static void CreateTrade(
    List<Trade> trades,
    User sender,
    Item wantedItem,
    string tradesFilePath,
    User activeUser,
    List<Item> items
)
{
    Console.Clear();

    // Ägaren till det valda (andras) itemet
    User receiver = wantedItem.Owner;

    List<Item> payload = new List<Item>();
    Item? offeredItem = PickOneOfOwnItems(items, activeUser);
    if (offeredItem == null)
    {
        Console.WriteLine("Cancelled.");
        Console.ReadLine();
        return;
    }

    payload.Add(wantedItem);
    payload.Add(offeredItem);

    Trade newTrade = new Trade(payload, sender, receiver, TradeStatus.Pending);

    trades.Add(newTrade);

    SaveTradesToFile(trades, tradesFilePath);

    Console.WriteLine($"Trade request sent to: {receiver.Name}");
    Console.WriteLine($"You want: {wantedItem.Name}");
    Console.WriteLine($"You offered: {offeredItem.Name}");

    Console.WriteLine("Press ENTER to continue");
    Console.ReadLine();
}

// Sparar hela listan av trades till fil.
static void SaveTradesToFile(List<Trade> trades, string tradesFilePath)
{
    List<string> tradeLines = new List<string>();
    foreach (Trade trade in trades)
    {
        string itemLines = "";
        foreach (Item item in trade.Items)
        {
            if (itemLines != "")
            {
                itemLines += "|";
            }

            itemLines += item.Name;
        }
        string tradeLine = $"{trade.Sender.Name},{trade.Receiver.Name},{itemLines},{trade.Status}";

        tradeLines.Add(tradeLine);
    }
    File.WriteAllLines(tradesFilePath, tradeLines);
}

static Item? PickOneOfOwnItems(List<Item> items, User activeUser)
{
    Console.Clear();
    Console.WriteLine("----- Pick one of your items to offer -----\n");

    List<Item> mine = new List<Item>();
    int i = 1;

    foreach (Item it in items)
    {
        if (it.Owner.Name == activeUser.Name)
        {
            Console.WriteLine($"{i}] {it.Name} - {it.Description}");
            mine.Add(it);
            i++;
        }
    }

    if (mine.Count == 0)
    {
        Console.WriteLine("\nYou don't have any items to offer. Press ENTER to cancel.");
        Console.ReadLine();
        return null;
    }

    Console.WriteLine("\nEnter number (or press ENTER to cancel):");

    string input = Console.ReadLine();

    if (input == "")
    {
        return null;
    }

    if (!int.TryParse(input, out int choice) || choice < 1 || choice > mine.Count)
    {
        Console.WriteLine("Invalid choice. Press ENTER to cancel.");
        Console.ReadLine();
        return null;
    }

    return mine[choice - 1];
}

// Låter en användare hantera sina Pending trades.
// Kan välja att godkänna, neka eller gå tillbaka.
void HandlePending(
    List<Trade> trades,
    User activeUser,
    string tradesFilePath,
    List<Item> items,
    string itemsFilePath
)
{
    Console.Clear();

    // skriv ut listan
    List<Trade> pendingList = PrintPending(trades, activeUser);

    if (pendingList.Count == 0)
    {
        Console.WriteLine("No pending requests right now.");
        Console.WriteLine("\nPress ENTER to return.");
        Console.ReadLine();
        return;
    }
    Console.WriteLine("\nEnter number to manage a request, or press ENTER to go back:");

    string input = Console.ReadLine();

    if (input == "")
    {
        return;
    }

    // försöker göra om input till en siffra och om det misslyckas hamnar man här
    if (!int.TryParse(input, out int inputChoice))
    {
        Console.WriteLine("Invalid choice. Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    // det användaren skriver in måste vara minst 1 och som mest listans längd.
    // mindre än 1 eller större än pendingList.Count är ogiltigt och då hamnar man här
    if (inputChoice < 1 || inputChoice > pendingList.Count)
    {
        Console.WriteLine("Invalid choice. Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    Trade selected = pendingList[inputChoice - 1];

    Console.Clear();
    Console.WriteLine("----- Trade details -----\n");
    Console.WriteLine($"From:   {selected.Sender.Name}");
    Console.WriteLine($"To:     {selected.Receiver.Name}");

    Console.WriteLine("Items:");
    foreach (Item item in selected.Items)
    {
        Console.WriteLine($"   - {item.Name}");
    }

    Console.WriteLine("\n[A] Approve   [D] Deny   [ENTER] Back");

    string action = Console.ReadLine();

    action = action.ToUpper();

    if (action == "A")
    {
        foreach (Item item in selected.Items)
        {
            if (item.Owner == selected.Sender)
                item.Owner = selected.Receiver;
            else if (item.Owner == selected.Receiver)
                item.Owner = selected.Sender;
        }
        selected.Status = TradeStatus.Approved;

        SaveTradesToFile(trades, tradesFilePath);
        SaveItemsToFile(items, itemsFilePath);

        Console.WriteLine("Request approved. Ownership updated and files saved.");
    }
    else if (action == "D")
    {
        selected.Status = TradeStatus.Denied;
        SaveTradesToFile(trades, tradesFilePath);
        Console.WriteLine("Request denied.");
    }
    else
    {
        Console.WriteLine("Cancelled.");
    }

    Console.WriteLine("Press ENTER to return.");
    Console.ReadLine();
}

// Skriver ut alla trades som är Pending för en användare.
// Returnerar listan med dessa trades.
static List<Trade> PrintPending(List<Trade> trades, User activeUser)
{
    Console.WriteLine("----- Pending trade requests -----\n");

    List<Trade> pendingList = new List<Trade>();

    int i = 1;

    foreach (Trade trade in trades)
    {
        if (trade.Receiver == activeUser && trade.Status == TradeStatus.Pending)
        {
            pendingList.Add(trade);

            Console.WriteLine($"{i}] From: {trade.Sender.Name} | Status: {trade.Status}");
            foreach (Item item in trade.Items)
            {
                Console.WriteLine($"    - {item.Name} (Owner: {item.Owner.Name})");
            }
            i++;
        }
    }
    return pendingList;
}

// Sparar hela listan av items till fil.
void SaveItemsToFile(List<Item> items, string itemsFilePath)
{
    List<string> itemLines = new List<string>();
    foreach (Item item in items)
    {
        string itemLine = $"{item.Name},{item.Description},{item.Owner.Name}";
        itemLines.Add(itemLine);
    }
    File.WriteAllLines(itemsFilePath, itemLines);
}
