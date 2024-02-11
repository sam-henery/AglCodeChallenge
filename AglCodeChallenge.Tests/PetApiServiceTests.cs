using AglCodeChallenge.ApiServices;
using AglCodeChallenge.DTOs;
using AglCodeChallenge.Options;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Runtime.CompilerServices;

namespace AglCodeChallenge.Tests
{
    public class PetApiServiceTests
    {
        private readonly PetApiService _petApiService;

        public PetApiServiceTests() 
        {
            var endpointsOptionsMock = new Mock<IOptions<EndpointsOptions>>();
            endpointsOptionsMock.Setup(o => o.Value).Returns(new EndpointsOptions
            {
                PetApiUrl = "http://example.com/api/pets"
            });

            var loggerMock = new Mock<ILogger<PetApiService>>();

            _petApiService = new PetApiService(endpointsOptionsMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task TestResponse_Success()
        {                           
            using var httpTest = new HttpTest();
            var expectedPetOwners = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Max", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Daisy", Type = "Cat" } } }
            };

            httpTest.RespondWithJson(expectedPetOwners);
                        
            var result = await _petApiService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(expectedPetOwners.Count, result.Count());
            Assert.Equivalent(expectedPetOwners, result, true);
        }

        [Fact]
        public async Task TestPetApiNotFound_Fail()
        {            
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: (int)HttpStatusCode.NotFound);

            await Assert.ThrowsAsync<FlurlHttpException>(_petApiService.GetAll);
        }

        [Fact]
        public async Task TestPetApiTimeout_Fail()
        {
            var loggerMock = new Mock<ILogger<PetApiService>>();

            using var httpTest = new HttpTest();
            httpTest.SimulateTimeout();

            await Assert.ThrowsAsync<FlurlHttpTimeoutException>(_petApiService.GetAll);
        }

        [Fact]
        public async Task TestMissingPetApiUrl_Fail()
        {
            var loggerMock = new Mock<ILogger<PetApiService>>();

            var endpointsOptionsMock = new Mock<IOptions<EndpointsOptions>>();
            endpointsOptionsMock.Setup(o => o.Value).Returns(new EndpointsOptions { PetApiUrl = "" });

            using var httpTest = new HttpTest();            

            var petApiService = new PetApiService(endpointsOptionsMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(petApiService.GetAll);
        }

        [Fact]
        public async Task TestInvalidJson_Fail()
        {
            using var httpTest = new HttpTest();
            var invalidPayload = new List<string>
            {
                "Cat", "Rocky", "Samwich"                
            };

            httpTest.RespondWithJson(invalidPayload);
           
            await Assert.ThrowsAsync<FlurlParsingException>(_petApiService.GetAll);
        }
                
        [Fact]
        public async Task TestEmptyCollection_Success()
        { 
            using var httpTest = new HttpTest();
            var expectedPetOwners = Enumerable.Empty<PetOwner>();

            httpTest.RespondWithJson(expectedPetOwners);
            
            var result = await _petApiService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(expectedPetOwners.Count(), result.Count());
            Assert.Equivalent(expectedPetOwners, result, true);
        }        
    }
}
