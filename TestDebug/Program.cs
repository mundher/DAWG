using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FSA;

namespace TestDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            //var at = AutomatonBuilder.Build(new string[] { "mop", "moth", "pop", "star", "stared", "stop", "top" });
            //var dot = at.ToDot();
            //File.WriteAllText("Myat.dot", dot);
            //TestSave(at);
            //TestFillWords();
            //TestNormal();
            //TestFr();
            //TestDawg();
            TestPrefix();
        }

        private static void TestPrefix()
        {
            var at = AutomatonBuilder.Build(File.ReadLines("G:\\words2.txt"));
            Console.WriteLine("Build");
            var dawg = new DAWG(at.Initial);
            string line;
            while ((line = Console.ReadLine()) != "")
            {
                var words = dawg.StartWith(line, 5);
                foreach (var word in words)
                {
                    Console.WriteLine("-> "+word);
                }
            }
        }

        private static void TestDawg()
        {
            //var at = AutomatonBuilder.Build(new[] { "mop", "moth", "pop", "star", "stared", "stop", "top" });
            var at = AutomatonBuilder.Build(File.ReadLines("G:\\words2.txt"));
            Console.WriteLine("Build");
            var dawg = new DAWG(at.Initial);
            Console.WriteLine(dawg.Contains("Moth"));
            Console.WriteLine(dawg.Contains("stare"));
            Console.ReadKey(true);
        }

        private static void TestFr()
        {
            var at = AutomatonBuilder.Build(File.ReadLines("G:\\wordsFr.txt"));
            var dot = at.ToDot();
            File.WriteAllText("fr.dot", dot);
        }

        private static void TestNormal()
        {
            //var at = AutomatonBuilder.Build(new string[] { /*"mop", "moth", "pop",*/ "star","stared", "stop", "top", "ztar" });
            var at = AutomatonBuilder.Build(new [] { "had", "hard", "he", "head", "heard", "her", "herd", "here" });
            Console.WriteLine("Build");
            Console.ReadKey(true);
            Console.WriteLine(at.Contains("moth"));
            var dot = at.ToDot();
            File.WriteAllText("Myat.dot", dot);
        }

        private static void TestFillWords()
        {
            var at = AutomatonBuilder.Build(File.ReadLines("G:\\wordsFr.txt"));
            Console.WriteLine(at.Contains("find"));
            Console.WriteLine("Build");
            Console.ReadKey(true);
            //TestSave(at);
            Console.WriteLine(at.Contains("sluggard"));
            Console.WriteLine("finish");
            Console.ReadKey(true);
            //TestSave(at);
        }

        private static void TestSave(Automaton at)
        {
            using (var stream = new FileStream("at.bin",FileMode.Create))
            {
                at.Store(stream);
            }
            using (var stream = new FileStream("at.bin",FileMode.Open))
            {
                var myat = new Automaton();
                myat.Load(stream);
                Console.WriteLine(myat.Contains("pop"));
                Console.WriteLine(myat.Contains("sta"));
                //var dot = at.ToDot();
                //File.WriteAllText("Myat.dot", dot);
            }
        }
    }
}
