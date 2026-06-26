using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace CompanyRegistryExtractor
{
    class Program
    {
        static Random rnd = new Random();
        static int lastSleep = -1;
        static string baseUrl = "";
        static string searchUrl = "";

        static int GetUniqueSleep()
        {
            int sleep;
            do { sleep = rnd.Next(1, 61) * 1000; }
            while (sleep == lastSleep);
            lastSleep = sleep;
            return sleep;
        }

        static string GetValueByLabel(string html, string label)
        {
            try
            {
                string pattern = Regex.Escape(label) + @".*?(?:L-Bold__POZoQ|lrxlmG__text__L-Bold)[^>]*>([^<]*)<";
                var m = Regex.Match(html, pattern, RegexOptions.Singleline);
                if (m.Success)
                    return m.Groups[1].Value.Trim();
            }
            catch { }
            return "";
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (!File.Exists("config.txt"))
            {
                Console.WriteLine("config.txt not found.");
                Console.ReadLine();
                return;
            }
            var config = File.ReadAllLines("config.txt");
            baseUrl = config[0].Trim();
            searchUrl = config[1].Trim();

            Console.WriteLine("=== Company Registry Extractor ===");
            Console.WriteLine("1. Collect national IDs from company registry");
            Console.WriteLine("2. Extract company data from input.xlsx into output.xlsx");
            Console.Write("Choose (1 or 2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
                RunCollectIDs();
            else if (choice == "2")
                RunExtractData();
            else
                Console.WriteLine("Invalid choice.");

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        // ─────────────────────────────────────────
        // MODE 1: Collect National IDs
        // ─────────────────────────────────────────
        static void RunCollectIDs()
        {
            var allIds = new HashSet<string>();
            if (File.Exists("national_ids.txt"))
            {
                foreach (var line in File.ReadAllLines("national_ids.txt"))
                    if (!string.IsNullOrWhiteSpace(line)) allIds.Add(line.Trim());
            }
            Console.WriteLine("Loaded existing IDs: " + allIds.Count);

            int startPage = 1;
            if (File.Exists("last_page.txt"))
            {
                int.TryParse(File.ReadAllText("last_page.txt").Trim(), out startPage);
                if (startPage < 1) startPage = 1;
                Console.WriteLine("Resuming from page: " + startPage);
            }

            IWebDriver driver = new FirefoxDriver();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            try
            {
                driver.Navigate().GoToUrl(baseUrl + "/login");
                Thread.Sleep(3000);
                Console.WriteLine(">>> Please login in the browser, then press Enter...");
                Console.ReadLine();

                driver.Navigate().GoToUrl(searchUrl);
                Thread.Sleep(10000);

                string mainTab = driver.CurrentWindowHandle;

                if (startPage > 1)
                {
                    Console.WriteLine("Navigating to page " + startPage + "...");
                    for (int p = 1; p < startPage; p++)
                    {
                        var nb = driver.FindElements(By.XPath("//button[@aria-label='\u0631\u0641\u062a\u0646 \u0628\u0647 \u0635\u0641\u062d\u0647\u0654 " + (p + 1) + "']"));
                        if (nb.Count == 0) break;
                        js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", nb[0]);
                        nb[0].Click();
                        Thread.Sleep(3000);
                    }
                }

                int currentPage = startPage;

                while (true)
                {
                    Console.WriteLine("\n=== Page " + currentPage + " ===");
                    File.WriteAllText("last_page.txt", currentPage.ToString());

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    try
                    {
                        wait.Until(d => d.FindElements(By.XPath("//button[@aria-label='\u067e\u0631\u0648\u0641\u0627\u06cc\u0644 \u0634\u0631\u06a9\u062a']")).Count > 0);
                    }
                    catch
                    {
                        Console.WriteLine("Timeout on page " + currentPage + ", retrying...");
                        driver.Navigate().Refresh();
                        Thread.Sleep(8000);
                    }

                    var btns = driver.FindElements(By.XPath("//button[@aria-label='\u067e\u0631\u0648\u0641\u0627\u06cc\u0644 \u0634\u0631\u06a9\u062a']"));
                    int countBefore = allIds.Count;

                    for (int i = 0; i < btns.Count; i++)
                    {
                        int retries = 0;
                        while (retries < 3)
                        {
                            try
                            {
                                var currentBtns = driver.FindElements(By.XPath("//button[@aria-label='\u067e\u0631\u0648\u0641\u0627\u06cc\u0644 \u0634\u0631\u06a9\u062a']"));
                                if (i >= currentBtns.Count) break;

                                js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", currentBtns[i]);
                                Thread.Sleep(200);
                                currentBtns[i].Click();
                                Thread.Sleep(2500);

                                var tabs = new List<string>(driver.WindowHandles);
                                if (tabs.Count > 1)
                                {
                                    string newTab = tabs.Find(t => t != mainTab);
                                    driver.SwitchTo().Window(newTab);
                                    Thread.Sleep(1000);

                                    string url = driver.Url;
                                    var parts = url.Split('/');
                                    foreach (var part in parts)
                                    {
                                        if (part.Length >= 10 && part.All(char.IsDigit))
                                        {
                                            if (allIds.Add(part))
                                            {
                                                File.AppendAllText("national_ids.txt", part + "\r\n", Encoding.UTF8);
                                                Console.WriteLine("[" + allIds.Count + "] " + part);
                                            }
                                        }
                                    }

                                    driver.Close();
                                    driver.SwitchTo().Window(mainTab);
                                    Thread.Sleep(300);
                                    break;
                                }
                                else
                                {
                                    retries++;
                                    Thread.Sleep(2000);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error btn " + i + " (retry " + retries + "): " + ex.Message);
                                try { driver.SwitchTo().Window(mainTab); } catch { }
                                retries++;
                                Thread.Sleep(2000);
                            }
                        }
                    }

                    Console.WriteLine("Page done. Found: " + (allIds.Count - countBefore) + " | Total: " + allIds.Count);

                    string nextLabel = "\u0631\u0641\u062a\u0646 \u0628\u0647 \u0635\u0641\u062d\u0647\u0654 " + (currentPage + 1);
                    var nextBtns = driver.FindElements(By.XPath("//button[@aria-label='" + nextLabel + "']"));
                    if (nextBtns.Count == 0)
                    {
                        Console.WriteLine("No more pages!");
                        if (File.Exists("last_page.txt")) File.Delete("last_page.txt");
                        break;
                    }

                    js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", nextBtns[0]);
                    Thread.Sleep(500);
                    nextBtns[0].Click();
                    currentPage++;
                    Thread.Sleep(4000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.WriteLine("\n=== DONE === Total: " + allIds.Count);
            driver.Quit();
        }

        // ─────────────────────────────────────────
        // MODE 2: Extract Company Data into Excel
        // ─────────────────────────────────────────
        static void RunExtractData()
        {
            if (!File.Exists("input.xlsx"))
            {
                Console.WriteLine("input.xlsx not found.");
                return;
            }

            var ids = new List<string>();
            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=input.xlsx;Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            OleDbConnection oledbConn = new OleDbConnection(connstring);
            try
            {
                oledbConn.Open();
                DataTable sheet = oledbConn.GetSchema("Tables");
                foreach (DataRow r in sheet.Rows)
                {
                    string sheetName = r["TABLE_NAME"].ToString();
                    if (sheetName.Contains("\u0634\u0646\u0627\u0633\u0647"))
                    {
                        using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheetName + "]", oledbConn))
                        {
                            OleDbDataAdapter da = new OleDbDataAdapter();
                            da.SelectCommand = cmd;
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            foreach (DataRow row in ds.Tables[0].Rows)
                                if (!string.IsNullOrWhiteSpace(row[0].ToString()))
                                    ids.Add(row[0].ToString().Trim());
                        }
                    }
                }
            }
            finally { oledbConn.Close(); }

            Console.WriteLine("Loaded " + ids.Count + " national IDs from input.xlsx");

            var doneIds = new HashSet<string>();
            string connstring2 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=output.xlsx;Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            try
            {
                using (OleDbConnection conCheck = new OleDbConnection(connstring2))
                {
                    conCheck.Open();
                    using (OleDbCommand cmdCheck = new OleDbCommand("SELECT [NationalId] FROM [Result$]", conCheck))
                    {
                        OleDbDataReader reader = cmdCheck.ExecuteReader();
                        while (reader.Read())
                            if (!reader.IsDBNull(0))
                                doneIds.Add(reader[0].ToString().Trim());
                    }
                }
            }
            catch { }

            Console.WriteLine("Already processed: " + doneIds.Count + " | Remaining: " + (ids.Count - doneIds.Count));

            IWebDriver driver = new FirefoxDriver();

            try
            {
                driver.Navigate().GoToUrl(baseUrl + "/login");
                Thread.Sleep(3000);
                Console.WriteLine(">>> Please login in the browser, then press Enter...");
                Console.ReadLine();

                int success = 0, failed = 0, skipped = 0;

                foreach (string national in ids)
                {
                    if (doneIds.Contains(national))
                    {
                        skipped++;
                        Console.WriteLine("Skipping " + national + " [already done]");
                        continue;
                    }

                    try
                    {
                        Console.WriteLine("\nProcessing: " + national);
                        driver.Navigate().GoToUrl(baseUrl + "/company/" + national + "/direct");
                        Thread.Sleep(GetUniqueSleep());

                        string html = driver.PageSource;

                        var nameMatch = Regex.Match(html, @"(?:rasmText_text__H5__Acc1F|lrxlmG__text__H5)[^>]*>(.*?)</p>");
                        string companyName = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : "";

                        string dateRaw = GetValueByLabel(html, "\u062a\u0627\u0631\u06cc\u062e \u062a\u0627\u0633\u06cc\u0633");
                        string date = Regex.Replace(dateRaw, @"\s*\(.*?\)", "").Trim();

                        string status = GetValueByLabel(html, "\u0648\u0636\u0639\u06cc\u062a \u0634\u0631\u06a9\u062a");
                        string companyType = GetValueByLabel(html, "\u0646\u0648\u0639 \u0634\u0631\u06a9\u062a");
                        string registerNum = GetValueByLabel(html, "\u0634\u0645\u0627\u0631\u0647 \u062b\u0628\u062a");

                        string capitalRaw = GetValueByLabel(html, "\u0633\u0631\u0645\u0627\u06cc\u0647 \u062b\u0628\u062a\u06cc");
                        string capital = Regex.Replace(capitalRaw, @"[^\d]", "");
                        if (capital.Length > 15) capital = capital.Substring(0, 15);

                        var addrMatch = Regex.Match(html, @"aria-label=""کپی آدرس: ([^""]+)""");
                        string address = addrMatch.Success
                            ? Regex.Replace(addrMatch.Groups[1].Value, @"\s+\d{10}\s*$", "").Trim()
                            : "";

                        var actMatch = Regex.Match(html, @"دسته.های اصلی[^:]*:.*?normal[;""][^>]*>(.*?)</p>", RegexOptions.Singleline);
                        string activity = actMatch.Success ? actMatch.Groups[1].Value.Trim() : "";

                        if (string.IsNullOrWhiteSpace(companyName)) companyName = "-";
                        if (string.IsNullOrWhiteSpace(date)) date = "-";
                        if (string.IsNullOrWhiteSpace(status)) status = "-";
                        if (string.IsNullOrWhiteSpace(companyType)) companyType = "-";
                        if (string.IsNullOrWhiteSpace(registerNum)) registerNum = "-";
                        if (string.IsNullOrWhiteSpace(capital)) capital = "-";
                        if (string.IsNullOrWhiteSpace(address)) address = "-";
                        if (string.IsNullOrWhiteSpace(activity)) activity = "-";

                        Console.WriteLine("  Name    : " + companyName);
                        Console.WriteLine("  Date    : " + date);
                        Console.WriteLine("  Status  : " + status);
                        Console.WriteLine("  Type    : " + companyType);
                        Console.WriteLine("  RegNum  : " + registerNum);
                        Console.WriteLine("  Capital : " + capital);
                        Console.WriteLine("  Address : " + address);
                        Console.WriteLine("  Activity: " + activity);

                        using (OleDbConnection Con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=output.xlsx;Extended Properties=\"Excel 12.0;HDR=Yes;\""))
                        {
                            Con.Open();
                            OleDbCommand cmd2 = new OleDbCommand();
                            cmd2.Connection = Con;
                            cmd2.CommandText = "insert into [Result$] ([NationalId],[CompanyName],[FoundDate],[Status],[CompanyType],[RegisterNum],[Capital],[Address],[Activity]) values (?,?,?,?,?,?,?,?,?)";

                            cmd2.Parameters.AddWithValue("?", national);
                            cmd2.Parameters.AddWithValue("?", companyName);
                            cmd2.Parameters.AddWithValue("?", date);
                            cmd2.Parameters.AddWithValue("?", status);
                            cmd2.Parameters.AddWithValue("?", companyType);
                            cmd2.Parameters.AddWithValue("?", registerNum);
                            cmd2.Parameters.AddWithValue("?", capital);
                            cmd2.Parameters.AddWithValue("?", address);
                            cmd2.Parameters.AddWithValue("?", activity);

                            cmd2.ExecuteNonQuery();
                            Con.Close();
                        }

                        doneIds.Add(national);
                        success++;
                        Console.WriteLine("  --> OK [" + (success + skipped) + "/" + ids.Count + "]");
                    }
                    catch (Exception e)
                    {
                        failed++;
                        Console.WriteLine("  --> FAILED: " + e.Message);
                    }
                }

                Console.WriteLine("\n=== DONE === Success: " + success + " | Failed: " + failed + " | Skipped: " + skipped);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            driver.Quit();
        }
    }
}