using MCTools.Helper;
using Microsoft.Office.Interop.Excel;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel; 

namespace MCTools
{
    public static class ReportHelper
    {
        private static bool exportEditedReportComplete;
        private static bool exportPreEditedReportComplete;
        private static bool exportFinalReportComplete;
        private static bool exportManualEditReportComplete;
        private static string username = "api_admin";
        private static string password = "0RbK9DsFyBSJvI8kxedz";
        private static string editedExcelFileName = "";

        public static void exportReport(DataGridView detected, DataGridView manualEdited, DataGridView editedReport, DataGridView preEditingReport)
        {

            exportEditedReportComplete = false;
            exportPreEditedReportComplete = false;
            exportFinalReportComplete = false;
            exportManualEditReportComplete = false;

            BackgroundWorker exportEditedReportBW = new BackgroundWorker();
            exportEditedReportBW.DoWork += exportEditedReport_DoWork;
            exportEditedReportBW.RunWorkerCompleted += exportEditedReport_RunWorkerCompleted;
            exportEditedReportBW.RunWorkerAsync(editedReport);

            BackgroundWorker exportPreEditedReportBW = new BackgroundWorker();
            exportPreEditedReportBW.DoWork += exportPreEditedReport_DoWork;
            exportPreEditedReportBW.RunWorkerCompleted += exportPreEditedReport_RunWorkerCompleted;
            exportPreEditedReportBW.RunWorkerAsync(preEditingReport);

            BackgroundWorker exportFinalReportBW = new BackgroundWorker();
            exportFinalReportBW.DoWork += exportFinalReport_DoWork;
            exportFinalReportBW.RunWorkerCompleted += exportFinalReport_RunWorkerCompleted;
            exportFinalReportBW.RunWorkerAsync(editedReport);

            BackgroundWorker exportManualEditReportBW = new BackgroundWorker();
            exportManualEditReportBW.DoWork += exportManualEditReport_DoWork;
            exportManualEditReportBW.RunWorkerCompleted += exportManualEditReport_RunWorkerCompleted;
            exportManualEditReportBW.RunWorkerAsync(new Tuple<DataGridView, DataGridView>(detected, manualEdited));
        }

        public static void exportManualEditReport_DoWork(object sender, DoWorkEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            try
            {
                String workspace = Globals.currentWorkspace;
                String fullPath = workspace + @"\Báo cáo sau chỉnh sửa";
                editedExcelFileName = fullPath + ".xlsx";
                Tuple<DataGridView, DataGridView> data = e.Argument as Tuple<DataGridView, DataGridView>;
                DataGridView dgvDetected = data.Item1;
                DataGridView dgvEdited = data.Item2;
                xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }
                xlApp.DisplayAlerts = false;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet = manualEditReportFormat(dgvEdited, xlWorkSheet);

                int rowStart = 5;
                for (int i = 0; i < dgvEdited.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvEdited.Columns.Count; j++)
                    {
                        if (dgvEdited.Rows[i].Cells[j].Value != null)
                        {
                            Color color = dgvEdited.Rows[i].Cells[j].Style.BackColor;
                            if (j == 0 || j == 1)
                            {
                                if (color == Color.Green)
                                {
                                    xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = "'" + dgvDetected.Rows[i].Cells[j].Value.ToString() + "/" + dgvEdited.Rows[i].Cells[j].Value.ToString();
                                }
                                else
                                {
                                    xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = "'" + dgvEdited.Rows[i].Cells[j].Value.ToString();
                                }
                            }
                            else
                            {
                                if (color == Color.Green)
                                {
                                    xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = dgvDetected.Rows[i].Cells[j].Value.ToString() + "/" + dgvEdited.Rows[i].Cells[j].Value.ToString();
                                }
                                else
                                {
                                    xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = dgvEdited.Rows[i].Cells[j].Value.ToString();
                                }
                                
                            }
                            
                            xlWorkSheet.Cells[i + 1 + rowStart, j + 1].Interior.Color = (color != Color.Red && color != Color.SkyBlue && color != Color.Green ? Color.White : color);
                        }
                    }
                }

