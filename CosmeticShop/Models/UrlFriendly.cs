using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CosmeticShop.Models
{
    public static class UrlFriendly
    {
        public static string ToURLFriendly(this string Url)
        {
            string str = Url.ToLower().Trim();

            //thay thế tiếng Việt
            str = Regex.Replace(str, @"[áàảạãăắằẳẵặâấầẩẫậ]", "a");
            str = Regex.Replace(str, @"[êếềệểéèẹẻễẽ]", "e");
            str = Regex.Replace(str, @"[ôồốộổơờớợởòóọỏỗỗõ]", "o");
            str = Regex.Replace(str, @"[íìỉịĩ]", "i");
            str = Regex.Replace(str, @"[ưứừựửủụúùữũ]", "u");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @" - ", "-").Trim();
            str = Regex.Replace(str, @"\s+", "-").Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
    }
}
