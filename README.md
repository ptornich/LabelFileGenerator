# Label File Generator for MS Dynamics 365 for Operations
Generate label files for Microsoft Dynamics 365 for Operations core models in specific languages. After generating the files, you will be able to search for labels in other languages.

You are more than welcome to contribute!

# Download
You can download the latest release here: https://github.com/ptornich/LabelFileGenerator/releases/download/v2.0/LabelFileGenerator.exe

After downloading you will find a self contained executable that can be run from any folder.

# Instructions
1) Run the executable
2) Enter the desired language *(or add the language to the file name)*
3) Wait a few seconds
4) Done!

# Running from the command line
When running from the command line the following arguments are accepted:

```
-l or --lang
The target language in which the label files should be generated. For example: pt-BR.

-f or --folder
The AOSService folder path. For example: K:\AosService\. (if not specified, first found will be used)

-v or --verbose
Display the created file names during the process.

--help
Display the help screen

--version
Display version information
```

For example:
```
LabelFileGenerator.exe -l pt-BR -v
```

The command line above will generate label files in brazilian portuguese with the verbose flag on.

# Adding the language to the file name
With the new added feature, now it's possible to add the target language directly to the file name.

For example:
```
LabelFileGenerator_pt-BR.exe
```

Double clicking this file would directly start the label generation for brazilian portugue labels.

# Screen shots

### Running the executable directly
![](https://github.com/ptornich/LabelFileGenerator/blob/master/Screenshots/Screen%20Shot%2001.png)

![](https://github.com/ptornich/LabelFileGenerator/blob/master/Screenshots/Screen%20Shot%2002.png)


### Running the executable directly with the target language on the file name
![](https://github.com/ptornich/LabelFileGenerator/blob/master/Screenshots/Screen%20Shot%2003.png)
