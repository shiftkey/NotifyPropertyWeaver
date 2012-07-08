using NotifyPropertyWeaver;

namespace GenericChildWithPropertyBeforeAfter
{
    public class ClassWithGenericPropertyChild : ClassWithGenericPropertyParent<string>
    {
        [NotifyProperty]
        public string Property1 { get; set; }
    }
}