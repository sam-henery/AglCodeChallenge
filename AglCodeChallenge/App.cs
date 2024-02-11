using AglCodeChallenge.DTOs;
using AglCodeChallenge.Enums;
using AglCodeChallenge.Options;
using AglCodeChallenge.Services;
using Microsoft.Extensions.Options;

namespace AglCodeChallenge
{
    public class App(IPetService petService, IOptions<SettingsOptions> settingsOptions)
    {
        private readonly SettingsOptions _settingsOptions = settingsOptions.Value;

        public async Task Run(string[] args)
        {
            var petTypeFilter = GetPetType(args) ?? GetPetType([_settingsOptions.DefaultPetTypeFilter]);

            var result = await petService.GetAllByOwnerGender(petTypeFilter);
            
            PrintResults(result);


            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static PetType? GetPetType(string[] args)
        {
            return args.Length > 0 && Enum.TryParse(typeof(PetType), args[0], out var result) ? (PetType?)result : null;            
        }

        private static void PrintResults(IEnumerable<PetOwner> petOwners)
        {
            foreach (var petOwner in petOwners)
            {
                Console.WriteLine($"{Environment.NewLine}{petOwner.Gender}{Environment.NewLine}");

                foreach (var pet in petOwner.Pets)
                {
                    Console.WriteLine($"- {pet.Name}");
                }

                Console.WriteLine($"{Environment.NewLine}");
            }
        }
    }    
}
