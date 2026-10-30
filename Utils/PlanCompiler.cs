using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfileMaker.Models;

namespace ProfileMaker.Utils
{
    /// <summary>
    /// Класс для заполнения блока ENTITIES
    /// </summary>
    public class PlanCompiler
    {
        private int _hexIndex;
        // Начальная координата X
        private double _zeroX;
        // Крайняя справа координата X
        private double _extremeX;
        // Начальная координата Y
        private double _zeroY;
        // Начальная координата Y
        private double _zeroZ;
        private int _horizontalScale;
        private int _verticalScale;

        /// <param name="sectionLength"> Длина секций </param>
        /// <param name="horizontalScale"> Периодный масштаб </param>
        /// <param name="verticalScale"> Вертикальный масштаб </param>
        public PlanCompiler(int sectionLength, int horizontalScale, int verticalScale)
        {
            _hexIndex = 4500;
            _zeroX = 0.0;
            _extremeX = sectionLength * (1000.0 / horizontalScale);
            _zeroY = 0.0;
            _zeroZ = 0.0;

            _horizontalScale = horizontalScale;
            _verticalScale = verticalScale;
        }

        /// <summary>
        /// Отрисовка рамки
        /// </summary>
        /// <returns> Строка с данными для отрисовки рамки </returns>
        public async Task<string> FrameDraw()
        {
            var frame = "";

            // Общая рамка
            frame += await CreateLine(-24, -24, -13, 407, "Штамп");
            frame += await CreateLine(-24, _extremeX + 460, 407, 407, "Шамп");
            frame += await CreateLine(_extremeX + 460, _extremeX + 460, -13, 407, "Штамп");
            frame += await CreateLine(-24, _extremeX + 460, -13, -13, "ШTамп");

            // Рамка TOCT
            frame += await CreateLine(-4, -4, -5, 397, "Штамп");
            frame += await CreateLine(-4, _extremeX + 455, 397, 397, "Штамп");
            frame += await CreateLine(_extremeX + 455, _extremeX + 455, -5, 397, "Штамп");
            frame += await CreateLine(-4, _extremeX + 455, -5, -5, "Штамп");

            // Левая подрамка ГОСТ 
            frame += await CreateLine(-16, -16, -5, 80, "Штамп");
            frame += await CreateLine(-11, -11, -5, 80, "Штамп");
            frame += await CreateLine(-4, -16, -5, -5, "Штамп");
            frame += await CreateLine(-4, -16, 20, 20, "Штамп");
            frame += await CreateLine(-4, -16, 55, 55, "Штамп");
            frame += await CreateLine(-4, -16, 80, 80, "Штамп");
            frame += await CreateText(-12, -1, "Инв. Nподл.", "Штамп", 1.5, 90);
            frame += await CreateText(-12, 25, "Подпись и дата", "штамп", 1.5, 90);
            frame += await CreateText(-12, 60, "Взам.инв.N", "тамп", 1.5, 90);

            //Правая подрмка ГОСТ
            frame += await CreateLine(_extremeX + 270, _extremeX + 270, -5, 50, "Штамп");
            frame += await CreateLine(_extremeX + 280, _extremeX + 280, 25, 50, "Штамп");
            frame += await CreateLine(_extremeX + 290, _extremeX + 290, -5, 50, "Штамп");
            frame += await CreateLine(_extremeX + 300, _extremeX + 300, 25, 50, "Штамп");
            frame += await CreateLine(_extremeX + 310, _extremeX + 310, -5, 50, "Штамп");
            frame += await CreateLine(_extremeX + 325, _extremeX + 325, -5, 50, "Штамп");
            frame += await CreateLine(_extremeX + 335, _extremeX + 335, -5, 50, "Штамп");
            frame += await CreateLine(_extremeX + 405, _extremeX + 405, -5, 25, "Штамп");
            frame += await CreateLine(_extremeX + 420, _extremeX + 420, 10, 25, "Штамп");
            frame += await CreateLine(_extremeX + 435, _extremeX + 435, 10, 25, "Штамп");

            frame += await CreateLine(_extremeX + 270, _extremeX + 455, 50, 50, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 45, 45, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 455, 40, 40, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 35, 35, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 30, 30, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 455, 25, 25, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 20, 20, "Штамп");
            frame += await CreateLine(_extremeX + 405, _extremeX + 455, 20, 20, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 15, 15, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 455, 10, 10, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, 5, 5, "Штамп");
            frame += await CreateLine(_extremeX + 270, _extremeX + 335, _zeroY, _zeroY, "Штамп");

            // Заполнение правого подрамника ГОСТ 
            frame += await CreateText(_extremeX + 271, -4, "Утв.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 271, 1, "H. Kонтр", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 271, 11, "T. Kонтр", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 271, 16, "Проверил", "Штaмп", 1.5);
            frame += await CreateText(_extremeX + 271, 21, "Paзpaботал", "Штамп", 1.5);

            frame += await CreateText(_extremeX + 271, 26, "Изм.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 281, 26, "Кол.уч.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 291, 26, "Лист.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 301, 26, "Nдок.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 311, 26, "Подпись", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 326, 26, "Дата", "Штамп", 1.5);

            frame += await CreateText(_extremeX + 406, 21, "Стадия", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 421, 21, "Лист.", "Штамп", 1.5);
            frame += await CreateText(_extremeX + 436, 21, "Листов", "Штамп", 1.5);

            // Легенда
            frame += await CreateText(_extremeX + 310, 392, "У С Л О В Н Ы Е  О Б О З Н А Ч Е Н И Я", "ГЕО-Легенда");
            var vertexesGroundLegend = await CreateVertex(_extremeX + 310, 380, 3, "ГЕО-Легенда");
            vertexesGroundLegend += await CreateVertex(_extremeX + 320, 380, 3, "ГЕО-Легенда");
            frame += await CreatePolyline(3, vertexesGroundLegend, "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 380, "-Уровень земли", "ГЕО-Легенда");

            var vertexesTubeLegend = await CreateVertex(_extremeX + 310, 370, 1, "ГЕО-Легенда");
            vertexesTubeLegend += await CreateVertex(_extremeX + 320, 370, 1, "ГЕО-Легенда");
            frame += await CreatePolyline(1, vertexesTubeLegend, "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 370, "-Ось трубопровода", "ГЕО-Легенда");

            frame += await CreateInsert(_extremeX + 315, 360, "ЛЭП", "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 360, "-ЛЭП", "ГЕО-Легенда");

            frame += await CreateInsert(_extremeX + 315, 350, "УЗА", "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 350, "-Узел заторной арматуры", "ГЕО-Легенда");

            frame += await CreateInsert(_extremeX + 315, 340, "ВАНТУЗ", "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 340, "-Вантуза", "ГЕО-Легенда");

            frame += await CreateInsert(_extremeX + 315, 330, "ДОРОГА", "ГЕО-Легенда");
            frame += await CreateText(_extremeX + 325, 330, "-Дорога", "ГЕО-Легенда");

            return frame;
        }

        /// <summary>
        /// Отрисовка таблицы
        /// </summary>
        /// <param name="showBiasToLength"> Флаг отрисовки Уклон/Длина </param>
        /// <param name="showDepthFlag"> Флаг отрисовки строки Глубины </param>
        /// <param name="showTubeMarkFlag"> Флаг отрисовки строки Отметка трубы </param>
        /// <param name="showGroundMarkFlag"> Флаг отрисовки строки Отметка земли </param>
        /// <param name="showDistanceFlag"> Флаг отрисовки строки Расстояний </param>
        /// <param name="showPicketsFlag"> Флаг отрисовки строки Пикетов </param>
        /// <param name="showLinePlanFlag"> Флаг отрисовки строки План линии </param>
        /// <param name="showKmFlag"> Флаг отрисовки строки Километры </param>
        /// <returns> Строка с данными для отрисовки таблицы </returns>
        public async Task<string> TableDraw(
        bool showBiasToLength,
        bool showDepthFlag,
        bool showTubeMarkFlag,
        bool showGroundMarkFlag,
        bool showDistanceFlag,
        bool showPicketsFlag,
        bool showLinePlanFlag,
        bool showKmFlag)
        {
            var table = "";

            // Отрисовка таблицы для подписей строк 
            table += await CreateLine(0, 90, 0, 0, "л-Подвал");
            table += await CreateLine(0, 90, 5, 5, "Л-Подвал");
            table += await CreateLine(0, 90, 30, 30, "Л-Подвал");
            table += await CreateLine(0, 90, 35, 35, "Л-Подвал");
            table += await CreateLine(0, 90, 45, 45, "Л-Подвал");
            table += await CreateLine(0, 90, 60, 60, "Л-Подвал");
            table += await CreateLine(0, 90, 75, 75, "Л-Подвал");
            table += await CreateLine(0, 90, 95, 95, "Л-Подвал");
            table += await CreateLine(0, 90, 105, 105, "Л-Подвал");
            table += await CreateLine(95, _extremeX + 95, 110, 110, "Л-Подвал");
            table += await CreateLine(0, 0, 0, 105, "Л-Подвал");
            table += await CreateLine(90, 90, 0, 105, "Л-Подвал");
            // Отрисовка подписей строк
            if (showKmFlag)
            {
                table += await CreateText(5, 1, "Километры", "Л-Подвал");
            }
            if (showLinePlanFlag)
            {
                table += await CreateText(5, 15, "План линии", "Л-Подвал");
            }
            if (showPicketsFlag)
            {
                table += await CreateText(5, 31, "Пикеты", "Л-Подвал");
            }
            if (showDistanceFlag)
            {
                table += await CreateText(5, 38, "Расстояния, м", "Л-Подвал");
            }
            if (showTubeMarkFlag)
            {
                table += await CreateText(5, 50, "Проектная отметка верха трубы, м", "Л-Подвал", 2);
            }
            if (showGroundMarkFlag)
            {
                table += await CreateText(5, 65, "Абсолютные отметки земли, м", "Л-Подвал", 2);
            }
            if (showDepthFlag)
            {
                table += await CreateText(15, 78, "Глубина, м", "Л-Подвал");
            }
            if (showBiasToLength)
            {
                table += await CreateLine(0, 90, 95, 105, "Л-Подвал");
                table += await CreateText(5, 100, "уклон, %", "Л-Подвал");
                table += await CreateText(65, 96, "Длина, м", "Л-Подвал");
            }

            // Отрисовка подпрофильной таблицы 
            table += await CreateLine(95, _extremeX + 95, 0, 0, "Л-ПоДвал");
            table += await CreateLine(95, _extremeX + 95, 5, 5, "Л-Пoдвал");
            table += await CreateLine(95, _extremeX + 95, 30, 30, "Л-Пoдвал");
            table += await CreateLine(95, _extremeX + 95, 35, 35, "Л-Пoдвал");
            table += await CreateLine(95, _extremeX + 95, 45, 45, "Л-Подвал");
            table += await CreateLine(95, _extremeX + 95, 60, 60, "Л-Подвал");
            table += await CreateLine(95, _extremeX + 95, 75, 75, "Л-Подвал");
            table += await CreateLine(95, _extremeX + 95, 95, 95, "Л-Подвал");
            table += await CreateLine(95, _extremeX + 95, 105, 105, "Л-Hодвал");

            table += await CreateLine(95, 95, 35, 45);
            table += await CreateLine(95, 95, 60, 105);
            table += await CreateLine(_extremeX + 95, _extremeX + 95, 0, 105);

            // Заполнение подпрофильной таблицы

            return table;
        }

        /// <summary>
        /// Отрисовка шкалы масштаба
        /// </summary>
        /// <param name="data"> Данные для текущей страницы </param>
        /// <returns> Строчка с данными для отрисовки шкалы масштаба </returns>
        public async Task<string> ScaleDraw(List<ExternalData> data)
        {
            var scale = "";

            var minY = data.Min(x => x.TubeMark) - data.Min(x => x.TubeMark) % 10;
            var maxY = data.Max(x => x.GroundMark);
            var minScale = minY - (minY % 10) - 10;
            var maxScale = maxY - (maxY % 10);
            var lineY = 0.0;
            for (var i = 0; i + minScale < maxY + 10; i += 5)
            {
                lineY = 110 + (minY - minScale + i) * (1000.0 / _verticalScale);
                scale += await CreateLine(83.5, 85, lineY, lineY);

                var scaleText = (minScale + i).ToString("G", CultureInfo.InvariantCulture);
                scale += await CreateText(75, lineY, scaleText);
            }

            scale += await CreateLine(83.5, 83.5, 110 + minY - minScale * (1000.0 / _verticalScale), lineY);
            scale += await CreateLine(85, 85, 110 + minY - minScale * (1000.0 / _verticalScale), lineY);
            scale += await CreateText(5, 155, $"M 1:{_horizontalScale} по горизонтали");
            scale += await CreateText(5, 165, $"M 1:{_verticalScale} по вертикали");

            return scale;
        }

        // <summary>
        // Отрисовка километровых отметок
        // </summary>
        /// <param name="data"> Данные для текущей страницы </param>
        /// <param name="sectionLength"> Длина отрисовываемого участка в метрах </param>
        /// <param name="enteredKm"> Вручную заданный стартовый километр </param>
        // <returns> Строчка с данными для отрисовки километровых отметок </returns>
        public async Task<string> KmPointDraw(List<ExternalData> data, int sectionLength)
        {
            var kmPoints = "";

            var startKm = (int)(data[1].LengthBy3DLine / (Consts.MInKm));
            for (var i = 0; i <= sectionLength / 1000; i++)
            {
                var circleX = 95 + i * (1000000.0 / _horizontalScale);
                kmPoints += await CreateCircle(circleX, 2.5, 2.5);
                kmPoints += await CreateLine(circleX, circleX, 0, 7);
                var textX = 100 + i * (1000000.0 / _horizontalScale);
                var textVal = (startKm + i).ToString("G", CultureInfo.InvariantCulture);
                kmPoints += await CreateText(textX, 1, textVal);
            }

            return kmPoints;
        }

        /// <summary>
        /// Отрисовка меток пикетов для пикета
        /// </summary>
        /// <param name="data"> Данные для текущей страницы </param>
        /// <param name="sectionLength"> Длина отрисовываемого участка в метрах </param>
        /// <param name="enteredKm"> Вручную заданный стартовый километр </param>
        /// <returns> Строка с данными для отрисовки меток пикета </returns>
        public async Task<string> PicketsDraw(List<ExternalData> data, int sectionLength)
        {
            var pickets = "";

            var startKm = (int)(data[1].LengthBy3DLine / Consts.MInKm);
            for (var i = 0; i <= sectionLength / 100; i++)
            {
                var lineX = 95 + i * (100000.0 / _horizontalScale);
                pickets += await CreateLine(lineX, lineX, 35, 28);

                var textX = 96 + i * (100000.0 / _horizontalScale);
                pickets += await CreateText(textX, 31, (startKm * 10 + i).ToString());
            }

            return pickets;
        }

        /// <summary>
        /// Отрисовка точек
        /// </summary>
        /// <param name="data"> Данные для текущей станции </param>
        /// <param name="sectionLength"> Длина отрисовываемого участка в метрах </param>
        /// <param name="sectionNumber"> Номер создаваемого участка </param>
        /// <param name="showBiasToLength"> Флаг отрисовки строки Уклон/Длина </param>
        /// <param name="enteredStartLength"> Вручную заданное стартовое расстояние </param>
        /// <returns> Строка с данными для отрисовки точек </returns>
        public async Task<string> PointDraw(
            List<ExternalData> data,
            int sectionLength,
            int sectionNumber,
            bool showBiasToLength,
            double enteredStartLength)
        {
            var points = "";

            var minTubeDepth = data.Min(x => x.TubeMark);
            // Смещение вниз для уменьшения пустого места под линией
            var shift = minTubeDepth - (minTubeDepth % 10) - 20;
            var prevPointX = 0.0;
            var shiftNote = false;
            var startLengthShift = enteredStartLength - enteredStartLength % 1000;

            foreach (var point in data)
            {
                var lineX = 95 +
                    +(point.LengthBy2DLine - startLengthShift - sectionNumber * sectionLength) *
                    (1000.0 / _horizontalScale);
                if (showBiasToLength)
                {
                    if (data.IndexOf(point) != 0)
                    {
                        // Линии уклон/длина
                        if (point.BiasToLength > 0)
                        {
                            points += await CreateLine(prevPointX, lineX, 95, 105);
                        }
                        else
                        {
                            points += await CreateLine(lineX, prevPointX, 95, 105);
                        }
                    }
                }

                // Точки для строки уклон/длина
                points += await CreateLine(lineX, lineX, 105, 95);

                // Точки для строки Расстояния
                points += await CreateLine(lineX, lineX, 45, 35);

                // Точки для профиля
                var lineY = 110 + ((point.GroundMark - shift) * (1000.0 / _verticalScale));
                points += await CreateLine(lineX, lineX, lineY, 110);
                if (point.Note != "")
                {
                    var type = "";
                    switch (point.Note)
                    {
                        case "УЗА": type = "УЗА"; break;
                        case "ЖД": type = "ДОРОГА"; break;
                        case "АД": type = "ДОРОГА"; break;
                        case "GAZ": type = "GAZ"; break;
                        case "VOD": type = "VOD"; break;
                        case "DAT": type = "DAT"; break;
                    }
                    if (point.Note.Contains("Задвижка") || point.Note.Contains("УЗА"))
                    {
                        type = "УЗА";
                    }
                    if (point.Note.Contains("ВЛ"))
                    {
                        type = "ЛЭП";
                    }
                    if (lineX - prevPointX < 3 && shiftNote)
                    {
                        if (type == "GAZ" || type == "VOD" || type == "DAT")
                        {
                            points += await CreateText(lineX - 0.5, 120, point.Note, "0", 1.5, 90);
                            points += await CreateText(lineX + 2.5, 120, point.LengthByFormat, "0", 1.5, 90);
                        }
                        else
                        {
                            points += await CreateText(lineX - 0.5, lineY + 20, point.Note, "0", 1.5, 90);
                            points += await CreateLine(lineX, lineX, lineY, lineY + 35);
                            points += await CreateText(lineX + 2.5, lineY + 20, point.LengthByFormat, "0", 1.5, 90);
                            /*if (type != "")
                            {
                                points += await CreateInsert(lineX, lineY + 35, type);
                            }*/
                        }
                        shiftNote = false;
                    }
                    else
                    {
                        if (type == "GAZ" || type == "VOD" || type == "DAT")
                        {
                            points += await CreateText(lineX - 0.5, 112, point.Note, "0", 1.5, 90);
                            points += await CreateText(lineX + 2.5, 112, point.LengthByFormat, "0", 1.5, 90);
                        }
                        else
                        {
                            points += await CreateText(lineX - 0.5, lineY + 2, point.Note, "0", 2, 90);
                            points += await CreateLine(lineX, lineX, lineY, lineY + 25);
                            points += await CreateText(lineX + 2.5, lineY + 2, point.LengthByFormat, "0", 2, 90);

                            /*if (type != "")
                            {
                                points += await CreateInsert(lineX, lineY + 25, type);
                            }*/
                        }

                        shiftNote = true;
                    }
                }
                else
                {
                    shiftNote = false;
                }

                prevPointX = lineX;
            }

            // Дорисовка имён пикетов в начале и в конце участка
            var firstPicketX = 95 + (data.First().LengthBy2DLine - startLengthShift - sectionNumber * sectionLength) * (1000.0 / _horizontalScale);
            var firstPicketY = 90 + ((data.First().GroundMark - shift) * (1000.0 / _verticalScale));
            points += await CreateText(firstPicketX, firstPicketY, data.First().LengthByFormat, "0", 3, 90);

            var lastPicketY = 90 + ((data.Last().GroundMark - shift) * (1000.0 / _verticalScale));
            points += await CreateText(prevPointX + 4, lastPicketY, data.Last().LengthByFormat, "0", 3, 90);

            return points;
        }

        // Summary
        // Отрисовка профиля трубы и земли
        // <summary>
        // <param name="data"> Данные для текущей станции </param>
        // <param name="sectionLength"> Длина отрисовываемого участка в метрах </param>
        // <param name="sectionNumber"> Номер создаваемого участка </param>
        // <returns> Строка с данными для отрисовки точек </returns>
        public async Task<string> ProfileDraw(List<ExternalData> data, int sectionLength, int sectionNumber, double enteredStartLength)
        {
            var groundVertexes = "";
            var tubeVertexes = "";
            var minTubeDepth = data.Min(x => x.TubeMark);
            // Смещение вниз для уменьшения пустого места под линией
            var shiftVertical = minTubeDepth - (minTubeDepth % 10) - 20;
            var startLengthShift = enteredStartLength - enteredStartLength % 1000;

            foreach (var point in data)
            {
                var vertexX = 95 + (point.LengthBy2DLine - startLengthShift - sectionNumber * sectionLength) * (1000.0 / _horizontalScale);
                var vertexGroundY = 110 + ((point.GroundMark - shiftVertical) * (1000.0 / _verticalScale));
                groundVertexes += await CreateVertex(vertexX, vertexGroundY, 3);

                var vertexTubeY = 110 + ((point.TubeMark - shiftVertical) * (1000.0 / _verticalScale));
                tubeVertexes += await CreateVertex(vertexX, vertexTubeY, 1);

            }
            var groundLine = await CreatePolyline(3, groundVertexes);
            var tubeLine = await CreatePolyline(1, tubeVertexes);

            return groundLine + tubeLine;
        }

/// <summary>
/// Отрисовка параметров точек трубопровода
/// </summary>
/// <param name="data"> Данные для текущей страницы </param>
/// <param name="sectionLength"> Длина отрисовываемого участка в метрах </param>
/// <param name="sectionNumber"> Номер создаваемого участка </param>
/// <param name="showBiasToLength"> Флаг отрисовки строки Уклон/Длина </param>
/// <param name="showDepthFlag"> Флаг отрисовки строки Глубины </param>
/// <param name="showTubeMarkFlag"> Флаг отрисовки строки Отметка трубы </param>
/// <param name="showGroundMarkFlag"> Флаг отрисовки строки Отметка земли </param>
/// <param name="showDistanceFlag"> Флаг отрисовки строки Расстояний </param>
/// <param name="radius"> Радиус трубы </param>
/// <param name="enteredStartLength"> Вручную введённое стартовое расстояние </param>
/// <returns> Строка с данными для отрисовки параметров точек трубопровода </returns>
public async Task<string> NumbersDraw(
    List<ExternalData> data,
    int sectionLength,
    int sectionNumber,
    bool showBiasToLength,
    bool showDepthFlag,
    bool showTubeMarkFlag,
    bool showGroundMarkFlag,
    bool showDistanceFlag,
    int radius,
    double enteredStartLength)
        {
            var numbersString = "";

            var prevDist = 0.0;
            var startLengthShift = enteredStartLength - enteredStartLength % 1000;
            foreach (var point in data)
            {
                var textX = 96 + (point.LengthBy2DLine - startLengthShift - sectionNumber * sectionLength) * (1000.0 / _horizontalScale);

                // Расстояния
                var distanceX = 0.0;
                var distanceY = 38.0;
                var turn = 0.0;
                if (data.IndexOf(point) == 0)
                {
                    distanceX = 93;
                }
                else
                {
                    distanceX = prevDist + (textX - prevDist - 4) / 2;
                }
                if (textX - prevDist < 7)
                {
                    turn = 90;
                    distanceX = textX - 1.1;
                }
                if (showDistanceFlag)
                {
                    var distance = point.DistanceBy2DLine.ToString("N1", CultureInfo.InvariantCulture);
                    numbersString += await CreateText(distanceX, distanceY, distance, "0", 1.5, turn);
                }

                // Отметки верхнего края трубы
                if (showTubeMarkFlag && textX - prevDist > 1)
                {
                    var shift = (double)radius / Consts.MmInM;
                    var tubeMarkText = (point.GroundMark - point.TubeDepth + shift).ToString("N2", CultureInfo.InvariantCulture);
                    numbersString += await CreateText(textX, 48, tubeMarkText, "0", 1.75, 90);
                }

                // Отметки земли
                if (showGroundMarkFlag && textX - prevDist > 1)
                {
                    var groundMarkText = point.GroundMark.ToString("N2", CultureInfo.InvariantCulture);
                    numbersString += await CreateText(textX, 62, groundMarkText, "0", 1.75, 90);
                }

                // Глубина
                if (showDepthFlag && textX - prevDist > 1)
                {
                    var depth = point.TubeDepth.ToString("N1", CultureInfo.InvariantCulture);
                    numbersString += await CreateText(textX, 78, depth, "0", 1.5, 90);
                }

                // Уклон и длина
                if (showBiasToLength && data.IndexOf(point) != 0)
                {
                    var biasX = prevDist;
                    var biasY = 103.5;
                    var lengthX = textX - 5;
                    var lengthY = 95.5;

                    if (point.BiasToLength > 0)
                    {
                        if (turn == 90)
                        {
                            if (textX - prevDist > 2)
                            {
                                biasX = prevDist + 0.5;
                                biasY = 102.0;
                                lengthX = textX - 1.5;
                            }
                            else
                            {
                                biasX = prevDist - 0.5;
                                biasY = 106.0;
                                lengthX = textX - 1.5;
                                lengthY = 91.5;
                            }
                        }
                    }
                    else
                    {
                        biasX = textX - 5;
                        lengthX = prevDist;
                        if (turn == 90)
                        {
                            if (textX - prevDist > 2)
                            {
                                biasX = textX - 1.5;
                                biasY = 101.0;
                                lengthX = prevDist + 0.5;
                            }
                            else
                            {
                                biasX = textX - 0.5;
                                biasY = 106.0;
                                lengthY = 91.5;
                            }
                        }
                    }
                    var bias = point.BiasToLength.ToString("N3", CultureInfo.InvariantCulture);
                    var length = point.DistanceBy2DLine.ToString("N2", CultureInfo.InvariantCulture);
                    numbersString += await CreateText(biasX, biasY, bias, "0", 1, turn);
                    numbersString += await CreateText(lengthX, lengthY, length, "0", 1, turn);
                }
                prevDist = textX;
            }
            return numbersString;
        }

        /// <summary>
        /// Отрисовка трассы
        /// </summary>
        /// <param name="data"> Данные профиля </param>
        /// <param name="sectionLength"> Длина отрисовываемого участка в метpax </param>
        /// <param name="sectionNumber"> номер создаваемого участка </param>
        /// <returns> Строка с данными для отрисовки трассы </returns>
        public async Task<string> TubeLineDraw(List<ExternalData> data, int sectionLength, int sectionNumber)
        {
            var tubeLine = "";
            _zeroX = data.Min(l => l.X);
            _zeroY = data.Min(l => l.Y);
            _zeroZ = data.Min(l => l.TubeMark);
            var vertexes = "";
            var picketCounter = 1;
            var prevPicketDist = 100.0 + sectionLength * sectionNumber;
            var prevLength = 0.0;
            var prevX = 0.0;
            var prevY = 0.0;
            foreach (var point in data)
            {
                var pointX = point.X - _zeroX;
                var pointY = point.Y - _zeroY;
                var pointZ = point.TubeMark - _zeroZ;
                // отрисовка точки
                if (!point.PointNumber.Contains("Inter"))
                {
                    tubeLine += await CreateCircle(pointX, pointY, 2.5);
                    tubeLine += await CreatePoint(pointX, pointY, pointZ);
                    tubeLine += await CreateLine(pointX - 3.5, pointX + 3.5, pointY - 3.5, pointY + 3.5);
                    tubeLine += await CreateLine(pointX - 3.5, pointX + 3.5, pointY + 3.5, pointY - 3.5);
                    tubeLine += await CreateText(pointX - 1, pointY - 6, point.PointNumber, "0", 3, 0, 1);
                }
                else if (data.IndexOf(point) == 0)
                {
                    // Отрисовка пикета вместо фейковой точки
                    // соседние точки опредеялются в зависимости от того начало это или конец секции
                    var angle = -Math.Atan2(data[1].X - pointX, data[1].Y - pointY);
                    var vertexPicket = await CreateVertex(pointX + Math.Cos(angle) * 7.5, pointY + Math.Sin(angle) * 7.5, 4);
                    vertexPicket += await CreateVertex(pointX + Math.Cos(angle + Math.PI) * 10, pointY + Math.Sin(angle + Math.PI) * 10, 4);
                    tubeLine += await CreatePolyline(4, vertexPicket);
                    tubeLine += await CreateText(pointX + Math.Cos(angle) * 7.5 - 2, pointY + Math.Sin(angle) * 7.5 - 2, (sectionNumber * sectionLength / 1000 * 10).ToString(), "0", 3, 0, 4);
                    tubeLine += await CreateCircle(pointX + Math.Cos(angle + Math.PI) * 7.5, pointY + Math.Sin(angle + Math.PI) * 7.5, 2.5, "0", 4);
                    tubeLine += await CreateText(pointX + Math.Cos(angle + Math.PI) * 10 + 3, pointY + Math.Sin(angle + Math.PI) * 10 + 3, (sectionNumber * sectionLength / 1000).ToString(), "0", 3, 0, 4);
                }
                else if (point.Equals(data.Last()))
                {
                    var angle = -Math.Atan2(pointX - prevX, pointY - prevY);
                    var vertexPicket = await CreateVertex(pointX + Math.Cos(angle) * 7.5, pointY + Math.Sin(angle) * 7.5, 4);
                    vertexPicket += await CreateVertex(pointX + Math.Cos(angle + Math.PI) * 10, pointY + Math.Sin(angle + Math.PI) * 10, 4);
                    tubeLine += await CreatePolyline(4, vertexPicket);
                    tubeLine += await CreateText(pointX + Math.Cos(angle) * 7.5 - 2, pointY + Math.Sin(angle) * 7.5 - 2, ((sectionNumber + 1) * sectionLength / 1000 * 10).ToString(), "0", 3, 0, 4);
                    tubeLine += await CreateCircle(pointX + Math.Cos(angle + Math.PI) * 7.5, pointY + Math.Sin(angle + Math.PI) * 7.5, 2.5, "0", 4);
                    tubeLine += await CreateText(pointX + Math.Cos(angle + Math.PI) * 10 + 3, pointY + Math.Sin(angle + Math.PI) * 10 + 3, ((sectionNumber + 1) * sectionLength / 1000).ToString(), "0", 3, 0, 4);
                }

                // вершина для линии трубы
                vertexes += await CreateVertex(pointX, pointY, 5);

                if (point.LengthBy2DLine > prevPicketDist && prevLength < prevPicketDist)
                {
                    while (point.LengthBy2DLine > prevPicketDist)
                    {
                        var lambda = (prevPicketDist - prevLength) / (point.LengthBy2DLine - prevPicketDist);
                        var picketX = (prevX + pointX * lambda) / (1 + lambda);
                        var picketY = (prevY + pointY * lambda) / (1 + lambda);

                        // отрисовка пикетов
                        var angle = -Math.Atan2(pointX - prevX, pointY - prevY);
                        var vertexPicket = await CreateVertex(picketX + Math.Cos(angle) * 7.5, picketY + Math.Sin(angle) * 7.5, 4);
                        vertexPicket += await CreateVertex(picketX + Math.Cos(angle + Math.PI) * 10, picketY + Math.Sin(angle + Math.PI) * 10, 4);
                        tubeLine += await CreatePolyline(4, vertexPicket);
                        tubeLine += await CreateText(picketX + Math.Cos(angle) * 7.5 - 2, picketY + Math.Sin(angle) * 7.5 - 2, (picketCounter + sectionNumber * sectionLength / 1000 * 10).ToString(), "0", 3, 0, 4);

                        // отрисовка километровых меток
                        if (picketCounter % 10 == 0)
                        {
                            tubeLine += await CreateCircle(picketX + Math.Cos(angle + Math.PI) * 7.5, picketY + Math.Sin(angle + Math.PI) * 7.5, 2.5, "0", 4);
                            tubeLine += await CreateText(picketX + Math.Cos(angle + Math.PI) * 10 + 3, picketY + Math.Sin(angle + Math.PI) * 10 + 3, (picketCounter / 10 + sectionNumber * sectionLength / 1000).ToString(), "0", 3, 0, 4);
                        }
                        picketCounter++;
                        prevPicketDist += 100;
                    }
                }
                prevX = pointX;
                prevY = pointY;
                prevLength = point.LengthBy2DLine;
            }
            var track = await CreatePolyline(5, vertexes);
            tubeLine += track;

            return tubeLine;
        }

        /// <summary>
        /// Создание элемента "линия"
        /// </summary>
        /// <param name="x1"> Координата X начала </param>
        /// <param name="x2"> Координата X конца </param>
        /// <param name="y1"> Координата Y начала </param>
        /// <param name="y2"> Координата Y конца </param>
        /// <param name="layer"> Название слоя </param>
        /// <returns> Элемент "линия" для формата dxf </returns>
        private async Task<string> CreateLine(double x1, double x2, double y1, double y2, string layer = "0")
        {
            string lineSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\LINE.txt";
            var lineSample = "";
            using (StreamReader reader = new StreamReader(lineSamplePath))
            {
                lineSample = await reader.ReadToEndAsync();
            }

            var x1String = x1.ToString("G", CultureInfo.InvariantCulture);
            var x2String = x2.ToString("G", CultureInfo.InvariantCulture);
            var y1String = y1.ToString("G", CultureInfo.InvariantCulture);
            var y2String = y2.ToString("G", CultureInfo.InvariantCulture);

            _hexIndex++;
            lineSample = lineSample.Replace("{HEX}", _hexIndex.ToString("X"));
            lineSample = lineSample.Replace("{LAYER}", layer);
            lineSample = lineSample.Replace("{X1}", $"{x1String}");
            lineSample = lineSample.Replace("{X2}", $"{x2String}");
            lineSample = lineSample.Replace("{Y1}", $"{y1String}");
            lineSample = lineSample.Replace("{Y2}", $"{y2String}");
            return "\n" + lineSample;
        }

        /// <summary>
        /// Создание элемента "текст"
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        /// <param name="text"> Текст </param>
        /// <param name="layer"> Название слоя </param>
        /// <param name="high"> Высота текста </param>
        /// <param name="turn"> Угол поворота текста </param>
        /// <param name="color"> Цвет </param>
        /// <returns> Элемент "текст" для формата dxf </returns>
        private async Task<string> CreateText(
            double x,
            double y,
            string text,
            string layer = "0",
            double high = 3.0,
            double turn = 0.0,
            int color = 7)
        {
            string textSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\TEXT.txt";
            var textSample = "";
            using (StreamReader reader = new StreamReader(textSamplePath))
            {
                textSample = await reader.ReadToEndAsync();
            }

            var xString = x.ToString("G", CultureInfo.InvariantCulture);
            var yString = y.ToString("G", CultureInfo.InvariantCulture);
            var highString = high.ToString("N1", CultureInfo.InvariantCulture);
            var turnString = turn.ToString("N1", CultureInfo.InvariantCulture);

            _hexIndex++;
            textSample = textSample.Replace("{HEX}", _hexIndex.ToString("X"));
            textSample = textSample.Replace("{LAYER}", layer);
            textSample = textSample.Replace("{COLOR}", $"{color}");
            textSample = textSample.Replace("{X}", $"{xString}");
            textSample = textSample.Replace("{Y}", $"{yString}");
            textSample = textSample.Replace("{HIGH}", $"{highString}");
            textSample = textSample.Replace("{TURN}", $"{turnString}");
            textSample = textSample.Replace("{TEXT}", $"{text}");

            return "\n" + textSample;
        }

        /// <summary>
        /// Создание элемента "круг"
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        /// <param name="r"> Радиус </param>
        /// <param name="layer"> Название слоя </param>
        /// <param name="color"> Цвет </param>
        /// <returns> Элемент "круг" для формата dxf / returns: string</returns>
        private async Task<string> CreateCircle(double x, double y, double r, string layer = "0", int color = 7)
        {
            string circleSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\CIRCLE.txt";
            var circleSample = "";
            using (StreamReader reader = new StreamReader(circleSamplePath))
            {
                circleSample = await reader.ReadToEndAsync();
            }

            var xString = x.ToString("G", CultureInfo.InvariantCulture);
            var yString = y.ToString("G", CultureInfo.InvariantCulture);
            var rString = r.ToString("G", CultureInfo.InvariantCulture);

            _hexIndex++;
            circleSample = circleSample.Replace("{HEX}", _hexIndex.ToString("X"));
            circleSample = circleSample.Replace("{LAYER}", layer);
            circleSample = circleSample.Replace("{COLOR}", $"{color}");
            circleSample = circleSample.Replace("{X}", $"{xString}");
            circleSample = circleSample.Replace("{Y}", $"{yString}");
            circleSample = circleSample.Replace("{R}", $"{rString}");

            return "\n" + circleSample;
        }

        /// <summary>
        /// Создание элемента "полилиния"
        /// </summary>
        /// <param name="color"> Цвет линии (1 - красный, 3 - зеленый) </param>
        /// <param name="layer"> Название слоя </param>
        /// <param name="vertexes"> Набор вершин линии (создаётся в методе CreateVertex) </param>
        /// <returns> Элемент "полилиния" для формата dxf </returns>
        private async Task<string> CreatePolyline(int color, string vertexes, string layer = "0")
        {
            string polylineSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\POLYLINE.txt";
            var polylineSample = "";
            using (StreamReader reader = new StreamReader(polylineSamplePath))
            {
                polylineSample = await reader.ReadToEndAsync();
            }
            _hexIndex++;
            polylineSample = polylineSample.Replace("{HEX1}", _hexIndex.ToString("X"));
            _hexIndex++;
            polylineSample = polylineSample.Replace("{HEX}", _hexIndex.ToString("X"));
            polylineSample = polylineSample.Replace("{LAYER}", layer);
            polylineSample = polylineSample.Replace("{COLOR}", $"{color} ");
            polylineSample = polylineSample.Replace("{VERTEXES}", vertexes);

            return "\n" + polylineSample;
        }

        /// <summary>
        /// Создание элемента "вершина"
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        /// <param name="color"> Цвет линии (1 - красный, 3 - зеленый) </param>
        /// <param name="layer"> Название слоя </param>
        /// <returns> Элемент "вершина" для формата dxf </returns>
        private async Task<string> CreateVertex(double x, double y, int color, string layer = "0")
        {
            string vertexSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\VERTEX.txt";
            var vertexSample = "";
            using (StreamReader reader = new StreamReader(vertexSamplePath))
            {
                vertexSample = await reader.ReadToEndAsync();
            }

            var xString = x.ToString("G", CultureInfo.InvariantCulture);
            var yString = y.ToString("G", CultureInfo.InvariantCulture);

            _hexIndex++;
            vertexSample = vertexSample.Replace("{HEX}", _hexIndex.ToString("X"));
            vertexSample = vertexSample.Replace("{LAYER}", layer);
            vertexSample = vertexSample.Replace("{COLOR}", $"{color}");
            vertexSample = vertexSample.Replace("{X}", $"{xString}");
            vertexSample = vertexSample.Replace("{Y}", $"{yString}");

            return vertexSample + "\n";
        }

        /// <summary>
        /// Создание элемента "вставка"
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        /// <param name="type"> Тип вставки </param>
        /// <param name="layer"> Название слоя </param>
        /// <returns> Элемент "вставка" для формата dxf </returns>
        private async Task<string> CreateInsert(double x, double y, string type, string layer = "0")
        {
            string insertSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\INSERT.txt";
            var insertSample = "";
            using (StreamReader reader = new StreamReader(insertSamplePath))
            {
                insertSample = await reader.ReadToEndAsync();
            }

            var xString = x.ToString("G", CultureInfo.InvariantCulture);
            var yString = y.ToString("G", CultureInfo.InvariantCulture);

            _hexIndex++;
            insertSample = insertSample.Replace("{HEX}", _hexIndex.ToString("X"));
            insertSample = insertSample.Replace("{LAYER}", layer);
            insertSample = insertSample.Replace("{TYPE} ", $"{type}");
            insertSample = insertSample.Replace("{X}", $"{xString}");
            insertSample = insertSample.Replace("{Y}", $"{yString}");

            return "\n" + insertSample;
        }


        /// <summary>
        /// Создание элекмента "точки"
        /// </summary>
        /// <param name="x"> Координата X </param>
        /// <param name="y"> Координата Y </param>
        /// <param name="layer"> Название слоя </param>
        /// <returns> Элемент "точка" для формата dxf </returns>
        private async Task<string> CreatePoint(double x, double y, double z, string layer = "0")
        {
            string pointSamplePath = AppDomain.CurrentDomain.BaseDirectory + "\\SAMPLES\\POINT.txt";
            var pointSample = "";
            using (StreamReader reader = new StreamReader(pointSamplePath))
            {
                pointSample = await reader.ReadToEndAsync();
            }

            var xString = x.ToString("G", CultureInfo.InvariantCulture);
            var yString = y.ToString("G", CultureInfo.InvariantCulture);
            var zString = z.ToString("G", CultureInfo.InvariantCulture);

            _hexIndex++;
            pointSample = pointSample.Replace("{HEX}", _hexIndex.ToString("X"));
            pointSample = pointSample.Replace("{LAYER}", layer);
            pointSample = pointSample.Replace("{X}", $"{xString}");
            pointSample = pointSample.Replace("{Y}", $"{yString}");
            pointSample = pointSample.Replace("{Z}", $"{zString}");
            return "\n" + pointSample;
        }


        /*public async Task<string> CreateKren()
        {
        var str = "";
        bool twoStationFlag = true;
        double alpha = 0.79;
        double krenX = 17;
        double krenY = 17;
        // Мачта центр
        str += await CreateCircle(0, 0, 5);
        str += await CreateLine(-7, 7, -7, 7, "0");
        str += await CreateLine(-7, 7, 7, -7, "0");

        /1 ось Х
        str += await CreateLine(0, 50, 0, 0, "0");
        str += await CreateLine(45, 50, 1, 0, "0");
        str += await CreateLine(45, 50, -1, 0, "0");
        str += await CreateText(50, 5, "X", "0");

        // Линия стоянки 1
        str += await CreateLine(0, 0, 0, -100, "0");
        str += await CreateText(5, -100, "Cт. 1", "0");
        if (twoStationFlag)
        {
        Tuple<double, double> shiftedYAxisPoint = rollPoint(0, 50, alpha);
        // ось У
        str += await CreateLine(0, shiftedYAxisPoint.Item1, 0, shiftedYAxisPoint.Item2, "0");
        Tuple<double, double> shiftedHornPoint = rollPoint(1, 45, alpha);
        str += await CreateLine(shiftedYAxisPoint.Iteml, shiftedHornPoint.Item1, shiftedYAxisPoint.Item2, shiftedHornPoint.Item2, "0");
        shiftedHornPoint = rollPoint(-1, 45, alpha);
        str += await CreateLine(shiftedYAxisPoint.Item1, shiftedHornPoint.Item1, shiftedYAxisPoint.Item2, shiftedHornPoint.Item2, "0");
        Tuple<double, double> shiftedAxisMarkPoint = rollPoint(5, 50, alpha);
        str += await CreateText(shiftedAxisMarkPoint.Item1, shiftedAxisMarkPoint.Item2, "y", "0");

        // Линия стоянки 2
        Tuple<double, double> shiftedStationPoint = rollPoint(100, 0, alpha);
        str += await CreateLine(0, shiftedStationPoint.Item1, 0, shiftedStationPoint.Item2, "0");
        Tuple<double, double> shiftedStationMarkPoint = rollPoint(100, 5, alpha);
        str += await CreateText(shiftedStationMarkPoint.Item1, shiftedStationMarkPoint.Item2, "Cт. 2", "0");
        }
        else
        {
        // ось Y
        str += await CreateLine(0, 0, 0, 50, "0");
        str += await CreateLine(0, 1, 50, 45, "0");
        str += await CreateLine(0, 1, 50, 45, "0");
        str += await CreateText(50, 5, "y", "0");
        }

        // Линия крена
        str += await CreateLine(0, krenX, 0, krenY, "0");
        double xBase= 0.0;
        double yBase = Math.Sqrt(Math.Pow(krenX, 2) + Math.Pow(krenY,2));
        double angle = -Math.Atan2(krenX, krenY) ;
        Tuple<double, double> shiftedKrenHornPoint = rollPoint(xBase + 1, yBase - 5, angle);
        str += await CreateLine(shiftedKrenHornPoint.Item1, krenX, shiftedKrenHornPoint.Item2, krenY, "0");
        shiftedKrenHornPoint = rollPoint(xBase - 1, yBase - 5, angle);
        str += await CreateLine(shiftedKrenHornPoint.Item1, krenX, shiftedKrenHornPoint.Item2, krenY, "0");
        str += await CreateText (krenX, krenY, "Суммарное отклонение", "0");

        return str;
        }

        private Tuple<double, double> rollPoint(double x, double y, double alpha, double xZero = 0, double yZero = 0)
        {
        double xShifted = -Math.Sin(alpha) * (y - yZero) + Math.Cos(alpha) * (x - xZero);
        double yShifted = Math.Cos(alpha) * (y - yZero) + Math.Sin(alpha) * (x - xZero);
        return new Tuple<double, double>(xShifted, yShifted);
        }*/
    }
}