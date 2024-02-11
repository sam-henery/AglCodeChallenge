using AglCodeChallenge.DTOs;

namespace AglCodeChallenge.ApiServices
{
    public interface IPetApiService
    {
        public Task<IEnumerable<PetOwner>> GetAll();
    }
}
