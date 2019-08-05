



<p align="center">
  <img height="300" src="wwwroot/img/logo.svg">
</p>

<p align="center">
  <img src="wwwroot/img/brand-bold.svg">
</p>

## Introduction

Lonefire is a blogging & content managing system built with Bootstrap & JQuery on ASP.NET Core 2.2.

It includes all the essentials on what you usually expect from a blogging & content managing system.

Designed with vector image and animation, Lonefire aims to provide you with a modern responsive web experience that can easily interact with other .NET Apps.

[Blogging Demo](https://blog.llldar.io/)
[CMS Demo](https://wenxue.llldar.io/)

## Getting Started

#### Installation

Install .NET core SDK 2.2 or Visual Studio version >= 2017

Pull source code from Github

#### Database connection

Add  `db_string.json`  with your own database connection string with format like:

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "[Your connection string]"
    }
}

```

you can refer to [connectionstrings.com](connectionstrings.com/) to write your own DB connection string.

by default Lonefire uses `Postgres SQL` you can change it from `Startup.cs`.

```c#
services.AddDbContext<ApplicationDbContext>(options =>                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
```

Change `UseNpgsql` to whatever DB that asp.net core 2.2 supports.

You might need to install nuget package to use other DB.

Also Change default value in`Data/ApplicationDBContext.cs` to your DB's default if you changed the DB.

For database migration, please refer to [this guide](Docs/Db_Create_Readme.md).

**Caveat**: this project is currently at Pre Alpha stage. It's not fully functional and is subject to change, use it at your own risk.

#### Starting Production Build

```
dotnet publish -o /path/to/production
cd /path/to/production
dotnet Lonefire.dll
```



## Contribution

Currently not accepting any contribution at Pre Alpha Stage.

## Credits

The open source projects used in this project:

- Bootstrap
- jquery
- jquery-easing
- jquery-hoverIntent
- jquery-validation

## License

MIT License `llldar`
