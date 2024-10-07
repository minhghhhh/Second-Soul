using System.ComponentModel.DataAnnotations;

namespace Second_Soul.ViewModel
{
    public class CouponViewmodel
    {
        public class CouponCreateViewModel
        {
            [Required]
            [MaxLength(50)]
            public string Code { get; set; }

            [Range(0, 100)]
            public decimal DiscountPercentage { get; set; }

            public int MaxDiscount { get; set; }

            public DateTime? ExpiryDate { get; set; }

            public int MinSpendAmount { get; set; }
        }

        public class CouponEditViewModel
        {
            public int Id { get; set; }

            [Required]
            [MaxLength(50)]
            public string Code { get; set; }

            [Range(0, 100)]
            public decimal DiscountPercentage { get; set; }

            public int MaxDiscount { get; set; }

            public DateTime? ExpiryDate { get; set; }

            public int MinSpendAmount { get; set; }

            public bool IsActive { get; set; }
        }

    }
}
