# XTM-Connect-for-Kentico-Xperience

This repository contains the source code of the XTM International connector for 
Kentico Xperience. XTM-Connect main purpose is management of translations by direct 
integration with XTM Cloud.

Version supported: v13

## NuGet

You can download Xtm-Connect for Kentico Xperience here:

[Xtm-Connect 3.1.0](https://linktodocumentation)
## Documentation

Here's documentation of installation and of using our Connector

[Documentation](https://github.com/xtm-connect/XTM-Connect-for-Kentico-Xperience/blob/main/xtm-connect-for-kentico.pdf)


## Contributing

Feel free to submit pull requst.

How to get started:

        1. Download Kentico-Connect nuget package and code from github repository.
        2. Add NuGet package to your project.
        3. Copy downloaded folders into your kentico instance folders.
        4. Add Xtm-Connector project to your solution (path: Xtm.Connector//Xtm.Connector.csproj)
        5. Add Xtm-Connect project to your solution (path: CMS//Xtm.Connect.csproj)
        5. Clear references to Xtm.Connect and Xtm.Connector from CMSApp.
        6. Add references to Xtm.Connect project and Xtm.Connector project to CMSApp.
