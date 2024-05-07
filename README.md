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


An example multiplayer (.io) web game created in .NET 8 and Angular 17.

The original was created by [Victor Zhou](https://github.com/vzhou842/example-.io-game) in Javascript and Node.js. 
It's been rewritten for .NET and Angular.

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
