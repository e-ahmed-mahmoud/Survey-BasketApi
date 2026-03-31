import { Routes } from '@angular/router';
import { authGuard, roleGuard } from '../../core/guards/auth.guard';

export const VOTES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./votes-list/votes-list.component').then((m) => m.VotesListComponent),
    canActivate: [roleGuard('Member')], canActivateChild: [roleGuard('Member')],
  },
  {
    path: ':pollId/vote',
    loadComponent: () =>
      import('./submit-vote/submit-vote.component').then((m) => m.SubmitVoteComponent),
  },
];
