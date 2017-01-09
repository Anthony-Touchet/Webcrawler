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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    public class Program
    {
        private static string _folderName = null;
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter a site:");                          // Get Input
            var site = Console.ReadLine();
            Console.WriteLine();
          
            var active = true;
            while (active)
            {
                Console.WriteLine("What would you like?");                          // Get Input
                var option = Console.ReadLine();
                Console.WriteLine();

                string line = null;     // Line that we are reading
                
                // Choices
                switch (option)         
                {
                    case "Skills":
                        FindSkills(line);      // Prints out skills
                        break;

                    case "Page Links":
                        var validLinks = FindLinksOnPage(line, site); // Prints all valid linkc on the page
                        foreach (var s in validLinks)
                        {
                            Console.WriteLine(s);
                        }

                        break;

                    case "All Links":
                        var fullLinks = FindLinksOnFullSite(line, site); // Prints all valid linkc on the page
                        foreach (var s in fullLinks)
                        {
                            Console.WriteLine(s);
                        }

                        break;

                    case "Quit":
                        active = false;                         // Quit Program
                        break;

                    case "New Site":
                        Console.WriteLine("Enter a site:");                          // Get Input
                        site = Console.ReadLine();
                        Console.WriteLine();
                        break;

                    default:
                        Console.WriteLine("Invalid Input");     // Not Valid Input
                        break;
                }

                Console.WriteLine();
            }    
        }
        
        public static void FindSkills(string line)
        {
            var skills = new List<string>();
            var sr = GetPageInfo("https://anthony-touchet.github.io");                  // Site name);

            while ((line = sr.ReadLine()) != null)
            {
                // These are the symbols we are using to find the info.
                if (!line.Contains("<li>") || !line.Contains("</li>"))
                {
                    continue;
                }

                var skill = line.Split('>')[1];              // Spliting lines from tabs
                skill = skill.Substring(0, skill.Length - 4);   // Taking the back part off
                skills.Add(skill);
            }

            sr.Close();
            foreach (var s in skills)
            {
                Console.WriteLine(s);
            }
        }

        public static List<string> FindLinksOnPage(string line, string siteName)
        {
            var links = new List<string>();    // List of links

            var sr = GetPageInfo(siteName);                  // Site name);

            // Read each line
            while ((line = sr.ReadLine()) != null)
            {
                // These are the symbols we are using to find the info.
                if (!line.Contains("<a ") && !line.Contains("< a "))
                {
                    continue;
                }

                var link = line; // Get Line

                // If Full link
                if (link.Contains("https://") || link.Contains("www."))
                {
                    var fullLinkStringList = link.Split('"');
                    foreach (var s in fullLinkStringList)
                    {
                        if (s.Contains("https://") || link.Contains("www."))
                        {
                            link = s;
                        }
                    }

                    // Try to get link
                    if (IsSiteValid(link))
                    {
                        links.Add(link);
                    }
                }

                // ReSharper disable once StyleCop.SA1108
                // Else if other part of the site
                else
                {
                    // Create Proper Link
                    var partialLinkStringList = link.Split('"');
                    for (int i = 0; i < partialLinkStringList.Length; i++)
                    {
                        if (partialLinkStringList[i].Contains("href"))
                        {
                            link = partialLinkStringList[i + 1];
                        }
                    }
                   
                    if (!link.Contains(siteName))
                    {
                        link = siteName + link.Substring(0, link.Length);
                    }

                    // Try to get link
                    if (IsSiteValid(link))
                    {
                        links.Add(link);
                    }                       
                }
            }

            sr.Close();
            return links;
        }

        public static List<string> FindLinksOnFullSite(string line, string siteName)
        {
            var links = FindLinksOnPage(line, siteName);    // List of links
            var tempLinks = links.ToList();                     // Create Temp list

            // Weed out Outside sites
            foreach (var l in tempLinks)
            {
                if (l.Contains(siteName))
                {
                    continue;
                }

                links.Remove(l);
            }

            tempLinks = links.ToList();
             
            foreach (var l in tempLinks)
            {
                var pageReader = GetPageInfo(l);                  // Site name
                pageReader.Close();
                var newPageLinks = FindLinksOnPage(line, l);
                
                links.AddRange(newPageLinks);
            }

            return links;
        }

        public static StreamReader GetPageInfo(string site)
        {
            string pageContent;
            var myRequest = (HttpWebRequest)WebRequest.Create(site);            // Asking for the site
            var myResponse = (HttpWebResponse)myRequest.GetResponse();          // Response to that request

            // Get the page info
            using (var sr = new StreamReader(myResponse.GetResponseStream()))
            {
                pageContent = sr.ReadToEnd();
            }
 
            var siteArray = site.Split('/');
            foreach (var s in siteArray)
            {
                if (s == "" || s == "https:" || s == "http:") continue;
                _folderName = s.Split('.')[0];
                break;
            }

            Directory.CreateDirectory(_folderName);

            // Writing to Text File
            var webToTextWriter = new StreamWriter(Environment.CurrentDirectory + "\\" + _folderName + "\\Website.txt");     // Save out info
            webToTextWriter.WriteLine(pageContent);                                                                         // Write info to a text file.
            webToTextWriter.Close();

            var webInfoWriter = new StreamWriter(Environment.CurrentDirectory + "\\" + _folderName + "\\WebInfo.txt");       // Save out info
            webInfoWriter.Close();

            var webToTextReader = new StreamReader(Environment.CurrentDirectory + "\\" + _folderName + "\\Website.txt");     // Read file.
            return webToTextReader;
        }

        public static bool IsSiteValid(string siteName)
        {
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(siteName);    // Asking for the site
                myRequest.Method = "HEAD";
                var myResponse = (HttpWebResponse)myRequest.GetResponse();  // Response to that request
                myResponse.Close();
                return true;
            }
            catch
            {
                // Else the link doesn't exist
                return false;
            }
        }
    }
}
