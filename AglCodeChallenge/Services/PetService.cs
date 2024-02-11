using AglCodeChallenge.ApiServices;
using AglCodeChallenge.DTOs;
using AglCodeChallenge.Enums;
using Microsoft.Extensions.Logging;

namespace AglCodeChallenge.Services
{
    public class PetService(IPetApiService petApiService, ILogger<PetService> logger) : IPetService
    {
        public async Task<IEnumerable<PetOwner>> GetAllByOwnerGender(PetType? petTypeFilter = null)
        {            
            var petData = await petApiService.GetAll();

            try
            {
                if (petData != null)
                {
                    return petData.Where(x => x.Pets != null && !string.IsNullOrWhiteSpace(x.Gender)).GroupBy(owner => owner.Gender)
                        .Select(group => new PetOwner
                        {
                            Gender = group.Key,
                            Pets = group.SelectMany(po => po.Pets.Where(pet => !petTypeFilter.HasValue || pet.Type.Equals(petTypeFilter.ToString())).Distinct()).OrderBy(x => x.Name)
                        });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error found grouping and filtering {petData} on {petTypeFilter}", petData, petTypeFilter);
            }

            return new List<PetOwner>();
        }
    }
}
