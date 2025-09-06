import { Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(c => c.Login)
  },
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout.component').then(c => c.LayoutComponent),
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard').then(c => c.Dashboard)
      },
      {
        path: 'todos',
        loadComponent: () => import('./pages/todo-list/todo-list.component').then(c => c.TodoListComponent)
      },
      {
        path: 'todos/:id',
        loadComponent: () => import('./pages/todo-detail/todo-detail.component').then(c => c.TodoDetailComponent)
      },
      {
        path: 'shared',
        loadComponent: () => import('./pages/shared-todos/shared-todos.component').then(c => c.SharedTodosComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/login'
  }
];
