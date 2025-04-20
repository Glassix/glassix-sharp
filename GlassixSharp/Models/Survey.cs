using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GlassixSharp.Models
{
    public class Survey
    {
        public int id;
        /// <summary>
        /// Questions are part of a survey(which cotains questions)
        /// </summary>
        public int surveyId;
        public string title;
        public string value; // 1-5
        /// <summary>
        /// The title of the parent. [Intro] column from[surveysHeaders] table
        /// </summary>
        public string headerTitle;
        /// <summary>
        /// Currentlly we only use rating
        /// </summary>

        public bool isDisabled;

        public bool isMandatory;

        public QuestionType questionType;

        public enum QuestionType
        {
            Undefined = 0,
            Rating = 1,
            Text = 2,
        }
    }
}
