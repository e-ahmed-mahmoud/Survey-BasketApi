import { Component, Inject } from '@angular/core';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

interface ConfirmData {
  title: string;
  message: string;
  confirmText?: string;
  confirmColor?: 'primary' | 'warn' | 'accent';
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title class="confirm-title">
      <mat-icon [color]="data.confirmColor ?? 'warn'">warning</mat-icon>
      {{ data.title }}
    </h2>
    <mat-dialog-content>
      <p>{{ data.message }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button [mat-dialog-close]="false" id="confirm-cancel">Cancel</button>
      <button mat-raised-button [color]="data.confirmColor ?? 'warn'"
        [mat-dialog-close]="true" id="confirm-ok">
        {{ data.confirmText ?? 'Confirm' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .confirm-title { display:flex; align-items:center; gap:10px; }
    mat-dialog-content p { font-size: 0.95rem; color: var(--text-secondary); }
  `],
})
export class ConfirmDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConfirmData,
    public dialogRef: MatDialogRef<ConfirmDialogComponent>
  ) {}
}
