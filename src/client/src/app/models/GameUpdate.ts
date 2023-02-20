export interface GameUpdate {
  t: number;
  me: Player;
  p: Player[];
  b: Bullet[];
  l: Leaderboard[];
}

export interface Player {
  direction: number;
  hp: number;
  id: string;
  x: number;
  y: number;
}

export interface Bullet {
  id: string;
  x: number;
  y: number;
}

export interface Leaderboard {
  username: string;
  score: number;
}
