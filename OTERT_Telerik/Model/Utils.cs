using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTERT.Model {

    [Serializable]
    public class wizardData {
        public int Step { get; set; }
        public int CustomerID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateCreated { get; set; }
        public string Code { get; set; }
        public DateTime DatePayed { get; set; }
        public bool locked { get; set; }
        public List<string> SelectedJobs { get; set; }
        public List<string> SelectedTasks { get; set; }
    }

    public class tasksTotalsPerJob {
        public int JobID { get; set; }
        public string JobName { get; set; }
        public int TasksCount { get; set; }
        public decimal TasksCost { get; set; }
    }

}