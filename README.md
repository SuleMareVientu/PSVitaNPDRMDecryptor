# PSVita NPDRM Decryptor
<p align="center"> <img src="https://i.imgur.com/piWq0Dj.png"> </p>

## Features
- Straightforward GUI to decrypt NPDRM protected apps/patches/addcont:
    - Allows for easier modding
    - Allows all apps to run on all firmware versions
    - Removes the need of plugins like [NoNpDrm](https://github.com/TheOfficialFloW/NoNpDrm)
    - Allows full control over the content you own

## Software Requirements
- Microsoft Windows 7 or later
- [.NET Framework 4.8.1](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481)
- (Optional) ["TrialPatch" user plugin to bypass game trials](https://github.com/SuleMareVientu/TrialPatch-PSV)
- (Optional) ["rePatch-reLoaded" kernel plugin to install patches & addcont without merging](https://github.com/SonicMastr/rePatch-reLoaded)

## Usage
TODO

## Installing decrypted applications
- App
    - Copy the VPK/folder to the PSVita (NOT inside `ux0:app`)
    - (VPK) Open VitaShell and install the package
    - (Folder) Open VitaShell and press △ on the app folder and choose `More -> Install folder`
    - Accept extended permissions
- Patch
    - Merge with main app or use rePatch
- Additional content
    - Use rePatch

For now app installation must be done through VitaShell because we need to generate a fake head.bin to display the app in the livearea.

## Installing decrypted applications (rePatch)
- App - Install as described before
- Patch/Additional content
    - [rePatch-reDux0 Wiki](https://github.com/dots-tb/rePatch-reDux0/wiki/)

## Input Folder Search Structure

```
├───addcont
│   └───TITLE_ID
│   │   └───DLC_FOLDER
├───patch
│   └───TITLE_ID
├───app (any name or TITLE_ID)
│   └───eboot.bin
│   └───...
```
<p align="center"> <img src="https://i.imgur.com/dvYhB2o.png"> </p>

## Changelog

### v1.0
- Initial release

## TODO
- Make `Refresh livearea` work on decrypted apps
- Add a "Troubleshooting" section

## Credits
- [PSVita DevWiki - Documentation](https://www.psdevwiki.com)
- [Team Molecule - sceutils](https://github.com/TeamMolecule/sceutils)
- [motoharu-gosuto - psvpfstools](https://github.com/motoharu-gosuto/psvpfstools)
- [LoopingAudioConverter - GUI base](https://github.com/libertyernie/LoopingAudioConverter)
- [BetterFolderBrowser - Folder Select Dialog](https://github.com/Willy-Kimura/BetterFolderBrowser)