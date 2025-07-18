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

!!! alert-warning "Warning"
    Make sure you make the repository public, the database will need to be able to see it.

### Updating The README

Now head to your GitHub homepage, you should see your new mod in the sidebar on the left, click it.  

Scroll down until you see `README.md` this file is special as it will be displayed on the mod website when people click on your mod.  

Click on it and then select the pencil icon in the top right of the file, this will open the web editor.  

This file format is called [Markdown](https://www.markdownguide.org/){target="_blank"}, it's a very simplistic way to write content for websites. If you don't know markdown you can use [this tutorial](https://www.markdowntutorial.com/){target="_blank"} to help you. Make sure your README gives a good description of your mod!

!!! alert-info "Tip"
    The first image in your README file will be used as the mod's thumbnail on the website, to insert an image, simply paste it inside of the editor. Make sure it's at the very top of your document. **This image should be in a 3:1 aspect ratio**.

## Releasing Your Mod

Now we're ready to release your mod, head over to the actions tab and select "Create Release", and in the top right select "Run Workflow", then simply run the workflow.  

After the workflow finishes, head back to the "Code" tab and select "releases" on the right sidebar, here you should see a new draft release, click on it.

Fill out the description with something like "First Release!", and press publish.  

You've successfully created a release on GitHub!

### 403 Error

If you get a 403 error, head to the repository settings page, and under "Actions -> General" scroll to the bottom under "Workflow permissions" and click "Read and Write Permissions", then click save. This will enable GitHub actions to create releases on your repo.

## Adding Your Mod To The Database

Now all that's left is to add your mod to the database, you can do so by following [this form](https://github.com/ow-mods/ow-mod-db/issues/new?assignees=&labels=add-mod&template=add-mod.yml&title=%5BYour+mod+name+here%5D){target="_blank"}. An admin will review your mod, make sure everything is setup correctly, and approve it into the database.  

Congratulations on publishing your first Outer Wilds mod!

## Updating Your Mod

To update your mod follow these steps

1. Bump the version in your `manifest.json`
2. Push your changes with GitHub desktop
3. Run the "Create Release" action again
4. Select releases on the right
5. Edit the description to whatever you want
6. Press "publish release"

!!! alert-info "Info"
    If the release action detects that the version in manifest.json already has a release for it, it will refuse to run.
