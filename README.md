# Fiddler ATIS Inspector

## Installation

1. Create the folder `Inspectors` under `Documents/Fiddler2`
2. Copy `FiddlerATIS.dll` and `Newtonsoft.Json.dll` into the previously created folder.


## Ignoring requests

1. On Fiddler go to `Rules -> Customize Rules` (or open the file `Documents/Fiddler/Scripts/CustomRules.js`)
2. Add the following code to the method `OnBeforeRequest`
```cs
if (oSession.oRequest.headers.Exists("X-Fiddler-Ignore")) 
{
    oSession["ui-hide"] = "true";
}
```
