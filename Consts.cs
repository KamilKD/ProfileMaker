using System;
using System.Collections.Generic;
using System.Text;

namespace ProfileMaker
{
    /// <summary>
    /// Настройки для схемы трубопровода
    /// </summary>
    public static class Consts
    {

        /// <summary>
        /// Масштаб по горизонтали 
        /// </summary>
        public static int HorizontalScale = 5000;

        /// <summary>
        /// Масштаб по вертикали
        /// </summary>
        public static int VerticalScale = 1000;

        /// <summary>
        /// Масштаб по вертикали, грунта
        /// </summary>
        public static int VerticalGroundScale = 100;

        /// <summary>
        /// Масштаб для отрисовки плана
        /// </summary>
        public static int PlaneScale = 1000;

        /// <summary>
        /// Длина участка при экспорте, м
        /// </summary>
        public static int LineLength = 5000;

        public static int MInKm = 1000;

        public static int MmInM = 1000;

        public static string DirectoryPath = @"C:\Users\1\Desktop\3\Работа\Транснефть\Профиль\ProfileMaker\Materials";


    }
}