                xlWorkBook.SaveAs(fullPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xlWorkBook.Close(0);
                xlApp.Quit();
                if (xlWorkSheet != null) Marshal.ReleaseComObject(xlWorkSheet);
                if (xlWorkBook != null) Marshal.ReleaseComObject(xlWorkBook);
                if (xlApp != null) Marshal.ReleaseComObject(xlApp);
            }
        }

        public static void exportManualEditReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            exportManualEditReportComplete = true;
            if (exportEditedReportComplete && exportFinalReportComplete && exportPreEditedReportComplete && exportManualEditReportComplete) 
            {
                exportFinishedHandler();
            }
        }

        public static void exportPreEditedReport_DoWork(object sender, DoWorkEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            try
            {
                String workspace = Globals.currentWorkspace;
                String fullPath = workspace + @"\Báo cáo trước chỉnh sửa";
                DataGridView dgv = e.Argument as DataGridView;
                xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }

                xlApp.DisplayAlerts = false;

                
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet = preEditedReportFormat(dgv, xlWorkSheet);

                int rowStart = 5;
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        if (dgv.Rows[i].Cells[j].Value != null)
                        {
                            if (j == 0 || j == 1)
                            {
                                xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = "'" + dgv.Rows[i].Cells[j].Value.ToString();
                            }
                            else
                            {
                                xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = dgv.Rows[i].Cells[j].Value.ToString();
                            }
                            Color color = dgv.Rows[i].Cells[j].Style.BackColor;
                            xlWorkSheet.Cells[i + 1 + rowStart, j + 1].Interior.Color = (color != Color.Red && color != Color.SkyBlue ? Color.White : color);
                        }
                    }
                }

                xlWorkBook.SaveAs(fullPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xlWorkBook.Close(0);
                xlApp.Quit();
                if (xlWorkSheet != null) Marshal.ReleaseComObject(xlWorkSheet);
                if (xlWorkBook != null) Marshal.ReleaseComObject(xlWorkBook);
                if (xlApp != null) Marshal.ReleaseComObject(xlApp);
            }            
        }

        public static void exportPreEditedReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            exportPreEditedReportComplete = true;
            if (exportEditedReportComplete && exportFinalReportComplete && exportPreEditedReportComplete && exportManualEditReportComplete)
            {
                exportFinishedHandler();
            }
        }

        public static void exportEditedReport_DoWork(object sender, DoWorkEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            try 
            {
                String workspace = Globals.currentWorkspace;
                String fullPath = workspace + @"\Biên bản chấm thi trắc nghiệm";
                DataGridView dgv = e.Argument as DataGridView;
                xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }

                xlApp.DisplayAlerts = false;

                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet = editedReportFormat(dgv, xlWorkSheet);

                int rowStart = 5;

                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        if (dgv.Rows[i].Cells[j].Value != null)
                        {
                            if (j == 0 || j == 1 || j == 4)
                            {
                                xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = "'" + dgv.Rows[i].Cells[j].Value.ToString();
                            }
                            else
                            {
                                xlWorkSheet.Cells[i + 1 + rowStart, j + 1] = dgv.Rows[i].Cells[j].Value.ToString();
                            }
                        }
                    }
                }

                //xlWorkSheet.Columns.AutoFit();


                xlWorkBook.SaveAs(fullPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xlWorkBook.Close(0);
                xlApp.Quit();
                if (xlWorkSheet != null) Marshal.ReleaseComObject(xlWorkSheet);
                if (xlWorkBook != null) Marshal.ReleaseComObject(xlWorkBook);
                if (xlApp != null) Marshal.ReleaseComObject(xlApp);
            }

        }

        public static void exportEditedReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            exportEditedReportComplete = true;
            if (exportEditedReportComplete && exportFinalReportComplete && exportPreEditedReportComplete && exportManualEditReportComplete)
            {
                exportFinishedHandler();
            }
        }

        public static void exportFinalReport_DoWork(object sender, DoWorkEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            try {

                String workspace = Globals.currentWorkspace;
                String fullPath = workspace + @"\Bảng điểm";
                DataGridView dgv = e.Argument as DataGridView;
                xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }

                xlApp.DisplayAlerts = false;
            
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                xlWorkSheet = finalReportFormat(dgv, xlWorkSheet);

                int rowStart = 7;

                xlWorkSheet.Cells[rowStart, 1] = "STT";
                xlWorkSheet.Cells[rowStart, 2] = "SBD";
                xlWorkSheet.Cells[rowStart, 3] = "Họ đệm";
                xlWorkSheet.Cells[rowStart, 4] = "Tên";
                xlWorkSheet.Cells[rowStart, 5] = "Ngày sinh";
                xlWorkSheet.Cells[rowStart, 6] = "Giới tính";
                xlWorkSheet.Cells[rowStart, 7] = "Điểm";
                xlWorkSheet.Cells[rowStart, 8] = "Ghi chú";

                for (int i = 1; i < dgv.Rows.Count; i++)
                {

                    if (dgv.Rows[i].Cells[0].Value != null) xlWorkSheet.Cells[rowStart + i, 1] = i.ToString();
                    if (dgv.Rows[i].Cells[0].Value != null) xlWorkSheet.Cells[rowStart + i, 2] = "'" + dgv.Rows[i].Cells[0].Value.ToString();
                    if (dgv.Rows[i].Cells[2].Value != null) xlWorkSheet.Cells[rowStart + i, 3] = dgv.Rows[i].Cells[2].Value.ToString();
                    if (dgv.Rows[i].Cells[3].Value != null) xlWorkSheet.Cells[rowStart + i, 4] = dgv.Rows[i].Cells[3].Value.ToString();
                    if (dgv.Rows[i].Cells[4].Value != null) xlWorkSheet.Cells[rowStart + i, 5] = "'" + dgv.Rows[i].Cells[4].Value.ToString();
                    if (dgv.Rows[i].Cells[5].Value != null) xlWorkSheet.Cells[rowStart + i, 6] = dgv.Rows[i].Cells[5].Value.ToString();
                    if (dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value != null)
                    {
                        if (!dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value.ToString().Equals(""))
                        {
                            xlWorkSheet.Cells[rowStart + i, 7] = dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value.ToString();
                        }
                        else
                        {
                            xlWorkSheet.Cells[rowStart + i, 7] = "0";
                        }
                    }
                    else
                    {
                        xlWorkSheet.Cells[rowStart + i, 7] = "0";
                    }
                    if (dgv.Rows[i].Cells[dgv.Columns.Count - 1].Value != null) xlWorkSheet.Cells[rowStart + i, 8] = dgv.Rows[i].Cells[dgv.Columns.Count - 1].Value.ToString();
                }


                xlWorkBook.SaveAs(fullPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xlWorkBook.Close(0);
                xlApp.Quit();
                if (xlWorkSheet != null) Marshal.ReleaseComObject(xlWorkSheet);
                if (xlWorkBook != null) Marshal.ReleaseComObject(xlWorkBook);
                if (xlApp != null) Marshal.ReleaseComObject(xlApp);
            }

        }

        public static void exportFinalReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            exportFinalReportComplete = true;
            if (exportEditedReportComplete && exportFinalReportComplete && exportPreEditedReportComplete && exportManualEditReportComplete)
            {
                exportFinishedHandler();
            }
        }

        private static void exportFinishedHandler()
        {
            
            Config cfg = new Config();
            String open = cfg.Get(Config.OPEN_FOLDER_AFTER_EXPORT, "No");
            if (open == "Yes") Process.Start(Globals.currentWorkspace);
            try
            {
                String hostName = cfg.Get(Config.HOST_NAME, "");
                if (hostName.Equals(""))
                {
                    hostName = "http://localhost:8080";
                    cfg.Set(Config.HOST_NAME, hostName);
                    cfg.Save();
                }

                string url = hostName + "/api/uploadManualEditLogFile";

                HttpResponseMessage message;
                var authContent = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                using (HttpClient client = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                using (HttpContent content = new ByteArrayContent(File.ReadAllBytes(editedExcelFileName)))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authContent);
                    formData.Add(content, "file", Path.GetFileName(editedExcelFileName));
                    formData.Add(new StringContent(Globals.examId.ToString()), "examId");
                    formData.Add(new StringContent(Globals.subjectId.ToString()), "subjectId");
                    var response = client.PostAsync(url, formData);
                    message = response.Result;
                }
                if (!message.IsSuccessStatusCode)
                {
                    MessageBox.Show("Sync data to server error!" + "\r\nCode: " + message.StatusCode);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Sync data to server error!" + "\r\nCode: " + ex.Message);
            }
            
        }

        private static Excel.Worksheet editedReportFormat(DataGridView dgv, Excel.Worksheet xlWorkSheet)
        {
            xlWorkSheet = commonFormat(xlWorkSheet);
            xlWorkSheet = formatEditedReport(dgv, xlWorkSheet);
            xlWorkSheet = insertHeader(dgv, xlWorkSheet, "BIÊN BẢN CHẤM THI TRẮC NGHIỆM", Globals.examName, Globals.subject, false);
            return xlWorkSheet;
        }

        private static Excel.Worksheet preEditedReportFormat(DataGridView dgv, Excel.Worksheet xlWorkSheet)
        {
            xlWorkSheet = commonFormat(xlWorkSheet);
            xlWorkSheet = formatPreEditedReport(dgv, xlWorkSheet);
            xlWorkSheet = insertHeader(dgv, xlWorkSheet, "BÁO CÁO TRƯỚC SỬA LỖI", Globals.examName, Globals.subject, false);
            return xlWorkSheet;
        }

        private static Excel.Worksheet manualEditReportFormat(DataGridView dgv, Excel.Worksheet xlWorkSheet)
        {
            xlWorkSheet = commonFormat(xlWorkSheet);
            xlWorkSheet = formatPreEditedReport(dgv, xlWorkSheet);
            xlWorkSheet = insertHeader(dgv, xlWorkSheet, "BÁO CÁO SAU SỬA LỖI", Globals.examName, Globals.subject, false);
            return xlWorkSheet;
        }

        private static Excel.Worksheet finalReportFormat(DataGridView dgv, Excel.Worksheet xlWorkSheet)
        {
            xlWorkSheet = commonFormat(xlWorkSheet);
            xlWorkSheet = formatFinalReport(dgv, xlWorkSheet);
            xlWorkSheet = insertHeader(dgv, xlWorkSheet, "", Globals.examName, Globals.subject, true);
            return xlWorkSheet;
        }

        private static Excel.Worksheet commonFormat(Excel.Worksheet xlWorkSheet)
        {
            

            xlWorkSheet.get_Range("A1", "C1").Merge();
            xlWorkSheet.get_Range("A2", "C2").Merge();

            xlWorkSheet.Rows.Font.Name = "Times New Roman";
            xlWorkSheet.Rows.Font.Bold = true;
            xlWorkSheet.Rows.Font.Size = 14;
            xlWorkSheet.Rows.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            xlWorkSheet.Rows.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            xlWorkSheet.Rows.RowHeight = 19;
            xlWorkSheet.Columns.ColumnWidth = 6;

            xlWorkSheet.PageSetup.Zoom = false;
            xlWorkSheet.PageSetup.FitToPagesWide = 1;
            xlWorkSheet.PageSetup.FitToPagesTall = 1;

            xlWorkSheet.Rows[1].RowHeight = 21;
            xlWorkSheet.Rows[2].RowHeight = 21;
            xlWorkSheet.Rows[3].RowHeight = 45;
            xlWorkSheet.Rows[4].RowHeight = 30;
            xlWorkSheet.Rows[6].RowHeight = 37.5;
            xlWorkSheet.Rows[6].WrapText = true;
            return xlWorkSheet;
        }

        private static Excel.Worksheet formatEditedReport(DataGridView dgv, Excel.Worksheet xlWorksheet)
        {
            
            xlWorksheet.Columns[1].ColumnWidth = 14;
            xlWorksheet.Columns[2].ColumnWidth = 8;
            xlWorksheet.Columns[3].ColumnWidth = 28.5;
            xlWorksheet.Columns[3].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Columns[4].ColumnWidth = 16;
            xlWorksheet.Columns[4].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Columns[5].ColumnWidth = 20;
            xlWorksheet.Columns[6].ColumnWidth = 9;
            xlWorksheet.Columns[6].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            for (int i = 1; i <= 5; i++) {
                xlWorksheet.Columns[dgv.Columns.Count - i].ColumnWidth = 11;
            }
            xlWorksheet.Columns[dgv.Columns.Count].ColumnWidth = 20;
            xlWorksheet.Columns[dgv.Columns.Count].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Rows[6].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            
            Excel.Range borderRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[6, 1], xlWorksheet.Cells[6 + dgv.Rows.Count - 1, dgv.Columns.Count]];
            borderRange.Borders.LineStyle = XlLineStyle.xlContinuous;
            return xlWorksheet;
        }

        private static Excel.Worksheet formatPreEditedReport(DataGridView dgv, Excel.Worksheet xlWorksheet)
        {
            xlWorksheet.Columns[1].ColumnWidth = 14;
            xlWorksheet.Columns[2].ColumnWidth = 8;

            xlWorksheet.get_Range("A1", "E1").Merge();
            xlWorksheet.get_Range("A2", "E2").Merge();

            Excel.Range borderRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[6, 1], xlWorksheet.Cells[6 + dgv.Rows.Count - 1, dgv.Columns.Count]];
            borderRange.Borders.LineStyle = XlLineStyle.xlContinuous;
            return xlWorksheet;
        }

        private static Excel.Worksheet formatFinalReport(DataGridView dgv, Excel.Worksheet xlWorksheet)
        {
            xlWorksheet.Columns[1].ColumnWidth = 6.86;
            xlWorksheet.Columns[2].ColumnWidth = 14;
            xlWorksheet.Columns[3].ColumnWidth = 28.5;
            xlWorksheet.Columns[3].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Columns[4].ColumnWidth = 16;
            xlWorksheet.Columns[4].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Columns[5].ColumnWidth = 20;
            xlWorksheet.Columns[6].ColumnWidth = 9;
            xlWorksheet.Columns[6].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            xlWorksheet.Columns[7].ColumnWidth = 11;
            xlWorksheet.Columns[8].ColumnWidth = 20;
            xlWorksheet.Columns[8].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            xlWorksheet.Rows[6].RowHeight = 19;
            xlWorksheet.Rows[7].RowHeight = 37.5;
            xlWorksheet.Rows[7].WrapText = true;

            xlWorksheet.get_Range("A1", "C1").Merge();
            xlWorksheet.get_Range("A2", "C2").Merge();
            xlWorksheet.get_Range("A4", "H4").Merge();
            xlWorksheet.get_Range("A5", "H5").Merge();

            xlWorksheet.Rows[7].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            Excel.Range borderRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[7, 1], xlWorksheet.Cells[6 + dgv.Rows.Count, 8]];
            borderRange.Borders.LineStyle = XlLineStyle.xlContinuous;
            return xlWorksheet;
        }

        private static Excel.Worksheet insertHeader(DataGridView dgv, Excel.Worksheet xlWorkSheet, String name, String examName, String subject, bool isFinal)
        {
            Config cfg = new Config();
            if (isFinal)
            {
                ((Excel.Range)xlWorkSheet.Range[xlWorkSheet.Cells[4, 1], xlWorkSheet.Cells[4, 8]]).Merge();
                ((Excel.Range)xlWorkSheet.Range[xlWorkSheet.Cells[5, 1], xlWorkSheet.Cells[5, 8]]).Merge();
                xlWorkSheet.Rows[1].Font.Size = 16;
                xlWorkSheet.Rows[2].Font.Size = 16;
                xlWorkSheet.Rows[4].Font.Size = 22;
                xlWorkSheet.Rows[5].Font.Size = 18;
                xlWorkSheet.Cells[1, 1] = cfg.Get(Config.SCHOOL, "TRƯỜNG THPT A").ToUpper();
                xlWorkSheet.Cells[2, 1] = cfg.Get(Config.ROOM, "PHÒNG XYZ").ToUpper();
                xlWorkSheet.Cells[4, 1] = "BẢNG ĐIỂM " + examName.ToUpper();
                xlWorkSheet.Cells[5, 1] = "Môn thi: " + subject;
                return xlWorkSheet;
            }
            else
            {
                ((Excel.Range)xlWorkSheet.Range[xlWorkSheet.Cells[3, 1], xlWorkSheet.Cells[3, dgv.Columns.Count]]).Merge();
                ((Excel.Range)xlWorkSheet.Range[xlWorkSheet.Cells[4, 1], xlWorkSheet.Cells[4, dgv.Columns.Count]]).Merge();
                xlWorkSheet.Rows[1].Font.Size = 16;
                xlWorkSheet.Rows[2].Font.Size = 16;
                xlWorkSheet.Rows[3].Font.Size = 22;
                xlWorkSheet.Rows[4].Font.Size = 18;
                xlWorkSheet.Cells[1, 1] = cfg.Get(Config.SCHOOL, "TRƯỜNG THPT A").ToUpper();
                xlWorkSheet.Cells[2, 1] = cfg.Get(Config.ROOM, "PHÒNG XYZ").ToUpper();
                xlWorkSheet.Cells[3, 1] = name;
                xlWorkSheet.Cells[4, 1] = examName + "                 Môn thi: " + subject;
                return xlWorkSheet;
            }
        }

        internal static void syncDataToServer(DataGridView dgv, List<Student> students)
        {
            NpgsqlConnection connection = null;
            String connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
            NpgsqlTransaction tran = connection.BeginTransaction();
            NpgsqlCommand cmd;

            int subjectId = Globals.subjectId;
            int examId = Globals.examId;
            string studentCode;
            string note;
            float point;

            var map = new Dictionary<String, int>();
            for (int i = 0; i < students.Count; i++) {
                map.Add(students[i].code, students[i].id);
            }
            try
            {
                for (int i = 1; i < dgv.Rows.Count; i++)
                {
                    note = "";
                    if (dgv.Rows[i].Cells[dgv.Columns.Count - 1].Value != null) {
                        note = dgv.Rows[i].Cells[dgv.Columns.Count - 1].Value.ToString();
                    }
                    studentCode = dgv.Rows[i].Cells[0].Value != null ? dgv.Rows[i].Cells[0].Value.ToString() : "";
                    if (dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value != null)
                    {
                        if (!dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value.ToString().Equals("")) point = float.Parse(dgv.Rows[i].Cells[dgv.Columns.Count - 2].Value.ToString());
                        else point = 0;
                    }
                    else
                    {
                        point = 0;
                    }

                    cmd = new NpgsqlCommand("INSERT INTO public.grade(student_id, subject_id, point, exam_id, status) values(@student_id, @subject_id, @point, @exam_id, @status)", connection);
                    cmd.Parameters.AddWithValue("exam_id", examId);
                    cmd.Parameters.AddWithValue("student_id", map[studentCode]);
                    cmd.Parameters.AddWithValue("subject_id", subjectId);
                    cmd.Parameters.AddWithValue("point", point);
                    
                    if (!note.Equals(""))
                    {
                        cmd.Parameters.AddWithValue("status", 2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("status", 1);
                    }
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
    }
}


