import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../../../core/services/users.service';
import { UserListItem } from '../../../core/models';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatButtonModule,
    MatIconModule, MatProgressBarModule, MatChipsModule, MatDialogModule,
  ],
  templateUrl: './users-list.component.html',
})
export class UsersListComponent implements OnInit {
  private readonly usersService = inject(UsersService);
  loading = signal(true);
  dataSource = new MatTableDataSource<UserListItem>([]);
  cols = ['name', 'email', 'roles', 'status', 'actions'];

  ngOnInit(): void {
    this.usersService.getAll().subscribe({
      next: (users) => { this.dataSource.data = users; this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
