import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { GameUpdate, Leaderboard } from 'src/app/models/GameUpdate';
import { GameHubService } from './GameHubService';

@Injectable({
  providedIn: 'root',
})
export class StateService {
  private renderDelay = 100;
  private gameUpdates: GameUpdate[] = [];
  private gameStart = 0;
  private firstServerTimestamp = 0;

  leaderboardUpdate$ = new Subject<Leaderboard[]>();

  constructor(private readonly gameHubService: GameHubService) {
    this.gameHubService.gameUpdate$.subscribe((gameUpdate) => {
      if (!this.firstServerTimestamp) {
        this.firstServerTimestamp = gameUpdate.t;
        this.gameStart = Date.now();
      }
      this.gameUpdates.push(gameUpdate);

      this.leaderboardUpdate$.next(gameUpdate.l);

      // Keep only one game update before the current server time
      const base = this.getBaseUpdate();
      if (base > 0) {
        this.gameUpdates.splice(0, base);
      }
    });
  }

  private currentServerTime(): number {
    return this.firstServerTimestamp + (Date.now() - this.gameStart) - this.renderDelay;
  }

  // Returns the index of the base update, the first game update before
  // current server time, or -1 if N/A.
  private getBaseUpdate(): number {
    const serverTime = this.currentServerTime();
    for (let i = this.gameUpdates.length - 1; i >= 0; i--) {
      if (this.gameUpdates[i].t <= serverTime) {
        return i;
      }
    }
    return -1;
  }

  getCurrentState(): GameUpdate | null {
    if (!this.firstServerTimestamp) {
      return null;
    }

    const base = this.getBaseUpdate();
    const serverTime = this.currentServerTime();

    // If base is the most recent update we have, use its state.
    // Otherwise, interpolate between its state and the state of (base + 1).
    if (base < 0 || base === this.gameUpdates.length - 1) {
      return this.gameUpdates[this.gameUpdates.length - 1];
    } else {
      const baseUpdate = this.gameUpdates[base];
      const next = this.gameUpdates[base + 1];
      const ratio = (serverTime - baseUpdate.t) / (next.t - baseUpdate.t);
      return {
        ...this.gameUpdates[base],
        me: this.interpolateObject(baseUpdate.me, next.me, ratio),
        p: this.interpolateObjectArray(baseUpdate.p, next.p, ratio),
        b: this.interpolateObjectArray(baseUpdate.b, next.b, ratio),
      };
    }
  }

  private interpolateObject<T extends object>(object1: any, object2: any, ratio: number): T {
    if (!object2) {
      return object1;
    }

    let interpolated: any = {};
    Object.keys(object1).forEach((key) => {
      if (key === 'direction') {
        interpolated[key] = this.interpolateDirection(object1[key], object2[key], ratio);
      } else {
        interpolated[key] = object1[key] + (object2[key] - object1[key]) * ratio;
      }
    });

    return interpolated as T;
  }

  private interpolateObjectArray<T extends object>(objects1: any[], objects2: any[], ratio: number) {
    return objects1.map((o) =>
      this.interpolateObject(
        o,
        objects2.find((o2) => o.id === o2.id),
        ratio
      )
    ) as T[];
  }

  // Determines the best way to rotate (cw or ccw) when interpolating a direction.
  // For example, when rotating from -3 radians to +3 radians, we should really rotate from
  // -3 radians to +3 - 2pi radians.
  private interpolateDirection(d1: number, d2: number, ratio: number) {
    const absD = Math.abs(d2 - d1);
    if (absD >= Math.PI) {
      // The angle between the directions is large - we should rotate the other way
      if (d1 > d2) {
        return d1 + (d2 + 2 * Math.PI - d1) * ratio;
      } else {
        return d1 - (d2 - 2 * Math.PI - d1) * ratio;
      }
    } else {
      // Normal interp
      return d1 + (d2 - d1) * ratio;
    }
  }
}
