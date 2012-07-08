using System;
using System.Linq;
using System.ComponentModel;
using FluentValidation;
using FluentValidation.Results;

namespace Fluent
{
    public class ModelBase : IDataErrorInfo, INotifyPropertyChanged
    {
        private IValidator validator;
        private ValidationResult validationResult;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelBase()
        {
            validator = ValidationFactory.GetValidator(GetType());
            validationResult = validator.Validate(this);
        }

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
            }
        }

        public string this[string propertyName]
        {
            get
            {
                return string.Join(Environment.NewLine, validationResult.Errors.Where(x=>x.PropertyName == propertyName).Select(x=>x.ErrorMessage));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            validationResult = validator.Validate(this);
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}