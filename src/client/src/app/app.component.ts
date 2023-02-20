import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { GameScreenStatus } from 'src/app/enums/GameScreenStatus';
import { AssetsService } from 'src/app/services/AssetsService';
import { GameHubService } from 'src/app/services/GameHubService';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  gameScreen$ = this.gameHubService.gameStatus$;
  gameScreenEnum = GameScreenStatus;
  username = '';

  constructor(private readonly loadAssetsService: AssetsService, private readonly gameHubService: GameHubService) {}

  ngOnInit(): void {
    this.loadAssetsService.downloadAllAssets().pipe(take(1)).subscribe();
    this.gameHubService.connect();
  }

  startGame(): void {
    this.gameHubService.startGame(this.username);
  }
}
