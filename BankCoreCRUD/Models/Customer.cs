using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankCoreCRUD.Models
{
    [Table(nameof(Customer))]
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustID { get; set; }
        [Required, MaxLength(50), Display(Name = "Customer Name")]
        public string CustName { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "DATE"), Display(Name = "DOB")]
        public DateTime DOB { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Required, Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required, DataType(DataType.ImageUrl), Display(Name = "Photo")]
        public string ImageUrl { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccID { get; set; }

        [ForeignKey(nameof(Transaction))]
        public int TranID { get; set; }

        public Transaction? Transaction { get; set; }
        public Account? Account { get; set; }
    }
}
