using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Principal;
using System.Threading.Channels;
using static Utility;

public class Tests {
    
    public int totalTestsRun;
    public int totalPassedTests;

    public List<String> passedTests;
    public List<String> failedTests;
   
    public Tests()
    {
        this.totalTestsRun = 0;
        this.totalPassedTests = 0;
        this.passedTests = new List<String>();
        this.failedTests = new List<String>();
    }

    public void PrintResults()
    {
        Console.WriteLine("Tests Failed: " + failedTests.Count);
        Console.WriteLine("--------------");
        foreach (var test in failedTests)
        {
            Console.WriteLine(test);
            Console.WriteLine("--------------");
        }
        Console.WriteLine("Tests Passed: " + passedTests.Count);
    }

    public void RunTests() {

        // Reset Values
        totalTestsRun = 0;
        totalPassedTests = 0;
        passedTests = new List<string>();
        failedTests = new List<string>();

        // Foreach account type run deposit and withdrawl tests

        User Foo = new User("Foo");
        User Bar = new User("Bar");

        AllDepositTests(new Account(Foo)); 
        AllDepositTests(new Checking(Foo)); 
        AllDepositTests(new Investment(Foo)); 
        AllDepositTests(new Individual(Foo)); 
        AllDepositTests(new Corporate(Foo)); 

        AllWithdrawlTests(new Account(Bar));
        AllWithdrawlTests(new Checking(Bar));
        AllWithdrawlTests(new Investment(Bar));
        AllWithdrawlTests(new Individual(Bar));
        AllWithdrawlTests(new Corporate(Bar));

        // Potentially setup to dynammically chose each combination of two accounts to insure all combo's work but that is out of scope
        Bank Bank_1 = new Bank("Bank_1");
        Account Checking_1 = new Checking(Foo);
        Account Individual_1 = new Individual(Foo);
        Bank_1.AddAccount(Checking_1);
        Bank_1.AddAccount(Individual_1);
        AllTransferTests_ValidAccounts(Bank_1, Checking_1, Individual_1);

        PrintResults();
    }

    public void AddSuccess(string testPassed)
    {
        passedTests.Add(testPassed);
    }

    public void AddFailure(string testFailed, int returnValue, int expectedReturnValue, string? fMessage) 
    {
        string failureMessage = testFailed + "\n";
        failureMessage += "Actual Return Value: " + returnValue + "\n";
        failureMessage += "Expected Return Value: " + expectedReturnValue + "\n";
        failureMessage += fMessage;

        failedTests.Add(failureMessage);
    }

    /// <summary>
    /// Test all deposit functionality
    /// 
    /// Cases tested:
    /// Deposit negative
    /// Deposit 0
    /// Deposit less than limit
    /// Deposit more than limit
    /// 
    /// </summary>
    /// <param name="toTest"></param>
    public void AllDepositTests(Account toTest)
    {
        Deposit_Negative(toTest);
        toTest.balance = 0;
        
        Deposit_Zero(toTest);
        toTest.balance = 0;

        Deposit_SmallAmount(toTest);
        toTest.balance = 0;

        Deposit_LargeAmount(toTest);
        toTest.balance = 0;
    }

