import { Routes } from '@angular/router';

export const POLLS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./poll-list/poll-list.component').then((m) => m.PollListComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./poll-detail/poll-detail.component').then((m) => m.PollDetailComponent),
  },
];
