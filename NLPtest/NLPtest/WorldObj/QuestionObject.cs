﻿

namespace NLPtest.WorldObj
{
    internal class QuestionObject : WorldObject
    {
        private QuestionType isIt;
        private QuestionType questionType;
        private string word;

        public QuestionObject(string word)
        {
            this.word = word;
        }

        public QuestionObject(QuestionType isIt)
        {
            this.isIt = isIt;
        }


        

      public enum QuestionType
        { What, When, Why, Whom, Where, HowMatch, How, IsIt };
    }
}