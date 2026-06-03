namespace ProfileMaker.Models
{
    /// <summary>
    /// Класс dxf групп для строковых кодов (0 - 9)
    /// </summary>
    public class DxfStringGroup
    {
        public int Code;

        public string Value;

        public DxfStringGroup(int code, string value)
        {
            Code = code;
            Value = value;
        }
    }

    /// <summary>
    /// Класс dxf групп для кодов с плавающей точкой (10 - 59)
    /// </summary>
    public class DxfDoubleGroup
    {
        public int Code;

        public string Value;

        public DxfDoubleGroup(int code, string value)
        {
            Code = code;
            Value = value;
        }
    }

    /// <summary>
    /// Класс dxf групп для целочисленных кодов (60 - 79)
    /// </summary>
    public class DxfIntGroup
    {
        public int Code;
        public string Value;

        public DxfIntGroup(int code, string value)
        {
            Code = code;
            Value = value;
        }
    }
}
