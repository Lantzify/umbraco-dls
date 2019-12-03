# DLS - Dynamic loginscreen

This is package dynamicly updates umbracos loginscreen background.

## Setup
After installing the package, add the follwing keys to web.config.

```
    <add key="DLSRoot" value="NodeID" />
    <add key="DLSLogo" value="PropertyAlias" />
    <add key="DLSBackground" value="PropertyAlias" />
```
## How it works
The "DLSRoot" is the only key that is required. The value should be the Node id of the page where the logo and/or background is located.
To speficy the logo and background you can use the "DLSLogo" and "DLSBackground" keys. The value should be the property alias of the picker. <br />

Now everytime the "DLSRoot" node is saved, the package will update the logo and background.

## Datatypes
The background picker works with the following datatypes:
- Media picker
- Multiple media picker
  - Takes first image. 
- Nested content
  - The package loops all sections in the nested content and returns the first found image. 

The logo picker work with:
* Media picker

## Web login
Username: admin@admin.com <br />
Password: 123456789

## Improvment
Feel free to reach out if you have feedback, ideas or feel like I could improve the code! I am allways looking for ways to improve!