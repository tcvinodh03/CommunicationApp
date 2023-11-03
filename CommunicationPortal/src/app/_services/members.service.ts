import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  membersList: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.membersList.length > 0) return of(this.membersList)
    return this.http.get<Member[]>(this.baseUrl + 'user').pipe(
      map(members => {
        this.membersList = members;
        return members;
      })
    );
  }

  getMember(username: string) {
    const member = this.membersList.find(x => x.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'user/' + username);
  }

  updateMember(member: Member) {

    return this.http.put(this.baseUrl + 'user', member).pipe(
      map(() => {
        const index = this.membersList.indexOf(member);
        this.membersList[index] = { ...this.membersList[index], ...member }
      })
    );
  }

  setMainPhoto(photoId: number) {
    debugger;
    return this.http.put(this.baseUrl + 'user/set-main-photo/' + photoId, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'user/delete-photo/' + photoId);
  }

  // getHttpOptions() {
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;
  //   const user = JSON.parse(userString);
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token
  //     })
  //   }
  // }
}
