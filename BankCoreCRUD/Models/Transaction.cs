using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankCoreCRUD.Models
{
    [Table(nameof(Transaction))]
    public class Transaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TranID { get; set; }
        [Required, MaxLength(50),Display(Name ="Account Type")]
        public string AccType { get; set; }
        public IList<Customer>? Customers { get; set; }
    }
}
