import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges } from '@angular/core';
import { AuthService } from 'app/services/auth.service';
import { timer } from 'rxjs';

@Component({
  selector: 'app-messages-view',
  templateUrl: './messages-view.component.html',
  styleUrls: ['./messages-view.component.scss']
})
export class MessagesViewComponent implements OnInit, OnChanges {
  @Input() selectedUserName: string;
  @Input() chatlog: any | null;

  @Output() sendMessage = new EventEmitter<string>();

  constructor(public auth:AuthService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.chatlog){
      timer(40).subscribe(() => this.scrollbottom());
    }
  }

  onSendMessage(message: string) {
    this.sendMessage.emit(message);
    timer(40).subscribe(() => this.scrollbottom());
  }

  async scrollbottom(){
    try{
    const chat = document.querySelector('.main .messages-container');
    chat.scrollTop = chat.scrollHeight;
    }
    catch(e){}
  }

  ngOnInit(): void {
    this.auth.getCurrentUser();
  }

}
