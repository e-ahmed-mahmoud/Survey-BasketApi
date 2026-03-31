import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { TokenService } from '../../core/services/token.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  adminOnly?: boolean;
  memberOnly?: boolean;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatMenuModule,
    MatDividerModule,
  ],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss'],
})
export class ShellComponent {
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);

  sidenavOpen = signal(true);
  isAdmin = this.tokenService.hasRole('Admin');
  isMember = this.tokenService.hasRole('Member');

  readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Polls', icon: 'poll', route: '/polls', adminOnly: true },
    { label: 'Vote', icon: 'how_to_vote', route: '/votes', memberOnly: true },
    { label: 'Users', icon: 'group', route: '/users', adminOnly: true },
    { label: 'Roles', icon: 'admin_panel_settings', route: '/roles', adminOnly: true },
  ];

  toggleSidenav(): void {
    this.sidenavOpen.update((v) => !v);
  }

  logout(): void {
    this.authService.logout();
  }

  get visibleNavItems(): NavItem[] {
    console.log(this.navItems);
    if (this.isAdmin) {
      return this.navItems.filter((item) => (!item.adminOnly || this.isAdmin) && !item.memberOnly);
    }
    if (this.isMember) {
      return this.navItems.filter((item) => (item.memberOnly && !item.adminOnly));
    }
    return this.navItems;
  }
}
