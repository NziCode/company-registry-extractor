using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Functions
    {
        private PersianCalendar Pc = new PersianCalendar();
        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "هزار میلیارد" };

        public string GetTodayDate8NoSlash()
        {
            try
            {
                return Pc.GetYear(DateTime.Now).ToString("0000") + Pc.GetMonth(DateTime.Now).ToString("00") + Pc.GetDayOfMonth(DateTime.Now).ToString("00");
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string GetTodayDate8BySlash()
        {
            try
            {
                return Pc.GetYear(DateTime.Now).ToString("0000") + "/" + Pc.GetMonth(DateTime.Now).ToString("00") + "/" + Pc.GetDayOfMonth(DateTime.Now).ToString("00");
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string ConvertDate8BySlash(string date)
        {
            try
            {
                if (date.Length == 8)
                {
                    return date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                }
                else if (date.Length < 8)
                {
                    date = date.PadLeft(8, '0');
                    return date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                }
                else
                {
                    return date;
                }
            }
            catch (Exception e)
            {
                return date;
            }
        }

        public string GetTodayTime8ByPoint()
        {
            try
            {
                return Pc.GetHour(DateTime.Now).ToString("00") + ":" + Pc.GetMinute(DateTime.Now).ToString("00") + ":" + Pc.GetSecond(DateTime.Now).ToString("00");
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string ConvertTodayTime6ByPoint(string time)
        {
            try
            {
                if (time.Length == 6)
                {
                    return time.Substring(0, 2) + ":" + time.Substring(2, 2) + ":" + time.Substring(2, 2);
                }
                else if (time.Length < 6)
                {
                    time = time.PadLeft(6, '0');
                    return time.Substring(0, 2) + ":" + time.Substring(2, 2) + ":" + time.Substring(2, 2);
                }
                else
                {
                    return time;
                }
            }
            catch (Exception e)
            {
                return time;
            }
        }



        /// <summary>
        /// GenerateFileName is a method that generate a file name by pattern : BranchID + DateTime + FormName
        /// </summary>
        /// <returns>FileName</returns>
        public string GenerateFileName(string branchId, string formName)
        {
            return branchId + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + formName + ".pdf";
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetYear3Col is a method that return year for Col3 form
        /// </summary>
        /// <returns>Year</returns>
        public string GetYear3Col(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 8)
                    return " " + date.Substring(0, 4);
                else
                    return "";
            }
            else
                return "";
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetMonth3Col is a method that return month for Col3 form
        /// </summary>
        /// <returns>Month</returns>
        public string GetMonth3Col(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 8)
                    return " " + date.Substring(4, 2);
                else
                    return "";
            }
            else
                return "";
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetDay3Col is a method that return day for Col3 form
        /// </summary>
        /// <returns>Day</returns>
        public string GetDay3Col(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 8)
                    return " " + date.Substring(6, 2);
                else
                    return "";
            }
            else
                return "";
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetDate6 is a method that return date without '13' and '/'
        /// </summary>
        /// <returns>Date Example: 971011</returns>
        public string GetDate6(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 10)
                    return date.Substring(2, 8).Replace("/", "");
                else
                    return date;
            }
            else
                return date;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetDate8 is a method that return date without '/'
        /// </summary>
        /// <returns>Date Example: 13971011</returns>
        public string GetDate8(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 10)
                    return date.Replace("/", "");
                else
                    return date;
            }
            else
                return date;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetReverseSlashDate is a method that by slash and reversly for forms
        /// </summary>
        /// <returns>Date Example: 12/10/1397 or 12/10/97</returns>
        public string GetReverseSlashDate(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 8)
                    return date.Substring(6, 2) + "/" + date.Substring(4, 2) + "/" + date.Substring(0, 4);
                else if (date.Length > 0 && date.Length == 6)
                    return date.Substring(4, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(0, 2);
                return date;
            }
            else
                return date;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetReverseDate8 is a method that by slash and reversly without '13' for forms
        /// </summary>
        /// <returns>Date Example: 12/10/97</returns>
        public string GetReverseDate8(string date)
        {
            if (date != null)
            {
                if (date.Length > 0 && date.Length == 10)
                    return date.Substring(8, 2) + "/" + date.Substring(5, 2) + "/" + date.Substring(0, 4);
                else
                    return date;
            }
            else
                return date;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// GetSlashString is a method that by slash for forms
        /// </summary>
        /// <returns>Date Example: 1397/02/12</returns>
        public string GetSlashString(string date)
        {
            if (date.Length == 8)
                return date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
            else if (date.Length == 6)
                return "13" + date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4, 2);
            return "";
        }

        private static string GetNum3(int num3)
        {
            string s = "";
            int d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + dahyek[d12 - 10];
            }
            else
            {
                int d2 = d12 / 10;
                if (d2 != 0)
                    s = s + dahgan[d2] + " و ";
                int d1 = d12 % 10;
                if (d1 != 0)
                    s = s + yakan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            };
            return s;
        }











        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetDateBySlash_10(string date)
        {
            if (date != null)
            {
                if (date.Length == 8)
                {
                    return date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                }
                else if (date.Length == 6)
                {
                    return "13" + date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4, 2);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetSeriSerial(string seri, string serial)
        {
            if (seri != null && serial != null)
            {
                if (seri.Trim().Length > 0 && serial.Trim().Length > 0)
                {
                    return seri.Trim() + "-" + serial.Trim();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetFullName(string name, string family)
        {
            if (name != null && family != null)
            {
                if (name.Trim().Length > 0 && family.Trim().Length > 0)
                {
                    return name.Trim() + " " + family.Trim();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetNationalCode(string nationalCode, int length)
        {
            if (nationalCode != null)
            {
                if (nationalCode.Trim().Length > 0)
                {
                    return nationalCode.Trim().PadLeft(length, '0');
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetAccNumber(string accNumber, int length)
        {
            if (accNumber != null)
            {
                if (accNumber.Trim().Length > 0)
                {
                    return accNumber.Trim().PadLeft(length, '0');
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetNumberAmount(string amount)
        {
            if (amount != null)
            {
                if (amount.Trim().Length > 0)
                {
                    return Convert.ToDecimal(amount.Replace(",", "")).ToString("#,#");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetNumber2Alphabet(string amount)
        {
            if (amount != null)
            {
                if (amount.Trim().Length > 0)
                {
                    string stotal = "";
                    if (amount == "0")
                    {
                        return yakan[0];
                    }
                    else
                    {
                        amount = amount.Replace(",", "").PadLeft(((amount.Length - 1) / 3 + 1) * 3, '0');
                        int L = amount.Length / 3 - 1;
                        for (int i = 0; i <= L; i++)
                        {
                            int b = int.Parse(amount.Substring(i * 3, 3));
                            if (b != 0)
                                stotal = stotal + GetNum3(b) + " " + basex[L - i] + " و ";
                        }
                        stotal = stotal.Substring(0, stotal.Length - 3);
                    }
                    return stotal;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        // V3 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string RemovePlus(string text)
        {
            if (text != null)
            {
                if (text.Trim().Length > 0)
                {
                    return text.Replace('+', ' ');
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }


        public string GetNextMonth(string currentDate)
        {
            try
            {
                string newYear = "";
                string newMonth = "";
                int newMonthTemp = 0;
                int newYearTemp = 0;
                var oldDate = currentDate;
                var oldYear = Convert.ToInt32(oldDate.Split('/')[0]);
                var oldMonth = Convert.ToInt32(oldDate.Split('/')[1]);
                if (oldMonth == 12)
                {
                    newYearTemp = oldYear + 1;
                    newMonthTemp = 1;
                }
                else
                {
                    newYearTemp = oldYear;
                    newMonthTemp = oldMonth + 1;
                }
                newYear = newYearTemp.ToString();
                newMonth = newMonthTemp.ToString().PadLeft(2, '0');
                string res = newYear + "/" + newMonth;
                return res;
            }
            catch (Exception)
            {

                return currentDate;
            }
        }
    }
}
