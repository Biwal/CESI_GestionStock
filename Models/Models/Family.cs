using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class Family
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            Family family = (Family)obj;
            return family.Id == Id;
        }
    }
}
