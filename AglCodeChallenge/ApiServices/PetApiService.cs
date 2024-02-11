using AglCodeChallenge.DTOs;
using Microsoft.Extensions.Options;
using Flurl.Http;
using AglCodeChallenge.Options;
using Microsoft.Extensions.Logging;


namespace AglCodeChallenge.ApiServices
{
    public class PetApiService(IOptions<EndpointsOptions> endpointsOptions, ILogger<PetApiService> logger) : IPetApiService
    {
        private readonly EndpointsOptions _endpointsOptions = endpointsOptions.Value;

        public async Task<IEnumerable<PetOwner>> GetAll()
        {
            try
            {
                return await _endpointsOptions.PetApiUrl.GetJsonAsync<IEnumerable<PetOwner>>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving pet data from Pet Api: {PetApiUrl}", _endpointsOptions.PetApiUrl);
                throw;
            }
        }
    }
}
