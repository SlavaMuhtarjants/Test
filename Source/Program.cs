using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simple.OData.Client;

using Weezlabs.Storgage.Model;
using System.Data.Spatial;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex regex = new Regex(@"^\d{5,10}$");

            while (true)
            {
                //Console.WriteLine(regex.IsMatch(Console.ReadLine()));
                regex.IsMatch("025813");
            }
        }
    }
}