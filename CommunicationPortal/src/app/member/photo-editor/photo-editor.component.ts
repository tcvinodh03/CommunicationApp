import { Component, OnInit, Input } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() memberObject: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  userObject: User | undefined;

  constructor(private accountServicer: AccountService, private memberService: MembersService) {
    this.accountServicer.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.userObject = user
      }
    })
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photId: number) {
    this.memberService.deletePhoto(photId).subscribe({
      next: _ => {
        if (this.memberObject) {
          this.memberObject.photos = this.memberObject.photos.filter(p => p.id != photId);

        }
      }
    })
  }

  setMainPhoto(photo: Photo) {
    debugger;
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.userObject && this.memberObject) {
          this.userObject.photoUrl = photo.url;
          this.accountServicer.setCurrentUser(this.userObject);
          this.memberObject.photoUrl = photo.url;
          this.memberObject.photos.forEach(p => {
            p.isMain = (p.id === photo.id);
          })
        }
      }
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'user/add-photo',
      authToken: 'Bearer ' + this.userObject?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }
    this.uploader.onSuccessItem = (item, response, status, header) => {
      if (response) {
        const photo = JSON.parse(response);
        this.memberObject?.photos.push(photo);
      }
    }
  }


}
