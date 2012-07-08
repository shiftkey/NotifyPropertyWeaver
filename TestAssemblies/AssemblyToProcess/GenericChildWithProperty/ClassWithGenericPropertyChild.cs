using NotifyPropertyWeaver;

namespace GenericChildWithProperty
{
    public class ClassWithGenericPropertyChild : ClassWithGenericPropertyParent<string>
    {
        [NotifyProperty]
        public string Property1 { get; set; }
    }
}