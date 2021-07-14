import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import 'hammerjs';

import { FuseModule } from '@fuse/fuse.module';
import { FuseSharedModule } from '@fuse/shared.module';
import { FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule } from '@fuse/components';

import { fuseConfig } from 'app/fuse-config';

import { AppComponent } from 'app/app.component';
import { LayoutModule } from 'app/layout/layout.module';
import { SampleModule } from 'app/main/sample/sample.module';
import { DashboardComponentComponent } from './main/pages/dashboard/dashboard-component/dashboard-component.component';
import { AppRoutingModule } from './app-routing.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { LoginComponent } from './main/pages/authentication/login-component/login.component';
import { ForgotPasswordComponent } from './main/pages/authentication/forgot-password/forgot-password.component';
import { RegisterComponent } from './main/pages/authentication/register/register.component';
import { FormsModule } from '@angular/forms';
import { ParentComponent } from './main/pages/parent-component/parent/parent/parent.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { FakeDbService } from 'app/fake-db/fake-db.service';
import { InMemoryWebApiModule } from 'angular-in-memory-web-api';
import { ChildComponent } from './main/pages/child-component/child-component.component';
import { MatCardModule } from '@angular/material/card';
//import { ChatModule } from './main/app/chat/chat.module';

//Firestore needs
import { AngularFireModule } from '@angular/fire';
import { AngularFirestoreModule } from '@angular/fire/firestore';
import { environment } from '../environments/environment';
import { ContactsListComponent } from './main/pages/child-component/contacts-list/contacts-list.component';
import { MessagesViewComponent } from './main/pages/child-component/messages-view/messages-view.component';
import { AddChildDialogComponent } from './main/pages/parent-component/add-child-dialog/add-child-dialog/add-child-dialog.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule } from '@angular/material/dialog';
import { MessagesMonitorComponent } from './main/pages/parent-component/messages-monitor/messages-monitor.component';
import { HotToastModule } from '@ngneat/hot-toast';
import { ChildListComponent } from './main/pages/parent-component/child-list/child-list.component';
import { ChildContactComponent } from './main/pages/parent-component/child-contact/child-contact.component';
import { AddFriendDialogComponent } from './main/pages/parent-component/add-friend-dialog/add-friend-dialog.component';

const appRoutes: Routes = [
    {
        path      : '**',
        redirectTo: 'login'
    },
];

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        RegisterComponent,
        ForgotPasswordComponent,
        DashboardComponentComponent,
        ParentComponent,
        ChildComponent,
        ContactsListComponent,
        MessagesViewComponent,
        AddChildDialogComponent,
        ChildListComponent,
        ChildContactComponent,
        MessagesMonitorComponent,
        AddFriendDialogComponent,
    ],
    imports     : [
        BrowserModule,
        FormsModule,
        HttpClientModule,
        MatCardModule,
        BrowserAnimationsModule,
        HotToastModule.forRoot(),    
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatToolbarModule,
        //ChatModule,
        MatCheckboxModule,
        HttpClientModule,
        InMemoryWebApiModule.forRoot(FakeDbService, {
            delay             : 0,
            passThruUnknownUrl: true
        }),
        RouterModule.forRoot(appRoutes, { relativeLinkResolution: 'legacy' }),

        TranslateModule.forRoot(),

        // Material moment date module
        MatMomentDateModule,

        // Material
        MatButtonModule,
        MatIconModule,
        MatTabsModule,
        MatPaginatorModule,
        MatTableModule,
        MatSelectModule,
        MatDialogModule,

        // Fuse modules
        FuseModule.forRoot(fuseConfig),
        FuseProgressBarModule,
        FuseSharedModule,
        FuseSidebarModule,
        FuseThemeOptionsModule,

        // App modules
        LayoutModule,
        SampleModule,
        AppRoutingModule,

        // for firestore
        AngularFireModule.initializeApp(environment.firebase),
        AngularFirestoreModule, 
    ],
    exports: [
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatCheckboxModule,
        MatToolbarModule,
      ],
    bootstrap   : [
        AppComponent
    ]
})
export class AppModule
{
}
