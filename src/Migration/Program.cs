using System;
using System.Collections.Generic;
using System.Linq;
using Business;
using Common.Data;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Migration {

    class Program {

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("fixprice <ratio>");
                Console.WriteLine("delcat");
                return;
            }
            if (args[0] == "fixprice")
                new FixPrice().Run(decimal.Parse(args[1]));
            else if (args[0] == "delcat")
                new RemoveDisabledCategories().Run();
        }

        
    }
}
