import { Component } from '@angular/core';
import { GameScreenStatus } from 'src/app/enums/GameScreenStatus';
import { GameHubService } from 'src/app/services/GameHubService';
import { AsyncPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CanvasComponent } from './app-canvas/app-canvas.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: true,
    imports: [
        CanvasComponent,
        FormsModule,
        AsyncPipe,
    ],
})
export class AppComponent {
  gameScreen$ = this.gameHubService.gameStatus$;
  gameScreenEnum = GameScreenStatus;
  username = '';

  constructor(private readonly gameHubService: GameHubService) {}

  connect(): void {
    this.gameHubService.connect();
  }

  startGame(): void {
    this.gameHubService.startGame(this.username);
  }
}
