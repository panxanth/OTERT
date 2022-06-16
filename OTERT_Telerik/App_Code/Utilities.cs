using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using Telerik.Windows.Documents.Flow.Model;
using System.Globalization;

public class Utilities {

    public void MyDelay(double newseconds) {
        DateTime newDate = DateTime.Now.AddSeconds(newseconds);
        do { } while (DateTime.Now < newDate);
    }

    private static string Ones(string Number) {
        int _Number = Convert.ToInt32(Number);
        string name = "";
        switch (_Number) {
            case 1:
                name = "Ενός";
                break;
            case 2:
                name = "Δύο";
                break;
            case 3:
                name = "Τριών";
                break;
            case 4:
                name = "Τεσσάρων";
                break;
            case 5:
                name = "Πέντε";
                break;
            case 6:
                name = "Έξι";
                break;
            case 7:
                name = "Επτά";
                break;
            case 8:
                name = "Οκτώ";
                break;
            case 9:
                name = "Εννέα";
                break;
        }
        return name;
    }

    private static string Tens(string Number) {
        int _Number = Convert.ToInt32(Number);
        string name = null;
        switch (_Number) {
            case 10:
                name = "Δέκα";
                break;
            case 11:
                name = "Έντεκα";
                break;
            case 12:
                name = "Δώδεκα";
                break;
            case 13:
                name = "Δεκατριών";
                break;
            case 14:
                name = "Δεκατεσσάρων";
                break;
            case 15:
                name = "Δεκαπέντε";
                break;
            case 16:
                name = "Δεκαέξι";
                break;
            case 17:
                name = "Δεκαεπτά";
                break;
            case 18:
                name = "Δεκαοκτώ";
                break;
            case 19:
                name = "Δεκαεννέα";
                break;
            case 20:
                name = "Είκοσι";
                break;
            case 30:
                name = "Τριάντα";
                break;
            case 40:
                name = "Σαράντα";
                break;
            case 50:
                name = "Πενήντα";
                break;
            case 60:
                name = "Εξήντα";
                break;
            case 70:
                name = "Εβδομήντα";
                break;
            case 80:
                name = "Ογδόντα";
                break;
            case 90:
                name = "Ενενήντα";
                break;
            default:
                if (_Number > 0) {
                    name = Tens(Number.Substring(0, 1) + "0") + " " + Ones(Number.Substring(1));
                }
                break;
        }
        return name;
    }

    private static string Hundreds(string Number) {
        int _Number = Convert.ToInt32(Number);
        string name = null;
        switch (_Number) {
            case 100:
                name = "Εκατόν";
                break;
            case 200:
                name = "Διακοσίων";
                break;
            case 300:
                name = "Τριακοσίων";
                break;
            case 400:
                name = "Τετρκοσίων";
                break;
            case 500:
                name = "Πεντακοσίων";
                break;
            case 600:
                name = "Εξακοσίων";
                break;
            case 700:
                name = "Επτακοσίων";
                break;
            case 800:
                name = "Οκτακοσίων";
                break;
            case 900:
                name = "Εννιακοσίων";
                break;
            default:
                if (_Number > 0) {
                    name = Hundreds(Number.Substring(0, 1) + "00") + " " + Tens(Number.Substring(1, 1) + "0") + " " + Ones(Number.Substring(2));
                }
                break;
        }
        return name;
    }

    private static string ConvertWholeNumber(string Number) {
        string word = "";
        try {
            bool beginsZero = false; //tests for 0XX    
            bool isDone = false; //test if already translated    
            double dblAmt = (Convert.ToDouble(Number, CultureInfo.InvariantCulture)); //if ((dblAmt > 0) && number.StartsWith("0"))    
            if (dblAmt > 0) { //test for zero or digit zero in a nuemric    
                beginsZero = Number.StartsWith("0");
                int numDigits = Number.Length;
                int pos = 0;//store digit grouping    
                string place = "";//digit grouping name:hundres,thousand,etc...    
                switch (numDigits) {
                    case 1: //ones' range    
                        word = Ones(Number);
                        isDone = true;
                        break;
                    case 2: //tens' range    
                        word = Tens(Number);
                        isDone = true;
                        break;
                    case 3: //hundreds' range    
                        if (Number == "100") { word = "Εκατό"; } else { word = Hundreds(Number); }
                        isDone = true;
                        break;
                    case 4: //thousands' range
                        if (Number.StartsWith("1")) {
                            if (Number.Substring(1, Number.Length - 1) == "100") {
                                word = "Χιλίων Εκατό";
                            } else {
                                word = "Χιλίων " + Hundreds(Number.Substring(1, Number.Length - 1));
                            }
                            isDone = true;
                        } else {
                            pos = (numDigits % 4) + 1;
                            place = " Χιλιάδων ";
                        }
                        break;
                    case 5:
                    case 6:
                        pos = (numDigits % 4) + 1;
                        place = " Χιλιάδων ";
                        break;
                    case 7: //millions' range    
                    case 8:
                    case 9:
                        pos = (numDigits % 7) + 1;
                        place = " Εκατομμυρίων ";
                        break;
                    case 10: //Billions's range    
                    case 11:
                    case 12:
                        pos = (numDigits % 10) + 1;
                        place = " Δισεκατομμυρίων ";
                        break;
                    //add extra case options for anything above Billion...    
                    default:
                        isDone = true;
                        break;
                }
                if (!isDone) { //if transalation is not done, continue...(Recursion comes in now!!)    
                    if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0") {
                        try {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                        }
                        catch { }
                    } else {
                        word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                    }
                    //check for trailing zeros    
                    //if (beginsZero) word = " and " + word.Trim();    
                }
                if (word.Trim().Equals(place.Trim())) word = ""; //ignore digit grouping names
            }
        }
        catch { }
        return word.Trim();
    }

    private static string ConvertToWords(string numb) {
        string val = "", wholeNo = numb, points = "", andStr = "", pointStr = "", endStr = "";
        try {
            numb = numb.Replace(",", ".");
            int decimalPlace = numb.IndexOf(".");
            if (decimalPlace > 0) {
                wholeNo = numb.Substring(0, decimalPlace);
                points = numb.Substring(decimalPlace + 1, 2);
                if (Convert.ToInt32(points) > 0) {
                    andStr = "Ευρώ και"; // just to separate whole numbers from points/cents    
                    endStr = "Λεπτών"; //Cents    
                    pointStr = ConvertWholeNumber(points);
                }
                else {
                    andStr = "Ευρώ";
                }
            } else {
                andStr = "Ευρώ";
            }
            val = string.Format("{0} {1} {2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
        }
        catch { }
        return val;
    }

    private static string ConvertDecimals(string number) {
        string cd = "", digit = "", engOne = "";
        for (int i = 0; i < number.Length; i++) {
            digit = number[i].ToString();
            if (digit.Equals("0")) {
                engOne = "Μηδέν";
            } else {
                engOne = Ones(digit);
            }
            cd += " " + engOne;
        }
        return cd;
    }

    public static string ConvertToText(string num2convert) {
        string isNegative = "";
        string string2return = "";
        try {
            num2convert = num2convert.Replace(",", ".");
            num2convert = Convert.ToDouble(num2convert, CultureInfo.InvariantCulture).ToString();
            if (num2convert.Contains("-")) {
                isNegative = "Minus ";
                num2convert = num2convert.Substring(1, num2convert.Length - 1);
            }
            if (num2convert == "0") {
                string2return = "Μηδέν";
            } else {
                string2return = isNegative + ConvertToWords(num2convert);
            }
        }
        catch (Exception) { }
        return string2return;
    }

}