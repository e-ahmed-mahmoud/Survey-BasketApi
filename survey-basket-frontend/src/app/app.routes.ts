import { Routes } from '@angular/router';
import { authGuard, noAuthGuard, roleGuard } from './core/guards/auth.guard';
import { ShellComponent } from './layout/shell/shell.component';

export const routes: Routes = [
  {
    path: 'auth',
    canActivate: [noAuthGuard],
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
      },
      {
        path: 'polls',
        loadChildren: () =>
          import('./features/polls/polls.routes').then((m) => m.POLLS_ROUTES),
      },
      {
        path: 'votes',
        loadChildren: () =>
          import('./features/votes/votes.routes').then((m) => m.VOTES_ROUTES),
      },
      {
        path: 'users',
        canActivate: [roleGuard('Admin')],
        loadChildren: () =>
          import('./features/users/users.routes').then((m) => m.USERS_ROUTES),
      },
      {
        path: 'roles',
        canActivate: [roleGuard('Admin')],
        loadChildren: () =>
          import('./features/users/roles.routes').then((m) => m.ROLES_ROUTES),
      },
      {
        path: 'account',
        loadChildren: () =>
          import('./features/account/account.routes').then((m) => m.ACCOUNT_ROUTES),
      },
    ],
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./shared/components/unauthorized/unauthorized.component').then(
        (m) => m.UnauthorizedComponent
      ),
  },
  { path: '**', redirectTo: '' },
];
