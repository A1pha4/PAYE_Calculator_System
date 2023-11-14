using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

class Program
{
    class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
    }

    static List<User> users = new List<User>();

    static decimal CalculatePAYE(decimal income, decimal allowances, decimal deductions)
    {
        decimal[] brackets = { 24000, 60000, decimal.MaxValue };
        decimal[] rates = { 0.10m, 0.20m, 0.30m };

        decimal taxableIncome = income - allowances - deductions;

        decimal tax = 0;

        for (int i = 0; i < brackets.Length; i++)
        {
            if (taxableIncome <= brackets[i])
            {
                tax += taxableIncome * rates[i];
                break;
            }
            else
            {
                tax += brackets[i] * rates[i];
                taxableIncome -= brackets[i];
            }
        }

        return tax;
    }

    static void GenerateTaxCertificate(string username, decimal income, decimal allowances, decimal deductions, decimal payeTax)
    {
        string certificateContent = $"Tax Certificate for {username}\n\n";
        certificateContent += $"Total Income: {income} KES\n";
        certificateContent += $"Total Allowances: {allowances} KES\n";
        certificateContent += $"Total Deductions: {deductions} KES\n";
        certificateContent += $"PAYE Tax: {payeTax} KES\n";
        certificateContent += "Thank you for using our services.\n";
        certificateContent += "Remember to file your taxes on time for efficient Government services\n";
        certificateContent += "\t\t\t\t\tKUMBUKA!! Ushuru Wetu kwa Ustawi wa Kenya  Yetu\t\t\t\t\t\n";

        string fileName = $"{username}_TaxCertificate_2023.txt";

        try
        {
            System.IO.File.WriteAllText(fileName, certificateContent);
            Console.WriteLine(certificateContent);
            Console.WriteLine($"Tax certificate generated successfully as {fileName}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error generating tax certificate: {e.Message}");
        }
    }
    static void ShowWelcomeMessage()
    {
        Console.WriteLine("\t\t\t\t\t****************************************************");
        Console.WriteLine("\t\t\t\t\t*                                                  *");
        Console.WriteLine("\t\t\t\t\t*             WELCOME TO THE ALPHA PAYE            *");
        Console.WriteLine("\t\t\t\t\t*                CALCULATION SYSTEM                *");
        Console.WriteLine("\t\t\t\t\t*                                                  *");
        Console.WriteLine("\t\t\t\t\t****************************************************");
        Console.WriteLine();
        Console.WriteLine();
    }

    static void CreateAccount()
    {
        Console.Clear();
        ShowWelcomeMessage();
        Console.WriteLine("Account Creation");

        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return;
        }

        Console.Write("Enter password: ");
        string password = GetHiddenPassword();

        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider()) //RandomNumberGenerator
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);

        using (var sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
            string hashedPassword = Convert.ToBase64String(hashedBytes);

            User newUser = new User { Username = username, PasswordHash = hashedPassword, Salt = salt };
            users.Add(newUser);

            Console.WriteLine("Account created successfully. Press any key to continue.");
            Console.ReadKey();
        }
    }

   static void Login()
    {
        Console.Clear();
        ShowWelcomeMessage();
        Console.WriteLine("Account Login");

        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = GetHiddenPassword();

        User user = users.Find(u => u.Username == username);

        if (user != null)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + user.Salt);
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = Convert.ToBase64String(hashedBytes);

                if (hashedPassword == user.PasswordHash)
                {
                    ShowTaxCalculationScreen(user);
                }
                else
                {
                    Console.WriteLine("Invalid username or password. Press any key to continue.");
                    Console.ReadKey();
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid username or password. Press any key to continue.");
            Console.ReadKey();
        }
    } 

    static string GetHiddenPassword()
    {
        StringBuilder password = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        return password.ToString();
    }

    static void ShowAccountCreationScreen()
    {
        while (true)
        {
            Console.Clear();
            ShowWelcomeMessage();
            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateAccount();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ShowTaxCalculationScreen(User user)
    {
        while (true)
        {
            Console.Clear();
            ShowWelcomeMessage();
            Console.WriteLine($"Welcome, {user.Username}!");
            Console.WriteLine("Tax Calculation Menu");
            Console.WriteLine("1. Calculate PAYE");
            Console.WriteLine("2. Generate Tax Certificate");
            Console.WriteLine("3. Logout");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    try
                    {
                        Console.WriteLine("Enter your monthly income (KES): ");
                        decimal income = Convert.ToDecimal(Console.ReadLine());

                        Console.WriteLine("Enter your allowances (KES): ");
                        decimal allowances = Convert.ToDecimal(Console.ReadLine());

                        Console.WriteLine("Enter your deductions (KES): ");
                        decimal deductions = Convert.ToDecimal(Console.ReadLine());

                        decimal payeTax = CalculatePAYE(income, allowances, deductions);

                        Console.WriteLine($"Your PAYE Tax is: {payeTax} KES. Press any key to continue.");
                        Console.ReadKey();
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric value. Press any key to continue.");
                        Console.ReadKey();
                    }
                    break;
                case "2":
                    try
                    {
                        Console.WriteLine("Enter your monthly income (KES): ");
                        decimal income = Convert.ToDecimal(Console.ReadLine());

                        Console.WriteLine("Enter your allowances (KES): ");
                        decimal allowances = Convert.ToDecimal(Console.ReadLine());

                        Console.WriteLine("Enter your deductions (KES): ");
                        decimal deductions = Convert.ToDecimal(Console.ReadLine());

                        decimal payeTax = CalculatePAYE(income, allowances, deductions);

                        GenerateTaxCertificate(user.Username, income, allowances, deductions, payeTax);
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric value. Press any key to continue.");
                        Console.ReadKey();
                    }
                    break;
                case "3":
                    ShowAccountCreationScreen();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void Main()
    {
        ShowAccountCreationScreen();
    }
}