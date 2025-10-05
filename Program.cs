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

        if (fields.Length < 5)
        {
            break;
        }

        string senderName = fields[0];
        string receiverName = fields[1];
        string statusText = fields[fields.Length - 1];

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

        // plocka alla items mellan index 2 och stanna innan sista eftersom det är status, i += 2 eftersom det är ett par (item&owner)
        for (int i = 2; i < fields.Length - 1; i += 2)
        {
            string itemName = fields[i]; //första fältet är item name
            string ownerName = fields[i + 1]; // plus 1 eftersom nästa är ägare

            User owner = null;
            foreach (User user in users)
            {
                if (user.Name == ownerName)
                {
                    owner = user;
                    break;
                }
            }

            if (owner == null)
            {
                break;
            }

            for (int j = 0; j < items.Count; j++)
            {
                if (items[j].Name == itemName && items[j].Owner == owner)
                {
                    tradedItems.Add(items[j]);
                    break;
                }
            }
        }

        TradeStatus currentStatus;
        switch (statusText)
        {
            case "Pending":
                currentStatus = TradeStatus.Pending;
                break;

            case "Approved":

                currentStatus = TradeStatus.Approved;
                break;

            case "Denied":

                currentStatus = TradeStatus.Denied;
                break;

            default:
                currentStatus = TradeStatus.None;
                break;
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
                    Console.WriteLine("2] Completed trades");
                    Console.WriteLine("3] Back to Main Menu");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            HandlePending(trades, activeUser, tradesFilePath, items, itemsFilePath);
                            break;

                        case "2":
                            PrintTradesByStatus(trades, activeUser);
                            break;

                        case "3":
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
    SaveItemsToFile(items, itemsFilePath);

    Console.WriteLine($"Item '{title}' uploaded successfully!");
    Console.ReadLine();
}

// Visar alla items som inte tillhör den inloggade användaren.
static void ShowItems(List<Item> items, User activeUser, List<Trade> trades, string tradesFilePath)
{
    Console.Clear();

    // Lista för andras items som inte tillhör den inloggade
    List<Item> othersItems = new List<Item>();

    foreach (Item item in items)
    {
        if (item.Owner != activeUser)
        {
            othersItems.Add(item);
        }
    }

    //Filtrera bort items som är med i pending-trades
    List<Item> availableItems = GetAvailableItems(othersItems, trades);

    for (int tries = 0; tries < availableItems.Count; tries++)
    {
        for (int i = 1; i < availableItems.Count; i++)
        {
            Item current = availableItems[i];
            Item previous = availableItems[i - 1];

            if (string.Compare(current.Owner.Name, previous.Owner.Name) < 0)
            {
                availableItems[i] = previous;
                availableItems[i - 1] = current;
            }
        }
    }

    if (availableItems.Count == 0)
    {
        Console.WriteLine("No available items yet.");
        Console.WriteLine("Press ENTER to return.");
        Console.ReadLine();
        return;
    }

    // Välj flera “andras” items, lås till första valda ägarens items
    List<Item> selected = PickItemsFromList(
        availableItems,
        "----- Pick items to request -----",
        true
    );

    if (selected.Count > 0)
    {
        CreateTrade(trades, activeUser, selected, tradesFilePath, items);
    }
}

// Returnerar en lista med items från 'availableItems'
// som ännu inte har blivit valda i 'selectedItems'
static List<Item> GetRemainingItems(List<Item> availableItems, List<Item> selectedItems)
{
    List<Item> remainingItems = new List<Item>();

    foreach (Item item in availableItems)
    {
        if (!selectedItems.Contains(item))
        {
            remainingItems.Add(item);
        }
    }

    return remainingItems;
}

