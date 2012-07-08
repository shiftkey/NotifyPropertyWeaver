using System.ComponentModel.DataAnnotations;

namespace DataAnnotations
{
    public class Person : ModelBase
    {
        [StringLength(15)] 
        public string GivenNames;

        [StringLength(15)] 
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
