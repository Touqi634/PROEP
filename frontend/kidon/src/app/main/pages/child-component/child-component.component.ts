import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from 'environments/environment';
import {AuthService} from '../../../services/auth.service';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { HotToastService } from '@ngneat/hot-toast';
import { AddFriendDialogComponent } from '../parent-component/add-friend-dialog/add-friend-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { User } from 'app/models/user';
import { ContactsService } from './contacts-list/contacts.service';

@Component({
  selector: 'app-child-component',
  templateUrl: './child-component.component.html',
  styleUrls: ['./child-component.component.scss']
})
export class ChildComponent implements OnInit {

  groupForm: FormGroup;
  msgVal:string = '';
  selectedUser:any;
  allMessages:any;
  chatlog:any = [];
  userDetails = [];
  friendsDetails = [];
  dialogRef: any;
  dataSourceFriend: MatTableDataSource<any>;
  displayedColumnsFriends: string[] = ['FriendName', 'DOB', 'btnBlockFriend', 'btnDelete'];
  currentUser: User;
  
  constructor(
    public auth: AuthService,
    private toast: HotToastService,
    public contactsService: ContactsService,
    private _formBuilder: FormBuilder,
    public _matDialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.groupForm = this._formBuilder.group({
      myChildren: ['', Validators.required],
    });
    this.getAllUsers();
    this.ReceiveMessage();
    this.ReceiveError();
    this.getFriends();
    this.currentUser = this.auth.getCurrentUser();
  }

  onTabChanged(event: MatTabChangeEvent){
    if(event.index == 0){
    this.chatlog = [].concat(this.chatlog);
    }
  }

  ReceiveMessage(){
    this.auth.connection.on("ReceiveMessage", (senderID,message) => {
      if(senderID == this.selectedUser.userId){
      this.chatlog.push({content: message, name: this.selectedUser.username});
      this.chatlog = [].concat(this.chatlog);
    }});  
  }

  ReceiveError(){
    this.auth.connection.on("ReceiveError", (message) => {
      this.chatlog.splice(-1,1);
      this.chatlog = [].concat(this.chatlog);
      this.toast.error(message,{
        dismissible: true
      });
  })}

  getAllUsers(): any{
    fetch(`${environment.url}/api/Users/`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.userDetails = Object.values(data);
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }

 async getAllMessages(){
    this.chatlog = [];
    fetch(`${environment.url}/api/Messages/`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.allMessages = Object.values(data);
      this.allMessages.forEach(element => {
        if(element.senderID == this.auth.currentUser.UserId && element.receiverId == this.selectedUser.userId){
          element.name = this.auth.currentUser.displayName;
          this.chatlog.push(element);
        }
        if(element.senderID == this.selectedUser.userId && element.receiverId == this.auth.currentUser.UserId){
          element.name = this.selectedUser.username;
          this.chatlog.push(element);
        }
      });
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }

  async getAllMessagesNew(id:any){
    this.chatlog = [];
    fetch(`${environment.url}/api/Messages/with/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'bearer ' + this.auth.returnToken()
        },
    })
        .then((response) => response.json())
        .then((data) => {
            //todo this has to be changed to use the end point api/Messages/{UserID of the person with whom the conversation is done}
            this.chatlog = Object.values(data);
            console.log(this.chatlog);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
  }

  async chatSend(theirMessage: string) {
    this.chatlog.push({content: theirMessage, name: this.auth.currentUser.displayName, senderID: this.auth.currentUser.UserId});
    this.msgVal = '';
    await this.sendmsg( this.selectedUser.userId,theirMessage);
    //this.scrollbottom();
  }

  async sendmsg(targetUserId:string ,msg:string)
  {
    try {
      await this.auth.connection.invoke( "SendMessageToUser",targetUserId ,msg);
    } catch (err) {
        console.error(err);
    }
  }

  async onUserSelected(usr: any) {
    this.selectedUser = usr;
    //await this.getAllMessages();
    try{
      await this.getAllMessagesNew(this.selectedUser.userId);      
    }
    catch{
      console.log("Loading... no user(child)");
    }
  }

  async onSendMessage(message: string) {
    this.chatSend(message);
  }

  async scrollbottom(){
    const chat = document.querySelector('#chatlog');
    chat.scrollTop = chat.scrollHeight;
  }

  async changeValue(value:any){
    this.userDetails.forEach(element =>{
      if(element.userId == value.value){
        this.selectedUser = JSON.parse(JSON.stringify(element));
      }
    })
     await this.getAllMessages();
     this.scrollbottom();
  }

  getFriends() {
    fetch(`${environment.url}/api/FriendAddressBook`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.friendsDetails = data;
      this.dataSourceFriend = new MatTableDataSource(this.friendsDetails);
      this.friendsDetails.forEach(element =>{
        //console.log(element);
      });  
    })
    .catch((error) => {
      console.error('No data found:', error);
    });
  }

  AddFriend(){
    this.dialogRef = this._matDialog.open(AddFriendDialogComponent, {
      panelClass: 'AddFriend-form',
      data      : {
        action : 'save'
      }
    });

    this.dialogRef.afterClosed().subscribe(response => {
      if ( !response ){
        return;
      }
      const actionType: string = response[0];
      const formData: FormGroup = response[1];

      switch (actionType){
        case 'save':
          const body = {
            UserEmail: formData.value.friendEmail,
          };
          this.LinkFriends(body);
          break;
        case 'delete':
          break;
      }
    });
  }

  LinkFriends(body: {UserEmail: string; }){
    const promise = fetch(`${environment.url}/api/FriendAddressBook/${body.UserEmail}`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'bearer ' + this.auth.userToken
      },
    });

    promise.then((data)=>{
      if(data.status == 201){
        this.toast.success("Friend is Successfully added!");
        this.getFriends();
        this.contactsService.getContacts();
      }
      else if(data.status == 409){     
        this.toast.error("Friend is already added!");   
      }
      else{          
        this.toast.error("Friend was not added, Email not found!"); 
      }
    });
  }


  BtnBlockFriend(data){
    const body = {
      FriendId: data.userId,
    };
    this.BlockFriend(body);
  }
  
  BlockFriend(body: {FriendId: string; }){
    const promise = fetch(`${environment.url}/api/Block/${body.FriendId}`, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'bearer ' + this.auth.userToken
      }
    });

    promise.then((data)=>{
      if(data.status == 201){
        this.toast.success("Friend was Successfully blocked!");
      }
      else if(data.status == 409){     
        this.toast.error("Friend is already blocked!");   
      }
      else{          
        this.toast.error("Friend was not successfully blocked!"); 
      }
    });
  }

  BtnDeleteFriend(data){
    const body = {
      FriendId: data.userId,
    };
    this.DeleteFriend(body);
  }

  DeleteFriend(body: {FriendId: string; }){
    const promise = fetch(`${environment.url}/api/FriendAddressBook/${body.FriendId}`, {
      method: 'DELETE',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'bearer ' + this.auth.userToken
      },
    });

    promise.then((data)=>{
      if(data.status == 204){
        this.toast.success("Friend was Successfully deleted!");
        this.contactsService.getContacts();
        this.getFriends();
      }
      else if(data.status == 409){     
        //this.toast.error("Friend is already deleted!");   
      }
      else{          
        this.toast.error("Friend was not deleted!"); 
      }
    });
  }
}
