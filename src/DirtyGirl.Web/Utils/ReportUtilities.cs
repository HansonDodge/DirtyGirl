using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;

namespace DirtyGirl.Web.Utils
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

    public class ReportUtilities
    {
        public static StyleContainer CreateStyles(HSSFWorkbook workbook)
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
        public static void CreateCell(NPOI.SS.UserModel.IRow row, int index, object value, NPOI.SS.UserModel.ICellStyle cellStyle)
        {
            var cell = row.CreateCell(index);
            if (value is decimal)
                cell.SetCellValue((double)((decimal)value));
            else
                if (value is int)
                    cell.SetCellValue((int)value);
                else
                    cell.SetCellValue((string)value);

            cell.CellStyle = cellStyle;
        }

        public static void ExportReportHeader(string title, NPOI.SS.UserModel.ISheet sheet, StyleContainer allStyles, ref int rowNumber)
        {
            var row = sheet.CreateRow(rowNumber++);
            row.HeightInPoints = 27;
            var titleCell = row.CreateCell(0);
            titleCell.SetCellValue(title);
            titleCell.CellStyle = allStyles.HeaderStyle;

            titleCell.CellStyle.WrapText = true;

            var titleMerge = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 7);
            sheet.AddMergedRegion(titleMerge);

            row = sheet.CreateRow(rowNumber++);
        }
    }
}