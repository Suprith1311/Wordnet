using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Syn.WordNet;
using Lucene.Net.Analysis; // for Analyser
using Lucene.Net.Documents; // for Document and Field
using Lucene.Net.Index; //for Index Writer
using Lucene.Net.Store; //for Directory
using Lucene.Net.Search; // for IndexSearcher
using Lucene.Net.QueryParsers;  // for QueryParser

namespace LuceneTest
{
    class LuceneApplication
    {
        //Fields
        Lucene.Net.Store.Directory luceneIndexDirectory;
        Lucene.Net.Analysis.Analyzer analyzer;
        Lucene.Net.Index.IndexWriter writer;
        Lucene.Net.Search.IndexSearcher searcher;
        Lucene.Net.QueryParsers.QueryParser parser;


        const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
        const string TEXT = "Text"; //Text Fields. For the column URL column.
        //const string TEXT_PASSAGE = "Passage Text"; //Text fields. For the Passage Text column.


        public LuceneApplication()
        {
            luceneIndexDirectory = null;
            analyzer = null;
            writer = null;
        }
        

        /// <summary>
        /// Creates the index at indexPath
        /// </summary>
        /// <param name="indexPath">Directory path to create the index</param>
        public void CreateIndex(string indexPath)
        {
            luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
            analyzer = new Lucene.Net.Analysis.SimpleAnalyzer();
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
            writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
        }

        /// <summary>
        /// Indexes the given text
        /// </summary>
        /// <param name="text">Text to index</param>
        public void IndexText(string passage)
        {
            // Lucene.Net.Documents.Field field_URL_text = new Field(TEXT_URL, url, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
            Lucene.Net.Documents.Field field_Text = new Field(TEXT, passage, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
            Lucene.Net.Documents.Document doc = new Document();
            // doc.Add(field_URL_text);
            doc.Add(field_Text);
            writer.AddDocument(doc);
        }

        /// <summary>
        /// Flushes buffer and closes the index
        /// </summary>
        public void CleanUpIndexer()
        {
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Dispose();
        }
        public void Searcher(string path)
        {
            luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(path);
            writer = null;
            analyzer = new Lucene.Net.Analysis.SimpleAnalyzer();
            parser = new QueryParser(VERSION, TEXT, analyzer);
        }
        /// <summary>
        /// Initialises the searcher object
        /// </summary>
        public void CreateSearcher()
        {
            searcher = new IndexSearcher(luceneIndexDirectory);
        }


        /// <summary>
        /// Initialises the parser object
        /// </summary>
        public void CreateParser()
        {
            parser = new QueryParser(VERSION, TEXT, analyzer);
            //parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TEXT_PASSAGE, analyzer);
        }

        /// <summary>
        /// Closes the index after searching
        /// </summary>
        public void CleanUpSearch()
        {
            searcher.Dispose();
        }

        /// <summary>
        /// Searches the index with the specified query text
        /// </summary>
        /// <param name="querytext">Text to search the index</param>
        /// <returns></returns>
        public TopDocs SearchIndex(string querytext)
        {

            System.Console.WriteLine("Searching for " + querytext);
            querytext = querytext.ToLower();
            
            Query query = parser.Parse(querytext);
            TopDocs results = searcher.Search(query, 10);
            System.Console.WriteLine("Found " + results.TotalHits + " documents.");

            return results;
        }

        /// <summary>
        /// Outputs results to the screen
        /// </summary>
        /// <param name="results">Search results</param>
        public void DisplayResults(TopDocs results)
        {
            List<String> evalrecords = new List<String>();
            int rank = 0;
            foreach (ScoreDoc scoreDoc in results.ScoreDocs)
            {
                rank++;
                // retrieve the document from the 'ScoreDoc' object
                Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);
                string field_URL = doc.Get(TEXT).ToString();
                //string field_Text = doc.Get(TEXT_PASSAGE).ToString();
                //Console.WriteLine("Rank " + rank + " score " + scoreDoc.Score + " text " + myFieldValue);
                Console.WriteLine();
                Console.WriteLine("Rank #" + rank);
                Console.WriteLine("Text: " + field_URL);
                Console.WriteLine();

                string topicId = "001";
                string studgroup = "10124021";
                String record = topicId + " Q0 " + TEXT + "" + rank + " " + scoreDoc.Score + " " + studgroup;
                evalrecords.Add(record);
            }
            System.IO.File.WriteAllLines(@"C:\Users\Suprith Kangokar\Desktop\SEM 3\IFN647- Advance Info & Retreival\baseline.txt", evalrecords);
        }
        public void CleanUpSearcher()
        {
            searcher.Dispose();
        }
       


    }
    }

