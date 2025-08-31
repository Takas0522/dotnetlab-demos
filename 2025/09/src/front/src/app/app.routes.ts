import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout.component').then(c => c.LayoutComponent),
    children: [
      {
        path: '',
        redirectTo: '/todos',
        pathMatch: 'full'
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
    redirectTo: '/'
  }
];
