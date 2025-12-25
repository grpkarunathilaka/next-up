import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoListComponent } from './todo-list.component';
import { TodoService } from '../../../../core/services/todo.service';
import { of, throwError } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';

describe('TodoListComponent', () => {
  let component: TodoListComponent;
  let fixture: ComponentFixture<TodoListComponent>;
  let todoService: jasmine.SpyObj<TodoService>;

  beforeEach(async () => {
    const todoServiceSpy = jasmine.createSpyObj('TodoService', [
      'loadTodos',
      'createTodo',
      'deleteTodo',
      'toggleComplete'
    ]);

    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [
        { provide: TodoService, useValue: todoServiceSpy },
        provideHttpClient()
      ]
    }).compileComponents();

    todoService = TestBed.inject(TodoService) as jasmine.SpyObj<TodoService>;
    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load todos on init', () => {
    todoService.loadTodos.and.returnValue(of([]));
    fixture.detectChanges();
    expect(todoService.loadTodos).toHaveBeenCalled();
  });

  it('should add todo when title is provided', () => {
    const mockTodo = { id: '1', title: 'Test', isCompleted: false, createdAt: new Date() };
    todoService.createTodo.and.returnValue(of(mockTodo));
    
    component.newTodoTitle = 'Test';
    component.addTodo();
    
    expect(todoService.createTodo).toHaveBeenCalledWith({ title: 'Test' });
  });

  it('should not add todo when title is empty', () => {
    component.newTodoTitle = '   ';
    component.addTodo();
    expect(todoService.createTodo).not.toHaveBeenCalled();
  });
});

