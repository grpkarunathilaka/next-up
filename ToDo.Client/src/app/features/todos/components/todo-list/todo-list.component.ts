// ============================================
// src/app/features/todos/components/todo-list/todo-list.component.ts
// ============================================
import { Component, OnInit, inject, signal, computed, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { TodoService } from '../../../../core/services/todo.service';
import { UndoRedoService } from '../../../../core/services/undo-redo.service';
import { KeyboardShortcutService } from '../../../../core/services/keyboard-shortcut.service';
import { TodoItemComponent } from '../todo-item/todo-item.component';
import { TodoStatsComponent } from '../todo-stats/todo-stats.component';
import { Todo } from '../../../../core/models/todo.model';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule, TodoItemComponent, TodoStatsComponent],
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent implements OnInit {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  
  readonly todoService = inject(TodoService);
  readonly undoService = inject(UndoRedoService<Todo[]>);
  readonly keyboardService = inject(KeyboardShortcutService);

  
  // Form state
  newTodoTitle = '';
  newTodoPriority: 'low' | 'medium' | 'high' = 'medium';
  searchTerm = '';
  
  // UI state
  isLoading = signal(false);
  error = signal<string | null>(null);
  darkMode = signal(false);
  shortcutsVisible = signal(false);
  
  // Placeholder for filter state, will be re-introduced later
  activeFilter = signal<string>('All');
  filters = ['All', 'Active', 'Completed', 'High Priority'];

  // Plain signal for todos list (for debugging reactivity)
  todosList = signal<Todo[]>([]);

  // Computed filtered todos based on search and active filter
  filteredTodos = computed(() => {
    const todos = this.todosList(); // Use the plain signal as source
    const filter = this.activeFilter();
    const search = this.searchTerm.toLowerCase();

    // Apply search filter
    let filtered = todos.filter(todo => 
      todo.title.toLowerCase().includes(search)
    );

    // Apply category filter
    switch (filter) {
      case 'Active':
        filtered = filtered.filter(t => !t.isCompleted);
        break;
      case 'Completed':
        filtered = filtered.filter(t => t.isCompleted);
        break;
      case 'High Priority':
        filtered = filtered.filter(t => t.priority === 'high');
        break;
    }

    return filtered;
  });

  ngOnInit(): void {
    // Subscribe to the todoService's todos$ observable to update the local signal
    this.todoService.todos$.subscribe(todos => {
      this.todosList.set(todos);
      this.undoService.saveState(todos); // Also update undo state here
    });

    // Initial load, which will trigger the subscription above
    this.loadTodos(); 
    this.setupKeyboardShortcuts();
    this.loadDarkModePreference();
  }

  loadTodos(): void {
    this.isLoading.set(true);
    this.error.set(null);
    
    // The subscription in ngOnInit will handle updating todosList
    this.todoService.loadTodos().subscribe({
      next: () => { // No need to pass todos here, subscription handles it
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load todos. Please try again.');
        this.isLoading.set(false);
        console.error('Load error:', err);
      }
    });
  }

  setupKeyboardShortcuts(): void {
    this.keyboardService.init();
    
    // Ctrl+N - Focus on new todo input
    this.keyboardService.register(
      { key: 'n', ctrl: true },
      () => {
        const input = document.querySelector<HTMLInputElement>('.todo-input');
        input?.focus();
      }
    );

    // Ctrl+Z - Undo
    this.keyboardService.register(
      { key: 'z', ctrl: true },
      () => this.undo()
    );

    // Ctrl+Y - Redo
    this.keyboardService.register(
      { key: 'y', ctrl: true },
      () => this.redo()
    );

    // Ctrl+F - Focus search
    this.keyboardService.register(
      { key: 'f', ctrl: true },
      () => this.searchInput?.nativeElement.focus()
    );

    // ? - Show shortcuts modal
    this.keyboardService.register(
      { key: '?' },
      () => this.showShortcuts()
    );
  }

  loadDarkModePreference(): void {
    const stored = localStorage.getItem('darkMode');
    if (stored) {
      this.darkMode.set(stored === 'true');
    }
  }

  toggleDarkMode(): void {
    this.darkMode.update(v => !v);
    localStorage.setItem('darkMode', String(this.darkMode()));
  }

  showShortcuts(): void {
    this.shortcutsVisible.set(true);
  }



  addTodo(): void {
    const title = this.newTodoTitle.trim();
    if (!title) return;

    this.isLoading.set(true);
    this.error.set(null);

    this.todoService.createTodo({ 
      title,
      priority: this.newTodoPriority,
      tags: []
    }).subscribe({
      next: () => {
        this.newTodoTitle = '';
        this.newTodoPriority = 'medium';
        this.isLoading.set(false);
        this.undoService.saveState(this.todoService.currentTodos);

      },
      error: (err) => {
        this.error.set('Failed to add todo.');
        this.isLoading.set(false);
        console.error('Create error:', err);
      }
    });
  }

  toggleTodo(id: string): void {
    const todo = this.todoService.currentTodos.find(t => t.id === id);
    if (!todo) return;

    this.todoService.updateTodo(id, { isCompleted: !todo.isCompleted }).subscribe({
      next: () => {
        this.undoService.saveState(this.todoService.currentTodos);
      },
      error: (err) => {
        this.error.set('Failed to update todo.');
        console.error('Toggle error:', err);
      }
    });
  }

  deleteTodo(id: string): void {
    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        this.undoService.saveState(this.todoService.currentTodos);
      },
      error: (err) => {
        this.error.set('Failed to delete todo.');
        console.error('Delete error:', err);
      }
    });
  }

  onDrop(event: CdkDragDrop<Todo[]>): void {
    const todos = [...this.todosList()];
    moveItemInArray(todos, event.previousIndex, event.currentIndex);
    this.todoService.reorderTodos(todos);
    this.undoService.saveState(todos);
  }

  undo(): void {
    const previousState = this.undoService.undo();
    if (previousState) {
      this.todoService.reorderTodos(previousState);
    }
  }

  redo(): void {
    const nextState = this.undoService.redo();
    if (nextState) {
      this.todoService.reorderTodos(nextState);
    }
  }

  getEmptyMessage(): string {
    const filter = this.activeFilter();
    if (this.searchTerm) {
      return 'No todos match your search';
    }
    if (filter === 'Completed') {
      return 'No completed todos yet';
    }
    if (filter === 'Active') {
      return 'No active todos. Great job!';
    }
    if (filter === 'High Priority') {
      return 'No high priority todos';
    }
    return 'No todos yet. Add one above!';
  }
}