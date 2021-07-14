import { Component, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from 'app/services/auth.service';
import { ContactsService } from '../../child-component/contacts-list/contacts.service';

@Component({
  selector: 'app-child-list',
  templateUrl: './child-list.component.html',
  styleUrls: ['./child-list.component.scss']
})
export class ChildListComponent implements OnInit {

  @Output() childSelected = new EventEmitter<any>();

  selectedChild: any;
  constructor(public contactsService: ContactsService, public auth: AuthService) { }

  ngOnInit() {
    this.auth.getCurrentUser();
    // await this.contactsService.getChildren();
    try{
      this.selectFirstContact();
    }
    catch{
      console.log("Loading... No child selected");
    }
  }

  
  private selectFirstContact() {
      this.onChildSelected(this.contactsService.children[0] as any);
  }

  ngOnDestroy(): void {
  }

  onChildSelected(user: any) {
     this.selectedChild = user;
     this.childSelected.emit(user);
  }
}
