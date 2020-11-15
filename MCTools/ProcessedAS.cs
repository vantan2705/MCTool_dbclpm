using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTools
{
    public class ProcessedAS
    {
        public Bitmap processedAnswerSheetImage {get; set;}
        public string studentId { get; set; }
        public string examId { get; set; }
        public List<StudentAnswer> answers { get; set; }

        public ProcessedAS()
        {

        }

        public ProcessedAS(Bitmap processedAnswerSheetImage, string studentId, string examId, List<StudentAnswer> answers)
        {
            this.processedAnswerSheetImage = processedAnswerSheetImage;
            this.studentId = studentId;
            this.examId = examId;
            this.answers = answers;
        }
    }
}
