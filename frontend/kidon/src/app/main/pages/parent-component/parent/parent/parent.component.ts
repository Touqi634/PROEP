import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { AuthService } from 'app/services/auth.service';
import { environment } from 'environments/environment';
import { AddChildDialogComponent } from '../../add-child-dialog/add-child-dialog/add-child-dialog.component';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { HotToastService } from '@ngneat/hot-toast';
import { HttpClient } from '@angular/common/http';
import { ContactsService } from 'app/main/pages/child-component/contacts-list/contacts.service';

@Component({
  selector: 'app-parent',
  templateUrl: './parent.component.html',
  styleUrls: ['./parent.component.scss']
})

export class ParentComponent implements OnInit {
  @ViewChild('paginatorChild') paginatorChild: MatPaginator;
  @ViewChild('paginatorFriend') paginatorFriend: MatPaginator;
  @ViewChild('table') table: any;
  @ViewChild('tabGroup') tabGroup: any;

  groupForm: FormGroup;
  dataSourceChild: MatTableDataSource<any>;
  dataSourceFriend: MatTableDataSource<any>;
  displayedColumnsChild: string[] = ['ChildName', 'DOB', 'btnDelete'];
  displayedColumnsFriends: string[] = ['FriendName', 'DOB', 'btnUnblockFriend', 'btnBlockFriend', 'btnDelete'];
  childrenDetails = [];
  friendsDetails = [];

  selectedChildIdBlock: any;
  selectedChild: any;
  dialogRef: any;
  chatofChild:any;
  selectedChildFriend: any;
  selectedTab = new FormControl(0);
  selectedUser:any;
  chatlog:any = [];
  msgVal:string = '';

  constructor(private _formBuilder: FormBuilder, 
    public _matDialog: MatDialog,
    private toast: HotToastService,
    public contactsService: ContactsService,
    public auth: AuthService,
    public http: HttpClient) { }

  ngOnInit(): void {

    this.groupForm = this._formBuilder.group({
      myChildren: ['', Validators.required],
    });
    this.ReceiveMessage();
    this.ReceiveError();
    this.getChildren();
  }

  getChildren() {
    fetch(`${environment.url}/api/Family/children`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
        this.childrenDetails = data;
        this.dataSourceChild = new MatTableDataSource(this.childrenDetails);
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }

  SelectedChild(data){
    this.getFriendsOfChild(data.userId)
    this.selectedChildIdBlock = data.userId;
  }

  async getFriendsOfChild(id: any) {
    fetch(`${environment.url}/api/FriendAddressBook/${id}`, {
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
        if(element.username != null){
          this.selectedTab.setValue(3);
        }
      });

      if(this.friendsDetails.length === 0){
        this.toast.error("This child is currently not connected with friends!");
      }      
    })
    .catch((error) => {
      console.error('No data found:', error);
    });
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

