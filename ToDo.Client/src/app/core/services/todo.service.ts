import { Injectable, inject, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, of, throwError } from 'rxjs';
import { Todo, CreateTodoDto, TodoStats } from '../models/todo.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/v1/todos`;
  
  private todosSubject = new BehaviorSubject<Todo[]>([]);
  public readonly todos$ = this.todosSubject.asObservable();
  
  // Helper method to get current todos value
  get currentTodos(): Todo[] {
    return this.todosSubject.value;
  }
  
  public readonly stats = computed(() => {
    const todos = this.todosSubject.value;
    const completed = todos.filter(t => t.isCompleted).length;
    const total = todos.length;
    
    return {
      total,
      completed,
      pending: total - completed,
      completionRate: total > 0 ? Math.round((completed / total) * 100) : 0
    } as TodoStats;
  });

  loadTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl).pipe(
      tap(todos => this.todosSubject.next(todos)),
      catchError(err => {
        console.error('Load error:', err);
        return of([]);
      })
    );
  }

  createTodo(dto: CreateTodoDto): Observable<Todo> {
        return this.http.post<Todo>(this.apiUrl, dto).pipe(
          tap(newTodo => {
            console.log('TodoService: HTTP POST successful, response: ', newTodo);
            const current = this.todosSubject.value;
            const updatedTodos = [...current, newTodo];
            this.todosSubject.next(updatedTodos);
            console.log('TodoService: New todo added, todosSubject updated:', updatedTodos);
          }),
          catchError(err => {
            console.error('TodoService: HTTP POST failed, error:', err);
            return throwError(() => err); // Re-throw the error for the component to handle
          })
        );
  }

  updateTodo(id: string, updates: Partial<Todo>): Observable<Todo> {
    return this.http.patch<Todo>(`${this.apiUrl}/${id}`, updates).pipe(
      tap(updated => {
        const current = this.todosSubject.value;
        const index = current.findIndex(t => t.id === id);
        if (index !== -1) {
          current[index] = updated;
          this.todosSubject.next([...current]);
        }
      })
    );
  }

  deleteTodo(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const current = this.todosSubject.value;
        this.todosSubject.next(current.filter(t => t.id !== id));
      })
    );
  }

  reorderTodos(todos: Todo[]): void {
    this.todosSubject.next(todos);
  }
}