import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  memberObject: Member ={} as Member;
  imagesObject: GalleryItem[] = [];
  messages: Message[] = [];
  constructor(private memberServicer: MembersService, private route: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.memberObject = data['member']
    })
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.getImages();
  }
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    }
  }

  loadMember() {
    const userName = this.route.snapshot.paramMap.get('username');
    if (!userName) return;
    this.memberServicer.getMember(userName).subscribe({
      next: member => {
        this.memberObject = member,
          this.getImages()
      }
    })
  }

  loadMessages() {
    if (this.memberObject) {
      this.messageService.getMessageThread(this.memberObject.userName).subscribe({
        next: response => this.messages = response
      })
    }
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  getImages() {
    if (!this.memberObject) return;

    for (const photo of this.memberObject?.photos) {
      this.imagesObject.push(new ImageItem({ src: photo.url, thumb: photo.url })),
        this.imagesObject.push(new ImageItem({ src: photo.url, thumb: photo.url }))
    }
  }

}
