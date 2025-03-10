import { CommonModule } from '@angular/common';

import { Component, Input, OnInit, ViewChild,ChangeDetectionStrategy } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]

})

export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm | undefined;
  @Input() userName?: string;
  
  messageContent = '';

  constructor(public messageServicer: MessageService) { }

  ngOnInit(): void {
    // this.loadMessages();
  }

  sendMessage() {
    if (!this.userName) return;
    this.messageServicer.sendMessage(this.userName, this.messageContent).then(()=>{
      this.messageForm?.reset();
    })
  }

}
