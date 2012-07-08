using NotifyPropertyWeaver;

namespace HierachyBeforeAfterAndSimple
{
    public class ClassChild : ClassBase
    {

        [NotifyProperty]
        public string Property1 { get; set; }

        [DoNotNotify]
        public bool BeforeAfterCalled { get; set; }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            BeforeAfterCalled = true;
            OnPropertyChanged(propertyName);
        }

    }
}