
# C# - Examinerande uppgift: Trading system

Det här är en examinerande uppgift i kursen **Objektorienterad programmering**. 
 
Uppgiften gick ut på att designa och implementera ett *trading-system* där användare, items och trades sparas lokalt i filer så att datan finns kvar nästa gång man startar programmet.

#### Programmet är ett konsolbaserat trading-system där man kan:
- Registrera en användare  
- Logga in och logga ut  
- Ladda upp saker (items) att byta  
- Bläddra bland andra användares items  
- Skicka en trade-förfrågan på andras items  
- Se inkommande trade-förfrågningar  
- Acceptera eller neka en trade-förfrågan  
- Bläddra bland slutförda trades  

## Så kör du programmet
Öppna projektet i terminalen och kör:

```bash
dotnet run
```

## Mina designval
### Komposition
Jag använder **komposition** i uppgiften, eftersom det finns tydliga *har-en*-relationer. En användare har saker (items) och en trade har både en avsändare, en mottagare samt en lista med items. 

### Enum
Jag använder *enum* för att hålla ordning på olika tillstånd i programmet:
- `TradeStatus` – berättar om en trade är **Pending**, **Approved** eller **Denied**.
- `Menu` – håller reda på vilken meny användaren är i (tex Login, Main osv).

Jag valde att använda enums istället för strängar eftersom det minskar risken för stavfel och för att jag tycker koden blir lättare att läsa.

### Arv, interface och polymorfism
Jag har inte använt *arv, interface* eller *polymorfism* eftersom klasserna (`User`, `Item`, `Trade`) har olika roller i programmet. Det finns ingen naturlig hierarki mellan dem och har inga *är-en*-relationer, en trade är till exempel inte en användare eller ett item.  

Ett interface hade kunnat vara användbart om jag hade haft olika typer av användare, till exempel en admin. Men nu har jag bara en typ av användare så såg inte att det behövdes då.

### Metoder
#### User
- `RegisterUser` - låter en användare skapa ett nytt konto. Returnerar en `bool` (true/false) som visar om registreringen lyckades.
- `LoginUser` - loggar in en användare med e-post och lösenord. Returnerar en `User` om inloggningen lyckas, annars `null`.
- `TryLogin` - jämför sparad e-post och lösenord med användarens input. Används i `LoginUser` för att bekräfta inloggning.

#### Item
- `UploadItem` - skapar ett nytt item kopplat till den inloggade användaren.
- `ShowItems` - visar andra användares tillgängliga items och låter den inloggade användaren välja vilka de vill ha i en trade.
- `FilterByOwner` - returnerar alla items som ägs av en viss användare.
- `SaveItemsToFile` - sparar hela listan av items till filen *items.csv*.
- `GetAvailableItems` - returnerar items som inte är med i någon pågående (pending) trade.
- `GetRemainingItems` - används för att visa kvarvarande valbara items i listor när man skapar en trade.
- `SelectItemPrompt` - skriver ut en lista med items och låter användaren välja genom att ange ett nummer (index).

#### Trade
- `CreateTrade` - skapar en ny trade mellan den inloggade användaren (sender) till en annan användare (receiver) baserat på valda items. 
- `SaveTradesToFile` - sparar alla trades till filen *trades.csv*
- `HandlePending` - låter användaren hantera inkommande (pending) trades genom att godkänna eller neka.
- `PrintPending` - visar alla trades med status pending för den inloggade användaren.
- `PrintTradesByStatus` - visar alla trades som är slutförda (Approved och Denied) och skriver ut dem i konsolen med hjälp av:
- `PrintTrade` - skriver ut detaljer om en enskild trade: avsändare, mottagare, vilka items samt status.
- `PickOwnItems` - låter användaren välja egna items att erbjuda i en trade.
- `PickItemsFromList` - skriver ut en lista och låta användaren välja flera items. Kan även låsa urvalet så man bara kan välja från samma ägare.

### Samband mellan metoder (flöde)
Här är några exempel på hur metoderna hänger ihop i programmet.

#### När man skapar en trade:

```text
CreateTrade()
-> PickOwnItems()              // väljer egna items att erbjuda
-> SaveTradesToFile()          // sparar traden till CSV
-> skriver ut sammanfattning
```

#### När man hanterar pending:

```text
HandlePending()
-> PrintPending()              // visar inkommande förfrågningar
-> [A] Approve / [D] Deny     // uppdaterar status
-> SaveTradesToFile()         // sparar status Approved/Denied
-> SaveItemsToFile()          // sparar nya ägare (vid Approved)
```

#### Visa slutförda trades:

```text
PrintTradesByStatus()
-> PrintTrade()             // skriver ut wanted/offered/status
```

#### Filformat (CSV)
- `users.csv`: `name,email,password`
- `items.csv`: `itemName,description,ownerName`
- `trades.csv`: `sender,receiver,item,itemOwner,...,status`

### Reflektioner och förbättringar
Under arbetets gång märkte jag att koden växte ganska snabbt, vilket gjorde det svårare att behålla en tydlig struktur. Jag valde att dela upp koden i flera egna metoder för att göra den mer lättläst och undvika upprepning. Men i efterhand inser jag att jag hade kunnat planera strukturen bättre från början. Flera av metoderna löser dessutom liknande problem, och vissa hade kunnat slås ihop eller göras mer generella. Jag har också funderat på om vissa av metoderna egentligen hade passat bättre i sina respektive klasser, eller kanske i separata filer, för att göra koden mer lättöverskådlig.

Filhanteringen var ganska klurig och jag fick göra flera justeringar i hur en trade skrivs och läses, även nu, sista minuten. Eftersom ägarskap på items ändras efter en godkänd trade, blev det utmanande att behålla korrekt historik. Jag hade länge problem med att tidigare trades fick fel ägare när en ny trade godkändes, eftersom ägarskapet på items uppdateras i samband med det. Efter mycket testande insåg jag att jag inte behövde skriva över ägarna i `trades.csv` alls, utan bara uppdatera statusen. Jag flyttade därför `SaveTradesToFile` till innan själva ägarbytet sker. På så sätt sparas traden korrekt i filen, men utan att gamla trades påverkas när ägarskapen uppdateras. Det gjorde att completed trades nu visas som de ska och att historiken förblir korrekt. Åtminstone tror jag det, har inte haft tid att testa det tillräckligt mycket än, får jag erkänna.

När det gäller flödet för att skapa en trade valde jag att låta användaren först se alla andra användares items i en gemensam lista, och sedan välja med hjälp av index vilket item de vill ha. När det första valet görs låses listan till just den ägaren, så att användaren bara ser fler items från samma person. Efter mycket om och men fick jag det att fungera, men i efterhand hade det nog varit enklare att börja med att välja användare först och sedan se den personens items...

Om jag hade mer tid hade jag velat lägga till mer felhantering, till exempel att förhindra ogiltiga val eller tomma inputs. Men sammanfattningsvis är jag nöjd med programmets funktionalitet och logik, och jag ser flera möjligheter till förbättringar både i struktur och planering framöver.






