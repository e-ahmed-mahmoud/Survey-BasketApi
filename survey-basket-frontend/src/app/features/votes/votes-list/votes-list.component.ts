import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PollsService } from '../../../core/services/polls.service';
import { Poll } from '../../../core/models';

@Component({
  selector: 'app-votes-list',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatChipsModule,
  ],
  template: `
    <div class="fade-in">
      <div class="page-header">
        <div class="header-left">
          <h1 class="page-title">Vote</h1>
          <p class="page-subtitle">Choose a published poll to participate in</p>
        </div>
      </div>

      @if (loading()) {
        <div style="display:flex;justify-content:center;padding:80px"><mat-spinner diameter="48" /></div>
      } @else {
        <div class="grid-3">
          @for (poll of polls(); track poll.id) {
            <div class="sb-card vote-card">
              <mat-icon class="vote-card-icon">poll</mat-icon>
              <h3>{{ poll.title }}</h3>
              <p>{{ poll.summary }}</p>
              <div class="vote-card-meta">
                <span>Ends {{ poll.endsAt | date:'mediumDate' }}</span>
              </div>
              <a mat-raised-button color="primary" [routerLink]="[poll.id, 'vote']"
                [id]="'vote-btn-' + poll.id">
                <mat-icon>how_to_vote</mat-icon> Vote Now
              </a>
            </div>
          } @empty {
            <div class="empty-state">
              <mat-icon>inbox</mat-icon>
              <p>No active polls available for voting</p>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .vote-card {
      display: flex; flex-direction: column; gap: 12px;
      .vote-card-icon { font-size: 40px; width: 40px; height: 40px; color: var(--primary); }
      h3 { margin: 0; font-size: 1.1rem; }
      p { margin: 0; color: var(--text-secondary); font-size: 0.9rem; flex: 1; }
      .vote-card-meta { font-size: 0.8rem; color: var(--text-muted); }
    }
    .empty-state {
      grid-column: 1 / -1; text-align: center; padding: 80px;
      color: var(--text-muted);
      mat-icon { font-size: 64px; width: 64px; height: 64px; display: block; margin: 0 auto 16px; }
    }
  `],
})
export class VotesListComponent implements OnInit {
  private readonly pollsService = inject(PollsService);
  polls = signal<Poll[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.pollsService.getCurrentPolls().subscribe({
      next: (polls) => { this.polls.set(polls.filter(p => p.isPublished)); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
