# Disclaimer
This is my first ever release of a project, so there might be some bugs. I've tested every sorting type there currently is and they all worked without issues, so even if you encounter problems, having your folders sorted incorrectly shouldn't be one of said problems.

Please do not attempt to sort root directories of Windows (System32, Windows, Program Files, etc.) using this tool, as it could end up breaking your Windows installation. It shouldn't be able to sort folders that are read-only and folders that require high-level privileges like the System32 folder, not as far as my tests have proven anyway, but for your own sake I wouldn't recommend testing that theory yourself, just in case.

# Current Features
## Sorting Types
- [x] Sorting by file types
- [x] Sorting by file size
- [x] Sorting alphabetically
- [ ] Sorting by matching file names
## Sorting Parameters
- [x] Toggle including sub-folders in sorting (Only applied to folders in root directory, not folders in folders, will change in the future)
- [x] Toggle capital first letters for new folders
- [x] Toggle excluding zip files from sorting
## Other Quality of Life Features
- [ ] Undo sorting button
- [ ] Preview planned sorting
- [x] Preview chosen folder
