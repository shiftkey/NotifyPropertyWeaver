using NotifyPropertyWeaver;

namespace ComplexHierachy
{
    public class ClassChild3 : ClassChild2 
    {
        [NotifyProperty]
        public string Property2 { get; set; }
    }
}