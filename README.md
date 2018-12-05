# Quartic DataCopy

This is a Windows Console app that copies files from one directory to another. It has following features

- copy all files from a source dir to dest dir
- copy all files matching a pattern
- Optional cleanup of files once they are copied

This can be used to copy files on a local shared network.

# To Run

```
PS C:\Users\Dell\workspace\Quartic DataCopy\Quartic DataCopy\bin\Debug> & '.\Quartic DataCopy.exe' --help
Quartic DataCopy 1.0.0.0
Copyright �  2018

  -v, --verbose          (Default: false) Set output to verbose messages.

  -s, --source           Required. Source directory.

  -d, --dest             Required. Destination Directory.

  -p, --pattern          (Default: *) Regex pattern to filter files in source directory.

  -r, --remove-source    (Default: false) If provided, will delete the source files once they're copied.

  -l, --logdir           Specify the directory for logging. If not specified, no logs will be written.

  --help                 Display this help screen.

  --version              Display version information.

```


## Future work
- copy all files created after a certain timestamp
- copy all files created before a certain timestamp