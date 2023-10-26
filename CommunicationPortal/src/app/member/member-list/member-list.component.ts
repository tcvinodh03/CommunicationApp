import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
members:Member[]=[];
constructor(private memberServices:MembersService){}

  ngOnInit(): void {
   
    this.loadMembers();
    debugger;
  }

  loadMembers(){
    this.memberServices.getMembers().subscribe({
      
      next:mem=>this.members=mem
    })
  }
}
