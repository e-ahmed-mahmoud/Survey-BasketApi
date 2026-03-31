import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
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
          <div class="brand-icon"><mat-icon>lock_reset</mat-icon></div>
          <h1 class="brand-name">Reset Password</h1>
          <p class="brand-tagline">We'll send you a link to reset your password.</p>
        </div>
        <div class="branding-bg"></div>
      </div>
      <div class="auth-form-panel">
        <div class="auth-form-card">
          <div class="form-header">
            <h2>Forgot password?</h2>
            <p>Enter your email and we'll send a reset code</p>
          </div>
          @if (sent()) {
            <div class="success-box">
              <mat-icon>check_circle</mat-icon>
              <p>Reset code sent! Check your inbox.</p>
              <a routerLink="/auth/reset-password" mat-raised-button color="primary">Enter reset code</a>
            </div>
          } @else {
            <form [formGroup]="form" (ngSubmit)="onSubmit()" novalidate>
              <mat-form-field appearance="outline">
                <mat-label>Email address</mat-label>
                <mat-icon matPrefix>email</mat-icon>
                <input matInput type="email" formControlName="email" id="forgot-email" />
                @if (form.get('email')?.invalid && form.get('email')?.touched) {
                  <mat-error>Enter a valid email address</mat-error>
                }
              </mat-form-field>
              <button mat-raised-button color="primary" type="submit" class="submit-btn" [disabled]="loading()">
                @if (loading()) { <mat-spinner diameter="20" /> } @else { Send Reset Code }
              </button>
            </form>
          }
          <p class="auth-switch"><a routerLink="/auth/login">← Back to Sign In</a></p>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../login/login.component.scss'],
})
export class ForgotPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  loading = signal(false);
  sent = signal(false);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.authService.forgotPassword(this.form.value.email!).subscribe({
      next: () => { this.loading.set(false); this.sent.set(true); },
      error: () => this.loading.set(false),
    });
  }
}
