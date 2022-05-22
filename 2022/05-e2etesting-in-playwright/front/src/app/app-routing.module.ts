import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShadowDomComponent } from './components/shadow-dom/shadow-dom.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserComponent } from './components/user/user.component';

const routes: Routes = [
  { path: 'users', component: UserListComponent },
  { path: 'user', component: UserComponent },
  { path: 'user/:id', component: UserComponent },
  { path: 'shadow', component: ShadowDomComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
