using System.Diagnostics;
using static Utility;

public class Account
{
    public string accountId;
    public User owner;
    public float balance;
    public float? withdrawlLimit;


    public Account(User owner)
    {
        accountId = GenerateAccountId();
        this.owner = owner;
        balance = 0;
    }

    public string GenerateAccountId()
    {
        string newId = Guid.NewGuid().ToString("N");
        return newId;
    }

    
    public int Deposit(float amount)
    {
        if (amount <= 0)
        {
            Utility.PrintError((int)ErrorCode.Deposit, "Invalid Deposit Amount", "Amount ($" + amount + ") to deposit less than/equal to $0.");
            return (int)ErrorCode.Deposit;
        }

        balance += amount;
        return (int)ErrorCode.Success;
    }

    public int Withdrawl(float amount)
    {
        if (amount <= 0)
        {
            Utility.PrintError((int)ErrorCode.Withdrawl, "Invalid Withdrawl Amount", "Amount ($" + amount + ") to deposit less than/equal to $0.");
            return (int)ErrorCode.Withdrawl;
        }
        if (amount > balance)
        {
            Utility.PrintError((int)ErrorCode.Withdrawl, "Invalid Withdrawl Amount", "Amount ($" + amount + ") is greater than current balance.");
            return (int)ErrorCode.Withdrawl;
        }
        if (withdrawlLimit != null && amount > withdrawlLimit)
        {
            Utility.PrintError((int)ErrorCode.Withdrawl, "Invalid Withdrawl Amount", "Amount ($" + amount + ") is greater than withdrawl limit of [$" + withdrawlLimit + "].");
            return (int)ErrorCode.Withdrawl;
        }

        balance -= amount;
        return (int)ErrorCode.Success;
    }
}

public class Checking : Account
{
    public Checking(User owner) : base(owner)
    {
    }
}

public class Investment : Account
{
    public Investment(User owner) : base(owner)
    {
    }
}

public class Individual : Investment
{
    public Individual(User owner) : base(owner)
    {
        balance = 0;
        withdrawlLimit = 500.0f;
    }
}

public class Corporate : Investment
{
    public Corporate(User owner) : base(owner)
    {
        balance = 0;
    }
}