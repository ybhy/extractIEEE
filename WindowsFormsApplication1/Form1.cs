using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data;
using System.Data.SqlClient;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public List<string> url = new List<string>();
        public string urlhead = "http://ieeexplore.ieee.org";
        public int index = 0;

        public int sum = 1000;
        public int sumcount = 0;
        public int indexsum = 0;
        public int start = 953;
        public int end = 2700;
        public Form1()
        {
            InitializeComponent();
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Navigate("http://ieeexplore.ieee.org/xpl/RecentIssue.jsp?punumber=79");
            this.webBrowser1.ScriptErrorsSuppressed = true;
            url.Clear();
            timer1.Stop();
            timer2.Stop();
            //Test();
        }
        //public void Test()
        //{
        //    LWEntities1 TDB = new LWEntities1();
        //    HXfinechemicals tab = new HXfinechemicals();
        //    tab.title = "title";
        //    tab.abstracts = "abstract";
        //    tab.keywords = "keywords";
        //    TDB.HXfinechemicals.Add(tab);
        //    TDB.SaveChanges();
        //}
        private void timer1_Start(object sender, EventArgs e)
        {
            foreach (HtmlElement ahref in this.webBrowser1.Document.GetElementsByTagName("A"))
            {
                // Console.WriteLine(ahref.InnerText);
            }
            if (index < url.Count)
            {
                Console.WriteLine(index + 1);
                this.webBrowser1.Navigate(url[index]);
                Extraction(this.webBrowser1.Document);
                index++;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            using (LWEntities7 TDB = new LWEntities7())
            {
                IQueryable<TXsignalprocessingurl> ieurl = from c in TDB.TXsignalprocessingurl select c;
                //int count = 0;
                this.timer2.Start();
                sumcount = ieurl.Count();
                int indexiiiiii = 0;
                foreach (var c in ieurl)
                {
                    if (indexiiiiii >= start && indexiiiiii < end)
                        url.Add(c.url);
                    indexiiiiii++;
                }
            }
        }
        private void Extraction(HtmlDocument document)
        {
            LWEntities7 TDB = new LWEntities7();
            TXsignalprocessing tab = new TXsignalprocessing();
            string abstracts = "";
            string keywords = "";
            string title = "";
            string inspec = "";
            string inspecnot = "";
            string ieeeterms = "";
            foreach (HtmlElement div in document.GetElementsByTagName("div"))
            {

                if (div.GetAttribute("className").Equals("title"))
                {
                    //Console.WriteLine("title: "+div.InnerText);
                    title = div.InnerText;
                    Console.WriteLine("title: " + title);
                    //tab.title = title;
                }
                if (div.GetAttribute("data-section").Equals("articleDetails.ajax"))
                {
                    foreach (HtmlElement div1 in div.GetElementsByTagName("div"))
                    {
                        if (div1.GetAttribute("className").Equals("article"))
                        {
                            abstracts = div1.InnerText;
                            Console.WriteLine("abstract: " + div1.InnerText);
                            //tab.abstracts = div1.InnerText;
                        }
                    }
                }
                if (div.GetAttribute("data-section").Equals("abstractKeywords.ajax"))
                {
                    foreach (HtmlElement div1 in div.GetElementsByTagName("div"))
                    {
                        if (div1.GetAttribute("className").Equals("section"))
                        {
                            Boolean isAuthorKeywords = false;
                            foreach (HtmlElement h2 in div1.GetElementsByTagName("h2"))
                            {
                                if (h2.InnerText.Equals("AUTHOR KEYWORDS"))
                                {
                                    isAuthorKeywords = true;
                                    break;
                                }
                            }
                            if (isAuthorKeywords)
                            {
                                foreach (HtmlElement li in div1.GetElementsByTagName("li"))
                                {
                                    keywords += li.InnerText + ";";
                                }
                            }

                            Boolean isInspec = false;
                            foreach (HtmlElement h2 in div1.GetElementsByTagName("h2"))
                            {
                                if (h2.InnerText.Equals("INSPEC: CONTROLLED INDEXING"))
                                {
                                    isInspec = true;
                                    break;
                                }
                            }
                            if (isInspec)
                            {
                                foreach (HtmlElement li in div1.GetElementsByTagName("li"))
                                {
                                    inspec += li.InnerText + ";";
                                }
                            }

                            Boolean isInspecNot = false;
                            foreach (HtmlElement h2 in div1.GetElementsByTagName("h2"))
                            {
                                if (h2.InnerText.Equals("INSPEC: NON CONTROLLED INDEXING"))
                                {
                                    isInspecNot = true;
                                    break;
                                }
                            }
                            if (isInspecNot)
                            {
                                foreach (HtmlElement li in div1.GetElementsByTagName("li"))
                                {
                                    inspecnot += li.InnerText + ";";
                                }
                            }

                            Boolean isIeeeTerms = false;
                            foreach (HtmlElement h2 in div1.GetElementsByTagName("h2"))
                            {
                                if (h2.InnerText.Equals("IEEE TERMS"))
                                {
                                    isIeeeTerms = true;
                                    break;
                                }
                            }
                            if (isIeeeTerms)
                            {
                                foreach (HtmlElement li in div1.GetElementsByTagName("li"))
                                {
                                    ieeeterms += li.InnerText + ";";
                                }
                            }
                        }
                    }
                }
            }
            tab.abstracts = abstracts;
            tab.title = title;
            tab.keywords = keywords;
            tab.inspec = inspec;
            tab.inspecnot = inspecnot;
            tab.ieeeterms = ieeeterms;
            tab.label = "Math";
            tab.search = "function theory";
            TDB.TXsignalprocessing.Add(tab);
            TDB.SaveChanges();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Console.WriteLine("sum : " + sum);
            foreach (HtmlElement ahref in this.webBrowser1.Document.GetElementsByTagName("A"))
            {
                if (ahref.GetAttribute("className").Equals("ng-binding ng-scope"))
                {
                    string urll = urlhead + ahref.GetAttribute("href");
                    if (!(urll.Contains("mostRecentIssue")))
                    {
                        LWEntities7 TDB = new LWEntities7();
                        TXsignalprocessingurl tab = new TXsignalprocessingurl();
                        tab.url = urll;
                        TDB.TXsignalprocessingurl.Add(tab);
                        TDB.SaveChanges();
                    }

                }
                if (ahref.GetAttribute("ng-click").Equals("selectPage(page + 1)"))
                {
                    sum--;
                    if (sum <= 0)
                    {
                        this.timer1.Stop();
                        //this.timer2.Start();
                        Console.WriteLine(url.Count);
                    }
                    ahref.InvokeMember("click");
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (sumcount >= indexsum)
            {
                Console.WriteLine("index : " + indexsum);
                this.webBrowser1.Navigate(url[indexsum]);
                Extraction(this.webBrowser1.Document);
                indexsum++;
            }
        }

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}