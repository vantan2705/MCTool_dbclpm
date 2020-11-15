using AnswerSheetProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTools
{
    public class StudentAnswer
    {
        public Char answerText { get; set; }
        public Point questionPosition { get; set; }

        public StudentAnswer()
        {

        }

        public StudentAnswer(Char answerText, Point questionPosition)
        {
            this.answerText = answerText;
            this.questionPosition = questionPosition;
        }
    }
}
