import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RolesService } from '../../../core/services/roles.service';
import { RoleItem } from '../../../core/models';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressBarModule],
  templateUrl: './roles-list.component.html',
})
export class RolesListComponent implements OnInit {
  private readonly rolesService = inject(RolesService);
  loading = signal(true);
  dataSource = new MatTableDataSource<RoleItem>([]);
  cols = ['name', 'isDefault', 'actions'];

  ngOnInit(): void {
    this.rolesService.getAll().subscribe({
      next: (roles) => { this.dataSource.data = roles; this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
