using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.Model;
using athena.Command;
using System.Windows.Input;
using System.Windows.Forms;
using System.Xml;
using ImageConverter.Helper;
using System.Configuration;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;
namespace ImageConverter.ViewModel
{
    public class ImageViewModel : BaseViewModel
    {
      private ImageFormatModel[] _imageFormats;
      private string _sourcePath;
      private string _targetPath;
      private bool _isSingleFileToConvert;
      private bool _isFilesInFolderToConvert;
      private bool _reduceImage = true;
      private int _reducePercent;
      private int _reduceHeightPixel;
      private int _reduceWidthPixel;
      private bool _scaleByPercent = false;
      private ICommand _browseSourceFileCommand;
      private ICommand _browseTargetFileCommand;
      private ICommand _convertCommand;
      private ICommand _cancelCommand;
      private ImageFormatModel _selectedImageFormat;
      private bool _isBusy = false;
      private BackgroundWorker _backgroundWorker;
      private float _convertPercentCompletion;
      public ImageViewModel()
      {
          GenerateImageFormatList();
          IsSingleFileToConvert = true;
          CancelLabel = "Exit";
          if (_backgroundWorker == null)
          {
              _backgroundWorker = new BackgroundWorker();
              _backgroundWorker.WorkerReportsProgress = true;
              _backgroundWorker.WorkerSupportsCancellation = true;
              _backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
              _backgroundWorker.DoWork += _backgroundWorker_DoWork;
              _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
          }
      }



      public string SourcePath
      {
          get
          {
              return _sourcePath;
          }
          set
          {
              _sourcePath = value;
              OnPropertyChanged("SourcePath");
              IsFilesInFolderToConvert = Directory.Exists(SourcePath);
          }
      }

      public string TargetPath
      {
          get
          {
              return _targetPath;
          }
          set
          {
              _targetPath = value;
              OnPropertyChanged("TargetPath");
          }
      }

        public bool IsBusy
      {
          get { return _isBusy; }
          set
          {
              if (_isBusy != value)
              {
                  _isBusy = value;
                  OnPropertyChanged("IsBusy");
                  CancelLabel = _isBusy ? "Cancel" : "Exit";
                  OnPropertyChanged("CancelLabel");
              }
          }
      }

      public bool IsSingleFileToConvert 
      {
          get 
          { 
              return _isSingleFileToConvert; 
          } 
          set
          {
              if (_isSingleFileToConvert == value) return;
              _isSingleFileToConvert = value;
              OnPropertyChanged("IsSingleFileToConvert");
              IsFilesInFolderToConvert = !_isSingleFileToConvert;
          }
      }
      public bool IsFilesInFolderToConvert
      {
          get
          {
              return _isFilesInFolderToConvert;
          }
          set
          {
              if (_isFilesInFolderToConvert == value) return;
              _isFilesInFolderToConvert = value;
              OnPropertyChanged("IsFilesInFolderToConvert");
              IsSingleFileToConvert = !_isFilesInFolderToConvert;
          }
      }

      public int ReducePercent
      {
          get
          {
              return _reducePercent;
          }
          set
          {
              if (_reducePercent == value) return;
              _reducePercent = value;
              OnPropertyChanged("ReducePercent");
              Properties.Settings.Default.reduceImageByPercent = _reducePercent;
              Properties.Settings.Default.Save();
          }
      }

      public int ReduceHeightPixel
      {
          get
          {
              return _reduceHeightPixel;
          }
          set
          {
              if (_reduceHeightPixel == value) return;
              _reduceHeightPixel = value;
              OnPropertyChanged("ReduceHeightPixel");
              Properties.Settings.Default.reduceImageByScale = new System.Drawing.Size(_reduceWidthPixel, _reduceHeightPixel);
              Properties.Settings.Default.Save();
          }
      }

