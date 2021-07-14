import { Injectable } from '@angular/core';
import { AuthService } from 'app/services/auth.service';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContactsService {
  contacts: object[]=[];
  childContacts:any;
  children: any;

  constructor(public auth: AuthService) {}


 async getContacts() {
  this.contacts = [];
    if(this.auth.userType != "parent"){
     fetch(`${environment.url}/api/Family/parent`, {
         method: 'GET',
         headers: {
             'Content-Type': 'application/json',
             'Authorization': 'bearer ' + this.auth.returnToken()
         },
     }) .then((response) => response.json())
        .then((data) => {
            this.contacts.push(data);
        })
        .catch((error) => {
            console.log('Error: get parent before loading', );
        });
    }
     
    fetch(`${environment.url}/api/FriendAddressBook/`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })
    .then((response) => response.json())
    .then((data) => {
      this.contacts= this.contacts.concat(Object.values(data));
    })
    .catch((error) => {
      console.log('Error: get friends before loading',);
    });
  }

  async getChildContacts(id: any) {
    fetch(`${environment.url}/api/FriendAddressBook/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.childContacts = Object.values(data);
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }


  async getChildren() {
    console.log('test2');  
    fetch(`${environment.url}/api/Family/children`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      if(data.status !== 404){
      this.children = Object.values(data);
      this.contacts = this.contacts.concat(this.children);
      }
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }
}
