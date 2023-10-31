import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  memberObject: Member | undefined;
  userObject: User | null = null;


  constructor(private accountService: AccountService, private memberServicer: MembersService, private toastrService: ToastrService) {
    this.loadUser();
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadUser() {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.userObject = user
    })
  }

  loadMember() {
    if (!this.userObject) return;
    this.memberServicer.getMember(this.userObject.userName).subscribe({
      next: member => this.memberObject = member
    })
  }

  updateMembers() {
    debugger;
    this.memberServicer.updateMember(this.editForm?.value).subscribe({
      next:_=>{
        debugger;
        this.toastrService.success("this.memberObject");
        this.editForm?.reset(this.memberObject);
      }
    })
   
  }

}
