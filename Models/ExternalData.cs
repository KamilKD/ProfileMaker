using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace ProfileMaker.Models
{

    /// <summary>
    /// Входные даннные импортированные из файла ехсеl
    /// </summary>
    public class ExternalData
    {
        /// <summary>
        /// № точки
        /// </summary>
        public string PointNumber { get; private set; }

        /// <summary>
        /// Координата Х
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Отметка земли
        /// </summary>
        public double GroundMark { get; private set; }

        /// <summary>
        /// Глубина трубы
        /// </summary>
        public double TubeDepth { get; private set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// Расстояние между точками по 3д линии
        /// </summary>
        public double DistanceBy3DLine { get; private set; }

        /// <summary>
        /// Общая длина по 3д линии земли
        /// </summary>
        public double LengthBy3DOnGround { get; private set; }

        /// <summary>
        /// Расстояние между точками по 2д линии
        /// </summary>
        public double DistanceBy2DLine { get; private set; }

        /// <summary>
        /// Общая длина по 2д линии
        /// </summary>
        public double LengthBy2DLine { get; private set; }

        /// <summary>
        /// Отметка трубы
        /// </summary>
        public double TubeMark { get; private set; }

        /// <summary>
        /// Растояние между точками по 3д линии трубы
        /// </summary>
        public double DistanceBy3DOnTube { get; private set; }

        /// <summary>
        /// Общая длина по 3д линии трубы
        /// </summary>
        public double LengthBy3DLine { get; private set; }

        /// <summary>
        /// Уклон/длина по трубе
        /// </summary>
        public double BiasToLength { get; private set; }

        /// <summary>
        /// Длина по 2д в формате ПК0+00,00
        /// </summary>
        public string LengthByFormat { get; private set; }

        public ExternalData(string pointNumber,
        double x,
        double y,
        double groundMark,
        double tubeDepth,
        string note,
        double distanceBy3DLine,
        double lengthBy3DOnGround,
        double distanceBy2DLine,
        double lengthBy2DLine,
        double tubeMark,
        double distanceBy3DOnTube,
        double lengthBy3DLine,
        double biasToLength,
        string lengthByFormat)
        {
            PointNumber = pointNumber;
            X = x;
            Y = y;
            GroundMark = groundMark;
            TubeDepth = tubeDepth;
            Note = note;
            DistanceBy3DLine = distanceBy3DLine;
            LengthBy3DOnGround = lengthBy3DOnGround;
            DistanceBy2DLine = distanceBy2DLine;
            LengthBy2DLine = lengthBy2DLine;
            TubeMark = tubeMark;
            DistanceBy3DOnTube = distanceBy3DOnTube;
            LengthBy3DLine = lengthBy3DLine;
            BiasToLength = biasToLength;
            LengthByFormat = lengthByFormat;
        }

        /// <summary>
        /// Создание промежуточной точки для концов участков
        /// </summary>
        /// <param name="leftPoint"> Точка слева от промежуточной </param>
        /// <param name="rightPoint"> Тoчкa справа от промежуточной </param>
        /// <param name="lineLength"> Длина учстков разбиения, м </param>
        /// <returns> Промежуточная точка </returns>
        public static ExternalData CreateFakePoint(ExternalData leftPoint, ExternalData rightPoint, int lineLength)
        {
            var lambda = (lineLength - leftPoint.LengthBy2DLine) / (rightPoint.LengthBy2DLine - lineLength);

            var number = $"InterPoint {leftPoint.PointNumber}-{rightPoint.PointNumber}";
            var x = (leftPoint.X + rightPoint.X * lambda) / (1 + lambda);
            var y = (leftPoint.Y + rightPoint.Y * lambda) / (1 + lambda);
            var groundMark = (leftPoint.GroundMark + rightPoint.GroundMark * lambda) / (1 + lambda);
            var tubeDepth = (leftPoint.TubeDepth + rightPoint.TubeDepth * lambda) / (1 + lambda);
            var note = "";
            var distanceBy3DLine = Math.Sqrt(Math.Pow(x - leftPoint.X, 2) + Math.Pow(y - leftPoint.Y, 2) + Math.Pow(groundMark - leftPoint.GroundMark, 2));
            var lengthBy3DOnGround = distanceBy3DLine + leftPoint.LengthBy3DOnGround;
            var distanceBy2DLine = Math.Sqrt(Math.Pow(x - leftPoint.X, 2) + Math.Pow(y - leftPoint.Y, 2));
            var lengthBy2DLine = distanceBy2DLine + leftPoint.LengthBy2DLine;
            var tubeMark = groundMark - tubeDepth;
            var distanceBy3DOnTube = Math.Sqrt(Math.Pow(x - leftPoint.X, 2) + Math.Pow(y - leftPoint.Y, 2) + Math.Pow(tubeMark - leftPoint.TubeMark, 2));
            var lengthBy3DLine = distanceBy3DOnTube + leftPoint.LengthBy3DLine;
            var biasToLength = Math.Atan2(tubeMark - leftPoint.TubeMark, distanceBy3DOnTube);
            var lengthByFormat = leftPoint.LengthByFormat;

            return new ExternalData(
            number,
            x,
            y,
            groundMark,
            tubeDepth,
            note,
            distanceBy3DLine,
            lengthBy3DOnGround,
            distanceBy2DLine,
            lengthBy2DLine,
            tubeMark,
            distanceBy3DOnTube,
            lengthBy3DLine,
            biasToLength,
            lengthByFormat);
        }

        /// <summary>
        /// Создание промежуточной точки для интерполированного профиля
        /// </summary>
        /// <param name="leftPoint"> Точка слева от промежуточной </param>
        /// <param name="rightPoint"> Тoчкa справа от промежуточной </param>
        /// <param name="prevPoint"> Прeдыдушая интрполированная Toчkа </param>
        /// <param name="lineLength"> Длина учстков разбиения, м </раram>
        /// <returns> Интерполированная точка </returns>
        public static ExternalData InterpolateByInterval(ExternalData leftPoint, ExternalData rightPoint, ExternalData prevPoint, int lineLength)
        {
            var lambda = (lineLength - leftPoint.LengthBy2DLine) / (rightPoint.LengthBy2DLine - lineLength);

            var number = $"InterPoint {leftPoint.PointNumber}-{rightPoint.PointNumber}";
            var x = (leftPoint.X + rightPoint.X * lambda) / (1 + lambda);
            var y = (leftPoint.Y + rightPoint.Y * lambda) / (1 + lambda);
            var groundMark = (leftPoint.GroundMark + rightPoint.GroundMark * lambda) / (1 + lambda);
            var tubeDepth = (leftPoint.TubeDepth + rightPoint.TubeDepth * lambda) / (1 + lambda);

            var note = "";
            var distanceBy3DLine = Math.Sqrt(Math.Pow(x - prevPoint.X, 2) + Math.Pow(y - prevPoint.Y, 2) + Math.Pow(groundMark - prevPoint.GroundMark, 2));
            var lengthBy3DOnGround = distanceBy3DLine + prevPoint.LengthBy3DOnGround;
            var distanceBy2DLine = Math.Sqrt(Math.Pow(x - prevPoint.X, 2) + Math.Pow(y - prevPoint.Y, 2));
            var lengthBy2DLine = distanceBy2DLine + prevPoint.LengthBy2DLine;
            var tubeMark = groundMark - tubeDepth;
            var distanceBy3DOnTube = Math.Sqrt(Math.Pow(x - prevPoint.X, 2) + Math.Pow(y - prevPoint.Y, 2) + Math.Pow(tubeMark - prevPoint.TubeMark, 2));
            var lengthBy3DLine = distanceBy3DOnTube + prevPoint.LengthBy3DLine;
            var biasToLength = Math.Atan2(tubeMark - prevPoint.TubeMark, distanceBy3DOnTube);
            var lengthByFormat = prevPoint.LengthByFormat;

            return new ExternalData(
            number,
            x,
            y,
            groundMark,
            tubeDepth,
            note,
            distanceBy3DLine,
            lengthBy3DOnGround,
            distanceBy2DLine,
            lengthBy2DLine,
            tubeMark,
            distanceBy3DOnTube,
            lengthBy3DLine,
            biasToLength,
            lengthByFormat);
        }
    }
}
