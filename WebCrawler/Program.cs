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
            string site = "https://anthony-touchet.github.io";
            string pageContent = null;
            var myRequest = (HttpWebRequest)WebRequest.Create(site);    //Asking for the site
            var myResponse = (HttpWebResponse)myRequest.GetResponse();  //Response to that request

            using (var sr = new StreamReader(myResponse.GetResponseStream()))   //Get the page info
            {
                pageContent = sr.ReadToEnd();
            }

            StreamWriter webToTextWriter = new StreamWriter(Environment.CurrentDirectory + "\\WebInfo.txt");    //Save out info
            webToTextWriter.WriteLine(pageContent);                                                             //Write info to a text file.
            webToTextWriter.Close();
           
            StreamReader webToTextReader = new StreamReader(Environment.CurrentDirectory + "\\WebInfo.txt");    //Read file.
            string line = null;    //Line that we are reading

           // FindSkills(line, webToTextReader);
            FindLinks(line, webToTextReader, site);

            Console.ReadLine();
        }
        
        static void FindSkills(string line, StreamReader sr)
        {
            List<string> skills = new List<string>();

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("<li>") && line.Contains("</li>")) //These are the symbols we are using to find the info.
                {
                    string skill = line.Split('>')[1];              //Spliting lines from tabs
                    skill = skill.Substring(0, skill.Length - 4);   //Taking the back part off
                    skills.Add(skill);
                }
            }

            foreach (var s in skills)
            {
                Console.WriteLine(s);
            }
        }

        static void FindLinks(string line, StreamReader sr, string siteName)
        {
            List<string> links = new List<string>();    //List of links

            while ((line = sr.ReadLine()) != null)  //Read each line
            {
                if (line.Contains("<a href") && line.Contains("</a>")) //These are the symbols we are using to find the info.
                {
                    string link = line; //Get Line
                    
                    //If Full link
                    if (link.Contains("https://www"))
                    {
                        link = link.Split('"')[1];
                        link = link.Substring(0, link.Length - 2);
                        links.Add(link);
                    }

                    //Else if other part of the site
                    else
                    {   
                        //Create Proper Link
                        link = link.Split('"')[1];
                        link = siteName + "/" + link.Substring(0, link.Length);
                        
                        //Try to get link
                        try
                        {
                            var myRequest = (HttpWebRequest)WebRequest.Create(link);    //Asking for the site
                            myRequest.Method = "HEAD";
                            var myResponse = (HttpWebResponse)myRequest.GetResponse();  //Response to that request
                            myResponse.Close();
                            links.Add(link);        //If no errors happen, Link was good
                        }
                        catch
                        {
                            //Else the link doesn't exist
                        }                       
                    }
                }
            }

            foreach (var s in links)
            {
                Console.WriteLine(s);
            }
        }
    }
}
