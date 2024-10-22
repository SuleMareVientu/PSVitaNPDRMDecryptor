# PSVita NPDRM Decryptor [![GitHub all releases](https://img.shields.io/github/downloads/SuleMareVientu/PSVitaNPDRMDecryptor/total?label=Total%20Downloads&logo=GitHub)](https://github.com/SuleMareVientu/PSVitaNPDRMDecryptor/releases/latest)
<p align="center"> <img src="https://i.imgur.com/uqz3MBD.png" alt="Program Screenshot" style="height: 75%; width: 75%; object-fit: contain"> </p>

## Features
- Straightforward GUI to decrypt NPDRM protected apps/patches/addcont:
    - Easier modding
    - Allows all apps to run on all firmware versions
    - Removes the need of plugins like [NoNpDrm](https://github.com/TheOfficialFloW/NoNpDrm)
    - Full control over the content you own

## Software Requirements
- Microsoft Windows 7 or later (x64)
- [.NET Framework 4.8.1](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net481)
- (Optional) ["TrialPatch" user plugin to bypass game trials](https://github.com/SuleMareVientu/TrialPatch-PSV/releases/latest) (e.g. Rayman Legends, FEZ)
- (Optional) ["rePatch-reLoaded" kernel plugin to install patches & addcont without merging](https://github.com/SonicMastr/rePatch-reLoaded/releases/latest)

## Usage
If you have a PKG file and not a NPDRM app in folder format you'll first need to extract the PKG with ["pkg2zip.exe"](https://github.com/lusid1/pkg2zip/releases/latest) or similar by providing the correct zRIF.

 - Use the `+` button or drag & drop encrypted folders, select the output directory and click the blue arrow
 - Adjust the options in the checkboxes to your liking
 - Remove highlighted folders with the `X` button

Follow the [Input Folder Search Structure](#Input-Folder-Search-Structure) section below to setup your input folders correctly with patches and addcont.

#### The program will search for patches/addcont only when decrypting the main app with the specified folder setup.

Patches and additional content can still be decrypted separately.

## Installing decrypted applications
- App (**WITHOUT `head.bin`**)
    - Copy the app's VPK/folder to the PSVita (NOT inside `ux0:app`)
    - (**VPK**) Open VitaShell and install the package
    - (**Folder**) Open VitaShell and press △ on the app folder and choose `More -> Install folder`
    - Accept extended permissions
- App (**WITH `head.bin`**)
    - Copy the app folder to `ux0:app/...` (where you would normally place NPDRM content)
    - Return to the  home screen in VitaShell (where it shows `os0:`, `ur0:`, `ux0:` etc.), press △ and choose `Refresh LiveArea™`
- Patch
    - Merge with main app or use rePatch
- Additional content
    - Use rePatch

For now app installation must be done through VitaShell because we need to generate a fake `head.bin` to display the app in the livearea.

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
<p align="center"> <img src="https://i.imgur.com/dvYhB2o.png" alt="Input Folder Search Structure Image" style="height: 100%; width: 100%; object-fit: contain"> </p>

## Troubleshooting
- "My app says it's a trial"
    - [Install the "TrialPatch" plugin.](https://github.com/SuleMareVientu/TrialPatch-PSV/releases/latest)
- "DLCs don't work"
    - [Install the "rePatch-reLoaded" plugin.](https://github.com/SonicMastr/rePatch-reLoaded/releases/latest)
- "I can't install the decrypted app, VitaShell throws and error"
    - Delete the NPDRM version. If that doesn't work, manually delete the folders with the TitleID of the game in `ux0:app`, `ux0:patch`, `ux0:addcont`, `ux0:appmeta` and reboot.  
    Also check if you have the `head.bin` file inside `app\sce_sys\package` and follow the [instructions decribed above](#Installing-decrypted-applications).
- "The program doesn't recognize a patch folder"
    - If you are trying to decrypt a single patch without the main app in the structure described above, you'll need to copy the original `work.bin` of the main app (`app\sce_sys\package\work.bin`) and place it inside the patch folder: `patchTitleID\sce_sys\package\work.bin`. Additional contents have their own `work.bin`, so you don't need the original one for DLC decryption.
- "Some apps decrypt incorrectly / don't function properly"
    - Open an issue and I'll look into it, some apps are known to cause issues.

## Changelog

### v1.0.1
- Improved UI
- Added options to output rePatch folders, copy original `head.bin` to the decrypted app and show terminal during decryption.
- Miscellaneous improvements

### v1.0
- Initial release

## TODO
- ~~Add proper support for `gdc` apps (e.g. Reader™ [PCSC80012])~~ See: [GBAtemp](https://gbatemp.net/threads/e-book-reader-pcsc80012-english-translation-how-to-add-custom-books.541424/)

## Credits
- [PSVita DevWiki - Documentation](https://www.psdevwiki.com)
- [Team Molecule - sceutils](https://github.com/TeamMolecule/sceutils)
- [motoharu-gosuto - psvpfstools](https://github.com/motoharu-gosuto/psvpfstools)
- [LoopingAudioConverter - GUI base](https://github.com/libertyernie/LoopingAudioConverter)
- [BetterFolderBrowser - Folder Select Dialog](https://github.com/Willy-Kimura/BetterFolderBrowser)
- ​[botik - Guidance & Help](https://gbatemp.net/members/botik.433115/)
