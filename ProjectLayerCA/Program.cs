using ProjectLayerClassLibrary;


string? displayMenu()
{
    Console.WriteLine("Possible operations:\n" 
        + "1. Change owner name\n"
        + "2. Change owner surname\n"
        + "3. Show balance\n"
        + "4. Deposit\n"
        + "5. Withdraw\n"
        + "6. Exit\n"
        + "What you want to do: ");
    return Console.ReadLine();
}

void showBalance(BankAccount bankAccount)
{
    Console.Write($"Account balance: {bankAccount.getAccountBalance()}\n");
}

void deposit(BankAccount bankAccount)
{
    bool correctOperation = false;
    while (!correctOperation)
    {
        Console.WriteLine("What amount you want to deposit: ");
        string? read = Console.ReadLine();
        if (float.TryParse(read, out float value))
        {
            try
            {
                correctOperation = bankAccount.deposit(value);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            Console.WriteLine("ENter value is not number!");
        }
    }

}

void withdraw(BankAccount bankAccount)
{
    bool correctOperation = false;
    while (!correctOperation)
    {
        Console.WriteLine("What amount you want to withdraw: ");
        string? read = Console.ReadLine();
        if (float.TryParse(read, out float value))
        {
            try
            {
                correctOperation = bankAccount.withdraw(value);
                if (!correctOperation)
                {
                    Console.WriteLine("You can't withdraw more than you have on account!");
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            Console.WriteLine("ENter value is not number!");
        }
    }

}

bool active = true;
BankAccount bankAccount = new BankAccount("Jan", "Kowalski");

Console.WriteLine($"Welcome in Bank {bankAccount.getAccountOwnerName()} {bankAccount.getAccountOwnerSurname()}!\n\n");
while (active)
{
    switch (displayMenu())
    {
        case "1":

            break;

        case "2":

            break;

        case "3":
            showBalance(bankAccount);
            break;

        case "4":
            deposit(bankAccount);
            break;

        case "5":
            withdraw(bankAccount);
            break;

        case "6":
            active = false;
            break;

        default:
            Console.WriteLine("Incorrect operation has been chosen!");
            break;
    }
}

Console.WriteLine("Goodbye!");