import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { BehaviorSubject, Subject } from 'rxjs';
import { ClientCalls } from 'src/app/constants/ClientCalls';
import { ServerCalls } from 'src/app/constants/ServerCalls';
import { GameScreenStatus } from 'src/app/enums/GameScreenStatus';
import { environment } from 'src/environments/environment';
import { GameUpdate } from 'src/app/models/GameUpdate';

@Injectable({
  providedIn: 'root',
})
export class GameHubService {
  private hubConnection: HubConnection = new HubConnectionBuilder()
    .withUrl(environment.gameHubUrl, {
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets,
    })
    .withHubProtocol(new MessagePackHubProtocol())
    .configureLogging(environment.signalRLogLevel)
    .build();

  gameUpdate$ = new Subject<GameUpdate>();
  gameStatus$ = new BehaviorSubject<GameScreenStatus>(GameScreenStatus.WelcomeScreen);

  constructor() {
    this.hubConnection.start();
    this.registerServerCalls();
  }

  public connect(): void {
    this.hubConnection.start().then(() => this.gameStatus$.next(GameScreenStatus.WelcomeScreen));
  }

  public startGame(username: string): void {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    this.hubConnection.send(ServerCalls.JoinGame, username);
    this.gameStatus$.next(GameScreenStatus.Playing);
  }

  public changeDirection(direction: number): void {
    if (this.hubConnection?.state !== HubConnectionState.Connected) {
      return;
    }

    this.hubConnection.send(ServerCalls.ChangeDirection, direction);
  }

  private registerServerCalls(): void {
    this.hubConnection.on(ClientCalls.GameUpdate, (data) => {
      const processedData = this.convertKeysToCamelCase(JSON.parse(data));
      this.gameUpdate$.next(processedData as GameUpdate);
    });

    this.hubConnection.on(ClientCalls.GameOver, () => this.gameStatus$.next(GameScreenStatus.Dead));
    this.hubConnection.onclose(() => this.gameStatus$.next(GameScreenStatus.Disconnected));
  }

  private convertKeysToCamelCase(o: any): any {
    if (o === null || o === undefined) {
      return o;
    } else if (Array.isArray(o)) {
      return o.map(this.convertKeysToCamelCase);
    }
    return typeof o !== 'object'
      ? o
      : Object.keys(o).reduce((prev: any, current: any): any => {
          const newKey = `${current[0].toLowerCase()}${current.slice(1)}`;
          if (typeof o[current] === 'object') {
            prev[newKey] = this.convertKeysToCamelCase(o[current]);
          } else {
            prev[newKey] = o[current];
          }
          return prev;
        }, {});
  }
}