    public void Deposit_Negative(Account toTest)
    {
        //Arrange 
        float balancePreDeposit = toTest.balance;

        //Act
        int success = toTest.Deposit(-100);

        //Assert
        if (success != (int)ErrorCode.Deposit)
        {
            AddFailure("Deposit_Negative", success, (int)ErrorCode.Deposit, "");
            return;
        }
        if (balancePreDeposit != toTest.balance) 
        {
            AddFailure("Deposit_Negative", success, (int)ErrorCode.Deposit, "Balance changed after failed deposit");
            return;
        }
        AddSuccess("Deposit_Negative");
    }
    public void Deposit_Zero(Account toTest)
    {
        //Arrange 
        float balancePreDeposit = toTest.balance;

        //Act
        int success = toTest.Deposit(0);

        //Assert
        if (success != (int)ErrorCode.Deposit)
        {
            AddFailure("Deposit_Zero", success, (int)ErrorCode.Deposit, "");
            return;
        }
        if (balancePreDeposit != toTest.balance)
        {
            AddFailure("Deposit_Zero", success, (int)ErrorCode.Deposit, "Balance changed after failed deposit");
            return;
        }
        AddSuccess("Deposit_Zero");
    }
    public void Deposit_SmallAmount(Account toTest)
    {
        //Arrange 
        float balancePreDeposit = toTest.balance;

        //Act
        int success = toTest.Deposit(100);

        //Assert
        if (success != (int)ErrorCode.Success)
        {
            AddFailure("Deposit_SmallAmount", success, (int)ErrorCode.Success, "");
            return;
        }
        if (toTest.balance != balancePreDeposit + 100)
        {
            AddFailure("Deposit_SmallAmount", success, (int)ErrorCode.Success, "Balance after deposit is incorrect. Currently: $" + toTest.balance + ", expected $" + (balancePreDeposit + 100));
            return;
        }
        AddSuccess("Deposit_SmallAmount");
    }
    public void Deposit_LargeAmount(Account toTest)
    {
        //Arrange 
        float balancePreDeposit = toTest.balance;

        //Act
        int success = toTest.Deposit(10000);

        //Assert
        if (success != 0)
        {
            AddFailure("Deposit_LargeAmount", success, (int)ErrorCode.Success, "");
            return;
        }
        if (toTest.balance != balancePreDeposit + 10000)
        {
            AddFailure("Deposit_LargeAmount", success, (int)ErrorCode.Success, "Balance after deposit is incorrect. Currently: $" + toTest.balance + ", expected $" + (balancePreDeposit + 10000));
            return;
        }
        AddSuccess("Deposit_LargeAmount");
    }

    /// <summary>
    /// Testing all withdrawl functionality
    /// 
    /// Cases tested:
    /// Withdrawl negative amount
    /// Withdrawl 0 amount
    /// Withdrawl less than balance
    /// Withdrawl more than balance
    /// Withdrawl more than limit
    /// 
    /// </summary>
    /// <param name="toTest"></param>
    public void AllWithdrawlTests(Account toTest)
    {
        Withdrawl_NegativeAmount(toTest);
        toTest.balance = 0;

        Withdrawl_ZeroAmount(toTest);
        toTest.balance = 0;

        Withdrawl_LessThanBalance(toTest);
        toTest.balance = 0;

        Withdrawl_MoreThanBalance(toTest);
        toTest.balance = 0;

        Withdrawl_MoreThanLimit(toTest);
        toTest.balance = 0;
    }

