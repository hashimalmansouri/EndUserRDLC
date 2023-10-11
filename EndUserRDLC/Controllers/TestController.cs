using EndUserRDLC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;

namespace EndUserRDLC.Controllers
{
    public class TestController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public TestController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        // get report to display it in iframe tag on view
        public IActionResult DisplayReport()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var bytes = GetReport("PDF");
            return Json(new { Success = true, Data = bytes });
        }
        // download rdlc file as PDF
        public IActionResult ExportPDF()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var bytes = GetReport("PDF");
            return File(bytes, "application/pdf", $"Test_{DateTime.Now.ToLongTimeString()}.pdf");
        }
        // prepare the report
        public byte[] GetReport(string renderType)
        {
            using var report = new LocalReport();
            var path = $"{_hostEnvironment.WebRootPath}\\reports\\rptTest.rdlc";
            report.ReportPath = path;
            report.DataSources.Add(new ReportDataSource("dsEmployee", GetEmployeeList()));
            var bytes = report.Render(renderType);
            return bytes;
        }
        // get list of employees
        private static List<Employee> GetEmployeeList()
        {
            return new List<Employee>
            {
                new Employee() { Id = 1, Name = "Ahmed", Job = "IT", Address = "Mukalla", Phone = "777111222" },
                new Employee() { Id = 2, Name = "Hashim", Job = "CS", Address = "Gharer", Phone = "700722733" },
                new Employee() { Id = 3, Name = "Ali", Job = "SE", Address = "Fuwwah", Phone = "733698741" },
                new Employee() { Id = 4, Name = "Saleh", Job = "IS", Address = "Al-Sheher", Phone = "788755766" },
            };
        }




        // to download rdlc report file
        public IActionResult DownloadRDLC()
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, "reports/rptTest.rdlc");
            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", "rptTest.rdlc");
        }
        // to upload rdlc report file at wwwroot/reports
        public IActionResult UploadRDLC(IFormFile myfile)
        {
            if (Path.GetExtension(myfile.FileName).ToLower() != ".rdlc")
            {
                return RedirectToAction(nameof(Index));
            }
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "reports");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var path = Path.Combine(uploadsFolder, myfile.FileName);
            if (System.IO.File.Exists(path))
            {
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(path);
                }
                catch (Exception)
                {

                }
            }
            myfile.CopyToAsync(new FileStream(path, FileMode.Create));
            return RedirectToAction(nameof(Index));
        }


    }
}
