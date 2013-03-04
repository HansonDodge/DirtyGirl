﻿using DirtyGirl.Models;
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
using DirtyGirl.Services;

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

            NPOI.SS.UserModel.ICellStyle rightAligned;
            NPOI.SS.UserModel.ICellStyle leftAligned;
            NPOI.SS.UserModel.ICellStyle titleStyle;
            NPOI.SS.UserModel.ICellStyle HeaderStyle;
            NPOI.SS.UserModel.ICellStyle Header2Style;
            NPOI.SS.UserModel.ICellStyle Header3Style;           

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();

            //Create new Excel sheet
            var sheet = CreateSheet(workbook);

            CreateStyles(workbook, out rightAligned, out leftAligned, out titleStyle, out HeaderStyle, out Header2Style, out Header3Style);                      
            ExportReportHeader(title, sheet, HeaderStyle, ref rowNumber);
            ExportReportSubheader(sheet, filter, Header3Style, ref rowNumber);

            ExportReportValues(report, sheet, rightAligned, leftAligned, ref rowNumber);
            ExportReportTshirts(report, sheet, rightAligned, leftAligned, Header3Style, ref rowNumber);
            ExportReportFees(report, sheet, rightAligned, titleStyle, Header2Style, Header3Style, ref rowNumber);
            ExportReportCharges(report, sheet, rightAligned, leftAligned, titleStyle, Header2Style, ref rowNumber);

            //Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                filename);     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        private void ExportReportSubheader(NPOI.SS.UserModel.ISheet sheet, vmAdmin_PerformanceFilter filter, NPOI.SS.UserModel.ICellStyle Header2Style, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);
            if (filter.EventId.HasValue && filter.EventId.Value > 0)
            {
                var thisEvent = _service.GetEventById(filter.EventId.Value);
                CreateCell(row, 0, string.Format("{0} - {1}", thisEvent.GeneralLocality, thisEvent.EventDates.Min().DateOfEvent.ToShortDateString()), Header2Style);
                row = sheet.CreateRow(rowNumber++);
            }
            else
            {
                if (filter.startDate.HasValue)
                {
                    var value = string.Format("Events from {0} to {1}", filter.startDate.Value.ToShortDateString(), filter.endDate.Value.ToShortDateString());
                    CreateCell(row, 0, value, Header2Style);
                    row = sheet.CreateRow(rowNumber++);
                }
            }
                     

        }
     
        private static void CreateStyles(HSSFWorkbook workbook, out NPOI.SS.UserModel.ICellStyle rightAligned, out NPOI.SS.UserModel.ICellStyle leftAligned, out NPOI.SS.UserModel.ICellStyle titleStyle, out NPOI.SS.UserModel.ICellStyle HeaderStyle, out NPOI.SS.UserModel.ICellStyle Header2Style, out NPOI.SS.UserModel.ICellStyle Header3Style)
        {
            var H1Font = workbook.CreateFont();
            H1Font.FontHeightInPoints = (short)24;
            H1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var H2Font = workbook.CreateFont();
            H2Font.FontHeightInPoints = (short)16;
            H2Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var H3Font = workbook.CreateFont();
            H3Font.FontHeightInPoints = (short)12;
            H3Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var TitleFont = workbook.CreateFont();
            TitleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            rightAligned = workbook.CreateCellStyle();
            rightAligned.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;

            leftAligned = workbook.CreateCellStyle();
            leftAligned.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;

            titleStyle = workbook.CreateCellStyle();
            titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            titleStyle.SetFont(TitleFont);

            // Fonts are set into a style so create a new one to use.
            HeaderStyle = workbook.CreateCellStyle();
            HeaderStyle.SetFont(H1Font);
            HeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

            // Fonts are set into a style so create a new one to use.
            Header2Style = workbook.CreateCellStyle();
            Header2Style.SetFont(H2Font);

            // Fonts are set into a style so create a new one to use.
            Header3Style = workbook.CreateCellStyle();
            Header3Style.SetFont(H3Font);
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

        private static void ExportReportHeader(string title, NPOI.SS.UserModel.ISheet sheet, NPOI.SS.UserModel.ICellStyle HeaderStyle, ref int rowNumber)
        {     
            var row = sheet.CreateRow(rowNumber++);
            row.HeightInPoints = 27;
            var titleCell = row.CreateCell(0);
            titleCell.SetCellValue(title);
            titleCell.CellStyle = HeaderStyle;

            titleCell.CellStyle.WrapText = true;

            NPOI.SS.Util.CellRangeAddress TitleMerge = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 7);
            sheet.AddMergedRegion(TitleMerge);

            row = sheet.CreateRow(rowNumber++);
        }

        private static void ExportReportCharges(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, NPOI.SS.UserModel.ICellStyle rightAligned, NPOI.SS.UserModel.ICellStyle leftAligned, NPOI.SS.UserModel.ICellStyle titleStyle, NPOI.SS.UserModel.ICellStyle Header2Style, ref int rowNumber)
        {
            if (report.ChargeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                CreateCell(row, 0, "Event Charges", Header2Style);
                row = sheet.CreateRow(rowNumber++);

                CreateCell(row, 0, "Name", titleStyle);
                CreateCell(row, 1, "Cost Total", titleStyle);
                CreateCell(row, 2, "Discount Total", titleStyle);
                CreateCell(row, 3, "Local Tax Total", titleStyle);
                CreateCell(row, 4, "State Tax Total", titleStyle);
                CreateCell(row, 5, "Actual Total", titleStyle);

                foreach (var charge in report.ChargeReport.OrderBy(x => x.Name))
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, charge.Name, leftAligned);
                    CreateCell(row, 1, charge.CostTotal.ToString("C"), rightAligned);
                    CreateCell(row, 2, charge.DiscountTotal.ToString("C"), rightAligned);
                    CreateCell(row, 3, charge.LocalTaxTotal.ToString("C"), rightAligned);
                    CreateCell(row, 4, charge.StateTaxTotal.ToString("C"), rightAligned);
                    CreateCell(row, 5, charge.ActualTotal.ToString("C"), rightAligned);
                }

            }
        }

        private static void ExportReportFees(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, NPOI.SS.UserModel.ICellStyle rightAligned, NPOI.SS.UserModel.ICellStyle titleStyle, NPOI.SS.UserModel.ICellStyle Header2Style, NPOI.SS.UserModel.ICellStyle Header3Style, ref int rowNumber)
        {
            if (report.FeeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                CreateCell(row, 0, "Event Fees", Header2Style);
                foreach (var feeType in report.FeeReport.GroupBy(x => x.FeeType).Select(x => x.Key))
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, feeType.ToString(), Header3Style);
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, "Cost", titleStyle);
                    CreateCell(row, 1, "Use Count", titleStyle);
                    CreateCell(row, 2, "Cost Total", titleStyle);
                    CreateCell(row, 3, "Discount Total", titleStyle);
                    CreateCell(row, 4, "Local Tax Total", titleStyle);
                    CreateCell(row, 5, "State Tax Total", titleStyle);
                    CreateCell(row, 6, "Actual Total", titleStyle);

                    foreach (var fee in report.FeeReport.Where(x => x.FeeType == feeType).OrderBy(x => x.Cost))
                    {
                        row = sheet.CreateRow(rowNumber++);
                        CreateCell(row, 0, fee.Cost.ToString("C"), rightAligned);
                        CreateCell(row, 1, fee.UseCount.ToString("C"), rightAligned);
                        CreateCell(row, 2, fee.CostTotal.ToString("C"), rightAligned);
                        CreateCell(row, 3, fee.DiscountTotal.ToString("C"), rightAligned);
                        CreateCell(row, 4, fee.LocalTaxTotal.ToString("C"), rightAligned);
                        CreateCell(row, 5, fee.StateTaxTotal.ToString("C"), rightAligned);
                        CreateCell(row, 6, fee.ActualTotal.ToString("C"), rightAligned);
                    }
                    row = sheet.CreateRow(rowNumber++);
                }
            }
        }

        private static void ExportReportTshirts(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, NPOI.SS.UserModel.ICellStyle rightAligned, NPOI.SS.UserModel.ICellStyle leftAligned, NPOI.SS.UserModel.ICellStyle Header3Style, ref int rowNumber)
        {

            if (report.TShirtSizes != null && report.TShirtSizes.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                CreateCell(row, 0, "T-Shirts", Header3Style);
                foreach (var size in report.TShirtSizes)
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, size.Keys.ElementAt(0).ToString(), leftAligned);
                    CreateCell(row, 1, size.Values.ElementAt(0).ToString(), rightAligned);
                }

                row = sheet.CreateRow(rowNumber++);

            }
        }

        private static void ExportReportValues(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, NPOI.SS.UserModel.ICellStyle rightAligned, NPOI.SS.UserModel.ICellStyle leftAligned, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);

            CreateCell(row, 0, "Event Revenue", leftAligned);
            CreateCell(row, 1, report.TotalRevenue.ToString("C"), rightAligned);
            CreateCell(row, 2, "Available Spots", leftAligned);
            CreateCell(row, 3, report.TotalSpots.ToString(), rightAligned);
            CreateCell(row, 4, "Fee Total", leftAligned);
            CreateCell(row, 5, report.FeeValue.ToString("C"), rightAligned);
            CreateCell(row, 6, "Charge Total", leftAligned);
            CreateCell(row, 7, report.ChargeValue.ToString("C"), rightAligned);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Days count", leftAligned);
            CreateCell(row, 1, report.DayCount.ToString(), rightAligned);
            CreateCell(row, 2, "Active Registrations", leftAligned);
            CreateCell(row, 3, report.SpotsTaken.ToString(), rightAligned);
            CreateCell(row, 4, "Discount value", leftAligned);
            CreateCell(row, 5, report.DiscountValue.ToString("C"), rightAligned);
            CreateCell(row, 6, "Local Tax", leftAligned);
            CreateCell(row, 7, report.ChargeLocalTaxValue.ToString("C"), rightAligned);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Event count", leftAligned);
            CreateCell(row, 1, report.EventCount.ToString(), rightAligned);
            CreateCell(row, 2, "Spots Available", leftAligned);
            CreateCell(row, 3, report.SpotsLeft.ToString(), rightAligned);
            CreateCell(row, 4, "Local Tax", leftAligned);
            CreateCell(row, 5, report.LocalTaxValue.ToString("C"), rightAligned);
            CreateCell(row, 6, "State Tax", leftAligned);
            CreateCell(row, 7, report.ChargeStateTaxValue.ToString("C"), rightAligned);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Revenue Per Day", leftAligned);
            CreateCell(row, 1, report.RevenuePerDay.ToString("C"), rightAligned);
            CreateCell(row, 2, "Registrations Per Day", leftAligned);
            CreateCell(row, 3, report.RegPerDay.ToString(), rightAligned);
            CreateCell(row, 4, "State Tax", leftAligned);
            CreateCell(row, 5, report.StateTaxValue.ToString("C"), rightAligned);
            CreateCell(row, 6, "Total Revenue", leftAligned);
            CreateCell(row, 7, report.ChargeActualRevenue.ToString("C"), rightAligned);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Revenue Per Event", leftAligned);
            CreateCell(row, 1, report.RevenuePerEvent.ToString("C"), rightAligned);
            row.CreateCell(2);
            row.CreateCell(3);
            CreateCell(row, 4, "Total Revenue", leftAligned);
            CreateCell(row, 5, report.RevenuePerEvent.ToString("C"), rightAligned);
            row.CreateCell(6);
            row.CreateCell(7);

            row = sheet.CreateRow(rowNumber++);
        }

        private static void CreateCell(NPOI.SS.UserModel.IRow row,int index,string value, NPOI.SS.UserModel.ICellStyle cellStyle )
        {
            var cell = row.CreateCell(index);
            cell.SetCellValue(value);
            cell.CellStyle = cellStyle;
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
