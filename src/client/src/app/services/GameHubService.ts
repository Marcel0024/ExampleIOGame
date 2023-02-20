import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
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
  private hubConnection: HubConnection | null = null;

  gameUpdate$ = new Subject<GameUpdate>();
  gameStatus$ = new BehaviorSubject<GameScreenStatus>(GameScreenStatus.WelcomeScreen);

  public connect(): void {
    this.hubConnection = this.buildHubConnection();
    this.hubConnection.start();

    this.registerServerCalls(this.hubConnection);
  }

  public startGame(username: string): void {
    this.hubConnection?.send(ServerCalls.JoinGame, username);
    this.gameStatus$.next(GameScreenStatus.Playing);
  }

  public changeDirection(direction: number): void {
    this.hubConnection?.send(ServerCalls.ChangeDirection, direction);
  }

  private registerServerCalls(hubConnection: HubConnection): void {
    hubConnection.on(ClientCalls.GameUpdate, (data) => {
      const processedData = this.convertKeysToCamelCase(JSON.parse(data));
      this.gameUpdate$.next(processedData as GameUpdate);
    });

    hubConnection.on(ClientCalls.GameOver, () => this.gameStatus$.next(GameScreenStatus.Dead));
    hubConnection.onclose(() => this.gameStatus$.next(GameScreenStatus.Disconnected));
  }

  private buildHubConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl(environment.gameHubUrl, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .withHubProtocol(new MessagePackHubProtocol())
      .configureLogging(environment.signalRLogLevel)
      .build();
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
