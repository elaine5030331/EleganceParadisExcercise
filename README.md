## Scaffold
`dotnet ef dbcontext scaffold "Server=.;Database=EleganceParadis;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context EleganceParadisContext --context-dir Data --context-namespace Infrastructure.Data --output-dir ..\ApplicationCore\Entities --namespace ApplicationCore.Entities --force --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\`

## add migrations
`
 dotnet ef migrations add init --output-dir Data\Migrations --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\
`

## database update
`dotnet ef database update --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\`