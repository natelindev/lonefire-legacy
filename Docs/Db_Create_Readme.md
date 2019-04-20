##### Database realated opearations

In package manager console, type:
`Add-Migration InitialCreate`
to create Migration scripts.

Then, type:
`Update-Database`
to apply the scrpits and make changes to DB

If you want to revert the DB change, type:
`Remove-Migration`


Using .NET CLI then it becomes:
`dotnet ef migrations add InitialCreate`
and 
`dotnet ef database update`
&
`dotnet ef migrations remove`


If you try to get model from Database, then use:
`dotnet ef dbcontext scaffold [Your connection string] Microsoft.EntityFrameworkCore.SqlServer -o Models`

To get the of Old UserInfo working with .NET Identity, use SQL command like:

```mssql
INSERT INTO lonefire.dbo.Users(Id,SecurityStamp,ConcurrencyStamp,Username,NormalizedUserName,PasswordHash, AccessFailedCount, EmailConfirmed, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled,Name) SELECT LOWER(NEWID()), LOWER(NEWID()), LOWER(NEWID()),UserName,UserName,LoginPassWord,0,0,1,0,0,Name FROM [OldDB].dbo.[User];
```
