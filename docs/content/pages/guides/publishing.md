---
Title: Publishing Your Mod
Sort_Priority: 30
---

# Publishing Your Mod

This guide will give a quick overview of how to publish your mod to [the database](https://github.com/ow-mods/ow-mod-db){target="_blank"}.

## Setting Up A GitHub Repo

To get started, you'll want to [create a GitHub account](https://github.com/signup){target="_blank"} and grab [GitHub Desktop](https://desktop.github.com/){target="_blank"}.

Once opened, you'll want to select File -> Add Local Repository.  Then navigate to your mod's **source code folder** (NOT the folder your mod builds to), if you're using Visual Studio, this will most likely be located in `C:\Users\YourName\source\repos\YourModName\`.

Once selected, it should alert you that there isn't a repo there, just click the small link that says "create a repository here".

Fill the form it gives you out with info about your mod, the description field is what will be displayed in the mod manager.  

Check the box that says "Initialize this repository with a README", we'll edit this in a bit.

When it asks for the Git Ignore, select "Visual Studio". And for license, select "MIT".

Now, with our repository created, select "Publish Repository" in the top right, this will push your repo to GitHub.

### Updating The README

Now head to your GitHub homepage, you should see your new mod in the sidebar on the left, click it.  

Scroll down until you see `README.md` this file is special is it will displayed on the mod website when people click on your mod.  

Click on it and then select the pencil icon in the top right of the file, this will open the web editor.  

This file format is called [Markdown](https://www.markdownguide.org/){target="_blank"}, it's a very simplistic way to write content for websites. If you don't know markdown you can use [this tutorial](https://www.markdowntutorial.com/){target="_blank"} to help you. Make sure your README gives a good description of your mod!

!!! alert-info "Tip"
    The first image in your README file will be used as the mod's thumbnail on the website, to insert an image, simply paste it inside of the editor. Make sure it's at the very top of your document.

## Zipping Your Mod

To zip your mod, start by building it.

Then, open the mod manager, scroll to your mod and click the ⋮ symbol to the right of it. From this menu, select "Show In Explorer".  

Now select the dll file, `manifest.json`, and `default-config.json`. You can select you mod's other assets as well, however, it's important you don't include `config.json`.

Now, create a zip archive of these files, if you don't have a program to do that we recommend [7Zip](https://www.7-zip.org/){target="_blank"}.

## Releasing Your Mod

Now that we have a zip file of your mod, head to the GitHub repository and select the "Releases" section on the right sidebar. Then, in the top-right, select "Draft A New Release".  

In the top dropdown that says "Choose a tag", type `v0.1.0`, or whatever version is states in your `manifest.json`.

!!! alert-warning "Warning"
    The version in the manifest file must match the tag on GitHub (minus the `v` part, that part is okay). It's very important to remember to update your manifest version before creating a release.

Fill this form out with a title like "Version 0.1.0" and a description like "This is my first mod release!".

Finally, in the box that says "Attach binaries...", upload the zip file you created of your mod before.

Now simply click "Publish release"!

## Adding Your Mod To The Database

Now all that's left is to add your mod to the database, you can do so by following [this form](https://github.com/ow-mods/ow-mod-db/issues/new?assignees=&labels=add-mod&template=add-mod.yml&title=%5BYour+mod+name+here%5D){target="_blank"}. An admin will review your mod, make sure everything is setup correctly, and approve it into the database.  

Congratulations on publishing your first Outer Wilds mod!

## Updating Your Mod

To update your mod follow these steps

1. Bump the version in your `manifest.json`
2. Push your changes with GitHub desktop
3. Build the mod
4. Follow the steps to zip the mod again
5. Create a new release on GitHub, **making sure the tag matches the version in the manifest**

### Automation

If you've created your project using the nuget package, you can simply navigate to the "Actions" tab on your repository, select "Create Release" and then run it.  

This will create a draft release for you with the tag, title, and zip already setup, and will alert you if the version in the manifest hasn't been bumped.
