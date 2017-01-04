// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Anthony">
//   Looking for information on the web.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------




namespace WebCrawler
{
    using System;
    using System.Net;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("What is the word you would like to search?:");
            var input = Console.ReadLine();

            string pageContent = null;
            var myRequest = (HttpWebRequest)WebRequest.Create("https://anthony-touchet.github.io/");    //Asking for the site
            var myResponse = (HttpWebResponse)myRequest.GetResponse();                                  //Response to that request

            using (var sr = new StreamReader(myResponse.GetResponseStream()))
            {
                pageContent = sr.ReadToEnd();
            }

            Console.WriteLine(pageContent.Contains(input) ? "There" : "Not there");

            Console.ReadLine();
        }
    }
}
