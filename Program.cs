using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Syn.WordNet;
using static System.Console;
using System.Windows.Forms;


namespace LuceneTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
          
            string filepath = null;
            string curPath = Directory.GetCurrentDirectory();
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "JSON|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filepath = ofd.SafeFileName;
            }
            string json = File.ReadAllText(filepath);
            List<records> storeRecords = DeserializeJSON(json);

            WriteLine("Hello Lucene.Net");

            LuceneApplication LuceneApp = new LuceneApplication();
            //wrdnet wordne = new wrdnet();


            LuceneApp.CreateIndex(curPath);
            WriteLine(curPath);
            WriteLine("Adding Documents to Index");

            DateTime start = System.DateTime.Now;
            for (int x = 0; x < storeRecords.Count; x++)
            {
                WriteLine("Adding record no #{0}", x+1);
                for (int y = 0; y < storeRecords[x].passages.Count; y++)
                {
                    
                     string single_text = (storeRecords[x].passages[y].url + storeRecords[x].passages[y].passage_text);
                    //WriteLine("URL: {0}", storeRecords[x].passages[y].url.ToString());
                    //WriteLine("Passage Text: {0}", storeRecords[x].passages[y].passage_text.ToString());
                    // LuceneApp.IndexText(storeRecords[x].passages[y].url + storeRecords[x].passages[y].passage_text);
                    
                    LuceneApp.IndexText(single_text);
                    
                    //LuceneApp.IndexText(storeRecords[x].passages[y].passage_text);
                }
            }
            DateTime end = System.DateTime.Now;
            WriteLine("Total time for indexing >> {0}", end - start);

            WriteLine("All documents added.");

            // clean up
            LuceneApp.CleanUpIndexer();

            LuceneApp.CreateSearcher();
            // var directory = Directory.GetCurrentDirectory();
            var directory = @"C:\Users\Suprith Kangokar\Desktop\LuceneTest\LuceneTest\LuceneTest\bin\Debug\Wordnet";
            var wordNet = new WordNetEngine();
            //  wordne.wordnet();
            string QUIT = "q";

            Write("Enter your query >>");
            string queryText = ReadLine();
            wordNet.LoadFromDirectory(directory);
            var synSetList = wordNet.GetSynSets(queryText);
            if (synSetList.Count == 0) Console.WriteLine("No SynSet found");
            string ex = "\t";


            foreach (var synSet in synSetList)
            {
                ex = string.Join(", ", synSet.Words);
                queryText += ("\t" + ex);



            }
            //  LuceneApp.CreateParser();

            //string QUIT = "q";

            //Write("Enter your query >>");
            //string queryText = ReadLine();



            while (queryText != QUIT)
            {
                LuceneApp.DisplayResults(LuceneApp.SearchIndex(queryText));
                Write("Enter your query or press 'q' to exit >>");
                queryText = ReadLine();
            }


            





            //WriteLine("Press Enter to exit.");
            //ReadLine();
        }
        public static List<records> DeserializeJSON(string jsonText)
        {
            try
            {
                var readRecords = JsonConvert.DeserializeObject<List<records>>(jsonText);
                
                return readRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message.ToString());
                return null;
            }
        }

        public static void DisplayOutput(string text)
        {
            Console.WriteLine(text);
        }
        
    }
}
