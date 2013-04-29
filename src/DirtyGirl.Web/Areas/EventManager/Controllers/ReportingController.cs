using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Security;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.EventManager.Models;
using DirtyGirl.Web.Utils;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Controllers;
using System.IO;
using System.Web.Configuration;
using NPOI.HSSF.UserModel;

namespace DirtyGirl.Web.Areas.EventManager.Controllers
{
    public class ReportingController : BaseController
    {
       private readonly IReportingService _service;
           

        #region Constructor

        public ReportingController(IReportingService reportService)
        {
            _service = reportService;  
        }

        #endregion
        public ActionResult Summary(vmEventManager_EventFilter filter)
        {
            var performanceFilter = SetEventFilter(filter);

            var vm = new vmEventManager_Summary
            {
                Filter = performanceFilter,
                Report = GetSummaryReport(performanceFilter)
            };

            return View(vm);
        }
        public ActionResult SummaryExport(vmEventManager_EventFilter filter)
        {
            var performanceFilter = SetEventFilter(filter);

            var report = GetSummaryReport(performanceFilter);

            return ExportSummaryReport(report, performanceFilter);
        }

       
        private vmEventManager_SummaryReport GetSummaryReport(vmEventManager_EventFilter performanceFilter)
        {
            var rpt = new vmEventManager_SummaryReport();            
         
            if (performanceFilter.EventId.HasValue)
            {
                var evt = _service.GetEventById(performanceFilter.EventId.Value);
                rpt.EventName = evt.GeneralLocality;

                var regs = _service.GetRegistrationsByEventId(evt.EventId);
                var waves = _service.GetWavesByEventID(evt.EventId);

                int waveNumber = 1;
                foreach (var wave in waves)
                {
                    var waveData = new SummaryReportWaveData
                        {
                            EventWaveID = wave.EventWaveId,
                            WaveNumber = waveNumber++,
                            StartTime = wave.StartTime,
                            Active = wave.IsActive.ToString(),
                            NumParticipants = regs.Count(x => x.EventWaveId == wave.EventWaveId && x.RegistrationStatus == RegistrationStatus.Active)
                        };
                    rpt.WaveData.Add(waveData);
                }
            }

            return rpt;
        }
        public ActionResult Detailed(vmEventManager_EventFilter filter)
        {
            var performanceFilter = SetEventFilter(filter);

            var vm = new vmEventManager_Detailed
            {
                Filter = performanceFilter,
                Report = GetDetailedReport(performanceFilter)
            };
            return View(vm);
        }
        public ActionResult DetailedPage([DataSourceRequest] DataSourceRequest request, int eventId)
        {

            var performanceFilter = new vmEventManager_EventFilter();
            performanceFilter.EventId = eventId;

            var report = GetDetailedReport(performanceFilter);
            var result = report.WaveData.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailedExport(vmEventManager_EventFilter filter)
        {
            var performanceFilter = SetEventFilter(filter);

            var report = GetDetailedReport(performanceFilter);
            return ExportDetailedReport(report, performanceFilter);
        }
        private vmEventManager_DetailedReport GetDetailedReport(vmEventManager_EventFilter performanceFilter)
        {
            var rpt = new vmEventManager_DetailedReport();

            if (performanceFilter.EventId.HasValue)
            {
                var evt = _service.GetEventById(performanceFilter.EventId.Value);
                rpt.EventName = evt.GeneralLocality;

                var regs = _service.GetRegistrationsByEventId(evt.EventId);
                var waves = _service.GetWavesByEventID(evt.EventId);

                foreach (var wave in waves)
                {
                    int waveId = wave.EventWaveId;
                    foreach (var reg in regs.Where(x => x.EventWaveId == waveId && x.RegistrationStatus == RegistrationStatus.Active))
                    {
                        var waveData = new DetailedReportWaveData
                            {
                                StartTime = wave.StartTime,
                                AgreedLegal = reg.AgreeToTerms.ToString(),
                                AgreedTrademark = reg.AgreeTrademark.ToString(),
                                ConfirmationCode = reg.ConfirmationCode,
                                EmergencyContact = reg.EmergencyContact,
                                EmergencyPhone = reg.EmergencyPhone,
                                Firstname = reg.FirstName,
                                Lastname = reg.LastName,
                                MedicalInformation = reg.MedicalInformation,
                                SpecialNeeds = reg.SpecialNeeds,
                                ShirtSize = reg.TShirtSize.ToString()
                            };
                        rpt.WaveData.Add(waveData);    
                    }
                    
                }
            }

            return rpt;
        }

        private vmEventManager_EventFilter SetEventFilter(vmEventManager_EventFilter filter)
        {
            var newFilter = new vmEventManager_EventFilter
            {
                EventList = GetEventList(),
                EventId = filter.EventId,               
            };            

            return newFilter;    
        }
       
        private List<SelectListItem> GetEventList()
        {
            var eList = _service.GetEventList()
                                .Where(x => x.EventDates.Min().DateOfEvent >= DateTime.Now.Date && x.EventDates.Min().DateOfEvent < DateTime.Now.AddDays(101).Date && x.IsActive == true)
                                .OrderBy(x => x.EventDates.Min())
                                .Select(x => new { x.EventId, Name = string.Format("{0} - {1}", x.GeneralLocality, x.EventDates.Min().DateOfEvent.ToShortDateString()) });

            return new SelectList(eList, "eventId", "name").ToList();
        }
        private FileResult ExportSummaryReport(vmEventManager_SummaryReport report, vmEventManager_EventFilter filter)
        {
            int rowNumber = 0;
            NPOI.SS.UserModel.IRow row;

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet();
            sheet.SetColumnWidth(0, 11 * 256);
            sheet.SetColumnWidth(1, 44 * 256);
            sheet.SetColumnWidth(2, 33 * 256);
            sheet.SetColumnWidth(3, 11 * 256);
            sheet.SetColumnWidth(4, 11 * 256);

            var allStyles = ReportUtilities.CreateStyles(workbook);

            ReportUtilities.ExportReportHeader("Summary Report", sheet, allStyles, ref rowNumber);
            if (filter.EventId != null)
            {
                var thisEvent = _service.GetEventById(filter.EventId.Value);

                row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, string.Format("{0} - {1}", thisEvent.GeneralLocality, thisEvent.EventDates.Min().DateOfEvent.ToShortDateString()), allStyles.Header2Style);
            }
            sheet.CreateRow(rowNumber++);
            row = sheet.CreateRow(rowNumber++);
            ReportUtilities.CreateCell(row, 0, "Location", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, 1, "Wave Number", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, 2, "Start Time", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, 3, "Num Participants", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, 4, "Active", allStyles.TitleStyle);
            foreach (var wave in report.WaveData)
            {
                row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, report.EventName, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, 1, wave.WaveNumber, allStyles.RightAligned);
                ReportUtilities.CreateCell(row, 2, wave.StartTime.ToString("MM/dd/yyyy HH:mm"), allStyles.RightAligned);
                ReportUtilities.CreateCell(row, 3, wave.NumParticipants, allStyles.RightAligned);
                ReportUtilities.CreateCell(row, 4, wave.Active, allStyles.LeftAligned);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "SummaryExport.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user        
        }

