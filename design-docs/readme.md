
# The NETWeasel Design Specification

## Purpose

**This document is in a draft state and is subject to change.**

This document seeks to establish and describe a top-down view of the NETWeasel project, its underlying technologies, and methodologies for performing its designed tasks.

## Project Goals

The project shall perform the following tasks:

- Generate EXE installer executables that self-extract and copy package contents to the designated installation directory.
- Generate packages to be used for applying updates.
- Provide a programmable interface for applications to self-apply updates.

The project shall make the following assurances:

- The installation on the target machine will always be in a usable state after the completion of the initial installation, such that an interrupted update will not prevent the application from launching or re-attempting a failed update.

_**Discuss:** Are there other project goals that should be enumerated here?_

## Building the Installer and Updater

### Specification

Instead of using a NuGet .nuspec file to describe the product, the developer will write and maintain an XML file that follows a schema that is specific to NETWeasel.

_**Discuss:** What are the advantages or disadvantages involving the use of .nuspec and NuGet for the use of packaging update and diff payloads?_

The specification shall allow for multiple build targets to be described. For instance, an application that is AOT-compiled may describe an x86 and an ARM target.

### 

### Installer Executable

The installer executable is self-extracting. An executable stub shall be provided by the project, and shall be pre-compiled natively for the 32-bit x86 architecture. During the release phase, the installer payload shall be appended to the PE resource section of the stub. The installer payload is identical to the update package described below.

_**Discuss:** There is an expectation that 32-bit x86 binaries will be executable on both x86-64 and ARM architectures via an emulation layer. Is this a reasonable expectation? Are there any advantages or disadvantages to providing a pre-compiled stub for non-x86 architectures?_

_**Discuss:** For multi-targeted architectures, how should the installer binary determine which target to apply?_

### Update Package

The update package shall be a compressed archive of files. The archive shall contain one directory for each build target described by the specification.

_**Discuss:** Other than a ZIP archive using the standard DEFLATE algorithm, what other algorithms or archive container formats are suitable, and what are their advantages and disadvantages? Consider how this will apply to the self-extracting behavior of the installation binary._

### Delta Package

To minimize the aggregate number of bytes required to be fetched to bring an old application version up to the current version, a variation of the update package, called a delta package, shall be generated in which it shall contain binary diffs for each individual file in the target installation directory.

For instance, if an installation is 3 versions behind, and if the total number of bytes between the 3 new diff packages is less than the number of bytes of the latest full update package, then the 3 diffs are fetched and applied consecutively. Otherwise, the latest update package is fetched and applied instead.

### Release Descriptor File

The project shall produce a release descriptor file which describes for each update the following items:
- Update package
    - file name
    - version
    - file size in bytes
    - SHA-256 hash
- Delta package
    - file name
    - version
    - file size in bytes
    - SHA-256 hash

_**Discuss:** In what format should the release file be? How should changes to the file schema be handled?_

## Applying Updates

### Launching the Application

Shortcuts placed on the desktop, Start menu, taskbar, or any other location shall point to a launcher executable that exists at the root of the installation directory. The launcher shall be responsible for examining the current release descriptor file, the presence of any unapplied or incompletely applied update packages or delta packages, determining if the files are intact by verifying their associated SHA-256 hashes against those in the release descriptor, and then applying unapplied packages.

After applying the update packages, if any, the launcher will invoke the application in the latest version directory.

_**Discuss:** It is likely not desirable to have shortcuts in the Windows taskbar, Start menu, and the desktop that have the startup directory set to a specific version folder. In this configuration, the updater will have to search for and patch each shortcut when an update is applied. Should the launcher executable simply ignore this directive and always launch the application with the current application version's directory set as its initial working directory?_

_**Discuss:** Squirrel.Windows supports the concepts of Squirrel-aware and Squirrel-unaware packages. Should the launcher make an effort to determine which executables in the current version directory are aware or unaware of NETWeasel? If so, how?_

_**Discuss:** In Squirrel.Windows, Squirrel-aware executables are invoked with predefined command-line arguments. For NETWeasel-aware executables, should we allow the developer to provide custom argument names? If so, how?_
