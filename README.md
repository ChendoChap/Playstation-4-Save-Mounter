# Playstation 4 Save Mounter

## Summary
This program allows you to mount save data as READ/WRITE
### You can
* Make decrypted copies of your saves
* Replace saves with modified ones
* Replace save files with someonelse's save files (share saves)



### You can't
* Replace save files with an encrypted save
* Use this on unexploited consoles

### You need
* To make sure you're using a recent ps4debug version, bin of the latest ps4debug (as of 11/14) is included in the download

## Prerequisites
* PS4 5.05
* FTP Client (eg filezilla, ...)
## Instructions
1) Load [jkpatch](https://github.com/xemio/jkpatch)
2) Start a game
3) Load [FTP](https://github.com/xvortex/ps4-ftp-vtx)
4) Open the tool
5) Enter the ip of your ps4 and click 'Connect'
6) Click 'Refresh Processes' and select your game in the combobox
7) Click 'Setup'
8) Click 'Find Dirs'
9) Select the save you want to mount in the combobox
10) Select the mount permission in the combobox (default is READ ONLY)
11) Click 'Mount'
12) Your save is now mounted and accessible from ftp in /mnt/pfs/ & in /mnt/sandbox/{title}/savedataX (it's the same just a different dir)
13) After you're done copying/replacing files click 'Unmount'

**don't replace files in sce_sys directory, it is unnecessary and will probably corrupt your save**



**Some games use another save format, they have an sce_ prefix in their name (saves can be found in /user/home/{userid}/savedata/{titleid} check the name there). they won't show up as search results**  
**This can probably be patched but I was too lazy** 



Here's a workaround
1) go to /user/home/{userid}/savedata/{titleid}
2) make a copy of the sce save: 2 files, the bin file(96KB), the sdimg file
3) rename them  
	"sce_sdmemory.bin" -> "temp.bin"  
    "sdimg_sce_sdmemory" -> "sdimg_temp"
4) go to /system_data/savedata/{userid}/db/user and download the database.db file
5) open it with an [sqlite editor](https://sqlitebrowser.org/)  
6) add a new record in the savedata table
7) fill in the data and you're done
8) replace the original database with the newer one
9) Click 'find dirs' again, it should now add a temp entry in the combobox
10) proceed as usual
11) go to /user/home/{userid}/savedata/{titleid}
	* delete the original sce_sdmemory.bin and sdimg_sce_sdmemory
	* rename temp.bin to sce_sdmemory.bin and temp to sdimg_sce_sdmemory
12) replace the modified database with the original one
13) you're done

## Authors
- Aida
- [ChendoChap](https://github.com/ChendoChap)
## Acknowledgments
* [golden](https://github.com/xemio)