      public int ReduceWidthPixel
      {
          get
          {
              return _reduceWidthPixel;
          }
          set
          {
              if (_reduceWidthPixel == value) return;
              _reduceWidthPixel = value;
              OnPropertyChanged("ReduceWidthPixel");
              Properties.Settings.Default.reduceImageByScale = new System.Drawing.Size(_reduceWidthPixel, _reduceHeightPixel);
              Properties.Settings.Default.Save();
          }
      }

      public ImageFormatModel SelectedImageFormat 
      {
          get { return _selectedImageFormat; }
          set
          {
              if (value.IsSelected)
              {
                  _selectedImageFormat = value;
                  OnPropertyChanged("SelectedImageFormat");
                  for (var i = 0; i < _imageFormats.Length; i++)
                  {
                      if (_imageFormats[i] != _selectedImageFormat)
                          _imageFormats[i].IsSelected = !_selectedImageFormat.IsSelected;
                  }
              }
          }
      }

      public bool ReduceImage
      {
          get
          {
              return _reduceImage;
          }
          set
          {
              if (_reduceImage != value)
              {
                  _reduceImage = value;
                  OnPropertyChanged("ReduceImage");
                  OnPropertyChanged("ConvertFormat");
              }
          }
      }

      public bool ConvertFormat
      {
          get
          {
              return !_reduceImage;
          }
          set
          {
              if (ConvertFormat != value)
                  ReduceImage = !value;
          }
      }

      public bool ScaleByPercent
      {
          get
          {
              return _scaleByPercent;
          }
          set
          {
              if (_scaleByPercent!=value)
              {
                  _scaleByPercent = value;
                  OnPropertyChanged("ScaleByPercent");
                  OnPropertyChanged("ScaleBySize");
                
              }
          }
      }
      
      public bool ScaleBySize
      {
          get
          {
              return !_scaleByPercent;
          }
            set
          {
              if (ScaleBySize != value)
                  ScaleByPercent = !value;
          }
      }

      public string CancelLabel { get; set; }

      public ICommand BrowseSourceFileCommand
      {
          get { return _browseSourceFileCommand ?? (_browseSourceFileCommand = new RelayCommand(a => BrowseSourceFile(), p => CanBrowseFile(p))); }
      }

      public ICommand BrowseTargetFileCommand
      {
          get { return _browseTargetFileCommand ?? (_browseTargetFileCommand = new RelayCommand(a => BrowseTargetFile(), p => CanBrowseFile(p))); }
      }

      public ICommand ConvertCommand
      {
          get { return _convertCommand ?? (_convertCommand = new RelayCommand(a => Convert(), p => { return !_isBusy; })); }
      }

