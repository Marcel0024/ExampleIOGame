import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { filter, map, merge, throttleTime } from 'rxjs';
import { fromEvent } from 'rxjs/internal/observable/fromEvent';
import { GameScreenStatus } from 'src/app/enums/GameScreenStatus';
import { AssetsService } from 'src/app/services/AssetsService';
import { CanvasRendererService } from 'src/app/services/CanvasRendererService';
import { GameHubService } from 'src/app/services/GameHubService';
import { StateService } from 'src/app/services/StateService';

@Component({
  selector: 'app-canvas',
  templateUrl: './app-canvas.component.html',
  styleUrls: ['./app-canvas.component.scss'],
})
export class CanvasComponent implements AfterViewInit {
  @ViewChild('gameCanvas', { static: false }) canvas!: ElementRef<HTMLCanvasElement>;
  canvasService: CanvasRendererService | null = null;

  renderScreen$ = this.gameHubService.gameStatus$.subscribe((status) => {
    if (status === GameScreenStatus.Playing) {
      requestAnimationFrame(() => this.renderGame());
    } else {
      requestAnimationFrame(() => this.renderMainMenuBackground());
    }
  });

  updadteCanvasDimensionOnResize$ = fromEvent(window, 'resize')
    .pipe(throttleTime(50))
    .subscribe(() => this.canvasService?.setCanvasDimensions());

  // used in UI
  updateLeaderboard$ = this.stateService.leaderboardUpdate$.pipe(
    filter(() => this.gameHubService.gameStatus$.value === GameScreenStatus.Playing)
  );

  handleInput = merge(
    fromEvent<MouseEvent>(window, 'mousestart'),
    fromEvent<MouseEvent>(window, 'mousemove'),
    fromEvent<TouchEvent>(window, 'touchmove'),
    fromEvent<TouchEvent>(window, 'touchstart')
  )
    .pipe(
      filter(() => this.gameHubService.gameStatus$.value === GameScreenStatus.Playing),
      throttleTime(50),
      map((event) => {
        if (event instanceof TouchEvent) {
          const touchEvent = event as TouchEvent;

          return {
            x: touchEvent.touches[0].clientX,
            y: touchEvent.touches[0].clientY,
          };
        } else if (event instanceof MouseEvent) {
          const mouseEvent = event as MouseEvent;

          return {
            x: mouseEvent.clientX,
            y: mouseEvent.clientY,
          };
        }

        return { x: 0, y: 0 };
      })
    )
    .subscribe((coordinates) => {
      const direction = Math.atan2(coordinates.x - window.innerWidth / 2, window.innerHeight / 2 - coordinates.y);
      this.gameHubService.changeDirection(direction);
    });

  constructor(
    private readonly stateService: StateService,
    private readonly gameHubService: GameHubService,
    private readonly assetsService: AssetsService
  ) {}

  ngAfterViewInit(): void {
    const canvasContext = this.canvas.nativeElement.getContext('2d');
    this.canvasService = new CanvasRendererService(this.assetsService, canvasContext!, this.canvas.nativeElement);

    this.canvasService.setCanvasDimensions();

    requestAnimationFrame(() => this.renderMainMenuBackground());
  }

  private renderGame(): void {
    this.canvasService?.renderGame(this.stateService.getCurrentState());

    // Rerun this render function on the next frame
    requestAnimationFrame(() => this.renderGame());
  }

  private renderMainMenuBackground(): void {
    this.canvasService?.renderMainMenuBackground();

    // Rerun this render function on the next frame
    requestAnimationFrame(() => this.renderMainMenuBackground());
  }
}
