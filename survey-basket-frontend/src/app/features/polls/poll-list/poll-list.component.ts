import { Component, inject, signal, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { PollsService } from '../../../core/services/polls.service';
import { Poll } from '../../../core/models';
import { PollFormDialogComponent } from '../poll-form-dialog/poll-form-dialog.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-poll-list',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatTableModule, MatPaginatorModule, MatSortModule,
    MatButtonModule, MatIconModule, MatDialogModule,
    MatTooltipModule, MatChipsModule, MatMenuModule,
    MatProgressBarModule, MatFormFieldModule, MatInputModule, MatDividerModule,
  ],
  templateUrl: './poll-list.component.html',
  styleUrls: ['./poll-list.component.scss'],
})
export class PollListComponent implements OnInit {
  private readonly pollsService = inject(PollsService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly authService = inject(AuthService);
  isAdminOnly = signal<boolean>(this.authService.hasRole('Admin'));

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  loading = signal(true);
  dataSource = new MatTableDataSource<Poll>([]);
  displayedColumns = ['title', 'summary', 'startsAt', 'endsAt', 'isPublished', 'actions'];

  ngOnInit(): void {
    this.loadPolls();
  }

  loadPolls(): void {
    this.loading.set(true);
    this.pollsService.getPolls().subscribe({
      next: (polls) => {
        this.dataSource.data = polls;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(PollFormDialogComponent, {
      width: '600px',
      data: { poll: null },
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) this.loadPolls();
    });
  }

  openEditDialog(poll: Poll): void {
    const ref = this.dialog.open(PollFormDialogComponent, {
      width: '600px',
      data: { poll },
    });
    ref.afterClosed().subscribe((saved) => {
      if (saved) this.loadPolls();
    });
  }

  togglePublish(poll: Poll): void {
    this.pollsService.togglePublished(poll.id).subscribe({
      next: () => {
        this.snackBar.open(
          `Poll ${poll.isPublished ? 'unpublished' : 'published'} successfully`,
          'OK',
          { panelClass: ['snack-success'] }
        );
        this.loadPolls();
      },
    });
  }

  deletePoll(poll: Poll): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Poll',
        message: `Are you sure you want to delete "${poll.title}"? This action cannot be undone.`,
        confirmText: 'Delete',
        confirmColor: 'warn',
      },
    });
    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.pollsService.deletePoll(poll.id).subscribe({
          next: () => {
            this.snackBar.open('Poll deleted', 'OK', { panelClass: ['snack-success'] });
            this.loadPolls();
          },
        });
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