      public ICommand CancelCommand
      {
          get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(a => Cancel())); }
      }
      public ImageFormatModel[] ImageFormats { get { return _imageFormats; } }

      public float ConvertPercentCompletion
      {
          get { return _convertPercentCompletion; }
          set
          {
              _convertPercentCompletion = value;
              OnPropertyChanged("ConvertPercentCompletion");
          }
      }

        #region Private Methods
        private void BrowseSourceFile()
        {
            if (IsSingleFileToConvert)
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    SourcePath = fileDialog.FileName;
                    if (string.IsNullOrEmpty(_targetPath))
                        TargetPath = fileDialog.FileName;
                }
            }
            else
            {
                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    SourcePath = folderDialog.SelectedPath;
                    if (string.IsNullOrEmpty(_targetPath))
                        TargetPath = folderDialog.SelectedPath;
                }
            }
        }
        
        private bool CanBrowseFile(object param)
        {
            return !_isBusy;
        }

        private void BrowseTargetFile()
        {
            if (IsSingleFileToConvert)
            {
                var fileDialog = new OpenFileDialog();

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                        TargetPath = fileDialog.FileName;
                }
            }
            else
            {
                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                  TargetPath = folderDialog.SelectedPath;
                }
            }
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Convert();
        }

        void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ConvertPercentCompletion = (float)(e.ProgressPercentage/100);
        }

        private void Convert()
        {
            IsBusy = true;
            //Task.Run(() =>
            //{
                var imageProcessor = new Helper.ImageProcessorHelper();
                if (IsSingleFileToConvert)
                {

                    if (ReduceImage)
                    {
                        if (ScaleByPercent)
                            imageProcessor.ReduceImageByPercent(SourcePath, TargetPath, _reducePercent);
                        else
                            imageProcessor.ReduceImageByScale(SourcePath, TargetPath, ReduceWidthPixel, ReduceHeightPixel);
                    }
                    else
                        imageProcessor.ConvertImageFormat(SourcePath, ImageFormats.Where(f => f.IsSelected == true).SingleOrDefault().Extension);
                }
                else
                { 
                    var files = new System.IO.DirectoryInfo(SourcePath).GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                                .Where(f => _imageFormats.Where(p=> p.Extension.ToLower() == f.Extension.ToLower()).Any()).ToArray();
                    //ConvertPercentCompletion = files.Count;
                    if (!Directory.Exists(TargetPath))
                        Directory.CreateDirectory(TargetPath);

                    foreach (var imgFile in files)
                    {
                        if (ReduceImage)
                        {
                            if (ScaleByPercent)
                                imageProcessor.ReduceImageByPercent(imgFile.FullName, Path.Combine(TargetPath, imgFile.Name), _reducePercent);
                            else
                                imageProcessor.ReduceImageByScale(imgFile.FullName, Path.Combine(TargetPath, imgFile.Name), ReduceWidthPixel, ReduceHeightPixel);
                        }
                        else
                            imageProcessor.ConvertImageFormat(imgFile.FullName, ImageFormats.Where(f => f.IsSelected == true).SingleOrDefault().Extension);
                    }
                }

                //Finished task.
                //IsBusy = false;
            //});
        }

        private void Cancel()
        {
            if (CancelLabel == "Exit")
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }

            _backgroundWorker.CancelAsync();
        }

        //private bool CanConvert(object o)
        //{
        //    return (!string.IsNullOrEmpty(SourcePath)) || (!_isBusy);
        //}

        private void GenerateImageFormatList()
        {
            var imageFormatsString = Properties.Settings.Default.imageFormats; 
            var lastSelectedImageFormat = Properties.Settings.Default.lastSelectedFormat;

            try
            {
                _reducePercent = Properties.Settings.Default.reduceImageByPercent;
                _reduceHeightPixel = Properties.Settings.Default.reduceImageByScale.Height;
                _reduceWidthPixel = Properties.Settings.Default.reduceImageByScale.Width;
            }
            catch (Exception) { }

            if (string.IsNullOrEmpty(imageFormatsString))
            {
                imageFormatsString = ".ico,.jpg,.png,.gif";
                SaveConfigurationSettings("imageFormats", imageFormatsString);
            }

             _imageFormats = new ImageFormatModel[]{};

             _imageFormats = imageFormatsString.Split(',').Select(i => (new ImageFormatModel { Extension = i })).ToArray();

             for (var i = 0; i < _imageFormats.Length; i++)
                 _imageFormats[i].SelectionChangedEvent += ImageViewModel_SelectionChangedEvent;
            if (!string.IsNullOrEmpty(lastSelectedImageFormat))
                for (var i = 0; i < _imageFormats.Length; i++)
                {
                    if (_imageFormats[i].Extension == lastSelectedImageFormat)
                        _imageFormats[i].IsSelected = true;
                }

        }

        void ImageViewModel_SelectionChangedEvent(object sender, EventArgs e)
        {
            var currentFormat = (ImageFormatModel)sender;
            if (currentFormat != null)
                for (var i = 0; i < _imageFormats.Length; i++)
                    if (_imageFormats[i] != currentFormat)
                        _imageFormats[i].IsSelected = false;
        }


        private void SaveConfigurationSettings(string key, string newValue)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = newValue;
            config.Save(ConfigurationSaveMode.Modified);
        }
        #endregion
    }
}