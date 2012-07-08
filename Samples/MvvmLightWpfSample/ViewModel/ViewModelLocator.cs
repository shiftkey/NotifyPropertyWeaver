
namespace MvvmLightWpfSample.ViewModel
{
	public class ViewModelLocator
	{

		public ViewModelLocator()
		{
			PersonViewModel = new PersonViewModel();
		}


		public PersonViewModel PersonViewModel { get; set; }
	}
}