// Låter användaren välja ett item från en lista, eller trycka ENTER för att avbryta
// Returnerar det valda itemet, eller null om användaren avbryter
static Item? SelectItemPrompt(List<Item> availableItems, List<Item> selectedItems, string menuTitle)
{
    Console.Clear();
    Console.WriteLine($"{menuTitle}\n");

    if (selectedItems.Count > 0)
    {
        Console.WriteLine("Selected so far: ");
        foreach (Item selectedItem in selectedItems)
        {
            Console.WriteLine(" - " + selectedItem.Name);
        }
    }
    Console.WriteLine("Available:");
    int i = 1;
    foreach (Item item in availableItems)
    {
        Console.WriteLine($"{i}] {item.Name} - {item.Description} (Owner: {item.Owner.Name})");
        i++;
    }

    Console.WriteLine("\nEnter number to add, or press ENTER to finish:");

    string input = Console.ReadLine();

    if (input == "")
    {
        return null;
    }

    // Försöker först omvandla användarens inmatning till en siffra (choice).
    // Kontrollerar sedan att siffran är minst 1 och inte större än antalet items i listan.
    // Listan som visas för användaren börjar på 1, men index i listan börjar på 0,
    // därför används choice - 1 för att få rätt item.
    // Om båda kontrollerna stämmer returneras det valda itemet.
    if (int.TryParse(input, out int choice) && choice >= 1 && choice <= availableItems.Count)
    {
        return availableItems[choice - 1];
    }

    Console.WriteLine("Invalid choice. Press ENTER to continue.");
    Console.ReadLine();
    return null;
}

// Skapar en ny trade mellan sender och itemets ägare.
// Sparar också traden i filen.
static void CreateTrade(
    List<Trade> trades,
    User sender,
    List<Item> wantedItems,
    string tradesFilePath,
    List<Item> items
)
{
    Console.Clear();

    // Ägaren till det valda (den andras) item
    User receiver = wantedItems[0].Owner;
    List<Item> payload = new List<Item>();

    // Kolla ägare + lägg till items i payload
    foreach (Item wantedItem in wantedItems)
    {
        payload.Add(wantedItem);
    }

    List<Item> offeredItems = PickOwnItems(items, sender, trades);

    foreach (Item offeredItem in offeredItems)
    {
        payload.Add(offeredItem);
    }

    //Skapa och spara traden
    Trade newTrade = new Trade(payload, sender, receiver, TradeStatus.Pending);
    trades.Add(newTrade);
    SaveTradesToFile(trades, tradesFilePath);

    Console.Clear();
    Console.WriteLine($"Trade request sent to: {receiver.Name}");
    Console.WriteLine("You want:");
    foreach (Item wantedItem in wantedItems)
    {
        Console.WriteLine($"- {wantedItem.Name}");
    }

    Console.WriteLine($"You offered: ");
    if (offeredItems.Count == 0)
    {
        Console.WriteLine("- nothing");
    }
    else
    {
        foreach (Item offeredItem in offeredItems)
        {
            Console.WriteLine(" - " + offeredItem.Name);
        }
    }

    Console.WriteLine("\nPress ENTER to continue");
    Console.ReadLine();
}

// Sparar hela listan av trades till fil.
static void SaveTradesToFile(List<Trade> trades, string tradesFilePath)
{
    List<string> tradeLines = new List<string>();

    foreach (Trade trade in trades)
    {
        string line = $"{trade.Sender.Name},{trade.Receiver.Name}";

        foreach (Item item in trade.Items)
        {
            line += $",{item.Name},{item.Owner.Name}";
        }
        line += $",{trade.Status}";
        tradeLines.Add(line);
    }
    File.WriteAllLines(tradesFilePath, tradeLines);
}

// Låter användaren välja sina egna items att erbjuda i en trade
// Returnerar en lista med valda items, eller en tom lista om inga väljs
static List<Item> PickOwnItems(List<Item> items, User activeUser, List<Trade> trades)
{
    // Hämta bara mina egna items
    List<Item> myItems = FilterByOwner(items, activeUser);

    // Filtrera bort de som redan är med i pending-trades
    myItems = GetAvailableItems(myItems, trades);

    if (myItems.Count == 0)
    {
        Console.WriteLine(
            "\nYou don't have any items to offer. Press ENTER to continue without offering."
        );
        Console.ReadLine();

        return new List<Item>();
    }

    // Lista över vad jag väljer
    List<Item> selectedItems = PickItemsFromList(
        myItems,
        "----- Pick your items to offer -----",
        false
    );

    return selectedItems;
}

