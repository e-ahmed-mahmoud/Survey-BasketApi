import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="auth-page">
      <div class="auth-branding">
        <div class="branding-content">
          <div class="brand-icon"><mat-icon>mark_email_read</mat-icon></div>
          <h1 class="brand-name">Confirm Email</h1>
        </div>
        <div class="branding-bg"></div>
      </div>
      <div class="auth-form-panel">
        <div class="auth-form-card" style="text-align:center">
          @if (loading()) {
            <mat-spinner diameter="48" style="margin: 40px auto" />
            <p>Confirming your email...</p>
          } @else if (success()) {
            <mat-icon style="font-size:64px;width:64px;height:64px;color:var(--success)">check_circle</mat-icon>
            <h2>Email Confirmed!</h2>
            <p>Your account has been verified successfully.</p>
            <a routerLink="/auth/login" mat-raised-button color="primary">Sign In</a>
          } @else {
            <mat-icon style="font-size:64px;width:64px;height:64px;color:var(--error)">cancel</mat-icon>
            <h2>Confirmation Failed</h2>
            <p>The confirmation link is invalid or has expired.</p>
            <a routerLink="/auth/login" mat-raised-button>Back to Login</a>
          }
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../login/login.component.scss'],
})
export class ConfirmEmailComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  loading = signal(true);
  success = signal(false);

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId') ?? '';
    const code   = this.route.snapshot.queryParamMap.get('code') ?? '';
    this.authService.confirmEmail({ userId, code }).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); },
      error: () => { this.loading.set(false); this.success.set(false); },
    });
  }
}
