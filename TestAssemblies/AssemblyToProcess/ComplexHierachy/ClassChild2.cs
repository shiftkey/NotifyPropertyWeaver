using NotifyPropertyWeaver;

namespace ComplexHierachy
{
    public class ClassChild2 : ClassParent
	{
		[NotifyProperty]
		public string Property1 { get; set; }
	}
}