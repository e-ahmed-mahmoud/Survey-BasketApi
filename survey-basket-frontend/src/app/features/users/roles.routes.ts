import { Routes } from '@angular/router';

export const ROLES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./roles-list/roles-list.component').then((m) => m.RolesListComponent),
  },
];
