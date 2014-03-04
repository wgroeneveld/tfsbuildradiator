
# (Very) simple Team Foundation Server Build radiator

## What's this?

I was looking for a simple REST API to gather build information from TFS. I'm used to working with jenkins, which is very simple. TFS doesn't even have a REST API... It has some basic SOAP capabilities (v2010), yay. Great. So I wrote a tiny C# app which simply dumps all build info into a JSON file. That way I can do with it whatever I want with Javascript. 

![Screenshot](http://i43.tinypic.com/2pzh66h.jpg)

* * *

## Configuration

change your tfsServer URL in TfsJsonOutput/App.config. 
change which builds you wish to display in index.html (var BUILDS) - 

```javascript
 radiator.getBuildStatus([ "CHC.CMR.CentralMailService (auto debug)", "CHP-1.x-DEV-DEBUG" ]);
```

The JS file loops through all found projects and only displays those in the array (we use the same TFS server for multiple teams)

### I don't like the colors

Configurable in radiator.js:

```javascript
	var buildColors = {
		"Succeeded": "lightgreen",
		"PartiallySucceeded": "yellow",
		"Failed": "red"
	};
```

Go nuts.

## Installation

You'll need MS' TFS client .NET DLL v11 installed. Use `gacutil` to deploy the provided dll - see install.bat in the gacutil dir.

You'll also need to have python installed on the server PC.

## Running

Execute TfsJsonOutput.exe - it outputs "buildstatus.json" into the same directory. An example has been checked in. 
Execute server.bat - it starts a simple server to serve index.html @ port 9000 (to be able to use `jQuery.getJSON()` locally)

Done!

### It doesn't work on my machine/browser!

Could be. Chrome does not allow GET requests from another origin so running the index.html file locally without the server will result in this error: "Failed to load resource: No 'Access-Control-Allow-Origin' header is present on the requested resource. Origin 'null' is therefore not allowed access. ". 
There's a config flag in chrome that disables this error, or simply use the python server!
