using GalaSoft.MvvmLight;

namespace MvvmLightWpfSample.ViewModel
{
	public class PersonViewModel : ViewModelBase
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