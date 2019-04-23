# NETWeasel

A Windows application installer and updater framework. Heavily inspired by [Squirrel.Windows](https://github.com/Squirrel/Squirrel.Windows), which has since been deprecated and driven by the shortcomings of ClickOnce.

### We're in development

This project is in the development phase and is having prototypes produced - no part of this project should be used for a production environment, *yet*. That being said, we'd love for additional input on how to design this. We're open to discussing any part of this project. Any part of this project, API or aspect could drastically change over versions. Please do not rely on anything substantial before we believe we're at a point where this can be considered *released*.

### Feature set

The feature set describes what we need to achieve for us to consider a first release.

We'll update this feature set periodically, and it is therefore subject to change at any moment. We're not defining any concrete dates, as this project is being driven by two full time employed software developers who are building NETWeasel in their spare time.

- NETWeasel.Packager
	- A application enabling developers to package any application into an `.msi` and `.exe` installer/setup file and archive for update purposes.
- NETWeasel.Updater (.NET Framework variant)
	-  A Nuget package to enable updates when an application runs. Much like `Squirrel.Windows`.

## NETWeasel.Windows

A .NET Core console executable to package applications to `.msi`, `.exe` and archive to feed the install and update process for `NETWeasel.Updater`.

### How to build
Clone the repo
```git
git clone https://github.com/NETWeasel/NETWeasel.Windows
```

Download the [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download) and if you're using Visual Studio, you'll need Visual Studio 2019 for the latest .NET Core support.

At this point, `NETWeasel.Packager` is ready to run, but it requires arguments to be fed to run the relevant package process. You can achieve this in Visual Studio by opening the debug properties in the project properties and adding `Application arguments`.

`NETWeasel.Packager` has several arguments it accepts at this point in time:

- `path` is required and specifies the directory of the artifacts folder of your application, typically the `debug` or `release` folder in the `bin` of your project you're intending to release.
- `output` is required and specifies the target directory for where NETWeasel will place the packaged artifacts (msi/exe/archives)
- `spec` is required and specifies the path to the NETWeasel XML specification file that describes the application you're packaging, read more about the specification below. You can get an example specification file from the `template` folder in the cloned repository.
- `nologo` prevents the ASCII NETWeasel being printed. This is has no effect on the building process.
- `nocleanup` prevents NETWeasel from deleting files it produces during the build process. Useful for when you want to diagnose issues or are developing NETWeasel itself.

Example application arguments:
```
-spec "C:\Path\To\spec.xml" -path "C:\Path\To\Bin\Folder" -output "C:\NETWeasel"
```

Start debugging in your IDE, preferably Visual Studio 2019.

### Dependencies

NETWeasel uses [WIX Toolset](http://wixtoolset.org/) to produce .msi and .exe files. The binaries are bundled in the repository and don't require you downloading anything. However, it may be of benefit to understand WIX Toolset prior to working on NETWeasel packaging.

In future major updates, we may be looking at moving away from the WIX dependency.

### What it does

The entry point for `NETWeasel.Packager` is in `Program.cs` which instantiates a new `Weasel` - this is the runner responsible for orchestrating WIX dependencies.

`Weasel` will attempt to create the desired output directory and begin to run WIX tools to discover, compile and output the relevant artifacts required to install the application. `Weasel` requires a specification to describe the application you're packaging to WIX. This comes in XML form:

```xml
<NETWeasel version="1">
    <Specification>
      <ProductName>
        My Application
      </ProductName>

      <ProductVersion>
        1.0.0.0
      </ProductVersion>

      <ProductManufacturer>
        My Application Inc.
      </ProductManufacturer>

      <ProductUpgradeGuid>
        <!-- TODO Generate a new GUID and place here -->
        <!-- Ensure this GUID never changes. Check it into source control. -->
      </ProductUpgradeGuid>
    </Specification>
</NETWeasel>
```

This specification will feed WIX when compiling the relevant installers.

`Weasel` will attempt to deserialize the specification and start running `Heat.exe`, which looks at the `path` argument, where the application's artifacts should be. `Heat.exe` will recursively discover all files in this directory and produce `SourceFiles.wxs`, a WIX file to describe which files to include in the compile process.

`Candle.exe` is subsequently ran, which produces `Product.wxs`, the definition of the WIX installed application.

Both `wxs` files are fed into `Light.exe`, which links and compiles to an `msi`. The `msi` can then be used to install the application on a target machine.
