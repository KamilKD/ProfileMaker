using ProfileMaker.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ProfileMaker.Utils
{
    /// <summary>
    /// Класс записи данных в файл dxf
    /// </summary>
    public static class DxfWriter
    {
        /// <summary>
        /// Запись данных профиля в файл
        /// </summary>
        /// <param name="filePath"> Путь к dxf дайлу </param>
        /// <param name="externalData"> Данные полуенные из еxcel </param>
        /// <param name="lineLength"> Длина участков разбиение, м </param>
        /// <param name="horizontalScale"> горизонтальный масштаб </param>
        /// <param name="vertcalScale"> Bepтикальный масштаб </param>
        /// <param name="showBiasToLength"> Флаг отрисовки строки уклон/Длина </param>
        /// <param name="showDepthFlag"> Флаг отрисовки строки глубины </рагam>
        /// <param name="showTubeMarkFlag"> флаг отрисовки строки Отметка трубы </param>
        /// <param name="showGroundMarkFlag"> флаг отрисовки строки Отметка земли</param>
        /// <param name="showDistanceFlag"> флаг отрисовки строки Расстояний </param>
        /// <param name="showPicketsFlag"> флаг отрисовки строки Пикетов </param>
        /// <param name="showLinePlanFlag"> флаг отрисовки строки План линии </param>
        /// <param name="showKmFlag"> флaг отрисовки строки илометры </param>
        /// <param name="enteredStartLength"> Вручнуо заданный стартовое растояние </param>
        /// <param name="radius"> Радиус трубы </param>
        public static async Task<bool> WriteProfile(
        string filePath,
        List<ExternalData> externalData,
        int lineLength,
        int horizontalScale,
        int vertcalScale,
        bool showBiasToLength,
        bool showDepthFlag,
        bool showTubeMarkFlag,
        bool showGroundMarkFlag,
        bool showDistanceFlag,
        bool showPicketsFlag,
        bool showLinePlanFlag,
        bool showKmFlag,
        int radius,
        double enteredStartLength)
        {
            var dataByLists = DivideBySections(externalData, lineLength, enteredStartLength);

            // Загрузка шаблонов
            string headerSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\HEADERS.txt";
            var headers = "";
            using (StreamReader reader = new StreamReader(headerSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                headers = await reader.ReadToEndAsync();
            }
            string classesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\CLASSES.txt";
            var classes = "";
            using (StreamReader reader = new StreamReader(classesSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                classes = await reader.ReadToEndAsync();
            }
            string tablesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\TABLES.txt";
            var tables = "";
            using (StreamReader reader = new StreamReader(tablesSamplePath))
            {
                tables = await reader.ReadToEndAsync();
            }
            string blocksSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\BLOCKS.txt";
            var blocks = "";
            using (StreamReader reader = new StreamReader(blocksSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                blocks = await reader.ReadToEndAsync();
            }
            string entitiesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\ENTITIES.txt";
            var entities = "";
            using (StreamReader reader = new StreamReader(entitiesSamplePath))
            {
                entities = await reader.ReadToEndAsync();
            }
            string objectsSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\OBJECTS.txt";
            var objects = "";
            using (StreamReader reader = new StreamReader(objectsSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                objects = await reader.ReadToEndAsync();
            }

            // Отрисовка графиков
            foreach (var page in dataByLists)
            {
                var planCompiler = new PlanCompiler(lineLength, horizontalScale, vertcalScale);
                var currEntities = entities;
                //currEntities += await planCompiler.CreateKren();
                currEntities += await planCompiler.FrameDraw();
                currEntities += await planCompiler.ScaleDraw(page);
                currEntities += await planCompiler.TableDraw(showBiasToLength,
                showDepthFlag,
                showTubeMarkFlag,
                showGroundMarkFlag,
                showDistanceFlag,
                showPicketsFlag,
                showLinePlanFlag,
                showKmFlag);
                if (showKmFlag)
                {
                    currEntities += await planCompiler.KmPointDraw(page, lineLength);
                }
                if (showPicketsFlag)
                {
                    currEntities += await planCompiler.PicketsDraw(page, lineLength);
                }
                currEntities += await planCompiler.PointDraw(page,
                lineLength,
                dataByLists.IndexOf(page),
                showBiasToLength,
                enteredStartLength);
                currEntities += await planCompiler.ProfileDraw(page,
                lineLength,
                dataByLists.IndexOf(page),
                enteredStartLength);
                currEntities += await planCompiler.NumbersDraw(page,
                lineLength,
                dataByLists.IndexOf(page),
                showBiasToLength,
                showDepthFlag,
                showTubeMarkFlag,
                showGroundMarkFlag,
                showDistanceFlag,
                radius,
                enteredStartLength);
                currEntities += "\n  0\nENDSEC";

                var fileIndex = dataByLists.IndexOf(page) + 1;
                var file = filePath.Insert(filePath.Length - 4, "_" + fileIndex.ToString());
                using (StreamWriter writer = new StreamWriter(file, true))
                {
                    await writer.WriteLineAsync(headers);
                    await writer.WriteLineAsync(classes);
                    await writer.WriteLineAsync(tables);
                    await writer.WriteLineAsync(blocks);
                    await writer.WriteLineAsync(currEntities);
                    await writer.WriteLineAsync(objects);
                    await writer.WriteLineAsync("  0\nEOF");
                }
            }
            return true;
        }

        /// <summary>
        /// Загрузка плана в файл
        /// </summary>
        /// <param name="filePath"> Пyrь к dxf дайлу </param>
        /// <param name="externalData"> Данные полуенные из еxсel </param>
        /// <param name="lineLength"> Длина участков разбиение, м </param>
        /// <param name="planeLength"> Длина учстка плана, м</рагаm>
        /// <param name="horizontalScale"> Гоpизонтальный масштaб </param>
        /// <param name="vertcalScale"> Вертикальный масшmтaб </param>
        /// <returns></returns>
        public static async Task<bool> WritePlane(
        string filePath,
        List<ExternalData> externalData,
        int lineLength,
        int planeLength,
        int horizontalScale,
        int verticalScale)
        {
            // Загрузка шаблонов
            string headerSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\HEADERS.txt";
            var headers = "";
            using (StreamReader reader = new StreamReader(headerSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                headers = await reader.ReadToEndAsync();
            }

            string classesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\CLASSES.txt";
            var classes = "";
            using (StreamReader reader = new StreamReader(classesSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                classes = await reader.ReadToEndAsync();
            }

            string tablesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\TABLES.txt";
            var tables = "";
            using (StreamReader reader = new StreamReader(tablesSamplePath))
            {
                tables = await reader.ReadToEndAsync();
            }

            string blocksSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\BLOCKS.txt";
            var blocks = "";
            using (StreamReader reader = new StreamReader(blocksSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                blocks = await reader.ReadToEndAsync();
            }

            string entitiesSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\ENTITIES.txt";
            var entities = "";
            using (StreamReader reader = new StreamReader(entitiesSamplePath))
            {
                entities = await reader.ReadToEndAsync();
            }

            string objectsSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\OBJECTS.txt";
            var objects = "";
            using (StreamReader reader = new StreamReader(objectsSamplePath, System.Text.Encoding.GetEncoding("windows-1251")))
            {
                objects = await reader.ReadToEndAsync();
            }

            var planeByLists = DivideBySections(externalData, planeLength);
            var a = 0;
            foreach (var page in planeByLists)
            {
                if (a < 3)
                {
                    var fileIndex = planeByLists.IndexOf(page);
                    var fileTrack = filePath.Insert(filePath.Length - 4, "_" + "Track" + fileIndex.ToString());
                    var trackPlanCompiler = new PlanCompiler(lineLength, horizontalScale, verticalScale);
                    var trackEntities = entities;
                    trackEntities += await trackPlanCompiler.TubeLineDraw(page, planeLength, planeByLists.IndexOf(page));
                    trackEntities += "\n  0\nENDSEC";
                    using (StreamWriter writer = new StreamWriter(fileTrack, true))
                    {
                        await writer.WriteLineAsync(headers);
                        await writer.WriteLineAsync(classes);
                        await writer.WriteLineAsync(tables);
                        await writer.WriteLineAsync(blocks);
                        await writer.WriteLineAsync(trackEntities);
                        await writer.WriteLineAsync(objects);
                        await writer.WriteLineAsync("  0\nEOF");
                    }
                    a++;
                }
            }
            return true;
        }

        /// <summary>
        /// Разделение входных данных по участкам
        /// </summary>
        /// <param name="externalData"> Данные полуенные из еxсel </param>
        /// <param name="lineLength"> Длина участков разбиения, м </param>
        /// <returns> Данные разделённые по участкам заданной длинны</returns>
        private static List<List<ExternalData>> DivideBySections(
        List<ExternalData> externalData,
        int lineLength,
        double enteredStartLength = 0.0)
        {
            var dataByLists = new List<List<ExternalData>>();
            dataByLists.Add(new List<ExternalData>());
            var i = 1;
            var lengthShift = (int)(enteredStartLength - enteredStartLength % Consts.MInKm);
            foreach (var data in externalData)
            {
                if (data.LengthBy2DLine - lengthShift < lineLength * i)
                {
                    dataByLists[i - 1].Add(data);
                }
                else
                {
                    // Добавляем фиктивную точку в конец участка
                    var fakePoint = ExternalData.CreateFakePoint(dataByLists[i - 1].Last(), data, lineLength * i + lengthShift);
                    dataByLists[i - 1].Add(fakePoint);
                    i++;
                    dataByLists.Add(new List<ExternalData>());
                    dataByLists[i - 1].Add(fakePoint);
                    dataByLists[i - 1].Add(data);
                }
            }
            return dataByLists;
        }
    }
}
