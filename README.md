
# (Very) simple Team Foundation Server Build radiator

## What's this?

I was looking for a simple REST API to gather build information from TFS. I'm used to working with jenkins, which is very simple. TFS doesn't even have a REST API... It has some basic SOAP capabilities (v2010), yay. Great. So I wrote a tiny C# app which simply dumps all build info into a JSON file. That way I can do with it whatever I want with Javascript. 

![Screenshot](http://i43.tinypic.com/2pzh66h.jpg)

* * *

## Configuration

change your tfsServer URL in TfsJsonOutput/App.config. 
change which builds you wish to display in index.html (var BUILDS) - 

```javascript
 var BUILDS = [ "CHC.CMR.CentralMailService (auto debug)", "CHP-1.x-DEV-DEBUG" ];
```

The JS file loops through all found projects and only displays those in the array (we use the same TFS server for multiple teams)