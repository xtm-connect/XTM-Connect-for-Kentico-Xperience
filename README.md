# XTM-Connect-for-Kentico-Xperience

This repository contains the source code of the XTM Connect - Kentico Xperience. The main purpose of the connector is to seamlessly connect Xperience by Kentico to the industry-leading translation management system XTM Cloud.

Version supported: v13

## Basic Requirements

An XTM account with API access enabled.

XTM login details provided either by your company administrator or by XTM International Ltd.

Don't have your credentials yet?

Sign up [here](https://xtm.cloud/trial/) for the free XTM Cloud free trial account.

Contact [XTM Sales team](mail:sales@xtm.cloud) to get access.

## NuGet package

You can download XTM Connect - Kentico Xperience here:

[Xtm-Connect 3.1.0](https://linktodocumentation)
## Documentation

You can access XTM Connect - Kentico Xperience documentation here:

[Documentation](https://github.com/xtm-connect/XTM-Connect-for-Kentico-Xperience/blob/main/xtm-connect-for-kentico.pdf)


## Contributing

Feel free to submit a pull request here.

How to get started:

    1. Download XTM Connect - Kentico Xperience nuget package and code from github repository.
    2. Add NuGet package to your project.
    3. Copy downloaded folders into your Kentico instance directory.
    4. Add XTM-Connector project to your solution (path: Xtm.Connector//Xtm.Connector.csproj)
    5. Add XTM-Connect project to your solution (path: CMS//Xtm.Connect.csproj)
    6. Clear references to Xtm.Connect and Xtm.Connector from CMSApp.
    7. Add references to Xtm.Connect project and Xtm.Connector project to CMSApp.
