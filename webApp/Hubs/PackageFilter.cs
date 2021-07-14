using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webApp.Hubs
{
    public class PackageFilter
    {
        private List<string> censoredWords;
        
       
        public PackageFilter(string package) 
        {
            SetCensoredWords(package);
        }

        public List<string> GetCensoredWords() 
        {
            return censoredWords;
        }

        private void SetCensoredWords(string package)
        {
            var logFile = File.ReadAllLines(@"Hubs\WordsPackage\"+package);
            censoredWords = new List<string>(logFile);
        }

    }
}
