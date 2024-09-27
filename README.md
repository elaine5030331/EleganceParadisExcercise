# EleganceParadis 
## About this project
- Language : C#
- Version : .Net 8
- Framework : Asp .Net Core WebAPI


## Tech
- Project architecture
	- Clean Architecture
	- Repository pattern
	- Unit of work pattern
- Data access
	- EFCore
	- Dapper
- Authentication
	- JWT
	- Roles-based authorization 
- Payment Gateway 
	- LinePay
- Job scheduling  
	- Coravel
- Media management 
	- Cloudinary
- SMTP service
	- MailKit 

## Database settings
Use EFCore CLI :
### Scaffold
```bash
dotnet ef dbcontext scaffold "Server=.;Database=EleganceParadis;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context EleganceParadisContext --context-dir Data --context-namespace Infrastructure.Data --output-dir ..\ApplicationCore\Entities --namespace ApplicationCore.Entities --force --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\
```

### Migrations
```bash
 dotnet ef migrations add init --output-dir Data\Migrations --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\

 dotnet ef database update --project .\Infrastructure\ --startup-project .\EleganceParadisAPI\
```