  async getChat(id:any){
    fetch(`${environment.url}/api/Messages/with/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.chatlog = Object.values(data);
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }

  async getChatofChild(idChild:any, idFriend:any){
    fetch(`${environment.url}/api/Family/${idChild}/chats/${idFriend}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'bearer ' + this.auth.returnToken()
      },
    })  
    .then((response) => response.json())
    .then((data) => {
      this.chatofChild = Object.values(data);
    })
    .catch((error) => {
      console.error('Error:', error);
    });
  }

  async onChildSelected(usr: any) {
    if(usr != this.selectedChild)
        this.chatofChild = [];
    this.selectedChild = usr;
   // get contacts list of child
  }

  async onUserSelected(usr: any) {
    this.selectedUser = usr;
    //await this.getAllMessages();
    try{
      await this.getChat(this.selectedUser.userId);
    }
    catch{
      console.log("Loading...");
    }
  }

  async onChildFriendSelected(usr: any) {
    this.selectedChildFriend = usr;
    await this.getChatofChild(this.selectedChild.userId,this.selectedChildFriend.userId);
  }

  async onSendMessage(message: string) {
    this.chatSend(message);
  }

  async chatSend(theirMessage: string) {
    this.chatlog.push({content: theirMessage, name: this.auth.currentUser.displayName, senderID: this.auth.currentUser.UserId});
      this.msgVal = '';
    await this.sendmsg( this.selectedUser.userId,theirMessage);
    //this.scrollbottom();
   }

  async sendmsg(targetUserId:string ,msg:string)
  {
    console.log("Target: " + targetUserId + " Message: " + msg);
    try {
      await this.auth.connection.invoke("SendMessageToUser",targetUserId ,msg);
    } catch (err) {
        console.error(err);
    }
  }

  onTabChanged(event: MatTabChangeEvent){
     if(event.index == 0){
      this.chatlog = [].concat(this.chatlog);
     }
     if(event.index == 1){
       this.chatofChild = [].concat(this.chatofChild);
     }
  }
  
  async scrollbottom(){
    const chat = document.querySelector('#chatlog');
    chat.scrollTop = chat.scrollHeight;
  }

 async AddChild(): Promise<void> {
    this.dialogRef = this._matDialog.open(AddChildDialogComponent, {
      panelClass: 'AddChild-form',
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
          //console.log(formData.value);
          if(this.auth.signupForChild(formData.value.childEmail, formData.value.childPassword, formData.value.childName)){
            console.log("created child firebase");         
            this.toast.loading('Adding',{
              duration: 2500,
            })
            setTimeout( async () => { 
              this.toast.success("Added " + formData.value.childName + " succesfully!");
              this.getChildren() 
              await this.contactsService.getContacts();
              await this.contactsService.getChildren(); 
            }, 2500 );
          }
          break;
        case 'delete':
          break;
      }
    });   
  }

 async BtnRemoveChild(childData){
    const body = {
      UserId: childData.userId,
    };
    this.RemoveChild(body);
    this.toast.loading('Removing',{
      duration: 2500,
    })
    setTimeout( async () => { 
      this.getChildren() 
     //await this.contactsService.getContacts();
      this.toast.success(childData.username + " is removed succesfully!");
      await this.contactsService.getContacts();
      await this.contactsService.getChildren(); 
    }, 2500 );
    //TODO: Remove in firebase
  }

  RemoveChild(body: {UserId: string; }){
    fetch(`${environment.url}/api/Family/${body.UserId}`, {
      method: 'DELETE',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'bearer ' + this.auth.userToken
      },
      body: JSON.stringify(body),
      })
      .then((response) => response.json())
      .then((data) => {
          console.log('Friend was Successfully deleted!' + Object.entries(data));
      })
      .catch((error) => {
          console.log('No content!');
      });
  }

  BtnBlockFriend(data){
    const body = {
      ChildId: this.selectedChildIdBlock,
      FriendId: data.userId,
    };
    this.BlockFriend(body);
  }

  BlockFriend(body: {ChildId:string; FriendId: string; }){
    const promise = fetch(`${environment.url}/api/Block/${body.ChildId}/${body.FriendId}`, {
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

  BtnUnblockFriend(data){
    const body = {
      ChildId: this.selectedChildIdBlock,
      FriendId: data.userId,
    };
    this.UnblockFriend(body);
  }

  UnblockFriend(body: {ChildId:string; FriendId: string; }){
    const promise = fetch(`${environment.url}/api/Block/${body.ChildId}/${body.FriendId}`, {
      method: 'DELETE',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'bearer ' + this.auth.userToken
      }
    })

    promise.then((data)=>{
      if(data.status == 204){
        this.toast.success("Friend was Successfully unblocked!");
      }
      else if(data.status == 409){     
        this.toast.error("Friend is already unblocked!");   
      }
      else{          
        this.toast.error("Friend was not successfully unblocked!"); 
      }
    });
  }
}
