export interface GameUpdate {
  t: number;
  me: Player;
  p: Player[];
  b: Bullet[];
  l: Leaderboard[];
}

export interface Player extends GameObject {
  dir: number;
  hp: number;
}

export interface Bullet extends GameObject {}

export interface Leaderboard {
  name: string;
  score: number;
}

export interface GameObject {
  id: string;
  x: number;
  y: number;
}
