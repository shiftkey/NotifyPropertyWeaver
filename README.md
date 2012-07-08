Injects [INotifyPropertyChanged](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx) code into properties at compile time.

 * No install required
 * No attributes required
 * No references required
 * No base class required
 * Supports .net 3.5, .net 4, .net 4.5, Silverlight 3, Silverlight 4, Silverlight 5 and Windows Phone 7
 * Supports client profile mode 

Feel free to ping me on twitter http://twitter.com/SimonCropp

### Setup can be simplified by installing the [Visual Studio package](http://visualstudiogallery.msdn.microsoft.com/bd351303-db8c-4771-9b22-5e51524fccd3) 

### Your Code
    
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", GivenNames, FamilyName);
            }
        }

    }

### What gets compiled

    public class Person : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        string givenNames;
        public string GivenNames
        {
            get { return givenNames; }
            set
            {
                if (value != givenNames)
                {
                    givenNames = value;
                    OnPropertyChanged("GivenNames");
                    OnPropertyChanged("FullName");
                }
            }
        }

        string familyName;
        public string FamilyName
        {
            get { return familyName; }
            set 
            {
                if (value != familyName)
                {
                    familyName = value;
                    OnPropertyChanged("FamilyName");
                    OnPropertyChanged("FullName");
                }
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", GivenNames, FamilyName);
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

## WIKI

[See Wiki for more doco](https://github.com/SimonCropp/NotifyPropertyWeaver/wiki/)