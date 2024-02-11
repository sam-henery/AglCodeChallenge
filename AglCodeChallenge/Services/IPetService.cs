using AglCodeChallenge.DTOs;
using AglCodeChallenge.Enums;

namespace AglCodeChallenge.Services
{
    public interface IPetService
    {
        Task<IEnumerable<PetOwner>> GetAllByOwnerGender(PetType? petTypeFilter = null);
    }
}
