using System;
using System.Linq;
using System.Reflection;

namespace Queen.Models {
    public class ApplicationHelper {
        public static string GetCurrentAssemblyVersion() {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        	
        }

    }
}