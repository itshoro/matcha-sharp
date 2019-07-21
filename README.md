# About
Lumione is a static site generator, that focuses on making it easy and fast to reuse snippets, it completely ignores blog utilities and theming to focus on serving the most simple to use interface to deploy.

### Your Style, your CSS
No need to override a default style: It's your CSS. Import frameworks, extend them or just use your own styling from scratch. Lumione is built to not make any assumptions about your stylings.

### Reusable snippets
Be it navigations, footers or simply components. Some elements are just going to show up over and over. Lumione allows you to include them into your file without any hassle.

## Quick Start

Initialize a lumione project, by using `dotnet lumione.dll init`.
An example file structure might look like this:
```
.
+-- deploy
+-- css
|   +-- style.scss
+-- js
|   +-- main.js
+-- assets
|   +-- favicons
|       +-- ...
|   +-- ...
+-- includes
|   +-- nav.html
|   +-- footer.html
|   +-- ...
+-- index.html
+-- 404.html
+-- impressum.html
```

After running `dotnet lumione.dll build` it populates the `deploy` folder, while you can pass arguments to customize how the build should work, the default folder structure would look like this:

```
deploy
+-- assets
|   +-- favicons
|   |   +-- ...
|   +-- ...
+-- js
|   +-- main.js
+-- css
|   +-- style.css
+-- 404
|   +-- index.html
+-- impressum
|   +-- index.html
+-- index.html
```

## Invokers & Processors
Lumione can be extended via plugins, these are seperated into two types: Invokers and Processors. Invokers are called before processors as they are essentially function calls. They are present as a command in your input files and get replaced, with a result string during building.

Processors on the other hand extend the build process itself, think of SASS Compilers, Beautifiers or minifiers. Right now Lumione has no default processor.

## Commands
```
init
    Initializes a project with the default folder structure.

build [-d|-m|-b]
    Builds the project to the default 'deploy' folder.
    
    -d: Pass your own destination path
    -m: Minify all output
    -b: Beautify all output
```