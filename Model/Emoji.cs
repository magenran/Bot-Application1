﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Emoji
    {
        static Dictionary<string, string> d = new Dictionary<string, string>();

        static Emoji()
        {
            d.Add("grinning", "\U0001F600");
            d.Add("smiling", "\U0001F601");
            d.Add("confused", "\U0001F615");
            d.Add("frowning ", "\u2639");
            d.Add("crying", "\U0001F622");
            d.Add("fearful", "\U0001F628");
            d.Add("flushed", "\U0001F633");
            d.Add("halo", "\U0001F607");
            d.Add("horns", "\U0001F608");
            d.Add("poo", "\U0001F4A9");
            d.Add("unicorn", "\U0001F984");
            d.Add("penguin", "\U0001F427");
            d.Add("meat", "\U0001F356");
            d.Add("pizza", "\U0001F355");
            d.Add("cofee", "\u2615");
            d.Add("island", "\U0001F3DD");
            d.Add("medal1", "\U0001F947");
            d.Add("medal2", "\U0001F948");
            d.Add("medal3", "\U0001F949");
            d.Add("trophy", "\U0001F396");
            d.Add("musical ", "\U0001F3B6");
            d.Add("guitar", "\U0001F3B8");
            d.Add("Israel", "\U0001F1EE\U0001F1F1");

            d.Add("thinking", "\U0001F914");
            d.Add("monkey", "\U0001F648");
            d.Add("robot", "\U0001F916");
            d.Add("facepalming", "\U0001F929");
            d.Add("student", "\U0001F468\u200D\U0001F393");
            d.Add("dizzy", "\U0001F635");
            d.Add("sunglasses", "\U0001F60E");

            
        }


        public static string get(string key)
        {
            string val = "";
            d.TryGetValue(key,out val);
            return " " + val + " ";
        }


    }
}