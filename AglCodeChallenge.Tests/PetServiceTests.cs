using AglCodeChallenge.ApiServices;
using AglCodeChallenge.DTOs;
using AglCodeChallenge.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AglCodeChallenge.Tests
{
    public class PetServiceTests
    {
        private readonly Mock<ILogger<PetService>> _loggerMock;

        public PetServiceTests()
        {
            _loggerMock = new Mock<ILogger<PetService>>();
        }

        [Fact]
        public async Task TestEmptyDataSetFromApi_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = Enumerable.Empty<PetOwner>();            

            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender();

            Assert.NotNull(result);
            Assert.Empty(result);           
        }

        [Fact]
        public async Task TestNullPetsCollection_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();
            
            var petData = new List<PetOwner>
            {
                new() { Gender = "Male" },
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Scratch", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Doogie", Type = "Cat" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Scratch", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Doogie", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestPetNameMissing_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Max", Type = "Cat" } } },                
                new() { Gender = "Female", Pets = new List<Pet> { new() { Type = "Cat" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Max", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestPetTypeMissing_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Max", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Daisy", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Randy" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender(Enums.PetType.Cat);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Max", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Daisy", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestPetOwnerGenderMissing_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = new List<PetOwner>
            {
                new() { Pets = new List<Pet> { new() { Name = "Max", Type = "Cat" } } },
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Minny", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Daisy", Type = "Cat" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Minny", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Daisy", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestFilteringOnCats_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Max", Type = "Dog" } } },
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Minny", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Daisy", Type = "Cat" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);
                       
            var result = await petService.GetAllByOwnerGender(Enums.PetType.Cat);
                        
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Minny", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Daisy", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestFilteringOnDogs_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();

            var petData = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Tiny", Type = "Dog" } } },
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Minny", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Rosie", Type = "Dog" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender(Enums.PetType.Dog);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Tiny", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Rosie", femaleOwner.Pets.First().Name);
        }

        [Fact]
        public async Task TestFilteringOnFish_Success()
        {
            var petApiServiceMock = new Mock<IPetApiService>();            

            var petData = new List<PetOwner>
            {
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Jeb", Type = "Fish" } } },
                new() { Gender = "Male", Pets = new List<Pet> { new() { Name = "Minny", Type = "Cat" } } },
                new() { Gender = "Female", Pets = new List<Pet> { new() { Name = "Donny", Type = "Fish" } } }
            };
            petApiServiceMock.Setup(service => service.GetAll()).ReturnsAsync(petData);

            var petService = new PetService(petApiServiceMock.Object, _loggerMock.Object);

            var result = await petService.GetAllByOwnerGender(Enums.PetType.Fish);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var maleOwner = result.First(o => o.Gender == "Male");
            Assert.Single(maleOwner.Pets);
            Assert.Equal("Jeb", maleOwner.Pets.First().Name);

            var femaleOwner = result.First(o => o.Gender == "Female");
            Assert.Single(femaleOwner.Pets);
            Assert.Equal("Donny", femaleOwner.Pets.First().Name);
        }
    }
}