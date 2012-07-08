using NotifyPropertyWeaver;

namespace ComplexHierachy
{
    public class ClassChild1 : ClassParent
    {
        [NotifyProperty]
        public string Property1 { get; set; }
    }
}