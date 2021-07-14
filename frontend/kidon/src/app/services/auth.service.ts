import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';
import * as signalR from '@microsoft/signalr'
import firebase from 'firebase/app';
import { AngularFireAuth } from '@angular/fire/auth';
import { AngularFirestore, AngularFirestoreDocument } from '@angular/fire/firestore';

import { Observable} from 'rxjs';
import { switchMap } from 'rxjs/operators';
import {IHttpConnectionOptions} from "@microsoft/signalr";
import { environment } from 'environments/environment';
import { Session } from 'inspector';
import { TokenError } from '@angular/compiler/src/ml_parser/lexer';
import { HotToastService } from '@ngneat/hot-toast';


@Injectable({
  providedIn: 'root'
  
})
export class AuthService {

  user$: Observable<User> | any;
  connection: any; 
  currentUser: User;
  userObject: string[] = [];
  userTypeDoc: any;
  userObjectData: [];
  matchObject: any;
  public userType: string;
  public secondaryApp : any;
  public userToken: any;

  constructor(
    private afAuth: AngularFireAuth,
    private toast: HotToastService,
        private afs: AngularFirestore,
        private router: Router,
  ) 
  { 
    this.SetUser();
  }

  SetUser(){
    //https://stackoverflow.com/questions/62925051/how-to-store-credentials-to-local-storage-to-allow-firebase-auth-to-sign-in
    this.afAuth.authState.subscribe((user) => {
      if (user) {
        this.SetToken();
        this.setCurrentUser(user.uid, user.displayName, user.email, this.userType);
        this.makeConnection();
        localStorage.setItem('user', JSON.stringify(user));
        this.setUserType(user.uid); //this will direct the user
      } else {
        localStorage.setItem('user', null);
      }
    });
  }

  //apply the google login for the parent
  async googleSignin() {
    const provider = new firebase.auth.GoogleAuthProvider();
    const credential = await this.afAuth.signInWithPopup(provider);
    await this.SetToken();
    let date: Date = new Date();  
    //google signIn!!!!
    this.addUserForGoogle(credential.user.uid, credential.user.displayName, date);
    await this.updateUserData(credential.user.uid, credential.user.email, credential.user.displayName, credential.user.photoURL, "Parent" );
    await this.makeConnection();
    return this.setUserType(credential.user.uid);
  }

  private setCurrentUser(UserId: string, UserName: string, Email:string, type:string){
    const data = { 
      UserId,
      UserName,
      Email,
      type
    } 
    this.afs.collection('users').doc(UserId).ref.get().then(function (doc) {
      if (doc.exists) {
        data.UserId = doc.data()['UserId'];
        data.UserName = doc.data()['displayName'];
        data.Email = doc.data()['email'];
        data.type = doc.data()['type'];

      } else {
        console.log("There is no document!");
      }
    }).catch(function (error) {
      console.log("There was an error getting your document:", error);
    });
    
    this.currentUser = data;
  }

  public getCurrentUser(): User{
    return this.currentUser;
  }

  //Sets user data to firestore on login user for google
  private updateUserData(UserId:string, email:string, displayName:string, photoURL:string,type:string) {
    const userRef: AngularFirestoreDocument<User> = this.afs.doc(`users/${UserId}`);
    const data = { 
      UserId,
      email,
      displayName,
      photoURL,
      type
    } 
    return userRef.set(data, { merge: true });

  }

  // Sets user data to firestore on login
  private updateUserDataWithParams(UserId:string, email:string, displayName:string, photoURL:string,type:string) {
    const userRef: AngularFirestoreDocument<User> = this.afs.doc(`users/${UserId}`);
    const data = { 
      UserId,
      email,
      displayName,
      photoURL,
      type
    } 
     return userRef.set(data, { merge: true });
  }

  //signIn 
  async signIn(email:string, password:string)
  {
    try{
    const credential = await this.afAuth.signInWithEmailAndPassword(email,password);
    
    await this.SetToken();
    //TODO: display name is empty here and is only called on login
    await this.setUserType(credential.user.uid);
    //await this.updateUserData(credential.user.uid, credential.user.email, credential.user.displayName, credential.user.photoURL, this.userType );

    await this.makeConnection();
    }
    catch (e){
      this.toast.error("The email and password you entered don’t match");
    }
  }

  async signup(email:string, password:string, name:string)
  {
    await this.afAuth.createUserWithEmailAndPassword(email,password)
    .then(res=>
      {
        //user uid
        const id = res.user.uid;
        //defult image
        const photo = "https://banner2.cleanpng.com/20180403/uuq/kisspng-raccoon-computer-icons-animal-avatar-woodland-5ac338422f1bb9.516471621522743362193.jpg";
        //store the user data into firestore 
        //this.router.navigate(['/login']);
        this.updateUserDataWithParams(id,email,name,photo,"parent");
        res.user.getIdToken().then(value =>
        {
          this.userToken = value;
          console.log(value);
          let date: Date = new Date();  
        try {
         this.addUser(id, name, date, value);
        }
        catch (error) {
         console.log('User is already added');
        }
        })
        //add user to the backend
        
      })
  }
 
