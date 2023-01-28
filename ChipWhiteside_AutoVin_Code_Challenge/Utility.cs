
using System.Diagnostics;

public static class Utility
{
    public enum ErrorCode
    {
        Success = 0,
        Deposit = 1,
        Withdrawl = 2,
        Transfer = 3
    }

    public static void PrintAccountDetails(Account account)
    {
        Debug.Print("----------------------------------------------------------------");
        Debug.Print("AccountId: {0}", account.accountId);
        Debug.Print("Owner: {0}", account.owner.name);
        Debug.Print("Balance: ${0}", account.balance);
        Debug.Print("Withdrawl Limit?: {0}", account.withdrawlLimit);
        Debug.Print("----------------------------------------------------------------");
    }

    public static void PrintError(int errorCode, string title, string message)
    {
        Debug.Print("************************************************************");
        Debug.Print("[{0}] {1} - {2}", errorCode, title, message);
        Debug.Print("************************************************************");
    }
}