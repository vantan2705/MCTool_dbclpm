using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTools
{
    public class Student
    {
        public int id { get; set; }
        public String code { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String dob { get; set; }
        public String gender { get; set; }


        public int examId { get; set; }

        public Student()
        {

        }

        public Student(int id, String code, String lastName, String firstName, String dob, String gender, int examId)
        {
            this.id = id;
            this.code = code;
            this.firstName = firstName;
            this.lastName = lastName;
            this.dob = dob;
            this.gender = gender;
            this.examId = examId;
        }
    }
}