        private FileResult ExportDetailedReport(vmEventManager_DetailedReport report, vmEventManager_EventFilter filter)
        {
            int rowNumber = 0;
            NPOI.SS.UserModel.IRow row;

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            int col = 0;
            var sheet = workbook.CreateSheet();
            sheet.SetColumnWidth(col++, 22 * 256);
            sheet.SetColumnWidth(col++, 22 * 256);
            sheet.SetColumnWidth(col++, 33 * 256);
            sheet.SetColumnWidth(col++, 33 * 256);
            sheet.SetColumnWidth(col++, 33 * 256);
            sheet.SetColumnWidth(col++, 22 * 256);
            sheet.SetColumnWidth(col++, 44 * 256);
            sheet.SetColumnWidth(col++, 44 * 256);
            sheet.SetColumnWidth(col++, 11 * 256);
            sheet.SetColumnWidth(col++, 15 * 256);
            sheet.SetColumnWidth(col++, 15 * 256);
            sheet.SetColumnWidth(col++, 18 * 256);

            var allStyles = ReportUtilities.CreateStyles(workbook);

            ReportUtilities.ExportReportHeader("Detailed Report", sheet, allStyles, ref rowNumber);
            if (filter.EventId != null)
            {
                var thisEvent = _service.GetEventById(filter.EventId.Value);

                row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, string.Format("{0} - {1}", thisEvent.GeneralLocality, thisEvent.EventDates.Min().DateOfEvent.ToShortDateString()), allStyles.Header2Style);
            }
            sheet.CreateRow(rowNumber++);
            row = sheet.CreateRow(rowNumber++);
            col = 0;
            ReportUtilities.CreateCell(row, col++, "Location", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Start Time", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "First Name", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Last Name", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Emergency Contact", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Emergency Phone", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Medical Information", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Special Needs", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Shirt Size", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Agreed Legal", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "Agreed Trademark", allStyles.TitleStyle);
            ReportUtilities.CreateCell(row, col++, "ConfirmationCode", allStyles.TitleStyle);

            foreach (var wave in report.WaveData)
            {
                col = 0;
                row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, col++, report.EventName, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.StartTime.ToString("MM/dd/yyyy HH:mm"), allStyles.RightAligned);
                ReportUtilities.CreateCell(row, col++, wave.Firstname, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.Lastname, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.EmergencyContact, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.EmergencyPhone, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.MedicalInformation, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.SpecialNeeds, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.ShirtSize, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.AgreedLegal, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.AgreedTrademark, allStyles.LeftAligned);
                ReportUtilities.CreateCell(row, col++, wave.ConfirmationCode, allStyles.LeftAligned);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "DetailedExport.xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user        
        }

    }
}
