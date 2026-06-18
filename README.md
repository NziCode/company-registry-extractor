# Company Registry Extractor

A two-mode automation tool built with **Selenium WebDriver (Firefox)** and **OleDb** that collects company data from a paginated, JavaScript-rendered business registry website and exports it to Excel.

---

## Features

- **Mode 1 — ID Collector**: Navigates a paginated, React/Next.js-rendered company listing, opens each company profile in a new tab, extracts the national ID from the URL, and saves it incrementally to a `.txt` file.
- **Mode 2 — Data Extractor**: Reads national IDs from an Excel input file, visits each company's detail page, parses the HTML to extract structured fields, and writes results row-by-row into an Excel output file.
- **Resume support**: Both modes resume from where they left off if interrupted.
- **Deduplication**: Mode 2 reads already-processed IDs from the output file and skips them on re-run.
- **Retry logic**: Each button click retries up to 3 times on failure.
- **Randomized delays**: Waits a unique random interval (1–60s) between requests to avoid rate limiting.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET Framework) |
| Browser Automation | Selenium WebDriver 4 + GeckoDriver |
| Browser | Mozilla Firefox |
| Excel I/O | Microsoft ACE OleDb 12.0 |
| HTML Parsing | Regex + string splitting |

---

## Challenges Solved

- **JavaScript-rendered pagination**: The listing page uses a React/MUI component for pagination with no URL changes. Solved by targeting `aria-label` attributes on pagination buttons.
- **No anchor tags on cards**: Company cards render without `<a>` tags. Solved by clicking each card's profile button, capturing the new tab's URL, and closing it.
- **Per-row Excel persistence**: OleDb does not flush writes until the connection closes. Solved by opening and closing a new connection per row.
- **Session resume**: Progress is tracked via `last_page.txt` and the output Excel file itself, allowing safe interruption and restart.

---

## Project Structure

```
.
├── src/
│   └── Program.cs        # Main application
├── .gitignore
└── README.md
```

---

## Setup

### Prerequisites

- .NET Framework 4.x
- Mozilla Firefox
- [GeckoDriver](https://github.com/mozilla/geckodriver/releases) on PATH
- Microsoft Access Database Engine (ACE OleDb 12.0)
- NuGet packages: `Selenium.WebDriver`, `Selenium.Support`

### Input

Place `input.xlsx` next to the executable. The workbook must contain a sheet with a specific name in its name, with national IDs in column A.

### Output

Place an empty `output.xlsx` next to the executable with a sheet named **Result** and these headers in row 1:

```
NationalId | CompanyName | FoundDate | Status | CompanyType | RegisterNum | Capital | Address | Activity
```

---

## Usage

```
=== Company Registry Extractor ===
1. Collect national IDs from company list
2. Extract company data from input.xlsx into output.xlsx
Choose (1 or 2):
```

Run the executable and choose a mode. For mode 1, log in manually when the browser opens, then press Enter to start collection.

---

## Notes

- Data files (`input.xlsx`, `output.xlsx`, `national_ids.txt`) are excluded from version control via `.gitignore`.
- No credentials or sensitive data are stored in the codebase.