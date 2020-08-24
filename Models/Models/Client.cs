using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public List<ClientOrder> ClientOrders { get; set; }
        
        public Client()
        {
            CreatedAt = DateTime.Now;
        }

        public string FullName
        {
            get
            {
                return Firstname + " " + Lastname;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is Client)
            {
                Client client = (Client)obj;
                return client.Id == Id;
            }

            return false;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
