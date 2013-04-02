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
using DirtyGirl.Services;
using NPOI.SS.Util;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class StyleContainer
    {
        public NPOI.SS.UserModel.ICellStyle Currency { get; set; }
        public NPOI.SS.UserModel.ICellStyle RightAligned { get; set; }
        public NPOI.SS.UserModel.ICellStyle LeftAligned { get; set; }
        public NPOI.SS.UserModel.ICellStyle TitleStyle { get; set; }
        public NPOI.SS.UserModel.ICellStyle HeaderStyle { get; set; }
        public NPOI.SS.UserModel.ICellStyle Header2Style { get; set; }
        public NPOI.SS.UserModel.ICellStyle Header3Style { get; set; }
    }
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

            var allStyles = CreateStyles(workbook);

            ExportReportHeader(title, sheet, allStyles, ref rowNumber);
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
                CreateCell(row, 0, string.Format("{0} - {1}", thisEvent.GeneralLocality, thisEvent.EventDates.Min().DateOfEvent.ToShortDateString()), allStyles.Header2Style);
                row = sheet.CreateRow(rowNumber++);
            }
            else
            {
                if (filter.startDate.HasValue)
                {
                    var value = string.Format("Events from {0} to {1}", filter.startDate.Value.ToShortDateString(), filter.endDate.Value.ToShortDateString());
                    CreateCell(row, 0, value, allStyles.Header2Style);
                    row = sheet.CreateRow(rowNumber++);
                }
            }
                     

        }

        private static StyleContainer CreateStyles(HSSFWorkbook workbook)
        {

            var styles = new StyleContainer();

            var h1Font = workbook.CreateFont();
            h1Font.FontHeightInPoints = (short)24;
            h1Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var h2Font = workbook.CreateFont();
            h2Font.FontHeightInPoints = (short)16;
            h2Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var h3Font = workbook.CreateFont();
            h3Font.FontHeightInPoints = (short)12;
            h3Font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var titleFont = workbook.CreateFont();
            titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            styles.RightAligned = workbook.CreateCellStyle();
            styles.RightAligned.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;

            styles.Currency = workbook.CreateCellStyle();
            styles.Currency.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
            //styles.Currency.DataFormat = HSSFDataFormat.GetBuiltinFormat("$#,##0.00");
           styles.Currency.DataFormat = (short)7;

            styles.LeftAligned = workbook.CreateCellStyle();
            styles.LeftAligned.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;

            styles.TitleStyle = workbook.CreateCellStyle();
            styles.TitleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styles.TitleStyle.SetFont(titleFont);

            // Fonts are set into a style so create a new one to use.
            styles.HeaderStyle = workbook.CreateCellStyle();
            styles.HeaderStyle.SetFont(h1Font);
            styles.HeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;

            // Fonts are set into a style so create a new one to use.
            styles.Header2Style = workbook.CreateCellStyle();
            styles.Header2Style.SetFont(h2Font);

            // Fonts are set into a style so create a new one to use.
            styles.Header3Style = workbook.CreateCellStyle();
            styles.Header3Style.SetFont(h3Font);

            return styles;
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

        private static void ExportReportHeader(string title, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {     
            var row = sheet.CreateRow(rowNumber++);
            row.HeightInPoints = 27;
            var titleCell = row.CreateCell(0);
            titleCell.SetCellValue(title);
            titleCell.CellStyle = allStyles.HeaderStyle;

            titleCell.CellStyle.WrapText = true;

            NPOI.SS.Util.CellRangeAddress titleMerge = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 7);
            sheet.AddMergedRegion(titleMerge);

            row = sheet.CreateRow(rowNumber++);
        }

        private static void ExportReportCharges(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {
            if (report.ChargeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                CreateCell(row, 0, "Event Charges", allStyles.Header2Style);
                row = sheet.CreateRow(rowNumber++);

                CreateCell(row, 0, "Name", allStyles.TitleStyle);
                CreateCell(row, 1, "Cost Total", allStyles.TitleStyle);
                CreateCell(row, 2, "Discount Total", allStyles.TitleStyle);
                CreateCell(row, 3, "Local Tax Total", allStyles.TitleStyle);
                CreateCell(row, 4, "State Tax Total", allStyles.TitleStyle);
                CreateCell(row, 5, "Actual Total", allStyles.TitleStyle);

                foreach (var charge in report.ChargeReport.OrderBy(x => x.Name))
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, charge.Name, allStyles.LeftAligned);
                    CreateCell(row, 1, charge.CostTotal, allStyles.Currency);
                    CreateCell(row, 2, charge.DiscountTotal, allStyles.Currency);
                    CreateCell(row, 3, charge.LocalTaxTotal, allStyles.Currency);
                    CreateCell(row, 4, charge.StateTaxTotal, allStyles.Currency);
                    CreateCell(row, 5, charge.ActualTotal, allStyles.Currency);
                }

            }
        }

        private static void ExportReportFees(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {
            if (report.FeeReport.Count > 0)
            {
                var row = sheet.CreateRow(rowNumber++);
                CreateCell(row, 0, "Event Fees", allStyles.Header2Style);
                foreach (var feeType in report.FeeReport.GroupBy(x => x.FeeType).Select(x => x.Key))
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, feeType.ToString(), allStyles.Header3Style);
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, "Cost", allStyles.TitleStyle);
                    CreateCell(row, 1, "Use Count", allStyles.TitleStyle);
                    CreateCell(row, 2, "Cost Total", allStyles.TitleStyle);
                    CreateCell(row, 3, "Discount Total", allStyles.TitleStyle);
                    CreateCell(row, 4, "Local Tax Total", allStyles.TitleStyle);
                    CreateCell(row, 5, "State Tax Total", allStyles.TitleStyle);
                    CreateCell(row, 6, "Actual Total", allStyles.TitleStyle);

                    foreach (var fee in report.FeeReport.Where(x => x.FeeType == feeType).OrderBy(x => x.Cost))
                    {
                        row = sheet.CreateRow(rowNumber++);
                        CreateCell(row, 0, fee.Cost, allStyles.Currency);
                        CreateCell(row, 1, fee.UseCount, allStyles.RightAligned);
                        CreateCell(row, 2, fee.CostTotal, allStyles.Currency);
                        CreateCell(row, 3, fee.DiscountTotal, allStyles.Currency);
                        CreateCell(row, 4, fee.LocalTaxTotal, allStyles.Currency);
                        CreateCell(row, 5, fee.StateTaxTotal, allStyles.Currency);
                        CreateCell(row, 6, fee.ActualTotal, allStyles.Currency);
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
                CreateCell(row, 0, "T-Shirts", allStyles.Header3Style);
                foreach (var size in report.TShirtSizes)
                {
                    row = sheet.CreateRow(rowNumber++);
                    CreateCell(row, 0, size.Keys.ElementAt(0).ToString(), allStyles.LeftAligned);
                    CreateCell(row, 1, size.Values.ElementAt(0), allStyles.RightAligned);
                }

                row = sheet.CreateRow(rowNumber++);

            }
        }

        private static void ExportReportValues(vmAdmin_PerformanceReport report, NPOI.SS.UserModel.ISheet sheet,  StyleContainer allStyles, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);

            CreateCell(row, 0, "Event Revenue", allStyles.LeftAligned);
            CreateCell(row, 1, report.TotalRevenue, allStyles.Currency);
            CreateCell(row, 2, "Available Spots", allStyles.LeftAligned);
            CreateCell(row, 3, report.TotalSpots, allStyles.RightAligned);
            CreateCell(row, 4, "Fee Total", allStyles.LeftAligned);
            CreateCell(row, 5, report.FeeValue, allStyles.Currency);
            CreateCell(row, 6, "Charge Total", allStyles.LeftAligned);
            CreateCell(row, 7, report.ChargeValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Days count", allStyles.LeftAligned);
            CreateCell(row, 1, report.DayCount, allStyles.RightAligned);
            CreateCell(row, 2, "Active Registrations", allStyles.LeftAligned);
            CreateCell(row, 3, report.SpotsTaken, allStyles.RightAligned);
            CreateCell(row, 4, "Discount value", allStyles.LeftAligned);
            CreateCell(row, 5, report.DiscountValue, allStyles.Currency);
            CreateCell(row, 6, "Local Tax", allStyles.LeftAligned);
            CreateCell(row, 7, report.ChargeLocalTaxValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Event count", allStyles.LeftAligned);
            CreateCell(row, 1, report.EventCount, allStyles.RightAligned);
            CreateCell(row, 2, "Spots Available", allStyles.LeftAligned);
            CreateCell(row, 3, report.SpotsLeft, allStyles.RightAligned);
            CreateCell(row, 4, "Local Tax", allStyles.LeftAligned);
            CreateCell(row, 5, report.LocalTaxValue, allStyles.Currency);
            CreateCell(row, 6, "State Tax", allStyles.LeftAligned);
            CreateCell(row, 7, report.ChargeStateTaxValue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Revenue Per Day", allStyles.LeftAligned);
            CreateCell(row, 1, report.RevenuePerDay, allStyles.Currency);
            CreateCell(row, 2, "Registrations Per Day", allStyles.LeftAligned);
            CreateCell(row, 3, report.RegPerDay, allStyles.RightAligned);
            CreateCell(row, 4, "State Tax", allStyles.LeftAligned);
            CreateCell(row, 5, report.StateTaxValue, allStyles.Currency);
            CreateCell(row, 6, "Total Revenue", allStyles.LeftAligned);
            CreateCell(row, 7, report.ChargeActualRevenue, allStyles.Currency);

            row = sheet.CreateRow(rowNumber++);
            CreateCell(row, 0, "Revenue Per Event", allStyles.LeftAligned);
            CreateCell(row, 1, report.RevenuePerEvent, allStyles.Currency);
            row.CreateCell(2);
            row.CreateCell(3);
            CreateCell(row, 4, "Total Revenue", allStyles.LeftAligned);
            CreateCell(row, 5, report.FeeActualRevenue, allStyles.Currency);
            row.CreateCell(6);
            row.CreateCell(7);

            row = sheet.CreateRow(rowNumber++);
        }

        private static void CreateCell(NPOI.SS.UserModel.IRow row,int index,object value, NPOI.SS.UserModel.ICellStyle cellStyle )
        {
            var cell = row.CreateCell(index);
            if(value is decimal)
                cell.SetCellValue((double)((decimal)value));
            else
                if(value is int)
                    cell.SetCellValue((int)value);
                else
                    cell.SetCellValue((string)value);

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
                         AgreeTrademark = x.AgreeTrademark,
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
                         TShirtSize = x.TShirtSize.HasValue ? x.TShirtSize.Value.ToString() : TShirtSize.Unknown.ToString(),
                         ThirdParty = (x.IsThirdPartyRegistration.HasValue && x.IsThirdPartyRegistration.Value) ? "Yes" : "No",
                         WaveDate = x.EventWave.EventDate.DateOfEvent,
                         WaveTime = x.EventWave.StartTime,
                         Zip = x.PostalCode,
                         DateAdded = x.DateAdded,
                         PacketDeliveryOption = (x.PacketDeliveryOption.HasValue ? x.PacketDeliveryOption.Value.ToString() : RegistrationMaterialsDeliveryOption.OnSitePickup.ToString()),
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
            headerRow.CreateCell(18).SetCellValue("Packet Delivery Option");
            headerRow.CreateCell(19).SetCellValue("Agreed to Legal Terms");
            headerRow.CreateCell(20).SetCellValue("Agreed to Trademark");
            headerRow.CreateCell(21).SetCellValue("Registration Date");

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
                row.CreateCell(18).SetCellValue(reg.PacketDeliveryOption);
                row.CreateCell(19).SetCellValue(reg.AgreeToTerms);
                row.CreateCell(20).SetCellValue(reg.AgreeTrademark);
                row.CreateCell(21).SetCellValue(reg.DateAdded.ToShortDateString());
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
