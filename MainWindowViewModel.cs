using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ProfileMaker.Models;
using ProfileMaker.Utils;

namespace ProfileMaker
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly DialogService _dialogService;
        // Здесь будут храниться данные из эксельника 
        private List<ExternalData> _externalData;

        public MainWindowViewModel()
        {
            _dialogService = new DialogService();
            _externalData = new List<ExternalData>();
            HorizontalScale = Consts.HorizontalScale;
            VerticalScale = Consts.VertcalScale;
            VerticalGroundScale = Consts.VertcalGroundScale;
            LineLength = Consts.LineLength;
            PlanLength = Consts.LineLength;
            ShowBiasToLengthFlag = true;
            ShowDepthFlag = true;
            ShowTubeMarkFlag = true;
            ShowGroundMarkFlag = true;
            ShowDistanceFlag = true;
            ShowPicketsFlag = true;
            ShowLinePlanFlag = false;
            ShowKmFlag = true;
            UseInterpolateCoordinates = false;
            InterpolatedInterval = 100;
            EnterStartLength = false;
            StartLength = 0.0;
            Radius = 350;
            _loadFileMessage = "";
            _saveButtonEnableFlag = false;
            /*if (DateTime.Now. Date >= new DateTime (2023, 3, 1))
            {
            MessageBox.Show("Update required.", "ProfileMaker Application");
            Environment.Exit (0);
            }*/
        }

        /// <summary>
        /// Масштаб по горизонтали
        /// </summary>
        public int HorizontalScale { get; set; }

        /// <summary>
        /// Масштаб по вертикали
        /// </summary>
        public int VerticalScale { get; set; }

        /// <summary>
        /// Масштаб по вертикали, грунта
        /// </summary>
        public int VerticalGroundScale { get; set; }

        /// <summary>
        /// Длина участка при экспорте, м
        // </summary>
        public int LineLength { get; set; }

        /// <summary>
        /// Длина участка плана при экспорте, м
        /// </summary>
        public int PlanLength { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Уклон/Длина
        /// </summary>
        public bool ShowBiasToLengthFlag { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Глубина
        /// </summary>
        public bool ShowDepthFlag { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Отметка трубы
        /// </summary>
        public bool ShowTubeMarkFlag { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Отемтка земли
        /// </summary>
        public bool ShowGroundMarkFlag { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Расстояния
        /// </summary>
        public bool ShowDistanceFlag { get; set; }

        /// <summary>
        /// Флаг отрисовки строки Пикеты
        /// </summary>
        public bool ShowPicketsFlag { get; set; }

        /// <summary>
        /// флаг отрисовки строки План линии
        /// </summary>
        public bool ShowLinePlanFlag { get; set; }

        /// <summary>
        /// флаг отрисовки строки Километры
        /// </summary>
        public bool ShowKmFlag { get; set; }

        /// <summary>
        /// Интервал интерполяции участков
        /// </summary>
        public int InterpolatedInterval { get; set; }

        /// <summary>
        /// Стартовое расстояние
        /// </summary>
        public double StartLength { get; set; }

        /// <summary>
        /// Диаметр трубы, мм
        /// </summary>
        public int Radius { get; set; }

        private bool _useRealCoordinates;
        /// <summary>
        /// Флаг использования реальных координат точек (1 - реальные, 0 - интерполированные)
        /// </summary>
        public bool UseInterpolateCoordinates
        {
            get => _useRealCoordinates;
            set
            {
                _useRealCoordinates = value;
                OnPropertyChanged("UseInterpolateCoordinates");
            }
        }


        private bool _enterStartLength;
        /// <summary>
        /// флаг применения введённого растояния в качестве начального 
        /// </summary>
        public bool EnterStartLength
        {
            get => _enterStartLength;
            set
            {
                _enterStartLength = value;
                OnPropertyChanged("EnterStartLength");
            }
        }

        private string _loadFileMessage;
        /// <summary>
        /// Сообщение о загрузке файла
        /// </summary>
        public string LoadFileMessage
        {
            get => _loadFileMessage;
            set
            {
                _loadFileMessage = value;
                OnPropertyChanged("LoadFileMessage");
            }
        }

        private bool _saveButtonEnableFlag;
        /// <summary>
        /// Флаг доступа к кнопке сохранения 
        /// </summary>
        public bool SaveButtonEnableFlag
        {
            get => _saveButtonEnableFlag;
            set
            {
                _saveButtonEnableFlag = value;
                OnPropertyChanged("SaveButtonEnableFlag");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private ICommand _loadFileCommand;

        public ICommand LoadFileCommand => _loadFileCommand ??
        (_loadFileCommand = new DelegateCommand(x => LoadFile()));

        /// <summary>
        /// Загрузка файла ехcel
        /// </summary>
        public void LoadFile()
        {
            try
            {
                if (_dialogService.OpenExcelDialog() == true)
                {
                    SaveButtonEnableFlag = false;
                    var startLength = EnterStartLength ? Math.Floor(StartLength) : 0;
                    _externalData = ExcelReader.ReadExcelDocument(_dialogService.FilePath, startLength);
                    LoadFileMessage = $"Файл {_dialogService.FileName} загружен";
                    SaveButtonEnableFlag = true;
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessage($"Heвозможно загрузить дайл. {e.Message}");
            }
        }

        private ICommand _saveProfileCommand;

        public ICommand SaveProileCommand => _saveProfileCommand ??
        (_saveProfileCommand = new DelegateCommand(x => SaveProfileFile()));

        /// <summary>
        /// Сохранение файла dxf
        /// <summary>
        public async void SaveProfileFile()
        {
            try
            {
                if (_dialogService.SaveDxfDialog() == true)
                {
                    SaveButtonEnableFlag = false;
                    var data = _externalData;
                    if (UseInterpolateCoordinates)
                    {
                        var interpolatedTable = new List<ExternalData>();
                        interpolatedTable.Add(_externalData.First());
                        var prevPoint = _externalData.First();
                        var pointCounter = 1;
                        for (var i = InterpolatedInterval; i < data.Last().LengthBy2DLine; i += InterpolatedInterval)
                        {
                            var leftPoint = _externalData.Where(x => x.LengthBy2DLine < pointCounter * InterpolatedInterval).Last();
                            var rightPoint = _externalData.Where(x => x.LengthBy2DLine > pointCounter * InterpolatedInterval).First();
                            var interpolatedPoint = ExternalData.InterpolateByInterval(leftPoint, rightPoint, interpolatedTable.Last(), InterpolatedInterval * pointCounter);
                            interpolatedTable.Add(interpolatedPoint);
                            pointCounter++;
                        }
                        data = interpolatedTable;
                    }
                    var startLength = EnterStartLength ? StartLength : 0;
                    var res = await DxfWriter.WriteProfile(
                    _dialogService.FilePath,
                    data,
                    LineLength,
                    HorizontalScale,
                    VerticalScale,
                    ShowBiasToLengthFlag,
                    ShowDepthFlag,
                    ShowTubeMarkFlag,
                    ShowGroundMarkFlag,
                    ShowDistanceFlag,
                    ShowPicketsFlag,
                    ShowLinePlanFlag,
                    ShowKmFlag,
                    Radius,
                    StartLength);
                    if (res)
                    {
                        MessageBox.Show($"Файл сохранен");
                    }
                    SaveButtonEnableFlag = true;
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessage($"Необходимо сохранить файл профиля. {e.Message}");
            }
        }

        private ICommand _savePlaneCommand;

        public ICommand SavePlaneCommand => _savePlaneCommand ??
            (_savePlaneCommand = new DelegateCommand(x => SavePlaneFile()));

        // <summary>
        // Сохранение файла dxf
        // </summary>
        public async void SavePlaneFile()
        {
            try
            {
                if (_dialogService.SaveDxfDialog() == true)
                {
                    SaveButtonEnableFlag = false;
                    var data = _externalData;
                    if (UseInterpolateCoordinates)
                    {
                        var interpolatedTable = new List<ExternalData>();
                        interpolatedTable.Add(_externalData.First());
                        var prevPoint = _externalData.First();
                        var pointCounter = 1;
                        for (var i = InterpolatedInterval; i < data.Last().LengthBy2DLine; i += InterpolatedInterval)
                        {
                            var leftPoint = _externalData.Where(x => x.LengthBy2DLine < pointCounter * InterpolatedInterval).Last();
                            var rightPoint = _externalData.Where(x => x.LengthBy2DLine > pointCounter * InterpolatedInterval).First();
                            var interpolatedPoint = ExternalData.InterpolateByInterval(leftPoint, rightPoint, interpolatedTable.Last(), InterpolatedInterval * pointCounter);
                            interpolatedTable.Add(interpolatedPoint);
                            pointCounter++;
                        }
                        data = interpolatedTable;
                    }
                    var res = await DxfWriter.WritePlane(
                    _dialogService.FilePath,
                    data,
                    LineLength,
                    PlanLength,
                    HorizontalScale,
                    VerticalScale);
                    if (res)
                    {
                        MessageBox.Show($"Файл сохранен");
                    }
                    SaveButtonEnableFlag = true;
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessage($"Невозможно сохранить файл профиля. (e.Message)");
            }
        }

        private ICommand _exportFileCommand;

        public ICommand ExportFileCommand => _exportFileCommand ??
            (_exportFileCommand = new DelegateCommand(x => ExportFile()));

        /// <summary>
        /// Экспорт файла эксель с аппроксимированными точками
        /// </summary>
        public void ExportFile()
        {
            try
            {
                SaveButtonEnableFlag = false;
                var interpolatedTable = new List<ExternalData>();
                if (_dialogService.SaveExcelDialog() == true)
                {
                    var i = 1;
                    var prevPoint = _externalData.First();
                    foreach (var point in _externalData)
                    {
                        if (point.LengthBy2DLine > LineLength * i)
                        {
                            var fakePoint = ExternalData.CreateFakePoint(prevPoint, point, LineLength);
                            interpolatedTable.Add(fakePoint);
                            i++;
                        }
                        interpolatedTable.Add(point);
                        prevPoint = point;
                    }
                    ExcelReader.WriteExcelDocument(_dialogService.FilePath, interpolatedTable);
                    SaveButtonEnableFlag = true;
                    MessageBox.Show("Файл сохранен");
                }
            }
            catch (Exception e)
            {
                _dialogService.ShowMessage($"Невозможно экспортировать файл. (e.Message)");
            }
        }
    }
}
