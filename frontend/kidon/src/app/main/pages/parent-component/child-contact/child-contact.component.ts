import { Component, OnDestroy, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AuthService } from 'app/services/auth.service';
import { ContactsService } from '../../child-component/contacts-list/contacts.service';


@Component({
  selector: 'app-child-contact',
  templateUrl: './child-contact.component.html',
  styleUrls: ['./child-contact.component.scss']
})
export class ChildContactComponent implements OnInit, OnChanges {
  @Output() userSelected = new EventEmitter<any>();
  @Input() child: any | null;

  selectedUser:any;

  constructor(public contactsService: ContactsService, public auth: AuthService) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.child){
      //get Child's friends
      try{
        this.contactsService.getChildContacts(this.child.userId);
        this.selectedUser = null;
      }
      catch{
        console.log("Loading...");
      }
     }
  }

  ngOnInit() {
    this.auth.getCurrentUser();
    //await this.contactsService.getContacts();
    // await this.contactsService.getChildren();
    //this.selectFirstContact();
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

