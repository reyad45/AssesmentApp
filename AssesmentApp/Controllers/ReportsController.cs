using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using AssesmentApp.Models;


namespace AssesmentApp.Controllers
{
    public class ReportsController : Controller
    {

        AssesmentDBEntities db = new AssesmentDBEntities();

        //
        // GET: /Reports/
        public ActionResult Products()
        {

            //var adtrack_test = (from s in db.adtrack_test
            //                select new adtrack_test { Id = s.Id,
            //                             advertiserName = s.advertiserName,
            //                             BrandName = s.BrandName,
            //                             PlatformType = s.PlatformType,
            //                             DeviceModel = s.DeviceModel,
            //                             date = Convert.ToDateTime(s.TimeStamp).ToString("dd/MM/YYYY"),
            //                             time = Convert.ToDateTime(s.TimeStamp).ToString("hh:mm tt")                           
                            
            //                });

            var adtrack_test = db.adtrack_test.ToList(); 
            ////--DateTime.ParseExact(g.TimeStamp, "dd/MM/YYYY", null)
            var distinctPlatformType = adtrack_test
              .GroupBy(p => new { p.PlatformType })
              .Select(g => g.First())
              .ToList();
            IEnumerable<SelectListItem> items = distinctPlatformType.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.Id),
                Text = c.PlatformType

            });
            ViewBag.PlatformDDL = items;


            var distinctadvertiserName = adtrack_test
              .GroupBy(p => new { p.advertiserName })
              .Select(g => g.First())
              .ToList();
            IEnumerable<SelectListItem> Publisher = distinctadvertiserName.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.Id),
                Text = c.advertiserName

            });
            ViewBag.PublisherDDL = Publisher;

            var distinctDeviceModel = adtrack_test
             .GroupBy(p => new { p.DeviceModel })
             .Select(g => g.First())
             .ToList();
            IEnumerable<SelectListItem> BrandName = distinctDeviceModel.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.Id),
                Text = c.DeviceModel

            });
            ViewBag.BrandDDL = BrandName;

            return View(adtrack_test);
        }



        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("SrNo"),
                                                     new DataColumn("ImageUrl"),
                                                     new DataColumn("advertiserName"),
                                                     new DataColumn("BrandName"),
                                                     new DataColumn("PlatformType"),
                                                     new DataColumn("DeviceModel"),
                                                     new DataColumn("Date"),
                                                     new DataColumn("TimeStamp")});

            var datas = from adtrack_test in db.adtrack_test select adtrack_test;

            foreach (var track in datas)
            {
                dt.Rows.Add(track.Id, track.ImageUrl, track.advertiserName, track.BrandName,
                    track.PlatformType, track.DeviceModel, Convert.ToDateTime(track.TimeStamp).ToString("MM/dd/yyyy"), Convert.ToDateTime(track.TimeStamp).ToString("hh:mm tt"));
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }
        }


    }
}