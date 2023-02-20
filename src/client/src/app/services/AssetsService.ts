import { Injectable } from '@angular/core';
import { Observable, toArray, mergeMap, tap, merge, map, retry } from 'rxjs';
import { Assets } from 'src/app/constants/Assets';

@Injectable({
  providedIn: 'root',
})
export class AssetsService {
  private loadedAssets = new Map<string, HTMLImageElement>([]);

  downloadAllAssets(): Observable<HTMLImageElement[]> {
    return merge(Assets.map(this.downloadAsset)).pipe(
      retry(2),
      mergeMap((assetResponse) =>
        assetResponse.pipe(
          tap((asset) => this.loadedAssets.set(asset.assetName, asset.image)),
          map((asset) => asset.image)
        )
      ),
      toArray()
    );
  }

  getAsset = (assetName: string): HTMLImageElement => this.loadedAssets.get(assetName)!;

  private downloadAsset(assetName: string): Observable<{ assetName: string; image: HTMLImageElement }> {
    return new Observable<{ assetName: string; image: HTMLImageElement }>((ops) => {
      const asset = new Image();
      asset.src = `/assets/${assetName}`;
      asset.onload = () => {
        ops.next({
          assetName: assetName,
          image: asset,
        });
        ops.complete();
      };
    });
  }
}
