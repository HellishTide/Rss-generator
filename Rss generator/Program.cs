using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Rss_generator
{
    class Item
    {
        public string title, description, pubDate, author, link, enclouser, fullText;

        public Item()
        {
            title = description = pubDate = author = link = enclouser = fullText = "";
        }

        public void SetItem(string titl, string desc, string date, string aut, string lk, string url, string text)
        {
            title = titl;
            description = desc;
            pubDate = date;
            author = aut;
            link = lk;
            enclouser = url;
            fullText = text;
        }
    }

    class Program
    {
        public static void Main()
        {
            try
            {
                GetRSS();
                GetText();
                GetInfo();
                CreateFile();
            }
            catch
            {
                Console.WriteLine("Something goes wrong!");
            }
            finally
            {
                Console.WriteLine("Complete");
                Console.ReadKey();
            }
        }

        public static string st_info = "";
        public static List<Item> fullItems = new List<Item>();

        private static void CreateFile()
        {
            string path = Environment.GetCommandLineArgs()[0];
            var directory = Path.GetDirectoryName(path);
            try
            {
                
                if (Directory.Exists(directory + @"\Rss"))
                {
                    //Console.WriteLine("That path exists already.");
                    path = directory + @"\Rss";
                }
                else
                {
                    path = directory + @"\Rss";
                    Directory.CreateDirectory(path);
                }
                using (FileStream fs = File.Create(path + @"\news.rss"))
                {
                    string schema = GenerateXMLSchema();
                    Byte[] info = new UTF8Encoding(true).GetBytes(schema);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
                /*
                // Open the stream and read it back.
                StreamReader sr = File.OpenText(path);
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
                */
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void GetText()
        {
            string title = "";
            string description = "";
            string fullText = "";
            string path = Environment.GetCommandLineArgs()[0];
            try
            {
                var directory = Path.GetDirectoryName(path);
                string[] newsFile = File.ReadAllLines(directory + @"\MyNews.txt");

                foreach (string dr in newsFile)
                {
                    if (dr != "" && title == "")
                    {
                        title = dr;
                    }
                    else if (dr != "" && title != "" && description == "")
                    {
                        description = dr;
                        try
                        {
                            fullItems.Find(x => x.title.Contains(title)).description = description;
                        }
                        catch { };
                    }
                    else if (dr != "" && dr != " " && title != "" && description != "" && dr != "-------------------------------")
                    {
                        fullText += dr + "\n";
                        try
                        { fullItems.Find(x => x.title.Contains(title)).fullText = fullText; }
                        catch { };
                    }
                    if (dr == "-------------------------------" && title != "" && description != "" && fullText != "")
                    {
                        title = description = fullText = "";
                    }
                }
            } catch { Console.WriteLine("File MyNews.txt not fouded"); }
        }

        private static void GetRSS()
        {
            string path = Environment.GetCommandLineArgs()[0];
            var directory = Path.GetDirectoryName(path);
            try
            {
                string[] rssFile = File.ReadAllLines(directory + @"\myRSS.txt");
                string title = "";
                string description = "";
                string link = "";
                string imgSRC = "";
                string author = "";
                string pubDate = "";
                bool f = false;

                foreach (string dr in rssFile)
                {
                    //Проверка на вхождение в item
                    if (dr.Contains("<item>") || f == true)
                    {
                        f = true;//Флаг нахождения в item
                                 //Получение заголовка
                        if (dr.Contains("<title>"))
                        {
                            String searchString = "<title>";
                            int startIndex = dr.IndexOf(searchString);
                            searchString = "</" + searchString.Substring(1);
                            int endIndex = dr.IndexOf(searchString);
                            title = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                        }
                        /*
                        //Получение подзаголовка
                        if (dr.Contains("<description>"))
                        {
                            String searchString = "<strong>";
                            int startIndex = dr.IndexOf(searchString)+8;
                            searchString = "</" + searchString.Substring(1);
                            int endIndex = dr.IndexOf(searchString)-8;
                            description = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                            Console.WriteLine(description);
                        }
                        */
                        //Получение ссылки на новость
                        if (dr.Contains("<link>"))
                        {
                            String searchString = "<link>";
                            int startIndex = dr.IndexOf(searchString);
                            searchString = "</" + searchString.Substring(1);
                            int endIndex = dr.IndexOf(searchString);
                            link = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                            //Console.WriteLine(link);
                        }
                        //Получение автора новости
                        if (dr.Contains("<author>"))
                        {
                            String searchString = "<author>";
                            int startIndex = dr.IndexOf(searchString);
                            searchString = "</" + searchString.Substring(1);
                            int endIndex = dr.IndexOf(searchString);
                            author = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                            //Console.WriteLine(author);
                        }
                        //Получение даты написания новости
                        if (dr.Contains("<pubDate>"))
                        {
                            String searchString = "<pubDate>";
                            int startIndex = dr.IndexOf(searchString);
                            searchString = "</" + searchString.Substring(1);
                            int endIndex = dr.IndexOf(searchString);
                            pubDate = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                            //Console.WriteLine(pubDate);
                        }
                        //Получение ссылки на картинку
                        if (dr.Contains("<img src="))
                        {
                            String searchString = "src=";
                            int startIndex = dr.IndexOf(searchString) + 4;
                            searchString = "width";
                            int endIndex = dr.IndexOf(searchString) - 6;
                            imgSRC = dr.Substring(startIndex, endIndex + searchString.Length - startIndex);
                            //Console.WriteLine(imgSRC);
                        }
                    }
                    if (dr.Contains("</item>"))
                    {
                        Item here = new Item();
                        here.SetItem(title, description, pubDate, author, link, imgSRC, "");
                        fullItems.Add(here);
                        f = false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("File myRSS.txt not fouded");
            }
        }

        private static string GetInfo()
        {
            foreach (Item he in fullItems)
            {
                string item =
                    "<item>" + "\n" +
                    he.title + "\n" +
                    he.link + "\n" +
                    "<description>" + "\n" +
                    he.description + "\n" +
                    "</description>" + "\n" +
                    he.pubDate + "\n" +
                    he.author + "\n" +
                    "<enclosure url=" + he.enclouser + "/>" + "\n" +
                    "<yandex:full-text>" + "\n" +
                    he.fullText +
                    "</yandex:full-text>" + "\n" +
                    "</item>" + "\n";
                st_info += item;
            }
            return st_info;
        }

        private static string GenerateXMLSchema()
        {
            string xmlSchema =
                "<?xml version=\"1.0\"  encoding=\"utf-8\"?>" + "\n" +
                "<rss xmlns:yandex=\"http://news.yandex.ru\" xmlns:media=\"http://search.yahoo.com/mrss/\" version=\"2.0\">" + "\n" +
                "<channel>" + "\n" +
                "<title> Северная правда - Главная </title >" + "\n" +
                "<link> http://www.xn--80aaafcmcb6evaidf6r.xn--p1ai/</link>" + "\n" +
                "<description> Газета Северная правда</description >" + "\n" +
                "<yandex:logo> http://xn--80aaafcmcb6evaidf6r.xn--p1ai/images/vipuski/SP_100px.png </yandex:logo>" + "\n" +
                "<yandex:logo type=\"square\"> http://xn--80aaafcmcb6evaidf6r.xn--p1ai/images/vipuski/SP180x180%20_1.png </yandex:logo>" + "\n" +
                "<language> ru - ru </language>" + "\n" +
                st_info + "\n" +
                "</channel>" + "\n" +
                "</rss>";
            return xmlSchema;    
        }
    }
}