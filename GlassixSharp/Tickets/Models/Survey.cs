using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GlassixSharp.Tickets.Models
{
    public class Survey
    {
        public int id { get; set; }
        /// <summary>
        /// Questions are part of a survey(which cotains questions)
        /// </summary>
        public int surveyId { get; set; }
        public string title { get; set; }
        public string value { get; set; } // 1-5
        /// <summary>
        /// The title of the parent. [Intro] column from[surveysHeaders] table
        /// </summary>
        public string headerTitle { get; set; }
        /// <summary>
        /// Currentlly we only use rating
        /// </summary>

        public bool isDisabled { get; set; }

        public bool isMandatory { get; set; }

        public QuestionType questionType { get; set; }

        public enum QuestionType
        {
            Undefined = 0,
            Rating = 1,
            Text = 2,
        }
    }
}
