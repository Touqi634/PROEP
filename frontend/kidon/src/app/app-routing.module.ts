import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { LoginComponent } from './main/pages/authentication/login-component/login.component';
import { ForgotPasswordComponent } from './main/pages/authentication/forgot-password/forgot-password.component';
import { RegisterComponent } from './main/pages/authentication/register/register.component';
import { ParentComponent } from './main/pages/parent-component/parent/parent/parent.component';
import { ChildComponent } from './main/pages/child-component/child-component.component';
import { ChildGuardGuard } from './child-guard.guard';
import { ParentGuardGuard } from './parent-guard.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {path: 'register', component: RegisterComponent},
  {path: 'forgot-password', component: ForgotPasswordComponent},
  {path: 'parent', component: ParentComponent, canActivate:[ParentGuardGuard]},
  {path: 'child', component: ChildComponent, canActivate:[ChildGuardGuard]},
  {path: 'chat', loadChildren: () => import('./main/app/apps.module').then(m => m.AppsModule)}
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
  ],
  exports: [
    RouterModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppRoutingModule { }
