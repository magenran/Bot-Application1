﻿using Bot_Application1.dataBase;
using System;
using System.Collections.Generic;

namespace Bot_Application1.IDialog
{
 
    internal static class EducationController
    {
      
        internal static string[] getStudyUnits()
        {

            return new string[]
            {
               "היסטוריה א'","היסטוריה ב'"
            };
        }

        internal static IEnumerable<string> getStudyCategory(string unit)
        {
            return new string[]
            {
               "בית שני","ציונות"
            };
        }

        internal static string[] getQuestion(string studySubject)
        {
            return new string[]
            {
               "שאלה כלשהי"
            };
        }
    }
}