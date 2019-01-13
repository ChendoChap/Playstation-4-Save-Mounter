# Playstation 4 Save Mounter 1.4

## Summary
This program allows you to mount save data as READ/WRITE
### You can
* Make decrypted copies of your saves
* Replace saves with modified ones
* Replace save files with someonelse's save files (share saves)
* Create new saves


### You can't
* Replace save files with an encrypted save
* Use this on unexploited consoles

### You need
* To make sure you're using a recent ps4debug version, bin of the latest ps4debug (as of 11/14) is included in the download
* To be able to run .net framework 2.0 executables (even windows 98 is able to do this)
## Prerequisites
* PS4 5.05
* FTP Client (eg filezilla, ...)
## Instructions (mounting existing saves)
1) Load [ps4debug](https://github.com/xemio/ps4debug)
2) Start a game
3) Load [FTP](https://github.com/xvortex/ps4-ftp-vtx)
4) Open the tool
5) Enter the ip of your ps4 and click 'Connect'
6) Click 'Get Processes' and select your game in the combobox
7) Click 'Setup'
8) Click 'Search'
9) Select the save you want to mount in the combobox
10) Click 'Mount'
11) Your save is now mounted and accessible from ftp in /mnt/pfs/ & in /mnt/sandbox/{title}/savedataX (it's the same just a different dir)
12) After you're done copying/replacing files click 'Unmount'

**don't replace files in sce_sys directory, it is unnecessary and will probably corrupt your save**


## Authors
- Aida
- [ChendoChap](https://github.com/ChendoChap)
## Acknowledgments
* [golden](https://github.com/xemio)
