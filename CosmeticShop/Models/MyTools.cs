using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmeticShop.Models
{
    public class MyTools
    {
        public static string ToURLFriendly(int Id, string Name)
        {
            string phrase = string.Format("{0}-{1}", Id, Name);
            string str = phrase.ToLower();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");// Remove invalid characters for param  
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-").Trim(); // convert multiple spaces into one hyphens   
            //str = str.Substring(0, str.Length <= 30 ? str.Length : 30).Trim(); //Trim to max 30 char  
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-"); // Replaces spaces with hyphens     
            return str;
        }
    }
}
