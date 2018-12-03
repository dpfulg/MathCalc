using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Metrics;

namespace MathCalcBurner
{
    class Program
    {
        // private static Metrics.Meter histogram = Metric.Meter("Responsetime", Unit.None, TimeUnit.Milliseconds);

        private static readonly Metrics.Timer timer = Metric.Timer("HTTP Requests", Unit.Requests);

        private static readonly Counter responseCodeCounter = Metric.Counter("ResponseCode", Unit.Custom("ResponseCode"));

        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Console.WriteLine("ERR: No target specified");
                Console.ReadLine();
                return;
            }

            if (!File.Exists("equations.txt"))
            {
                Console.WriteLine("ERR: No equations.txt found.");
                Console.ReadLine();
                return;
            }

            Metric.Config
                .WithHttpEndpoint("http://localhost:1234/")
                .WithAllCounters();

            Uri serverUri;
            Uri.TryCreate(args[0], UriKind.Absolute,  out serverUri);

            Console.WriteLine($"Targeting '{serverUri}'");

            var equations = File.ReadAllLines("equations.txt");
            Console.WriteLine($"Found {equations.Length} equations");

            var cts = new CancellationTokenSource();

            var taskCount = 4;

            Console.WriteLine($"Using {taskCount} tasks in parallel... See: http://localhost:1234");
            Console.WriteLine("==================================================================");

            for (var i = 0; i < taskCount; i++)
            {
                Task.Run(() => Run(serverUri, equations), cts.Token);
            }

            Console.ReadLine();
            cts.Cancel();
        }

        private static async void Run(Uri serverUri, string[] equations)
        {
            var httpClient = new HttpClient();
            var sw = new Stopwatch();
            
            while (true)
            {
                foreach (var equation in equations)
                {
                    using (timer.NewContext())
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(serverUri + "calc?eq=" + equation);
                            responseCodeCounter.Increment(response.StatusCode.ToString());

                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }
    }
}
