# Playstation 4 Save Mounter 1.5

## Summary
This program allows you to mount save data with RW permission and a lot more shit, just read this damn thing
### You can
* Make decrypted copies of any save (as long as it's encrypted with keys <= 5.05)
* Replace saves with modified ones
* Replace save files with someone else's save files (share saves)
* Create new saves
* Export saves to 5.05+ consoles

### You can't
* Replace save files with an encrypted save (if it's encrypted with keys > 5.05)
* Use this on unexploited consoles

### You need
* To make sure you're using a recent ps4debug version, bin of the latest ps4debug (as of 11/14) is included in the download
* To be able to run .net framework 2.0 executables (even windows 98 can run this)
## Prerequisites
* PS4 5.05
* FTP Client (eg filezilla, ...)
## Instructions

### Mounting saves
1) Load [ps4debug](https://github.com/xemio/ps4debug)
2) Load [FTP](https://github.com/xvortex/ps4-ftp-vtx)
3) Open the tool
4) Enter the ip of your ps4 and click 'Connect'
5) Click 'Setup' & select the user you want to use in the combobox
6) Click 'Get Games' & select the game you want to use in the combobox
7) Click 'Search' & select the save you want to mount
8) Click 'Mount'
9) Your save is now mounted and accessible from ftp in /mnt/pfs/ & in /mnt/sandbox/NPXS20001_000/savedataX (it's the same just a different dir)
10) After you're done copying/replacing files click 'Unmount'
### Creating saves
1) Load [ps4debug](https://github.com/xemio/ps4debug)
2) Load [FTP](https://github.com/xvortex/ps4-ftp-vtx)
3) Open the tool
4) Enter the ip of your ps4 and click 'Connect'
5) Click 'Setup' & select the user you want to use in the combobox
6) Click 'Get Games' & select the game you want to use in the combobox
7) Enter the desired save directory name in the textbox
8) Use the slider to choose the save's size
9) Click 'Create Save'
10) Click 'Search' to refresh the save list
### Exporting Saves to 5.05+ consoles
1)  mount the save you want to export.
2)  get the param.sfo file from the sce_sys directory
3)  open it in a hex editor or a ps4 compatible sfo editor
4)  change the psn id to the target's psn id (8 bytes, you get that by copying a save using settings, you'll need to change it to little endian) it's at 0x15C for hex editing... see video for sfo editor
5)  save the param.sfo & replace the one in the mounted dir
6)  unmount the save and copy the 2 save files sdimg & the .bin to your usb /PS4/SAVEDATA/{psn id}/{titleid}/
7)  remove the sdimg_ prefix from the filename
8)  now you should be able to copy the save to the account linked to the psn id (5.05+ console) using system settings
## Important

**- you don't need to start a game to modify its saves, it's actually better not to have one open because some games like gow 4 may overwrite parts of a save while you're busy modifying it resulting in a corrupted save.**

**- don't replace files in sce_sys directory, it is unnecessary and will probably corrupt your save**  

**- the workaround method is obsolete since update 1.4**  

**- some games require you to create your own save data with the appropriate name & size, fallout 4 is such a game. This problem was discussed in [issue #5](https://github.com/ChendoChap/Playstation-4-Save-Mounter/issues/5)**

**- Don't forget to regularly make backups of your saves and the savedata database, the ps4 deletes all your saves if the database gets corrupted while this mostly only happens when you actively mess with it, it's always better to be prepared**

**- It's possible to mount someone else's encrypted saves but there's currently no 'clean' way to do it. you need to temporarily replace the sdimg_xxx and the xxx.bin files with the ones you downloaded in your user's savedata directory. Be sure to restore the original files after you extracted the save because the ps4 could throw a fit if you reboot while those files are still there. (I'll make this process easier later on)**

## Resources
### Do note that not all of these were made using the latest save mounter version so slight differences are to be expected.

### Videos
  * exporting saves to 5.05+ consoles (latest version 1.5): [how to resign PS4 saves for different PS4 and profile, fw 5 05 and higher by 'Old Gamer'](https://www.youtube.com/watch?v=OpZ9C-MciZM)

  * mounting saves, transferring saves to other (regions/title ids) (old version): [PS4 Save Mounter Tutorial (Swap Saves Between Consoles & Games) by 'MODDED WARFARE'](https://www.youtube.com/watch?v=m_h4MsAaXdY)

  * mounting saves, using other people's decrypted saves (old version): [Playstation 4 Save Mounter Demonstration by 'Sc0rpion'](https://www.youtube.com/watch?v=Atw6480SX5I)
