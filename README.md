<div align="center">
  <img alt=".NET logo" title=".NET logo" src="https://github.com/marcel0024/ExampleIOGame/blob/main/src/client/src/assets/dotnetlogo.png" width="120">
  <img alt="Angular logo" title="Angular logo" src="https://github.com/marcel0024/ExampleIOGame/blob/main/src/client/src/assets/angularlogo.png"width="120">
  <h1>An Example .io Game</h1>
</div>

<h4 align="center">
  <a href="https://example-io-game.azurewebsites.net/">https://example-io-game.azurewebsites.net/</a>
</h4>

<p align="center">
  <a href="https://github.com/Marcel0024/ExampleIOGame/actions/workflows/example-io-game.yaml">
  <img src="https://github.com/Marcel0024/ExampleIOGame/actions/workflows/example-io-game.yaml/badge.svg" alt="Build Status"></img></a>
</p>

This repository contains a multiplayer (.io) web game, developed using .NET 8 and Angular 17.

The original game was developed by [Victor Zhou](https://github.com/vzhou842/example-.io-game), using Javascript and Node.js. In this version, we've transitioned to C# for server-side logic and Typescript for the client side.

One of the significant changes in this version is the implementation of the Composite Design Pattern. Instead of using separate arrays for each game object type, we've simplified the structure to use a single, unified array.

## Running it locally

To get started with the client, make sure you have Node and NPM & Angular installed. Then,

```bash
$ cd src/client
$ npm install
$ npm start
```

Then to get the .NET app started, make sure you have .NET sdk installed.

```bash
$ cd src/server
$ dotnet run
```

Now you can navigate to `http://localhost:4200/`
