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
            }
            break;

        case Menu.Register:
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
