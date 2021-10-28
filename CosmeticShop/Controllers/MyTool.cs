using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CosmeticShop.Controllers
{
    public class MyTool
    {
    }

    public static class StaticClass
    {
        /*public static string ToVND(this double dongia)
        {
            return $"{dongia.ToString("#,##0")} đ";
        }*/

        public static string ToUrlFriendly(this string tieuDe)
        {
            string str = tieuDe.ToLower().Trim();

            //thay thế tiếng Việt
            str = Regex.Replace(str, @"[áàảạãăắằẳẵặâấầẩẫậ]", "a");
            str = Regex.Replace(str, @"[éèẻẽẹ]","e");
            str = Regex.Replace(str, @"[êếềểễệ]","e");
            str = Regex.Replace(str, @"[đ]", "d");
            str = Regex.Replace(str, @"[íìỉĩị]","i");
            str = Regex.Replace(str, @"[óòỏõọ]","o");
            str = Regex.Replace(str, @"[ôốồổỗộ]","o");
            str = Regex.Replace(str, @"[ơớờởỡợ]","o");
            str = Regex.Replace(str, @"[ýỳỷỹỵ]","y");
            str = Regex.Replace(str, @"[úùủũụ]", "u");
            str = Regex.Replace(str, @"[ưứừửữự]", "u");
            
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
    }
}
