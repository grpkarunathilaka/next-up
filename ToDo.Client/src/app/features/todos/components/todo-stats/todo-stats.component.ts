import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TodoService } from '../../../../core/services/todo.service';

@Component({
  selector: 'app-todo-stats',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="stats-container">
      <div class="stat-card">
        <div class="stat-value">{{ todoService.stats().total }}</div>
        <div class="stat-label">Total</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ todoService.stats().pending }}</div>
        <div class="stat-label">Active</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ todoService.stats().completed }}</div>
        <div class="stat-label">Completed</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ todoService.stats().completionRate }}%</div>
        <div class="stat-label">Progress</div>
      </div>
    </div>
  `,
  styles: [`
    .stats-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 16px;
      margin-bottom: 24px;
    }

    .stat-card {
      background: white;
      padding: 20px;
      border-radius: 12px;
      text-align: center;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      transition: transform 0.2s;
    }

    .stat-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }

    .stat-value {
      font-size: 32px;
      font-weight: 700;
      color: #667eea;
      margin-bottom: 8px;
    }

    .stat-label {
      font-size: 14px;
      color: #666;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
  `]
})
export class TodoStatsComponent {
  readonly todoService = inject(TodoService);
}