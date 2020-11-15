using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCTools
{
    public class Config
    {
        public const String CURRENT_WORKSPACE = "current_work_space";
        public const String ANSWER_SHEET_IMG_PATH = "as_img_path";
        public const String SCHOOL = "school";
        public const String ROOM = "room";
        public const String OPEN_FOLDER_AFTER_EXPORT = "open_after_export";
        public const String CURRENT_TEMPLATE_INDEX =  "current_template";
        public const String HOST_NAME = "host_name";

        private Dictionary<String, String> list;
        private String filename;

        public Config()
        {
            Reload();
        }

        public String Get(String field, String defValue)
        {
            return (Get(field) == null || Get(field) == "") ? (defValue) : (Get(field));
        }
        public String Get(String field)
        {
            return (list.ContainsKey(field)) ? (list[field]) : (null);
        }

        public void Set(String field, Object value)
        {
            if (!list.ContainsKey(field))
                list.Add(field, value.ToString());
            else
                list[field] = value.ToString();
        }

        public void Save()
        {
            Save(this.filename);
        }

        public void Save(String filename)
        {
            this.filename = filename;

            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);

            System.IO.StreamWriter file = new System.IO.StreamWriter(new FileStream(filename, FileMode.Open, FileAccess.ReadWrite),Encoding.UTF8);

            foreach (String prop in list.Keys.ToArray())
                if (!String.IsNullOrWhiteSpace(list[prop]))
                    file.WriteLine(prop + "=" + list[prop]);

            file.Close();
        }

        public void Reload()
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = documents + @"\MCTOOLS";
            this.filename = path + @"\app.properties";
            list = new Dictionary<String, String>();

            if (System.IO.File.Exists(filename)) {
                LoadFromFile(filename);
            } else {
                 try
                {
                    // If the directory doesn't exist, create it.
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch (IOException e)
                {
                    if (e.Source != null) MessageBox.Show("OOPS, We have a problem {0}", e.Source);
                }
                 System.IO.File.Create(path + @"\app.properties").Close();
            }
        }

        private void LoadFromFile(String file)
        {
            foreach (String line in System.IO.File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    String key = line.Substring(0, index).Trim();
                    String value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        //ignore dublicates
                        list.Add(key, value);
                    }
                    catch { }
                }
            }
        }


    }
}
