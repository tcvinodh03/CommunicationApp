import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule,TimeagoModule]
})
export class MemberDetailComponent implements OnInit {
  memberObject: Member | undefined;
  imagesObject: GalleryItem[] = [];
  constructor(private memberServicer: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    
    this.loadMember();
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
  getImages() {
    if (!this.memberObject) return;
    
    for (const photo of this.memberObject?.photos) {
      this.imagesObject.push(new ImageItem({ src: photo.url, thumb: photo.url })),
      this.imagesObject.push(new ImageItem({ src: photo.url, thumb: photo.url }))
    }
  }

}
