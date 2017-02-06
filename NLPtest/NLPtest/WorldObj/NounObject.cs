﻿
using System;

namespace NLPtest.WorldObj
{
 
    public class NounObject : WorldObject
    {

 
        public NounObject(string word) : base (word)
        {

        }

        public override IWorldObject Clone()
        {
            NounObject res = new NounObject(Word);
            cloneBase(res);
            return res;
        }
    }
}