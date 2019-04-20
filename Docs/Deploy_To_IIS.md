##### Deploying this Application



In order to deploy this application, you can use `build->publish` in visual studio then choose the proper one for you.

Here we only showcase how to publish it on IIS for testing:



First we install or enable IIS in control panel in Windows.

Make sure to download and install .NET Core runtime at [this page](<https://dotnet.microsoft.com/download/dotnet-core/2.2>)



Then choose `publish to folder` option in your Visual Studio

After publishing complete you should add it in your IIS server

with option on `Application Pool` with `No Hosting code` (beacause the .NET Core module handles this)

binding your web to a certain port and you are good to go.



In case of `Error 500.19` you should modify your web folder so that everyone have access to it.

