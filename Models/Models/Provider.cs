using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class Provider
    {
        public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public string Address { get; set; }

        public string FullName {
            get {
                return this.Firstname + " " + this.Lastname;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            Provider provider = (Provider)obj;
            return provider.Id == Id;
        }
    }
}
