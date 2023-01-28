using static Utility;

public class Bank
{
    public string bankId;
    public string name;
    public Dictionary<string, Account> accounts;

    public Bank(string name) {
        this.bankId = Guid.NewGuid().ToString("N");
        this.name = name;
        accounts = new Dictionary<string, Account>();
    }

    public void AddAccount(Account account)
    {
        accounts.Add(account.accountId, account);
    }

    public void RemoveAccount(Account account)
    {
        // What if list doesnt contain the account
        accounts.Remove(account.accountId);
    }

    // TODO: Make this functionality much more robust and secure
    public int ExternalDepositRequest(string toAccountId, float amount)
    {
        Account aToDepositIn = accounts[toAccountId];
        if (aToDepositIn == null)
            return (int)ErrorCode.Transfer;
        aToDepositIn.Deposit(amount);
        return (int)ErrorCode.Success;
    }

    // Return Values
    // 0 = Success
    // 1 = Deposit Failure
    // 2 = Withdrawl Failure
    // 3 = Transfer Failure
    public int Transfer(string fromAccountId, string toAccountId, Bank externalBank, float amount)
    {
        int withdrawlSuccess = -1;
        int depositSuccess = -1;

        if (!accounts.ContainsKey(fromAccountId))
        {
            Utility.PrintError(3, "Invalid 'From' Account Id", "No account exists at " + name + " with id [" + fromAccountId + "].");
            return (int)ErrorCode.Transfer;
        }

        if (!accounts.ContainsKey(toAccountId))
        {
            if (externalBank == null)
            {
                Utility.PrintError(3, "Invalid 'To' Account Id", "No account exists at " + name + " with id [" + toAccountId + "].");
                return (int)ErrorCode.Transfer;
            }

            int success = externalBank.ExternalDepositRequest(toAccountId, amount);
            if (success != 0)
            {
                Utility.PrintError(success, "External Transfer Request Failed", "N/A");
                return success;
            }
        }

        float preTransactionBalance_From = accounts[fromAccountId].balance;
        withdrawlSuccess = accounts[fromAccountId].Withdrawl(amount);

        if (withdrawlSuccess != 0)
        {
            // Error with withdrawl, nothing should have been withdrawl
            // Check balance is the same
            if (preTransactionBalance_From != accounts[fromAccountId].balance)
            {
                // Balance changed, need to revert
            }
            return (int)ErrorCode.Withdrawl;
        }

        float preTransactionBalance_To = accounts[toAccountId].balance;
        depositSuccess = accounts[toAccountId].Deposit(amount);
        if (depositSuccess != 0)
        {
            // Error with deposit, nothing should have been deposited
            // Check balance is the same
            if (preTransactionBalance_To != accounts[toAccountId].balance)
            {
                // Balance changed, need to revert
            }

            return (int)ErrorCode.Deposit;
        }

        return (int)ErrorCode.Success;
    }
}