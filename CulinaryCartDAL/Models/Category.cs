namespace CulinaryCart.CulinaryCartDAL.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Menu> MenuItems { get; set; }
    }
}
