````
dotnet ef migrations add "InitialCreate" --project src/HaefeleSoftware.Api -o "Infrastructure/Migrations"
dotnet ef database update --project src/HaefeleSoftware.Api
dotnet ef migrations remove --project src/HaefeleSoftware.Api
````