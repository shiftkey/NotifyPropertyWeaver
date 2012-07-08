using Caliburn.Micro;

namespace CaliburnMicroWPFSample.ViewModels
{

	public class PersonViewModel : PropertyChangedBase
	{
	    public string GivenNames;
	    public string FamilyName;

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", GivenNames, FamilyName);
            }
        }

	}
}
