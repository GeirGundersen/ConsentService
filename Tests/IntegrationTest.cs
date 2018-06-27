using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class IntegrationTest
    {
        static List<Process> processes = new List<Process>();

        [ClassInitializeAttribute]
        public static void Setup(TestContext context)
        {
            var filename = GetFile(typeof(BankSimulator.Service));

            StartBankSimulator(filename, "0000");
            StartBankSimulator(filename, "0010");
            StartBankSimulator(filename, "0020");
            StartBankSimulator(filename, "0025");
            StartBankSimulator(filename, "0060");
            StartBankSimulator(filename, "0200");
        }

        [TestMethod]
        public void RunAll()
        {
            var filename = GetFile(typeof(ConsentRequester.BankClient));

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(filename, "testdata.csv");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            var lines = new List<string>();
            p.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);
                lines.Add(args.Data);
            };
            p.Start();

            p.BeginOutputReadLine();

            p.WaitForExit();

            Assert.IsTrue(lines.Count > 18);

            Assert.IsTrue(lines.Contains("Customer 18359000 bank consent : True"));

            Assert.IsTrue(lines.Contains("Customer 21671400 bank consent : False"));
        }

        [ClassCleanup]
        public static void StopProcesses()
        {
            foreach (var p in processes)
            {
                p.Kill();
            }
            processes.Clear();
        }

        private static string GetFile(Type type)
        {
            return type.Assembly.Location;
        }

        private static void StartBankSimulator(string filename, string bankid)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(filename, bankid);
            p.Start();
            processes.Add(p);
        }
    }
}
