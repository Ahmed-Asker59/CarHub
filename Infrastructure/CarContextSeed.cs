using Core.Entities;
using Infrastructure.Data;
using System.Text.Json;


namespace Infrastructure
{
    public class CarContextSeed
    {
        public static async Task SeedAsync(CarContext context)
        {
            if (!context.Brands.Any()) 
            {
                var brandsDate = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");

                var brands = JsonSerializer.Deserialize<List<Brand>>(brandsDate);

                context.Brands.AddRange(brands);

                await context.SaveChangesAsync();


            }

            if (!context.Makes.Any())
            {
                var makesDate = File.ReadAllText("../Infrastructure/Data/SeedData/makes.json");

                var makes = JsonSerializer.Deserialize<List<Make>>(makesDate);

                context.Makes.AddRange(makes);

                await context.SaveChangesAsync();

            }

            if (!context.Models.Any())
            {
                var modelsDate = File.ReadAllText("../Infrastructure/Data/SeedData/models.json");

                var models = JsonSerializer.Deserialize<List<Model>>(modelsDate);

                context.Models.AddRange(models);

                await context.SaveChangesAsync();

            }

            if (!context.Cars.Any())
            {
                var carsDate = File.ReadAllText("../Infrastructure/Data/SeedData/cars.json");
                
                var cars = JsonSerializer.Deserialize<List<Car>>(carsDate);
 
                context.Cars.AddRange(cars);

                await context.SaveChangesAsync();

            }

            



        }
    }
}