  async signupForChild(email:string, password:string, name:string)
  {
    if(this.secondaryApp == null || this.secondaryApp == undefined) 
    {
      var config = {apiKey: "AIzaSyCe5Ue3tjyQB24WkSZuw7UUkdtbwyKes_Q",
      authDomain: "kidonapp-93b5a.firebaseapp.com",
      projectId: "kidonapp-93b5a",
      storageBucket: "kidonapp-93b5a.appspot.com",
      messagingSenderId: "1025392106185",
      appId: "1:1025392106185:web:ae22a0ef68c981122a8b18",
      measurementId: "G-8VJBQNY2Y3"};
      
      this.secondaryApp = firebase.initializeApp(config,"secondary");
    }
 
    await this.secondaryApp.auth().createUserWithEmailAndPassword(email, password)
    .then(res=> {
      //user uid
      const id = res.user.uid;
      //defult image
      const photo = "https://banner2.cleanpng.com/20180403/uuq/kisspng-raccoon-computer-icons-animal-avatar-woodland-5ac338422f1bb9.516471621522743362193.jpg";
      //add child to firestore
      this.updateUserDataWithParams(id,email,name,photo,"child");
      //add user to the backend
      let date: Date = new Date();  

      try {
        this.addChild(this.currentUser.UserId, id, email, name, date);
      }
      catch (error) {
        console.log('User is already added');
      }
      //secondaryApp.auth().signOut();
    });
  }

  async setUserType(id:string){
      this.afs.doc("users/"+id).get().subscribe(item=>
        {
           let type = item.get("type");
           if(type === "child"){

            this.userType = "child";
            this.router.navigate(['/child']);
          }
          else{
            this.userType = "parent";
            this.router.navigate(['/parent']);
          }
        }
        );
    }

  async signOut() {
    await this.afAuth.signOut();
    this.router.navigate(['/']);
    window.location.reload();
  }

  //see https://stackoverflow.com/questions/58556771/how-to-send-firebase-jwt-getidtoken-to-http-get-request-in-angular-8
  SetToken(): Promise<string> {
    return new Promise((resolve, reject) => {
      this.afAuth.onAuthStateChanged( user => {
        if (user) {
          user.getIdToken().then(idToken => {
            this.userToken = idToken;
            resolve(idToken);
          });
        }
      });
    })
  }

  //see https://stackoverflow.com/questions/58556771/how-to-send-firebase-jwt-getidtoken-to-http-get-request-in-angular-8
  //getting the string value => GetToken().then((idToken) => { then just use idToken which is the actual value })
  getToken(): any {
    const user = firebase.auth().currentUser;
    return user.getIdToken();
  }
    
   addUser(userId: string, name: string, birthYear:Date, tokenValue:string): any {
      const body = {
          UserId: userId,
          Username: name,
          DateOfBirth: birthYear,
      };
      //await this.SetToken();
      //POST request with body equal on data in JSON format
      this.addParentUser(body,tokenValue);
  }

  addUserForGoogle(userId: string, name: string, birthYear:Date): any {
    const body = {
        UserId: userId,
        Username: name,
        DateOfBirth: birthYear,
    };
    //await this.SetToken();
    //POST request with body equal on data in JSON format
    this.addParentUserForGoogle(body);
}

  addChild(parentId: string, userId: string, email: string, name: string, birthYear:Date): any {
      const body = {
          UserId: userId,
          Username: name,
          Email: email,
          DateOfBirth: birthYear,
          ParentId: parentId,
      };
      //POST request with body equal on data in JSON format
      this.linkChildToParent(body);
  }

  private addParentUser(body: { DateOfBirth: Date; Username: string; UserId: string },tokenValue:string) {
      console.log(this.userToken);      
      fetch(`${environment.url}/api/Users/`, {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json',
              'Authorization': 'bearer '+ tokenValue
          },
          body: JSON.stringify(body),
      })
          .then((response) => response.json())
          //Then with the data from the response in JSON...
          .then((data) => {
              console.log('Successfully added user!' + Object.entries(data));
          })
          //Then with the error genereted...
          .catch((error) => {
              console.log('Could not add the user!');
          });
  }

  private addParentUserForGoogle(body: { DateOfBirth: Date; Username: string; UserId: string }) {
    console.log(this.userToken);      
    fetch(`${environment.url}/api/Users/`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'bearer '+ this.userToken
        },
        body: JSON.stringify(body),
    })
        .then((response) => response.json())
        //Then with the data from the response in JSON...
        .then((data) => {
            console.log('Successfully added user!' + Object.entries(data));
        })
        //Then with the error genereted...
        .catch((error) => {
            console.log('Could not add the user!');
        });
}

  private linkChildToParent(body: {  DateOfBirth: Date; Username: string; UserId: string ;ParentId: string; }) {
      fetch(`${environment.url}/api/Family/`, {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json',
              'Authorization': 'bearer ' + this.userToken
          },
          body: JSON.stringify(body),
      })
          .then((response) => response.json())
          //Then with the data from the response in JSON...
          .then((data) => {
              console.log('Successfully added user!' + Object.entries(data));
          })
          //Then with the error genereted...
          .catch((error) => {
              console.log('No content!');
              //console.error('No content!', error);
          });
  }

  async makeConnection()
  {
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return this.getToken();
      }
    };

    this.connection = new signalR.HubConnectionBuilder()
              .withUrl(`${environment.url}/messagehub`,options)
              .withAutomaticReconnect()
              .configureLogging(signalR.LogLevel.Information)
              .build();
    
    this.connection.onclose(this.start());
    await this.start();
  }

  async start() {
    try {
        await this.connection.start();
        
    } catch (err) {
        console.log("Still loading... messagehub");
    }
  }

  returnToken()
  {
    return this.userToken;
  }

  returnUserType()
  {
    return this.userType;
  }

  get isLoggedIn(): boolean {
    const  user  =  JSON.parse(localStorage.getItem('user'));
    return  user  !==  null;
}

}
