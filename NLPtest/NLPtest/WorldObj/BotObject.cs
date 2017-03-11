﻿using NLP.WorldObj;

namespace NLP.view
{

   
    internal class BotObject : PersonObject
    {
        public BotObject(string word) : base(word)
        {
        }

        public override IWorldObject Clone()
        {
            BotObject res = new BotObject(Word);
            res.Copy(this);
            return res;
        }



    }
}