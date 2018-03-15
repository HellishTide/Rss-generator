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
            Console.Title = "RSS Generator";
            try
            {
                GetRSS();
                GetText();
                GetInfo();
                CreateFile();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something goes wrong!");
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Complete!");
                Console.ReadKey();
            }
        }

        private static string TimeAdd(string dateString)
        {
            DateTime dateTime = DateTime.Parse(dateString);
            dateTime = dateTime.AddHours(3.0);
            string text = dateTime.ToString("r");
            text = text.Replace("GMT", "+0300");
            return "<pubDate>" + text + "</pubDate>";
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
                using (FileStream fileStream = File.Create(path + @"\news.rss"))
                {
                    string s = GenerateXMLSchema();
                    byte[] bytes = new UTF8Encoding(true).GetBytes(s);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void GetText()
        {
            string title = "";
            string text = "";
            string text2 = "";
            string path = Environment.GetCommandLineArgs()[0];
            try
            {
                string directoryName = Path.GetDirectoryName(path);
                string[] array = File.ReadAllLines(directoryName + "\\MyNews.txt");
                string[] array2 = array;
                foreach (string text3 in array2)
                {
                    if (text3 != "" && title == "")
                    {
                        title = text3;
                    }
                    else if (text3 != "" && title != "" && text == "")
                    {
                        text = text + text3 + "\n";
                        try
                        {
                            fullItems.Find((Item x) => x.title.Contains(title)).description = text;
                        }
                        catch
                        {
                        }
                    }
                    else if (text3 != "" && text3 != " " && title != "" && text != "" && text3 != "-------------------------------")
                    {
                        text2 = text2 + text3 + "\n";
                        try
                        {
                            fullItems.Find((Item x) => x.title.Contains(title)).fullText = text2;
                        }
                        catch
                        {
                        }
                    }
                    if (text3 == "-------------------------------" && title != "" && text != "" && text2 != "")
                    {
                        text = (title = (text2 = ""));
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File MyNews.txt not fouded");
            }
        }


        private static void GetRSS()
        {
            string path = Environment.GetCommandLineArgs()[0];
            string directoryName = Path.GetDirectoryName(path);
            try
            {
                string[] array = File.ReadAllLines(directoryName + "\\myRSS.txt");
                string titl = "";
                string desc = "";
                string lk = "";
                string url = "";
                string aut = "";
                string date = "";
                bool flag = false;
                string[] array2 = array;
                foreach (string text in array2)
                {
                    if (text.Contains("<item>") | flag)
                    {
                        flag = true;
                        if (text.Contains("<title>"))
                        {
                            string text2 = "<title>";
                            int num = text.IndexOf(text2);
                            text2 = "</" + text2.Substring(1);
                            int num2 = text.IndexOf(text2);
                            titl = text.Substring(num, num2 + text2.Length - num);
                        }
                        if (text.Contains("<link>"))
                        {
                            string text3 = "<link>";
                            int num3 = text.IndexOf(text3);
                            text3 = "</" + text3.Substring(1);
                            int num4 = text.IndexOf(text3);
                            lk = text.Substring(num3, num4 + text3.Length - num3);
                        }
                        if (text.Contains("<author>"))
                        {
                            string text4 = "<author>";
                            int num5 = text.IndexOf(text4);
                            text4 = "</" + text4.Substring(1);
                            int num6 = text.IndexOf(text4);
                            aut = text.Substring(num5, num6 + text4.Length - num5);
                        }
                        if (text.Contains("<pubDate>"))
                        {
                            string text5 = "<pubDate>";
                            int num7 = text.IndexOf(text5) + 9;
                            text5 = "</" + text5.Substring(1);
                            int num8 = text.IndexOf(text5) - 10;
                            date = text.Substring(num7, num8 + text5.Length - num7);
                            date = TimeAdd(date);
                        }
                        if (text.Contains("<img src="))
                        {
                            string value = "src=";
                            int num9 = text.IndexOf(value) + 4;
                            value = "width";
                            int num10 = text.IndexOf(value) - 6;
                            url = text.Substring(num9, num10 + value.Length - num9);
                        }
                    }
                    if (text.Contains("</item>"))
                    {
                        Item item = new Item();
                        item.SetItem(titl, desc, date, aut, lk, url, "");
                        fullItems.Add(item);
                        flag = false;
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File myRSS.txt not fouded");
            }
        }

        private static string GetInfo()
        {
            foreach (Item fullItem in fullItems)
            {
                string str = 
                    "<item>\n" + 
                    fullItem.title + "\n" +
                    fullItem.link + 
                    "\n<description>\n" + 
                    fullItem.description + 
                    "</description>\n" + 
                    fullItem.pubDate + 
                    "\n<enclosure url=" + 
                    fullItem.enclouser + 
                    "/>\n<yandex:full-text>\n" + 
                    fullItem.description + "\n" + 
                    fullItem.fullText + 
                    "</yandex:full-text>\n</item>\n";
                st_info += str;
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
                "<link>http://www.xn--80aaafcmcb6evaidf6r.xn--p1ai/</link>" + "\n" +
                "<description>Газета Северная правда</description>" + "\n" +
                "<yandex:logo>http://www.xn--80aaafcmcb6evaidf6r.xn--p1ai/images/vipuski/100.png</yandex:logo>" + "\n" +
                "<yandex:logo type=\"square\">http://www.xn--80aaafcmcb6evaidf6r.xn--p1ai/images/vipuski/180px.png</yandex:logo>" + "\n" +
                "<language>ru-ru</language>" + "\n" +
                st_info + "\n" +
                "</channel>" + "\n" +
                "</rss>";
            return xmlSchema;    
        }
    }
}