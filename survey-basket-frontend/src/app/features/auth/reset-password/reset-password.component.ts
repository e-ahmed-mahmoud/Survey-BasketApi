import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule,
  ],
  template: `
    <div class="auth-page">
      <div class="auth-branding">
        <div class="branding-content">
          <div class="brand-icon"><mat-icon>key</mat-icon></div>
          <h1 class="brand-name">New Password</h1>
          <p class="brand-tagline">Enter the code from your email and choose a new password.</p>
        </div>
        <div class="branding-bg"></div>
      </div>
      <div class="auth-form-panel">
        <div class="auth-form-card">
          <div class="form-header">
            <h2>Reset your password</h2>
            <p>Enter the reset code sent to your email</p>
          </div>
          <form [formGroup]="form" (ngSubmit)="onSubmit()" novalidate>
            <mat-form-field appearance="outline">
              <mat-label>Email address</mat-label>
              <mat-icon matPrefix>email</mat-icon>
              <input matInput type="email" formControlName="email" id="reset-email" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Reset code</mat-label>
              <mat-icon matPrefix>pin</mat-icon>
              <input matInput formControlName="resetCode" id="reset-code" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>New password</mat-label>
              <mat-icon matPrefix>lock</mat-icon>
              <input matInput type="password" formControlName="newPassword" id="reset-new-password" />
              @if (form.get('newPassword')?.invalid && form.get('newPassword')?.touched) {
                <mat-error>Password must be at least 6 characters</mat-error>
              }
            </mat-form-field>
            <button mat-raised-button color="primary" type="submit" class="submit-btn" [disabled]="loading()">
              @if (loading()) { <mat-spinner diameter="20" /> } @else { Reset Password }
            </button>
          </form>
          <p class="auth-switch"><a routerLink="/auth/login">← Back to Sign In</a></p>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../login/login.component.scss'],
})
export class ResetPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  loading = signal(false);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    resetCode: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
  });

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.authService.resetPassword(this.form.getRawValue()).subscribe({
      next: () => {
        this.snackBar.open('Password reset successfully! Please login.', 'OK', { panelClass: ['snack-success'] });
        this.router.navigate(['/auth/login']);
      },
      error: () => this.loading.set(false),
    });
  }
}
