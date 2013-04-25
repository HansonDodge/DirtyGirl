using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Areas.Admin.Models;
using DirtyGirl.Web.Utils;
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
using DirtyGirl.Services;
using NPOI.SS.Util;

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

        public FileResult ExportPerformance(vmAdmin_PerformanceFilter filter)
        {
            vmAdmin_PerformanceFilter performanceFilter = SetPerformanceFilter(filter);

             var report = GetPerformanceReport(performanceFilter);

             return ExportPerformanceReport(report, performanceFilter, "Performance Report", "PerformanceExport.xls");
        }

        private FileResult ExportPerformanceReport(vmAdmin_PerformanceReport report, vmAdmin_PerformanceFilter filter, string title, string filename)
        {                       
            int rowNumber = 0;
            NPOI.SS.UserModel.IRow row;
           
            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = CreateSheet(workbook);

            var allStyles = ReportUtilities.CreateStyles(workbook);

            ReportUtilities.ExportReportHeader(title, sheet, allStyles, ref rowNumber);
            ExportReportSubheader(sheet, filter, allStyles, ref rowNumber);

            ExportReportValues(report, sheet, allStyles, ref rowNumber);
            ExportReportTshirts(report, sheet, allStyles, ref rowNumber);
            ExportReportFees(report, sheet, allStyles, ref rowNumber);
            ExportReportCharges(report, sheet,allStyles, ref rowNumber);

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                filename);     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        private void ExportReportSubheader(NPOI.SS.UserModel.ISheet sheet, vmAdmin_PerformanceFilter filter, StyleContainer allStyles, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);
            if (filter.EventId.HasValue && filter.EventId.Value > 0)
            {
                var thisEvent = _service.GetEventById(filter.EventId.Value);
                ReportUtilities.CreateCell(row, 0, string.Format("{0} - {1}", thisEvent.GeneralLocality, thisEvent.EventDates.Min().DateOfEvent.ToShortDateString()), allStyles.Header2Style);
                row = sheet.CreateRow(rowNumber++);
            }
            else
            {
                if (filter.startDate.HasValue)
                {
                    var value = string.Format("Events from {0} to {1}", filter.startDate.Value.ToShortDateString(), filter.endDate.Value.ToShortDateString());
                    ReportUtilities.CreateCell(row, 0, value, allStyles.Header2Style);
                    row = sheet.CreateRow(rowNumber++);
                }
            }                     
        }
       
        private static NPOI.SS.UserModel.ISheet CreateSheet(HSSFWorkbook workbook)
        {
            var sheet = workbook.CreateSheet();
            sheet.SetColumnWidth(0, 17 * 256);
            sheet.SetColumnWidth(1, 11 * 256);
            sheet.SetColumnWidth(2, 21 * 256);
            sheet.SetColumnWidth(3, 14 * 256);
            sheet.SetColumnWidth(4, 14 * 256);
            sheet.SetColumnWidth(5, 14 * 256);
            sheet.SetColumnWidth(6, 14 * 256);
            sheet.SetColumnWidth(7, 14 * 256);
            return sheet;
        }

        

        private static void ExportReportCharges(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {
            if (report.ChargeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, "Event Charges", allStyles.Header2Style);
                row = sheet.CreateRow(rowNumber++);

                ReportUtilities.CreateCell(row, 0, "Name", allStyles.TitleStyle);
                ReportUtilities.CreateCell(row, 1, "Cost Total", allStyles.TitleStyle);
                ReportUtilities.CreateCell(row, 2, "Discount Total", allStyles.TitleStyle);
                ReportUtilities.CreateCell(row, 3, "Local Tax Total", allStyles.TitleStyle);
                ReportUtilities.CreateCell(row, 4, "State Tax Total", allStyles.TitleStyle);
                ReportUtilities.CreateCell(row, 5, "Actual Total", allStyles.TitleStyle);

                foreach (var charge in report.ChargeReport.OrderBy(x => x.Name))
                {
                    row = sheet.CreateRow(rowNumber++);
                    ReportUtilities.CreateCell(row, 0, charge.Name, allStyles.LeftAligned);
                    ReportUtilities.CreateCell(row, 1, charge.CostTotal, allStyles.Currency);
                    ReportUtilities.CreateCell(row, 2, charge.DiscountTotal, allStyles.Currency);
                    ReportUtilities.CreateCell(row, 3, charge.LocalTaxTotal, allStyles.Currency);
                    ReportUtilities.CreateCell(row, 4, charge.StateTaxTotal, allStyles.Currency);
                    ReportUtilities.CreateCell(row, 5, charge.ActualTotal, allStyles.Currency);
                }

            }
        }

        private static void ExportReportFees(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {
            if (report.FeeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, "Event Fees", allStyles.Header2Style);
                foreach (var feeType in report.FeeReport.GroupBy(x => x.FeeType).Select(x => x.Key))
                {
                    row = sheet.CreateRow(rowNumber++);
                    ReportUtilities.CreateCell(row, 0, feeType.ToString(), allStyles.Header3Style);
                    row = sheet.CreateRow(rowNumber++);
                    ReportUtilities.CreateCell(row, 0, "Cost", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 1, "Use Count", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 2, "Cost Total", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 3, "Discount Total", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 4, "Local Tax Total", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 5, "State Tax Total", allStyles.TitleStyle);
                    ReportUtilities.CreateCell(row, 6, "Actual Total", allStyles.TitleStyle);

                    foreach (var fee in report.FeeReport.Where(x => x.FeeType == feeType).OrderBy(x => x.Cost))
                    {
                        row = sheet.CreateRow(rowNumber++);
                        ReportUtilities.CreateCell(row, 0, fee.Cost, allStyles.Currency);
                        ReportUtilities.CreateCell(row, 1, fee.UseCount, allStyles.RightAligned);
                        ReportUtilities.CreateCell(row, 2, fee.CostTotal, allStyles.Currency);
                        ReportUtilities.CreateCell(row, 3, fee.DiscountTotal, allStyles.Currency);
                        ReportUtilities.CreateCell(row, 4, fee.LocalTaxTotal, allStyles.Currency);
                        ReportUtilities.CreateCell(row, 5, fee.StateTaxTotal, allStyles.Currency);
                        ReportUtilities.CreateCell(row, 6, fee.ActualTotal, allStyles.Currency);
                    }
                    row = sheet.CreateRow(rowNumber++);
                }
            }
        }

        private static void ExportReportTshirts(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet,  StyleContainer allStyles, ref int rowNumber)
        {

            if (report.TShirtSizes != null && report.TShirtSizes.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                ReportUtilities.CreateCell(row, 0, "T-Shirts", allStyles.Header3Style);
                foreach (var size in report.TShirtSizes)
                {
                    row = sheet.CreateRow(rowNumber++);
                    ReportUtilities.CreateCell(row, 0, size.Keys.ElementAt(0).ToString(), allStyles.LeftAligned);
                    ReportUtilities.CreateCell(row, 1, size.Values.ElementAt(0), allStyles.RightAligned);
                }

                row = sheet.CreateRow(rowNumber++);

            }
        }

        private static void ExportReportValues(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet,  StyleContainer allStyles, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);

            ReportUtilities.CreateCell(row, 0, "Event Revenue", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 1, report.TotalRevenue, allStyles.Currency);
            ReportUtilities.CreateCell(row, 2, "Available Spots", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 3, report.TotalSpots, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 4, "Fee Total", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 5, report.FeeValue, allStyles.Currency);
            ReportUtilities.CreateCell(row, 6, "Charge Total", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 7, report.ChargeValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            ReportUtilities.CreateCell(row, 0, "Days count", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 1, report.DayCount, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 2, "Active Registrations", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 3, report.SpotsTaken, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 4, "Discount value", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 5, report.DiscountValue, allStyles.Currency);
            ReportUtilities.CreateCell(row, 6, "Local Tax", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 7, report.ChargeLocalTaxValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            ReportUtilities.CreateCell(row, 0, "Event count", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 1, report.EventCount, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 2, "Spots Available", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 3, report.SpotsLeft, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 4, "Local Tax", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 5, report.LocalTaxValue, allStyles.Currency);
            ReportUtilities.CreateCell(row, 6, "State Tax", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 7, report.ChargeStateTaxValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            ReportUtilities.CreateCell(row, 0, "Revenue Per Day", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 1, report.RevenuePerDay, allStyles.Currency);
            ReportUtilities.CreateCell(row, 2, "Registrations Per Day", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 3, report.RegPerDay, allStyles.RightAligned);
            ReportUtilities.CreateCell(row, 4, "State Tax", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 5, report.StateTaxValue, allStyles.Currency);
            ReportUtilities.CreateCell(row, 6, "Total Revenue", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 7, report.ChargeActualRevenue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            ReportUtilities.CreateCell(row, 0, "Revenue Per Event", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 1, report.RevenuePerEvent, allStyles.Currency);
            row.CreateCell(2);
            row.CreateCell(3);
            ReportUtilities.CreateCell(row, 4, "Total Revenue", allStyles.LeftAligned);
            ReportUtilities.CreateCell(row, 5, report.FeeActualRevenue, allStyles.Currency);
            row.CreateCell(6);
            row.CreateCell(7);

            row = sheet.CreateRow(rowNumber++);
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

                var sizeTotals = (_service as ReportingService).GetTShirtSizeTotalsByEventId((int)filter.EventId);

                var vm = new vmAdmin_EventPerformance
                {
                    Filter = eventFilter,
                    Report = GetPerformanceReport(performanceFilter)
                };

                if (sizeTotals != null)
                {
                    vm.Report.TShirtSizes = (List<Dictionary<String, int>>)sizeTotals;
                }
                else
                {
                    vm.Report.TShirtSizes = new List<Dictionary<String, int>>();
                }

                return View(vm);
            }

            return View(new vmAdmin_EventPerformance { Filter = eventFilter, Report = new vmAdmin_PerformanceReport() });
        }

        public FileResult ExportEventPerformance(vmAdmin_EventFilter filter)
        {            
            var performanceFilter = new vmAdmin_PerformanceFilter { EventId = filter.EventId };
            performanceFilter = SetPerformanceFilter(performanceFilter);

            var sizeTotals = (_service as ReportingService).GetTShirtSizeTotalsByEventId((int)filter.EventId);
            var report = GetPerformanceReport(performanceFilter) ;
            if (sizeTotals != null)           
                report.TShirtSizes = (List<Dictionary<String, int>>)sizeTotals;
            else            
                report.TShirtSizes = new List<Dictionary<String, int>>();                     
           
            return ExportPerformanceReport(report, performanceFilter, "Event Performance", "EventPerformance.xls");         
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
                         AgreeTrademark = x.AgreeTrademark,
                         City = x.Locality,
                         Email = x.Email,
                         EmergencyContact = x.EmergencyContact,
                         EmergencyPhone = x.EmergencyPhone,
                         FirstName = x.FirstName,
                         LastName = x.LastName,
                         MedicalInformation = x.MedicalInformation,
                         Phone = x.Phone,
                         BirthDate  = x.Birthday.HasValue ? x.Birthday.Value.ToShortDateString():"Unknown",
                         RegistrantId = x.RegistrationId,
                         RegistrationType = x.RegistrationType.ToString(),
                         RegistrationValue = _service.GetRegistrationPathValue(x.RegistrationId).ToString("c"),
                         SpecialNeeds = x.SpecialNeeds,
                         State = x.Region.Code,
                         TShirtSize = x.TShirtSize.HasValue ? x.TShirtSize.Value.ToString() : TShirtSize.Unknown.ToString(),
                         ThirdParty = (x.IsThirdPartyRegistration.HasValue && x.IsThirdPartyRegistration.Value) ? "Yes" : "No",
                         WaveDate = x.EventWave.EventDate.DateOfEvent,
                         WaveTime = x.EventWave.StartTime,
                         Zip = x.PostalCode,
                         DateAdded = x.DateAdded,
                         ConfirmationCode = x.ConfirmationCode ?? "Unknown",
                         PacketDeliveryOption = (x.PacketDeliveryOption.HasValue ? x.PacketDeliveryOption.Value.ToString() : RegistrationMaterialsDeliveryOption.OnSitePickup.ToString())
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
            int col = 0;
            headerRow.CreateCell(col++).SetCellValue("Last Name");
            headerRow.CreateCell(col++).SetCellValue("First Name");
            headerRow.CreateCell(col++).SetCellValue("Wave Date");
            headerRow.CreateCell(col++).SetCellValue("Wave Time");

            headerRow.CreateCell(col++).SetCellValue("Registration Type");
            headerRow.CreateCell(col++).SetCellValue("Registration Value");
            headerRow.CreateCell(col++).SetCellValue("Address1");
            headerRow.CreateCell(col++).SetCellValue("Address2");

            headerRow.CreateCell(col++).SetCellValue("City");
            headerRow.CreateCell(col++).SetCellValue("State");
            headerRow.CreateCell(col++).SetCellValue("Zip");
            headerRow.CreateCell(col++).SetCellValue("Email");
            headerRow.CreateCell(col++).SetCellValue("Birthday");

            headerRow.CreateCell(col++).SetCellValue("Phone");
            headerRow.CreateCell(col++).SetCellValue("Emergency Contact");
            headerRow.CreateCell(col++).SetCellValue("Emergency Phone");
            headerRow.CreateCell(col++).SetCellValue("Medical Information");

            headerRow.CreateCell(col++).SetCellValue("Special Needs");
            headerRow.CreateCell(col++).SetCellValue("T-Shirt Size");
            headerRow.CreateCell(col++).SetCellValue("Packet Delivery Option");
            headerRow.CreateCell(col++).SetCellValue("Agreed to Legal Terms");
            headerRow.CreateCell(col++).SetCellValue("Agreed to Trademark");
            headerRow.CreateCell(col++).SetCellValue("Registration Date");
            headerRow.CreateCell(col++).SetCellValue("Confirmation Code");

            //(Optional) freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;

            //Populate the sheet with values from the grid data
            foreach (vmAdmin_RegistrantListItem reg in registrants)
            {
                //Create a new row
                var row = sheet.CreateRow(rowNumber++);
                col = 0;
                //Set values for the cells
                row.CreateCell(col++).SetCellValue(reg.LastName);
                row.CreateCell(col++).SetCellValue(reg.FirstName);
                row.CreateCell(col++).SetCellValue(reg.WaveDate.ToString("MM/dd/yyyy"));
                row.CreateCell(col++).SetCellValue(reg.WaveTime.ToString("hh:mm tt"));

                row.CreateCell(col++).SetCellValue(reg.RegistrationType.ToString());
                row.CreateCell(col++).SetCellValue(reg.RegistrationValue);
                row.CreateCell(col++).SetCellValue(reg.Address1);
                row.CreateCell(col++).SetCellValue(reg.Address2);

                row.CreateCell(col++).SetCellValue(reg.City);
                row.CreateCell(col++).SetCellValue(reg.State);
                row.CreateCell(col++).SetCellValue(reg.Zip);
                row.CreateCell(col++).SetCellValue(reg.Email);
                row.CreateCell(col++).SetCellValue(reg.BirthDate);

                row.CreateCell(col++).SetCellValue(reg.Phone);
                row.CreateCell(col++).SetCellValue(reg.EmergencyContact);
                row.CreateCell(col++).SetCellValue(reg.EmergencyPhone);
                row.CreateCell(col++).SetCellValue(reg.MedicalInformation);

                row.CreateCell(col++).SetCellValue(reg.SpecialNeeds);
                row.CreateCell(col++).SetCellValue(reg.TShirtSize);
                row.CreateCell(col++).SetCellValue(reg.PacketDeliveryOption);
                row.CreateCell(col++).SetCellValue(reg.AgreeToTerms);
                row.CreateCell(col++).SetCellValue(reg.AgreeTrademark);
                row.CreateCell(col++).SetCellValue(reg.DateAdded.ToShortDateString());
                row.CreateCell(col++).SetCellValue(reg.ConfirmationCode ?? "Unknown");
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
            DateTime sDate = filter.startDate.Value;

            if (filter.fullEvent && filter.EventId.HasValue)            // if getting a single event, set the start back to the oldest registration
            {
                var regs = _service.GetRegistrationsByEventId(filter.EventId.Value);
                if (regs.Any())
                    sDate = regs.Min(x => x.DateAdded);
            }
            return new vmAdmin_PerformanceReport
                {
                    FeeReport = _service.GetFeeReport(filter.EventId, sDate, filter.endDate.Value),
                    ChargeReport = _service.GetEventChargeReport(filter.EventId, sDate, filter.endDate.Value),
                    DayCount = (filter.endDate.Value - sDate).Days + 1,
                    TotalSpots = _service.GetSpotsAvailable(filter.EventId, sDate, filter.endDate.Value),
                    SpotsTaken = _service.GetSpotsTaken(filter.EventId, sDate, filter.endDate.Value),
                    RedemptionRegCount = _service.GetRedemptionSpots(filter.EventId, sDate, filter.endDate.Value),
                    EventCount = _service.GetEventCount(filter.EventId, sDate, filter.endDate.Value)
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
                    endDate = filter.endDate,
                    fullEvent = false
                };

            if (filter.EventId.HasValue)
            {
                var evt = _service.GetEventById(filter.EventId.Value);

                if (!filter.startDate.HasValue)
                {
                    newFilter.startDate = evt.EventFees.Min(x => x.EffectiveDate).Date;
                    newFilter.fullEvent = true;
                }
                if (!filter.endDate.HasValue)
                    newFilter.endDate = DateTime.Now >= evt.EventDates.Max(x => x.DateOfEvent).Date ? evt.EventDates.Max(x => x.DateOfEvent) : DateTime.Now.Date;
            }
            else
            {
                if (!filter.startDate.HasValue)
                    newFilter.startDate = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)).Date;

                if (!filter.endDate.HasValue)
                    newFilter.endDate = DateTime.Now.Date;            
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
