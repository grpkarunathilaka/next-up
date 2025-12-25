import { Routes } from '@angular/router';
import { TodoListComponent } from './features/todos/components/todo-list/todo-list.component';

export const routes: Routes = [
  { path: '', component: TodoListComponent },
  { path: '**', redirectTo: '' }
];