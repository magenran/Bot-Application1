//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model.dataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class userScore
    {
        public string userID { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public Nullable<int> Score { get; set; }
        public System.DateTime dateTime { get; set; }
        public string QuestionID { get; set; }
        public string SubquestionID { get; set; }
    }
}