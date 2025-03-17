using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BankCoreCRUD.Models
{
    [Table(nameof(Account))]
    public class Account
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AcctID { get; set; }
        [Required, MaxLength(50), Display(Name = "Branch")]
        public string Branch { get; set; } = string.Empty;
        [AllowNull]
        public List<Customer>? Customers { get; set; }
    }
}
