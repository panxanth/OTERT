using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace OTERT.WebServices {

    [System.Web.Script.Services.ScriptService]
    public class test : System.Web.Services.WebService {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getData() {
            HttpContext.Current.Response.Clear();
            List<Student> student = new List<Student>();
            Student stu = new Student();
            int code = 10000;
            for (long i = 0; i < 10; i++) {
                Student[] s = new Student[10];
                s[0] = new Student() { UniversityCode = code + 1, CourseFees = 2000.00, CGPA = 7.52, Duration = 90, Title = "Distributed Component Architecture" };
                s[1] = new Student() { UniversityCode = code + 2, CourseFees = 1000.00, CGPA = 9.55, Duration = 60, Title = "Data Structures" };
                s[2] = new Student() { UniversityCode = code + 3, CourseFees = 1750.00, CGPA = 9.03, Duration = 75, Title = "Neural Networks" };
                s[3] = new Student() { UniversityCode = code + 4, CourseFees = 2000.00, CGPA = 8.91, Duration = 90, Title = "Genetic Algorithms" };
                s[4] = new Student() { UniversityCode = code + 5, CourseFees = 1000.00, CGPA = 9.55, Duration = 30, Title = "Grid Computing" };
                s[5] = new Student() { UniversityCode = code + 6, CourseFees = 2500.00, CGPA = 9.87, Duration = 60, Title = "Cloud Computing" };
                s[6] = new Student() { UniversityCode = code + 7, CourseFees = 1500.00, CGPA = 9.75, Duration = 90, Title = "Enterprise Computing" };
                s[7] = new Student() { UniversityCode = code + 8, CourseFees = 1250.00, CGPA = 9.66, Duration = 45, Title = "Mobile Computing" };
                s[8] = new Student() { UniversityCode = code + 9, CourseFees = 1000.00, CGPA = 8.33, Duration = 60, Title = "WAP and XML" };
                s[9] = new Student() { UniversityCode = code + 10, CourseFees = 1500.00, CGPA = 8.66, Duration = 75, Title = "Design Patterns" };
                foreach (Student st in s) { student.Add(st); }
                code += 10;
            }
            HttpContext.Current.Response.ContentType = "application/json;charset=utf-8";
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            HttpContext.Current.Response.Write(String.Format("{{\"d\":{{\"results\":{0},\"__count\":{1}}}}}", serialize.Serialize(student), student.Count));
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }

    public class Student {
        #region Properties
        /// <summary>
        /// Gets or sets the student name.
        /// </summary>
        public long UniversityCode { get; set; }

        /// <summary>
        /// Gets or sets the course title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the duration of the course in days.
        /// </summary>               
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets course fees.
        /// </summary>       
        public double CourseFees { get; set; }

        /// <summary>
        /// Gets or sets CGPA.
        /// </summary>       
        public double CGPA { get; set; }

        #endregion
    }
}