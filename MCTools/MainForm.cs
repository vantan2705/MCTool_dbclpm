using AnswerSheetProcess;
using Emgu.CV;
using Emgu.CV.Structure;
using MCTools.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCTools
{
    public partial class MainForm : Form
    {
        int currentStep;
        List<ProcessedAS> lstProcessedAS;
        string[,] errorMessages;
        public bool isCancel { get; set; }
        public List<Student> students {get; set;}
        public List<Answer> answers { get; set; }
        bool containsDuplicate = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            currentStep = 1;
            students = new List<Student>();
            answers = new List<Answer>();
            ChangeUIForNextStep();
        }

        private void resetUI()
        {
            pbStep1.Cursor = Cursors.Default;
            pbStep2.Cursor = Cursors.Default;
            pbStep3.Cursor = Cursors.Default;
            pbStep4.Cursor = Cursors.Default;
            pbStep5.Cursor = Cursors.Default;
            pbStep6.Cursor = Cursors.Default;

            pbStep1.Image = Properties.Resources.Step1___Gray;
            pbStep2.Image = Properties.Resources.Step2___Gray;
            pbStep3.Image = Properties.Resources.Step3___Gray;
            pbStep4.Image = Properties.Resources.Step4___Gray;
            pbStep5.Image = Properties.Resources.Step5___Gray;
            pbStep6.Image = Properties.Resources.Step6___Gray;
                    
            currentStep = 1;
            students = new List<Student>();
            answers = new List<Answer>();
            dgvAnswer.Rows.Clear();
            dgvDetected.Rows.Clear();
            dgvEditing.Rows.Clear();
            dgvListStudent.Rows.Clear();
            dgvReport.Rows.Clear();
            ChangeUIForNextStep();
            tabControl1.SelectedIndex = 0;
        }

        private void ChangeUIForNextStep()
        {
            switch (currentStep)
            {
                case 1:
                    btnNextStep.Visible = false;
                    lbNextStep.Visible = false;
                    pbStep1.Cursor = Cursors.Hand;
                    pbStep1.Enabled = true;
                    pbStep1.Image = Properties.Resources.Step1___Red;
                    break;
                case 2:
                    pbStep1.Cursor = Cursors.Default;
                    pbStep1.Enabled = false;
                    pbStep2.Enabled = true;
                    pbStep1.Image = Properties.Resources.Step1___Green;
                    pbStep2.Cursor = Cursors.Hand;
                    pbStep2.Image = Properties.Resources.Step2___Red;
                    break;
                case 3:
                    pbStep2.Cursor = Cursors.Default;
                    pbStep2.Enabled = false;
                    pbStep2.Image = Properties.Resources.Step2___Green;
                    pbStep3.Cursor = Cursors.Hand;
                    pbStep3.Image = Properties.Resources.Step3___Red;
                    break;
                case 4:
                    btnNextStep.Visible = true;
                    lbNextStep.Text = "Bước tiếp theo";
                    lbNextStep.Visible = true;
                    pbStep3.Cursor = Cursors.Default;
                    tabControl1.SelectedIndex = 3;
                    pbStep3.Enabled = false;
                    pbStep3.Image = Properties.Resources.Step3___Green;
                    pbStep4.Image = Properties.Resources.Step4___Red;
                    break;
                case 5:
                    lbErrorMessage.Visible = false;
                    tabControl1.SelectedIndex = 4;
                    pbStep4.Enabled = false;
                    pbStep4.Image = Properties.Resources.Step4___Green;
                    pbStep5.Cursor = Cursors.Hand;
                    pbStep5.Image = Properties.Resources.Step5___Red;
                    break;
                case 6:
                    lbNextStep.Text = "Hoàn tất";
                    pbStep5.Cursor = Cursors.Default;
                    pbStep5.Enabled = false;
                    pbStep5.Image = Properties.Resources.Step5___Green;
                    pbStep6.Cursor = Cursors.Hand;
                    pbStep6.Image = Properties.Resources.Step6___Red;
                    break;
                case 7:
                    pbStep6.Cursor = Cursors.Default;
                    pbStep6.Enabled = false;
                    pbStep6.Image = Properties.Resources.Step6___Green;
                    break;
            }
        }


        private void NextStep()
        {
            
            currentStep++;
            if (currentStep == 5 && containsDuplicate)
            {
                MessageBox.Show("Vui lòng xử lý trường hợp sinh viên có số báo danh bị trùng để tiếp tục!!");
                currentStep--;
                return;
            }
            Invoker.InvokeIfRequired(dgvAnswer, new MethodInvoker(() =>
            {
                ChangeUIForNextStep();
            }));

            if (currentStep == 5)
            {
                
                createReport();
            }
            else if (currentStep == 6)
            {
                ReportHelper.exportReport(dgvDetected, dgvEditing, dgvReport, dgvDetected);
                ReportHelper.syncDataToServer(dgvReport, students);
                MessageBox.Show("Báo cáo đang được xuất");
                currentStep++;
                ChangeUIForNextStep();
                currentStep--;
                lbNextStep.Text = "Bắt đầu lại";
            }
            else if (currentStep == 7)
            {
                resetUI();
            }
        }

        private void createReport()
        {
            String[,] reportData = CommonHelper.getReport(dgvListStudent, dgvEditing, dgvAnswer);
            Invoker.InvokeIfRequired(dgvReport, new MethodInvoker(() =>
            {
                fillDataToGridView(dgvReport, reportData);
            }));
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        private void pbStep1_Click(object sender, EventArgs e)
        {
            isCancel = true;
            Form frm = new SetupTool(this);
            frm.ShowDialog();

            if (!isCancel)
            {
                String[,] answerData = CommonHelper.convertListAnswerTo2DArray(answers);
                String[,] studentData = CommonHelper.convertListStudentTo2DArray(students);

                Invoker.InvokeIfRequired(dgvAnswer, new MethodInvoker(() =>
                {
                    fillDataToGridView(dgvAnswer, answerData);
                }));

                Invoker.InvokeIfRequired(dgvListStudent, new MethodInvoker(() =>
                {
                    fillDataToGridView(dgvListStudent, studentData);
                }));
                
                

                NextStep();
            }
            
        }

        private void fillDataToGridView(DataGridView dgv, String[,] data)
        {
            int nRow = data.GetLength(0);
            int nCol = data.GetLength(1);
            Invoker.InvokeIfRequired(dgvListStudent, new MethodInvoker(() =>
            {
                dgv.ColumnCount = nCol;
                for (int i = 0; i < nCol; i++)
                {
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }));
            for (int i = 0; i < nRow; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv);
                for (int j = 0; j < nCol; j++)
                {
                    row.Cells[j].Value = data[i, j];
                }
                Invoker.InvokeIfRequired(dgv, new MethodInvoker(() =>
                {
                    dgv.Rows.Add(row);
                }));
            }
        }

        private void pbStep2_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            Config cfg = new Config();
            String path = cfg.Get(Config.ANSWER_SHEET_IMG_PATH, "");
            if (path != "") {
                openFileDialog1.InitialDirectory = path;
            }
            openFileDialog1.Title = "Chọn danh sách ảnh bài thi";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "dat Files (*.dat)|*.dat";
            openFileDialog1.DefaultExt = "dat";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                byte[] imageBytes = File.ReadAllBytes(filePath);
                if (SigningHelper.VerifyImages(imageBytes))
                {
                    List<Bitmap> imgs = SigningHelper.GetImagesFromEncryptedBytes(imageBytes);
                    bgImageProcessing.RunWorkerAsync(imgs);
                    processProgressBar.Visible = true;
                    lbProcessProgressDescription.Visible = true;
                }
                else
                {
                    MessageBox.Show("File ảnh bài thi bị lỗi!!");
                }
            }
        }

        private void pbStep3_Click(object sender, EventArgs e)
        {
            errorMessages = new string[dgvEditing.Rows.Count, dgvEditing.Columns.Count];
            for (int i = 1; i < dgvEditing.Rows.Count; i++)
            {
                for (int j = 2; j < dgvEditing.Columns.Count; j++)
                {
                    if (dgvEditing.Rows[i].Cells[j].Value != null)
                    {
                        Invoker.InvokeIfRequired(dgvEditing, new MethodInvoker(() =>
                        {
                            dgvEditing.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            if (dgvEditing.Rows[i].Cells[j].Value.ToString().Equals("*"))
                            {
                                dgvEditing.Rows[i].Cells[j].Style.BackColor = dgvDetected.Rows[i].Cells[j].Style.BackColor = Color.SkyBlue;
                                dgvEditing.Rows[i].Cells[j].Style.ForeColor = dgvDetected.Rows[i].Cells[j].Style.ForeColor = Color.White;
                                errorMessages[i, j] = "Không xác định được đáp án (Không có đáp án nào được tô)";
                            }
                            else if (dgvEditing.Rows[i].Cells[j].Value.ToString().Equals("-"))
                            {
                                dgvEditing.Rows[i].Cells[j].Style.BackColor = dgvDetected.Rows[i].Cells[j].Style.BackColor = Color.Red;
                                dgvEditing.Rows[i].Cells[j].Style.ForeColor = dgvDetected.Rows[i].Cells[j].Style.ForeColor = Color.White;
                                errorMessages[i, j] = "Không xác định được đáp án (Bị tô nhiều hơn 1 đáp án)";
                            }
                        }));
                    }
                    
                }
            }

            for (int i = 1; i < dgvEditing.Rows.Count; i++ )
            {
                for (int j = 0; j < 2; j++)
                {
                    if (dgvEditing.Rows[i].Cells[j].Value != null)
                    {
                        if (dgvEditing.Rows[i].Cells[j].Value.ToString().Contains("*") || dgvEditing.Rows[i].Cells[j].Value.ToString().Contains("-"))
                        {
                            Invoker.InvokeIfRequired(dgvEditing, new MethodInvoker(() =>
                            {
                                dgvEditing.Rows[i].Cells[j].Style.BackColor = dgvDetected.Rows[i].Cells[j].Style.BackColor = Color.Red;
                                dgvEditing.Rows[i].Cells[j].Style.ForeColor = dgvDetected.Rows[i].Cells[j].Style.ForeColor = Color.White;

                                if (j == 0) errorMessages[i, j] = "Không nhận diện được SBD";
                                else errorMessages[i, j] = "Không nhận diện được mã đề";
                            }));
                        }
                    }
                }
            }

            string currentStudentId;
            for (int i = 1; i < dgvEditing.Rows.Count-1; i++)
            {
                if (dgvEditing.Rows[i].Cells[0].Value != null)
                {
                    currentStudentId = dgvEditing.Rows[i].Cells[0].Value.ToString();
                    for (int j = i + 1; j < dgvEditing.Rows.Count; j++)
                    {
                        if (dgvEditing.Rows[j].Cells[0].Value != null)
                        {
                            if (dgvEditing.Rows[j].Cells[0].Value.ToString().Equals(currentStudentId))
                            {
                                Invoker.InvokeIfRequired(dgvEditing, new MethodInvoker(() =>
                                {
                                    dgvEditing.Rows[j].Cells[0].Style.BackColor = dgvDetected.Rows[j].Cells[0].Style.BackColor = Color.Red;
                                    dgvEditing.Rows[j].Cells[0].Style.ForeColor = dgvDetected.Rows[j].Cells[0].Style.ForeColor = Color.White;


                                    dgvEditing.Rows[i].Cells[0].Style.BackColor = dgvDetected.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgvEditing.Rows[i].Cells[0].Style.ForeColor = dgvDetected.Rows[i].Cells[0].Style.ForeColor = Color.White;
                                    errorMessages[i, 0] = "Số báo danh bị trùng";
                                    containsDuplicate = true;
                                }));
                            }
                        }
                    }
                }
            }
            NextStep();
        }

        string editingValue;

        private void dgvEditing_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editingValue = dgvEditing.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

        }

        private void dgvEditing_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Invoker.InvokeIfRequired(dgvEditing, new MethodInvoker(() =>
            {
                if (dgvEditing.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != editingValue)
                {
                        dgvEditing.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                        dgvEditing.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                
                }
                int ind=0, cnt=0;
                if (e.ColumnIndex == 0) {
                    for (int i = 1; i < dgvEditing.Rows.Count; i++)
                    {
                        if (i != e.RowIndex && dgvEditing.Rows[i].Cells[0].Value.ToString() == editingValue)
                        {
                            cnt++;
                            ind = i;
                        }
                    }
                }
                if (cnt == 1)
                {
                    dgvEditing.Rows[ind].Cells[0].Style.BackColor = Color.White;
                    dgvEditing.Rows[ind].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                    containsDuplicate = false;
                }
            }));
        }

        #region Moving Answer Sheet
        int xPos, yPos;
        private bool dragging;
        private void pbAnswerSheet_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                xPos = e.X;
                yPos = e.Y;
                dragging = true;
            }
        }

        private void pbAnswerSheet_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging && e.Button == MouseButtons.Left)
            {
                this.pbAnswerSheet.Left += (e.X - xPos);
                this.pbAnswerSheet.Top += (e.Y - yPos);
            }
        }

        private void pbAnswerSheet_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            if (this.pbAnswerSheet.Left > 0)
            {
                this.pbAnswerSheet.Left = 0;
            }
            if (this.pbAnswerSheet.Top > 0)
            {
                this.pbAnswerSheet.Top = 0;
            }
        }

        #endregion

        AnswerSheetProcess.Point drawPoint;
        int drawType;
        Bitmap currentStudentAnswerImage;
        bool shouldPaint = false;

        private void dgvEditing_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0 && e.RowIndex <= lstProcessedAS.Count)
            {
                ProcessedAS processedAS = lstProcessedAS[e.RowIndex-1];
                currentStudentAnswerImage = lstProcessedAS[e.RowIndex - 1].processedAnswerSheetImage;
                pbAnswerSheet.Image = currentStudentAnswerImage;

                shouldPaint = true;
                if (e.ColumnIndex >= 2)
                {
                    drawPoint = processedAS.answers[e.ColumnIndex - 2].questionPosition;
                    drawType = Constant.DRAW_RECT_ON_ANSWER;
                    pbAnswerSheet.Left = -drawPoint.x + 30;
                    pbAnswerSheet.Top = -drawPoint.y + 250;
                }
                else if (e.ColumnIndex == 0)
                {
                    drawPoint = new AnswerSheetProcess.Point(1111, 260);
                    drawType = Constant.DRAW_RECT_ON_STUDENT_ID;
                    pbAnswerSheet.Left = -drawPoint.x + 50;
                    pbAnswerSheet.Top = -drawPoint.y + 240;
                }
                else if (e.ColumnIndex == 1)
                {
                    drawPoint = new AnswerSheetProcess.Point(1370, 260);
                    drawType = Constant.DRAW_RECT_ON_EXAM_ID;
                    pbAnswerSheet.Left = -drawPoint.x + 50;
                    pbAnswerSheet.Top = -drawPoint.y + 240;
                }
                else
                {
                    shouldPaint = false;
                }
                lbErrorMessage.Visible = true;
                lbErrorMessage.Text = errorMessages[e.RowIndex, e.ColumnIndex];
            }
        }



        #region Image Zooming
        private double[] zoomFactor = { .25, .50, .75, 1, 1.25, 1.5, 1.75, 2.0, 2.5, 3.0 };
        private int zoomIndex1 = 3;
        private void pbAnswerSheet_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                int orgX = 0; //Transforms the location of mouse to the point at 100% resolution 
                int orgY = 0;

                if (zoomFactor[zoomIndex1] != 1)
                {
                    orgX = Convert.ToInt32(e.X / zoomFactor[zoomIndex1]);
                    orgY = Convert.ToInt32(e.Y / zoomFactor[zoomIndex1]);
                }
                if (zoomFactor[zoomIndex1] == 1)
                {
                    orgX = e.X;
                    orgY = e.Y;
                }

                if (e.Delta > 0)
                {
                    if (zoomIndex1 < 7)
                        zoomIndex1++;
                    int zoom1 = Convert.ToInt32(zoomFactor[zoomIndex1] * 100);
                    pbAnswerSheet.Image = ZoomingControl(currentStudentAnswerImage, zoomIndex1);
                    //using centre of picturebox to bring the the centre of the image in focus(centre of) of the client area(panel)
                    int X = (int)((this.pbAnswerSheet.Width / 2) - this.splitContainer.Panel1.Width / 2);
                    int Y = (int)((this.pbAnswerSheet.Height / 2) - this.splitContainer.Panel1.Height / 2 + e.Delta);

                    //using the mouse location to zoom
                    //int X = (int)((orgX * zoomFactor[zoomIndex1]) - this.mainContainer.Panel1.Width / 2);
                    //int Y = (int)((orgY * zoomFactor[zoomIndex1]) - this.mainContainer.Panel1.Height / 2 + e.Delta);
                    this.splitContainer.Panel1.AutoScrollPosition = new System.Drawing.Point(X, Y);

                }
                else if (e.Delta < 0)
                {
                    if (zoomIndex1 > 0)
                        zoomIndex1--;
                    int zoom1 = Convert.ToInt32(zoomFactor[zoomIndex1] * 100);
                    pbAnswerSheet.Image = ZoomingControl(currentStudentAnswerImage, zoomIndex1);
                    // using centre of picturebox to bring the the centre of the image in focus(centre of) of the client area(panel)
                    int X = (int)((this.pbAnswerSheet.Width / 2) - this.splitContainer.Panel1.Width / 2);
                    int Y = (int)((this.pbAnswerSheet.Height / 2) - this.splitContainer.Panel1.Height / 2 + e.Delta);

                    //using the mouse location to zoom
                    //int X = (int)((orgX * zoomFactor[zoomIndex1]) - this.mainContainer.Panel1.Width / 2);
                    //int Y = (int)((orgY * zoomFactor[zoomIndex1]) - this.mainContainer.Panel1.Height / 2 + e.Delta);
                    this.splitContainer.Panel1.AutoScrollPosition = new System.Drawing.Point(X, Y);
                    this.pbAnswerSheet.Left = 0;
                    this.pbAnswerSheet.Top = 0;
                }
            }
        }

        private Bitmap ZoomingControl(Bitmap currentStudentAnswerImage, int zoomIndex)
        {
            Bitmap bm = new Bitmap(currentStudentAnswerImage, Convert.ToInt32(currentStudentAnswerImage.Width * zoomFactor[zoomIndex]), Convert.ToInt32(currentStudentAnswerImage.Height * zoomFactor[zoomIndex]));
            return bm;
        }

        #endregion

        private void pbAnswerSheet_Paint(object sender, PaintEventArgs e)
        {
            if (shouldPaint)
            {
                Pen pen = new Pen(Color.Red, 2);
                Graphics g = e.Graphics;
                Rectangle rect;
                if (drawType == Constant.DRAW_RECT_ON_STUDENT_ID)
                {
                    rect = new Rectangle(drawPoint.x, drawPoint.y + 25, 250, 550);
                }
                else if (drawType == Constant.DRAW_RECT_ON_EXAM_ID)
                {
                    rect = new Rectangle(drawPoint.x-30, drawPoint.y + 25, 160, 550);
                }
                else
                {
                    rect = new Rectangle(drawPoint.x, drawPoint.y, 300, 38);
                }
                g.DrawRectangle(pen, rect);

                pen.Dispose();
            }
        }

        private void bgImageProcessing_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Bitmap> images = (List<Bitmap>) e.Argument;

            Globals.numberOfAnswerSheet = images.Count;
            int i = 1;
            lstProcessedAS = new List<ProcessedAS>();
            foreach (Bitmap bmp in images)
            {
                ProcessedAS processedAS = ASProcessor.ProcessAS(bmp);
                lstProcessedAS.Add(processedAS);
                int percentages = i * 100 / images.Count;
                bgImageProcessing.ReportProgress(percentages);
                i++;
            }
            String[,] data = CommonHelper.convertListListProcessedAsTo2DArray(lstProcessedAS);

            fillDataToGridView(dgvDetected, data);
            fillDataToGridView(dgvEditing, data);
        }

        private void bgImageProcessing_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbProcessProgressDescription.Text = String.Format("Đã xử lý {0}%", e.ProgressPercentage);
            processProgressBar.Value = e.ProgressPercentage;
        }

        private void bgImageProcessing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            processProgressBar.Visible = false;
            lbProcessProgressDescription.Visible = false;
            NextStep();
            pbStep3_Click(sender, e);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void TSMIInfoSetting_Click(object sender, EventArgs e)
        {
            EditPersonalInfo frm = new EditPersonalInfo();
            frm.ShowDialog();
        }

        private void TSMIOtherSetting_Click(object sender, EventArgs e)
        {
            Settings frm = new Settings();
            frm.ShowDialog();
        }

        private void TSMIRestart_Click(object sender, EventArgs e)
        {
            resetUI();
        }

        private void TSMIExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TSMILicense_Click(object sender, EventArgs e)
        {
            AboutMe frm = new AboutMe();
            frm.ShowDialog();
        }

    }
}
