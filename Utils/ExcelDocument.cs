using Microsoft.Office.Interop.Excel;
using ProfileMaker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProfileMaker.Utils
{
    /// <summary>
    /// Клас для чтения схемы из еxcel
    /// </summary>
    public static class ExcelReader
    {
        /// <summary>
        /// Чтение даных из файла по указанному пути
        /// </summary>
        /// <param name="documentPath"> Путь к файлу </param>
        /// <param name="useRealCoordinates"> Флаг использования реальных координат точек </param>
        /// <param name="InterpolatedIntervаl"> Длинна участков интерполяшии </param>
        /// <param name="startLength"> заданная вручнуо дистаншия первой точки </param>
        /// <returns> Спиок записей из таблицы </returns>
        public static List<ExternalData> ReadExcelDocument(string documentPath, double startLength)
        {
            var excelApp = new Excel.Application();
            var workBook = excelApp.Workbooks.Open(documentPath);
            var workSheet = (Excel.Worksheet)excelApp.Sheets[1];
            var externalData = new List<ExternalData>();
            try
            {
                externalData = FillExternalData(workSheet, startLength);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                workBook.Saved = true;
                workBook.Close();
                excelApp.Quit();
            }
            return externalData;
        }

        /// <summary>
        /// Экспорт таблицы с интерполироваными данными
        /// </summary>
        /// <param name="documentPath"> Путь к дайлу </param>
        /// <param name="data"> Данные для записи </param>
        public static void WriteExcelDocument(string documentPath, List<ExternalData> data)
        {
            var excelApp = new Excel.Application();
            var workBook = excelApp.Workbooks.Add();
            var workSheet = (Excel.Worksheet)excelApp.Sheets[1];
            try
            {
                FillExcelSheet(workSheet, data);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                workBook.SaveAs(documentPath);
                workBook.Close();
                excelApp.Quit();
            }
        }

        /// <summary>
        /// Считывание данных из ехсеl в модель
        /// </summary>
        /// <param name="sheet"> Лист excel </param>
        /// <param name="startLength"> Вручную введенное стартовое растояние </param>
        /// <returns> Cписок записей </returns>
        private static List<ExternalData> FillExternalData(Excel.Worksheet sheet, double startLength)
        {
            var externalData = new List<ExternalData>();
            // Количество записей в файле
            var recordCount = GetRecordCount(sheet);
            var prevX = 0.0;
            var prevY = 0.0;
            var prevZ = 0.0;
            var prevLengthBy3DOnGround = 0.0;
            var prevLengthBy2DLine = 0.0;
            var prevTubeMark = 0.0;
            var prevLengthBy3DLine = 0.0;
            var distanceBy3DLine = 0.0;
            var lengthBy3DOnGround = startLength;
            var distanceBy2DLine = 0.0;
            var lengthBy2DLine = startLength;
            var tubeMark = 0.0;
            var distanceBy3DOnTube = 0.0;
            var lengthBy3DLine = startLength;
            var biasToLength = 0.0;
            var picketNameHead = (int)(lengthBy2DLine / 100);
            var picketNameTail = Math.Round(lengthBy2DLine % 100, 2);
            var lengthByFormat = $"ПК{picketNameHead}+{picketNameTail}";
            for (var i = 0; i < recordCount; i++)
            {
                var pointNumber = Convert.ToString((sheet.Cells[2 + i, 1] as Excel.Range).Value);
                var x = Convert.ToDouble((sheet.Cells[2 + i, 2] as Excel.Range).Value);
                var y = Convert.ToDouble((sheet.Cells[2 + i, 3] as Excel.Range).Value);
                var z = Convert.ToDouble((sheet.Cells[2 + i, 4] as Excel.Range).Value);
                var depth = Convert.ToDouble((sheet.Cells[2 + i, 5] as Excel.Range).Value);
                tubeMark = z - depth;
                // Примечание может быть пустым полем
                var note = (sheet.Cells[2 + i, 6] as Excel.Range).Value != null
                ? (string)(sheet.Cells[2 + i, 6] as Excel.Range).Value
                : "";
                var noteCorrect = "";
                switch (note)
                {
                    case "AD": noteCorrect = "АД"; break;
                    case "ZD": noteCorrect = "ЖД"; break;
                    case "VL": noteCorrect = "ВЛ"; break;
                    case "UZA": noteCorrect = "УЗА"; break;
                    case "Reka": noteCorrect = "Река"; break;
                    case "KIP": noteCorrect = "КИП"; break;
                    default: noteCorrect = note; break;
                }
                if (i != 0)
                {
                    distanceBy3DLine = Math.Sqrt(Math.Pow(x - prevX, 2) +
                    +Math.Pow(y - prevY, 2) + Math.Pow(z - prevZ, 2));

                    lengthBy3DOnGround = prevLengthBy3DOnGround + distanceBy3DLine;

                    distanceBy2DLine = Math.Sqrt(Math.Pow(x - prevX, 2) + Math.Pow(y - prevY, 2));

                    lengthBy2DLine = prevLengthBy2DLine + distanceBy2DLine;

                    distanceBy3DOnTube = Math.Sqrt(Math.Pow(x - prevX, 2) +
                    +Math.Pow(y - prevY, 2) + Math.Pow(tubeMark - prevTubeMark, 2));
                    lengthBy3DLine = prevLengthBy3DLine + distanceBy3DOnTube;
                    biasToLength = Math.Atan2(tubeMark - prevTubeMark, distanceBy3DOnTube);
                    picketNameHead = (int)(lengthBy2DLine / 100);
                    picketNameTail = Math.Round(lengthBy2DLine % 100, 2);
                    lengthByFormat = $"ПК{picketNameHead}+{picketNameTail}";
                }

                externalData.Add(new ExternalData(
                pointNumber,
                x,
                y,
                z,
                depth,
                noteCorrect,
                distanceBy3DLine,
                lengthBy3DOnGround,
                distanceBy2DLine,
                lengthBy2DLine,
                tubeMark,
                distanceBy3DOnTube,
                lengthBy3DLine,
                biasToLength,
                lengthByFormat));

                prevX = x;
                prevY = y;
                prevZ = z;
                prevLengthBy3DOnGround = lengthBy3DOnGround;
                prevLengthBy2DLine = lengthBy2DLine;
                prevTubeMark = tubeMark;
                prevLengthBy3DLine = lengthBy3DLine;
            }
            return externalData;
        }

        /// <summary>
        /// Получение количества записей в файле excel
        /// </summary>
        /// <param name="sheet"> Лист excel </param>
        /// <returns> Количество записей </returns>
        private static int GetRecordCount(Excel.Worksheet sheet)
        {
            var recordCount = 0;
            while ((sheet.Cells[2 + recordCount, 1] as Excel.Range).Value != null)
            {
                if ((sheet.Cells[2 + recordCount, 2] as Excel.Range).Value == null ||
                (sheet.Cells[2 + recordCount, 3] as Excel.Range).Value == null ||
                (sheet.Cells[2 + recordCount, 4] as Excel.Range).Value == null ||
                (sheet.Cells[2 + recordCount, 5] as Excel.Range).Value == null)
                {
                    throw new Exception($"Нарушена целостность данных в таблице. Проверьте данные в строке {recordCount + 1}");
                }
                recordCount++;
            }
            return recordCount;
        }


        /// <summary>
        /// Заполнение листа Excel
        /// </summary>
        /// <param name="sheet"> Рабочий лист Excel </param>>>
        /// <param name="data"> Список данных </param>
        private static void FillExcelSheet(Worksheet sheet, List<ExternalData> data)
        {
            sheet.Cells[1, 1] = "№ точки";
            sheet.Cells[1, 2] = "X";
            sheet.Cells[1, 3] = "Y";
            sheet.Cells[1, 4] = "Отметка земли";
            sheet.Cells[1, 5] = "Глубина трубы";
            sheet.Cells[1, 0] = "Примечание";
            sheet.Cells[1, 3] = "Расстояние между точками по Зд линии";
            sheet.Cells[1, 9] = "Общая длина по 3д линии земли";
            sheet.Cells[1, 10] = "Расст. между точками по 2д линии";
            sheet.Cells[1, 11] = "Общая длина по 2д линии";
            sheet.Cells[1, 12] = "Отметка трубы";
            sheet.Cells[1, 13] = "Расст, между точками по 3д линии трубы";
            sheet.Cells[1, 14] = "Общая длина по 3д линии трубы";
            sheet.Cells[1, 15] = "Уклон длина по трубе";
            sheet.Cells[1, 16] = "Длина по 2д в формате ПК0+00,00";

            var rowIndex = 2;
            foreach (var point in data)
            {
                sheet.Cells[rowIndex, 1] = point.PointNumber;
                sheet.Cells[rowIndex, 2] = point.X.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 3] = point.Y.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 4] = point.GroundMark.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 5] = point.TubeDepth.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 6] = point.Note;
                sheet.Cells[rowIndex, 8] = point.DistanceBy3DLine.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 9] = point.LengthBy3DOnGround.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 10] = point.DistanceBy2DLine.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 11] = point.LengthBy2DLine.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 12] = point.TubeMark.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 13] = point.DistanceBy3DOnTube.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 14] = point.LengthBy3DOnGround.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 15] = point.BiasToLength.ToString("G", CultureInfo.InvariantCulture);
                sheet.Cells[rowIndex, 16] = point.LengthByFormat;
                rowIndex++;
            }

            ((Excel.Range)sheet.Columns[1]).ColumnWidth = 10;
            ((Excel.Range)sheet.Columns[2]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[3]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[4]).ColumnWidth = 10;
            ((Excel.Range)sheet.Columns[5]).ColumnWidth = 10;
            ((Excel.Range)sheet.Columns[6]).ColumnWidth = 10;
            ((Excel.Range)sheet.Columns[8]).ColumnWidth = 10;
            ((Excel.Range)sheet.Columns[9]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[10]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[11]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[12]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[13]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[14]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[15]).ColumnWidth = 15;
            ((Excel.Range)sheet.Columns[16]).ColumnWidth = 15;
        }
    }
}
