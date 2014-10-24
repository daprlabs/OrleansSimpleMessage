// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Console client for the SMS system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmsConsoleClient
{
    using System;
    using System.Threading.Tasks;

    using Orleans;

    using SimpleMessage.GrainInterfaces;

    /// <summary>
    /// Console client for the SMS system.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public static void Main(string[] args)
        {
            
            // Initialize Orleans.
            OrleansClient.Initialize();

            // Get a reference to the user's grain.

            switch (args[0])
            {
                case "get":
                    GetMessages(args).Wait();
                    break;
                case "send":
                    SendMessage(args).Wait();
                    break;

                case "watch":
                    WatchForMessages(args).Wait();
                    break;
            }
        }

        private static async Task GetMessages(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(Usage());
                return;
            }

            var id = args[1];

            // Get a reference to the user's grain.
            var user = GrainFactory.GetGrain<ISmsGrain>(id);

            // Get the messages from the grain.
            var messages = await user.GetMessages();

            // Display the results.
            Console.WriteLine("Got {0} messages for {1}:", messages.Count, id);
            var i = 0;
            foreach (var msg in messages)
            {
                Console.WriteLine("  [{0}] {1}", i++, msg);
            }
        }

        private static async Task SendMessage(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(Usage());
                return;
            }

            var id = args[1];
            var message = args[2];

            // Get a reference to the user's grain.
            var user = GrainFactory.GetGrain<ISmsGrain>(id);

            // Send the message
            await user.NewMessage(message);
            Console.WriteLine("Sent \"{0}\" to {1}", message, id);
        }

        private static async Task WatchForMessages(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(Usage());
                return;
            }

            var id = args[1];

            // Register an SMS observer, providing the concrete implementation.
            // SmsWatcherFactory is created by Orleans through code generation.
            var observer = await SmsObserverFactory.CreateObjectReference(new SmsObserver(id));

            // Get a reference to the user's grain.
            var user = GrainFactory.GetGrain<ISmsGrain>(id);

            // Register the watcher with the grain.
            await user.WatchForMessages(observer);

            // Block until the user presses a key.
            Console.WriteLine("Waiting for messages. Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Returns the program help message.
        /// </summary>
        /// <returns>
        /// The program help message.
        /// </returns>
        private static string Usage()
        {
            const string HelpTemplate =
                "Usage: \n"
                + "\t{0} get <id>\t\t- Get messages for id.\n"
                + "\t{0} send <id> <message>\t- Send <message> to <id>.\n"
                + "\t{0} watch <id>\t\t- Watch for new messages to <id>.\n";
            return string.Format(HelpTemplate, AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
