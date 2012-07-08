namespace Fluent
{
    public class Person : ModelBase
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
