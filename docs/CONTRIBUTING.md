# Contributing

This file will go over how to get a local build of the docs working

## Requirements

First and foremost, install [Python 3.10](), this guide will assume the python version on your PATH is 3.10, to check this run `python --version`.

## Installing Pipenv

Next up we need to install pipenv, this is an advanced package manager for python that provides some much needed improvements to pip.

```sh
python -m pip install pipenv --user
```

This will install pipenv to your user, meaning you'll be able to use it anywhere.

## Generating The Site

Now, head to the `docs` folder within OWML and run `pipenv sync --dev`, this will install all required packages.

Then, run `pipenv shell` to launch a sub-shell environment.

Finally, run `menagerie generate`, this will output the site to a folder called `out`.

## Viewing The Site

Now, head into the `out` folder and run `python -m http.server 8080`, this will start a web server in the out folder.

Finally, connect to `localhost:8080` with your browser, the website should be displayed.

After editing the site, you'll want to regenerate, I recommend you have two terminals open, one to generate, and the other to host the HTTP server.

## Getting Schemas

Schemas will not be in the site by default, to get them copy the schema in `Schemas` of the OWML folder and put it in `docs/content/pages/schemas` (you need to create the schemas folder). Do not commit this folder to git, as we only want to keep one copy of the schema in source control at once.

