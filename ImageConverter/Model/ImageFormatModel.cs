using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.Model
{
  public  class ImageFormatModel: INotifyPropertyChanged
    {
      private bool _isSelected;
      public bool IsSelected 
      { 
          get { return _isSelected; }
          set
          {
              if (_isSelected == value) return;
             
              _isSelected = value;
              OnPropertyChanged("IsSelected");
              if (value)
              {
                 OnSelected();
              }
          }
      }
        public string Extension { get; set; }

        public event EventHandler SelectionChangedEvent;

      private void OnSelected()
        {
          if (SelectionChangedEvent!=null)
          {
              SelectionChangedEvent(this, new EventArgs());
          }
        }

      private void OnPropertyChanged(string propertyName)
      {
          if (PropertyChanged != null)
              PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
      public event PropertyChangedEventHandler PropertyChanged;
    }
}