// Låter en användare hantera sina Pending trades.
// Kan välja att godkänna, neka eller gå tillbaka.
static void HandlePending(
    List<Trade> trades,
    User activeUser,
    string tradesFilePath,
    List<Item> items,
    string itemsFilePath
)
{
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
    Console.WriteLine("--------- Manage trade ---------");
    Console.WriteLine(
        $"\nManage request #{inputChoice} from {selected.Sender.Name} → {selected.Receiver.Name}"
    );
    Console.WriteLine("\n[A] Approve   [D] Deny   [ENTER] Back");

    string action = Console.ReadLine();

    action = action.ToUpper();

    if (action == "A")
    {
        Console.Clear();
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
        Console.Clear();
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
    Console.Clear();
    Console.WriteLine($"----- Pending trade requests (for {activeUser.Name}) -----");

    List<Trade> pendingList = new List<Trade>();

    int i = 1;

    foreach (Trade trade in trades)
    {
        if (trade.Receiver == activeUser && trade.Status == TradeStatus.Pending)
        {
            pendingList.Add(trade);

            Console.WriteLine($"\n{i}] From: {trade.Sender.Name}");
            Console.WriteLine("   Wants:");
            foreach (Item item in trade.Items)
            {
                if (item.Owner == trade.Receiver)
                {
                    Console.WriteLine($"     - {item.Name}");
                }
            }

            Console.WriteLine("   Offers:");
            bool anyWanted = false;
            foreach (Item item in trade.Items)
            {
                if (item.Owner == trade.Sender)
                {
                    Console.WriteLine($"     - {item.Name}");
                    anyWanted = true;
                }
            }
            i++;
            if (!anyWanted)
            {
                Console.WriteLine("    - nothing");
            }
        }
    }
    return pendingList;
}

//Filtrerar en lista av items och returnerar endast de som ägs av en viss användare
static List<Item> FilterByOwner(List<Item> source, User owner)
{
    List<Item> result = new List<Item>();
    foreach (Item item in source)
    {
        if (item.Owner == owner)
            result.Add(item);
    }
    return result;
}

// Sparar hela listan av items till fil.
static void SaveItemsToFile(List<Item> items, string itemsFilePath)
{
    List<string> itemLines = new List<string>();
    foreach (Item item in items)
    {
        string itemLine = $"{item.Name},{item.Description},{item.Owner.Name}";
        itemLines.Add(itemLine);
    }
    File.WriteAllLines(itemsFilePath, itemLines);
}

// Skriver ut alla trades för den inloggade användaren som är antingen Approved eller Denied.
static void PrintTradesByStatus(List<Trade> trades, User activeUser)
{
    Console.Clear();
    Console.WriteLine("--------------------------------------");
    Console.WriteLine($"      Completed trades   ");
    Console.WriteLine("--------------------------------------\n");

    int i = 1;
    bool foundApproved = false;
    bool foundDenied = false;

    Console.WriteLine("-------- [A] Approved trades ---------\n");
    foreach (Trade trade in trades)
    {
        if (trade.Status == TradeStatus.Approved)
        {
            foundApproved = true;

            PrintTrade(trade, i);
            i++;
        }
    }
    if (!foundApproved)
    {
        Console.WriteLine($"- No approved trades\n");
    }

    // //Denied trades
    Console.WriteLine("---------- [D] Denied trades ---------\n");
    int index = 1;
    foreach (Trade trade in trades)
    {
        if (
            (trade.Sender == activeUser || trade.Receiver == activeUser)
            && trade.Status == TradeStatus.Denied
        )
        {
            foundDenied = true;
            PrintTrade(trade, index);
            index++;
        }
    }
    if (!foundDenied)
    {
        Console.WriteLine($"- No denied trades \n");
    }

    Console.WriteLine("--------------------------------------\n");
    Console.WriteLine("Press ENTER to return.");
    Console.ReadLine();
}

static void PrintTrade(Trade trade, int index)
{
    List<string> wantedItems = new List<string>();
    List<string> offeredItems = new List<string>();

    Console.WriteLine($"{index}] {trade.Sender.Name} -> {trade.Receiver.Name}");

    if (trade.Status == TradeStatus.Approved)
    {
        foreach (Item item in trade.Items)
        {
            if (item.Owner == trade.Receiver)
            {
                offeredItems.Add(item.Name);
            }
            else
            {
                wantedItems.Add(item.Name);
            }
        }
    }
    else
    {
        foreach (Item item in trade.Items)
        {
            if (item.Owner == trade.Receiver)
            {
                wantedItems.Add(item.Name);
            }
            else if (item.Owner == trade.Sender)
            {
                offeredItems.Add(item.Name);
            }
        }
    }
    // (Vet inte om vi gått igenom detta i kursen, så i värsta fall får jag skriva om det till en hederlig if-sats istället)
    // Kollar villkoret före "?". Är det true → används/skrivs det som står efter "?";
    // är det false så används/skrivs det som står efter ":"
    string wantedText = wantedItems.Count > 0 ? string.Join(" and ", wantedItems) : "nothing";
    string offeredText = offeredItems.Count > 0 ? string.Join(" and ", offeredItems) : "nothing";

    Console.WriteLine(
        $"   Wanted: {wantedText}\n   Offered: {offeredText}\n   Status: {trade.Status}\n"
    );
}

// Returnerar alla items som inte är med i någon pågående (Pending) trade
static List<Item> GetAvailableItems(List<Item> allItems, List<Trade> trades)
{
    List<Item> availableItems = new List<Item>();

    foreach (Item item in allItems)
    {
        bool isPending = false;

        // Kolla om item redan finns med i en pending trade
        foreach (Trade trade in trades)
        {
            if (trade.Status == TradeStatus.Pending)
            {
                foreach (Item traded in trade.Items)
                {
                    if (traded == item)
                    {
                        isPending = true;
                        break;
                    }
                }
            }
            if (isPending)
                break;
        }
        if (!isPending)
        {
            availableItems.Add(item);
        }
    }
    return availableItems;
}

// Låter användaren välja flera items från en lista.
// Om "sameOwnerOnly" är true så låser metoden valet till den ägare som tillhör det första itemet som väljs (används i ShowItems).
// Om false, kan användaren välja fritt bland alla (används i PickOwnItems).
static List<Item> PickItemsFromList(List<Item> sourceItems, string header, bool sameOwnerOnly)
{
    Console.Clear();
    List<Item> selectedItems = new List<Item>();
    User lockedOwner = null;
    string currentTitle = header;

    while (true)
    {
        // Bygg aktuell lista över valbara items (ev. filtrerad på låst ägare)
        List<Item> availableItems = new List<Item>();

        for (int i = 0; i < sourceItems.Count; i++)
        {
            Item currentItem = sourceItems[i];
            if (lockedOwner == null || currentItem.Owner == lockedOwner)
            {
                availableItems.Add(currentItem);
            }
        }

        //Ta bort de som redan är valda
        List<Item> remainingItems = GetRemainingItems(availableItems, selectedItems);
        if (remainingItems.Count == 0)
        {
            break;
        }

        // Välj ett item eller avbryt med ENTER
        Item? chosenItem = SelectItemPrompt(remainingItems, selectedItems, currentTitle);

        if (chosenItem == null)
        {
            break;
        }

        // Lås ägare efter första valet om så önskas
        if (sameOwnerOnly && lockedOwner == null)
        {
            lockedOwner = chosenItem.Owner;
            currentTitle = $"----- Pick items to request (only from: {lockedOwner.Name}) -----";
        }
        selectedItems.Add(chosenItem);
    }
    return selectedItems;
}
