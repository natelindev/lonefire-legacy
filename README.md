![](wwwroot/img/logo.svg)

![](wwwroot/img/brand-bold.svg)



## Introduction

Lonefire is a blogging & content managing system built with Bootstrap & Jquery on ASP.NET Core 2.2.

It includes all the essentials on what you usually expect from a blogging & content managing system.

Designed with vector image and animation, Lonefire aims to provide you with a mordern responsive web experience that can easily interact with other .NET Apps.

[Live Demo](https://lonefire.llldar.io/)

## Getting Started

#### Installation

Install .NET core SDK 2.2 or Visual Studio version >= 2017

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

For database migration, please refer to [this guide](Docs/Db_Create_Readme.md).

Note that this project is currently at Pre Alpha stage. It's not fully functional and is subject to change, use it at your own risk.

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

![](wwwroot/img/MIT-License.png)

MIT License

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the “Software”), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
