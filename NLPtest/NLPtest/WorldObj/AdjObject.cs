﻿

namespace NLPtest.WorldObj
{
    internal class AdjObject : WorldObject
    {


        public AdjObject()
        {
    
        }

        public AdjObject(string word) : base(word)
        {
          
        }

        public override IWorldObject Clone()
        {
            AdjObject res = new AdjObject(Word);
            cloneBase(res);
            return res;
        }

    }
}