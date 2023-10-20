using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.ViewModel
{
  public  class BaseViewModel: INotifyPropertyChanged
  {
      public void OnPropertyChanged(string propertyName)
      {
          if (PropertyChanged!=null)
              PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
