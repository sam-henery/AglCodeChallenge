
namespace AglCodeChallenge.DTOs
{
    public class PetOwner
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public IEnumerable<Pet> Pets { get; set; } = Enumerable.Empty<Pet>();
    }
}
