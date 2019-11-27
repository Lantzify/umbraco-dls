# DLS - Dynamic loginscreen

This is package updates umbracos loginscreen background.

## Setup
After installing the package, add the follwing keys to web.config.

```
    <add key="DLSRoot" value="NodeID" />
    <add key="DLSLogo" value="DocumentTypeAlias" />
    <add key="DLSBackground" value="DocumentTypeAlias" />
```
## How it works
The "DLSRoot" is the only key that is required. The value should be the Document Type Alias of the page where the logo and background is located.
To speficy the logo and background you can use the "DLSLogo" and "DLSBackground" keys. Now everytime the "DLSRoot" is saved the package will update the logo and background.

## Works with
This is package  works with the following datatypes:
* Media picker
* Multiple media picker

## Web login
Username: admin@admin.com 
Password: 123456789

## To-Do

* Add support for Neasted content