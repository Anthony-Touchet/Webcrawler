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
            string pageContent = null;
            var myRequest = (HttpWebRequest)WebRequest.Create("https://anthony-touchet.github.io/");    //Asking for the site
            var myResponse = (HttpWebResponse)myRequest.GetResponse();                                  //Response to that request

            using (var sr = new StreamReader(myResponse.GetResponseStream()))   //Get the page info
            {
                pageContent = sr.ReadToEnd();
            }

            StreamWriter webToTextWriter = new StreamWriter(Environment.CurrentDirectory + "\\WebInfo.txt");    //Save out info
            webToTextWriter.WriteLine(pageContent);                                                             //Write info to a text file.
            webToTextWriter.Close();

            StreamReader webToTextReader = new StreamReader(Environment.CurrentDirectory + "\\WebInfo.txt");    //Read file.

            //GET SKILLS

            string line;    //Line that we are reading
            List<string> skills = new List<string>();

            while ((line = webToTextReader.ReadLine()) != null)
            {
                if (line.Contains("<li>") && line.Contains("</li>")) //These are the symbols we are using to find the info.
                {
                    string skill = line.Split('>')[1];  //Shrinking the lines
                    skill = skill.Substring(0, skill.Length - 4);
                    skills.Add(skill);
                }
            }

            foreach (var s in skills)
            {
                Console.WriteLine(s);
            }

            Console.ReadLine();
        }
    }
}
