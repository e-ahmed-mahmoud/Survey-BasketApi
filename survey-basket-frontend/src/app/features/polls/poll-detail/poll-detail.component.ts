import { Component, inject, signal, OnInit, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PollsService } from '../../../core/services/polls.service';
import { Poll } from '../../../core/models';
import { PollFormDialogComponent } from '../poll-form-dialog/poll-form-dialog.component';
import { QuestionsListComponent } from './questions-list/questions-list.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-poll-detail',
  standalone: true,
  imports: [
    CommonModule, RouterLink, QuestionsListComponent,
    MatButtonModule, MatIconModule, MatTabsModule,
    MatChipsModule, MatProgressSpinnerModule, MatDialogModule, MatTooltipModule
  ],
  templateUrl: './poll-detail.component.html',
  styleUrls: ['./poll-detail.component.scss'],
})
export class PollDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly pollsService = inject(PollsService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly authService = inject(AuthService);
  isAdminOnly = signal<boolean>(this.authService.hasRole('Admin'));

  poll = signal<Poll | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    //this.isAdminOnly.set(this.authService.hasRole('Admin'));
    this.pollsService.getPollById(id).subscribe({
      next: (p) => { this.poll.set(p); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openEdit(): void {
    const ref = this.dialog.open(PollFormDialogComponent, {
      width: '600px',
      data: { poll: this.poll() },
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        const id = Number(this.route.snapshot.paramMap.get('id'));
        this.loading.set(true);
        this.pollsService.getPollById(id).subscribe({
          next: (p) => { this.poll.set(p); this.loading.set(false); },
        });
      }
    });
  }

  togglePublish(): void {
    const p = this.poll();
    if (!p) return;
    this.pollsService.togglePublished(p.id).subscribe({
      next: () => {
        this.snackBar.open(`Poll ${p.isPublished ? 'unpublished' : 'published'}!`, 'OK', { panelClass: ['snack-success'] });
        this.poll.update((prev) => prev ? { ...prev, isPublished: !prev.isPublished } : prev);
      },
    });
  }
}
