using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCTools.Helper
{
    public static class CommonHelper
    {
        public static String[,] convertListAnswerTo2DArray(List<Answer> answers)
        {
            int nRow = answers.Count + 1;
            int nCol = answers[0].rightAnswers.Count + 1;
            String[,] res = new String[nRow, nCol];
            res[0, 0] = "Mã đề";
            for (int i = 1; i < nCol; i++)
            {
                res[0, i] = i.ToString();
            }
            for (int i = 1; i < nRow; i++)
            {
                res[i, 0] = answers[i - 1].code;
                for (int j = 1; j < nCol; j++)
                {
                    res[i, j] = answers[i - 1].rightAnswers[j - 1].ToString();
                }
            }
            return res;
        }

        public static String[,] convertListListProcessedAsTo2DArray(List<ProcessedAS> lstProcessedAS)
        {
            int nOfQuestion = Globals.numberOfQuestion;

            int nRow = lstProcessedAS.Count+1;
            int nCol = nOfQuestion + 2;

            String[,] res = new String[nRow, nCol];

            res[0, 0] = "SBD";
            res[0, 1] = "Mã đề";
            for (int i = 1; i <= nOfQuestion; i++)
            {
                res[0, i+1] = i.ToString();
            }
            ProcessedAS tmp;
            List<StudentAnswer> answers;
            for (int i = 0; i < lstProcessedAS.Count; i++)
            {
                tmp = lstProcessedAS[i];
                res[i+1, 0] = tmp.studentId;
                res[i+1, 1] = tmp.examId;
                answers = tmp.answers;
                for (int j = 0; j < answers.Count; j++)
                {
                    res[i+1, j + 2] = answers[j].answerText.ToString();
                }
            }
            return res;
        }

        public static String[,] convertListStudentTo2DArray(List<Student> students)
        {
            int nRow = students.Count + 1;
            int nCol = 5;

            String[,] res = new String[nRow, nCol];
            res[0, 0] = "SBD";
            res[0, 1] = "Họ đệm";
            res[0, 2] = "Tên";
            res[0, 3] = "Ngày sinh";
            res[0, 4] = "Giới tính";

            for (int i = 1; i <= students.Count; i++)
            {
                res[i, 0] = students[i - 1].code;
                res[i, 1] = students[i - 1].lastName;
                res[i, 2] = students[i - 1].firstName;
                res[i, 3] = students[i - 1].dob;
                res[i, 4] = students[i - 1].gender;
            }
            return res;
        }

        public static string[,] getReport(DataGridView dgvListStudent, DataGridView dgvEditing, DataGridView dgvAnswer)
        {
            int nOfQuestion = Globals.numberOfQuestion;

            int nRow = dgvListStudent.Rows.Count;
            int nCol = nOfQuestion + 12;

            String[,] res = new String[nRow, nCol];
            String[] errorMessages = new String[dgvEditing.Rows.Count];
            List<String> lstStudentId = new List<String>();

            res[0, 0] = "SBD";
            res[0, 1] = "Mã đề";
            res[0, 2] = "Họ đệm";
            res[0, 3] = "Tên";
            res[0, 4] = "Ngày sinh";
            res[0, 5] = "Giới tính";
            for (int i = 1; i <= nOfQuestion; i++)
            {
                res[0, i + 5] = i.ToString();
            }
            res[0, nOfQuestion + 6] = "Số câu đúng";
            res[0, nOfQuestion + 7] = "Số câu sai";
            res[0, nOfQuestion + 8] = "Số câu không tô";
            res[0, nOfQuestion + 9] = "Số câu tô lỗi";
            res[0, nOfQuestion + 10] = "Điểm";
            res[0, nOfQuestion + 11] = "Ghi chú";

            for (int i = 1; i < dgvListStudent.Rows.Count; i++)
            {
                int wrongAnswer = 0;
                int rightAnswer = 0;
                int noAnswer = 0;
                int invalidAnswer = 0;
                res[i, 0] = dgvListStudent.Rows[i].Cells[0].Value.ToString();
                res[i, 2] = dgvListStudent.Rows[i].Cells[1].Value.ToString();
                res[i, 3] = dgvListStudent.Rows[i].Cells[2].Value.ToString();
                res[i, 4] = dgvListStudent.Rows[i].Cells[3].Value.ToString();
                res[i, 5] = dgvListStudent.Rows[i].Cells[4].Value.ToString();

                bool found = false;
                
                for (int j = 1; j < dgvEditing.Rows.Count; j++)
                {
                    if (dgvEditing.Rows[j].Cells[0].Style.BackColor != Color.Red)
                    {
                        String studentID = dgvEditing.Rows[j].Cells[0].Value.ToString();
                        if (studentID == res[i, 0])
                        {
                            found = true;

                            for (int k = 1; k <= nOfQuestion; k++)
                            {
                                String value = dgvEditing.Rows[j].Cells[k + 1].Value.ToString();
                                res[i, k + 5] = value;
                            }

                            String examID = dgvEditing.Rows[j].Cells[1].Value.ToString();
                            res[i, 1] = examID;
                            if (dgvEditing.Rows[j].Cells[1].Style.BackColor == Color.Red)
                            {
                                res[i, nOfQuestion + 11] = "Mã đề tô lỗi";
                            }
                            else
                            {
                                int expectedAnsRow = -1;
                                for (int k = 0; k < dgvAnswer.Rows.Count; k++)
                                {
                                    if (examID == dgvAnswer.Rows[k].Cells[0].Value.ToString())
                                    {
                                        expectedAnsRow = k;
                                        break;
                                    }
                                }
                                if (expectedAnsRow == -1)
                                {
                                    res[i, nOfQuestion + 11] = "Mã đề không tồn tại";
                                }
                                else
                                {
                                    for (int k = 1; k <= nOfQuestion; k++)
                                    {
                                        String value = dgvEditing.Rows[j].Cells[k + 1].Value.ToString();
                                        if (value == "-")
                                        {
                                            invalidAnswer++;
                                        }
                                        else if (value == "*")
                                        {
                                            noAnswer++;
                                        }
                                        else
                                        {
                                            if (value == dgvAnswer.Rows[expectedAnsRow].Cells[k].Value.ToString())
                                            {
                                                rightAnswer++;
                                            }
                                            else
                                            {
                                                wrongAnswer++;
                                            }
                                        }
                                        
                                    }
                                }
                                
                            }
                            break;
                        }    
                    }
                }
                if (!found)
                {
                    res[i, nOfQuestion + 11] = "Không có bài thi";
                }
                else
                {
                    res[i, nOfQuestion + 6] = rightAnswer.ToString();
                    res[i, nOfQuestion + 7] = wrongAnswer.ToString();
                    res[i, nOfQuestion + 8] = noAnswer.ToString() ;
                    res[i, nOfQuestion + 9] = invalidAnswer.ToString();
                    double score = (1.0 * rightAnswer / (nOfQuestion * 1.0)) * 10.0;
                    score = (Math.Floor(score * 4.0)) / 4.0;
                    res[i, nOfQuestion + 10] =  string.Format("{0:N2}", score);
                }
            }
            return res;
        }
    }
}
    