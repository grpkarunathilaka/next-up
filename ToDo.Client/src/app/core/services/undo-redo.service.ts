import { Injectable, signal } from '@angular/core';

interface HistoryState<T> {
  state: T;
  timestamp: number;
}

@Injectable({
  providedIn: 'root'
})
export class UndoRedoService<T> {
  private history: HistoryState<T>[] = [];
  private currentIndex = -1;
  private maxHistory = 50;

  public canUndo = signal(false);
  public canRedo = signal(false);

  saveState(state: T): void {
    this.history = this.history.slice(0, this.currentIndex + 1);
    
    this.history.push({
      state: JSON.parse(JSON.stringify(state)),
      timestamp: Date.now()
    });

    if (this.history.length > this.maxHistory) {
      this.history.shift();
    } else {
      this.currentIndex++;
    }

    this.updateCanFlags();
  }

  undo(): T | null {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.updateCanFlags();
      return this.history[this.currentIndex].state;
    }
    return null;
  }

  redo(): T | null {
    if (this.currentIndex < this.history.length - 1) {
      this.currentIndex++;
      this.updateCanFlags();
      return this.history[this.currentIndex].state;
    }
    return null;
  }

  private updateCanFlags(): void {
    this.canUndo.set(this.currentIndex > 0);
    this.canRedo.set(this.currentIndex < this.history.length - 1);
  }
}