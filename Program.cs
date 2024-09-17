using System;
using System.Collections.Generic;

namespace ATMManagement
{
    public delegate void TransactionEventHandler(string message);


    public class Account
    {
        public string AccountHolderName { get; private set; }
        public string PhoneNumber { get; private set; }
        public decimal Balance { get; private set; }


        public List<string> TransactionHistory { get; private set; }


        public event TransactionEventHandler OnTransaction;

        public Account(string accountHolderName, string phoneNumber, decimal initialBalance)
        {
            AccountHolderName = accountHolderName;
            PhoneNumber = phoneNumber;
            Balance = initialBalance;
            TransactionHistory = new List<string>();
        }

        // Phương thức rút tiền
        public void Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine("Số dư không đủ.");
                return;
            }

            Balance -= amount;
            string transaction = $"Đã rút {amount:C}. Số dư hiện tại: {Balance:C}.";
            TransactionHistory.Add(transaction);
            OnTransaction?.Invoke(transaction);
        }

        // Phương thức chuyển tiền
        public void Transfer(Account targetAccount, decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine("Số dư không đủ.");
                return;
            }

            Balance -= amount;
            targetAccount.Balance += amount;
            string transaction = $"Đã chuyển {amount:C} đến tài khoản {targetAccount.AccountHolderName}. Số dư hiện tại: {Balance:C}.";
            TransactionHistory.Add(transaction);
            OnTransaction?.Invoke(transaction);
        }

        // Phương thức kiểm tra đăng nhập
        public bool Login(string name, string phone)
        {
            return AccountHolderName == name && PhoneNumber == phone;
        }

        // Phương thức xem lịch sử giao dịch
        public void ViewTransactionHistory()
        {
            Console.WriteLine($"Lịch sử giao dịch của {AccountHolderName}:");
            foreach (string transaction in TransactionHistory)
            {
                Console.WriteLine(transaction);
            }
        }
    }

    public class ATM
    {
        public void ProcessWithdrawal(Account account, decimal amount)
        {
            account.Withdraw(amount);
        }

        public void ProcessTransfer(Account sourceAccount, Account targetAccount, decimal amount)
        {
            sourceAccount.Transfer(targetAccount, amount);
        }

        // Phương thức gửi tin nhắn
        public void SendSMS(string message)
        {
            Console.WriteLine($"SMS: {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;


            Account account1 = new Account("nguyen quan", "09042005", 5000000);
            Account account2 = new Account("huy vu", "09041234", 5000000);
            Account account3 = new Account("trong nguyen", "09045678", 5000000);
            Account account4 = new Account("lan thanh", "09048989", 5000000);


            ATM atm = new ATM();


            account1.OnTransaction += atm.SendSMS;
            account2.OnTransaction += atm.SendSMS;


            Account loggedInAccount = null;

            while (loggedInAccount == null)
            {
                Console.WriteLine("Vui lòng đăng nhập:");
                Console.Write("Nhập tên tài khoản: ");
                string inputName = Console.ReadLine();
                Console.Write("Nhập số điện thoại: ");
                string inputPhone = Console.ReadLine();

                if (account1.Login(inputName, inputPhone))
                {
                    loggedInAccount = account1;
                    Console.WriteLine("Đăng nhập thành công! Xin chào, " + account1.AccountHolderName);
                }
                else if (account2.Login(inputName, inputPhone))
                {
                    loggedInAccount = account2;
                    Console.WriteLine("Đăng nhập thành công! Xin chào, " + account2.AccountHolderName);
                }
                else
                {
                    Console.WriteLine("Tên tài khoản hoặc số điện thoại không đúng. Vui lòng thử lại.");
                }
            }


            string userChoice;
            do
            {

                Console.WriteLine("Chọn giao dịch:");
                Console.WriteLine("1. Rút tiền");
                Console.WriteLine("2. Chuyển tiền");
                Console.WriteLine("3. Xem lịch sử giao dịch");
                Console.WriteLine("4. Xem số dư tài khoản");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Rút tiền
                        Console.Write("Nhập số tiền muốn rút: ");
                        decimal withdrawAmount;
                        if (decimal.TryParse(Console.ReadLine(), out withdrawAmount))
                        {
                            atm.ProcessWithdrawal(loggedInAccount, withdrawAmount);
                        }
                        else
                        {
                            Console.WriteLine("Số tiền không hợp lệ.");
                        }
                        break;

                    case "2":
                        // Chuyển tiền
                        Console.Write("Nhập tên tài khoản người nhận: ");
                        string receiverName = Console.ReadLine();

                        // Tìm tài khoản người nhận theo tên
                        Account targetAccount = FindAccountByName(receiverName, account1, account2);

                        if (targetAccount != null)
                        {
                            Console.Write("Nhập số tiền muốn chuyển: ");
                            decimal transferAmount;
                            if (decimal.TryParse(Console.ReadLine(), out transferAmount))
                            {
                                atm.ProcessTransfer(loggedInAccount, targetAccount, transferAmount);
                            }
                            else
                            {
                                Console.WriteLine("Số tiền không hợp lệ.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Tài khoản người nhận không tồn tại.");
                        }
                        break;

                    case "3":
                        // Xem lịch sử giao dịch
                        loggedInAccount.ViewTransactionHistory();
                        break;

                    case "4":
                        // Xem số dư tài khoản
                        Console.WriteLine($"Số dư hiện tại của bạn là: {loggedInAccount.Balance:C}");
                        break;

                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }


                Console.WriteLine("Bạn có muốn tiếp tục giao dịch không? (y/n)");
                userChoice = Console.ReadLine().ToLower();
            }
            while (userChoice == "y");

            Console.WriteLine("Cảm ơn bạn đã sử dụng dịch vụ!");
        }


        static Account FindAccountByName(string name, Account account1, Account account2)
        {
            if (account1.AccountHolderName == name)
            {
                return account1;
            }
            else if (account2.AccountHolderName == name)
            {
                return account2;
            }
            return null;
        }
    }


}