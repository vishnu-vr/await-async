/*
 * AUTHORS : Niyas and Vishnu
 */


using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var prgm = new Program();
            prgm.Story_1();
            prgm.Story_2();

            Console.ReadLine();
        }

        string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        async Task AwaitingComplexFuncWithTask(string caller)
        {
            Console.WriteLine(caller + ": Code Before Awaiting Task");
            await Task.Run(() =>
            {
                WasteTime(caller+": AwaitingComplexFuncWithTask()");
            });
            // ... waiting for task
            Console.WriteLine(caller + ": Code After Awaiting Task");
        }

        async Task AwaitingComplexFuncWithoutTask(string caller)
        {
            Console.WriteLine(caller + ": Code Before Awaiting Function");
            await WasteTime(caller + ": AwaitingComplexFuncWithoutTask()");
            Console.WriteLine(caller + ": Code After Awaiting Function");
        }

        // Some heavy calculation to waste time
        // increase the zeros to increase complexity exponentially
        async Task WasteTime(string caller)
        {
            Console.WriteLine(caller + ": Started wasting time");

            int num = 0;
            var hash = ComputeSha256Hash(Convert.ToString(num));
            while (!Regex.Match(hash, "^000000").Success)
            {
                num++;
                hash = ComputeSha256Hash(Convert.ToString(num));
            }

            Console.WriteLine(caller + ": Finished wasting time");
        }

        // Main Thread (MT) starts here
        void Story_1()
        {
            Console.WriteLine("Story_1() started");

            // STORY 1
            // MT will go inside "AwaitingComplexFuncWithTask()" and executes
            // everything until it sees an await line.
            // Now since it is awaiting a "Task", a new thread will be assigned to do
            // the rest in AwaitingComplexFuncWithTask().
            // Now MT can return and execute the rest in this
            // function.
            AwaitingComplexFuncWithTask("Story_1()");
            Console.WriteLine("Story_1(): I am below AwaitingComplexFuncWithTask()");
        }

        void Story_2()
        {
            Console.WriteLine("Story_2() started");

            // STORY 2
            // MT will go inside "AwaitingComplexFuncWithoutTask()" and executes
            // everything until it sees an await line.
            // But now since it is awaiting for a function
            // MT will go inside until it sees an await line again in it.
            // But unfortunately our MT will not come across any line awaiting a "Task" (C# TASK, NOT A FUNC)
            // Now MT will have to execute the rest of the task in that function since no
            // new threads could be created.
            AwaitingComplexFuncWithoutTask("Story_2()");
            Console.WriteLine("Story_2(): I am below AwaitingComplexFuncWithoutTask()");
        }

        /* 
         * MORAL OF STORY
         * ==============
         * AWAIT AND ASYNC KEYWORDS WILL NOT MAKE ANY SENSE IF IT'S
         * USED WITHOUT ANY "TASKS". (WHICH IS AN IMPLEMENTATION OF IAsyncResult)
         * 
         * FYI: TASK CAN BE FOUND IN NETWORK CALLS, I/O OPS ETC...
        */
    }
}
