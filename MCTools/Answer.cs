using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTools
{
    public class Answer
    {
        public int subjectId;
        public int examId;
        public List<Char> rightAnswers { get; set; }
        public String code { get; set; }

        public Answer()
        {
            rightAnswers = new List<Char>();   
        }

        public Answer(int id, String code, String answers, int subjectId, int examId)
        {
            this.rightAnswers = new List<Char>();
            this.code = code;
            this.subjectId = subjectId;
            this.examId = examId;
            char c;
            for (int i = 0; i < answers.Length; i++)
            {
                c = answers[i];
                this.rightAnswers.Add(c);
            }
        }
    }
}
