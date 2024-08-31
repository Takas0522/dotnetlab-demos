import { Routes } from '@angular/router';
import { UsersComponent } from './components/users/users.component';
import { UserComponent } from './components/user/user.component';

export const routes: Routes = [
    { path: '', component: UsersComponent, title: 'ユーザーリスト' },
    { path: 'user/:id', component: UserComponent, title: 'ユーザー詳細' },
    { path: 'user', component: UserComponent, title: 'ユーザー詳細' }
];
