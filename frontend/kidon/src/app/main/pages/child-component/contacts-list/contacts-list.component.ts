import { Component, OnDestroy, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AuthService } from 'app/services/auth.service';
import { ContactsService } from './contacts.service';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})
export class ContactsListComponent implements OnInit, OnDestroy {
  @Output() userSelected = new EventEmitter<any>();

  selectedUser: any;
  constructor(public contactsService: ContactsService, public auth: AuthService) { }
  // ngOnChanges(changes: SimpleChanges): void {
  //   if(changes.c)
  // }

 async ngOnInit() {
  await this.auth.getCurrentUser();
  await this.contactsService.getContacts();
  if(this.auth.userType == 'parent')
  await this.contactsService.getChildren();

  this.selectFirstContact();
  }

  
  private selectFirstContact() {
      this.onUserSelected(this.contactsService.contacts[0] as any);
  }

  ngOnDestroy(): void {
  }

  onUserSelected(user: any) {
     this.selectedUser = user;
     this.userSelected.emit(user);
  }

}
function ngOnChanges(changes: any, SimpleChanges: any) {
  throw new Error('Function not implemented.');
}

