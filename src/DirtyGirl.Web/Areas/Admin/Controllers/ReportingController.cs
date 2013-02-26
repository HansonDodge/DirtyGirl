using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NPOI.HSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{    
    public class ReportingController : BaseController
    {

        #region private members

        private readonly IReportingService _service;        

        #endregion

        #region Constructor

        public ReportingController(IReportingService reportService)
        {
            this._service = reportService;  
        }

        #endregion

        #region Performance

        public ActionResult Performance(vmAdmin_PerformanceFilter filter)
        {  
            var performanceFilter = SetPerformanceFilter(filter);

            var vm = new vmAdmin_Performance
                {
                    Filter = performanceFilter,
                    Report = GetPerformanceReport(performanceFilter)
                };
         
            return View(vm);
        }        

        #endregion

        #region Event Performance

        public ActionResult EventPerformance(vmAdmin_EventFilter filter)
        {
            var eventFilter = SetEventFilter(filter);

            if (filter.EventId.HasValue)
            {
                var performanceFilter = new vmAdmin_PerformanceFilter { EventId = filter.EventId };
                performanceFilter = SetPerformanceFilter(performanceFilter);

                var vm = new vmAdmin_EventPerformance
                {
                    Filter = eventFilter,
                    Report = GetPerformanceReport(performanceFilter)
                };

                return View(vm);
            }

            return View(new vmAdmin_EventPerformance { Filter = eventFilter, Report = new vmAdmin_PerformanceReport() });
        }

        #endregion

        #region registrant

        public ActionResult Registrant(vmAdmin_EventFilter filter)
        {
            var vm = SetEventFilter(filter);
            return View(vm);
        }

        public ActionResult _RegistrantReport([DataSourceRequest] DataSourceRequest request, int? eventId)
        { 
            return Json(GetRegistrantReport(eventId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        private List<vmAdmin_RegistrantListItem> GetRegistrantReport(int? eventId)
        {
            List<vmAdmin_RegistrantListItem> rList = new List<vmAdmin_RegistrantListItem>();

            if (eventId.HasValue)
            {
                rList = _service.GetRegistrationsByEventId(eventId.Value).Select(x => new vmAdmin_RegistrantListItem
                     {
                         Address1 = x.Address1,
                         Address2 = x.Address2,
                         AgreeToTerms = x.AgreeToTerms,
                         City = x.Locality,
                         Email = x.Email,
                         EmergencyContact = x.EmergencyContact,
                         EmergencyPhone = x.EmergencyPhone,
                         FirstName = x.FirstName,
                         LastName = x.LastName,
                         MedicalInformation = x.MedicalInformation,
                         Phone = x.Phone,
                         RegistrantId = x.RegistrationId,
                         RegistrationType = x.RegistrationType.ToString(),
                         RegistrationValue = _service.GetRegistrationPathValue(x.RegistrationId).ToString("c"),
                         SpecialNeeds = x.SpecialNeeds,
                         State = x.Region.Code,
                         TShirtSize = x.TShirtSize.Value.ToString(),
                         ThirdParty = (x.IsThirdPartyRegistration.HasValue && x.IsThirdPartyRegistration.Value) ? "Yes" : "No",
                         WaveDate = x.EventWave.EventDate.DateOfEvent,
                         WaveTime = x.EventWave.StartTime,
                         Zip = x.PostalCode,
                         DateAdded = x.DateAdded
                     }).OrderBy(x => x.LastName).ToList();
            }

            return rList;
        }

        public FileResult ExportRegistrants(int? eventId)
        {
            //Get the data representing the current grid state - page, sort and filter
            IEnumerable registrants = GetRegistrantReport(eventId);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            /*(Optional) set the width of the columns
            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 50 * 256);
            sheet.SetColumnWidth(2, 50 * 256);
            sheet.SetColumnWidth(3, 50 * 256); */

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue("Last Name");
            headerRow.CreateCell(1).SetCellValue("First Name");
            headerRow.CreateCell(2).SetCellValue("Wave Date");
            headerRow.CreateCell(3).SetCellValue("Wave Time");

            headerRow.CreateCell(4).SetCellValue("Registration Type");
            headerRow.CreateCell(5).SetCellValue("Registration Value");
            headerRow.CreateCell(6).SetCellValue("Address1");
            headerRow.CreateCell(7).SetCellValue("Address2");

            headerRow.CreateCell(8).SetCellValue("City");
            headerRow.CreateCell(9).SetCellValue("State");
            headerRow.CreateCell(10).SetCellValue("Zip");
            headerRow.CreateCell(11).SetCellValue("Email");

            headerRow.CreateCell(12).SetCellValue("Phone");
            headerRow.CreateCell(13).SetCellValue("Emergency Contact");
            headerRow.CreateCell(14).SetCellValue("Emergency Phone");
            headerRow.CreateCell(15).SetCellValue("Medical Information");

            headerRow.CreateCell(16).SetCellValue("Special Needs");
            headerRow.CreateCell(17).SetCellValue("T-Shirt Size");
            headerRow.CreateCell(18).SetCellValue("Agreed to Legal Terms");
            headerRow.CreateCell(19).SetCellValue("Registration Date");

            //(Optional) freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;

            //Populate the sheet with values from the grid data
            foreach (vmAdmin_RegistrantListItem reg in registrants)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(0).SetCellValue(reg.LastName);
                row.CreateCell(1).SetCellValue(reg.FirstName);
                row.CreateCell(2).SetCellValue(reg.WaveDate.ToString("MM/dd/yyyy"));
                row.CreateCell(3).SetCellValue(reg.WaveTime.ToString("hh:mm tt"));

                row.CreateCell(4).SetCellValue(reg.RegistrationType.ToString());
                row.CreateCell(5).SetCellValue(reg.RegistrationValue);
                row.CreateCell(6).SetCellValue(reg.Address1);
                row.CreateCell(7).SetCellValue(reg.Address2);

                row.CreateCell(8).SetCellValue(reg.City);
                row.CreateCell(9).SetCellValue(reg.State);
                row.CreateCell(10).SetCellValue(reg.Zip);
                row.CreateCell(11).SetCellValue(reg.Email);

                row.CreateCell(12).SetCellValue(reg.Phone);
                row.CreateCell(13).SetCellValue(reg.EmergencyContact);
                row.CreateCell(14).SetCellValue(reg.EmergencyPhone);
                row.CreateCell(15).SetCellValue(reg.MedicalInformation);

                row.CreateCell(16).SetCellValue(reg.SpecialNeeds);
                row.CreateCell(17).SetCellValue(reg.TShirtSize);
                row.CreateCell(18).SetCellValue(reg.AgreeToTerms);
                row.CreateCell(19).SetCellValue(reg.DateAdded.ToShortDateString());
            }

            //Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "GridExcelExport.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user

        }

        #endregion

        #region Ajax

        public ActionResult _AjaxPerformanceReport(vmAdmin_PerformanceFilter filter)
        {
            var report = GetPerformanceReport(filter);

            return PartialView("partial/PerformanceReport", report);
        }

        private vmAdmin_PerformanceReport GetPerformanceReport(vmAdmin_PerformanceFilter filter)
        {            
            return new vmAdmin_PerformanceReport
                {
                    FeeReport = _service.GetFeeReport(filter.EventId, filter.startDate.Value, filter.endDate.Value),
                    ChargeReport = _service.GetEventChargeReport(filter.EventId, filter.startDate.Value, filter.endDate.Value),
                    DayCount = (filter.endDate.Value - filter.startDate.Value).Days + 1,
                    TotalSpots = _service.GetSpotsAvailable(filter.EventId, filter.startDate.Value, filter.endDate.Value),
                    SpotsTaken = _service.GetSpotsTaken(filter.EventId, filter.startDate.Value, filter.endDate.Value),
                    RedemptionRegCount = _service.GetRedemptionSpots(filter.EventId, filter.startDate.Value, filter.endDate.Value),
                    EventCount = _service.GetEventCount(filter.EventId, filter.startDate.Value, filter.endDate.Value)
                };
        }

        #endregion

        #region Filter

        private vmAdmin_PerformanceFilter SetPerformanceFilter(vmAdmin_PerformanceFilter filter)
        {
            var newFilter = new vmAdmin_PerformanceFilter
                {
                    EventList = GetEventList(), 
                    EventId = filter.EventId,
                    startDate = filter.startDate,
                    endDate = filter.endDate
                };

            if (filter.EventId.HasValue)
            {
                var evt = _service.GetEventById(filter.EventId.Value);

                if (!filter.startDate.HasValue)
                    newFilter.startDate = evt.EventFees.Min(x => x.EffectiveDate);

                if (!filter.endDate.HasValue)
                    newFilter.endDate = DateTime.Now >= evt.EventDates.Max(x => x.DateOfEvent) ? evt.EventDates.Max(x => x.DateOfEvent) : DateTime.Now;
            }
            else
            {
                if (!filter.startDate.HasValue)
                    newFilter.startDate = DateTime.Now.AddDays(-(DateTime.Now.Day - 1));

                if (!filter.endDate.HasValue)
                    newFilter.endDate = DateTime.Now;            
            }

            newFilter.endDate = newFilter.endDate.Value.AddDays(1).Date.AddSeconds(-1);

            return newFilter;    
        }

        private vmAdmin_EventFilter SetEventFilter(vmAdmin_EventFilter filter)
        {
            return new vmAdmin_EventFilter { EventId = filter.EventId, EventList = GetEventList() };
        }

        private List<SelectListItem> GetEventList()
        {
            var eList = _service.GetEventList()
                                .Select(x => new { x.EventId, Name = string.Format("{0} - {1}", x.GeneralLocality, x.EventDates.Min().DateOfEvent.ToShortDateString()) });

            return new SelectList(eList, "eventId", "name").ToList();
        }

        #endregion

    }
}
