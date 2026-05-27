namespace CulinaryCart.Model
{
public class DietaryPreference
    {
        public int DietId { get; set; }
        public string Diet { get; set; }
        public ICollection<Menu> MenuItems { get; set; }
    }
}
