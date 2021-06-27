# Null checker
**version: 1.1.2**   
**Unity version : 2020.3+**

## Description
_Null checker_ allows you to see the potential null references in your editor and fix them.   

Every _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_ reference will trigger the _null checker_ and highlight it to tell the user the reference is missing or exists.   

The null reference fix is performed via a simple _[GetComponent()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html)_ on the object the script you are trying to fix is attached on.   

## Settings

You will find by default a '_Assets/Nullchecker/Settings/NullCheckerSettings.asset_' in your project settings. This setting file is automatically generated the first time you will open the _null checker_ settings in your project settings.   

### Settings description

- **Line pixel size**: Hight of a single line to display the error message.  
- **Line pixel spacing**: Space in pixel between two line of error message.  
- **Valid color**: Color your _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_ reference will be highlighted with if a reference exists.  
- **Error color**: Color your _[Object](https://docs.unity3d.com/ScriptReference/Object.html)_ reference will be highlighted with if a reference is missing.  
- **Base assembly**: Assembly that tolerate classes without namespace.   
- **Default warning**: Default message displayed next to a missing reference.   
- **Setting path override**: Allow you to specify a new path for the setting of the _null checker_ if you want to.   

## Known issues
- _[Image](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Image.html)_ type is not recognized as a fixable refence at the moment.   
- The _Setting path override_ will be overriden at Unity start up if it has been modified.