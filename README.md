# Null checker
**version: 1.4.0**   
**Unity version : 2020.3+**

## Description
_Null checker_ allows you to see the potential null references in your editor and fix them if possible.   

Any _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_, class, struct or even asset reference will trigger the _null checker_ and highlight it to tell the user the reference is either missing or valid.   

The null reference fixes _Object_ reference via simple _[GetComponent()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html)_ on the object the script you are trying to fix is attached on.

## Settings

You will find by default a '_Assets/Nullchecker/Settings/NullCheckerSettings.asset_' in your project settings. This setting file is automatically generated the first time you will open the _null checker_ settings in your project settings.   

### Settings description

- **Line size**: Hight of a single line to display the error message.  
- **Line spacing**: Space in pixel between two line of error message.  
- **Valid color**: Color your _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_ reference will be highlighted with if a reference exists.  
- **Error color**: Color your _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_ reference will be highlighted with if a reference is missing.
- **Default warning**: Default message displayed next to a missing reference. If the string is empty, no message will appear and the line will pop  
- **Setting path override**: Allow you to specify a new path for the setting of the _null checker_ if you want to.   

## Known issues 
- Some errors may pops when you remove an element from a serialized collection 
