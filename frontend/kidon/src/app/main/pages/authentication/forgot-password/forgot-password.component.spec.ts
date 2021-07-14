import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ForgotPasswordComponent } from './forgot-password.component';
import {RouterTestingModule} from '@angular/router/testing';
import {HttpClientModule} from '@angular/common/http';
import {FuseModule} from '../../../../../@fuse/fuse.module';
import {fuseConfig} from '../../../../fuse-config';
import {ToastrModule} from 'ng6-toastr-notifications';
import {FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule} from '../../../../../@fuse/components';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {FuseSharedModule} from '../../../../../@fuse/shared.module';
import {CookieService} from 'ngx-cookie-service';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientModule,
        FuseModule.forRoot(fuseConfig),
        ToastrModule.forRoot(),
        FuseProgressBarModule,
        BrowserAnimationsModule,
        FuseSharedModule,
        FuseSidebarModule,
        FuseThemeOptionsModule,
      ],
      declarations: [ ForgotPasswordComponent ],
      providers: [
        CookieService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
