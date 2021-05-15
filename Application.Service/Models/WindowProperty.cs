using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppService.Models
{
    public class WindowProperty : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _property;

        public WindowProperty()
        {
            _property = "Empty";
        }

        public string this[string columnName]
        {
            get {
                
                    if (columnName == "Property"
                        && !(this.Property.StartsWith("http://") || this.Property.StartsWith("https://")))
                        return "Введіть коректне посилання!";
               
                return "";
            }
        }

        public string Property
        {
            get { return _property; }
            set { _property = value;
                OnPropertyChanged("Property");
            }
        }

        public string Error  {
            get {
                if (!(this.Property.StartsWith("http://") || this.Property.StartsWith("https://")))
                    return "Введіть коректне посилання!";

                return "";
            }
            }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop) 
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


    }
}
