import { Injectable } from '@angular/core';
import { fromEvent, filter, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class KeyboardShortcutService {
  private shortcuts = new Map<string, () => void>();

  register(keys: { key: string; ctrl?: boolean; shift?: boolean }, callback: () => void): void {
    const keyStr = `${keys.ctrl ? 'ctrl+' : ''}${keys.shift ? 'shift+' : ''}${keys.key}`;
    this.shortcuts.set(keyStr, callback);
  }

  init(): void {
    fromEvent<KeyboardEvent>(document, 'keydown')
      .pipe(
        filter(e => {
          const target = e.target as HTMLElement;
          return target.tagName !== 'INPUT' && target.tagName !== 'TEXTAREA';
        }),
        map(e => ({
          key: `${e.ctrlKey || e.metaKey ? 'ctrl+' : ''}${e.shiftKey ? 'shift+' : ''}${e.key.toLowerCase()}`,
          event: e
        }))
      )
      .subscribe(({ key, event }) => {
        const callback = this.shortcuts.get(key);
        if (callback) {
          event.preventDefault();
          callback();
        }
      });
  }
}