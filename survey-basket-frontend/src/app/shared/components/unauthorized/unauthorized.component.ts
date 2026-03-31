import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule],
  template: `
    <div class="unauthorized-page">
      <mat-icon>block</mat-icon>
      <h1>403 – Access Denied</h1>
      <p>You don't have permission to access this page.</p>
      <a routerLink="/dashboard" mat-raised-button color="primary">Go to Dashboard</a>
    </div>
  `,
  styles: [`
    .unauthorized-page {
      display: flex; flex-direction: column; align-items: center;
      justify-content: center; height: 100vh; text-align: center; gap: 16px;
      background: var(--page-bg);
      mat-icon { font-size: 80px; width: 80px; height: 80px; color: var(--error); }
      h1 { font-size: 2rem; margin: 0; }
      p { color: var(--text-secondary); }
    }
  `],
})
export class UnauthorizedComponent {}