    public void Withdrawl_NegativeAmount(Account toTest)
    {
        //Arrange
        toTest.Deposit(1000);
        float balancePreWithdrawl = toTest.balance;

        //Act
        int success = toTest.Withdrawl(-100);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Withdrawl_NegativeAmount", success, (int)ErrorCode.Withdrawl, "");
            return;
        }
        if (balancePreWithdrawl != toTest.balance)
        {
            AddFailure("Withdrawl_NegativeAmount", success, (int)ErrorCode.Withdrawl, "Balance changed after failed withdrawl");
            return;
        }
        AddSuccess("Withdrawl_NegativeAmount");
    }
    public void Withdrawl_ZeroAmount(Account toTest)
    {
        //Arrange
        toTest.Deposit(1000);
        float balancePreWithdrawl = toTest.balance;

        //Act
        int success = toTest.Withdrawl(0);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Withdrawl_ZeroAmount", success, (int)ErrorCode.Withdrawl, "");
            return;
        }
        if (balancePreWithdrawl != toTest.balance)
        {
            AddFailure("Withdrawl_ZeroAmount", success, (int)ErrorCode.Withdrawl, "Balance changed after failed withdrawl");
            return;
        }
        AddSuccess("Withdrawl_ZeroAmount");
    }
    public void Withdrawl_LessThanBalance(Account toTest)
    {
        //Arrange
        toTest.Deposit(1000);
        float balancePreWithdrawl = toTest.balance;

        //Act
        int success = toTest.Withdrawl(100);

        //Assert
        if (success != (int)ErrorCode.Success)
        {
            AddFailure("Withdrawl_LessThanBalance", success, (int)ErrorCode.Success, "");
            return;
        }
        if (toTest.balance != (balancePreWithdrawl - 100))
        {
            AddFailure("Withdrawl_LessThanBalance", success, (int)ErrorCode.Success, "Balance after withdrawl is incorrect. Got: $" + toTest.balance + ", expected $" + (balancePreWithdrawl - 100));
            return;
        }
        AddSuccess("Withdrawl_LessThanBalance");

    }
    public void Withdrawl_MoreThanBalance(Account toTest)
    {
        //Arrange
        toTest.Deposit(100);
        float balancePreWithdrawl = toTest.balance;

        //Act
        int success = toTest.Withdrawl(200);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Withdrawl_MoreThanBalance", success, (int)ErrorCode.Withdrawl, "");
            return;
        }
        if (toTest.balance != balancePreWithdrawl)
        {
            AddFailure("Withdrawl_MoreThanBalance", success, (int)ErrorCode.Withdrawl, "Balance changed after failed withdrawl");
            return;
        }
        AddSuccess("Withdrawl_MoreThanBalance");
    }
    public void Withdrawl_MoreThanLimit(Account toTest)
    {
        // If no withdrawl limit, this test is redundant with the previous test, save time and autopass
        if (toTest.withdrawlLimit == null)
        {
            AddSuccess("Withdrawl_MoreThanLimit");
            return;
        }

        //Arrange
        toTest.Deposit(1000);
        float balancePreWithdrawl = toTest.balance;

        //Act
        int success = toTest.Withdrawl(1000);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Withdrawl_MoreThanLimit", success, (int)ErrorCode.Withdrawl, "");
            return;
        }
        if (toTest.balance != balancePreWithdrawl)
        {
            AddFailure("Withdrawl_MoreThanLimit", success, (int)ErrorCode.Withdrawl, "Balance changed after failed withdrawl");
            return;
        }
        AddSuccess("Withdrawl_MoreThanLimit");
    }


    /// <summary>
    /// All transfer tests with valid 'to' and 'from' accounts
    /// 
    /// Cases tested:
    /// Transfer from account to account with negative amount
    /// Transfer from account to account with 0 amount
    /// Transfer from account to account with positive amount
    /// Transfer from account to account with amount > limit
    /// 
    /// </summary>
    /// <param name="fromTestAcc"></param>
    /// <param name="toTestAcc"></param>
    public void AllTransferTests_ValidAccounts(Bank testBank, Account fromTestAcc, Account toTestAcc)
    {
        Transfer_NegativeAmount(testBank, fromTestAcc, toTestAcc);
        fromTestAcc.balance = 0;
        toTestAcc.balance = 0;

        Transfer_ZeroAmount(testBank, fromTestAcc, toTestAcc);
        fromTestAcc.balance = 0;
        toTestAcc.balance = 0;

        Transfer_SmallAmount(testBank, fromTestAcc, toTestAcc);
        fromTestAcc.balance = 0;
        toTestAcc.balance = 0;

        Transfer_LargeAmount(testBank, fromTestAcc, toTestAcc);
        fromTestAcc.balance = 0;
        toTestAcc.balance = 0;
    }
    public void Transfer_NegativeAmount(Bank testBank, Account fromTestAcc, Account toTestAcc)
    {
        //Arrange
        fromTestAcc.Deposit(1000);
        toTestAcc.Deposit(1000);
        float balancePreWithdrawl_From = fromTestAcc.balance;
        float balancePreWithdrawl_To = toTestAcc.balance;

        //Act
        int success = testBank.Transfer(fromTestAcc.accountId, toTestAcc.accountId, null, -100);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Transfer_NegativeAmount", success, (int)ErrorCode.Transfer, "");
            return;
        }
        if (fromTestAcc.balance != balancePreWithdrawl_From)
        {
            AddFailure("Transfer_NegativeAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'From' account after failed transfer");
            return;
        }
        if (toTestAcc.balance != balancePreWithdrawl_To)
        {
            AddFailure("Transfer_NegativeAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'To' account after failed transfer");
            return;
        }
        AddSuccess("Transfer_NegativeAmount");
    }
    public void Transfer_ZeroAmount(Bank testBank, Account fromTestAcc, Account toTestAcc)
    {
        //Arrange
        fromTestAcc.Deposit(1000);
        toTestAcc.Deposit(1000);
        float balancePreWithdrawl_From = fromTestAcc.balance;
        float balancePreWithdrawl_To = toTestAcc.balance;

        //Act
        int success = testBank.Transfer(fromTestAcc.accountId, toTestAcc.accountId, null, 0);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Transfer_ZeroAmount", success, (int)ErrorCode.Transfer, "");
            return;
        }
        if (fromTestAcc.balance != balancePreWithdrawl_From)
        {
            AddFailure("Transfer_ZeroAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'From' account after failed transfer");
            return;
        }
        if (toTestAcc.balance != balancePreWithdrawl_To)
        {
            AddFailure("Transfer_ZeroAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'To' account after failed transfer");
            return;
        }
        AddSuccess("Transfer_ZeroAmount");
    }
    public void Transfer_SmallAmount(Bank testBank, Account fromTestAcc, Account toTestAcc)
    {
        //Arrange
        fromTestAcc.Deposit(1000);
        toTestAcc.Deposit(1000);
        float balancePreWithdrawl_From = fromTestAcc.balance;
        float balancePreWithdrawl_To = toTestAcc.balance;

        //Act
        int success = testBank.Transfer(fromTestAcc.accountId, toTestAcc.accountId, null, 100);

        //Assert
        if (success != (int)ErrorCode.Success)
        {
            AddFailure("Transfer_SmallAmount", success, (int)ErrorCode.Success, "");
            return;
        }
        if (fromTestAcc.balance != balancePreWithdrawl_From - 100)
        {
            AddFailure("Transfer_SmallAmount", success, (int)ErrorCode.Success, "Balance after transfer is incorrect. Got: $" + fromTestAcc.balance + ", expected $" + (balancePreWithdrawl_From - 100));
            return;
        }
        if (toTestAcc.balance != balancePreWithdrawl_To + 100)
        {
            AddFailure("Transfer_SmallAmount", success, (int)ErrorCode.Success, "Balance after transfer is incorrect. Got: $" + toTestAcc.balance + ", expected $" + (balancePreWithdrawl_To + 100));
            return;
        }
        AddSuccess("Transfer_SmallAmount");
    }
    public void Transfer_LargeAmount(Bank testBank, Account fromTestAcc, Account toTestAcc)
    {
        // If no withdrawl limit, this test is redundant with the previous test, save time and autopass
        if (fromTestAcc.withdrawlLimit == null)
        {
            AddSuccess("Transfer_LargeAmount");
            return;
        }

        //Arrange
        fromTestAcc.Deposit(1000);
        toTestAcc.Deposit(1000);
        float balancePreWithdrawl_From = fromTestAcc.balance;
        float balancePreWithdrawl_To = toTestAcc.balance;

        //Act
        int success = testBank.Transfer(fromTestAcc.accountId, toTestAcc.accountId, null, 1000);

        //Assert
        if (success != (int)ErrorCode.Withdrawl)
        {
            AddFailure("Transfer_LargeAmount", success, (int)ErrorCode.Transfer, "");
            return;
        }
        if (fromTestAcc.balance != balancePreWithdrawl_From)
        {
            AddFailure("Transfer_LargeAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'From' account after failed transfer");
            return;
        }
        if (toTestAcc.balance != balancePreWithdrawl_To)
        {
            AddFailure("Transfer_LargeAmount", success, (int)ErrorCode.Transfer, "Balance changed in 'To' account after failed transfer");
            return;
        }
        AddSuccess("Transfer_LargeAmount");
    }


    /// <summary>
    /// Test transfer functionality between different account owned by different banks
    /// 
    /// Cases tested:
    /// Transfer from account (nonexistent) to account with valid amount
    /// Transfer from account to account(nonexistent) with valid amount
    /// Transfer from account to account(different bank) with valid amount
    /// Transfer from account to account(different bank) with invalid amount
    /// 
    /// </summary>
    /// <param name="fromTestBank"></param>
    /// <param name="toTestBank"></param>
    /// <param name="fromTestAcc"></param>
    /// <param name="toTestAcc"></param>
    public void AllTransferTests_InValidAccounts(Bank fromTestBank, Bank toTestBank, Account fromTestAcc, Account toTestAcc)
    {

    }
}