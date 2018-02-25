using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;

    public class Utilities {

        public void MyDelay(double newseconds) {
            DateTime newDate = DateTime.Now.AddSeconds(newseconds);
            do { } while (DateTime.Now < newDate);
        }

}