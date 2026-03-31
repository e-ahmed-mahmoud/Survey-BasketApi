import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserAccountService } from '../../core/services/user-account.service';
import { AuthService } from '../../core/services/auth.service';
import { AccountInfo } from '../../core/models';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatCardModule, MatProgressSpinnerModule, MatDividerModule,
  ],
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  private readonly accountService = inject(UserAccountService);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb = inject(FormBuilder);

  info = signal<AccountInfo | null>(null);
  loading = signal(true);
  saving = signal(false);
  changingPwd = signal(false);
  hideCurrentPwd = signal(true);
  hideNewPwd = signal(true);

  profileForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName:  ['', [Validators.required, Validators.minLength(2)]],
  });

  passwordForm = this.fb.nonNullable.group({
    currentPassword:    ['', Validators.required],
    newPassword:        ['', [Validators.required, Validators.minLength(6)]],
    confirmNewPassword: ['', Validators.required],
  });

  ngOnInit(): void {
    this.accountService.getAccountInfo().subscribe({
      next: (info) => {
        this.info.set(info);
        this.profileForm.patchValue({ firstName: info.firstName, lastName: info.lastName });
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) { this.profileForm.markAllAsTouched(); return; }
    this.saving.set(true);
    this.accountService.updateAccount(this.profileForm.getRawValue()).subscribe({
      next: () => {
        this.snackBar.open('Profile updated!', 'OK', { panelClass: ['snack-success'] });
        this.saving.set(false);
      },
      error: () => this.saving.set(false),
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) { this.passwordForm.markAllAsTouched(); return; }
    this.changingPwd.set(true);
    this.accountService.changePassword(this.passwordForm.getRawValue()).subscribe({
      next: () => {
        this.snackBar.open('Password changed! Please login again.', 'OK', { panelClass: ['snack-success'] });
        this.passwordForm.reset();
        this.changingPwd.set(false);
        this.authService.logout();
      },
      error: () => this.changingPwd.set(false),
    });
  }
}
