using System.ComponentModel.DataAnnotations;

namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class OrderHistory
    {
        [Key]
        public int HistoryID { get; set; }
        
        public int FoodItemID { get; set; }
        public string FoodItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; }   
        public DateTime OrderDate { get; set; }
    }
}
