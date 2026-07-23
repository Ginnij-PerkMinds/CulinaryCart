namespace CulinaryCart.CulinaryCartBAL.Models.DTO
{
    public class DietUpdateRequest
    {
        public int Id { get; set; }
        public string Diet { get; set; }
    }

    public class DietaryPreferenceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
