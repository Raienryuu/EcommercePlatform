rm -r Data/Migrations

dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
