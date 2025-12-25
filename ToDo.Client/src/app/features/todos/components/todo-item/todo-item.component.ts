import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Todo } from '../../../../core/models/todo.model';

@Component({
  selector: 'app-todo-item',
  standalone: true,
  imports: [CommonModule],
   templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.css'] 
})

export class TodoItemComponent {
  @Input({ required: true }) todo!: Todo;
  @Output() toggle = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();

  onToggle(): void {
    this.toggle.emit(this.todo.id);
  }

  onDelete(): void {
    this.delete.emit(this.todo.id);
  }
}