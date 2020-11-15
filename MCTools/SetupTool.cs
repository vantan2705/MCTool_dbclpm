using AnswerSheetProcess;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCTools
{
    public partial class SetupTool : Form
    {
        MainForm parentForm;
        LoadingDialog loadingDialog;
        bool dataValid;
        public SetupTool()
        {
            InitializeComponent();
            cbbTemplate.SelectedIndex = 0;
            jobLoadExamInfo.RunWorkerAsync();
            loadingDialog = new LoadingDialog("Đang lấy thông tin...");
            loadingDialog.ShowDialog();
        }

        public SetupTool(MainForm parent)
        {
            dataValid = true;
            parentForm = parent;
            InitializeComponent();
            Config cfg = new Config();
            string path = cfg.Get("current_work_space");
            tb_WorkspacePath.Text = path;
            Globals.currentWorkspace = path;
            String templateIndex = cfg.Get(Config.CURRENT_TEMPLATE_INDEX);
            if (templateIndex != null)
            {
                cbbTemplate.SelectedIndex = Convert.ToInt32(templateIndex);
            }
            else
            {
                cbbTemplate.SelectedIndex = 0;
            }
            

            jobLoadExamInfo.RunWorkerAsync();
            loadingDialog = new LoadingDialog("Đang lấy thông tin...");
            loadingDialog.ShowDialog();   

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            parentForm.isCancel = true;
            this.Close();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (tb_WorkspacePath.Text == "")
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục làm việc");
            }
            else
            {
                if (cbbSubject.SelectedValue != null)
                {
                    parentForm.isCancel = false;
                    Globals.examName = cbbExam.Text.ToString();
                    Globals.subject = cbbSubject.Text.ToString();
                    Globals.subjectId = (int) cbbSubject.SelectedValue;
                    Globals.examId = (int) cbbExam.SelectedValue;
                    Globals.currentTemplate = TemplateUtils.GetTemplate(cbbTemplate.SelectedIndex);
                    jobLoadDataFromDB.RunWorkerAsync(new Tuple<int, int>((int)cbbExam.SelectedValue, (int)cbbSubject.SelectedValue));
                    loadingDialog = new LoadingDialog("Đang lấy thông tin...");
                    loadingDialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không có môn thi nào được chọn");
                }
            }
        }

        private void jobLoadDataFromDB_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<int, int> data = e.Argument as Tuple<int, int>;
            NpgsqlConnection connection = null;

            int examId = data.Item1;
            int subjectId = data.Item2;

            List<Student> lstStudents = new List<Student>();
            List<Answer> lstAnswers = new List<Answer>();

            try
            {
                String connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                connection = new NpgsqlConnection(connectionString);

                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.student where exam_id = @exam_id", connection);

                cmd.Parameters.Add(new NpgsqlParameter("exam_id", examId));

                NpgsqlDataReader reader = cmd.ExecuteReader();

                
                

                for (int i = 0; reader.Read(); i++)
                {
                    Student student = new Student(int.Parse(reader["id"].ToString()), reader["code"].ToString(), reader["lastname"].ToString(), reader["firstname"].ToString(), reader["dob"].ToString(), reader["gender"].ToString(), int.Parse(reader["exam_id"].ToString()));
                    lstStudents.Add(student);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Dispose();
            }

            try
            {
                String connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                connection = new NpgsqlConnection(connectionString);

                connection.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.answer where exam_id = @exam_id and subject_id = @subject_id", connection);

                cmd.Parameters.Add(new NpgsqlParameter("exam_id", examId));
                cmd.Parameters.Add(new NpgsqlParameter("subject_id", subjectId));

                NpgsqlDataReader reader = cmd.ExecuteReader();

                for (int i = 0; reader.Read(); i++)
                {
                    Answer answer = new Answer(int.Parse(reader["id"].ToString()), reader["test_code"].ToString(), reader["answers"].ToString(), int.Parse(reader["subject_id"].ToString()), int.Parse(reader["exam_id"].ToString()));
                    lstAnswers.Add(answer);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Dispose();
            }

            bool nOfQValid = true;
            if (lstAnswers.Count == 0)
            {
                dataValid = false;
                MessageBox.Show("Môn thi chưa được nhập đáp án");
                nOfQValid = false;
                parentForm.isCancel = true;
            } else if (lstStudents.Count == 0)
            {
                dataValid = false;
                MessageBox.Show("Chưa có sinh viên được thêm trong kỳ thi");
                nOfQValid = false;
                parentForm.isCancel = true;
            }
            
            if (nOfQValid) {
                dataValid = true;
                parentForm.students.AddRange(lstStudents);
                parentForm.answers.AddRange(lstAnswers);

                Globals.numberOfQuestion = lstAnswers[0].rightAnswers.Count;
                
            }
        }

        private void jobLoadDataFromDB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingDialog.Close();
            if (dataValid)
            {
                this.Close();
            }
        }

        private void btnSelectWorkspacePath_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tb_WorkspacePath.Text = fbd.SelectedPath;
                    Globals.currentWorkspace = fbd.SelectedPath;
                    Config cfg = new Config();
                    cfg.Set(Config.CURRENT_WORKSPACE, fbd.SelectedPath);
                    cfg.Save();
                }
            }
        }

        private void jobLoadExamInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable dtExam = new DataTable();
            //DataTable dtSubject = new DataTable();
            DataRow row;
            NpgsqlDataReader reader;
            NpgsqlCommand cmd;
            NpgsqlConnection connection = null;


            dtExam.Columns.Add("ID", typeof(int));
            dtExam.Columns.Add("DisplayMember");
            try
            {
                String connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                connection = new NpgsqlConnection(connectionString);

                connection.Open();

                cmd = new NpgsqlCommand("SELECT * FROM public.exam", connection);

                reader = cmd.ExecuteReader();

                for (int i = 0; reader.Read(); i++)
                {
                    row = dtExam.NewRow();
                    row["ID"] = reader["id"].ToString();
                    row["DisplayMember"] = reader["name"].ToString();

                    dtExam.Rows.Add(row);
                }

                Invoker.InvokeIfRequired(cbbExam, new MethodInvoker(() =>
                {
                    cbbExam.DisplayMember = "DisplayMember";
                    cbbExam.ValueMember = "ID";
                    cbbExam.DataSource = dtExam;
                    cbbExam.SelectedIndex = 0;
                }));

                connection.Close();

                //loadSubjectInfo((int)cbbExam.SelectedValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Dispose();
            }

            

        }

        private void jobLoadExamInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingDialog.Close();
        }

        private void loadSubjectInfo(int examId)
        {
            DataTable dtSubject = new DataTable();
            DataRow row;
            NpgsqlDataReader reader;
            NpgsqlCommand cmd;
            NpgsqlConnection connection = null;


            dtSubject.Columns.Add("ID", typeof(int));
            dtSubject.Columns.Add("DisplayMember");
            try
            {
                String connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                connection = new NpgsqlConnection(connectionString);

                connection.Open();

                cmd = new NpgsqlCommand("SELECT s.name, s.id FROM public.subject s, public.exam_subject es where es.exam_id = @exam_id and s.id = es.subject_id", connection);

                cmd.Parameters.Add(new NpgsqlParameter("exam_id", examId));
                reader = cmd.ExecuteReader();

                for (int i = 0; reader.Read(); i++)
                {
                    row = dtSubject.NewRow();
                    row["ID"] = reader["id"].ToString();
                    row["DisplayMember"] = reader["name"].ToString();

                    dtSubject.Rows.Add(row);
                }
                Invoker.InvokeIfRequired(cbbSubject, new MethodInvoker(() =>
                {
                    cbbSubject.DisplayMember = "DisplayMember";
                    cbbSubject.ValueMember = "ID";
                    cbbSubject.DataSource = dtSubject;
                    cbbSubject.SelectedIndex = 0;
                }));

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Dispose();
            }
        }


        private void jobLoadSubjectInfo_DoWork(object sender, DoWorkEventArgs e)
        {

            int data = (int) e.Argument;

            loadSubjectInfo(data);
        }


        private void btnGetSubject_Click(object sender, EventArgs e)
        {
            jobLoadSubjectInfo.RunWorkerAsync(cbbExam.SelectedValue);
        }

        private void cbbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadSubjectInfo((int)cbbExam.SelectedValue);
        }

        private void cbbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config cfg = new Config();
            cfg.Set(Config.CURRENT_TEMPLATE_INDEX, cbbTemplate.SelectedIndex);
            cfg.Save();
        }
    }
}
