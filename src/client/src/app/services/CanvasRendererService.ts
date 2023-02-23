import { Bullet, GameUpdate, Player } from 'src/app/models/GameUpdate';
import { AssetsService } from './AssetsService';

export class CanvasRendererService {
  mapSize = 3000; // todo get details from server
  bulletRadius = 10;
  playerRadius = 20;
  playerMaxHP = 100;

  constructor(
    private readonly assetsService: AssetsService,
    private readonly canvasContext: CanvasRenderingContext2D,
    private readonly canvasElement: HTMLCanvasElement
  ) {}

  renderGame(state: GameUpdate | null) {
    if (!state?.me) {
      return;
    }

    // Draw background
    this.renderBackground(state.me.x, state.me.y);

    // Draw boundaries
    this.canvasContext.strokeStyle = 'black';
    this.canvasContext.lineWidth = 1;
    this.canvasContext.strokeRect(
      this.canvasElement.width / 2 - state.me.x,
      this.canvasElement.height / 2 - state.me.y,
      this.mapSize,
      this.mapSize
    );

    // Draw all bullets
    state.b.forEach((b) => this.renderBullet(state.me, b));

    // Draw all players
    this.renderPlayer(state.me, state.me);
    state.p.forEach((p) => this.renderPlayer(state.me, p));
  }

  renderMainMenuBackground(): void {
    const t = Date.now() / 7500;
    const x = this.mapSize / 2 + 800 * Math.cos(t);
    const y = this.mapSize / 2 + 800 * Math.sin(t);
    this.renderBackground(x, y);
  }

  // On small screens (e.g. phones), we want to "zoom out" so players can still see at least
  // 800 in-game units of width.
  setCanvasDimensions(): void {
    const scaleRatio = Math.max(1, 800 / window.innerWidth);
    this.canvasElement.width = scaleRatio * window.innerWidth;
    this.canvasElement.height = scaleRatio * window.innerHeight;
  }

  private renderBackground(x: number, y: number): void {
    if (!this.canvasContext) {
      return;
    }

    const backgroundX = this.mapSize / 2 - x + this.canvasElement.width / 2;
    const backgroundY = this.mapSize / 2 - y + this.canvasElement.height / 2;
    const backgroundGradient = this.canvasContext.createRadialGradient(
      backgroundX,
      backgroundY,
      this.mapSize / 10,
      backgroundX,
      backgroundY,
      this.mapSize / 2
    );
    backgroundGradient.addColorStop(0, 'black');
    backgroundGradient.addColorStop(1, 'gray');
    this.canvasContext.fillStyle = backgroundGradient;
    this.canvasContext.fillRect(0, 0, this.canvasElement.width, this.canvasElement.height);
  }

  private renderBullet(me: Player, bullet: Bullet): void {
    const { x, y } = bullet;
    this.canvasContext?.drawImage(
      this.assetsService.getAsset('bullet.svg'),
      this.canvasElement.width / 2 + x - me.x - this.bulletRadius,
      this.canvasElement.height / 2 + y - me.y - this.bulletRadius,
      this.bulletRadius * 2,
      this.bulletRadius * 2
    );
  }

  private renderPlayer(me: Player, player: Player): void {
    const { x, y, dir } = player;
    const canvasX = this.canvasElement.width / 2 + x - me.x;
    const canvasY = this.canvasElement.height / 2 + y - me.y;

    if (!this.canvasContext) {
      return;
    }

    // Draw ship
    this.canvasContext.save();
    this.canvasContext.translate(canvasX, canvasY);
    this.canvasContext.rotate(dir);
    this.canvasContext.drawImage(
      this.assetsService.getAsset('ship.svg'),
      -this.playerRadius,
      -this.playerRadius,
      this.playerRadius * 2,
      this.playerRadius * 2
    );
    this.canvasContext.restore();

    // Draw health bar
    this.canvasContext.fillStyle = 'white';
    this.canvasContext.fillRect(canvasX - this.playerRadius, canvasY + this.playerRadius + 8, this.playerRadius * 2, 2);
    this.canvasContext.fillStyle = 'red';
    this.canvasContext.fillRect(
      canvasX - this.playerRadius + (this.playerRadius * 2 * player.hp) / this.playerMaxHP,
      canvasY + this.playerRadius + 8,
      this.playerRadius * 2 * (1 - player.hp / this.playerMaxHP),
      2
    );
  }
}
