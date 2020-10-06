using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace RockAndPaper
{
    class Menu
    {
        public void nemuView(string[] args)
        {
            int count = 0;
            Console.WriteLine("Available moves:");
            foreach (string item in args)
            {
                count++;
                Console.WriteLine($"#{count}: {item}");
            }
            Console.WriteLine($"#0: Exit");
        }
    }

    class Program
    {
        public static string HMACkey { get; private set; }
        public static string HMAC { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length % 2 == 0)
            {
                Console.WriteLine("Odd userChoice of arguments required");
                Environment.Exit(0);
            }
            if (args.Length < 3)
            {
                Console.WriteLine("Need more 3");
                Environment.Exit(0);
            }
            if (args.Length != args.Distinct().Count())
            {
                Console.WriteLine("There are the same arguments");
                Environment.Exit(0);
            }

            var rng = RandomNumberGenerator.Create();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);
            HMACkey = Convert.ToBase64String(buff);

            int middle = (args.Length / 2) + 1;
            string computerStroke = args[new Random().Next(0, args.Length)];

            var sha = new HMACSHA256(Encoding.UTF8.GetBytes(HMACkey));
            var bytes = Encoding.UTF8.GetBytes(computerStroke);
            var hash = sha.ComputeHash(bytes);
            HMAC = string.Concat(Array.ConvertAll(hash, b => b.ToString("X2"))).ToLower();
            Console.WriteLine("HMAC: " + HMAC);

            Menu n = new Menu();
            int playerNumber = -1;
            string userChoice = "null";

            while (playerNumber > args.Length || playerNumber < 0) 
            { 
                n.nemuView(args);
                userChoice = Console.ReadLine();
                playerNumber = int.Parse(userChoice);
                if (playerNumber == 0)
                {
                    Environment.Exit(0);
                }
                if (playerNumber > args.Length || playerNumber < 0)
                {
                    Console.WriteLine("Incorrect meaning");
                }
            }
            Console.WriteLine($"Select option: {userChoice}");

            string userStroke = args[playerNumber - 1];
            Console.WriteLine($"You choosed: {userStroke}");
            Console.WriteLine($"The computer chose: {computerStroke}");

            if (playerNumber > middle)
            {
                while (args[middle - 1] != userStroke)
                {
                    int numIndex = Array.IndexOf(args, args[0]);
                    string firstElement = args[0];
                    args = args.Where((val, idx) => idx != numIndex).ToArray();
                    args = args.Concat(new string[] { firstElement }).ToArray();
                }
            }
            if (playerNumber < middle)
            {
                while (args[middle - 1] != userStroke)
                {
                    int numIndex = Array.IndexOf(args, args[args.Length - 1]);
                    string lastElement = args[args.Length - 1];
                    args = args.Where((val, idx) => idx != numIndex).ToArray();
                    string[] tmpArr = new string[args.Length + 1];
                    tmpArr[0] = lastElement;
                    for (int i = 0; i < args.Length; i++)
                    {
                        tmpArr[i + 1] = args[i];
                    }
                    args = tmpArr;
                }
            }

            if (userStroke == computerStroke)
            {
                Console.WriteLine($"Draw");
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (computerStroke == args[i])
                    {
                        if (i < middle)
                        {
                            Console.WriteLine($"User win");
                        }
                        else
                        {
                            Console.WriteLine($"Computer win");
                        }

                    }

                }
            } 
            Console.WriteLine("HMACkey: " + HMACkey);
        }
    }
}




