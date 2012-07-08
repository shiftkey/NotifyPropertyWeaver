using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAnnotations
{
    public class ModelBase : IDataErrorInfo, INotifyPropertyChanged
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, dictionary.Values);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                string value;
                dictionary.TryGetValue(propertyName, out value);
                return value;
            }
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            var context = new ValidationContext(this, null, null)
                              {
                                  MemberName = propertyName
                              };
            var results = new List<ValidationResult>();
            if (Validator.TryValidateProperty(after, context, results))
            {
                dictionary.Remove(propertyName);
            }
            else
            {
                dictionary[propertyName] = string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage));
            }

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}